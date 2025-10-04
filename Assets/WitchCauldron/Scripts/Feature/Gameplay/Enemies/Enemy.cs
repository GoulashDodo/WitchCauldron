using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Enemies.Services;
using Zenject;

namespace WitchCauldron.Scripts.Feature.Gameplay.Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        private EnemyService _enemyService;
        
        private EnemySettings _settings;
        
        [Inject]
        public void Initialize(EnemyService enemyService)
        {
            _enemyService = enemyService;
        }
        
        
    }
}