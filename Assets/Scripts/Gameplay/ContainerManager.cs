using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Unity.Collections;
using Unity.Netcode;
using Actors.Containers;

namespace Gameplay
{
    public class ContainerManager : NetworkBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] [Range(0, 20)] private int spawnCount = 1;
        [SerializeField] private Vector3 spawnPosition = Vector3.zero;
        [SerializeField] private Vector3 spawnOffset = Vector3.zero;
        
        [Header("Container Settings")]
        [SerializeField] [Range(0, 10)] private int minContainerValue;
        [SerializeField] [Range(0, 50)] private int maxContainerValue = 20;
        
        [Header("References")]
        [SerializeField] private GameObject spawnPrefab;
        
        private GameObject[] _containers;

        private readonly NetworkList<ContainerData> _containerData = new();
        private readonly NetworkList<FixedString64Bytes> _sessionIds = new();
        
        public UnityAction<int, int> OnContainerValueChanged;
        public UnityAction<int> OnContainersSpawned;
        
        public int ContainerCount => _containerData.Count;

        private void Awake()
        {
            _containerData.OnListChanged += OnContainerDataChanged;
        }
        
        // ====================
        
        public override void OnNetworkSpawn()
        {
            SpawnContainers();
            
            if (IsServer)
                SetInitialContainerData();
        }

        [Rpc(SendTo.Server)]
        private void OnContainerInteractServerRpc(int containerIndex, int newValue, string sessionId,
            RpcParams rpcParams = default)
        {
            var containerData = _containerData[containerIndex];
            if (containerData.IsLocked) return;
            
            // Register the session ID if it's not already in the list
            if (!_sessionIds.Contains(sessionId))
                _sessionIds.Add(sessionId);
            
            OnContainerInteractClientRpc(rpcParams.Receive.SenderClientId, containerData.Value);
            
            _containerData[containerIndex] = new ContainerData
            {
                Value = newValue,
                IsLocked = false
            };
        }

        [Rpc(SendTo.Everyone)]
        private void OnContainerInteractClientRpc(ulong clientId, int containerValue, RpcParams rpcParams = default)
        {
            Events.EventManager.Singleton.GameplayEvents.EmitSetPlayerContainerValueEvent(clientId, containerValue);
        }
        
        // ====================
        
        public int[] GetContainerValues()
        {
            var values = new int[_containerData.Count];
            
            for (var i = 0; i < _containerData.Count; i++)
                values[i] = _containerData[i].Value;
            
            return values;
        }
        
        public void SetContainerLockState(int index, bool isLocked)
        {
            _containerData[index] = new ContainerData
            {
                Value = _containerData[index].Value,
                IsLocked = isLocked
            };
        }
        
        public void SetAllContainersLockState(bool isLocked, int[] excludeIndexes = null)
        {
            for (var i = 0; i < _containerData.Count; i++)
            {
                if (excludeIndexes != null && excludeIndexes.Contains(i)) continue;
                
                SetContainerLockState(i, isLocked);
            }
        }
        
        public string[] GetSessionIds()
        {
            var sessionIds = new string[_sessionIds.Count];
            
            for (var i = 0; i < _sessionIds.Count; i++)
                sessionIds[i] = _sessionIds[i].ToString();
            
            return sessionIds;
        }

        public void ResetContainers()
        {
            for (var i = 0; i < _containerData.Count; i++)
            {
                _containerData[i] = new ContainerData
                {
                    Value = Random.Range(minContainerValue, maxContainerValue),
                    IsLocked = true
                };
            }
        }
        
        // ====================
        
        // Called when the list of container data changes on all clients
        private void OnContainerDataChanged(NetworkListEvent<ContainerData> networkListEvent)
        {
            UpdateContainerVisuals(networkListEvent.Index, networkListEvent.Value);
            
            OnContainerValueChanged?.Invoke(networkListEvent.Index, networkListEvent.Value.Value);
            
            if (IsServer && networkListEvent.Index == _containerData.Count - 1)
                OnContainersSpawned?.Invoke(_containerData.Count);
        }
        
        // ====================
        
        private void SpawnContainers()
        {
            _containers = new GameObject[spawnCount];
            
            for (var i = 0; i < spawnCount; i++)
            {
                var containerSpawnPosition = spawnPosition + spawnOffset * i;

                _containers[i] = Instantiate(spawnPrefab, containerSpawnPosition, Quaternion.identity, transform);
                
                var containerController = _containers[i].GetComponent<ContainerController>();
                
                containerController.Index = i;
                containerController.OnInteract += (index, value, sessionId) =>
                    OnContainerInteractServerRpc(index, value, sessionId);

                if (_containerData.Count > i) // True on clients, sets initial data
                    UpdateContainerVisuals(i, _containerData[i]);
            }
        }

        private void UpdateContainerVisuals(int index, ContainerData data)
        {
            var containerController = _containers[index].GetComponent<ContainerController>();
            
            containerController.ToggleLock(data.IsLocked);
            containerController.SetText(data.IsLocked ? "locked" :
                data.Value == -1 ? "null" : data.Value.ToString());
        }
        
        private void SetInitialContainerData()
        {
            for (var i = 0; i < spawnCount; i++)
            {
                _containerData.Add(new ContainerData
                {
                    Value = Random.Range(minContainerValue, maxContainerValue),
                    IsLocked = true
                });
            }
        }
        
        // ====================
        
        private void OnDrawGizmosSelected()
        {
            for (var i = 0; i < spawnCount; i++)
                Gizmos.DrawWireSphere(spawnPosition + spawnOffset * i, 0.3f);
        }
    }
}
