using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Receipt Lists", menuName = "Game/Potions/Brewing/Receipt Lists")]
    public class PotionReceiptList : ScriptableObject
    {
        [field:SerializeField] public PotionReceipt[] Receipts { get; private set; }
    }
}