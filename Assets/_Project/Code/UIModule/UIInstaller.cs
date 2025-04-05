using UnityEngine;
using Zenject;

namespace UI
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private GUIView _view;
        [SerializeField] private PauseUIView _pauseUIView;
        
        public override void InstallBindings()
        {
            Container.Bind<GUIView>().FromInstance(_view).AsSingle();
            Container.Bind<PauseUIView>().FromInstance(_pauseUIView).AsSingle();
            
            Container.Bind<UIActions>().AsSingle();
            Container.Bind<GUIActions>().AsSingle();
            Container.Bind<PauseActions>().AsSingle();
        }
    }
}