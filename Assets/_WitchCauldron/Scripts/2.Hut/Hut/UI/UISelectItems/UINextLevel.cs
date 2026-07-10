using Core.Run;
using Core.SceneManagement;
using Gameplay._root;
using Hut.SelectedItems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hut.UI.UISelectItems
{
    public class UINextLevel : MonoBehaviour
    {
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private TMP_Text _nextLevelButtonText;

        private RunState _runState;
        private SceneLoader _sceneLoader;
        private SelectedItemsRuntime _selectedItemsRuntime;

        private const string StartNextLevelText = "Start: ";


        public void Initialize(
            RunState runState,
            SceneLoader sceneLoader,
            SelectedItemsRuntime selectedItemsRuntime)
        {
            _runState = runState;
            _sceneLoader = sceneLoader;
            _selectedItemsRuntime = selectedItemsRuntime;
            RefreshState();

            if (isActiveAndEnabled)
                Subscribe();
        }
        
        private void OnEnable()
        {
            if (_sceneLoader == null)
                return;

            RefreshState();
            Subscribe();
        }
        
        private void OnDisable()
        {
            Unsubscribe();
        }
        private void RefreshState()
        {
            if (!_runState.HasCurrentLevel && !_runState.IsCompleted)
                _runState.StartNewRun();
            
            _nextLevelButtonText.text = $"{StartNextLevelText}{_runState.CurrentLevelName}";

            if (_nextLevelButton != null)
                _nextLevelButton.interactable = _runState.HasCurrentLevel && _selectedItemsRuntime.HasRequiredSelectedItems;
        }
        
        private void Subscribe()
        {
            Unsubscribe();

            if (_nextLevelButton != null)
                _nextLevelButton.onClick.AddListener(StartNextLevel);

            if (_selectedItemsRuntime != null)
                _selectedItemsRuntime.SelectionChanged += RefreshState;
        }

        private void Unsubscribe()
        {
            if (_nextLevelButton != null)
                _nextLevelButton.onClick.RemoveListener(StartNextLevel);

            if (_selectedItemsRuntime != null)
                _selectedItemsRuntime.SelectionChanged -= RefreshState;
        }

        private void StartNextLevel()
        {
            if (!_runState.HasCurrentLevel)
            {
                RefreshState();
                return;
            }

            if (!_selectedItemsRuntime.HasRequiredSelectedItems)
            {
                RefreshState();
                return;
            }

            var gameplayEntryPoint = new GameplayEntryParameters(
                _runState.CurrentLevelId,
                _selectedItemsRuntime.GetSelectedItemsIds()
                );
            
            _sceneLoader.LoadGameplay(gameplayEntryPoint);
        }
    }
}
