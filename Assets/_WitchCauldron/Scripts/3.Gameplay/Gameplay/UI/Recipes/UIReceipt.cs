using Gameplay.Items.Combination.ScriptableObjects;
using Gameplay.Items.Knowledge;
using Gameplay.Items.SO;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Recipes
{
    public class UIReceipt : MonoBehaviour
    {


        [SerializeField] private Image _imageFirstItem;
        [SerializeField] private Image _imageSecondItem;
        [SerializeField] private Image _imageResultItem;


        public void Initialize(CombinationRule rule, ItemKnowledgeService knowledgeService)
        {
            gameObject.SetActive(true);
            
            ShowItem(_imageFirstItem, rule.ItemA, knowledgeService);
            ShowItem(_imageSecondItem, rule.ItemB, knowledgeService);
            ShowItem(_imageResultItem, rule.Result, knowledgeService);
        }
     
        public void Clear()
        {
            _imageFirstItem.sprite = null;
            _imageSecondItem.sprite = null;
            
          
            _imageResultItem.sprite = null;
            
            gameObject.SetActive(false);
        }

        private static void ShowItem(Image image, ItemSettings item, ItemKnowledgeService knowledgeService)
        {
            if (image == null)
                return;

            image.sprite = item != null ? item.Icon : null;
            image.color = knowledgeService != null && knowledgeService.IsDiscovered(item)
                ? Color.white
                : Color.black;
            image.enabled = image.sprite != null;
        }
        
    }
}
