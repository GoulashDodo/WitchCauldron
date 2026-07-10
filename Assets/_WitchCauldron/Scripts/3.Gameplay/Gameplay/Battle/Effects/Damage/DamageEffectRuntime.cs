using Gameplay.Battle.Effects.Base;
using Gameplay.Battle.HealthSystem;
using Gameplay.Battle.HealthSystem.Structs;

namespace Gameplay.Battle.Effects.Damage
{
    public class DamageEffectRuntime : EffectRuntime<DamageEffectData>
    {
        
        private IDamageable _damageable;

        private float _damageTimer;
        
        protected override void OnApply()
        {
            if (Target.TryGetComponent(out _damageable))
            {
                ApplyDamage();
            }
            _damageTimer = Data.Periodicity;
        }

        protected override void OnTick(float deltaTime)
        {
            if (_damageable == null)
                return;
            
            _damageTimer -= deltaTime;

            if (_damageTimer <= 0f)
            {
                ApplyDamage();
                _damageTimer = Data.Periodicity;
            }
            
        }
        
        protected override void OnRemove() { }

        private void ApplyDamage()
        {
            var damage = new BattleDamage(Data.Damage, Data.DamageType);
            _damageable.TakeDamage(damage);
            
            
        }        
        
        
    }
}