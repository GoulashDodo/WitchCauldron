using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Commands.Damage
{
    [CreateAssetMenu(fileName = "Damage Command", menuName = "Game/Items/Parameters/Damage", order = 0)]
    public class DamageCommandParameters : UseCommandParameters
    {
        [field: SerializeField] public int Damage { get; private set; }
    }
}