using _WitchCauldron.Scripts.Feature.Gameplay.Waves.ScriptableObjects;
using R3;

namespace _WitchCauldron.Scripts.Feature.Gameplay.Waves.Service
{
    public interface IWaveService
    {
        ReadOnlyReactiveProperty<float> Progress01 { get;}
        Observable<Unit> LevelCompleted { get;}
        
        void StartLevel();
        void StopLevel();
        
    }
}