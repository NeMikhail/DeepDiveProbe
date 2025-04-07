using GameCoreModule;
using MAEngine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Input
{
    public class InputActions : IAction, IPreInitialisation, IExecute, ICleanUp
    {
        private InputSystem_Actions _controls;
        private InputEventBus _inputEventBus;
        private bool _isBinded;
        

        private Vector2 _currentMovementVector;

        [Inject]
        public void Construct(InputEventBus inputEventBus)
        {
            _inputEventBus = inputEventBus;
        }

        public void PreInitialisation()
        {
            _controls = new InputSystem_Actions();
            EnableInput();
            _inputEventBus.OnEnableInput += EnableInput;
            _inputEventBus.OnDisableInput += DisableInput;
            BindInput();
            _currentMovementVector = Vector2.zero;
            bool isWebGLOnDesktop = !Application.isMobilePlatform
                                    && Application.platform == RuntimePlatform.WebGLPlayer;

            bool isWebGLOnMobile = Application.isMobilePlatform
                                   && Application.platform == RuntimePlatform.WebGLPlayer;
            
        }

        public void Cleanup()
        {
            _inputEventBus.OnEnableInput -= EnableInput;
            _inputEventBus.OnDisableInput -= DisableInput;
            UnbindInput();
        }

        public void Execute(float deltaTime)
        {
            if (_isBinded)
            {
                ReadMovementVector();
                ReadLookVector();
            }
        }

        private void ReadMovementVector()
        {
            Vector2 movementVector = 
                _controls.Player.Move.ReadValue<Vector2>();
            if (_currentMovementVector != movementVector)
            {
                _currentMovementVector = movementVector;
                _inputEventBus.OnMovementInputValueChanged?.Invoke(movementVector);
            }
        }
        
        private void ReadLookVector()
        {
            Vector2 lookVector = _controls.Player.Look.ReadValue<Vector2>();
            _inputEventBus.OnLookInputValueChanged?.Invoke(lookVector);
        }

        private void EnableInput()
        {
            _controls.Enable();
        }

        private void DisableInput()
        {
            _controls.Disable();
        }

        private void BindInput()
        {
            var gameInput = _controls.Player;
            var uiInput = _controls.UI;

            BindGameInput(gameInput);
            BindUIInput(uiInput);
            
            _isBinded = true;
        }

        private void UnbindInput()
        {
            var gameInput = _controls.Player;
            var uiInput = _controls.UI;

            UnBindGameInput(gameInput);
            UnBindUIInput(uiInput);
            
            _isBinded = false;
        }

        private void BindGameInput(InputSystem_Actions.PlayerActions gameInput)
        {
            gameInput.Pause.performed += InvokePauseEvent;
            gameInput.Attack.performed += InvokeAttackPerformedEvent;
            gameInput.Attack.canceled += InvokeAttackCanceledEvent;
            gameInput.Jump.performed += InvokeJumpPerformedEvent;
            gameInput.Jump.canceled += InvokeJumpCanceledEvent;
            gameInput.Sprint.performed += InvokeRunPerformedEvent;
            gameInput.Sprint.canceled += InvokeRunCanceledEvent;
            gameInput.Interact.performed += InvokeInteractPerformedEvent;
            gameInput.MoveLeft.performed += InvokeMoveLeftEvent;
            gameInput.MoveRight.performed += InvokeMoveRightEvent;
            gameInput.MoveUp.performed += InvokeMoveUpEvent;
            gameInput.MoveDown.performed += InvokeMoveDownEvent;
        }
        
        private void UnBindGameInput(InputSystem_Actions.PlayerActions gameInput)
        {
            gameInput.Pause.performed -= InvokePauseEvent;
            gameInput.Attack.performed -= InvokeAttackPerformedEvent;
            gameInput.Attack.canceled -= InvokeAttackCanceledEvent;
            gameInput.Jump.performed -= InvokeJumpPerformedEvent;
            gameInput.Jump.canceled -= InvokeJumpCanceledEvent;
            gameInput.Sprint.performed -= InvokeRunPerformedEvent;
            gameInput.Sprint.canceled -= InvokeRunCanceledEvent;
            gameInput.Interact.performed -= InvokeInteractPerformedEvent;
            gameInput.MoveLeft.performed -= InvokeMoveLeftEvent;
            gameInput.MoveRight.performed -= InvokeMoveRightEvent;
            gameInput.MoveUp.performed -= InvokeMoveUpEvent;
            gameInput.MoveDown.performed -= InvokeMoveDownEvent;
        }
        
        private void BindUIInput(InputSystem_Actions.UIActions uiInput)
        {
            uiInput.Click.performed += InvokeUIClickPerformedEvent;
            uiInput.Click.canceled += InvokeUIClickCanceledEvent;
        }
        
        private void UnBindUIInput(InputSystem_Actions.UIActions uiInput)
        {
            uiInput.Click.performed -= InvokeUIClickPerformedEvent;
            uiInput.Click.canceled -= InvokeUIClickCanceledEvent;
        }
        
        private void InvokePauseEvent(InputAction.CallbackContext obj)
        {
            _inputEventBus.OnPauseButtonPerformed?.Invoke();
        }
        
        private void InvokeAttackPerformedEvent(InputAction.CallbackContext obj)
        {
            _inputEventBus.OnAttackButtonPerformed?.Invoke();
        }
        
        private void InvokeAttackCanceledEvent(InputAction.CallbackContext obj)
        {
            _inputEventBus.OnAttackButtonCanceled?.Invoke();
        }
        
        private void InvokeJumpPerformedEvent(InputAction.CallbackContext obj)
        {
            _inputEventBus.OnJumpButtonPerformed?.Invoke();
        }
        
        private void InvokeJumpCanceledEvent(InputAction.CallbackContext obj)
        {
            _inputEventBus.OnJumpButtonCanceled?.Invoke();
        }

        private void InvokeRunPerformedEvent(InputAction.CallbackContext obj)
        {
            _inputEventBus.OnRunButtonPerformed?.Invoke();
        }
        
        private void InvokeRunCanceledEvent(InputAction.CallbackContext obj)
        {
            _inputEventBus.OnRunButtonCanceled?.Invoke();
        }

        private void InvokeInteractPerformedEvent(InputAction.CallbackContext obj)
        {
            _inputEventBus.OnInteractButtonPerformed?.Invoke();
        }
        
        private void InvokeMoveLeftEvent(InputAction.CallbackContext obj)
        {
            _inputEventBus.OnMoveLeftButtonPerformed?.Invoke();
        }
        
        private void InvokeMoveRightEvent(InputAction.CallbackContext obj)
        {
            _inputEventBus.OnMoveRightButtonPerformed?.Invoke();
        }
        
        private void InvokeMoveUpEvent(InputAction.CallbackContext obj)
        {
            _inputEventBus.OnMoveUpButtonPerformed?.Invoke();
        }

        private void InvokeMoveDownEvent(InputAction.CallbackContext obj)
        {
            _inputEventBus.OnMoveDownButtonPerformed?.Invoke();
        }

        private void InvokeUIClickPerformedEvent(InputAction.CallbackContext obj)
        {
            _inputEventBus.OnUIClickButtonPerfomed?.Invoke();
        }

        private void InvokeUIClickCanceledEvent(InputAction.CallbackContext obj)
        {
            _inputEventBus.OnUIClickButtonCanceled?.Invoke();
        }
        
    }
}

