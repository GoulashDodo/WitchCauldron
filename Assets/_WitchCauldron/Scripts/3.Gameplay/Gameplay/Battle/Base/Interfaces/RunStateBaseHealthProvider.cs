using Core.Run;
using Gameplay.Battle.HealthSystem.Core;

namespace Gameplay.Battle.Base.Interfaces
{
    public class RunStateBaseHealthProvider : IBaseHealthProvider
    {
        private readonly Health _baseHealth;

        public RunStateBaseHealthProvider(RunState runState)
        {
            _baseHealth = new Health(runState.BaseHealth.MaxHealth);
        }

        public IHealth GetBaseHealth()
        {
            return _baseHealth;
        }
    }
}
