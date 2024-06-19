using UnityEngine.Events;

namespace Events
{
    public enum LogType
    {
        HOST_CREATED,
        HOST_CLOSED,
            
        HOST_JOINED,
        HOST_LEFT,
            
        GAME_STARTED,
        GAME_OVER,
        GAME_WON,
        GAME_RESTARTED,
    }
    
    public class LogEvents
    {
        public event UnityAction<string, LogType> NetworkLogEvent;
        
        public void EmitNetworkLogEvent(LogType logType)
        {
            NetworkLogEvent?.Invoke(AlgoQuestServices.Http.SessionId, logType);
        }
    }
}
