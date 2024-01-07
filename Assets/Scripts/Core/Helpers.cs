using UnityEngine;

namespace Core
{
    public static class Helpers
    {
        public static void CloseApplication()
        {
            Application.Quit();
        }
        
        public static void CopyToClipboard(string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }
    }
}
