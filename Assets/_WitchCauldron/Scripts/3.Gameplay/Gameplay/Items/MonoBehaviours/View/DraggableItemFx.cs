using System.Collections;
using R3;
using UnityEngine;

namespace Gameplay.Items.MonoBehaviours.View
{
    
    public class DraggableItemFx : MonoBehaviour
    {
        
        private CompositeDisposable _disposables;
        
        private DraggableItem _item;

        private SpriteRenderer _spriteRenderer;
        private Transform _viewTransform;
        
        private int _sortingOrderBuffer;
        private Vector3 _initialLocalScale;
        private bool _isPickedUp;
        private bool _isCombineReady;
        private Coroutine _spawnPopCoroutine;

        private readonly float _pickedUpScale = 1.25f;
        [SerializeField] private float _pickedUpScaleLerpSpeed = 14f;
        [SerializeField] private float _combineReadyScaleAmplitude = 0.1f;
        [SerializeField] private float _combineReadyPulseSpeed = 14f;
        [SerializeField] private float _spawnPopStartScale = 0.75f;
        [SerializeField] private float _spawnPopOvershootScale = 1.12f;
        [SerializeField] private float _spawnPopDuration = 0.18f;

        private void Awake()
        {
            _item = GetComponentInParent<DraggableItem>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _viewTransform = transform;
            _initialLocalScale = _viewTransform.localScale;
        }

        private void Update()
        {
            if (_spawnPopCoroutine != null)
                return;

            _viewTransform.localScale = Vector3.Lerp(
                _viewTransform.localScale,
                GetTargetScale(),
                Time.deltaTime * _pickedUpScaleLerpSpeed);
        }

        private void OnEnable()
        {
            _disposables = new CompositeDisposable();
            _item.PickedUp.Subscribe(OnPickedUp).AddTo(_disposables);
            _item.Dropped.Subscribe(OnDrop).AddTo(_disposables);
        }

        private void OnDisable()
        {
            _disposables?.Dispose();
            _disposables = null;

            if (_spawnPopCoroutine != null)
            {
                StopCoroutine(_spawnPopCoroutine);
                _spawnPopCoroutine = null;
            }

            _viewTransform.localScale = _initialLocalScale;
            _isPickedUp = false;
            _isCombineReady = false;
        }

        
        private void OnPickedUp(Unit _)
        {
            _isPickedUp = true;
            _sortingOrderBuffer = _spriteRenderer.sortingOrder;
            _spriteRenderer.sortingOrder = 999;
        }


        private void OnDrop(Unit _)
        {
            _isPickedUp = false;
            _spriteRenderer.sortingOrder = _sortingOrderBuffer;
        }

        public void PlaySpawnPop()
        {
            if (_spawnPopCoroutine != null)
                StopCoroutine(_spawnPopCoroutine);

            _spawnPopCoroutine = StartCoroutine(SpawnPop());
        }

        public void SetCombineReady(bool isReady)
        {
            _isCombineReady = isReady;
        }

        private IEnumerator SpawnPop()
        {
            var elapsed = 0f;
            var startScale = _initialLocalScale * _spawnPopStartScale;
            var overshootScale = _initialLocalScale * _spawnPopOvershootScale;

            _viewTransform.localScale = startScale;

            while (elapsed < _spawnPopDuration)
            {
                elapsed += Time.deltaTime;

                var progress = Mathf.Clamp01(elapsed / _spawnPopDuration);
                var targetScale = progress <= 0.65f
                    ? Vector3.Lerp(startScale, overshootScale, progress / 0.65f)
                    : Vector3.Lerp(overshootScale, _initialLocalScale, (progress - 0.65f) / 0.35f);

                _viewTransform.localScale = targetScale;
                yield return null;
            }

            _viewTransform.localScale = GetTargetScale();
            _spawnPopCoroutine = null;
        }

        private Vector3 GetTargetScale()
        {
            var scaleMultiplier = _isPickedUp ? _pickedUpScale : 1f;

            if (_isCombineReady)
            {
                var pulse = (Mathf.Sin(Time.time * _combineReadyPulseSpeed) + 1f) * 0.5f;
                scaleMultiplier *= 1f + pulse * _combineReadyScaleAmplitude;
            }

            return _initialLocalScale * scaleMultiplier;
        }

        
    }
}
