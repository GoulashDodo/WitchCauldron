using UnityEngine;

namespace Gameplay.Battle.Waves.SpawnArea
{
    public interface ISpawnArea
    {
        Vector3 GetRandomPosition();
    }
}