using UnityEngine;
using MAEngine;

namespace Player
{
    public class PlayerView : MonoBehaviour, IView
    {
        [SerializeField] private Rigidbody2D _playerRB;
        [SerializeField] private GameObject _object;
        private int _direction;
        private string _viewID;

        public Rigidbody2D PlayerRB { get => _playerRB; }
        public GameObject Object { get => _object; }
        public int Direction { get => _direction; set => _direction = value; }
        public string ViewID { get => _viewID; set => _viewID = value; }

        private void Awake()
        {
            _viewID = _object.name;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {

        }
    }
}
