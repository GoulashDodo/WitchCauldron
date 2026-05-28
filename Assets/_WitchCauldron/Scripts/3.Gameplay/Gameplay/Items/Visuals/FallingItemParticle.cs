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
        private float _age;
        private Color _startColor;

        public void Initialize(Vector2 velocity, float angularSpeed, float lifetime)
        {
            _velocity = velocity;
            _angularSpeed = angularSpeed;
            _lifetime = Mathf.Max(0.01f, lifetime);
        }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _startColor = _renderer.color;
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            _age += dt;

            transform.position += (Vector3)(_velocity * dt);
            transform.Rotate(0f, 0f, _angularSpeed * dt);

            var t = Mathf.Clamp01(_age / _lifetime);
            var color = _startColor;
            color.a *= 1f - t;
            _renderer.color = color;

            if (_age >= _lifetime)
                Destroy(gameObject);
        }
    }
}
