using System.Collections.Generic;

namespace Gameplay.SortingAlgorithms
{
    public static class BubbleSort
    {
        public static IEnumerable<(int, int)> Run(IList<int> values)
        {
            var swaps = new List<(int, int)>();

            for (var i = 0; i < values.Count - 1; i++)
            {
                for (var j = 0; j < values.Count - i - 1; j++)
                {
                    if (values[j] <= values[j + 1]) continue;

                    (values[j], values[j + 1]) = (values[j + 1], values[j]);

                    swaps.Add((j, j + 1));
                }
            }

            return swaps;
        }
    }
}
