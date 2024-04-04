using UnityEngine;
using UnityEngine.UI;
using AlgoQuestServices;

namespace UI.MainMenu
{
    public class UserMenu : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMPro.TMP_InputField usernameField;
        [SerializeField] private TMPro.TMP_Text feedbackText;
        [SerializeField] private Button[] buttonsToDisable;
        [Header("Menu Management")]
        [SerializeField] private GameObject userMenu;
        [SerializeField] private GameObject multiplayerMenu;

        public async void StartSession()
        {
            feedbackText.text = "";
            ToggleButtons(false);

            var res = await Sessions.Create(new Sessions.CreateSessionRequestPayload { username = usernameField.text });

            if (!res.Success)
            {
                feedbackText.text = res.Message;
                ToggleButtons(true);
                
                return;
            }
            
            ToggleButtons(true);
            
            userMenu.SetActive(false);
            multiplayerMenu.SetActive(true);
        }

        private void ToggleButtons(bool toggle)
        {
            foreach (var button in buttonsToDisable)
                button.interactable = toggle;
        }
    }
}
