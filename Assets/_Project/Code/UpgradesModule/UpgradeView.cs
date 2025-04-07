using System;
using GameCoreModule;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Upgrade
{
    public class UpgradeView : MonoBehaviour
    {
        [SerializeField] private ButtonView _upgradeButton;
        [SerializeField] private TMP_Text _upgradeText;
        [SerializeField] private Image _upgradeImage;
        private UpgradesEventBus _upgradesEventBus;
        private UpgradeID _upgradeID;
        
        public ButtonView UpgradeButton => _upgradeButton;
        public TMP_Text UpgradeText => _upgradeText;
        public Image UpgradeImage => _upgradeImage;

        public void InitializeView(UpgradeConfig config, UpgradesEventBus eventBus)
        {
            _upgradesEventBus = eventBus;
            _upgradeID = config.UpgradeId;
            _upgradeButton.Button.onClick.AddListener(UpgradeButtonClick);
            _upgradeText.text = config.UpgradeText;
            _upgradeImage.sprite = config.UpgradeImage;
        }

        private void UpgradeButtonClick()
        {
            _upgradesEventBus.OnUpgradeApplied?.Invoke(_upgradeID);
        }

        private void OnDestroy()
        {
            _upgradeButton.Button.onClick.RemoveListener(UpgradeButtonClick);
        }
    }
}