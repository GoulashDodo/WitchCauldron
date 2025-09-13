using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Item;
using WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Services;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Spawners
{
    public class ItemSpawner : MonoBehaviour, ILeftButtonPressable
    {

        [SerializeField] private DraggableItem _itemToSpawn;

        private DraggableItemService _service;
        
        
        public void Initialize(DraggableItemService service)
        {
            _service = service;
        }
       
        public void OnLeftButtonPressed(Vector3 mousePosition)
        {
            
            _service.TrySpawnDraggableItem(_itemToSpawn, transform.position);
            
        }

    }
}