using Core.Run;
using Core.SceneManagement;
using Core.UI;
using Gameplay._root;
using Gameplay._root.SO;
using Gameplay.Battle.Base.Interfaces;
using Gameplay.Battle.Waves.Service;
using Gameplay.Level;
using Gameplay.Level.SO;
using Gameplay.UI.Recipes;
using UnityEngine;
using Zenject;

namespace Gameplay.UI
{
    public class UIGameplayRootBinder : MonoBehaviour
    {
        
        [SerializeField] private UIBaseHealth _uiBaseHealth;
        
        [SerializeField] private UIProgressBar _uiProgressBar;
        [SerializeField] private UIWaveAlert _uiWaveAlert;

        [SerializeField] private UIReceiptParent _uiReceiptPrent;
        
        
        [SerializeField] private UILose _uiLose;
        [SerializeField] private UIWin _uiWin;
    
        
        [Inject]
        public void Construct(UIRootView view, IBaseHealthProvider baseHealthProvider, IWaveService waveService, G game,
            SceneLoader sceneLoader,
            RunState runState,
            GameplayRunFlowController runFlowController,
            GameplayPauseService pauseService,
            LevelSettings levelSettings,
            GameplaySettings gameplaySettings)
        {
            view.AttachSceneUI(gameObject);
            
            InitializeUI(baseHealthProvider, waveService, game, sceneLoader, runState, runFlowController, pauseService, levelSettings, gameplaySettings);
            
        }


        private void InitializeUI(IBaseHealthProvider baseHealthProvider, IWaveService waveService, G game,
            SceneLoader sceneLoader,
            RunState runState,
            GameplayRunFlowController runFlowController,
            GameplayPauseService pauseService,
            LevelSettings levelSettings,
            GameplaySettings gameplaySettings)
        {
            
            _uiBaseHealth.Initialize(baseHealthProvider);
            _uiProgressBar.Initialize(waveService);

            _uiReceiptPrent.Initialize(runState);
            
            _uiWaveAlert.Initialize(waveService);
            
            _uiLose.Initialize(game, sceneLoader, pauseService);
            _uiWin.Initialize(game, runFlowController, pauseService, levelSettings, gameplaySettings);
            
        }
        
        
    }
}
