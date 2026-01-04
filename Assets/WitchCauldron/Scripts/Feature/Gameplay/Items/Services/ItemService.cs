using System.Collections.Generic;
using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using WitchCauldron.Scripts.Feature.Gameplay.Combination.Service;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Model;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Settings;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Commands.Processor;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Model;

namespace WitchCauldron.Scripts.Feature.Gameplay.Items.Services
{
    public class ItemService
    {

        private readonly IUseCommandProcessor _useCommandProcessor;
        
        private readonly MouseClickHandler _mouseClickHandler;
        private readonly CombinationService _combinationService;
        
        private readonly Dictionary<string, ItemSettings> _allItemSettings;        
        
        
        public ItemService(MouseClickHandler mouseClickHandler, CombinationService combinationService, AllItemSettings allItemSettings, IUseCommandProcessor useCommandProcessor)
        {
            _mouseClickHandler = mouseClickHandler;
            _combinationService = combinationService;
            _useCommandProcessor = useCommandProcessor;


            _allItemSettings = new Dictionary<string, ItemSettings>();
            foreach (var setting in allItemSettings.ItemSettings)
            {
                _allItemSettings.Add(setting.TypeId, setting);
            }
            
        }
        
        
        public DraggableItem SpawnDraggableItem(ItemSettings itemSettings,  Vector3 initialPosition, bool startDragging = false)
        {
            
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
                
                SpawnDraggableItem(result, midPoint);
                DespawnDraggableItem(item);
                DespawnDraggableItem(otherItem);
                
                return true;
            }
            
            return false;
        }

        public void UseItem(UsableItem usableItem, Vector2 position)
        {
            var itemSettings = _allItemSettings[usableItem.TypeId];
            Debug.Log($"[Item service]: Using {usableItem.TypeId}");


            foreach (var commandParameters in itemSettings.OnUseCommands)
            {
                _useCommandProcessor.Process(commandParameters, position);
            }
            
            DespawnDraggableItem(usableItem);
            
            
        }
    }
}