using UnityEngine;
using UnityEngine.InputSystem;
using Events;

namespace Input
{
    public class InputManager : MonoBehaviour, InputControls.IPlayerActions
    {
        private InputControls _inputControls;

        private void OnEnable()
        {
            _inputControls = new InputControls();
            
            _inputControls.Player.SetCallbacks(this);
            _inputControls.Enable();
        }
        
        private void OnDisable()
        {
            _inputControls.Disable();
        }
        
        // ====================
        
        public void OnMove(InputAction.CallbackContext context)
        {
            EventManager.Singleton.InputEvents.EmitMoveEvent(context.ReadValue<Vector2>());
        }
        
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed) EventManager.Singleton.InputEvents.EmitInteractEvent();
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed) EventManager.Singleton.InputEvents.EmitPauseEvent();
        }
    }
}
