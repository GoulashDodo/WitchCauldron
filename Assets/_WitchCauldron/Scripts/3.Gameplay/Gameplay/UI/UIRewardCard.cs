using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class UIRewardCard : MonoBehaviour
    {
        private const float HiddenScale = 0.82f;
        private const float PopScale = 1.08f;

        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _countText;

        private CanvasGroup _canvasGroup;

        public void Initialize(Sprite icon, int count = 1)
        {
            gameObject.SetActive(true);

            _icon.sprite = icon;
        
            var shouldShowCount = count > 1;
            _countText.gameObject.SetActive(shouldShowCount);
            _countText.text = shouldShowCount ? count.ToString() : string.Empty;
        }

        public void PrepareHidden()
        {
            EnsureCanvasGroup();
            _canvasGroup.alpha = 0f;
            transform.localScale = Vector3.one * HiddenScale;
        }

        public IEnumerator PlayAppear(float duration)
        {
            EnsureCanvasGroup();

            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);

                _canvasGroup.alpha = t;
                transform.localScale = Vector3.one * Mathf.LerpUnclamped(HiddenScale, PopScale, EaseOutBack(t));
                yield return null;
            }

            elapsed = 0f;
            const float settleDuration = 0.08f;

            while (elapsed < settleDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                transform.localScale = Vector3.one * Mathf.Lerp(PopScale, 1f, Mathf.Clamp01(elapsed / settleDuration));
                yield return null;
            }

            _canvasGroup.alpha = 1f;
            transform.localScale = Vector3.one;
        }

        private void EnsureCanvasGroup()
        {
            if (_canvasGroup != null)
                return;

            _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        private static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }
    }
}
