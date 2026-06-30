using System;
using System.Collections.Generic;
using Core.Input.Clickable;
using Core.Run;
using Gameplay._root.SO;
using Gameplay.Items.Combination.Service;
using Gameplay.Items.MonoBehaviours;
using Gameplay.Items.MonoBehaviours.View;
using Gameplay.Items.SO;
using Gameplay.Items.Usable.Commands;
using Gameplay.Items.Usable.Commands.Preview;
using Gameplay.Items.Usable.Commands.Processor;
using Gameplay.Items.Visuals;
using UnityEngine;

namespace Gameplay.Items.Services
{
    public class ItemService
    {

        private readonly IUseCommandProcessor _useCommandProcessor;
        private readonly IUseCommandPreviewProcessor _previewProcessor;
        private readonly ItemUseFxPlayer _fxPlayer = new();
        
        private readonly MouseClickHandler _mouseClickHandler;
        private readonly CombinationService _combinationService;
        private readonly RunState _runState;
        
        private readonly Dictionary<string, ItemSettings> _allItemSettings = new();
        private readonly GameObject _combineSuccessParticlePf;
        
        public ItemService(MouseClickHandler mouseClickHandler, 
            CombinationService combinationService, 
            GameplaySettings gameplaySettings, 
            RunState runState,
            IUseCommandProcessor useCommandProcessor,
            IUseCommandPreviewProcessor previewProcessor)
        {
            _mouseClickHandler = mouseClickHandler;
            _combinationService = combinationService;
            _runState = runState;
            _useCommandProcessor = useCommandProcessor;
            _previewProcessor = previewProcessor;


            
            var allItemSettings = gameplaySettings.AllItemsSettings;
            
            foreach (var setting in allItemSettings.ItemSettings)
            {
                _allItemSettings.Add(setting.TypeId, setting);
            }
            _combineSuccessParticlePf = allItemSettings.CombineSuccessPrefab;
            
        }
        
        
        public DraggableItem SpawnDraggableItem(string itemTypeId,  Vector3 initialPosition, bool startDragging = false)
        {
            
            var itemSettings = _allItemSettings[itemTypeId];
            _runState.DiscoveredItems.DiscoverItem(itemTypeId);
            
            var itemPf = itemSettings.ItemPf;
            var item = UnityEngine.Object.Instantiate(itemPf, initialPosition, Quaternion.identity);            
            
            item.Initialize(itemSettings, this, _mouseClickHandler.MouseToWorldPosition);
            InitializeUsePreviewFx(item);

            if (startDragging)
            {
                item.StartDragging();

                if (item is ILeftButtonReleasable releasable)
                    _mouseClickHandler.CaptureRelease(releasable);
            }
            
            return item;
        }  

        public bool TrySpawnDraggableItem(string itemTypeId, Vector3 initialPosition, bool startDragging = false)
        {
            return TrySpawnDraggableItem(itemTypeId, initialPosition, out _, startDragging);
        }

        public bool TrySpawnDraggableItem(
            string itemTypeId,
            Vector3 initialPosition,
            out DraggableItem item,
            bool startDragging = false)
        {
            if (string.IsNullOrWhiteSpace(itemTypeId) || !_allItemSettings.ContainsKey(itemTypeId))
            {
                Debug.LogWarning($"[Item service]: Item settings with type id '{itemTypeId}' were not found.");
                item = null;
                return false;
            }

            item = SpawnDraggableItem(itemTypeId, initialPosition, startDragging);
            return true;
        }

