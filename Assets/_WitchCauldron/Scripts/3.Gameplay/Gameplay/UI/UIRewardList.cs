using Core.Run;
using Gameplay._root.SO;
using Gameplay.Items.SO;
using UnityEngine;

namespace Gameplay.UI
{
    public class UIRewardList : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private UIRewardCard _rewardCardPrefab;
        [SerializeField] private Sprite _recipeIcon;

        public void Initialize(UnlockReward[] rewards, GameplaySettings gameplaySettings)
        {
            Clear();

            if (rewards == null || rewards.Length == 0 || _content == null || _rewardCardPrefab == null)
                return;

            var recipeCount = 0;

            foreach (var reward in rewards)
            {
                if (reward.Type == UnlockRewardType.Recipe)
                {
                    recipeCount++;
                    continue;
                }

                if (reward.Type == UnlockRewardType.SelectableItem && TryGetItemIcon(reward.UnlockId, gameplaySettings, out var itemIcon))
                    CreateCard(itemIcon);
            }

            if (recipeCount > 0)
                CreateCard(_recipeIcon, recipeCount);
        }

        private void Clear()
        {
            if (_content == null)
                return;

            for (var i = _content.childCount - 1; i >= 0; i--)
                Destroy(_content.GetChild(i).gameObject);
        }

        private void CreateCard(Sprite icon, int count = 1)
        {
            var card = Instantiate(_rewardCardPrefab, _content, false);
            card.Initialize(icon, count);
        }

        private static bool TryGetItemIcon(string itemId, GameplaySettings gameplaySettings, out Sprite icon)
        {
            icon = null;

            var allItems = gameplaySettings != null ? gameplaySettings.AllItemsSettings : null;
            if (string.IsNullOrWhiteSpace(itemId) || allItems == null || allItems.ItemSettings == null)
                return false;

            foreach (var itemSettings in allItems.ItemSettings)
            {
                if (itemSettings == null || itemSettings.TypeId != itemId)
                    continue;

                icon = itemSettings.Icon;
                return true;
            }

            return false;
        }
    }
}
