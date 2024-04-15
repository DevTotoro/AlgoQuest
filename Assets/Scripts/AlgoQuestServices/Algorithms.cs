using System.Threading.Tasks;

namespace AlgoQuestServices
{
    public static class Algorithms
    {
        public struct GetAlgorithmResponsePayload
        {
            public string id;
            public string type;
            public int numberOfValues;
            public int minValue;
            public int maxValue;
        };

        public static async Task<HttpResponse<GetAlgorithmResponsePayload>> GetByType(Gameplay.SortingAlgorithm type)
        {
            var algorithmType = GetAlgorithmType(type);

            return await Http.Get<GetAlgorithmResponsePayload>($"algorithms/{algorithmType}");
        }
        
        // ====================
        
        public static string GetAlgorithmType(Gameplay.SortingAlgorithm algorithm)
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