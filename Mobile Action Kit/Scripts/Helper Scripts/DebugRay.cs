using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class DebugRay : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This Script Creates a Straight Ray From The Current Position.";
        [Space(10)]

        public Camera TargetCamera; // Reference to the target camera
        public float MaxRayDistance = 1000f;
        public Color RayColor = Color.blue;
        public float HitDistance; // Field to store the distance to the hit point

        public void ResettingDescription()
        {
            ScriptInfo = "This Script Creates a Straight Ray From The Current Position.";
        }

        void Update()
        {
            // Ensure a TargetCamera is assigned
            if (TargetCamera != null)
            {
                // Check if the TargetCamera is currently active
                if (TargetCamera.gameObject.activeInHierarchy)
                {
                    // Draw the debug ray from the TargetCamera's perspective
                    Debug.DrawRay(transform.position, transform.forward * MaxRayDistance, RayColor);

                    // Create a ray starting at the current position and going forward
                    Ray ray = new Ray(transform.position, transform.forward);
                    RaycastHit hit;

                    // Perform the raycast
                    if (Physics.Raycast(ray, out hit, MaxRayDistance))
                    {
                        // If the ray hits something, update the HitDistance
                        HitDistance = hit.distance;
                    }
                    else
                    {
                        // If the ray doesn't hit anything, set HitDistance to zero
                        HitDistance = 0;
                    }
                }
            }
            }

          
    }
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace MobileActionKit
//{
//    public class DebugRay : MonoBehaviour
//    {
//        [TextArea]
//        [ContextMenuItem("Reset Description", "ResettingDescription")]
//        public string ScriptInfo = "This Script Creates a Straight Ray From The Current Position.";
//        [Space(10)]

//        public Camera currentCamera;
//        public float MaxRayDistance = 1000f;
//        public Color RayColor = Color.blue;
//        public float HitDistance; // Field to store the distance to the hit point
//        public string TargetCameraTag = "MainCamera"; // Tag for the target camera

//        public void ResettingDescription()
//        {
//            ScriptInfo = "This Script Creates a Straight Ray From The Current Position.";
//        }

//        void Update()
//        {

//            if (currentCamera != null && currentCamera.CompareTag(TargetCameraTag))
//            {
//                // Draw the debug ray
//                Debug.DrawRay(transform.position, transform.forward * MaxRayDistance, RayColor);
//            }

//            // Create a ray starting at the current position and going forward
//            Ray ray = new Ray(transform.position, transform.forward);
//            RaycastHit hit;

//            // Perform the raycast
//            if (Physics.Raycast(ray, out hit, MaxRayDistance))
//            {
//                // If the ray hits something, update the HitDistance
//                HitDistance = hit.distance;
//            }
//            else
//            {
//                // If the ray doesn't hit anything, set HitDistance to zero
//                HitDistance = 0;
//            }
//        }
//    }
//}
