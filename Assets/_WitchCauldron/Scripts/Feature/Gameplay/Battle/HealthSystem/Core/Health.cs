using System;
using Feature.Gameplay.Battle.HealthSystem.Structs;
using R3;
using UnityEngine;

namespace Feature.Gameplay.Battle.HealthSystem.Core
{
    public class Health : IHealth, IDamageable, IDisposable
    {

        private readonly float _maxHealth;       
        private readonly ReactiveProperty<float> _currentHealth;
        
        private readonly Subject<DamageInfo> _damaged = new();
        
        private readonly Subject<DeathInfo> _died = new();

        
        public Observable<float> CurrentHealth => _currentHealth;
        
        
        public Observable<DamageInfo> Damaged => _damaged;
        public Observable<DeathInfo> Died => _died;
        
        
        public Health(float maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = new ReactiveProperty<float>(maxHealth);
        }
        
        
        public void TakeDamage(BattleDamage battleDamage)
        {
            if (battleDamage.Amount <= 0)
            {
                Debug.LogWarning("Damage must be positive");
                return;
            }            
            

            var newHealth = Mathf.Max(_currentHealth.Value - battleDamage.Amount, 0f);
            _currentHealth.Value = newHealth;

            
            var info = new DamageInfo(battleDamage.Amount, _currentHealth.Value, _maxHealth);

            
            _damaged.OnNext(info);

            if (newHealth <= 0f)
            {
                _died.OnNext(new DeathInfo());
            }
        }

        public void Dispose()
        {
            _currentHealth?.Dispose();
            _damaged?.Dispose();
            _died?.Dispose();
        }
    }
}
