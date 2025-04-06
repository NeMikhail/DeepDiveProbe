using MAEngine;
using UnityEngine;
using Zenject;

namespace GameCoreModule
{
    public class GameStateActions : IAction, IInitialisation, ICleanUp
    {
        private GameEventBus _gameEventBus;
        private InputEventBus _inputEventBus;
        private StateEventsBus _stateEventsBus;
        private UIEventBus _uiEventBus;
        
        private GameState _currentGameState;

        [Inject]
        public void Construct(GameEventBus gameEventBus, InputEventBus inputEventBus, StateEventsBus stateEventsBus,
            UIEventBus uiEventBus)
        {
            _gameEventBus = gameEventBus;
            _inputEventBus = inputEventBus;
            _stateEventsBus = stateEventsBus;
            _uiEventBus = uiEventBus;
        }

        public void Initialisation()
        {
            _inputEventBus.OnPauseButtonPerformed += PauseButtonAction;
            _stateEventsBus.OnPauseStateActivate += SetPauseState;
            _stateEventsBus.OnPlayStateActivate += SetPlayingState;
            _gameEventBus.OnGameOver += SetGameOverState;
            _stateEventsBus.OnGetCurrentState += SendCurrentState;
            SetPlayingState();
        }

        public void Cleanup()
        {
            _inputEventBus.OnPauseButtonPerformed -= PauseButtonAction;
            _stateEventsBus.OnPauseStateActivate -= SetPauseState;
            _stateEventsBus.OnPlayStateActivate -= SetPlayingState;
            _gameEventBus.OnGameOver -= SetGameOverState;
            _stateEventsBus.OnGetCurrentState -= SendCurrentState;
        }
        
        private void SendCurrentState(StateCallback callback)
        {
            callback.State = _currentGameState;
        }

        private void PauseButtonAction()
        {
            if (_currentGameState != GameState.PauseState)
            {
                _stateEventsBus.OnPauseStateActivate?.Invoke();
                _uiEventBus.OnOpenPauseMenu?.Invoke();
            }
        }

        private void SetPlayingState()
        {
            if (_currentGameState != GameState.PlayState)
            {
                Time.timeScale = 1f;
                _inputEventBus.OnEnableInput?.Invoke();
                _currentGameState = GameState.PlayState;
                _gameEventBus.OnStateChanged?.Invoke(_currentGameState);
            }
        }
        private void SetPauseState()
        {
            if (_currentGameState != GameState.PauseState)
            {
                Time.timeScale = 0f;
                _inputEventBus.OnDisableInput?.Invoke();
                _currentGameState = GameState.PauseState;
                _gameEventBus.OnStateChanged?.Invoke(_currentGameState);
            }
        }

        private void SetGameOverState()
        {
            Time.timeScale = 0f;
            _inputEventBus.OnDisableInput?.Invoke();
            _currentGameState = GameState.GameOver;
            _gameEventBus.OnStateChanged?.Invoke(_currentGameState);
        }

    }
}
