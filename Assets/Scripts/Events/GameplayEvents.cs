using UnityEngine.Events;

namespace Events
{
    public class GameplayEvents
    {
        public event UnityAction<ulong, int> SetPlayerContainerValueEvent;
        
        public event UnityAction GameOverEvent;
        public event UnityAction GameWonEvent;
        public event UnityAction RequestRestartEvent;
        public event UnityAction RestartEvent;
        
        public void EmitSetPlayerContainerValueEvent(ulong clientId, int value)
        {
            SetPlayerContainerValueEvent?.Invoke(clientId, value);
        }
        
        public void EmitGameOverEvent()
        {
            GameOverEvent?.Invoke();
        }
        
        public void EmitGameWonEvent()
        {
            GameWonEvent?.Invoke();
        }
        
        public void EmitRequestRestartEvent()
        {
            RequestRestartEvent?.Invoke();
        }
        
        public void EmitRestartEvent()
        {
            RestartEvent?.Invoke();
        }
    }
}
