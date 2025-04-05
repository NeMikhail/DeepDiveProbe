using GameCoreModule;
using MAEngine;
using UnityEngine;
using Zenject;

namespace Player
{
    public class CameraMovementActions : IAction, IInitialisation, IFixedExecute
    {
        private SceneViewsContainer _sceneViewsContainer;
        private PlayerView _playerView;
        private PlayerConfig _playerConfig;
        private Rigidbody2D _cameraRB;

        [Inject]
        public void Construct(SceneViewsContainer sceneViewsContainer,
            PlayerConfig playerConfig)
        {
            _sceneViewsContainer = sceneViewsContainer;
            _playerConfig = playerConfig;
        }

        public void Initialisation()
        {
            _playerView = _sceneViewsContainer.GetPlayerView();
            _cameraRB = Camera.main.gameObject.GetComponent<Rigidbody2D>();
            SetStartCameraPosition();
        }

        private void SetStartCameraPosition()
        {
            Vector3 startPosition = new Vector3(_playerView.transform.position.x,
                _playerView.transform.position.y,
                _cameraRB.gameObject.transform.position.z);
            _cameraRB.gameObject.transform.position = startPosition;
        }

        public void FixedExecute(float fixedDeltaTime)
        {
            Vector2 cameraPosition = _cameraRB.gameObject.transform.position;
            Vector2 playerPosition = _playerView.Object.transform.position;
            if (cameraPosition != playerPosition)
            {
                Vector3 velocity = (playerPosition - cameraPosition) * Time.deltaTime * _playerConfig.CameraSpeed * 100f;
                _cameraRB.linearVelocity = velocity;
            }
        }
    }
}
