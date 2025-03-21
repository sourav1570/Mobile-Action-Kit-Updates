using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(InputActionFunctionBinder))]
public class InputActionFunctionBinderEditor : Editor
{
    private InputActionFunctionBinder binder;
    private SerializedProperty savedFunctions;

    private void OnEnable()
    {
        binder = (InputActionFunctionBinder)target;
        savedFunctions = serializedObject.FindProperty("savedFunctions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Function Binder", EditorStyles.boldLabel);

        if (binder.inputActions != null)
        {
            if (GUILayout.Button("Populate Functions from InputActions"))
            {
                binder.AutoPopulateFunctions();
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(binder);
                Repaint();
            }
        }

        EditorGUILayout.Space();

        for (int i = 0; i < binder.savedFunctions.Count; i++)
        {
            var functionEntry = binder.savedFunctions[i];

            EditorGUILayout.LabelField("Function: " + functionEntry.functionName, EditorStyles.boldLabel);

            for (int j = 0; j < functionEntry.targetScripts.Count; j++)
            {
                EditorGUILayout.BeginVertical("box");

                // Script Selection
                functionEntry.targetScripts[j] = (MonoBehaviour)EditorGUILayout.ObjectField(
                    "Script", functionEntry.targetScripts[j], typeof(MonoBehaviour), true
                );

                if (functionEntry.targetScripts[j] != null)
                {
                    List<string> methodOptions = InputActionFunctionBinder.GetPublicVoidMethods(functionEntry.targetScripts[j]);

                    bool useperformedFunction = EditorGUILayout.Toggle("Use Performed Function", functionEntry.performedMethodNames[j] != "");

                    if (useperformedFunction)
                    {
                        // Ensure that previous selection persists
                        int selectedPerformedIndex = methodOptions.IndexOf(functionEntry.performedMethodNames[j]);
                        if (selectedPerformedIndex == -1) selectedPerformedIndex = 0; // Default to first method if none found

                        functionEntry.performedMethodNames[j] = methodOptions.Count > 0
                            ? methodOptions[EditorGUILayout.Popup("Performed Function", selectedPerformedIndex, methodOptions.ToArray())]
                            : "";
                    }
                    else
                    {
                        functionEntry.performedMethodNames[j] = ""; // Reset performed function if disabled
                    }

                  

                    // Ensure that previous selection persists
                    //int selectedCanceledIndex = methodOptions.IndexOf(functionEntry.canceledMethodNames[j]);
                    //if (selectedCanceledIndex == -1) selectedCanceledIndex = 0;

                    //functionEntry.canceledMethodNames[j] = methodOptions.Count > 0
                    //    ? methodOptions[EditorGUILayout.Popup("Canceled Function", selectedCanceledIndex, methodOptions.ToArray())]
                    //    : "";

                    // Checkbox to enable canceled function selection
                    bool useCanceledFunction = EditorGUILayout.Toggle("Use Canceled Function", functionEntry.canceledMethodNames[j] != "");

                    if (useCanceledFunction)
                    {
                        // Ensure that previous selection persists
                        int selectedCanceledIndex = methodOptions.IndexOf(functionEntry.canceledMethodNames[j]);
                        if (selectedCanceledIndex == -1) selectedCanceledIndex = 0;

                        functionEntry.canceledMethodNames[j] = methodOptions.Count > 0
                            ? methodOptions[EditorGUILayout.Popup("Canceled Function", selectedCanceledIndex, methodOptions.ToArray())]
                            : "";
                    }
                    else
                    {
                        functionEntry.canceledMethodNames[j] = ""; // Reset canceled function if disabled
                    }

                }
                else
                {
                    EditorGUILayout.LabelField("No valid script selected.");
                }

                // Remove button
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    Undo.RecordObject(binder, "Remove Script Entry");
                    functionEntry.targetScripts.RemoveAt(j);
                    functionEntry.performedMethodNames.RemoveAt(j);
                    functionEntry.canceledMethodNames.RemoveAt(j);
                    j--; // Adjust index after removal
                }

                EditorGUILayout.EndVertical();
            }

            // Add new script entry
            if (GUILayout.Button("Add Script"))
            {
                Undo.RecordObject(binder, "Add Script Entry");
                functionEntry.targetScripts.Add(null);
                functionEntry.performedMethodNames.Add("");
                functionEntry.canceledMethodNames.Add("");
            }
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Save"))
        {
            binder.Save();
            EditorUtility.SetDirty(binder);
            serializedObject.ApplyModifiedProperties();
            Repaint(); // Force UI refresh
        }

        serializedObject.ApplyModifiedProperties();
    }
}







//using UnityEditor;
//using UnityEngine;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;

//[CustomEditor(typeof(InputActionFunctionBinder))]
//public class InputActionFunctionBinderEditor : Editor
//{
//    private InputActionFunctionBinder binder;
//    private SerializedProperty savedFunctions;
//    private string scriptPath;

//    private void OnEnable()
//    {
//        binder = (InputActionFunctionBinder)target;
//        savedFunctions = serializedObject.FindProperty("savedFunctions");

//        // Locate the script file path
//        MonoScript monoScript = MonoScript.FromMonoBehaviour(binder);
//        scriptPath = AssetDatabase.GetAssetPath(monoScript);
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        DrawDefaultInspector();

//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Function Binder", EditorStyles.boldLabel);

