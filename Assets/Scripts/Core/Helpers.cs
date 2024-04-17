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
        
        public static int[] GetUnsortedRandomArray(int length, int min, int max)
        {
            var array = new int[length];
            
            for (var i = 0; i < length; i++)
                array[i] = Random.Range(min, max);
            
            // Check if the array is sorted
            var isSorted = true;
            for (var i = 0; i < length - 1; i++)
            {
                if (array[i] <= array[i + 1]) continue;
                
                isSorted = false;
                break;
            }
            
            // If the array is sorted, regenerate it
            if (isSorted)
                array = GetUnsortedRandomArray(length, min, max);

            return array;
        }
    }
}
