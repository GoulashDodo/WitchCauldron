using R3;
using UnityEngine;

namespace Feature.Gameplay.Items.Model
{
    
    [RequireComponent(typeof(DraggableItem))]
    public class DraggableItemFx : MonoBehaviour
    {
        
        private readonly CompositeDisposable _disposables = new();
        
        private DraggableItem _item;

        private SpriteRenderer _spriteRenderer;



        private int _sortingOrderBuffer;

        private void Awake()
        {
            _item = GetComponent<DraggableItem>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            _item.PickedUp.Subscribe(OnPickedUp).AddTo(_disposables);
            _item.Dropped.Subscribe(OnDrop).AddTo(_disposables);
        }

        private void OnDisable()
        {
            _disposables.Dispose();
        }

        
        private void OnPickedUp(Unit _)
        {
            _sortingOrderBuffer = _spriteRenderer.sortingOrder;
            _spriteRenderer.sortingOrder = 999;
        }


        private void OnDrop(Unit _)
        {
            _spriteRenderer.sortingOrder = _sortingOrderBuffer;
        }
        
        
        
        
        
    }
}