using R3;

namespace Feature.Gameplay.Battle.Waves.Service
{
    public interface IWaveService
    {
        ReadOnlyReactiveProperty<float> Progress01 { get;}
        
        void StartWaves();
        void StopWaves();
        
    }
}