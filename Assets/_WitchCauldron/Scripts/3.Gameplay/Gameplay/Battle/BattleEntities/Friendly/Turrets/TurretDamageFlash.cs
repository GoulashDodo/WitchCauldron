using System.Collections;
using R3;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Friendly.Turrets
{
    public class TurretDamageFlash : MonoBehaviour
    {
        private static readonly int FlashColorId = Shader.PropertyToID("_FlashColor");
        private static readonly int FlashAmountId = Shader.PropertyToID("_FlashAmount");

        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField, Range(0f, 1f)] private float _flashAmount = 0.5f;
        [SerializeField] private float _flashInDuration = 0.035f;
        [SerializeField] private float _flashHoldDuration = 0.025f;
        [SerializeField] private float _flashOutDuration = 0.06f;
        [SerializeField] private int _flashCount = 2;

        private readonly CompositeDisposable _disposables = new();

        private Turret _turret;
        private SpriteRenderer[] _spriteRenderers;
        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _flashCoroutine;

        private void Awake()
        {
            _turret = GetComponentInParent<Turret>();
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            _propertyBlock = new MaterialPropertyBlock();

            SetFlashColor(_flashColor);
            SetFlashAmount(0f);
        }

        private void Start()
        {
            if (_turret == null)
            {
                Debug.LogWarning($"{nameof(TurretDamageFlash)} on {name} could not find parent {nameof(Turret)}.");
                return;
            }

            _turret.Health.Damaged
                .Subscribe(_ => Play())
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void Play()
        {
            if (_spriteRenderers == null || _spriteRenderers.Length == 0)
                return;

            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
            }

            _flashCoroutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            for (var i = 0; i < _flashCount; i++)
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
            SetRendererPropertyBlock(block => block.SetColor(FlashColorId, color));
        }

        private void SetFlashAmount(float amount)
        {
            SetRendererPropertyBlock(block => block.SetFloat(FlashAmountId, amount));
        }

        private void SetRendererPropertyBlock(System.Action<MaterialPropertyBlock> update)
        {
            if (_spriteRenderers == null)
                return;

            foreach (var spriteRenderer in _spriteRenderers)
            {
                if (spriteRenderer == null)
                    continue;

                spriteRenderer.GetPropertyBlock(_propertyBlock);
                update(_propertyBlock);
                spriteRenderer.SetPropertyBlock(_propertyBlock);
            }
        }
    }
}
