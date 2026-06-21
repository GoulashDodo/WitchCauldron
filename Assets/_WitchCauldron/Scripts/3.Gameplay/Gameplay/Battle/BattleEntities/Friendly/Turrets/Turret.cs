using Core.Data;
using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.HealthSystem;
using Gameplay.Battle.HealthSystem.Core;
using Gameplay.Battle.HealthSystem.Structs;
using R3;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Friendly.Turrets
{
    public class Turret : MonoBehaviour, IDamageable, IEnemyAttackTarget
    {
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private Transform _shootingPoint;
        
        [SerializeField] private float _maxHealth = 10f;
        [SerializeField] private float _attackRange = 5f;
        [SerializeField] private float _attackCooldown = 1f;
        
        private readonly Collider2D[] _overlapBuffer = new Collider2D[32];
        private readonly CompositeDisposable _disposables = new();
        
        private ContactFilter2D _contactFilter;
        private Health _health;
        private float _nextAttackTime;
        private bool _isDead;

        public IHealth Health => _health;
        public IDamageable Damageable => this;

        private void Awake()
        {
            _health = new Health(Mathf.Max(1f, _maxHealth));
            _health.Died
                .Subscribe(_ => Die())
                .AddTo(_disposables);
            
            EnsureEnemyAttackRaycastTarget();
            
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
            if (_isDead)
                return;
            
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
            projectile.Launch(target);
        }

        public void TakeDamage(BattleDamage battleDamage)
        {
            if (_isDead)
                return;
            
            _health.TakeDamage(battleDamage);
        }

        private void Die()
        {
            if (_isDead)
                return;

            _isDead = true;
            Destroy(gameObject);
        }

        private void EnsureEnemyAttackRaycastTarget()
        {
            if (!TryGetComponent<Collider2D>(out _))
            {
                var attackCollider = gameObject.AddComponent<BoxCollider2D>();
                attackCollider.isTrigger = true;
            }

            var baseLayer = LayerMask.NameToLayer(Layers.Base);
            if (baseLayer >= 0 && gameObject.layer == 0)
            {
                gameObject.layer = baseLayer;
            }
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            _health?.Dispose();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(_shootingPoint ? _shootingPoint.position : transform.position,
                Mathf.Max(0f, _attackRange));
        }
    }
}
