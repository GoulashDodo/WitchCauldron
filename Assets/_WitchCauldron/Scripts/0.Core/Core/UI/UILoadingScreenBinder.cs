using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class UILoadingScreenBinder : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _dimmingImage;
        [SerializeField] private Color _dimmingColor = new(0.02f, 0.01f, 0.04f, 0.78f);
        [SerializeField] private float _fadeInDuration = 0.18f;
        [SerializeField] private float _fadeOutDuration = 0.16f;

        private Coroutine _fadeRoutine;

        private void Awake()
        {
            EnsureCanvasGroup();
            EnsureDimmingImage();
            HideImmediate();
        }

        public void Show(Action onCompleted = null)
        {
            PlayFade(1f, _fadeInDuration, onCompleted);
        }

        public void Hide()
        {
            PlayFade(0f, _fadeOutDuration);
        }

        public void HideImmediate()
        {
            StopFade();
            EnsureCanvasGroup();
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }

        private void PlayFade(float targetAlpha, float duration, Action onCompleted = null)
        {
            StopFade();
            EnsureCanvasGroup();
            EnsureDimmingImage();
            _fadeRoutine = StartCoroutine(Fade(targetAlpha, duration, onCompleted));
        }

        private IEnumerator Fade(float targetAlpha, float duration, Action onCompleted)
        {
            var startAlpha = _canvasGroup.alpha;
            var elapsed = 0f;

            _canvasGroup.blocksRaycasts = true;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            _canvasGroup.alpha = targetAlpha;
            _canvasGroup.blocksRaycasts = targetAlpha > 0f;
            _fadeRoutine = null;
            onCompleted?.Invoke();
        }

        private void StopFade()
        {
            if (_fadeRoutine == null)
                return;

            StopCoroutine(_fadeRoutine);
            _fadeRoutine = null;
        }

        private void EnsureCanvasGroup()
        {
            if (_canvasGroup != null)
                return;

            _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        private void EnsureDimmingImage()
        {
            if (_dimmingImage == null)
                _dimmingImage = CreateDimmingImage();

            _dimmingImage.color = _dimmingColor;
            _dimmingImage.raycastTarget = false;
            _dimmingImage.transform.SetAsFirstSibling();
        }

        private Image CreateDimmingImage()
        {
            var dimmingObject = new GameObject("LoadingDimOverlay", typeof(RectTransform), typeof(Image));
            dimmingObject.transform.SetParent(transform, false);

            var rectTransform = (RectTransform)dimmingObject.transform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            return dimmingObject.GetComponent<Image>();
        }
    }
}
