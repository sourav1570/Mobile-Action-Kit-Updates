using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace MobileActionKit
{
    public class CreateRequiredUIForGrenadeSetup : EditorWindow
    {
        public PlayerGrenadeThrower PlayerGrenadeThrowerScript;
        public Canvas Canvas2D;

        private const string PrefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Grenades & Melee/";
        private readonly string[] PrefabNames = { "Grenade Hands Selected UI", "Grenade Quick Throw"};

        [MenuItem("Tools/Mobile Action Kit/Player/Grenade/Player Grenade Throw UI Setup", priority = 7)]
        public static void ShowWindow()
        {
            GetWindow<CreateRequiredUIForGrenadeSetup>("Setup Player Grenade UI");
        }

        private void OnGUI()
        {
            // Title
            GUILayout.Label("Setup Player Grenade UI", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Input Fields Section
            GUILayout.Label("UI Setup", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            // Grenade Thrower Script Field
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Player Grenade Thrower Script", GUILayout.Width(200)); // Fixed label width
            PlayerGrenadeThrowerScript = (PlayerGrenadeThrower)EditorGUILayout.ObjectField(
                PlayerGrenadeThrowerScript,
                typeof(PlayerGrenadeThrower),
                true
            );
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
            if (GUILayout.Button("Add Required UI", GUILayout.Width(200), GUILayout.Height(30)))
            {
                if (PlayerGrenadeThrowerScript == null || Canvas2D == null)
                {
                    Debug.LogError("Please assign both PlayerGrenadeThrowerScript and Canvas2D.");
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
            if (prefabName == "Grenade Hands Selected UI")
            {
                PlayerGrenadeThrowerScript.RequiredComponents.GrenadeSelectOrThrowUIButtons.Add(uiInstance.transform.GetChild(1).GetComponent<Button>());
                 

                return; // No need to process buttons for Advanced Crosshair UI
            }

            if (prefabName == "Grenade Quick Throw")
            {
                PlayerGrenadeThrowerScript.RequiredComponents.GrenadeSelectOrThrowUIButtons.Add(uiInstance.transform.GetChild(1).GetComponent<Button>());
                return;

            }
        }

    }
}
