using Core.Data;
using Gameplay.Battle.HealthSystem;
using Gameplay.Battle.HealthSystem.Structs;
using System.Collections;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Enemies.Core
{
    
    [RequireComponent(typeof(Enemy))]
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField] private LayerMask _targetLayerMask;
        [SerializeField, Range(0f, 1f)] private float _fallbackHitNormalizedTime = 0.5f;
        
        private Enemy _enemy;
        private float _nextAttackTime;
        private IDamageable _currentTarget;
        private Coroutine _fallbackHitCoroutine;
        private int _attackId;
        private bool _hitApplied;

        private void Awake()
        {
            _enemy = GetComponent<Enemy>();
            
            if (_targetLayerMask == 0)
            {
                _targetLayerMask = LayerMask.GetMask(Layers.Base);
            }
        }

        public bool TryFindTarget(out IDamageable target)
        {
            var hit = Physics2D.Raycast(
                transform.position,
                Vector2.left,
                _enemy.Settings.AttackDistance,
                _targetLayerMask);

            target = hit.collider != null
                ? hit.collider.GetComponentInParent<IEnemyAttackTarget>()?.Damageable
                : null;

            return target != null;
        }

        public bool TryStartAttack(IDamageable target)
        {
            if (Time.time < _nextAttackTime)
                return false;

            var attackCooldown = GetAttackCooldown();
            
            _currentTarget = target;
            _hitApplied = false;
            _attackId++;
            _nextAttackTime = Time.time + attackCooldown;
            
            if (_fallbackHitCoroutine != null)
                StopCoroutine(_fallbackHitCoroutine);
            
            _fallbackHitCoroutine = StartCoroutine(ApplyFallbackHit(_attackId, attackCooldown));
            
            return true;
        }

        public void ApplyCurrentAttackHit()
        {
            ApplyAttackHit(_attackId);
        }

        private IEnumerator ApplyFallbackHit(int attackId, float attackCooldown)
        {
            yield return new WaitForSeconds(attackCooldown * _fallbackHitNormalizedTime);
            ApplyAttackHit(attackId);
        }

        private void ApplyAttackHit(int attackId)
        {
            if (attackId != _attackId || _hitApplied)
                return;

            _hitApplied = true;
            _currentTarget?.TakeDamage(new BattleDamage(_enemy.Settings.Damage, DamageType.Physical));
        }

        private float GetAttackCooldown()
        {
            return 1f / Mathf.Max(_enemy.Settings.AttackSpeed, 0.01f);
        }
    }
}
