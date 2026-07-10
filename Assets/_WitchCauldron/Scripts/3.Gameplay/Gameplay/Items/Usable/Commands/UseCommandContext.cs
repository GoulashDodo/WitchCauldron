using Gameplay.Items.SO;
using Gameplay.Items.Visuals;
using Gameplay.Battle.BattleEntities.Enemies.Core;
using UnityEngine;

namespace Gameplay.Items.Usable.Commands
{
    public class UseCommandContext
    {
        private bool _impactFxPlayed;

        public UseCommandContext(
            ItemSettings itemSettings,
            ItemUseFxPlayer fxPlayer,
            Vector3 itemWorldScale,
            Enemy targetEnemy = null,
            bool suppressImpactFx = false)
        {
            ItemSettings = itemSettings;
            FxPlayer = fxPlayer;
            ItemWorldScale = itemWorldScale;
            TargetEnemy = targetEnemy;
            SuppressImpactFx = suppressImpactFx;
        }

        public ItemSettings ItemSettings { get; }
        public ItemUseFxPlayer FxPlayer { get; }
        public Vector3 ItemWorldScale { get; }
        public Enemy TargetEnemy { get; }
        public bool SuppressImpactFx { get; }

        public void PlayImpactFxOnce(Vector2 position)
        {
            if (SuppressImpactFx || _impactFxPlayed)
                return;

            FxPlayer?.PlayImpactFx(position, ItemSettings, ItemWorldScale);
            _impactFxPlayed = true;
        }

        public UseCommandContext WithTarget(Enemy targetEnemy, bool suppressImpactFx)
        {
            return new UseCommandContext(ItemSettings, FxPlayer, ItemWorldScale, targetEnemy, suppressImpactFx);
        }
    }
}
