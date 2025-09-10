using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Structures;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Potion Receipt", menuName = "Game/Potions/Brewing/Reciept")]
    public class BrewingReceipt : ScriptableObject
    {
        
        [field: SerializeField] public string Name { get; private set; }
        
        [field: SerializeField] public BrewingReceiptPart[] Parts { get; private set; }
        
    }
}