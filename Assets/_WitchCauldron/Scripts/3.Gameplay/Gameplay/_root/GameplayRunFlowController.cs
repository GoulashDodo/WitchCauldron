using Core.Audio;
using Core.Data;
using Core.Run;
using Core.SceneManagement;
using Gameplay.Level.SO;
using Gameplay.Rewards;

namespace Gameplay._root
{
    public class GameplayRunFlowController
    {
        private readonly RunState _runState;
        private readonly SceneLoader _sceneLoader;
        private readonly LevelSettings _levelSettings;
        private readonly AudioService _audioService;
        private bool _levelCompleted;

        public GameplayRunFlowController(
            RunState runState,
            SceneLoader sceneLoader,
            LevelSettings levelSettings,
            AudioService audioService)
        {
            _runState = runState;
            _sceneLoader = sceneLoader;
            _levelSettings = levelSettings;
            _audioService = audioService;
        }

        public void CompleteLevelAndOpenHut(VictoryRewardBreakdown rewards)
        {
            CompleteLevelAndLoadScene(Scenes.Hut, rewards);
        }

        public void CompleteLevelAndOpenMainMenu(VictoryRewardBreakdown rewards)
        {
            CompleteLevelAndLoadScene(Scenes.MainMenu, rewards);
        }

        private void CompleteLevelAndLoadScene(string sceneName, VictoryRewardBreakdown rewards)
        {
            if (_levelCompleted)
                return;

            _levelCompleted = true;
            rewards = EnsureRewards(rewards);
            
            _runState.Progress.MarkLevelCompleted(_runState.CurrentLevelIndex);
            _runState.Wallet.Add(rewards.TotalMoney);
            _runState.ApplyUnlockRewards(rewards.UnlockRewards);
            PlayUnlockRewardAudio(rewards.UnlockRewards);
            _runState.TrySetNextLevel();
            _sceneLoader.LoadScene(sceneName);
        }

        private VictoryRewardBreakdown EnsureRewards(VictoryRewardBreakdown rewards)
        {
            if (rewards.TotalMoney > 0 || rewards.UnlockRewards != null)
                return rewards;

            return new VictoryRewardBreakdown(
                _levelSettings != null ? _levelSettings.CompletionMoneyReward : 0,
                0,
                0,
                _levelSettings != null ? _levelSettings.CompletionRewards : null);
        }

        private void PlayUnlockRewardAudio(UnlockReward[] rewards)
        {
            if (rewards == null)
                return;

            foreach (var reward in rewards)
            {
                switch (reward.Type)
                {
                    case UnlockRewardType.SelectableItem:
                        _audioService.PlayUi(AudioId.Unlock_Item);
                        break;
                    case UnlockRewardType.Recipe:
                        _audioService.PlayUi(AudioId.Unlock_Recipe);
                        break;
                }
            }
        }
    }
}
