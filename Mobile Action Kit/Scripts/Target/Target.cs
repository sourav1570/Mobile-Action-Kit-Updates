using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class Target : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This Script Rotates The X Axis of Target If Shoot By Any Weapon";
        [Space(10)]

        public bool EnableTargetHitRotation = false;
        public float XRotationSpeed;
        public bool DeactivateOnRotationComplete = true;

        [HideInInspector]
        public bool StartRotating = false;

        public void ResettingDescription()
        {
            ScriptInfo = "This Script Rotates The X Axis of Target If Shoot By Any Weapon";
        }
        void Update()
        {
            if(EnableTargetHitRotation == true)
            {
                if (StartRotating == true)
                {
                    if (transform.rotation.x >= 0f)
                    {
                        if(DeactivateOnRotationComplete == true)
                        {
                            gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        transform.Rotate(XRotationSpeed * Time.deltaTime, transform.rotation.y, transform.rotation.z);
                    }
                }
            }
        }
        public void Damage()
        {
            StartRotating = true;
        }



    }
}