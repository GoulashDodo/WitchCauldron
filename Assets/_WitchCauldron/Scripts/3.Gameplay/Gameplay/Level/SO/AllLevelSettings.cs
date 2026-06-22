using UnityEngine;

namespace Gameplay.Level.SO
{
    
    [CreateAssetMenu(fileName = "AllLevelSettings", menuName = "Game/Gameplay/Level/All Level Settings")]
    public class AllLevelSettings : ScriptableObject
    {
        
        
        [field: SerializeField] public LevelSettings[] AllSettings { get; private set; }
    }
}
