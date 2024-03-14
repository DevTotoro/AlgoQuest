using Cinemachine;
using UnityEngine;
using Unity.Netcode;
using Events;

namespace Actors.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [Header("Movement")]
        [SerializeField] [Range(0f, 50f)] private float movementSpeed = 5f;
        [SerializeField] [Range(0f, 50f)] private float rotationSpeed = 5f;

        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private AudioListener audioListener;
        
        private Vector3 _movementDirection = Vector3.zero;
        private Vector3 _rotationDirection = Vector3.zero;
        private float _movementDistance;

        private void OnEnable()
        {
            EventManager.Singleton.InputEvents.MoveEvent += OnMove;
        }
        
        private void OnDisable()
        {
            EventManager.Singleton.InputEvents.MoveEvent -= OnMove;
        }

        private void Update()
        {
            _movementDistance = movementSpeed * Time.deltaTime;
            
            HandleMovement();
            HandleRotation();
        }
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                virtualCamera.Priority = 0;

                return;
            }
            
            audioListener.enabled = true;
            
            virtualCamera.Priority = 1;
        }
        
        // ====================

        private void OnMove(Vector2 input)
        {
            if (!IsOwner) return;
            
            _movementDirection = new Vector3(input.x, 0f, input.y);
            _rotationDirection = _movementDirection;
        }
        
        // ====================

        private void HandleMovement()
        {
            if (Core.GameManager.Singleton.IsPaused || _movementDirection == Vector3.zero) return;
            
            transform.Translate(_movementDirection * _movementDistance, Space.World);
        }

        private void HandleRotation()
        {
            if (Core.GameManager.Singleton.IsPaused || _rotationDirection == Vector3.zero) return;
            
            var targetRotation = Quaternion.LookRotation(_rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
