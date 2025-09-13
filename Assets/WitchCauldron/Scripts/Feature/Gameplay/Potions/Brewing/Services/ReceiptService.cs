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
        
        private readonly Subject<BrewingReceipt> _currentReceiptChanged = new();
        
        
        public ReceiptService(ICommandProcessor cmd,  PotionReceiptList receiptList)
        {
            _cmd = cmd;
            _receiptList = receiptList;
            
            _currentReceiptChanged.Skip(1).Subscribe(receipt => CreateBrewingSession(receipt));
        }
        
        public void SelectRandomReceipt()
        {
            _currentReceiptChanged.OnNext(_receiptList.Receipts[
                Random.Range(0, _receiptList.Receipts.Length)
            ]);
        }
        
        
        private bool CreateBrewingSession(BrewingReceipt receipt)
        {

            var cmdParameters = new CmdCreateBrewingSessionParameters(receipt);
            
            var result = _cmd.Process(cmdParameters);


            return result;
        }
        
        
    }
}
