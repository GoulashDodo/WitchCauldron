using System.Collections.Generic;
using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Settings;


namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Services
{
    public class PotionService
    {
        
        
        //private readonly Dictionary<string, PotionSettings> _allPotionsSettings =  new(); 
        
        private readonly PotionInventory _inventory;

        public PotionService()
        {   
            
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