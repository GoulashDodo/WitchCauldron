using WitchCauldron.Scripts.Core.GameRoot.Cmd.Interfaces;
using WitchCauldron.Scripts.Core.GameRoot.State.Providers;
using WitchCauldron.Scripts.Feature.Gameplay.Clickable;
using WitchCauldron.Scripts.Feature.Gameplay.DragAndDrop.Cmd;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Commands;
using Zenject;

namespace WitchCauldron.Scripts.Core.GameRoot.Root.CompositionRoot.Gameplay.Registrations
{
    public class GameplayCommandsRegistrator : IInitializable
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly IGameStateProvider _gameStateProvider;
        private readonly MouseClickHandler _mouseClickHandler;
        
        public GameplayCommandsRegistrator(ICommandProcessor commandProcessor, IGameStateProvider gameStateProvider, MouseClickHandler mouseClickHandler)
        {
            _commandProcessor = commandProcessor;
            _gameStateProvider = gameStateProvider;
            _mouseClickHandler = mouseClickHandler;
        }
        
        public void Initialize()
        {
            _commandProcessor.RegisterCommand(new CmdCreateBrewingSession(_gameStateProvider.GameState));
            _commandProcessor.RegisterCommand(new CmdTryAddIngredient(_gameStateProvider.GameState));
            _commandProcessor.RegisterCommand(new CmdSetMainCauldron(_gameStateProvider.GameState));
            _commandProcessor.RegisterCommand(new CmdTrySpawnDraggableItem(_mouseClickHandler));
        }
        
        
    }
}