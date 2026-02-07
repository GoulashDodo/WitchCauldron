namespace _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Core.Behaviours.Movement
{
    public interface IMoveBehaviour
    {
        void Initialize(MoveContext context);
        void Tick(float deltaTime);
    }
}