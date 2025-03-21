using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace MobileActionKit
{
    public class CreateRequiredUIForMeleeSetup : EditorWindow
    {
        public MeleeAttack PlayerMeleeAttackScript;
        public Canvas Canvas2D;

        private const string PrefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Grenades & Melee/";
        private readonly string[] PrefabNames = { "Melee UI Button Big", "Melee UI Button Small" };

        [MenuItem("Tools/Mobile Action Kit/Player/Melee/Player Melee Attack UI Setup", priority = 7)]
        public static void ShowWindow()
        {
            GetWindow<CreateRequiredUIForMeleeSetup>("Setup Melee UI");
        }

        private void OnGUI()
        {
            // Title
            GUILayout.Label("Setup Player Melee UI", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Input Fields Section
            GUILayout.Label("UI Setup", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            // Grenade Thrower Script Field
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Player Melee Attack Script", GUILayout.Width(200)); // Fixed label width
            PlayerMeleeAttackScript = (MeleeAttack)EditorGUILayout.ObjectField(
                PlayerMeleeAttackScript,
                typeof(MeleeAttack),
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
                if (PlayerMeleeAttackScript == null || Canvas2D == null)
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
            if (prefabName == "Melee UI Button Big")
            {
                PlayerMeleeAttackScript.RequiredComponents.MeleeUIButtons.Add(uiInstance.transform.GetChild(1).GetComponent<Button>());


                return; // No need to process buttons for Advanced Crosshair UI
            }

            if (prefabName == "Melee UI Button Small")
            {
                PlayerMeleeAttackScript.RequiredComponents.MeleeUIButtons.Add(uiInstance.transform.GetChild(1).GetComponent<Button>());
                return;

            }
        }

    }
}
