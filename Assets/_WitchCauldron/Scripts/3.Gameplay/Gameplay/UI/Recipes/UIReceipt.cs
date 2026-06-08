using Gameplay.Items.Combination.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Recipes
{
    public class UIReceipt : MonoBehaviour
    {


        [SerializeField] private Image _imageFirstItem;
        [SerializeField] private Image _imageSecondItem;
        [SerializeField] private Image _imageResultItem;


        public void Initialize(CombinationRule rule)
        {
            gameObject.SetActive(true);
            
            _imageFirstItem.sprite = rule.ItemA.Icon;
            _imageSecondItem.sprite = rule.ItemB.Icon;
            _imageResultItem.sprite = rule.Result.Icon;
        }
     
        public void Clear()
        {
            _imageFirstItem.sprite = null;
            _imageSecondItem.sprite = null;
            _imageResultItem.sprite = null;
            
            gameObject.SetActive(false);
        }
        
    }
}
