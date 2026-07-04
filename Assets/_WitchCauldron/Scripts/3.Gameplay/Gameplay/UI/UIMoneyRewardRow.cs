using TMPro;
using System.Collections;
using UnityEngine;

namespace Gameplay.UI
{
    public class UIMoneyRewardRow : MonoBehaviour
    {
        private const float HiddenScale = 0.92f;
        private const float PopScale = 1.06f;

        [SerializeField] private TMP_Text _labelText;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private bool _hideWhenZero;

        private CanvasGroup _canvasGroup;
        private int _amount;

        public void Initialize(string label, int amount)
        {
            _amount = amount;

            if (_labelText != null)
                _labelText.text = label;

            if (_amountText != null)
                _amountText.text = FormatAmount(amount);

            if (_hideWhenZero)
                gameObject.SetActive(amount > 0);
            else
                gameObject.SetActive(true);
        }

        public void PrepareHidden()
        {
            EnsureCanvasGroup();
            _canvasGroup.alpha = 0f;
            transform.localScale = Vector3.one * HiddenScale;
        }

        public IEnumerator PlayAppear(float duration)
        {
            if (!gameObject.activeSelf)
                yield break;

            EnsureCanvasGroup();

            yield return AnimateAppear(duration);
        }

        public IEnumerator PlayTotalCount(int total, float appearDuration, float countDuration)
        {
            Initialize(_labelText != null ? _labelText.text : "Total", 0);
            PrepareHidden();

            yield return PlayAppear(appearDuration);
            yield return CountAmount(total, countDuration);
        }

        private IEnumerator AnimateAppear(float duration)
        {
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = EaseOutBack(t);

                _canvasGroup.alpha = t;
                transform.localScale = Vector3.one * Mathf.LerpUnclamped(HiddenScale, PopScale, eased);
                yield return null;
            }

            elapsed = 0f;
            const float settleDuration = 0.08f;

            while (elapsed < settleDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / settleDuration);
                transform.localScale = Vector3.one * Mathf.Lerp(PopScale, 1f, t);
                yield return null;
            }

            _canvasGroup.alpha = 1f;
            transform.localScale = Vector3.one;
        }

        private IEnumerator CountAmount(int targetAmount, float duration)
        {
            if (_amountText == null)
                yield break;

            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var value = Mathf.RoundToInt(Mathf.Lerp(0, targetAmount, EaseOutCubic(t)));
                _amountText.text = FormatAmount(value);
                yield return null;
            }

            _amount = targetAmount;
            _amountText.text = FormatAmount(_amount);
        }

        private void EnsureCanvasGroup()
        {
            if (_canvasGroup != null)
                return;

            _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        private static string FormatAmount(int amount)
        {
            return amount > 0 ? $"+{amount}" : amount.ToString();
        }

        private static float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        private static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }
    }
}
