using System;
using UnityEngine;

namespace GameStateSystem
{
    public class GameStateController
    {
        public static event Action<GameState> OnCurrentGameStateChanged = delegate { };


        private GameState _currentGameState;


        public GameState CurrentGameState
        {
            get => _currentGameState;
            set
            {
                if (value == _currentGameState)
                {
                    return;
                }


                _currentGameState = value;
                Debug.Log($"Current game state changed to: {value.ToString()}");
                OnCurrentGameStateChanged(value);
            }
        }


        public GameStateController(GameState initialState)
        {
            _currentGameState = initialState;
            OnCurrentGameStateChanged(initialState);
        }
    }
}