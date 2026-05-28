using Gameplay.Battle.Waves.Service;
using Gameplay.Battle.Waves.Enums;
using Gameplay.Battle.Waves.Structures;
using R3;
using UnityEngine;

namespace Gameplay.UI
{
    [RequireComponent(typeof(UIPopupText))]
    public class UIWaveAlert : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private float _messageDuration = 2f;
        [SerializeField] private string _firstWaveMessage = "First wave!";
        [SerializeField] private string _hugeWaveMessage = "Huge wave!";
        [SerializeField] private string _finalWaveMessage = "Final wave!";

        private UIPopupText _popupText;

        private void Awake()
        {
            _popupText = GetComponent<UIPopupText>();
        }

        public void Initialize(IWaveService service)
        {
            service.WaveStarted
                .Subscribe(ShowWaveAlert)
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void ShowWaveAlert(WaveStartedEvent wave)
        {
            var message = GetMessage(wave);

            if (string.IsNullOrEmpty(message))
                return;

            StopAllCoroutines();
            StartCoroutine(_popupText.Show(message, _messageDuration));
        }

        private string GetMessage(WaveStartedEvent wave)
        {
            if (wave.IsFirst)
                return _firstWaveMessage;

            if (wave.IsFinal)
                return _finalWaveMessage;

            return wave.Type == WaveType.Huge ? _hugeWaveMessage : null;
        }
    }
}
