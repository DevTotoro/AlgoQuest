using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Actors.Containers;
using Gameplay.SortingAlgorithms;

namespace Gameplay
{
    public enum SortingAlgorithm
    {
        BubbleSort,
        SelectionSort
    }
    
    public class SortingAlgorithmManager : NetworkBehaviour
    {
        [Header("Algorithm")]
        [SerializeField] private SortingAlgorithm algorithm;
        
        [Header("References")]
        [SerializeField] private ContainerController[] containers;
        
        private IEnumerable<(int, int)> _swaps;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            var values = new int[containers.Length];
            for (var i = 0; i < containers.Length; i++)
                values[i] = containers[i].Data.Value;

            _swaps = RunAlgorithm(values);
            
            Debug.Log($"Swaps: {string.Join(", ", _swaps)}");
        }

        private IEnumerable<(int, int)> RunAlgorithm(IList<int> values)
        {
            return algorithm switch
            {
                SortingAlgorithm.BubbleSort => BubbleSort.Run(values),
                SortingAlgorithm.SelectionSort => SelectionSort.Run(values),
                _ => new List<(int, int)>()
            };
        }
    }
}
