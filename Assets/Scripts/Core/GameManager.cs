using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Multiplayer")]
        [SerializeField] private GameObject playerPrefab;

        private static GameManager _singleton;
        
        private void OnEnable()
        {
            if (_singleton != null)
            {
                Destroy(this);
                return;
            }
            
            _singleton = this;
            DontDestroyOnLoad(gameObject);
            
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        }

        // Called per joined client
        private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            
            var player = Instantiate(playerPrefab).GetComponent<NetworkObject>();
            player.SpawnAsPlayerObject(clientId, true);
        }
    }
}
