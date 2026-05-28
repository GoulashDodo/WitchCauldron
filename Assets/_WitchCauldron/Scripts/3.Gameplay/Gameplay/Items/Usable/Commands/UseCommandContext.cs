using Gameplay.Items.SO;
using Gameplay.Items.Visuals;

namespace Gameplay.Items.Usable.Commands
{
    public class UseCommandContext
    {
        public UseCommandContext(ItemSettings itemSettings, ItemUseFxPlayer fxPlayer)
        {
            ItemSettings = itemSettings;
            FxPlayer = fxPlayer;
        }

        public ItemSettings ItemSettings { get; }
        public ItemUseFxPlayer FxPlayer { get; }
    }
}
