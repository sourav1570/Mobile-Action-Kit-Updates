using UnityEngine;

namespace MobileActionKit
{
    public class CameraShakerEffect : MonoBehaviour
    {
        public Transform CameraShakerGameObject;
        public float ShakeDuration = 0.6f;
        public float ShakeMagnitude = 0.3f;

        private Vector3 LastPosition;
        private Vector3 LastRotation;

        public Vector3 MinPosition = new Vector3(-0.3f,-0.3f,-0.3f);
        public Vector3 MaxPosition = new Vector3(0.3f,0.3f, -0.3f);
        public Vector3 MinRotation = new Vector3(-0.3f, -0.3f, -0.3f);
        public Vector3 MaxRotation = new Vector3(0.3f, 0.3f, 0.3f);

        private void Start()
        {
            // Save the initial position and rotation of the camera
            LastPosition = CameraShakerGameObject.localPosition;
            LastRotation = CameraShakerGameObject.localEulerAngles;
        }

        public void Shake()
        {
            if (CameraShakerGameObject == null)
            {
               // Debug.LogWarning("CameraToShake is not assigned.");
                return;
            }

            // Cancel any existing LeanTween animations on the camera
            LeanTween.cancel(CameraShakerGameObject.gameObject);

            // Generate random shake offset for position
            Vector3 randomPositionOffset = new Vector3(
                Random.Range(MinPosition.x, MaxPosition.x) * ShakeMagnitude,
                Random.Range(MinPosition.y, MaxPosition.y) * ShakeMagnitude,
                Random.Range(MinPosition.z, MaxPosition.z) * ShakeMagnitude
            );

            // Generate random shake offset for rotation
            Vector3 randomRotationOffset = new Vector3(
                Random.Range(MinRotation.x, MaxRotation.x) * ShakeMagnitude,
                Random.Range(MinRotation.y, MaxRotation.y) * ShakeMagnitude,
                Random.Range(MinRotation.z, MaxRotation.z) * ShakeMagnitude
            );

            // Apply position shake using LeanTween
            LeanTween.moveLocal(CameraShakerGameObject.gameObject, LastPosition + randomPositionOffset, ShakeDuration)
                .setEase(LeanTweenType.easeShake)
                .setLoopPingPong(1)
                .setOnComplete(() =>
                {
                    // Reset position after the shake
                    CameraShakerGameObject.localPosition = LastPosition;
                });

            // Apply rotation shake using LeanTween
            LeanTween.rotateLocal(CameraShakerGameObject.gameObject, LastRotation + randomRotationOffset, ShakeDuration)
                .setEase(LeanTweenType.easeShake)
                .setLoopPingPong(1)
                .setOnComplete(() =>
                {
                    // Reset rotation after the shake
                    CameraShakerGameObject.localEulerAngles = LastRotation;
                });

           // Debug.Log("Shake triggered using LeanTween.");
        }
    }
}


//using UnityEngine;

//namespace MobileActionKit
//{
//    public class CameraShakerEffect : MonoBehaviour
//    {
//        public Transform CameraToShake;
//        public float CameraShakeDuration = 0.8f;
//        public float CameraShakeResetDuration = 0.8f; // Reset duration
//        public float CameraShakeMagnitude = 0.3f;

//        private Vector3 LastPosition;
//        private Vector3 LastRotation;

//        public Vector3 MinimumPosition;
//        public Vector3 MaximumPosition;
//        public Vector3 MinimumRotation;
//        public Vector3 MaximumRotation;

//        private void Start()
//        {
//            // Save the initial position and rotation of the camera
//            LastPosition = CameraToShake.localPosition;
//            LastRotation = CameraToShake.localEulerAngles;
//        }

//        public void Shake()
//        {
//            if (CameraToShake == null)
//            {
//                Debug.LogWarning("CameraToShake is not assigned.");
//                return;
//            }

