using Network;
using UnityEngine;

namespace Ui.PauseMenu
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMPro.TMP_Text codeText;
        [SerializeField] private GameObject codeCopiedText;
        
        private void OnEnable()
        {
            codeText.text = $"Code: {RelayService.JoinCode}";
            
            codeCopiedText.SetActive(false);
        }
        
        public void Resume()
        {
            Events.EventManager.Singleton.InputEvents.EmitPauseEvent();
        }

        public void Quit()
        {
            Core.Helpers.CloseApplication();
        }

        public void CopyCode()
        {
            Core.Helpers.CopyToClipboard(RelayService.JoinCode);
            
            codeCopiedText.SetActive(true);
        }
    }
}
