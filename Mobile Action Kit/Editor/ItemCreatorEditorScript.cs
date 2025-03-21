using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using MobileActionKit;

namespace MobileMonetizationPro
{
    [CustomEditor(typeof(ItemUsageController))]
    public class ItemCreatorEditorScript : Editor
    {
        private ItemUsageController script;

        private void OnEnable()
        {
            script = (ItemUsageController)target;
        }

        private GUIStyle GetButtonStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            style.normal.textColor = Color.green;
            return style;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            DisplayInGameConsumableItemsList();

            if (GUI.changed) // Ensure changes are tracked
            {
                Undo.RecordObject(script, "Modify ItemUsageController");
                EditorUtility.SetDirty(script);
            }

            if (GUILayout.Button("SAVE THE CHANGES", GetButtonStyle(), GUILayout.Width(450), GUILayout.Height(40)))
            {
                SaveChanges();
            }
        }

        private void DisplayInGameConsumableItemsList()
        {
            Undo.RecordObject(script, "Modify ItemUsageController"); // Track modifications

            script.ItemName = EditorGUILayout.TextField("Item Name", script.ItemName);
            script.GameobjectsDeactivationDelay = EditorGUILayout.FloatField("Gameobjects Deactivation Delay", script.GameobjectsDeactivationDelay);
            script.DefaultItems = EditorGUILayout.IntField("Default Items", script.DefaultItems);
            script.CanMaximumCarry = EditorGUILayout.IntField("Can Maximum Carry", script.CanMaximumCarry);
            script.TimeToEnableButtons = EditorGUILayout.FloatField("Time To ReEnable Buttons And GameObjects", script.TimeToEnableButtons);
            script.AddScript = EditorGUILayout.ObjectField("Add Script", script.AddScript, typeof(MonoBehaviour), true) as MonoBehaviour;

            if (script.AddScript != null)
            {
                script.FunctionToInvokeWhenUsing = DisplayFunctionList(script.AddScript, script.FunctionToInvokeWhenUsing, "Function To Invoke When Using");
                script.FunctionToInvokeWhenGameRestart = DisplayFunctionList(script.AddScript, script.FunctionToInvokeWhenGameRestart, "Function To Invoke When Game Restart");
            }
        }

        private string DisplayFunctionList(MonoBehaviour script, string selectedFunctionName, string functionName)
        {
            List<string> functionNames = GetPublicVoidMethodNames(script);
            if (functionNames.Count == 0)
            {
                EditorGUILayout.HelpBox("No public void methods found in the selected script.", MessageType.Warning);
                return null;
            }

            int selectedIndex = functionNames.IndexOf(selectedFunctionName);
            if (selectedIndex == -1)
            {
                selectedIndex = 0; // Default to the first method if not found
            }

            selectedIndex = EditorGUILayout.Popup(functionName, selectedIndex, functionNames.ToArray());

            return functionNames[selectedIndex];
        }

        private List<string> GetPublicVoidMethodNames(MonoBehaviour script)
        {
            List<string> methodNames = new List<string>();
            Type type = script.GetType();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            string[] unityMethodNames = { "Awake", "Start", "Update", "FixedUpdate", "LateUpdate", "OnEnable", "OnDisable", "OnDestroy", "OnValidate" };

            foreach (MethodInfo method in methods)
            {
                if (method.DeclaringType == type && method.ReturnType == typeof(void) &&
                    method.GetParameters().Length == 0 && !unityMethodNames.Contains(method.Name))
                {
                    methodNames.Add(method.Name);
                }
            }

            return methodNames;
        }

        private void SaveChanges()
        {
            Undo.RecordObject(script, "Modify ItemUsageController");
            EditorUtility.SetDirty(script);

            // Ensure the scene recognizes the changes
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log("Changes saved successfully.");
        }
    }
}




//using UnityEngine;
//using UnityEditor;
//using System.Collections.Generic;
//using UnityEngine.UI;
//using System.Reflection;
//using System;
//using System.Linq;
//using TMPro;
//using MobileActionKit;

//namespace MobileMonetizationPro
//{
//    [CustomEditor(typeof(ItemUsageController))]
//    public class ItemCreatorEditorScript : Editor
//    {
//        private ItemUsageController script;

//        private void OnEnable()
//        {          
//            script = (ItemUsageController)target;
//        }

//        private GUIStyle GetButtonStyle()
//        {
//            GUIStyle style = new GUIStyle(GUI.skin.button);
//            style.normal.textColor = Color.green;
//            style.fontSize = 14; // Adjust the font size as needed
//            style.fontStyle = FontStyle.Bold;

//            return style;
//        }

