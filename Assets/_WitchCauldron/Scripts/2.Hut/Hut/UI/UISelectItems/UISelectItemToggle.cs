using Gameplay.Items.SO;
using UnityEngine;
using UnityEngine.UI;

namespace Hut.UI.UISelectItems
{
    
    [RequireComponent(typeof(Toggle))]
    public class UISelectItemToggle : MonoBehaviour
    {
        
        [SerializeField] private Image _image;
        private Toggle _toggle;
        
        
        public bool IsSelected => _toggle.isOn;
        public string SettingsTypeId { get; private set; }


        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
        }
        
        public void Initialize(ItemSettings itemSettings)
        {
            SettingsTypeId = itemSettings.TypeId;

            _image.sprite = itemSettings.Icon;
            
        }
        
        
        
    }
}
