using Core.GameRoot._root.CompositionRoot.Game;
using Core.GameRoot.View;
using Feature.Gameplay.Battle.Base.Interfaces;
using Feature.Gameplay.Battle.Waves.Service;
using Feature.Gameplay.Level;
using UnityEngine;
using Zenject;

namespace Feature.Gameplay.UI
{
    public class UIGameplayRootBinder : MonoBehaviour
    {
        
        [SerializeField] private UIBaseHealth _uiBaseHealth;
        
        [SerializeField] private UIProgressBar _uiProgressBar;

        [SerializeField] private UILose _uiLose;

        [SerializeField] private UIWin _uiWin;
    
        
        [Inject]
        public void Construct(UIRootView view, IBaseHealthProvider baseHealthProvider, IWaveService waveService, G game, SceneLoader sceneLoader)
        {
            view.AttachSceneUI(gameObject);
            
            InitializeUI(baseHealthProvider, waveService, game, sceneLoader);
            
        }


        private void InitializeUI(IBaseHealthProvider baseHealthProvider, IWaveService waveService, G game, SceneLoader sceneLoader)
        {
            
            _uiBaseHealth.Initialize(baseHealthProvider);
            _uiProgressBar.Initialize(waveService);
            _uiLose.Initialize(game, sceneLoader);
            _uiWin.Initialize(game, sceneLoader);
            
        }
        
        
    }
}
