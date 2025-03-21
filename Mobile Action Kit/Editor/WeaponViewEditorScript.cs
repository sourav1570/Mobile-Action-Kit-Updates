using UnityEditor;
using UnityEngine;
using System.IO;
using MobileActionKit;


namespace MobileActionKit
{
    public class WeaponViewEditorScript : EditorWindow
    {
        public enum ModifyWeaponOption
        {
            ModifyPlayerWeaponWithoutOptics,
            ModifyPlayerWeaponWithOptics
        }

        private ModifyWeaponOption selectedOption;
        private GameObject weaponPivot;
        private PlayerWeapon PlayerWeaponScript;
        private WeaponOptics weaponOptics;
        private GameObject crossHair;

        private bool AdjustWeaponTransform = false;
        private bool isAimed;

        public Vector3 AimedWeaponPivotPosition;
        public Vector3 AimedWeaponPivotRotation;

        public Vector3 HipFireWeaponPivotPosition;
        public Vector3 HipFireWeaponPivotRotation;

        private bool enableCrosshair;

        public Camera PlayerCamera;
        public Camera WeaponCamera;

        //public bool OverrideValuesInRuntime = true;

        public float PlayerCameraDefaultFov = 60f;
        public float PlayerCameraMagnifiedFov = 45f;

        public float WeaponCameraDefaultFov = 60f;
        public float WeaponCameraMagnifiedFov = 10f;

        private const string SaveFilePath = "Assets/WeaponEditorData.json";

        [MenuItem("Tools/Mobile Action Kit/Player/FireArms/Weapon View", priority = 3)]
        public static void ShowWindow()
        {
            GetWindow<WeaponViewEditorScript>("Weapon View");
        }
        private void OnGUI()
        {
            // Set a dynamic label width based on the window size, making sure the label is always readable
            EditorGUIUtility.labelWidth = Mathf.Min(position.width * 0.4f, 250); // 40% of window width, capped at 250px

            // Title for the window
            GUILayout.Label("Modify Player Weapon", EditorStyles.boldLabel);

            // Weapon Settings Section
            EditorGUILayout.LabelField("Weapon Settings", EditorStyles.boldLabel);
            weaponPivot = (GameObject)EditorGUILayout.ObjectField("Weapon Pivot", weaponPivot, typeof(GameObject), true);
            PlayerWeaponScript = (PlayerWeapon)EditorGUILayout.ObjectField("Player Weapon Script", PlayerWeaponScript, typeof(PlayerWeapon), true);
            //OverrideValuesInRuntime = EditorGUILayout.Toggle("Override Values In Runtime", OverrideValuesInRuntime);

            // Camera Settings Section
            EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);
            PlayerCamera = (Camera)EditorGUILayout.ObjectField("Player Camera", PlayerCamera, typeof(Camera), true);
            WeaponCamera = (Camera)EditorGUILayout.ObjectField("Weapon Camera", WeaponCamera, typeof(Camera), true);

            // FOV Settings Section
            EditorGUILayout.LabelField("Field of View (FOV) Settings", EditorStyles.boldLabel);
            PlayerCameraDefaultFov = EditorGUILayout.FloatField("Player Camera Default FOV", PlayerCameraDefaultFov);
            PlayerCameraMagnifiedFov = EditorGUILayout.FloatField("Player Camera Magnified FOV", PlayerCameraMagnifiedFov);
            WeaponCameraDefaultFov = EditorGUILayout.FloatField("Weapon Camera Default FOV", WeaponCameraDefaultFov);
            WeaponCameraMagnifiedFov = EditorGUILayout.FloatField("Weapon Camera Magnified FOV", WeaponCameraMagnifiedFov);

            // Aiming and Adjustment Settings
            EditorGUILayout.LabelField("Aiming & Adjustment Settings", EditorStyles.boldLabel);
            isAimed = EditorGUILayout.Toggle("Is Aimed", isAimed);
            AdjustWeaponTransform = EditorGUILayout.Toggle("Adjust Weapon Transform", AdjustWeaponTransform);

            // Option Selection Section
            EditorGUILayout.LabelField("Weapon Modification Options", EditorStyles.boldLabel);
            selectedOption = (ModifyWeaponOption)EditorGUILayout.EnumPopup("Select Option", selectedOption);

            switch (selectedOption)
            {
                case ModifyWeaponOption.ModifyPlayerWeaponWithoutOptics:
                    ShowPlayerWeaponWithoutOptics();
                    break;
                case ModifyWeaponOption.ModifyPlayerWeaponWithOptics:
                    weaponOptics = (WeaponOptics)EditorGUILayout.ObjectField("Weapon Optics", weaponOptics, typeof(WeaponOptics), true);
                    ShowPlayerWeaponWithOptics();
                    break;
            }

            // Crosshair Settings Section
            EditorGUILayout.LabelField("CrossHair Settings", EditorStyles.boldLabel);
            crossHair = (GameObject)EditorGUILayout.ObjectField("CrossHair", crossHair, typeof(GameObject), true);
            enableCrosshair = EditorGUILayout.Toggle("Enable Crosshair", enableCrosshair);

            // Action Buttons
            EditorGUILayout.Space();
            if (GUILayout.Button("Store Values"))
            {
                SaveValues();
            }
            if (GUILayout.Button("Apply Values"))
            {
                LoadValues();
            }

            // Toggle Crosshair visibility based on the setting
            if (crossHair != null)
            {
                crossHair.gameObject.SetActive(enableCrosshair);
            }

