using UnityEngine;

namespace Actors.Containers
{
    public enum ContainerState
    {
        Empty,
        Full,
        Locked
    }
    
    public struct ContainerData
    {
        public ContainerState State;
        public int Value;
    }
    
    [RequireComponent(typeof(Collider))]
    public class ContainerController : MonoBehaviour, Interfaces.IInteractive<ContainerData>
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

        private ContainerData _data;

        private void Awake()
        {
            _data = new ContainerData { State = initialState, Value = initialValue };
            
            UpdateVisuals();
        }
        
        // ====================
        
        public void Interact(ContainerData data)
        {
            Debug.Log($"Interacting with container: {data.State} {data.Value}");
        }
        
        public void Highlight(bool highlight)
        {
            highlightContainer.SetActive(highlight);
        }
        
        // ====================
        
        private void UpdateVisuals()
        {
            emptyContainer.SetActive(_data.State == ContainerState.Empty);
            fullContainer.SetActive(_data.State == ContainerState.Full);
            lockedContainer.SetActive(_data.State == ContainerState.Locked);

            if (_data.State == ContainerState.Locked)
            {
                valueText.text = "locked";
                return;
            }
            
            valueText.text = _data.Value.ToString();
        }
    }
}