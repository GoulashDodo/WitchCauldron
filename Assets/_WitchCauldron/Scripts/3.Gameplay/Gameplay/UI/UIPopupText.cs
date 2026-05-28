

using System.Collections;
using TMPro;
using UnityEngine;

namespace Gameplay.UI
{
    public class UIPopupText : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _fadeDuration = 0.5f;

        public IEnumerator Show(string message, float duration)
        {
            _text.text = message;

            _canvasGroup.alpha = 0f;
            yield return Fade(0f, 1f);

            yield return new WaitForSeconds(duration);

            yield return Fade(1f, 0f);
        }

        private IEnumerator Fade(float from, float to)
        {
            if (_fadeDuration <= 0f)
            {
                _canvasGroup.alpha = to;
                yield break;
            }

            float time = 0f;

            while (time < _fadeDuration)
            {
                time += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(from, to, time / _fadeDuration);
                yield return null;
            }

            _canvasGroup.alpha = to;
        }
    }
}
