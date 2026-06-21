using System.Collections;
using Gameplay.Battle.HealthSystem.Structs;
using TMPro;
using UnityEngine;

namespace Gameplay.UI.Enemies
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(TMP_Text))]
    public class UIDamageText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _lifetime = 0.8f;
        [SerializeField] private float _fadeDuration = 0.8f;
        [SerializeField] private float _riseDistance = 45f;
        [SerializeField] private float _sideSpread = 18f;
        [SerializeField] private float _sineAmplitude = 24f;
        [SerializeField] private float _sineFrequency = 1f;
        [SerializeField] private bool _useDamageSettingsFontSize = true;
        [SerializeField, Min(0.1f)] private float _fontSizeMultiplier = 1f;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
            _text ??= GetComponent<TMP_Text>();
            _canvasGroup ??= GetComponent<CanvasGroup>();
        }

        public void Play(DamageInfo damageInfo, DamageTypeTextSettings settings)
        {
            _text.text = Mathf.CeilToInt(damageInfo.Amount).ToString();
            _text.color = settings != null ? settings.TextColor : GetFallbackColor(damageInfo.Type);

            if (_useDamageSettingsFontSize)
            {
                var fontSize = settings != null ? settings.GetFontSize(damageInfo.Amount) : GetFallbackFontSize(damageInfo.Amount);
                _text.fontSize = fontSize * _fontSizeMultiplier;
            }

            StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            var startPosition = _rectTransform.anchoredPosition;
            var sideDirection = Random.value < 0.5f ? -1f : 1f;
            var baseSideOffset = Random.Range(-_sideSpread, _sideSpread);

            _canvasGroup.alpha = 1f;

            var time = 0f;
            while (time < _lifetime)
            {
                time += Time.deltaTime;
                var t = Mathf.Clamp01(time / _lifetime);

                var sineOffset = Mathf.Sin(t * Mathf.PI * 2f * _sineFrequency) * _sineAmplitude * sideDirection;
                _rectTransform.anchoredPosition = startPosition + new Vector2(baseSideOffset + sineOffset, _riseDistance * t);
                _canvasGroup.alpha = GetAlpha(time);

                yield return null;
            }

            Destroy(gameObject);
        }

        private float GetAlpha(float time)
        {
            if (_fadeDuration <= 0f)
                return 0f;

            var fadeStart = Mathf.Max(0f, _lifetime - _fadeDuration);
            if (time <= fadeStart)
                return 1f;

            var fadeProgress = Mathf.InverseLerp(fadeStart, _lifetime, time);
            return 1f - fadeProgress;
        }

        private static Color GetFallbackColor(DamageType damageType)
        {
            return damageType switch
            {
                DamageType.Fire => new Color(1f, 0.32f, 0.08f),
                DamageType.Poison => new Color(0.45f, 1f, 0.22f),
                _ => Color.white
            };
        }

        private static float GetFallbackFontSize(float damage)
        {
            return Mathf.Lerp(18f, 42f, Mathf.InverseLerp(1f, 20f, damage));
        }
    }
}
