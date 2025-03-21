using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace MobileActionKit
{
    public class SniperScope : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This Script Provides The Sniper Scope Behaviour To The Weapon";

        [System.Serializable]
        public class ScopeSettings
        {
            [Tooltip("The PlayerWeapon script attached to the weapon. This controls weapon-specific behavior.")]
            public PlayerWeapon PlayerWeaponScript;

            [Tooltip("The UI overlay that appears when the sniper scope is active.")]
            public GameObject ScopeUIOverlay;

            [Tooltip("Field of view when the sniper scope is active.")]
            public float ScopeFOV = 15f;

            [Tooltip("Delay (in seconds) before the scope UI activates after scoping.")]
            public float ScopeActivationDelay = 0.15f;

            [Tooltip("Array of UI elements to deactivate when scoping.")]
            public GameObject[] UIElementsToDeactivate;
        }

        [System.Serializable]
        public class ZoomSettings
        {
            [Tooltip("Enable or disable zoom controls for the sniper scope.")]
            public bool EnableZoomControls = true;

            [Tooltip("Use a slider UI element to control the zoom level.")]
            public bool UseZoomSlider = false;

            [Tooltip("The slider UI element for adjusting the zoom level.")]
            public Slider ZoomSlider;

            [Tooltip("Minimum Field of View (FOV) value for the zoom.")]
            public float MinZoomFOV = 5f;

            [Tooltip("Speed of zooming in or out.")]
            public float ZoomSpeed = 3f;
        }

        [System.Serializable]
        public class ShootingSettings
        {
            [Tooltip("Animator component to handle shooting animations.")]
            public Animator ShotAnimatorComponent;

            [Tooltip("Animation clip to play when a shot is fired.")]
            public AnimationClip ShotAnimationClip;
        }

        [Tooltip("Settings related to the sniper scope functionality.")]
        public ScopeSettings ScopeConfig = new ScopeSettings();

        [Tooltip("Settings related to zoom functionality for the sniper scope.")]
        public ZoomSettings ZoomConfig = new ZoomSettings();

        [Tooltip("Settings related to shooting animations while scoped.")]
        public ShootingSettings ShootingConfig = new ShootingSettings();

        [HideInInspector]
        public bool IsScoped = false;


        public void ResettingDescription()
        {
            ScriptInfo = "This Script Provides The Sniper Scope Behaviour To The Weapon ";
        }
        void Start()
        {
            if(ScopeConfig.PlayerWeaponScript != null)
            {
                if(ScopeConfig.PlayerWeaponScript.ShootingFeatures.UseSniperScopeUI == true)
                {
                    if (ZoomConfig.UseZoomSlider == true)
                    {
                        ZoomConfig.ZoomSlider.direction = Slider.Direction.LeftToRight;
                        ZoomConfig.ZoomSlider.minValue = ZoomConfig.MinZoomFOV;
                        ZoomConfig.ZoomSlider.maxValue = ScopeConfig.ScopeFOV;
                        ZoomConfig.ZoomSlider.value = ScopeConfig.ScopeFOV;
                    }
                    else
                    {
                        ZoomConfig.ZoomSlider.gameObject.SetActive(false);
                    }
                }
            }
        
        }
        void OnDisable()
        {
            IsScoped = false;

            if (ScopeConfig.PlayerWeaponScript != null)
            {
                LeanTween.cancel(ScopeConfig.PlayerWeaponScript.WeaponPositionsRotationsDurations.WeaponPivot.gameObject);
                LeanTween.moveLocalX(ScopeConfig.PlayerWeaponScript.WeaponPositionsRotationsDurations.WeaponPivot, ScopeConfig.PlayerWeaponScript.WeaponPositionsRotationsDurations.HipFireWeaponPivotPosition.x, 0f);
                LeanTween.moveLocalY(ScopeConfig.PlayerWeaponScript.WeaponPositionsRotationsDurations.WeaponPivot, ScopeConfig.PlayerWeaponScript.WeaponPositionsRotationsDurations.HipFireWeaponPivotPosition.y, 0f);
                LeanTween.moveLocalZ(ScopeConfig.PlayerWeaponScript.WeaponPositionsRotationsDurations.WeaponPivot, ScopeConfig.PlayerWeaponScript.WeaponPositionsRotationsDurations.HipFireWeaponPivotPosition.z, 0f);
                LeanTween.rotateLocal(ScopeConfig.PlayerWeaponScript.WeaponPositionsRotationsDurations.WeaponPivot, ScopeConfig.PlayerWeaponScript.WeaponPositionsRotationsDurations.HipFireWeaponPivotRotation, 0f);


                if (ScopeConfig.PlayerWeaponScript.ShootingFeatures.UseSniperScopeUI == true)
                {                  
                    OnUnscoped();
                }
            }
        }
        public void Sniperscope()
        {
            if (ScopeConfig.PlayerWeaponScript != null)
            {
                if (ScopeConfig.PlayerWeaponScript.ShootingFeatures.UseSniperScopeUI == true)
                {
                   
                    if (IsScoped == false)
                    {
                        StartCoroutine(Onscoped());
                    }
                    else
                    {
                        OnUnscoped();
                    }
                     
                }
            }
        }
        public void OnUnscoped()
        {
            if (ScopeConfig.PlayerWeaponScript != null)
            {
                if (ScopeConfig.PlayerWeaponScript.ShootingFeatures.UseSniperScopeUI == true)
                {
                    for (int x = 0; x < ScopeConfig.UIElementsToDeactivate.Length; x++)
                    {
                        if(ScopeConfig.UIElementsToDeactivate[x] != null)
                        {
                            ScopeConfig.UIElementsToDeactivate[x].SetActive(true);
                        }
                     
                    }
                    if(ScopeConfig.ScopeUIOverlay != null)
                    {
                        ScopeConfig.ScopeUIOverlay.SetActive(false);
                    }
                  
                    if (ZoomConfig.UseZoomSlider == true)
                    {
                        if(ZoomConfig.ZoomSlider != null)
                        {
                            ZoomConfig.ZoomSlider.gameObject.SetActive(false);
                        }
                    }
                    // new change;
                    foreach (Transform g in transform.GetComponentsInChildren<Transform>())
                    {
                        if (g.gameObject.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            g.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                        }
                        if (g.gameObject.GetComponent<MeshRenderer>() != null)
                        {
                            g.gameObject.GetComponent<MeshRenderer>().enabled = true;
                        }
                    }

                    IsScoped = false;
                }
            }
        }
        IEnumerator Onscoped()
        {
            yield return new WaitForSeconds(ScopeConfig.ScopeActivationDelay);
            if (ScopeConfig.PlayerWeaponScript.IsAimed == true)
            {
                for (int x = 0; x < ScopeConfig.UIElementsToDeactivate.Length; x++)
                {
                    ScopeConfig.UIElementsToDeactivate[x].SetActive(false);
                }
                ScopeConfig.ScopeUIOverlay.SetActive(true);
                if (ZoomConfig.UseZoomSlider == true)
                {
                    ZoomConfig.ZoomSlider.gameObject.SetActive(true);
                }
                // new change;
                foreach (Transform g in transform.GetComponentsInChildren<Transform>())
                {
                    if (g.gameObject.GetComponent<SkinnedMeshRenderer>() != null)
                    {
                        g.gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    }
                    if (g.gameObject.GetComponent<MeshRenderer>() != null)
                    {
                        g.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    }
                }

                if (ScopeConfig.ScopeUIOverlay.GetComponent<MeshRenderer>() != null)
                {
                    ScopeConfig.ScopeUIOverlay.GetComponent<MeshRenderer>().enabled = true;
                    foreach (Transform g in ScopeConfig.ScopeUIOverlay.GetComponentsInChildren<Transform>())
                    {
                        if (g.gameObject.GetComponent<MeshRenderer>() != null)
                        {
                            g.gameObject.GetComponent<MeshRenderer>().enabled = true;
                        }
                    }
                }

                IsScoped = true;
            }
           
        }
    }
}