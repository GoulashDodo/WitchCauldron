using System;
using _WitchCauldron.Scripts.Feature.Gameplay.HealthSystem.Structs;
using R3;
using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.HealthSystem.Core
{
    public class HealthModel : IDamageable, IDisposable
    {

        private readonly float _maxHealth;       
        private readonly ReactiveProperty<float> _currentHealth;

        
        
        
        private readonly Subject<DamageInfo> _damaged = new();
        
        private readonly Subject<DeathInfo> _died = new();

        
        
        public Observable<DamageInfo> Damaged => _damaged;
        public Observable<DeathInfo> Died => _died;
        
        
        public HealthModel(float maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = new ReactiveProperty<float>(maxHealth);
        }
        
        
        public void TakeDamage(float damage)
        {
            
            
            if (damage <= 0)
            {
                Debug.LogWarning("Damage must be positive");
                return;
            }            

            
            

            var newHealth = Mathf.Max(_currentHealth.Value - damage, 0f);
            _currentHealth.Value = newHealth;

            
            var info = new DamageInfo(damage, _currentHealth.Value, _maxHealth);

            
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