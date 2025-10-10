using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Settings
{
    

    public class PotionSettings : ScriptableObject  
    {
        [field: SerializeField] public string TypeId {get; private set;}
        
        
        [field: Space(10)]
        [field: SerializeField] public string TitleLid {get; private set;} 
        
        

    }


}