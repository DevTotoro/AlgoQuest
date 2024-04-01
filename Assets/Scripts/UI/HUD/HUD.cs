using UnityEngine;

namespace UI.HUD
{
    public class HUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMPro.TMP_Text timerText;
        
        private void Awake()
        {
            Events.EventManager.Singleton.GameplayEvents.TimerUpdatedEvent += OnTimerUpdatedEvent;
        }

        private void OnTimerUpdatedEvent(string time)
        {
            timerText.text = time;
        }
    }
}
