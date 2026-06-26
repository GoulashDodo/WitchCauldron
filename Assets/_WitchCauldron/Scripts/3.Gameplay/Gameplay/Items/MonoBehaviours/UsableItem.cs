using R3;
using UnityEngine;

namespace Gameplay.Items.MonoBehaviours
{
    public class UsableItem : CombinableItem
    {
        private readonly Subject<Unit> _useMissed = new();

        public Observable<Unit> UseMissed => _useMissed;
        
        protected override void OnDrop()
        {
            if (IsInCombineZone())
            {
                base.OnDrop();
                return;
            }

            if (!CanUseAtCurrentPosition())
            {
                CompleteDrop();
                _useMissed.OnNext(Unit.Default);
                return;
            }

            if (ItemService.TryUseItem(this, transform.position))
            {
                CompleteDrop();
                return;
            }

            CompleteDrop();
            _useMissed.OnNext(Unit.Default);
        }

        public bool CanUseAtCurrentPosition()
        {
            return ItemPlacementQuery.CanUseOnBattleground(Collider, Transform, OverlapBuffer);
        }

    }
}   
