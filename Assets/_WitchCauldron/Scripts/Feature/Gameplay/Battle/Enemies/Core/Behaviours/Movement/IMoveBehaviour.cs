namespace Feature.Gameplay.Battle.Enemies.Core.Behaviours.Movement
{
    public interface IMoveBehaviour
    {
        void Initialize(MoveContext context);
        void Tick(float deltaTime);
    }
}