//            // Generate random shake offset for position
//            Vector3 randomPositionOffset = new Vector3(
//                Random.Range(MinimumPosition.x, MaximumPosition.x) * CameraShakeMagnitude,
//                Random.Range(MinimumPosition.y, MaximumPosition.y) * CameraShakeMagnitude,
//                Random.Range(MinimumPosition.z, MaximumPosition.z) * CameraShakeMagnitude
//            );

//            // Generate random shake offset for rotation
//            Vector3 randomRotationOffset = new Vector3(
//                Random.Range(MinimumRotation.x, MaximumRotation.x) * CameraShakeMagnitude,
//                Random.Range(MinimumRotation.y, MaximumRotation.y) * CameraShakeMagnitude,
//                Random.Range(MinimumRotation.z, MaximumRotation.z) * CameraShakeMagnitude
//            );

//            // Apply position shake using LeanTween
//            LeanTween.moveLocal(CameraToShake.gameObject, LastPosition + randomPositionOffset, CameraShakeDuration)
//                .setEase(LeanTweenType.easeShake)
//                .setLoopPingPong(1)
//                .setOnComplete(() =>
//                {
//                    // After shaking, gradually reset the position back to original using CameraShakeResetDuration
//                    LeanTween.moveLocal(CameraToShake.gameObject, LastPosition, CameraShakeResetDuration).setEase(LeanTweenType.easeInOutQuad);
//                });

//            // Apply rotation shake using LeanTween
//            LeanTween.rotateLocal(CameraToShake.gameObject, LastRotation + randomRotationOffset, CameraShakeDuration)
//                .setEase(LeanTweenType.easeShake)
//                .setLoopPingPong(1)
//                .setOnComplete(() =>
//                {
//                    // After shaking, gradually reset the rotation back to original using CameraShakeResetDuration
//                    LeanTween.rotateLocal(CameraToShake.gameObject, LastRotation, CameraShakeResetDuration).setEase(LeanTweenType.easeInOutQuad);
//                });

//            Debug.Log("Shake triggered using LeanTween.");
//        }
//    }
//}


//using System.Collections;
//using UnityEngine;

//namespace MobileActionKit
//{
//    public class CameraShakerEffect : MonoBehaviour
//    {
//        public Transform CameraToShake;
//        public float CameraShakeDuration = 0.8f;
//        public float CameraShakeMagnitude = 0.3f;

//        Vector3 LastPosition;
//        Vector3 LastRotation;

//        public Vector3 MinimumPosition;
//        public Vector3 MaximumPosition;
//        public Vector3 MinimumRotation;
//        public Vector3 MaximumRotation;

//        private void Start()
//        {
//            LastPosition = CameraToShake.localPosition;
//            LastRotation = CameraToShake.localEulerAngles;
//        }
//        public IEnumerator Shake()
//        {
//            float elapsed = 0.0f;
//            while (elapsed < CameraShakeDuration)
//            {
//                float xpos = Random.Range(MinimumPosition.x, MaximumPosition.x) * CameraShakeMagnitude;
//                float ypos = Random.Range(MinimumPosition.y, MaximumPosition.y) * CameraShakeMagnitude;
//                float zpos = Random.Range(MinimumPosition.z, MaximumPosition.z) * CameraShakeMagnitude;

//                float xrot = Random.Range(MinimumRotation.x, MaximumRotation.x) * CameraShakeMagnitude;
//                float yrot = Random.Range(MinimumRotation.y, MaximumRotation.y) * CameraShakeMagnitude;
//                float zrot = Random.Range(MinimumRotation.z, MaximumRotation.z) * CameraShakeMagnitude;

//                CameraToShake.localPosition = new Vector3(xpos, ypos, zpos);
//                CameraToShake.localEulerAngles = new Vector3(xrot, yrot, zrot);

//                elapsed += Time.deltaTime;

//                yield return null;
//            }
//            CameraToShake.localPosition = LastPosition;
//            CameraToShake.localEulerAngles = LastRotation;
//        }
//    }
//}