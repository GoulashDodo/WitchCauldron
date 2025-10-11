using System;
using R3;
using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Item.Settings;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Services;

namespace WitchCauldron.Scripts.Feature.Gameplay.Items.Item.Model
{
    public class DraggableItem : MonoBehaviour, ILeftButtonPressable,ILeftButtonReleasable, IDisposable
    {
        public string TypeId { get; private set; }

        protected ItemService ItemService;
        
        
        private bool _isDragging;
        private IDisposable _positionSubscription;


        
        protected Transform Transform;
        protected Collider2D Collider;

        
        protected Collider2D[] OverlapBuffer;
        
        public void Initialize(
            ItemSettings itemSettings,
            ItemService itemService,
            Observable<Vector2> grabbedPosition,
            bool startDragging = false
        )
        {
            TypeId = itemSettings.TypeId;
            ItemService = itemService;

            if (startDragging)  Drag();
            
            _positionSubscription = grabbedPosition.Subscribe(position =>
            {
                if (_isDragging)
                {
                    Transform.position = position;
                }
            });
        }
        
        
        private void Awake()
        {
            Transform = transform;
            Collider = GetComponent<Collider2D>();

            OverlapBuffer = new Collider2D[8];
            
            

        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void Drag()
        {
            _isDragging = true;  
        }
        
        protected virtual void OnDrop()
        {
            _isDragging = false;
        }
        
        public void Dispose()
        {
            _positionSubscription?.Dispose();
        }

        public void OnLeftButtonPressed(Vector3 mousePosition) => Drag();
        
        public void OnLeftButtonReleased(Vector3 mousePosition) => OnDrop();



        
    }
}
