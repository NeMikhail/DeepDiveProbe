using GameCoreModule;
using MAEngine;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerMovementActions : IAction, IInitialisation, IFixedExecute
    {
        private SceneViewsContainer _sceneViewsContainer;
        private PlayerView _playerView;
        private InputEventBus _inputEvents;
        private PlayerConfig _playerConfig;
        private Vector2 _movementVector;
        private Vector2 _lastPosition;

        [Inject]
        public void Construct(SceneViewsContainer sceneViewsContainer, InputEventBus inputEvents,
            PlayerConfig playerConfig)
        {
            _sceneViewsContainer = sceneViewsContainer;
            _inputEvents = inputEvents;
            _playerConfig = playerConfig;
        }

        public void Initialisation()
        {
            _playerView = _sceneViewsContainer.GetPlayerView();
            _inputEvents.OnMovementInputValueChanged += UpdateInputVector;
            _lastPosition = _playerView.Object.transform.position;
        }

        private void UpdateInputVector(Vector2 movementVector)
        {
            _movementVector = movementVector;
        }

        public void FixedExecute(float fixedDeltaTime)
        {
            if (_movementVector != Vector2.zero)
            {
                _playerView.PlayerRB.AddForce(_movementVector * _playerConfig.Speed);
            }

            Vector2 antiInertia = (_lastPosition - 
                (Vector2)_playerView.Object.transform.position)
                * _playerConfig.InertiaModifier * 100f;
            _lastPosition = (Vector2)_playerView.Object.transform.position;
            _playerView.PlayerRB.AddForce(antiInertia);
        }
    }
}
