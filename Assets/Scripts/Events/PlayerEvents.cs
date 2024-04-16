using UnityEngine.Events;

namespace Events
{
    public class PlayerEvents
    {
        public event UnityAction<bool> MoveEvent;
        
        public void EmitMoveEvent(bool moving)
        {
            MoveEvent?.Invoke(moving);
        }
    }
}
