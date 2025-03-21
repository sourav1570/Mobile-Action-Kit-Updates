using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;


namespace MobileActionKit
{
    [CustomEditor(typeof(AttachmentsActivator))]
    public class ConditionalActivatorEditor : Editor
    {
        private SerializedProperty inventoryItemsProperty;

        private void OnEnable()
        {
            inventoryItemsProperty = serializedObject.FindProperty("inventoryItems");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
       
            // Display each item in the inventoryItems list
            for (int i = 0; i < inventoryItemsProperty.arraySize; i++)
            {
                SerializedProperty itemProperty = inventoryItemsProperty.GetArrayElementAtIndex(i);
                //SerializedProperty IsThisItemActivatedCurrently = itemProperty.FindPropertyRelative("IsThisItemActivatedCurrently");
                SerializedProperty keyNameProp = itemProperty.FindPropertyRelative("keyName");
                SerializedProperty objectsToActivateProp = itemProperty.FindPropertyRelative("ObjectsToActivate");
                SerializedProperty targetScriptProp = itemProperty.FindPropertyRelative("AttachmentFunctionScript");
                SerializedProperty selectedFunctionNameProp = itemProperty.FindPropertyRelative("AttachmentFunction");

                EditorGUILayout.BeginVertical("box");

                // Display each field in InventoryItem
                //EditorGUILayout.PropertyField(IsThisItemActivatedCurrently);
                EditorGUILayout.PropertyField(keyNameProp);
                EditorGUILayout.PropertyField(objectsToActivateProp, true);
                EditorGUILayout.PropertyField(targetScriptProp);

                // Populate dropdown if targetScript is assigned
                if (targetScriptProp.objectReferenceValue != null)
                {
                    MonoBehaviour targetScript = (MonoBehaviour)targetScriptProp.objectReferenceValue;
                    List<string> availableMethods = new List<string>();

                    // Get only public, parameterless methods unique to the target script
                    MethodInfo[] methods = targetScript.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    foreach (MethodInfo method in methods)
                    {
                        if (method.GetParameters().Length == 0)
                        {
                            availableMethods.Add(method.Name);
                        }
                    }

                    // Dropdown with available functions
                    if (availableMethods.Count > 0)
                    {
                        int selectedIndex = availableMethods.IndexOf(selectedFunctionNameProp.stringValue);
                        selectedIndex = selectedIndex >= 0 ? selectedIndex : 0;

                        selectedIndex = EditorGUILayout.Popup("Attachment Function", selectedIndex, availableMethods.ToArray());
                        selectedFunctionNameProp.stringValue = availableMethods[selectedIndex];
                    }
                    else
                    {
                        EditorGUILayout.LabelField("No public parameterless functions found in target script.");
                        selectedFunctionNameProp.stringValue = string.Empty;
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Please assign a Target Script.", MessageType.Warning);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            // Add and Remove buttons outside of the list items
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            // "Add Inventory" button to add a new item
            if (GUILayout.Button("Add New Attachment"))
            {
                inventoryItemsProperty.arraySize++;
            }

            // "Remove Inventory" button to remove the last item
            if (GUILayout.Button("Remove Last Added Attachment") && inventoryItemsProperty.arraySize > 0)
            {
                inventoryItemsProperty.arraySize--;
            }

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}