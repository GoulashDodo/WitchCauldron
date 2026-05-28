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
            
        }
        
        
        public DraggableItem SpawnDraggableItem(string itemTypeId,  Vector3 initialPosition, bool startDragging = false)
        {
            
            var itemSettings = _allItemSettings[itemTypeId];
            
            var itemPf = itemSettings.ItemPf;
            var item = Object.Instantiate(itemPf, initialPosition, Quaternion.identity);            
            
            item.Initialize(itemSettings, this, _mouseClickHandler.MouseToWorldPosition, startDragging);
            InitializeUsePreviewFx(item);
            
            return item;
        }  

        public bool TrySpawnDraggableItem(string itemTypeId, Vector3 initialPosition, bool startDragging = false)
        {
            if (string.IsNullOrWhiteSpace(itemTypeId) || !_allItemSettings.ContainsKey(itemTypeId))
            {
                Debug.LogWarning($"[Item service]: Item settings with type id '{itemTypeId}' were not found.");
                return false;
            }

            SpawnDraggableItem(itemTypeId, initialPosition, startDragging);
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
                
                SpawnDraggableItem(result.TypeId, midPoint);
                DespawnDraggableItem(item);
                DespawnDraggableItem(otherItem);
                
                return true;
            }
            
            return false;
        }

        public void UseItem(UsableItem usableItem, Vector2 position)
        {
            DespawnDraggableItem(usableItem);
            
            var itemSettings = _allItemSettings[usableItem.TypeId];
            Debug.Log($"[Item service]: Using {usableItem.TypeId}");

            var context = new UseCommandContext(itemSettings, _fxPlayer);

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
