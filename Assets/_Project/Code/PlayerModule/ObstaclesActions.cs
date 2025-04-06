using System;
using System.Collections.Generic;
using System.Linq;
using GameCoreModule;
using MAEngine;
using UnityEngine;
using Zenject;

namespace Player
{
    public class ObstaclesActions : IAction, IInitialisation, ICleanUp, IFixedExecute
    {
        private MapView _mapView;
        private GameEventBus _gameEventBus;
        private PlayerView _playerView;
        private PlayerEventBus _playerEventBus;
        private ObstaclesSpawnConfig _obstaclesSpawnConfig;
        
        private System.Random _random;
        private Dictionary<int, ObstaclesLine> _obstacleLines;
        private int _currentSpawnLineIndex;
        private int _layerActiveObject;
        private int _layerInactiveObject;
        private List<ObstacleView> _movingObstacles;
        private bool _isPlaying;

        private float _stageOxygenCount;
        private List<PrefabID> _stageSpawnableObstacles;
        

        [Inject]
        public void Construct(MapView mapView, GameEventBus gameEventBus, PlayerView playerView,
            PlayerEventBus playerEventBus, ObstaclesSpawnConfig obstaclesSpawnConfig)
        {
            _mapView = mapView;
            _gameEventBus = gameEventBus;
            _playerView = playerView;
            _playerEventBus = playerEventBus;
            _obstaclesSpawnConfig = obstaclesSpawnConfig;
        }


        public void Initialisation()
        {
            _obstacleLines = new Dictionary<int, ObstaclesLine>();
            _random = new System.Random();
            _movingObstacles = new List<ObstacleView>();
            _layerActiveObject = LayerMask.NameToLayer("ActiveObstacle");
            _layerInactiveObject = LayerMask.NameToLayer("InactiveObstacle");
            _stageOxygenCount = _obstaclesSpawnConfig.Stage1OxygenSpawnCount;
            _stageSpawnableObstacles = _obstaclesSpawnConfig.Stage1Obstacles;
            BindSpawnLinesTriggers(_mapView.Layer1View);
            _currentSpawnLineIndex = 0;
            SpawnObstaclesOnLine();
            _currentSpawnLineIndex = 1;
            SpawnObstaclesOnLine();
            _playerEventBus.OnTriggerSpawnLine += SpawnObstaclesOnLine;
            _playerEventBus.OnChangeLayer += ChangeCurrentLayerForObstacles;
            _playerEventBus.OnStageChanged += ChangeStage;
            _gameEventBus.OnStateChanged += ChangeState;
            _isPlaying = true;
        }

        public void Cleanup()
        {
            UnBindSpawnLinesTriggers(_mapView.Layer1View);
            _playerEventBus.OnTriggerSpawnLine -= SpawnObstaclesOnLine;
            _playerEventBus.OnChangeLayer -= ChangeCurrentLayerForObstacles;
            _playerEventBus.OnStageChanged -= ChangeStage;
            _gameEventBus.OnStateChanged -= ChangeState;
        }
        
        public void FixedExecute(float fixedDeltaTime)
        {
            if (_isPlaying)
            {
                AddMovingObstacles();
                MoveObstacles();
            }
            else
            {
                StopMovables();
            }
        }

        private void ChangeState(GameState state)
        {
            if (state == GameState.PlayState)
            {
                _isPlaying = true;
            }
            else
            {
                _isPlaying = false;
            }
        }
        
        private void ChangeStage(StageID stageID)
        {
            if (stageID == StageID.Stage1)
            {
                _stageOxygenCount = _obstaclesSpawnConfig.Stage1OxygenSpawnCount;
                _stageSpawnableObstacles = _obstaclesSpawnConfig.Stage1Obstacles;
            }
            else if (stageID == StageID.Stage2)
            {
                _stageOxygenCount = _obstaclesSpawnConfig.Stage2OxygenSpawnCount;
                _stageSpawnableObstacles = _obstaclesSpawnConfig.Stage2Obstacles;
            }
            else
            {
                _stageOxygenCount = _obstaclesSpawnConfig.Stage3OxygenSpawnCount;
                _stageSpawnableObstacles = _obstaclesSpawnConfig.Stage3Obstacles;
            }
        }

