using System.Collections.Generic;
using Feature.Gameplay._root.SO;
using Feature.Gameplay.Battle.Enemies.Core;
using Feature.Gameplay.Battle.Enemies.SO;
using R3;
using UnityEngine;

namespace Feature.Gameplay.Battle.Enemies.Services
{
    public class EnemyService
    {

        
        private readonly Dictionary<string, EnemySettings> _allEnemies = new();
        
        private readonly Dictionary<int, Enemy> _allExistingEnemies = new();

        private readonly ReactiveProperty<int> _activeEnemyCount = new(0);

        public ReadOnlyReactiveProperty<int> ActiveEnemyCount => _activeEnemyCount;
        public int ActiveEnemyCountValue => _activeEnemyCount.Value;



        public EnemyService(GameplaySettings settings)
        {
            var allEnemySettings = settings.AllEnemiesSettings;
            
            foreach (var enemySetting in allEnemySettings.AllSettings)
            {
                _allEnemies.Add(enemySetting.TypeId, enemySetting);
            }
        }
        
        
        
        public Enemy SpawnEnemy(string typeId, Vector3 position)
        {
            
            var enemySettings = _allEnemies[typeId];
            
            var enemyPf = enemySettings.EnemyPf;
            var enemy = Object.Instantiate(enemyPf, position, Quaternion.identity);
            
            enemy.Construct(this, enemySettings);
            
            
            
            return enemy;
        }

        public void DamageEnemy(int enemyId, int damage)
        {
            var enemy = _allExistingEnemies[enemyId];
            enemy.TakeDamage(damage);
        }




        public void RegisterEnemy(Enemy enemyToRegister)
        {
            _allExistingEnemies.Add(enemyToRegister.GetInstanceID() , enemyToRegister);
            _activeEnemyCount.Value = _allExistingEnemies.Count;

        }


        public void UnregisterEnemy(Enemy enemyToUnregister)
        {
            _allExistingEnemies.Remove(enemyToUnregister.GetInstanceID());
            _activeEnemyCount.Value = _allExistingEnemies.Count;
        }
        
        
    }
}
