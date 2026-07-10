using System.Collections;
using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Friendly
{
    public class LifetimeController : MonoBehaviour
    {
        [SerializeField] private float _lifetime;
        [SerializeField] private float _blinkDuration = 0.6f;
        [SerializeField] private float _blinkInterval = 0.1f;

        private SpriteRenderer[] _spriteRenderers;
        private bool[] _spriteRendererEnabledStates;
        private Coroutine _lifetimeCoroutine;

        private void Awake()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            _spriteRendererEnabledStates = new bool[_spriteRenderers.Length];
        }

        private void OnEnable()
        {
            CacheRendererStates();
            _lifetimeCoroutine = StartCoroutine(LifetimeRoutine());
        }

        private void OnDisable()
        {
            if (_lifetimeCoroutine != null)
            {
                StopCoroutine(_lifetimeCoroutine);
                _lifetimeCoroutine = null;
            }

            SetRenderersVisible(true);
        }

        private IEnumerator LifetimeRoutine()
        {
            var delayBeforeBlink = Mathf.Max(0f, _lifetime - _blinkDuration);
            if (delayBeforeBlink > 0f)
                yield return new WaitForSeconds(delayBeforeBlink);

            var elapsed = 0f;
            var visible = true;
            var blinkInterval = Mathf.Max(0.01f, _blinkInterval);
            while (elapsed < _blinkDuration)
            {
                visible = !visible;
                SetRenderersVisible(visible);

                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval;
            }

            Destroy(gameObject);
        }

        private void SetRenderersVisible(bool visible)
        {
            for (var i = 0; i < _spriteRenderers.Length; i++)
            {
                if (_spriteRenderers[i] != null)
                    _spriteRenderers[i].enabled = visible && _spriteRendererEnabledStates[i];
            }
        }

        private void CacheRendererStates()
        {
            for (var i = 0; i < _spriteRenderers.Length; i++)
            {
                _spriteRendererEnabledStates[i] = _spriteRenderers[i] != null && _spriteRenderers[i].enabled;
            }
        }
    }
}