//        if (binder.inputActions != null)
//        {
//            if (GUILayout.Button("Populate Functions from InputActions"))
//            {
//                binder.AutoPopulateFunctions();
//                serializedObject.ApplyModifiedProperties();
//            }
//        }

//        EditorGUILayout.Space();

//        for (int i = 0; i < binder.savedFunctions.Count; i++)
//        {
//            var functionEntry = binder.savedFunctions[i];


//            EditorGUILayout.LabelField("Function: " + functionEntry.functionName, EditorStyles.boldLabel);

//            for (int j = 0; j < functionEntry.targetScripts.Count; j++)
//            {


//                EditorGUILayout.BeginHorizontal();
//                EditorGUILayout.LabelField("Script", GUILayout.Width(60)); // Label width
//                functionEntry.targetScripts[j] = (MonoBehaviour)EditorGUILayout.ObjectField(
//                    functionEntry.targetScripts[j], typeof(MonoBehaviour), true, GUILayout.Width(350) // Expands properly
//                );
//                EditorGUILayout.EndHorizontal();


//                List<string> methodOptions = InputActionFunctionBinder.GetPublicVoidMethods(functionEntry.targetScripts[j]);
//                int selectedMethodIndex = Mathf.Max(0, methodOptions.IndexOf(functionEntry.methodNames[j]));




//                EditorGUILayout.BeginHorizontal();
//                if (methodOptions.Count > 0)
//                {
//                    EditorGUILayout.LabelField("Function:", GUILayout.Width(60));
//                    selectedMethodIndex = EditorGUILayout.Popup(selectedMethodIndex, methodOptions.ToArray(), GUILayout.Width(350));
//                    functionEntry.methodNames[j] = methodOptions[selectedMethodIndex];
//                }
//                else
//                {
//                    EditorGUILayout.LabelField("No valid functions found");
//                }

//                if (GUILayout.Button("X", GUILayout.Width(20)))
//                {
//                    Undo.RecordObject(binder, "Remove Script Entry");
//                    binder.RemoveScriptFromFunction(functionEntry.functionName, j);
//                }

//                EditorGUILayout.EndHorizontal();




//            }

//            if (GUILayout.Button("Add Script"))
//            {
//                Undo.RecordObject(binder, "Add Script Entry");
//                functionEntry.targetScripts.Add(null);
//                functionEntry.methodNames.Add("");
//            }


//        }

//        EditorGUILayout.Space();

//        if (GUILayout.Button("Save"))
//        {
//            InjectFunctionsIntoScript();
//        }

//        if (GUILayout.Button("Remove All Injected Code"))
//        {
//            RemoveInjectedFunctions();
//        }

//        serializedObject.ApplyModifiedProperties();
//    }

//    private void InjectFunctionsIntoScript()
//    {
//        if (binder == null || string.IsNullOrEmpty(scriptPath)) return;

//        List<string> existingMethods = GetExistingMethods();
//        StringBuilder newMethods = new StringBuilder();

//        foreach (var functionEntry in binder.savedFunctions)
//        {
//            string functionName = functionEntry.functionName; // Already has "On" prefix

//            if (!existingMethods.Contains(functionName))
//            {
//                newMethods.AppendLine($"\tpublic void {functionName}() => InvokeFunction(\"{functionName}\");");
//            }
//        }

//        if (newMethods.Length > 0)
//        {
//            AppendToScript(newMethods.ToString());
//        }
//    }

//    private List<string> GetExistingMethods()
//    {
//        List<string> methods = new List<string>();
//        if (!File.Exists(scriptPath)) return methods;

//        string[] lines = File.ReadAllLines(scriptPath);
//        foreach (string line in lines)
//        {
//            if (line.Trim().StartsWith("public void On"))
//            {
//                string methodName = line.Split('(')[0].Replace("public void ", "").Trim();
//                methods.Add(methodName);
//            }
//        }
//        return methods;
//    }

//    private void AppendToScript(string newMethods)
//    {
//        if (!File.Exists(scriptPath)) return;

//        string[] lines = File.ReadAllLines(scriptPath);
//        List<string> modifiedLines = new List<string>();

//        bool inserted = false;
//        int classEndIndex = -1;

//        for (int i = 0; i < lines.Length; i++)
//        {
//            string line = lines[i];

//            if (line.Contains("}"))
//            {
//                classEndIndex = i;
//            }

//            if (!inserted && classEndIndex > 0 && i == classEndIndex + 1)
//            {
//                modifiedLines.Add("");
//                modifiedLines.Add(newMethods.Trim());
//                inserted = true;
//            }

//            modifiedLines.Add(line);
//        }

//        File.WriteAllLines(scriptPath, modifiedLines.ToArray());

//        AssetDatabase.Refresh();
//        EditorUtility.SetDirty(target);
//        serializedObject.ApplyModifiedProperties();
//    }

//    private void RemoveInjectedFunctions()
//    {
//        if (!File.Exists(scriptPath)) return;

//        string[] lines = File.ReadAllLines(scriptPath);
//        List<string> modifiedLines = new List<string>();

//        foreach (string line in lines)
//        {
//            if (!line.Trim().StartsWith("public void On"))
//            {
//                modifiedLines.Add(line);
//            }
//        }

//        File.WriteAllLines(scriptPath, modifiedLines.ToArray());

//        AssetDatabase.Refresh();
//        EditorUtility.SetDirty(target);
//        serializedObject.ApplyModifiedProperties();
//    }
//}

