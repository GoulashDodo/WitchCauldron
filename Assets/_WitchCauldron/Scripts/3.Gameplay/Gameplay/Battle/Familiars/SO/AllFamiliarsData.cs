using UnityEngine;

namespace Gameplay.Battle.Familiars.SO
{
    [CreateAssetMenu(fileName = "AllFamiliarData", menuName = "Game/Gameplay/Familiars/All Familiar Data", order = 0)]
    public class AllFamiliarsData : ScriptableObject
    {
        [field: SerializeField] public FamiliarData[] AllData { get; private set; }
    }
}