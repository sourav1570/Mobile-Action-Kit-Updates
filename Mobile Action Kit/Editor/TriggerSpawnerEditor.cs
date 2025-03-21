using UnityEditor;
using UnityEngine;

namespace MobileActionKit
{
    [CustomEditor(typeof(AiMaintainingSpawner))]
    public class TriggerSpawnerEditor : Editor
    {
        public SerializedProperty ScriptInfo;
        public SerializedProperty SpawnWithinVolume;
        public SerializedProperty SpawnVolume;
        public SerializedProperty UseAgentsListScript;
        public SerializedProperty AiAgentsListScript;
        public SerializedProperty Agents;
        //public SerializedProperty RandomlySpawnAgents;
        public SerializedProperty MaxNumberOfAgentsToSpawn;
        public SerializedProperty UsePreciseSpawnPoints;
        public SerializedProperty PreciseSpawnPoints;
        
        public SerializedProperty AiUpdaterScript;
        public SerializedProperty AgentsToMaintainInGame;
        public SerializedProperty SpawnPointRadius;
        //public SerializedProperty AgentsSeparationRadius;
        public SerializedProperty DisplaySpawnedAgentsList;

        public SerializedProperty TriggeringGameObject;

        //public SerializedProperty MinSpawnTimeIntervals;
        //public SerializedProperty MaxSpawnTimeIntervals;

        public SerializedProperty ShowGizmosInTheScene;
        public SerializedProperty SpawnerActivationType;
        public SerializedProperty GizmoColor;
        //public SerializedProperty go;

     

        public SerializedProperty SpawnPoint;

        public SerializedProperty UseAiMaintainingSpawnersGlobalAiList;
        public SerializedProperty AiMaintainingSpawnersGlobalAiListScript;

        public SerializedProperty SpawnOnStartDelay;

        AiMaintainingSpawner st;


