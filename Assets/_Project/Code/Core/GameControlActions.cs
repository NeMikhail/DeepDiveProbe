using System.Collections.Generic;
using MAEngine;
using UnityEngine;
using Zenject;

namespace GameCoreModule
{
    public class GameControlActions : IAction, IInitialisation, ICleanUp
    {
        private StateEventsBus _stateEventsBus;
        private List<GameObject> _screensList;
        private CanvasList _canvasList;
        private UIEventBus _uiEventBus;

        [Inject]
        public void Construct(StateEventsBus stateEventsBus, CanvasList canvasList, UIEventBus uiEventBus)
        {
            _stateEventsBus = stateEventsBus;
            _canvasList = canvasList;
            _uiEventBus = uiEventBus;
        }

        public void Initialisation()
        {
            InitializeScreensList();
            _uiEventBus.OnOpenPauseMenu += ShowPauseScreen;
            _stateEventsBus.OnPlayStateActivate += ShowGUIScreen;
        }

        public void Cleanup()
        {
            _uiEventBus.OnOpenPauseMenu -= ShowPauseScreen;
            _stateEventsBus.OnPlayStateActivate -= ShowGUIScreen;
        }
        
        private void InitializeScreensList()
        {
            _screensList = new List<GameObject>
            {
                _canvasList.PauseCanvas,
                _canvasList.GUICanvas
            };
        }
        
        private void ShowPauseScreen()
        {
            ShowCurrentScreen(_canvasList.PauseCanvas);
        }

        private void ShowGUIScreen()
        {
            ShowCurrentScreen(_canvasList.GUICanvas);
        }
        
        
        private void ShowCurrentScreen(GameObject targetScreenObject)
        {
            foreach (GameObject canvasObject in _screensList)
            {
                if (canvasObject == targetScreenObject)
                {
                    canvasObject.SetActive(true);
                }
                else
                {
                    canvasObject.SetActive(false);
                }
            }
        }
    }
}