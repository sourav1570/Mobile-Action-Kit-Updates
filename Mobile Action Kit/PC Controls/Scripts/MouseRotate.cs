using UnityEngine;


namespace MobileActionKit
{
    public class MouseRotate : MonoBehaviour
    {
        public float rotationSpeed = 100.0f;

        void Update()
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, -mouseX, Space.World);
            transform.Rotate(Vector3.right, mouseY, Space.Self);
        }
    }
}
