using Gameplay.Items.SO;
using Gameplay.Items.Visuals;
using UnityEngine;

namespace Gameplay.Items.Usable.Commands
{
    public class UseCommandContext
    {
        public UseCommandContext(ItemSettings itemSettings, ItemUseFxPlayer fxPlayer, Vector3 itemWorldScale)
        {
            ItemSettings = itemSettings;
            FxPlayer = fxPlayer;
            ItemWorldScale = itemWorldScale;
        }

        public ItemSettings ItemSettings { get; }
        public ItemUseFxPlayer FxPlayer { get; }
        public Vector3 ItemWorldScale { get; }
    }
}
