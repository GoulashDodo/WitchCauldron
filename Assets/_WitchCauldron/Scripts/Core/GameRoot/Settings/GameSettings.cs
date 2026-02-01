using _WitchCauldron.Scripts.Feature.Gameplay.Enemies.SO;
using _WitchCauldron.Scripts.Feature.Gameplay.Items.Settings;
using UnityEngine;

namespace _WitchCauldron.Scripts.Core.GameRoot.Settings
{
    [CreateAssetMenu(fileName = "New Game Settings", menuName = "Game/Settings/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        
        
        [field: Header("Gameplay")]
        [field: SerializeField] public AllEnemySettings AllEnemiesSettings  { get; private set; }
        [field: SerializeField] public AllItemSettings AllItemsSettings { get; private set; }
        
    }
}