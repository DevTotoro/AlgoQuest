using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Multiplayer")]
        [SerializeField] private GameObject[] playerPrefabs;
        
        [Header("UI Elements")]
        [SerializeField] private GameObject pauseMenu;

        public static GameManager Singleton { get; private set; }
        
        public bool IsPaused { get; private set; }
        
        private void OnEnable()
        {
            if (Singleton != null)
            {
                Destroy(this);
                return;
            }
            
            Singleton = this;
            DontDestroyOnLoad(gameObject);
            
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
            
            Events.EventManager.Singleton.InputEvents.PauseEvent += OnPause;
        }

        // Called per joined client
        private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            Debug.Log($"Client {clientId} loaded scene {sceneName}");
            
            if (!NetworkManager.Singleton.IsServer) return;
            
            var playerPrefab = playerPrefabs[NetworkManager.Singleton.ConnectedClients.Count - 1];
            
            var player = Instantiate(playerPrefab).GetComponent<NetworkObject>();
            player.SpawnAsPlayerObject(clientId, true);
        }

        private void OnPause()
        {
            IsPaused = !IsPaused;
            
            pauseMenu.SetActive(IsPaused);
        }
    }
}
