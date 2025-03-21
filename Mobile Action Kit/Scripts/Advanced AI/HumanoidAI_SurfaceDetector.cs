using System.Collections;
using System.Collections.Generic;
using MobileActionKit;
using UnityEngine;


namespace MobileActionKit
{
    public class HumanoidAI_SurfaceDetector : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "The HumanoidAI_SurfaceDetector script offers a sophisticated solution for enhancing AI behavior through dynamic footstep sound adjustments based on terrain/Mesh types. " +
            "By employing tags and raycasting, this approach continuously checks ahead of the AI agent at intervals set by Min/Max timers. It provides detailed environmental responsiveness, " +
            "allowing AI characters to interact realistically with various surfaces like grass, concrete, or wood. However, due to its continuous raycasting nature, developers should consider performance implications, " +
            "particularly on mobile platforms where it may slightly impact FPS. For smaller environments, alternative script like 'HumanoidAISurfaces' which do collider-based detection may be more performance-efficient, " +
            "ensuring optimal gameplay experience across different device capabilities.";

        [Tooltip("Length of the raycast that detects surfaces in front of the AI character. Adjust this value based on how far ahead you want the AI to detect surfaces.")]
        public float raycastLength = 2.0f;
        [Tooltip("Minimum time interval between consecutive raycasts. Controls how often the script performs raycast checks.")]
        public float minTimeToRaycast = 0.5f;

        [Tooltip("Maximum time interval between consecutive raycasts. Controls how often the script performs raycast checks.")]
        public float maxTimeToRaycast = 1.0f;

        [Tooltip("Layers to ignore during raycasting. Objects on these layers will not trigger surface detections. Useful for ignoring irrelevant objects like UI elements or other AI agents.")]
        public LayerMask ignoredLayers;

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
            [Tooltip("Audio clip to play when the AI walks on surfaces with the corresponding tag. For example, assign a rustling sound for 'Grass' or a hard impact sound for 'Concrete'.")]
            public AudioClip footStepAudioClip;
        }

        [Tooltip("List of Custom footstep sounds for different surface tags. Populate this list with tags and corresponding audio clips to simulate realistic footstep behavior based on terrain type.")]
        public List<CustomFootStepSoundsClass> customFootStepSounds = new List<CustomFootStepSoundsClass>();

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
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
                if (Physics.Raycast(transform.position, transform.forward, out hit, raycastLength, ignoredLayers))
                {
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
                            // Set the footstep audio clip for the AI component
                            transform.root.GetComponent<CoreAiBehaviour>().Components.HumanoidAiAudioPlayerComponent.DefaultFootStepSounds.FootStepAudioClip = footStepSound.footStepAudioClip;
                            foundMatchingTag = true;
                            break; // Exit loop after playing the first matching footstep sound
                        }
                    }

                    // If no matching tag was found, use the stored footstep audio clip
                    if (!foundMatchingTag)
                    {
                        transform.root.GetComponent<CoreAiBehaviour>().Components.HumanoidAiAudioPlayerComponent.DefaultFootStepSounds.FootStepAudioClip = transform.root.GetComponent<CoreAiBehaviour>()
                            .Components.HumanoidAiAudioPlayerComponent.StoredFootStepAudioClip;
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
    }
}