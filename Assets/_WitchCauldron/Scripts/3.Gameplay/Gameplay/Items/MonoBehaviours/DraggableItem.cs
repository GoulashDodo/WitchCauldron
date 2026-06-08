using System;
using Core.Input.Clickable;
using Gameplay.Items.Services;
using Gameplay.Items.SO;
using R3;
using UnityEngine;

namespace Gameplay.Items.MonoBehaviours
{
    public class DraggableItem : MonoBehaviour, ILeftButtonPressable,ILeftButtonReleasable, IDisposable
    {
        public string TypeId { get; private set; }
        public ItemSettings Settings { get; private set; }
        public bool IsDragging { get; private set; }

        protected ItemService ItemService;


        private IDisposable _positionSubscription;


        
        protected Transform Transform;
        protected Collider2D Collider;

        
        protected Collider2D[] OverlapBuffer;
        
        
        private readonly Subject<Unit> _pickedUp = new();
        private readonly Subject<Unit> _dropped = new();
        
        public Observable<Unit> PickedUp => _pickedUp;
        public Observable<Unit> Dropped => _dropped;

        private Vector2 _dragOffset;
        
        
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
                if (IsDragging)
                {
                    Transform.position = position + _dragOffset;
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
            if (IsDragging)
                return;

            IsDragging = true;
            _pickedUp.OnNext(Unit.Default);
        }
        
        protected virtual void OnDrop()
        {
            _dropped.OnNext(Unit.Default);
            IsDragging = false;
        }
        
        public void Dispose()
        {
            _positionSubscription?.Dispose();
        }

        public void OnLeftButtonPressed(Vector3 mousePosition)
        {
            
            _dragOffset = new Vector2 (Transform.position.x -  mousePosition.x, Transform.position.y - mousePosition.y);
            Drag();
        }

        public void OnLeftButtonReleased(Vector3 mousePosition) => OnDrop();
        
        
    }

}
