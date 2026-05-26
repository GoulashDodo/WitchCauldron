using System;
using System.Collections.Generic;
using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.BattleEntities.Enemies.Services;
using Gameplay.Battle.BattleEntities.Enemies.SO;
using Gameplay.Battle.Waves.Enums;
using Gameplay.Battle.Waves.SO;
using Gameplay.Battle.Waves.SO.Structures;
using Gameplay.Battle.Waves.SpawnArea;
using Gameplay.Level.SO;
using R3;
using UnityEngine;
using Zenject;

namespace Gameplay.Battle.Waves.Service
{
    public class WaveService : IWaveService, ITickable, IDisposable
    {
        private readonly ISpawnArea _spawnArea;
        private readonly EnemyService _enemyService;
        private readonly WaveSettings _waveSettings;
        private readonly List<WaveRuntimeState> _runtimeStates = new();

        public ReadOnlyReactiveProperty<float> Progress01 => _progress01;
        private readonly ReactiveProperty<float> _progress01 = new(0f);

        public Observable<Unit> WavesCompleted => _wavesCompleted;
        private readonly Subject<Unit> _wavesCompleted = new();

        private float _levelStartTime;
        private float _levelDuration;
        private bool _isRunning;

        public WaveService(
            ISpawnArea spawnArea,
            EnemyService enemyService,
            LevelSettings levelSettings)
        {
            _spawnArea = spawnArea;
            _enemyService = enemyService;

            
            _waveSettings = levelSettings.WaveSettings;
        }

        public void StartWaves()
        {
            if (_isRunning)
                StopWaves();

            BuildRuntimeStates();

            if (_runtimeStates.Count == 0)
            {
                Debug.LogWarning("WaveService cannot start: wave settings have no waves with enemies.");
                _progress01.Value = 1f;
                _wavesCompleted.OnNext(Unit.Default);
                return;
            }

            _levelStartTime = Time.time;
            _progress01.Value = 0f;
            _isRunning = true;
        }

        public void StopWaves()
        {
            _isRunning = false;
            ClearRuntimeStates();
        }

        public void Tick()
        {
            if (!_isRunning)
                return;

            var elapsedTime = Time.time - _levelStartTime;
            var hasActiveWaves = false;

            for (var i = 0; i < _runtimeStates.Count; i++)
            {
                var state = _runtimeStates[i];
                state.TryMarkCleared(elapsedTime);

                if (state.IsSpawnComplete)
                    continue;

                hasActiveWaves = true;

                TryStartWave(state, GetPreviousState(i), elapsedTime);
                TrySpawnFromWave(state, elapsedTime);
                state.TryMarkCleared(elapsedTime);
            }

            _progress01.Value = _levelDuration <= 0f ? 1f : Mathf.Clamp01(elapsedTime / _levelDuration);

            if (!hasActiveWaves)
            {
                _progress01.Value = 1f;
                _isRunning = false;
                _wavesCompleted.OnNext(Unit.Default);
            }
        }

        public void Dispose()
        {
            ClearRuntimeStates();
            _progress01.Dispose();
            _wavesCompleted.Dispose();
        }

        private void BuildRuntimeStates()
        {
            ClearRuntimeStates();
            _levelDuration = 0f;

            var waves = _waveSettings != null ? _waveSettings.Waves : null;

            if (waves == null)
                return;

            foreach (var wave in waves)
            {
                if (wave == null)
                    continue;

                var state = new WaveRuntimeState(wave, _waveSettings.StartDelay, _enemyService);

                if (!state.HasSpawns)
                    continue;

                _runtimeStates.Add(state);

                _levelDuration = Mathf.Max(_levelDuration, state.EndTime);
            }
        }

        private WaveRuntimeState GetPreviousState(int index)
        {
            return index > 0 ? _runtimeStates[index - 1] : null;
        }

        private void TryStartWave(WaveRuntimeState state, WaveRuntimeState previousState, float elapsedTime)
        {
            if (state.IsStarted)
                return;

            if (!CanStartWave(state, previousState, elapsedTime))
                return;

            state.Start(elapsedTime);
            _levelDuration = Mathf.Max(_levelDuration, state.EndTime);
        }

        private bool CanStartWave(WaveRuntimeState state, WaveRuntimeState previousState, float elapsedTime)
        {
            if (state.Wave.StartMode == WaveStartMode.Timeline || previousState == null)
                return elapsedTime >= state.ScheduledStartTime;

            if (!previousState.IsCleared)
                return false;

            return elapsedTime >= previousState.ClearedTime + state.Wave.StartTime;
        }

