using UnityEngine;
using Unity.Netcode;

namespace Network
{
    public class ConnectionService : MonoBehaviour
    {
        public static void CreateHost()
        {
            NetworkManager.Singleton.StartHost();
        }
        
        public static void CreateClient()
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
