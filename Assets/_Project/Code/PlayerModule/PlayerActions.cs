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
        private AudioEventBus _audioEventBus;
        private GameEventBus _gameEventBus;

        [Inject]
        public void Construct(PlayerView playerView, PlayerConfig playerConfig, PlayerEventBus playerEventBus,
            SaveLoadEventBus saveLoadEventBus, AudioEventBus audioEventBus, GameEventBus gameEventBus)
        {
            _playerView = playerView;
            _playerConfig = playerConfig;
            _playerEventBus = playerEventBus;
            _saveLoadEventBus = saveLoadEventBus;
            _audioEventBus = audioEventBus;
            _gameEventBus = gameEventBus;
        }


        public void Initialisation()
        {
            _playerView.CurrentOxygen = _playerConfig.OxygenValue;
            _playerEventBus.OnTriggerLineExit += RemoveOxygen;
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
            _playerEventBus.OnTriggerLineExit -= RemoveOxygen;
            _playerEventBus.OnAddOxygen -= AddOxygen;
            _playerEventBus.OnInteractWithObstacle -= InteractWithObstacle;
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
                _audioEventBus.OnPlaySound?.Invoke(AudioResourceID.Sound_Death);
                _audioEventBus.OnStopMusicAndEnvSound?.Invoke();
                _gameEventBus.OnGameOver?.Invoke();
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
            _audioEventBus.OnPlaySound?.Invoke(AudioResourceID.Sound_OxygenPickup);
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
                _audioEventBus.OnPlaySound?.Invoke(AudioResourceID.Sound_Win);
                _gameEventBus.OnWin?.Invoke();
            }
        }


    }
}