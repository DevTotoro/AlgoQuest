using UnityEngine;

namespace Ui.GameplayMenus
{
    public class GameWonMenu : MonoBehaviour
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