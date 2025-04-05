using System.Collections.Generic;

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
    }
}