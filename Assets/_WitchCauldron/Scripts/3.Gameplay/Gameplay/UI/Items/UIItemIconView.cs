using Gameplay.Items.SO;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Items
{
    public class UIItemIconView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Color _discoveredColor = Color.white;
        [SerializeField] private Color _undiscoveredColor = Color.black;

        private void Awake()
        {
            _image ??= GetComponent<Image>();
        }

        public void Show(ItemSettings item, bool isDiscovered)
        {
            if (_image == null)
                _image = GetComponent<Image>();

            if (_image == null)
                return;

            _image.sprite = item != null ? item.Icon : null;
            _image.color = isDiscovered ? _discoveredColor : _undiscoveredColor;
            _image.enabled = item?.Icon != null;
        }

        public void Clear()
        {
            if (_image == null)
                _image = GetComponent<Image>();

            if (_image == null)
                return;

            _image.sprite = null;
            _image.color = _discoveredColor;
            _image.enabled = false;
        }
    }
}
