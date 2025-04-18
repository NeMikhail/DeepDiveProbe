﻿using GameCoreModule;
using MAEngine;
using MAEngine.Extention;
using UnityEngine;
using Upgrade;
using Zenject;

namespace Player
{
    public class PlayerMovementActions : IAction, IInitialisation, ICleanUp, IFixedExecute
    {
        private PlayerView _playerView;
        private InputEventBus _inputEvents;
        private PlayerConfig _playerConfig;
        private PlayerEventBus _playerEventBus;
        private GameEventBus _gameEventBus;
        private UpgradesContainer _upgradeContainer;
        private UpgradesEventBus _upgradeEventBus;
        
        private int _currentLine;
        private int _currentLayer;
        private int _targetLayer;
        private MovementDirection _movementDirection;
        private bool _isMoving;
        private Timer _movementTimer;
        private float _targetPositionX;
        private float _startPositionX;
        private bool _isPlayingState;
        private float _lineChangingTime;
        private float _layerChangingTime;

        [Inject]
        public void Construct(InputEventBus inputEvents,
            PlayerConfig playerConfig, PlayerView playerView, PlayerEventBus playerEventBus,
            GameEventBus gameEventBus, UpgradesContainer upgradeContainer, UpgradesEventBus upgradeEventBus)
        {
            _inputEvents = inputEvents;
            _playerConfig = playerConfig;
            _playerView = playerView;
            _playerEventBus = playerEventBus;
            _gameEventBus = gameEventBus;
            _upgradeContainer = upgradeContainer;
            _upgradeEventBus = upgradeEventBus;
        }

        public void Initialisation()
        {
            _currentLine = 2;
            _currentLayer = 2;
            _playerView.CurrentLayer = 2;
            _isPlayingState = true;
            _inputEvents.OnMoveLeftButtonPerformed += TryMoveLeft;
            _inputEvents.OnMoveRightButtonPerformed += TryMoveRight;
            _inputEvents.OnMoveUpButtonPerformed += TryMoveUp;
            _inputEvents.OnMoveDownButtonPerformed += TryMoveDown;
            _playerEventBus.OnStageChanged += ChangeSpeed;
            _gameEventBus.OnStateChanged += ChangeState;
            _upgradeEventBus.OnUpgradeApplied += UpgradeApplied;
            _lineChangingTime = _playerConfig.LineChangingTime;
            _layerChangingTime = _playerConfig.LayerChangingTime;
        }

        public void Cleanup()
        {
            _inputEvents.OnMoveLeftButtonPerformed -= TryMoveLeft;
            _inputEvents.OnMoveRightButtonPerformed -= TryMoveRight;
            _inputEvents.OnMoveUpButtonPerformed -= TryMoveUp;
            _inputEvents.OnMoveDownButtonPerformed -= TryMoveDown;
            _playerEventBus.OnStageChanged -= ChangeSpeed;
            _gameEventBus.OnStateChanged -= ChangeState;
        }

        public void FixedExecute(float fixedDeltaTime)
        {
            if (_isPlayingState)
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
            else
            {
                _playerView.PlayerRB.linearVelocity = Vector2.zero;
            }
        }
        
        private void UpgradeApplied(UpgradeID upgradeID)
        {
            if (upgradeID == UpgradeID.UpgradeAntiDepth)
            {
                _playerView.CurrentSpeed = _playerConfig.Speed;
            }
            else if (upgradeID == UpgradeID.UpgradeSpeedBoost)
            {
                _lineChangingTime -= 0.1f;
                _layerChangingTime -= _layerChangingTime - 0.1f;
            }
            
        }


        private void ChangeState(GameState state)
        {
            if (state == GameState.PlayState)
            {
                _isPlayingState = true;
            }
            else
            {
                _isPlayingState = false;
            }
        }
        
        private void ChangeSpeed(StageID stageID)
        {
            if (stageID == StageID.Stage1)
            {
                //PlayerPrefs.SetInt(UpgradeRequirement.GameStage3Achived.ToString(), 0);
                _playerView.CurrentSpeed = _playerConfig.Speed;
            }
            else if (stageID == StageID.Stage2)
            {
                PlayerPrefs.SetInt(UpgradeRequirement.GameStage2Achived.ToString(), 1);
                if (!_upgradeContainer.ActiveUpgrades.Contains(UpgradeID.UpgradeAntiDepth))
                {
                    _playerView.CurrentSpeed = _playerConfig.Speed * 1.3f;
                }
            }
            else if (stageID == StageID.Stage3)
            {
                PlayerPrefs.SetInt(UpgradeRequirement.GameStage3Achived.ToString(), 1);
                if (!_upgradeContainer.ActiveUpgrades.Contains(UpgradeID.UpgradeAntiDepth))
                {
                    _playerView.CurrentSpeed = _playerConfig.Speed * 1.5f;
                }
            }
        }

        private void MoveDown()
        {
            _playerView.PlayerRB.linearVelocity =
                new Vector2(_playerView.PlayerRB.linearVelocity.x, -1 * _playerView.CurrentSpeed);
            int absDepth = (int)Mathf.Abs(_playerView.transform.position.y);
            _playerEventBus.OnDepthChanged?.Invoke(absDepth);
            StageID stageID = StageID.NONE;
            if (absDepth < 400)
            {
                stageID = StageID.Stage1;
            }
            else if (absDepth < 800)
            {
                stageID = StageID.Stage2;
            }
            else
            {
                stageID = StageID.Stage3;
            }

            if (_playerView.CurrentStage != stageID)
            {
                _playerView.CurrentStage = stageID;
                _playerEventBus.OnStageChanged?.Invoke(stageID);
            }
            _playerView.CurrentDepth = absDepth;
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
            if (!_isMoving && _isPlayingState)
            {
                if (_currentLine != 1)
                {
                    _currentLine--;
                    _movementDirection = MovementDirection.Left;
                    _movementTimer = new Timer(_lineChangingTime);
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
            if (!_isMoving && _isPlayingState)
            {
                if (_currentLine != 3)
                {
                    _currentLine++;
                    _movementDirection = MovementDirection.Right;
                    _movementTimer = new Timer(_lineChangingTime);
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
            if (!_isMoving && _isPlayingState)
            {
                if (_currentLayer != 3)
                {
                    _targetLayer = _currentLayer + 1;
                    _movementDirection = MovementDirection.Up;
                    _playerView.PlayerSpriteRenderer.sprite = _playerConfig.GoingUpSprite;
                    _movementTimer = new Timer(_layerChangingTime);
                    _isMoving = true;
                }
            }
        }
        
        private void TryMoveDown()
        {
            if (!_isMoving && _isPlayingState)
            {
                if (_currentLayer != 1)
                {
                    _targetLayer = _currentLayer - 1;
                    _movementDirection = MovementDirection.Down;
                    _playerView.PlayerSpriteRenderer.sprite = _playerConfig.GoingDownSprite;
                    _movementTimer = new Timer(_lineChangingTime);
                    _isMoving = true;
                }
            }
        }
    }
}
