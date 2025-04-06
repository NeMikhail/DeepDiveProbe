using System;

namespace GameCoreModule
{
    public class StateEventsBus
    {
        private Action _onPlayStateActivate;
        private Action _onPauseStateActivate;
        private Action<StateCallback> _onGetCurrentState;

        public Action OnPlayStateActivate
        { get => _onPlayStateActivate; set => _onPlayStateActivate = value; }
        public Action OnPauseStateActivate
        { get => _onPauseStateActivate; set => _onPauseStateActivate = value; }
        public Action<StateCallback> OnGetCurrentState
        { get => _onGetCurrentState; set => _onGetCurrentState = value; }
        
    }
}