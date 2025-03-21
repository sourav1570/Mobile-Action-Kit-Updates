using System.Collections.Generic;
using UnityEngine;


namespace MobileActionKit
{
    public class RaycastVisualizer : MonoBehaviour
    {
        [System.Serializable]
        public class RaycastConfig
        {
            public GameObject raycastOrigin;
            public float raycastLength = 10f;
            public Color rayColor = Color.red;
            public bool enableVisualRaycasts = true;
        }

        public List<RaycastConfig> raycastConfigs = new List<RaycastConfig>();
        private string targetCameraTag = "MainCamera"; // Specify which camera tag to use for rendering rays

        private void OnDrawGizmos()
        {
            // Ensure gizmos are only drawn in the Scene view and not in Game view during play mode
            //if (!Application.isPlaying)
            //{
            Camera currentCamera = Camera.current;

            // Check if the camera has the target tag
            if (currentCamera != null && currentCamera.CompareTag(targetCameraTag))
            {
                foreach (var config in raycastConfigs)
                {
                    if (config.enableVisualRaycasts && config.raycastOrigin != null)
                    {
                        Gizmos.color = config.rayColor;
                        Gizmos.DrawRay(config.raycastOrigin.transform.position, config.raycastOrigin.transform.forward * config.raycastLength);
                    }
                }
            }
            // }
        }
    }
}


//using UnityEditor;
//using UnityEngine;
//using System.Collections.Generic;

//public class RaycastVisualizer : MonoBehaviour
//{
//    [System.Serializable]
//    public class RaycastConfig
//    {
//        public GameObject raycastOrigin;
//        public float raycastLength = 10f;
//        public Color rayColor = Color.red;
//        public bool enableVisualRaycasts = true;
//    }

//    // List of raycast configurations
//    public List<RaycastConfig> raycastConfigs = new List<RaycastConfig>();

//    // This will handle the drawing of the rays in the Scene view only
//    private void OnDrawGizmos()
//    {
//        // Ensure gizmos are only drawn in the Scene view and not in Game view during play mode
//        if (!Application.isPlaying)
//        {
//            // Iterate over each raycast configuration
//            foreach (var config in raycastConfigs)
//            {
//                // If raycasting visualization is enabled and the raycastOrigin is assigned
//                if (config.enableVisualRaycasts && config.raycastOrigin != null)
//                {
//                    // Set Gizmo color
//                    Gizmos.color = config.rayColor;

//                    // Draw a ray in the Scene view from the raycastOrigin's position
//                    Gizmos.DrawRay(config.raycastOrigin.transform.position, config.raycastOrigin.transform.forward * config.raycastLength);
//                }
//            }
//        }
//    }
//}




//using UnityEditor;
//using UnityEngine;
//using System.Collections.Generic;

//public class RaycastVisualizer : MonoBehaviour
//{
//    [System.Serializable]
//    public class RaycastConfig
//    {
//        public GameObject raycastOrigin;
//        public float raycastLength = 10f;
//        public Color rayColor = Color.red;
//        public bool enableVisualRaycasts = true;
//    }

//    // List of raycast configurations
//    public List<RaycastConfig> raycastConfigs = new List<RaycastConfig>();

//    // This will handle the drawing of the rays
//    private void OnDrawGizmos()
//    {
//        // Iterate over each raycast configuration
//        foreach (var config in raycastConfigs)
//        {
//            // If raycasting visualization is enabled and the raycastOrigin is assigned
//            if (config.enableVisualRaycasts && config.raycastOrigin != null)
//            {
//                // Set Gizmo color
//                Gizmos.color = config.rayColor;

//                // Draw a ray in the Scene view from the raycastOrigin's position
//                Gizmos.DrawRay(config.raycastOrigin.transform.position, config.raycastOrigin.transform.forward * config.raycastLength);
//            }
//        }
//    }
//}

//[CustomEditor(typeof(RaycastVisualizer))]
//public class RaycastVisualizerEditor : Editor
//{
//    private RaycastVisualizer raycastVisualizer;

//    private void OnEnable()
//    {
//        // Get the RaycastVisualizer component
//        raycastVisualizer = (RaycastVisualizer)target;
//    }

//    public override void OnInspectorGUI()
//    {
//        // Hide the entire RaycastVisualizer component by commenting out or removing this line if needed
//        // DrawDefaultInspector();

//        // If you want to hide everything in the inspector, just return early
//        // return;

//        // Show some specific properties or change the layout
//        GUILayout.Space(10); // For UI spacing