        private void MoveObstacles()
        {
            List<ObstacleView> obstaclesToRemove = new List<ObstacleView>();
            foreach (ObstacleView obstacleView in _movingObstacles)
            {
                if (obstacleView.MovingTimer != null || obstacleView.IsStalking)
                {
                    obstacleView.Move();
                }
                else
                {
                    obstaclesToRemove.Add(obstacleView);
                }
            }
            foreach (ObstacleView obstacleView in obstaclesToRemove)
            {
                _movingObstacles.Remove(obstacleView);
            }
        }
        
        private void StopMovables()
        {
            foreach (ObstacleView obstacleView in _movingObstacles)
            {
                if (obstacleView.MovingTimer != null || obstacleView.IsStalking)
                {
                    obstacleView.Stop();
                }
            }
        }

        private void AddMovingObstacles()
        {
            foreach (ObstaclesLine line in _obstacleLines.Values)
            {
                foreach (ObstacleView obstacleView in line.Obstacles)
                {
                    switch (obstacleView.PrefabID)
                    {
                        case PrefabID.OxygenBubble:
                            MoveObstacleUp(obstacleView, 1f);
                            break;
                        case PrefabID.MovingHorizontalObstacle:
                            MoveObstacleHorizontal(obstacleView, 2f);
                            break;
                        case PrefabID.MovingVerticalUpObstacle:
                            MoveObstacleUp(obstacleView, 1.5f);
                            break;
                        case PrefabID.MovingVerticalDownObstacle:
                            MoveObstacleDown(obstacleView, 0.8f);
                            break;
                        case PrefabID.MovingChaoticVerticalObstacle:
                            if (_random.Next(0, 2) == 1)
                            {
                                MoveObstacleUp(obstacleView, 3);
                            }
                            else
                            {
                                MoveObstacleDown(obstacleView, 1);
                            }
                            break;
                        case PrefabID.MovingChaoticHorizontalObstacle:
                            MoveObstacleHorizontalChaotic(obstacleView, 2.5f);
                            break;
                        case PrefabID.MovingLayersObstacle:
                            MoveObstacleUp(obstacleView, 3);
                        break;
                        case PrefabID.StalkingObstacle:
                            if (!obstacleView.IsStalking)
                            {
                                StartStalkingMovement(obstacleView, 1.5f);
                            }
                            break;
                        default:
                            break;
                        
                    }
                }
            }
        }

        private void StartStalkingMovement(ObstacleView obstacleView, float speed)
        {
            obstacleView.StartStalkMoving(speed, _playerView);
            _movingObstacles.Add(obstacleView);
        }

        private void MoveObstacleHorizontalChaotic(ObstacleView obstacleView, float speed)
        {
            if (!_movingObstacles.Contains(obstacleView))
            {
                if (obstacleView.CurrentLine != 1 && obstacleView.CurrentLine != 3)
                {
                    if (_random.Next(0, 2) == 1)
                    {
                        MovementDirection lastDirection = obstacleView.Direction;
                        obstacleView.Direction = MovementDirection.Right;
                        if (lastDirection != obstacleView.Direction)
                        {
                            ChangeDirection(obstacleView);
                        }
                    }
                    else
                    {
                        MovementDirection lastDirection = obstacleView.Direction;
                        obstacleView.Direction = MovementDirection.Left;
                        if (lastDirection != obstacleView.Direction)
                        {
                            ChangeDirection(obstacleView);
                        }
                    }
                }
                else if (obstacleView.CurrentLine == 1)
                {
                    MovementDirection lastDirection = obstacleView.Direction;
                    obstacleView.Direction = MovementDirection.Right;
                    if (lastDirection != obstacleView.Direction)
                    {
                        ChangeDirection(obstacleView);
                    }
                }
                else if (obstacleView.CurrentLine == 3)
                {
                    MovementDirection lastDirection = obstacleView.Direction;
                    obstacleView.Direction = MovementDirection.Left;
                    if (lastDirection != obstacleView.Direction)
                    {
                        ChangeDirection(obstacleView);
                    }
                }
                _movingObstacles.Add(obstacleView);
                obstacleView.StartMoving(10 / speed);
            }
        }

        private void MoveObstacleDown(ObstacleView obstacleView, float speed)
        {
            if (!_movingObstacles.Contains(obstacleView))
            {
                obstacleView.Direction = MovementDirection.Down;
                _movingObstacles.Add(obstacleView);
                obstacleView.StartMoving(10 / speed);
            }
        }

