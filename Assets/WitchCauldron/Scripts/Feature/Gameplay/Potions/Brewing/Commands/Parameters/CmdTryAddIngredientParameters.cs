using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Cauldron;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Commands.Parameters
{
    public class CmdTryAddIngredientParameters : ICommandParameter
    {
        
        public readonly BrewingSession BrewingSession;
        public readonly BrewingIngredient Ingredient;
        
        
        public CmdTryAddIngredientParameters(BrewingSession brewingSession, BrewingIngredient ingredient)
        {
            BrewingSession = brewingSession;
            Ingredient = ingredient;
        }
    }
}