            // Reset label width back to default after drawing the GUI
            EditorGUIUtility.labelWidth = 0;
        }

        //private void OnGUI()
        //{
        //    GUILayout.Label("Modify Player Weapon", EditorStyles.boldLabel);

        //    weaponPivot = (GameObject)EditorGUILayout.ObjectField("Weapon Pivot", weaponPivot, typeof(GameObject), true);
        //    playerWeapon = (PlayerWeapon)EditorGUILayout.ObjectField("Player Weapon", playerWeapon, typeof(PlayerWeapon), true);

        //    OverrideValuesInRuntime = EditorGUILayout.Toggle("OverrideValuesInRuntime", OverrideValuesInRuntime);


        //    PlayerCamera = (Camera)EditorGUILayout.ObjectField("Player Camera", PlayerCamera, typeof(Camera), true);
        //    WeaponCamera = (Camera)EditorGUILayout.ObjectField("Weapon Camera", WeaponCamera, typeof(Camera), true);


        //    PlayerCameraDefaultFov = EditorGUILayout.FloatField("Player Camera Default Fov", PlayerCameraDefaultFov);
        //    PlayerCameraAimedFov = EditorGUILayout.FloatField("Player Camera Aimed Fov", PlayerCameraAimedFov);
        //    WeaponCameraHipFireFov = EditorGUILayout.FloatField("Weapon Camera HipFire Fov", WeaponCameraHipFireFov);
        //    WeaponCameraAimedFov = EditorGUILayout.FloatField("Weapon Camera Aimed Fov", WeaponCameraAimedFov);

        //    isAimed = EditorGUILayout.Toggle("Is Aimed", isAimed);
        //    AdjustWeapon = EditorGUILayout.Toggle("Adjust Weapon", AdjustWeapon);

        //    selectedOption = (ModifyWeaponOption)EditorGUILayout.EnumPopup("Select Option", selectedOption);

        //    switch (selectedOption)
        //    {
        //        case ModifyWeaponOption.ModifyPlayerWeaponWithoutOptics:
        //            ShowPlayerWeaponWithoutOptics();
        //            break;
        //        case ModifyWeaponOption.ModifyPlayerWeaponWithOptics:
        //            weaponOptics = (WeaponOptics)EditorGUILayout.ObjectField("Weapon Optics", weaponOptics, typeof(WeaponOptics), true);
        //            ShowPlayerWeaponWithOptics();
        //            break;
        //    }

        //    EditorGUILayout.Space();

        //    // CrossHair section
        //    EditorGUILayout.LabelField("CrossHair Settings", EditorStyles.boldLabel);
        //    crossHair = (GameObject)EditorGUILayout.ObjectField("CrossHair", crossHair, typeof(GameObject), true);
        //    enableCrosshair = EditorGUILayout.Toggle("Enable Crosshair", enableCrosshair);

        //    if (GUILayout.Button("Copy Values in Script"))
        //    {
        //        SaveValues();

        //    }
        //    if (GUILayout.Button("Paste Values in Script"))
        //    {
        //        LoadValues();
        //    }


        //    if (crossHair != null)
        //    {
        //        if (enableCrosshair == true)
        //        {
        //            crossHair.gameObject.SetActive(true);
        //        }
        //        else
        //        {
        //            crossHair.gameObject.SetActive(false);
        //        }
        //    }
        //}

