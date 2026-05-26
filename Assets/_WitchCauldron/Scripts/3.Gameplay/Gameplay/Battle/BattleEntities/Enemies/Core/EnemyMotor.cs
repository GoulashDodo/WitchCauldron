using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Enemies.Core
{
    
    [RequireComponent(typeof(Enemy))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMotor : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;
        private Enemy _enemy;

        public float SpeedMultiplier { get; private set; } = 1f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _enemy = GetComponent<Enemy>();
        }


        public void MoveLeft(float deltaTime)
        {
            if (_enemy.Settings == null)
                return;

            var speed = _enemy.Settings.MaxSpeed * SpeedMultiplier;
            var nextPosition = _rigidbody.position + Vector2.left * (speed * deltaTime);
            _rigidbody.MovePosition(nextPosition);
        }

        public void SetSpeedMultiplier(float value)
        {
            SpeedMultiplier = Mathf.Max(0f, value);
        }
        
    }
}
