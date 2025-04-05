using Zenject;
using GameCoreModule;
using UnityEngine;
using SO;

public class CoreGameInstaller : MonoInstaller
{
    [SerializeField] private PrefabsContainer _prefabsContainer;
    [SerializeField] private SceneViewsLinks _sceneViewsLinks;
    [SerializeField] private CanvasList _canvasList;

    public override void InstallBindings()
    {
        InstallEventBuses();
        InstallContainers();
        InstallActions();
        InstallOperators();
    }

    private void InstallEventBuses()
    {
        Container.Bind<GameEventBus>().AsSingle();
        Container.Bind<StateEventsBus>().AsSingle();
        Container.Bind<PlayerEventBus>().AsSingle();
        Container.Bind<UIEventBus>().AsSingle();
        Container.Bind<AudioEventBus>().AsSingle();
        Container.Bind<InputEventBus>().AsSingle();
        Container.Bind<SaveLoadEventBus>().AsSingle();
    }

    private void InstallContainers()
    {
        Container.Bind<SceneViewsContainer>().AsSingle();
        Container.Bind<PrefabsContainer>().FromInstance(_prefabsContainer).AsSingle();
        Container.Bind<SceneViewsLinks>().FromInstance(_sceneViewsLinks).AsSingle();
        Container.Bind<CanvasList>().FromInstance(_canvasList).AsSingle();
    }

    private void InstallActions()
    {
        Container.Bind<GameStateActions>().AsSingle();
        Container.Bind<GameControlActions>().AsSingle();
    }

    private void InstallOperators()
    {
        Container.Bind<PoolsContainer>().AsSingle();
        Container.Bind<PoolsOperator>().AsSingle();
        Container.Bind<SpawnOperator>().AsSingle();
    }
}