using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;
using MobileActionKit;

[CustomEditor(typeof(ItemUsageCheck))]
public class ItemUsageCheckEditor : Editor
{
    private string[] methodNames = new string[0];
    private int selectedMethodIndex = 0;
    private string extractedItemName = "";

    public override void OnInspectorGUI()
    {
        ItemUsageCheck script = (ItemUsageCheck)target;

        // Display ScriptInfo as a non-editable text area
        EditorGUILayout.LabelField("Script Info", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(script.ScriptInfo, MessageType.Info);

        // Drag and drop the target script
        script.targetScript = (MonoBehaviour)EditorGUILayout.ObjectField("Monobehaviour Script", script.targetScript, typeof(MonoBehaviour), true);

        // Extract public void methods if a script is assigned
        if (script.targetScript != null)
        {
            Type scriptType = script.targetScript.GetType();
            methodNames = scriptType
                .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly)
                .Where(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
                .Select(m => m.Name)
                .ToArray();

            if (methodNames.Length > 0)
            {
                selectedMethodIndex = EditorGUILayout.Popup("Select Method", selectedMethodIndex, methodNames);
                script.selectedMethod = methodNames[selectedMethodIndex];
            }
            else
            {
                EditorGUILayout.HelpBox("No public void methods found.", MessageType.Warning);
            }
        }

        // Drag and drop the ItemUsageController
        script.itemUsageController = (GameObject)EditorGUILayout.ObjectField("Item Usage Controller Script", script.itemUsageController, typeof(GameObject), true);

        // Extract ItemName before button click
        if (script.itemUsageController != null)
        {
            ItemUsageController controller = script.itemUsageController.GetComponent<ItemUsageController>();
            if (controller != null)
            {
                extractedItemName = controller.ItemName;
                //EditorGUILayout.LabelField("Extracted Item Name: ", extractedItemName);
            }
            else
            {
                EditorGUILayout.HelpBox("ItemUsageController component not found on the assigned GameObject.", MessageType.Error);
            }
        }

        // Button to add required code
        if (GUILayout.Button("Add Required Code To The Script"))
        {
            if (script.targetScript != null && !string.IsNullOrEmpty(script.selectedMethod) && !string.IsNullOrEmpty(extractedItemName))
            {
                AddCodeToScript(script.targetScript, script.selectedMethod, extractedItemName);
            }
            else
            {
                Debug.LogError("Please assign a target script, select a method, and set a valid Item Usage Controller.");
            }
        }

        // Button to remove added code
        if (GUILayout.Button("Remove Added Code From The Script"))
        {
            if (script.targetScript != null && !string.IsNullOrEmpty(script.selectedMethod) && !string.IsNullOrEmpty(extractedItemName))
            {
                RemoveCodeFromScript(script.targetScript, script.selectedMethod, extractedItemName);
            }
            else
            {
                Debug.LogError("Please assign a target script, select a method, and set a valid Item Usage Controller.");
            }
        }
    }

    private void AddCodeToScript(MonoBehaviour targetScript, string methodName, string itemName)
    {
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(targetScript));

        if (string.IsNullOrEmpty(scriptPath))
        {
            Debug.LogError("Could not find the script file.");
            return;
        }

        string[] lines = File.ReadAllLines(scriptPath);
        string codeToInject = $"        PlayerPrefs.SetInt(\"{itemName}IsInUse\", 1);";

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains($"void {methodName}()"))
            {
                int insertIndex = i + 1;
                while (insertIndex < lines.Length && lines[insertIndex].Trim() != "}")
                {
                    insertIndex++;
                }

                if (insertIndex < lines.Length)
                {
                    Array.Resize(ref lines, lines.Length + 1);
                    for (int j = lines.Length - 1; j > insertIndex; j--)
                    {
                        lines[j] = lines[j - 1];
                    }

                    lines[insertIndex] = codeToInject;
                    File.WriteAllLines(scriptPath, lines);
                    AssetDatabase.Refresh();

                    Debug.Log($"Code added to {methodName} in {targetScript.GetType().Name}");
                    return;
                }
            }
        }

        Debug.LogError("Could not find the function in the script.");
    }

    private void RemoveCodeFromScript(MonoBehaviour targetScript, string methodName, string itemName)
    {
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(targetScript));

        if (string.IsNullOrEmpty(scriptPath))
        {
            Debug.LogError("Could not find the script file.");
            return;
        }

        string[] lines = File.ReadAllLines(scriptPath);
        string codeToRemove = $"        PlayerPrefs.SetInt(\"{itemName}IsInUse\", 1);";
        bool removed = false;

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Trim() == codeToRemove.Trim())
            {
                lines[i] = "";
                removed = true;
            }
        }

        if (removed)
        {
            File.WriteAllLines(scriptPath, lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray());
            AssetDatabase.Refresh();
            Debug.Log($"Code removed from {methodName} in {targetScript.GetType().Name}");
        }
        else
        {
            Debug.LogWarning("No injected code found to remove.");
        }
    }
}
