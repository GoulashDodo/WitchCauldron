using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Services;
using Zenject;

namespace WitchCauldron.Scripts.Common.Utilits.Debugging
{
    public sealed class DebugReceiptServiceInvoker : MonoBehaviour
    {
        private ReceiptService _service;

        
        public void Initialize(ReceiptService service)
        {
            _service = service;
        }


        public void SelectRandomReceipt()
        {
            if (_service == null)
            {
                Debug.LogWarning("[ReceiptServiceInvoker] Service is not set.");
                return;
            }

            _service.SelectRandomReceipt();
        }

#if UNITY_EDITOR
        [ContextMenu("Brewing/Select Random Receipt")]
        private void Ctx_SelectRandomReceipt()
        {
            SelectRandomReceipt();
        }
#endif
    }
}
