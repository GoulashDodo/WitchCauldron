using Feature.Gameplay._root.SO;
using UnityEngine;

namespace Core.GameRoot.SO
{
    [CreateAssetMenu(fileName = "New Game Settings", menuName = "Game/Settings/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        
        [field: SerializeField] public GameplaySettings GameplaySettings { get; set; }
        
    }
}
