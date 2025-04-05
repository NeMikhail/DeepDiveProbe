using System.Collections.Generic;
using MAEngine;
using MAEngine.Extention;
using UnityEngine;

namespace Player
{
    public class ObstacleView : MonoBehaviour
    {
        [SerializeField] private GameObject _obstacleObject;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Scene2DActor _scene2DActor;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        private int _layerId;
        private PrefabID _prefabID;
        private List<ObstacleView> _obstacles;
        private MovementDirection _direction;
        private int _currentLine;
        private int _currentLayer;
        private Timer _movingTimer;
        private Vector2 _targetPosition;
        private Vector2 _startPosition;
        private bool _isStalking;
        private float _stalkingSpeed;
        private PlayerView _playerView;

        public GameObject ObstacleObject => _obstacleObject;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public Scene2DActor Scene2DActor => _scene2DActor;
        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public int LayerId => _layerId;
        public PrefabID PrefabID => _prefabID;
        public List<ObstacleView> Obstacles => _obstacles;
        public MovementDirection Direction
        {get => _direction; set => _direction = value; }
        public int CurrentLine
        {get => _currentLine; set => _currentLine = value; }
        public int CurrentLayer
        {get => _currentLayer; set => _currentLayer = value; }
        public Timer MovingTimer => _movingTimer;
        public Vector2 TargetPosition => _targetPosition;
        public Vector2 StartPosition => _startPosition;
        public bool IsStalking => _isStalking;
        public float StalkingSpeed => _stalkingSpeed;
        public PlayerView PlayerView => _playerView;
        

        public void SetObstacleData(int layer, PrefabID prefabID,List<ObstacleView> obstacles, int line)
        {
            _layerId = layer;
            _prefabID = prefabID;
            _obstacles = obstacles;
            _direction = MovementDirection.Left;
            _currentLine = line;
            _currentLayer = layer;
            _isStalking = false;
        }
        
        public void StartMoving(float time)
        {
            _movingTimer = new Timer(time);
            _startPosition = _rigidbody2D.transform.position;
            switch (_direction)
            {
                case MovementDirection.Left:
                    _targetPosition = _startPosition + new Vector2(-3f, 0);
                    break;
                case MovementDirection.Right:
                    _targetPosition = _startPosition + new Vector2(3f, 0);
                    break;
                case MovementDirection.Up:
                    _targetPosition = _startPosition + new Vector2(0, 5f);
                    break;
                case MovementDirection.Down:
                    _targetPosition = _startPosition + new Vector2(0, -5f);
                    break;
                default:
                    Debug.LogError("Invalid movement direction");
                    break;
            }
        }
        public void StartStalkMoving(float speed, PlayerView playerView)
        {
            _isStalking = true;
            _stalkingSpeed = speed;
            _playerView = playerView;
        }

        public void Move()
        {
            if (_isStalking)
            {
                _playerView.IsLightOn = true;
                
                if (_playerView.IsLightOn)
                {
                    _targetPosition = _playerView.PlayerRB.gameObject.transform.position;
                }
                else
                {
                    _targetPosition = _rigidbody2D.transform.position;
                }

                Vector2 direction = (_targetPosition - (Vector2)_rigidbody2D.transform.position).normalized;
                _rigidbody2D.linearVelocity = direction * _stalkingSpeed;
            }
            else if (_movingTimer.Wait())
            {
                if (_direction == MovementDirection.Right)
                {
                    _currentLine++;
                }
                else if (_direction == MovementDirection.Left)
                {
                    _currentLine--;
                }
                
                _rigidbody2D.linearVelocity =
                    new Vector2(0, 0);
                _rigidbody2D.position = new Vector2(_targetPosition.x, _targetPosition.y);
                _movingTimer = null;
            }
            else
            {
                float progress = 1f - (_movingTimer.GetRemainingTime() / _movingTimer.Duration);
                float desiredX = Mathf.Lerp(_startPosition.x, _targetPosition.x, progress);
                float desiredY = Mathf.Lerp(_startPosition.y, _targetPosition.y, progress);
                float velocityX = (desiredX - _rigidbody2D.position.x) / Time.fixedDeltaTime;
                float velocityY = (desiredY - _rigidbody2D.position.y) / Time.fixedDeltaTime;
                _rigidbody2D.linearVelocity = new Vector2(velocityX, velocityY);
            }
        }

        private void OnDestroy()
        {
            _movingTimer = null;
            _isStalking = false;
        }
    }
}