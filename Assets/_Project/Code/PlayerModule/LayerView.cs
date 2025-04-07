using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class LayerView : MonoBehaviour
    {
        [SerializeField] private Transform _spawnZonesTransform;
        private List<SpawnZonesView> _spawnZones;
        
        public Transform SpawnZonesTransform { get => _spawnZonesTransform; }
        public List<SpawnZonesView> SpawnZones { get => _spawnZones; set => _spawnZones = value; }
    }
}