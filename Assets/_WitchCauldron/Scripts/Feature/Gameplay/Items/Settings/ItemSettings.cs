using _WitchCauldron.Scripts.Common.Extensions.UnityInspector;
using _WitchCauldron.Scripts.Feature.Gameplay.Items.Model;
using _WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Commands;
using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Items.Settings
{
    [CreateAssetMenu(fileName = "Item Settings", menuName = "Game/Items/Settings", order = 0)]
    public class ItemSettings : ScriptableObject
    {
        [field: SerializeField] public string TypeId { get; private set; }
        [field: SerializeField] public DraggableItem ItemPf { get; private set; }
        
        [field: Space(10)]
        [field: SerializeField] public string TitleLid { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
   
        
        [field: Space(10)][Header("On Item use")]
        [field: SerializeField, Expandable] public UseCommandParameters[] OnUseCommands { get; private set; }
        
    }
}