using Gameplay.Battle.Base.Interfaces;
using Gameplay.Battle.HealthSystem;
using Gameplay.Battle.HealthSystem.Core;
using Gameplay.Battle.HealthSystem.Structs;
using UnityEngine;
using Zenject;

namespace Gameplay.Battle.Base.Core
{
    public class BaseView : MonoBehaviour, IDamageable
    {
        private IHealth Health { get; set; }

        
        [Inject]
        public void Construct(IBaseHealthProvider healthProvider)
        {
            Health = healthProvider.GetBaseHealth();
        }
        
        
        public void TakeDamage(BattleDamage battleDamage)
        {
            Health.TakeDamage(battleDamage);
        }
    }
}
