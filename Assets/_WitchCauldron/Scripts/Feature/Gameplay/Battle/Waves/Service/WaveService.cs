using System;
using System.Collections.Generic;
using Core.GameRoot._root;
using Feature.Gameplay.Battle.Enemies.Services;
using Feature.Gameplay.Battle.Waves.Enums;
using Feature.Gameplay.Battle.Waves.SO;
using Feature.Gameplay.Battle.Waves.SO.Structures;
using Feature.Gameplay.Battle.Waves.SpawnArea;
using Feature.Gameplay.Level.SO;
using R3;
using UnityEngine;
using Zenject;

namespace Feature.Gameplay.Battle.Waves.Service
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
            _runtimeStates.Clear();
        }

        public void Tick()
        {
            if (!_isRunning)
                return;

            var elapsedTime = Time.time - _levelStartTime;
            var hasActiveWaves = false;

            foreach (var state in _runtimeStates)
            {
                if (state.IsComplete)
                    continue;

                hasActiveWaves = true;
                TrySpawnFromWave(state, elapsedTime);
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
            _progress01.Dispose();
            _wavesCompleted.Dispose();
        }

        private void BuildRuntimeStates()
        {
            _runtimeStates.Clear();
            _levelDuration = 0f;

            var waves = _waveSettings != null ? _waveSettings.Waves : null;

            if (waves == null)
                return;

            foreach (var wave in waves)
            {
                if (wave == null || wave.TotalEnemyCount <= 0)
                    continue;

                var state = new WaveRuntimeState(wave, _waveSettings.StartDelay);
                _runtimeStates.Add(state);

                _levelDuration = Mathf.Max(_levelDuration, state.EndTime);
            }
        }

        private void TrySpawnFromWave(WaveRuntimeState state, float elapsedTime)
        {
            if (elapsedTime < state.NextSpawnTime)
                return;

            var spawn = state.TakeNextSpawn();

            if (spawn == null)
                return;

            var position = GetSpawnPosition(state.Wave);
            _enemyService.SpawnEnemy(spawn.EnemyTypeId, position);

            state.NextSpawnTime += state.Wave.SpawnInterval;
        }

        private Vector3 GetSpawnPosition(WaveDefinition wave)
        {
            return wave.SpawnPositionMode == SpawnPositionMode.SpecificPosition
                ? wave.SpecificSpawnPosition
                : _spawnArea.GetRandomPosition();
        }

        private sealed class WaveRuntimeState
        {
            private readonly List<EnemySpawnRuntime> _spawns = new();

            public WaveDefinition Wave { get; }
            public float NextSpawnTime { get; set; }
            public float EndTime { get; }
            public bool IsComplete => _spawns.Count == 0;

            public WaveRuntimeState(WaveDefinition wave, float levelStartDelay)
            {
                Wave = wave;
                NextSpawnTime = levelStartDelay + wave.StartTime;
                EndTime = NextSpawnTime + wave.Duration;

                if (wave.Enemies == null)
                    return;

                foreach (var enemy in wave.Enemies)
                {
                    if (enemy.Count <= 0 || string.IsNullOrWhiteSpace(enemy.EnemyTypeId))
                        continue;

                    _spawns.Add(new EnemySpawnRuntime(enemy.EnemyTypeId, enemy.Count, enemy.Weight));
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
    }
}
