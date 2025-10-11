using R3;
using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Enemies.Services;
using WitchCauldron.Scripts.Feature.Gameplay.HealthSystem;
using Zenject;

namespace WitchCauldron.Scripts.Feature.Gameplay.Enemies
{
    public abstract class Enemy : MonoBehaviour, IDamagable
    {
        private EnemyService _enemyService;
        private EnemySettings _settings;
        
        
        private Health _health;

        public Observable<int> TakenDamage => _health.TakenDamage;

        
        [Inject]
        public void Initialize(EnemyService enemyService)
        {
            _enemyService = enemyService;
            _health = new Health();
        }
        
        
        public void TakeDamage(int damage)
        {
            _health.TakeDamage(damage);
        }
    }
}