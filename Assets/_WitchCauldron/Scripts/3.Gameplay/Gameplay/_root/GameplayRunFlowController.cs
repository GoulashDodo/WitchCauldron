using Core.Data;
using Core.Run;
using Core.SceneManagement;
using Gameplay.Level.SO;

namespace Gameplay._root
{
    public class GameplayRunFlowController
    {
        private readonly RunState _runState;
        private readonly SceneLoader _sceneLoader;
        private readonly LevelSettings _levelSettings;
        private bool _levelCompleted;

        public GameplayRunFlowController(RunState runState, SceneLoader sceneLoader, LevelSettings levelSettings)
        {
            _runState = runState;
            _sceneLoader = sceneLoader;
            _levelSettings = levelSettings;
        }

        public void CompleteLevelAndOpenHut()
        {
            CompleteLevelAndLoadScene(Scenes.Hut);
        }

        public void CompleteLevelAndOpenMainMenu()
        {
            CompleteLevelAndLoadScene(Scenes.MainMenu);
        }

        private void CompleteLevelAndLoadScene(string sceneName)
        {
            if (_levelCompleted)
                return;

            _levelCompleted = true;
            
            _runState.Progress.MarkLevelCompleted(_runState.CurrentLevelIndex);
            _runState.Wallet.Add(_levelSettings.CompletionMoneyReward);
            _runState.ApplyUnlockRewards(_levelSettings.CompletionRewards);
            _runState.TrySetNextLevel();
            _sceneLoader.LoadScene(sceneName);
        }
    }
}
