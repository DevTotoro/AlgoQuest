using Network;
using SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class AlgorithmMenu : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMPro.TMP_Text feedbackText;
        [SerializeField] private Button[] buttonsToDisable;
        
        public async void CreateHost(GameSceneSO scene)
        {
            feedbackText.text = "";
            
            ToggleButtons(false);
            
            try
            {
                await RelayService.Singleton.CreateHost();
                
                await Core.Helpers.Log(Events.LogType.HOST_CREATED);
                
                SceneService.LoadSceneNetwork(scene);
            }
            catch (Unity.Services.Relay.RelayServiceException e)
            {
                feedbackText.text = e.Message;
            }
            
            ToggleButtons(true);
        }
        
        private void ToggleButtons(bool toggle)
        {
            foreach (var button in buttonsToDisable)
                button.interactable = toggle;
        }
    }
}
