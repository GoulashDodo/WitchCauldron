using Core.Audio;
using Core.SceneManagement;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class UIRootView : MonoBehaviour
    {
        
        [SerializeField] private UILoadingScreenBinder _loadingScreen;
        [SerializeField] private Transform _uiSceneContainer;

        private AudioService _audioService;
        private SceneLoader _sceneLoader;

        public void Initialize(SceneLoader sceneLoader, AudioService audioService)
        {
            Debug.Log("Initializing UIRootView");
            _audioService = audioService;
            _sceneLoader = sceneLoader;
            sceneLoader.OnSceneLoadingStarted.Subscribe(_ => EnableLoadingScreen());
            sceneLoader.OnSceneLoadingEnded.Subscribe(_ => DisableLoadingScreen());
        }
        
        private void Awake()
        {
            _loadingScreen.gameObject.SetActive(true);
            _loadingScreen.HideImmediate();
        }
        
        private void EnableLoadingScreen()
        {
            _loadingScreen.Show(_sceneLoader.NotifyLoadingScreenFadeInCompleted);
        }

        private void DisableLoadingScreen()
        {
            _loadingScreen.Hide();
        }

        public void AttachSceneUI(GameObject sceneUI)
        {
            ClearSceneUI();

            sceneUI.transform.SetParent(_uiSceneContainer, false);
            AttachButtonClickAudio(sceneUI);
        }

        private void AttachButtonClickAudio(GameObject sceneUI)
        {
            if (_audioService == null || sceneUI == null)
                return;

            var buttons = sceneUI.GetComponentsInChildren<Button>(true);
            foreach (var button in buttons)
            {
                button.onClick.RemoveListener(PlayClickSound);
                button.onClick.AddListener(PlayClickSound);
            }
        }

        private void PlayClickSound()
        {
            _audioService?.PlayUi(AudioId.UI_Click);
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