        private void TrySpawnFromWave(WaveRuntimeState state, float elapsedTime)
        {
            if (!state.IsStarted)
                return;

            if (elapsedTime < state.NextSpawnTime)
                return;

            var spawn = state.TakeNextSpawn();

            if (spawn == null)
                return;

            var position = GetSpawnPosition(state.Wave);
            var enemy = _enemyService.SpawnEnemy(spawn.EnemyTypeId, position);
            state.TrackSpawnedEnemy(enemy);
            state.TryMarkCleared(elapsedTime);

            state.NextSpawnTime += state.Wave.SpawnInterval;
        }

        private void ClearRuntimeStates()
        {
            foreach (var state in _runtimeStates)
                state.Dispose();

            _runtimeStates.Clear();
        }

        private Vector3 GetSpawnPosition(WaveDefinition wave)
        {
            return wave.SpawnPositionMode == SpawnPositionMode.SpecificPosition
                ? wave.SpecificSpawnPosition
                : _spawnArea.GetRandomPosition();
        }

        private sealed class WaveRuntimeState : IDisposable
        {
            private readonly List<EnemySpawnRuntime> _spawns = new();
            private readonly Dictionary<int, IDisposable> _enemyDeathSubscriptions = new();

            public WaveDefinition Wave { get; }
            public float NextSpawnTime { get; set; }
            public float ScheduledStartTime { get; }
            public float EndTime { get; private set; }
            public float ClearedTime { get; private set; }
            public bool IsStarted { get; private set; }
            public bool HasSpawns => TotalRemainingCount > 0;
            public bool IsSpawnComplete => IsStarted && _spawns.Count == 0;
            public bool IsCleared { get; private set; }

            public WaveRuntimeState(WaveDefinition wave, float levelStartDelay, EnemyService enemyService)
            {
                Wave = wave;
                ScheduledStartTime = levelStartDelay + wave.StartTime;
                NextSpawnTime = ScheduledStartTime;

                if (wave.Enemies == null)
                {
                    EndTime = ScheduledStartTime;
                    return;
                }

                if (wave.SpawnMode == WaveSpawnMode.PointBudget)
                    BuildPointBudgetSpawns(wave, enemyService);
                else
                    BuildManualSpawns(wave);

                EndTime = ScheduledStartTime + Mathf.Max(0, TotalRemainingCount - 1) * wave.SpawnInterval;
            }

            public void Start(float elapsedTime)
            {
                IsStarted = true;
                NextSpawnTime = elapsedTime;
                EndTime = elapsedTime + Mathf.Max(0, TotalRemainingCount - 1) * Wave.SpawnInterval;
            }

            private void BuildManualSpawns(WaveDefinition wave)
            {
                foreach (var enemy in wave.Enemies)
                {
                    if (enemy.Count <= 0 || string.IsNullOrWhiteSpace(enemy.EnemyTypeId))
                        continue;

                    _spawns.Add(new EnemySpawnRuntime(enemy.EnemyTypeId, enemy.Count, enemy.Weight));
                }
            }

            private void BuildPointBudgetSpawns(WaveDefinition wave, EnemyService enemyService)
            {
                var candidates = BuildBudgetCandidates(wave, enemyService);
                var remainingBudget = Mathf.Max(0, wave.PointBudget);

                foreach (var candidate in candidates)
                {
                    for (var i = 0; i < candidate.MinCount; i++)
                    {
                        if (remainingBudget < candidate.PointPrice || !candidate.CanAddMore)
                        {
                            Debug.LogWarning(
                                $"Wave budget cannot satisfy MinCount for enemy '{candidate.EnemyTypeId}'.");
                            break;
                        }

                        candidate.Count++;
                        remainingBudget -= candidate.PointPrice;
                    }
                }

                while (TryTakeWeightedCandidate(candidates, remainingBudget, out var selected))
                {
                    selected.Count++;
                    remainingBudget -= selected.PointPrice;
                }

                foreach (var candidate in candidates)
                {
                    if (candidate.Count <= 0)
                        continue;

                    _spawns.Add(new EnemySpawnRuntime(candidate.EnemyTypeId, candidate.Count, candidate.Weight));
                }
            }

