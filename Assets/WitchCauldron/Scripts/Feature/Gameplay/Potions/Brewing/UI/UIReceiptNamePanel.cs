using R3;
using TMPro;
using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.UI
{
    public class UIReceiptNamePanel : MonoBehaviour
    {
        
        [SerializeField] private TextMeshProUGUI _nameText;

        public void Initialze()
        {
            //cauldronViewModel.BrewingSession.Skip(1).Subscribe(session =>  DisplayReceiptName(session.Receipt.Name));
        }

        private void DisplayReceiptName(string receiptName)
        {
            _nameText.text = receiptName;
        }
        
    }
}
