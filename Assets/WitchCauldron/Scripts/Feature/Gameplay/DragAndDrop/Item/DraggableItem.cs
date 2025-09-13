using System;
using R3;
using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Item
{
    public class DraggableItem : MonoBehaviour, ILeftButtonReleasable, IDisposable
    {
        [SerializeField] private BrewingIngredient _brewingIngredient;

        private Transform _transform;
        private bool _isDragging;
        private IDisposable _positionSubscription;

        private void Awake()
        {
            _transform = transform;
        }

        public void Initialize(Observable<Vector2> grabbedPosition)
        {
            _isDragging = true;

            _positionSubscription = grabbedPosition.Subscribe(position =>
            {
                if (_isDragging)
                {
                    _transform.position = position;
                }
            });
        }

        private void Drop()
        {

            _isDragging = false;
            _positionSubscription?.Dispose();

            Destroy(gameObject);
        }

        public void OnLeftButtonReleased(Vector3 mousePosition)
        {
            Drop();
        }

        public void Dispose()
        {
            _positionSubscription?.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}
