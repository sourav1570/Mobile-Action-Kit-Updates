using System.Collections;
using System.Collections.Generic;
using MobileActionKit;
using UnityEngine;


namespace MobileActionKit
{
    public class PlayerSurfaceDetector : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "The PlayerSurfaceDetector script offers a sophisticated solution for enhancing player behavior through dynamic footstep sound adjustments based on terrain/Mesh types. " +
            "By employing tags and raycasting, this approach continuously checks ahead of the Player at intervals set by Min/Max timers. It provides detailed environmental responsiveness, " +
            "allowing player to interact realistically with various surfaces like grass, concrete, or wood. However, due to its continuous raycasting nature, developers should consider performance implications, " +
            "particularly on mobile platforms where it may slightly impact FPS. For smaller environments, alternative script like 'PlayerSurfaces' which do collider-based detection may be more performance-efficient, " +
            "ensuring optimal gameplay experience across different device capabilities.";

        [Tooltip("Reference to the FirstPersonController script for player movement and footstep handling.")]
        public FirstPersonController FirstPersonControllerScript;

        [System.Serializable]
        public class AudioSourcesProperties
        {
            [Tooltip("Audio source for walking sounds.")]
            public AudioSource WalkAudioSourceComponent;
            [Tooltip("Audio source for running sounds.")]
            public AudioSource RunAudioSourceComponent;
            [Tooltip("Audio source for jump start sounds.")]
            public AudioSource JumpStartAudioSourceComponent;
            [Tooltip("Audio source for jump landing sounds.")]
            public AudioSource JumpLandAudioSourceComponent;
        }

        [Tooltip("Holds references to different audio sources used for movement sounds.")]
        public AudioSourcesProperties AudioSourceComponents;

        [Tooltip("Length of the raycast that detects surfaces in front of the Player. Adjust this value based on how far ahead you want the Player to detect surfaces.")]
        public float raycastLength = 2.0f;
        [Tooltip("Minimum time interval between consecutive raycasts. Controls how often the script performs raycast checks.")]
        public float minTimeToRaycast = 0.5f;

        [Tooltip("Maximum time interval between consecutive raycasts. Controls how often the script performs raycast checks.")]
        public float maxTimeToRaycast = 1.0f;

        [Tooltip("Visible layers during raycasting. Objects on these layers will trigger surface detections.")]
        public LayerMask VisibleLayers;

        [Tooltip("Toggle to enable or disable visual debugging of raycasts in the Scene view. When enabled (true), the raycast is visualized with raycastColor, during development and testing.")]
        public bool debugRaycast = true; // Toggle to enable/disable raycast debugging

        [Tooltip("Color used to visualize the raycast in the Scene view. Helps developers see where the raycast is directed during debugging.")]
        public Color raycastColor = Color.red; // Default raycast color

        // Class for custom footstep sounds
        [System.Serializable]
        public class CustomFootStepSoundsClass
        {
            [Tooltip("The tag representing a specific surface type (e.g., 'Grass', 'Concrete'). Assign tags to game objects in Unity to differentiate surfaces.")]
            public string tag;
          
            [Tooltip("Audio clip for walking on this surface type.")]
            public AudioClip WalkAudioClip;
            [Tooltip("Audio clip for running on this surface type.")]
            public AudioClip RunAudioClip;
            [Tooltip("Audio clip for jump start on this surface type.")]
            public AudioClip JumpStartAudioClip;
            [Tooltip("Audio clip for jump landing on this surface type.")]
            public AudioClip JumpLandAudioClip;

            [Tooltip("Time interval between each footstep while standing and running on this surface.")]
            public float TimeBetweenStandRunSteps = 0.4f;
            [Tooltip("Time interval between each footstep while crouching and running on this surface.")]
            public float TimeBetweenCrouchRunSteps = 0.8f;
            [Tooltip("Time interval between each footstep while standing and walking on this surface.")]
            public float TimeBetweenStandWalkSteps = 0.6f;
            [Tooltip("Time interval between each footstep while crouching and walking on this surface.")]
            public float TimeBetweenCrouchWalkSteps = 0.8f;
            [Tooltip("Time interval between each footstep while aiming and standing on this surface.")]
            public float TimeBetweenAimedStandSteps = 1f;
            [Tooltip("Time interval between each footstep while aiming and crouching on this surface.")]
            public float TimeBetweenAimedCrouchSteps = 1.2f;
        }

        [Tooltip("List of Custom footstep sounds for different surface tags. Populate this list with tags and corresponding audio clips to simulate realistic footstep behavior based on terrain type.")]
        public List<CustomFootStepSoundsClass> customFootStepSounds = new List<CustomFootStepSoundsClass>();


        private AudioClip DefaultWalkSounds;
        private AudioClip DefaultRunSounds;
        private AudioClip DefaultJumpStartSounds;
        private AudioClip DefaultJumpLandSounds;

