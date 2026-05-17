using Feature.Gameplay.Items.Usable.Model;
using UnityEngine;

namespace Feature.Gameplay.Items.Model
    {
        public class DamageItem : UsableItem
        {
            public override void Use(Vector2 position)
            {
                Debug.Log($"{name}: Damaging enemy at:  {position}");
            }
        }
    }