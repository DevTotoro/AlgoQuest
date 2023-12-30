using UnityEngine;
using UnityEngine.InputSystem;
using Events;

namespace Input
{
    public class InputManager : MonoBehaviour, InputControls.IPlayerActions
    {
        private static InputManager Singleton { get; set; }
        
        private InputControls _inputControls;

        private void OnEnable()
        {
            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Singleton = this;
            DontDestroyOnLoad(gameObject);
            
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
    }
}
