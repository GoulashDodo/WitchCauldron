using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Core.GameRoot.State.Root;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Commands.Parameters;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Commands
{
    public class CmdSetMainCauldron : ICommand<CmdSetMainCauldronParameters>
    {
        private readonly GameState _gameState;

        public CmdSetMainCauldron(GameState gameState)
        {
            _gameState = gameState;
        }

        public bool Handle(CmdSetMainCauldronParameters commandParameter)
        {
            
            
            throw new System.NotImplementedException();
        }
    }
}