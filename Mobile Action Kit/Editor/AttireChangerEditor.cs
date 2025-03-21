using UnityEngine;
using UnityEditor;

namespace MobileActionKit
{
    [CustomEditor(typeof(SwitchTeam))] // Changed "YourComponent" to "ChangeAttire"
    public class AttireChangerEditor : Editor
    {
        private SwitchTeam component; // Changed "YourComponent" to "ChangeAttire"

        private SerializedProperty BodySkinnedMesh;
        private SerializedProperty BodyMeshRenderer;
        private SerializedProperty WeaponMeshRenderer;
        private SerializedProperty ScriptInfo;
        private SerializedProperty TeamsAndMaterials;

        private void OnEnable()
        {
            component = (SwitchTeam)target; // Changed "YourComponent" to "ChangeAttire"

            TeamsAndMaterials = serializedObject.FindProperty("TeamsAndMaterials");
            BodySkinnedMesh = serializedObject.FindProperty("BodySkinnedMesh");

            BodyMeshRenderer = serializedObject.FindProperty("BodyMeshRenderer");

            WeaponMeshRenderer = serializedObject.FindProperty("WeaponMeshRenderer");

            ScriptInfo = serializedObject.FindProperty("ScriptInfo");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(ScriptInfo, true);

            //Add Label
            // Add Label with blue color
            GUIStyle blueLabelStyle = new GUIStyle(EditorStyles.boldLabel);
            blueLabelStyle.normal.textColor = Color.yellow;
            EditorGUILayout.LabelField("Drag and Drop Skinned Mesh Renderers And Mesh Renderers for Ai Body", blueLabelStyle);

            EditorGUILayout.PropertyField(BodySkinnedMesh, true);
            EditorGUILayout.PropertyField(BodyMeshRenderer, true);

            // Add Label with blue color
            GUIStyle yellowLabelStyle = new GUIStyle(EditorStyles.boldLabel);
            yellowLabelStyle.normal.textColor = Color.yellow;
            EditorGUILayout.LabelField("Drag and Drop Mesh Renderers for Ai Weapon", yellowLabelStyle);
            EditorGUILayout.PropertyField(WeaponMeshRenderer, true);

            EditorGUILayout.PropertyField(TeamsAndMaterials, true);

            //EditorGUILayout.PropertyField(skinnedMaterialIndexProperty);
            //EditorGUILayout.PropertyField(meshMaterialIndexProperty);

            if (GUILayout.Button("Change Team"))
            {
                ChangeTeam();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ChangeTeam()
        {
            component.ChangeTargetScriptTeamName();
        }
    }
}