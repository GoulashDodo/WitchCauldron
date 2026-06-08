using Gameplay.Items.Combination.ScriptableObjects;
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

        private const int RecipesPerPage = 6;
        private int _currentPage;
        private int TotalPages => Mathf.CeilToInt(_combinationRuleList.Rules.Count / (float)RecipesPerPage);
        
        public void Initialize()
        {
            _previousButton.onClick.AddListener(PreviousPage);
            _nextButton.onClick.AddListener(NextPage);

            ShowPage(0);
        }
        
        private void ShowPage(int page)
        {
            _currentPage = Mathf.Clamp(page, 0, Mathf.Max(0, TotalPages - 1));

            int startIndex = _currentPage * RecipesPerPage;

            for (int i = 0; i < _recipeSlots.Length; i++)
            {
                int recipeIndex = startIndex + i;

                if (recipeIndex < _combinationRuleList.Rules.Count)
                    _recipeSlots[i].Initialize(_combinationRuleList.Rules[recipeIndex]);
                else
                    _recipeSlots[i].Clear();
            }

            _previousButton.interactable = _currentPage > 0;
            _nextButton.interactable = _currentPage < TotalPages - 1;

            _pageText.text = $"{_currentPage + 1} / {TotalPages}";
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
