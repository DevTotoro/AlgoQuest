using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public static class SceneService
    {
        public static void LoadScene(GameSceneSO scene, bool loadByIndex = false)
        {
            if (loadByIndex)
                SceneManager.LoadScene(scene.BuildIndex);
            else
                SceneManager.LoadScene(scene.SceneName);
        }
        
        public static void LoadSceneNetwork(GameSceneSO scene)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(scene.SceneName, LoadSceneMode.Single);
        }
    }
}
