using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class AiNonCombatChatter : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script is responsible for playing back AudioClips(usually conversations or radio chatter) whenever Player gets within a certain range that is defined by the sphere collider attached to this gameobject. " +
            "Audio clips from the 'ChatterAudioClips' list and will be played sequentially in the order they are listed. If player will get outside of the trigger and then will re-enter it " +
            "then the Chatter playback sequence will resume from the next clip in the list. The list of chatter audio clips is played only once for the duration of the player`s presence within the trigger. ";

        //[Tooltip("Ai agent will play chatter audio clips only if this checkbox is enabled.")]
        //public bool PlayChatterAudioClip = true;
        [Tooltip("Drag&Drop root gameobject of this AI agent with CoreAiBehaviour script attached to it into this field.")]
        public CoreAiBehaviour CoreAiBehaviourScript;

        [Tooltip("Drag&Drop gameobject(usually the Player) that will trigger the playback of the AiNonCombatChatter audio clips.")]
        public Transform ChatterTriggerObject;

        [Tooltip("Minimum delay before playing the chatter audio clips.")]
        public float MinChatterDelay;
        [Tooltip("Maximum delay before playing the chatter audio clips.")]
        public float MaxChatterDelay;

        [Tooltip("Drag&Drop Audio Source component attached to this gameobject into this field.")]
        public AudioSource AudioSourceComponent;

        [Tooltip("If checked, the audio clip will continue playing until all clips have finished, even if the player exits the trigger collider. " +
            "If unchecked, the audio clip will stop and resume from where it left off if the player re-enters the trigger collider. This ensures the player hears all audio clips in the list properly.")]
        public bool NoElementReplayOnTriggerReEnter = false;

        [Tooltip("List of Audio clips to to play sequentially in the order they are listed.")]
        public AudioClip[] ChatterAudioClips;

        [Tooltip("Minimal time interval between playback of the Audio Clips from the list.")]
        public float MinTimeIntervalBetweenChatterClips;
        [Tooltip("Maximal time interval between playback of the Audio Clips from the list.")]
        public float MaxTimeIntervalBetweenChatterClips;

        private int currentIndex = 0;
        private float DelayBeforeStarting;
        private bool exitedTrigger = false;
        private bool Once = false;

        bool IsInitialDelayCompleted = false;

        [HideInInspector]
        public bool DeactivateFunctioning = false;

        public bool DoNotPlayAnyNewAudioClipNow = false;

        private void Start()
        {
            if(DeactivateFunctioning == false)
            {
                DelayBeforeStarting = Random.Range(MinChatterDelay, MaxChatterDelay);
                gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        
        }

        private void OnTriggerEnter(Collider other)
        {
            if (DeactivateFunctioning == false)
            {
                if (other.gameObject.transform.root == ChatterTriggerObject && other.transform.root != this.transform.root && other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
                {
                    //if (PlayChatterAudioClip == true)
                    //{
                    if (Once == false)
                    {
                        exitedTrigger = false;
                        StartCoroutine(PlayAudioSequence());
                        Once = true;
                    }
                    //}
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (DeactivateFunctioning == false)
            {
                if (other.gameObject.transform.root == ChatterTriggerObject && other.transform.root != this.transform.root && other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
                {
                    //if (PlayChatterAudioClip == true)
                    //{
                    AudioSourceComponent.Stop();
                    Once = false;
                    exitedTrigger = true;
                    //}
                }
            }
        }
        public void ImmediatelyStopConversationSounds()
        {
            if (DeactivateFunctioning == false)
            {
                AudioSourceComponent.Stop();
                Once = false;
                exitedTrigger = true;
                DoNotPlayAnyNewAudioClipNow = true;

                if (gameObject.GetComponent<Speaker>() != null)
                {
                    gameObject.GetComponent<Speaker>().StopChatting();
                }
                CoreAiBehaviourScript.PlayDefaultBehaviourSoundsNow = true;
                gameObject.SetActive(false);

            }
        }
        IEnumerator PlayAudioSequence()
        {
            if(IsInitialDelayCompleted == false)
            {
                yield return new WaitForSeconds(DelayBeforeStarting);
                IsInitialDelayCompleted = true;
            }

            if (exitedTrigger == false)
            {
                if (currentIndex < ChatterAudioClips.Length)
                {
                    if (AudioSourceComponent != null && ChatterAudioClips[currentIndex] != null)
                    {
                        if (exitedTrigger == false && DoNotPlayAnyNewAudioClipNow == false)
                        {
                            // Debug.Log("Playing" + currentIndex);
                            AudioSourceComponent.clip = ChatterAudioClips[currentIndex];
                            AudioSourceComponent.Play();
                        }
                        else
                        {
                            AudioSourceComponent.Stop();
                        }
                        //  Debug.Log(AudioSourceComponent.clip.length + "Audio");
                        yield return new WaitForSeconds(AudioSourceComponent.clip.length);
                        float randomDelay = Random.Range(MinTimeIntervalBetweenChatterClips, MaxTimeIntervalBetweenChatterClips);
                        //  Debug.Log(randomDelay + "wait");
                        yield return new WaitForSeconds(randomDelay);
                        if (exitedTrigger == false)
                        {
                            currentIndex = currentIndex + 1;
                        }
                    }
                }
            }
            else
            {
                AudioSourceComponent.Stop();
            }

            if (currentIndex >= ChatterAudioClips.Length)
            {
                if(gameObject.GetComponent<Speaker>() != null)
                {
                    gameObject.GetComponent<Speaker>().StopChatting();
                    CoreAiBehaviourScript.PlayDefaultBehaviourSoundsNow = true;
                    gameObject.SetActive(false);
                }
                else
                {
                    CoreAiBehaviourScript.PlayDefaultBehaviourSoundsNow = true;
                    gameObject.SetActive(false);
                }
              
            }
            else
            {
                if (exitedTrigger == false)
                {
                    StartCoroutine(PlayAudioSequence());
                }
            }
        }

    }
}