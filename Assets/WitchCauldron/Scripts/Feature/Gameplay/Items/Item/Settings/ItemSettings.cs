using UnityEngine;
using WitchCauldron.Scripts.Feature.Gameplay.Items.Item.Model;

namespace WitchCauldron.Scripts.Feature.Gameplay.Items.Item.Settings
{
    [CreateAssetMenu(fileName = "Item Settings", menuName = "Game/Items/Settings", order = 0)]
    public class ItemSettings : ScriptableObject
    {
        [field: SerializeField] public string TypeId { get; private set; }
        [field: SerializeField] public DraggableItem ItemPf { get; private set; }
        
        [field: Space(10)]
        [field: SerializeField] public string TitleLid { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
   
        
        
    }
}