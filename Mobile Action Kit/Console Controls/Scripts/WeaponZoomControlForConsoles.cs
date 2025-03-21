using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MobileActionKit
{
    public class WeaponZoomControlForConsoles : MonoBehaviour
    {
        public static WeaponZoomControlForConsoles instance;

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

        public int ZoomLevels = 5;

        private int currentZoomLevel = 0; // Track current zoom step (0 = no zoom, 5 = max zoom)
        private float stepSize; // How much FOV changes per step

        private float defaultFOV;
        private float magnifiedFOV;

        [HideInInspector]
        public bool StopFunctioning = false;

        PlayerWeapon PlayerWeaponInUse;
       

        private float targetFOV;
        private float currentFOV;
        private bool isZoomingIn = false;
        private bool isZoomingOut = false;

        bool IsScope = false;

        private void Awake()
        {
            instance = this;

        }
        private void Start()
        {
            for (int x = 0; x < AllPlayerWeaponComponent.Length; x++)
            {
                if (AllPlayerWeaponComponent[x].gameObject.activeInHierarchy == true)
                {
                    PlayerWeaponInUse = AllPlayerWeaponComponent[x];
                }
                AllPlayerWeaponComponent[x].gameObject.AddComponent<WeaponActivationCheckerForConsoles>();
            }
        }
        public void UpdatePlayerWeapon(PlayerWeapon PlayerWeaponComponent)
        {
            PlayerWeaponInUse = PlayerWeaponComponent;

            if (PlayerWeaponInUse.ShootingFeatures.UseSniperScopeUI == true)
            {
                defaultFOV = PlayerWeaponInUse.ShootingFeatures.SniperScopeScript.ScopeConfig.ScopeFOV;
                magnifiedFOV = PlayerWeaponInUse.CamerasFovParameters.PlayerCameraMagnifiedFov;

                // Calculate step size (total FOV range divided by steps)
                stepSize = (defaultFOV - magnifiedFOV) / ZoomLevels;
               
            }
        }
        private void Update()
        {

            if (PlayerWeaponInUse != null)
            {
                if (PlayerWeaponInUse.ShootingFeatures.UseSniperScopeUI == true)
                {
                    if (PlayerWeaponInUse.IsAimed == true)
                    {
                        if (IsScope == false)
                        {
                            IsScope = true;
                            currentZoomLevel = 0;
                            // Tween the camera's field of view
                            LeanTween.value(PlayerWeaponInUse.RequiredComponents.PlayerCamera.gameObject, UpdateCameraFOV, PlayerWeaponInUse.RequiredComponents.PlayerCamera.fieldOfView, PlayerWeaponInUse.ShootingFeatures.SniperScopeScript.ScopeConfig.ScopeFOV, DefaultMagnifiedFovDuration)
                            .setOnUpdate((float value) =>
                             {
                                 PlayerWeaponInUse.RequiredComponents.PlayerCamera.fieldOfView = value;
                             })
                             .setOnComplete(() =>
                             {
 
                             });
                        }
                    }
                    else
                    {
                        IsScope = false;
                    }

                }
                else
                {
                    IsScope = false;
                }
            }
            
            
           


        }

        public void OnZoomIn()
        {
            if (PlayerWeaponInUse.ShootingFeatures.UseSniperScopeUI == true)
            {
                if (PlayerManager.instance.IsScoping && PlayerWeaponInUse.IsAimed)
                {
                    if (currentZoomLevel < ZoomLevels) // Prevent over-zooming
                    {
                        currentZoomLevel++; // Move to next zoom level
                        UpdateFOV();
                    }
                }
            }
        }

        public void OnZoomOut()
        {
            if (PlayerWeaponInUse.ShootingFeatures.UseSniperScopeUI == true)
            {
                if (PlayerManager.instance.IsScoping && PlayerWeaponInUse.IsAimed)
                {
                    if (currentZoomLevel > 0) // Prevent over-unzooming
                    {
                        currentZoomLevel--; // Move to previous zoom level
                        UpdateFOV();
                    }
                }
            }
        }

        private void UpdateFOV()
        {
            if (PlayerWeaponInUse.ShootingFeatures.UseSniperScopeUI == true)
            {
                float newFOV = defaultFOV - (stepSize * currentZoomLevel);

                // Check if the zoom level is already at max or min before updating
                if (PlayerWeaponInUse.RequiredComponents.PlayerCamera.fieldOfView == newFOV)
                    return; // Prevents reapplying the same value

                PlayerWeaponInUse.RequiredComponents.PlayerCamera.fieldOfView = newFOV;
            }
        }
        private void UpdateCameraFOV(float value)
        {
            PlayerWeaponInUse.RequiredComponents.PlayerCamera.fieldOfView = value;
        }
    }
}
