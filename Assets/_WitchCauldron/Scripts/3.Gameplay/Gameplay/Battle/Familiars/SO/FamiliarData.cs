using UnityEngine;

namespace Gameplay.Battle.Familiars.SO
{
    [CreateAssetMenu(fileName = "Familiars Data", menuName = "Game/Gameplay/Familiars/Familiar Data")]
    public class FamiliarData : ScriptableObject
    {
        [field: SerializeField] public string FamiliarTypeId { get; private set; }
        
        [field: SerializeField] public GameObject FamiliarPrefab { get; private set; }
        
        
        [field: Header("Description")]
        [field: SerializeField] public string FamiliarName { get; private set; }
        [field: SerializeField] public string FamiliarDescription { get; private set; }
        
        [field: SerializeField] public Sprite FamiliarIcon { get; private set; }
        
        
    }
}