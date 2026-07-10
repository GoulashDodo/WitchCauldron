using UnityEngine;

namespace Gameplay.Items.SO
{
    [CreateAssetMenu(fileName = "All Item Settings", menuName = "Game/Gameplay/Items/All Item Settings", order = 0)]
    public class AllItemSettings : ScriptableObject
    {
        [field: SerializeField] public ItemSettings[] ItemSettings { get; private set; }

        [field: SerializeField] public GameObject CombineSuccessPrefab { get; private set; }

    }
}