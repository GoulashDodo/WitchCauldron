using R3;
using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.HealthSystem
{
    public class Health : IDamagable
    {

        private ReactiveProperty<int> _maxHealth;       
        private ReactiveProperty<int> _currentHealth;
        
        
        private readonly Subject<int> _takenDamage = new Subject<int>();
        public Observable<int> TakenDamage => _takenDamage;

        public Health()
        {
            
        }
        
        
        public void TakeDamage(int damage)
        {
            if (damage < 0)
            {
                Debug.LogWarning("Damage is negative!");
                return;
            }
            
            _currentHealth.Value -= damage;
            _takenDamage.OnNext(damage);
        }
    }
}