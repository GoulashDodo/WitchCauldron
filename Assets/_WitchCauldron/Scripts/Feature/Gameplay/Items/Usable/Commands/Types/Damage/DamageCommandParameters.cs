using UnityEngine;

namespace Feature.Gameplay.Items.Usable.Commands.Damage
{
    [CreateAssetMenu(fileName = "Damage Command", menuName = "Game/Gameplay/Items/Parameters/Damage", order = 0)]
    public class DamageCommandParameters : UseCommandParameters
    {
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public bool IsArea { get; private set; }
    }
}
