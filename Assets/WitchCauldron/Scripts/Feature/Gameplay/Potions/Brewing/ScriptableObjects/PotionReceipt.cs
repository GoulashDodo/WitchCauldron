using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Structures;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Potion Receipt", menuName = "Game/Potions/Brewing/Reciept")]
    public class PotionReceipt : ScriptableObject
    {
        
        [field: SerializeField] public PotionReceiptPart[] Parts { get; private set; }
        
    }
}