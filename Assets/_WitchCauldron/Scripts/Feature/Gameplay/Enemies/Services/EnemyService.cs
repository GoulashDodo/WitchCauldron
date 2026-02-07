using System.Collections.Generic;
using _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Core;
using _WitchCauldron.Scripts.Feature.Gameplay.Enemies.SO;
using R3;
using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Services
{
    public class EnemyService
    {

        
        private readonly Dictionary<string, EnemySettings> _allEnemies = new();
        
        private readonly Dictionary<int, Enemy> _allExistingEnemies = new();



        public EnemyService(AllEnemySettings allEnemySettings)
        {
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

        }


        public void UnregisterEnemy(Enemy enemyToUnregister)
        {
            _allExistingEnemies.Remove(enemyToUnregister.GetInstanceID());
        }
        
        
    }
}