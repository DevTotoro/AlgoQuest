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
        [SerializeField] private GameObject successMenu;

        private bool _isPaused;
        private bool _isGameOver;
        private bool _isSuccess;
        
        public static GameManager Singleton { get; private set; }
        
        public bool ShouldBeInteractive => !_isPaused && !_isGameOver && !_isSuccess;
        
        private void OnEnable()
        {
            if (Singleton != null)
            {
                Destroy(this);
                return;
            }
            
            Singleton = this;
            
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
            
            Events.EventManager.Singleton.InputEvents.PauseEvent += OnPause;
            
            Events.EventManager.Singleton.GameplayEvents.GameOverEvent += OnGameOver;
            Events.EventManager.Singleton.GameplayEvents.GameWonEvent += OnGameWon;
            Events.EventManager.Singleton.GameplayEvents.RestartEvent += OnRestart;
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
            if (_isGameOver || _isSuccess) return;
            
            _isPaused = !_isPaused;
            
            pauseMenu.SetActive(_isPaused);
        }

        private void OnGameOver()
        {
            _isGameOver = true;
            
            gameOverMenu.SetActive(true);
            
            Events.EventManager.Singleton.PlayerEvents.EmitMoveEvent(false);
        }
        
        private void OnGameWon()
        {
            _isSuccess = true;
            
            successMenu.SetActive(true);
            
            Events.EventManager.Singleton.PlayerEvents.EmitMoveEvent(false);
        }

        private void OnRestart()
        {
            _isGameOver = false;
            _isSuccess = false;
            
            gameOverMenu.SetActive(false);
            successMenu.SetActive(false);
        }
    }
}
