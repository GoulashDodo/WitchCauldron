using System.Collections.Generic;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions
{
    public class PotionInventory
    {
        
        
        private readonly List<Potion> _potions;
        
        public readonly int MaxCapacity;
        public IReadOnlyCollection<Potion> Potions => _potions;
        
        public PotionInventory(List<Potion> initialPotions, int maxCapacity)
        {
            _potions = initialPotions;
            
            MaxCapacity = maxCapacity;

        }
        
        
        public void AddToInventory(Potion potion)
        {
            if (_potions.Count >= MaxCapacity)
            {
                _potions.RemoveAt(0);
            }
            
            _potions.Add(potion);
        }
        
        public Potion UsePotion(int index)
        {
            var  potion = _potions[index];
            _potions.RemoveAt(index);
            return potion;
        }
        
    }
}