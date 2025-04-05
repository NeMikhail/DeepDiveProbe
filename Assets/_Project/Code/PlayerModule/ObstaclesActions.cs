using System;
using System.Collections.Generic;
using GameCoreModule;
using MAEngine;
using UnityEngine;
using Zenject;

namespace Player
{
    public class ObstaclesActions : IAction, IInitialisation, ICleanUp
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
            _layerActiveObject = LayerMask.NameToLayer("ActiveObstacle");
            _layerInactiveObject = LayerMask.NameToLayer("InactiveObstacle");
            
            BindSpawnLinesTriggers(_mapView.Layer1View);
            _currentSpawnLineIndex = 0;
            SpawnObstaclesOnLine();
            _currentSpawnLineIndex = 1;
            SpawnObstaclesOnLine();
            _playerEventBus.OnTriggerSpawnLine += SpawnObstaclesOnLine;
            _playerEventBus.OnChangeLayer += ChangeCurrentLayerForObstacles;
        }

        private void ChangeCurrentLayerForObstacles(int currentIndex)
        {
            if (currentIndex == 1)
            {
                _mapView.Layer1BlackObject.SetActive(false);
                _mapView.Layer2BlackObject.SetActive(false);
            }
            else if (currentIndex == 2)
            {
                _mapView.Layer1BlackObject.SetActive(true);
                _mapView.Layer2BlackObject.SetActive(false);
            }
            else if (currentIndex == 3)
            {
                _mapView.Layer1BlackObject.SetActive(true);
                _mapView.Layer2BlackObject.SetActive(true);
            }
            foreach (KeyValuePair<int, ObstaclesLine> obstaclesLine in _obstacleLines)
            {
                List<ObstacleView> obstacleViews = obstaclesLine.Value.Obstacles;
                foreach (ObstacleView obstacleView in obstacleViews)
                {
                    if (obstacleView.LayerId == currentIndex)
                    {
                        obstacleView.ObstacleObject.layer = _layerActiveObject;
                        obstacleView.SpriteRenderer.color =
                            ChangeTransperency(obstacleView.SpriteRenderer.color, 1f);
                    }
                    else if (obstacleView.LayerId > currentIndex + 1)
                    {
                        obstacleView.ObstacleObject.layer = _layerInactiveObject;
                        obstacleView.SpriteRenderer.color =
                            ChangeTransperency(obstacleView.SpriteRenderer.color, 0.1f);
                    }
                    else if(obstacleView.LayerId > currentIndex)
                    {
                        obstacleView.ObstacleObject.layer = _layerInactiveObject;
                        obstacleView.SpriteRenderer.color =
                            ChangeTransperency(obstacleView.SpriteRenderer.color, 0.2f);
                    }
                    else
                    {
                        obstacleView.ObstacleObject.layer = _layerInactiveObject;
                        obstacleView.SpriteRenderer.color =
                            ChangeTransperency(obstacleView.SpriteRenderer.color, 1f);
                    }
                }
            }
        }

        private Color ChangeTransperency(Color color, float transperency)
        {
            Color newColor = new Color(color.r, color.g, color.b, transperency);
            return newColor;
        }


        public void Cleanup()
        {
            UnBindSpawnLinesTriggers(_mapView.Layer1View);
            _playerEventBus.OnTriggerSpawnLine -= SpawnObstaclesOnLine;
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
                
                oxygenCount = _obstaclesSpawnConfig.Stage1OxygenSpawnCount;
                spawnableObstacles = _obstaclesSpawnConfig.Stage1Obstacles;
                
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
                        obstacleView.SetObstacleData(layerId, spawningPreset[i], obstaclesLine.Obstacles);
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
            bool isObstacleSpawned = false;
            for (int i = 0; i < 9; i++)
            {
                if (i == 0)
                {
                    isObstacleSpawned = false;
                }
                else if(i == 3)
                {
                    isObstacleSpawned = false;
                }
                else if (i == 6)
                {
                    isObstacleSpawned = false;
                }
                    
                if (isOxygenSpawning)
                {
                    if (oxygenSpawnIndexes.Contains(i))
                    {
                        spawningPreset.Add(PrefabID.OxygenBubble);
                        continue;
                    }
                }
                PrefabID obstacleId = PrefabID.None;
                if (!isObstacleSpawned)
                {
                    if (_random.Next(0, 2) == 1)
                    {
                        int obstacleIndex = _random.Next(0, spawnableObstacles.Count);
                        obstacleId = spawnableObstacles[obstacleIndex];
                        isObstacleSpawned = true;
                    }
                    else if(i == 2 || i == 5 || i == 8)
                    {
                        int obstacleIndex = _random.Next(0, spawnableObstacles.Count);
                        obstacleId = spawnableObstacles[obstacleIndex];
                        isObstacleSpawned = true;
                    }
                }
                spawningPreset.Add(obstacleId);
            }
            return spawningPreset;
        }


        private void RemoveOldObstacles()
        {
            //Debug.Log($"Obstacles removed on {_currentSpawnLineIndex - 5}");
        }
    }
}