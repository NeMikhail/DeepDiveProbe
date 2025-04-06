using UnityEngine;
using MAEngine;

namespace Player
{
    public class PlayerView : MonoBehaviour, IView
    {
        [SerializeField] private Rigidbody2D _playerRB;
        [SerializeField] private GameObject _object;
        [SerializeField] private SpriteRenderer _playerSpriteRenderer;
        [SerializeField] private Scene2DActor _winZoneActor;
        [SerializeField] private GameObject _oxygenShieldObject;
        private int _currentLine;
        private int _currentLayer;
        private int _currentOxygen;
        private int _currentDepth;
        private bool _isLightOn;
        private StageID _currentStage;
        private float _currentSpeed;
        
        private string _viewID;

        public Rigidbody2D PlayerRB { get => _playerRB; }
        public GameObject Object { get => _object; }
        public SpriteRenderer PlayerSpriteRenderer { get => _playerSpriteRenderer; }
        public Scene2DActor WinZoneActor { get => _winZoneActor; }
        public GameObject OxygenShieldObject { get => _oxygenShieldObject; }
        public string ViewID { get => _viewID; set => _viewID = value; }
        public int CurrentLine { get => _currentLine; set => _currentLine = value; }
        public int CurrentLayer { get => _currentLayer; set => _currentLayer = value; }
        public int CurrentOxygen { get => _currentOxygen; set => _currentOxygen = value; }
        public int CurrentDepth { get => _currentDepth; set => _currentDepth = value; }
        public bool IsLightOn { get => _isLightOn; set => _isLightOn = value; }
        public StageID CurrentStage { get => _currentStage; set => _currentStage = value; }
        public float CurrentSpeed { get => _currentSpeed; set => _currentSpeed = value; }
    }
}
