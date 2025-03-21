using UnityEngine;
using UnityEditor;

namespace MobileActionKit
{
    [CustomEditor(typeof(CoreAiBehaviour))]
    public class SimpleAiBehaviourEditor : Editor
    {

        private CoreAiBehaviour mytar;

        public SerializedProperty
            ScriptInfo,
                AiType,
         AgentName,
         DebugAgentState,
            Components,
             Speeds,
             NavMeshAgentSettings,
            // AiAgentAnimatorParameters,
            AiHearing,
            AiCharging,
            AiDeadBodyAlerts,
            AiStrafing,
            AiMeleeAttack,
            ZombieCharging,
            //  FollowCommanderValues,
            ZombieSpeeds,
            AiFiringPoints,
            AiCovers,
            DebugInfo,
            EnableGrenadesThrow,
            AgentRole,
            CombatStateBehaviours,
            NonCombatBehaviours,
            ChargingProbability,
            CoversUsageProbability,
            FiringPointsUsageProbability,
            //  DebugTotalProbabilityValue,
            EnableEnemyLostAlerts,
            PostCombatAimedScanBehaviour,
              AiPursue,
             // AiDirectionMarkers,
            // DeadBodiesSeen,
            //AutoUniqueIdentity,
            GrenadeAlerts,
            // CoverPoints,
            StationedShooting;


        bool CanShow = false;

        private SerializedProperty DefaultBehaviour;
        private SerializedProperty EnableSoundAlerts;
        private SerializedProperty EnableHeadRotations;
        private SerializedProperty ChooseEnemyPursueType;

        private SerializedProperty CombatBehaviourUseImpactAnimations;

        private SerializedProperty AiChargingCloseProximityBehaviour;
        private SerializedProperty AiChargingSprintingBehaviour;
        private SerializedProperty AiEmergencyState;

