using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Services;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Settings;
using Zenject;

namespace WitchCauldron.Scripts.Feature.Gameplay.Items.Spawners
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