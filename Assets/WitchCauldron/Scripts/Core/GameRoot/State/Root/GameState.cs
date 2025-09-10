using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Model;

namespace WitchCauldron.Scripts.Core.GameRoot.State.Root
{
    public class GameState
    {
        public Cauldron Cauldron { get; private set; }


        public GameState(Cauldron cauldron)
        {
            Cauldron = cauldron;
        }
        
        
    }
}