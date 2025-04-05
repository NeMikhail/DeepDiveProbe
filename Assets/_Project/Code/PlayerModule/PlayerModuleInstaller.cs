using Zenject;
using Player;
using UnityEngine;

public class PlayerModuleInstaller : MonoInstaller
{
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private PlayerView _playerView;
    [SerializeField] private MapView _mapView;
    
    public override void InstallBindings()
    {
        Container.Bind<PlayerConfig>().FromInstance(_playerConfig).AsSingle();
        Container.Bind<PlayerView>().FromInstance(_playerView).AsSingle();
        Container.Bind<MapView>().FromInstance(_mapView).AsSingle();
        Container.Bind<PlayerMovementActions>().AsSingle();
        Container.Bind<MapGenerationActions>().AsSingle();
    }
}