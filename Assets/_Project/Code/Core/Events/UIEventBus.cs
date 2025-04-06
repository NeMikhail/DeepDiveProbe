using System;

namespace GameCoreModule
{
    public class UIEventBus
    {
        private Action<PointerCheckEventCallback> _onPointerCheck;
        private Action _onOpenPauseMenu;
        
        public Action<PointerCheckEventCallback> OnPointerCheck
        { get => _onPointerCheck; set => _onPointerCheck = value; }
        public Action OnOpenPauseMenu
        { get => _onOpenPauseMenu; set => _onOpenPauseMenu = value; }
    }
}