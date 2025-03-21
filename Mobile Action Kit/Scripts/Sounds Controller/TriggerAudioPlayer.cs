using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class TriggerAudioPlayer : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script is responsible for playing audio clips when the player enter the 'Trigger Collider' attached with this gameObject." +
            " These audio clips could include commander voices, mission-related audio, etc.";
        [Space(10)]

        [Tooltip("Enter the root gameobject tag name to detect collision with before playing back assigned audio clips.")]
        public string RootObjectTag = "Player";

        [Tooltip("Minimum delay (in seconds) before playing the first assigned audio clip.")]
        public float MinPlaybackDelay = 1f;

        [Tooltip("[Maximum delay (in seconds) before playing the first assigned audio clip.")]
        public float MaxPlaybackDelay = 2f;

        [Tooltip("Drag and drop the AudioSource component from the player hierarchy into this field.")]
        public AudioSource BriefingAudioSourceComponent;

        [Tooltip("Drag and drop one or more audio clips from the project into this field.")]
        public AudioClip[] AudioClipsToPlay;

        [Tooltip("Minimum time (in seconds) between playing each audio clip.")]
        public float MinAudioClipsPlaybackInterval = 2f;

        [Tooltip("Maximum time (in seconds) between playing each audio clip.")]
        public float MaxAudioClipsPlaybackInterval = 4f;

        private int currentIndex = 0;
        private bool isPlaying = false;
        bool IsStartPlaying = false;

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == RootObjectTag)
            {
                if (BriefingAudioSourceComponent != null)
                {
                    if (!BriefingAudioSourceComponent.isPlaying)
                    {
                        if (IsStartPlaying == false)
                        {
                            StartCoroutine(FirstClipPlayingDelay());
                            IsStartPlaying = true;
                        }
                    }
                }
            }
        }
        IEnumerator FirstClipPlayingDelay()
        {
            float Randomise = Random.Range(MinPlaybackDelay, MaxPlaybackDelay);
            yield return new WaitForSeconds(Randomise);
            BriefingAudioSourceComponent.clip = AudioClipsToPlay[currentIndex];
            BriefingAudioSourceComponent.Play();
            Invoke("StartPlayingSequence", BriefingAudioSourceComponent.clip.length);
        }
        private void StartPlayingSequence()
        {
            isPlaying = true;
            StartCoroutine(PlayAudioSequence());
        }

        private IEnumerator PlayAudioSequence()
        {
            yield return new WaitForSeconds(Random.Range(MinAudioClipsPlaybackInterval, MaxAudioClipsPlaybackInterval));
            currentIndex++;
            if (currentIndex < AudioClipsToPlay.Length)
            {
                BriefingAudioSourceComponent.clip = AudioClipsToPlay[currentIndex];
                BriefingAudioSourceComponent.Play();
                yield return new WaitForSeconds(BriefingAudioSourceComponent.clip.length);
                isPlaying = false;
                StartCoroutine(PlayAudioSequence());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}