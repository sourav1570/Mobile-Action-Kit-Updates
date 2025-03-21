using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class Speaker : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script is assigned to Ai agent to make him a speaker for the duration of NonCombatChatter. " +
            "So that other AI agents that are assigned to the list of Listeners would not be able to speak and thus interrupt the speaker. " +
            "This is achieved by preventing the AiNonCombatChatter script on those listeners agents from playing back any audio clips from their 'ChatterAudioClips' lists. " +
            "And after the 'Speaker' Ai agent will finish playback of all the audio clips from his list then Speaker as well as all his Listeners will begin performing their " +
            "default behaviours and playback audio clips from 'DefaultBehaviourAudioClips' list located inside 'HumanoidAiAudioPlayer' script.";

        [Tooltip("Drag and drop all the 'AI agents' listening to this AI agent.")]
        public CoreAiBehaviour[] Listeners;

        private void Awake()
        {
            for (int i = 0; i < Listeners.Length; i++)
            {
                if(Listeners[i].Components.AiNonCombatChatterComponent != null)
                {
                    Listeners[i].Components.AiNonCombatChatterComponent.gameObject.SetActive(true);
                    Listeners[i].Components.AiNonCombatChatterComponent.DeactivateFunctioning = true;
                }
            }
        }
        public void StopChatting()
        {
            for (int i = 0; i < Listeners.Length; i++)
            {
                if (Listeners[i].Components.AiNonCombatChatterComponent != null)
                {
                    Listeners[i].PlayDefaultBehaviourSoundsNow = true;
                    Listeners[i].Components.AiNonCombatChatterComponent.gameObject.SetActive(false);
                }
            }
        }
    }
}