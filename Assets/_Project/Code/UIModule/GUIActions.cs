using System.Collections.Generic;
using GameCoreModule;
using MAEngine;
using MAEngine.Extention;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace UI
{
    public class GUIActions : IAction, IPreInitialisation, IInitialisation, ICleanUp, ILateExecute
    {
        private const float DIALOGUE_DELAY = 8f;
        
        private GUIView _guiView;
        private StateEventsBus _stateEventsBus;
        private PlayerEventBus _playerEventBus;
        private UIEventBus _uiEventBus;
        private SaveLoadEventBus _saveLoadEventBus;
        private InputEventBus _inputEventBus;
        private GameEventBus _gameEventBus;

        private DialogueView _activeDialogue;
        private Timer _dialogueTimer;

        [Inject]
        public void Construct(GUIView guiView, StateEventsBus stateEventsBus, PlayerEventBus playerEventBus,
            UIEventBus uiEventBus, SaveLoadEventBus saveLoadEventBus, InputEventBus inputEventBus, 
            GameEventBus gameEventBus)
        {
            _guiView = guiView;
            _stateEventsBus = stateEventsBus;
            _playerEventBus = playerEventBus;
            _uiEventBus = uiEventBus;
            _saveLoadEventBus = saveLoadEventBus;
            _inputEventBus = inputEventBus;
            _gameEventBus = gameEventBus;
        }
        
        public void PreInitialisation()
        {
            _guiView.InitializeView();
        }

        public void Initialisation()
        {
            _playerEventBus.OnChangeLayer += ChangeLayerIndication;
            _playerEventBus.OnOxygenChanged += UpdateOxygenSlider;
            _playerEventBus.OnDepthChanged += UpdateDepthSlider;
            _playerEventBus.OnStageChanged += StartStageDialogue;
            _guiView.PauseButton.Button.onClick.AddListener(SetPauseState);
            _inputEventBus.OnJumpButtonPerformed += SkipPhrase;
            _gameEventBus.OnWin += SetWinScreen;
            _gameEventBus.OnGameOver += SetLoseScreen;
            _guiView.WinRetryButton.Button.onClick.AddListener(ReloadScene);
            _guiView.LoseRetryButton.Button.onClick.AddListener(ReloadScene);
            _guiView.WinProgressClearButton.Button.onClick.AddListener(ClearProgress);
            LoadBestDepth();
        }

        public void Cleanup()
        {
            _playerEventBus.OnChangeLayer -= ChangeLayerIndication;
            _playerEventBus.OnOxygenChanged -= UpdateOxygenSlider;
            _playerEventBus.OnDepthChanged -= UpdateDepthSlider;
            _playerEventBus.OnStageChanged -= StartStageDialogue;
            _guiView.PauseButton.Button.onClick.RemoveListener(SetPauseState);
            _inputEventBus.OnJumpButtonPerformed -= SkipPhrase;
            _gameEventBus.OnWin -= SetWinScreen;
            _gameEventBus.OnGameOver -= SetLoseScreen;
            _guiView.WinRetryButton.Button.onClick.RemoveListener(ReloadScene);
            _guiView.LoseRetryButton.Button.onClick.RemoveListener(ReloadScene);
            _guiView.WinProgressClearButton.Button.onClick.RemoveListener(ClearProgress);
        }
        
        public void LateExecute(float fixedDeltaTime)
        {
            if (_activeDialogue != null)
            {
                if (_dialogueTimer.Wait())
                {
                    _activeDialogue.NextPhrase();
                    CheckDialogueEnd();
                }
            }
        }

        private void SkipPhrase()
        {
            if (_activeDialogue != null)
            {
                _activeDialogue.NextPhrase();
                _dialogueTimer = new Timer(DIALOGUE_DELAY);
                CheckDialogueEnd();
            }
        }
        
        private void CheckDialogueEnd()
        {
            if (_activeDialogue.IsClosed)
            {
                _activeDialogue = null;
                _dialogueTimer = null;
                _stateEventsBus?.OnPlayStateActivate?.Invoke();
            }
        }
        
        private void StartStageDialogue(StageID stageID)
        {
            _stateEventsBus?.OnPauseStateActivate?.Invoke();
            if (stageID == StageID.Stage1)
            {
                _activeDialogue = _guiView.DialoguesView.Dialogue1;
            }
            else if (stageID == StageID.Stage2)
            {
                _activeDialogue = _guiView.DialoguesView.Dialogue2;
            }
            else if (stageID == StageID.Stage3)
            {
                _activeDialogue = _guiView.DialoguesView.Dialogue3;
            }
            _activeDialogue.InitializeDialogue();
            if (_guiView.BestDepthSlider.value != 0)
            {
                _activeDialogue.SetChangableText((int)_guiView.BestDepthSlider.value);
            }

            _dialogueTimer = new Timer(DIALOGUE_DELAY);
        }
        
        private void LoadBestDepth()
        {
            SavableInt loadedDepth = new SavableInt();
            _saveLoadEventBus.OnGetInt?.Invoke(SaveDataKey.BestDepth, loadedDepth);
            if (loadedDepth.IsLoaded)
            {
                _guiView.BestDepthSlider.gameObject.SetActive(true);
                _guiView.BestDepthSlider.value = loadedDepth.Value;
                _guiView.BestDepthText.text = $"{loadedDepth.Value}m.";
            }
            else
            {
                _guiView.BestDepthSlider.gameObject.SetActive(false);
            }
        }
        
        private void ChangeLayerIndication(int layer)
        {
            _guiView.UpdateLayerImages(layer);
        }
        
        private void UpdateOxygenSlider(int oxygenValue)
        {
            _guiView.OxygenSlider.value = oxygenValue;
        }
        
        private void UpdateDepthSlider(int depth)
        {
            _guiView.DepthSlider.value = depth;
            _guiView.DepthText.text = $"{depth}m.";
        }
        
        private void SetPauseState()
        {
            StateCallback callback = new StateCallback();
            _stateEventsBus.OnGetCurrentState?.Invoke(callback);
            if (callback.State != GameState.PauseState)
            {
                ClearGUI();
                _stateEventsBus.OnPauseStateActivate?.Invoke();
                _uiEventBus.OnOpenPauseMenu?.Invoke();
            }
        }
        
        private void ClearGUI()
        {
            
        }
        
        private void ClearProgress()
        {
            _saveLoadEventBus?.OnClearProgress.Invoke();
        }

        private void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void SetLoseScreen()
        {
            _guiView.LoseScreenObject.SetActive(true);
            _inputEventBus.OnDisableInput?.Invoke();
        }

        private void SetWinScreen()
        {
            _guiView.WinScreenObject.SetActive(true);
            _inputEventBus.OnDisableInput?.Invoke();
        }
    }
}