using UnityEngine;

namespace Events
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Singleton { get; private set; }
        
        public InputEvents InputEvents { get; private set; }
        public GameplayEvents GameplayEvents { get; private set; }
        public PlayerEvents PlayerEvents { get; private set; }
        public ContainerEvents ContainerEvents { get; private set; }

        private void OnEnable()
        {
            if (Singleton != null)
            {
                Destroy(this);
                return;
            }
            
            Singleton = this;
            DontDestroyOnLoad(gameObject);
            
            InputEvents = new InputEvents();
            GameplayEvents = new GameplayEvents();
            PlayerEvents = new PlayerEvents();
            ContainerEvents = new ContainerEvents();
        }
    }
}
