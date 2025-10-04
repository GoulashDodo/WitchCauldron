using System;
using System.Linq;
using R3;
using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.Data;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.Cauldrons;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.ScriptableObjects;
using WitchCauldron.Scripts.Feature.Gameplay.Clickable;

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
            
            float radius = 0.5f; 
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position,
                radius,
                LayerMask.GetMask(Layers.Cauldron));

            if (hits != null)
            {

                Cauldron[] cauldrons = { };
                
                foreach (Collider2D hit in hits)
                {
                    cauldrons = hit.gameObject.GetComponents<Cauldron>();
                }
                
                if (cauldrons.Length != 0)
                {
                    cauldrons.FirstOrDefault()?.TryAddItem(_brewingIngredient);
                }
                
            }

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
