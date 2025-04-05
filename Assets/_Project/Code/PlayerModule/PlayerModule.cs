using MAEngine;

namespace Player
{
    public class PlayerModule : BasicModule
    {
        public override void Initialise()
        {
            base.Initialise();
            InitializeMovementActions();
            InitializeMapGeneration();
        }

        private void InitializeMovementActions()
        {
            PlayerMovementActions playerMovement =
                _di.Resolve<PlayerMovementActions>();
            _actions.Add(playerMovement);
        }
        
        private void InitializeMapGeneration()
        {
            MapGenerationActions mapGenerationActions =
                _di.Resolve<MapGenerationActions>();
            _actions.Add(mapGenerationActions);
        }
    }
}
