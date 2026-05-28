using System;
using System.Collections.Generic;
using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.BattleEntities.Enemies.Services;
using Gameplay.Battle.BattleEntities.Enemies.SO;
using Gameplay.Battle.Waves.Enums;
using Gameplay.Battle.Waves.SO;
using Gameplay.Battle.Waves.SO.Structures;
using Gameplay.Battle.Waves.SpawnArea;
using Gameplay.Battle.Waves.Structures;
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
        private readonly List<WaveDefinition> _waves = new();

        public ReadOnlyReactiveProperty<WaveProgress> Progress => _progress;
        private readonly ReactiveProperty<WaveProgress> _progress = new(new WaveProgress(0f, -1, 0, 0f, false));

        public Observable<WaveStartedEvent> WaveStarted => _waveStarted;
        private readonly Subject<WaveStartedEvent> _waveStarted = new();

        public Observable<Unit> WavesCompleted => _wavesCompleted;
        private readonly Subject<Unit> _wavesCompleted = new();

        private float _levelStartTime;
        private float _nextWaveStartTime;
        private int _currentWaveIndex;
        private WaveRuntimeState _currentWave;
        private bool _isRunning;
        private bool _isCompleted;

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

            _isCompleted = false;
            BuildWaves();

            if (_waves.Count == 0)
            {
                Debug.LogWarning("WaveService cannot start: wave settings have no waves with enemies.");
                CompleteWaves();
                return;
            }

            _levelStartTime = Time.time;
            _nextWaveStartTime = _waveSettings.StartDelay + Mathf.Max(0f, _waves[0].StartTime);
            _currentWaveIndex = -1;
            _currentWave = null;
            _isRunning = true;
            SetProgress(0f, -1, _waves.Count, 0f, false);
        }

        public void StopWaves()
        {
            _isRunning = false;
            ClearCurrentWave();
            _waves.Clear();
        }

        public void Tick()
        {
            if (!_isRunning)
                return;

            var elapsedTime = Time.time - _levelStartTime;

            if (_currentWave == null)
            {
                TryStartNextWave(elapsedTime);
                UpdateProgress();
                return;
            }

            TrySpawnFromCurrentWave(elapsedTime);
            _currentWave.TryMarkCleared();

            if (_currentWave.IsCleared)
                FinishCurrentWave(elapsedTime);

            UpdateProgress();
        }

        public void Dispose()
        {
            ClearCurrentWave();
            _progress.Dispose();
            _waveStarted.Dispose();
            _wavesCompleted.Dispose();
        }

        private void BuildWaves()
        {
            ClearCurrentWave();
            _waves.Clear();

            var waves = _waveSettings != null ? _waveSettings.Waves : null;

            if (waves == null)
                return;

            foreach (var wave in waves)
            {
                if (wave == null)
                    continue;

                if (!WaveSpawnPlanBuilder.HasSpawns(wave, _enemyService))
                    continue;

                _waves.Add(wave);
            }
        }

        private void TryStartNextWave(float elapsedTime)
        {
            if (elapsedTime < _nextWaveStartTime)
                return;

            if (_currentWaveIndex + 1 >= _waves.Count)
            {
                CompleteWaves();
                return;
            }

            _currentWaveIndex++;
            var wave = _waves[_currentWaveIndex];
            _currentWave = new WaveRuntimeState(wave, WaveSpawnPlanBuilder.Build(wave, _enemyService));
            _currentWave.Start(elapsedTime);

            _waveStarted.OnNext(new WaveStartedEvent(_currentWaveIndex, _waves.Count, wave.Type));
        }

        private void TrySpawnFromCurrentWave(float elapsedTime)
        {
            if (elapsedTime < _currentWave.NextSpawnTime)
                return;

            var spawn = _currentWave.TakeNextSpawn();

            if (spawn == null)
                return;

            var position = GetSpawnPosition(_currentWave.Wave);
            var enemy = _enemyService.SpawnEnemy(spawn.EnemyTypeId, position);
            _currentWave.TrackSpawnedEnemy(enemy);
            _currentWave.TryMarkCleared();

            _currentWave.NextSpawnTime += _currentWave.Wave.SpawnInterval;
        }

        private void FinishCurrentWave(float elapsedTime)
        {
            ClearCurrentWave();

            if (_currentWaveIndex + 1 >= _waves.Count)
            {
                CompleteWaves();
                return;
            }

            _nextWaveStartTime = elapsedTime + Mathf.Max(0f, _waves[_currentWaveIndex + 1].StartTime);
        }

        private void CompleteWaves()
        {
            if (_isCompleted)
                return;

            _isCompleted = true;
            _isRunning = false;
            ClearCurrentWave();
            SetProgress(1f, _waves.Count - 1, _waves.Count, 1f, false);
            _wavesCompleted.OnNext(Unit.Default);
        }

        private void ClearCurrentWave()
        {
            _currentWave?.Dispose();
            _currentWave = null;
        }

        private void UpdateProgress()
        {
            if (_waves.Count == 0)
            {
                SetProgress(1f, -1, 0, 1f, false);
                return;
            }

            var completedWaves = _currentWave == null ? _currentWaveIndex + 1 : _currentWaveIndex;
            completedWaves = Mathf.Clamp(completedWaves, 0, _waves.Count);
            var currentWaveProgress = _currentWave != null ? _currentWave.SpawnProgress01 : 0f;
            var waitingForClear = _currentWave != null && _currentWave.IsSpawnComplete && !_currentWave.IsCleared;
            var levelProgress = Mathf.Clamp01((completedWaves + currentWaveProgress) / _waves.Count);

            SetProgress(levelProgress, _currentWaveIndex, _waves.Count, currentWaveProgress, waitingForClear);
        }

        private void SetProgress(
            float level01,
            int currentWaveIndex,
            int totalWaves,
            float currentWave01,
            bool isWaitingForClear)
        {
            var clampedLevel = Mathf.Clamp01(level01);
            _progress.Value = new WaveProgress(
                clampedLevel,
                currentWaveIndex,
                totalWaves,
                Mathf.Clamp01(currentWave01),
                isWaitingForClear);
        }

        private Vector3 GetSpawnPosition(WaveDefinition wave)
        {
            return wave.SpawnPositionMode == SpawnPositionMode.SpecificPosition
                ? wave.SpecificSpawnPosition
                : _spawnArea.GetRandomPosition();
        }

        private sealed class WaveRuntimeState : IDisposable
        {
            private readonly List<EnemySpawnRuntime> _spawns;
            private readonly Dictionary<int, IDisposable> _enemyDeathSubscriptions = new();
            private readonly int _totalSpawnCount;
            private int _spawnedCount;

            public WaveDefinition Wave { get; }
            public float NextSpawnTime { get; set; }
            public bool IsStarted { get; private set; }
            public bool IsSpawnComplete => IsStarted && _spawns.Count == 0;
            public bool IsCleared { get; private set; }
            public float SpawnProgress01 => _totalSpawnCount <= 0 ? 1f : Mathf.Clamp01((float)_spawnedCount / _totalSpawnCount);

            public WaveRuntimeState(WaveDefinition wave, List<EnemySpawnRuntime> spawns)
            {
                Wave = wave;
                _spawns = spawns;
                _totalSpawnCount = TotalRemainingCount;
            }

            public void Start(float elapsedTime)
            {
                IsStarted = true;
                NextSpawnTime = elapsedTime;
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

                    _spawnedCount++;
                    return spawn;
                }

                return null;
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

            public void TryMarkCleared()
            {
                if (IsCleared || !IsStarted || _spawns.Count > 0 || _enemyDeathSubscriptions.Count > 0)
                    return;

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

        private static class WaveSpawnPlanBuilder
        {
            public static bool HasSpawns(WaveDefinition wave, EnemyService enemyService)
            {
                return Build(wave, enemyService).Count > 0;
            }

            public static List<EnemySpawnRuntime> Build(WaveDefinition wave, EnemyService enemyService)
            {
                if (wave.Enemies == null)
                    return new List<EnemySpawnRuntime>();

                return wave.SpawnMode == WaveSpawnMode.PointBudget
                    ? BuildPointBudgetSpawns(wave, enemyService)
                    : BuildManualSpawns(wave);
            }

            private static List<EnemySpawnRuntime> BuildManualSpawns(WaveDefinition wave)
            {
                var spawns = new List<EnemySpawnRuntime>();

                foreach (var enemy in wave.Enemies)
                {
                    if (enemy == null || enemy.Count <= 0 || string.IsNullOrWhiteSpace(enemy.EnemyTypeId))
                        continue;

                    spawns.Add(new EnemySpawnRuntime(enemy.EnemyTypeId, enemy.Count, enemy.Weight));
                }

                return spawns;
            }

            private static List<EnemySpawnRuntime> BuildPointBudgetSpawns(WaveDefinition wave, EnemyService enemyService)
            {
                var candidates = BuildBudgetCandidates(wave, enemyService);
                var remainingBudget = Mathf.Max(0, wave.PointBudget);

                foreach (var candidate in candidates)
                {
                    while (candidate.Count < candidate.MinCount && candidate.CanAfford(remainingBudget))
                    {
                        candidate.Count++;
                        remainingBudget -= candidate.PointPrice;
                    }
                }

                while (TryTakeWeightedCandidate(candidates, remainingBudget, out var selected))
                {
                    selected.Count++;
                    remainingBudget -= selected.PointPrice;
                }

                var spawns = new List<EnemySpawnRuntime>();

                foreach (var candidate in candidates)
                {
                    if (candidate.Count > 0)
                        spawns.Add(new EnemySpawnRuntime(candidate.EnemyTypeId, candidate.Count, candidate.Weight));
                }

                return spawns;
            }

            private static List<BudgetCandidate> BuildBudgetCandidates(WaveDefinition wave, EnemyService enemyService)
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
                    if (candidate.CanAfford(remainingBudget))
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

                    if (roll < 0)
                    {
                        selected = candidate;
                        return true;
                    }
                }

                return false;
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
