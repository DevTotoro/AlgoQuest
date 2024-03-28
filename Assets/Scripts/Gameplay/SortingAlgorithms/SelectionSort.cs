using System.Collections.Generic;

namespace Gameplay.SortingAlgorithms
{
    public static class SelectionSort
    {
        public static IEnumerable<(int, int)> Run(IList<int> values)
        {
            var swaps = new List<(int, int)>();
            
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
                
                swaps.Add((i, minIndex));
            }

            return swaps;
        }
    }
}
