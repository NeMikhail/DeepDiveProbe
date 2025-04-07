using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "SO/Config/PlayerConfig", order = 0)]
    public class PlayerConfig : ScriptableObject
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _lineChangingTime;
        [SerializeField] private float _layerChangingTime;
        [SerializeField] private int _oxygenValue;
        [SerializeField] private int _addingOxygenValue;
        [SerializeField] private int _maxShieldCharge;
        [SerializeField] private float _line1PositionX;
        [SerializeField] private float _line2PositionX;
        [SerializeField] private float _line3PositionX;
        
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _goingDownSprite;
        [SerializeField] private Sprite _goingUpSprite;

        public float Speed { get => _speed; }
        public float LineChangingTime { get => _lineChangingTime; }
        public float LayerChangingTime { get => _layerChangingTime; }
        public int OxygenValue { get => _oxygenValue; }
        public int AddingOxygenValue { get => _addingOxygenValue; }
        public int MaxShieldCharge { get => _maxShieldCharge; }
        public float Line1PositionX { get => _line1PositionX; }
        public float Line2PositionX { get => _line2PositionX; }
        public float Line3PositionX { get => _line3PositionX; }
        public Sprite DefaultSprite { get => _defaultSprite; }
        public Sprite GoingDownSprite { get => _goingDownSprite; }
        public Sprite GoingUpSprite { get => _goingUpSprite; }
    }
}
