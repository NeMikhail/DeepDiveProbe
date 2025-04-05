using System.Collections.Generic;
using GameCoreModule;
using MAEngine;
using Player;
using UnityEngine;
using Zenject;

namespace UI
{
    public class GUIActions : IAction, IPreInitialisation, IInitialisation, ICleanUp, IFixedExecute
    {
        private GUIView _guiView;
        private StateEventsBus _stateEventsBus;
        private PlayerEventBus _playerEventBus;
        private GameEventBus _gameEventBus;
        private UIEventBus _uiEventBus;

        [Inject]
        public void Construct(GUIView guiView, StateEventsBus stateEventsBus, PlayerEventBus playerEventBus,
            GameEventBus gameEventBus, UIEventBus uiEventBus)
        {
            _guiView = guiView;
            _stateEventsBus = stateEventsBus;
            _playerEventBus = playerEventBus;
            _gameEventBus = gameEventBus;
            _uiEventBus = uiEventBus;
        }
        
        public void PreInitialisation()
        {
            _guiView.InitializeView();
            
        }

        public void Initialisation()
        {
            
        }

        public void Cleanup()
        {
            
        }
        
        public void FixedExecute(float fixedDeltaTime)
        {
            UpdateUI();
            
        }
        
        public void UpdateUI()
        {

        }
        
        
        private void SetPauseState()
        {
            ClearGUI();
            _stateEventsBus.OnPauseStateActivate?.Invoke();
        }
        
        private void ClearGUI()
        {
            
        }
    }
}