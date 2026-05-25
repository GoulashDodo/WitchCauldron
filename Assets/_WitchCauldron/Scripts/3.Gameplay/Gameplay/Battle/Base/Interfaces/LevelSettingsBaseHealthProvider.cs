using Gameplay.Battle.HealthSystem.Core;
using Gameplay.Level.SO;

namespace Gameplay.Battle.Base.Interfaces
{
    public class LevelSettingsBaseHealthProvider : IBaseHealthProvider
    {
        
        private readonly Health _baseHealth;

        public LevelSettingsBaseHealthProvider(LevelSettings levelSettings)
        {
            _baseHealth = new Health(levelSettings.BaseHealth);
        }
        
        
        public IHealth GetBaseHealth()
        {
            return _baseHealth;
        }
    }
}