using Gameplay._root.SO;
using Hut.SO;
using UnityEngine;

namespace Core.SO
{
    [CreateAssetMenu(fileName = "New Game Settings", menuName = "Game/Settings/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [field: SerializeField] public HutSettings HutSettings { get; private set; }

        [field: SerializeField] public GameplaySettings GameplaySettings { get; private set; }
        
    }
}
