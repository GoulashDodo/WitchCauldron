using Gameplay.Battle.HealthSystem.Core;
using Gameplay.Level.SO;
using UnityEngine;

namespace Gameplay.Rewards
{
    public class VictoryRewardCalculator
    {
        public VictoryRewardBreakdown Calculate(
            LevelSettings levelSettings,
            IHealth baseHealth,
            float completionTime)
        {
            if (levelSettings == null)
                return default;

            var levelReward = Mathf.Max(0, levelSettings.CompletionMoneyReward);
            var baseHealthBonus = CalculateBaseHealthBonus(levelSettings, baseHealth);
            var timeBonus = CalculateTimeBonus(levelSettings, completionTime);

            return new VictoryRewardBreakdown(
                levelReward,
                baseHealthBonus,
                timeBonus,
                levelSettings.CompletionRewards);
        }

        private static int CalculateBaseHealthBonus(LevelSettings levelSettings, IHealth baseHealth)
        {
            var maxBonus = Mathf.Max(0, levelSettings.MaxBaseHealthBonus);

            if (maxBonus <= 0 || baseHealth == null || baseHealth.MaxHealth <= 0f)
                return 0;

            var health01 = Mathf.Clamp01(baseHealth.CurrentHealthValue / baseHealth.MaxHealth);
            return Mathf.RoundToInt(maxBonus * health01);
        }

        private static int CalculateTimeBonus(LevelSettings levelSettings, float completionTime)
        {
            var maxBonus = Mathf.Max(0, levelSettings.MaxTimeBonus);
            var targetTime = Mathf.Max(0f, levelSettings.TargetCompletionTime);

            if (maxBonus <= 0 || targetTime <= 0f || completionTime <= 0f)
                return 0;

            var overtime = Mathf.Max(0f, completionTime - targetTime);
            var bonus01 = Mathf.Clamp01(1f - overtime / targetTime);

            return Mathf.RoundToInt(maxBonus * bonus01);
        }
    }
}
