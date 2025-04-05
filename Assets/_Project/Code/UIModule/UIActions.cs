using System.Collections.Generic;
using GameCoreModule;
using MAEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;

namespace UI
{
    public class UIActions : IAction, IInitialisation, ICleanUp
    {
        private UIEventBus _uiEventBus;
        
        private int _uILayer;

        [Inject]
        public void Construct(UIEventBus uiEventBus)
        {
            _uiEventBus = uiEventBus;
        }
        
        public void Initialisation()
        {
            _uILayer = LayerMask.NameToLayer("UI");
            _uiEventBus.OnPointerCheck += CheckIsPointerOnUI;
        }

        public void Cleanup()
        {
            _uiEventBus.OnPointerCheck -= CheckIsPointerOnUI;
        }
        
        private void CheckIsPointerOnUI(PointerCheckEventCallback callback)
        { 
            callback.IsPointerOverUI = IsPointerOverUIElement();
        }
        
        public bool IsPointerOverUIElement()
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults());
        }
        
        private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == _uILayer)
                    return true;
            }
            return false;
        }
        
        static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            if (Touchscreen.current != null)
            {
                if (Touchscreen.current.touches.Count > 0)
                {
                    eventData.position = Touchscreen.current.position.ReadValue();
                }
            }
            else
            {
                eventData.position = Mouse.current.position.ReadValue();
            }
#if PLATFORM_WEBGL
            eventData.position = Mouse.current.position.ReadValue();
#endif
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }
}