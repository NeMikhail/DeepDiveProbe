using System;
using UnityEngine;

namespace GameCoreModule
{
    public class InputEventBus
    {
        private Action _onEnableInput;
        private Action _onDisableInput;
        private Action _onEnableMobileInput;
        
        private Action<Vector2> _onMovementInputValueChanged;
        private Action<Vector2> _onLookInputValueChanged;
        
        private Action _onPauseButtonPerfomed;
        private Action _onAttackButtonPerformed;
        private Action _onAttackButtonCanceled;
        private Action _onJumpButtonPerformed;
        private Action _onJumpButtonCanceled;
        private Action _onRunButtonPerformed;
        private Action _onRunButtonCanceled;
        private Action _onInteractButtonPerformed;
        private Action _onMoveLeftButtonPerformed;
        private Action _onMoveRightButtonPerformed;
        private Action _onMoveUpButtonPerformed;
        private Action _onMoveDownButtonPerformed;

        private Action _onUIClickButtonPerfomed;
        private Action _onUIClickButtonCanceled;
        


        public Action OnEnableInput { get => _onEnableInput; set => _onEnableInput = value; }
        public Action OnDisableInput { get => _onDisableInput; set => _onDisableInput = value; }

        public Action OnEnableMobileInput { get => _onEnableMobileInput; set => _onEnableMobileInput = value; }

        public Action<Vector2> OnMovementInputValueChanged 
        { get => _onMovementInputValueChanged; set => _onMovementInputValueChanged = value; }
        public Action<Vector2> OnLookInputValueChanged
        { get => _onLookInputValueChanged; set => _onLookInputValueChanged = value; }
        
        public Action OnPauseButtonPerformed 
        { get => _onPauseButtonPerfomed; set => _onPauseButtonPerfomed = value; }
        public Action OnAttackButtonPerformed 
        { get => _onAttackButtonPerformed; set => _onAttackButtonPerformed = value; }
        public Action OnAttackButtonCanceled
        { get => _onAttackButtonCanceled; set => _onAttackButtonCanceled = value; }
        public Action OnJumpButtonPerformed
        { get => _onJumpButtonPerformed; set => _onJumpButtonPerformed = value; }
        public Action OnJumpButtonCanceled
        { get => _onJumpButtonCanceled; set => _onJumpButtonCanceled = value; }
        public Action OnRunButtonPerformed
        { get => _onRunButtonPerformed; set => _onRunButtonPerformed = value; }
        public Action OnRunButtonCanceled
        { get => _onRunButtonCanceled; set => _onRunButtonCanceled = value; }
        public Action OnInteractButtonPerformed
        { get => _onInteractButtonPerformed; set => _onInteractButtonPerformed = value; }
        public Action OnMoveLeftButtonPerformed
        { get => _onMoveLeftButtonPerformed; set => _onMoveLeftButtonPerformed = value; }
        public Action OnMoveRightButtonPerformed
        { get => _onMoveRightButtonPerformed; set => _onMoveRightButtonPerformed = value; }
        public Action OnMoveUpButtonPerformed
        { get => _onMoveUpButtonPerformed; set => _onMoveUpButtonPerformed = value; }
        public Action OnMoveDownButtonPerformed
        { get => _onMoveDownButtonPerformed; set => _onMoveDownButtonPerformed = value; }
        
        public Action OnUIClickButtonPerfomed
        { get => _onUIClickButtonPerfomed; set => _onUIClickButtonPerfomed = value; }
        public Action OnUIClickButtonCanceled
        { get => _onUIClickButtonCanceled; set => _onUIClickButtonCanceled = value; }
    }
}
