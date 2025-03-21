using UnityEngine;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class RotateAndReset_GameObject : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script enables object rotation using mouse input. It can reset the rotation smoothly when released and limit rotation angles. Optionally, rotation can be restricted to a UI-defined area.";

        [HideInInspector]
        public Quaternion defaultRotation;

        [Tooltip("The speed at which the object rotates when the mouse moves.")]
        public float rotationSpeed = 10f;

        [Tooltip("If enabled, the object will reset to its default rotation when the mouse is released.")]
        public bool ResetRotation = true;

        [Tooltip("The speed at which the object resets to its default rotation.")]
        public float resetSpeed = 2f;

        [Tooltip("Enable to allow rotation around the X-axis.")]
        public bool rotateX = true;

        [Tooltip("Enable to allow rotation around the Y-axis.")]
        public bool rotateY = true;

        [Tooltip("Enable to allow rotation around the Z-axis.")]
        public bool rotateZ = true;

        [Tooltip("Minimum allowed rotation angle on the X-axis.")]
        public float minXRotation = -45f;

        [Tooltip("Maximum allowed rotation angle on the X-axis.")]
        public float maxXRotation = 45f;

        [Tooltip("Minimum allowed rotation angle on the Y-axis.")]
        public float minYRotation = -45f;

        [Tooltip("Maximum allowed rotation angle on the Y-axis.")]
        public float maxYRotation = 45f;

        [Tooltip("Minimum allowed rotation angle on the Z-axis.")]
        public float minZRotation = -45f;

        [Tooltip("Maximum allowed rotation angle on the Z-axis.")]
        public float maxZRotation = 45f;

        private float speedMultiplier = 100f;
        private bool isRotating = false;
        private bool isInsideUI = false;

        [Tooltip("Enable this to allow rotation only when the mouse is inside the specified UI area.")]
        public bool UseRotationArea = false;

        [Tooltip("The UI RectTransform that defines the area where rotation is allowed.")]
        public RectTransform rotationArea; // Assign the UI Image

        private Canvas canvas; // Reference to the Canvas


        void Start()
        {
            defaultRotation = transform.rotation;

            if(rotationArea != null)
            {
                canvas = rotationArea?.GetComponentInParent<Canvas>(); // Find parent Canvas

            }
        }

        void Update()
        {
            if(UseRotationArea == true && rotationArea != null)
            {
                isInsideUI = IsPointerInsideUI();

                if (!isInsideUI)
                {
                    isRotating = false;

                    // If ResetRotation is enabled, smoothly reset rotation when outside UI
                    if (ResetRotation)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, resetSpeed * Time.deltaTime);
                    }

                    return;
                }
            }
           

            if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * speedMultiplier * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * speedMultiplier * Time.deltaTime;

                Vector3 newRotation = transform.rotation.eulerAngles;

                if (rotateX)
                {
                    newRotation.x += mouseY;
                    newRotation.x = Mathf.Clamp(newRotation.x, minXRotation, maxXRotation);
                }
                if (rotateY)
                {
                    newRotation.y -= mouseX;
                    newRotation.y = Mathf.Clamp(newRotation.y, minYRotation, maxYRotation);
                }
                if (rotateZ)
                {
                    newRotation.z += mouseX;
                    newRotation.z = Mathf.Clamp(newRotation.z, minZRotation, maxZRotation);
                }

                transform.rotation = Quaternion.Euler(newRotation);
                isRotating = true;
            }
            else
            {
                isRotating = false;
            }

            if (!isRotating && ResetRotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, resetSpeed * Time.deltaTime);
            }
        }

        private bool IsPointerInsideUI()
        {
            if (rotationArea == null || canvas == null)
                return false;

            return RectTransformUtility.RectangleContainsScreenPoint(rotationArea, Input.mousePosition, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera);
        }
    }
}






//using UnityEngine;


//namespace MobileActionKit
//{
//    public class RotateAndReset_GameObject : MonoBehaviour
//    {
//        [HideInInspector]
//        public Quaternion defaultRotation;

//        // Variables to control rotation speed and reset speed
//        public float rotationSpeed = 10f;
//        public bool ResetRotation = true;
//        public float resetSpeed = 2f;

//        // Toggle which axes to rotate on
//        public bool rotateX = true;
//        public bool rotateY = true;
//        public bool rotateZ = true;

//        // Clamping ranges for X, Y, and Z rotation
//        public float minXRotation = -45f;
//        public float maxXRotation = 45f;
//        public float minYRotation = -45f;
//        public float maxYRotation = 45f;
//        public float minZRotation = -45f;
//        public float maxZRotation = 45f;

//        // Multiplier to amplify the rotation effect
//        private float speedMultiplier = 100f;

//        // Flag to check if the object is being rotated
//        private bool isRotating = false;

//        void Start()
//        {
//            // Store the initial rotation of the GameObject
//            defaultRotation = transform.rotation;
//        }
//        void Update()
//        {
//            // Check if the mouse button is held down
//            if (Input.GetMouseButton(0))
//            {
//                // Get the mouse movement
//                float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * speedMultiplier * Time.deltaTime;
//                float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * speedMultiplier * Time.deltaTime;

//                Vector3 newRotation = transform.rotation.eulerAngles;

//                // Apply rotation based on enabled axes and clamp them
//                if (rotateX)
//                {
//                    newRotation.x += mouseY;
//                    newRotation.x = Mathf.Clamp(newRotation.x, minXRotation, maxXRotation);
//                }
//                if (rotateY)
//                {
//                    newRotation.y -= mouseX;
//                    newRotation.y = Mathf.Clamp(newRotation.y, minYRotation, maxYRotation);
//                }
//                if (rotateZ)
//                {
//                    newRotation.z += mouseX;
//                    newRotation.z = Mathf.Clamp(newRotation.z, minZRotation, maxZRotation);
//                }

//                // Apply the clamped rotation to the object
//                transform.rotation = Quaternion.Euler(newRotation);

//                isRotating = true;
//            }
//            else
//            {
//                isRotating = false;
//            }

//            // If not rotating, reset to default rotation smoothly
//            if (!isRotating && ResetRotation == true)
//            {
//                transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, resetSpeed * Time.deltaTime);
//            }
//        }
//    }
//}