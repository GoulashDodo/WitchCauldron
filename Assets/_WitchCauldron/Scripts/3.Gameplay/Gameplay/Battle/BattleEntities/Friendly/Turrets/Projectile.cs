using System.Collections.Generic;
using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.Effects;
using Gameplay.Battle.Effects.Base;
using Gameplay.Battle.HealthSystem.Structs;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Friendly.Turrets
{
    public class Projectile : MonoBehaviour
    {
        private readonly Collider2D[] _overlapBuffer = new Collider2D[16];
        private readonly Collider2D[] _targetBuffer = new Collider2D[32];
        private readonly HashSet<Enemy> _hitEnemies = new();

        [Header("Hit")]
        [SerializeField] private float _hitDistance = 0.1f;
        [SerializeField] private float _lifeTime = 5f;
        [SerializeField] private float _damage = 3;
        [SerializeField] private DamageType _damageType = DamageType.Physical;
        [SerializeField] private EffectData _effectOnHit;
        [SerializeField] private GameObject _impactParticlePrefab;

        [Header("Movement")]
        [SerializeField] private float _speed = 3;

        [Header("Ricochet")]
        [SerializeField] private int _bounceCount;
        [SerializeField] private float _bounceRange = 3f;
        [SerializeField] private bool _canHitSameEnemy;
        
        private ContactFilter2D _contactFilter;
        private Vector3 _direction;
        private float _destroyTime;
        private int _remainingBounces;
        private Enemy _lastHitEnemy;
        private float _lastHitEnemyIgnoreTime;

        private void Awake()
        {
            _contactFilter = new ContactFilter2D
            {
                useTriggers = true
            };
        }

        public void Launch(Vector3 direction)
        {
            _direction = direction.sqrMagnitude > 0.001f ? direction.normalized : transform.right;
            _destroyTime = Time.time + Mathf.Max(0.01f, _lifeTime);
            _remainingBounces = Mathf.Max(0, _bounceCount);
            _hitEnemies.Clear();
            _lastHitEnemy = null;
            _lastHitEnemyIgnoreTime = 0f;

            RotateToDirection();
        }

        private void Update()
        {
            if (Time.time >= _destroyTime)
            {
                Destroy(gameObject);
                return;
            }

            MoveForward();
            TryHitEnemy();
        }

        private void MoveForward()
        {
            transform.position += _direction * (_speed * Time.deltaTime);
        }

        private void TryHitEnemy()
        {
            var count = Physics2D.OverlapCircle(transform.position, Mathf.Max(0f, _hitDistance), _contactFilter, _overlapBuffer);

            for (var i = 0; i < count; i++)
            {
                var hit = _overlapBuffer[i];
                if (hit == null)
                    continue;

                var enemy = hit.GetComponentInParent<Enemy>();
                if (!IsValidTarget(enemy))
                    continue;

                if (enemy == _lastHitEnemy && Time.time < _lastHitEnemyIgnoreTime)
                    continue;

                if (!_canHitSameEnemy && _hitEnemies.Contains(enemy))
                    continue;

                Hit(enemy);
                return;
            }
        }

        private void Hit(Enemy enemy)
        {
            var hitPosition = transform.position;

            enemy.TakeDamage(new BattleDamage(_damage, _damageType));
            ApplyEffect(enemy);
            PlayImpactParticles(hitPosition);

            _hitEnemies.Add(enemy);
            _lastHitEnemy = enemy;
            _lastHitEnemyIgnoreTime = Time.time + 0.08f;

            if (_remainingBounces <= 0 || !TryBounce(hitPosition, enemy))
            {
                Destroy(gameObject);
                return;
            }

            _remainingBounces--;
        }

        private void ApplyEffect(Enemy enemy)
        {
            if (_effectOnHit == null)
                return;

            var receiver = enemy.GetComponentInParent<EffectReceiver>();
            receiver?.AddEffect(_effectOnHit);
        }

        private void PlayImpactParticles(Vector3 position)
        {
            if (_impactParticlePrefab == null)
                return;

            Instantiate(_impactParticlePrefab, position, Quaternion.identity);
        }

        private bool TryBounce(Vector3 hitPosition, Enemy currentEnemy)
        {
            if (!TryFindNextTarget(hitPosition, currentEnemy, out var nextTarget))
                return false;

            _direction = nextTarget.transform.position - hitPosition;
            if (_direction.sqrMagnitude <= 0.001f)
                return false;

            _direction.Normalize();
            RotateToDirection();
            return true;
        }

        private bool TryFindNextTarget(Vector3 position, Enemy currentEnemy, out Enemy target)
        {
            target = null;

            var count = Physics2D.OverlapCircle(
                position,
                Mathf.Max(0f, _bounceRange),
                _contactFilter,
                _targetBuffer);

            var bestSqrDistance = float.MaxValue;

            for (var i = 0; i < count; i++)
            {
                var hit = _targetBuffer[i];
                if (hit == null)
                    continue;

                var enemy = hit.GetComponentInParent<Enemy>();
                if (!IsValidTarget(enemy) || enemy == currentEnemy)
                    continue;

                if (!_canHitSameEnemy && _hitEnemies.Contains(enemy))
                    continue;

                var sqrDistance = (enemy.transform.position - position).sqrMagnitude;
                if (sqrDistance >= bestSqrDistance)
                    continue;

                bestSqrDistance = sqrDistance;
                target = enemy;
            }

            return target != null;
        }

        private void RotateToDirection()
        {
            var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private static bool IsValidTarget(Enemy enemy)
        {
            return enemy != null && !enemy.IsDead && enemy.gameObject.activeInHierarchy;
        }
    }
}