        private void MoveObstacleUp(ObstacleView obstacleView, float speed)
        {
            if (!_movingObstacles.Contains(obstacleView))
            {
                obstacleView.Direction = MovementDirection.Up;
                _movingObstacles.Add(obstacleView);
                obstacleView.StartMoving(10 / speed);
            }
        }
        
        private void MoveObstacleHorizontal(ObstacleView obstacleView, float speed)
        {
            if (!_movingObstacles.Contains(obstacleView))
            {
                if (obstacleView.CurrentLine == 1)
                {
                    MovementDirection lastDirection = obstacleView.Direction;
                    obstacleView.Direction = MovementDirection.Right;
                    if (lastDirection != obstacleView.Direction)
                    {
                        ChangeDirection(obstacleView);
                    }

                }
                else if (obstacleView.CurrentLine == 3)
                {
                    MovementDirection lastDirection = obstacleView.Direction;
                    obstacleView.Direction = MovementDirection.Left;
                    if (lastDirection != obstacleView.Direction)
                    {
                        ChangeDirection(obstacleView);
                    }
                }
                _movingObstacles.Add(obstacleView);
                obstacleView.StartMoving(10 / speed);
            }
        }

        private void ChangeDirection(ObstacleView obstacleView)
        {
            Vector3 currentScale = obstacleView.SpriteRenderer.gameObject.transform.localScale;
            obstacleView.SpriteRenderer.gameObject.transform.localScale =
                new Vector3(-1 * currentScale.x, currentScale.y, currentScale.z);
        }

        private void ChangeCurrentLayerForObstacles(int currentIndex)
        {
            Vector3 smallSize1 = new Vector3(0.8f, 0.8f, 1);
            Vector3 smallSize2 = new Vector3(0.5f, 0.5f, 1);
            Vector3 bigSize1 = new Vector3(1.2f, 1.2f, 1);
            Vector3 bigSize2 = new Vector3(1.4f, 1.4f, 1);
            foreach (KeyValuePair<int, ObstaclesLine> obstaclesLine in _obstacleLines)
            {
                List<ObstacleView> obstacleViews = obstaclesLine.Value.Obstacles;
                foreach (ObstacleView obstacleView in obstacleViews)
                {
                    Vector3 scaleMultipler = Vector3.one;
                    if (obstacleView.SpriteRenderer.gameObject.transform.localScale.x > 0)
                    {
                        scaleMultipler = Vector3.one;
                    }
                    else
                    {
                        scaleMultipler = new Vector3(-1, 1, 1);
                    }
                    if (obstacleView.LayerId == currentIndex)
                    {
                        obstacleView.ObstacleObject.layer = _layerActiveObject;
                        obstacleView.SpriteRenderer.color = Color.white;
                        Vector3 scale = new Vector3(scaleMultipler.x, scaleMultipler.y, scaleMultipler.z);
                        obstacleView.SpriteRenderer.gameObject.transform.localScale = scale;
                    }
                    else if (obstacleView.LayerId > currentIndex + 1)
                    {
                        obstacleView.ObstacleObject.layer = _layerInactiveObject;
                        obstacleView.SpriteRenderer.color = Color.black;
                        obstacleView.SpriteRenderer.color =
                            ChangeTransperency(obstacleView.SpriteRenderer.color, 0.1f);
                        Vector3 scale = new Vector3(scaleMultipler.x * bigSize2.x,
                            scaleMultipler.y * bigSize2.y, scaleMultipler.z * bigSize2.z);
                        obstacleView.SpriteRenderer.gameObject.transform.localScale = scale;
                    }
                    else if(obstacleView.LayerId > currentIndex)
                    {
                        obstacleView.ObstacleObject.layer = _layerInactiveObject;
                        obstacleView.SpriteRenderer.color = Color.gray;
                        obstacleView.SpriteRenderer.color =
                            ChangeTransperency(obstacleView.SpriteRenderer.color, 0.2f);
                        Vector3 scale = new Vector3(scaleMultipler.x * bigSize1.x,
                            scaleMultipler.y * bigSize1.y, scaleMultipler.z * bigSize1.z);
                        obstacleView.SpriteRenderer.gameObject.transform.localScale = scale;
                    }
                    else if(obstacleView.LayerId == currentIndex - 2)
                    {
                        obstacleView.ObstacleObject.layer = _layerInactiveObject;
                        obstacleView.SpriteRenderer.color = Color.black;
                        Vector3 scale = new Vector3(scaleMultipler.x * smallSize2.x,
                            scaleMultipler.y * smallSize2.y, scaleMultipler.z * smallSize2.z);
                        obstacleView.SpriteRenderer.gameObject.transform.localScale = scale;
                    }
                    else if(obstacleView.LayerId == currentIndex - 1)
                    {
                        obstacleView.ObstacleObject.layer = _layerInactiveObject;
                        obstacleView.SpriteRenderer.color = Color.gray;
                        Vector3 scale = new Vector3(scaleMultipler.x * smallSize1.x,
                            scaleMultipler.y * smallSize1.y, scaleMultipler.z * smallSize1.z);
                        obstacleView.SpriteRenderer.gameObject.transform.localScale = scale;
                    }
                }
            }
        }

