using System.Threading.Tasks;

namespace AlgoQuestServices
{
    public static class TimeTrials
    {
        public struct CreateTimeTrialPayload
        {
            public Gameplay.SortingAlgorithm type;
            public int time; // ms
            public int numberOfValues;
            public int requiredMoves;
            public string[] sessions;
        }

        public struct GetTimeTrialsQueryParams
        {
            public Gameplay.SortingAlgorithm type;
            public int numberOfValues;
            public int requiredMoves;
        }

        public struct GetTimeTrialsResponsePayload
        {
            public int time;
            public Sessions.CreateSessionResponsePayload[] sessions;
        }

        public static async Task Create(CreateTimeTrialPayload data)
        {
            var dataPrivate = new CreateTimeTrialPayloadPrivate
            {
                type = GetAlgorithmType(data.type),
                time = data.time,
                numberOfValues = data.numberOfValues,
                requiredMoves = data.requiredMoves,
                sessions = data.sessions
            };
            
            await Create(dataPrivate);
        }

        public static async Task<HttpResponse<GetTimeTrialsResponsePayload[]>> Get(GetTimeTrialsQueryParams queryParams)
        {
            var dataPrivate = new GetTimeTrialsQueryParamsPrivate
            {
                type = GetAlgorithmType(queryParams.type),
                numberOfValues = queryParams.numberOfValues,
                requiredMoves = queryParams.requiredMoves,
            };
            
            return await Get(dataPrivate);
        }

        // ====================
        
        private struct CreateTimeTrialPayloadPrivate
        {
            public string type;
            public int time; // ms
            public int numberOfValues;
            public int requiredMoves;
            public string[] sessions;
        }
        
        private struct GetTimeTrialsQueryParamsPrivate
        {
            public string type;
            public int numberOfValues;
            public int requiredMoves;
        }
        
        private static async Task Create(CreateTimeTrialPayloadPrivate data)
        {
            await Http.Post<object, CreateTimeTrialPayloadPrivate>("time-trials", data);
        }

        private static async Task<HttpResponse<GetTimeTrialsResponsePayload[]>> Get(
            GetTimeTrialsQueryParamsPrivate queryParams)
        {
            var query = $"?type={queryParams.type}" +
                        $"&numberOfValues={queryParams.numberOfValues}" +
                        $"&requiredMoves={queryParams.requiredMoves}";
            
            return await Http.Get<GetTimeTrialsResponsePayload[]>($"time-trials", query);
        }
        
        // ====================
        
        private static string GetAlgorithmType(Gameplay.SortingAlgorithm algorithm)
        {
            return algorithm switch
            {
                Gameplay.SortingAlgorithm.BubbleSort => "BUBBLE_SORT",
                Gameplay.SortingAlgorithm.SelectionSort => "SELECTION_SORT",
                _ => "UNKNOWN"
            };
        }
    }
}
