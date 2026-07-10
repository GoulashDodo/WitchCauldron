using System;
using Gameplay.Battle.Base.Interfaces;
using Gameplay.Battle.BattleEntities.Enemies.Services;
using Gameplay.Battle.HealthSystem.Core;
using Gameplay.Battle.HealthSystem.Structs;
using Gameplay.Battle.Waves.Service;
using Gameplay._root.SO;
using R3;
using UnityEngine;
using Zenject;

namespace Gameplay.Level
{
    public class G : ITickable, IDisposable
    {
        private readonly WaveService _waveService;
        private readonly EnemyService _enemyService;
        private readonly IHealth _baseHealth;
        private readonly GameplaySettings _gameplaySettings;

        
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private bool _areWavesCompleted;
        private bool _isGameEnded;
        private bool _isWinPending;
        private float _winTime;
        
        
        private readonly Subject<Unit> _gameStarted = new();
        private readonly Subject<Unit> _gameWon = new();
        private readonly Subject<DeathInfo> _gameLost = new();

        
        
        public Observable<Unit> GameStarted => _gameStarted;
        public Observable<DeathInfo> GameLost => _gameLost;
        public Observable<Unit> GameWon => _gameWon;

        public G(
            WaveService waveService,
            EnemyService enemyService,
            IBaseHealthProvider baseHealthProvider,
            GameplaySettings gameplaySettings)
        {
            _waveService = waveService;
            _enemyService = enemyService;
            _baseHealth = baseHealthProvider.GetBaseHealth();
            _gameplaySettings = gameplaySettings;
            
            SubscribeToGameplayEvents();
        }

        public void StartGameplay()
        {
            Debug.Log("Starting Gameplay");
            _gameStarted.OnNext(Unit.Default);
            
            _areWavesCompleted = false;
            _isGameEnded = false;
            _isWinPending = false;
            _waveService.StartWaves();
        }

        public void Tick()
        {
            if (!_isWinPending || Time.time < _winTime)
                return;

            CompletePendingWin();
        }

        
        
        private void EndGameplay()
        {
            if (_isGameEnded)
                return;

            _isGameEnded = true;
            _waveService.StopWaves();
        }

        private void SubscribeToGameplayEvents()
        {
            _baseHealth.Died
                .Subscribe(deathInfo =>
                {
                    if (_isGameEnded)
                        return;

                    EndGameplay();
                    _gameLost.OnNext(deathInfo);
                })
                .AddTo(_compositeDisposable);
            
            _waveService.WavesCompleted
                .Subscribe(_ =>
                {
                    _areWavesCompleted = true;
                    TryWin();
                })
                .AddTo(_compositeDisposable);

            _enemyService.ActiveEnemyCount
                .Subscribe(_ => TryWin())
                .AddTo(_compositeDisposable);
        }

        private void TryWin()
        {
            if (_isGameEnded || !_areWavesCompleted || _enemyService.ActiveEnemyCountValue > 0)
                return;

            _isGameEnded = true;
            _waveService.StopWaves();

            var victoryDelay = Mathf.Max(0f, _gameplaySettings != null ? _gameplaySettings.VictoryDelay : 0f);

            if (victoryDelay <= 0f)
            {
                _gameWon.OnNext(Unit.Default);
                return;
            }

            _isWinPending = true;
            _winTime = Time.time + victoryDelay;
        }

        private void CompletePendingWin()
        {
            if (!_isWinPending)
                return;

            _isWinPending = false;
            _gameWon.OnNext(Unit.Default);
        }


        public void Dispose()
        {
            _waveService?.Dispose();
            _gameStarted?.Dispose();
            _gameWon?.Dispose();
            _gameLost?.Dispose();
            _compositeDisposable?.Dispose();
        }

        #region FORCE

        public void ForceWin()
        {
            if (_isGameEnded)
                return;

            _isGameEnded = true;
            _isWinPending = false;
            _waveService.StopWaves();
            _gameWon.OnNext(Unit.Default);
        }

        public void ForceLose()
        {
            if (_isGameEnded)
                return;

            _baseHealth.TakeDamage(new BattleDamage(float.MaxValue, DamageType.Physical));
        }

        #endregion        
        
    }
}
