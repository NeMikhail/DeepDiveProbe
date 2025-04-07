using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "ObstaclesSpawnConfig", menuName = "SO/Config/ObstaclesSpawnConfig", order = 0)]
    public class ObstaclesSpawnConfig : ScriptableObject
    {
        [SerializeField] private float _stage1OxygenSpawnCount;
        [SerializeField] private float _stage2OxygenSpawnCount;
        [SerializeField] private float _stage3OxygenSpawnCount;
        
        [SerializeField] private List<int> _oxygenSpawnIndexes;
        [SerializeField] private List<PrefabID> _stage1Obstacles;
        [SerializeField] private List<PrefabID> _stage2Obstacles;
        [SerializeField] private List<PrefabID> _stage3Obstacles;
        
        public float Stage1OxygenSpawnCount { get => _stage1OxygenSpawnCount; }
        public float Stage2OxygenSpawnCount { get => _stage2OxygenSpawnCount; }
        public float Stage3OxygenSpawnCount { get => _stage3OxygenSpawnCount; }
        public List<int> OxygenSpawnIndexes { get => _oxygenSpawnIndexes; }
        public List<PrefabID> Stage1Obstacles { get => _stage1Obstacles; }
        public List<PrefabID> Stage2Obstacles { get => _stage2Obstacles; }
        public List<PrefabID> Stage3Obstacles { get => _stage3Obstacles; }
    }
}