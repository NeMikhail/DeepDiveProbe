using UnityEngine;

namespace GameCoreModule
{
    public class CanvasList : MonoBehaviour
    {
        [SerializeField] private GameObject _pauseCanvas;
        [SerializeField] private GameObject _guiCanvas;
        
        public GameObject PauseCanvas => _pauseCanvas;
        public GameObject GUICanvas => _guiCanvas;

    }
}