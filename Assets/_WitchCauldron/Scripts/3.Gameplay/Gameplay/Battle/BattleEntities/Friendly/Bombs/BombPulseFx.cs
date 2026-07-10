using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Friendly.Bombs
{
    [DisallowMultipleComponent]
    public sealed class BombPulseFx : MonoBehaviour
    {
        [SerializeField] private Transform _viewRoot;
        [SerializeField] private float _startPulseSpeed = 3f;
        [SerializeField] private float _endPulseSpeed = 11f;
        [SerializeField] private float _startScaleAmplitude = 0.04f;
        [SerializeField] private float _endScaleAmplitude = 0.16f;

        private Bomb _bomb;
        private Transform _target;
        private Vector3 _initialScale;
        private float _phase;

        private void Awake()
        {
            _bomb = GetComponent<Bomb>();
            _target = _viewRoot ? _viewRoot : transform;
            _initialScale = _target.localScale;
        }

        private void OnEnable()
        {
            _phase = 0f;

            if (_target != null)
                _target.localScale = _initialScale;
        }

        private void Update()
        {
            if (_target == null || _bomb == null)
                return;

            var progress = _bomb.FuseProgress;
            var speed = Mathf.Lerp(_startPulseSpeed, _endPulseSpeed, progress);
            var amplitude = Mathf.Lerp(_startScaleAmplitude, _endScaleAmplitude, progress);

            _phase += Time.deltaTime * speed;
            var pulse = (Mathf.Sin(_phase) + 1f) * 0.5f;
            var scaleMultiplier = 1f + pulse * amplitude;

            _target.localScale = _initialScale * scaleMultiplier;
        }

        private void OnDisable()
        {
            if (_target != null)
                _target.localScale = _initialScale;
        }
    }
}
