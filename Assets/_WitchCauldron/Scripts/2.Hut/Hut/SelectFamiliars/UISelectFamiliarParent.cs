using Gameplay.Battle.Familiars.SO;
using Hut.SelectedItems;
using UnityEngine;
using UnityEngine.UI;

namespace Hut.SelectFamiliars
{
    [RequireComponent(typeof(ToggleGroup))]
    public class UISelectFamiliarParent : MonoBehaviour
    {
        [SerializeField] private UISelectFamiliarToggle _togglePf;
        [SerializeField] private AllFamiliarsData _allFamiliarsData;
        [SerializeField] private bool _allowSwitchOff;

        private ToggleGroup _toggleGroup;

        private void Awake()
        {
            _toggleGroup = GetComponent<ToggleGroup>();
        }

        public void Initialize(SelectedFamiliarRuntime selectedFamiliarRuntime)
        {
            if (_toggleGroup == null)
                _toggleGroup = GetComponent<ToggleGroup>();

            if (_allFamiliarsData == null || _allFamiliarsData.AllData == null)
                return;

            _toggleGroup.allowSwitchOff = _allowSwitchOff;

            var hasSelectedFamiliar = selectedFamiliarRuntime.HasSelectedFamiliar;

            foreach (var familiarData in _allFamiliarsData.AllData)
            {
                if (familiarData == null)
                    continue;

                var toggle = Instantiate(_togglePf, transform, false);
                toggle.Initialize(familiarData, _toggleGroup, selectedFamiliarRuntime);

                if (!_allowSwitchOff && !hasSelectedFamiliar)
                {
                    selectedFamiliarRuntime.Select(familiarData.FamiliarTypeId);
                    toggle.SetSelectedWithoutNotify(true);
                    hasSelectedFamiliar = true;
                }
            }
        }
    }
}
