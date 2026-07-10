using System;
using Core.Data;
using Gameplay.Battle.Base.Core;
using Gameplay.Items.Combination;
using UnityEngine;

namespace Gameplay.Items.MonoBehaviours
{
    public static class ItemPlacementQuery
    {
        public static bool IsInCombineZone(Collider2D itemCollider, Transform self, Collider2D[] buffer)
        {
            if (itemCollider == null || buffer == null)
                return false;

            var count = itemCollider.Overlap(new ContactFilter2D { useTriggers = true }, buffer);

            for (var i = 0; i < count; i++)
            {
                var c = buffer[i];
                if (c == null)
                    continue;

                if (self != null && c.transform == self)
                    continue;

                if (c.GetComponentInParent<CombineZone>() != null)
                {
                    Array.Clear(buffer, 0, count);
                    return true;
                }
            }

            Array.Clear(buffer, 0, count);
            return false;
        }

        public static bool CanUseOnBattleground(Collider2D itemCollider, Transform self, Collider2D[] buffer)
        {
            if (itemCollider == null || buffer == null)
                return false;

            var contactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = LayerMask.GetMask(Layers.Battleground),
                useTriggers = true
            };

            var count = itemCollider.Overlap(contactFilter, buffer);

            for (var i = 0; i < count; i++)
            {
                var c = buffer[i];
                if (c == null)
                    continue;

                if (self != null && c.transform == self)
                    continue;

                if (c.TryGetComponent(out BattlegroundView _))
                {
                    Array.Clear(buffer, 0, count);
                    return true;
                }
            }

            Array.Clear(buffer, 0, count);
            return false;
        }

        public static CombinableItem FindBestOverlappedCombinableItem(
            Collider2D itemCollider,
            Transform self,
            Collider2D[] buffer,
            out int count)
        {
            count = 0;

            if (itemCollider == null || buffer == null)
                return null;

            var contactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = LayerMask.GetMask(Layers.Item),
                useTriggers = true
            };

            count = itemCollider.Overlap(contactFilter, buffer);

            CombinableItem best = null;
            var bestSqr = float.MaxValue;

            for (var i = 0; i < count; i++)
            {
                var c = buffer[i];
                if (c == null)
                    continue;

                if (self != null && c.transform == self)
                    continue;

                if (!c.TryGetComponent(out CombinableItem other))
                    continue;

                var closest = c.ClosestPoint(self != null ? self.position : itemCollider.transform.position);
                var sqr = ((Vector2)(self != null ? self.position : itemCollider.transform.position) - closest).sqrMagnitude;

                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = other;
                }
            }

            return best;
        }
    }
}
