using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Reflection;
using System;
using System.Linq;
using TMPro;

namespace MobileActionKit
{
    [CustomEditor(typeof(CreateUpgradableItem))]
    public class CreateUpgradableItem_EditorScript : Editor
    {
        private CreateUpgradableItem CreateMultiItem_PurchasableItemHelper;
        CreateUpgradableItem script;

        private void OnEnable()
        {
            CreateMultiItem_PurchasableItemHelper = (CreateUpgradableItem)target;
        }
        private GUIStyle GetButtonStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.green;
            style.fontSize = 14; // Adjust the font size as needed
            style.fontStyle = FontStyle.Bold;

            return style;
        }

        public override void OnInspectorGUI()
        {
            script = (CreateUpgradableItem)target;
            DrawDefaultInspector();

            EditorGUILayout.Space();
            DisplayConsumableItemsList();

            DisplayItemList(CreateMultiItem_PurchasableItemHelper.CreateItem, "");

            if (GUILayout.Button("SAVE THE CHANGES", GetButtonStyle(), GUILayout.Width(450), GUILayout.Height(40)))
            {
                // Check if the prefab exists
                string prefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/ItemsFunctionSavingPrefabs/CreateUpgradableItem.prefab";
                GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                // Get information for PlayerPrefs key
                string sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
                string gameObjectName = script.gameObject.name;
                string prefabKey = $"{sceneName}_{gameObjectName}";

                if (existingPrefab != null && PlayerPrefs.HasKey(prefabKey))
                {
                    // If the prefab exists and the key exists, apply changes directly to the existing prefab
                    CreateUpgradableItem existingPrefabScript = existingPrefab.GetComponent<CreateUpgradableItem>();
                    if (existingPrefabScript != null)
                    {
                        EditorUtility.CopySerialized(script, existingPrefabScript);

                        // Mark the scene as dirty to ensure changes are saved
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                    }
                }
                else
                {
                    // If the prefab doesn't exist or the key doesn't exist, instantiate a new prefab from the original asset
                    GameObject prefabAsset = PrefabUtility.LoadPrefabContents(prefabPath);
                    if (prefabAsset != null)
                    {
                        // Apply changes to the instantiated prefab
                        CreateUpgradableItem newPrefabScript = prefabAsset.GetComponent<CreateUpgradableItem>();
                        if (newPrefabScript != null)
                        {
                            EditorUtility.CopySerialized(script, newPrefabScript);

                            // Save the instantiated prefab as a new prefab asset
                            PrefabUtility.SaveAsPrefabAsset(prefabAsset, prefabPath);

                            // Save information about the new prefab in PlayerPrefs
                            PlayerPrefs.SetString(prefabKey, prefabPath);
                            PlayerPrefs.Save();

                            // Unload the prefab contents
                            PrefabUtility.UnloadPrefabContents(prefabAsset);
                        }
                    }
                }
            }

        }

        private void DisplayItemList<T>(List<T> itemList, string itemType)
        {
            bool isListEmpty = itemList.Count == 0;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add " + itemType, GUILayout.ExpandWidth(true)))
            {
                AddItemToItemList(itemList);
            }
            if (!isListEmpty && GUILayout.Button("Remove", GUILayout.ExpandWidth(true)))
            {
                itemList.RemoveAt(itemList.Count - 1);
            }



            EditorGUILayout.EndHorizontal();
        }

        private void AddItemToItemList<T>(List<T> itemList)
        {
            // You can customize this method to add items of the appropriate type to your list.
            // For now, it adds default values to the list.
            itemList.Add(default(T));
        }
        private void DisplayConsumableItemsList()
        {
            GUIStyle itemLabelStyle = new GUIStyle(EditorStyles.boldLabel);
            itemLabelStyle.normal.textColor = Color.white; // Set text color to dark black

            for (int i = 0; i < CreateMultiItem_PurchasableItemHelper.CreateItem.Count; i++)
            {
                GUILayout.BeginVertical("Box");

                EditorGUILayout.LabelField("Item " + (i + 1), itemLabelStyle); // Apply the style

                CreateUpgradableItemClass item = CreateMultiItem_PurchasableItemHelper.CreateItem[i];

                if (item != null)
                {
                   // script.ItemName = EditorGUILayout.TextField("Item Name", script.ItemName);
                   // script.UIButton = (Button)EditorGUILayout.ObjectField("UI Button", script.UIButton, typeof(Button), true);
                   // script.UpgradeMessageText = (TextMeshProUGUI)EditorGUILayout.ObjectField("Upgrade Message Text", script.UpgradeMessageText, typeof(TextMeshProUGUI), true);

                    item.PriceTag = EditorGUILayout.IntField("Price", item.PriceTag);
                    item.UpgradeMessage = EditorGUILayout.TextField("Upgrade Message", item.UpgradeMessage);
                    item.AddScript = (MonoBehaviour)EditorGUILayout.ObjectField("Add Script", item.AddScript, typeof(MonoBehaviour), true);

                    if (item.AddScript != null)
                    {
                        item.FunctionToInvokeWhenPurchased = DisplayFunctionList(item.AddScript, item.FunctionToInvokeWhenPurchased, "Function To Invoke When Purchased");
                        item.FunctionToInvokeWhenPurchasedAndGameRestart = DisplayFunctionList(item.AddScript, item.FunctionToInvokeWhenPurchasedAndGameRestart, "Function To Invoke When Game Restart");
                    }
                }

                GUILayout.EndVertical();
            }
        }
        private string DisplayFunctionList(MonoBehaviour script, string selectedFunctionName, string FunctionName)
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

            selectedIndex = EditorGUILayout.Popup(FunctionName, selectedIndex, functionNames.ToArray());

            if (selectedIndex >= 0 && selectedIndex < functionNames.Count)
            {
                return functionNames[selectedIndex];
            }

            return null;
        }

        private List<string> GetPublicVoidMethodNames(MonoBehaviour script)
        {
            List<string> methodNames = new List<string>();

            Type type = script.GetType();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            // List of Unity-specific method names to filter out
            string[] unityMethodNames = {
        "Awake", "Start", "Update", "FixedUpdate", "LateUpdate",
        "OnEnable", "OnDisable", "OnDestroy", "OnValidate", /* Add more as needed */
    };

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
    }
}

