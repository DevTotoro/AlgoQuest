using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;
using Gameplay.SortingAlgorithms;
using Actors.Containers;
using Events;

namespace Gameplay
{
    public enum SortingAlgorithm
    {
        BubbleSort,
        SelectionSort
    }

    public enum SortingAlgorithmMode
     {
         Guided,
         TimeTrial
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
         [SerializeField] private SortingAlgorithmMode mode;

         [Header("Spawn Config")]
         [SerializeField] private Vector3 spawnPosition = Vector3.zero;
         [SerializeField] [Range(0.0f, 10.0f)] private float spacing = 3.0f;
         
         [Header("References")]
         [SerializeField] private GameObject spawnPrefab;
         
         private GameObject[] _containers;
         private bool _firstContainerSwapSuccess;
         private readonly IList<string> _sessionIds = new List<string>();
         
         private IList<(ContainerAlgorithmState, ContainerAlgorithmState)> _swaps;
         private int _swapIndex, _minVal, _maxVal;
         
         private readonly Core.Timer _timer = new();

         private ContainerController[] ContainerControllers =>
             _containers.Select(container => container.GetComponent<ContainerController>()).ToArray();
         
         private readonly NetworkVariable<FixedString4096Bytes> _highScores = new();
         
         // ====================

         private void Awake()
         {
             EventManager.Singleton.GameplayEvents.RequestRestartEvent += RequestRestartRpc;
             
             EventManager.Singleton.UIEvents.CloseApplicationEvent += OnApplicationClose;
             
             EventManager.Singleton.LogEvents.NetworkLogEvent += NetworkLogRpc;
         }
         
         private void Update()
         {
             if (!IsServer) return;

             if (_timer.Run())
                 TimerUpdatedRpc(_timer.TimeString);
         }
         
         // ====================
         
         public override void OnNetworkSpawn()
         {
             if (!IsServer)
             {
                 if (mode == SortingAlgorithmMode.TimeTrial)
                     EventManager.Singleton.UIEvents.EmitToggleTimerEvent(true);
                 
                 if (_highScores.Value.Length != 0)
                     SetHighScoresRpc(_highScores.Value.ToString());
             }
             
             if (IsServer)
                 FetchAlgorithmConfig();
         }
         
         public override void OnNetworkDespawn()
         {
             // Despawn all containers when server shuts down
             if (IsServer && _containers != null)
             {
                 foreach (var container in _containers)
                 {
                     container.TryGetComponent<NetworkObject>(out var networkObject);
                     
                     if (networkObject != null && networkObject.IsSpawned)
                         networkObject.Despawn();
                 }
             }
             
             base.OnNetworkDespawn();
         }

         [Rpc(SendTo.Everyone)]
         private void TimerUpdatedRpc(string time)
         {
             EventManager.Singleton.UIEvents.EmitSetTimerEvent(time);
         }

         [Rpc(SendTo.Everyone)]
         private void SetHighScoresRpc(string highScores)
         {
             EventManager.Singleton.UIEvents.EmitToggleLeaderboardEvent(true);
             EventManager.Singleton.UIEvents.EmitSetLeaderboardEvent(highScores);
         }
         
         [Rpc(SendTo.Everyone)]
         private void GameOverRpc()
         {
             EventManager.Singleton.GameplayEvents.EmitGameOverEvent();
             
             EventManager.Singleton.LogEvents.EmitNetworkLogEvent(Events.LogType.GAME_OVER);
         }
         
         [Rpc(SendTo.Everyone)]
         private void GameWonRpc()
         {
             EventManager.Singleton.GameplayEvents.EmitGameWonEvent();
             
             EventManager.Singleton.LogEvents.EmitNetworkLogEvent(Events.LogType.GAME_WON);
         }
         
         [Rpc(SendTo.Server)]
         private void RequestRestartRpc()
         {
             Restart();
             
             RestartRpc();
         }
         
         [Rpc(SendTo.Everyone)]
         private void RestartRpc()
         {
             EventManager.Singleton.GameplayEvents.EmitRestartEvent();
             EventManager.Singleton.UIEvents.EmitSetTimerEvent("00:00:00");
         }

         [Rpc(SendTo.Server)]
         private void NetworkLogRpc(string sessionId, Events.LogType type)
         {
             Log(sessionId, type);
         }
         
         // ====================
         
