using Gameplay.Battle.HealthSystem.Structs;
using R3;

namespace Gameplay.Battle.HealthSystem.Core
{
    public interface IHealth : IDamageable
    {
        float CurrentHealthValue { get; }
        float MaxHealth { get; }
        
        Observable<float> CurrentHealth { get; }
        Observable<DamageInfo> Damaged { get; }
        Observable<DeathInfo> Died { get; }
    }
}
