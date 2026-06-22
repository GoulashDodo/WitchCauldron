using System;
using Core.Data;
using Core.SceneManagement;
using Gameplay._root;
using Gameplay.Level;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class UILose : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private GameObject _panel;
    
        
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _exitButton;
        
        private SceneLoader _sceneLoader;
        private GameplayPauseService _pauseService;
        private IDisposable _pauseHandle;

        public void Initialize(G game, SceneLoader sceneLoader, GameplayPauseService pauseService)
        {
            _sceneLoader = sceneLoader;
            _pauseService = pauseService;

            HidePanel();
            SubscribeToButtons();

            game.GameLost
                .Subscribe(_ => ShowPanel())
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            ReleasePause();
            UnsubscribeFromButtons();
            _disposables.Dispose();
        }

        private void ShowPanel()
        {
            RequestPause();

            _panel.SetActive(true);
        }

        private void HidePanel()
        {
            _panel.SetActive(false);
            ReleasePause();
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
            ReleasePause();
            _sceneLoader.LoadScene(Scenes.Gameplay);
        }

        private void ExitToMainMenu()
        {
            ReleasePause();
            _sceneLoader.LoadScene(Scenes.MainMenu);
        }

        private void RequestPause()
        {
            if (_pauseService == null || _pauseHandle != null)
                return;

            _pauseHandle = _pauseService.RequestPause();
        }

        private void ReleasePause()
        {
            _pauseHandle?.Dispose();
            _pauseHandle = null;
        }
    }
}
