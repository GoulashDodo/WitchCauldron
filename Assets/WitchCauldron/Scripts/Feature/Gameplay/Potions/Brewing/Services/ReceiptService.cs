using R3;
using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Commands.Parameters;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Services
{
    public class ReceiptService
    {
        private readonly ICommandProcessor _cmd;
        
        //TODO: Refactor this
        private readonly PotionReceiptList _receiptList;
        
        private readonly ReactiveProperty<BrewingReceipt> _currentReceipt = new();
        
        
        public ReceiptService(ICommandProcessor cmd,  PotionReceiptList receiptList)
        {
            _cmd = cmd;
            _receiptList = receiptList;
            
            _currentReceipt.Subscribe(CreateBrewingSession);
        }
        
        public void SelectRandomReceipt()
        {
            _currentReceipt.Value =  _receiptList.Receipts[
                Random.Range(0, _receiptList.Receipts.Length)
            ]; 
        }
        
        
        private void CreateBrewingSession(BrewingReceipt receipt)
        {

            var cmdParameters = new CmdCreateBrewingSessionParameters(receipt);
            
            _cmd.Process(cmdParameters);
            
        }
        
        
    }
}
