using Gameplay.Battle.Waves.Enums;

namespace Gameplay.Battle.Waves.Structures
{
    public readonly struct WaveStartedEvent
    {
        public readonly int WaveIndex;
        public readonly int TotalWaves;
        public readonly WaveType Type;
        public readonly bool IsFirst;
        public readonly bool IsFinal;

        public WaveStartedEvent(int waveIndex, int totalWaves, WaveType type)
        {
            WaveIndex = waveIndex;
            TotalWaves = totalWaves;
            Type = type;
            IsFirst = waveIndex == 0;
            IsFinal = type == WaveType.Final || waveIndex == totalWaves - 1;
        }
    }
}
