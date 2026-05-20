using Core.GameRoot.Data;
using Core.GameRoot._root.CompositionRoot.Game;
using Feature.Gameplay.Level;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Feature.Gameplay.UI
{
    public class UILose : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private GameObject _panel;
    
        
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _exitButton;
        
        private SceneLoader _sceneLoader;

        public void Initialize(G game, SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;

            HidePanel();
            SubscribeToButtons();

            game.GameLost
                .Subscribe(_ => ShowPanel())
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            UnsubscribeFromButtons();
            _disposables.Dispose();
        }

        private void ShowPanel()
        {

            _panel.SetActive(true);
        }

        private void HidePanel()
        {
            _panel.SetActive(false);
        }

        private void SubscribeToButtons()
        {
            UnsubscribeFromButtons();

            _retryButton.onClick.AddListener(RestartGameplay);
            _exitButton.onClick.AddListener(ExitToMainMenu);
        }

        private void UnsubscribeFromButtons()
        {
            _retryButton.onClick.RemoveListener(RestartGameplay);
            _exitButton.onClick.RemoveListener(ExitToMainMenu);
        }

        private void RestartGameplay()
        {
            _sceneLoader.LoadScene(Scenes.Gameplay);
        }

        private void ExitToMainMenu()
        {
            _sceneLoader.LoadScene(Scenes.MainMenu);
        }
    }
}
