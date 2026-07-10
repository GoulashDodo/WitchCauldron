using Gameplay.Items.Knowledge;
using Gameplay.Items.SO;
using Gameplay.UI.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Hut.UI.UIAlmanac
{
    [RequireComponent(typeof(Toggle))]
    public class UIAlmanacItemToggle : MonoBehaviour
    {
        [SerializeField] private UIItemIconView _iconView;
        [SerializeField] private GameObject _unviewedMarker;
        [SerializeField] private GameObject _lockObject;

        private Toggle _toggle;
        private ItemSettings _item;
        private ItemKnowledgeService _knowledgeService;
        private System.Action<UIAlmanacItemToggle> _selected;

        public ItemSettings Item => _item;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
        }

        private void OnDestroy()
        {
            if (_toggle != null)
                _toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        public void Initialize(
            ItemSettings item,
            ToggleGroup toggleGroup,
            ItemKnowledgeService knowledgeService,
            System.Action<UIAlmanacItemToggle> selected)
        {
            _toggle ??= GetComponent<Toggle>();
            _item = item;
            _knowledgeService = knowledgeService;
            _selected = selected;

            if (_toggle != null)
            {
                _toggle.group = toggleGroup;
                _toggle.SetIsOnWithoutNotify(false);
                _toggle.onValueChanged.RemoveListener(OnValueChanged);
                _toggle.onValueChanged.AddListener(OnValueChanged);
            }

            Refresh();
        }

        public void SelectWithoutNotify()
        {
            _toggle ??= GetComponent<Toggle>();
            _toggle?.SetIsOnWithoutNotify(true);
        }

        public void Refresh()
        {
            if (_item == null || _knowledgeService == null)
                return;

            var isAvailable = _knowledgeService.IsAvailable(_item);
            var isDiscovered = _knowledgeService.IsDiscovered(_item);

            _iconView?.Show(_item, isDiscovered);

            if (_toggle != null)
                _toggle.interactable = isAvailable;

            if (_unviewedMarker != null)
                _unviewedMarker.SetActive(isAvailable && isDiscovered && !_knowledgeService.IsAlmanacViewed(_item));

            if (_lockObject != null)
                _lockObject.SetActive(!isAvailable);
        }

        private void OnValueChanged(bool isOn)
        {
            if (isOn)
                _selected?.Invoke(this);
        }
    }
}
