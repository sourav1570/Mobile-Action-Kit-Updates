using UnityEngine;

namespace MobileActionKit
{
    public class PcMouseLook : MonoBehaviour
    {
        public enum RotationAxes { HorizontalAndVertical, Horizontal, Vertical }

        public RotationAxes axes = RotationAxes.HorizontalAndVertical;
        public bool HideCursor = false; // Toggle to hide the cursor or not
        public float smoothTime = 0.05f; // Adjust for more/less smoothing

        [System.Serializable]
        public class WeaponDefaultRotationClass
        {
            public float XSensitivitySpeed = 140F;
            public float YSensitivitySpeed = 140F;
            public float MinVerticalAxis = -60F;
            public float MaxVerticalAxis = 60F;
            public float MinHorizontalAxis = -360F;
            public float MaxHorizontalAxis = 360F;
        }

        public WeaponDefaultRotationClass WeaponDefaultMouseAxes = new WeaponDefaultRotationClass();
        public WeaponDefaultRotationClass WeaponAimedMouseAxes = new WeaponDefaultRotationClass();

        private float smoothRotationX = 0f;
        private float smoothRotationY = 0f;
      
        private float rotationX = 0F;
        private float rotationY = 0F;
        private Quaternion initialRotation; // Store the initial rotation

        private void Start()
        {
            // Capture the initial rotation
            initialRotation = transform.localRotation;

            if (HideCursor)
            {
                Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
                Cursor.visible = false; // Hide the cursor
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void Update()
        {
            if (PlayerManager.instance != null)
            {
                if (PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                {
                    if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == false)
                    {
                        MouseInputAxes(WeaponDefaultMouseAxes.XSensitivitySpeed, WeaponDefaultMouseAxes.YSensitivitySpeed, WeaponDefaultMouseAxes.MinVerticalAxis,
                        WeaponDefaultMouseAxes.MaxVerticalAxis, WeaponDefaultMouseAxes.MinHorizontalAxis, WeaponDefaultMouseAxes.MaxHorizontalAxis);
                    }
                    else
                    {
                        MouseInputAxes(WeaponAimedMouseAxes.XSensitivitySpeed, WeaponAimedMouseAxes.YSensitivitySpeed, WeaponAimedMouseAxes.MinVerticalAxis,
                        WeaponAimedMouseAxes.MaxVerticalAxis, WeaponAimedMouseAxes.MinHorizontalAxis, WeaponAimedMouseAxes.MaxHorizontalAxis);
                    }
                }

            }
            else
            {
                MouseInputAxes(WeaponDefaultMouseAxes.XSensitivitySpeed, WeaponDefaultMouseAxes.YSensitivitySpeed, WeaponDefaultMouseAxes.MinVerticalAxis,
                WeaponDefaultMouseAxes.MaxVerticalAxis, WeaponDefaultMouseAxes.MinHorizontalAxis, WeaponDefaultMouseAxes.MaxHorizontalAxis);
            }


        }
        //public void MouseInputAxes(float XSensitivitySpeed, float YSensitivitySpeed, float MinVerticalAxis, float MaxVerticalAxis,
        //    float MinHorizontalAxis, float MaxHorizontalAxis)
        //{
        //    // Read the mouse input axes
        //    float mouseX = Input.GetAxis("Mouse X") * XSensitivitySpeed * Time.deltaTime;
        //    float mouseY = Input.GetAxis("Mouse Y") * YSensitivitySpeed * Time.deltaTime;

        //    // Calculate new rotations based on the chosen axes
        //    if (axes == RotationAxes.HorizontalAndVertical || axes == RotationAxes.Horizontal)
        //    {
        //        rotationY += mouseX;
        //        rotationY = ClampAngle(rotationY, MinHorizontalAxis, MaxHorizontalAxis);
        //    }

        //    if (axes == RotationAxes.HorizontalAndVertical || axes == RotationAxes.Vertical)
        //    {
        //        rotationX -= mouseY; // Invert the mouseY input
        //        rotationX = ClampAngle(rotationX, MinVerticalAxis, MaxVerticalAxis);
        //    }

        //    // Create the local rotation Quaternions based on the initial rotation
        //    Quaternion localRotationX = initialRotation * Quaternion.Euler(rotationX, 0f, 0f);
        //    Quaternion localRotationY = initialRotation * Quaternion.Euler(0f, rotationY, 0f);

        //    // Apply the rotations
        //    transform.localRotation = localRotationY * localRotationX;
        //}


        public void MouseInputAxes(float XSensitivitySpeed, float YSensitivitySpeed, float MinVerticalAxis, float MaxVerticalAxis,
            float MinHorizontalAxis, float MaxHorizontalAxis)
        {
            float mouseX = Input.GetAxis("Mouse X") * XSensitivitySpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * YSensitivitySpeed * Time.deltaTime;

            rotationY += mouseX;
            rotationY = ClampAngle(rotationY, MinHorizontalAxis, MaxHorizontalAxis);

            rotationX -= mouseY;
            rotationX = ClampAngle(rotationX, MinVerticalAxis, MaxVerticalAxis);

            // Smooth the rotation
            smoothRotationX = Mathf.Lerp(smoothRotationX, rotationX, smoothTime);
            smoothRotationY = Mathf.Lerp(smoothRotationY, rotationY, smoothTime);

            Quaternion localRotationX = initialRotation * Quaternion.Euler(smoothRotationX, 0f, 0f);
            Quaternion localRotationY = initialRotation * Quaternion.Euler(0f, smoothRotationY, 0f);

            transform.localRotation = localRotationY * localRotationX;
        }


        public static float ClampAngle(float angle, float min, float max)
        {
            // Clamp the angle between min and max values
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
