using UnityEngine;
using Zenject;

namespace Upgrade
{
    public class UpgradeInstaller : MonoInstaller
    {
        [SerializeField] private UpgradesListConfig _upgradesListConfig;
        
        public override void InstallBindings()
        {
            Container.Bind<UpgradesListConfig>().FromInstance(_upgradesListConfig).AsSingle();
            Container.Bind<UpgradesContainer>().AsSingle();
            Container.Bind<UpgradeActions>().AsSingle();
        }
    }
}