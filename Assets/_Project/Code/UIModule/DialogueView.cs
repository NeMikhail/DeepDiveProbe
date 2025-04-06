using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class DialogueView : MonoBehaviour
    {
        [SerializeField] private List<RectTransform> _phrasesList;
        [SerializeField] private TMP_Text _changableText;
        private int _phraseIndex;
        private bool _isClosed;

        public List<RectTransform> PhrasesList => _phrasesList;
        public TMP_Text ChangableText => _changableText;
        public bool IsClosed => _isClosed;

        public void InitializeDialogue()
        {
            foreach (RectTransform rectTransform in _phrasesList)
            {
                rectTransform.gameObject.SetActive(false);
            }
            SetPhrase(0);
            gameObject.SetActive(true);
            _isClosed = false;
        }
        
        public void CloseDialogue()
        {
            foreach (RectTransform rectTransform in _phrasesList)
            {
                rectTransform.gameObject.SetActive(false);
            }
            gameObject.SetActive(false);
            _isClosed = true;
        }

        private void SetPhrase(int index)
        {
            _phrasesList[index].gameObject.SetActive(true);
            _phraseIndex = index;
        }

        public void NextPhrase()
        {
            int index = _phraseIndex + 1;
            if (index < _phrasesList.Count)
            {
                SetPhrase(index);
            }
            else
            {
                CloseDialogue();
            }
        }

        public void SetChangableText(int value)
        {
            if (_changableText != null)
            {
                string changedText = $"The previous probe reached a depth of:\n{value} meters";
                _changableText.text = changedText;
            }
            
        }
    }
}