        private Color ChangeTransperency(Color color, float transperency)
        {
            Color newColor = new Color(color.r, color.g, color.b, transperency);
            return newColor;
        }
        
        private void BindSpawnLinesTriggers(LayerView layerView)
        {
            foreach (SpawnZonesView spawnZonesView in layerView.SpawnZones)
            {
                spawnZonesView.SpawnLineActor.TriggerEnter += TriggerSpawnLine;
            }
        }
        
        private void UnBindSpawnLinesTriggers(LayerView layerView)
        {
            foreach (SpawnZonesView spawnZonesView in layerView.SpawnZones)
            {
                spawnZonesView.SpawnLineActor.TriggerEnter -= TriggerSpawnLine;
            }
        }

        private void TriggerSpawnLine(Scene2DActor scene2DActor, Collider2D collider)
        {
            if (collider.gameObject == _playerView.gameObject)
            {
                _currentSpawnLineIndex++;
                _playerEventBus.OnTriggerSpawnLine?.Invoke();
            }
        }
        
        private void SpawnObstaclesOnLine()
        {
            if (_currentSpawnLineIndex < _mapView.Layer1View.SpawnZones.Count)
            {
                List<Transform> spawnTransforms = new List<Transform>
                {
                    _mapView.Layer1View.SpawnZones[_currentSpawnLineIndex].Line1SpawnTransform,
                    _mapView.Layer1View.SpawnZones[_currentSpawnLineIndex].Line2SpawnTransform,
                    _mapView.Layer1View.SpawnZones[_currentSpawnLineIndex].Line3SpawnTransform,
                    _mapView.Layer2View.SpawnZones[_currentSpawnLineIndex].Line1SpawnTransform,
                    _mapView.Layer2View.SpawnZones[_currentSpawnLineIndex].Line2SpawnTransform,
                    _mapView.Layer2View.SpawnZones[_currentSpawnLineIndex].Line3SpawnTransform,
                    _mapView.Layer3View.SpawnZones[_currentSpawnLineIndex].Line1SpawnTransform,
                    _mapView.Layer3View.SpawnZones[_currentSpawnLineIndex].Line2SpawnTransform,
                    _mapView.Layer3View.SpawnZones[_currentSpawnLineIndex].Line3SpawnTransform
                };
                
                List<int> spawnIndexes = _obstaclesSpawnConfig.OxygenSpawnIndexes;
                List<PrefabID> spawnableObstacles;
                float oxygenCount;
                bool isOxygenSpawning = false;
                
                oxygenCount = _stageOxygenCount;
                spawnableObstacles = _stageSpawnableObstacles;
                
                if (oxygenCount < 1)
                {
                    oxygenCount = _random.Next(0, 2);
                }

                foreach (int index in spawnIndexes)
                {
                    int spawnIndex;
                    if (_currentSpawnLineIndex <= 10)
                    {
                        spawnIndex = _currentSpawnLineIndex;
                    }
                    else if(_currentSpawnLineIndex <= 100)
                    {
                        spawnIndex = _currentSpawnLineIndex % 10;
                    }
                    else if(_currentSpawnLineIndex <= 1000)
                    {
                        spawnIndex = _currentSpawnLineIndex % 100;
                        spawnIndex = spawnIndex % 10;
                    }
                    else
                    {
                        spawnIndex = _currentSpawnLineIndex % 1000;
                        spawnIndex = spawnIndex % 100;
                        spawnIndex = spawnIndex % 10;
                    }

                    if (index == spawnIndex)
                    {
                        isOxygenSpawning = true;
                    }
                }
                
                List<int> oxygenSpawnIndexes = new List<int>();
                if (isOxygenSpawning)
                {
                    for (int i = 0; i < oxygenCount; i++)
                    {
                        int index = 10;
                        while (!oxygenSpawnIndexes.Contains(index))
                        {
                            index = _random.Next(0, 10);
                            if (!oxygenSpawnIndexes.Contains(index))
                            {
                                oxygenSpawnIndexes.Add(index);
                            }
                        }
                    }
                }
                
                List<PrefabID> spawningPreset = CreateSpawnPreset(isOxygenSpawning,
                    oxygenSpawnIndexes, spawnableObstacles);
                ObstaclesLine obstaclesLine = new ObstaclesLine();
                obstaclesLine.Obstacles = new List<ObstacleView>();
                GameObjectSpawnCallback callback;
                int layerId;
                int lineid;
                for (int i = 0; i < spawningPreset.Count; i++)
                {
                    callback = new GameObjectSpawnCallback();
                    if (spawningPreset[i] != PrefabID.None)
                    {
                        _gameEventBus.OnSpawnObject(spawningPreset[i], Vector2.zero, spawnTransforms[i], callback);
                        GameObject spawnedObject = callback.SpawnedObject;
                        spawnedObject.transform.position = spawnTransforms[i].position;
                        ObstacleView obstacleView = spawnedObject.GetComponent<ObstacleView>();
                        if (i < 3)
                        {
                            layerId = 1;
                            obstacleView.SpriteRenderer.sortingOrder = -1;
                        }
                        else if(i < 6)
                        {
                            layerId = 2;
                            obstacleView.SpriteRenderer.sortingOrder = 0;
                        }
                        else
                        {
                            layerId = 3;
                            obstacleView.SpriteRenderer.sortingOrder = 1;
                        }
                        lineid = i % 3 + 1;
                        obstacleView.SetObstacleData(layerId, spawningPreset[i], obstaclesLine.Obstacles,lineid);
                        obstacleView.Scene2DActor.TriggerEnter += OnObstacleEnter;
                        obstaclesLine.Obstacles.Add(obstacleView);
                    }
                }
                _obstacleLines.Add(_currentSpawnLineIndex, obstaclesLine);
                ChangeCurrentLayerForObstacles(_playerView.CurrentLayer);
                //Debug.Log($"Obstacles spawned on {_currentSpawnLineIndex}");
                if (_currentSpawnLineIndex >= 5)
                {
                    RemoveOldObstacles();
                }
            }
        }

