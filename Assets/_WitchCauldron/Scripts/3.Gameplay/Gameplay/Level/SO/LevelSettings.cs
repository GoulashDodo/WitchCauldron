using Core.Run;
using Gameplay.Battle.Waves.SO;
using Gameplay.Items.SO;
using UnityEngine;

namespace Gameplay.Level.SO
{
    [CreateAssetMenu(fileName = "New Level Config", menuName = "Game/Gameplay/Level/Level Config", order = 0)]
    public class LevelSettings : ScriptableObject
    {
        [field: SerializeField] public string LevelId { get; private set; }
        
        [field: SerializeField] public string LevelName { get; private set; }
        
        
        [field:SerializeField] public float BaseHealth { get; private set; }
        
        
        
        
        
        [field:SerializeField] public WaveSettings WaveSettings { get; private set; } 

        [field: Space(10)]
        [field: SerializeField] public UnlockReward[] CompletionRewards { get; private set; }
        
   
        
    }
}
