using Common.Extensions.UnityInspector;
using Feature.Gameplay.Items.Model;
using Feature.Gameplay.Items.Usable.Commands;
using UnityEngine;

namespace Feature.Gameplay.Items.SO
{
    [CreateAssetMenu(fileName = "Item Settings", menuName = "Game/Gameplay/Items/Settings", order = 0)]
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