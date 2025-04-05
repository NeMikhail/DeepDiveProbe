using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class GUIView : MonoBehaviour
    {
        [SerializeField] private ButtonView _pauseButton;
        
        public ButtonView PauseButton => _pauseButton;

        public void InitializeView()
        {
            
        }
    }
}
