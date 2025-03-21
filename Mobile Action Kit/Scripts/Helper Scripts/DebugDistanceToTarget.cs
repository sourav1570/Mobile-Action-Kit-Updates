using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MobileActionKit
{
    public class DebugDistanceToTarget : MonoBehaviour
    {
        public Transform target; // The target to measure the distance to
        public float distanceToTarget; // Public field to display the distance

        // Update is called once per frame
        void Update()
        {
            if (target != null)
            {
                // Calculate the distance magnitude between the current object and the target
                distanceToTarget = (transform.position - target.position).magnitude;
            }
        }
    }
}