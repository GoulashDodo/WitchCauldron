using System.Collections.Generic;
using Hut.SO;
using Hut.SelectedItems;
using UnityEngine;

namespace Hut.UI.UISelectItems
{
    public class UISelectItemParent : MonoBehaviour
    {
        private List<UISelectItemToggle> _toggles = new();
        
        [SerializeField] private UISelectItemToggle _togglePf;
        
        //TODO: Change this, test purpose only
        [SerializeField] private AllSelectableItems _allItemSettings;

        private SelectedItemsRuntime _selectedItemsRuntime;

        public void Initialize(SelectedItemsRuntime selectedItemsRuntime)
        {
            _selectedItemsRuntime = selectedItemsRuntime;

            if (_togglePf == null || _allItemSettings == null || _allItemSettings.ItemSettings == null)
                return;

            var allSettings = _allItemSettings.ItemSettings;

            foreach (var setting in allSettings)
            {
                if (setting == null)
                    continue;

                var toggle = Instantiate(_togglePf, gameObject.transform, false);
                toggle.Initialize(setting, _selectedItemsRuntime);
                _toggles.Add(toggle);
            }
        }


        public string[] GetSelectedItemsIds()
        {
            return _selectedItemsRuntime.GetSelectedItemsIds();
        }
    }
}
