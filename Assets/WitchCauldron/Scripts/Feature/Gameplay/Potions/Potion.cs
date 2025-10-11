using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Settings;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions
{
    
    public abstract class Potion : MonoBehaviour
    {

        public PotionSettings Settings { get; protected set; }  
        
        public abstract void Use(Vector2 position);


    }
}