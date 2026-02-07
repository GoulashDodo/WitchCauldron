namespace _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Core.Behaviours.Movement
{
    public class MovementController
    {
        private readonly MoveContext _context;
        private IMoveBehaviour _behaviour;

        public MovementController(MoveContext context)
        {
            _context = context;
        }

        public void SetBehaviour(IMoveBehaviour behaviour)
        {
            _behaviour = behaviour;
            _behaviour.Initialize(_context);
        }

        public void Tick(float deltaTime)
        {
            _behaviour?.Tick(deltaTime);
        }

       
    }
}