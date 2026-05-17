using System;
using Core.GameRoot.Input.Clickable;
using Feature.Gameplay.Items.Services;
using Feature.Gameplay.Items.SO;
using R3;
using UnityEngine;

namespace Feature.Gameplay.Items.Model
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
        
        
        private Subject<Unit> _pickedUp = new Subject<Unit>();
        private Subject<Unit> _dropped = new Subject<Unit>();
        
        public Observable<Unit> PickedUp => _pickedUp;
        public Observable<Unit> Dropped => _dropped;
        
        
        
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
            _pickedUp.OnNext(Unit.Default);
            _isDragging = true;  
        }
        
        protected virtual void OnDrop()
        {
            _dropped.OnNext(Unit.Default);
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
