using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    public class InputEvents
    {
        public event UnityAction<Vector2> MoveEvent;
        
        public void EmitMoveEvent(Vector2 input)
        {
            MoveEvent?.Invoke(input);
        }
    }
}
