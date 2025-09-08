using System.Collections.Generic;
using R3;
using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Services
{
    public class ReceiptService
    {
        private readonly ICommandProcessor _cmd;
        
        //TODO: Refactor this
        private readonly PotionReceiptList _receiptList;
        
        private readonly ReactiveProperty<BrewingReceipt> _currentReceipt = new();
        
        private readonly Subject<List<BrewingIngredient>> _ingredientsSelected = new();

        public Observable<List<BrewingIngredient>> IngredientsSelected => _ingredientsSelected;
        
        
        
        public ReceiptService(ICommandProcessor cmd,  PotionReceiptList receiptList)
        {
            _cmd = cmd;
            _receiptList = receiptList;
            
            _currentReceipt.Subscribe(SetIngredientsList);
        }
        
        public void SelectRandomReceipt()
        {
            _currentReceipt.Value =  _receiptList.Receipts[
                Random.Range(0, _receiptList.Receipts.Length)
            ]; 
        }
        
        public void SelectRandomOtherReceipt()
        {
            if (_receiptList.Receipts.Length <= 1)
            {
                return;
            }

            BrewingReceipt newReceipt;
            do
            {
                newReceipt = _receiptList.Receipts[
                    Random.Range(0, _receiptList.Receipts.Length)
                ];
            }
            while (newReceipt == _currentReceipt.CurrentValue);

            _currentReceipt.Value = newReceipt;
        }
        private void SetIngredientsList(BrewingReceipt receipt)
        {
            var ingredients = new List<BrewingIngredient>();
            

            foreach (var part in receipt.Parts)
            {
                for (int i = 0; i < part.Quantity; i++)
                {
                    ingredients.Add(part.Ingredient);
                }
            }

            _ingredientsSelected.OnNext(ingredients);
            
        }
        
        
    }
}
