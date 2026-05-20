using Feature.Gameplay.Battle.Waves.Service;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Feature.Gameplay.UI
{
    public class UIProgressBar : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private Slider _slider;

        public void Initialize(IWaveService waveService)
        {
            _slider.minValue = 0f;
            _slider.maxValue = 1f;
            _slider.wholeNumbers = false;

            waveService.Progress01
                .Subscribe(UpdateProgress)
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void UpdateProgress(float progress)
        {
            _slider.value = Mathf.Clamp01(progress);
        }
    }
}
