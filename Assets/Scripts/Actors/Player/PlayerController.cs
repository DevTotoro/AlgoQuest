using UnityEngine;
using Events;

namespace Actors.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] [Range(0f, 50f)] private float movementSpeed = 5f;
        
        private Vector3 _movementDirection = Vector3.zero;
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
        }
        
        // ====================

        private void OnMove(Vector2 input)
        {
            _movementDirection = new Vector3(input.x, 0f, input.y);
        }
        
        // ====================

        private void HandleMovement()
        {
            if (_movementDirection == Vector3.zero) return;
            
            transform.Translate(_movementDirection * _movementDistance, Space.World);
        }
    }
}
