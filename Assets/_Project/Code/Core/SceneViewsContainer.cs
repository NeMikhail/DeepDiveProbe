using MAEngine;
using MAEngine.Extention;
using Player;
using SO;
using System;
using Zenject;

namespace GameCoreModule
{
    public class SceneViewsContainer
    {
        private PrefabsContainer _prefabsContainer;
        private SceneViewsLinks _sceneViewsLinks;
        private SerializableDictionary<string, IView> _viewsDict;

        [Inject]
        public void Construct(PrefabsContainer prefabsContainer,
            SceneViewsLinks sceneViewsLinks)
        {
            _prefabsContainer = prefabsContainer;
            _sceneViewsLinks = sceneViewsLinks;
            Initialize();
        }

        private void Initialize()
        {
            foreach (BasicView view in _sceneViewsLinks.Views)
            {
                AddView(view);
            }
        }

        public SceneViewsContainer()
        {
            _viewsDict = new SerializableDictionary<string, IView>();
        }

        public void AddView(IView view)
        {
            string id = GenerateID(view);
            view.ViewID = id;
            _viewsDict.Add(id, view);
        }

        private string GenerateID(IView view)
        {
            string id = $"{view.Object.name}";
            return id;
        }

        public IView GetView(string id)
        {
            return _viewsDict.GetValue(id);
        }

        public PlayerView GetPlayerView()
        {
            PlayerView playerView = null;
            string viewID = _prefabsContainer.GetPrefab(PrefabID.PlayerPrefab).name;
            playerView = (PlayerView)_viewsDict.GetValue(viewID);
            return playerView;
        }
    }
}
