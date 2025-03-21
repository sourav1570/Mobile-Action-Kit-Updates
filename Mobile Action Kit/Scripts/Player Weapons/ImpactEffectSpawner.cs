using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class ImpactEffectSpawner : MonoBehaviour
    {
        [TextArea]     
        public string ScriptInfo = "Spawns a hit impact effect at the point of raycast hit point and plays a randomized impact audio clips assigned below." +
                                    " The hit effect prefab is instantiated at the impact position and aligned to the surface normal." +
                                    " A random audio clip is selected and played using an AudioSource." +
                                    " The pitch and volume of the sound are randomized within defined ranges.";


        [Tooltip("Prefab of the visual hit effect to spawn at the point of impact.")]
        public GameObject HitEffectPrefab;
        [Tooltip("Audio source that will play the impact sound.")]
        public AudioSource AudioSourceComponent;
        [Tooltip("Array of audio clips to choose from when playing the impact sound.")]
        public AudioClip[] AudioClipsToPlay;

        [Tooltip("Minimum pitch variation for the impact sound.")]
        public float MinPitchSound;
        [Tooltip("Maximum pitch variation for the impact sound.")]
        public float MaxPitchSound;
        [Tooltip("Minimum volume variation for the impact sound.")]
        public float MinVolume;
        [Tooltip("Maximum volume variation for the impact sound.")]
        public float MaxVolume;

        public void PlaySound()
        {
            if (AudioSourceComponent != null)
            {
                if(AudioClipsToPlay.Length > 0)
                {
                    AudioSourceComponent.clip = AudioClipsToPlay[Random.Range(0, AudioClipsToPlay.Length)];
                    if (AudioSourceComponent.isPlaying)
                    {
                        AudioSourceComponent.Stop();
                    }
                    if (!AudioSourceComponent.isPlaying)
                    {
                        AudioSourceComponent.pitch = Random.Range(MinPitchSound, MaxPitchSound);
                        AudioSourceComponent.volume = Random.Range(MinVolume, MaxVolume);
                        AudioSourceComponent.Play();
                    }
                }
            }
        }
    }
}