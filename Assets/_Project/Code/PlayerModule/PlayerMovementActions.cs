using GameCoreModule;
using MAEngine;
using MAEngine.Extention;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerMovementActions : IAction, IInitialisation, ICleanUp, IFixedExecute
    {
        private SceneViewsContainer _sceneViewsContainer;
        private PlayerView _playerView;
        private InputEventBus _inputEvents;
        private PlayerConfig _playerConfig;
        private PlayerEventBus _playerEventBus;
        
        private int _currentLine;
        private int _currentLayer;
        private int _targetLayer;
        private MovementDirection _movementDirection;
        private bool _isMoving;
        private Timer _movementTimer;
        private float _targetPositionX;
        private float _startPositionX;

        [Inject]
        public void Construct(SceneViewsContainer sceneViewsContainer, InputEventBus inputEvents,
            PlayerConfig playerConfig, PlayerView playerView, PlayerEventBus playerEventBus)
        {
            _sceneViewsContainer = sceneViewsContainer;
            _inputEvents = inputEvents;
            _playerConfig = playerConfig;
            _playerView = playerView;
            _playerEventBus = playerEventBus;
        }

        public void Initialisation()
        {
            _currentLine = 2;
            _currentLayer = 2;
            _playerView.CurrentLayer = 2;
            _inputEvents.OnMoveLeftButtonPerformed += TryMoveLeft;
            _inputEvents.OnMoveRightButtonPerformed += TryMoveRight;
            _inputEvents.OnMoveUpButtonPerformed += TryMoveUp;
            _inputEvents.OnMoveDownButtonPerformed += TryMoveDown;
        }
        
        public void Cleanup()
        {
            _inputEvents.OnMoveLeftButtonPerformed -= TryMoveLeft;
            _inputEvents.OnMoveRightButtonPerformed -= TryMoveRight;
            _inputEvents.OnMoveUpButtonPerformed -= TryMoveUp;
            _inputEvents.OnMoveDownButtonPerformed -= TryMoveDown;
        }

        public void FixedExecute(float fixedDeltaTime)
        {
            if (_isMoving)
            {
                switch (_movementDirection)
                {
                    case MovementDirection.Left:
                        MoveHorizontal();
                        break;
                    case MovementDirection.Right:
                        MoveHorizontal();
                        break;
                    case MovementDirection.Up:
                        MoveVertical();
                        break;
                    case MovementDirection.Down:
                        MoveVertical();
                        break;
                }
            }

            MoveDown();
        }

        private void MoveDown()
        {
            _playerView.PlayerRB.linearVelocity =
                new Vector2(_playerView.PlayerRB.linearVelocity.x, -1 * _playerConfig.Speed);
            _playerEventBus.OnDepthChanged?.Invoke(-(int)_playerView.transform.position.y);
        }

        private void MoveVertical()
        {
            if (_movementTimer.Wait())
            {
                _isMoving = false;
                _currentLayer = _targetLayer;
                _playerView.CurrentLayer = _currentLayer;
                _playerEventBus.OnChangeLayer?.Invoke(_currentLayer);
                _playerView.PlayerSpriteRenderer.sprite = _playerConfig.DefaultSprite;
            }
        }

        private void MoveHorizontal()
        {
            if (_movementTimer.Wait())
            {
                _isMoving = false;
                _playerView.CurrentLine = _currentLine;
                _playerView.PlayerRB.linearVelocity =
                    new Vector2(0, _playerView.PlayerRB.linearVelocity.y);
                _playerView.PlayerRB.position = new Vector2(_targetPositionX, _playerView.PlayerRB.position.y);
                _playerView.PlayerSpriteRenderer.sprite = _playerConfig.DefaultSprite;
            }
            else
            {
                float progress = 1f - (_movementTimer.GetRemainingTime() / _movementTimer.Duration);
                float desiredX = Mathf.Lerp(_startPositionX, _targetPositionX, progress);
                float velocityX = (desiredX - _playerView.PlayerRB.position.x) / Time.fixedDeltaTime;
                _playerView.PlayerRB.linearVelocity = new Vector2(velocityX, _playerView.PlayerRB.linearVelocity.y);
            }
        }

        private void TryMoveLeft()
        {
            if (!_isMoving)
            {
                if (_currentLine != 1)
                {
                    _currentLine--;
                    _movementDirection = MovementDirection.Left;
                    _movementTimer = new Timer(_playerConfig.LineChangingTime);
                    _isMoving = true;
                    if (_currentLine == 1)
                    {
                        _targetPositionX = _playerConfig.Line1PositionX;
                    }
                    else if(_currentLine == 2)
                    {
                        _targetPositionX = _playerConfig.Line2PositionX;
                    }
                    _startPositionX = _playerView.PlayerRB.position.x;
                }
            }
        }
        
        private void TryMoveRight()
        {
            if (!_isMoving)
            {
                if (_currentLine != 3)
                {
                    _currentLine++;
                    _movementDirection = MovementDirection.Right;
                    _movementTimer = new Timer(_playerConfig.LineChangingTime);
                    _isMoving = true;
                    if (_currentLine == 3)
                    {
                        _targetPositionX = _playerConfig.Line3PositionX;
                    }
                    else if(_currentLine == 2)
                    {
                        _targetPositionX = _playerConfig.Line2PositionX;
                    }
                    _startPositionX = _playerView.PlayerRB.position.x;
                }
            }
        }
        
        private void TryMoveUp()
        {
            if (!_isMoving)
            {
                if (_currentLayer != 3)
                {
                    _targetLayer = _currentLayer + 1;
                    _movementDirection = MovementDirection.Up;
                    _playerView.PlayerSpriteRenderer.sprite = _playerConfig.GoingUpSprite;
                    _movementTimer = new Timer(_playerConfig.LayerChangingTime);
                    _isMoving = true;
                }
            }
        }
        
        private void TryMoveDown()
        {
            if (!_isMoving)
            {
                if (_currentLayer != 1)
                {
                    _targetLayer = _currentLayer - 1;
                    _movementDirection = MovementDirection.Down;
                    _playerView.PlayerSpriteRenderer.sprite = _playerConfig.GoingDownSprite;
                    _movementTimer = new Timer(_playerConfig.LayerChangingTime);
                    _isMoving = true;
                }
            }
        }
    }
}
