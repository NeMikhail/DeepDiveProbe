using System.Collections.Generic;
using GameCoreModule;
using MAEngine;
using UnityEngine;
using Zenject;

namespace Upgrade
{
    public class UpgradeActions : IAction, IInitialisation, ICleanUp
    {
        private UpgradesContainer _upgradesContainer;
        private UpgradesListConfig _upgradesListConfig;
        private UpgradesEventBus _upgradesEventBus;
        private PlayerEventBus _playerEventBus;
        private StateEventsBus _stateEventsBus;

        [Inject]
        public void Construct(UpgradesContainer upgradeContainer, UpgradesListConfig upgradeListConfig,
            UpgradesEventBus upgradeEventBus, PlayerEventBus playerEventBus, StateEventsBus stateEventsBus)
        {
            _upgradesContainer = upgradeContainer;
            _upgradesListConfig = upgradeListConfig;
            _upgradesEventBus = upgradeEventBus;
            _playerEventBus = playerEventBus;
            _stateEventsBus = stateEventsBus;
        }
        
        public void Initialisation()
        {
            _playerEventBus.OnTriggerSpawnLine += AddExp;
            _upgradesEventBus.OnUpgradeApplied += ApplyUpgrade;
            _upgradesContainer.CurrentExp = 0;
        }
        
        public void Cleanup()
        {
            _playerEventBus.OnTriggerSpawnLine -= AddExp;
            _upgradesEventBus.OnUpgradeApplied -= ApplyUpgrade;
        }
        
        private void AddExp()
        {
            if(_upgradesContainer.CheckUpgrade(UpgradeID.UpgradeExtraEXP))
            {
                _upgradesContainer.CurrentExp += 2;
            }
            else
            {
                _upgradesContainer.CurrentExp++;
            }
            if (_upgradesContainer.CurrentExp >= _upgradesListConfig.LevelExp)
            {
                _upgradesContainer.CurrentExp = _upgradesListConfig.LevelExp - _upgradesContainer.CurrentExp;
                List<UpgradeID> availableUpgrades =
                    _upgradesListConfig.GetAvaliableUpgrades(_upgradesContainer.ActiveUpgrades);
                if (availableUpgrades != null)
                {
                    _upgradesEventBus.OnLevelUp?.Invoke(availableUpgrades);
                    _stateEventsBus.OnPauseStateActivate?.Invoke();
                }
            }
            _upgradesEventBus.OnExpChanged?.Invoke(_upgradesContainer.CurrentExp);
        }
        
        private void ApplyUpgrade(UpgradeID upgradeID)
        {
            if (upgradeID != UpgradeID.NONE)
            {
                _upgradesContainer.AddUpgrade(upgradeID);
            }
            _stateEventsBus.OnPlayStateActivate?.Invoke();
        }
    }
}