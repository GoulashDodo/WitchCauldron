using Core.Run;
using Gameplay.Battle.HealthSystem.Core;
using Gameplay.Level.SO;

namespace Gameplay.Battle.Base.Interfaces
{
    public class LevelSettingsBaseHealthProvider : IBaseHealthProvider
    {
        
        private readonly Health _baseHealth;

        public LevelSettingsBaseHealthProvider(LevelSettings levelSettings, RunState runState)
        {
            var additionalMaxHealth = runState != null ? runState.BaseUpgrades.AdditionalMaxHealth : 0f;
            _baseHealth = new Health(levelSettings.BaseHealth + additionalMaxHealth);
        }
        
        
        public IHealth GetBaseHealth()
        {
            return _baseHealth;
        }
    }
}
