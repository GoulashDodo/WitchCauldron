using R3;

namespace WitchCauldron.Scripts.Feature.Gameplay.HealthSystem
{
    public interface IDamagable
    {
        Observable<int> TakenDamage { get; }
            
        void TakeDamage(int damage);
    }
}