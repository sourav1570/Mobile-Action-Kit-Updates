using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace MobileActionKit
{
    public class CreateRequiredWeaponUIEditorScript : EditorWindow
    {
        public PlayerManager PlayerManagerScript;
        public Canvas Canvas2D;

        private const string PrefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Others/";
        private readonly string[] PrefabNames = { "Fire UI", "Reload UI", "Aiming UI", "Advanced Crosshair UI", "Ammo Info"};

        [MenuItem("Tools/Mobile Action Kit/Player/FireArms/Setup Weapon UI", priority = 4)]
        public static void ShowWindow()
        {
            GetWindow<CreateRequiredWeaponUIEditorScript>("Setup Weapon UI");
        }

        private void OnGUI()
        {
            GUILayout.Label("Setup Weapon UI", EditorStyles.boldLabel);

            PlayerManagerScript = (PlayerManager)EditorGUILayout.ObjectField("Player Manager Script", PlayerManagerScript, typeof(PlayerManager), true);
            Canvas2D = (Canvas)EditorGUILayout.ObjectField("Canvas 2D", Canvas2D, typeof(Canvas), true);

            if (GUILayout.Button("Add Required UI"))
            {
                if (PlayerManagerScript == null || Canvas2D == null)
                {
                    Debug.LogError("Please assign both PlayerManagerScript and Canvas2D.");
                    return;
                }

                AddRequiredUI();
            }
        }

        private void AddRequiredUI()
        {
            foreach (var prefabName in PrefabNames)
            {
                string fullPath = PrefabPath + prefabName + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);

                if (prefab == null)
                {
                    Debug.LogError($"Prefab not found at {fullPath}");
                    continue;
                }

                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, Canvas2D.transform);
                PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                SetupUI(instance, prefabName);
            }
        }

        private void SetupUI(GameObject uiInstance, string prefabName)
        {
            Button[] buttons = uiInstance.GetComponentsInChildren<Button>(true);

            // Assign Crosshair to all PlayerWeapon scripts
            PlayerWeapon[] allPlayerWeaponScripts = FindObjectsOfType<PlayerWeapon>();

            if (prefabName == "Advanced Crosshair UI")
            {
                foreach (PlayerWeapon playerWeapon in allPlayerWeaponScripts)
                {
                    if (playerWeapon.ShootingFeatures != null)
                    {
                        playerWeapon.ShootingFeatures.WeaponCrosshair = uiInstance;
                    }
                }
                if(uiInstance.GetComponent<CrossHair>() != null)
                {
                    uiInstance.GetComponent<CrossHair>().CrouchScript = PlayerManagerScript.transform.root.GetComponent<Crouch>();

                }
              
                return; // No need to process buttons for Advanced Crosshair UI
            }

            if (prefabName == "Ammo Info")
            {
                foreach (PlayerWeapon playerWeapon in allPlayerWeaponScripts)
                {
                    if (playerWeapon.ShootingFeatures != null)
                    {
                        playerWeapon.Reload.TotalAmmoText = uiInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                        playerWeapon.Reload.ShotsInMagCountText = uiInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    }
                }
                return;

            }

            // Handle buttons for other UI prefabs
            foreach (var button in buttons)
            {
                if (prefabName == "Fire UI")
                {
                    PlayerManagerScript.FireButton = button;
                    foreach (PlayerWeapon playerWeapon in allPlayerWeaponScripts)
                    {
                        if (playerWeapon.ShootingFeatures != null)
                        {
                            playerWeapon.RequiredComponents.FireButton = button;
                        }
                    }
                }
                else if (prefabName == "Reload UI")
                {
                    PlayerManagerScript.ReloadButton = button;
                }
                else if (prefabName == "Aiming UI")
                {
                    PlayerManagerScript.AimingButton = button;
                }
            }
        }

    }
}
