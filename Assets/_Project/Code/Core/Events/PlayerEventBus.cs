using System;
using UnityEngine;

namespace GameCoreModule
{
    public class PlayerEventBus
    {
        private Action<GameObject> _tryInteractWithObject;
        
        public Action<GameObject> OnTryInteractWithObject
        { get => _tryInteractWithObject; set => _tryInteractWithObject = value; }
    }
}