        public bool TrySpawnPlacementGhost(string itemTypeId, Vector3 initialPosition, Action<bool> completed)
        {
            if (string.IsNullOrWhiteSpace(itemTypeId) || !_allItemSettings.TryGetValue(itemTypeId, out var itemSettings))
            {
                Debug.LogWarning($"[Item service]: Item settings with type id '{itemTypeId}' were not found.");
                return false;
            }

            var sourceRenderer = itemSettings.ItemPf != null
                ? itemSettings.ItemPf.GetComponentInChildren<SpriteRenderer>()
                : null;

            if (sourceRenderer == null || sourceRenderer.sprite == null)
                return false;

            var ghostObject = new GameObject($"{itemSettings.TypeId}_PlacementGhost");
            ghostObject.transform.position = initialPosition;
            ghostObject.transform.localScale = sourceRenderer.transform.lossyScale;

            var renderer = ghostObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sourceRenderer.sprite;
            renderer.color = new Color(sourceRenderer.color.r, sourceRenderer.color.g, sourceRenderer.color.b, 0.55f);
            renderer.sortingLayerID = sourceRenderer.sortingLayerID;
            renderer.sortingOrder = Mathf.Max(sourceRenderer.sortingOrder, 1000);
            renderer.flipX = sourceRenderer.flipX;
            renderer.flipY = sourceRenderer.flipY;

            var collider = ghostObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = sourceRenderer.sprite.bounds.size;

            var ghost = ghostObject.AddComponent<ItemSpawnGhost>();
            ghost.Initialize(itemSettings, this, _mouseClickHandler.MouseToWorldPosition, _previewProcessor, completed);
            _mouseClickHandler.CaptureRelease(ghost);

            return true;
        }
        
        
        private void DespawnDraggableItem(DraggableItem item)
        {
            if (item == null)
                return;

            item.Dispose();
            item.gameObject.SetActive(false);
        }
        
        public bool TryCombineItems(CombinableItem item, CombinableItem otherItem)
        {
            
            var selfSettings = _allItemSettings[item.TypeId];
            var otherSettings = _allItemSettings[otherItem.TypeId];
            
            var result = _combinationService.TryCombine(selfSettings, otherSettings);

            if (result != null)
            {
                
                Debug.Log($"[Item service]: Combining {item.TypeId} and {otherItem.TypeId}");
                var midPoint = (item.gameObject.transform.position + otherItem.gameObject.transform.position) / 2;
                
                if (_combineSuccessParticlePf != null)
                {
                    UnityEngine.Object.Instantiate(_combineSuccessParticlePf, midPoint, Quaternion.identity);
                }

                var resultItem = SpawnDraggableItem(result.TypeId, midPoint);
                var draggableItemFx = resultItem.GetComponentInChildren<DraggableItemFx>();
                if (draggableItemFx != null)
                    draggableItemFx.PlaySpawnPop();

                DespawnDraggableItem(item);
                DespawnDraggableItem(otherItem);
                
                return true;
            }
            
            return false;
        }

        public bool CanCombineItems(CombinableItem item, CombinableItem otherItem)
        {
            if (item == null || otherItem == null)
                return false;

            var selfSettings = _allItemSettings[item.TypeId];
            var otherSettings = _allItemSettings[otherItem.TypeId];

            return _combinationService.TryCombine(selfSettings, otherSettings) != null;
        }

        public bool TryUseItem(UsableItem usableItem, Vector2 position)
        {
            if (usableItem == null)
                return false;

            var itemWorldScale = usableItem.transform.lossyScale;
            var itemSettings = _allItemSettings[usableItem.TypeId];
            if (itemSettings.OnUseCommands == null || itemSettings.OnUseCommands.Length == 0)
                return false;

            Debug.Log($"[Item service]: Using {usableItem.TypeId}");

            var context = new UseCommandContext(itemSettings, _fxPlayer, itemWorldScale);
            var used = false;

            foreach (var commandParameters in itemSettings.OnUseCommands)
            {
                used |= _useCommandProcessor.Process(commandParameters, position, context);
            }

            if (!used)
            {
                _fxPlayer.PlayImpactFx(position, itemSettings, itemWorldScale);
            }

            DespawnDraggableItem(usableItem);
            return true;
        }
        
        public ItemSettings GetItemSettings(string itemTypeId)
        {
            return _allItemSettings[itemTypeId];
        }
        
        private void InitializeUsePreviewFx(DraggableItem item)
        {
            if (item is not UsableItem usableItem)
                return;

            if (!usableItem.TryGetComponent(out UsableItemPreviewFx previewFx))
                previewFx = usableItem.gameObject.AddComponent<UsableItemPreviewFx>();

            previewFx.Initialize(_previewProcessor);

            if (!usableItem.TryGetComponent<UsableItemMissFx>(out _))
                usableItem.gameObject.AddComponent<UsableItemMissFx>();
        }

    }
}
