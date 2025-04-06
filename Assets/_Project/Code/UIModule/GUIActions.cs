using System.Collections.Generic;
using GameCoreModule;
using MAEngine;
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
            _playerEventBus.OnChangeLayer += ChangeLayerIndication;
            _playerEventBus.OnOxygenChanged += UpdateOxygenSlider;
            _playerEventBus.OnDepthChanged += UpdateDepthSlider;
            _guiView.PauseButton.Button.onClick.AddListener(SetPauseState);
        }

        public void Cleanup()
        {
            _playerEventBus.OnChangeLayer -= ChangeLayerIndication;
            _playerEventBus.OnOxygenChanged -= UpdateOxygenSlider;
            _playerEventBus.OnDepthChanged -= UpdateDepthSlider;
            _guiView.PauseButton.Button.onClick.RemoveListener(SetPauseState);
        }
        
        public void FixedExecute(float fixedDeltaTime)
        {
            UpdateUI();
            
        }
        
        public void UpdateUI()
        {

        }
        
        private void ChangeLayerIndication(int layer)
        {
            _guiView.UpdateLayerImages(layer);
        }
        
        private void UpdateOxygenSlider(int oxygenValue)
        {
            _guiView.OxygenSlider.value = oxygenValue;
        }
        
        private void UpdateDepthSlider(int depth)
        {
            _guiView.DepthSlider.value = depth;
            _guiView.DepthText.text = $"{depth}m.";
        }
        
        private void SetPauseState()
        {
            StateCallback callback = new StateCallback();
            _stateEventsBus.OnGetCurrentState?.Invoke(callback);
            if (callback.State != GameState.PauseState)
            {
                ClearGUI();
                _stateEventsBus.OnPauseStateActivate?.Invoke();
                _uiEventBus.OnOpenPauseMenu?.Invoke();
            }
        }
        
        private void ClearGUI()
        {
            
        }
    }
}