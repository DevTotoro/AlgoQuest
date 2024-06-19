using System.Threading.Tasks;

namespace AlgoQuestServices
{
    public static class ContainerInteractions
    {
        public struct CreateContainerInteractionPayload
        {
            public int containerIndex;
            public int receivedValue;
            public int placedValue;
            public bool isValid;
            public int[] containerValues;
            public Gameplay.SortingAlgorithm algorithmType;
            public Gameplay.SortingAlgorithmMode gameMode;
            public string sessionId;
        }

        public static async Task Create(CreateContainerInteractionPayload data)
        {
            var dataInternal = new CreateContainerInteractionPayloadInternal
            {
                containerIndex = data.containerIndex,
                receivedValue = data.receivedValue,
                placedValue = data.placedValue,
                isValid = data.isValid,
                containerValues = data.containerValues,
                algorithmType = Algorithms.GetAlgorithmType(data.algorithmType),
                gameMode = Algorithms.GetAlgorithmMode(data.gameMode),
                sessionId = data.sessionId
            };
            
            await CreateInternal(dataInternal);
        }

        // ====================
        
        private struct CreateContainerInteractionPayloadInternal
        {
            public int containerIndex;
            public int receivedValue;
            public int placedValue;
            public bool isValid;
            public int[] containerValues;
            public string algorithmType;
            public string gameMode;
            public string sessionId;
        }
        
        private static async Task CreateInternal(CreateContainerInteractionPayloadInternal data)
        {
            await Http.Post<object, CreateContainerInteractionPayloadInternal>("container-interactions", data);
        }
    }
}
