using System.Threading.Tasks;

namespace AlgoQuestServices
{
    public static class Algorithms
    {
        public enum AlgorithmCompletionStatus
        {
            Success,
            Failure
        }
        
        private struct CreateAlgorithmRequestPayload
        {
            public string type;
            public string status;
            public int time; // ms
            public string[] sessions;
        }
        
        public struct CreateAlgorithmResponsePayload
        {
            public string id;
            public string type;
            public string status;
            public int time;
            public Sessions.CreateSessionResponsePayload[] sessions;
        }

        public static async Task<HttpResponse<CreateAlgorithmResponsePayload>> Create(
            Gameplay.SortingAlgorithm algorithm, AlgorithmCompletionStatus status, int time, string[] sessions)
        {
            var data = new CreateAlgorithmRequestPayload
            {
                type = GetAlgorithmType(algorithm),
                status = GetAlgorithmCompletionStatus(status),
                time = time,
                sessions = sessions
            };
            
            return await Create(data);
        }

        public static async Task<HttpResponse<CreateAlgorithmResponsePayload[]>> GetHighScores(
            Gameplay.SortingAlgorithm algorithm, int take = 10)
        {
            return await Http.Get<CreateAlgorithmResponsePayload[]>($"algorithms/high-scores",
                $"?algorithm={GetAlgorithmType(algorithm)}&take={take}");
        }
        
        private static async Task<HttpResponse<CreateAlgorithmResponsePayload>> Create(CreateAlgorithmRequestPayload data)
        {
            return await Http.Post<CreateAlgorithmResponsePayload, CreateAlgorithmRequestPayload>("algorithms", data);
        }
        
        private static string GetAlgorithmType(Gameplay.SortingAlgorithm algorithm)
        {
            return algorithm switch
            {
                Gameplay.SortingAlgorithm.BubbleSort => "BUBBLE_SORT",
                Gameplay.SortingAlgorithm.SelectionSort => "SELECTION_SORT",
                _ => "UNKNOWN"
            };
        }

        private static string GetAlgorithmCompletionStatus(AlgorithmCompletionStatus status)
        {
            return status switch
            {
                AlgorithmCompletionStatus.Success => "SUCCESS",
                AlgorithmCompletionStatus.Failure => "FAILURE",
                _ => "UNKNOWN"
            };
        }
    }
}