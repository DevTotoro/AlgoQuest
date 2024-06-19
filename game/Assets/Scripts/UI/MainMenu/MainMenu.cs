using UnityEngine;

namespace UI.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        public static void Quit()
        {
            Core.Helpers.CloseApplication();
        }
    }
}
