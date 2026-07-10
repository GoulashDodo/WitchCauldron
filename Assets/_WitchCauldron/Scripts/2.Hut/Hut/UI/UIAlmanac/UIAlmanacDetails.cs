using System.Collections.Generic;
using Gameplay.Items.Knowledge;
using Gameplay.Items.SO;
using Gameplay.UI.Items;
using TMPro;
using UnityEngine;

namespace Hut.UI.UIAlmanac
{
    public class UIAlmanacDetails : MonoBehaviour
    {
        [SerializeField] private UIItemIconView _iconView;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private Transform _recipesContainer;
        [SerializeField] private UIAlmanacRecipeRow _recipeRowPrefab;
        [SerializeField] private string _unknownText = "???";
        [SerializeField] private string _priceFormat = "{0}";

        private readonly List<UIAlmanacRecipeRow> _recipeRows = new();

        public void Show(ItemSettings item, ItemKnowledgeService knowledgeService)
        {
            if (item == null || knowledgeService == null)
            {
                Clear();
                return;
            }

            gameObject.SetActive(true);

            var isDiscovered = knowledgeService.IsDiscovered(item);
            _iconView?.Show(item, isDiscovered);

            if (_titleText != null)
                _titleText.text = isDiscovered ? item.TitleLid : _unknownText;

            if (_descriptionText != null)
                _descriptionText.text = isDiscovered ? item.DescriptionLid : _unknownText;

            if (_priceText != null)
                _priceText.text = isDiscovered ? string.Format(_priceFormat, item.Price) : _unknownText;

            ShowRecipes(item, knowledgeService);
        }

        public void Clear()
        {
            _iconView?.Clear();

            if (_titleText != null)
                _titleText.text = string.Empty;

            if (_descriptionText != null)
                _descriptionText.text = string.Empty;

            if (_priceText != null)
                _priceText.text = string.Empty;

            ClearRecipes();
            gameObject.SetActive(false);
        }

        private void ShowRecipes(ItemSettings item, ItemKnowledgeService knowledgeService)
        {
            ClearRecipes();

            if (_recipesContainer == null || _recipeRowPrefab == null)
                return;

            var recipes = knowledgeService.GetUnlockedRecipesForResult(item);

            foreach (var recipe in recipes)
            {
                var row = Instantiate(_recipeRowPrefab, _recipesContainer, false);
                row.Initialize(recipe, knowledgeService);
                _recipeRows.Add(row);
            }
        }

        private void ClearRecipes()
        {
            foreach (var row in _recipeRows)
            {
                if (row != null)
                    Destroy(row.gameObject);
            }

            _recipeRows.Clear();
        }
    }
}
