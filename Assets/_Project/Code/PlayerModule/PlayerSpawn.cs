using GameCoreModule;
using MAEngine;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerSpawn : IAction, IInitialisation
    {
        private const string PLAYER_SPAWN_POINT_ID = "PlayerSpawnPoint";
        private GameEventBus _gameEventBus;
        private Transform _playerSpawnPoint;

        [Inject]
        public void Construct(GameEventBus gameEventBus, SceneViewsContainer sceneViewsContainer)
        {
            _gameEventBus = gameEventBus;
            _playerSpawnPoint = sceneViewsContainer.GetView(PLAYER_SPAWN_POINT_ID).Object.transform;
        }

        public void Initialisation()
        {
            GameObjectSpawnCallback callback = new GameObjectSpawnCallback();
            _gameEventBus.OnSpawnObject?.Invoke(PrefabID.PlayerPrefab,
                _playerSpawnPoint.position, _playerSpawnPoint, callback);
        }
    }
}
