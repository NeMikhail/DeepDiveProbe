using System;

namespace GameCoreModule
{
    public class UIEventBus
    {
        private Action<PointerCheckEventCallback> _onPointerCheck;
        
        public Action<PointerCheckEventCallback> OnPointerCheck
        { get => _onPointerCheck; set => _onPointerCheck = value; }
    }
}