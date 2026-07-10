using Gameplay.Battle.Familiars.SO;
using Hut.SelectedItems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hut.SelectFamiliars
{
    [RequireComponent(typeof(Toggle))]
    public class UISelectFamiliarToggle : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _name;

        [SerializeField] private Toggle _toggle;
        private SelectedFamiliarRuntime _selectedFamiliarRuntime;

        public string FamiliarTypeId { get; private set; }

        private void OnDestroy()
        {
            if (_toggle != null)
                _toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        public void Initialize(
            FamiliarData familiarData,
            ToggleGroup toggleGroup,
            SelectedFamiliarRuntime selectedFamiliarRuntime)
        {
            if (_toggle == null)
                _toggle = GetComponent<Toggle>();

            if (familiarData == null || selectedFamiliarRuntime == null || _toggle == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _selectedFamiliarRuntime = selectedFamiliarRuntime;
            FamiliarTypeId = familiarData.FamiliarTypeId;

            _toggle.group = toggleGroup;
            _toggle.SetIsOnWithoutNotify(_selectedFamiliarRuntime.SelectedFamiliarId == FamiliarTypeId);

            if (_image != null)
                _image.sprite = familiarData.FamiliarIcon;

            if (_name != null)
                _name.text = familiarData.FamiliarName;


            _toggle.onValueChanged.RemoveListener(OnValueChanged);
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(bool isOn)
        {
            if (_selectedFamiliarRuntime == null)
                return;

            if (isOn)
            {
                _selectedFamiliarRuntime.Select(FamiliarTypeId);
                return;
            }

            if (_selectedFamiliarRuntime.SelectedFamiliarId == FamiliarTypeId)
                _selectedFamiliarRuntime.Clear();
        }

        public void SetSelectedWithoutNotify(bool isSelected)
        {
            if (_toggle == null)
                _toggle = GetComponent<Toggle>();

            if (_toggle != null)
                _toggle.SetIsOnWithoutNotify(isSelected);
        }
    }
}
