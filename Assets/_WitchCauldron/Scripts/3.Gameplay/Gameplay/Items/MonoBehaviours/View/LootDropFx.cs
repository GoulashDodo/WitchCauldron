using System.Collections;
using UnityEngine;

namespace Gameplay.Items.MonoBehaviours.View
{
    public class LootDropFx : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.4f;
        [SerializeField] private float _arcHeight = 0.25f;
        [SerializeField] private float _startScale = 0.85f;
        [SerializeField] private float _landScale = 1.05f;

        private Coroutine _dropCoroutine;
        private Collider2D[] _colliders;
        private Vector3 _initialScale;

        private void Awake()
        {
            _colliders = GetComponentsInChildren<Collider2D>();
            _initialScale = transform.localScale;
        }

        public void Play(Vector3 startPosition, Vector3 endPosition)
        {
            if (_dropCoroutine != null)
            {
                StopCoroutine(_dropCoroutine);
            }

            _dropCoroutine = StartCoroutine(DropRoutine(startPosition, endPosition));
        }

        private IEnumerator DropRoutine(Vector3 startPosition, Vector3 endPosition)
        {
            SetCollidersEnabled(false);

            var elapsed = 0f;
            var duration = Mathf.Max(0.01f, _duration);
            transform.position = startPosition;
            transform.localScale = _initialScale * _startScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var arc = Mathf.Sin(t * Mathf.PI) * _arcHeight;

                transform.position = Vector3.Lerp(startPosition, endPosition, EaseOutCubic(t)) + Vector3.up * arc;
                transform.localScale = GetScale(t);

                yield return null;
            }

            transform.position = endPosition;
            transform.localScale = _initialScale;
            SetCollidersEnabled(true);
            _dropCoroutine = null;
        }

        private Vector3 GetScale(float t)
        {
            if (t < 0.8f)
                return Vector3.Lerp(_initialScale * _startScale, _initialScale * _landScale, t / 0.8f);

            return Vector3.Lerp(_initialScale * _landScale, _initialScale, (t - 0.8f) / 0.2f);
        }

        private void SetCollidersEnabled(bool enabled)
        {
            if (_colliders == null)
                return;

            foreach (var itemCollider in _colliders)
            {
                if (itemCollider != null)
                    itemCollider.enabled = enabled;
            }
        }

        private static float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }
    }
}