        void OnEnable()
        {
            mytar = target as CoreAiBehaviour;


            // Setup the SerializedProperties
            AiType = serializedObject.FindProperty("AiType");

            AgentName = serializedObject.FindProperty("AgentName");
            DebugAgentState = serializedObject.FindProperty("DebugAgentState");
            CombatStateBehaviours = serializedObject.FindProperty("CombatStateBehaviours");
            NonCombatBehaviours = serializedObject.FindProperty("NonCombatBehaviours");

            ChargingProbability = serializedObject.FindProperty("ChargingProbability");
            CoversUsageProbability = serializedObject.FindProperty("CoversUsageProbability");
            FiringPointsUsageProbability = serializedObject.FindProperty("FiringPointsUsageProbability");
            // DebugTotalProbabilityValue = serializedObject.FindProperty("DebugTotalProbabilityValue");


            Components = serializedObject.FindProperty("Components");
            Speeds = serializedObject.FindProperty("Speeds");
            NavMeshAgentSettings = serializedObject.FindProperty("NavMeshAgentSettings");
            //   AiAgentAnimatorParameters = serializedObject.FindProperty("AiAgentAnimatorParameters");

            StationedShooting = serializedObject.FindProperty("StationedShooting");

            AiHearing = serializedObject.FindProperty("AiHearing");
            AiCharging = serializedObject.FindProperty("AiCharging");
            AiDeadBodyAlerts = serializedObject.FindProperty("AiDeadBodyAlerts");
            AiPursue = serializedObject.FindProperty("AiPursue");

            AiStrafing = serializedObject.FindProperty("AiStrafing");
            //FollowCommanderValues = serializedObject.FindProperty("FollowCommanderValues");
            AiFiringPoints = serializedObject.FindProperty("AiFiringPoints");
            AiCovers = serializedObject.FindProperty("AiCovers");
            DebugInfo = serializedObject.FindProperty("DebugInfo");

            EnableGrenadesThrow = serializedObject.FindProperty("EnableGrenadesThrow");
            AgentRole = serializedObject.FindProperty("AgentRole");

            ScriptInfo = serializedObject.FindProperty("ScriptInfo");

            EnableEnemyLostAlerts = serializedObject.FindProperty("EnableEnemyLostAlerts");

            // AiDirectionMarkers = serializedObject.FindProperty("AiDirectionMarkers");
            // Cube = serializedObject.FindProperty("Cube");
            // DeadBodiesSeen = serializedObject.FindProperty("DeadBodiesSeen");
            // AutoUniqueIdentity = serializedObject.FindProperty("AutoUniqueIdentity");
            //CoverPoints = serializedObject.FindProperty("CoverPoints");

            GrenadeAlerts = serializedObject.FindProperty("GrenadeAlerts");
            AiMeleeAttack = serializedObject.FindProperty("AiMeleeAttack");

            DefaultBehaviour = serializedObject.FindProperty("NonCombatBehaviours.DefaultBehaviour");
            EnableSoundAlerts = serializedObject.FindProperty("NonCombatBehaviours.EnableSoundAlerts");
            EnableHeadRotations = serializedObject.FindProperty("NonCombatBehaviours.EnableHeadRotations");
            ChooseEnemyPursueType = serializedObject.FindProperty("CombatStateBehaviours.ChooseEnemyPursueType");

            CombatBehaviourUseImpactAnimations = serializedObject.FindProperty("CombatStateBehaviours.UseImpactAnimations");

            AiChargingCloseProximityBehaviour = serializedObject.FindProperty("AiCharging.CloseProximityBehaviour");
            AiChargingSprintingBehaviour = serializedObject.FindProperty("AiCharging.SprintingBehaviour");
            AiEmergencyState = serializedObject.FindProperty("AiEmergencyState");

            PostCombatAimedScanBehaviour = serializedObject.FindProperty("PostCombatAimedScanBehaviour");
            ZombieCharging = serializedObject.FindProperty("ZombieCharging");
            ZombieSpeeds = serializedObject.FindProperty("ZombieSpeeds");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            // EditorGUI.BeginChangeCheck();
            //  sotar.Update();

            mytar.ChangeGameobjectName();

            EditorGUILayout.PropertyField(ScriptInfo);
            EditorGUILayout.PropertyField(AgentName);
            EditorGUILayout.PropertyField(AgentRole);
            EditorGUILayout.PropertyField(DebugAgentState);

            if (mytar.DebugAgentState == true)
            {
                EditorGUILayout.PropertyField(DebugInfo, true);
            }            
            if (mytar.AgentRole != CoreAiBehaviour.Role.Zombie)
            {
                // EditorGUILayout.PropertyField(CoverPoints, true);
                EditorGUILayout.PropertyField(CombatStateBehaviours, true);
                EditorGUILayout.PropertyField(NonCombatBehaviours, true);


                EditorGUILayout.PropertyField(Components, true);
                //  EditorGUILayout.PropertyField(AiAgentAnimatorParameters, true);
                EditorGUILayout.PropertyField(Speeds, true);
                EditorGUILayout.PropertyField(NavMeshAgentSettings, true);

              //  EditorGUILayout.PropertyField(AiDirectionMarkers, true);

                if (mytar.CombatStateBehaviours.EnableAiCharging == true)
                {
                    EditorGUILayout.PropertyField(AiCharging, true);

                }

                //if (mytar.NonCombatBehaviours.EnableHeadMovements == true)
                //{
                //    if (mytar.GetComponent<HeadIk>() == null)
                //    {
                //        mytar.gameObject.AddComponent<HeadIk>();
                //    }
                //}
                //else
                //{
                //    if (mytar.GetComponent<HeadIk>() != null)
                //    {
                //        DestroyImmediate(mytar.GetComponent<HeadIk>());
                //    }
                //}

                if (mytar.CombatStateBehaviours.TakeCovers == true)
                {
                    EditorGUILayout.PropertyField(AiCovers, true);

                }

                if (mytar.CombatStateBehaviours.UseFiringPoints == true)
                {

                    EditorGUILayout.PropertyField(AiFiringPoints, true);

                }

                if (mytar.CombatStateBehaviours.EnablePostCombatScan == true)
                {
                    EditorGUILayout.PropertyField(PostCombatAimedScanBehaviour, true);
                }


                if (mytar.CombatStateBehaviours.EnableStrafing == true)
                {
                    EditorGUILayout.PropertyField(AiStrafing, true);
                }

                if (mytar.CombatStateBehaviours.EnableMeleeAttack == true)
                {
                    EditorGUILayout.PropertyField(AiMeleeAttack, true);
                }

                if (mytar.CombatStateBehaviours.EnableStationedCrouchedShooting == true)
                {
                    EditorGUILayout.PropertyField(StationedShooting, true);
                }


                //if (mytar.FollowCommander == true)
                //{
                //    EditorGUILayout.PropertyField(FollowCommanderValues, true);
                //}

                if (mytar.CombatStateBehaviours.ChooseEnemyPursueType != CoreAiBehaviour.EnemyPursueTypes.DoNotPursueEnemy)
                {
                    EditorGUILayout.PropertyField(AiPursue, true);
                }

                if (mytar.NonCombatBehaviours.EnableEmergencyAlerts == true)
                {
                    EditorGUILayout.PropertyField(AiEmergencyState, true);
                    //EditorGUILayout.PropertyField(DeadBodiesSeen, true);
                    // EditorGUILayout.PropertyField(AutoUniqueIdentity);
                }

                if (mytar.NonCombatBehaviours.EnableDeadBodyAlerts == true)
                {
                    EditorGUILayout.PropertyField(AiDeadBodyAlerts, true);
                    //EditorGUILayout.PropertyField(DeadBodiesSeen, true);
                    // EditorGUILayout.PropertyField(AutoUniqueIdentity);
                }

                if (mytar.NonCombatBehaviours.EnableSoundAlerts == true)
                {
                    EditorGUILayout.PropertyField(AiHearing, true);
                }

                if (mytar.CombatStateBehaviours.EnableGrenadeAlerts == true)
                {
                    EditorGUILayout.PropertyField(GrenadeAlerts, true);

                }
                //if (mytar.AgentRole == CoreAiBehaviour.Role.Follower)
                //{
                //    EditorGUILayout.PropertyField(FollowCommanderValues, true);
                //}

                if (mytar.AgentRole == CoreAiBehaviour.Role.Leader)
                {
                    //if (mytar.GetComponent<Followers>() == null)
                    //{
                    //    mytar.gameObject.AddComponent<Followers>();
                    //}
                    //else if (mytar.GetComponent<Followers>() != null)
                    //{
                    //    mytar.GetComponent<Followers>().enabled = true;
                    //}

                    if (mytar.GetComponent<AiGuide>() == null)
                    {
                        mytar.gameObject.AddComponent<AiGuide>();
                    }
                    else if (mytar.GetComponent<AiGuide>() != null)
                    {
                        mytar.GetComponent<AiGuide>().enabled = true;
                    }
                }
                else if (mytar.AgentRole != CoreAiBehaviour.Role.Leader)
                {
                    //if(mytar.GetComponent<Followers>() != null)
                    //{
                    //    mytar.GetComponent<Followers>().enabled = false;
                    //}
                    if (mytar.GetComponent<AiGuide>() != null)
                    {
                        mytar.GetComponent<AiGuide>().enabled = false;
                    }

                }

                if (mytar.AgentRole == CoreAiBehaviour.Role.Follower)
                {
                    //if (mytar.GetComponent<Followers>() == null)
                    //{
                    //    mytar.gameObject.AddComponent<Followers>();
                    //}
                    //else if (mytar.GetComponent<Followers>() != null)
                    //{
                    //    mytar.GetComponent<Followers>().enabled = true;
                    //}

                    if (mytar.GetComponent<AiFollower>() == null)
                    {
                        mytar.gameObject.AddComponent<AiFollower>();
                    }
                    else if (mytar.GetComponent<AiFollower>() != null)
                    {
                        mytar.GetComponent<AiFollower>().enabled = true;
                    }
                }
                else if (mytar.AgentRole != CoreAiBehaviour.Role.Follower)
                {
                    //if(mytar.GetComponent<Followers>() != null)
                    //{
                    //    mytar.GetComponent<Followers>().enabled = false;
                    //}
                    if (mytar.GetComponent<AiFollower>() != null)
                    {
                        mytar.GetComponent<AiFollower>().enabled = false;
                    }

                }


                //CoreAiBehaviour.AiTypes st = (CoreAiBehaviour.AiTypes)AiType.enumValueIndex; 

                //if (mytar.AgentRole == CoreAiBehaviour.Role.Follower)
                //{
                //    if (mytar.GetComponent<AiGuide>() != null)
                //    {
                //        DestroyImmediate(mytar.GetComponent<AiGuide>());
                //    }
                //    if (mytar.GetComponent<Followers>() != null)
                //    {
                //        DestroyImmediate(mytar.GetComponent<Followers>());
                //    }
                //}
                //else if (mytar.AgentRole == CoreAiBehaviour.Role.Leader)
                //{
                //    if (mytar.GetComponent<AiGuide>() != null)
                //    {
                //        DestroyImmediate(mytar.GetComponent<AiGuide>());
                //    }
                //    if (mytar.GetComponent<Followers>() != null)
                //    {
                //        DestroyImmediate(mytar.GetComponent<Followers>());
                //    }
                //}
                //else
                //{
                //    if (mytar.GetComponent<AiGuide>() != null)
                //    {
                //        DestroyImmediate(mytar.GetComponent<AiGuide>());
                //    }
                //    if(mytar.GetComponent<Followers>() != null)
                //    {
                //        DestroyImmediate(mytar.GetComponent<Followers>());
                //    }
                //}

                //if (mytar.EnableGrenadesThrow == true)
                //{
                //    if (mytar.GetComponent<HumanoidGrenadeThrower>() == null)
                //    {
                //        mytar.gameObject.AddComponent<HumanoidGrenadeThrower>();
                //    }
                //}
                //else
                //{
                //    if (mytar.GetComponent<HumanoidGrenadeThrower>() != null)
                //    {
                //        DestroyImmediate(mytar.GetComponent<HumanoidGrenadeThrower>()); 
                //    }
                //}


                if (mytar.CombatStateBehaviours.UseGrenades == true)
                {
                    if (mytar.GetComponent<HumanoidGrenadeThrower>() == null)
                    {
                        mytar.gameObject.AddComponent<HumanoidGrenadeThrower>();
                    }
                    else if (mytar.GetComponent<HumanoidGrenadeThrower>() != null)
                    {
                        mytar.GetComponent<HumanoidGrenadeThrower>().enabled = true;
                    }
                }
                else if (mytar.GetComponent<HumanoidGrenadeThrower>() != null)
                {
                    mytar.GetComponent<HumanoidGrenadeThrower>().enabled = false;
                }

                //else if (mytar.NonCombatBehaviours.DefaultBehaviour == CoreAiBehaviour.InvestigationTypes.Patrolling)
                //{
                //    if (mytar.GetComponent<Patrolling>() == null)
                //    {
                //        mytar.gameObject.AddComponent<Patrolling>();
                //    }
                //    if (mytar.GetComponent<Wandering>() != null)
                //    {
                //        DestroyImmediate(mytar.GetComponent<Wandering>());
                //    }
                //    if (mytar.GetComponent<ScanningScript>() != null)
                //    {
                //        DestroyImmediate(mytar.GetComponent<ScanningScript>());
                //    }
                //    if (mytar.GetComponent<HeadIk>() == null)
                //    {
                //        mytar.gameObject.AddComponent<HeadIk>();
                //    }
                //}
                //else if (mytar.NonCombatBehaviours.DefaultBehaviour == CoreAiBehaviour.InvestigationTypes.Wandering)
                //{
                //    if (mytar.GetComponent<Patrolling>() != null)
                //    {
                //        DestroyImmediate(mytar.GetComponent<Patrolling>());

                //    }
                //    if (mytar.GetComponent<Wandering>() == null)
                //    {
                //        mytar.gameObject.AddComponent<Wandering>();
                //    }
                //    if (mytar.GetComponent<ScanningScript>() != null)
                //    {
                //        DestroyImmediate(mytar.GetComponent<ScanningScript>());
                //    }
                //    if (mytar.GetComponent<HeadIk>() == null)
                //    {
                //        mytar.gameObject.AddComponent<HeadIk>();
                //    }
                //}

                //if (mytar.CombatStateBehaviours.UseImpactAnimations == true)
                //{
                //    if (mytar.GetComponent<HumanoidBodyHits>() == null)
                //    {
                //        mytar.gameObject.AddComponent<HumanoidBodyHits>();
                //    }
                //}
                //else
                //{
                //    if (mytar.GetComponent<HumanoidBodyHits>() != null)
                //    {
                //        DestroyImmediate(mytar.GetComponent<HumanoidBodyHits>());
                //    }
                //}
                if (mytar.CombatStateBehaviours.EnableAiCharging == true)
                {
                    if (mytar.UpdateChargeProbability == false)
                    {
                        // mytar.ProbabilityOfCharge = 100;
                        ++mytar.CountsForEditorScript;
                        mytar.UpdateChargeProbability = true;
                    }
                }
                else
                {
                    if (mytar.UpdateChargeProbability == true)
                    {
                        if (mytar.CountsForEditorScript <= 0)
                        {
                            mytar.CountsForEditorScript = 0;
                        }
                        else
                        {
                            --mytar.CountsForEditorScript;
                        }
                        mytar.UpdateChargeProbability = false;
                    }
                   // mytar.ChargingProbability = 0;
                }

                if (mytar.CombatStateBehaviours.TakeCovers == true)
                {

                    if (mytar.UpdateCoverProbability == false)
                    {
                        ++mytar.CountsForEditorScript;
                        // mytar.ProbabilityOfCover = 100;
                        mytar.UpdateCoverProbability = true;
                    }
                }
                else
                {
                    if (mytar.UpdateCoverProbability == true)
                    {
                        if(mytar.CountsForEditorScript <= 0)
                        {
                            mytar.CountsForEditorScript = 0;
                        }
                        else
                        {
                            --mytar.CountsForEditorScript;
                        }
                        mytar.UpdateCoverProbability = false;
                    }
                   // mytar.CoversUsageProbability = 0;
                }

                if (mytar.CombatStateBehaviours.UseFiringPoints == true)
                {
                    if (mytar.UpdateWaypointProbability == false)
                    {
                        // mytar.ProbabilityOfWaypointUse = 100;
                        ++mytar.CountsForEditorScript;
                        mytar.UpdateWaypointProbability = true;
                    }

                }
                else
                {
                    if (mytar.UpdateWaypointProbability == true)
                    {
                        if (mytar.CountsForEditorScript <= 0)
                        {
                            mytar.CountsForEditorScript = 0;
                        }
                        else
                        {
                            --mytar.CountsForEditorScript;
                        }
                        mytar.UpdateWaypointProbability = false;
                    }
                   // mytar.FiringPointsUsageProbability = 0;
                }

                if (mytar.CountsForEditorScript >= 2)
                {
                    CanShow = true;
                }
                else
                {
                    CanShow = false;
                }

                if (CanShow == true)
                {
                    //  EditorGUILayout.PropertyField(DebugTotalProbabilityValue);

                    if (mytar.UpdateChargeProbability == true)
                    {
                        EditorGUILayout.PropertyField(ChargingProbability, true);

                    }
                    if (mytar.UpdateCoverProbability == true)
                    {
                        EditorGUILayout.PropertyField(CoversUsageProbability, true);
                    }
                    if (mytar.UpdateWaypointProbability == true)
                    {
                        EditorGUILayout.PropertyField(FiringPointsUsageProbability, true);
                    }

                    if (mytar.UpdateChargeProbability == false && mytar.UpdateCoverProbability == false
                        && mytar.UpdateWaypointProbability == false)
                    {
                        mytar.CountsForEditorScript = 0;
                    }
                }

                if (mytar.CombatStateBehaviours.UseFiringPoints == false && mytar.CombatStateBehaviours.TakeCovers == false
                    && mytar.CombatStateBehaviours.EnableAiCharging == false)
                {
                    mytar.CountsForEditorScript = 0;
                }
            }
            else
            {
                EditorGUILayout.PropertyField(Components, true);
                EditorGUILayout.PropertyField(NavMeshAgentSettings, true);
                mytar.CombatStateBehaviours.TakeCovers = false;
                mytar.CombatStateBehaviours.UseFiringPoints = false;
                mytar.CombatStateBehaviours.EnableStrafing = false;
                mytar.CombatStateBehaviours.UseGrenades = false;
                mytar.CombatStateBehaviours.EnableGrenadeAlerts = false;
                mytar.CombatStateBehaviours.EnableStationedCrouchedShooting = false;
                mytar.NonCombatBehaviours.EnableEmergencyAlerts = false;
                mytar.CombatStateBehaviours.EnableMeleeAttack = true;
                mytar.CombatStateBehaviours.EnableAiCharging = true;
                mytar.AiCharging.OpenFireBehaviour.StopAndShootProbability = 0;

                EditorGUILayout.PropertyField(DefaultBehaviour);
                EditorGUILayout.PropertyField(ChooseEnemyPursueType);
                EditorGUILayout.PropertyField(EnableSoundAlerts);
                // Do not show EnableDeadBodyAlerts here
                EditorGUILayout.PropertyField(EnableHeadRotations);
                EditorGUILayout.PropertyField(CombatBehaviourUseImpactAnimations);

                EditorGUILayout.PropertyField(ZombieSpeeds, true);

                EditorGUILayout.PropertyField(ZombieCharging,true);
                //EditorGUILayout.PropertyField(AiChargingCloseProximityBehaviour, true);
                //EditorGUILayout.PropertyField(AiChargingSprintingBehaviour, true);


                //if (mytar.CombatStateBehaviours.EnableMeleeAttack == true)
                //{
                 EditorGUILayout.PropertyField(AiMeleeAttack, true);
                //}
                
                if (mytar.NonCombatBehaviours.EnableSoundAlerts == true)
                {
                    EditorGUILayout.PropertyField(AiHearing, true);
                }
                if (mytar.CombatStateBehaviours.ChooseEnemyPursueType != CoreAiBehaviour.EnemyPursueTypes.DoNotPursueEnemy)
                {
                    EditorGUILayout.PropertyField(AiPursue, true);
                }

                if (mytar.Components.HumanoidFiringBehaviourComponent != null)
                {
                    DestroyImmediate(mytar.Components.HumanoidFiringBehaviourComponent.GetComponent<HumanoidAiWeaponFiringBehaviour>());
                }
                if (mytar.GetComponent<HumanoidGrenadeThrower>() != null)
                {
                    DestroyImmediate(mytar.GetComponent<HumanoidGrenadeThrower>());
                }
                if (mytar.GetComponent<AiFollower>() != null)
                {
                    DestroyImmediate(mytar.GetComponent<AiFollower>());
                }
                if (mytar.GetComponent<AiGuide>() != null)
                {
                    DestroyImmediate(mytar.GetComponent<AiGuide>());
                }
                if (mytar.GetComponent<SpineRotation>() != null)
                {
                    DestroyImmediate(mytar.GetComponent<SpineRotation>());
                }
            }

            if (mytar.CombatStateBehaviours.UseImpactAnimations == true)
            {
                if (mytar.GetComponent<HumanoidBodyHits>() == null)
                {
                    mytar.gameObject.AddComponent<HumanoidBodyHits>();
                }
                else if (mytar.GetComponent<HumanoidBodyHits>() != null)
                {
                    mytar.GetComponent<HumanoidBodyHits>().enabled = true;
                }
            }
            else if (mytar.GetComponent<HumanoidBodyHits>() != null)
            {
                mytar.GetComponent<HumanoidBodyHits>().enabled = false;
            }

            if (mytar.NonCombatBehaviours.EnableHeadRotations == true)
            {
                if (mytar.GetComponent<HeadIK>() == null)
                {
                    mytar.gameObject.AddComponent<HeadIK>();
                }
                else if (mytar.GetComponent<HeadIK>() != null)
                {
                    mytar.GetComponent<HeadIK>().enabled = true;
                }
            }
            else if (mytar.GetComponent<HeadIK>() != null)
            {
                mytar.GetComponent<HeadIK>().enabled = false;
            }


            if (mytar.NonCombatBehaviours.DefaultBehaviour == CoreAiBehaviour.InvestigationTypes.Scan ||
                mytar.CombatStateBehaviours.ChooseEnemyPursueType == CoreAiBehaviour.EnemyPursueTypes.EnableApproachingEnemyPursue
                || mytar.CombatStateBehaviours.ChooseEnemyPursueType == CoreAiBehaviour.EnemyPursueTypes.EnableStationedEnemyPursue
                || mytar.CombatStateBehaviours.EnablePostCombatScan == true)
            {
                if (mytar.GetComponent<ScanningScript>() == null)
                {
                    mytar.gameObject.AddComponent<ScanningScript>();
                }
                else if (mytar.GetComponent<ScanningScript>() != null)
                {
                    mytar.GetComponent<ScanningScript>().enabled = true;
                }
            }
            else if (mytar.GetComponent<ScanningScript>() != null)
            {
                mytar.GetComponent<ScanningScript>().enabled = false;
            }

            if (mytar.NonCombatBehaviours.DefaultBehaviour == CoreAiBehaviour.InvestigationTypes.Patrol)
            {
                if (mytar.GetComponent<Patrolling>() == null)
                {
                    mytar.gameObject.AddComponent<Patrolling>();
                }
                else if (mytar.GetComponent<Patrolling>() != null)
                {
                    mytar.GetComponent<Patrolling>().enabled = true;
                }
            }
            else if (mytar.GetComponent<Patrolling>() != null)
            {
                mytar.GetComponent<Patrolling>().enabled = false;
            }

            if (mytar.NonCombatBehaviours.DefaultBehaviour == CoreAiBehaviour.InvestigationTypes.Wander)
            {
                if (mytar.GetComponent<Wandering>() == null)
                {
                    mytar.gameObject.AddComponent<Wandering>();
                }
                else if (mytar.GetComponent<Wandering>() != null)
                {
                    mytar.GetComponent<Wandering>().enabled = true;
                }
            }
            else if (mytar.GetComponent<Wandering>() != null)
            {
                mytar.GetComponent<Wandering>().enabled = false;
            }




            // EditorGUILayout.PropertyField(Cube);



            mytar.OverrallProbabilityPercentage();
            serializedObject.ApplyModifiedProperties();


            //    case CoreAiBehaviour.AiTypes.StationaryBot:






            //        if (EditorGUI.EndChangeCheck())
            //        {
            //            sotar.ApplyModifiedProperties();
            //            GUI.FocusControl(null);
            //        }
            //        EditorGUI.BeginChangeCheck();


            //        switch (mytar.CurrentTab)
            //        {
            //            case "INFO":
            //                EditorGUILayout.PropertyField(ScriptInfo);
            //                EditorGUILayout.PropertyField(CurrentState);
            //                EditorGUILayout.PropertyField(DebugRaycastToTarget);
            //                EditorGUILayout.PropertyField(DebugRaycastColor);
            //                break;

            //            case "COMPONENTS":

            //                EditorGUILayout.PropertyField(HumanoidFiringBehaviourComponent);
            //                break;

            //            case "ANIMATIONS":
            //                EditorGUILayout.PropertyField(AimingAnimationName);
            //                EditorGUILayout.PropertyField(FireAnimationName);
            //                EditorGUILayout.PropertyField(ReloadAnimationName);
            //                EditorGUILayout.PropertyField(IdleAnimationName);
            //                break;

            //            case "DETECTION":
            //                EditorGUILayout.PropertyField(DetectionRange);
            //                EditorGUILayout.PropertyField(EnableFieldOfView);
            //                EditorGUILayout.PropertyField(FieldOfViewValue);
            //                EditorGUILayout.PropertyField(MinTimeToCheckForClosestEnemies);
            //                EditorGUILayout.PropertyField(MaxTimeToCheckForClosestEnemies);                        
            //                break;

            //            case "SPEEDS":
            //                EditorGUILayout.PropertyField(AiRotationSpeed);
            //                EditorGUILayout.PropertyField(RunSpeed);
            //                EditorGUILayout.PropertyField(WalkingBackwardSpeed);
            //                EditorGUILayout.PropertyField(WalkingLeftSpeed);
            //                EditorGUILayout.PropertyField(WalkingRightSpeed);
            //                break;

            //            case "STRAFE":
            //                EditorGUILayout.PropertyField(EnableStrafingWhileShooting);
            //                EditorGUILayout.PropertyField(EnableCustomStrafeDirections);
            //                EditorGUILayout.PropertyField(CustomStrafeDirections, true);
            //                EditorGUILayout.PropertyField(CustomStrafeDirectionRadius);
            //                EditorGUILayout.PropertyField(MinimumShootingStrafeRange);
            //                EditorGUILayout.PropertyField(MaximumShootingStrafeRange);
            //                EditorGUILayout.PropertyField(MinimumTimeBetweenShootingStrafes);
            //                EditorGUILayout.PropertyField(MaximumTimeBetweenShootingStrafes);
            //                EditorGUILayout.PropertyField(TimeToStayAlertAfterEnemyLost);
            //                break;

            //            case "HEARING":
            //                EditorGUILayout.PropertyField(EnableHearing);
            //                EditorGUILayout.PropertyField(HearingRange);
            //                EditorGUILayout.PropertyField(ErrorSoundPercentage);
            //                EditorGUILayout.PropertyField(TimeToStayAlertAfterSound);
            //                break;
            //        }


            //        if (EditorGUI.EndChangeCheck())
            //        {
            //            sotar.ApplyModifiedProperties();
            //        }

            //        break;

            //    case CoreAiBehaviour.AiTypes.WaypointBot:



            //        if (EditorGUI.EndChangeCheck())
            //        {
            //            sotar.ApplyModifiedProperties();
            //            GUI.FocusControl(null);
            //        }
            //        EditorGUI.BeginChangeCheck();


            //        switch (mytar.CurrentTab)
            //        {
            //            case "INFO":
            //                EditorGUILayout.PropertyField(ScriptInfo);
            //                EditorGUILayout.PropertyField(CurrentState);
            //                EditorGUILayout.PropertyField(DebugRaycastToTarget);
            //                EditorGUILayout.PropertyField(DebugRaycastColor);
            //                EditorGUILayout.PropertyField(CurrentWayPointTransform);
            //                break;

            //            case "COMPONENTS":
            //                EditorGUILayout.PropertyField(NavMeshAgentComponent);
            //                EditorGUILayout.PropertyField(HumanoidFiringBehaviourComponent);
            //                break;

            //            case "ANIMATIONS":
            //                EditorGUILayout.PropertyField(AimingAnimationName);
            //                EditorGUILayout.PropertyField(FireAnimationName);
            //                EditorGUILayout.PropertyField(ReloadAnimationName);
            //                EditorGUILayout.PropertyField(IdleAnimationName);
            //                EditorGUILayout.PropertyField(RunAnimationName);
            //                EditorGUILayout.PropertyField(WalkBackAnimationName);
            //                EditorGUILayout.PropertyField(WalkRightAnimationName);
            //                EditorGUILayout.PropertyField(WalkLeftAnimationName);
            //                EditorGUILayout.PropertyField(SprintingAnimationName);
            //                break;

            //            case "DETECTION":
            //                EditorGUILayout.PropertyField(DetectionRange);
            //                EditorGUILayout.PropertyField(EnableFieldOfView);
            //                EditorGUILayout.PropertyField(FieldOfViewValue);
            //                EditorGUILayout.PropertyField(MinTimeToCheckForClosestEnemies);
            //                EditorGUILayout.PropertyField(MaxTimeToCheckForClosestEnemies);
            //                break;

            //            case "SPEEDS":
            //                EditorGUILayout.PropertyField(AiRotationSpeed);
            //                EditorGUILayout.PropertyField(RunSpeed);
            //                EditorGUILayout.PropertyField(SprintSpeed);
            //                EditorGUILayout.PropertyField(WalkingBackwardSpeed);
            //                EditorGUILayout.PropertyField(WalkingLeftSpeed);
            //                EditorGUILayout.PropertyField(WalkingRightSpeed);
            //                break;

            //            case "STRAFE":
            //                EditorGUILayout.PropertyField(EnableStrafingWhileShooting);
            //                EditorGUILayout.PropertyField(EnableCustomStrafeDirections);
            //                EditorGUILayout.PropertyField(CustomStrafeDirections, true);
            //                EditorGUILayout.PropertyField(CustomStrafeDirectionRadius);
            //                EditorGUILayout.PropertyField(MinimumShootingStrafeRange);
            //                EditorGUILayout.PropertyField(MaximumShootingStrafeRange);
            //                EditorGUILayout.PropertyField(MinimumTimeBetweenShootingStrafes);
            //                EditorGUILayout.PropertyField(MaximumTimeBetweenShootingStrafes);
            //                EditorGUILayout.PropertyField(MinimumStrafeRange);
            //                EditorGUILayout.PropertyField(MaximumStrafeRange);
            //                EditorGUILayout.PropertyField(MinimumTimeBetweenStrafes);
            //                EditorGUILayout.PropertyField(MaximumTimeBetweenStrafes);
            //                break;

            //            case "HEARING":
            //                EditorGUILayout.PropertyField(EnableHearing);
            //                EditorGUILayout.PropertyField(HearingRange);
            //                EditorGUILayout.PropertyField(RandomiseHearing);
            //                EditorGUILayout.PropertyField(ErrorSoundPercentage);
            //                EditorGUILayout.PropertyField(MinDistanceToSprintAfterSound);
            //                EditorGUILayout.PropertyField(MaxDistanceToSprintAfterSound);
            //                break;

            //            case "EMERGENCY":
            //                EditorGUILayout.PropertyField(InvestigationRadiusFromTheDeadBody);
            //                EditorGUILayout.PropertyField(TimeToStayNearDeadBody);
            //                EditorGUILayout.PropertyField(EmergencySprintTimerIfNoCoverFound);
            //                EditorGUILayout.PropertyField(EmergencyRadiusIfNoCoverFound);
            //                EditorGUILayout.PropertyField(RangeToFindEmergencyCover);
            //                EditorGUILayout.PropertyField(MinimumTimeToStayInEmergency);
            //                EditorGUILayout.PropertyField(MaximumTimeToStayInEmergency);
            //                EditorGUILayout.PropertyField(MinimumTimeBetweenEmergencyCover);
            //                EditorGUILayout.PropertyField(MaximumTimeBetweenEmergencyCover);
            //                EditorGUILayout.PropertyField(EmergencyAlert);
            //                EditorGUILayout.PropertyField(InvestigationAlertRadius);
            //                break;

            //            case "WAYPOINTS":
            //                EditorGUILayout.PropertyField(ChooseRandomWayPoints);
            //                EditorGUILayout.PropertyField(RangeToFindWayPoint);
            //                EditorGUILayout.PropertyField(MinimumTimeBetweenWaypoints);
            //                EditorGUILayout.PropertyField(MaximumTimeBetweenWaypoints);
            //                break;

            //            case "GUARD":
            //                EditorGUILayout.PropertyField(MaintainDistanceWithCommander);
            //                EditorGUILayout.PropertyField(DistanceToSprintToCommanderIfHeIsFurtherThan);
            //                break;
            //        }


            //        if (EditorGUI.EndChangeCheck())
            //        {
            //            sotar.ApplyModifiedProperties();
            //        }

            //        break;

            //    case CoreAiBehaviour.AiTypes.CoverBot:



            //        if (EditorGUI.EndChangeCheck())
            //        {
            //            sotar.ApplyModifiedProperties();
            //            GUI.FocusControl(null);
            //        }
            //        EditorGUI.BeginChangeCheck();


            //        switch (mytar.CurrentTab)
            //        {
            //            case "INFO":
            //                EditorGUILayout.PropertyField(ScriptInfo);
            //                EditorGUILayout.PropertyField(CurrentState);
            //                EditorGUILayout.PropertyField(DebugRaycastToTarget);
            //                EditorGUILayout.PropertyField(DebugRaycastColor);
            //                EditorGUILayout.PropertyField(CurrentCoverPointTransform);
            //                break;

            //            case "COMPONENTS":
            //                EditorGUILayout.PropertyField(NavMeshAgentComponent);
            //                EditorGUILayout.PropertyField(HumanoidFiringBehaviourComponent);
            //                break;

            //            case "ANIMATIONS":
            //                EditorGUILayout.PropertyField(AimingAnimationName);
            //                EditorGUILayout.PropertyField(FireAnimationName);
            //                EditorGUILayout.PropertyField(ReloadAnimationName);
            //                EditorGUILayout.PropertyField(IdleAnimationName);
            //                EditorGUILayout.PropertyField(RunAnimationName);
            //                EditorGUILayout.PropertyField(SprintingAnimationName);
            //                EditorGUILayout.PropertyField(StandCoverLeftAnimationName);
            //                EditorGUILayout.PropertyField(StandCoverRightAnimationName);
            //                EditorGUILayout.PropertyField(CoverFireAnimationName);
            //                EditorGUILayout.PropertyField(CoverReloadAnimationName);
            //                EditorGUILayout.PropertyField(DefaultStateAnimationName);
            //                break;

            //            case "DETECTION":
            //                EditorGUILayout.PropertyField(DetectionRange);
            //                EditorGUILayout.PropertyField(EnableFieldOfView);
            //                EditorGUILayout.PropertyField(FieldOfViewValue);
            //                EditorGUILayout.PropertyField(MinTimeToCheckForClosestEnemies);
            //                EditorGUILayout.PropertyField(MaxTimeToCheckForClosestEnemies);
            //                break;

            //            case "SPEEDS":
            //                EditorGUILayout.PropertyField(AiRotationSpeed);
            //                EditorGUILayout.PropertyField(RunSpeed);
            //                EditorGUILayout.PropertyField(SprintSpeed);
            //                EditorGUILayout.PropertyField(WalkingBackwardSpeed);
            //                EditorGUILayout.PropertyField(WalkingLeftSpeed);
            //                EditorGUILayout.PropertyField(WalkingRightSpeed);
            //                break;

            //            case "STRAFE":
            //                EditorGUILayout.PropertyField(EnableStrafingWhileShooting);
            //                EditorGUILayout.PropertyField(EnableCustomStrafeDirections);
            //                EditorGUILayout.PropertyField(CustomStrafeDirections, true);
            //                EditorGUILayout.PropertyField(CustomStrafeDirectionRadius);
            //                EditorGUILayout.PropertyField(MinimumShootingStrafeRange);
            //                EditorGUILayout.PropertyField(MaximumShootingStrafeRange);
            //                EditorGUILayout.PropertyField(MinimumTimeBetweenShootingStrafes);
            //                EditorGUILayout.PropertyField(MaximumTimeBetweenShootingStrafes);
            //                EditorGUILayout.PropertyField(MinimumStrafeRange);
            //                EditorGUILayout.PropertyField(MaximumStrafeRange);
            //                EditorGUILayout.PropertyField(MinimumTimeBetweenStrafes);
            //                EditorGUILayout.PropertyField(MaximumTimeBetweenStrafes);
            //                break;

            //            case "HEARING":
            //                EditorGUILayout.PropertyField(EnableHearing);
            //                EditorGUILayout.PropertyField(HearingRange);
            //                EditorGUILayout.PropertyField(RandomiseHearing);
            //                EditorGUILayout.PropertyField(ErrorSoundPercentage);
            //                EditorGUILayout.PropertyField(MinDistanceToSprintAfterSound);
            //                EditorGUILayout.PropertyField(MaxDistanceToSprintAfterSound);
            //                break;

            //            case "EMERGENCY":
            //                EditorGUILayout.PropertyField(InvestigationRadiusFromTheDeadBody);
            //                EditorGUILayout.PropertyField(TimeToStayNearDeadBody);
            //                EditorGUILayout.PropertyField(EmergencySprintTimerIfNoCoverFound);
            //                EditorGUILayout.PropertyField(EmergencyRadiusIfNoCoverFound);
            //                EditorGUILayout.PropertyField(RangeToFindEmergencyCover);
            //                EditorGUILayout.PropertyField(MinimumTimeToStayInEmergency);
            //                EditorGUILayout.PropertyField(MaximumTimeToStayInEmergency);
            //                EditorGUILayout.PropertyField(MinimumTimeBetweenEmergencyCover);
            //                EditorGUILayout.PropertyField(MaximumTimeBetweenEmergencyCover);
            //                EditorGUILayout.PropertyField(EmergencyAlert);
            //                EditorGUILayout.PropertyField(InvestigationAlertRadius);
            //                break;

            //            case "COVER":

            //                EditorGUILayout.PropertyField(ContinouslyTakeCovers);
            //                EditorGUILayout.PropertyField(RangeToFindCoverPoint);
            //                EditorGUILayout.PropertyField(MinimumTimeBetweenCovers);
            //                EditorGUILayout.PropertyField(MaximumTimeBetweenCovers);
            //                break;

            //            case "GUARD":
            //                EditorGUILayout.PropertyField(MaintainDistanceWithCommander);
            //                EditorGUILayout.PropertyField(DistanceToSprintToCommanderIfHeIsFurtherThan);
            //                break;
            //        }


            //        if (EditorGUI.EndChangeCheck())
            //        {
            //            sotar.ApplyModifiedProperties();
            //        }

            //        break;

            //    case CoreAiBehaviour.AiTypes.ChargeBot:



            //        if (EditorGUI.EndChangeCheck())
            //        {
            //            sotar.ApplyModifiedProperties();
            //            GUI.FocusControl(null);
            //        }
            //        EditorGUI.BeginChangeCheck();


            //        switch (mytar.CurrentTab)
            //        {
            //            case "INFO":
            //                EditorGUILayout.PropertyField(ScriptInfo);
            //                EditorGUILayout.PropertyField(CurrentState);
            //                EditorGUILayout.PropertyField(DebugRaycastToTarget);
            //                EditorGUILayout.PropertyField(DebugRaycastColor);
            //                break;

            //            case "COMPONENTS":
            //                EditorGUILayout.PropertyField(NavMeshAgentComponent);
            //                EditorGUILayout.PropertyField(HumanoidFiringBehaviourComponent);
            //                break;

            //            case "ANIMATIONS":
            //                EditorGUILayout.PropertyField(AimingAnimationName);
            //                EditorGUILayout.PropertyField(FireAnimationName);
            //                EditorGUILayout.PropertyField(ReloadAnimationName);
            //                EditorGUILayout.PropertyField(IdleAnimationName);
            //                EditorGUILayout.PropertyField(RunAnimationName);
            //                EditorGUILayout.PropertyField(SprintingAnimationName);
            //                break;

            //            case "DETECTION":
            //                EditorGUILayout.PropertyField(DetectionRange);
            //                EditorGUILayout.PropertyField(EnableFieldOfView);
            //                EditorGUILayout.PropertyField(FieldOfViewValue);
            //                EditorGUILayout.PropertyField(MinTimeToCheckForClosestEnemies);
            //                EditorGUILayout.PropertyField(MaxTimeToCheckForClosestEnemies);
            //                break;

            //            case "SPEEDS":
            //                EditorGUILayout.PropertyField(AiRotationSpeed);
            //                EditorGUILayout.PropertyField(RunSpeed);
            //                EditorGUILayout.PropertyField(SprintSpeed);
            //                EditorGUILayout.PropertyField(WalkingBackwardSpeed);
            //                EditorGUILayout.PropertyField(WalkingLeftSpeed);
            //                EditorGUILayout.PropertyField(WalkingRightSpeed);
            //                break;

            //            case "STRAFE":
            //                EditorGUILayout.PropertyField(EnableStrafingWhileShooting);
            //                EditorGUILayout.PropertyField(EnableCustomStrafeDirections);
            //                EditorGUILayout.PropertyField(CustomStrafeDirections, true);
            //                EditorGUILayout.PropertyField(CustomStrafeDirectionRadius);
            //                EditorGUILayout.PropertyField(MinimumShootingStrafeRange);
            //                EditorGUILayout.PropertyField(MaximumShootingStrafeRange);
            //                EditorGUILayout.PropertyField(MinimumTimeBetweenShootingStrafes);
            //                EditorGUILayout.PropertyField(MaximumTimeBetweenShootingStrafes);
            //                EditorGUILayout.PropertyField(MinimumStrafeRange);
            //                EditorGUILayout.PropertyField(MaximumStrafeRange);
            //                EditorGUILayout.PropertyField(MinimumTimeBetweenStrafes);
            //                EditorGUILayout.PropertyField(MaximumTimeBetweenStrafes);
            //                break;

            //            case "HEARING":
            //                EditorGUILayout.PropertyField(EnableHearing);
            //                EditorGUILayout.PropertyField(HearingRange);
            //                EditorGUILayout.PropertyField(RandomiseHearing);
            //                EditorGUILayout.PropertyField(ErrorSoundPercentage);
            //                EditorGUILayout.PropertyField(MinDistanceToSprintAfterSound);
            //                EditorGUILayout.PropertyField(MaxDistanceToSprintAfterSound);
            //                break;

            //            case "EMERGENCY":
            //                EditorGUILayout.PropertyField(InvestigationRadiusFromTheDeadBody);
            //                EditorGUILayout.PropertyField(TimeToStayNearDeadBody);
            //                EditorGUILayout.PropertyField(EmergencySprintTimerIfNoCoverFound);
            //                EditorGUILayout.PropertyField(EmergencyRadiusIfNoCoverFound);
            //                EditorGUILayout.PropertyField(RangeToFindEmergencyCover);
            //                EditorGUILayout.PropertyField(MinimumTimeToStayInEmergency);
            //                EditorGUILayout.PropertyField(MaximumTimeToStayInEmergency);
            //                EditorGUILayout.PropertyField(MinimumTimeBetweenEmergencyCover);
            //                EditorGUILayout.PropertyField(MaximumTimeBetweenEmergencyCover);
            //                EditorGUILayout.PropertyField(EmergencyAlert);
            //                EditorGUILayout.PropertyField(InvestigationAlertRadius);
            //                break;

            //            case "CHARGE":
            //                EditorGUILayout.PropertyField(MinimumDistanceToCharge);
            //                EditorGUILayout.PropertyField(MaximumDistanceToCharge);
            //                EditorGUILayout.PropertyField(MinimumStoppingDistance);
            //                EditorGUILayout.PropertyField(MaximumStoppingDistance);
            //                break;

            //            case "GUARD":
            //                EditorGUILayout.PropertyField(MaintainDistanceWithCommander);
            //                EditorGUILayout.PropertyField(DistanceToSprintToCommanderIfHeIsFurtherThan);
            //                break;
            //        }


            //        if (EditorGUI.EndChangeCheck())
            //        {
            //            sotar.ApplyModifiedProperties();
            //        }
            //        break;

            //}


        }
    }
}