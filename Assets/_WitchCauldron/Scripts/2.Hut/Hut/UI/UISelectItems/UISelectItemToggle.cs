using Gameplay.Items.SO;
using Hut.SelectedItems;
using UnityEngine;
using UnityEngine.UI;

namespace Hut.UI.UISelectItems
{
    
    [RequireComponent(typeof(Toggle))]
    public class UISelectItemToggle : MonoBehaviour
    {
        
        [SerializeField] private Image _image;
        private Toggle _toggle;
        private SelectedItemsRuntime _selectedItemsRuntime;
        
        public bool IsSelected => _toggle.isOn;
        public string SettingsTypeId { get; private set; }


        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
        }
        
        private void OnDestroy()
        {
            if (_toggle != null)
                _toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        public void Initialize(ItemSettings itemSettings, SelectedItemsRuntime selectedItemsRuntime)
        {
            if (_toggle == null)
                _toggle = GetComponent<Toggle>();

            if (itemSettings == null || selectedItemsRuntime == null || _toggle == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _selectedItemsRuntime = selectedItemsRuntime;
            SettingsTypeId = itemSettings.TypeId;

            if (_image != null)
                _image.sprite = itemSettings.Icon;

            _toggle.SetIsOnWithoutNotify(_selectedItemsRuntime.IsSelected(SettingsTypeId));
            _toggle.onValueChanged.RemoveListener(OnValueChanged);
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(bool isOn)
        {
            if (_selectedItemsRuntime == null)
                return;

            if (_selectedItemsRuntime.SetSelected(SettingsTypeId, isOn))
                return;

            _toggle.SetIsOnWithoutNotify(false);
        }
    }
}
