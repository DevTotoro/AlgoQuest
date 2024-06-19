using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class OptionsMenu : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMPro.TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;
        
        private void Start()
        {
            SetupResolutionDropdown();

            fullscreenToggle.isOn = Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen;
        }

        private void SetupResolutionDropdown()
        {
            var resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            
            var options = new System.Collections.Generic.List<string>();
            var currentResolutionIndex = 0;
            
            for (var i = 0; i < resolutions.Length; i++)
            {
                var option = resolutions[i].width + " X " + resolutions[i].height;
                
                // Prevent duplicates
                if (options.Contains(option)) continue;
                
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            
            resolutionDropdown.AddOptions(options);
            
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
    }
}