//        // Conditionally hide or show the raycast configuration list
//        if (raycastVisualizer != null)
//        {
//            // Display the raycast configurations list in the inspector
//            SerializedProperty raycastConfigs = serializedObject.FindProperty("raycastConfigs");

//            // Check if the raycastConfigs list is not null and display it
//            EditorGUILayout.PropertyField(raycastConfigs, new GUIContent("Raycast Configurations"), true);
//        }

//        // Apply modified properties to the serialized object
//        serializedObject.ApplyModifiedProperties();
//    }

//    private void OnSceneGUI()
//    {
//        // Iterate over each raycast configuration
//        foreach (var config in raycastVisualizer.raycastConfigs)
//        {
//            // If raycast visualization is enabled and the origin is set
//            if (config.enableVisualRaycasts && config.raycastOrigin != null)
//            {
//                // Get the position and direction for the ray
//                Vector3 origin = config.raycastOrigin.transform.position;
//                Vector3 direction = config.raycastOrigin.transform.forward * config.raycastLength;

//                // Draw the ray in the scene view
//                Handles.color = config.rayColor;
//                Handles.DrawLine(origin, origin + direction);

//                // Optionally, draw an arrowhead to indicate direction
//                Handles.ArrowHandleCap(0, origin + direction, Quaternion.LookRotation(direction), 1f, EventType.Repaint);
//            }
//        }
//    }
//}








//using UnityEditor;
//using UnityEngine;

//public class RaycastVisualizer : MonoBehaviour
//{
//    // Public fields for raycast properties
//    public float raycastLength = 10f;
//    public Color rayColor = Color.red;
//    public GameObject raycastOrigin;
//    public bool enableVisualRaycasts = true;

//    // This will handle the drawing of the ray
//    private void OnDrawGizmos()
//    {
//        // If raycasting visualization is enabled and the raycastOrigin is assigned
//        if (enableVisualRaycasts && raycastOrigin != null)
//        {
//            // Set Gizmo color
//            Gizmos.color = rayColor;

//            // Draw a ray in the Scene view from the raycastOrigin's position
//            Gizmos.DrawRay(raycastOrigin.transform.position, raycastOrigin.transform.forward * raycastLength);
//        }
//    }
//}

//[CustomEditor(typeof(RaycastVisualizer))]
//public class RaycastVisualizerEditor : Editor
//{
//    private RaycastVisualizer raycastVisualizer;

//    private void OnEnable()
//    {
//        // Get the RaycastVisualizer component
//        raycastVisualizer = (RaycastVisualizer)target;
//    }

//    public override void OnInspectorGUI()
//    {
//        // Draw default inspector
//        DrawDefaultInspector();

//        // Space for better UI
//        GUILayout.Space(10);

//        // If the RaycastVisualizer is not null, display custom fields
//        if (raycastVisualizer != null)
//        {
//    // Toggle to enable/disable visualization
//    raycastVisualizer.enableVisualRaycasts = EditorGUILayout.Toggle("Enable Visual Raycasts", raycastVisualizer.enableVisualRaycasts);

//    // Field for raycast length
//    raycastVisualizer.raycastLength = EditorGUILayout.FloatField("Raycast Length", raycastVisualizer.raycastLength);

//    // Color picker for ray color
//    raycastVisualizer.rayColor = EditorGUILayout.ColorField("Ray Color", raycastVisualizer.rayColor);

//    // GameObject field for raycast origin
//    raycastVisualizer.raycastOrigin = (GameObject)EditorGUILayout.ObjectField("Raycast Origin", raycastVisualizer.raycastOrigin, typeof(GameObject), true);
//}

//// Apply modified properties to the serialized object
//serializedObject.ApplyModifiedProperties();
//    }

//    private void OnSceneGUI()
//{
//    // If raycast visualization is enabled and the origin is set
//    if (raycastVisualizer.enableVisualRaycasts && raycastVisualizer.raycastOrigin != null)
//    {
//        // Get the position and direction for the ray
//        Vector3 origin = raycastVisualizer.raycastOrigin.transform.position;
//        Vector3 direction = raycastVisualizer.raycastOrigin.transform.forward * raycastVisualizer.raycastLength;

//        // Draw the ray in the scene view
//        Handles.color = raycastVisualizer.rayColor;
//        Handles.DrawLine(origin, origin + direction);

//        // Optionally, draw an arrowhead to indicate direction
//        Handles.ArrowHandleCap(0, origin + direction, Quaternion.LookRotation(direction), 1f, EventType.Repaint);
//    }
//}
//}
