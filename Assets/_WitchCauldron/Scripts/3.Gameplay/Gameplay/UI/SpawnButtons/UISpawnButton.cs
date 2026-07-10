using Gameplay.Items.Services;
using Gameplay.Items.SO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.UI.SpawnButtons
{
    public class UISpawnButton : MonoBehaviour, IPointerDownHandler
    {

        [SerializeField] private Image _icon;
        [SerializeField] private Image _cooldownFill;
        private Button _button;
        
        
        private ItemSettings _itemSettings;
        private ItemService _itemService;

        private bool _isOnCooldown;
        private bool _isPlacementPending;
        private float _cooldownTimer;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }
        
        public void Initialize(
            string settingsId,
            ItemService itemService)
        {
            _itemService = itemService;

            
            _itemSettings = itemService.GetItemSettings(settingsId);
            
            _icon.sprite = _itemSettings.Icon;
            _cooldownFill.fillAmount = 0f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isOnCooldown || _isPlacementPending)
                return;

            if (!TryGetWorldPosition(eventData.position, out var worldPosition))
                return;

            if (!_itemService.TrySpawnPlacementGhost(_itemSettings.TypeId, worldPosition, OnPlacementCompleted))
                return;

            _isPlacementPending = true;
            _button.interactable = false;
        }

        private void OnPlacementCompleted(bool accepted)
        {
            _isPlacementPending = false;

            if (accepted)
            {
                StartCooldown();
                return;
            }

            if (!_isOnCooldown)
                _button.interactable = true;
        }

        private static bool TryGetWorldPosition(Vector2 screenPosition, out Vector3 worldPosition)
        {
            var camera = Camera.main;
            if (camera == null)
            {
                worldPosition = default;
                return false;
            }

            worldPosition = camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0f;
            return true;
        }

        private void Update()
        {
            if (!_isOnCooldown)
                return;

            _cooldownTimer += Time.deltaTime;

            var cooldown = _itemSettings.SpawnCooldown;
            _cooldownFill.fillAmount = 1f - Mathf.Clamp01(_cooldownTimer / cooldown);

            if (_cooldownTimer >= cooldown)
                StopCooldown();
        }

        private void StartCooldown()
        {
            var cooldown = _itemSettings.SpawnCooldown;
            if (cooldown <= 0f)
            {
                _button.interactable = true;
                return;
            }

            _button.interactable = false;
            _isOnCooldown = true;
            _cooldownTimer = 0f;
            _cooldownFill.fillAmount = 1f;
        }

        private void StopCooldown()
        {
            _button.interactable = true;
            _isOnCooldown = false;
            _cooldownTimer = 0f;
            _cooldownFill.fillAmount = 0f;
        }
    }
}
