using UnityEngine;


namespace MobileActionKit
{
    public class MotionController : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script controls the movement and rotation of an object. It allows smooth oscillating movement along the X, Y, or Z axes and continuous rotation around these axes.";

        [Header("Movement Settings")]
        [Tooltip("Enable or disable movement along the X-axis")]
        public bool moveX = false;

        [Tooltip("Enable or disable movement along the Y-axis")]
        public bool moveY = false;

        [Tooltip("Enable or disable movement along the Z-axis")]
        public bool moveZ = false;

        [Tooltip("Distance to move along the X-axis")]
        public float moveDistanceX = 1f;

        [Tooltip("Distance to move along the Y-axis")]
        public float moveDistanceY = 1f;

        [Tooltip("Distance to move along the Z-axis")]
        public float moveDistanceZ = 1f;

        [Tooltip("Duration for one complete movement cycle")]
        public float moveDuration = 1f;
        private Vector3 originalPosition;

        [Header("Rotation Settings")]
        [Tooltip("Enable or disable rotation around the X-axis")]
        public bool rotateX = false;

        [Tooltip("Enable or disable rotation around the Y-axis")]
        public bool rotateY = false;

        [Tooltip("Enable or disable rotation around the Z-axis")]
        public bool rotateZ = false;

        [Tooltip("Rotation speed around the X-axis (degrees per second)")]
        public float rotationSpeedX = 30f;

        [Tooltip("Rotation speed around the Y-axis (degrees per second)")]
        public float rotationSpeedY = 30f;

        [Tooltip("Rotation speed around the Z-axis (degrees per second)")]
        public float rotationSpeedZ = 30f;


        private void Start()
        {
            originalPosition = transform.position;
            Move();
        }

        private void Update()
        {
            // Handle rotation
            if (rotateX)
            {
                transform.Rotate(rotationSpeedX * Time.deltaTime, 0, 0);
            }
            if (rotateY)
            {
                transform.Rotate(0, rotationSpeedY * Time.deltaTime, 0);
            }
            if (rotateZ)
            {
                transform.Rotate(0, 0, rotationSpeedZ * Time.deltaTime);
            }
        }

        private void Move()
        {
            if (moveX)
            {
                LeanTween.moveLocalX(gameObject, originalPosition.x + moveDistanceX, moveDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(() => LeanTween.moveLocalX(gameObject, originalPosition.x, moveDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(Move));
            }
            if (moveY)
            {
                LeanTween.moveLocalY(gameObject, originalPosition.y + moveDistanceY, moveDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(() => LeanTween.moveLocalY(gameObject, originalPosition.y, moveDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(Move));
            }
            if (moveZ)
            {
                LeanTween.moveLocalZ(gameObject, originalPosition.z + moveDistanceZ, moveDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(() => LeanTween.moveLocalZ(gameObject, originalPosition.z, moveDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(Move));
            }
        }
    }
}