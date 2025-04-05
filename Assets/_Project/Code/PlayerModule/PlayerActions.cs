using GameCoreModule;
using MAEngine;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerActions : IAction, IInitialisation, ICleanUp
    {
        private PlayerView _playerView;
        private PlayerConfig _playerConfig;
        private PlayerEventBus _playerEventBus;

        [Inject]
        public void Construct(PlayerView playerView, PlayerConfig playerConfig, PlayerEventBus playerEventBus)
        {
            _playerView = playerView;
            _playerConfig = playerConfig;
            _playerEventBus = playerEventBus;
        }


        public void Initialisation()
        {
            _playerView.CurrentOxygen = _playerConfig.OxygenValue;
            _playerEventBus.OnTriggerSpawnLine += RemoveOxygen;
            _playerEventBus.OnAddOxygen += AddOxygen;
            _playerEventBus.OnInteractWithObstacle += InteractWithObstacle;
        }

        private void InteractWithObstacle(ObstacleView obstcle)
        {
            if (obstcle.PrefabID == PrefabID.OxygenBubble)
            {
                _playerEventBus.OnAddOxygen?.Invoke();
                obstcle.Obstacles.Remove(obstcle);
                GameObject.Destroy(obstcle.gameObject);
            }
            else
            {
                _playerEventBus.OnGameOver?.Invoke();
                Debug.LogError("GameOver");
            }
        }

        public void Cleanup()
        {
            _playerEventBus.OnTriggerSpawnLine -= RemoveOxygen;
            _playerEventBus.OnAddOxygen -= AddOxygen;
        }

        private void RemoveOxygen()
        {
            if (_playerView.CurrentOxygen > 0)
            {
                _playerView.CurrentOxygen--;
                _playerEventBus.OnOxygenChanged?.Invoke(_playerView.CurrentOxygen);
            }
            else
            {
                _playerEventBus.OnGameOver?.Invoke();
                Debug.LogError("GameOver");
            }
        }

        private void AddOxygen()
        {
            if (_playerView.CurrentOxygen < _playerConfig.OxygenValue)
            {
                _playerView.CurrentOxygen += _playerConfig.AddingOxygenValue;
                _playerEventBus.OnOxygenChanged?.Invoke(_playerView.CurrentOxygen);
            }
        }


    }
}