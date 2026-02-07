using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Clickable
{
    public sealed class MouseClickHandler : IInitializable, ITickable, IDisposable
    {
        private const int DefaultMaxHits = 16;

        private readonly GameInput _gameInput;
        private readonly Camera _camera;

        private readonly Collider2D[] _hits;
        private readonly ContactFilter2D _filter;

        private readonly ReactiveProperty<Vector2> _mouseToWorldPosition = new();
        public Observable<Vector2> MouseToWorldPosition => _mouseToWorldPosition;

        
        public MouseClickHandler(
            GameInput gameInput,
            [InjectOptional] Camera camera = null,
            int maxHits = DefaultMaxHits,
            LayerMask clickableMask = default,
            bool includeTriggers = true)
        {
            _gameInput = gameInput;
            _camera = camera != null ? camera : Camera.main;

            _hits = new Collider2D[maxHits];

            _filter = new ContactFilter2D();
            _filter.useTriggers = includeTriggers;

            if (clickableMask.value == 0)
                clickableMask = Physics2D.DefaultRaycastLayers;

            _filter.SetLayerMask(clickableMask);
            _filter.useLayerMask = true;

         
        }

        public void Initialize()
        {
            _gameInput.Gameplay.Enable();
            _gameInput.Gameplay.LeftMousePressed.performed += HandleLeftMousePressed;
            _gameInput.Gameplay.LeftMousePressed.canceled += HandleLeftMouseReleased;
        }

        public void Tick()
        {
            if (!TryGetMouseWorldPoint(out var worldPoint))
                return;

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

            Dispatch(worldPoint,
                onCollider: (col) =>
                {
                    if (col.TryGetComponent<ILeftButtonPressable>(out var pressable))
                        pressable.OnLeftButtonPressed(worldPoint);
                });
        }

        private void HandleLeftMouseReleased(InputAction.CallbackContext _)
        {
            if (!TryGetMouseWorldPoint(out var worldPoint))
                return;

            Dispatch(worldPoint,
                onCollider: (col) =>
                {
                    if (col.TryGetComponent<ILeftButtonReleasable>(out var releasable))
                        releasable.OnLeftButtonReleased(worldPoint);
                });
        }

        private void Dispatch(Vector2 worldPoint, Action<Collider2D> onCollider)
        {
            var hitCount = Physics2D.OverlapPoint(worldPoint, _filter, _hits);

            for (int i = 0; i < hitCount; i++)
            {
                var hit = _hits[i];
                if (!hit) continue;
                onCollider(hit);
            }
        }

        private bool TryGetMouseWorldPoint(out Vector2 worldPoint)
        {
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
