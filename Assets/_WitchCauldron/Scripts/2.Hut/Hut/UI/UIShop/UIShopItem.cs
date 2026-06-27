using System;
using Hut.Shop;
using Hut.Shop.SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hut.UI.UIShop
{
    public class UIShopItem : MonoBehaviour
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TMP_Text _itemName;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TMP_Text _buyButtonText;
        [SerializeField] private GameObject _lockObject;

        private ShopUpgradeDefinition _upgrade;
        private ShopService _shopService;
        private Action _refreshRequested;
        
        public void Initialize(ShopUpgradeDefinition upgrade, ShopService shopService, Action refreshRequested)
        {
            _upgrade = upgrade;
            _shopService = shopService;
            _refreshRequested = refreshRequested;

            if (_buyButtonText == null && _buyButton != null)
                _buyButtonText = _buyButton.GetComponentInChildren<TMP_Text>(true);

            if (_buyButton != null)
            {
                _buyButton.onClick.RemoveListener(Buy);
                _buyButton.onClick.AddListener(Buy);
            }

            Refresh();
        }

        private void OnDestroy()
        {
            if (_buyButton != null)
                _buyButton.onClick.RemoveListener(Buy);
        }

        public void Refresh()
        {
            if (_upgrade == null || _shopService == null)
                return;

            var unlocked = _shopService.IsUnlocked(_upgrade);
            var purchased = _shopService.IsPurchased(_upgrade);
            var canBuy = _shopService.CanBuy(_upgrade);

            if (_itemIcon != null)
            {
                _itemIcon.sprite = _upgrade.Icon;
                _itemIcon.enabled = _upgrade.Icon != null;
            }

            if (_itemName != null)
                _itemName.text = _upgrade.DisplayName;

            if (_description != null)
                _description.text = _upgrade.Description;

            if (_price != null)
                _price.text = _upgrade.Price.ToString();

            if (_lockObject != null)
                _lockObject.SetActive(!unlocked);

            if (_buyButton != null)
                _buyButton.interactable = canBuy;

            if (_buyButtonText != null)
                _buyButtonText.text = GetButtonText(unlocked, purchased, canBuy);
        }

        private void Buy()
        {
            if (_upgrade == null || _shopService == null)
                return;

            if (!_shopService.TryBuy(_upgrade))
            {
                Refresh();
                return;
            }

            _refreshRequested?.Invoke();
        }

        private static string GetButtonText(bool unlocked, bool purchased, bool canBuy)
        {
            if (purchased)
                return "Bought";

            if (!unlocked)
                return "Locked";

            return canBuy ? "Buy" : "Need money";
        }
    }
}
