using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public class InputActionFunctionBinder : MonoBehaviour
{
    public static InputActionFunctionBinder instance;

    public bool DebugLogs = false;

    [System.Serializable]
    public class FunctionEntry
    {
        public string functionName;
        public List<MonoBehaviour> targetScripts = new List<MonoBehaviour>();
        public List<string> performedMethodNames = new List<string>();
        public List<string> canceledMethodNames = new List<string>();
    }

    [HideInInspector]
    public List<FunctionEntry> savedFunctions = new List<FunctionEntry>();
    public InputActionAsset inputActions;
    private bool isActive = true;

    private void Awake()
    {
        if (instance == null) instance = this;
        BindInputActions(); // Ensure input actions are set up at runtime
    }
    public void AutoPopulateFunctions()
    {
        if (inputActions == null) return;

        // Preserve existing selections instead of clearing everything
        Dictionary<string, FunctionEntry> existingEntries = new Dictionary<string, FunctionEntry>();
        foreach (var entry in savedFunctions)
        {
            existingEntries[entry.functionName] = entry;
          
        }

        savedFunctions.Clear();

        foreach (var map in inputActions.actionMaps)
        {
            foreach (var action in map.actions)
            {
                string functionName = "On" + action.name;

                FunctionEntry entry;
                if (existingEntries.TryGetValue(functionName, out entry))
                {
                    savedFunctions.Add(entry); // Preserve user selections
                    
                }
                else
                {
                    entry = new FunctionEntry { functionName = functionName };
                    savedFunctions.Add(entry);
                }

                
                action.performed += ctx => InvokeFunction(functionName, false, action, ctx);
                action.canceled += ctx => InvokeFunction(functionName, true, action, ctx);
            }
        }
        if (DebugLogs == true)
        {
            Debug.Log("Functions auto-populated.");
        }
    }
    private void OnDisable()
    {
        isActive = false; // Stop function execution
    }

    private void OnEnable()
    {
        isActive = true; // Allow function execution again

        // Ensure the InputActionAsset remains correct
        if (inputActions != null && GetComponent<PlayerInput>() != null)
        {
            GetComponent<PlayerInput>().actions = inputActions;
        }
    }

    private void BindInputActions()
    {
        if (inputActions == null) return;

        foreach (var map in inputActions.actionMaps)
        {
            foreach (var action in map.actions)
            {
                string functionName = "On" + action.name;
           
                action.performed += ctx => InvokeFunction(functionName, false, action, ctx);
                action.canceled += ctx => InvokeFunction(functionName, true, action, ctx);
            }
        }
    }

    public void Save()
    {
        if(DebugLogs == true)
        {
            Debug.Log("Functions saved.");
        }
       
    }
    private void InvokeFunction(string functionName, bool isCanceled, InputAction action, InputAction.CallbackContext ctx)
    {
        if (!isActive) return; // Prevent function execution when disabled

        foreach (var functionEntry in savedFunctions)
        {
            if (functionEntry.functionName == functionName)
            {
                for (int i = 0; i < functionEntry.targetScripts.Count; i++)
                {
                    if (functionEntry.targetScripts[i] != null)
                    {
                        string methodName = isCanceled ? functionEntry.canceledMethodNames[i] : functionEntry.performedMethodNames[i];
                        if (!string.IsNullOrEmpty(methodName))
                        {
                            if (DebugLogs == true)
                            {
                                Debug.Log($"Invoking {methodName} on {functionEntry.targetScripts[i].name} (Canceled: {isCanceled})");
                            }
                            functionEntry.targetScripts[i].Invoke(methodName, 0f);
                        }
                    }
                }
            }
        }
    }
    public static List<string> GetPublicVoidMethods(MonoBehaviour script)
    {
        List<string> methods = new List<string>();
        if (script == null) return methods;

        MethodInfo[] methodInfos = script.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
        foreach (var method in methodInfos)
        {
            if (method.ReturnType == typeof(void) && method.GetParameters().Length == 0)
            {
                methods.Add(method.Name);
            }
        }
        return methods;
    }
}




//using UnityEngine;
//using System.Collections.Generic;
//using System.Reflection;
//using UnityEngine.InputSystem;

//public class InputActionFunctionBinder : MonoBehaviour
//{
//    public static InputActionFunctionBinder instance;

