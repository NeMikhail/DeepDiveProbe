using Zenject;
using Player;
using UnityEngine;

public class PlayerModuleInstaller : MonoInstaller
{
    [SerializeField] private PlayerConfig _playerConfig;
    public override void InstallBindings()
    {
        Container.Bind<PlayerConfig>().FromInstance(_playerConfig).AsSingle();
        Container.Bind<PlayerSpawn>().AsSingle();
        Container.Bind<PlayerMovementActions>().AsSingle();
        Container.Bind<CameraMovementActions>().AsSingle();
    }
}