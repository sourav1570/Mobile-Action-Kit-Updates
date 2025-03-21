using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace MobileActionKit
{
    [Serializable]
    public class FunctionInfo
    {
        public MonoBehaviour script;
        public string scriptName;
        public List<string> functionNames;
        public int selectedFunctionIndex;
    }

    public class ScriptFunctionSelector : MonoBehaviour
    {
        public List<FunctionInfo> functions = new List<FunctionInfo>();

        private void OnValidate()
        {
            foreach (var function in functions)
            {
                function.functionNames = GetFunctionNames(function.script);
            }
        }

        private List<string> GetFunctionNames(MonoBehaviour script)
        {
            List<string> functionNames = new List<string>();
            if (script != null)
            {
                Type type = script.GetType();
                MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                foreach (MethodInfo method in methods)
                {
                    functionNames.Add(method.Name);
                }
            }
            return functionNames;
        }
    }
}