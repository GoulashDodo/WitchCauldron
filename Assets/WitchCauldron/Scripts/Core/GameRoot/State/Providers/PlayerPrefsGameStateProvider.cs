using System;
using R3;
using UnityEngine;
using WitchCauldron.Scripts.Core.GameRoot.State.Root;
using WitchCauldron.Scripts.Feature.Gameplay.Potions.Brewing.Model;

namespace WitchCauldron.Scripts.Core.GameRoot.State.Providers
{
    public class PlayerPrefsGameStateProvider : IGameStateProvider
    {

        private const string GAME_STATE_KEY = nameof(GAME_STATE_KEY);
        
        public GameState GameState { get; private set; }

        
        
        public Observable<GameState> LoadGameState()
        {

            if (!PlayerPrefs.HasKey(GAME_STATE_KEY))
            {
                GameState = CreateGameStateFromSettings();
                SaveGameState();
            }
            else
            {
                var json = PlayerPrefs.GetString(GAME_STATE_KEY);
                //_gameStateData = JsonUtility.FromJson<GameStateData>(json);
            }
            
            return Observable.Return(GameState);
        }

        public Observable<bool> SaveGameState()
        {
            //var json = JsonUtility.ToJson(_gameStateData, true);
            //PlayerPrefs.SetString(GAME_STATE_KEY, json);
            
            return Observable.Return(true);
        }

        public Observable<bool> ResetGameState()
        {

            throw new NotImplementedException();
        }
        
        
        private GameState CreateGameStateFromSettings()
        {
            var state = new GameState(new Cauldron());

            return state;
        }
        
        
    }
}