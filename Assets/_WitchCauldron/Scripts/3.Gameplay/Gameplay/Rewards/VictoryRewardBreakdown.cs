using Core.Run;

namespace Gameplay.Rewards
{
    public readonly struct VictoryRewardBreakdown
    {
        public int LevelReward { get; }
        public int BaseHealthBonus { get; }
        public int TimeBonus { get; }
        public int TotalMoney { get; }
        public UnlockReward[] UnlockRewards { get; }

        public VictoryRewardBreakdown(
            int levelReward,
            int baseHealthBonus,
            int timeBonus,
            UnlockReward[] unlockRewards)
        {
            LevelReward = levelReward;
            BaseHealthBonus = baseHealthBonus;
            TimeBonus = timeBonus;
            TotalMoney = levelReward + baseHealthBonus + timeBonus;
            UnlockRewards = unlockRewards;
        }
    }
}
