using UnityEngine;

namespace SceneManagement
{
    [CreateAssetMenu(fileName = "GameScene", menuName = "Scene Management/Game Scene")]
    public class GameSceneSO : ScriptableObject
    {
        [Space(20)]
        [SerializeField] private string sceneName;
        [SerializeField] private int buildIndex;
        
        public string SceneName => sceneName;
        public int BuildIndex => buildIndex;
    }
}
