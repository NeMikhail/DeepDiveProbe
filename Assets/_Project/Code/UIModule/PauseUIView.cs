using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PauseUIView : MonoBehaviour
    {
        [SerializeField] private ButtonView _continueButton;
        [SerializeField] private ButtonView _exitButton;
        [SerializeField] private ButtonView _settingsButton;
        [SerializeField] private RectTransform _pauseRectTransform;
        [SerializeField] private RectTransform _settingsRectTransform;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private ButtonView _settingsApplyButton;
        [SerializeField] private ButtonView _settingsBackButton;
        
        public ButtonView ContinueButton => _continueButton;
        public ButtonView ExitButton => _exitButton;
        public ButtonView SettingsButton => _settingsButton;
        public RectTransform PauseRectTransform => _pauseRectTransform;
        public RectTransform SettingsRectTransform => _settingsRectTransform;
        public Slider MusicSlider => _musicSlider;
        public Slider SFXSlider => _sfxSlider;
        public ButtonView SettingsApplyButton => _settingsApplyButton;
        public ButtonView SettingsBackButton => _settingsBackButton;

    }
}