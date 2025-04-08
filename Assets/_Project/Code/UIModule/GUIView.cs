using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Upgrade;

namespace UI
{
    public class GUIView : MonoBehaviour
    {
        [SerializeField] private ButtonView _pauseButton;
        [SerializeField] private Slider _oxygenSlider;
        [SerializeField] private Slider _depthSlider;
        [SerializeField] private Image _Layer1Image;
        [SerializeField] private Image _Layer2Image;
        [SerializeField] private Image _Layer3Image;
        [SerializeField] private Sprite _activeLayerSprite;
        [SerializeField] private Sprite _inactiveLayerSprite;
        [SerializeField] private TMP_Text _depthText;
        [SerializeField] private Slider _bestDepthSlider;
        [SerializeField] private TMP_Text _bestDepthText;
        [SerializeField] private DialoguesView _dialoguesView;
        [SerializeField] private GameObject _winScreenObject;
        [SerializeField] private GameObject _loseScreenObject;
        [SerializeField] private ButtonView _winRetryButton;
        [SerializeField] private ButtonView _loseRetryButton;
        [SerializeField] private ButtonView _winProgressClearButton;
        [SerializeField] private GameObject _upgradePanel;
        [SerializeField] private RectTransform _upgragesLayout;
        [SerializeField] private Slider _expSlider;
        [SerializeField] private ButtonView _denyUpgrade;
        [SerializeField] private List<GameObject> _extraLifes;
        [SerializeField] private GameObject _mobileInputPanel;
        private List<UpgradeView> _activeUpgradeViews;
        
        public ButtonView PauseButton => _pauseButton;
        public Slider OxygenSlider => _oxygenSlider;
        public Slider DepthSlider => _depthSlider;
        public TMP_Text DepthText => _depthText;
        public Slider BestDepthSlider => _bestDepthSlider;
        public TMP_Text BestDepthText => _bestDepthText;
        public DialoguesView DialoguesView => _dialoguesView;
        public GameObject WinScreenObject => _winScreenObject;
        public GameObject LoseScreenObject => _loseScreenObject;
        public ButtonView WinRetryButton => _winRetryButton;
        public ButtonView LoseRetryButton => _loseRetryButton;
        public ButtonView WinProgressClearButton => _winProgressClearButton;
        public GameObject UpgradePanel => _upgradePanel;
        public RectTransform UpgradesLayout => _upgragesLayout;
        public Slider ExpSlider => _expSlider;
        public ButtonView DenyUpgrade => _denyUpgrade;
        public List<GameObject> ExtraLifes => _extraLifes;
        public GameObject MobileInputPanel => _mobileInputPanel;
        public List<UpgradeView> ActiveUpgradeViews => _activeUpgradeViews;
        

        public void InitializeView()
        {
            _activeUpgradeViews = new List<UpgradeView>();
            _oxygenSlider.value = _oxygenSlider.maxValue;
            _depthSlider.value = 0;
            UpdateLayerImages(2);
        }

        public void UpdateLayerImages(int layer)
        {
            switch (layer)
            {
                case 1:
                    _Layer1Image.sprite = _activeLayerSprite;
                    _Layer2Image.sprite = _inactiveLayerSprite;
                    _Layer3Image.sprite = _inactiveLayerSprite;
                    break;
                case 2:
                    _Layer1Image.sprite = _inactiveLayerSprite;
                    _Layer2Image.sprite = _activeLayerSprite;
                    _Layer3Image.sprite = _inactiveLayerSprite;
                    break;
                case 3:
                    _Layer1Image.sprite = _inactiveLayerSprite;
                    _Layer2Image.sprite = _inactiveLayerSprite;
                    _Layer3Image.sprite = _activeLayerSprite;
                    break;
                default:
                    _Layer1Image.sprite = _inactiveLayerSprite;
                    _Layer2Image.sprite = _inactiveLayerSprite;
                    _Layer3Image.sprite = _inactiveLayerSprite;
                    break;
                
            }
        }
    }
}
