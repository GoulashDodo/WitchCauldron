using System.Collections.Generic;
using Hut.SO;
using UnityEngine;

namespace Hut.UI.UISelectItems
{
    public class UISelectItemParent : MonoBehaviour
    {
        private List<UISelectItemToggle> _toggles = new();
        
        [SerializeField] private UISelectItemToggle _togglePf;
        
        //TODO: Change this, test purpose only
        [SerializeField] private AllSelectableItems _allItemSettings;

        public void Initialize()
        {

            var allSettings = _allItemSettings.ItemSettings;

            foreach (var setting in allSettings)
            {
                var toggle = Instantiate(_togglePf, gameObject.transform, false);
                toggle.Initialize(setting);
                _toggles.Add(toggle);
            }

            
        }


        public string[] GetSelectedItemsIds()
        {
            var ids = new List<string>();

            foreach (var toggle in _toggles)
            {
                if (toggle.IsSelected)
                {
                    ids.Add(toggle.SettingsTypeId);
                }
            }
            
            
            return ids.ToArray();
        }
        
        
        
    }
}