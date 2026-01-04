using R3;
using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Waves.ScriptableObjects;
using WitchCauldron.Scripts.Feature.Gameplay.Waves.SpawnArea;

namespace WitchCauldron.Scripts.Feature.Gameplay.Waves.Service
{
    public class WaveService : IWaveService
    {
        private readonly ISpawnArea _spawnArea;


        private readonly Subject<Unit> _onLevelCompleted = new Subject<Unit>();
        private readonly ReactiveProperty<float> _progress01;

        
        private LevelScript _script;
        private float _levelStartTime;
        private bool _isRunning;
        private CompositeDisposable _disposables;

        public ReadOnlyReactiveProperty<float> Progress01 => _progress01;
        public Observable<Unit> LevelCompleted => _onLevelCompleted;


        public void StartLevel(LevelScript levelScript)
        {
            if(_isRunning) StopLevel();
            _isRunning = true;
            _script = levelScript;

            Observable.EveryUpdate()
                .TakeWhile(_ => _isRunning)
                .Subscribe(_ =>
                {
                    var time = Mathf.Clamp01((Time.time - _levelStartTime) / Mathf.Max(0.0001f, _script.LevelDuration));
                    _progress01.Value = time;
                    
                    if (time >= 1f)
                    {
                        _isRunning = false;
                        _onLevelCompleted.OnNext(Unit.Default);
                    }
                    
                })
                .AddTo(_disposables);

        }



        public void StopLevel()
        {
            _onLevelCompleted.OnNext(Unit.Default);
            
        }

    }
}