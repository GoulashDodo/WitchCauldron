using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Brewing.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Receipt Lists", menuName = "Game/Potions/Brewing/Receipt Lists")]
    public class PotionReceiptList : ScriptableObject
    {
        [field:SerializeField] public BrewingReceipt[] Receipts { get; private set; }


        public BrewingReceipt GetRandomReceipt()
        {
            return Receipts[Random.Range(0, Receipts.Length)];
        }
    }
}