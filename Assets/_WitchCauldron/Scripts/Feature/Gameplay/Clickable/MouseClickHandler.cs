using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Clickable
{
    public sealed class MouseClickHandler : IInitializable, ITickable, IDisposable
    {
        private readonly GameInput _gameInput;
        private Camera _camera;

        private readonly ReactiveProperty<Vector2> _mouseToWorldPosition = new();
        public Observable<Vector2> MouseToWorldPosition => _mouseToWorldPosition;

        private const int MaxHits = 16;
        private readonly Collider2D[] _hitBuffer = new Collider2D[MaxHits];

        public MouseClickHandler(GameInput gameInput)
        {
            _gameInput = gameInput;
        }

        public void Initialize()
        {
            _camera = Camera.main;

            _gameInput.Gameplay.Enable();
            _gameInput.Gameplay.LeftMousePressed.performed += HandleLeftMousePressed;
            _gameInput.Gameplay.LeftMousePressed.canceled += HandleLeftMouseReleased;
        }

        public void Tick()
        {
            if (TryGetMouseWorldPoint(out var worldPoint))
                _mouseToWorldPosition.Value = worldPoint;
        }

        public void Dispose()
        {
            _gameInput.Gameplay.LeftMousePressed.performed -= HandleLeftMousePressed;
            _gameInput.Gameplay.LeftMousePressed.canceled -= HandleLeftMouseReleased;

            _gameInput.Gameplay.Disable();
            _mouseToWorldPosition.Dispose();
        }

        private void HandleLeftMousePressed(InputAction.CallbackContext _)
        {
            if (!TryGetMouseWorldPoint(out var worldPoint))
                return;

            var hit = Physics2D.OverlapPoint(worldPoint);
            if (hit == null)
                return;

            if (hit.TryGetComponent<ILeftButtonPressable>(out var clickable))
                clickable.OnLeftButtonPressed(worldPoint);
        }

        private void HandleLeftMouseReleased(InputAction.CallbackContext _)
        {
            if (!TryGetMouseWorldPoint(out var worldPoint))
                return;

            var hitCount = Physics2D.OverlapPoint(worldPoint, new ContactFilter2D(), _hitBuffer);

            for (int i = 0; i < hitCount; i++)
            {
                var hit = _hitBuffer[i];
                if (!hit) continue;

                if (hit.TryGetComponent<ILeftButtonReleasable>(out var releasable))
                    releasable.OnLeftButtonReleased(worldPoint);
            }
        }

        private bool TryGetMouseWorldPoint(out Vector2 worldPoint)
        {
            if (_camera == null)
                _camera = Camera.main;

            if (_camera == null || Mouse.current == null)
            {
                worldPoint = default;
                return false;
            }

            var screenPosition = Mouse.current.position.ReadValue();
            var wp = _camera.ScreenToWorldPoint(screenPosition);
            wp.z = 0f;

            worldPoint = wp;
            return true;
        }
    }
}
