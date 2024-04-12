using UnityEngine;

namespace UI.HUD
{
    public class HUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject timerContainer;
        [SerializeField] private TMPro.TMP_Text timerText;
        [Space]
        [SerializeField] private GameObject highScoresContainer;
        [SerializeField] private TMPro.TMP_Text highScoresText;
        
        private void Awake()
        {
            Events.EventManager.Singleton.UIEvents.ToggleTimerEvent += OnToggleTimerEvent;
            Events.EventManager.Singleton.UIEvents.ToggleLeaderboardEvent += OnToggleLeaderboardEvent;
            
            Events.EventManager.Singleton.UIEvents.SetTimerEvent += OnSetTimerEvent;
            Events.EventManager.Singleton.UIEvents.SetLeaderboardEvent += OnSetLeaderboardEvent;
        }
        
        private void OnToggleTimerEvent(bool enable)
        {
            timerContainer.SetActive(enable);
        }
        
        private void OnToggleLeaderboardEvent(bool enable)
        {
            highScoresContainer.SetActive(enable);
        }

        private void OnSetTimerEvent(string time)
        {
            timerText.text = time;
        }

        private void OnSetLeaderboardEvent(string highScores)
        {
            highScoresText.text = highScores;
        }
    }
}
