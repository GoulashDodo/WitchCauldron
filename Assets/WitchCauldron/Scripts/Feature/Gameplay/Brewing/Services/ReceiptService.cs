using WitchCauldron.Scripts.Feature.Gameplay.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Brewing.Services
{
    public class ReceiptService
    {
        
		private readonly PotionReceiptList _receiptList;
        
        public ReceiptService(PotionReceiptList receiptList)
        {
            _receiptList = receiptList;
        }
        
        public BrewingReceipt GetRandomBrewingReceipt()
        {

            var randomReceipt = _receiptList.GetRandomReceipt();
            return randomReceipt;
        }
        
    }
}