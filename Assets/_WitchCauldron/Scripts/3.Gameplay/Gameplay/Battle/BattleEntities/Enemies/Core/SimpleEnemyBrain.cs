using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Enemies.Core
{
    public class SimpleEnemyBrain : MonoBehaviour
    {
        private Enemy _enemy;
        private EnemyMotor _motor;
        private EnemyAttack _attack;

        private void Awake()
        {
            _enemy = GetComponent<Enemy>();
            _motor = GetComponent<EnemyMotor>();
            _attack = GetComponent<EnemyAttack>();
        }

        private void FixedUpdate()
        {
            if (!_enemy.IsInitialized || _enemy.IsDead)
                return;

            if (_attack.TryFindTarget(out var target))
            {
                if (_attack.TryStartAttack(target))
                {
                    _enemy.Events.RaiseAttackPerformed();
                }
                return;
            }

            _motor.MoveLeft(Time.fixedDeltaTime);
        }
    }
}
