using UnityEngine;

namespace MobileActionKit
{
    public class RaycastDebug : MonoBehaviour
    {
        public Transform targetObject; // The object to raycast towards

        public string ObjectHitting;

        private void Update()
        {
            // Create a raycast from the current position towards the target object
            Ray ray = new Ray(transform.position, (targetObject.position - transform.position).normalized);
            RaycastHit hit;

            // Perform the raycast and check if it hits an object
            if (Physics.Raycast(ray, out hit))
            {
                // Log the name of the object that the raycast hits
                ObjectHitting = hit.collider.gameObject.name;
            }
        }
    }
}