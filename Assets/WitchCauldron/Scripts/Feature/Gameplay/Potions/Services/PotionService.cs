using System.Collections.Generic;
using ObservableCollections;
using R3;
using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.Cauldrons;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.Services;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Settings;


namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Services
{
    public class PotionService
    {
        
        
        //private readonly Dictionary<string, PotionSettings> _allPotionsSettings =  new(); 
        
        private readonly PotionInventory _inventory;

        public PotionService(CauldronService cauldronService)
        {
            cauldronService.AllCauldrons.ObserveAdd().Subscribe(cauldronE =>
            {
                var cauldron = cauldronE.Value;

                cauldron.BrewingSessionFinished.Subscribe(AddPotion);
            });
            
            _inventory = new PotionInventory(new List<Potion>(), 5);
            
            /*
            foreach (var potionSetting in allPotionsSettings.PotionSettings)
            {
                //_allPotionsSettings.Add(potionSetting.TypeId, potionSetting);
            }
            */
        }


        public void AddPotion(PotionSettings potionSettings)
        {
            
            Debug.Log($"Potion service: Adding potion {potionSettings.TypeId}");
            
            //_inventory.AddToInventory(potion);
        }


        public void UsePotion(int potionIndex, Vector2 positionToUse)
        {
            if (potionIndex >= _inventory.Potions.Count) Debug.Log($"Potion Service: {potionIndex} is out of potions count in inventory.");
            
            var potionToUse = _inventory.UsePotion(potionIndex);
            potionToUse.Use(positionToUse);
        }
        
        
    }
}