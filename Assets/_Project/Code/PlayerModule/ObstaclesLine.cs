using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class ObstaclesLine
    {
        private List<ObstacleView> _obstacles;

        public List<ObstacleView> Obstacles
        {
            get => _obstacles;
            set => _obstacles = value;
        }

        public void RemoveAll()
        {
            foreach (ObstacleView view in _obstacles)
            {
                GameObject.Destroy(view.gameObject);
            }
        }
    }
}