using System.Collections.Generic;

namespace Gameplay.SortingAlgorithms
{
    public static class BubbleSort
    {
        public static IList<(ContainerAlgorithmState, ContainerAlgorithmState)> Run(IList<int> values)
        {
            var containerStates = new List<(ContainerAlgorithmState, ContainerAlgorithmState)>();

            for (var i = 0; i < values.Count - 1; i++)
            {
                for (var j = 0; j < values.Count - i - 1; j++)
                {
                    if (values[j] <= values[j + 1]) continue;

                    (values[j], values[j + 1]) = (values[j + 1], values[j]);

                    var containerState1 = new ContainerAlgorithmState
                    {
                        Index = j,
                        TargetValue = values[j],
                        PossibleValues = new[] { values[j], values[j + 1], -1 }
                    };
                    
                    var containerState2 = new ContainerAlgorithmState
                    {
                        Index = j + 1,
                        TargetValue = values[j + 1],
                        PossibleValues = new[] { values[j], values[j + 1], -1 }
                    };
                    
                    containerStates.Add((containerState1, containerState2));
                }
            }

            return containerStates;
        }
    }
}
