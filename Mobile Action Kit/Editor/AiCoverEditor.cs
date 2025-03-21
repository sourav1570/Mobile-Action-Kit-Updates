using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MobileActionKit
{
    [CustomEditor(typeof(AiCover))]
    public class AiCoverEditor : Editor
    {
        //  public SerializedProperty coverNodes;
        // public SerializedProperty Rotate;
        public SerializedProperty ScriptInfo;
     
        public SerializedProperty SingleAgentCover;
        public SerializedProperty UniversalHidingCover;
        public SerializedProperty SpecificTeamCover;
        public SerializedProperty TeamName;
       /// public SerializedProperty OpenFirePoints;
        //public SerializedProperty coverNodes;

        AiCover st;
        void OnEnable()
        {
            // Setup the SerializedProperties
            //  InitializeChildCovers = serializedObject.FindProperty("InitializeChildCovers");
            //    coverNodes = serializedObject.FindProperty("coverNodes");
            //Rotate = serializedObject.FindProperty("Rotate");
            ScriptInfo = serializedObject.FindProperty("ScriptInfo");       

            SingleAgentCover = serializedObject.FindProperty("SingleAgentCover");
            UniversalHidingCover = serializedObject.FindProperty("UniversalHidingCover");
            SpecificTeamCover = serializedObject.FindProperty("SpecificTeamCover");
            TeamName = serializedObject.FindProperty("TeamName");

           // OpenFirePoints = serializedObject.FindProperty("OpenFirePoints");

            //coverNodes = serializedObject.FindProperty("coverNodes");

            st = target as AiCover;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            // EditorGUILayout.PropertyField(InitializeChildCovers);
            //   EditorGUILayout.PropertyField(coverNodes,true);
            // EditorGUILayout.PropertyField(Rotate);
            EditorGUILayout.PropertyField(ScriptInfo);
            EditorGUILayout.PropertyField(SingleAgentCover);
            EditorGUILayout.PropertyField(UniversalHidingCover);
            EditorGUILayout.PropertyField(SpecificTeamCover);

           // EditorGUILayout.PropertyField(OpenFirePoints,true);

            // EditorGUILayout.PropertyField(coverNodes,true);

            //if (st.Rotate == true)
            //{
            //    Vector3 obj = st.gameObject.transform.eulerAngles;
            //    obj.y = 90f;
            //    st.gameObject.transform.eulerAngles = obj;
            //}
            //else
            //{
            //    Vector3 obj = st.gameObject.transform.eulerAngles;
            //    obj.y = 0f;
            //    st.gameObject.transform.eulerAngles = obj;
            //}

            if (st.SpecificTeamCover == true)
            {
                EditorGUILayout.PropertyField(TeamName);
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
}