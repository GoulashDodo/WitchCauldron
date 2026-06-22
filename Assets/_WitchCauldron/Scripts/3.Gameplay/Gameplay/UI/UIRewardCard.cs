using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class UIRewardCard : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _countText;

        public void Initialize(Sprite icon, int count = 1)
        {
            gameObject.SetActive(true);

            _icon.sprite = icon;
        
            var shouldShowCount = count > 1;
            _countText.gameObject.SetActive(shouldShowCount);
            _countText.text = shouldShowCount ? count.ToString() : string.Empty;
        }
    }
}
