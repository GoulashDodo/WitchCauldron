using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Cmd.Parameters;
using WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Item;

namespace WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Services
{
    public class DraggableItemService
    {
        private readonly ICommandProcessor _cmd;

        public DraggableItemService(ICommandProcessor cmd)
        {
            _cmd = cmd;
        }

        public bool TrySpawnDraggableItem(DraggableItem itemToSpawn, Vector3 initialPosition)
        {
            return _cmd.Process(new CmdTrySpawnDraggableItemParameters(itemToSpawn, initialPosition));
        }
        
    }
}