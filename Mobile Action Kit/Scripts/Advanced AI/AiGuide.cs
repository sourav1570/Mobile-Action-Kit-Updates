using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This Script will Make The Ai Bot To Go To Predefined Destinations Taking The Player Making Sure He will behave as Commander
namespace MobileActionKit
{
    public class AiGuide : MonoBehaviour
    {

        [TextArea]
        // [ReadOnly]
        public string ScriptInfo = "This script turns Ai agent to a leader of Player and/or other AI agents, that guides player and/or other AI agents to certain destination point.";

        [Tooltip("Ai Leader will go through destination points in the order they are listed below.Starting from Point in 'Element 0' field." +
            "If Ai Leader encounters any resistance on his way than he will switch to other behaviours depending on the state those situations might put him into." +
            "As soon as all interrupting factors are eliminated and  Ai Leader is no longer distracted then he will resume his destination following behaviour." +
            "After reaching last destination point Leader will switch to its default behaviour(wandering, patrolling etc.)")]
        public Transform[] DestinationPoints;

        [Tooltip("Drag and drop 'PlayerFollower' component attached to Player from the hierarchy into this field. " +
            "Adding 'PlayerFollower' component will allow this Ai agent to become the leader of the Player and not get too far from the player while moving towards the destination.")]
        public PlayerFollower PlayerFollowerScript;

        [Tooltip("Drag and drop into this field one or multiple Ai agents from the hierarchy window(with their role set as the 'Follower' inside the 'core Ai behaviour' script). " +
            "This functionality is designed to work with AI agents that are pre-placed in the level by  mission designer.It is not intended to work with spawned AI agents.")]
        public List<AiFollower> FollowerAgents = new List<AiFollower>();

        //[Tooltip("Tag of the  player should be written in this field in order for the Ai Guide to wait for Player to catch up if Ai Guide got too far ahead pass certain distance specified in fields below." +
        //    "It will make Ai Guide stop and wait for the player to get within specified range again.")]
        //public string PlayerTag = "Player";

        [Tooltip("Drag and drop this agent`s root gameobject with 'NavMeshAgent' component attached to it into this field.")]
        public NavMeshAgent NavMeshAgentComponent;

        [Tooltip("Decide Whether the Ai agent will sprint towards the destination points or only do running between destination points")]
        public bool SprintToDestination = true;

        [Tooltip("Specifies remaining distance to destination point to switch from walking or sprinting to running towards it.")]
        public float MinRunDistance = 7f;
        [Tooltip("Specifies remaining distance to destination point to switch from walking or sprinting to running towards it.")]
        public float MaxRunDistance = 7f;

        [Tooltip("Specifies remaining distance to destination point to switch from running or sprinting to walking towards it.")]
        public float MinWalkDistance = 7f;
        [Tooltip("Specifies remaining distance to destination point to switch from running or sprinting to walking towards it.")]
        public float MaxWalkDistance = 7f;


        float RunningDistanceToDestination;
        float WalkingDistanceToDestination;

        [Tooltip("Specifies the value at which distance the Ai agent will register Destination point as reached and look for the new one (if there are more of them) - Recommended Range (1,3)")]
        public float StoppingDistanceToDestinationPoint = 2f;

        [Tooltip("Minimum Time interval To check distance with followers in case the followers are getting far behind from this Ai agent leader.")]
        public float MinTimeToCheckOnFollowers = 0.3f;
        [Tooltip("Maximum Time To check distance with followers in case the followers are getting far behind from this Ai agent leader.")]
        public float MaxTimeToCheckOnFollowers = 0.7f;

        [Tooltip("If Ai Leader exceeds the specified distance. Ai agent will stop and wait until all the followers will catch up again.")]
        public float StopAndWaitDistance = 20f;

        int ReachPoints = 0;
        GameObject Soldier;
        //bool OverwritingSprinting = false;
        private Animator anim;
        private HumanoidVisibilityChecker FOV;
        //   private MasterAiBehaviour MAB;
        private CoreAiBehaviour SAB;
        bool StopLoop = false;
        Transform Reach;

        bool allDistancesWithinMaxDistance = true;

