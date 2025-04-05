using MAEngine;
using UnityEngine;

namespace Player
{
    public class SpawnZonesView : MonoBehaviour
    {
        [SerializeField] private Transform _line1SpawnTransform;
        [SerializeField] private Transform _line2SpawnTransform;
        [SerializeField] private Transform _line3SpawnTransform;
        [SerializeField] private Scene2DActor _spawnLineActor;
        
        public Transform Line1SpawnTransform { get => _line1SpawnTransform; }
        public Transform Line2SpawnTransform { get => _line2SpawnTransform; }
        public Transform Line3SpawnTransform { get => _line3SpawnTransform; }
        public Scene2DActor SpawnLineActor { get => _spawnLineActor; }
    }
}