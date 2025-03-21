using UnityEditor;
using UnityEngine;


namespace MobileActionKit
{
    [CustomEditor(typeof(FriendlyFire))]
    public class FriendlyFireEditor : Editor
    {
        SerializedProperty ScriptInfo;
        SerializedProperty FriendlyFireOptions;
        SerializedProperty TargetsScript;
        SerializedProperty defaultGameOverPanel;
        SerializedProperty friendlyFireGameOverPanel;
        SerializedProperty warningsToPlayer;
        SerializedProperty MakePlayerTraitorImmediatelyIfFriendlyDie;
        SerializedProperty FriendlyFireGameOverUIActivationDelay;
        SerializedProperty RestartGameUIButton;
        SerializedProperty AudioSourceComponent;
        SerializedProperty AudioClipsToPlaybackBasedOnFriendlyDistance;

        void OnEnable()
        {
            ScriptInfo = serializedObject.FindProperty("ScriptInfo");
            FriendlyFireOptions = serializedObject.FindProperty("FriendlyFireOptions");
            TargetsScript = serializedObject.FindProperty("TargetsScript");

            defaultGameOverPanel = serializedObject.FindProperty("DefaultGameOverPanel");
            friendlyFireGameOverPanel = serializedObject.FindProperty("FriendlyFireGameOverPanel");
            warningsToPlayer = serializedObject.FindProperty("WarningsToPlayer");
            MakePlayerTraitorImmediatelyIfFriendlyDie = serializedObject.FindProperty("MakePlayerTraitorImmediatelyIfFriendlyDie");
            FriendlyFireGameOverUIActivationDelay = serializedObject.FindProperty("FriendlyFireGameOverUIActivationDelay");
            RestartGameUIButton = serializedObject.FindProperty("RestartGameUIButton");
            
            AudioSourceComponent = serializedObject.FindProperty("AudioSourceComponent");
            AudioClipsToPlaybackBasedOnFriendlyDistance = serializedObject.FindProperty("AudioClipsToPlaybackBasedOnFriendlyDistance");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(ScriptInfo);
            EditorGUILayout.PropertyField(TargetsScript);
            EditorGUILayout.PropertyField(FriendlyFireOptions);
            FriendlyFire.FriendlyFireOptionsClass shootingOption = (FriendlyFire.FriendlyFireOptionsClass)FriendlyFireOptions.enumValueIndex;

            switch (shootingOption)
            {
                case FriendlyFire.FriendlyFireOptionsClass.GameOver:
                    EditorGUILayout.PropertyField(FriendlyFireGameOverUIActivationDelay);
                    EditorGUILayout.PropertyField(defaultGameOverPanel);
                    EditorGUILayout.PropertyField(friendlyFireGameOverPanel);
                    EditorGUILayout.PropertyField(RestartGameUIButton);
                    break;

                case FriendlyFire.FriendlyFireOptionsClass.MakePlayerToBeTraitor:
                  
                    EditorGUILayout.PropertyField(warningsToPlayer);
                    EditorGUILayout.PropertyField(MakePlayerTraitorImmediatelyIfFriendlyDie);
                    EditorGUILayout.PropertyField(RestartGameUIButton);
                    EditorGUILayout.PropertyField(AudioSourceComponent);
                    EditorGUILayout.PropertyField(AudioClipsToPlaybackBasedOnFriendlyDistance, true);
                    break;

                case FriendlyFire.FriendlyFireOptionsClass.DisableShootingOnFriendlies:
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
