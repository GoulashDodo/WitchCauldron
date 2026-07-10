using System;
using System.Collections;
using Core.Audio;
using Gameplay._root;
using Gameplay._root.SO;
using Gameplay.Battle.Base.Interfaces;
using Gameplay.Battle.Waves.Service;
using Gameplay.Level;
using Gameplay.Level.SO;
using Gameplay.Rewards;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class UIWin : MonoBehaviour
    {
        private enum WinScreenStep
        {
            Money,
            Rewards
        }

        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private GameObject _panel;
        [SerializeField] private GameObject _moneyPanel;
        [SerializeField] private GameObject _rewardsPanel;
    
        
        [SerializeField] private Button _continueButton;
        [SerializeField] private UIMoneyRewardBreakdown _moneyRewardBreakdown;
        [SerializeField] private UIRewardList _rewardList;
        [SerializeField] private float _rootAppearDuration = 0.22f;
        [SerializeField] private float _panelFadeDuration = 0.18f;
        [SerializeField] private float _continueAppearDuration = 0.16f;
        
        private GameplayRunFlowController _runFlowController;
        private GameplayPauseService _pauseService;
        private AudioService _audioService;
        private IDisposable _pauseHandle;
        private LevelSettings _levelSettings;
        private GameplaySettings _gameplaySettings;
        private IBaseHealthProvider _baseHealthProvider;
        private IWaveService _waveService;
        private VictoryRewardCalculator _rewardCalculator;
        private VictoryRewardBreakdown _currentRewards;
        private WinScreenStep _currentStep;
        private Coroutine _sequenceRoutine;
        private Coroutine _continueButtonRoutine;

        public void Initialize(
            G game,
            GameplayRunFlowController runFlowController,
            GameplayPauseService pauseService,
            LevelSettings levelSettings,
            GameplaySettings gameplaySettings,
            AudioService audioService,
            IBaseHealthProvider baseHealthProvider,
            IWaveService waveService,
            VictoryRewardCalculator rewardCalculator)
        {
            _runFlowController = runFlowController;
            _pauseService = pauseService;
            _levelSettings = levelSettings;
            _gameplaySettings = gameplaySettings;
            _audioService = audioService;
            _baseHealthProvider = baseHealthProvider;
            _waveService = waveService;
            _rewardCalculator = rewardCalculator;

            HidePanel();
            SubscribeToButtons();

            game.GameWon
                .Subscribe(_ => ShowPanel())
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            StopSequence();
            StopContinueButtonAnimation();
            ReleasePause();
            UnsubscribeFromButtons();
            _disposables.Dispose();
        }

        private void ShowPanel()
        {
            RequestPause();
            _audioService?.PlayUi(AudioId.Victory);
            _currentRewards = CalculateRewards();

            _panel.SetActive(true);
            PrepareRootPanelHidden();
            SetContinueInteractable(false);
            StartSequence(ShowMoneyStepSequence());
        }

        private IEnumerator ShowMoneyStepSequence()
        {
            yield return PlayRootPanelAppear();

            _currentStep = WinScreenStep.Money;

            SetPanelActive(_moneyPanel, true);
            SetPanelActive(_rewardsPanel, false);

            if (_moneyRewardBreakdown != null)
            {
                _moneyRewardBreakdown.Initialize(_currentRewards);
                yield return _moneyRewardBreakdown.PlaySequence();
            }

            SetContinueInteractable(true);
        }

        private IEnumerator ShowRewardsStepSequence()
        {
            SetContinueInteractable(false);
            _currentStep = WinScreenStep.Rewards;

            yield return FadePanel(_moneyPanel, false);
            SetPanelActive(_rewardsPanel, true);
            yield return FadePanel(_rewardsPanel, true);

            if (_rewardList != null)
            {
                _rewardList.Initialize(_currentRewards.UnlockRewards, _gameplaySettings);
                yield return _rewardList.PlaySequence();
            }

            SetContinueInteractable(true);
        }

        private VictoryRewardBreakdown CalculateRewards()
        {
            return _rewardCalculator.Calculate(
                _levelSettings,
                _baseHealthProvider?.GetBaseHealth(),
                _waveService != null ? _waveService.ElapsedTime : 0f);
        }

        private void HidePanel()
        {
            StopSequence();
            StopContinueButtonAnimation();
            _panel.SetActive(false);
            SetPanelActive(_moneyPanel, false);
            SetPanelActive(_rewardsPanel, false);
            SetContinueHiddenImmediate();
            ReleasePause();
        }

        private void SubscribeToButtons()
        {
            UnsubscribeFromButtons();


            if (_continueButton != null)
                _continueButton.onClick.AddListener(Continue);

        }

        private void UnsubscribeFromButtons()
        {

            if (_continueButton != null)
                _continueButton.onClick.RemoveListener(Continue);

        }

        private void Continue()
        {
            if (_currentStep == WinScreenStep.Money && HasUnlockRewards())
            {
                StartSequence(ShowRewardsStepSequence());
                return;
            }

            ContinueToHut();
        }

        private bool HasUnlockRewards()
        {
            return _currentRewards.UnlockRewards != null && _currentRewards.UnlockRewards.Length > 0;
        }

        private static void SetPanelActive(GameObject panel, bool isActive)
        {
            if (panel != null)
                panel.SetActive(isActive);
        }

        private void StartSequence(IEnumerator sequence)
        {
            StopSequence();
            _sequenceRoutine = StartCoroutine(sequence);
        }

        private void StopSequence()
        {
            if (_sequenceRoutine == null)
                return;

            StopCoroutine(_sequenceRoutine);
            _sequenceRoutine = null;
        }

        private void SetContinueInteractable(bool interactable)
        {
            if (_continueButton == null)
                return;

            StopContinueButtonAnimation();
            _continueButton.interactable = interactable;

            if (interactable)
                _continueButtonRoutine = StartCoroutine(PlayContinueButtonAppear());
            else
                SetContinueHiddenImmediate();
        }

        private void SetContinueHiddenImmediate()
        {
            if (_continueButton == null)
                return;

            var buttonObject = _continueButton.gameObject;
            var canvasGroup = GetOrAddCanvasGroup(buttonObject);

            _continueButton.interactable = false;
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            buttonObject.transform.localScale = Vector3.one * 0.94f;
        }

        private IEnumerator PlayContinueButtonAppear()
        {
            var buttonObject = _continueButton.gameObject;
            var canvasGroup = GetOrAddCanvasGroup(buttonObject);
            var elapsed = 0f;
            const float startScale = 0.94f;
            const float popScale = 1.05f;

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            buttonObject.transform.localScale = Vector3.one * startScale;

            while (elapsed < _continueAppearDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / _continueAppearDuration);
                var eased = EaseOutCubic(t);

                canvasGroup.alpha = t;
                buttonObject.transform.localScale = Vector3.one * Mathf.Lerp(startScale, popScale, eased);
                yield return null;
            }

            elapsed = 0f;
            const float settleDuration = 0.07f;

            while (elapsed < settleDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                buttonObject.transform.localScale = Vector3.one * Mathf.Lerp(popScale, 1f, Mathf.Clamp01(elapsed / settleDuration));
                yield return null;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            buttonObject.transform.localScale = Vector3.one;
            _continueButton.interactable = true;
            _continueButtonRoutine = null;
        }

        private void StopContinueButtonAnimation()
        {
            if (_continueButtonRoutine == null)
                return;

            StopCoroutine(_continueButtonRoutine);
            _continueButtonRoutine = null;
        }

        private IEnumerator FadePanel(GameObject panel, bool fadeIn)
        {
            if (panel == null)
                yield break;

            var canvasGroup = GetOrAddCanvasGroup(panel);
            var from = fadeIn ? 0f : 1f;
            var to = fadeIn ? 1f : 0f;
            var elapsed = 0f;

            canvasGroup.alpha = from;

            while (elapsed < _panelFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / _panelFadeDuration));
                yield return null;
            }

            canvasGroup.alpha = to;

            if (!fadeIn)
                panel.SetActive(false);
        }

        private static CanvasGroup GetOrAddCanvasGroup(GameObject target)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();
            return canvasGroup != null ? canvasGroup : target.AddComponent<CanvasGroup>();
        }

        private void PrepareRootPanelHidden()
        {
            if (_panel == null)
                return;

            var canvasGroup = GetOrAddCanvasGroup(_panel);
            canvasGroup.alpha = 0f;
            _panel.transform.localScale = Vector3.one * 0.94f;
        }

        private IEnumerator PlayRootPanelAppear()
        {
            if (_panel == null)
                yield break;

            var canvasGroup = GetOrAddCanvasGroup(_panel);
            var elapsed = 0f;
            const float popScale = 1.03f;

            while (elapsed < _rootAppearDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / _rootAppearDuration);
                var eased = EaseOutCubic(t);

                canvasGroup.alpha = t;
                _panel.transform.localScale = Vector3.one * Mathf.Lerp(0.94f, popScale, eased);
                yield return null;
            }

            elapsed = 0f;
            const float settleDuration = 0.08f;

            while (elapsed < settleDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _panel.transform.localScale = Vector3.one * Mathf.Lerp(popScale, 1f, Mathf.Clamp01(elapsed / settleDuration));
                yield return null;
            }

            canvasGroup.alpha = 1f;
            _panel.transform.localScale = Vector3.one;
        }

        private static float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        private void ExitToMainMenu()
        {
            ReleasePause();
            _runFlowController.CompleteLevelAndOpenMainMenu(_currentRewards);
        }

        private void ContinueToHut()
        {
            ReleasePause();
            _runFlowController.CompleteLevelAndOpenHut(_currentRewards);
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
