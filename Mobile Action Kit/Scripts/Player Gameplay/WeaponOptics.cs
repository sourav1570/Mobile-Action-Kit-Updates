using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class WeaponOptics : MonoBehaviour
    {
        [TextArea]     
        public string ScriptInfo = "This script overrides several default values of aimed player weapon as well as player and weapon cameras stored in 'PlayerWeapon' script if weapon equipped with the optical sight." +
             " It helps to set desired position and rotation of the aimed weapon, and adjust zoom values for both player and weapon cameras to any particular type of the optical scope. " +
             "This script is to be disabled if weapon is not equipped with any kind of optics and uses iron sights.";

        [Tooltip("Respective 'Shooting point' gameobject from the player's hierarchy is to be placed into this field.")]
        public PlayerWeapon PlayerWeaponComponent;

        [Tooltip("Enable this option to use a blur effect when aiming.")]
        public bool UseAimedBlurEffect = false;

        [Tooltip("GameObject representing the aimed blur effect. This effect is shown when 'UseAimedBlurEffect' is enabled.")]
        public GameObject AimedBlurEffect;

        [Tooltip("Enable this option if the weapon is equipped with a sniper scope.")]
        public bool IsSniperScope = false;

        [Tooltip("Overrides the position and rotation values of the aimed weapon pivot for better alignment with the optical sight.")]
        public OverrideAimedWeaponPivotValuesClass OverrideAimedWeaponPivotValues;

        [Tooltip("Overrides the zoom and field-of-view parameters for both player and weapon cameras.")]
        public CamerasFovParametersClass OverrideCamerasFovParameters;

        [System.Serializable]
        public class CamerasFovParametersClass
        {
            [Header("Player Camera Fov Parameters")]
            [Tooltip("The field of view (FOV) of the player camera when zoomed in with the optics.")]
            public float PlayerCameraMagnifiedFov = 15f;

            [Tooltip("The speed at which the player camera transitions to the magnified FOV.")]
            public float PlayerCameraFovChangeSpeed = 7f;

            [Header("Weapon Camera Fov Parameters")]
            [Tooltip("The field of view (FOV) of the weapon camera when zoomed in with the optics.")]
            public float WeaponCameraMagnifiedFov = 10f;

            [Tooltip("The speed at which the weapon camera transitions to the magnified FOV.")]
            public float WeaponCameraFovChangeSpeed = 6f;
        }

        [System.Serializable]
        public class OverrideAimedWeaponPivotValuesClass
        {
            [Tooltip("The local position of the weapon pivot when aiming with the optics.")]
            public Vector3 AimedWeaponPivotPosition;

            [Tooltip("The local rotation of the weapon pivot when aiming with the optics.")]
            public Vector3 AimedWeaponPivotRotation;
        }

        private void OnEnable()
        {
            PlayerWeaponComponent.StoreDefaultValues();

            PlayerWeaponComponent.ShootingFeatures.UseBlurEffectOnAim = UseAimedBlurEffect;
            PlayerWeaponComponent.ShootingFeatures.BlurEffect = AimedBlurEffect;
            PlayerWeaponComponent.ShootingFeatures.UseSniperScopeUI = IsSniperScope;
            PlayerWeaponComponent.WeaponPositionsRotationsDurations.AimedWeaponPivotPosition = OverrideAimedWeaponPivotValues.AimedWeaponPivotPosition;
            PlayerWeaponComponent.WeaponPositionsRotationsDurations.AimedWeaponPivotRotation = OverrideAimedWeaponPivotValues.AimedWeaponPivotRotation;
          
            PlayerWeaponComponent.CamerasFovParameters.PlayerCameraFovChangeSpeed = OverrideCamerasFovParameters.PlayerCameraFovChangeSpeed;
            PlayerWeaponComponent.CamerasFovParameters.PlayerCameraMagnifiedFov = OverrideCamerasFovParameters.PlayerCameraMagnifiedFov;


         
            PlayerWeaponComponent.CamerasFovParameters.WeaponCameraFovChangeSpeed = OverrideCamerasFovParameters.WeaponCameraFovChangeSpeed;
            PlayerWeaponComponent.CamerasFovParameters.WeaponCameraMagnifiedFov = OverrideCamerasFovParameters.WeaponCameraMagnifiedFov;

            PlayerWeaponComponent.DoNotResetValues = true;
        }
        //[ContextMenu("PasteWeaponPivotPostionAndRotations")]
        //public void PasteWeaponPivotPostionAndRotations()
        //{
        //    OverrideAimedWeaponPivotValues.AimedWeaponPivotPosition = PlayerWeaponComponent.WeaponPositionsRotationsDurations.ShootingPointParent.transform.localPosition;
        //    OverrideAimedWeaponPivotValues.AimedWeaponPivotRotation = PlayerWeaponComponent.WeaponPositionsRotationsDurations.ShootingPointParent.transform.localEulerAngles;        
        //}

        //[ContextMenu("PasteWeaponPivotPostionAndRotations")]
        //public void PasteWeaponPivotPostionAndRotations()
        //{
        //    // Get the ShootingPointParent's transform
        //    var shootingPointParent = PlayerWeaponComponent.WeaponPositionsRotationsDurations.ShootingPointParent;

        //    if (shootingPointParent == null)
        //    {
        //        Debug.LogError("ShootingPointParent is null! Ensure it is assigned.");
        //        return;
        //    }

        //    // Capture position (this part works correctly for you)
        //    OverrideAimedWeaponPivotValues.AimedWeaponPivotPosition = shootingPointParent.transform.localPosition;

        //    // Capture rotation and normalize it
        //    Vector3 rawRotation = shootingPointParent.transform.localEulerAngles;
        //    OverrideAimedWeaponPivotValues.AimedWeaponPivotRotation = NormalizeEulerAngles(rawRotation);

        //    // Debug to confirm
        //    Debug.Log($"Captured Position: {OverrideAimedWeaponPivotValues.AimedWeaponPivotPosition}");
        //    Debug.Log($"Captured Rotation (Normalized): {OverrideAimedWeaponPivotValues.AimedWeaponPivotRotation}");
        //}

        ///// <summary>
        ///// Normalize Euler angles to the range -180 to 180.
        ///// </summary>
        //private Vector3 NormalizeEulerAngles(Vector3 eulerAngles)
        //{
        //    eulerAngles.x = (eulerAngles.x > 180) ? eulerAngles.x - 360 : eulerAngles.x;
        //    eulerAngles.y = (eulerAngles.y > 180) ? eulerAngles.y - 360 : eulerAngles.y;
        //    eulerAngles.z = (eulerAngles.z > 180) ? eulerAngles.z - 360 : eulerAngles.z;
        //    return eulerAngles;
        //}

        [ContextMenu("PasteWeaponPivotPostionAndRotations")]
        public void PasteWeaponPivotPostionAndRotations()
        {
            // Get the ShootingPointParent's transform
            var shootingPointParent = PlayerWeaponComponent.WeaponPositionsRotationsDurations.WeaponPivot;

            if (shootingPointParent == null)
            {
                Debug.LogError("ShootingPointParent is null! Ensure it is assigned.");
                return;
            }

            // Capture position (this part works correctly for you)
            OverrideAimedWeaponPivotValues.AimedWeaponPivotPosition = shootingPointParent.transform.localPosition;

            // Capture rotation
            Vector3 rawRotation = shootingPointParent.transform.localEulerAngles;

            // Normalize rotation to avoid getting 360 values or values out of range
            Vector3 normalizedRotation = NormalizeRotation(rawRotation);

            // Format the rotation values to remove excessive trailing zeros
            OverrideAimedWeaponPivotValues.AimedWeaponPivotRotation = FormatRotation(normalizedRotation);

            // Debug to confirm
            Debug.Log($"Captured Position: {OverrideAimedWeaponPivotValues.AimedWeaponPivotPosition}");
            Debug.Log($"Captured Rotation (Normalized): {OverrideAimedWeaponPivotValues.AimedWeaponPivotRotation}");
        }

        /// <summary>
        /// Normalizes Euler angles to the range of -180 to 180 degrees to prevent 360 wrapping behavior.
        /// </summary>
        private Vector3 NormalizeRotation(Vector3 eulerAngles)
        {
            eulerAngles.x = NormalizeAngle(eulerAngles.x);
            eulerAngles.y = NormalizeAngle(eulerAngles.y);
            eulerAngles.z = NormalizeAngle(eulerAngles.z);
            return eulerAngles;
        }

        /// <summary>
        /// Normalizes a single angle to the range of -180 to 180 degrees.
        /// </summary>
        private float NormalizeAngle(float angle)
        {
            if (angle > 180) angle -= 360;
            if (angle < -180) angle += 360;
            return angle;
        }

        /// <summary>
        /// Formats the rotation vector values to a fixed precision, trimming unnecessary trailing zeros.
        /// </summary>
        private Vector3 FormatRotation(Vector3 original)
        {
            return new Vector3(
                RoundToPrecision(original.x),
                RoundToPrecision(original.y),
                RoundToPrecision(original.z)
            );
        }

        /// <summary>
        /// Rounds a single value to 7 decimal places to avoid discrepancies like 0.320007
        /// </summary>
        private float RoundToPrecision(float value)
        {
            float tolerance = 0.00001f;

            // If the value is very close to the original input, keep it as it is (for example -0.32)
            if (Mathf.Abs(value - (float)Math.Round(value, 4)) < tolerance)
            {
                return (float)Math.Round(value, 4); // Round to 4 decimals
            }

            return (float)Math.Round(value, 6); // Otherwise, round to 6 decimals to minimize discrepancy
        }


    }
}