        void OnEnable()
        {
            // Setup the SerializedProperties
            ScriptInfo = serializedObject.FindProperty("ScriptInfo");

            TriggeringGameObject = serializedObject.FindProperty("TriggeringGameObject");
            SpawnWithinVolume = serializedObject.FindProperty("SpawnWithinVolume");
            SpawnVolume = serializedObject.FindProperty("SpawnVolume");
            UseAgentsListScript = serializedObject.FindProperty("UseAgentsListScript");
            AiAgentsListScript = serializedObject.FindProperty("AiAgentsListScript");
            Agents = serializedObject.FindProperty("Agents");
            MaxNumberOfAgentsToSpawn = serializedObject.FindProperty("MaxNumberOfAgentsToSpawn");
            UsePreciseSpawnPoints = serializedObject.FindProperty("UsePreciseSpawnPoints");
            PreciseSpawnPoints = serializedObject.FindProperty("PreciseSpawnPoints");
            
            AiUpdaterScript = serializedObject.FindProperty("AiUpdaterScript");
            AgentsToMaintainInGame = serializedObject.FindProperty("AgentsToMaintainInGame");
            SpawnPointRadius = serializedObject.FindProperty("SpawnPointRadius");
            DisplaySpawnedAgentsList = serializedObject.FindProperty("DisplaySpawnedAgentsList");
            //AgentsSeparationRadius = serializedObject.FindProperty("AgentsSeparationRadius");
           // RandomlySpawnAgents = serializedObject.FindProperty("RandomlySpawnAgents");

            //MinSpawnTimeIntervals = serializedObject.FindProperty("MinSpawnTimeIntervals");
            //MaxSpawnTimeIntervals = serializedObject.FindProperty("MaxSpawnTimeIntervals");

            ShowGizmosInTheScene = serializedObject.FindProperty("ShowGizmosInTheScene");

            SpawnerActivationType = serializedObject.FindProperty("SpawnerActivationType");
            GizmoColor = serializedObject.FindProperty("GizmosColor");

            //go = serializedObject.FindProperty("go");

           

            SpawnPoint = serializedObject.FindProperty("SpawnPoint");

            UseAiMaintainingSpawnersGlobalAiList = serializedObject.FindProperty("UseAiMaintainingSpawnersGlobalAiList");
            AiMaintainingSpawnersGlobalAiListScript = serializedObject.FindProperty("AiMaintainingSpawnersGlobalAiListScript");

            SpawnOnStartDelay = serializedObject.FindProperty("SpawnOnStartDelay");

            st = target as AiMaintainingSpawner;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(ScriptInfo, true);
            EditorGUILayout.PropertyField(ShowGizmosInTheScene, true);
            EditorGUILayout.PropertyField(GizmoColor);
            EditorGUILayout.PropertyField(SpawnerActivationType, true);
            EditorGUILayout.PropertyField(TriggeringGameObject);
            EditorGUILayout.PropertyField(SpawnWithinVolume);
            EditorGUILayout.PropertyField(UseAgentsListScript);

            EditorGUILayout.PropertyField(UseAiMaintainingSpawnersGlobalAiList);

            if (st.UseAiMaintainingSpawnersGlobalAiList == true)
            {
                EditorGUILayout.PropertyField(AiMaintainingSpawnersGlobalAiListScript);
            }

                // EditorGUILayout.PropertyField(go);
                if (st.UseAgentsListScript == true)
            {
                
                EditorGUILayout.PropertyField(AiAgentsListScript);
                EditorGUILayout.PropertyField(MaxNumberOfAgentsToSpawn);
                //EditorGUILayout.PropertyField(RandomlySpawnAgents);
                //EditorGUILayout.PropertyField(MinSpawnTimeIntervals);
                //EditorGUILayout.PropertyField(MaxSpawnTimeIntervals);
                if (st.SpawnWithinVolume == true)
                {
                    EditorGUILayout.PropertyField(SpawnVolume);
                }
                else
                {
                    EditorGUILayout.PropertyField(SpawnPoint);
                    EditorGUILayout.PropertyField(UsePreciseSpawnPoints);
                    if (st.UsePreciseSpawnPoints == true)
                    {
                        EditorGUILayout.PropertyField(PreciseSpawnPoints, true);
                    }
                    EditorGUILayout.PropertyField(SpawnPointRadius);

                }

                

                EditorGUILayout.PropertyField(AiUpdaterScript);
                EditorGUILayout.PropertyField(AgentsToMaintainInGame);
                //EditorGUILayout.PropertyField(AgentsSeparationRadius);
                EditorGUILayout.PropertyField(DisplaySpawnedAgentsList, true);

            }
            else
            {
                
                EditorGUILayout.PropertyField(Agents, true);
                EditorGUILayout.PropertyField(MaxNumberOfAgentsToSpawn);
                //EditorGUILayout.PropertyField(RandomlySpawnAgents);
                //EditorGUILayout.PropertyField(MinSpawnTimeIntervals);
                //EditorGUILayout.PropertyField(MaxSpawnTimeIntervals);


                if (st.SpawnWithinVolume == true)
                {
                    EditorGUILayout.PropertyField(SpawnVolume);
                }
                else
                {
                    EditorGUILayout.PropertyField(SpawnPoint);
                    EditorGUILayout.PropertyField(UsePreciseSpawnPoints);
                    if (st.UsePreciseSpawnPoints == true)
                    {
                        EditorGUILayout.PropertyField(PreciseSpawnPoints, true);
                    }
                    EditorGUILayout.PropertyField(SpawnPointRadius);

                }

               
                

                EditorGUILayout.PropertyField(AiUpdaterScript);
                EditorGUILayout.PropertyField(AgentsToMaintainInGame);
                //EditorGUILayout.PropertyField(AgentsSeparationRadius);
               

            }


            if(st.SpawnerActivationType == AiMaintainingSpawner.ChooseSpawnerType.SpawnOnTriggerEnter)
            {
                // Add Collider Buttons
                if (GUILayout.Button("Add Box Collider"))
                {
                    AddCollider<BoxCollider>();
                }

                if (GUILayout.Button("Add Capsule Collider"))
                {
                    AddCollider<CapsuleCollider>();
                }

                if (GUILayout.Button("Add Sphere Collider"))
                {
                    AddCollider<SphereCollider>();
                }

            }
            else
            {
                EditorGUILayout.PropertyField(SpawnOnStartDelay);

                if (st.GetComponent<Collider>() != null)
                {
                    DestroyImmediate(st.GetComponent<Collider>());
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
        // Helper method to add a collider component with IsTrigger checked
        private void AddCollider<T>() where T : Collider
        {
            T collider;
            collider = st.gameObject.AddComponent<T>();
            collider.isTrigger = true;
        }

    }
}