            private static List<BudgetCandidate> BuildBudgetCandidates(
                WaveDefinition wave,
                EnemyService enemyService)
            {
                var candidates = new List<BudgetCandidate>();

                foreach (var enemy in wave.Enemies)
                {
                    if (enemy == null || string.IsNullOrWhiteSpace(enemy.EnemyTypeId))
                        continue;

                    if (!enemyService.TryGetEnemySettings(enemy.EnemyTypeId, out var settings))
                    {
                        Debug.LogWarning($"Wave references unknown enemy type '{enemy.EnemyTypeId}'.");
                        continue;
                    }

                    candidates.Add(new BudgetCandidate(enemy, settings));
                }

                return candidates;
            }

            private static bool TryTakeWeightedCandidate(
                List<BudgetCandidate> candidates,
                int remainingBudget,
                out BudgetCandidate selected)
            {
                selected = null;
                var totalWeight = 0;

                foreach (var candidate in candidates)
                {
                    if (!candidate.CanAfford(remainingBudget))
                        continue;

                    totalWeight += candidate.Weight;
                }

                if (totalWeight <= 0)
                    return false;

                var roll = UnityEngine.Random.Range(0, totalWeight);

                foreach (var candidate in candidates)
                {
                    if (!candidate.CanAfford(remainingBudget))
                        continue;

                    roll -= candidate.Weight;

                    if (roll >= 0)
                        continue;

                    selected = candidate;
                    return true;
                }

                return false;
            }

            private int TotalRemainingCount
            {
                get
                {
                    var count = 0;

                    foreach (var spawn in _spawns)
                        count += spawn.Count;

                    return count;
                }
            }

            public EnemySpawnRuntime TakeNextSpawn()
            {
                if (_spawns.Count == 0)
                    return null;

                var totalWeight = 0;

                foreach (var spawn in _spawns)
                    totalWeight += spawn.Weight;

                var roll = UnityEngine.Random.Range(0, totalWeight);

                for (var i = 0; i < _spawns.Count; i++)
                {
                    var spawn = _spawns[i];
                    roll -= spawn.Weight;

                    if (roll >= 0)
                        continue;

                    spawn.Count--;

                    if (spawn.Count <= 0)
                        _spawns.RemoveAt(i);

                    return spawn;
                }

                return _spawns[0];
            }

            public void TrackSpawnedEnemy(Enemy enemy)
            {
                if (enemy == null)
                    return;

                var enemyId = enemy.GetInstanceID();

                _enemyDeathSubscriptions[enemyId] = enemy.Events.Died.Subscribe(_ =>
                {
                    if (_enemyDeathSubscriptions.Remove(enemyId, out var subscription))
                        subscription.Dispose();
                });
            }

            public void Dispose()
            {
                foreach (var subscription in _enemyDeathSubscriptions.Values)
                    subscription.Dispose();

                _enemyDeathSubscriptions.Clear();
            }

            public void TryMarkCleared(float elapsedTime)
            {
                if (IsCleared || !IsStarted || _spawns.Count > 0 || _enemyDeathSubscriptions.Count > 0)
                    return;

                ClearedTime = elapsedTime;
                IsCleared = true;
            }
        }

        private sealed class EnemySpawnRuntime
        {
            public string EnemyTypeId { get; }
            public int Count { get; set; }
            public int Weight { get; }

            public EnemySpawnRuntime(string enemyTypeId, int count, int weight)
            {
                EnemyTypeId = enemyTypeId;
                Count = count;
                Weight = Mathf.Max(1, weight);
            }
        }

        private sealed class BudgetCandidate
        {
            public string EnemyTypeId { get; }
            public int PointPrice { get; }
            public int Weight { get; }
            public int MinCount { get; }
            public int MaxCount { get; }
            public int Count { get; set; }
            public bool CanAddMore => MaxCount <= 0 || Count < MaxCount;

            public BudgetCandidate(EnemySpawnDefinition definition, EnemySettings settings)
            {
                EnemyTypeId = definition.EnemyTypeId;
                PointPrice = Mathf.Max(1, settings.PointPrice);
                Weight = Mathf.Max(1, definition.Weight);
                MinCount = Mathf.Max(0, definition.MinCount);
                MaxCount = Mathf.Max(0, definition.MaxCount);
            }

            public bool CanAfford(int remainingBudget)
            {
                return CanAddMore && PointPrice <= remainingBudget;
            }
        }
    }
}
