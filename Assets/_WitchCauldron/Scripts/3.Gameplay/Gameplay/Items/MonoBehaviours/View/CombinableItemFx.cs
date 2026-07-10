using System.Collections;
using R3;
using UnityEngine;

namespace Gameplay.Items.MonoBehaviours.View
{
    public class CombinableItemFx : MonoBehaviour
    {
        
        [SerializeField] private float _shakeDuration = 0.18f;
        [SerializeField] private float _shakeAmplitude = 0.08f;
        [SerializeField] private int _shakeVibrations = 4;
        
        private CompositeDisposable _disposables;
        
        private CombinableItem _item;
        private DraggableItemFx _draggableItemFx;
        private Transform _viewTransform;
        private Coroutine _shakeCoroutine;
        private Vector3 _initialLocalPosition;
        private CombinableItemFx _currentTargetFx;
        
        private void Awake()
        {
            _item = GetComponentInParent<CombinableItem>();
            _draggableItemFx = GetComponent<DraggableItemFx>();
            _viewTransform = transform;
            _initialLocalPosition = _viewTransform.localPosition;
        }
        
        private void OnEnable()
        {
            _disposables = new CompositeDisposable();
            _item.CombineFailed.Subscribe(OnCombineFailed).AddTo(_disposables);
            _item.CombineTargetChanged.Subscribe(OnCombineTargetChanged).AddTo(_disposables);
        }

        private void OnDisable()
        {
            _disposables?.Dispose();
            _disposables = null;

            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
                _shakeCoroutine = null;
            }

            _viewTransform.localPosition = _initialLocalPosition;
            SetTargetCombineReady(null);
            SetCombineReady(false);
        }
        
        private void OnCombineFailed(CombinableItem other)
        {
            if (_shakeCoroutine != null)
                StopCoroutine(_shakeCoroutine);

            _shakeCoroutine = StartCoroutine(Shake());
        }

        private void OnCombineTargetChanged(CombineTargetState state)
        {
            var isReady = state.CanCombine && state.Target != null;
            SetCombineReady(isReady);
            SetTargetCombineReady(isReady ? state.Target : null);
        }

        private IEnumerator Shake()
        {
            var origin = _viewTransform.localPosition;
            var elapsed = 0f;

            while (elapsed < _shakeDuration)
            {
                elapsed += Time.deltaTime;

                var progress = elapsed / _shakeDuration;
                var damping = 1f - progress;
                var offsetX = Mathf.Sin(progress * _shakeVibrations * Mathf.PI * 2f)
                              * _shakeAmplitude
                              * damping;

                _viewTransform.localPosition = origin + new Vector3(offsetX, 0f, 0f);

                yield return null;
            }

            _viewTransform.localPosition = origin;
            _shakeCoroutine = null;
        }

        private void SetCombineReady(bool isReady)
        {
            _draggableItemFx?.SetCombineReady(isReady);
        }

        private void SetTargetCombineReady(CombinableItem target)
        {
            var targetFx = target != null
                ? target.GetComponentInChildren<CombinableItemFx>()
                : null;

            if (_currentTargetFx == targetFx)
                return;

            if (_currentTargetFx != null)
                _currentTargetFx.SetCombineReady(false);

            _currentTargetFx = targetFx;

            if (_currentTargetFx != null)
                _currentTargetFx.SetCombineReady(true);
        }
    }
}
