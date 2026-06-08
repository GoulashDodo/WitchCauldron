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
            if (!IsDragging)
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
            var best = FindBestOverlappedCombinableItem(out var count);

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

        private CombinableItem FindBestOverlappedCombinableItem(out int count)
        {
            var contactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = LayerMask.GetMask(Layers.Item),
                useTriggers = true
            };

            count = Collider.Overlap(contactFilter, OverlapBuffer);

            CombinableItem best = null;
            float bestSqr = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                var c = OverlapBuffer[i];
                if (c == null) continue;

                if (c.transform == Transform) continue;

                if (!c.TryGetComponent(out CombinableItem other)) continue;

                Vector2 closest = c.ClosestPoint(Transform.position);
                float sqr = ((Vector2)Transform.position - closest).sqrMagnitude;

                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = other;
                }
            }

            return best;
        }

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
