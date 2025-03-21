using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    [RequireComponent(typeof(AudioSource))]
    public class CollisionSounds : MonoBehaviour
    {

        public string[] TagsToDiscard;
        private AudioSource AttachedAudioSource;
        public AudioClip[] SoundClip;

        public float MinimumPitchSound;
        public float MaximumPitchSound;
        public float MinimumVolume;
        public float MaximumVolume;

        void Start()
        {
            AttachedAudioSource = GetComponent<AudioSource>();
        }
        void OnCollisionEnter(Collision c)
        {
            for (int x = 0; x < TagsToDiscard.Length; x++)
            {
                if (c.transform.root.tag != TagsToDiscard[x])
                {
                    PlayAttachedSounds();
                }
            }
        }
        public void PlayAttachedSounds()
        {
            AttachedAudioSource.clip = SoundClip[Random.Range(0, SoundClip.Length)];
            if (AttachedAudioSource.isPlaying)
            {
                AttachedAudioSource.Stop();
            }
            if (!AttachedAudioSource.isPlaying)
            {
                AttachedAudioSource.pitch = Random.Range(MinimumPitchSound, MaximumPitchSound);
                AttachedAudioSource.volume = Random.Range(MinimumVolume, MaximumVolume);
                AttachedAudioSource.Play();
            }
        }
    }
}
