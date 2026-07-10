using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Items.Usable.Commands.Preview;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Friendly.Turrets
{
    public class Turret : MonoBehaviour, IPlacementRadiusPreviewSource
    {
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private Transform _shootingPoint;
        
        [SerializeField] private float _attackRange = 5f;
        [SerializeField] private float _attackCooldown = 1f;
        
        private readonly Collider2D[] _overlapBuffer = new Collider2D[32];
        
        private ContactFilter2D _contactFilter;
        private float _nextAttackTime;

        public float PreviewRadius => Mathf.Max(0f, _attackRange);
        public Transform PreviewOrigin => _shootingPoint ? _shootingPoint : transform;

        private void Awake()
        {
            _contactFilter = new ContactFilter2D
            {
                useTriggers = true
            };

            if (!_shootingPoint)
            {
                _shootingPoint = transform;
            }
            
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
                _shootingPoint.position,
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

                var sqrDistance = (enemy.transform.position - _shootingPoint.position).sqrMagnitude;
                if (sqrDistance >= bestSqrDistance)
                    continue;

                bestSqrDistance = sqrDistance;
                target = enemy;
            }

            return target != null;
        }

        private void Shoot(Enemy target)
        {
            var projectile = Instantiate(_projectilePrefab, _shootingPoint.position, Quaternion.identity);
            projectile.Launch(target.transform.position - _shootingPoint.position);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(_shootingPoint ? _shootingPoint.position : transform.position,
                Mathf.Max(0f, _attackRange));
        }
    }
}
