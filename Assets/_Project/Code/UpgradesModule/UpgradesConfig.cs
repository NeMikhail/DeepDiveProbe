using System.Collections.Generic;
using System.Linq;
using MAEngine.Extention;
using UnityEngine;

namespace Upgrade
{
    [CreateAssetMenu(fileName = "UpgradesListConfig", menuName = "SO/Config/UpgradesListConfig")]
    public class UpgradesListConfig : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<UpgradeID, UpgradeConfig> _upgradesConfigs;
        [SerializeField] private int _levelExp;
        
        public SerializableDictionary<UpgradeID, UpgradeConfig> UpgradesConfigsList => _upgradesConfigs;
        public int LevelExp => _levelExp;
        
        public UpgradeConfig GetUpgradeConfig(UpgradeID upgradeId, List<UpgradeID> activeUpgrades)
        {
            if (activeUpgrades.Contains(upgradeId))
            {
                int maxLevel = _upgradesConfigs[upgradeId].MaxUpgradeLevels;
                int currentLevel = 0;
                foreach (UpgradeID id in activeUpgrades)
                {
                    if (id == upgradeId)
                    {
                        currentLevel++;
                    }
                }
                if (currentLevel >= maxLevel)
                {
                    return null;
                }
            }
            return _upgradesConfigs[upgradeId];
        }

        public List<UpgradeID> GetAvaliableUpgrades(List<UpgradeID> activeUpgrades)
        {
            List<UpgradeID> avaliableUpgrades = new List<UpgradeID>();
            foreach (UpgradeID upgradeId in _upgradesConfigs.GetAllKeys())
            {
                for (int i = 0; i < _upgradesConfigs[upgradeId].MaxUpgradeLevels; i++)
                {
                    if (_upgradesConfigs[upgradeId].UpgradeRequirement != UpgradeRequirement.NONE)
                    {
                        int isRequrement = 
                            PlayerPrefs.GetInt(_upgradesConfigs[upgradeId].UpgradeRequirement.ToString());
                        if (isRequrement == 0)
                        {
                            break;
                        }
                    }
                    avaliableUpgrades.Add(upgradeId);
                }
            }
            if (avaliableUpgrades.Count <= activeUpgrades.Count)
            {
                return null;
            }
            foreach (UpgradeID id in activeUpgrades)
            {
                if (avaliableUpgrades.Contains(id))
                {
                    avaliableUpgrades.Remove(id);
                }
            }
            return avaliableUpgrades;
        }
    }
}