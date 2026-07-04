using System.Collections;
using System.Collections.Generic;
using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.BattleEntities.Friendly.Core;
using Gameplay.Battle.HealthSystem.Structs;
using Gameplay.Items.Usable.Commands.Preview;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Friendly.Bombs
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(FriendlyAttackableEntity))]
    [RequireComponent(typeof(BombPulseFx))]
    public sealed class Bomb : MonoBehaviour, IPlacementRadiusPreviewSource
    {
        [SerializeField] private Transform _explosionOrigin;
        [SerializeField] private float _explosionDelay = 2f;
        [SerializeField] private float _explosionRadius = 2.5f;
        [SerializeField] private float _damage = 25f;
        [SerializeField] private DamageType _damageType = DamageType.Fire;

        private readonly Collider2D[] _overlapBuffer = new Collider2D[64];
        private readonly HashSet<Enemy> _damagedEnemies = new();

        private ContactFilter2D _contactFilter;
        private Coroutine _explodeCoroutine;
        private float _explodeStartedAt;
        private float _explodeAt;
        private bool _exploded;

        public float PreviewRadius => Mathf.Max(0f, _explosionRadius);
        public Transform PreviewOrigin => _explosionOrigin ? _explosionOrigin : transform;
        public float ExplosionDelay => Mathf.Max(0f, _explosionDelay);
        public float FuseProgress => ExplosionDelay <= 0f
            ? 1f
            : Mathf.Clamp01((Time.time - _explodeStartedAt) / ExplosionDelay);

        private void Awake()
        {
            _contactFilter = new ContactFilter2D
            {
                useTriggers = true
            };
        }

        private void OnEnable()
        {
            _exploded = false;
            _explodeStartedAt = Time.time;
            _explodeAt = _explodeStartedAt + ExplosionDelay;
            _explodeCoroutine = StartCoroutine(ExplodeAfterDelay());
        }

        private void OnDisable()
        {
            if (_explodeCoroutine == null)
                return;

            StopCoroutine(_explodeCoroutine);
            _explodeCoroutine = null;
        }

        private IEnumerator ExplodeAfterDelay()
        {
            var delay = Mathf.Max(0f, _explodeAt - Time.time);
            yield return new WaitForSeconds(delay);
            Explode();
        }

        private void Explode()
        {
            if (_exploded)
                return;

            _exploded = true;
            _explodeCoroutine = null;

            var origin = PreviewOrigin.position;
            var radius = Mathf.Max(0f, _explosionRadius);
            var count = Physics2D.OverlapCircle(origin, radius, _contactFilter, _overlapBuffer);
            var damageAmount = Mathf.Max(0f, _damage);

            _damagedEnemies.Clear();

            for (var i = 0; i < count; i++)
            {
                var hit = _overlapBuffer[i];
                if (hit == null)
                    continue;

                var enemy = hit.GetComponentInParent<Enemy>();
                if (enemy == null || enemy.IsDead || !_damagedEnemies.Add(enemy))
                    continue;

                if (damageAmount > 0f)
                    enemy.TakeDamage(new BattleDamage(damageAmount, _damageType));
            }

            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(
                _explosionOrigin ? _explosionOrigin.position : transform.position,
                Mathf.Max(0f, _explosionRadius));
        }
    }
}
