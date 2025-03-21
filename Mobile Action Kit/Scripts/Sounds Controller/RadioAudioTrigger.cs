using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class RadioAudioTrigger : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script is responsible for audio clips playback at the beginning of the mission.";

        [Space(10)]

        [Tooltip("If checked, audio clips will start playing when the game begins.")]
        public bool EnableMissionStartAudioBriefing = true;

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

        private void Start()
        {
            if (EnableMissionStartAudioBriefing && BriefingAudioSourceComponent != null)
            {
                StartCoroutine(FirstClipPlayingDelay());
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
        }
    }
}