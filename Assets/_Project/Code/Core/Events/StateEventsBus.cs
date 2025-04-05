using System;

namespace GameCoreModule
{
    public class StateEventsBus
    {
        private Action _onPlayStateActivate;
        private Action _onPauseStateActivate;

        public Action OnPlayStateActivate
        { get => _onPlayStateActivate; set => _onPlayStateActivate = value; }
        public Action OnPauseStateActivate
        { get => _onPauseStateActivate; set => _onPauseStateActivate = value; }
        
    }
}