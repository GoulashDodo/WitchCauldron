using System;
using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Structures
{
    [Serializable]
    public struct PotionReceiptPart
    {
        [field:SerializeField] public PotionIngredient Ingredient{ get; private set; }
        [field:SerializeField] public int Quantity{ get; private set; }
    }
}