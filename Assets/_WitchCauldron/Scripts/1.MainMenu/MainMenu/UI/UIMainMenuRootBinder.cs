using Core.Run;
using Core.SceneManagement;
using Core.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MainMenu.UI
{
    public class UIMainMenuRootBinder : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _exitButton;

        private RunState _runState;
        private SceneLoader _sceneLoader;

        [Inject]
        public void Construct(UIRootView view, RunState runState, SceneLoader sceneLoader)
        {
            _runState = runState;
            _sceneLoader = sceneLoader;

            view.AttachSceneUI(gameObject);

            if (isActiveAndEnabled)
                SubscribeToButtons();
        }

        private void Awake()
        {
            CacheButtons();
        }

        private void OnEnable()
        {
            if (_sceneLoader == null)
                return;

            SubscribeToButtons();
        }

        private void OnDisable()
        {
            UnsubscribeFromButtons();
        }

        private void CacheButtons()
        {
            var buttons = GetComponentsInChildren<Button>(true);

            foreach (var button in buttons)
            {
                if (button.name == "StartButton")
                    _startButton = button;

                if (button.name == "ExitButton")
                    _exitButton = button;
            }
        }

        private void SubscribeToButtons()
        {
            UnsubscribeFromButtons();

            if (_startButton != null)
                _startButton.onClick.AddListener(StartNewRun);

            if (_exitButton != null)
                _exitButton.onClick.AddListener(ExitGame);
        }

        private void UnsubscribeFromButtons()
        {
            if (_startButton != null)
                _startButton.onClick.RemoveListener(StartNewRun);

            if (_exitButton != null)
                _exitButton.onClick.RemoveListener(ExitGame);
        }

        private void StartNewRun()
        {
            if (!_runState.StartNewRun())
                return;

            _sceneLoader.LoadGameplay(_runState.CurrentLevelId);
        }

        private static void ExitGame()
        {
            Application.Quit();
        }
    }
}
