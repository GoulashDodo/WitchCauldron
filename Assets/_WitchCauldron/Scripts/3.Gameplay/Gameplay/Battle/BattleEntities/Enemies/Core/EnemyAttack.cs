using Core.Data;
using Gameplay.Battle.HealthSystem;
using Gameplay.Battle.HealthSystem.Structs;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Enemies.Core
{
    
    [RequireComponent(typeof(Enemy))]
    public class EnemyAttack : MonoBehaviour
    {
        private Enemy _enemy;
        private LayerMask _baseLayerMask;
        private float _nextAttackTime;

        private void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _baseLayerMask = LayerMask.GetMask(Layers.Base);
        }

        public bool TryFindTarget(out IDamageable target)
        {
            var hit = Physics2D.Raycast(
                transform.position,
                Vector2.left,
                _enemy.Settings.AttackDistance,
                _baseLayerMask);

            target = hit.collider != null
                ? hit.collider.GetComponentInParent<IDamageable>()
                : null;

            return target != null;
        }

        public bool TryAttack(IDamageable target)
        {
            if (Time.time < _nextAttackTime)
                return false;

            target.TakeDamage(new BattleDamage(_enemy.Settings.Damage, DamageType.Physical));
            _nextAttackTime = Time.time + GetAttackCooldown();
            return true;
        }

        private float GetAttackCooldown()
        {
            return 1f / Mathf.Max(_enemy.Settings.AttackSpeed, 0.01f);
        }
    }
}