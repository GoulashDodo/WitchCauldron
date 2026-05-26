using Core.Run;
using Core.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hut.UI
{
    public class UINextLevel : MonoBehaviour
    {
        
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private TMP_Text _nextLevelButtonText;

        private RunState _runState;
        private SceneLoader _sceneLoader;

        private const string StartNextLevelText = "Start: ";


        public void Initialize(RunState runState, SceneLoader sceneLoader)
        {
            _runState = runState;
            _sceneLoader = sceneLoader;
            RefreshState();

            if (isActiveAndEnabled)
                SubscribeToButtons();
        }
        
        private void OnEnable()
        {
            if (_sceneLoader == null)
                return;

            RefreshState();
            SubscribeToButtons();
        }
        
        private void OnDisable()
        {
            UnsubscribeFromButtons();
        }

        
        private void RefreshState()
        {
            if (!_runState.HasCurrentLevel && !_runState.IsCompleted)
                _runState.StartNewRun();

            var canStartNextLevel = _runState.HasCurrentLevel;

            gameObject.SetActive(canStartNextLevel);

            if (_nextLevelButton != null)
                _nextLevelButton.interactable = canStartNextLevel;

            if (canStartNextLevel && _nextLevelButtonText != null)
                _nextLevelButtonText.text = $"{StartNextLevelText}{_runState.CurrentLevelId}";
        }
        
        
        private void SubscribeToButtons()
        {
            UnsubscribeFromButtons();

            if (_nextLevelButton != null)
                _nextLevelButton.onClick.AddListener(StartNextLevel);
        }

        private void UnsubscribeFromButtons()
        {
            if (_nextLevelButton != null)
                _nextLevelButton.onClick.RemoveListener(StartNextLevel);
        }

        private void StartNextLevel()
        {
            if (!_runState.HasCurrentLevel)
            {
                RefreshState();
                return;
            }

            _sceneLoader.LoadGameplay(_runState.CurrentLevelId);
        }
    }
}
