using System;
using System.Linq;
using Core.SceneManagement;
using Gameplay._root.SO;
using Gameplay.Battle.Base.Interfaces;
using Gameplay.Battle.BattleEntities.Enemies.Services;
using Gameplay.Battle.Familiars.Service;
using Gameplay.Battle.Waves.Service;
using Gameplay.Battle.Waves.SpawnArea;
using Gameplay.Items.Combination.Service;
using Gameplay.Items.Services;
using Gameplay.Items.Usable.Commands.Damage;
using Gameplay.Items.Usable.Commands.Effect;
using Gameplay.Items.Usable.Commands.Handler;
using Gameplay.Items.Usable.Commands.Preview;
using Gameplay.Items.Usable.Commands.Processor;
using Gameplay.Items.Usable.Commands.Ricochet;
using Gameplay.Items.Usable.Commands.Spawn;
using Gameplay.Level;
using Gameplay.Level.SO;
using UnityEngine;
using Zenject;

namespace Gameplay._root.Installers
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
            
         

            Container.BindInterfacesAndSelfTo<G>().AsSingle();

            BindEntryParameters();
            BindLevelData();
            
            Container.Bind<IBaseHealthProvider>().To<LevelSettingsBaseHealthProvider>().AsSingle();
            
            BindServices();
            
            
            Container.Bind<GameplayRunFlowController>().AsSingle();
            Container.Bind<IInitializable>().To<GameplayBootstrap>().AsSingle().NonLazy();
            
        }


        private void BindEntryParameters()
        {
            Container.Bind<GameplayEntryParameters>()
                .FromMethod(ctx =>
                {
                    var entryParameters = ctx.Container.Resolve<SceneParametersPayload>().GameplayEntryParameters;

                    if (entryParameters == null)
                        throw new InvalidOperationException("Gameplay entry parameters were not provided.");

                    return entryParameters;
                })
                .AsSingle();
        }

        private void BindLevelData()
        {
            Container.Bind<LevelSettings>()
                .FromMethod(ctx =>
                {
                    var gameplaySettings = ctx.Container.Resolve<GameplaySettings>();
                    var entryParameters = ctx.Container.Resolve<GameplayEntryParameters>();

                    var levelSettings = gameplaySettings.AllLevelSettings.AllSettings
                        .FirstOrDefault(settings => settings != null && settings.LevelId == entryParameters.LevelId);


                    return levelSettings;
                })
                .AsSingle();
        }
        
        
        private void BindServices()
        {
            BindUseCommandHandlers();
            
            Container.Bind<IUseCommandProcessor>().To<UseCommandProcessor>().AsSingle();
            Container.Bind<IUseCommandPreviewProcessor>().To<UseCommandPreviewProcessor>().AsSingle();
            Container.Bind<ItemService>().AsSingle();
            Container.Bind<EnemyService>().AsSingle();
            Container.Bind<FamiliarService>().AsSingle();
            Container.BindInterfacesAndSelfTo<WaveService>().AsSingle();
            Container.Bind<CombinationService>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameplayPauseService>().AsSingle();
            Container.BindInterfacesAndSelfTo<DropService>().AsSingle();


        }

        private void BindUseCommandHandlers()
        {
            Container.Bind<IUseCommandHandler>().To<DamageCommandHandler>().AsSingle();
            Container.Bind<IUseCommandHandler>().To<SpawnCommandHandler>().AsSingle();
            Container.Bind<IUseCommandHandler>().To<EffectCommandHandler>().AsSingle();
            Container.Bind<IUseCommandHandler>().To<RicochetCommandHandler>().AsSingle();
        }
        
      
     
    }
}
