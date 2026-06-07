using Core.Input.Clickable;
using Gameplay.Items.Services;
using R3;
using UnityEngine;
using Zenject;

namespace Gameplay.Items.Spawners
{
    public class ItemSpawner : MonoBehaviour, ILeftButtonPressable
    {

        
        public Observable<Unit> ItemSpawned => _itemSpawned;
        private readonly Subject<Unit> _itemSpawned = new();

        public Observable<Unit> CooldownRestored => _cooldownRestored;
        private readonly Subject<Unit> _cooldownRestored = new();

        public bool CanSpawn => !_isOnCooldown;
        
        
        [SerializeField] private string _itemToSpawnTypeId;
        [SerializeField, Min(0f)] private float _cooldown;
        
        private bool _isOnCooldown;
        private float _cooldownTimer;
        
        private ItemService _service;
        
        [Inject]
        public void Initialize(ItemService service)
        {
            _service = service;
        }
        
        public void OnLeftButtonPressed(Vector3 mousePosition)
        {

            TrySpawnItem(mousePosition);
        }

        private void Update()
        {
            TickCooldown(Time.deltaTime);
        }
        
        private bool TrySpawnItem(Vector3 mousePosition)
        {
            if (!CanSpawn)
                return false;

            if (_service == null)
            {
                Debug.LogWarning($"{nameof(ItemSpawner)} on '{name}' was not injected. Spawn it through Zenject or call Initialize.");
                return false;
            }
            
            if (!_service.TrySpawnDraggableItem(_itemToSpawnTypeId, mousePosition, true))
                return false;

            _itemSpawned.OnNext(Unit.Default);
            StartCooldown();
            return true;
        }

        private void TickCooldown(float deltaTime)
        {
            if (!_isOnCooldown)
                return;

            _cooldownTimer += deltaTime;

            if (_cooldownTimer < _cooldown)
                return;

            RestoreCooldown();
        }

        private void StartCooldown()
        {
            if (_cooldown <= 0f)
                return;

            _isOnCooldown = true;
            _cooldownTimer = 0f;
        }

        private void RestoreCooldown()
        {
            _isOnCooldown = false;
            _cooldownTimer = 0f;
            _cooldownRestored.OnNext(Unit.Default);
        }

        private void OnDestroy()
        {
            _itemSpawned.Dispose();
            _cooldownRestored.Dispose();
        }
    }
}
