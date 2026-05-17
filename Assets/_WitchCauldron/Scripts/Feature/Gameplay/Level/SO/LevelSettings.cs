using Feature.Gameplay.Battle.Waves.SO;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Feature.Gameplay.Level.SO
{
    [CreateAssetMenu(fileName = "New Level Config", menuName = "Game/Gameplay/Level/Level Config", order = 0)]
    public class LevelSettings : ScriptableObject
    {
        [field: SerializeField] public string LevelId { get; private set; }
        
        
        
        [field:SerializeField] public float BaseHealth { get; private set; }
        
        
        [field: InlineEditor]
        [field:SerializeField] public WaveSettings WaveSettings { get; private set; } 
        
        
    }
}