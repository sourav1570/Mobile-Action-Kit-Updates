using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class CameraRecoil : MonoBehaviour
    {
        public static CameraRecoil instance;

        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This Script Controls Camera Recoil. It uses the Values Below and Shakes the camera on each Player weapon Shot";
        [Space(10)]

        public float CamRotationSpeed = 6f;
        public float CamReturnSpeed = 25f;

        public Vector3 HipFireRecoilRotation = new Vector3(2f, 2f, 2f);
        public Vector3 AimedRecoilRotation = new Vector3(0.5f, 0.5f, 0.5f);

        private Vector3 CurrentRotation;
        private Vector3 rot;

        private bool canRecoil = true; // Flag to track if recoil can be called

        public float DelayRecoil = 0.1f;

        public void ResettingDescription()
        {
            ScriptInfo = "This Script Controls Camera Recoil. It uses the Values Below and Shakes the camera on each Player weapon Shot";
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        void FixedUpdate()
        {
            CurrentRotation = Vector3.Lerp(CurrentRotation, Vector3.zero, CamReturnSpeed * Time.deltaTime);
            rot = Vector3.Slerp(rot, CurrentRotation, CamRotationSpeed * Time.fixedDeltaTime);
            transform.localRotation = Quaternion.Euler(rot);
        }

        public void Recoil(bool aiming)
        {
            if (canRecoil)
            {
                StartCoroutine(DelayedRecoil(aiming));
            }
        }

        private IEnumerator DelayedRecoil(bool aiming)
        {
            canRecoil = false; // Prevent further recoil calls
            yield return new WaitForSeconds(DelayRecoil); // Initial delay of 0.1 seconds

            if (aiming)
            {
                CurrentRotation += new Vector3(-AimedRecoilRotation.x, Random.Range(-AimedRecoilRotation.y, AimedRecoilRotation.y), Random.Range(-AimedRecoilRotation.z, AimedRecoilRotation.z));
            }
            else
            {
                CurrentRotation += new Vector3(-HipFireRecoilRotation.x, Random.Range(-HipFireRecoilRotation.y, HipFireRecoilRotation.y), Random.Range(-HipFireRecoilRotation.z, HipFireRecoilRotation.z));
            }
            canRecoil = true; // Re-enable recoil
        }
    }
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace MobileActionKit
//{
//    public class CameraRecoil : MonoBehaviour
//    {
//        public static CameraRecoil instance;

//        [TextArea]
//        [ContextMenuItem("Reset Description", "ResettingDescription")]
//        public string ScriptInfo = "This Script Controls Camera Recoil.It uses the Values Below and Shake the camera on each Player weapon Shot";
//        [Space(10)]

//        public float CamRotationSpeed = 6f;
//        public float CamReturnSpeed = 25f;


//        public Vector3 HipFireRecoilRotation = new Vector3(2f, 2f, 2f);

//        public Vector3 AimedRecoilRotation = new Vector3(0.5f, 0.5f, 0.5f);

//        private Vector3 CurrentRotation;
//        private Vector3 rot;
//        public void ResettingDescription()
//        {
//            ScriptInfo = "This Script Controls Camera Recoil.It uses the Values Below and Shake the camera on each Player weapon Shot";
//        }
//        void Awake()
//        {
//            if (instance == null)
//            {
//                instance = this;
//            }
//        }
//        void FixedUpdate()
//        {
//            CurrentRotation = Vector3.Lerp(CurrentRotation, Vector3.zero, CamReturnSpeed * Time.deltaTime);
//            rot = Vector3.Slerp(rot, CurrentRotation, CamRotationSpeed * Time.fixedDeltaTime);
//            transform.localRotation = Quaternion.Euler(rot);
//        }
//        public void Recoil(bool aiming)
//        {
//            if (aiming == true)
//            {
//                CurrentRotation += new Vector3(-AimedRecoilRotation.x, Random.Range(-AimedRecoilRotation.y, AimedRecoilRotation.y), Random.Range(-AimedRecoilRotation.z, AimedRecoilRotation.z));
//            }
//            else
//            {
//                CurrentRotation += new Vector3(-HipFireRecoilRotation.x, Random.Range(-HipFireRecoilRotation.y, HipFireRecoilRotation.y), Random.Range(-HipFireRecoilRotation.z, HipFireRecoilRotation.z));
//            }
//        }

//    }
//}