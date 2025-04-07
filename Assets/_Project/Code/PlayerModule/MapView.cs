using UnityEngine;

namespace Player
{
    public class MapView : MonoBehaviour
    {
        [SerializeField] private LayerView _layer1View;
        [SerializeField] private LayerView _layer2View;
        [SerializeField] private LayerView _layer3View;
        
        [SerializeField] private Transform _leftWallTransform;
        [SerializeField] private Transform _rightWallTransform;
        
        public LayerView Layer1View { get => _layer1View; }
        public LayerView Layer2View { get => _layer2View; }
        public LayerView Layer3View { get => _layer3View; }
        public Transform LeftWallTransform { get => _leftWallTransform; }
        public Transform RightWallTransform { get => _rightWallTransform; }
    }
}