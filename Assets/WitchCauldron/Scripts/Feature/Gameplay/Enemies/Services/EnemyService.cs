using System.Collections.Generic;
using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Enemies.Services
{
    public class EnemyService
    {

        private readonly Dictionary<int, Enemy> _allEnemies = new();
        
        

        public Enemy SpawnEnemy(Enemy enemyPf, Vector3 position)
        {
            var enemy = Object.Instantiate(enemyPf, position, Quaternion.identity);
            enemy.Initialize(this);
            _allEnemies.Add(enemy.GetEntityId() , enemy);
            
            return enemy;
        }
        
    }
}