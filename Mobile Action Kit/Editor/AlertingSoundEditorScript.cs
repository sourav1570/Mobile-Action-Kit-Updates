using UnityEditor;
using UnityEngine;

namespace MobileActionKit
{
    [CustomEditor(typeof(AlertingSound))]
    public class AlertingSoundEditorScript : Editor
    {
        SerializedProperty scriptInfoProp;
        SerializedProperty AlertingSoundReactions;
        SerializedProperty teamAffectedProp;
        SerializedProperty teamNameProp;
        SerializedProperty SoundInvestigationError;
        SerializedProperty timeToDeactivateProp;
        SerializedProperty AllowSprinting;
        SerializedProperty AllowRunning;
        SerializedProperty AllowWalking;

        SerializedProperty OverriteOtherAiApproach;
        AlertingSound st;

        void OnEnable()
        {
            // Initialize SerializedProperties
            scriptInfoProp = serializedObject.FindProperty("ScriptInfo");
            AlertingSoundReactions = serializedObject.FindProperty("AlertingSoundReactions");
            teamAffectedProp = serializedObject.FindProperty("TeamAffectedBySound");
            teamNameProp = serializedObject.FindProperty("TeamName");
            SoundInvestigationError = serializedObject.FindProperty("SoundInvestigationError");
            timeToDeactivateProp = serializedObject.FindProperty("TimeToDeactivate");
            AllowSprinting = serializedObject.FindProperty("AllowSprinting");
            AllowRunning = serializedObject.FindProperty("AllowRunning");
            AllowWalking = serializedObject.FindProperty("AllowWalking");

            OverriteOtherAiApproach = serializedObject.FindProperty("OverriteOtherAiApproach");

            st = target as AlertingSound;
        }

        public override void OnInspectorGUI()
        {
            // Update the serialized object
            serializedObject.Update();

            // Display default inspector property fields
            EditorGUILayout.PropertyField(scriptInfoProp);
            EditorGUILayout.PropertyField(AlertingSoundReactions);
            EditorGUILayout.PropertyField(teamAffectedProp);

            // Show teamNameProp only if TeamAffectedBySound is SelectedTeam
            if (teamAffectedProp.enumValueIndex == (int)AlertingSound.ChooseHumanoidAiTeamSelection.SelectedTeam)
            {
                EditorGUILayout.PropertyField(teamNameProp);
            }

            EditorGUILayout.PropertyField(SoundInvestigationError);
            EditorGUILayout.PropertyField(timeToDeactivateProp);

            EditorGUILayout.PropertyField(OverriteOtherAiApproach);

            if (st.OverriteOtherAiApproach == true)
            {
                EditorGUILayout.LabelField("Customise other humanoid Ai animations reaching towards this sound coordinate", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(AllowSprinting);
                EditorGUILayout.PropertyField(AllowRunning);
                EditorGUILayout.PropertyField(AllowWalking);
            }

            // Apply changes to the serialized object
            serializedObject.ApplyModifiedProperties();
        }
    }
}