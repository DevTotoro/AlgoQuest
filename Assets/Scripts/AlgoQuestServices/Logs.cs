using System.Threading.Tasks;
using UnityEngine;

namespace AlgoQuestServices
{
    public static class Logs
    {
        public struct CreateLogPayload
        {
            public Events.LogType type;
            
            public Gameplay.SortingAlgorithm algorithmType;
            public Gameplay.SortingAlgorithmMode gameMode;
            public int[] containerValues;
            
            public string sessionId;

            public bool nullAlgorithmType;
            public bool nullGameMode;
            public bool nullContainerValues;
        }
        
        public static async Task Create(CreateLogPayload data, string message = null)
        {
            var dataInternal = new CreateLogPayloadInternal
            {
                type = GetLogType(data.type),
                message = message ?? GetLogMessage(data.type),
                
                sessionId = data.sessionId
            };
            
            if (!data.nullAlgorithmType) dataInternal.algorithmType = Algorithms.GetAlgorithmType(data.algorithmType);
            if (!data.nullGameMode) dataInternal.gameMode = Algorithms.GetAlgorithmMode(data.gameMode);
            if (!data.nullContainerValues) dataInternal.containerValues = data.containerValues;
            
            await CreateInternal(dataInternal);
        }

        // ====================
        
        private struct CreateLogPayloadInternal
        {
            public string type;
            public string message;
            
            public string algorithmType;
            public string gameMode;
            public int[] containerValues;
            
            public string sessionId;
        }
        
        private static async Task CreateInternal(CreateLogPayloadInternal data)
        {
            await Http.Post<object, CreateLogPayloadInternal>("logs", data);
        }
        
        // ====================
        
        private static string GetLogType(Events.LogType logType)
        {
            return logType switch
            {
                Events.LogType.HOST_CREATED => "HOST_CREATED",
                Events.LogType.HOST_CLOSED => "HOST_CLOSED",
                
                Events.LogType.HOST_JOINED => "HOST_JOINED",
                Events.LogType.HOST_LEFT => "HOST_LEFT",
                
                Events.LogType.GAME_STARTED => "GAME_STARTED",
                Events.LogType.GAME_OVER => "GAME_OVER",
                Events.LogType.GAME_WON => "GAME_WON",
                Events.LogType.GAME_RESTARTED => "GAME_RESTARTED",
                
                _ => "UNKNOWN"
            };
        }

        private static string GetLogMessage(Events.LogType logType)
        {
            return logType switch
            {
                Events.LogType.HOST_CREATED => "A host has been created",
                Events.LogType.HOST_CLOSED => "A host has been closed",
                
                Events.LogType.HOST_JOINED => "A player has joined the host",
                Events.LogType.HOST_LEFT => "A player has left the host",
                
                Events.LogType.GAME_STARTED => "A game has started",
                Events.LogType.GAME_OVER => "A game ended in defeat",
                Events.LogType.GAME_WON => "A game ended in victory",
                Events.LogType.GAME_RESTARTED => "A game has been restarted",
                
                _ => "Unknown log type"
            };
        }
    }
}
