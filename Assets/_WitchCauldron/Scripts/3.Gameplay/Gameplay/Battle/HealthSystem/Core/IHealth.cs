using Gameplay.Battle.HealthSystem.Structs;
using R3;

namespace Gameplay.Battle.HealthSystem.Core
{
    public interface IHealth : IDamageable
    {
        Observable<float> CurrentHealth { get; }
        Observable<DamageInfo> Damaged { get; }
        Observable<DeathInfo> Died { get; }
    }
}
