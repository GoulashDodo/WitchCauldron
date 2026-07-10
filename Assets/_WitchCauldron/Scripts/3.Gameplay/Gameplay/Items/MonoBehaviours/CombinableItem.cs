using System;
using Core.Data;
using R3;
using UnityEngine;

namespace Gameplay.Items.MonoBehaviours
{
    public readonly struct CombineTargetState
    {
        public readonly CombinableItem Target;
        public readonly bool CanCombine;

        public CombineTargetState(CombinableItem target, bool canCombine)
        {
            Target = target;
            CanCombine = canCombine;
        }
    }

    public class CombinableItem : DraggableItem 
    {
        
        private readonly Subject<CombinableItem> _combineFailed = new();
        private readonly Subject<CombineTargetState> _combineTargetChanged = new();

        public Observable<CombinableItem> CombineFailed => _combineFailed;
        public Observable<CombineTargetState> CombineTargetChanged => _combineTargetChanged;

        private CombinableItem _currentCombineTarget;
        private bool _currentTargetCanCombine;

        private void Update()
        {
            if (!IsDragging || !IsInCombineZone())
            {
                SetCurrentCombineTarget(null, false);
                return;
            }

            var best = FindBestOverlappedCombinableItem(out var count);
            var canCombine = best != null && ItemService.CanCombineItems(this, best);

            SetCurrentCombineTarget(best, canCombine);
            Array.Clear(OverlapBuffer, 0, count);
        }
        
        protected override void OnDrop()
        {
            var isInCombineZone = IsInCombineZone();
            var count = 0;
            var best = isInCombineZone ? FindBestOverlappedCombinableItem(out count) : null;

            if (best != null)
            { 
                var combined = ItemService.TryCombineItems(this, best);

                if (!combined)
                    _combineFailed.OnNext(best);
            }
    
            Array.Clear(OverlapBuffer, 0, count);
            SetCurrentCombineTarget(null, false);
    
            base.OnDrop();
            
        }

        protected bool IsInCombineZone()
        {
            return ItemPlacementQuery.IsInCombineZone(Collider, Transform, OverlapBuffer);
        }

        private CombinableItem FindBestOverlappedCombinableItem(out int count)
            => ItemPlacementQuery.FindBestOverlappedCombinableItem(Collider, Transform, OverlapBuffer, out count);

        private void SetCurrentCombineTarget(CombinableItem target, bool canCombine)
        {
            if (_currentCombineTarget == target && _currentTargetCanCombine == canCombine)
                return;

            _currentCombineTarget = target;
            _currentTargetCanCombine = canCombine;
            _combineTargetChanged.OnNext(new CombineTargetState(target, canCombine));
        }
        
    }
}
