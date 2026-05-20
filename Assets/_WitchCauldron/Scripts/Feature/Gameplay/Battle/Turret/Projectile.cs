using Feature.Gameplay.Battle.Enemies.Core;
using UnityEngine;

namespace Feature.Gameplay.Battle.Turret
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _hitDistance = 0.1f;
        [SerializeField] private float _lifeTime = 5f;
        [SerializeField] private float _damage = 3;
        [SerializeField] private float _speed = 3;
        
        private Enemy _target;
        private float _destroyTime;

        public void Launch(Enemy target)
        {
            _target = target;
            _destroyTime = Time.time + Mathf.Max(0.01f, _lifeTime);
        }

        private void Update()
        {
            if (Time.time >= _destroyTime)
            {
                Destroy(gameObject);
                return;
            }

            if (_target == null || !_target.gameObject.activeInHierarchy)
            {
                Destroy(gameObject);
                return;
            }

            MoveTowardsTarget();
        }

        private void MoveTowardsTarget()
        {
            var targetPosition = _target.transform.position;
            var nextPosition = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                _speed * Time.deltaTime);

            transform.position = nextPosition;

            var direction = targetPosition - transform.position;
            if (direction.sqrMagnitude > 0.001f)
            {
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            if ((targetPosition - transform.position).sqrMagnitude > _hitDistance * _hitDistance)
                return;

            _target.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}
