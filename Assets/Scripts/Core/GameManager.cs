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
        [SerializeField] private GameObject gameOverMenu;

        private bool _isPaused;
        private bool _isGameOver;
        
        public static GameManager Singleton { get; private set; }
        
        public bool ShouldBeInteractive => !_isPaused && !_isGameOver;
        
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
            
            Events.EventManager.Singleton.GameplayEvents.GameOverEvent += OnGameOver;
            Events.EventManager.Singleton.GameplayEvents.RetryEvent += OnRetry;
        }

        // Called per joined client
        private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            
            var playerPrefab = playerPrefabs[NetworkManager.Singleton.ConnectedClients.Count - 1];
            
            var player = Instantiate(playerPrefab).GetComponent<NetworkObject>();
            player.SpawnAsPlayerObject(clientId, true);
        }

        private void OnPause()
        {
            if (_isGameOver) return;
            
            _isPaused = !_isPaused;
            
            pauseMenu.SetActive(_isPaused);
        }

        private void OnGameOver()
        {
            _isGameOver = true;
            
            gameOverMenu.SetActive(true);
        }
        
        private void OnRetry()
        {
            _isGameOver = false;
            
            gameOverMenu.SetActive(false);
        }
    }
}
