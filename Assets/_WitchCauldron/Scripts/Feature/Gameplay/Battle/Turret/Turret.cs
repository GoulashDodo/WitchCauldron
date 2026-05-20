using Feature.Gameplay.Battle.Enemies.Core;
using UnityEngine;

namespace Feature.Gameplay.Battle.Turret
{
    public class Turret : MonoBehaviour
    {
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private float _attackRange = 5f;
        [SerializeField] private float _attackCooldown = 1f;

        private readonly Collider2D[] _overlapBuffer = new Collider2D[32];
        private ContactFilter2D _contactFilter;
        private float _nextAttackTime;

        private void Awake()
        {
            _contactFilter = new ContactFilter2D
            {
                useTriggers = true
            };
        }

        private void Update()
        {
            if (Time.time < _nextAttackTime)
                return;

            if (!TryFindTarget(out var target))
                return;

            Shoot(target);
            _nextAttackTime = Time.time + Mathf.Max(0.01f, _attackCooldown);
        }

        private bool TryFindTarget(out Enemy target)
        {
            target = null;

            var count = Physics2D.OverlapCircle(
                transform.position,
                Mathf.Max(0f, _attackRange),
                _contactFilter,
                _overlapBuffer);

            var bestSqrDistance = float.MaxValue;

            for (var i = 0; i < count; i++)
            {
                var hit = _overlapBuffer[i];
                if (hit == null)
                    continue;

                var enemy = hit.GetComponentInParent<Enemy>();
                if (enemy == null || !enemy.gameObject.activeInHierarchy)
                    continue;

                var sqrDistance = (enemy.transform.position - transform.position).sqrMagnitude;
                if (sqrDistance >= bestSqrDistance)
                    continue;

                bestSqrDistance = sqrDistance;
                target = enemy;
            }

            return target != null;
        }

        private void Shoot(Enemy target)
        {
            var projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
            projectile.Launch(target);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, Mathf.Max(0f, _attackRange));
        }
    }
}
