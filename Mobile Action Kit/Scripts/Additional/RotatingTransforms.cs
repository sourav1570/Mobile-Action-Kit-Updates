using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class RotatingTransforms : MonoBehaviour
    {

        public static void ChangeRotation(Transform ThisTransform, Vector3 From, Vector3 to, float speed)
        {
            var newRotation = Quaternion.Slerp(ThisTransform.rotation, Quaternion.LookRotation(From - to), speed * Time.deltaTime).eulerAngles;
            newRotation.x = 0;
            newRotation.z = 0;
            ThisTransform.rotation = Quaternion.Euler(newRotation);
        }

    }
}