        private float PrevTimeBetweenStandWalkSteps;
        private float PrevTimeBetweenCrouchWalkSteps;
        private float PrevTimeBetweenStandRunSteps;
        private float PrevTimeBetweenCrouchRunSteps;
        private float PrevTimeBetweenAimedStandSteps;
        private float PrevTimeBetweenAimedCrouchSteps;

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        
            DefaultWalkSounds = AudioSourceComponents.WalkAudioSourceComponent.clip;
            DefaultRunSounds = AudioSourceComponents.RunAudioSourceComponent.clip;
            DefaultJumpStartSounds = AudioSourceComponents.JumpStartAudioSourceComponent.clip;
            DefaultJumpLandSounds = AudioSourceComponents.JumpLandAudioSourceComponent.clip;

            StorePreviousValues();
            // Start the coroutine to perform raycasts at random intervals
            StartCoroutine(RaycastAtRandomIntervals());
        }

        private IEnumerator RaycastAtRandomIntervals()
        {
            while (true)
            {
                // Wait for a random amount of time between minTimeToRaycast and maxTimeToRaycast
                float waitTime = Random.Range(minTimeToRaycast, maxTimeToRaycast);
                yield return new WaitForSeconds(waitTime);

                // Perform the raycast
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, raycastLength, VisibleLayers))
                {
                   //Debug.Log(hit.transform.name);
                    // Visualize the raycast if debugRaycast is enabled
                    if (debugRaycast)
                    {
                        Debug.DrawRay(transform.position, transform.forward * raycastLength, raycastColor, 0.5f);
                    }

                    // Check if the hit object's tag matches any of the tags in customFootStepSounds
                    bool foundMatchingTag = false;
                    foreach (var footStepSound in customFootStepSounds)
                    {
                        if (hit.collider.gameObject.CompareTag(footStepSound.tag))
                        {
                            if (FirstPersonControllerScript != null)
                            {
                                FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenStandWalkSteps = footStepSound.TimeBetweenStandWalkSteps;
                                FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenCrouchWalkSteps = footStepSound.TimeBetweenCrouchWalkSteps;
                                FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenStandRunSteps = footStepSound.TimeBetweenStandRunSteps;
                                FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenCrouchRunSteps = footStepSound.TimeBetweenCrouchRunSteps;
                                FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenAimedStandSteps = footStepSound.TimeBetweenAimedStandSteps;
                                FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenAimedCrouchSteps = footStepSound.TimeBetweenAimedCrouchSteps;
                            }
                         
                            AudioSourceComponents.WalkAudioSourceComponent.clip = footStepSound.WalkAudioClip;
                            AudioSourceComponents.RunAudioSourceComponent.clip = footStepSound.RunAudioClip;
                            AudioSourceComponents.JumpStartAudioSourceComponent.clip = footStepSound.JumpStartAudioClip;
                            AudioSourceComponents.JumpLandAudioSourceComponent.clip = footStepSound.JumpLandAudioClip;


                            foundMatchingTag = true;
                            break; // Exit loop after playing the first matching footstep sound
                        }
                    }

                    // If no matching tag was found, use the stored footstep audio clip
                    if (!foundMatchingTag)
                    {
                        ResetAllValues();
                        AudioSourceComponents.WalkAudioSourceComponent.clip = DefaultWalkSounds;
                        AudioSourceComponents.RunAudioSourceComponent.clip = DefaultRunSounds;
                        AudioSourceComponents.JumpStartAudioSourceComponent.clip = DefaultJumpStartSounds;
                        AudioSourceComponents.JumpLandAudioSourceComponent.clip = DefaultJumpLandSounds;

                    }
                }
                else
                {

                    // Visualize the raycast to the maximum length if debugRaycast is enabled
                    if (debugRaycast)
                    {
                        Debug.DrawRay(transform.position, transform.forward * raycastLength, raycastColor, 0.5f);
                    }
                }
            }
        }
        public void StorePreviousValues()
        {
            if(FirstPersonControllerScript != null)
            {
                PrevTimeBetweenStandWalkSteps = FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenStandWalkSteps;
                PrevTimeBetweenCrouchWalkSteps = FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenCrouchWalkSteps;
                PrevTimeBetweenStandRunSteps = FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenStandRunSteps;
                PrevTimeBetweenCrouchRunSteps = FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenCrouchRunSteps;
                PrevTimeBetweenAimedStandSteps = FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenAimedStandSteps;
                PrevTimeBetweenAimedCrouchSteps = FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenAimedCrouchSteps;
            }
            
        }
        public void ResetAllValues()
        {
            if (FirstPersonControllerScript != null)
            {
                FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenStandWalkSteps = PrevTimeBetweenStandWalkSteps;
                FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenCrouchWalkSteps = PrevTimeBetweenCrouchWalkSteps;
                FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenStandRunSteps = PrevTimeBetweenStandRunSteps;
                FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenCrouchRunSteps = PrevTimeBetweenCrouchRunSteps;
                FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenAimedStandSteps = PrevTimeBetweenAimedStandSteps;
                FirstPersonControllerScript.WalkAndRunSounds.TimeBetweenAimedCrouchSteps = PrevTimeBetweenAimedCrouchSteps;
            }
        }
    }
}