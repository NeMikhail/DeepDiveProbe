using Zenject;
using Player;
using UnityEngine;

public class PlayerModuleInstaller : MonoInstaller
{
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private ObstaclesSpawnConfig _obstaclesSpawnConfig;
    [SerializeField] private PlayerView _playerView;
    [SerializeField] private MapView _mapView;
    
    public override void InstallBindings()
    {
        Container.Bind<PlayerConfig>().FromInstance(_playerConfig).AsSingle();
        Container.Bind<ObstaclesSpawnConfig>().FromInstance(_obstaclesSpawnConfig).AsSingle();
        Container.Bind<PlayerView>().FromInstance(_playerView).AsSingle();
        Container.Bind<MapView>().FromInstance(_mapView).AsSingle();
        Container.Bind<PlayerMovementActions>().AsSingle();
        Container.Bind<MapGenerationActions>().AsSingle();
        Container.Bind<ObstaclesActions>().AsSingle();
        Container.Bind<PlayerActions>().AsSingle();
    }
}