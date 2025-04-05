using GameCoreModule;
using MAEngine;
using UnityEngine;
using Zenject;

namespace UI
{
    public class PauseActions : IAction, IInitialisation, ICleanUp
    {
        private PauseUIView _pauseUIView;
        private StateEventsBus _stateEventsBus;
        private AudioEventBus _audioEventBus;
        private InputEventBus _inputEventBus;
        
        private float _currentMusicVolume;
        private float _currentSoundVolume;
        private bool _musicVolumeChanged;
        private bool _sfxVolumeChanged;

        [Inject]
        public void Construct(PauseUIView pauseUIView, StateEventsBus stateEventsBus, AudioEventBus audioEventBus,
            InputEventBus inputEventBus)
        {
            _pauseUIView = pauseUIView;
            _stateEventsBus = stateEventsBus;
            _audioEventBus = audioEventBus;
            _inputEventBus = inputEventBus;
        }
        
        public void Initialisation()
        {
            _inputEventBus.OnAttackButtonCanceled += OnPointerUp;
        }

        public void Cleanup()
        {
            _inputEventBus.OnAttackButtonCanceled -= OnPointerUp;
        }
        
        
        public void OnPointerUp()
        {
            if (_musicVolumeChanged)
            {
                CheckMusicVolume();
                _musicVolumeChanged = false;
            }
            if (_sfxVolumeChanged)
            {
                CheckSFXVolume();
                _sfxVolumeChanged = false;
            }
        }
        
        private void ContinueGame()
        {
            _stateEventsBus.OnPlayStateActivate?.Invoke();
        }
        
        private void ExitGame()
        {
            Application.Quit();
        }
        
        private void OpenSettings()
        {
            _pauseUIView.SettingsRectTransform.gameObject.SetActive(true);
            _pauseUIView.PauseRectTransform.gameObject.SetActive(false);
            SetupSettingsSliders();
        }

        private void SetupSettingsSliders()
        {
            AudioCallback callback = new AudioCallback();
            _audioEventBus.OnGetAudioSettings?.Invoke(callback);
            _pauseUIView.MusicSlider.value = callback.MusicVolume;
            _pauseUIView.SFXSlider.value = callback.SoundVolume;
            _currentMusicVolume = callback.MusicVolume;
            _currentSoundVolume = callback.SoundVolume;
            
        }

        private void CloseSettings()
        {
            _pauseUIView.SettingsRectTransform.gameObject.SetActive(false);
            _pauseUIView.PauseRectTransform.gameObject.SetActive(true);
            _audioEventBus.OnSetMusicVolume?.Invoke(_currentMusicVolume);
            _audioEventBus.OnSetSoundVolume?.Invoke(_currentSoundVolume);
        }
        
        private void ApplySettings()
        {
            _currentMusicVolume = _pauseUIView.MusicSlider.value;
            _currentSoundVolume = _pauseUIView.SFXSlider.value;
            CloseSettings();
        }
        
        private void SFXVolumeValueChanged(float volume)
        {
            _audioEventBus.OnSetSoundVolume?.Invoke(volume);
            _sfxVolumeChanged = true;
        }

        private void MusicVolumeValueChanged(float volume)
        {
            _audioEventBus.OnSetMusicVolume?.Invoke(volume);
            _musicVolumeChanged = true;
        }
        
        private void CheckSFXVolume()
        {
            _audioEventBus.OnPlaySound?.Invoke(AudioResourceID.Sound_Test);
        }

        private void CheckMusicVolume()
        {
            _audioEventBus.OnPlayMusic?.Invoke(AudioResourceID.Sound_Test);
        }
    }
}