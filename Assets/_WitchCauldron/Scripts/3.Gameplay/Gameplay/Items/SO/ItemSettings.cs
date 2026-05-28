using Gameplay.Items.MonoBehaviours;
using Gameplay.Items.Usable.Commands;
using Gameplay.Items.Visuals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.Items.SO
{
    [CreateAssetMenu(fileName = "Item Settings", menuName = "Game/Gameplay/Items/Settings", order = 0)]
    public class ItemSettings : ScriptableObject
    {
        [field: SerializeField] public string TypeId { get; private set; }
        [field: SerializeField] public DraggableItem ItemPf { get; private set; }
        
        [field: Space(10)]
        [field: SerializeField] public string TitleLid { get; private set; }

        [field: Space(10)][Header("On Item use")]
        [field: InlineEditor]
        [field: SerializeField] public UseCommandParameters[] OnUseCommands { get; private set; }

        [field: Space(10)]
        [field: SerializeField] public ItemUseVisualSettings UseVisuals { get; private set; } = new();
        
    }
}
