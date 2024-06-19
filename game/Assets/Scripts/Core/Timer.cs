using UnityEngine;

namespace Core
{
    public class Timer
    {
        private float _time;
        private bool _isRunning;
        
        private float Hours => Mathf.FloorToInt(_time / 3600);
        private float Minutes => Mathf.FloorToInt(_time / 60);
        private float Seconds => Mathf.FloorToInt(_time % 60);
        
        public bool IsRunning => _isRunning;
        
        public string TimeString => $"{Hours:00}:{Minutes:00}:{Seconds:00}";
        
        public int TimeElapsedInMs => Mathf.FloorToInt(_time * 1000);
        
        public void Start()
        {
            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;
        }
        
        public void Reset()
        {
            _time = 0;
        }

        public bool Run()
        {
            if (!_isRunning) return false;

            _time += Time.deltaTime;
            
            return true;
        }

        public static string GetTimeString(int ms)
        {
            var time = ms / 1000f;
            var hours = Mathf.FloorToInt(time / 3600);
            var minutes = Mathf.FloorToInt(time / 60);
            var seconds = Mathf.FloorToInt(time % 60);
            
            return $"{hours:00}:{minutes:00}:{seconds:00}";
        }
    }
}
