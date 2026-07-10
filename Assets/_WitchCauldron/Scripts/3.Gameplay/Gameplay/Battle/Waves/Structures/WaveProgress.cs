namespace Gameplay.Battle.Waves.Structures
{
    public readonly struct WaveProgress
    {
        public readonly float Level01;
        public readonly int CurrentWaveIndex;
        public readonly int TotalWaves;
        public readonly float CurrentWave01;
        public readonly bool IsWaitingForClear;

        public WaveProgress(float level01, int currentWaveIndex, int totalWaves, float currentWave01, bool isWaitingForClear)
        {
            Level01 = level01;
            CurrentWaveIndex = currentWaveIndex;
            TotalWaves = totalWaves;
            CurrentWave01 = currentWave01;
            IsWaitingForClear = isWaitingForClear;
        }
    }
}
