using UnityEngine;

namespace Core
{
    public class Timer
    {
        private float _time;
        private bool _isRunning;
        
        public float Hours => Mathf.FloorToInt(_time / 3600);
        public float Minutes => Mathf.FloorToInt(_time / 60);
        public float Seconds => Mathf.FloorToInt(_time % 60);
        
        public string TimeString => $"{Hours:00}:{Minutes:00}:{Seconds:00}";
        
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
    }
}
