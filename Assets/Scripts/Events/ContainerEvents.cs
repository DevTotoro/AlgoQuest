using UnityEngine.Events;
using Actors.Containers;

namespace Events
{
    public class ContainerEvents
    {
        public event UnityAction<ulong, ContainerData> ContainerDataUpdatedEvent;
        public event UnityAction ContainerSpawnedEvent;
        
        public void EmitContainerDataUpdatedEvent(ulong senderClientId, ContainerData data)
        {
            ContainerDataUpdatedEvent?.Invoke(senderClientId, data);
        }
        
        public void EmitContainerSpawnedEvent()
        {
            ContainerSpawnedEvent?.Invoke();
        }
    }
}
