using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Core;
using Events;
using Interfaces;
using Actors.Containers;

namespace Actors.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [Header("Movement")]
        [SerializeField] [Range(0f, 50f)] private float movementSpeed = 5f;
        [SerializeField] [Range(0f, 50f)] private float rotationSpeed = 10f;
        
        [Header("Interactions")]
        [SerializeField] private bool enableInteractions = true;
        [SerializeField] [Range(0f, 5f)] private float interactionDistance = 1f;
        [SerializeField] private Vector3 interactionOffset = Vector3.zero;
        [SerializeField] private LayerMask interactionLayerMask;
        
        [Header("Container")]
        [SerializeField] private ContainerState initialState = ContainerState.Empty;
        [SerializeField] private int initialValue;
        
        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private AudioListener audioListener;
        [Space]
        [SerializeField] private GameObject emptyContainer;
        [SerializeField] private GameObject fullContainer;
        [SerializeField] private TMPro.TextMeshProUGUI valueText;
        
        // Movement
        private Vector3 _movementDirection = Vector3.zero;
        private Vector3 _rotationDirection = Vector3.zero;
        private float _movementDistance;
        
        // Interactions
        private readonly Collider[] _collidersInRange = new Collider[10];
        private IInteractive<ContainerData> _currentContainer;

        private readonly NetworkVariable<ContainerData> _networkData = new(default,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private void OnEnable()
        {
            EventManager.Singleton.InputEvents.MoveEvent += OnMove;
            EventManager.Singleton.InputEvents.InteractEvent += OnInteract;
            
            EventManager.Singleton.ContainerEvents.ContainerDataUpdatedEvent += OnContainerDataUpdated;
            
            EventManager.Singleton.GameplayEvents.RetryEvent += OnRetry;
            EventManager.Singleton.GameplayEvents.RestartEvent += OnRestart;
        }
        
        private void OnDisable()
        {
            EventManager.Singleton.InputEvents.MoveEvent -= OnMove;
            EventManager.Singleton.InputEvents.InteractEvent -= OnInteract;
            
            EventManager.Singleton.ContainerEvents.ContainerDataUpdatedEvent -= OnContainerDataUpdated;
            
            EventManager.Singleton.GameplayEvents.RetryEvent -= OnRetry;
            EventManager.Singleton.GameplayEvents.RestartEvent -= OnRestart;
        }

        private void Awake()
        {
            _networkData.OnValueChanged += OnDataChanged;
        }

        private void Update()
        {
            _movementDistance = movementSpeed * Time.deltaTime;
            
            CheckForInteractive();
            HandleMovement();
            HandleRotation();
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                virtualCamera.Priority = 0;
                
                UpdateContainerVisuals();

                return;
            }
            
            audioListener.enabled = true;
            
            virtualCamera.Priority = 1;
            
            _networkData.Value = new ContainerData { State = initialState, Value = initialValue };
        }
        
        // ====================

        private void OnMove(Vector2 input)
        {
            if (!IsOwner) return;
            
            _movementDirection = new Vector3(input.x, 0f, input.y);
            _rotationDirection = _movementDirection;
            
            EventManager.Singleton.PlayerEvents.EmitMoveEvent(_movementDirection != Vector3.zero);
        }
        
        private void OnInteract()
        {
            if (!IsOwner || !GameManager.Singleton.ShouldBeInteractive || !enableInteractions ||
                _currentContainer == null) return;

            _currentContainer.Interact(_networkData.Value);
        }
        
        private void OnContainerDataUpdated(ulong senderClientId, ContainerData data)
        {
            if (!IsOwner || senderClientId != OwnerClientId) return;
            
            _networkData.Value = data;
        }
        
        private void OnDataChanged(ContainerData previousData, ContainerData newData)
        {
            UpdateContainerVisuals();
        }
        
        private void OnRetry()
        {
            if (!IsOwner) return;
            
            _networkData.Value = new ContainerData { State = initialState, Value = initialValue };
        }

        private void OnRestart()
        {
            if (!IsOwner) return;
            
            _networkData.Value = new ContainerData { State = initialState, Value = initialValue };
        }
        
        // ====================

        private void HandleMovement()
        {
            if (!GameManager.Singleton.ShouldBeInteractive || _movementDirection == Vector3.zero) return;
            
            transform.Translate(_movementDirection * _movementDistance, Space.World);
        }

        private void HandleRotation()
        {
            if (!GameManager.Singleton.ShouldBeInteractive || _rotationDirection == Vector3.zero) return;
            
            var targetRotation = Quaternion.LookRotation(_rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        private void CheckForInteractive()
        {
            // Check if there are any colliders in range (may or may not be interactive)
            if (Physics.OverlapSphereNonAlloc(transform.position + interactionOffset, interactionDistance,
                    _collidersInRange, interactionLayerMask) <= 0) 
            {
                _currentContainer?.Highlight(false);
                _currentContainer = null;
                return;
            }
            
            // Find the closest interactive
            var closestDistance = float.MaxValue;
            IInteractive<ContainerData> closestInteractive = null;
            
            foreach (var coll in _collidersInRange)
            {
                if (!coll || !coll.TryGetComponent(out IInteractive<ContainerData> interactive)) continue;
                
                var distance = Vector3.Distance(transform.position, coll.transform.position);
                if (distance >= closestDistance) continue;
                
                closestDistance = distance;
                closestInteractive = interactive;
            }
            
            // Highlight the closest interactive
            if (closestInteractive == null)
            {
                _currentContainer?.Highlight(false);
                _currentContainer = null;
                return;
            }
            
            _currentContainer?.Highlight(false);
            _currentContainer = closestInteractive;
            _currentContainer.Highlight(true);
        }
        
        private void UpdateContainerVisuals()
        {
            emptyContainer.SetActive(_networkData.Value.State == ContainerState.Empty);
            fullContainer.SetActive(_networkData.Value.State == ContainerState.Full);
            
            valueText.text = _networkData.Value.Value.ToString();
        }
        
        // ====================
        
        private void OnDrawGizmosSelected()
        {
            if (!enableInteractions) return;
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + interactionOffset, interactionDistance);
        }
    }
}
