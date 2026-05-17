using UnityEngine;

namespace Feature.Gameplay.Battle.Waves.SpawnArea
{
    public interface ISpawnArea
    {
        Vector3 GetRandomPosition();
    }
}