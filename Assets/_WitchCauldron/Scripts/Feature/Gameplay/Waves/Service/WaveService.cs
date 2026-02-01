using _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Services;
using _WitchCauldron.Scripts.Feature.Gameplay.Waves.SpawnArea;
using R3;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Waves.Service
{
    public class WaveService : IWaveService
    {
        
        private readonly ISpawnArea _spawnArea;
        private EnemyService _enemyService;
        

        private readonly Subject<Unit> _onLevelCompleted = new Subject<Unit>();
        private readonly ReactiveProperty<float> _progress01;

        
        private float _levelStartTime;
        private bool _isRunning;

        public WaveService(ISpawnArea spawnArea, EnemyService enemyService)
        {
            _spawnArea = spawnArea;
            _enemyService = enemyService;
        }

        public ReadOnlyReactiveProperty<float> Progress01 => _progress01;
        public Observable<Unit> LevelCompleted => _onLevelCompleted;


        
        
        public void StartLevel()
        {
            if(_isRunning) StopLevel();
            _isRunning = true;

            
            //TEST ONLY

            var rPosition = _spawnArea.GetRandomPosition();
            
            _enemyService.SpawnEnemy("Enemy_Dummy", rPosition);

        }



        public void StopLevel()
        {
            _onLevelCompleted.OnNext(Unit.Default);
            
        }

    }
}