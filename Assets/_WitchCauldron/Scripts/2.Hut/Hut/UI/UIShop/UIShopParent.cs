using System.Collections.Generic;
using Core.Run;
using Hut.Shop;
using Hut.Shop.SO;
using UnityEngine;

namespace Hut.UI.UIShop
{
    public class UIShopParent : MonoBehaviour
    {
        private readonly List<UIShopItem> _items = new();

        [SerializeField] private Transform _content;
        [SerializeField] private UIShopItem _itemPrefab;

        private AllShopUpgrades _allShopUpgrades;
        private ShopService _shopService;
        private RunState _runState;

        public void Initialize(AllShopUpgrades allShopUpgrades, ShopService shopService, RunState runState)
        {
            _allShopUpgrades = allShopUpgrades;
            _shopService = shopService;
            _runState = runState;

            Build();
            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Build()
        {
            Clear();

            if (_shopService == null || _allShopUpgrades == null || _allShopUpgrades.Upgrades == null)
                return;

            var content = _content != null ? _content : transform;
            if (_itemPrefab != null)
                _itemPrefab.gameObject.SetActive(false);

            foreach (var upgrade in _allShopUpgrades.Upgrades)
            {
                if (upgrade == null)
                    continue;

                if (upgrade.HideUntilUnlocked && !_shopService.IsUnlocked(upgrade))
                    continue;

                var item = CreateItem(content);
                if (item == null)
                    continue;

                item.Initialize(upgrade, _shopService, Refresh);
                _items.Add(item);
            }
        }

        private UIShopItem CreateItem(Transform content)
        {
            if (_itemPrefab == null)
                return null;

            var item = Instantiate(_itemPrefab, content, false);
            item.gameObject.SetActive(true);
            return item;
        }

        private void Refresh()
        {
            Build();
        }

        private void Clear()
        {
            foreach (var item in _items)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }

            _items.Clear();
        }

        private void Subscribe()
        {
            Unsubscribe();

            if (_runState != null)
                _runState.Wallet.BalanceChanged += RefreshWallet;
        }

        private void Unsubscribe()
        {
            if (_runState != null)
                _runState.Wallet.BalanceChanged -= RefreshWallet;
        }

        private void RefreshWallet(int balance)
        {
            foreach (var item in _items)
                item.Refresh();
        }
    }
}
