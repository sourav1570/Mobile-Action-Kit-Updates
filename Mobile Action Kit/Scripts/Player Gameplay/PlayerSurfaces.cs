using UnityEngine;

namespace MobileActionKit
{
    public class PlayerSurfaces : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This Script is Responsible To Change the Player Walk and Run Sounds Within the Surface Area For Example : Metal Surface , Grass Surface etc... ";
        [Space(10)]

        [Tooltip("Reference to the PlayerManager script for player-related functions.")]
        public PlayerManager PlayerManagerScript;

        [Tooltip("Reference to the FirstPersonController script for movement-related functions.")]
        public FirstPersonController FirstPersonControllerScript;

        [System.Serializable]
        public class AudioSourcesProperties
        {
            [Tooltip("Audio source for the walking sound.")]
            public AudioSource WalkAudioSourceComponent;

            [Tooltip("Audio source for the running sound.")]
            public AudioSource RunAudioSourceComponent;

            [Tooltip("Audio source for the jump start sound.")]
            public AudioSource JumpStartAudioSourceComponent;

            [Tooltip("Audio source for the jump landing sound.")]
            public AudioSource JumpLandAudioSourceComponent;
        }

        [System.Serializable]
        public class AudioClipProperties
        {
            [Tooltip("Audio clip to be played when walking.")]
            public AudioClip WalkAudioClip;

            [Tooltip("Audio clip to be played when running.")]
            public AudioClip RunAudioClip;

            [Tooltip("Audio clip to be played when starting a jump.")]
            public AudioClip JumpStartAudioClip;

            [Tooltip("Audio clip to be played when landing after a jump.")]
            public AudioClip JumpLandAudioClip;
        }

        [System.Serializable]
        public class TimeStepsProperties
        {
            [Tooltip("Time interval between each footstep sound while standing and running.")]
            public float TimeBetweenStandRunSteps = 0.4f;

            [Tooltip("Time interval between each footstep sound while crouching and running.")]
            public float TimeBetweenCrouchRunSteps = 0.8f;

            [Tooltip("Time interval between each footstep sound while standing and walking.")]
            public float TimeBetweenStandWalkSteps = 0.6f;

            [Tooltip("Time interval between each footstep sound while crouching and walking.")]
            public float TimeBetweenCrouchWalkSteps = 0.8f;

            [Tooltip("Time interval between each footstep sound while aiming and standing.")]
            public float TimeBetweenAimedStandSteps = 1f;

            [Tooltip("Time interval between each footstep sound while aiming and crouching.")]
            public float TimeBetweenAimedCrouchSteps = 1.2f;
        }

        [Tooltip("Contains references to the audio sources for different movement sounds.")]
        public AudioSourcesProperties AudioSourceComponents;

        [Tooltip("Contains references to the audio clips for different movement sounds.")]
        public AudioClipProperties AudioClips;

        [Tooltip("Stores footstep time intervals based on different movement states.")]
        public TimeStepsProperties TimeSteps;


        private AudioClip CurrentWalkClip;
        private AudioClip CurrentRunClip;
        private AudioClip CurrentJumpStartClip;
        private AudioClip CurrentJumpLandClip;

        private float PrevTimeBetweenWalkStandSteps;
        private float PrevTimeBetweenWalkCrouchSteps;
        private float PrevTimeBetweenRunStandSteps;
        private float PrevTimeBetweenRunCrouchSteps;
        private float PrevTimeBetweenStandStepsAimed;
        private float PrevTimeBetweenCrouchStepsAimed;

        void Start()
        {
            CurrentWalkClip = AudioSourceComponents.WalkAudioSourceComponent.clip;
            CurrentRunClip = AudioSourceComponents.RunAudioSourceComponent.clip;
            if (AudioSourceComponents.JumpStartAudioSourceComponent != null)
            {
                CurrentJumpStartClip = AudioSourceComponents.JumpStartAudioSourceComponent.clip;
            }
            if (AudioSourceComponents.JumpLandAudioSourceComponent != null)
            {
                CurrentJumpLandClip = AudioSourceComponents.JumpLandAudioSourceComponent.clip;
            }
            StorePreviousValues();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                //  joystick.Timer = 0f;
                // PlayerManagerScript.Timer = 0f;
                ModifyNewValues();
                AudioSourceComponents.WalkAudioSourceComponent.clip = AudioClips.WalkAudioClip;
                AudioSourceComponents.RunAudioSourceComponent.clip = AudioClips.RunAudioClip;

                if (AudioClips.JumpStartAudioClip != null)
                {
                    AudioSourceComponents.JumpStartAudioSourceComponent.clip = AudioClips.JumpStartAudioClip;
                }
                if (AudioClips.JumpLandAudioClip != null)
                {
                    AudioSourceComponents.JumpLandAudioSourceComponent.clip = AudioClips.JumpLandAudioClip;
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                ResetAllValues();
                AudioSourceComponents.WalkAudioSourceComponent.clip = CurrentWalkClip;
                AudioSourceComponents.RunAudioSourceComponent.clip = CurrentRunClip;

                if (CurrentJumpStartClip != null)
                {
                    AudioSourceComponents.JumpStartAudioSourceComponent.clip = CurrentJumpStartClip;
                }
                if (CurrentJumpLandClip != null)
                {
                    AudioSourceComponents.JumpLandAudioSourceComponent.clip = CurrentJumpLandClip;
                }
            }
        }
        public void StorePreviousValues()
        {
            PrevTimeBetweenWalkStandSteps = FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenStandWalkSteps;
            PrevTimeBetweenWalkCrouchSteps = FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenCrouchWalkSteps;
            PrevTimeBetweenRunStandSteps = FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenStandRunSteps;
            PrevTimeBetweenRunCrouchSteps = FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenCrouchRunSteps;
            PrevTimeBetweenStandStepsAimed = FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenAimedStandSteps;
            PrevTimeBetweenCrouchStepsAimed = FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenAimedCrouchSteps;
        }
        public void ModifyNewValues()
        {
            FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenStandWalkSteps = TimeSteps.TimeBetweenStandWalkSteps;
            FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenCrouchWalkSteps = TimeSteps.TimeBetweenCrouchWalkSteps;
            FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenStandRunSteps = TimeSteps.TimeBetweenStandRunSteps;
            FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenCrouchRunSteps = TimeSteps.TimeBetweenCrouchRunSteps;
            FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenAimedStandSteps = TimeSteps.TimeBetweenAimedStandSteps;
            FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenAimedCrouchSteps = TimeSteps.TimeBetweenAimedCrouchSteps;
        }
        public void ResetAllValues()
        {
            FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenStandWalkSteps = PrevTimeBetweenWalkStandSteps;
            FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenCrouchWalkSteps = PrevTimeBetweenWalkCrouchSteps;
            FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenStandRunSteps = PrevTimeBetweenRunStandSteps;
            FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenCrouchRunSteps = PrevTimeBetweenRunCrouchSteps;
            FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenAimedStandSteps = PrevTimeBetweenStandStepsAimed;
            FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenAimedCrouchSteps = PrevTimeBetweenCrouchStepsAimed;
        }

    }
}