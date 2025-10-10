using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Brewing.ScriptableObjects.Structures;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Settings;

namespace WitchCauldron.Scripts.Feature.Gameplay.Brewing.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Potion Receipt", menuName = "Game/Brewing/Reciept")]
    public class BrewingReceipt : ScriptableObject
    {
        
        [field: SerializeField] public string TypeId {get;  private set;}
        
        
        
        [field: Space(10)] [field : Header("Description")] 
        [field: SerializeField] public string TitleLid { get; private set; }
        
        
        [field: Space(10)] [field : Header("Result")] 
        
        [field : SerializeField] public PotionSettings ResultPotionSettings{ get; private set; }
        
        [field: Space(10)] [field: Header("Ingredients")]
        [field: SerializeField] public BrewingReceiptPart[] Parts { get; private set; }
    }
}