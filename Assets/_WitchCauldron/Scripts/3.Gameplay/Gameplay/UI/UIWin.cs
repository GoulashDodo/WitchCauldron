using System;
using Gameplay._root;
using Gameplay._root.SO;
using Gameplay.Level;
using Gameplay.Level.SO;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class UIWin : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private GameObject _panel;
    
        
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private UIRewardList _rewardList;
        
        private GameplayRunFlowController _runFlowController;
        private GameplayPauseService _pauseService;
        private IDisposable _pauseHandle;
        private LevelSettings _levelSettings;
        private GameplaySettings _gameplaySettings;

        public void Initialize(
            G game,
            GameplayRunFlowController runFlowController,
            GameplayPauseService pauseService,
            LevelSettings levelSettings,
            GameplaySettings gameplaySettings)
        {
            _runFlowController = runFlowController;
            _pauseService = pauseService;
            _levelSettings = levelSettings;
            _gameplaySettings = gameplaySettings;

            HidePanel();
            SubscribeToButtons();

            game.GameWon
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

            if (_rewardList != null)
                _rewardList.Initialize(_levelSettings.CompletionRewards, _gameplaySettings);

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


            if (_continueButton != null)
                _continueButton.onClick.AddListener(ContinueToHut);

            if (_exitButton != null)
                _exitButton.onClick.AddListener(ExitToMainMenu);
        }

        private void UnsubscribeFromButtons()
        {

            if (_continueButton != null)
                _continueButton.onClick.RemoveListener(ContinueToHut);

            if (_exitButton != null)
            {
                _exitButton.onClick.RemoveListener(ExitToMainMenu);
            }
        }

        private void ExitToMainMenu()
        {
            ReleasePause();
            _runFlowController.CompleteLevelAndOpenMainMenu();
        }

        private void ContinueToHut()
        {
            ReleasePause();
            _runFlowController.CompleteLevelAndOpenHut();
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
