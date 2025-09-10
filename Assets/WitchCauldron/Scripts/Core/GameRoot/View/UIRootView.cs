using R3;
using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.Root;

namespace WitchCauldron.Scripts.Core.GameRoot.View
{
    public class UIRootView : MonoBehaviour
    {
        
        [SerializeField] private UILoadingScreenBinder _loadingScreen;
        [SerializeField] private Transform _uiSceneContainer;

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

        public void AttachSceneUI(GameObject sceneUI)
        {
            ClearSceneUI();

            sceneUI.transform.SetParent(_uiSceneContainer, false);
        }

        private void ClearSceneUI()
        {
            var childCount = _uiSceneContainer.childCount;
            for (var i = 0; i < childCount; i++)
            {
                Destroy(_uiSceneContainer.GetChild(i).gameObject);
            }
        }
        
        
    }
}