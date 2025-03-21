using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class WeaponInspection : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script handles the weapon inspection system, allowing players to zoom in on weapons, adjust the camera's field of view, and toggle UI buttons accordingly.";

        [System.Serializable]
        public class WeaponData
        {
            [Tooltip("The weapon model that will be inspected.")]
            public GameObject WeaponModel;

            [Tooltip("The camera's field of view value when inspecting this weapon.")]
            public float FieldOfViewValue;

            [Tooltip("The duration of the transition to the new field of view.")]
            public float Duration;

            [Tooltip("Objects that should be deactivated when inspecting this weapon.")]
            public GameObject[] ObjToDeactivate;
        }

        [Tooltip("The camera used for inspection.")]
        public Camera InspectionCamera;
        private float DefaultFOV;

        [Tooltip("Button to start inspecting the weapon.")]
        public GameObject InspectWeaponButton;

        [Tooltip("Button to return to the default view.")]
        public GameObject ExitInspectButton;

        [Tooltip("List of weapons that can be inspected, including their FOV settings and deactivation objects.")]
        public List<WeaponData> Weapons = new List<WeaponData>();

        private void Start()
        {
            // Initialize default field of view
            Initialize();

            // Add listeners to the buttons
            if (InspectWeaponButton != null)
                InspectWeaponButton.GetComponent<Button>().onClick.AddListener(InspectWeapon);

            if (ExitInspectButton != null)
                ExitInspectButton.GetComponent<Button>().onClick.AddListener(BackToDefault);

            // Setup initial state
            SetupInitialState();
        }

        public void Initialize()
        {
            if (InspectionCamera != null)
                DefaultFOV = InspectionCamera.fieldOfView;
        }

        public void SetDefaultFOV()
        {
            if (InspectionCamera != null)
                InspectionCamera.fieldOfView = DefaultFOV;
        }

        public void ChangeFOV(float targetFOV, float duration)
        {
            if (InspectionCamera != null)
            {
                LeanTween.value(InspectionCamera.gameObject, InspectionCamera.fieldOfView, targetFOV, duration).setOnUpdate((float value) =>
                {
                    InspectionCamera.fieldOfView = value;
                });
            }
        }

        private void SetupInitialState()
        {
            // Ensure the BackButton is initially inactive
            ExitInspectButton.SetActive(false);
            InspectWeaponButton.SetActive(true);
        }

        public void InspectWeapon()
        {
            // Find the active weapon in the list
            WeaponData activeWeapon = Weapons.Find(weapon => weapon.WeaponModel != null && weapon.WeaponModel.activeSelf);

            if (activeWeapon != null)
            {
                // Perform the inspect behavior
                WeaponToInspect(activeWeapon);
            }
        }

        public void BackToDefault()
        {
            // Find the active weapon in the list
            WeaponData activeWeapon = Weapons.Find(weapon => weapon.WeaponModel != null && weapon.WeaponModel.activeSelf);

            if (activeWeapon != null)
            {
                // Reset to default setup
                GoBackToDefaultSetup(activeWeapon);
            }
        }

        private void WeaponToInspect(WeaponData weaponData)
        {
            // Activate the weapon to inspect
            weaponData.WeaponModel.SetActive(true);

            // Smoothly change the Camera's field of view to FieldOfViewValue over Duration
            ChangeFOV(weaponData.FieldOfViewValue, weaponData.Duration);

            // Deactivate specified objects
            foreach (var obj in weaponData.ObjToDeactivate)
            {
                if (obj != null)
                    obj.SetActive(false);
            }

            // Toggle button states
            InspectWeaponButton.SetActive(false);
            ExitInspectButton.SetActive(true);
        }

        private void GoBackToDefaultSetup(WeaponData weaponData)
        {
            // Reset camera to default FOV
            ChangeFOV(DefaultFOV, weaponData.Duration);

            // Reactivate objects to their default state
            foreach (var obj in weaponData.ObjToDeactivate)
            {
                if (obj != null)
                    obj.SetActive(true);
            }

            // Reset camera FOV to default
            SetDefaultFOV();

            // Toggle button states
            InspectWeaponButton.SetActive(true);
            ExitInspectButton.SetActive(false);
        }
    }

}


//using System.Collections.Generic;
//using UnityEngine;

//public class WeaponInspection : MonoBehaviour
//{
//    public float FieldOfViewValue;
//    public float Duration;
//    public Camera Cam;

//    private float DefaultFOV;
//    public GameObject[] ObjToDeactivate;

//    public GameObject InspectButtom;
//    public GameObject BackButton;

//    private void Start()
//    {
//        DefaultFOV = Cam.fieldOfView;
//    }
//    public void WeaponToInspect(GameObject Weapon)
//    {
//        Weapon.SetActive(true);

//        // Smoothly change the Camera's field of view to 5 over 1 second using LeanTween
//        LeanTween.value(Cam.gameObject, Cam.fieldOfView, FieldOfViewValue, Duration).setOnUpdate((float value) =>
//        {
//            Cam.fieldOfView = value;
//        });

//        for(int x = 0;x < ObjToDeactivate.Length; x++)
//        {
//            ObjToDeactivate[x].SetActive(false);
//        }

//        InspectButtom.SetActive(false);
//        BackButton.SetActive(true);
//    }
//    public void GoBackToDefaultSetup(GameObject Weapon)
//    {
//        Weapon.SetActive(true);

//        // Smoothly change the Camera's field of view to 5 over 1 second using LeanTween
//        LeanTween.value(Cam.gameObject, Cam.fieldOfView, DefaultFOV, Duration).setOnUpdate((float value) =>
//        {
//            Cam.fieldOfView = value;
//        });

//        for (int x = 0; x < ObjToDeactivate.Length; x++)
//        {
//            ObjToDeactivate[x].SetActive(true);
//        }

//        InspectButtom.SetActive(true);
//        BackButton.SetActive(false);
//    }
//}
