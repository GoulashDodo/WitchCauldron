using System;
using Core.GameRoot.Data;
using UnityEngine;

namespace Feature.Gameplay.Items.Model
{
    public class CombinableItem : DraggableItem 
    {
        protected override void OnDrop()
        {
            var contactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = LayerMask.GetMask(Layers.Item),
                useTriggers = true
            };
            
            
            int count = Collider.Overlap(contactFilter, OverlapBuffer);
    
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
    
            if (best != null)
            {
                ItemService.TryCombineItems(this, best);
            }
    
            Array.Clear(OverlapBuffer, 0, count);
    
            base.OnDrop();
            
        }
        
    }
}