        private void ShowPlayerWeaponWithoutOptics()
        {
            //if (OverrideValuesInRuntime == true)
            //{
            //    if (PlayerWeaponScript != null)
            //    {
            //        PlayerWeaponScript.CamerasFovParameters.DefaultPlayerCamFov = PlayerCameraDefaultFov;
            //        PlayerWeaponScript.CamerasFovParameters.PlayerCameraMagnifiedFov = PlayerCameraMagnifiedFov;
            //        PlayerWeaponScript.CamerasFovParameters.DefaultWeaponCamFov = WeaponCameraDefaultFov;
            //        PlayerWeaponScript.CamerasFovParameters.WeaponCameraMagnifiedFov = WeaponCameraMagnifiedFov;
            //    }

            //}

            HipFireWeaponPivotPosition = EditorGUILayout.Vector3Field("Hip Fire Weapon Pivot Position", HipFireWeaponPivotPosition);
            HipFireWeaponPivotRotation = EditorGUILayout.Vector3Field("Hip Fire Weapon Pivot Rotation", HipFireWeaponPivotRotation);

            AimedWeaponPivotPosition = EditorGUILayout.Vector3Field("Aimed Weapon Pivot Position", AimedWeaponPivotPosition);
            AimedWeaponPivotRotation = EditorGUILayout.Vector3Field("Aimed Weapon Pivot Rotation", AimedWeaponPivotRotation);

            if (isAimed)
            {
                if (PlayerCamera != null)
                {
                    PlayerCamera.fieldOfView = PlayerCameraMagnifiedFov;
                  
                }
                if (WeaponCamera != null)
                {
                    WeaponCamera.fieldOfView = WeaponCameraMagnifiedFov;

                }
 

            }
            else
            {
                if (PlayerCamera != null)
                {

                    PlayerCamera.fieldOfView = PlayerCameraDefaultFov;
                   
                }
                if (WeaponCamera != null)
                {
                    WeaponCamera.fieldOfView = WeaponCameraDefaultFov;

                }
            }

            if (weaponPivot != null && AdjustWeaponTransform == true)
            {
                if (isAimed)
                {

                    if (PlayerWeaponScript != null)
                    {
                        PlayerCamera.fieldOfView = PlayerCameraMagnifiedFov;
                    }

                    if (weaponPivot != null)
                    {
                        if (AimedWeaponPivotPosition != null)
                        {
                            weaponPivot.transform.localPosition = AimedWeaponPivotPosition;

                        }
                        if (AimedWeaponPivotRotation != null)
                        {
                            weaponPivot.transform.localRotation = Quaternion.Euler(AimedWeaponPivotRotation);
                        }
                    }



                }
                else
                {
                    if (PlayerWeaponScript != null)
                    {
                        PlayerCamera.fieldOfView = PlayerCameraDefaultFov;
                    }
                    if (weaponPivot != null)
                    {
                        if (HipFireWeaponPivotPosition != null)
                        {
                            weaponPivot.transform.localPosition = HipFireWeaponPivotPosition;

                        }
                        if (HipFireWeaponPivotRotation != null)
                        {
                            weaponPivot.transform.localRotation = Quaternion.Euler(HipFireWeaponPivotRotation);
                        }
                    }

                }
            }
            else
            {
                if (isAimed)
                {
                    if (PlayerWeaponScript != null)
                    {
                        PlayerCamera.fieldOfView = PlayerCameraMagnifiedFov;
                    }
                    AimedWeaponPivotPosition = weaponPivot.transform.localPosition;
                    AimedWeaponPivotRotation = weaponPivot.transform.localEulerAngles;
                }
                else
                {
                    if (PlayerCamera != null)
                    {
                        PlayerCamera.fieldOfView = PlayerCameraDefaultFov;
                    }
                    if (HipFireWeaponPivotPosition != null)
                    {
                        if (weaponPivot != null)
                        {
                            HipFireWeaponPivotPosition = weaponPivot.transform.localPosition;

                        }
                    }
                    if (HipFireWeaponPivotRotation != null)
                    {
                        if (weaponPivot != null)
                        {
                            HipFireWeaponPivotRotation = weaponPivot.transform.localEulerAngles;
                        }
                    }
                }
            }

        }

        private void ShowPlayerWeaponWithOptics()
        {
            AimedWeaponPivotPosition = EditorGUILayout.Vector3Field("Aimed Weapon Pivot Position", AimedWeaponPivotPosition);
            AimedWeaponPivotRotation = EditorGUILayout.Vector3Field("Aimed Weapon Pivot Rotation", AimedWeaponPivotRotation);

            if (weaponPivot != null && AdjustWeaponTransform == true)
            {
                if (isAimed)
                {
                    if (PlayerCamera != null)
                    {
                        PlayerCamera.fieldOfView = PlayerCameraMagnifiedFov;

                    }
                    if (WeaponCamera != null)
                    {
                        WeaponCamera.fieldOfView = WeaponCameraMagnifiedFov;

                    }


                }
                else
                {
                    if (PlayerCamera != null)
                    {

                        PlayerCamera.fieldOfView = PlayerCameraDefaultFov;

                    }
                    if (WeaponCamera != null)
                    {
                        WeaponCamera.fieldOfView = WeaponCameraDefaultFov;

                    }
                }

                if (isAimed)
                {
                    if (weaponPivot != null)
                    {
                        if (AimedWeaponPivotPosition != null)
                        {
                            weaponPivot.transform.localPosition = AimedWeaponPivotPosition;

                        }
                        if (AimedWeaponPivotRotation != null)
                        {
                            weaponPivot.transform.localRotation = Quaternion.Euler(AimedWeaponPivotRotation);
                        }
                    }
                }
                else
                {
                    if (weaponPivot != null)
                    {
                        if (HipFireWeaponPivotPosition != null)
                        {
                            weaponPivot.transform.localPosition = HipFireWeaponPivotPosition;

                        }
                        if (HipFireWeaponPivotRotation != null)
                        {
                            weaponPivot.transform.localRotation = Quaternion.Euler(HipFireWeaponPivotRotation);
                        }
                    }

                }

            }
            else
            {
                if (isAimed)
                {
                    AimedWeaponPivotPosition = weaponPivot.transform.localPosition;
                    AimedWeaponPivotRotation = weaponPivot.transform.localEulerAngles;
                }
                else
                {
                    if (HipFireWeaponPivotPosition != null)
                    {
                        if (weaponPivot != null)
                        {
                            HipFireWeaponPivotPosition = weaponPivot.transform.localPosition;

                        }
                    }
                    if (HipFireWeaponPivotRotation != null)
                    {
                        if (weaponPivot != null)
                        {
                            HipFireWeaponPivotRotation = weaponPivot.transform.localEulerAngles;
                        }
                    }
                }
            }
        }

