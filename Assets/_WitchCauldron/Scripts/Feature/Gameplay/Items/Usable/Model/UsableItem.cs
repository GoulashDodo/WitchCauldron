using System;
using Core.GameRoot.Data;
using Feature.Gameplay.Battle.Base;
using Feature.Gameplay.Battle.Base.Core;
using Feature.Gameplay.Items.Model;
using UnityEngine;

namespace Feature.Gameplay.Items.Usable.Model
{
    public class UsableItem : CombinableItem
    {
        
        protected override void OnDrop()
        {
            var contactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = LayerMask.GetMask(Layers.Battleground),
                useTriggers = true
            };
            
            int count = Collider.Overlap(contactFilter, OverlapBuffer);
    
            BattlegroundView best = null;
            float bestSqr = float.MaxValue;
    
            for (int i = 0; i < count; i++)
            {
                var c = OverlapBuffer[i];
                if (c == null) continue;
    
                if (c.transform == Transform) continue;
    
                if (!c.TryGetComponent(out BattlegroundView other)) continue;
    
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
                ItemService.UseItem(this, transform.position);
            }
    
            Array.Clear(OverlapBuffer, 0, count);
    
            base.OnDrop();
            
        }
        

    }
}   