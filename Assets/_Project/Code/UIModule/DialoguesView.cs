using UnityEngine;

namespace UI
{
    public class DialoguesView : MonoBehaviour
    {
        [SerializeField] private DialogueView _dialogue1;
        [SerializeField] private DialogueView _dialogue2;
        [SerializeField] private DialogueView _dialogue3;
        
        public DialogueView Dialogue1 => _dialogue1;
        public DialogueView Dialogue2 => _dialogue2;
        public DialogueView Dialogue3 => _dialogue3;
    }
}