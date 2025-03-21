using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class Patrolling : MonoBehaviour
    {

        [TextArea]
        public string ScriptInfo = "This Script enables Ai agent to follow a predefined  path and sets various parameters of patrolling thus allowing mission designer to set up " +
            "strict and predictable rout for Ai agent or introduce more loose and less predictable path between patrol points.";

        [Tooltip("Drag and drop 'CoreAiBehaviour Script' component attached with this gameObject from the hierarchy into this field.")]
        public CoreAiBehaviour CoreAiBehaviourScript;

        [Tooltip("Drag and drop 'Patrol Points Finder' child game object with the 'Find Patrol Points' Script attached to it from the hierarchy into this field.")]
        public FindPatrolPoints PatrolPointsFinderScript;

        [System.Serializable]
        public enum ChoosePatrolPoints
        {
            ClosestPatrolPoints,
            RandomPatrolPoints
        }

        [Tooltip("Choose one of the two patrol point selection options from the drop-down list. " +
            "If 'Closest Patrol Points' option is selected then activated Ai agent will follow the specified route by going from one closest Patrol Point" +
            " to the next nearest one and thus following a certain path depending on the place of activation of patrolling Ai agent.If 'Random Patrol Points' option" +
            " is selected then Ai agent will be going between specified patrol points in no particular order.")]
        public ChoosePatrolPoints PatrolPattern;

        [Tooltip("Minimum allowed time to travel from previous patrol point to the next one.")]
        public float MinTimeBetweenPatrolPoints;
        [Tooltip("Maximum allowed time to travel from previous patrol point to the next one.")]
        public float MaxTimeBetweenPatrolPoints;

        [Tooltip("The minimum radius within which Ai agent will find a new patrol point.")]
        public float MinRadiusToFindPatrolPoints = 100f;
        [Tooltip("The maximum radius within which Ai agent will find a new patrol point.")]
        public float MaxRadiusToFindPatrolPoints = 100f;


        //[Tooltip("The radius within which Ai will move/wander towards random points created.")]
        //public float RadiusToWanderingAround = 100f;
        //[Tooltip("If enabled Ai agent will move towards the set of predefined points created in the game. For example : A guard moving from one door to another and loops between the door.")]
        //public bool UsePredefinedPointsToWander = false;

        //[Tooltip("If enabled Ai agent will automatically find all the predefined points which are active at the time when the game starts and will move between them.")]
        //public bool AutoFindPredefinedPoints = true;
        [Tooltip("Set the minimum time the AI agent waits at the current Patrol point before moving out to the next one. " +
            "If the 'Min/MaxTimeBetweenPatrolPoints' is smaller than what is needed for Ai agent to reach next Patrol Point, e.i if the next Patrol point is further away than amount of " +
            "seconds allowed for agent to travel in order to reach it, then the designated 'MinTimeAtPatrolPoint' time will not be utilised because Ai agent will not have a chance to reach that point in the first place. ")]
        public float MinTimeAtPatrolPoint = 2f;
        [Tooltip("Set the maximum time the AI agent waits at the current Patrol point before moving out to the next one. " +
            "If the 'Min/MaxTimeBetweenPatrolPoints' is smaller than what is needed for Ai agent to reach next Patrol Point, e.i if the next Patrol point is further away than amount of " +
            "seconds allowed for agent to travel in order to reach it, then the designated 'MaxTimeAtPatrolPoint' time will not be utilised because Ai agent will not have a chance to reach that point in the first place.")]
        public float MaxTimeAtPatrolPoint = 5f;

        [Tooltip("If enabled then it will make Ai agent to reverse his patrol rout after reaching last Patrol point. " +
            "If disabled then after reacing last Patrol Point Ai agent will move out to first Patrol point thus performing his patrol behaviour in a loop fashion." +
            "For example if there are 3 Patrol points and this checkbox is checked then Ai Agent will go through them as follows 1, 2, 3, 2, 1(so called ping - pong pattern)." +
            "And if disabled then the order will be 1, 2, 3, 1, 2, 3(so called loop pattern).")]
        public bool ReversePatrollingOrder = true;
        //[Tooltip("Drag and drop one or more Predefined points created in the hierarchy into this fields")]
        //public Transform[] PredefinedPoints;
        string WalkIdleParametreName = "WalkIdle";
        string WalkIdleSpeedParameterName = "WalkIdleAnimationSpeed";

        [Tooltip("Specify the walking animation speed during Patrolling behaviour. i.e. the speed of the playback of the animation clip.")]
        public float PatrollingAnimationSpeed = 1f;
       // [Tooltip("Copy and paste the Patrolling Animation clip name from the Animator Component into this field.")]
        // public string PatrollingAnimationClipName = "Rifle Walk";
        [Tooltip("The minimum distance to stop near the Patrol point and to consider it as reached.")]
        public float MinimumStoppingDistance = 1f;
        [Tooltip("The maximum distance to stop near the Patrol point and to consider it as reached.")]
        public float MaximumStoppingDistance = 2f;

        float SaveTime;
        Vector3 patrolpoint;

        [HideInInspector]
        public float Timer;

        GameObject PatrolPoint;
        int Counts;

        float StoppingDistance;
        bool StartMoving = true;

        bool SaveMyOrder = false;
        float TimeToFindNewPointToMove;

        float RadiusToFindPatrolPoints;
        bool ForceStoppedIfOnlyOnePatrolPoint = false;

        Vector3 SavedCurrentPositionForPatrolPointFinding;
        float GetAccuratePatrolPointRange;

        [HideInInspector]
        public Transform[] MyPatrolPoints;

        bool ReversePatrollingOrderTemp;

        void Start()
        {
            ReversePatrollingOrderTemp = ReversePatrollingOrder;
            if (CoreAiBehaviourScript.AgentRole == CoreAiBehaviour.Role.Zombie)
            {
                CoreAiBehaviourScript.PatrollingAnimName = "WalkForward";
                WalkIdleParametreName = "WalkForward";
                WalkIdleSpeedParameterName = "WalkForwardAnimationSpeed";
            }
            else
            {
                CoreAiBehaviourScript.PatrollingAnimName = WalkIdleParametreName;
            }
         
            RadiusToFindPatrolPoints = Random.Range(MinRadiusToFindPatrolPoints, MaxRadiusToFindPatrolPoints);

            SavedCurrentPositionForPatrolPointFinding = transform.position;
            if (PatrolPointsFinderScript.GetComponent<SphereCollider>() != null)
            {
                PatrolPointsFinderScript.GetComponent<SphereCollider>().radius = RadiusToFindPatrolPoints;
            }
            GetAccuratePatrolPointRange = RadiusToFindPatrolPoints;// / 2;

            PatrolPointsFinderScript.gameObject.SetActive(true);

            TimeToFindNewPointToMove = Random.Range(MinTimeBetweenPatrolPoints, MaxTimeBetweenPatrolPoints);
            SaveTime = TimeToFindNewPointToMove;
            StoppingDistance = Random.Range(MinimumStoppingDistance, MaximumStoppingDistance);
            SaveMyOrder = ReversePatrollingOrderTemp;

        }
        public void AiPatrol()
        {
            if (CoreAiBehaviourScript.HealthScript.IsDied == false)
            {
                Vector3 CheckforNewCover = SavedCurrentPositionForPatrolPointFinding - transform.position;

                if (CheckforNewCover.sqrMagnitude >= GetAccuratePatrolPointRange * GetAccuratePatrolPointRange)
                {
                    SavedCurrentPositionForPatrolPointFinding = transform.position;
                    PatrolPointsFinderScript.gameObject.SetActive(true);
                }

                if (PatrolPointsFinderScript.IsFindingCompleted == true)
                {
                    MyPatrolPoints = new Transform[PatrolPointsFinderScript.PatrolPoint.Length];
                    for (int x = 0; x < MyPatrolPoints.Length; x++)
                    {
                        MyPatrolPoints[x] = PatrolPointsFinderScript.PatrolPoint[x].transform;
                    }

                    if (PatrolPattern == ChoosePatrolPoints.ClosestPatrolPoints)
                    {
                        System.Array.Sort(MyPatrolPoints, (enemy1, enemy2) =>
                        Vector3.Distance(transform.position, enemy1.transform.position)
                        .CompareTo(Vector3.Distance(transform.position, enemy2.transform.position))
                        );
                    }
                    else
                    {
                        System.Random rng = new System.Random();
                        int n = MyPatrolPoints.Length;
                        while (n > 1)
                        {
                            n--;
                            int k = rng.Next(n + 1);
                            GameObject temp = MyPatrolPoints[k].gameObject;
                            MyPatrolPoints[k] = MyPatrolPoints[n];
                            MyPatrolPoints[n] = temp.transform;
                        }
                    }

                    PatrolPointsFinderScript.IsFindingCompleted = false;
                }
               
                if (CoreAiBehaviourScript.SearchingForSound == false && CoreAiBehaviourScript.IsAgentRoleLeader == false)
                {
                    CoreAiBehaviourScript.enableIkupperbodyRotations(ref CoreAiBehaviourScript.ActivateWalkIk);
                    Timer += Time.deltaTime;

                    CoreAiBehaviourScript.ReachnewCoverpoint = false;
                    if (MyPatrolPoints.Length > 0)
                    {
                        PatrollingController();
                    }
                }
            }

        }
        public void PatrollingController()
        {
            if (Timer >= TimeToFindNewPointToMove)
            {
               
                if(CoreAiBehaviourScript.Components.NavMeshAgentComponent.enabled == true)
                {
                    CoreAiBehaviourScript.Components.NavMeshAgentComponent.destination = MyPatrolPoints[Counts].transform.position;

                    CoreAiBehaviourScript.anim.SetBool(CoreAiBehaviourScript.AiAgentAnimatorParameters.DefaultStateParameterName, true);

                    CoreAiBehaviourScript.SetAnimationForFullBody(WalkIdleParametreName);
                    CoreAiBehaviourScript.anim.SetFloat(WalkIdleSpeedParameterName, PatrollingAnimationSpeed);
                    CoreAiBehaviourScript.AnimController(false, CoreAiBehaviourScript.Speeds.MovementSpeeds.WalkForwardSpeed, CoreAiBehaviourScript.AiAgentAnimatorParameters.DefaultStateParameterName, true, false);

                    GetPatrollingPoints();
                    FindNewPatrolPoint();
                }
                else
                {
                    CoreAiBehaviourScript.Components.NavMeshAgentComponent.enabled = true;
                    CoreAiBehaviourScript.NavMeshObstacleComponent.enabled = false;
                }
               
              
            }
            else
            {

                if (ForceStoppedIfOnlyOnePatrolPoint == false)
                {
                    Vector3 Dis = transform.position - MyPatrolPoints[Counts].transform.position;

                    if (Dis.magnitude < StoppingDistance)
                    {
                        FindNewPatrolPoint();

                        if (MyPatrolPoints.Length == 1)
                        {
                            ForceStoppedIfOnlyOnePatrolPoint = true;
                        }
                        else
                        {
                            GetPatrollingPoints();
                            CoreAiBehaviourScript.SetAnimationForFullBody(CoreAiBehaviourScript.AiAgentAnimatorParameters.IdleParameterName);
                            CoreAiBehaviourScript.AnimController(true, 0f, CoreAiBehaviourScript.AiAgentAnimatorParameters.DefaultStateParameterName, true, false);
                            StartMoving = false; 
                            StartCoroutine(IsMovable());
                        }
                    }
                    else
                    {
                        if (StartMoving == true)
                        {
                            if (CoreAiBehaviourScript.Components.NavMeshAgentComponent.enabled == true)
                            {
                                CoreAiBehaviourScript.ApplyRootMotion(false);
                                CoreAiBehaviourScript.Components.NavMeshAgentComponent.destination = MyPatrolPoints[Counts].transform.position;
                                CoreAiBehaviourScript.anim.SetBool(CoreAiBehaviourScript.AiAgentAnimatorParameters.DefaultStateParameterName, true);

                                CoreAiBehaviourScript.SetAnimationForFullBody(WalkIdleParametreName);
                                CoreAiBehaviourScript.anim.SetFloat(WalkIdleSpeedParameterName, PatrollingAnimationSpeed);
                                CoreAiBehaviourScript.AnimController(false, CoreAiBehaviourScript.Speeds.MovementSpeeds.WalkForwardSpeed, CoreAiBehaviourScript.AiAgentAnimatorParameters.DefaultStateParameterName, true, false);
                            }
                            else
                            {
                                CoreAiBehaviourScript.Components.NavMeshAgentComponent.enabled = true; 
                                CoreAiBehaviourScript.NavMeshObstacleComponent.enabled = false;
                            }
                          
                        }

                    }

                }
                else
                {
                    CoreAiBehaviourScript.SetAnimationForFullBody(CoreAiBehaviourScript.AiAgentAnimatorParameters.IdleParameterName);
                    CoreAiBehaviourScript.AnimController(true, 0f, CoreAiBehaviourScript.AiAgentAnimatorParameters.DefaultStateParameterName, true, false);
                    StartMoving = false;
                }
            }
        }
        public void GetPatrollingPoints()
        {
            TimeToFindNewPointToMove = 10000f;
        }
        public void FindNewPatrolPoint()
        {
            //Reshuffle
            if (PatrolPattern == ChoosePatrolPoints.RandomPatrolPoints)
            {
                System.Random rng = new System.Random();
                int n = MyPatrolPoints.Length;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    GameObject temp = MyPatrolPoints[k].gameObject;
                    MyPatrolPoints[k] = MyPatrolPoints[n];
                    MyPatrolPoints[n] = temp.transform;
                }

            }


            if (ReversePatrollingOrderTemp == true)
            {
                --Counts;
                if (Counts < 0)
                {
                    ReversePatrollingOrderTemp = false;
                    Counts = 0;
                }
            }
            else
            {
                ++Counts;
            }

            if (Counts >= MyPatrolPoints.Length)
            {
                if (SaveMyOrder == true)
                {
                    int rp = MyPatrolPoints.Length - 1;
                    Counts = rp - 1;
                    ReversePatrollingOrderTemp = SaveMyOrder;
                }
                else
                {
                    Counts = 0;
                }

            }

            Timer = 0f;
            TimeToFindNewPointToMove = SaveTime;
        }
        IEnumerator IsMovable()
        {
            float RandomiseWaiting = Random.Range(MinTimeAtPatrolPoint, MaxTimeAtPatrolPoint);
            yield return new WaitForSeconds(RandomiseWaiting);
            StartMoving = true;
            Timer = 0f;
            TimeToFindNewPointToMove = SaveTime;  
        }

    }
}