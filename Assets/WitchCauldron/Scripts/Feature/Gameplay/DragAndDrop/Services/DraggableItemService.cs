using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Item;

namespace WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Services
{
    public class DraggableItemService
    {

        private readonly MouseClickHandler _mouseClickHandler;
        
        public DraggableItemService(MouseClickHandler mouseClickHandler)
        {
            _mouseClickHandler = mouseClickHandler;
        }
       
        public void SpawnDraggableItem(DraggableItem itemToSpawn, Vector3 initialPosition)
        {
            var item = Object.Instantiate(itemToSpawn, initialPosition, Quaternion.identity);
            
            item.Initialize(_mouseClickHandler.MouseToWorldPosition);
        }
        
    }
}