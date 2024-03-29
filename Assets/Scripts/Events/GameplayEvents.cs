using UnityEngine.Events;

namespace Events
{
    public class GameplayEvents
    {
        public event UnityAction GameOverEvent;
        public event UnityAction RetryEvent;
        public event UnityAction RequestRetryEvent;
        
        public void EmitGameOverEvent()
        {
            GameOverEvent?.Invoke();
        }
        
        public void EmitRetryEvent()
        {
            RetryEvent?.Invoke();
        }
        
        public void EmitRequestRetryEvent()
        {
            RequestRetryEvent?.Invoke();
        }
    }
}
