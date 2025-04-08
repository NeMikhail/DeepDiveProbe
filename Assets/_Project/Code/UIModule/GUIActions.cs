using System.Collections.Generic;
using System.Linq;
using GameCoreModule;
using MAEngine;
using MAEngine.Extention;
using UnityEngine;
using UnityEngine.SceneManagement;
using Upgrade;
using Zenject;
using Random = System.Random;

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
        private UpgradesEventBus _upgradesEventBus;
        private UpgradesListConfig _upgradesListConfig;

        private DialogueView _activeDialogue;
        private Timer _dialogueTimer;
        private System.Random _random;
        private int _extraLifes;

        [Inject]
        public void Construct(GUIView guiView, StateEventsBus stateEventsBus, PlayerEventBus playerEventBus,
            UIEventBus uiEventBus, SaveLoadEventBus saveLoadEventBus, InputEventBus inputEventBus, 
            GameEventBus gameEventBus, UpgradesEventBus upgradesEventBus, UpgradesListConfig upgradesListConfig)
        {
            _guiView = guiView;
            _stateEventsBus = stateEventsBus;
            _playerEventBus = playerEventBus;
            _uiEventBus = uiEventBus;
            _saveLoadEventBus = saveLoadEventBus;
            _inputEventBus = inputEventBus;
            _gameEventBus = gameEventBus;
            _upgradesEventBus = upgradesEventBus;
            _upgradesListConfig = upgradesListConfig;
        }
        
        public void PreInitialisation()
        {
            _guiView.InitializeView();
            _inputEventBus.OnEnableMobileInput += EnableMobileInput;
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
            _upgradesEventBus.OnLevelUp += InitializeUpgradeScreen;
            _upgradesEventBus.OnUpgradeApplied += CloseUpgradeScreen;
            _upgradesEventBus.OnExpChanged += UpdateLevelSlider;
            _playerEventBus.OnExtraLifeAdded += AddExtraLife;
            _playerEventBus.OnExtraLifeRemoved += RemoveExtraLife;
            _guiView.DenyUpgrade.Button.onClick.AddListener(DenyUpgrade);
            LoadBestDepth();
            _guiView.ExpSlider.maxValue = _upgradesListConfig.LevelExp;
            _random = new Random();
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
            _upgradesEventBus.OnLevelUp -= InitializeUpgradeScreen;
            _upgradesEventBus.OnUpgradeApplied -= CloseUpgradeScreen;
            _upgradesEventBus.OnExpChanged -= UpdateLevelSlider;
            _playerEventBus.OnExtraLifeAdded -= AddExtraLife;
            _playerEventBus.OnExtraLifeRemoved -= RemoveExtraLife;
            _guiView.DenyUpgrade.Button.onClick.RemoveListener(DenyUpgrade);
            _inputEventBus.OnEnableMobileInput -= EnableMobileInput;
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

        private void EnableMobileInput()
        {
            _guiView.MobileInputPanel.SetActive(true);
        }
        
        private void RemoveExtraLife()
        {
            _extraLifes--;
            if (_guiView.ExtraLifes.Count - 1 >= _extraLifes)
            {
                _guiView.ExtraLifes[_extraLifes].SetActive(false);
            }
        }

        private void AddExtraLife()
        {
            if (_guiView.ExtraLifes.Count - 1 >= _extraLifes)
            {
                _guiView.ExtraLifes[_extraLifes].SetActive(true);
            }
            _extraLifes++;
        }
        
        private void UpdateLevelSlider(int value)
        {
            _guiView.ExpSlider.value = value;
        }
        
        private void DenyUpgrade()
        {
            _upgradesEventBus.OnUpgradeApplied?.Invoke(UpgradeID.NONE);
        }

        private void CloseUpgradeScreen(UpgradeID upgradeId)
        {
            foreach (UpgradeView upgradeView in _guiView.ActiveUpgradeViews)
            {
                GameObject.Destroy(upgradeView.gameObject);
            }
            _guiView.ActiveUpgradeViews.Clear();
            _guiView.UpgradePanel.gameObject.SetActive(false);
        }

        private void InitializeUpgradeScreen(List<UpgradeID> upgradeIds)
        {
            if (upgradeIds.Count == 0)
            {
                _upgradesEventBus?.OnUpgradeApplied.Invoke(UpgradeID.NONE);
                return;
            }
            else if(upgradeIds.Count == 1)
            {
                SpawnUpgradeButton(upgradeIds[0]);
            }
            else if(upgradeIds.Count == 2)
            {
                SpawnUpgradeButton(upgradeIds[0]);
                SpawnUpgradeButton(upgradeIds[1]);
            }
            else
            {
                int index1;
                int index2;
                int index3;
                index1 = _random.Next(0, upgradeIds.Count());
                SpawnUpgradeButton(upgradeIds[index1]);
                index2 = _random.Next(0, upgradeIds.Count());
                while (index2 == index1)
                {
                    index2 = _random.Next(0, upgradeIds.Count());
                }
                SpawnUpgradeButton(upgradeIds[index2]);
                index3 = _random.Next(0, upgradeIds.Count());
                while (index3 == index1 || index3 == index2)
                {
                    index3 = _random.Next(0, upgradeIds.Count());
                }
                SpawnUpgradeButton(upgradeIds[index3]);
            }
            _guiView.UpgradePanel.gameObject.SetActive(true);
        }

        private void SpawnUpgradeButton(UpgradeID upgradeId)
        {
            GameObjectSpawnCallback callback = new GameObjectSpawnCallback();
            _gameEventBus.OnSpawnObject?.Invoke(PrefabID.UIUpgradePanel, Vector3.zero,
                _guiView.UpgradesLayout, callback);
            UpgradeView view = callback.SpawnedObject.GetComponent<UpgradeView>();
            view.InitializeView(_upgradesListConfig.UpgradesConfigsList[upgradeId], _upgradesEventBus);
            _guiView.ActiveUpgradeViews.Add(view);
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