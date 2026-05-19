using Feature.Gameplay.Battle.Base.Interfaces;
using Feature.Gameplay.Battle.HealthSystem;
using Feature.Gameplay.Battle.HealthSystem.Core;
using UnityEngine;
using Zenject;

namespace Feature.Gameplay.Battle.Base.Core
{
    public class BaseView : MonoBehaviour, IDamageable
    {
        private IHealth Health { get; set; }

        
        [Inject]
        public void Construct(IBaseHealthProvider healthProvider)
        {
            Health = healthProvider.GetBaseHealth();
        }
        
        
        public void TakeDamage(float damage)
        {
            Health.TakeDamage(damage);
        }
    }
}
