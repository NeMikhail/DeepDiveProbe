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
            InitializeObstaclesActions();
            InitializePlayerActions();
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

        private void InitializeObstaclesActions()
        {
            ObstaclesActions obstaclesActions =
                _di.Resolve<ObstaclesActions>();
            _actions.Add(obstaclesActions);
        }

        private void InitializePlayerActions()
        {
            PlayerActions playerActions =
                _di.Resolve<PlayerActions>();
            _actions.Add(playerActions);
        }
    }
}
