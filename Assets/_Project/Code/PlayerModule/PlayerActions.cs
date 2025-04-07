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
        private UpgradesEventBus _upgradesEventBus;
        private bool _isInvincible = false;
        private int _shieldCharge;
        private int _maxShieldCharge;
        private int _oxygenRecovery;
        private int _currentOxygenRecovery;
        private float _oxygenRecoveryChance;
        private System.Random _random;
        private int _extraLifes;

        [Inject]
        public void Construct(PlayerView playerView, PlayerConfig playerConfig, PlayerEventBus playerEventBus,
            SaveLoadEventBus saveLoadEventBus, AudioEventBus audioEventBus, GameEventBus gameEventBus,
            UpgradesEventBus upgradesEventBus)
        {
            _playerView = playerView;
            _playerConfig = playerConfig;
            _playerEventBus = playerEventBus;
            _saveLoadEventBus = saveLoadEventBus;
            _audioEventBus = audioEventBus;
            _gameEventBus = gameEventBus;
            _upgradesEventBus = upgradesEventBus;
        }


        public void Initialisation()
        {
            _playerView.CurrentOxygen = _playerConfig.OxygenValue;
            _maxShieldCharge = _playerConfig.MaxShieldCharge;
            _playerEventBus.OnTriggerLineExit += RemoveOxygen;
            _playerEventBus.OnTriggerLineExit += DeActivateShield;
            _playerEventBus.OnAddOxygen += AddOxygen;
            _playerEventBus.OnInteractWithObstacle += InteractWithObstacle;
            _playerView.WinZoneActor.TriggerEnter += WinAction;
            _upgradesEventBus.OnUpgradeApplied += UpgradeApplied;
            _oxygenRecovery = 0;
            _currentOxygenRecovery = _oxygenRecovery;
            _oxygenRecoveryChance = 0;
            _random = new System.Random();
            _extraLifes = 0;
        }

        public void Cleanup()
        {
            _playerEventBus.OnTriggerLineExit -= RemoveOxygen;
            _playerEventBus.OnTriggerLineExit -= DeActivateShield;
            _playerEventBus.OnAddOxygen -= AddOxygen;
            _playerEventBus.OnInteractWithObstacle -= InteractWithObstacle;
            _playerView.WinZoneActor.TriggerEnter -= WinAction;
            _upgradesEventBus.OnUpgradeApplied -= UpgradeApplied;
        }
        
        private void UpgradeApplied(UpgradeID upgradeID)
        {
            if (upgradeID == UpgradeID.UpgradeShieldTime)
            {
                _maxShieldCharge++;
            }
            else if (upgradeID == UpgradeID.UpgradeOxygenRecovery)
            {
                _oxygenRecovery = 1;
            }
            else if (upgradeID == UpgradeID.UpgradeOxygenRecoveryChance)
            {
                _oxygenRecoveryChance += 0.25f;
            }
            else if (upgradeID == UpgradeID.UpgradeExtraLife)
            {
                _playerEventBus.OnExtraLifeAdded?.Invoke();
                _extraLifes++;
            }
            else if (upgradeID == UpgradeID.UpgradeNightVision)
            {
                _playerEventBus.OnActivateNightvision?.Invoke();
            }
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
                if (!_isInvincible)
                {
                    if (_oxygenRecoveryChance == 0)
                    {
                        RemoveOxygen(3);
                    }
                    else
                    {
                        int chance = _random.Next(0, 100);
                        if (chance / 100f > _oxygenRecoveryChance)
                        {
                            RemoveOxygen(3);
                        }
                    }
                    
                }
            }
        }

        private void RemoveOxygen()
        {
            if (_currentOxygenRecovery == 0)
            {
                _currentOxygenRecovery = _oxygenRecovery;
                RemoveOxygen(1);
            }
            else
            {
                _currentOxygenRecovery--;
            }
        }
        private void RemoveOxygen(int oxygenValue)
        {
            if (_playerView.CurrentOxygen > oxygenValue)
            {
                _playerView.CurrentOxygen -= oxygenValue;
                _playerEventBus.OnOxygenChanged?.Invoke(_playerView.CurrentOxygen);
                if (oxygenValue > 1)
                {
                    ActivateShield();
                }
            }
            else
            {
                if (_extraLifes == 0)
                {
                    _playerView.CurrentOxygen = 0;
                    _playerEventBus.OnOxygenChanged?.Invoke(_playerView.CurrentOxygen);
                    TryRewriteBestResult();
                    _audioEventBus.OnPlaySound?.Invoke(AudioResourceID.Sound_Death);
                    _audioEventBus.OnStopMusicAndEnvSound?.Invoke();
                    PlayerPrefs.SetInt(UpgradeRequirement.SecondTry.ToString(), 1);
                    _gameEventBus.OnGameOver?.Invoke();
                }
                else
                {
                    _extraLifes--;
                    _playerView.CurrentOxygen = _playerConfig.OxygenValue;
                    _playerEventBus.OnOxygenChanged?.Invoke(_playerView.CurrentOxygen);
                    _playerEventBus.OnExtraLifeRemoved?.Invoke();
                }

            }
        }

        private void ActivateShield()
        {
            _playerView.OxygenShieldObject.SetActive(true);
            _shieldCharge = _maxShieldCharge;
            _isInvincible = true;
        }
        
        private void DeActivateShield()
        {
            _shieldCharge--;
            if (_shieldCharge == 0)
            {
                _playerView.OxygenShieldObject.SetActive(false);
                _isInvincible = false;
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