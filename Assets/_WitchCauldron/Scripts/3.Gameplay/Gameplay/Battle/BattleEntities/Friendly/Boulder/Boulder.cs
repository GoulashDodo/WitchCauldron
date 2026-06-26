using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.HealthSystem.Structs;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Friendly.Boulder
{
    public class Boulder : MonoBehaviour
    {

        [SerializeField] private float _damage = 10;
        [SerializeField] private DamageType _damageType = DamageType.Physical;

        [SerializeField] private float _speed = 10f;

        private void Update()
        {
            transform.Translate(Vector3.right * (_speed * Time.deltaTime));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Enemy enemy))
            {
                var damage = new BattleDamage(_damage, _damageType);
                enemy.Health.TakeDamage(damage);
            }
        }
        
    }
}
