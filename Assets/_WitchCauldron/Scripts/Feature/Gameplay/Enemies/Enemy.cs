using _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Services;
using _WitchCauldron.Scripts.Feature.Gameplay.HealthSystem;
using R3;
using UnityEngine;
using Zenject;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Enemies
{
    public class Enemy : MonoBehaviour, IDamagable
    {
        private EnemyService _enemyService;
        
        
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