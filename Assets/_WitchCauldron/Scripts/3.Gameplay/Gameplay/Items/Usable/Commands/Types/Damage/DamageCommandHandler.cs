using System.Collections.Generic;
using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.HealthSystem;
using Gameplay.Battle.HealthSystem.Structs;
using Gameplay.Items.Usable.Commands.Handler;
using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Damage
{
    public sealed class DamageCommandHandler : UseCommandHandler<DamageCommandParameters>
    {

        private readonly Collider2D[] _buffer = new Collider2D[20];
        private readonly ContactFilter2D _contactFilter = new();
        
        public override bool Handle(DamageCommandParameters p, Vector2 pos, UseCommandContext context = null)
        {
            if (context?.TargetEnemy != null)
            {
                DoDamage(p, pos, context.TargetEnemy);
                return true;
            }

            var radius = Mathf.Max(0f, p.Radius);
            var hitCount = Physics2D.OverlapCircle(pos, radius, _contactFilter, _buffer);

            bool damaged;
            if (p.IsArea)
            {
                damaged = DamageAllEnemies(p, pos, hitCount);
            }
            else
            {
                damaged = DamageFirstEnemy(p, pos, hitCount);
            }

            if (damaged)
                context?.PlayImpactFxOnce(pos);

            return damaged;
        }

        private bool DamageFirstEnemy(DamageCommandParameters p, Vector2 pos, int hitCount)
        {
            for (var i = 0; i < hitCount; i++)
            {
                var hit = _buffer[i];
                if (!TryGetEnemyDamageable(hit, out var damageable))
                    continue;

                DoDamage(p, pos, damageable);
                return true;
            }

            return false;
        }

        private bool DamageAllEnemies(DamageCommandParameters p, Vector2 pos, int hitCount)
        {
            var damagedEnemies = new HashSet<Enemy>();

            for (var i = 0; i < hitCount; i++)
            {
                var hit = _buffer[i];
                if (!TryGetEnemyDamageable(hit, out var damageable, out var enemy))
                    continue;

                if (!damagedEnemies.Add(enemy))
                    continue;

                DoDamage(p, pos, damageable);
            }

            return damagedEnemies.Count > 0;
        }

        private static bool TryGetEnemyDamageable(Collider2D hit, out IDamageable damageable)
        {
            return TryGetEnemyDamageable(hit, out damageable, out _);
        }

        private static bool TryGetEnemyDamageable(Collider2D hit, out IDamageable damageable, out Enemy enemy)
        {
            damageable = null;
            enemy = null;

            if (hit == null)
                return false;

            enemy = hit.GetComponentInParent<Enemy>();
            if (enemy == null)
                return false;

            damageable = enemy;
            return true;
        }

        private static void DoDamage(DamageCommandParameters p, Vector2 pos, IDamageable damageable)
        {
            Debug.Log($"Do damage {p.Damage} at {pos}");
            damageable.TakeDamage(new BattleDamage(p.Damage, p.DamageType));
        }
    }
}
