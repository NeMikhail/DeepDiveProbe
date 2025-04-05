using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "SO/Config/PlayerConfig", order = 0)]
    public class PlayerConfig : ScriptableObject
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _inertiaModifier;
        [SerializeField] private float _cameraSpeed;

        public float Speed { get => _speed; }
        public float InertiaModifier { get => _inertiaModifier; }
        public float CameraSpeed { get => _cameraSpeed; set => _cameraSpeed = value; }
    }
}
