using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Services;
using Zenject;

namespace WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Item.Spawners
{
    public class ItemSpawner : MonoBehaviour, ILeftButtonPressable
    {

        [SerializeField] private DraggableItem _itemToSpawn;

        private DraggableItemService _service;
        
        [Inject]
        public void Initialize(DraggableItemService service)
        {
            _service = service;
        }
       
        public void OnLeftButtonPressed(Vector3 mousePosition)
        {
            
            _service.SpawnDraggableItem(_itemToSpawn, transform.position);
            
        }

    }
}