        private void SaveValues()
        {
            WeaponEditorData data;
            if (selectedOption == ModifyWeaponOption.ModifyPlayerWeaponWithoutOptics)
            {
                data = new WeaponEditorData
                {
                    SelectedOption = selectedOption,
                    IsAimed = isAimed,
                    AimedWeaponPivotPosition = AimedWeaponPivotPosition,
                    AimedWeaponPivotRotation = AimedWeaponPivotRotation,
                    HipFireWeaponPivotPosition = HipFireWeaponPivotPosition,
                    HipFireWeaponPivotRotation = HipFireWeaponPivotRotation,

                    PlayerCameraDefaultFov = PlayerCameraDefaultFov,
                    PlayerCameraAimedFov = PlayerCameraMagnifiedFov,
                    WeaponCameraHipFireFov = WeaponCameraDefaultFov,
                    WeaponCameraAimedFov = WeaponCameraMagnifiedFov,

                    EnableCrosshair = enableCrosshair
                };
            }
            else
            {
                data = new WeaponEditorData
                {
                    SelectedOption = selectedOption,
                    IsAimed = isAimed,
                    AimedWeaponPivotPosition = AimedWeaponPivotPosition,
                    AimedWeaponPivotRotation = AimedWeaponPivotRotation,
                    HipFireWeaponPivotPosition = HipFireWeaponPivotPosition,
                    HipFireWeaponPivotRotation = HipFireWeaponPivotRotation,

                    PlayerCameraDefaultFov = PlayerCameraDefaultFov,
                    PlayerCameraAimedFov = PlayerCameraMagnifiedFov,
                    WeaponCameraHipFireFov = WeaponCameraDefaultFov,
                    WeaponCameraAimedFov = WeaponCameraMagnifiedFov,

                    EnableCrosshair = enableCrosshair
                };
            }


            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SaveFilePath, json);

            Debug.Log("Values saved successfully.");
        }

        private void LoadValues()
        {
            if (File.Exists(SaveFilePath))
            {
                string json = File.ReadAllText(SaveFilePath);
                WeaponEditorData data = JsonUtility.FromJson<WeaponEditorData>(json);


                if (selectedOption == ModifyWeaponOption.ModifyPlayerWeaponWithoutOptics)
                {
                    selectedOption = data.SelectedOption;
                    isAimed = data.IsAimed;
                    PlayerWeaponScript.WeaponPositionsRotationsDurations.AimedWeaponPivotPosition = data.AimedWeaponPivotPosition;
                    PlayerWeaponScript.WeaponPositionsRotationsDurations.AimedWeaponPivotRotation = data.AimedWeaponPivotRotation;
                    PlayerWeaponScript.WeaponPositionsRotationsDurations.HipFireWeaponPivotPosition = data.HipFireWeaponPivotPosition;
                    PlayerWeaponScript.WeaponPositionsRotationsDurations.HipFireWeaponPivotRotation = data.HipFireWeaponPivotRotation;

                    PlayerWeaponScript.CamerasFovParameters.DefaultPlayerCamFov = data.PlayerCameraDefaultFov;
                    PlayerWeaponScript.CamerasFovParameters.PlayerCameraMagnifiedFov = data.PlayerCameraAimedFov;
                    PlayerWeaponScript.CamerasFovParameters.DefaultWeaponCamFov = data.WeaponCameraHipFireFov;
                    PlayerWeaponScript.CamerasFovParameters.WeaponCameraMagnifiedFov = data.WeaponCameraAimedFov;

                    enableCrosshair = data.EnableCrosshair;
                }
                else
                {
                    selectedOption = data.SelectedOption;
                    isAimed = data.IsAimed;

                    weaponOptics.OverrideAimedWeaponPivotValues.AimedWeaponPivotPosition = data.AimedWeaponPivotPosition;
                    weaponOptics.OverrideAimedWeaponPivotValues.AimedWeaponPivotRotation = data.AimedWeaponPivotRotation;

                    weaponOptics.OverrideCamerasFovParameters.PlayerCameraMagnifiedFov = data.PlayerCameraAimedFov;

                    weaponOptics.OverrideCamerasFovParameters.WeaponCameraMagnifiedFov = data.WeaponCameraAimedFov;

                    enableCrosshair = data.EnableCrosshair;
                }


                Debug.Log("Loaded values from saved data.");
            }
        }
    }

    [System.Serializable]
    public class WeaponEditorData
    {
        public WeaponViewEditorScript.ModifyWeaponOption SelectedOption;
        public bool IsAimed;
        public Vector3 AimedWeaponPivotPosition;
        public Vector3 AimedWeaponPivotRotation;
        public Vector3 HipFireWeaponPivotPosition;
        public Vector3 HipFireWeaponPivotRotation;

        public float PlayerCameraDefaultFov;
        public float PlayerCameraAimedFov;
        public float WeaponCameraHipFireFov;
        public float WeaponCameraAimedFov;

        public bool EnableCrosshair;
    }

}


//using UnityEditor;
//using UnityEngine;
//using System.IO;
//using System.Collections.Generic;
//using MobileActionKit;

//public class WeaponEditorWindow : EditorWindow
//{
//    public enum ModifyWeaponOption
//    {
//        ModifyPlayerWeaponWithoutOptics,
//        ModifyPlayerWeaponWithOptics
//    }

//    private ModifyWeaponOption selectedOption;
//    private GameObject weaponPivot;
//    private PlayerWeapon playerWeapon;
//    private WeaponOptics weaponOptics;

//    public Camera PlayerCamera;
//    public Camera WeaponCamera;

//    public float PlayerCameraDefaultFov = 60f;
//    public float PlayerCameraAimedFov = 15f;

//    public float WeaponCameraHipFireFov = 60f;
//    public float WeaponCameraAimedFov = 10f;

//    private GameObject crossHair;
//    private bool AdjustWeapon = false;
//    private bool isAimed;

//    public Vector3 AimedWeaponPivotPosition;
//    public Vector3 AimedWeaponPivotRotation;