         private void OnAlgorithmConfigFetched(int spawnCount, int minVal, int maxVal)
         {
             if (!IsServer) return;

             _minVal = minVal;
             _maxVal = maxVal;
             
             SpawnContainers(spawnCount);
             InitializeContainers();
         }

         private void OnContainerInitialized()
         {
             if (!IsServer) return;

             if (ContainerControllers.All(c => c.IsInitialized))
                 InitializeAlgorithm();
         }
         
         private async void OnContainerInteracted(string sessionId, int index, int prevValue, int newValue)
         {
             if (!IsServer) return;
             
             if (!_sessionIds.Contains(sessionId))
                 _sessionIds.Add(sessionId);

             if (mode == SortingAlgorithmMode.TimeTrial && !_timer.IsRunning)
             {
                 EventManager.Singleton.LogEvents.EmitNetworkLogEvent(Events.LogType.GAME_STARTED);
                 
                 _timer.Start();
             }
             
             var moveValid = IsMoveValid(index, newValue);
             
             await LogContainerInteraction(sessionId, index, prevValue, newValue, moveValid);
             
             if (!moveValid)
             {
                 GameOverRpc();
                 
                 _timer.Stop();
                 
                 return;
             }
             
             var moveCorrect = IsMoveCorrect(index, newValue);
             if (moveCorrect)
             {
                 if (_firstContainerSwapSuccess)
                     await NextSwap();
                 else
                     _firstContainerSwapSuccess = true;

                 return;
             }
             
             _firstContainerSwapSuccess = false;
         }

         private void OnApplicationClose()
         {
             EventManager.Singleton.LogEvents.EmitNetworkLogEvent(IsServer
                 ? Events.LogType.HOST_CLOSED
                 : Events.LogType.HOST_LEFT);
         }
         
         // ====================
         
         private void SpawnContainers(int spawnCount)
         {
             if (!IsServer) return;
             
             _containers = new GameObject[spawnCount];

             var firstContainerPosition = spawnPosition - new Vector3(spacing * (spawnCount - 1) / 2, 0, 0);
             
             for (var i = 0; i < spawnCount; i++)
             {
                 var containerSpawnPosition = firstContainerPosition + new Vector3(i * spacing, 0, 0);

                 _containers[i] = Instantiate(spawnPrefab, containerSpawnPosition, Quaternion.identity, transform);

                 var networkObject = _containers[i].GetComponent<NetworkObject>();
                 networkObject.Spawn();

                 var containerController = _containers[i].GetComponent<ContainerController>();
                 containerController.Index = i;
                 containerController.OnInitialized += OnContainerInitialized;
                 containerController.OnInteracted += OnContainerInteracted;
             }
         }
         
         private void InitializeContainers()
         {
             if (!IsServer) return;
             
             var values = Core.Helpers.GetUnsortedRandomArray(ContainerControllers.Length, _minVal, _maxVal);
             
             for (var i = 0; i < ContainerControllers.Length; i++)
                 ContainerControllers[i].Initialize(values[i], true);
         }
         
         private void InitializeAlgorithm()
         {
             if (!IsServer) return;

             var values = ContainerControllers.Select(container => container.Value).ToArray();

             _swaps = algorithm switch
             {
                 SortingAlgorithm.BubbleSort => BubbleSort.Run(values),
                 SortingAlgorithm.SelectionSort => SelectionSort.Run(values),
                 _ => throw new System.NotImplementedException()
             };

             switch (mode)
             {
                 case SortingAlgorithmMode.Guided:
                     InitializeGuidedMode();
                     break;
                 
                 case SortingAlgorithmMode.TimeTrial:
                     InitializeTimeTrialMode();
                     break;
                 
                 default:
                     throw new System.NotImplementedException();
             }
         }

         private void InitializeGuidedMode()
         {
             UnlockCurrentContainers();
             
             EventManager.Singleton.LogEvents.EmitNetworkLogEvent(Events.LogType.GAME_STARTED);
         }
         
         private void InitializeTimeTrialMode()
         {
             FetchHighScores(_containers.Length);
             
             EventManager.Singleton.UIEvents.EmitToggleTimerEvent(true);
             
             foreach (var container in ContainerControllers)
                 container.SetIsLocked(false);
         }

         private void UnlockCurrentContainers()
         {
             if (!IsServer || _swapIndex >= _swaps.Count) return;
             
             var (container1State, container2State) = _swaps[_swapIndex];
             
             var containers = ContainerControllers;
             
             for (var i = 0; i < containers.Length; i++)
                 containers[i].SetIsLocked(!(i == container1State.Index || i == container2State.Index));
         }
         
