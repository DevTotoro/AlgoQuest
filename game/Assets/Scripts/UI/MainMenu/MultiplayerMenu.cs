using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;
using RelayService = Network.RelayService;

namespace UI.MainMenu
{
    public class MultiplayerMenu : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMPro.TMP_InputField codeInputField;
        [SerializeField] private TMPro.TMP_Text feedbackText;
        [SerializeField] private Button[] buttonsToDisable;
        
        public async void JoinHost()
        {
            feedbackText.text = "";
            
            if (string.IsNullOrEmpty(codeInputField.text))
            {
                feedbackText.text = "Please enter a code";
                return;
            }
            
            ToggleButtons(false);

            try
            {
                await RelayService.Singleton.JoinHost(codeInputField.text);
            }
            catch (RelayServiceException e)
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
