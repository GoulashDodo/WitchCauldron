using System.Collections.Generic;
using Gameplay.Battle.Effects;
using Gameplay.Items.Usable.Commands.Handler;
using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Effect
{
    public class EffectCommandHandler : UseCommandHandler<EffectCommandParameters>
    {
        
        private readonly Collider2D[] _buffer = new Collider2D[20];
        private readonly ContactFilter2D _contactFilter = new();
        
        public override bool Handle(EffectCommandParameters p, Vector2 pos, UseCommandContext context = null)
        {
            var radius = Mathf.Max(0f, p.Radius);
            var hitCount = Physics2D.OverlapCircle(pos, radius, _contactFilter, _buffer);

            bool applied;
            if (p.IsArea)
            {
                applied = ApplyEffectToAllEnemies(p, pos, hitCount);
            }
            else
            {
                applied = ApplyEffectToFirstEnemy(p, pos, hitCount);
            }

            if (applied)
                context?.FxPlayer?.PlayImpactFx(pos, context.ItemSettings, context.ItemWorldScale);

            return applied;
        }
        
        private bool ApplyEffectToFirstEnemy(EffectCommandParameters p, Vector2 pos, int hitCount)
        {
            for (var i = 0; i < hitCount; i++)
            {
                var hit = _buffer[i];
                if (!TryGetEnemyEffectReceiver(hit, out var damageable))
                    continue;

                ApplyEffect(p, pos, damageable);
                return true;
            }

            return false;
        }

        private bool ApplyEffectToAllEnemies(EffectCommandParameters p, Vector2 pos, int hitCount)
        {
            var appliedEnemies = new HashSet<EffectReceiver>();

            for (var i = 0; i < hitCount; i++)
            {
                var hit = _buffer[i];
                if (!TryGetEnemyEffectReceiver(hit, out var damageable, out var enemy))
                    continue;

                if (!appliedEnemies.Add(enemy))
                    continue;

                ApplyEffect(p, pos, damageable);
            }

            return appliedEnemies.Count > 0;
        }

        private static bool TryGetEnemyEffectReceiver(Collider2D hit, out EffectReceiver receiver)
        {
            return TryGetEnemyEffectReceiver(hit, out receiver, out _);
        }

        private static bool TryGetEnemyEffectReceiver(Collider2D hit, out EffectReceiver receiver, out EffectReceiver enemyReceiver)
        {
            receiver = null;
            enemyReceiver = null;

            if (hit == null)
                return false;

            enemyReceiver = hit.GetComponentInParent<EffectReceiver>();
            if (enemyReceiver == null)
                return false;

            receiver = enemyReceiver;
            return true;
        }

        private static void ApplyEffect(EffectCommandParameters p, Vector2 pos, EffectReceiver enemyReceiver)
        {
            Debug.Log($"Applying effect at {p.EffectToApply.EffectName} at {pos}");
            enemyReceiver.AddEffect(p.EffectToApply);
            
        }
        
    }
}
