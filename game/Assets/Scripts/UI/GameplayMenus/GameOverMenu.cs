using UnityEngine;

namespace Ui.GameplayMenus
{
    public class GameOverMenu : MonoBehaviour
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
