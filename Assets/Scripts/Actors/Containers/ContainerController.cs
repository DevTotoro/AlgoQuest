using Events;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

namespace Actors.Containers
{
    [RequireComponent(typeof(Collider))]
    public class ContainerController : NetworkBehaviour, Interfaces.IInteractive
    {
        [Header("References")]
        [SerializeField] private GameObject lockedContainer;
        [SerializeField] private GameObject unlockedContainer;
        [SerializeField] private GameObject highlightContainer;
        [Space]
        [SerializeField] private TMPro.TextMeshProUGUI valueText;
        
        private bool _isValueInitialized, _isLockedInitialized;
        
        private readonly NetworkVariable<int> _value = new(-1);
        private readonly NetworkVariable<bool> _isLocked = new();

        public bool IsInitialized => _isValueInitialized && _isLockedInitialized;
        public int Value => _value.Value;
        public int Index { private get; set; }
        
        public event UnityAction OnInitialized;
        public event UnityAction<int, int> OnInteracted;

        private void Awake()
        {
            _value.OnValueChanged += OnValueChanged;
            _isLocked.OnValueChanged += OnIsLockedChanged;
        }
        
        // ====================

        public override void OnNetworkSpawn()
        {
            if (IsServer) return;
            
            UpdateVisuals();
        }

        [Rpc(SendTo.Server)]
        private void InteractRpc(string sessionId, int value, RpcParams rpcParams = default)
        {
            EventManager.Singleton.PlayerEvents.EmitRegisterSessionIdEvent(sessionId);

            OnContainerInteractRpc(rpcParams.Receive.SenderClientId, Value);

            OnInteracted?.Invoke(Index, value);
            
            _value.Value = value;
        }

        [Rpc(SendTo.Everyone)]
        private void OnContainerInteractRpc(ulong clientId, int value)
        {
            EventManager.Singleton.GameplayEvents.EmitSetPlayerContainerValueEvent(clientId, value);
        }
        
        // ====================
        
        private void OnValueChanged(int oldValue, int newValue)
        {
            if (IsServer && !_isValueInitialized)
            {
                _isValueInitialized = true;

                if (_isValueInitialized && _isLockedInitialized)
                    OnInitialized?.Invoke();
            }
            
            UpdateVisuals();
        }
        
        private void OnIsLockedChanged(bool oldIsLocked, bool newIsLocked)
        {
            if (IsServer && !_isLockedInitialized)
            {
                _isLockedInitialized = true;
                
                if (_isValueInitialized && _isLockedInitialized)
                    OnInitialized?.Invoke();
            }
            
            UpdateVisuals();
        }
        
        // ====================
        
        public void Initialize(int value, bool isLocked)
        {
            if (!IsServer) return;
            
            if (_value.Value == value)
                OnValueChanged(_value.Value, value);
            
            if (_isLocked.Value == isLocked)
                OnIsLockedChanged(_isLocked.Value, isLocked);
            
            _value.Value = value;
            _isLocked.Value = isLocked;
        }

        public void Reset()
        {
            if (!IsServer) return;
            
            _isValueInitialized = false;
            _isLockedInitialized = false;
        }
        
        public void SetIsLocked(bool isLocked)
        {
            if (!IsServer) return;
            
            _isLocked.Value = isLocked;
        }
        
        public void Interact(int value)
        {
            if (_isLocked.Value)
                return;
            
            InteractRpc(AlgoQuestServices.Http.SessionId, value);
        }
        
        public void Highlight(bool highlight)
        {
            highlightContainer.SetActive(highlight);
        }
        
        // ====================
        
        private void UpdateVisuals()
        {
            if (IsServer && (!_isValueInitialized || !_isLockedInitialized)) return;
            
            var isLocked = _isLocked.Value;
            
            valueText.text = isLocked ? "locked" : Value == -1 ? "null" : Value.ToString();
            
            lockedContainer.SetActive(isLocked);
            unlockedContainer.SetActive(!isLocked);
        }
    }
}
