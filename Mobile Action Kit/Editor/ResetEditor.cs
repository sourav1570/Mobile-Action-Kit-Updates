using System.Collections;
using UnityEngine;
using UnityEditor;

namespace MobileActionKit
{
    [CustomEditor(typeof(DeveloperMode))]
    public class ResetEditor : Editor
    {
        public SerializedProperty ScriptInfo;
        public SerializedProperty AddGoldBars;
        public SerializedProperty GoldBars;      
        DeveloperMode st;
        void OnEnable()
        {
            // Setup the SerializedProperties
            AddGoldBars = serializedObject.FindProperty("AddGoldBars");
            GoldBars = serializedObject.FindProperty("GoldBars");
            ScriptInfo = serializedObject.FindProperty("ScriptInfo");
            st = target as DeveloperMode;
        }

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            serializedObject.Update();
            EditorGUILayout.PropertyField(ScriptInfo);
            EditorGUILayout.PropertyField(AddGoldBars);

            if (st.AddGoldBars == true)
            {
                EditorGUILayout.PropertyField(GoldBars);
            }

            DeveloperMode datascript = (DeveloperMode)target;
            if (GUILayout.Button("Delete Saved Data Stored"))
            {
                datascript.deletealldata();
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}