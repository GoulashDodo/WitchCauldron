using System;
using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Brewing.ScriptableObjects.Structures
{
    [Serializable]
    public struct BrewingReceiptPart
    {
        [field:SerializeField] public BrewingIngredient Ingredient{ get; private set; }
        [field:SerializeField] public int Quantity{ get; private set; }
    }
}