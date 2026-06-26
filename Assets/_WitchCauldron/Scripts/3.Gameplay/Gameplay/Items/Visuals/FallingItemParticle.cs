using UnityEngine;

namespace Gameplay.Items.Visuals
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FallingItemParticle : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private Vector2 _velocity;
        private float _angularSpeed;
        private float _lifetime = 1f;
        private float _gravity = 16f;
        private float _bounceDamping = 0.48f;
        private float _groundY;
        private float _age;
        private int _remainingBounces = 2;
        private Color _startColor;

        public void Initialize(
            Vector2 velocity,
            float angularSpeed,
            float lifetime,
            float gravity,
            int bounceCount,
            float bounceDamping)
        {
            _velocity = velocity;
            _angularSpeed = angularSpeed;
            _lifetime = Mathf.Max(0.01f, lifetime);
            _gravity = Mathf.Max(0f, gravity);
            _remainingBounces = Mathf.Max(0, bounceCount);
            _bounceDamping = Mathf.Clamp01(bounceDamping);
        }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _startColor = _renderer.color;
            _groundY = transform.position.y;
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            _age += dt;

            _velocity += Vector2.down * (_gravity * dt);
            transform.position += (Vector3)(_velocity * dt);
            transform.Rotate(0f, 0f, _angularSpeed * dt);
            TryBounce();

            var t = Mathf.Clamp01(_age / _lifetime);
            var color = _startColor;
            color.a *= 1f - t;
            _renderer.color = color;

            if (_age >= _lifetime)
                Destroy(gameObject);
        }

        private void TryBounce()
        {
            if (_remainingBounces <= 0 || _velocity.y >= 0f || transform.position.y > _groundY)
                return;

            var position = transform.position;
            position.y = _groundY;
            transform.position = position;

            _velocity = new Vector2(_velocity.x * _bounceDamping, -_velocity.y * _bounceDamping);
            _angularSpeed *= _bounceDamping;
            _remainingBounces--;
        }
    }
}
