using Core.Audio;
using Gameplay.Battle.Base.Interfaces;
using Gameplay.Battle.HealthSystem;
using Gameplay.Battle.HealthSystem.Core;
using Gameplay.Battle.HealthSystem.Structs;
using UnityEngine;
using Zenject;

namespace Gameplay.Battle.Base.Core
{
    public class BaseView : MonoBehaviour, IDamageable, IEnemyAttackTarget
    {
        private IHealth Health { get; set; }
        private AudioService _audioService;
        public IDamageable Damageable => this;

        
        [Inject]
        public void Construct(IBaseHealthProvider healthProvider, AudioService audioService)
        {
            Health = healthProvider.GetBaseHealth();
            _audioService = audioService;
        }
        
        
        public void TakeDamage(BattleDamage battleDamage)
        {
            Health.TakeDamage(battleDamage);
            _audioService?.PlaySfx(AudioId.Base_Damage, transform.position);

            if (Health.CurrentHealthValue <= 0f)
                _audioService?.PlaySfx(AudioId.Base_Destroyed, transform.position);
        }

    }
}
