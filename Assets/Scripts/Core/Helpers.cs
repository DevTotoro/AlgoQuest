using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public static class Helpers
    {
        public static void CloseApplication()
        {
            Events.EventManager.Singleton.UIEvents.EmitCloseApplicationEvent();
            
            Application.Quit();
        }
        
        public static void CopyToClipboard(string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }

        public static async Task Log(Events.LogType logType)
        {
            var data = new AlgoQuestServices.Logs.CreateLogPayload
            {
                type = logType,
                
                sessionId = AlgoQuestServices.Http.SessionId,
                
                nullAlgorithmType = true,
                nullGameMode = true,
                nullContainerValues = true
            };

            await AlgoQuestServices.Logs.Create(data);
        }
    }
}
