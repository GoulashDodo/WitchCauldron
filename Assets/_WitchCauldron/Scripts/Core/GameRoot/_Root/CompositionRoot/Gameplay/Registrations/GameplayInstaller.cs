using Core.GameRoot.Input.Clickable;
using Feature.Gameplay._root;
using Feature.Gameplay.Battle.Enemies.Services;
using Feature.Gameplay.Battle.Model;
using Feature.Gameplay.Battle.Waves.Service;
using Feature.Gameplay.Battle.Waves.SpawnArea;
using Feature.Gameplay.Combination.Service;
using Feature.Gameplay.Items.Services;
using Feature.Gameplay.Items.Usable.Commands.Processor;
using UnityEngine;
using Zenject;

namespace Core.GameRoot._root.CompositionRoot.Gameplay.Registrations
{
    public sealed class GameplayInstaller : MonoInstaller
    {


        [Header("Level Objects")] 
        [SerializeField] private Base _base;
        [SerializeField] private BoxSpawnArea _spawnArea;
        
        public override void InstallBindings()
        {
            
            Container.BindInterfacesAndSelfTo<MouseClickHandler>()
                .AsSingle()
                .NonLazy();
            
            
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
            Container.BindInterfacesAndSelfTo<WaveService>().AsSingle();
            Container.Bind<CombinationService>().AsSingle();
            Container.Bind<ItemService>().AsSingle();

        }
        
      
     
    }
}
