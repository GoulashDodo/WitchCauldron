using _WitchCauldron.Scripts.Feature.Gameplay.HealthSystem;
using _WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Commands.Handler;
using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Commands.Damage
{
    public sealed class DamageCommandHandler : UseCommandHandler<DamageCommandParameters>
    {

        public override bool Handle(DamageCommandParameters p, Vector2 pos)
        {

            var hit = Physics2D.OverlapPoint(pos);
            Debug.Log(hit);
            
            if (hit == null)
            {
                return false;
            }

            if (!hit.TryGetComponent<IDamageable>(out var damagable))
            {
                return false;
            }

            
            Debug.Log($"Do damage {p.Damage} at {pos}");
            damagable.TakeDamage(p.Damage);
            
            return true;
        }
    }
}