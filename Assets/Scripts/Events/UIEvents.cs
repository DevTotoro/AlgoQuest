using UnityEngine.Events;

namespace Events
{
    public class UIEvents
    {
        public event UnityAction<bool> ToggleTimerEvent;
        public event UnityAction<bool> ToggleLeaderboardEvent; 
        
        public event UnityAction<string> SetTimerEvent;
        public event UnityAction<string> SetLeaderboardEvent;
        
        public void EmitToggleTimerEvent(bool enable)
        {
            ToggleTimerEvent?.Invoke(enable);
        }
        
        public void EmitToggleLeaderboardEvent(bool enable)
        {
            ToggleLeaderboardEvent?.Invoke(enable);
        }
        
        public void EmitSetTimerEvent(string time)
        {
            SetTimerEvent?.Invoke(time);
        }
        
        public void EmitSetLeaderboardEvent(string highScores)
        {
            SetLeaderboardEvent?.Invoke(highScores);
        }
    }
}
