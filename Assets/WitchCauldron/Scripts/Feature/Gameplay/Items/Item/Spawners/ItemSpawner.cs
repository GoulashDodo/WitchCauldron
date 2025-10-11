using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Item.Settings;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Services;
using Zenject;

namespace WitchCauldron.Scripts.Feature.Gameplay.Items.Item.Spawners
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