//    public Vector3 HipFireWeaponPivotPosition;
//    public Vector3 HipFireWeaponPivotRotation;

//    private bool enableCrosshair;

//    private const string SaveFilePath = "Assets/WeaponEditorData.json";

//    [MenuItem("Window/Weapon Editor")]
//    public static void ShowWindow()
//    {
//        GetWindow<WeaponEditorWindow>("Weapon Editor");
//    }

//    private void OnGUI()
//    {
//        GUILayout.Label("Modify Player Weapon", EditorStyles.boldLabel);

//        PlayerCamera = (Camera)EditorGUILayout.ObjectField("Player Camera", PlayerCamera, typeof(Camera), true);
//        WeaponCamera = (Camera)EditorGUILayout.ObjectField("Weapon Camera", WeaponCamera, typeof(Camera), true);

//        PlayerCameraDefaultFov = EditorGUILayout.FloatField("Player Camera Default Fov", PlayerCameraDefaultFov);
//        PlayerCameraAimedFov = EditorGUILayout.FloatField("Player Camera Aimed Fov", PlayerCameraAimedFov);
//        WeaponCameraHipFireFov = EditorGUILayout.FloatField("Weapon Camera HipFire Fov", WeaponCameraHipFireFov);
//        WeaponCameraAimedFov = EditorGUILayout.FloatField("Weapon Camera Aimed Fov", WeaponCameraAimedFov);

//        selectedOption = (ModifyWeaponOption)EditorGUILayout.EnumPopup("Select Option", selectedOption);

//        switch (selectedOption)
//        {
//            case ModifyWeaponOption.ModifyPlayerWeaponWithoutOptics:
//                ShowPlayerWeaponWithoutOptics();
//                break;
//            case ModifyWeaponOption.ModifyPlayerWeaponWithOptics:
//                ShowPlayerWeaponWithOptics();
//                break;
//        }

//        EditorGUILayout.Space();

//        // CrossHair section
//        EditorGUILayout.LabelField("CrossHair Settings", EditorStyles.boldLabel);
//        crossHair = (GameObject)EditorGUILayout.ObjectField("CrossHair", crossHair, typeof(GameObject), true);
//        enableCrosshair = EditorGUILayout.Toggle("Enable Crosshair", enableCrosshair);

//        if (GUILayout.Button("Save Values in Script"))
//        {
//            SaveValues();
//            LoadValues();
//        }

//        if (enableCrosshair == true)
//        {
//            crossHair?.SetActive(true);
//        }
//        else
//        {
//            crossHair?.SetActive(false);
//        }

//        EditorGUILayout.Space();

//    }

//    private void ShowPlayerWeaponWithoutOptics()
//    {
//        weaponPivot = (GameObject)EditorGUILayout.ObjectField("Weapon Pivot", weaponPivot, typeof(GameObject), true);
//        playerWeapon = (PlayerWeapon)EditorGUILayout.ObjectField("Player Weapon", playerWeapon, typeof(PlayerWeapon), true);

//        isAimed = EditorGUILayout.Toggle("Is Aimed", isAimed);
//        AdjustWeapon = EditorGUILayout.Toggle("Adjust Weapon", AdjustWeapon);

//        if (isAimed)
//        {
//            PlayerCamera.fieldOfView = PlayerCameraAimedFov;
//            AimedWeaponPivotPosition = EditorGUILayout.Vector3Field("Aimed Weapon Pivot Position", AimedWeaponPivotPosition);
//            AimedWeaponPivotRotation = EditorGUILayout.Vector3Field("Aimed Weapon Pivot Rotation", AimedWeaponPivotRotation);
//        }
//        else
//        {
//            PlayerCamera.fieldOfView = PlayerCameraDefaultFov;
//            HipFireWeaponPivotPosition = EditorGUILayout.Vector3Field("Hip Fire Weapon Pivot Position", HipFireWeaponPivotPosition);
//            HipFireWeaponPivotRotation = EditorGUILayout.Vector3Field("Hip Fire Weapon Pivot Rotation", HipFireWeaponPivotRotation);
//        }

//        if (weaponPivot != null && AdjustWeapon == true)
//        {
//            if (isAimed)
//            {
//                PlayerCamera.fieldOfView = PlayerCameraAimedFov;
//                weaponPivot.transform.localPosition = AimedWeaponPivotPosition;
//                weaponPivot.transform.localRotation = Quaternion.Euler(AimedWeaponPivotRotation);
//            }
//            else
//            {
//                PlayerCamera.fieldOfView = PlayerCameraDefaultFov;
//                weaponPivot.transform.localPosition = HipFireWeaponPivotPosition;
//                weaponPivot.transform.localRotation = Quaternion.Euler(HipFireWeaponPivotRotation);
//            }
//        }
//        else
//        {
//            if (isAimed)
//            {
//                PlayerCamera.fieldOfView = PlayerCameraAimedFov;
//                AimedWeaponPivotPosition = weaponPivot.transform.localPosition;
//                AimedWeaponPivotRotation = weaponPivot.transform.localEulerAngles;
//            }
//            else
//            {
//                PlayerCamera.fieldOfView = PlayerCameraDefaultFov;
//                HipFireWeaponPivotPosition = weaponPivot.transform.localPosition;
//                HipFireWeaponPivotRotation = weaponPivot.transform.localEulerAngles;
//            }
//        }
//    }

