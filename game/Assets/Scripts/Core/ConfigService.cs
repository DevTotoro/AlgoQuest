using UnityEngine;

namespace Core
{
    public class ConfigService : MonoBehaviour
    {
        public static void SetQuality(int quality)
        {
            QualitySettings.SetQualityLevel(quality);
        }
        
        public static void SetResolution(int resolutionIndex)
        {
            var resolution = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        }
        
        public static void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreenMode = isFullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        }
    }
}
