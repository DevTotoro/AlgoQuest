using UnityEngine;
using Unity.Netcode;

namespace Actors.Containers
{
    public enum ContainerState
    {
        Empty,
        Full,
        Locked
    }
    
    public struct ContainerData : INetworkSerializable
    {
        public ContainerState State;
        public int Value;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref State);
            serializer.SerializeValue(ref Value);
        }
    }
    
    [RequireComponent(typeof(Collider))]
    public class ContainerController : NetworkBehaviour, Interfaces.IInteractive<ContainerData>
    {
        [Header("Data")]
        [SerializeField] private ContainerState initialState = ContainerState.Locked;
        [SerializeField] private int initialValue;
        
        [Header("References")]
        [SerializeField] private GameObject emptyContainer;
        [SerializeField] private GameObject fullContainer;
        [SerializeField] private GameObject lockedContainer;
        [SerializeField] private GameObject highlightContainer;
        [Space]
        [SerializeField] private TMPro.TextMeshProUGUI valueText;
        
        private readonly NetworkVariable<ContainerData> _networkData = new();
        
        public ContainerData Data => _networkData.Value;

        private void Awake()
        {
            _networkData.OnValueChanged += OnDataChanged;
        }
        
        // ====================
        
        public override void OnNetworkSpawn()
        {
            if (IsServer) _networkData.Value = new ContainerData { State = initialState, Value = initialValue };
            
            UpdateVisuals();
        }
        
        [Rpc(SendTo.Server)]
        private void SetDataRpc(ContainerData data, RpcParams rpcParams = default)
        {
            SendContainerDataUpdatedEventRpc(rpcParams.Receive.SenderClientId, _networkData.Value);
            
            _networkData.Value = data;
        }
        
        [Rpc(SendTo.Everyone)]
        private void SendContainerDataUpdatedEventRpc(ulong senderClientId, ContainerData data)
        {
            Events.EventManager.Singleton.ContainerEvents.EmitContainerDataUpdatedEvent(senderClientId, data);
        }
        
        // ====================
        
        public void Interact(ContainerData data)
        {
            if (_networkData.Value.State == ContainerState.Locked || data.State == ContainerState.Locked) return;
            
            ContainerData newData;

            switch (_networkData.Value.State)
            {
                case ContainerState.Empty: // Container empty
                    if (data.State == ContainerState.Empty) return;
                    
                    newData = new ContainerData { State = ContainerState.Full, Value = data.Value };
                    break;
                
                case ContainerState.Full: // Container full
                    switch (data.State)
                    {
                        case ContainerState.Empty: // Data empty
                            newData = new ContainerData { State = ContainerState.Empty, Value = 0 };
                            break;
                        
                        case ContainerState.Full: // Data full
                            newData = new ContainerData { State = ContainerState.Full, Value = data.Value };
                            break;
                        
                        case ContainerState.Locked: // Data locked
                        default:
                            return;
                    }
                    break;
                
                case ContainerState.Locked: // Container locked
                default:
                    return;
            }
            
            SetDataRpc(newData);
        }
        
        public void Highlight(bool highlight)
        {
            highlightContainer.SetActive(highlight);
        }
        
        // ====================
        
        private void OnDataChanged(ContainerData previousData, ContainerData newData)
        {
            UpdateVisuals();
        }
        
        // ====================
        
        private void UpdateVisuals()
        {
            emptyContainer.SetActive(_networkData.Value.State == ContainerState.Empty);
            fullContainer.SetActive(_networkData.Value.State == ContainerState.Full);
            lockedContainer.SetActive(_networkData.Value.State == ContainerState.Locked);

            if (_networkData.Value.State == ContainerState.Locked)
            {
                valueText.text = "locked";
                return;
            }
            
            valueText.text = _networkData.Value.Value.ToString();
        }
    }
}