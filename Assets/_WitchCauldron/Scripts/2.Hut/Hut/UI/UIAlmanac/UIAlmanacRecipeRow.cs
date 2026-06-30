using Gameplay.Items.Combination.ScriptableObjects;
using Gameplay.Items.Knowledge;
using Gameplay.UI.Items;
using UnityEngine;

namespace Hut.UI.UIAlmanac
{
    public class UIAlmanacRecipeRow : MonoBehaviour
    {
        [SerializeField] private UIItemIconView _firstItemIcon;
        [SerializeField] private UIItemIconView _secondItemIcon;

        public void Initialize(CombinationRule rule, ItemKnowledgeService knowledgeService)
        {
            if (rule == null || knowledgeService == null)
            {
                Clear();
                return;
            }

            gameObject.SetActive(true);
            _firstItemIcon?.Show(rule.ItemA, knowledgeService.IsDiscovered(rule.ItemA));
            _secondItemIcon?.Show(rule.ItemB, knowledgeService.IsDiscovered(rule.ItemB));
        }

        public void Clear()
        {
            _firstItemIcon?.Clear();
            _secondItemIcon?.Clear();
            gameObject.SetActive(false);
        }
    }
}