//    [System.Serializable]
//    public class FunctionEntry
//    {
//        public string functionName; // Will store "On" + functionName
//        public List<MonoBehaviour> targetScripts = new List<MonoBehaviour>();
//        public List<string> methodNames = new List<string>();
//    }

//public void OnR2_OneTime() => InvokeFunction("OnR2_OneTime");
//	public void OnR2_Hold() => InvokeFunction("OnR2_Hold");

//public void OnR2() => InvokeFunction("OnR2");
//	public void OnL2() => InvokeFunction("OnL2");

//public void OnR1() => InvokeFunction("OnR1");
//	public void OnL1() => InvokeFunction("OnL1");

//public void OnOptions_Key() => InvokeFunction("OnOptions_Key");

//public void OnDpad_Right() => InvokeFunction("OnDpad_Right");
//	public void OnDpad_Left() => InvokeFunction("OnDpad_Left");
//	public void OnDpad_Up() => InvokeFunction("OnDpad_Up");
//	public void OnDpad_Down() => InvokeFunction("OnDpad_Down");

//public void OnSquare4() => InvokeFunction("OnSquare4");

//public void OnSqaure3() => InvokeFunction("OnSqaure3");

//public void OnSquare2() => InvokeFunction("OnSquare2");

//public void OnTriangle() => InvokeFunction("OnTriangle");

//public void OnSquare() => InvokeFunction("OnSquare");

//public void OnY() => InvokeFunction("OnY");
//	public void OnO() => InvokeFunction("OnO");

//public void OnLookAround() => InvokeFunction("OnLookAround");
//	public void OnSprint() => InvokeFunction("OnSprint");
//	public void OnMove() => InvokeFunction("OnMove");
//	public void OnX() => InvokeFunction("OnX");

//    [HideInInspector]
//    public List<FunctionEntry> savedFunctions = new List<FunctionEntry>();
//    public InputActionAsset inputActions;

//    private void Awake()
//    {
//        if (instance == null) instance = this;
//    }

//    public List<string> GetInputActionNames()
//    {
//        List<string> actionNames = new List<string>();
//        if (inputActions == null) return actionNames;

//        foreach (var map in inputActions.actionMaps)
//        {
//            foreach (var action in map.actions)
//            {
//                actionNames.Add(action.name);
//            }
//        }
//        return actionNames;
//    }
//    public void AutoPopulateFunctions()
//    {
//        if (inputActions == null) return;

//        List<string> actionNames = GetInputActionNames();

//        // Remove functions that are no longer present in InputActions
//        savedFunctions.RemoveAll(f => !actionNames.Contains(f.functionName.Replace("On", "")));

//        // Add missing functions
//        foreach (var actionName in actionNames)
//        {
//            string onFunctionName = "On" + actionName;
//            if (!savedFunctions.Exists(f => f.functionName == onFunctionName))
//            {
//                savedFunctions.Add(new FunctionEntry { functionName = onFunctionName });
//            }
//        }
//    }


//    public void RemoveScriptFromFunction(string functionName, int index)
//    {
//        var entry = savedFunctions.Find(f => f.functionName == functionName);
//        if (entry != null && index < entry.targetScripts.Count)
//        {
//            entry.targetScripts.RemoveAt(index);
//            entry.methodNames.RemoveAt(index);
//        }
//    }

//    public void InvokeFunction(string functionName)
//    {
//        var entry = savedFunctions.Find(f => f.functionName == functionName);
//        if (entry != null)
//        {
//            for (int i = 0; i < entry.targetScripts.Count; i++)
//            {
//                if (entry.targetScripts[i] != null && !string.IsNullOrEmpty(entry.methodNames[i]))
//                {
//                    entry.targetScripts[i].Invoke(entry.methodNames[i], 0);
//                }
//            }
//        }
//    }

//    public static List<string> GetPublicVoidMethods(MonoBehaviour script)
//    {
//        List<string> methods = new List<string>();
//        if (script == null) return methods;

//        MethodInfo[] methodInfos = script.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
//        foreach (var method in methodInfos)
//        {
//            if (method.ReturnType == typeof(void) && method.GetParameters().Length == 0)
//            {
//                methods.Add(method.Name);
//            }
//        }
//        return methods;
//    }
//}
