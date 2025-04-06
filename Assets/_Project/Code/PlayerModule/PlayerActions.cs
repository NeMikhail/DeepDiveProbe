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
        private SaveLoadEventBus _saveLoadEventBus;

        [Inject]
        public void Construct(PlayerView playerView, PlayerConfig playerConfig, PlayerEventBus playerEventBus,
            SaveLoadEventBus saveLoadEventBus)
        {
            _playerView = playerView;
            _playerConfig = playerConfig;
            _playerEventBus = playerEventBus;
            _saveLoadEventBus = saveLoadEventBus;
        }


        public void Initialisation()
        {
            _playerView.CurrentOxygen = _playerConfig.OxygenValue;
            _playerEventBus.OnTriggerSpawnLine += RemoveOxygen;
            _playerEventBus.OnAddOxygen += AddOxygen;
            _playerEventBus.OnInteractWithObstacle += InteractWithObstacle;
            _playerView.WinZoneActor.TriggerEnter += WinAction;
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
                RemoveOxygen(3);
            }
        }

        public void Cleanup()
        {
            _playerEventBus.OnTriggerSpawnLine -= RemoveOxygen;
            _playerEventBus.OnAddOxygen -= AddOxygen;
            _playerView.WinZoneActor.TriggerEnter -= WinAction;
        }

        private void RemoveOxygen()
        {
            RemoveOxygen(1);
        }
        private void RemoveOxygen(int oxygenValue)
        {
            if (_playerView.CurrentOxygen > oxygenValue)
            {
                _playerView.CurrentOxygen -= oxygenValue;
                _playerEventBus.OnOxygenChanged?.Invoke(_playerView.CurrentOxygen);
            }
            else
            {
                _playerView.CurrentOxygen = 0;
                _playerEventBus.OnOxygenChanged?.Invoke(_playerView.CurrentOxygen);
                TryRewriteBestResult();
                _playerEventBus.OnGameOver?.Invoke();
                Debug.LogError("GameOver");
            }
        }

        private void TryRewriteBestResult()
        {
            int absDepth = Mathf.Abs(_playerView.CurrentDepth);
            SavableInt loadedDepth = new SavableInt();
            _saveLoadEventBus.OnGetInt?.Invoke(SaveDataKey.BestDepth, loadedDepth);
            int bestDepth = 0;
            if (loadedDepth.IsLoaded)
            {
                bestDepth = loadedDepth.Value;
            }
            if (absDepth > bestDepth)
            {
                _saveLoadEventBus.OnSaveInt?.Invoke(SaveDataKey.BestDepth, absDepth);
                _saveLoadEventBus.OnSaveData?.Invoke();
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
        
        private void WinAction(Scene2DActor actor, Collider2D collider)
        {
            if (_playerView.gameObject == collider.gameObject)
            {
                Debug.LogError("Win");
                _playerEventBus.OnWin?.Invoke();
            }
        }


    }
}