//    private void ShowPlayerWeaponWithOptics()
//    {
//        weaponPivot = (GameObject)EditorGUILayout.ObjectField("Weapon Pivot", weaponPivot, typeof(GameObject), true);
//        weaponOptics = (WeaponOptics)EditorGUILayout.ObjectField("Weapon Optics", weaponOptics, typeof(WeaponOptics), true);

//        AimedWeaponPivotPosition = EditorGUILayout.Vector3Field("Aimed Weapon Pivot Position", AimedWeaponPivotPosition);
//        AimedWeaponPivotRotation = EditorGUILayout.Vector3Field("Aimed Weapon Pivot Rotation", AimedWeaponPivotRotation);

//        if (weaponPivot != null && isAimed)
//        {
//            weaponPivot.transform.localPosition = AimedWeaponPivotPosition;
//            weaponPivot.transform.localRotation = Quaternion.Euler(AimedWeaponPivotRotation);
//        }
//        else
//        {
//            AimedWeaponPivotPosition = weaponPivot.transform.localPosition;
//            AimedWeaponPivotRotation = weaponPivot.transform.localEulerAngles;
//        }
//    }

//    private void SaveValues()
//    {
//        WeaponEditorData data = new WeaponEditorData
//        {
//            SelectedOption = selectedOption,
//            IsAimed = isAimed,
//            AimedWeaponPivotPosition = AimedWeaponPivotPosition,
//            AimedWeaponPivotRotation = AimedWeaponPivotRotation,
//            HipFireWeaponPivotPosition = HipFireWeaponPivotPosition,
//            HipFireWeaponPivotRotation = HipFireWeaponPivotRotation,
//            EnableCrosshair = enableCrosshair
//        };

//        string json = JsonUtility.ToJson(data, true);
//        File.WriteAllText(SaveFilePath, json);

//        Debug.Log("Values saved successfully.");
//    }

//    private void LoadValues()
//    {
//        if (File.Exists(SaveFilePath))
//        {
//            string json = File.ReadAllText(SaveFilePath);
//            WeaponEditorData data = JsonUtility.FromJson<WeaponEditorData>(json);

//            selectedOption = data.SelectedOption;
//            isAimed = data.IsAimed;
//            AimedWeaponPivotPosition = data.AimedWeaponPivotPosition;
//            AimedWeaponPivotRotation = data.AimedWeaponPivotRotation;
//            HipFireWeaponPivotPosition = data.HipFireWeaponPivotPosition;
//            HipFireWeaponPivotRotation = data.HipFireWeaponPivotRotation;
//            enableCrosshair = data.EnableCrosshair;

//            Debug.Log("Values loaded successfully.");
//        }
//        else
//        {
//            Debug.LogError("No saved data found.");
//        }
//    } 

//    [System.Serializable]
//    public class VisualRaycast
//    {
//        public GameObject raycastOrigin;
//        public Color rayColor = Color.red;
//    }

//    [System.Serializable]
//    public class WeaponEditorData
//    {
//        public ModifyWeaponOption SelectedOption;
//        public bool IsAimed;
//        public Vector3 AimedWeaponPivotPosition;
//        public Vector3 AimedWeaponPivotRotation;
//        public Vector3 HipFireWeaponPivotPosition;
//        public Vector3 HipFireWeaponPivotRotation;
//        public bool EnableCrosshair;
//    }
//}




//using UnityEditor;
//using UnityEngine;
//using System.IO;
//using System.Collections.Generic;
//using MobileActionKit;

//public class WeaponEditorWindow : EditorWindow
//{
//    public enum ModifyWeaponOption
//    {
//        ModifyPlayerWeaponWithoutOptics,
//        ModifyPlayerWeaponWithOptics
//    }

//    private ModifyWeaponOption selectedOption;
//    private GameObject weaponPivot;
//    private PlayerWeapon playerWeapon;
//    private WeaponOptics weaponOptics;

//    public Camera PlayerCamera;
//    public Camera WeaponCamera;

//    public float PlayerCameraDefaultFov = 60f;
//    public float PlayerCameraAimedFov = 15f;

//    public float WeaponCameraHipFireFov = 60f;
//    public float WeaponCameraAimedFov = 10f;


//    private GameObject crossHair;

//    private bool AdjustWeapon = false;
//    private bool isAimed;

//    public Vector3 AimedWeaponPivotPosition;
//    public Vector3 AimedWeaponPivotRotation;

//    public Vector3 HipFireWeaponPivotPosition;
//    public Vector3 HipFireWeaponPivotRotation;

//    private bool enableCrosshair;

//    private bool enableVisualRaycasts;
//    private float raycastLength = 5f;
//    private List<VisualRaycast> visualRaycasts = new List<VisualRaycast>();

//    private const string SaveFilePath = "Assets/WeaponEditorData.json";

//    [MenuItem("Window/Weapon Editor")]
//    public static void ShowWindow()
//    {
//        GetWindow<WeaponEditorWindow>("Weapon Editor");
//    }

//    private void OnGUI()
//    {
//        GUILayout.Label("Modify Player Weapon", EditorStyles.boldLabel);

//        PlayerCamera = (Camera)EditorGUILayout.ObjectField("Player Camera", PlayerCamera, typeof(Camera), true);
//        WeaponCamera = (Camera)EditorGUILayout.ObjectField("Weapon Camera", WeaponCamera, typeof(Camera), true);

