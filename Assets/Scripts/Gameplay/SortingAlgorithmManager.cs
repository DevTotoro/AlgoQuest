using System.Collections.Generic;
using System.Linq;
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

    public struct ContainerAlgorithmState
    {
        public int Index { get; set; }
        public int TargetValue { get; set; }
        public int[] PossibleValues { get; set; }
    }
    
    public class SortingAlgorithmManager : NetworkBehaviour
    {
        [Header("Algorithm")]
        [SerializeField] private SortingAlgorithm algorithm;
        
        [Header("References")]
        [SerializeField] private ContainerController[] containers;
        
        private IList<(ContainerAlgorithmState, ContainerAlgorithmState)> _swaps;
        private int _swapIndex;
        private bool _firstContainerSwapSuccess;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            var values = new int[containers.Length];
            for (var i = 0; i < containers.Length; i++)
            {
                values[i] = containers[i].Data.Value;
                
                containers[i].AlgorithmIndex = i;
                containers[i].ContainerDataChangedEvent += OnContainerValueChanged;
            }

            _swaps = RunAlgorithm(values);
            _swapIndex = 0;
        }
        
        // ====================
        
        private void OnContainerValueChanged(int containerIndex, int value)
        {
            if (!IsServer) return;
            
            var (container1State, container2State) = _swaps[_swapIndex];
            
            if (containerIndex != container1State.Index && containerIndex != container2State.Index)
            {
                Debug.Log("Invalid container index - GAME OVER");
                
                return;
            }

            var targetValue = containerIndex == container1State.Index
                ? container1State.TargetValue
                : container2State.TargetValue;

            if (value == targetValue)
            {
                if (_firstContainerSwapSuccess)
                    NextSwap();
                else
                    _firstContainerSwapSuccess = true;

                return;
            }
            
            var possibleValues = containerIndex == container1State.Index
                ? container1State.PossibleValues
                : container2State.PossibleValues;
            
            if (!possibleValues.Contains(value))
            {
                Debug.Log("Invalid value - GAME OVER");
                
                return;
            }
            
            _firstContainerSwapSuccess = false;
        }
        
        // ====================

        private IList<(ContainerAlgorithmState, ContainerAlgorithmState)> RunAlgorithm(IList<int> values)
        {
            return algorithm switch
            {
                SortingAlgorithm.BubbleSort => BubbleSort.Run(values),
                SortingAlgorithm.SelectionSort => SelectionSort.Run(values),
                _ => new List<(ContainerAlgorithmState, ContainerAlgorithmState)>()
            };
        }

        private void NextSwap()
        {
            _swapIndex++;

            if (_swapIndex >= _swaps.Count)
            {
                Debug.Log("Algorithm completed");

                return;
            }
            
            _firstContainerSwapSuccess = false;
        }
    }
}
