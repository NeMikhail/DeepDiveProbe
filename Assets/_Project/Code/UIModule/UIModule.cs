using MAEngine;

namespace UI
{
    public class UIModule : BasicModule
    {
        public override void Initialise()
        {
            base.Initialise();
            InitializeUIActions();
            InitializeGUIActions();
            InitializePauseActions();
        }

        private void InitializeUIActions()
        {
            UIActions uiAction =
                _di.Resolve<UIActions>();
            _actions.Add(uiAction);
        }

        private void InitializeGUIActions()
        {
            GUIActions guiActions =
                _di.Resolve<GUIActions>();
            _actions.Add(guiActions);
        }
        
        private void InitializePauseActions()
        {
            PauseActions pauseActions =
                _di.Resolve<PauseActions>();
            _actions.Add(pauseActions);
        }
    }
}