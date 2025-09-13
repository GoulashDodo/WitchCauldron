using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WitchCauldron.Scripts.Feature.Gameplay.Clickable
{
    public class MouseClickHandler : IDisposable
    {
        private readonly GameInput _gameInput;
        private readonly Camera _camera;
        
        private readonly ReactiveProperty<Vector2> _mouseToWorldPosition = new();
        public Observable<Vector2> MouseToWorldPosition => _mouseToWorldPosition;
        
        
        public MouseClickHandler(GameInput gameInput)
        {

            _gameInput = gameInput;
            _camera = Camera.main;

            _gameInput.Gameplay.LeftMousePressed.performed += HandleLeftMousePressed;
            _gameInput.Gameplay.LeftMousePressed.canceled += HandleLeftMouseReleased;

            _gameInput.Gameplay.MouseMoved.performed += HandleMouseMoved;
        }

        private void HandleMouseMoved(InputAction.CallbackContext ctx)
        {
            var screenPosition = Mouse.current.position.ReadValue();
            var worldPoint = _camera.ScreenToWorldPoint(screenPosition);
            
            _mouseToWorldPosition.Value = worldPoint;
        }


        private void HandleLeftMousePressed(InputAction.CallbackContext context)
        {

            var screenPosition = Mouse.current.position.ReadValue();
            var worldPoint = _camera.ScreenToWorldPoint(screenPosition);

            var hit = Physics2D.OverlapPoint(worldPoint);
            if (hit == null) return;

            if (hit.TryGetComponent<ILeftButtonPressable>(out var clickable))
            {
                clickable.OnLeftButtonPressed(worldPoint);
            }
        }

        private void HandleLeftMouseReleased(InputAction.CallbackContext context)
        {
            var screenPosition = Mouse.current.position.ReadValue();
            var worldPoint = _camera.ScreenToWorldPoint(screenPosition);

            var hit = Physics2D.OverlapPoint(worldPoint);
            if (hit == null) return;


            if (hit.TryGetComponent<ILeftButtonReleasable>(out var clickable))
            {
                clickable.OnLeftButtonReleased(worldPoint);
            }
        }

        
        public void Dispose()
        {
            _gameInput.Gameplay.LeftMousePressed.performed -= HandleLeftMousePressed;
            _gameInput.Gameplay.LeftMousePressed.canceled -= HandleLeftMouseReleased;
            
            _gameInput.Gameplay.MouseMoved.performed -= HandleMouseMoved;
        }
    }
}