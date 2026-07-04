using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Items.Usable.Commands.Preview;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Friendly.Turrets
{
    public sealed class StraightLineTurret : MonoBehaviour, IPlacementBoxPreviewSource
    {
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private Transform _shootingPoint;
        [SerializeField] private float _attackRange = 7f;
        [SerializeField] private float _attackWidth = 1f;
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private bool _aimAtTarget = true;

        private readonly Collider2D[] _overlapBuffer = new Collider2D[32];

        private ContactFilter2D _contactFilter;
        private float _nextAttackTime;
        
        public Vector2 PreviewSize => new(Mathf.Max(0f, _attackRange), Mathf.Max(0.01f, _attackWidth));
        public Vector2 PreviewOffset => Vector2.right * (Mathf.Max(0f, _attackRange) * 0.5f);
        public Transform PreviewOrigin => _shootingPoint ? _shootingPoint : transform;

        private void Awake()
        {
            _contactFilter = new ContactFilter2D
            {
                useTriggers = true
            };

            if (!_shootingPoint)
                _shootingPoint = transform;
        }

        private void Update()
        {
            if (Time.time < _nextAttackTime)
                return;

            if (!TryFindTargetInLane(out var target))
                return;

            Shoot(target);
            _nextAttackTime = Time.time + Mathf.Max(0.01f, _attackCooldown);
        }

        private bool TryFindTargetInLane(out Enemy target)
        {
            target = null;

            var forward = GetForwardDirection();
            var center = (Vector2)_shootingPoint.position + forward * (Mathf.Max(0f, _attackRange) * 0.5f);
            var size = new Vector2(Mathf.Max(0f, _attackRange), Mathf.Max(0.01f, _attackWidth));
            var angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
            var count = Physics2D.OverlapBox(center, size, angle, _contactFilter, _overlapBuffer);
            var bestForwardDistance = float.MaxValue;

            for (var i = 0; i < count; i++)
            {
                var hit = _overlapBuffer[i];
                if (hit == null)
                    continue;

                var enemy = hit.GetComponentInParent<Enemy>();
                if (enemy == null || enemy.IsDead || !enemy.gameObject.activeInHierarchy)
                    continue;

                var toEnemy = (Vector2)enemy.transform.position - (Vector2)_shootingPoint.position;
                var forwardDistance = Vector2.Dot(toEnemy, forward);
                if (forwardDistance < 0f || forwardDistance >= bestForwardDistance)
                    continue;

                bestForwardDistance = forwardDistance;
                target = enemy;
            }

            return target != null;
        }

        private void Shoot(Enemy target)
        {
            if (_projectilePrefab == null)
                return;

            var projectile = Instantiate(_projectilePrefab, _shootingPoint.position, Quaternion.identity);
            Vector3 direction = _aimAtTarget && target != null
                ? target.transform.position - _shootingPoint.position
                : GetForwardDirection();
            projectile.Launch(direction);
        }

        private Vector2 GetForwardDirection()
        {
            var direction = _shootingPoint ? _shootingPoint.right : transform.right;
            return direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.right;
        }

        private void OnDrawGizmosSelected()
        {
            var shootingPoint = _shootingPoint ? _shootingPoint : transform;
            var forward = (Vector2)(shootingPoint.right.sqrMagnitude > 0.001f ? shootingPoint.right.normalized : Vector3.right);
            var center = (Vector2)shootingPoint.position + forward * (Mathf.Max(0f, _attackRange) * 0.5f);
            var size = new Vector3(Mathf.Max(0f, _attackRange), Mathf.Max(0.01f, _attackWidth), 0f);
            var angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;

            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.AngleAxis(angle, Vector3.forward), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
