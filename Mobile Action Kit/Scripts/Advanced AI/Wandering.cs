using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class Wandering : MonoBehaviour
    {

        [TextArea]
        public string ScriptInfo = "This Script allows developers to set up unpredictable traveling pattern on the level for Ai agent.  ";

        [Tooltip("If checked than gizmos will appear in the scene view with the exact wandering area.")]
        public bool ShowGizmoInScene;

        [Tooltip("Set the color of the wandering area gizmos in the unity scene view.")]
        public Color GizmoColor = Color.red;

        public float DebugWanderingRadius;

        [Tooltip("Drag and drop 'CoreAiBehaviour Script' component attached with this gameobject from the hierarchy into this field.")] 
        public CoreAiBehaviour CoreAiBehaviourScript;
        [Tooltip("Drag and drop 'Animator' component attached with this gameobject from the hierarchy into this field.")]
        public Animator AnimatorComponent;
        [Tooltip("Drag and drop 'AiPathFinder Script' attached with this gameobject from the hierarchy into this field.")]
        public AiPathFinder PathFinder;
        [Tooltip("Minimum time between  wandering points creation on the navmesh.")]
        public float MinTimeBetweenWanderPoints;
        [Tooltip("Maximum time between wandering points creation on the navmesh.")]
        public float MaxTimeBetweenWanderPoints;

        [Tooltip("If checked then will ensure that Ai agent will wander within certain area of interest that misssion designer will set for this Ai agent to roam around without eventually leaving this area.")]
        public bool StoreInitialPosition = false;

        [Tooltip("Minimum possible radius for a new wandering point creation on the navmesh." +
            "Note that the Wandering point cannot be created closer than the minimal radius.")]
        public float MinWanderingRadius = 100f;
        [Tooltip("Maximum possible radius for a new wandering point creation on the navmesh." +
            "Note that the Wandering point cannot be created farther than the maximal radius.")]
        public float MaxWanderingRadius = 100f;

        //[Tooltip("Set the minimum time the AI agent waits at the current wander point before creating a new one. " +
        //    "EXAMPLE - If the Min / MaxTimeBetweenWanderPoints is small, like 2 - 3seconds, and the generated wandering point is further than these 2 - 3 seconds allow agent to travel to reach it, " +
        //    "then the designated 'TimeAtWanderingPoint' will not be utilised because Ai agent will not have a chance to reach that point in the first place. ")]
        //public float MinTimeAtWanderingPoint = 2f;
        //[Tooltip("Set the maximum time the AI agent waits at the current wander point on the navmesh before creating a new one. " +
        //    "EXAMPLE - If the Min / MaxTimeBetweenWanderPoints is short, like 2 - 3seconds, and the generated wandering point is further than these 2 - 3 seconds allow agent to travel to reach it, " +
        //    "then the designated 'TimeAtWanderingPoint' will not be utilised because Ai agent will not have a chance to reach that point in the first place. ")]
        //public float MaxTimeAtWanderingPoint = 5f;

 
        string WalkIdleParametreName = "WalkIdle";
        string WalkIdleSpeedParameterName = "WalkIdleAnimationSpeed";

        [Tooltip("Specify the walking animation speed during wandering behaviour. i.e. the speed of the playback of the animation clip.")]
        public float WanderingAnimationSpeed = 1f;
        //[Tooltip("Copy and paste the Patrolling Animation clip name from the Animator Component into this field.")]
        //public string WanderingAnimationClipName = "Rifle Walk";
        [Tooltip("The minimum distance to stop near the wandering point and to consider it as reached.")]
        public float MinimumStoppingDistance = 1f;
        [Tooltip("The maximum distance to stop near the wandering point and to consider it as reached.")]
        public float MaximumStoppingDistance = 2f;

        float SaveTime;
        Vector3 wanderpoint;
        float Timer;
        float StoppingDistance;

       // bool IsStopping = false;

        float TimeToFindNewPointToMove;
        float RadiusToWanderingAround = 0.1f;
        Vector3 StoredPosition;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (ShowGizmoInScene == true)
            {
                if(StoreInitialPosition == true)
                {
                    Gizmos.color = GizmoColor;
                    Gizmos.DrawWireSphere(StoredPosition, RadiusToWanderingAround);
                }
                else
                {
                    Gizmos.color = GizmoColor;
                    Gizmos.DrawWireSphere(transform.position, RadiusToWanderingAround);
                }
             
            }
           
        }
