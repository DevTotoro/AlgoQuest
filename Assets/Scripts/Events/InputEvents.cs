using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    public class InputEvents
    {
        public event UnityAction<Vector2> MoveEvent;
        public event UnityAction PauseEvent;
        
        public void EmitMoveEvent(Vector2 input)
        {
            MoveEvent?.Invoke(input);
        }
        
        public void EmitPauseEvent()
        {
            PauseEvent?.Invoke();
        }
    }
}
