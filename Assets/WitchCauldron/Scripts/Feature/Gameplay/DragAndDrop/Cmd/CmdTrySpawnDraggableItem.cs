using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Cmd.Parameters;

namespace WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Cmd
{
    public class CmdTrySpawnDraggableItem : ICommand<CmdTrySpawnDraggableItemParameters>
    {

        private readonly MouseClickHandler _handler;

        public CmdTrySpawnDraggableItem(MouseClickHandler handler)
        {
            _handler = handler;
        }


        public bool Handle(CmdTrySpawnDraggableItemParameters commandParameter)
        {
            
            var item = Object.Instantiate(commandParameter.ItemToSpawn, commandParameter.SpawnPosition, Quaternion.identity);

            item.Initialize(_handler.MouseToWorldPosition);
            
            return true;
        }
    }
}