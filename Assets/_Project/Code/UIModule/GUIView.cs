using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
        
        public ButtonView PauseButton => _pauseButton;
        public Slider OxygenSlider => _oxygenSlider;
        public Slider DepthSlider => _depthSlider;
        public TMP_Text DepthText => _depthText;
        public Slider BestDepthSlider => _bestDepthSlider;
        public TMP_Text BestDepthText => _bestDepthText;
        public DialoguesView DialoguesView => _dialoguesView;
        

        public void InitializeView()
        {
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
