using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    [RequireComponent(typeof(AudioSource))]
    public class MultipleAmbientSoundsPlayer : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script, handles the playback of ambient sounds in the game. " +
            "It supports playing sounds either randomly or sequentially, based on the 'PlaySoundsRandomly' field. " +
            "Each sound can have unique properties such as volume, priority, loop behavior, and a time range for playback duration. " +
            "The script uses a coroutine to manage the timing and playback of sounds, ensuring seamless transitions between them. " +
            "It requires an 'AudioSource' component on the same GameObject to function properly and can be configured using the 'AmbientSoundProperties' list.";
        [Space(10)]

        [Tooltip("Determines whether the ambient sounds should be played in random order. If unchecked, sounds will play sequentially.")]
        public bool PlaySoundsRandomly = true;

        [System.Serializable]
        public class AmbientSounds
        {
            [Tooltip("Name of the sound for identification purposes.")]
            public string SoundName;

            [Tooltip("The audio clip to play for this ambient sound.")]
            public AudioClip AudioClipToPlay;

            [Tooltip("Volume level for this audio clip (0 = muted, 1 = full volume).")]
            public float AudioVolume;

            [Tooltip("Whether this audio clip should loop continuously when played.")]
            public bool Loop = true;

            [Tooltip("The priority of this audio clip (0 = highest priority, 256 = lowest priority). Useful when multiple audio sources are competing.")]
            [Range(0, 256)]
            public int AudioPriority = 128;

            [Tooltip("The minimum amount of time (in seconds) before this sound can start playing.")]
            public float MinimumTimeToPlayThisSound = 3f;

            [Tooltip("The maximum amount of time (in seconds) before this sound can start playing.")]
            public float MaximumTimeToPlayThisSound = 5f;
        }

        private AudioSource CurrentAudioSource;

        [Tooltip("List of ambient sound properties. Each entry defines the characteristics of a single sound.")]
        public List<AmbientSounds> AmbientSoundProperties = new List<AmbientSounds>();

        int CountClip = 0;

        public void Start()
        {
            CurrentAudioSource = GetComponent<AudioSource>();
            StartCoroutine(PlaySounds());
        }
        public IEnumerator PlaySounds()
        {
            if (PlaySoundsRandomly == true)
            {
                int RandomiseClips = Random.Range(0, AmbientSoundProperties.Count);
                CurrentAudioSource.clip = AmbientSoundProperties[RandomiseClips].AudioClipToPlay;
                CurrentAudioSource.volume = AmbientSoundProperties[RandomiseClips].AudioVolume;
                CurrentAudioSource.loop = AmbientSoundProperties[RandomiseClips].Loop;
                CurrentAudioSource.priority = AmbientSoundProperties[RandomiseClips].AudioPriority;
                CurrentAudioSource.Play();
                float RandomTime = Random.Range(AmbientSoundProperties[RandomiseClips].MinimumTimeToPlayThisSound, AmbientSoundProperties[RandomiseClips].MaximumTimeToPlayThisSound);
                yield return new WaitForSeconds(RandomTime);
                StartCoroutine(PlaySounds());
            }
            else
            {
                if (CountClip >= AmbientSoundProperties.Count)
                {
                    CountClip = 0;
                }
                CurrentAudioSource.clip = AmbientSoundProperties[CountClip].AudioClipToPlay;
                CurrentAudioSource.volume = AmbientSoundProperties[CountClip].AudioVolume;
                CurrentAudioSource.loop = AmbientSoundProperties[CountClip].Loop;
                CurrentAudioSource.priority = AmbientSoundProperties[CountClip].AudioPriority;
                CurrentAudioSource.Play();
                float RandomTime = Random.Range(AmbientSoundProperties[CountClip].MinimumTimeToPlayThisSound, AmbientSoundProperties[CountClip].MaximumTimeToPlayThisSound);
                yield return new WaitForSeconds(RandomTime);
                ++CountClip;
                StartCoroutine(PlaySounds());
            }
        }
        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.gameObject.transform.root.tag == "Player")
        //    {

        //    }
        //}

    }
}


