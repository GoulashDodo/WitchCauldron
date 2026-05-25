using Core.SO;
using Gameplay._root.SO;
using UnityEngine;
using Zenject;

namespace Core.CompositionRoot.Installers
{
    [CreateAssetMenu(fileName = "GlobalSettingsInstaller",  menuName = "Installers/Global Settings Installer")]
    public class GlobalSettingsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private GameSettings _gameSettings;

        public override void InstallBindings()
        {
            
            Container.Bind<GameSettings>().FromInstance(_gameSettings).AsSingle();
            Container.Bind<GameplaySettings>().FromInstance(_gameSettings.GameplaySettings).AsSingle();
            
        }
    }
}
