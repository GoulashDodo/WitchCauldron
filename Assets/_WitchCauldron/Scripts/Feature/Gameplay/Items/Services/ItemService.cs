using System.Collections.Generic;
using Core.GameRoot.Input.Clickable;
using Feature.Gameplay._root.SO;
using Feature.Gameplay.Items.Combination.Service;
using Feature.Gameplay.Items.Model;
using Feature.Gameplay.Items.SO;
using Feature.Gameplay.Items.Usable.Commands.Processor;
using Feature.Gameplay.Items.Usable.Model;
using UnityEngine;

namespace Feature.Gameplay.Items.Services
{
    public class ItemService
    {

        private readonly IUseCommandProcessor _useCommandProcessor;
        
        private readonly MouseClickHandler _mouseClickHandler;
        private readonly CombinationService _combinationService;
        
        private readonly Dictionary<string, ItemSettings> _allItemSettings = new();        
        
        
        public ItemService(MouseClickHandler mouseClickHandler, 
            CombinationService combinationService, 
            GameplaySettings gameplaySettings, 
            IUseCommandProcessor useCommandProcessor)
        {
            _mouseClickHandler = mouseClickHandler;
            _combinationService = combinationService;
            _useCommandProcessor = useCommandProcessor;


            
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
            
            return item;
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


            foreach (var commandParameters in itemSettings.OnUseCommands)
            {
                _useCommandProcessor.Process(commandParameters, position);
            }
            
            
            
        }
    }
}