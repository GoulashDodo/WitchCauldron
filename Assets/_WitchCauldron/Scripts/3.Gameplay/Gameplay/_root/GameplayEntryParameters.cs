using System;

namespace Gameplay._root
{
    public class GameplayEntryParameters
    {
        public string LevelId { get; }
        
        public string SelectedFamiliar {get;}
        public string[] SelectedItemsIds { get; }

        public GameplayEntryParameters(string levelId)
        {
            LevelId = levelId;
            SelectedFamiliar = String.Empty;

            //TODO: Change, test purpose only
            SelectedItemsIds = new string[] {"Item_MoonPebble", "Item_Egg"};
        }

        public GameplayEntryParameters(string levelId, string[] selectedItemsIds, string selectedFamiliar = null)
        {
            LevelId = levelId;
            SelectedFamiliar = selectedFamiliar ?? String.Empty;
            SelectedItemsIds = selectedItemsIds ?? Array.Empty<string>();
        }
    }
}