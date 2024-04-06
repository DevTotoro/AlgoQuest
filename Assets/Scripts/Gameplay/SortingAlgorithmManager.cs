using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Actors.Containers;
using Gameplay.SortingAlgorithms;
using Unity.Collections;

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

        private readonly Core.Timer _timer = new();
        
        private readonly NetworkList<FixedString64Bytes> _sessionIds = new();
        private readonly NetworkVariable<FixedString4096Bytes> _highScores = new();

        private void Awake()
        {
            Events.EventManager.Singleton.GameplayEvents.RetryEvent += OnRetryEvent;
            Events.EventManager.Singleton.GameplayEvents.RequestRetryEvent += OnRequestRetryEvent;

            Events.EventManager.Singleton.ContainerEvents.UserInteractedWithContainerEvent +=
                OnUserInteractedWithContainerEvent;
        }

        private void Update()
        {
            if (!IsServer) return;

            if (_timer.Run())
                SendTimerUpdatedEventRpc(_timer.TimeString);
        }
        
        // ====================
        
        public override void OnNetworkSpawn()
        {
            if (_highScores.Value.Length != 0)
                Events.EventManager.Singleton.GameplayEvents.EmitHighScoresFetchedEvent(_highScores.Value.ToString());
            
            if (!IsServer) return;
            
            GetHighScores();
            
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
            
            _timer.Start();
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

        [Rpc(SendTo.Everyone)]
        private void SendTimerUpdatedEventRpc(string time)
        {
            Events.EventManager.Singleton.GameplayEvents.EmitTimerUpdatedEvent(time);
        }

        [Rpc(SendTo.Server)]
        private void RegisterSessionIdRpc(string sessionId)
        {
            if (_sessionIds.Contains(sessionId)) return;
            
            _sessionIds.Add(sessionId);
            
            Debug.Log($"Session ID registered: {sessionId}");
        }
        
        // ====================
        
        private async void OnContainerValueChanged(int containerIndex, int value)
        {
            if (!IsServer) return;
            
            var (container1State, container2State) = _swaps[_swapIndex];
            
            if (containerIndex != container1State.Index && containerIndex != container2State.Index)
            {
                SendGameOverEventRpc();
                
                _timer.Stop();

                await AlgoQuestServices.Algorithms.Create(algorithm,
                    AlgoQuestServices.Algorithms.AlgorithmCompletionStatus.Failure, _timer.TimeElapsedInMs,
                    GetSessionIds());
                
                return;
            }

            var targetValue = containerIndex == container1State.Index
                ? container1State.TargetValue
                : container2State.TargetValue;

            if (value == targetValue)
            {
                if (_firstContainerSwapSuccess)
                    await NextSwap();
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
                
                _timer.Stop();

                await AlgoQuestServices.Algorithms.Create(algorithm,
                    AlgoQuestServices.Algorithms.AlgorithmCompletionStatus.Failure, _timer.TimeElapsedInMs,
                    GetSessionIds());
                
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
            
            _timer.Reset();
            _timer.Start();
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
        
        private void OnUserInteractedWithContainerEvent(string sessionId)
        {
            RegisterSessionIdRpc(sessionId);
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

        private async Task NextSwap()
        {
            _swapIndex++;

            if (_swapIndex >= _swaps.Count)
            {
                Debug.Log("Algorithm completed");
                
                _timer.Stop();
                
                Debug.Log($"Time elapsed: {_timer.TimeString}");

                await AlgoQuestServices.Algorithms.Create(algorithm,
                    AlgoQuestServices.Algorithms.AlgorithmCompletionStatus.Success, _timer.TimeElapsedInMs,
                    GetSessionIds());

                return;
            }
            
            _firstContainerSwapSuccess = false;
        }
        
        private bool CheckContainersSpawned()
        {
            return containers.All(container => container.IsSpawned);
        }
        
        private string[] GetSessionIds()
        {
            var sessionIds = new string[_sessionIds.Count];
            
            for (var i = 0; i < _sessionIds.Count; i++)
                sessionIds[i] = _sessionIds[i].ToString();
            
            return sessionIds;
        }
        
        private async void GetHighScores()
        {
            var res = await AlgoQuestServices.Algorithms.GetHighScores(algorithm);
            
            if (!res.Success)
            {
                Debug.LogError("Failed to get high scores");
                return;
            }
            
            var highScores = res.Data;
            
            var highScoresString = "";
            
            foreach (var highScore in highScores)
            {
                var time = Core.Timer.GetTimeString(highScore.time);

                highScoresString +=
                    $"- {time} | {string.Join(", ", highScore.sessions.Select(session => session.username))}\n";
            }
            
            _highScores.Value = highScoresString;
            
            Events.EventManager.Singleton.GameplayEvents.EmitHighScoresFetchedEvent(highScoresString);
        }
    }
}
