using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Core.GameRoot.State.Providers;
using WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Cmd;
using WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Services;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Commands;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.ScriptableObjects;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Services;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root.CompositionRoot.Gameplay.Registrations
{
    public static class GameplayRegistrations
    {
        public static void Register(DiContainer container, PotionReceiptList receiptList)
        {
            container.Bind<PotionReceiptList>().FromInstance(receiptList).AsSingle();
            
            container.Bind<ReceiptService>().AsSingle();

            container.Bind<BrewingService>().AsSingle();

            container.Bind<DraggableItemService>().AsSingle();

            var mouseClickHandler = new MouseClickHandler(container.Resolve<GameInput>());

            container.BindInstance(mouseClickHandler).AsSingle();
            
            RegisterCommands(container);
            
        }

        private static void RegisterCommands(DiContainer container)
        {
            var commandProcessor = container.Resolve<ICommandProcessor>();
            var gameStateProvider = container.Resolve<IGameStateProvider>();
            
            commandProcessor.RegisterCommand(new CmdCreateBrewingSession(gameStateProvider.GameState));
            commandProcessor.RegisterCommand(new CmdTryAddIngredient(gameStateProvider.GameState));
            commandProcessor.RegisterCommand(new CmdSetMainCauldron(gameStateProvider.GameState));
            commandProcessor.RegisterCommand(new CmdTrySpawnDraggableItem(container.Resolve<MouseClickHandler>()));
        }
        
        
    }
}