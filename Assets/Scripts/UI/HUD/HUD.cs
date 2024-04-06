using UnityEngine;

namespace UI.HUD
{
    public class HUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMPro.TMP_Text timerText;
        [SerializeField] private TMPro.TMP_Text highScoresText;
        
        private void Awake()
        {
            Events.EventManager.Singleton.GameplayEvents.TimerUpdatedEvent += OnTimerUpdatedEvent;
            Events.EventManager.Singleton.GameplayEvents.HighScoresFetchedEvent += OnHighScoresFetchedEvent;
        }

        private void OnTimerUpdatedEvent(string time)
        {
            timerText.text = time;
        }

        private void OnHighScoresFetchedEvent(string highScores)
        {
            highScoresText.text = highScores;
        }
    }
}
