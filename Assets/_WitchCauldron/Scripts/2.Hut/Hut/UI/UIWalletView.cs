using Core.Run;
using TMPro;
using UnityEngine;

namespace Hut.UI
{
    public class UIWalletView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private string _format = "{0}";

        private RunState _runState;

        public void Initialize(RunState runState)
        {
            Unsubscribe();

            _runState = runState;

            if (_runState == null)
                return;

            Refresh(_runState.Wallet.Balance);
            _runState.Wallet.BalanceChanged += Refresh;
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Refresh(int balance)
        {
            if (_moneyText == null)
                return;

            _moneyText.text = string.Format(_format, balance);
        }

        private void Unsubscribe()
        {
            if (_runState == null)
                return;

            _runState.Wallet.BalanceChanged -= Refresh;
            _runState = null;
        }
    }
}
