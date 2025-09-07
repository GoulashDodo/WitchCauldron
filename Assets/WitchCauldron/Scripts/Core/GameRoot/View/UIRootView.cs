using R3;
using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.Root;

namespace WitchCauldron.Scripts.Core.GameRoot.View
{
    public class UIRootView : MonoBehaviour
    {
        
        [SerializeField] private UILoadingScreenBinder _loadingScreen;
        
        public void Initialize(SceneLoader sceneLoader)
        {
            sceneLoader.OnSceneLoadingStarted.Subscribe(_ => EnableLoadingScreen());
            sceneLoader.OnSceneLoadingEnded.Subscribe(_ => DisableLoadingScreen());
            
        }
        
        
        
        private void Awake()
        {
            DisableLoadingScreen();
        }
        
        private void EnableLoadingScreen()
        {
            _loadingScreen.gameObject.SetActive(true);
        }

        private void DisableLoadingScreen()
        {
            _loadingScreen.gameObject.SetActive(false);
        }
        
    }
}