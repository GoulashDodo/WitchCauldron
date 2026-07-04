using Core.Audio;
using Core.Run;
using Core.SceneManagement;
using Core.UI;
using Gameplay._root;
using Gameplay._root.SO;
using Gameplay.Battle.Base.Interfaces;
using Gameplay.Battle.Waves.Service;
using Gameplay.Level;
using Gameplay.Level.SO;
using Gameplay.Rewards;
using Hut.UI.UIAlmanac;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.UI
{
    public class UIGameplayRootBinder : MonoBehaviour
    {
        
        [SerializeField] private UIBaseHealth _uiBaseHealth;
        
        [SerializeField] private UIProgressBar _uiProgressBar;
        [SerializeField] private UIWaveAlert _uiWaveAlert;

        
        
        [SerializeField] private UILose _uiLose;
        [SerializeField] private UIWin _uiWin;
        [SerializeField] private UIAlmanacRoot _almanacRoot;
        [SerializeField] private Button _showAlmanacButton;

        private void OnDestroy()
        {
            if (_showAlmanacButton != null && _almanacRoot != null)
                _showAlmanacButton.onClick.RemoveListener(_almanacRoot.Show);
        }
    
        
        [Inject]
        public void Construct(UIRootView view, IBaseHealthProvider baseHealthProvider, IWaveService waveService, G game,
            SceneLoader sceneLoader,
            RunState runState,
            GameplayRunFlowController runFlowController,
            GameplayPauseService pauseService,
            LevelSettings levelSettings,
            GameplaySettings gameplaySettings,
            AudioService audioService,
            VictoryRewardCalculator rewardCalculator)
        {
            view.AttachSceneUI(gameObject);
            
            InitializeUI(baseHealthProvider, waveService, game, sceneLoader, runState, runFlowController, pauseService, levelSettings, gameplaySettings, audioService, rewardCalculator);
            
        }


        private void InitializeUI(IBaseHealthProvider baseHealthProvider, IWaveService waveService, G game,
            SceneLoader sceneLoader,
            RunState runState,
            GameplayRunFlowController runFlowController,
            GameplayPauseService pauseService,
            LevelSettings levelSettings,
            GameplaySettings gameplaySettings,
            AudioService audioService,
            VictoryRewardCalculator rewardCalculator)
        {
            
            _uiBaseHealth.Initialize(baseHealthProvider);
            _uiProgressBar.Initialize(waveService);

            
            _uiWaveAlert.Initialize(waveService);
            
            _uiLose.Initialize(game, sceneLoader, pauseService, audioService);
            _uiWin.Initialize(game, runFlowController, pauseService, levelSettings, gameplaySettings, audioService, baseHealthProvider, waveService, rewardCalculator);

            if (_almanacRoot != null)
                _almanacRoot.Initialize(gameplaySettings, runState);

            if (_showAlmanacButton != null && _almanacRoot != null)
                _showAlmanacButton.onClick.AddListener(_almanacRoot.Show);
            
        }
        
        
    }
}
