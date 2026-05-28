using System.Collections;
using R3;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Enemies.Core
{
    public class EnemyDamageFlash : MonoBehaviour
    {
        private static readonly int FlashColorId = Shader.PropertyToID("_FlashColor");
        private static readonly int FlashAmountId = Shader.PropertyToID("_FlashAmount");

        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField, Range(0f, 1f)] private float _flashAmount = 0.5f;
        [SerializeField] private float _flashInDuration = 0.035f;
        [SerializeField] private float _flashHoldDuration = 0.025f;
        [SerializeField] private float _flashOutDuration = 0.06f;
        [SerializeField] private int _flashCount = 2;

        private SpriteRenderer _spriteRenderer;
        
        
        private readonly CompositeDisposable _disposables = new();

        private Enemy _enemy;
        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _flashCoroutine;

        private void Awake()
        {
            _enemy = GetComponentInParent<Enemy>();
            ResolveSpriteRenderer();

            _propertyBlock = new MaterialPropertyBlock();

            SetFlashColor(_flashColor);
            SetFlashAmount(0f);
        }

        private void Start()
        {
            if (_enemy == null)
            {
                Debug.LogWarning($"{nameof(EnemyDamageFlash)} on {name} could not find parent {nameof(Enemy)}.");
                return;
            }

            _enemy.Events.Damaged
                .Subscribe(_ => Play())
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        public void Play()
        {
            if (_spriteRenderer == null)
                return;

            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
            }

            _flashCoroutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            for (int i = 0; i < _flashCount; i++)
            {
                yield return LerpFlashAmount(0f, _flashAmount, _flashInDuration);

                if (_flashHoldDuration > 0f)
                    yield return new WaitForSeconds(_flashHoldDuration);

                yield return LerpFlashAmount(_flashAmount, 0f, _flashOutDuration);
            }

            SetFlashAmount(0f);
            _flashCoroutine = null;
        }

        private IEnumerator LerpFlashAmount(float from, float to, float duration)
        {
            if (duration <= 0f)
            {
                SetFlashAmount(to);
                yield break;
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                SetFlashAmount(Mathf.Lerp(from, to, t));
                yield return null;
            }

            SetFlashAmount(to);
        }

        private void SetFlashColor(Color color)
        {
            if (_spriteRenderer == null)
                return;

            _spriteRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(FlashColorId, color);
            _spriteRenderer.SetPropertyBlock(_propertyBlock);
        }

        private void SetFlashAmount(float amount)
        {
            if (_spriteRenderer == null)
                return;

            _spriteRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(FlashAmountId, amount);
            _spriteRenderer.SetPropertyBlock(_propertyBlock);
        }

        private void ResolveSpriteRenderer()
        {
            if (_spriteRenderer != null)
                return;

            if (TryGetComponent(out _spriteRenderer))
                return;

            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (_spriteRenderer != null)
                return;

            _spriteRenderer = GetComponentInParent<SpriteRenderer>();
            if (_spriteRenderer != null)
                return;

            Debug.LogWarning($"{nameof(EnemyDamageFlash)} on {name} could not find {nameof(SpriteRenderer)}.");
        }
    }
}