#endif
        void Awake()
        {
            StoredPosition = transform.position;
            TimeToFindNewPointToMove = Random.Range(MinTimeBetweenWanderPoints, MaxTimeBetweenWanderPoints);           
            RadiusToWanderingAround = Random.Range(MinWanderingRadius, MaxWanderingRadius);
            DebugWanderingRadius = RadiusToWanderingAround;
            SaveTime = TimeToFindNewPointToMove;
            TimeToFindNewPointToMove = 0f;
            StoppingDistance = Random.Range(MinimumStoppingDistance, MaximumStoppingDistance);
          
        }
        void Start()
        {
            if(CoreAiBehaviourScript.AgentRole == CoreAiBehaviour.Role.Zombie)
            {
                CoreAiBehaviourScript.PatrollingAnimName = "WalkForward";
                WalkIdleParametreName = "WalkForward";
                WalkIdleSpeedParameterName = "WalkForwardAnimationSpeed";
            }
            else
            {
                CoreAiBehaviourScript.PatrollingAnimName = WalkIdleParametreName;
            }
            
        }
        public void Wander()
        {
            if (CoreAiBehaviourScript.HealthScript.IsDied == false && PathFinder.PauseTheNavMeshSearching == false)
            { 
                if (CoreAiBehaviourScript.SearchingForSound == false && CoreAiBehaviourScript.IsAgentRoleLeader == false)
                {
                    CoreAiBehaviourScript.StopSpineRotation = true;
                    Timer += Time.deltaTime;

                    CoreAiBehaviourScript.ReachnewCoverpoint = false;
                    AiWander();
                }
            }
        }
        public void AiWander()
        {
            CoreAiBehaviourScript.enableIkupperbodyRotations(ref CoreAiBehaviourScript.ActivateWalkIk);

            //if (CoreAiBehaviourScript.Components.HumanoidFiringBehaviourComponent != null)
            //{
            //    StopCoroutine(CoreAiBehaviourScript.Components.HumanoidFiringBehaviourComponent.ReloadTimer());
            //}
            if (Timer >= TimeToFindNewPointToMove)
            {
                //IsStopping = false;
                CreatingAWanderPoint();
                //PathFinder.CalculatePathForCombat(0f,
                //    0f, patrolpoint);
                //if (PathFinder.IsPathComplete == true)
                //{
                TimeToFindNewPointToMove = SaveTime;
                Timer = 0;
                //}
                //else
                //{
                //    AnimatorComponent.SetBool(CoreAiBehaviourScript.AiAgentAnimatorParameters.DefaultStateParameterName, true);
                //    AnimatorComponent.SetBool(CoreAiBehaviourScript.AiAgentAnimatorParameters.IdleParameterName, true);
                //    AnimatorComponent.SetBool(WalkIdleParametreName, false);
                //    CoreAiBehaviourScript.AnimController(true, 0f, CoreAiBehaviourScript.AiAgentAnimatorParameters.DefaultStateParameterName, true, false);
                //}
            }
            else
            {
                if (Vector3.Distance(PathFinder.closestPoint, transform.position) < StoppingDistance)
                {
                    AnimatorComponent.SetBool(CoreAiBehaviourScript.AiAgentAnimatorParameters.IdleParameterName, true);
                    AnimatorComponent.SetBool(WalkIdleParametreName, false);
                    CoreAiBehaviourScript.AnimController(true, 0f, CoreAiBehaviourScript.AiAgentAnimatorParameters.DefaultStateParameterName, true, false);
                    //if (IsStopping == false)
                    //{
                    //    StartCoroutine(IsMovable());
                    //    IsStopping = true;
                    //}
                }
                else
                {
                    PathFinder.FindClosestPointTowardsDestination(wanderpoint);
                    if (CoreAiBehaviourScript.Components.NavMeshAgentComponent.enabled == true)
                    {
                        CoreAiBehaviourScript.Components.NavMeshAgentComponent.SetDestination(PathFinder.closestPoint);
                    }
                    CoreAiBehaviourScript.SetAnimationForFullBody(WalkIdleParametreName);
                    AnimatorComponent.SetFloat(WalkIdleSpeedParameterName, WanderingAnimationSpeed);
                    CoreAiBehaviourScript.AnimController(false, CoreAiBehaviourScript.Speeds.MovementSpeeds.WalkForwardSpeed, CoreAiBehaviourScript.AiAgentAnimatorParameters.DefaultStateParameterName, true, false);
                }
            }
           // Debug.DrawLine(transform.position, patrolpoint, Color.red);

        }
        public void CreatingAWanderPoint()
        {
            if (StoreInitialPosition == false)
            {
                RadiusToWanderingAround = Random.Range(MinWanderingRadius, MaxWanderingRadius);
                DebugWanderingRadius = RadiusToWanderingAround;
                wanderpoint = GenerateRandomNavmeshLocation.RandomLocation(transform, RadiusToWanderingAround);
            }
            else
            {
                wanderpoint = GenerateRandomNavmeshLocation.RandomLocationInVector3(StoredPosition, RadiusToWanderingAround);
            }
            PathFinder.FindClosestPointTowardsDestination(wanderpoint);
            if (CoreAiBehaviourScript.Components.NavMeshAgentComponent.enabled == true)
            {
                CoreAiBehaviourScript.Components.NavMeshAgentComponent.SetDestination(PathFinder.closestPoint);
            } 
        }
        //IEnumerator IsMovable()
        //{
        //    float RandomiseWaiting = Random.Range(MinTimeAtWanderingPoint, MaxTimeAtWanderingPoint);
        //    yield return new WaitForSeconds(RandomiseWaiting);
        //    TimeToFindNewPointToMove = 0f;
        //}
    }
}