//        PlayerCameraDefaultFov = EditorGUILayout.FloatField("Player Camera Default Fov", PlayerCameraDefaultFov);
//        PlayerCameraAimedFov = EditorGUILayout.FloatField("Player Camera Aimed Fov", PlayerCameraAimedFov);
//        WeaponCameraHipFireFov = EditorGUILayout.FloatField("Weapon Camera HipFire Fov", WeaponCameraHipFireFov);
//        WeaponCameraAimedFov = EditorGUILayout.FloatField("Weapon Camera Aimed Fov", WeaponCameraAimedFov);

//        selectedOption = (ModifyWeaponOption)EditorGUILayout.EnumPopup("Select Option", selectedOption);

//        switch (selectedOption)
//        {
//            case ModifyWeaponOption.ModifyPlayerWeaponWithoutOptics:
//                ShowPlayerWeaponWithoutOptics();
//                break;
//            case ModifyWeaponOption.ModifyPlayerWeaponWithOptics:
//                ShowPlayerWeaponWithOptics();
//                break;
//        }

//        EditorGUILayout.Space();

//        // CrossHair section
//        EditorGUILayout.LabelField("CrossHair Settings", EditorStyles.boldLabel);
//        crossHair = (GameObject)EditorGUILayout.ObjectField("CrossHair", crossHair, typeof(GameObject), true);
//        enableCrosshair = EditorGUILayout.Toggle("Enable Crosshair", enableCrosshair);



//        if (GUILayout.Button("Save Values in Script"))
//        {
//            SaveValues();
//            LoadValues();
//        }

//        if (enableCrosshair == true)
//        {
//            crossHair?.SetActive(true);
//        }
//        else
//        {
//            crossHair?.SetActive(false);
//        }

//        EditorGUILayout.Space();
//        GUILayout.Label("Visual Raycasts", EditorStyles.boldLabel);
//        enableVisualRaycasts = EditorGUILayout.Toggle("Enable Visual Raycasts", enableVisualRaycasts);

//        if (enableVisualRaycasts)
//        {
//            raycastLength = EditorGUILayout.FloatField("Raycast Length", raycastLength);

//            if (GUILayout.Button("Add Visual Raycast"))
//            {
//                visualRaycasts.Add(new VisualRaycast());
//            }

//            for (int i = 0; i < visualRaycasts.Count; i++)
//            {
//                EditorGUILayout.BeginHorizontal();
//                visualRaycasts[i].raycastOrigin = (GameObject)EditorGUILayout.ObjectField("Raycast Origin", visualRaycasts[i].raycastOrigin, typeof(GameObject), true);
//                visualRaycasts[i].rayColor = EditorGUILayout.ColorField("Ray Color", visualRaycasts[i].rayColor);

//                if (GUILayout.Button("Remove"))
//                {
//                    visualRaycasts.RemoveAt(i);
//                }
//                EditorGUILayout.EndHorizontal();
//            }
//        }
//    }

//    private void ShowPlayerWeaponWithoutOptics()
//    {
//        weaponPivot = (GameObject)EditorGUILayout.ObjectField("Weapon Pivot", weaponPivot, typeof(GameObject), true);
//        playerWeapon = (PlayerWeapon)EditorGUILayout.ObjectField("Player Weapon", playerWeapon, typeof(PlayerWeapon), true);

//        isAimed = EditorGUILayout.Toggle("Is Aimed", isAimed);
//        AdjustWeapon = EditorGUILayout.Toggle("Adjust Weapon", AdjustWeapon);

//        if (isAimed)
//        {
//            PlayerCamera.fieldOfView = PlayerCameraAimedFov;
//            AimedWeaponPivotPosition = EditorGUILayout.Vector3Field("Aimed Weapon Pivot Position", AimedWeaponPivotPosition);
//            AimedWeaponPivotRotation = EditorGUILayout.Vector3Field("Aimed Weapon Pivot Rotation", AimedWeaponPivotRotation);
//        }
//        else
//        {
//            PlayerCamera.fieldOfView = PlayerCameraDefaultFov;
//            HipFireWeaponPivotPosition = EditorGUILayout.Vector3Field("Hip Fire Weapon Pivot Position", HipFireWeaponPivotPosition);
//            HipFireWeaponPivotRotation = EditorGUILayout.Vector3Field("Hip Fire Weapon Pivot Rotation", HipFireWeaponPivotRotation);
//        }

//        if (weaponPivot != null && AdjustWeapon == true)
//        {
//            if (isAimed)
//            {
//                PlayerCamera.fieldOfView = PlayerCameraAimedFov;
//                weaponPivot.transform.localPosition = AimedWeaponPivotPosition;
//                weaponPivot.transform.localRotation = Quaternion.Euler(AimedWeaponPivotRotation);
//            }
//            else
//            {
//                PlayerCamera.fieldOfView = PlayerCameraDefaultFov;
//                weaponPivot.transform.localPosition = HipFireWeaponPivotPosition;
//                weaponPivot.transform.localRotation = Quaternion.Euler(HipFireWeaponPivotRotation);
//            }
//        }
//        else
//        {
//            if (isAimed)
//            {
//                PlayerCamera.fieldOfView = PlayerCameraAimedFov;
//                AimedWeaponPivotPosition = weaponPivot.transform.localPosition;
//                AimedWeaponPivotRotation = weaponPivot.transform.localEulerAngles;
//            }
//            else
//            {
//                PlayerCamera.fieldOfView = PlayerCameraDefaultFov;
//                HipFireWeaponPivotPosition = weaponPivot.transform.localPosition;
//                HipFireWeaponPivotRotation = weaponPivot.transform.localEulerAngles;
//            }
//        }
//    }

