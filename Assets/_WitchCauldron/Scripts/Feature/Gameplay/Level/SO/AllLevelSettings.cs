
using Sirenix.OdinInspector;
using UnityEngine;

namespace Feature.Gameplay.Level.SO
{
    
    [CreateAssetMenu(fileName = "AllLevelSettings", menuName = "Game/Gameplay/Level/All Level Settings")]
    public class AllLevelSettings : ScriptableObject
    {
        
        
        [field:InlineEditor]
        [field: SerializeField] public LevelSettings[] AllSettings { get; private set; }
    }
}