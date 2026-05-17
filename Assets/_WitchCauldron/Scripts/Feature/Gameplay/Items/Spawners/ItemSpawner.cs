using Core.GameRoot.Input.Clickable;
using Feature.Gameplay.Items.Services;
using UnityEngine;
using Zenject;

namespace Feature.Gameplay.Items.Spawners
{
    public class ItemSpawner : MonoBehaviour, ILeftButtonPressable
    {

        [SerializeField] private string _itemToSpawnTypeId;

        private ItemService _service;
        
        [Inject]
        public void Initialize(ItemService service)
        {
            _service = service;
        }
       
        public void OnLeftButtonPressed(Vector3 mousePosition)
        {
            _service.SpawnDraggableItem(_itemToSpawnTypeId, mousePosition, true);
        }

    }
}