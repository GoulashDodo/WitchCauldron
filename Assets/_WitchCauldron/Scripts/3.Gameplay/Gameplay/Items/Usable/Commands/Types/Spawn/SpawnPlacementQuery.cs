using Gameplay._root.SO;
using Gameplay.Battle.BattleEntities.Friendly.Core;
using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Spawn
{
    public static class SpawnPlacementQuery
    {
        public static bool CanSpawnAt(Vector2 position, GameplaySettings gameplaySettings, Collider2D[] buffer)
        {
            var minDistance = gameplaySettings != null
                ? Mathf.Max(0f, gameplaySettings.SpawnedObjectMinDistance)
                : 0f;

            if (minDistance <= 0f)
                return true;

            if (buffer == null || buffer.Length == 0)
                return false;

            var contactFilter = new ContactFilter2D
            {
                useTriggers = true
            };

            var count = Physics2D.OverlapCircle(position, minDistance, contactFilter, buffer);

            for (var i = 0; i < count; i++)
            {
                var hit = buffer[i];
                if (hit == null)
                    continue;

                var friendly = hit.GetComponentInParent<FriendlyAttackableEntity>();
                if (friendly == null || friendly.IsDead || !friendly.gameObject.activeInHierarchy)
                    continue;

                return false;
            }

            return true;
        }
    }
}
