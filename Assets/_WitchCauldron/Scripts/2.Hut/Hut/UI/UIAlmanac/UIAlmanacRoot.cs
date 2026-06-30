using System.Collections.Generic;
using Core.Run;
using Gameplay._root.SO;
using Gameplay.Items.Knowledge;
using Gameplay.Items.SO;
using UnityEngine;
using UnityEngine.UI;

namespace Hut.UI.UIAlmanac
{
    public class UIAlmanacRoot : MonoBehaviour
    {
        [SerializeField] private Transform _itemsContainer;
        [SerializeField] private UIAlmanacItemToggle _itemTogglePrefab;
        [SerializeField] private ToggleGroup _toggleGroup;
        [SerializeField] private UIAlmanacDetails _details;
        [SerializeField] private Button _closeButton;

        private readonly List<UIAlmanacItemToggle> _toggles = new();
        private ItemKnowledgeService _knowledgeService;

        private void OnDestroy()
        {
            if (_closeButton != null)
                _closeButton.onClick.RemoveListener(Hide);
        }

        public void Initialize(GameplaySettings gameplaySettings, RunState runState)
        {
            if (gameplaySettings == null || runState == null)
                return;

            _knowledgeService = new ItemKnowledgeService(
                gameplaySettings.AllItemsSettings,
                gameplaySettings.CombinationRuleList,
                runState);

            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(Hide);
                _closeButton.onClick.AddListener(Hide);
            }

            RebuildItems();
            Hide();
        }

        public void Show()
        {
            RefreshItems();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void RebuildItems()
        {
            ClearItems();

            if (_itemsContainer == null || _itemTogglePrefab == null || _knowledgeService == null)
                return;

            foreach (var item in _knowledgeService.GetAllItems())
            {
                if (item == null)
                    continue;

                var toggle = Instantiate(_itemTogglePrefab, _itemsContainer, false);
                toggle.Initialize(item, _toggleGroup, _knowledgeService, Select);
                _toggles.Add(toggle);
            }
        }

        private void Select(UIAlmanacItemToggle toggle)
        {
            if (toggle?.Item == null || _knowledgeService == null)
                return;

            if (!_knowledgeService.IsAvailable(toggle.Item))
                return;

            if (_knowledgeService.IsDiscovered(toggle.Item))
                _knowledgeService.MarkAlmanacViewed(toggle.Item);

            toggle.SelectWithoutNotify();
            _details?.Show(toggle.Item, _knowledgeService);
            RefreshItems();
        }

        private void ClearItems()
        {
            foreach (var toggle in _toggles)
            {
                if (toggle != null)
                    Destroy(toggle.gameObject);
            }

            _toggles.Clear();
        }

        private void RefreshItems()
        {
            foreach (var itemToggle in _toggles)
                itemToggle.Refresh();
        }
    }
}
