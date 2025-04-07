using MAEngine;

namespace Upgrade
{
    public class UpgradeModule : BasicModule
    {
        public override void Initialise()
        {
            base.Initialise();
            InitializeUpgradeActions();
        }

        private void InitializeUpgradeActions()
        {
            UpgradeActions upgradeActions =
                _di.Resolve<UpgradeActions>();
            _actions.Add(upgradeActions);
        }
    }
}