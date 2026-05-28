using Gameplay.Battle.Waves.Structures;
using R3;

namespace Gameplay.Battle.Waves.Service
{
    public interface IWaveService
    {
        ReadOnlyReactiveProperty<WaveProgress> Progress { get; }
        Observable<WaveStartedEvent> WaveStarted { get; }
        Observable<Unit> WavesCompleted { get; }

        void StartWaves();
        void StopWaves();
    }
        
}
