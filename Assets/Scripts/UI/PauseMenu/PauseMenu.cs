using UnityEngine;

namespace Ui.PauseMenu
{
    public class PauseMenu : MonoBehaviour
    {
        public void Resume()
        {
            Events.EventManager.Singleton.InputEvents.EmitPauseEvent();
        }

        public void Quit()
        {
            Core.Helpers.CloseApplication();
        }
    }
}
