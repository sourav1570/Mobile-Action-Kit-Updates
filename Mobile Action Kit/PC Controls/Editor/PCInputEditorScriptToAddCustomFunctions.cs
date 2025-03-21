using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace MobileActionKit
{
    [CustomEditor(typeof(PcInputManager))]
    public class PCInputEditorScriptToAddCustomFunctions : Editor
    {
        public override void OnInspectorGUI()
        {
            PcInputManager script = (PcInputManager)target;

            // Display Script Info
            EditorGUILayout.LabelField("Script Information", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(script.ScriptInfo, MessageType.Info);

            EditorGUILayout.Space();
            DrawDefaultInspectorFields(script);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Note: Sprinting Cancels Other Key Actions", EditorStyles.boldLabel);

            // Single Keycode Inputs Section
            EditorGUILayout.LabelField("Single Keycode Inputs", EditorStyles.boldLabel);
            for (int i = 0; i < script.SingleKeycodeInputs.Count; i++)
            {
                var key = script.SingleKeycodeInputs[i];
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField($"Key Input {i + 1}", EditorStyles.boldLabel);

                key.Control = (PcInputManager.Options)EditorGUILayout.EnumPopup("Control", key.Control);
                key.KeyCode = (KeyCode)EditorGUILayout.EnumPopup("KeyCode", key.KeyCode);

                key.CallFunctionContinously = EditorGUILayout.Toggle("Call Function Continuously", key.CallFunctionContinously);

                if (key.AddScripts == null)
                    key.AddScripts = new List<MonoBehaviour>();
                if (key.FunctionsToInvoke == null)
                    key.FunctionsToInvoke = new List<string>();

                // Allow adding/removing multiple scripts
                for (int j = 0; j < key.AddScripts.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    key.AddScripts[j] = (MonoBehaviour)EditorGUILayout.ObjectField($"Script {j + 1}", key.AddScripts[j], typeof(MonoBehaviour), true);

                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        key.AddScripts.RemoveAt(j);
                        key.FunctionsToInvoke.RemoveAt(j);
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();

                    // Extract and display method list for each script
                    if (key.AddScripts[j] != null)
                    {
                        Type scriptType = key.AddScripts[j].GetType();

                        // Fetch methods that return either void or float and have no parameters
                        string[] methodNames = scriptType
                            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                            .Where(m => (m.ReturnType == typeof(void) || m.ReturnType == typeof(float)) && m.GetParameters().Length == 0)
                            .Select(m => m.Name)
                            .ToArray();

                        if (methodNames.Length > 0)
                        {
                            int selectedIndex = Array.IndexOf(methodNames, key.FunctionsToInvoke[j]);
                            if (selectedIndex < 0) selectedIndex = 0;

                            selectedIndex = EditorGUILayout.Popup("Select Method", selectedIndex, methodNames);
                            key.FunctionsToInvoke[j] = methodNames[selectedIndex];
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("No public void or float methods found in this script.", MessageType.Warning);
                        }
                    }

                }

                if (GUILayout.Button("Add New Action"))
                {
                    key.AddScripts.Add(null);
                    key.FunctionsToInvoke.Add("");
                }

                // Allow adding/removing multiple scripts
                for (int j = 0; j < key.AddScriptsForReset.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    key.AddScriptsForReset[j] = (MonoBehaviour)EditorGUILayout.ObjectField($"Script {j + 1}", key.AddScriptsForReset[j], typeof(MonoBehaviour), true);

                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        key.AddScriptsForReset.RemoveAt(j);
                        key.FunctionsToInvokeForReset.RemoveAt(j);
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();

                    // Extract and display method list for each script
                    if (key.AddScriptsForReset[j] != null)
                    {
                        Type scriptType = key.AddScriptsForReset[j].GetType();

                        // Fetch methods that return either void or float and have no parameters
                        string[] methodNames = scriptType
                            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                            .Where(m => (m.ReturnType == typeof(void) || m.ReturnType == typeof(float)) && m.GetParameters().Length == 0)
                            .Select(m => m.Name)
                            .ToArray();

                        if (methodNames.Length > 0)
                        {
                            int selectedIndex = Array.IndexOf(methodNames, key.FunctionsToInvokeForReset[j]);
                            if (selectedIndex < 0) selectedIndex = 0;

                            selectedIndex = EditorGUILayout.Popup("Select Method", selectedIndex, methodNames);
                            key.FunctionsToInvokeForReset[j] = methodNames[selectedIndex];
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("No public void or float methods found in this script.", MessageType.Warning);
                        }
                    }

                }

                if (GUILayout.Button("Add Reset Action"))
                {
                    key.AddScriptsForReset.Add(null);
                    key.FunctionsToInvokeForReset.Add("");
                }


                EditorGUILayout.EndVertical();
            }

            // Buttons for Adding/Removing Key Inputs
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add New Key Input"))
            {
                script.SingleKeycodeInputs.Add(new PcInputManager.PcKeys());
            }
            if (script.SingleKeycodeInputs.Count > 0 && GUILayout.Button("Remove Last Key Input"))
            {
                script.SingleKeycodeInputs.RemoveAt(script.SingleKeycodeInputs.Count - 1);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Shooting Weapons Switching Section
            EditorGUILayout.LabelField("Player Shooting Weapons Switching", EditorStyles.boldLabel);

            // Display Script Info
            EditorGUILayout.LabelField("Switching Weapons Info", EditorStyles.boldLabel);

            string Message = "Weapon activation is based on the active weapon list index in the 'PlayerWeaponsManager' Script. " +
                "For example, if the first weapon in the active weapon list is the AK-47, then pressing keycode for 'Weapon 1' as specified by the keycode below will activate the AK-47, " +
                "If a weapon slot is empty, no weapon will be activated.";

            EditorGUILayout.HelpBox(Message, MessageType.Info);

            EditorGUILayout.Space();

            for (int i = 0; i < script.ShootingWeaponsSwitching.Count; i++)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField($"Weapon {i + 1}", EditorStyles.boldLabel);

               // script.ShootingWeaponsSwitching[i].Slot = EditorGUILayout.TextField("Slot", script.ShootingWeaponsSwitching[i].Slot);
                script.ShootingWeaponsSwitching[i].KeyCode = (KeyCode)EditorGUILayout.EnumPopup("KeyCode", script.ShootingWeaponsSwitching[i].KeyCode);

                EditorGUILayout.EndVertical();
            }

            // Buttons for Adding/Removing Shooting Weapons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Weapon Binding"))
            {
                script.ShootingWeaponsSwitching.Add(new PcInputManager.SwitchWeaponNumericClass());
            }
            if (script.ShootingWeaponsSwitching.Count > 0 && GUILayout.Button("Unbind Last Weapon"))
            {
                script.ShootingWeaponsSwitching.RemoveAt(script.ShootingWeaponsSwitching.Count - 1);
            }
            EditorGUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawDefaultInspectorFields(PcInputManager script)
        {
            SerializedObject serializedObject = new SerializedObject(script);
            SerializedProperty property = serializedObject.GetIterator();

            property.NextVisible(true);
            while (property.NextVisible(false))
            {
                if (property.name != "SingleKeycodeInputs" && property.name != "ShootingWeaponsSwitching")
                    EditorGUILayout.PropertyField(property, true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}






//using UnityEngine;
//using UnityEditor;
//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Reflection;

//namespace MobileActionKit
//{
//    [CustomEditor(typeof(PcInput_KeycodeControl))]
//    public class PCInputEditorScriptToAddCustomFunctions : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            PcInput_KeycodeControl script = (PcInput_KeycodeControl)target;

//            // Display Script Info
//            EditorGUILayout.LabelField("Script Information", EditorStyles.boldLabel);
//            EditorGUILayout.HelpBox(script.ScriptInfo, MessageType.Info);

//            EditorGUILayout.Space();
//            DrawDefaultInspectorFields(script);
//            EditorGUILayout.Space();

//            // Single Keycode Inputs
//            EditorGUILayout.LabelField("Single Keycode Inputs", EditorStyles.boldLabel);
//            for (int i = 0; i < script.SingleKeycodeInputs.Count; i++)
//            {
//                var key = script.SingleKeycodeInputs[i];
//                EditorGUILayout.BeginVertical(GUI.skin.box);
//                EditorGUILayout.LabelField($"Key Input {i + 1}", EditorStyles.boldLabel);

//                key.Control = (PcInput_KeycodeControl.Options)EditorGUILayout.EnumPopup("Control", key.Control);
//                key.KeyCode = (KeyCode)EditorGUILayout.EnumPopup("KeyCode", key.KeyCode);

//                if (key.AddScripts == null)
//                    key.AddScripts = new List<MonoBehaviour>();
//                if (key.FunctionsToInvoke == null)
//                    key.FunctionsToInvoke = new List<string>();

//                // Allow adding/removing multiple scripts
//                for (int j = 0; j < key.AddScripts.Count; j++)
//                {
//                    EditorGUILayout.BeginHorizontal();
//                    key.AddScripts[j] = (MonoBehaviour)EditorGUILayout.ObjectField($"Script {j + 1}", key.AddScripts[j], typeof(MonoBehaviour), true);

//                    if (GUILayout.Button("-", GUILayout.Width(20)))
//                    {
//                        key.AddScripts.RemoveAt(j);
//                        key.FunctionsToInvoke.RemoveAt(j);
//                        continue;
//                    }
//                    EditorGUILayout.EndHorizontal();

//                    // Extract and display method list for each script
//                    if (key.AddScripts[j] != null)
//                    {
//                        Type scriptType = key.AddScripts[j].GetType();
//                        string[] methodNames = scriptType
//                            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
//                            .Where(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
//                            .Select(m => m.Name)
//                            .ToArray();

//                        if (methodNames.Length > 0)
//                        {
//                            int selectedIndex = Array.IndexOf(methodNames, key.FunctionsToInvoke[j]);
//                            if (selectedIndex < 0) selectedIndex = 0;

//                            selectedIndex = EditorGUILayout.Popup("Select Method", selectedIndex, methodNames);
//                            key.FunctionsToInvoke[j] = methodNames[selectedIndex];
//                        }
//                        else
//                        {
//                            EditorGUILayout.HelpBox("No public void methods found in this script.", MessageType.Warning);
//                        }
//                    }
//                }

//                if (GUILayout.Button("Add Script"))
//                {
//                    key.AddScripts.Add(null);
//                    key.FunctionsToInvoke.Add("");
//                }

//                if (GUILayout.Button("Remove Key Input"))
//                {
//                    script.SingleKeycodeInputs.RemoveAt(i);
//                    continue;
//                }

//                EditorGUILayout.EndVertical();
//            }

//            if (GUILayout.Button("Add New Key Input"))
//            {
//                script.SingleKeycodeInputs.Add(new PcInput_KeycodeControl.PcKeys());
//            }

//            EditorGUILayout.Space();

//            // Shooting Weapons Switching
//            EditorGUILayout.LabelField("Shooting Weapons Switching", EditorStyles.boldLabel);
//            for (int i = 0; i < script.ShootingWeaponsSwitching.Count; i++)
//            {
//                EditorGUILayout.BeginHorizontal();
//                script.ShootingWeaponsSwitching[i].WeaponName = EditorGUILayout.TextField("Weapon Name", script.ShootingWeaponsSwitching[i].WeaponName);
//                script.ShootingWeaponsSwitching[i].KeyCode = (KeyCode)EditorGUILayout.EnumPopup("KeyCode", script.ShootingWeaponsSwitching[i].KeyCode);

//                if (GUILayout.Button("-", GUILayout.Width(20)))
//                {
//                    script.ShootingWeaponsSwitching.RemoveAt(i);
//                    continue;
//                }
//                EditorGUILayout.EndHorizontal();
//            }

//            if (GUILayout.Button("Add Shooting Weapon Key"))
//            {
//                script.ShootingWeaponsSwitching.Add(new PcInput_KeycodeControl.SwitchWeaponNumericClass());
//            }

//            if (GUI.changed)
//            {
//                EditorUtility.SetDirty(target);
//            }
//        }

//        private void DrawDefaultInspectorFields(PcInput_KeycodeControl script)
//        {
//            SerializedObject serializedObject = new SerializedObject(script);
//            SerializedProperty property = serializedObject.GetIterator();

//            property.NextVisible(true);
//            while (property.NextVisible(false))
//            {
//                if (property.name != "SingleKeycodeInputs" && property.name != "ShootingWeaponsSwitching")
//                    EditorGUILayout.PropertyField(property, true);
//            }

//            serializedObject.ApplyModifiedProperties();
//        }
//    }
//}