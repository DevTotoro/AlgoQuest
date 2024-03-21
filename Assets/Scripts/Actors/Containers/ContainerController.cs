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
        [Header("Visuals")]
        [SerializeField] private GameObject emptyContainer;
        [SerializeField] private GameObject fullContainer;
        [SerializeField] private GameObject lockedContainer;
        [SerializeField] private GameObject highlightContainer;
        [Space]
        [SerializeField] private TMPro.TextMeshProUGUI valueText;
        
        [Header("Data")]
        [SerializeField] private ContainerState initialState = ContainerState.Locked;
        [SerializeField] private int initialValue;

        private NetworkVariable<ContainerData> _networkData = new();

        private void Awake()
        {
            if (IsServer)
                _networkData = new NetworkVariable<ContainerData>(new ContainerData
                    { State = initialState, Value = initialValue });
            
            _networkData.OnValueChanged += OnDataChanged;
        }
        
        // ====================
        
        public override void OnNetworkSpawn()
        {
            UpdateVisuals();
        }
        
        [Rpc(SendTo.Server)]
        private void SetDataRpc(ContainerData data)
        {
            _networkData.Value = data;
        }
        
        // ====================
        
        public void Interact(ContainerData data)
        {
            SetDataRpc(data);
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