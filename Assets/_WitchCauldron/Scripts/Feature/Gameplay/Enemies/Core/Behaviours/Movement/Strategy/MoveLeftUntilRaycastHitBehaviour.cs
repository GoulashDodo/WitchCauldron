using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Core.Behaviours.Movement.Strategy
{
    public class MoveLeftUntilRaycastHitBehaviour : IMoveBehaviour
    {
        private MoveContext _context;

        public void Initialize(MoveContext context)
        {
            _context = context;
        }

        public void Tick(float deltaTime)
        {
            if (_context == null)
                return;

            if (IsStopConditionMet())
                return;

            MoveLeft(deltaTime);
        }

        private bool IsStopConditionMet()
        {
            var hit = Physics2D.Raycast(
                origin: _context.Transform.position,
                direction: Vector2.left,
                distance: _context.StopDistance,
                layerMask: _context.StopLayerMask);

            return hit.collider != null;
        }

        private void MoveLeft(float deltaTime)
        {
            var nextPosition = _context.Rigidbody.position + Vector2.left * (_context.Speed * deltaTime);
            _context.Rigidbody.MovePosition(nextPosition);
        }
    }
}