using UnityEngine;

namespace Gameplay.Items.Usable.Commands.Spawn
{
    [CreateAssetMenu(fileName = "Spawn Command", menuName = "Game/Gameplay/Items/Parameters/Spawn", order = 1)]
    public class SpawnCommandParameters : UseCommandParameters
    {
        [field: SerializeField] public GameObject Prefab { get; set; }
    }
}