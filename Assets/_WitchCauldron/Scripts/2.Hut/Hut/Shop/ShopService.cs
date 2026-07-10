using Core.Audio;
using Core.Run;
using Hut.Shop.SO;

namespace Hut.Shop
{
    public class ShopService
    {
        private readonly RunState _runState;
        private readonly AudioService _audioService;

        public ShopService(RunState runState, AudioService audioService)
        {
            _runState = runState;
            _audioService = audioService;
        }

        public bool IsPurchased(ShopUpgradeDefinition upgrade)
        {
            return upgrade != null && _runState.PurchasedShopUpgrades.HasUpgrade(upgrade.UpgradeId);
        }

        public bool IsUnlocked(ShopUpgradeDefinition upgrade)
        {
            if (upgrade == null)
                return false;

            var conditions = upgrade.UnlockConditions;
            if (conditions == null || conditions.Length == 0)
                return true;

            foreach (var condition in conditions)
            {
                if (!IsConditionMet(condition))
                    return false;
            }

            return true;
        }

        public bool CanBuy(ShopUpgradeDefinition upgrade)
        {
            return upgrade != null &&
                   !IsPurchased(upgrade) &&
                   IsUnlocked(upgrade) &&
                   _runState.Wallet.CanSpend(upgrade.Price);
        }

        public bool TryBuy(ShopUpgradeDefinition upgrade)
        {
            if (!CanBuy(upgrade))
            {
                _audioService.PlayUi(AudioId.Shop_Failed);
                return false;
            }

            if (!_runState.Wallet.TrySpend(upgrade.Price))
            {
                _audioService.PlayUi(AudioId.Shop_Failed);
                return false;
            }

            _runState.PurchasedShopUpgrades.MarkPurchased(upgrade.UpgradeId);
            ApplyEffects(upgrade.Effects);
            _audioService.PlayUi(AudioId.Shop_Buy);
            return true;
        }

        private bool IsConditionMet(ShopUpgradeUnlockCondition condition)
        {
            switch (condition.Type)
            {
                case ShopUpgradeUnlockConditionType.CompletedLevelIndex:
                    return _runState.Progress.HighestCompletedLevelIndex >= condition.Amount;
                case ShopUpgradeUnlockConditionType.PurchasedUpgrade:
                    return _runState.PurchasedShopUpgrades.HasUpgrade(condition.TargetId);
                default:
                    return false;
            }
        }

        private void ApplyEffects(ShopUpgradeEffect[] effects)
        {
            if (effects == null)
                return;

            foreach (var effect in effects)
                ApplyEffect(effect);
        }

        private void ApplyEffect(ShopUpgradeEffect effect)
        {
            switch (effect.Type)
            {
                case ShopUpgradeEffectType.UnlockSelectableItem:
                    _runState.UnlockedSelectableItems.UnlockNewItem(effect.TargetId);
                    _runState.DiscoveredItems.DiscoverItem(effect.TargetId);
                    _audioService.PlayUi(AudioId.Unlock_Item);
                    break;
                case ShopUpgradeEffectType.UnlockRecipe:
                    _runState.UnlockedRecipes.UnlockCombination(effect.TargetId);
                    _audioService.PlayUi(AudioId.Unlock_Recipe);
                    break;
                case ShopUpgradeEffectType.IncreaseBaseMaxHealth:
                    _runState.BaseHealth.AddMaxHealth(effect.Amount);
                    break;
                case ShopUpgradeEffectType.IncreaseSelectedItemsCapacity:
                    _runState.SelectedItemsCapacity.AddCapacity(effect.Amount);
                    break;
            }
        }
    }
}
