using UnityEngine.Events;

namespace Events
{
    public class PlayerEvents
    {
        public event UnityAction<bool> MoveEvent;
        public event UnityAction<string> RegisterSessionIdEvent; 
        
        public void EmitMoveEvent(bool moving)
        {
            MoveEvent?.Invoke(moving);
        }
        
        public void EmitRegisterSessionIdEvent(string sessionId)
        {
            RegisterSessionIdEvent?.Invoke(sessionId);
        }
    }
}
