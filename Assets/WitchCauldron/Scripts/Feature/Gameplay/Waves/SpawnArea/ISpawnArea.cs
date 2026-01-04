using UnityEngine;

namespace WitchCauldron.Scripts.Feature.Gameplay.Waves.SpawnArea
{
    public interface ISpawnArea
    {
        Vector3 GetRandomPosition();
    }
}