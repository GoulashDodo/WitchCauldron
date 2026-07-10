using Gameplay.Battle.HealthSystem.Structs;

namespace Gameplay.Battle.HealthSystem
{
    public interface IDamageable
    {
            
        void TakeDamage(BattleDamage battleDamage);
    }
}
