using UnityEngine.Events;
using Actors.Containers;

namespace Events
{
    public class ContainerEvents
    {
        public event UnityAction<ulong, ContainerData> ContainerDataUpdatedEvent;
        public event UnityAction ContainerSpawnedEvent;
        public event UnityAction<string> UserInteractedWithContainerEvent;
        
        public void EmitContainerDataUpdatedEvent(ulong senderClientId, ContainerData data)
        {
            ContainerDataUpdatedEvent?.Invoke(senderClientId, data);
        }
        
        public void EmitContainerSpawnedEvent()
        {
            ContainerSpawnedEvent?.Invoke();
        }
        
        public void EmitUserInteractedWithContainerEvent(string sessionId)
        {
            UserInteractedWithContainerEvent?.Invoke(sessionId);
        }
    }
}
