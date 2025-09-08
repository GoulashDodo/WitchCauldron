using System;
using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Structures
{
    [Serializable]
    public struct BrewingReceiptPart
    {
        [field:SerializeField] public BrewingIngredient Ingredient{ get; private set; }
        [field:SerializeField] public int Quantity{ get; private set; }
    }
}