using UI;
using UnityEngine;

namespace Upgrade
{
    [CreateAssetMenu(fileName = "UpgradeConfig", menuName = "SO/Config/UpgradeConfig")]
    public class UpgradeConfig : ScriptableObject
    {
        [SerializeField] private UpgradeID _upgradeId;
        [SerializeField] private Sprite _upgradeImage;
        [SerializeField] private string _upgradeText;
        [SerializeField] private int _maxUpgradeLevels;
        [SerializeField] private UpgradeRequirement _upgradeRequirement;
        
        public UpgradeID UpgradeId => _upgradeId;
        public Sprite UpgradeImage => _upgradeImage;
        public string UpgradeText => _upgradeText;
        public int MaxUpgradeLevels => _maxUpgradeLevels;
        public UpgradeRequirement UpgradeRequirement => _upgradeRequirement;
    }
}