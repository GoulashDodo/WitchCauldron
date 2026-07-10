using System.Collections.Generic;
using Core.Run;
using Gameplay._root.SO;
using Gameplay.Items.Combination.ScriptableObjects;
using Gameplay.Items.Knowledge;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Recipes
{
    public class UIReceiptParent : MonoBehaviour
    {
        
        [SerializeField] private UIReceipt[] _recipeSlots;

        [SerializeField] private Button _previousButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private TMP_Text _pageText;
        
        [SerializeField] private CombinationRuleList _combinationRuleList;

        private readonly List<CombinationRule> _unlockedRules = new();
        private ItemKnowledgeService _knowledgeService;

        private const int RecipesPerPage = 6;
        private int _currentPage;
        private int TotalPages => Mathf.CeilToInt(_unlockedRules.Count / (float)RecipesPerPage);
        
        public void Initialize(RunState runState, GameplaySettings gameplaySettings)
        {
            _previousButton.onClick.AddListener(PreviousPage);
            _nextButton.onClick.AddListener(NextPage);

            _unlockedRules.Clear();
            var ruleList = gameplaySettings != null && gameplaySettings.CombinationRuleList != null
                ? gameplaySettings.CombinationRuleList
                : _combinationRuleList;

            _knowledgeService = new ItemKnowledgeService(
                gameplaySettings?.AllItemsSettings,
                ruleList,
                runState);

            if (ruleList?.Rules == null)
            {
                ShowPage(0);
                return;
            }

            foreach (var rule in ruleList.Rules)
            {
                if (rule != null && runState.UnlockedRecipes.HasRecipe(rule.RecipeId))
                    _unlockedRules.Add(rule);
            }

            ShowPage(0);
        }
        
        private void ShowPage(int page)
        {
            _currentPage = Mathf.Clamp(page, 0, Mathf.Max(0, TotalPages - 1));

            int startIndex = _currentPage * RecipesPerPage;

            for (int i = 0; i < _recipeSlots.Length; i++)
            {
                int recipeIndex = startIndex + i;

                if (recipeIndex < _unlockedRules.Count)
                    _recipeSlots[i].Initialize(_unlockedRules[recipeIndex], _knowledgeService);
                else
                    _recipeSlots[i].Clear();
            }

            _previousButton.interactable = _currentPage > 0;
            _nextButton.interactable = _currentPage < TotalPages - 1;

            _pageText.text = TotalPages > 0 ? $"{_currentPage + 1} / {TotalPages}" : "0 / 0";
        }
        
        private void PreviousPage()
        {
            ShowPage(_currentPage - 1);
        }

        private void NextPage()
        {
            ShowPage(_currentPage + 1);
        }
        
    }
}
