using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Commands.Parameters
{
    public class CmdCreateBrewingSessionParameters : ICommandParameter
    {
        public readonly BrewingReceipt Receipt;

        public CmdCreateBrewingSessionParameters(BrewingReceipt receipt)
        {
            Receipt = receipt;
        }
        
    }
}