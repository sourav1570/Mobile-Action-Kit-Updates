using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class DebugGizmos : MonoBehaviour
    {
        public float Radius;

        void OnDrawGizmos()
        {
            // Display the explosion radius when selected
            Gizmos.color = new Color(1, 1, 0, 0.75F);
            Gizmos.DrawSphere(transform.position, Radius);
        }


    }
}