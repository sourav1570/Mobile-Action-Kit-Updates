using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace MobileActionKit
{
    public class CreateRequiredUIForPickupWeapon : EditorWindow
    {
        public GameObject Player;
        public PlayerWeaponsManager PlayerWeaponsManagerScript;
        public Canvas Canvas2D;

        private const string PrefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Weapon & Scopes/";
        private readonly string[] PrefabNames = {"Weapon Pickup Information"};

        [MenuItem("Tools/Mobile Action Kit/Player/FireArms/Pickup Weapon/Create Pickup Weapon UI", priority = 9)]
        public static void ShowWindow()
        {
            GetWindow<CreateRequiredUIForPickupWeapon>("Setup Player Pickup Weapon UI");
        }

        private void OnGUI()
        {
            // Title
            GUILayout.Label("Setup Player Pickup Weapon UI", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Input Fields Section
            GUILayout.Label("UI Setup", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");


            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Player", GUILayout.Width(200)); // Fixed label width
            Player = (GameObject)EditorGUILayout.ObjectField(Player,typeof(GameObject),true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Player Weapons Manager Script", GUILayout.Width(200)); // Fixed label width
            PlayerWeaponsManagerScript = (PlayerWeaponsManager)EditorGUILayout.ObjectField(PlayerWeaponsManagerScript, typeof(PlayerWeaponsManager), true);
            EditorGUILayout.EndHorizontal();

            // Canvas 2D Field
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Canvas 2D", GUILayout.Width(200)); // Fixed label width
            Canvas2D = (Canvas)EditorGUILayout.ObjectField(
                Canvas2D,
                typeof(Canvas),
                true
            );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            GUILayout.Space(10);

            // Button Section
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Required UI & Scripts", GUILayout.Width(200), GUILayout.Height(30)))
            {
                if (Player == null || Canvas2D == null || PlayerWeaponsManagerScript == null)
                {
                    Debug.LogError("Please assign both Player and Canvas2D and PlayerWeaponsManagerComponent.");
                    return;
                }
                AddRequiredUI();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
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
            if (prefabName == "Weapon Pickup Information")
            {
                WeaponPickupManager C;
                if (Player.GetComponent<WeaponPickupManager>() == null)
                {
                    C = Player.AddComponent<WeaponPickupManager>();
                }
                else
                {
                    C = Player.GetComponent<WeaponPickupManager>();
                }
                C.PlayerWeaponsManagerScript = PlayerWeaponsManagerScript;
                C.DroppedWeaponPosition = Player.transform;
                C.WeaponInfo = uiInstance.transform.GetChild(0).gameObject;
                C.WeaponIcon = uiInstance.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
                C.CollectWeaponButton = uiInstance.transform.GetChild(0).transform.GetChild(3).GetComponent<Button>();
                C.CollectAmmoButton = uiInstance.transform.GetChild(0).transform.GetChild(2).GetComponent<Button>();
                C.AmmoText = uiInstance.transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                return; // No need to process buttons for Advanced Crosshair UI
            }
        }

    }
}
