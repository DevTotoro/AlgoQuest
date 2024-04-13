using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using Gameplay.SortingAlgorithms;

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
     
     [RequireComponent(typeof(ContainerManager))]
     public class SortingAlgorithmManager : NetworkBehaviour
     {
         [Header("Algorithm")]
         [SerializeField] private SortingAlgorithm algorithm;
         [SerializeField] private SortingAlgorithmMode mode;
         
         private IList<(ContainerAlgorithmState, ContainerAlgorithmState)> _swaps;
         private int _swapIndex;
         private bool _firstContainerSwapSuccess, _shouldReceiveContainerValueChanged;

         private readonly Core.Timer _timer = new();
         
         private readonly NetworkVariable<FixedString4096Bytes> _highScores = new();
         
         private ContainerManager _containerManager;

         private void Awake()
         {
             Events.EventManager.Singleton.GameplayEvents.RequestRestartEvent += RequestRestartRpc;
             
             _containerManager = GetComponent<ContainerManager>();
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
             if (mode == SortingAlgorithmMode.TimeTrial)
             {
                 Events.EventManager.Singleton.UIEvents.EmitToggleLeaderboardEvent(true);
                 Events.EventManager.Singleton.UIEvents.EmitToggleTimerEvent(true);
                 
                 if (_highScores.Value.Length != 0)
                     SendHighScoresFetchedEventRpc(_highScores.Value.ToString());
             }
             
             if (!IsServer) return;
             
             _containerManager.OnContainerValueChanged += OnContainerValueChanged;
             
             if (mode == SortingAlgorithmMode.TimeTrial)
                _containerManager.OnContainersSpawned += GetHighScores;
             
             Initialize();
         }
         
         [Rpc(SendTo.Everyone)]
         private void SendTimerUpdatedEventRpc(string time)
         {
             Events.EventManager.Singleton.UIEvents.EmitSetTimerEvent(time);
         }
         
         [Rpc(SendTo.Everyone)]
         private void SendHighScoresFetchedEventRpc(string highScores)
         {
             Events.EventManager.Singleton.UIEvents.EmitSetLeaderboardEvent(highScores);
         }
         
         [Rpc(SendTo.Everyone)]
         private void GameOverRpc()
         {
             Events.EventManager.Singleton.GameplayEvents.EmitGameOverEvent();
         }
         
         [Rpc(SendTo.Everyone)]
         private void GameWonRpc()
         {
             Events.EventManager.Singleton.GameplayEvents.EmitGameWonEvent();
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
             Events.EventManager.Singleton.GameplayEvents.EmitRestartEvent();
         }
         
         // ====================
         
         private async void OnContainerValueChanged(int containerIndex, int value)
         {
             if (!IsServer || !_shouldReceiveContainerValueChanged) return;
             
             var (container1State, container2State) = _swaps[_swapIndex];

             if (mode == SortingAlgorithmMode.TimeTrial && containerIndex != container1State.Index &&
                 containerIndex != container2State.Index)
             {
                 GameOverRpc();
                 
                 _timer.Stop();
                 
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
             
             if (mode == SortingAlgorithmMode.TimeTrial && !possibleValues.Contains(value))
             {
                 GameOverRpc();
                 
                 _timer.Stop();
                 
                 return;
             }
             
             _firstContainerSwapSuccess = false;
         }
         
         // ====================
         
         private void Initialize()
         {
             _shouldReceiveContainerValueChanged = false;
             
             var values = _containerManager.GetContainerValues();
             
             _swaps = RunAlgorithm(values);

             switch (mode)
             {
                 case SortingAlgorithmMode.Guided:
                     UnlockCurrentContainers();
                     break;
                 
                 case SortingAlgorithmMode.TimeTrial:
                     _containerManager.SetAllContainersLockState(false);

                     _timer.Start();
                     
                     break;
                 
                 default:
                     Debug.LogError("Invalid sorting algorithm mode");
                     return;
             }
             
             _shouldReceiveContainerValueChanged = true;
         }
         
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
             _shouldReceiveContainerValueChanged = false;
             
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
                     numberOfValues = _containerManager.ContainerCount,
                     requiredMoves = _swaps.Count,
                     sessions = _containerManager.GetSessionIds()
                 };
                 await AlgoQuestServices.TimeTrials.Create(data);
             
                 GetHighScores(_containerManager.ContainerCount);
                 
                 return;
             }
             
             _firstContainerSwapSuccess = false;
             
             _shouldReceiveContainerValueChanged = true;
         }

         private void UnlockCurrentContainers()
         {
             if (_swapIndex >= _swaps.Count) return;
             
             var (container1State, container2State) = _swaps[_swapIndex];
             
             _containerManager.SetContainerLockState(container1State.Index, false);
             _containerManager.SetContainerLockState(container2State.Index, false);

             _containerManager.SetAllContainersLockState(true, new[] { container1State.Index, container2State.Index });
         }
         
         private void Restart()
         {
             if (!IsServer) return;
             
             _shouldReceiveContainerValueChanged = false;
             
             _swapIndex = 0;
             
             _containerManager.ResetContainers();
             
             _timer.Reset();
             
             Initialize();
             
             _shouldReceiveContainerValueChanged = true;
         }
         
         private async void GetHighScores(int numberOfValues)
         {
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
             
             SendHighScoresFetchedEventRpc(highScoresString);
         }
     }
}
