using _WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using _WitchCauldron.Scripts.Feature.Gameplay.Items.Services;
using _WitchCauldron.Scripts.Feature.Gameplay.Items.Settings;
using UnityEngine;
using Zenject;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Items.Spawners
{
    public class ItemSpawner : MonoBehaviour, ILeftButtonPressable
    {

        [SerializeField] private ItemSettings _itemSettingsToSpawn;

        private ItemService _service;
        
        [Inject]
        public void Initialize(ItemService service)
        {
            _service = service;
        }
       
        public void OnLeftButtonPressed(Vector3 mousePosition)
        {
            _service.SpawnDraggableItem(_itemSettingsToSpawn, mousePosition, true);
        }

    }
}