        private void OnObstacleEnter(Scene2DActor scene2DActor, Collider2D collider)
        {
            if (collider.gameObject == _playerView.gameObject)
            {
                ObstacleView obstacleView = scene2DActor.gameObject.GetComponent<ObstacleView>();
                _playerEventBus.OnInteractWithObstacle?.Invoke(obstacleView);
            }
        }

        private List<PrefabID> CreateSpawnPreset(bool isOxygenSpawning, List<int> oxygenSpawnIndexes,
            List<PrefabID> spawnableObstacles)
        {
            List<PrefabID> spawningPreset = new List<PrefabID>();
            int obstacleSpawnIndex = 0;
            obstacleSpawnIndex = _random.Next(0, 9);
            while (oxygenSpawnIndexes.Contains(obstacleSpawnIndex))
            {
                obstacleSpawnIndex = _random.Next(0, 9);
            }
            for (int i = 0; i < 9; i++)
            {
                if (isOxygenSpawning)
                {
                    if (oxygenSpawnIndexes.Contains(i))
                    {
                        spawningPreset.Add(PrefabID.OxygenBubble);
                        continue;
                    }
                }
                PrefabID obstacleId = PrefabID.None;
                if (obstacleSpawnIndex == i)
                {
                    int obstacleIndex = _random.Next(0, spawnableObstacles.Count);
                    obstacleId = spawnableObstacles[obstacleIndex];
                }
                spawningPreset.Add(obstacleId);
            }
            return spawningPreset;
        }
        
        private void RemoveOldObstacles()
        {
            int removingIndex = _currentSpawnLineIndex - 5;
            List<ObstacleView> obstacleViews = _obstacleLines[removingIndex].Obstacles;
            foreach (ObstacleView obstacleView in obstacleViews)
            {
                if (_movingObstacles.Contains(obstacleView))
                {
                    _movingObstacles.Remove(obstacleView);
                }
            }
            _obstacleLines[removingIndex].RemoveAll();
            _obstacleLines.Remove(removingIndex);
            //Debug.Log($"Obstacles removed on {_currentSpawnLineIndex - 5}");
        }
    }
}