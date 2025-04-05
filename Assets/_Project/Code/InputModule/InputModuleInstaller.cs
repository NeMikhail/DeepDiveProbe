using Zenject;
using Input;

public class InputModuleInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<InputActions>().AsSingle();
    }
}