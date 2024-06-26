using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Core;
using Events;
using Interfaces;
using LogType = Events.LogType;

namespace Actors.Player
{
    [RequireComponent(typeof(CapsuleCollider))]
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
        
        [Header("Collisions")]
        [SerializeField] private bool enableCollisions = true;
        [SerializeField] private LayerMask collisionLayerMask;
        
        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private AudioListener audioListener;
        [Space]
        [SerializeField] private TMPro.TextMeshProUGUI valueText;

        private CapsuleCollider _capsuleCollider;
        
        // Movement
        private Vector3 _movementDirection = Vector3.zero;
        private Vector3 _rotationDirection = Vector3.zero;
        private float _movementDistance;
        
        // Interactions
        private readonly Collider[] _collidersInRange = new Collider[10];
        private IInteractive _currentInteractive;

        private readonly NetworkVariable<int> _containerValue =
            new(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private void OnEnable()
        {
            EventManager.Singleton.InputEvents.MoveEvent += OnMove;
            EventManager.Singleton.InputEvents.InteractEvent += OnInteract;
            
            EventManager.Singleton.GameplayEvents.SetPlayerContainerValueEvent += OnSetPlayerContainerValue;
            EventManager.Singleton.GameplayEvents.RestartEvent += OnRestart;
        }
        
        private void OnDisable()
        {
            EventManager.Singleton.InputEvents.MoveEvent -= OnMove;
            EventManager.Singleton.InputEvents.InteractEvent -= OnInteract;
            
            EventManager.Singleton.GameplayEvents.SetPlayerContainerValueEvent -= OnSetPlayerContainerValue;
            EventManager.Singleton.GameplayEvents.RestartEvent -= OnRestart;
        }

        private void Awake()
        {
            _capsuleCollider = GetComponent<CapsuleCollider>();
            
            _containerValue.OnValueChanged += OnDataChanged;
        }

        private void Update()
        {
            _movementDistance = movementSpeed * Time.deltaTime;
            
            CheckForInteractive();
            HandleCollisions();
            HandleMovement();
            HandleRotation();
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                virtualCamera.Priority = 0;
                
                SetValueText(_containerValue.Value);

                return;
            }
            
            audioListener.enabled = true;
            
            virtualCamera.Priority = 1;
            
            if (!IsServer)
                EventManager.Singleton.LogEvents.EmitNetworkLogEvent(LogType.HOST_JOINED);
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
                _currentInteractive == null) return;
        
            _currentInteractive.Interact(_containerValue.Value);
        }
        
        private void OnDataChanged(int previousValue, int newValue)
        {
            SetValueText(newValue);
        }
        
        private void OnSetPlayerContainerValue(ulong senderClientId, int containerValue)
        {
            if (!IsOwner || senderClientId != OwnerClientId) return;
            
            _containerValue.Value = containerValue;
        }
        
        private void OnRestart()
        {
            if (!IsOwner) return;

            _containerValue.Value = -1;
        }
        
        // ====================

        private void HandleCollisions()
        {
            if (!IsOwner || !enableCollisions) return;

            var position = transform.position;
            var top = position + _capsuleCollider.center + Vector3.up * _capsuleCollider.height / 2;
            var bottom = position + _capsuleCollider.center - Vector3.up * _capsuleCollider.height / 2;
            
            // Check if player can move in the direction of the input
            if (!Physics.CapsuleCast(top, bottom, _capsuleCollider.radius, _movementDirection, _movementDistance,
                    collisionLayerMask)) return;
            
            // Attempt to move in the X axis
            var moveDirectionX = new Vector3(_movementDirection.x, 0, 0).normalized;
            if (!Physics.CapsuleCast(top, bottom, _capsuleCollider.radius, moveDirectionX, _movementDistance,
                    collisionLayerMask))
            {
                _movementDirection = moveDirectionX;
                return;
            }
            
            // Attempt to move in the Z axis
            var moveDirectionZ = new Vector3(0, 0, _movementDirection.z).normalized;
            if (!Physics.CapsuleCast(top, bottom, _capsuleCollider.radius, moveDirectionZ, _movementDistance,
                    collisionLayerMask))
            {
                _movementDirection = moveDirectionZ;
                return;
            }
            
            // Player cannot move
            _movementDirection = Vector3.zero;
        }

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
                _currentInteractive?.Highlight(false);
                _currentInteractive = null;
                return;
            }
            
            // Find the closest interactive
            var closestDistance = float.MaxValue;
            IInteractive closestInteractive = null;
            
            foreach (var coll in _collidersInRange)
            {
                if (!coll || !coll.TryGetComponent(out IInteractive interactive)) continue;
                
                var distance = Vector3.Distance(transform.position, coll.transform.position);
                if (distance >= closestDistance) continue;
                
                closestDistance = distance;
                closestInteractive = interactive;
            }
            
            // Highlight the closest interactive
            if (closestInteractive == null)
            {
                _currentInteractive?.Highlight(false);
                _currentInteractive = null;
                return;
            }
            
            _currentInteractive?.Highlight(false);
            _currentInteractive = closestInteractive;
            _currentInteractive.Highlight(true);
        }
        
        private void SetValueText(int value)
        {
            valueText.text = value == -1 ? "null" : value.ToString();
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
