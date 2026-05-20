using System;
using System.Linq;
using Feature.Gameplay._root;
using Feature.Gameplay._root.SO;
using Feature.Gameplay.Battle.Base.Interfaces;
using Feature.Gameplay.Battle.Enemies.Services;
using Feature.Gameplay.Battle.Waves.Service;
using Feature.Gameplay.Battle.Waves.SpawnArea;
using Feature.Gameplay.Items.Combination.Service;
using Feature.Gameplay.Items.Services;
using Feature.Gameplay.Items.Usable.Commands.Processor;
using Feature.Gameplay.Level;
using Feature.Gameplay.Level.SO;
using UnityEngine;
using Zenject;

namespace Core.GameRoot._root.CompositionRoot.Gameplay
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
            
            
            Container.Bind<IInitializable>().To<GameBootstrap>().AsSingle().NonLazy();
            
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
            Container.Bind<EnemyService>().AsSingle();
            Container.BindInterfacesAndSelfTo<WaveService>().AsSingle();
            Container.Bind<CombinationService>().AsSingle();
            Container.Bind<ItemService>().AsSingle();

        }
        
      
     
    }
}
