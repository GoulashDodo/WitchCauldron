using Feature.Gameplay.HealthSystem;
using Feature.Gameplay.HealthSystem.Core;
using UnityEngine;
using Zenject;

namespace Feature.Gameplay.Battle.Model
{
    public class Base : MonoBehaviour, IDamageable
    {
        
        public HealthComponent Health { get; private set; }

        
        [Inject]
        public void Construct()
        {
            Health = new HealthComponent(10);
        }
        
        
        public void TakeDamage(float damage)
        {
            Health.TakeDamage(damage);
        }
    }
}