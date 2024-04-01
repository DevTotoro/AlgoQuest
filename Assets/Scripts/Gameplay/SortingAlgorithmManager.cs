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

        private void Awake()
        {
            Events.EventManager.Singleton.GameplayEvents.RetryEvent += OnRetryEvent;
            Events.EventManager.Singleton.GameplayEvents.RequestRetryEvent += OnRequestRetryEvent;
        }
        
        // ====================

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            
            if (CheckContainersSpawned())
                InitializeRpc();
            else
                Events.EventManager.Singleton.ContainerEvents.ContainerSpawnedEvent += OnContainerSpawnedEvent;
        }
        
        [Rpc(SendTo.Server)]
        private void InitializeRpc()
        {
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
        
        [Rpc(SendTo.Everyone)]
        private void SendGameOverEventRpc()
        {
            Events.EventManager.Singleton.GameplayEvents.EmitGameOverEvent();
        }
        
        [Rpc(SendTo.Server)]
        private void SendRequestRetryEventRpc()
        {
            SendRetryEventRpc();
        }
        
        [Rpc(SendTo.Everyone)]
        private void SendRetryEventRpc()
        {
            Events.EventManager.Singleton.GameplayEvents.EmitRetryEvent();
        }
        
        // ====================
        
        private void OnContainerValueChanged(int containerIndex, int value)
        {
            if (!IsServer) return;
            
            var (container1State, container2State) = _swaps[_swapIndex];
            
            if (containerIndex != container1State.Index && containerIndex != container2State.Index)
            {
                SendGameOverEventRpc();
                
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
                SendGameOverEventRpc();
                
                return;
            }
            
            _firstContainerSwapSuccess = false;
        }

        private void OnRetryEvent()
        {
            if (!IsServer) return;
            
            _swapIndex = 0;
            
            foreach (var container in containers)
                container.Reset();
        }
        
        private void OnRequestRetryEvent()
        {
            SendRequestRetryEventRpc();
        }
        
        private void OnContainerSpawnedEvent()
        {
            if (!IsServer) return;
            
            if (!CheckContainersSpawned()) return;
            
            Events.EventManager.Singleton.ContainerEvents.ContainerSpawnedEvent -= OnContainerSpawnedEvent;
            
            InitializeRpc();
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
        
        private bool CheckContainersSpawned()
        {
            return containers.All(container => container.IsSpawned);
        }
    }
}
