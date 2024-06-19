using UnityEngine;

namespace Audio
{
    public class AudioService : MonoBehaviour
    {
        public void PlayAudio(AudioClipSO audioClip)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            
            audioClip.Play(audioSource);
            
            Destroy(audioSource, audioClip.Length);
        }
        
        public static void SetVolume(float volume)
        {
            AudioListener.volume = volume;
        }
    }
}
