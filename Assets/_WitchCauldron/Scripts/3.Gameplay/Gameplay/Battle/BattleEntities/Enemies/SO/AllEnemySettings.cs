using UnityEngine;

namespace Gameplay.Battle.BattleEntities.Enemies.SO
{
    [CreateAssetMenu(fileName = "All Enemy Settings", menuName = "Game/Gameplay/Enemies/All Enemy Settings")]

    public class AllEnemySettings : ScriptableObject
    {
        [field: SerializeField, Min(0f)] public float SpawnMinDistance { get; private set; } = 1f;
        [field: SerializeField, Min(1)] public int SpawnPositionAttempts { get; private set; } = 10;
        [field: SerializeField] public Vector2 SpawnSpeedMultiplierRange { get; private set; } = new Vector2(0.9f, 1.1f);
        [field: SerializeField] public EnemySettings[] AllSettings { get;  private set; }

        public float GetRandomSpawnSpeedMultiplier()
        {
            var min = Mathf.Max(0f, Mathf.Min(SpawnSpeedMultiplierRange.x, SpawnSpeedMultiplierRange.y));
            var max = Mathf.Max(0f, Mathf.Max(SpawnSpeedMultiplierRange.x, SpawnSpeedMultiplierRange.y));

            return Mathf.Approximately(min, max)
                ? min
                : Random.Range(min, max);
        }
    }
}
