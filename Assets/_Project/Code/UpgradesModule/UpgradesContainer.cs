using System.Collections.Generic;

namespace Upgrade
{
    public class UpgradesContainer
    {
        private List<UpgradeID> _activeUpgrades;
        private int _currentExp;
        
        public List<UpgradeID> ActiveUpgrades => _activeUpgrades;
        public int CurrentExp
        {
            get => _currentExp;
            set => _currentExp = value;
        }

        public void AddUpgrade(UpgradeID upgradeId)
        {
            _activeUpgrades.Add(upgradeId);
        }
        
        public void RemoveUpgrade(UpgradeID upgradeId)
        {
            _activeUpgrades.Remove(upgradeId);
        }

        public bool CheckUpgrade(UpgradeID upgradeId)
        {
            return _activeUpgrades.Contains(upgradeId);
        }
        
        public UpgradesContainer()
        {
            _activeUpgrades = new List<UpgradeID>();
        }
    }
}