        bool CheckDistances = false;

        bool ContinueSprinting = false;
        bool AllBecomeSoldiers = false;

        bool DisableBodyGuardBehaviour = false;

        [HideInInspector]
        public bool IsWaiting = false;

        bool IsTransitionStarted = false;

        bool StopDistanceChecks = false;

        bool AllFollowersInCombat = false;

        void Start()
        {
            // Soldier = GameObject.FindGameObjectWithTag(PlayerTag);

            anim = GetComponent<Animator>();
            // MAB = GetComponent<MasterAiBehaviour>();
            SAB = GetComponent<CoreAiBehaviour>();

            if (GetComponent<HumanoidVisibilityChecker>() != null)
            {
                FOV = GetComponent<HumanoidVisibilityChecker>();
            }

            //if(GetComponent<MasterAiBehaviour>() != null)
            //{
            //    MAB.Commanderorder = true;
            //}

            if (GetComponent<CoreAiBehaviour>() != null)
            {
                SAB.IsAgentRoleLeader = true;
            }

            if (SprintToDestination == false)
            {
                MinRunDistance = Mathf.Infinity;
                MaxRunDistance = Mathf.Infinity;
            }

            for (int x = 0; x < FollowerAgents.Count; x++)
            {
                if (FollowerAgents[x] != null)
                {
                    if (FollowerAgents[x].GetComponent<CoreAiBehaviour>() != null)
                    {
                        FollowerAgents[x].GetComponent<CoreAiBehaviour>().IsBodyguard = true;
                        FollowerAgents[x].GetComponent<CoreAiBehaviour>().BossTransform = transform;

                    }
                }
            }

            DestinationsToReachAt();
            CheckDistancesWithFollowers();
        }
        IEnumerator CoroForDistanceCheck()
        {
            float RandomValue = Random.Range(MinTimeToCheckOnFollowers, MaxTimeToCheckOnFollowers);
            yield return new WaitForSeconds(RandomValue);
            if(StopDistanceChecks == false)
            {
                CheckAliveFollowersOnly();
                CheckDistancesWithFollowers();
                CheckDistances = false;
            }
       

        }
        public void CheckAliveFollowersOnly()
        {
            for (int x = 0; x < FollowerAgents.Count; x++)
            {
                if (FollowerAgents[x] == null)
                {
                    FollowerAgents.Remove(FollowerAgents[x]);
                }
            }
        }
        // Method to check distances with followers
        public void CheckDistancesWithFollowers()
        {
            allDistancesWithinMaxDistance = true;

            if (ContinueSprinting == false)
            {
                foreach (AiFollower target in FollowerAgents)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                    if (distanceToTarget > target.RandomValueForDistanceCheck)
                    {
                        allDistancesWithinMaxDistance = false;
                        break;
                    }
                }


                if (PlayerFollowerScript != null)
                {
                    float distanceWithPlayer = Vector3.Distance(transform.position, PlayerFollowerScript.transform.position);
                    if (distanceWithPlayer > PlayerFollowerScript.RandomValueForDistanceCheck)
                    {
                        allDistancesWithinMaxDistance = false;
                    }
                }

                if (allDistancesWithinMaxDistance == true)
                {                  
                    ContinueSprinting = true;
                }
            }
            else
            {
                foreach (AiFollower target in FollowerAgents)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                    if (distanceToTarget > StopAndWaitDistance)
                    {
                        allDistancesWithinMaxDistance = false;
                        ContinueSprinting = false;
                        break;
                    }
                }

                if (PlayerFollowerScript != null)
                {
                    float distanceWithPlayer = Vector3.Distance(transform.position, PlayerFollowerScript.transform.position);
                    if (distanceWithPlayer > StopAndWaitDistance)
                    {
                        allDistancesWithinMaxDistance = false;
                        ContinueSprinting = false;
                    }
                }
            }

            if (allDistancesWithinMaxDistance == true)
            {
                if (StopDistanceChecks == false)
                {
                    if (IsWaiting == true)
                    {
                        IsWaiting = false;
                        StopLoop = false;
                        DestinationsToReachAt();
                        IsTransitionStarted = false;
                    }
                }
            }

