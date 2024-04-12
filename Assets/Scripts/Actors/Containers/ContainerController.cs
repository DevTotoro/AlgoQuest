using System;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

namespace Actors.Containers
{
    public struct ContainerData : INetworkSerializable, IEquatable<ContainerData>
    {
        public int Value;
        public bool IsLocked;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref IsLocked);
            serializer.SerializeValue(ref Value);
        }

        public bool Equals(ContainerData other)
        {
            return Value == other.Value && IsLocked == other.IsLocked;
        }
    }
    
    [RequireComponent(typeof(Collider))]
    public class ContainerController : MonoBehaviour, Interfaces.IInteractive
    {
        [Header("References")]
        [SerializeField] private GameObject lockedContainer;
        [SerializeField] private GameObject unlockedContainer;
        [SerializeField] private GameObject highlightContainer;
        [Space]
        [SerializeField] private TMPro.TextMeshProUGUI valueText;
        
        public int Index { private get; set; }
        
        public event UnityAction<int, int, string> OnInteract;

        public void Interact(int value)
        {
            OnInteract?.Invoke(Index, value, AlgoQuestServices.Http.SessionId);
        }
        
        public void Highlight(bool highlight)
        {
            highlightContainer.SetActive(highlight);
        }
        
        public void ToggleLock(bool isLocked)
        {
            lockedContainer.SetActive(isLocked);
            unlockedContainer.SetActive(!isLocked);
        }

        public void SetText(string text)
        {
            valueText.text = text;
        }
    }
}
