using TMPro;
using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.Services;

namespace WitchCauldron.Scripts.Feature.Gameplay.Brewing.UI
{
    public class UIReceiptNamePanel : MonoBehaviour
    {
        
        [SerializeField] private TextMeshProUGUI _nameText;

        private BrewingService _service;
        
        public void Initialize()
        {
        }

        private void DisplayReceiptName(string receiptName)
        {
            _nameText.text = receiptName;
        }
        
    }
}
