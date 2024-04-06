using UnityEngine;

namespace Ui.SuccessMenu
{
    public class SuccessMenu : MonoBehaviour
    {
        public void Restart()
        {
            Events.EventManager.Singleton.GameplayEvents.EmitRequestRestartEvent();
        }

        public void Quit()
        {
            Core.Helpers.CloseApplication();
        }
    }
}
