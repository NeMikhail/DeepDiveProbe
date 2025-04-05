using System.Collections.Generic;
using MAEngine;
using UnityEngine;

namespace Player
{
    public class ObstacleView : MonoBehaviour
    {
        [SerializeField] private GameObject _obstacleObject;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Scene2DActor _scene2DActor;
        private int _layerId;
        private PrefabID _prefabID;
        private List<ObstacleView> _obstacles;

        public GameObject ObstacleObject => _obstacleObject;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public Scene2DActor Scene2DActor => _scene2DActor;
        public int LayerId => _layerId;
        public PrefabID PrefabID => _prefabID;
        public List<ObstacleView> Obstacles => _obstacles;

        public void SetObstacleData(int layer, PrefabID prefabID,List<ObstacleView> obstacles)
        {
            _layerId = layer;
            _prefabID = prefabID;
            _obstacles = obstacles;
        }
    }
}