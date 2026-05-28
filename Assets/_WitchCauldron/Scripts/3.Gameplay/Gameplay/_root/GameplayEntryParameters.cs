using System;
using Gameplay.Items.SO;

namespace Gameplay._root
{
    public class GameplayEntryParameters
    {
        public string LevelId { get; }
        public string[] SelectedItemsIds { get; }

        public GameplayEntryParameters(string levelId)
        {
            LevelId = levelId;
            
            //TODO: Change, test purpose only
            SelectedItemsIds = new string[] {"Item_MoonPebble", "Item_Egg"};
        }

        public GameplayEntryParameters(string levelId, string[] selectedItemsIds)
        {
            LevelId = levelId;
            SelectedItemsIds = selectedItemsIds ?? Array.Empty<string>();
        }
    }
}