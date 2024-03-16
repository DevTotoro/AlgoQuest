using Events;
using UnityEngine;
using Unity.Netcode;

namespace Actors.Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : NetworkBehaviour
    {
        [Header("Animation Parameters")]
        [SerializeField] private string isMoving = "IsMoving";
        
        private Animator _animator;

        private void OnEnable()
        {
            EventManager.Singleton.PlayerEvents.MoveEvent += SetIsMovingAnimation;
        }

        private void OnDisable()
        {
            EventManager.Singleton.PlayerEvents.MoveEvent -= SetIsMovingAnimation;
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        
        private void SetIsMovingAnimation(bool moving)
        {
            if (!IsOwner) return;
            
            _animator.SetBool(isMoving, moving);
        }
    }
}
