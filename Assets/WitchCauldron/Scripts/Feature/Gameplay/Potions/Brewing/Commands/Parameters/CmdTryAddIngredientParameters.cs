using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Commands.Parameters
{
    public class CmdTryAddIngredientParameters : ICommandParameter
    {
        public readonly BrewingIngredient Ingredient;

        public CmdTryAddIngredientParameters(BrewingIngredient ingredient)
        {
            Ingredient = ingredient;
        }
    }
}