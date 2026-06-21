using System.Collections.Generic;
using Core.Input.Clickable;
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
        
        private readonly Dictionary<string, ItemSettings> _allItemSettings = new();
        private readonly GameObject _combineSuccessParticlePf;
        
        public ItemService(MouseClickHandler mouseClickHandler, 
            CombinationService combinationService, 
            GameplaySettings gameplaySettings, 
            IUseCommandProcessor useCommandProcessor,
            IUseCommandPreviewProcessor previewProcessor)
        {
            _mouseClickHandler = mouseClickHandler;
            _combinationService = combinationService;
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
            
            var itemPf = itemSettings.ItemPf;
            var item = Object.Instantiate(itemPf, initialPosition, Quaternion.identity);            
            
            item.Initialize(itemSettings, this, _mouseClickHandler.MouseToWorldPosition);
            InitializeUsePreviewFx(item);

            if (startDragging)
                item.StartDragging();
            
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
                    Object.Instantiate(_combineSuccessParticlePf, midPoint, Quaternion.identity);
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

        public void UseItem(UsableItem usableItem, Vector2 position)
        {
            var itemWorldScale = usableItem.transform.lossyScale;
            DespawnDraggableItem(usableItem);
            
            var itemSettings = _allItemSettings[usableItem.TypeId];
            Debug.Log($"[Item service]: Using {usableItem.TypeId}");

            var context = new UseCommandContext(itemSettings, _fxPlayer, itemWorldScale);

            foreach (var commandParameters in itemSettings.OnUseCommands)
            {
                _useCommandProcessor.Process(commandParameters, position, context);
            }
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
        }

    }
}
