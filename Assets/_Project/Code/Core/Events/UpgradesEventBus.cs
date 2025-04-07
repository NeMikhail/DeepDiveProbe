using System;
using System.Collections.Generic;

namespace GameCoreModule
{
    public class UpgradesEventBus
    {
        private Action<List<UpgradeID>> _onLevelUp;
        private Action<UpgradeID> _onUpgradeApplied;
        private Action<int> _onExpChanged;
        
        public Action<List<UpgradeID>> OnLevelUp
        { get => _onLevelUp; set => _onLevelUp = value; }
        public Action<UpgradeID> OnUpgradeApplied
        { get => _onUpgradeApplied; set => _onUpgradeApplied = value; }
        public Action<int> OnExpChanged
        { get => _onExpChanged; set => _onExpChanged = value; }
    }
}