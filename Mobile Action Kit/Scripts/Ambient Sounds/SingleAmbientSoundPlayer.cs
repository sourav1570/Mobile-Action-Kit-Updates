using UnityEngine;

namespace MobileActionKit
{
    [RequireComponent(typeof(AudioSource))]
    public class SingleAmbientSoundPlayer : MonoBehaviour
    {
        public AudioClip AudioClipToPlay;
        public float AudioVolume;
        public bool ShouldLoop = true;
        [Range(0, 256)]
        public int AudioPriority = 128;

        private AudioSource CurrentAudioSource;

        public void Start()
        {
            CurrentAudioSource = GetComponent<AudioSource>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.transform.root.tag == "Player")
            {
                CurrentAudioSource.clip = AudioClipToPlay;
                CurrentAudioSource.volume = AudioVolume;
                CurrentAudioSource.loop = ShouldLoop;
                CurrentAudioSource.priority = AudioPriority;
                CurrentAudioSource.Play();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.transform.root.tag == "Player")
            {
                CurrentAudioSource.Stop();
            }
        }
    }
}