         private async Task NextSwap()
         {
             _swapIndex++;
             
             if (mode == SortingAlgorithmMode.Guided)
                 UnlockCurrentContainers();
         
             if (_swapIndex >= _swaps.Count)
             {
                 GameWonRpc();
                 
                 if (mode != SortingAlgorithmMode.TimeTrial) return;

                 _timer.Stop();
                 
                 var data = new AlgoQuestServices.TimeTrials.CreateTimeTrialPayload
                 {
                     type = algorithm,
                     time = _timer.TimeElapsedInMs,
                     numberOfValues = _containers.Length,
                     requiredMoves = _swaps.Count,
                     sessions = _sessionIds.ToArray()
                 };
                 await AlgoQuestServices.TimeTrials.Create(data);
             
                 FetchHighScores(_containers.Length);
                 
                 return;
             }
             
             _firstContainerSwapSuccess = false;
         }
         
         private void Restart()
         {
             if (!IsServer) return;

             _swapIndex = 0;
             
             foreach (var container in ContainerControllers)
                 container.Reset();
             
             _timer.Reset();
             
             InitializeContainers();
             
             EventManager.Singleton.LogEvents.EmitNetworkLogEvent(Events.LogType.GAME_RESTARTED);
         }
         
         private bool IsMoveValid(int index, int value)
         {
             if (mode != SortingAlgorithmMode.TimeTrial) return true;
             
             var (container1State, container2State) = _swaps[_swapIndex];

             var indexValid = index == container1State.Index || index == container2State.Index;
             if (!indexValid) return false;
             
             var possibleValues = index == container1State.Index
                 ? container1State.PossibleValues
                 : container2State.PossibleValues;
             
             return possibleValues.Contains(value);
         }
         
         private bool IsMoveCorrect(int index, int value)
         {
             var (container1State, container2State) = _swaps[_swapIndex];

             var indexCorrect = index == container1State.Index || index == container2State.Index;
             if (!indexCorrect) return false;

             return value ==
                    (index == container1State.Index ? container1State.TargetValue : container2State.TargetValue);
         }
         
         // ====================
         
         private async void FetchAlgorithmConfig()
         {
             if (!IsServer) return;
             
             var res = await AlgoQuestServices.Algorithms.GetByType(algorithm);
             
             if (!res.Success)
             {
                 Debug.LogError("Failed to fetch algorithm config");
                 return;
             }

             OnAlgorithmConfigFetched(res.Data.numberOfValues, res.Data.minValue, res.Data.maxValue);
         }
         
         private async void FetchHighScores(int numberOfValues)
         {
             if (!IsServer) return;
             
             var queryParams = new AlgoQuestServices.TimeTrials.GetTimeTrialsQueryParams
             {
                 type = algorithm,
                 numberOfValues = numberOfValues,
                 requiredMoves = _swaps.Count,
             };

             var res = await AlgoQuestServices.TimeTrials.Get(queryParams);
             
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
             
             SetHighScoresRpc(highScoresString);
         }

         private async Task LogContainerInteraction(string sessionId, int index, int containerPrevValue,
             int containerNewValue, bool isValid)
         {
             if (!IsServer) return;

             var data = new AlgoQuestServices.ContainerInteractions.CreateContainerInteractionPayload
             {
                 containerIndex = index,
                 receivedValue = containerPrevValue,
                 placedValue = containerNewValue,
                 isValid = isValid,
                 containerValues = ContainerControllers.Select((container, i) => i == index ? containerNewValue : container.Value).ToArray(),
                 algorithmType = algorithm,
                 gameMode = mode,
                 sessionId = sessionId
             };

             await AlgoQuestServices.ContainerInteractions.Create(data);
         }

         private async void Log(string sessionId, Events.LogType type)
         {
             if (!IsServer) return;
             
             var containers = _containers?.Length > 0
                 ? ContainerControllers.Select(container => container.Value).ToArray()
                 : null;
             
            var data = new AlgoQuestServices.Logs.CreateLogPayload
            {
                type = type,
                
                algorithmType = algorithm,
                gameMode = mode,
                containerValues = containers,
                
                sessionId = sessionId,
                
                nullContainerValues = containers == null
            };
            
            await AlgoQuestServices.Logs.Create(data);
         }
     }
}
