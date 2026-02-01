using UnityEngine;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Waves.SpawnArea
{
    public interface ISpawnArea
    {
        Vector3 GetRandomPosition();
    }
}