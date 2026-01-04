    using UnityEngine;
    using WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Model;

    namespace WitchCauldron.Scripts.Feature.Gameplay.Items.Model
    {
        public class DamageItem : UsableItem
        {
            public override void Use(Vector2 position)
            {
                Debug.Log($"{name}: Damaging enemy at:  {position}");
            }
        }
    }