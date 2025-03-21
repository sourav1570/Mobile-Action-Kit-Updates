using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class MouseScrollWeaponZoom : MonoBehaviour
    {
        public static MouseScrollWeaponZoom instance;

        [TextArea]
        public string ScriptInfo = "MouseScrollWeaponZoom manage zoom for sniper scopes. "
        + "It dynamically adjusts the field of view (FOV) for scoped weapons. "
        + "This script modifies the player's camera zoom smoothly when using a sniper scope.";

        [HideInInspector]
        public string ControlName;

        //public Controls Options;
        [Tooltip("Reference to the SwitchingPlayerWeapons component, which manages weapon switching.")]
        public SwitchingPlayerWeapons SwitchingPlayerWeaponsComponent;

        [Tooltip("Array of all PlayerWeapon components available to the player.")]
        public PlayerWeapon[] AllPlayerWeaponComponent;

        [Tooltip("Speed at which the sniper scope zooms in and out when using the mouse scroll wheel.")]
        public float ZoomSpeedForSniperScope = 50f;

        [Tooltip("Default duration for magnified field of view transition when scoping with a sniper rifle.")]
        public float DefaultMagnifiedFovDuration = 0.1f;


        [HideInInspector]
        public float currentFOV = 0f;  // Current field of view

        bool once = false;
        [HideInInspector]
        public bool StopFunctioning = false;

        PlayerWeapon PlayerWeaponInUse;

        private void Awake()
        {
            instance = this;

        }
        private void Start()
        {
            for(int x = 0; x < AllPlayerWeaponComponent.Length; x++)
            {
                if(AllPlayerWeaponComponent[x].gameObject.activeInHierarchy == true)
                {
                    PlayerWeaponInUse = AllPlayerWeaponComponent[x];
                }
                AllPlayerWeaponComponent[x].gameObject.AddComponent<WeaponActivationChecker>();
            }
        }
        public void UpdatePlayerWeapon(PlayerWeapon PlayerWeaponComponent)
        {
            once = false;
            PlayerWeaponInUse = PlayerWeaponComponent;
         
        }
        void Update()
        {
            if (StopFunctioning == false)
            {
                //if (Options == Controls.SwitchWeapons)
                //{
                //    if (Input.GetAxis("Mouse ScrollWheel") > 0)
                //    {
                //        SwitchingPlayerWeaponsComponent.ActivateNextWeapon();
                //    }
                //    if (Input.GetAxis("Mouse ScrollWheel") < 0)
                //    {
                //        SwitchingPlayerWeaponsComponent.ActivatePreviousWeapon(); 
                //    }
                //}
                //else
                //{
                if (PlayerWeaponInUse != null)
                {
                    if (PlayerWeaponInUse.gameObject.activeInHierarchy == true)
                    {
                        if (PlayerWeaponInUse.ShootingFeatures.UseSniperScopeUI == true)
                        {
                            if (once == true)
                            {
                                float scrollInput = Input.GetAxis("Mouse ScrollWheel");

                                if (scrollInput != 0)
                                {
                                    // Calculate the target field of view
                                    float targetFOV = Mathf.Clamp(currentFOV - scrollInput * ZoomSpeedForSniperScope, PlayerWeaponInUse.ShootingFeatures.SniperScopeScript.ZoomConfig.MinZoomFOV, PlayerWeaponInUse.ShootingFeatures.SniperScopeScript.ScopeConfig.ScopeFOV);

                                    // Smoothly interpolate between the current and target field of view
                                    currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * ZoomSpeedForSniperScope);

                                    // Update the camera's field of view
                                    PlayerWeaponInUse.RequiredComponents.PlayerCamera.fieldOfView = currentFOV;
                                }
                            }

                            if (PlayerManager.instance.IsScoping == true)
                            {
                                if (once == false)
                                {
                                    // Tween the camera's field of view
                                    LeanTween.value(PlayerWeaponInUse.RequiredComponents.PlayerCamera.gameObject, UpdateCameraFOV, PlayerWeaponInUse.RequiredComponents.PlayerCamera.fieldOfView, PlayerWeaponInUse.ShootingFeatures.SniperScopeScript.ScopeConfig.ScopeFOV, DefaultMagnifiedFovDuration)
                                        .setOnUpdate((float value) =>
                                        {
                                            PlayerWeaponInUse.RequiredComponents.PlayerCamera.fieldOfView = value;
                                            currentFOV = value;
                                        })
                                        .setOnComplete(() =>
                                        {
                                            Debug.Log("Camera FOV change complete.");
                                            once = true;
                                        });


                                }

                            }
                            else
                            {
                                once = false;
                            }
                        }
                    }
                }
            }

        }
        private void UpdateCameraFOV(float value)
        {
            PlayerWeaponInUse.RequiredComponents.PlayerCamera.fieldOfView = value;
        }
    }
}
