using Feature.Gameplay.Battle.HealthSystem.Structs;

namespace Feature.Gameplay.Battle.HealthSystem
{
    public interface IDamageable
    {
            
        void TakeDamage(BattleDamage battleDamage);
    }
}
