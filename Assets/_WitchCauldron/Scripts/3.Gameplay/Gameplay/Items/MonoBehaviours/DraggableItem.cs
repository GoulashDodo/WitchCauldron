using System;
using Core.Input.Clickable;
using Gameplay.Items.Services;
using Gameplay.Items.SO;
using R3;
using UnityEngine;

namespace Gameplay.Items.MonoBehaviours
{
    public class DraggableItem : UnityEngine.MonoBehaviour, ILeftButtonPressable,ILeftButtonReleasable, IDisposable
    {
        public string TypeId { get; private set; }
        public ItemSettings Settings { get; private set; }
        public bool IsDragging => _isDragging;

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
            Settings = itemSettings;
            TypeId = itemSettings.TypeId;
            ItemService = itemService;
            
            _positionSubscription = grabbedPosition.Subscribe(position =>
            {
                if (_isDragging)
                {
                    Transform.position = position;
                }
            });
        }
        
        
        protected virtual void Awake()
        {
            Transform = transform;
            Collider = GetComponent<Collider2D>();

            OverlapBuffer = new Collider2D[8];
            
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void StartDragging()
        {
            Drag();
        }

        private void Drag()
        {
            if (_isDragging)
                return;

            _isDragging = true;
            _pickedUp.OnNext(Unit.Default);
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
