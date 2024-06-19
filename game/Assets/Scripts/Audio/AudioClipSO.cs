using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(fileName = "AudioClip", menuName = "Audio/Audio Clip")]
    public class AudioClipSO : ScriptableObject
    {
        [Space(20)]
        [SerializeField] private AudioClip audioClip;
        [SerializeField] [Range(0f, 1f)] private float volume = 1f;
        [SerializeField] [Range(.1f, 3f)] private float pitch = 1f;
        
        public float Length => audioClip.length;
        
        public void Play(AudioSource audioSource)
        {
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            
            audioSource.Play();
        }
    }
}
