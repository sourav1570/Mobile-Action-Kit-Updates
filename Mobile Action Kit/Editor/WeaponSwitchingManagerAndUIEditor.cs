using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace MobileActionKit
{
    public class WeaponSwitchingManagerAndUIEditor : EditorWindow
    {
        public GameObject RequiredScriptsRootGameObject;
        public Canvas Canvas2D;

        private GameObject WeaponManager;
        private GameObject SwitchingWeaponsUI;
        private const string PrefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Others/";
        private readonly string[] PrefabNames = { "Weapon Manager", "Switching Weapons UI" };

        [MenuItem("Tools/Mobile Action Kit/Player/FireArms/Setup Weapon Switching UI & Scripts", priority = 5)]
        public static void ShowWindow()
        {
            GetWindow<WeaponSwitchingManagerAndUIEditor>("Setup Weapon Switching UI & Scripts");
        }

        private void OnGUI()
        {
            // Set a dynamic label width based on the window size, making sure the label is always readable
            EditorGUIUtility.labelWidth = Mathf.Min(position.width * 0.4f, 250); // 40% of window width, capped at 250px

            // Title for the window
            GUILayout.Label("Setup Weapon Switching UI & Scripts", EditorStyles.boldLabel);

            // Required Scripts Section
            EditorGUILayout.LabelField("Required Scripts", EditorStyles.boldLabel);
            RequiredScriptsRootGameObject = (GameObject)EditorGUILayout.ObjectField("Required Scripts Root GameObject", RequiredScriptsRootGameObject, typeof(GameObject), true);
            Canvas2D = (Canvas)EditorGUILayout.ObjectField("Canvas 2D", Canvas2D, typeof(Canvas), true);

            // Button Section for Adding UI
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Required UI"))
            {
                if (RequiredScriptsRootGameObject == null || Canvas2D == null)
                {
                    Debug.LogError("Please assign both PlayerManagerScript and Canvas2D.");
                    return;
                }

                AddRequiredObjects();
            }

            // Reset label width back to default after drawing the GUI
            EditorGUIUtility.labelWidth = 0;
        }

        //private void OnGUI()
        //{
        //    GUILayout.Label("Setup Weapon Switching UI & Scripts", EditorStyles.boldLabel);

        //    RequiredScriptsRootGameObject = (GameObject)EditorGUILayout.ObjectField("Required Scripts Root GameObject", RequiredScriptsRootGameObject, typeof(GameObject), true);
        //    Canvas2D = (Canvas)EditorGUILayout.ObjectField("Canvas 2D", Canvas2D, typeof(Canvas), true);

        //    if (GUILayout.Button("Add Required UI"))
        //    {
        //        if (RequiredScriptsRootGameObject == null || Canvas2D == null)
        //        {
        //            Debug.LogError("Please assign both PlayerManagerScript and Canvas2D.");
        //            return;
        //        }

        //        AddRequiredObjects();
        //    }
        //}

        private void AddRequiredObjects()
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

                SetupUIAndObjects(instance, prefabName);

                if(WeaponManager != null && SwitchingWeaponsUI != null)
                {
                    WeaponManager.transform.GetChild(1).GetComponent<SwitchingPlayerWeapons>().SwipeArea = SwitchingWeaponsUI.transform.GetChild(0).gameObject;
                    WeaponManager.transform.GetChild(1).GetComponent<SwitchingPlayerWeapons>().SwipeLeftButton = SwitchingWeaponsUI.transform.GetChild(0).transform.GetChild(0).GetComponent<Button>();
                    WeaponManager.transform.GetChild(1).GetComponent<SwitchingPlayerWeapons>().SwipeRightButton = SwitchingWeaponsUI.transform.GetChild(0).transform.GetChild(1).GetComponent<Button>();
                }
              
            }
        }

        private void SetupUIAndObjects(GameObject uiInstance, string prefabName)
        {
            if (prefabName == "Weapon Manager")
            {
                WeaponManager = uiInstance;
                uiInstance.transform.SetParent(RequiredScriptsRootGameObject.transform);
                uiInstance.transform.position = Vector3.zero;
                return; // No need to process buttons for Advanced Crosshair UI
            }

            if (prefabName == "Switching Weapons UI")
            {
                SwitchingWeaponsUI = uiInstance;
                if (Canvas2D != null && SwitchingWeaponsUI != null)
                {
                    SwitchingWeaponsUI.transform.SetParent(Canvas2D.transform);
                    SwitchingWeaponsUI.transform.SetSiblingIndex(1); // Set the index to 1
                }
                return;

            }


        }

    }
}
