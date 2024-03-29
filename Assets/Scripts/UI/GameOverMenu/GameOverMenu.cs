using UnityEngine;

namespace Ui.GameOverMenu
{
    public class GameOverMenu : MonoBehaviour
    {
        public void Retry()
        {
            Events.EventManager.Singleton.GameplayEvents.EmitRequestRetryEvent();
        }

        public void Quit()
        {
            Core.Helpers.CloseApplication();
        }
    }
}
