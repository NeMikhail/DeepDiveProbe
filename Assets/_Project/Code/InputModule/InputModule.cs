using MAEngine;

namespace Input
{
    public class InputModule : BasicModule
    {
        public override void Initialise()
        {
            base.Initialise();
            InitializeAction();
        }
        
        private void InitializeAction()
        {
            InputActions inputAction =
                _di.Resolve<InputActions>();
            _actions.Add(inputAction);
        }
    }
}
