using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.HealthSystem.Structs;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Friendly.Turrets
{
    public class Projectile : MonoBehaviour
    {
        private readonly Collider2D[] _overlapBuffer = new Collider2D[8];

        [SerializeField] private float _hitDistance = 0.1f;
        [SerializeField] private float _lifeTime = 5f;
        [SerializeField] private float _damage = 3;
        [SerializeField] private float _speed = 3;
        
        private ContactFilter2D _contactFilter;
        private Vector3 _direction;
        private float _destroyTime;

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

            var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
                if (enemy == null || !enemy.gameObject.activeInHierarchy)
                    continue;

                enemy.TakeDamage(new BattleDamage(_damage, DamageType.Physical));
                Destroy(gameObject);
                return;
            }
        }
    }
}
