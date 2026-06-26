using System.Collections;
using R3;
using UnityEngine;

namespace Gameplay.Items.MonoBehaviours.View
{
    [RequireComponent(typeof(UsableItem))]
    public class UsableItemMissFx : MonoBehaviour
    {
        [SerializeField] private float _returnDuration = 0.28f;
        [SerializeField] private float _returnArcHeight = 0.35f;
        [SerializeField] private float _scalePunch = 0.08f;
        [SerializeField] private float _shakeDistance = 0.08f;

        private readonly CompositeDisposable _disposables = new();

        private UsableItem _item;
        private Transform _transform;
        private Vector3 _initialScale;
        private Coroutine _missCoroutine;

        private void Awake()
        {
            _item = GetComponent<UsableItem>();
            _transform = transform;
            _initialScale = _transform.localScale;
        }

        private void OnEnable()
        {
            _item.UseMissed.Subscribe(OnUseMissed).AddTo(_disposables);
        }

        private void OnDisable()
        {
            _disposables.Clear();

            if (_missCoroutine != null)
            {
                StopCoroutine(_missCoroutine);
                _missCoroutine = null;
            }

            _transform.localScale = _initialScale;
        }

        private void OnUseMissed(Unit _)
        {
            if (_missCoroutine != null)
                StopCoroutine(_missCoroutine);

            var startPosition = _transform.position;
            var returnPosition = _item.LastDragStartPosition;
            returnPosition.z = startPosition.z;

            _missCoroutine = (returnPosition - startPosition).sqrMagnitude <= 0.0025f
                ? StartCoroutine(ShakeRoutine(startPosition))
                : StartCoroutine(ReturnRoutine(startPosition, returnPosition));
        }

        private IEnumerator ReturnRoutine(Vector3 startPosition, Vector3 returnPosition)
        {
            var elapsed = 0f;
            var duration = Mathf.Max(0.01f, _returnDuration);
            var arcHeight = Mathf.Min(_returnArcHeight, Vector3.Distance(startPosition, returnPosition) * 0.45f);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = SmoothStep(t);
                var arc = Mathf.Sin(t * Mathf.PI) * arcHeight;

                _transform.position = Vector3.Lerp(startPosition, returnPosition, eased) + Vector3.up * arc;
                _transform.localScale = _initialScale * (1f + Mathf.Sin(t * Mathf.PI) * _scalePunch);

                yield return null;
            }

            _transform.position = returnPosition;
            _transform.localScale = _initialScale;
            _missCoroutine = null;
        }

        private IEnumerator ShakeRoutine(Vector3 position)
        {
            var elapsed = 0f;
            var duration = Mathf.Max(0.01f, _returnDuration * 0.65f);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var fade = 1f - t;
                var offset = Mathf.Sin(t * Mathf.PI * 4f) * _shakeDistance * fade;

                _transform.position = position + Vector3.right * offset;
                _transform.localScale = _initialScale * (1f + Mathf.Sin(t * Mathf.PI) * _scalePunch);

                yield return null;
            }

            _transform.position = position;
            _transform.localScale = _initialScale;
            _missCoroutine = null;
        }

        private static float SmoothStep(float t)
        {
            return t * t * (3f - 2f * t);
        }
    }
}
