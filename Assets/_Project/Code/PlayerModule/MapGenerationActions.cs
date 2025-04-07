using System;
using System.Collections.Generic;
using GameCoreModule;
using MAEngine;
using UnityEngine;
using Zenject;

namespace Player
{
    public class MapGenerationActions : IAction, IInitialisation
    {
        private MapView _mapView;
        private GameEventBus _gameEventBus;
        private System.Random _random;

        private int _lastLeftWallId;
        private int _lastRightWallId;
        
        [Inject]
        public void Construct(MapView mapView, GameEventBus gameEventBus)
        {
            _mapView = mapView;
            _gameEventBus = gameEventBus;
        }
        
        public void Initialisation()
        {
            _random = new System.Random(); 
            SpawnWalls();
            InitializeSpawnZones(_mapView.Layer1View);
            InitializeSpawnZones(_mapView.Layer2View);
            InitializeSpawnZones(_mapView.Layer3View);
        }

        private void InitializeSpawnZones(LayerView layerView)
        {
            layerView.SpawnZones = new List<SpawnZonesView>();
            float ypos = 0;
            Vector2 position = Vector2.zero;
            for (int i = 0; i < 238; i++)
            {
                position = new Vector2(0, ypos);
                GameObjectSpawnCallback callback = new GameObjectSpawnCallback();
                _gameEventBus.OnSpawnObject(PrefabID.SpawnZones,
                    Vector2.zero, layerView.SpawnZonesTransform, callback);
                GameObject spawnZonesObject = callback.SpawnedObject;
                spawnZonesObject.transform.position = new Vector2(layerView.SpawnZonesTransform.position.x,
                    layerView.SpawnZonesTransform.position.y + position.y);
                SpawnZonesView view = callback.SpawnedObject.GetComponent<SpawnZonesView>();
                layerView.SpawnZones.Add(view);
                ypos -= 5f;
            }
        }

        private void SpawnWalls()
        {
            PrefabID leftWallId = PrefabID.None;
            PrefabID rightWallId = PrefabID.None;
            float ypos = 0;
            Vector2 position = Vector2.zero;
            for (int i = 0; i < 122; i++)
            {
                int wallIndex = _random.Next(0, 4);
                while (wallIndex == _lastLeftWallId)
                {
                    wallIndex = _random.Next(0, 4);
                }
                _lastLeftWallId = wallIndex;
                switch (wallIndex)
                {
                    case 0:
                        leftWallId = PrefabID.Wall1L;
                        break;
                    case 1:
                        leftWallId = PrefabID.Wall2L;
                        break;
                    case 2:
                        leftWallId = PrefabID.Wall3L;
                        break;
                    case 3:
                        leftWallId = PrefabID.Wall4L;
                        break;
                    default:
                        break;
                }

                wallIndex = _random.Next(0, 4);
                while (wallIndex == _lastRightWallId)
                {
                    wallIndex = _random.Next(0, 4);
                }
                _lastRightWallId = wallIndex;
                switch (wallIndex)
                {
                    case 0:
                        rightWallId = PrefabID.Wall1R;
                        break;
                    case 1:
                        rightWallId = PrefabID.Wall2R;
                        break;
                    case 2:
                        rightWallId = PrefabID.Wall3R;
                        break;
                    case 3:
                        rightWallId = PrefabID.Wall4R;
                        break;
                    default:
                        break;
                }
                position = new Vector2(0, ypos);
                Transform leftWallTransform = _mapView.LeftWallTransform;
                Transform rightWallTransform = _mapView.RightWallTransform;
                GameObjectSpawnCallback callback = new GameObjectSpawnCallback();
                _gameEventBus.OnSpawnObject(leftWallId, position, leftWallTransform, callback);
                callback.SpawnedObject.transform.position = new Vector2(-6.7f, position.y);
                _gameEventBus.OnSpawnObject(rightWallId, position, rightWallTransform, callback);
                callback.SpawnedObject.transform.position = new Vector2(6.7f, position.y);
                ypos -= 10;
            }
        }
    }
}