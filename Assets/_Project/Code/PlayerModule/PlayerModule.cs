using MAEngine;

namespace Player
{
    public class PlayerModule : BasicModule
    {
        public override void Initialise()
        {
            base.Initialise();
            InitializeSpawnAction();
            InitializeMovementActions();
            InitializeCameraMovement();
        }

        private void InitializeSpawnAction()
        {
            PlayerSpawn playerSpawn = _di.Resolve<PlayerSpawn>();
            _actions.Add(playerSpawn);
        }

        private void InitializeMovementActions()
        {
            PlayerMovementActions playerMovement =
                _di.Resolve<PlayerMovementActions>();
            _actions.Add(playerMovement);
        }

        private void InitializeCameraMovement()
        {
            CameraMovementActions cameraMovement =
                _di.Resolve<CameraMovementActions>();
            _actions.Add(cameraMovement);
        }
    }
}
