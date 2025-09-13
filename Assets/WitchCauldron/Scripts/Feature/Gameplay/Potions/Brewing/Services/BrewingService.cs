using System.Collections.Generic;
using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Cauldron;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Commands.Parameters;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Services
{
    public class BrewingService
    {

        private ICommandProcessor _cmd;
        
        private readonly Queue<BrewingIngredient> _ingredients = new();
        
        
        public BrewingService(ICommandProcessor cmd)
        {
            _cmd = cmd;
        }
        
        
        public bool TryDequeueIngredient(BrewingSession session,BrewingIngredient ingredient)
        {
            var cmdParameters = new CmdTryAddIngredientParameters(session, ingredient);
            var result = _cmd.Process(cmdParameters);

            return result;
        }
        
        
    }
}