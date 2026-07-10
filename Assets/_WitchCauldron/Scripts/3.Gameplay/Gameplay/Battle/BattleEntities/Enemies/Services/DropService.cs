using Gameplay.Battle.BattleEntities.Enemies.Core;
using Gameplay.Battle.BattleEntities.Enemies.SO;
using Gameplay.Items.MonoBehaviours.View;
using Gameplay.Items.Services;
using R3;
using UnityEngine;
using Zenject;

namespace Gameplay.Battle.BattleEntities.Enemies.Services
{
    public class DropService : IInitializable, System.IDisposable
    {
        private const float DropMinXOffset = -0.35f;
        private const float DropMaxXOffset = 0.35f;
        private const float DropMinYOffset = 0.1f;
        private const float DropMaxYOffset = 0.45f;
        
        private readonly EnemyService _enemyService;
        private readonly ItemService _itemService;
        private readonly CompositeDisposable _disposables = new();

        public DropService(EnemyService enemyService, ItemService itemService)
        {
            _enemyService = enemyService;
            _itemService = itemService;
        }

        public void Initialize()
        {
            _enemyService.EnemyDied
                .Subscribe(TryDropLoot)
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void TryDropLoot(Enemy enemy)
        {
            var lootDefinitions = enemy.Settings.LootDefinitions;

            if (lootDefinitions == null || lootDefinitions.Length == 0)
                return;

            foreach (var lootDefinition in lootDefinitions)
            {
                if (!CanDrop(lootDefinition))
                    continue;

                var startPosition = enemy.transform.position;
                if (!_itemService.TrySpawnDraggableItem(lootDefinition.DropItemTypeId, startPosition, out var item))
                    continue;

                var endPosition = GetDropPosition(startPosition);
                var dropFx = item.GetComponent<LootDropFx>();
                if (dropFx == null)
                    dropFx = item.gameObject.AddComponent<LootDropFx>();

                dropFx.Play(startPosition, endPosition);
            }
        }

        
        
        private static Vector3 GetDropPosition(Vector3 origin)
        {
            return origin + new Vector3(
                Random.Range(DropMinXOffset, DropMaxXOffset),
                Random.Range(DropMinYOffset, DropMaxYOffset),
                0f);
        }

        private static bool CanDrop(EnemyLootDefinition lootDefinition)
        {
            if (lootDefinition == null)
                return false;

            if (string.IsNullOrWhiteSpace(lootDefinition.DropItemTypeId))
                return false;

            var chance = Mathf.Clamp01(lootDefinition.ChanceToDropItem);
            return chance >= 1f || chance > 0f && Random.value < chance;
        }
        
    }
}