//        public override void OnInspectorGUI()
//        {
//            DrawDefaultInspector();

//            EditorGUILayout.Space();
//            DisplayInGameConsumableItemsList();

//            if (GUILayout.Button("SAVE THE CHANGES", GetButtonStyle(), GUILayout.Width(450), GUILayout.Height(40)))
//            {
//                SaveChanges();
//            }
//        }

//        private void DisplayInGameConsumableItemsList()
//        {
//            script.ItemName = EditorGUILayout.TextField("Item Name", script.ItemName);

//            script.GameobjectsDeactivationDelay = EditorGUILayout.FloatField("Gameobjects Deactivation Delay", script.GameobjectsDeactivationDelay);

//            script.DefaultItems = EditorGUILayout.IntField("Default Items", script.DefaultItems);

//            script.CanMaximumCarry = EditorGUILayout.IntField("Can Maximum Carry", script.CanMaximumCarry);

//            script.TimeToEnableButtons = EditorGUILayout.FloatField("Time To ReEnable Buttons And GameObjects", script.TimeToEnableButtons);

//            script.AddScript = EditorGUILayout.ObjectField("Add Script", script.AddScript, typeof(MonoBehaviour), true) as MonoBehaviour;

//            if (script.AddScript != null)
//            {
//                script.FunctionToInvokeWhenUsing = DisplayFunctionList(script.AddScript, script.FunctionToInvokeWhenUsing, "Function To Invoke When Using");
//                script.FunctionToInvokeWhenGameRestart = DisplayFunctionList(script.AddScript, script.FunctionToInvokeWhenGameRestart, "Function To Invoke When Game Restart");
//            }
//        }

//        private string DisplayFunctionList(MonoBehaviour script, string selectedFunctionName, string FunctionName)
//        {
//            List<string> functionNames = GetPublicVoidMethodNames(script);
//            if (functionNames.Count == 0)
//            {
//                EditorGUILayout.HelpBox("No public void methods found in the selected script.", MessageType.Warning);
//                return null;
//            }

//            int selectedIndex = functionNames.IndexOf(selectedFunctionName);
//            if (selectedIndex == -1)
//            {
//                selectedIndex = 0; // Default to the first method if not found
//            }

//            selectedIndex = EditorGUILayout.Popup(FunctionName, selectedIndex, functionNames.ToArray());

//            if (selectedIndex >= 0 && selectedIndex < functionNames.Count)
//            {
//                return functionNames[selectedIndex];
//            }

//            return null;
//        }

//        private List<string> GetPublicVoidMethodNames(MonoBehaviour script)
//        {
//            List<string> methodNames = new List<string>();

//            Type type = script.GetType();
//            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);

//            // List of Unity-specific method names to filter out
//            string[] unityMethodNames = {
//                "Awake", "Start", "Update", "FixedUpdate", "LateUpdate",
//                "OnEnable", "OnDisable", "OnDestroy", "OnValidate", /* Add more as needed */
//            };

//            foreach (MethodInfo method in methods)
//            {
//                if (method.DeclaringType == type && method.ReturnType == typeof(void) &&
//                    method.GetParameters().Length == 0 && !unityMethodNames.Contains(method.Name))
//                {
//                    methodNames.Add(method.Name);
//                }
//            }

//            return methodNames;
//        }

//        private void SaveChanges()
//        {
//            string prefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/ItemsFunctionSavingPrefabs/ItemUsageController.prefab";
//            GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

//            string sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
//            string gameObjectName = script.gameObject.name;
//            string prefabKey = $"{sceneName}_{gameObjectName}";

//            if (existingPrefab != null && PlayerPrefs.HasKey(prefabKey))
//            {
//                CreateUpgradableItem existingPrefabScript = existingPrefab.GetComponent<CreateUpgradableItem>();
//                if (existingPrefabScript != null)
//                {
//                    EditorUtility.CopySerialized(script, existingPrefabScript);
//                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
//                }
//            }
//            else
//            {
//                GameObject prefabAsset = PrefabUtility.LoadPrefabContents(prefabPath);
//                if (prefabAsset != null)
//                {
//                    CreateUpgradableItem newPrefabScript = prefabAsset.GetComponent<CreateUpgradableItem>();
//                    if (newPrefabScript != null)
//                    {
//                        EditorUtility.CopySerialized(script, newPrefabScript);
//                        PrefabUtility.SaveAsPrefabAsset(prefabAsset, prefabPath);
//                        PlayerPrefs.SetString(prefabKey, prefabPath);
//                        PlayerPrefs.Save();
//                        PrefabUtility.UnloadPrefabContents(prefabAsset);
//                    }
//                }
//            }
//        }
//    }
//}
