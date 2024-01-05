using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;

namespace Network
{
    public class RelayService : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private int maxConnections = 4;
        
        public static RelayService Singleton { get; private set; }

        private static UnityTransport _unityTransport;
        
        public static string JoinCode { get; private set; }
        
        private void OnEnable()
        {
            if (Singleton != null)
            {
                Destroy(this);
                return;
            }
            
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            _unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            
            await Authenticate();
        }

        public async Task CreateHost()
        {
            var allocation = await Unity.Services.Relay.RelayService.Instance.CreateAllocationAsync(maxConnections);

            var relayServerData = new RelayServerData(allocation, "dtls");
            _unityTransport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            
            JoinCode = await Unity.Services.Relay.RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log($"Host code: {JoinCode}");
        }
        
        public async Task JoinHost(string code)
        {
            var allocation = await Unity.Services.Relay.RelayService.Instance.JoinAllocationAsync(code);

            var relayServerData = new RelayServerData(allocation, "dtls");
            _unityTransport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            
            Debug.Log($"Joined host {code}");
        }
        
        private static async Task Authenticate()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}
