using Core.SceneManagement;
using Core.UI;
using Gameplay.Battle.Base.Interfaces;
using Gameplay.Battle.Waves.Service;
using Gameplay.Items.Services;
using Gameplay.Level;
using Gameplay.UI.SpawnButtons;
using UnityEngine;
using Zenject;

namespace Gameplay.UI
{
    public class UIGameplayRootBinder : MonoBehaviour
    {
        
        [SerializeField] private UIBaseHealth _uiBaseHealth;
        
        [SerializeField] private UIProgressBar _uiProgressBar;
        [SerializeField] private UIWaveAlert _uiWaveAlert;

        [SerializeField] private UISpawnButtonParent _uiSpawnButtonParent;
        
        [SerializeField] private UILose _uiLose;
        [SerializeField] private UIWin _uiWin;
    
        
        [Inject]
        public void Construct(UIRootView view, IBaseHealthProvider baseHealthProvider, IWaveService waveService, G game,
            SceneLoader sceneLoader, ItemService itemService, SceneParametersPayload payload)
        {
            view.AttachSceneUI(gameObject);
            
            InitializeUI(baseHealthProvider, waveService, game, sceneLoader, itemService, payload);
            
        }


        private void InitializeUI(IBaseHealthProvider baseHealthProvider, IWaveService waveService, G game,
            SceneLoader sceneLoader, ItemService itemService, SceneParametersPayload payload)
        {
            
            _uiBaseHealth.Initialize(baseHealthProvider);
            _uiProgressBar.Initialize(waveService);

            _uiSpawnButtonParent.Initialize(payload.GameplayEntryParameters, itemService);
            
            _uiWaveAlert.Initialize(waveService);
            
            _uiLose.Initialize(game, sceneLoader);
            _uiWin.Initialize(game, sceneLoader);
            
        }
        
        
    }
}
