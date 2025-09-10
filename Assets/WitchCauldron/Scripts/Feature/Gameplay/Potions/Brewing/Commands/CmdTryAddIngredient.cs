using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Core.GameRoot.State.Root;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Commands.Parameters;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Commands
{
    public class CmdTryAddIngredient : ICommand<CmdTryAddIngredientParameters>
    {
        private readonly GameState _gameState;

        public CmdTryAddIngredient(GameState gameState)
        {
            _gameState = gameState;
        }

        public bool Handle(CmdTryAddIngredientParameters commandParameter)
        {
            _gameState.Cauldron.TryAddIngredient(commandParameter.Ingredient);
            return true;
        }
    }
}