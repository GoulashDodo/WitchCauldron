using _WitchCauldron.Scripts.Feature.Gameplay.HealthSystem;
using _WitchCauldron.Scripts.Feature.Gameplay.HealthSystem.Core;
using _WitchCauldron.Scripts.Feature.Gameplay.Level._Root;
using UnityEngine;
using Zenject;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Battle.Model
{
    public class Base : MonoBehaviour, IDamageable
    {
        
        public HealthModel Health { get; private set; }

        
        [Inject]
        public void Construct(LevelConfig levelConfig)
        {
            Health = new HealthModel(levelConfig.BaseHealth);
        }
        
        
        public void TakeDamage(float damage)
        {
            Health.TakeDamage(damage);
        }
    }
}