#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace MobileActionKit
{
    [CustomEditor(typeof(WaveSpawner))]
    public class WaveSpawnerEditor : Editor
    {
        WaveSpawner st;

        SerializedProperty ScriptInfo;
        SerializedProperty ActivationType;
        SerializedProperty DrawSpawnerGizmos;
        SerializedProperty gizmosColor;
        //SerializedProperty Agents;
        //SerializedProperty UseSeparateScriptForAssigningAgents;
        //SerializedProperty AgentList;
        //SerializedProperty Agents;
        SerializedProperty ColliderComponent;
        SerializedProperty NextWaveAfterPreviousWaveDestroyed;
       // SerializedProperty usingPredefinedSpawnPoints;
       // SerializedProperty predefinedPoints;
        SerializedProperty WaveName;
        SerializedProperty EnableWavesCounterUI;
        SerializedProperty waveText;
        SerializedProperty TimeToDeactiveWaveText;
        SerializedProperty displayCountDownTimer;
        SerializedProperty countDownTimerText;
        //SerializedProperty AgentsSeparationRadius;
        SerializedProperty AiUpdaterScript;
        SerializedProperty waves;
        SerializedProperty TriggerWaves;
       // SerializedProperty AllWaveScripts;

        SerializedProperty MinCasualtiesCheckTime;
        SerializedProperty MaxCasualtiesCheckTime;

        SerializedProperty Player;

        private void OnEnable()
        {
            ScriptInfo = serializedObject.FindProperty("ScriptInfo");
            ActivationType = serializedObject.FindProperty("ActivationType");
            DrawSpawnerGizmos = serializedObject.FindProperty("DrawSpawnerGizmos");
            gizmosColor = serializedObject.FindProperty("GizmosColor");

            Player = serializedObject.FindProperty("Player");
            //UseSeparateScriptForAssigningAgents = serializedObject.FindProperty("UseSeparateScriptForAssigningAgents");
            //AgentList = serializedObject.FindProperty("AgentList");
            //Agents = serializedObject.FindProperty("Agents");

            //Agents = serializedObject.FindProperty("Agents");
            ColliderComponent = serializedObject.FindProperty("ColliderComponent");
            NextWaveAfterPreviousWaveDestroyed = serializedObject.FindProperty("NextWaveAfterPreviousWaveDestroyed");
           // usingPredefinedSpawnPoints = serializedObject.FindProperty("UsingPredefinedSpawnPoints");
           // predefinedPoints = serializedObject.FindProperty("PredefinedPoints");
            WaveName = serializedObject.FindProperty("WaveName");
            EnableWavesCounterUI = serializedObject.FindProperty("EnableWavesCounterUI");
            waveText = serializedObject.FindProperty("WaveText");
            TimeToDeactiveWaveText = serializedObject.FindProperty("TimeToDeactiveWaveText");
            displayCountDownTimer = serializedObject.FindProperty("DisplayCountDownTimer");
            countDownTimerText = serializedObject.FindProperty("CountDownTimerText");
            //AgentsSeparationRadius = serializedObject.FindProperty("AgentsSeparationRadius");
            AiUpdaterScript = serializedObject.FindProperty("AiUpdaterScript");
            waves = serializedObject.FindProperty("Waves");
            TriggerWaves = serializedObject.FindProperty("TriggerWaves");

            MinCasualtiesCheckTime = serializedObject.FindProperty("MinCasualtiesCheckTime");
            MaxCasualtiesCheckTime = serializedObject.FindProperty("MaxCasualtiesCheckTime");

           // AllWaveScripts = serializedObject.FindProperty("AllWaveScripts");

            st = target as WaveSpawner;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(ScriptInfo,true);
            EditorGUILayout.PropertyField(ActivationType);
            EditorGUILayout.PropertyField(DrawSpawnerGizmos);
            EditorGUILayout.PropertyField(gizmosColor);
            //EditorGUILayout.PropertyField(UseSeparateScriptForAssigningAgents);
            EditorGUILayout.PropertyField(Player);
            if (st.ActivationType == WaveSpawner.WaveType.StartOnTriggerEnter)
            {
                EditorGUILayout.PropertyField(ColliderComponent);
                if (st.GetComponent<BoxCollider>() == null)
                {
                    BoxCollider b = st.gameObject.AddComponent<BoxCollider>();
                    st.ColliderComponent = b;
                    st.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                }
                else
                {
                    st.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                }
            }
            else
            {
                if (st.gameObject.GetComponent<Collider>() != null)
                {
                    DestroyImmediate(st.gameObject.GetComponent<Collider>());
                }
            }

            //if(st.UseSeparateScriptForAssigningAgents == true)
            //{
            //    EditorGUILayout.PropertyField(AgentList); 
            //}
            //else
            //{
            //    EditorGUILayout.PropertyField(Agents);
            //}
           

            if (st.ActivationType == WaveSpawner.WaveType.BeginOnPlay || st.ActivationType == WaveSpawner.WaveType.StartOnTriggerEnter)
            {
                EditorGUILayout.PropertyField(NextWaveAfterPreviousWaveDestroyed);
                EditorGUILayout.PropertyField(MinCasualtiesCheckTime);
                EditorGUILayout.PropertyField(MaxCasualtiesCheckTime);
                EditorGUILayout.PropertyField(WaveName);
                EditorGUILayout.PropertyField(EnableWavesCounterUI);
                EditorGUILayout.PropertyField(displayCountDownTimer);

                if (st.EnableWavesCounterUI == true)
                {
                    EditorGUILayout.PropertyField(waveText);
                    EditorGUILayout.PropertyField(TimeToDeactiveWaveText);
                }
                if (st.DisplayCountDownTimer == true)
                {
                    EditorGUILayout.PropertyField(countDownTimerText);
                }
            }


            //EditorGUILayout.PropertyField(usingPredefinedSpawnPoints);

            //if (st.UsingPredefinedSpawnPoints == true)
            //{
            //    EditorGUILayout.PropertyField(predefinedPoints, true);
            //}

            //EditorGUILayout.PropertyField(AgentsSeparationRadius);
            EditorGUILayout.PropertyField(AiUpdaterScript);

            if (st.ActivationType == WaveSpawner.WaveType.BeginOnPlay || st.ActivationType == WaveSpawner.WaveType.StartOnTriggerEnter)
            {
                EditorGUILayout.PropertyField(waves, true);

            }
            else
            {
                EditorGUILayout.PropertyField(MinCasualtiesCheckTime);
                EditorGUILayout.PropertyField(MaxCasualtiesCheckTime);
               // EditorGUILayout.PropertyField(AllWaveScripts,true);
                EditorGUILayout.PropertyField(TriggerWaves, true);

            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

