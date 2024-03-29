using System.Collections.Generic;

namespace Gameplay.SortingAlgorithms
{
    public static class SelectionSort
    {
        public static IList<(ContainerAlgorithmState, ContainerAlgorithmState)> Run(IList<int> values)
        {
            var containerStates = new List<(ContainerAlgorithmState, ContainerAlgorithmState)>();
            
            for (var i = 0; i < values.Count - 1; i++)
            {
                var minIndex = i;
                for (var j = i + 1; j < values.Count; j++)
                {
                    if (values[j] >= values[minIndex]) continue;
                    
                    minIndex = j;
                }

                if (minIndex == i) continue;
                
                (values[i], values[minIndex]) = (values[minIndex], values[i]);
                
                var containerState1 = new ContainerAlgorithmState
                {
                    Index = i,
                    TargetValue = values[i],
                    PossibleValues = new[] { values[i], values[minIndex], 0 }
                };
                
                var containerState2 = new ContainerAlgorithmState
                {
                    Index = minIndex,
                    TargetValue = values[minIndex],
                    PossibleValues = new[] { values[i], values[minIndex], 0 }
                };
                
                containerStates.Add((containerState1, containerState2));
            }

            return containerStates;
        }
    }
}
