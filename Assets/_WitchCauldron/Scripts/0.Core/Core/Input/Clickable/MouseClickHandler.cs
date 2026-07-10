using System;
using Core.Data;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Core.Input.Clickable
{
    public sealed class MouseClickHandler : IInitializable, ITickable, IDisposable
    {
        private readonly GameInput _gameInput;
        private Camera _camera;
        private ILeftButtonReleasable _activeReleasable;

        private readonly ReactiveProperty<Vector2> _mouseToWorldPosition = new();
        public Observable<Vector2> MouseToWorldPosition => _mouseToWorldPosition;

        private const int MaxHits = 16;
        private readonly Collider2D[] _hitBuffer = new Collider2D[MaxHits];

        public MouseClickHandler(GameInput gameInput)
        {
            _gameInput = gameInput;
        }

        public void CaptureRelease(ILeftButtonReleasable releasable)
        {
            _activeReleasable = releasable;
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

            if (!TryGetLeftButtonPressable(worldPoint, out var clickable))
                return;

            clickable.OnLeftButtonPressed(worldPoint);

            if (clickable is ILeftButtonReleasable releasable)
                _activeReleasable = releasable;
        }

        private void HandleLeftMouseReleased(InputAction.CallbackContext _)
        {
            if (!TryGetMouseWorldPoint(out var worldPoint))
                return;

            if (_activeReleasable != null)
            {
                _activeReleasable.OnLeftButtonReleased(worldPoint);
                _activeReleasable = null;
                return;
            }

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

        private bool TryGetLeftButtonPressable(Vector2 worldPoint, out ILeftButtonPressable pressable)
        {
            pressable = null;

            var hitCount = Physics2D.OverlapPoint(worldPoint, new ContactFilter2D(), _hitBuffer);
            if (hitCount == 0)
                return false;

            var itemLayer = LayerMask.NameToLayer(Layers.Item);

            for (int i = 0; i < hitCount; i++)
            {
                var hit = _hitBuffer[i];
                if (!hit || hit.gameObject.layer != itemLayer)
                    continue;

                if (hit.TryGetComponent(out pressable))
                    return true;
            }

            for (int i = 0; i < hitCount; i++)
            {
                var hit = _hitBuffer[i];
                if (!hit)
                    continue;

                if (hit.TryGetComponent(out pressable))
                    return true;
            }

            return false;
        }
    }
}
