using _WitchCauldron.Scripts.Feature.Gameplay._Root;
using _WitchCauldron.Scripts.Feature.Gameplay.Battle.Model;
using _WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using _WitchCauldron.Scripts.Feature.Gameplay.Combination.ScriptableObjects;
using _WitchCauldron.Scripts.Feature.Gameplay.Combination.Service;
using _WitchCauldron.Scripts.Feature.Gameplay.Enemies.Services;
using _WitchCauldron.Scripts.Feature.Gameplay.Items.Services;
using _WitchCauldron.Scripts.Feature.Gameplay.Items.Usable.Commands.Processor;
using _WitchCauldron.Scripts.Feature.Gameplay.Level._Root;
using _WitchCauldron.Scripts.Feature.Gameplay.Waves.Service;
using _WitchCauldron.Scripts.Feature.Gameplay.Waves.SpawnArea;
using UnityEngine;
using Zenject;

namespace _WitchCauldron.Scripts.Core.GameRoot._Root.CompositionRoot.Gameplay.Registrations
{
    public sealed class GameplayInstaller : MonoInstaller
    {
        [Header("Configs")]
        [SerializeField] private LevelConfig _levelConfig;
        
        [SerializeField] private CombinationRuleList  _combinationRuleList;


        [Header("Level Objects")] 
        [SerializeField] private Base _base;
        [SerializeField] private BoxSpawnArea _spawnArea;
        
        public override void InstallBindings()
        {
            
            Container.BindInterfacesAndSelfTo<MouseClickHandler>()
                .AsSingle()
                .NonLazy();

            
            Container.BindInstance(_combinationRuleList).AsSingle();
            Container.BindInstance(_levelConfig).AsSingle();
            
            
            Container.Bind<Base>()
                    .FromInstance(_base)
                    .AsSingle();
            
            
            Container
                .Bind<ISpawnArea>()
                .FromInstance(_spawnArea)
                .AsSingle();
            
            Container.Bind<IUseCommandProcessor>().To<UseCommandProcessor>().AsSingle();
         
            BindServices();
            
            
            Container.Bind<IInitializable>().To<GameBootstrap>().AsSingle().NonLazy();
            
        }



        private void BindServices()
        {
            
            Container.Bind<EnemyService>().AsSingle();
            Container.Bind<WaveService>().AsSingle();
            Container.Bind<CombinationService>().AsSingle();
            Container.Bind<ItemService>().AsSingle();

        }
        
      
     
    }
}
