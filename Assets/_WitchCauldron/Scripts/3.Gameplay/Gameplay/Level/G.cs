using System;
using Gameplay.Battle.Base.Interfaces;
using Gameplay.Battle.Enemies.Services;
using Gameplay.Battle.HealthSystem.Core;
using Gameplay.Battle.HealthSystem.Structs;
using Gameplay.Battle.Waves.Service;
using R3;
using UnityEngine;

namespace Gameplay.Level
{
    public class G : IDisposable
    {
        private readonly WaveService _waveService;
        private readonly EnemyService _enemyService;
        private readonly IHealth _baseHealth;
        private readonly Subject<Unit> _gameWon = new();

        
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private bool _areWavesCompleted;
        private bool _isGameEnded;
        
        public Observable<DeathInfo> GameLost => _baseHealth.Died;
        public Observable<Unit> GameWon => _gameWon;

        public G(WaveService waveService, EnemyService enemyService, IBaseHealthProvider baseHealthProvider)
        {
            _waveService = waveService;
            _enemyService = enemyService;
            _baseHealth = baseHealthProvider.GetBaseHealth();
            
            SubscribeToGameplayEvents();
        }

        public void StartGameplay()
        {
            Debug.Log("Starting Gameplay");
            _areWavesCompleted = false;
            _isGameEnded = false;
            _waveService.StartWaves();
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
            GameLost.Subscribe(_ => EndGameplay()).AddTo(_compositeDisposable);
            
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
            _gameWon.OnNext(Unit.Default);
        }


        public void Dispose()
        {
            _waveService?.Dispose();
            _gameWon?.Dispose();
            _compositeDisposable?.Dispose();
        }
    }
}
