using UnityEngine.Events;

namespace Events
{
    public class GameplayEvents
    {
        public event UnityAction GameOverEvent;
        public event UnityAction RetryEvent;
        public event UnityAction RequestRetryEvent;
        
        public event UnityAction SuccessEvent;
        public event UnityAction RestartEvent;
        public event UnityAction RequestRestartEvent;
        
        public event UnityAction<string> TimerUpdatedEvent;
        public event UnityAction<string> HighScoresFetchedEvent;
        
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
        
        public void EmitSuccessEvent()
        {
            SuccessEvent?.Invoke();
        }
        
        public void EmitRestartEvent()
        {
            RestartEvent?.Invoke();
        }
        
        public void EmitRequestRestartEvent()
        {
            RequestRestartEvent?.Invoke();
        }
        
        public void EmitTimerUpdatedEvent(string time)
        {
            TimerUpdatedEvent?.Invoke(time);
        }
        
        public void EmitHighScoresFetchedEvent(string highScores)
        {
            HighScoresFetchedEvent?.Invoke(highScores);
        }
    }
}
