using Gameplay.Battle.Waves.Service;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class UIProgressBar : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private Slider _slider;
        [SerializeField, Min(0f)] private float _fillSmoothTime = 0.2f;

        private float _targetProgress;
        private float _smoothVelocity;

        public void Initialize(IWaveService waveService)
        {
            _slider.minValue = 0f;
            _slider.maxValue = 1f;
            _slider.wholeNumbers = false;
            _slider.value = 0f;

            _targetProgress = 0f;
            _smoothVelocity = 0f;

            waveService.Progress
                .Subscribe(progress => UpdateProgress(progress.Level01))
                .AddTo(_disposables);
        }

        private void Update()
        {
            if (Mathf.Approximately(_slider.value, _targetProgress))
                return;

            if (_fillSmoothTime <= 0f)
            {
                _slider.value = _targetProgress;
                return;
            }

            _slider.value = Mathf.SmoothDamp(
                _slider.value,
                _targetProgress,
                ref _smoothVelocity,
                _fillSmoothTime);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void UpdateProgress(float progress)
        {
            _targetProgress = Mathf.Clamp01(progress);
        }
    }
}
