using _WitchCauldron.Scripts.Core.GameRoot.Settings;
using _WitchCauldron.Scripts.Feature.Gameplay.Enemies.SO;
using _WitchCauldron.Scripts.Feature.Gameplay.Items.Settings;
using UnityEngine;
using Zenject;

namespace _WitchCauldron.Scripts.Core.GameRoot._Root.CompositionRoot.Game.Registrations
{
    [CreateAssetMenu(
        fileName = "GlobalSettingsInstaller",
        menuName = "Installers/Global Settings Installer")]
    public class GlobalSettingsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private GameSettings _gameSettings;

        public override void InstallBindings()
        {
            Container
                .Bind<GameSettings>()
                .FromInstance(_gameSettings)
                .AsSingle();

            var enemiesSettings = _gameSettings.AllEnemiesSettings;
            
            Container
                .Bind<AllEnemySettings>()
                .FromInstance(enemiesSettings)
                .AsSingle();
            
            var itemSettings = _gameSettings.AllItemsSettings;
            
            Container
                .Bind<AllItemSettings>()
                .FromInstance(itemSettings)
                .AsSingle();
            
            
            
        }
    }
}