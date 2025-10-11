using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Items.Item.Model
{
    public class DamageItem : UsableItem
    {
        public override void Use(Vector2 position)
        {
            Debug.Log($"{name}: Damaging enemy at:  {position}");
        }
    }
}