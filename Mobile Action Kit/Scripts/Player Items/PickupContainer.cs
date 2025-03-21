using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MobileActionKit
{
    public class PickupContainer : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script handles the delayed activation of a specified object. The object will be activated after the defined time interval.";

        [Tooltip("Duration in seconds before the object is activated")]
        public float ActivationDelay;

        [Tooltip("The GameObject that will be activated after the delay")]
        public GameObject ObjectToActivate;

        private void Start()
        {
            // Start the coroutine to activate the object after the specified delay
            StartCoroutine(ActivateObjectAfterDelay());
        }

        private IEnumerator ActivateObjectAfterDelay()
        {
            // Wait for the specified activation delay
            yield return new WaitForSeconds(ActivationDelay);

            // Activate the object
            if (ObjectToActivate != null)
            {
                ObjectToActivate.SetActive(true);
            }
        }
    }
}