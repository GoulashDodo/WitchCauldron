using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Item;

namespace WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Cmd.Parameters
{
    public class CmdTrySpawnDraggableItemParameters : ICommandParameter
    {
        public DraggableItem ItemToSpawn { get; }
        public Vector3 SpawnPosition { get; }
        
        
        public CmdTrySpawnDraggableItemParameters(DraggableItem itemToSpawn, Vector3 spawnPosition)
        {
            ItemToSpawn = itemToSpawn;
            SpawnPosition = spawnPosition;
        }


    }
}