            //foreach (Transform target in FollowersScript.FollowerAgents)
            //{
            //    allDistancesWithinCatchingDistance = false;
            //    float distanceToTarget = Vector3.Distance(transform.position, target.position);
            //    if (distanceToTarget < CatchingUpDistanceWithFollower)
            //    {
            //        allDistancesWithinCatchingDistance = true;
            //        break;
            //    }
            //}


        }
        void Update()
        {
            //if(MAB != null)
            //      {
            //          if(MAB.CombatStarted == false && MAB.SearchingForSound == false && FOV.ConnectionLost == false)
            //          {
            //              if(StopLoop == true)
            //              {
            //                  NavMeshAgentComponent.destination = Reach.position;

            //                  Vector3 DistanceFromPointToReach = transform.position - Reach.position;
            //                  Vector3 DistanceFromsoldier = transform.position - Soldier.transform.position;

            //                  if (DistanceFromsoldier.sqrMagnitude < DistanceToCheckWithSoldierWhenGoesFurther * DistanceToCheckWithSoldierWhenGoesFurther)
            //                  {
            //                      OverwritingSprinting = true;
            //                  }

            //                  if (DistanceFromsoldier.sqrMagnitude < DistanceWithSoldierToMaintain * DistanceWithSoldierToMaintain && OverwritingSprinting == true)
            //                  {
            //                      if (DistanceFromPointToReach.sqrMagnitude < StoppingDistanceFromDestination * StoppingDistanceFromDestination)
            //                      {
            //                          NavMeshAgentComponent.isStopped = true;
            //                          MAB.SetAnims(MAB.Aiming);
            //                          anim.SetBool(MAB.DefaultState, true);
            //                          StopLoop = false;
            //                          DestinationsToReachAt();
            //                      }
            //                      else if (DistanceFromPointToReach.sqrMagnitude > StoppingDistanceFromDestination * StoppingDistanceFromDestination && DistanceFromPointToReach.sqrMagnitude < DistanceToStartRunningWhenDestinationGetsCloser * DistanceToStartRunningWhenDestinationGetsCloser)
            //                      {
            //                          NavMeshAgentComponent.isStopped = false;
            //                          MAB.SetAnims(MAB.Run);
            //                          anim.SetBool(MAB.DefaultState, true);
            //                          NavMeshAgentComponent.speed = MAB.AdvancingSpeed;
            //                      }
            //                      else
            //                      {
            //                          MAB.SmoothAnimatorWeightChanger(1, 1f);
            //                          NavMeshAgentComponent.isStopped = false;
            //                          MAB.SetAnims(MAB.Sprinting);                        
            //                          NavMeshAgentComponent.speed = MAB.SprintSpeed;
            //                      }
            //                  }
            //                  else
            //                  {
            //                      OverwritingSprinting = false;
            //                      NavMeshAgentComponent.isStopped = true;
            //                      MAB.SetAnims(MAB.Idle);
            //                      anim.SetBool(MAB.DefaultState, true);
            //                  }                                
            //              }
            //          }
            //      }
            if (SAB != null)
            {
                if (SAB.HealthScript.IsDied == true || DisableBodyGuardBehaviour == true)
                {
                    if (AllBecomeSoldiers == false)
                    {
                        for (int x = 0; x < FollowerAgents.Count; x++)
                        {
                            if (FollowerAgents[x] != null)
                            {
                                if (FollowerAgents[x].GetComponent<CoreAiBehaviour>() != null)
                                {
                                    FollowerAgents[x].GetComponent<CoreAiBehaviour>().IsBodyguard = false;
                                    FollowerAgents[x].GetComponent<CoreAiBehaviour>().BossTransform = transform;
                                    FollowerAgents[x].GetComponent<CoreAiBehaviour>().AgentRole = CoreAiBehaviour.Role.Fighter;
                                }
                            }
                        }

                        //if (SAB.HealthScript.IsDied == false)
                        //{
                        //    SAB.IsBodyguard = false;
                        //    SAB.AgentRole = CoreAiBehaviour.Role.Fighter;
                        //}

                        AllBecomeSoldiers = true;
                    }
                }

                if (SAB.CombatStarted == false && SAB.SearchingForSound == false && FOV.ConnectionLost == false && SAB.HealthScript.IsDied == false && SAB.WasInCombatStateBefore == false)
                {
                    if (StopLoop == true)
                    {
                        if (AllFollowersInCombat == true)
                        {
                            for (int x = 0; x < FollowerAgents.Count; x++)
                            {
                                if(FollowerAgents[x] != null)
                                {
                                    if (FollowerAgents[x].GetComponent<CoreAiBehaviour>() != null)
                                    {
                                        //FollowerAgents[x].GetComponent<CoreAiBehaviour>().CombatStarted = false;
                                        //FollowerAgents[x].GetComponent<CoreAiBehaviour>().VisibilityCheck.ForcedCombatState = false;
                                        FollowerAgents[x].GetComponent<CoreAiBehaviour>().FindEnemiesScript.OriginalFov = FollowerAgents[x].GetComponent<CoreAiBehaviour>().SavedFov;
                                    }
                                }
                              
                            }
                            AllFollowersInCombat = false;
                        }

                        NavMeshAgentComponent.enabled = true;
                        SAB.NavMeshObstacleComponent.enabled = false;
                        SAB.Components.HumanoidFiringBehaviourComponent.FireNow = false;
                        NavMeshAgentComponent.destination = Reach.position;
                        SAB.StopSpineRotation = true;
                        Vector3 DistanceFromPointToReach = transform.position - Reach.position;
                        // Vector3 DistanceFromsoldier = transform.position - Soldier.transform.position;

                        if (CheckDistances == false)
                        {
                            StartCoroutine(CoroForDistanceCheck());
                            CheckDistances = true;
                        }

                        //if (allDistancesWithinCatchingDistance == true && OverwritingSprinting == false)
                        //{
                        // OverwritingSprinting = true;
                        //}

                        if (allDistancesWithinMaxDistance == true || ContinueSprinting == true || IsWaiting == true)
                        {
                            if (DistanceFromPointToReach.sqrMagnitude < StoppingDistanceToDestinationPoint * StoppingDistanceToDestinationPoint || IsWaiting == true)
                            {  
                                if(Reach.transform.GetComponent<AiLeaderHaltPoint>() == null)
                                {
                                    SAB.IsLeaderMoving = false;
                                    SAB.enableIkupperbodyRotations(ref SAB.ActivateScanIk);
                                    // SAB.AnimatorLayerWeightControllerScript.ChangeLayerWeight(1, 0f);
                                    NavMeshAgentComponent.isStopped = true;
                                    SAB.SetAnimationForFullBody(SAB.AiAgentAnimatorParameters.FireParameterName);
                                    anim.SetBool(SAB.AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                    StopLoop = false;
                                    DestinationsToReachAt();

                                }
                                else
                                {
                                    SAB.IsLeaderMoving = true;
                                    SAB.enableIkupperbodyRotations(ref SAB.ActivateNoIk);
                                    NavMeshAgentComponent.isStopped = true;
                                    if (Reach.transform.GetComponent<AiLeaderHaltPoint>().HaltPoseAnimation == AiLeaderHaltPoint.Cover.HaltLeft)
                                    {
                                        SAB.SetAnimationForFullBody(SAB.AiAgentAnimatorParameters.LeftCoverIdle);
                                        anim.SetBool(SAB.AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                        if (IsTransitionStarted == false)
                                        {
                                            SAB.Components.HumanoidAiAudioPlayerComponent.PlayNonRecurringSoundClips(SAB.Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceLeaderAtHaltPointAudioClips);
                                            StopDistanceChecks = true;
                                            StartCoroutine(RestTime(Reach.transform.GetComponent<AiLeaderHaltPoint>().MinHaltTime, Reach.transform.GetComponent<AiLeaderHaltPoint>().MaxHaltTime));
                                            LeanTween.rotateY(gameObject, Reach.transform.GetComponent<AiLeaderHaltPoint>().HaltPoseLeftRotation, Reach.transform.GetComponent<AiLeaderHaltPoint>().RotationalOffsetDuration);
                                            IsTransitionStarted = true;
                                        }
                                    }
                                    else if (Reach.transform.GetComponent<AiLeaderHaltPoint>().HaltPoseAnimation == AiLeaderHaltPoint.Cover.HaltRight)
                                    {
                                        if (IsTransitionStarted == false)
                                        {
                                            SAB.Components.HumanoidAiAudioPlayerComponent.PlayNonRecurringSoundClips(SAB.Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceLeaderAtHaltPointAudioClips);
                                            StopDistanceChecks = true;
                                            StartCoroutine(RestTime(Reach.transform.GetComponent<AiLeaderHaltPoint>().MinHaltTime, Reach.transform.GetComponent<AiLeaderHaltPoint>().MaxHaltTime));
                                            LeanTween.rotateY(gameObject, Reach.transform.GetComponent<AiLeaderHaltPoint>().HaltPoseRightRotation, Reach.transform.GetComponent<AiLeaderHaltPoint>().RotationalOffsetDuration);
                                            IsTransitionStarted = true;
                                        }
                                        SAB.SetAnimationForFullBody(SAB.AiAgentAnimatorParameters.RightCoverIdle);
                                        anim.SetBool(SAB.AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                    }
                                    else
                                    {
                                        if (IsTransitionStarted == false)
                                        {
                                            SAB.Components.HumanoidAiAudioPlayerComponent.PlayNonRecurringSoundClips(SAB.Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceLeaderAtHaltPointAudioClips);
                                            StopDistanceChecks = true;
                                            StartCoroutine(RestTime(Reach.transform.GetComponent<AiLeaderHaltPoint>().MinHaltTime, Reach.transform.GetComponent<AiLeaderHaltPoint>().MaxHaltTime));
                                            LeanTween.rotateY(gameObject, Reach.transform.GetComponent<AiLeaderHaltPoint>().HaltPoseNeutralRotation, Reach.transform.GetComponent<AiLeaderHaltPoint>().RotationalOffsetDuration);
                                            IsTransitionStarted = true;
                                        }
                                        SAB.StandingCoverMovement(SAB.AiAgentAnimatorParameters.StandCoverNeutralParameterName);
                                    }

                                    IsWaiting = true;
                                }
                            }
                            else if (DistanceFromPointToReach.sqrMagnitude > StoppingDistanceToDestinationPoint * StoppingDistanceToDestinationPoint
                                && DistanceFromPointToReach.sqrMagnitude < RunningDistanceToDestination * RunningDistanceToDestination
                                && DistanceFromPointToReach.sqrMagnitude > WalkingDistanceToDestination * WalkingDistanceToDestination)
                            {
                                SAB.IsLeaderMoving = true;
                                IsWaiting = false;
                                SAB.enableIkupperbodyRotations(ref SAB.ActivateRunningIk);

                                //SAB.AnimatorLayerWeightControllerScript.ChangeLayerWeight(1, 0f);
                                NavMeshAgentComponent.isStopped = false;
                                SAB.SetAnimationForFullBody(SAB.AiAgentAnimatorParameters.RunForwardParameterName);
                                SAB.ConnectWithUpperBodyAimingAnimation();
                                anim.SetBool(SAB.AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                NavMeshAgentComponent.speed = SAB.Speeds.MovementSpeeds.RunForwardSpeed;
                            }
                            else if (DistanceFromPointToReach.sqrMagnitude > StoppingDistanceToDestinationPoint * StoppingDistanceToDestinationPoint
                                && DistanceFromPointToReach.sqrMagnitude < RunningDistanceToDestination * RunningDistanceToDestination
                                && DistanceFromPointToReach.sqrMagnitude <= WalkingDistanceToDestination * WalkingDistanceToDestination)
                            {
                                SAB.IsLeaderMoving = true;
                                IsWaiting = false;
                                SAB.enableIkupperbodyRotations(ref SAB.ActivateWalkAimIk);
                                //SAB.AnimatorLayerWeightControllerScript.ChangeLayerWeight(1, 0f);
                                NavMeshAgentComponent.isStopped = false;
                                SAB.SetAnimationForFullBody(SAB.AiAgentAnimatorParameters.WalkForwardParameterName);
                                SAB.ConnectWithUpperBodyAimingAnimation();
                                anim.SetBool(SAB.AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                NavMeshAgentComponent.speed = SAB.Speeds.MovementSpeeds.WalkForwardAimingSpeed;


                            }
                            else
                            {
                                SAB.IsLeaderMoving = true;
                                IsWaiting = false;
                                SAB.enableIkupperbodyRotations(ref SAB.ActivateSprintingIk);
                                //  SAB.AnimatorLayerWeightControllerScript.ChangeLayerWeight(1, 1f);
                                NavMeshAgentComponent.isStopped = false;
                                SAB.SetAnimationForFullBody(SAB.AiAgentAnimatorParameters.SprintingParameterName);
                                NavMeshAgentComponent.speed = SAB.Speeds.MovementSpeeds.SprintSpeed;
                            }
                        }
                        else
                        {
                            if(SAB.NonCombatBehaviours.DefaultBehaviour == CoreAiBehaviour.InvestigationTypes.Patrol || SAB.NonCombatBehaviours.DefaultBehaviour == CoreAiBehaviour.InvestigationTypes.Wander
                                || SAB.NonCombatBehaviours.DefaultBehaviour == CoreAiBehaviour.InvestigationTypes.Stationary || SAB.NonCombatBehaviours.DefaultBehaviour == CoreAiBehaviour.InvestigationTypes.Scan)
                            {
                                SAB.enableIkupperbodyRotations(ref SAB.ActivateNoIk);
                                SAB.SetAnimationForFullBody(SAB.AiAgentAnimatorParameters.IdleParameterName);
                                anim.SetBool(SAB.AiAgentAnimatorParameters.DefaultStateParameterName, true);
                            }
                            SAB.IsLeaderMoving = false;
                            IsWaiting = false;
                            SAB.Components.HumanoidAiAudioPlayerComponent.PlayRecurringSoundClips(SAB.Components.HumanoidAiAudioPlayerComponent.RecurringSounds.LeaderAudioClips);
                            NavMeshAgentComponent.isStopped = true;
                            
                        }
                    }
                }
                else if(SAB.CombatStarted == true)
                {
                    if (AllFollowersInCombat == false)
                    {
                        for (int x = 0; x < FollowerAgents.Count; x++)
                        {
                            if (FollowerAgents[x] != null)
                            {
                                if (FollowerAgents[x].GetComponent<CoreAiBehaviour>() != null)
                                {
                                    //FollowerAgents[x].GetComponent<CoreAiBehaviour>().CombatStarted = true;
                                    //FollowerAgents[x].GetComponent<CoreAiBehaviour>().VisibilityCheck.ForcedCombatState = true;
                                    FollowerAgents[x].GetComponent<CoreAiBehaviour>().FindEnemiesScript.OriginalFov = 360f;
                                }
                            }
                        }
                        AllFollowersInCombat = true;
                    }
                }
            }
        }
        IEnumerator RestTime(float Min,float Max)
        {
            float Randomise = Random.Range(Min, Max);
            yield return new WaitForSeconds(Randomise);
            StopDistanceChecks = false;
            CheckDistances = false;
        }
        public void DestinationsToReachAt()
        {
            if (StopLoop == false)
            {
                if (ReachPoints < DestinationPoints.Length)
                {
                    RunningDistanceToDestination = Random.Range(MinRunDistance, MaxRunDistance);
                    WalkingDistanceToDestination = Random.Range(MinWalkDistance, MaxWalkDistance);
                    Reach = DestinationPoints[ReachPoints];
                    ++ReachPoints;
                    StopLoop = true;
                }
                else
                {
                    SAB.IsLeaderMoving = false;
                    //if(MAB != null)
                    //{
                    //    MAB.Commanderorder = false;
                    //}
                    if (SAB != null)
                    {
                        SAB.IsAgentRoleLeader = false;
                    }
                    DisableBodyGuardBehaviour = true;
                }
            }
        }
    }
}
