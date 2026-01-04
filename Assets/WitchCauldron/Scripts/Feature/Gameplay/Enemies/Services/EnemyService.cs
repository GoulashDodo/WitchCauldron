using System.Collections.Generic;
using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Enemies.Services
{
    public class EnemyService
    {

        private readonly Dictionary<int, Enemy> _allEnemies = new();
        

        public Enemy SpawnEnemy(EnemySettings settings, Vector3 position)
        {
            var enemyPf = settings.EnemyPf;
            var enemy = Object.Instantiate(enemyPf, position, Quaternion.identity);
            
            enemy.Initialize(this);
            _allEnemies.Add(enemy.GetEntityId() , enemy);
            
            return enemy;
        }

        public void DamageEnemy(int enemyId, int damage)
        {
            var enemy = _allEnemies[enemyId];
            enemy.TakeDamage(damage);
        }
        
    }
}