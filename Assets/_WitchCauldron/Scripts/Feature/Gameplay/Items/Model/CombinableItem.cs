using System;
using _WitchCauldron.Scripts.Core.GameRoot.Data;
using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Items.Model
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
    
                // Перестраховка: иногда Unity может вернуть родственный коллайдер того же объекта (много коллайдеров на одном GO)
                if (c.transform == Transform) continue;
    
                if (!c.TryGetComponent(out CombinableItem other)) continue;
    
                // Выбираем ближайший по точке контакта
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
    
            // Чистим только использованную часть буфера (не обязательно, но удобно для отладки)
            Array.Clear(OverlapBuffer, 0, count);
    
            base.OnDrop();
            
        }
        
    }
}