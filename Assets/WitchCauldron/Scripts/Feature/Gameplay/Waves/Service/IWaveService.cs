using R3;
using WitchCauldron.Scripts.Feature.Gameplay.Waves.ScriptableObjects;

namespace WitchCauldron.Scripts.Feature.Gameplay.Waves.Service
{
    public interface IWaveService
    {
        ReadOnlyReactiveProperty<float> Progress01 { get;}
        Observable<Unit> LevelCompleted { get;}
        
        void StartLevel(LevelScript level);
        void StopLevel();
        
    }
}