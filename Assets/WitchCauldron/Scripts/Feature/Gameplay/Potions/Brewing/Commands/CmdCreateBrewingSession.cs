using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Core.GameRoot.State.Root;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Cauldron;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Commands.Parameters;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Commands
{
    public class CmdCreateBrewingSession : ICommand<CmdCreateBrewingSessionParameters>
    {
        private readonly GameState _gameState;

        public CmdCreateBrewingSession(GameState gameState)
        {
            _gameState = gameState;
        }
        
        public bool Handle(CmdCreateBrewingSessionParameters commandParameter)
        {

            var session = new BrewingSession(commandParameter.Receipt);
            
            //_gameState.MainCauldronViewModel.SetBrewingSession(session);
            
            return true;
        }
    }
}