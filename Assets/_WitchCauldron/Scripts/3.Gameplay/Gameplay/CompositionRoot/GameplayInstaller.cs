using System;
using System.Linq;
using Core.SceneManagement;
using Gameplay._root;
using Gameplay._root.SO;
using Gameplay.Battle.Base.Interfaces;
using Gameplay.Battle.Enemies.Services;
using Gameplay.Battle.Waves.Service;
using Gameplay.Battle.Waves.SpawnArea;
using Gameplay.Items.Combination.Service;
using Gameplay.Items.Services;
using Gameplay.Items.Usable.Commands.Processor;
using Gameplay.Level;
using Gameplay.Level.SO;
using UnityEngine;
using Zenject;

namespace Gameplay.CompositionRoot
{
    public sealed class GameplayInstaller : MonoInstaller
    {


        [Header("Level Objects")] 
        [SerializeField] private BoxSpawnArea _spawnArea;
        
        public override void InstallBindings()
        {
            
            
            Container
                .Bind<ISpawnArea>()
                .FromInstance(_spawnArea)
                .AsSingle();
            
         
            
            Container.Bind<G>().AsSingle();

            BindLevelData();
            
            Container.Bind<IBaseHealthProvider>().To<LevelSettingsBaseHealthProvider>().AsSingle();
            
            BindServices();
            
            
            Container.BindInterfacesTo<GameplayRunFlowController>().AsSingle().NonLazy();
            Container.Bind<IInitializable>().To<GameplayBootstrap>().AsSingle().NonLazy();
            
        }


        private void BindLevelData()
        {
            Container.Bind<LevelSettings>()
                .FromMethod(ctx =>
                {
                    var gameplaySettings = ctx.Container.Resolve<GameplaySettings>();
                    var entryParameters = ctx.Container.Resolve<SceneParametersPayload>().GameplayEntryParameters;

                    if (entryParameters == null)
                        throw new InvalidOperationException("Gameplay entry parameters were not provided.");

                    var levelSettings = gameplaySettings.AllLevelSettings.AllSettings
                        .FirstOrDefault(settings => settings != null && settings.LevelId == entryParameters.LevelId);


                    return levelSettings;
                })
                .AsSingle();
        }
        
        
        private void BindServices()
        {
            Container.Bind<IUseCommandProcessor>().To<UseCommandProcessor>().AsSingle();
            Container.Bind<ItemService>().AsSingle();
            Container.Bind<EnemyService>().AsSingle();
            Container.BindInterfacesAndSelfTo<WaveService>().AsSingle();
            Container.Bind<CombinationService>().AsSingle();
            Container.BindInterfacesAndSelfTo<DropService>().AsSingle();


        }
        
      
     
    }
}