//    private void ShowPlayerWeaponWithOptics()
//    {
//        weaponPivot = (GameObject)EditorGUILayout.ObjectField("Weapon Pivot", weaponPivot, typeof(GameObject), true);
//        weaponOptics = (WeaponOptics)EditorGUILayout.ObjectField("Weapon Optics", weaponOptics, typeof(WeaponOptics), true);

//        AimedWeaponPivotPosition = EditorGUILayout.Vector3Field("Aimed Weapon Pivot Position", AimedWeaponPivotPosition);
//        AimedWeaponPivotRotation = EditorGUILayout.Vector3Field("Aimed Weapon Pivot Rotation", AimedWeaponPivotRotation);

//        if (weaponPivot != null && isAimed)
//        {
//            weaponPivot.transform.localPosition = AimedWeaponPivotPosition;
//            weaponPivot.transform.localRotation = Quaternion.Euler(AimedWeaponPivotRotation);
//        }
//        else
//        {

//            AimedWeaponPivotPosition = weaponPivot.transform.localPosition;
//            AimedWeaponPivotRotation = weaponPivot.transform.localEulerAngles;
//        }
//    }

//    private void SaveValues()
//    {
//        WeaponEditorData data = new WeaponEditorData
//        {
//            SelectedOption = selectedOption,
//            IsAimed = isAimed,
//            AimedWeaponPivotPosition = AimedWeaponPivotPosition,
//            AimedWeaponPivotRotation = AimedWeaponPivotRotation,
//            HipFireWeaponPivotPosition = HipFireWeaponPivotPosition,
//            HipFireWeaponPivotRotation = HipFireWeaponPivotRotation,
//            EnableCrosshair = enableCrosshair
//        };

//        string json = JsonUtility.ToJson(data, true);
//        File.WriteAllText(SaveFilePath, json);

//        Debug.Log("Values saved successfully.");
//    }

//    private void LoadValues()
//    {
//        if (File.Exists(SaveFilePath))
//        {
//            string json = File.ReadAllText(SaveFilePath);
//            WeaponEditorData data = JsonUtility.FromJson<WeaponEditorData>(json);

//            selectedOption = data.SelectedOption;
//            isAimed = data.IsAimed;
//            AimedWeaponPivotPosition = data.AimedWeaponPivotPosition;
//            AimedWeaponPivotRotation = data.AimedWeaponPivotRotation;
//            HipFireWeaponPivotPosition = data.HipFireWeaponPivotPosition;
//            HipFireWeaponPivotRotation = data.HipFireWeaponPivotRotation;
//            enableCrosshair = data.EnableCrosshair;

//            Debug.Log("Loaded values from saved data.");
//        }
//    }
//    private void OnDrawGizmos()
//    {
//        if (enableVisualRaycasts && visualRaycasts != null && visualRaycasts.Count > 0)
//        {
//            foreach (var visualRaycast in visualRaycasts)
//            {
//                if (visualRaycast.raycastOrigin != null)
//                {
//                    // Draw the ray in the Game view using Debug.DrawRay
//                    Vector3 origin = visualRaycast.raycastOrigin.transform.position;
//                    Vector3 direction = visualRaycast.raycastOrigin.transform.forward * raycastLength;
//                    Debug.DrawRay(origin, direction, visualRaycast.rayColor, 0, false);  // The last parameter "false" means it won't persist after play mode ends
//                }
//            }
//        }
//    }


//    // Use SceneView.duringSceneGui to draw Gizmos in the Scene view
//    [InitializeOnLoad]
//    public static class SceneViewGizmoDrawer
//    {
//        static SceneViewGizmoDrawer()
//        {
//            // Register callback for drawing gizmos
//            SceneView.duringSceneGui += OnSceneGUI;
//        }

//        static void OnSceneGUI(SceneView sceneView)
//        {
//            // Draw visual raycasts
//            WeaponEditorWindow window = GetWindow<WeaponEditorWindow>();
//            if (window.enableVisualRaycasts && window.visualRaycasts != null && window.visualRaycasts.Count > 0)
//            {
//                foreach (var visualRaycast in window.visualRaycasts)
//                {
//                    if (visualRaycast.raycastOrigin != null)
//                    {
//                        Handles.color = visualRaycast.rayColor;
//                        Vector3 origin = visualRaycast.raycastOrigin.transform.position;
//                        Vector3 direction = visualRaycast.raycastOrigin.transform.forward * window.raycastLength;
//                        Handles.DrawLine(origin, origin + direction);
//                        Handles.SphereHandleCap(0, origin + direction, Quaternion.identity, 0.1f, EventType.Repaint);
//                    }
//                }
//            }
//        }

//    }

//    [System.Serializable]
//    private class VisualRaycast
//    {
//        public GameObject raycastOrigin;
//        public Color rayColor = Color.red;
//    }

//    [System.Serializable]
//    public class WeaponEditorData
//    {
//        public ModifyWeaponOption SelectedOption;
//        public bool IsAimed;
//        public Vector3 AimedWeaponPivotPosition;
//        public Vector3 AimedWeaponPivotRotation;
//        public Vector3 HipFireWeaponPivotPosition;
//        public Vector3 HipFireWeaponPivotRotation;
//        public bool EnableCrosshair;
//    }

//}








