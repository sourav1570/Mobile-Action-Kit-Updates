using UnityEngine;
using UnityEngine.AI;

namespace MobileActionKit
{
    public class AiPathFinder : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This Script role is to generate the navmesh path for the Ai agent to travel from point A to point B. It is used in every case when Ai agent has to navigate the level.";

        [Tooltip("Drag and drop into this field 'Nav Mesh Agent' component located above in the inspector")]
        public NavMeshAgent NavMeshAgentComponent;

        [HideInInspector]
        public Vector3 closestPoint = Vector3.positiveInfinity;

        [HideInInspector]
        public Vector3 PreviousDestination = Vector3.positiveInfinity;

        [HideInInspector]
        public bool RecalculatePathCornerOnce = false;
        bool SpawnOnce = false;

        public int CompleteNavMeshPathChecksAttempts = 30; 

      //  [HideInInspector]
        public float MinRadiusFromCoordinate = 2f;
      //  [HideInInspector]
        public float MaxRadiusFromCoordinate = 4f;

        float DefaultMinDis = 1f;
        float DefaultMaxDis = 2f;

        [HideInInspector]
        public float MeterIncrementing = 100f;
        float CurrentRadius = 1f;

        public float RadiusToIncrementingInEachCheck = 10f;

        [HideInInspector]
        public bool PathIsUnreachable = false;

        [HideInInspector]
        public Vector3 IsSameDestination;

        NavMeshObstacle NavMeshObstacleComponent;
        
        //bool WasNavMeshObstacleActivated = false;

        [HideInInspector]
        public int InitialNavigationChecks = 5;
        int CurrentChecks = 0;
        [HideInInspector]
        public bool NoMoreChecks = false;

        [HideInInspector]
        public bool IsNavMeshObstacleDisabled = false;

        //public GameObject Cube;

        // Important to call it as when we are searching for new covers or new firing points we need to make sure to disable Nav Mesh Obstacle and enable Nav Mesh Agent to be able to properly search
        // for valid covers and firing points in the game.
        [HideInInspector]
        public bool PauseTheNavMeshSearching = false;

        public bool DebugDestinationPoints = false;
        public GameObject FirstStepVisualizer;
        public GameObject SecondStepVisualizer;
        public GameObject ThirdStepVisualizer;

        [HideInInspector]
        public bool IsNavMeshThirdAttemptCheck = false;

        private void Awake()
        {
            if (GetComponent<NavMeshObstacle>() != null)
            {
                NavMeshObstacleComponent = GetComponent<NavMeshObstacle>();
            }
        }
        private void Start()
        {        
            DefaultMinDis = MinRadiusFromCoordinate;
            DefaultMaxDis = MaxRadiusFromCoordinate;


            Vector3 randomDirection = Random.insideUnitSphere * 100.0f;
            randomDirection += transform.position;
            NavMeshHit navMeshHit;
            for (int i = 0; i < 10; i++)
            {
                if (NavMesh.SamplePosition(randomDirection, out navMeshHit, 100.0f, NavMesh.AllAreas))
                {
                    Vector3 randomPoint = navMeshHit.position;
                    PreviousDestination = randomPoint;
                    break;
                }
                else
                {
                    randomDirection = Random.insideUnitSphere * 100.0f;
                    randomDirection += transform.position;
                }
            }

        }
        public void FindClosestPointTowardsDestination(Vector3 agentDestination)
        {

            //NavMeshPath debugpath = new NavMeshPath();
            //if (NavMesh.CalculatePath(transform.position, agentDestination, NavMesh.AllAreas, debugpath))
            //{
            //    if (debugpath.status == NavMeshPathStatus.PathComplete)
            //    {
            //        Debug.Log("Path is complete my friend");
            //    }
            //    else if (debugpath.status == NavMeshPathStatus.PathPartial)
            //    {
            //        Debug.Log("Try Again Next time");
            //    }
            //    else if (debugpath.status == NavMeshPathStatus.PathInvalid)
            //    {
            //        Debug.Log("Not Valid");
            //    }
            //}

            //if(WasNavMeshObstacleActivated == false)
            //{
            //    if (NavMeshObstacleComponent != null)
            //    {
            //        if (NavMeshObstacleComponent.enabled == true)
            //        {
            //            NavMeshObstacleComponent.enabled = false;
            //            WasNavMeshObstacleActivated = true;
            //        }
            //    }
            //}

            if (IsNavMeshObstacleDisabled == true && PauseTheNavMeshSearching == false)
            {
                // This code is added recently to fix the problem with the snapping. in more detail when I was testing AI firing points and when the AI agent stopped and shoot the player at firing point and than move to
                // new firing point than basically in the same frame before moving he was deactivating navmesh obstacle and activating navmesh agent component which in result in the next frame after that created the snapping
                // for the humanoid AI agent. To solve this issue what are we basically doing now is we are deactivating navmesh obstacle and also deactivating navmesh agent component and than in the next frame we check if
                // both are deactivated and we are ready to do check valid/invalid point with the destination so basically making sure that current checks is bigger than 1 (meaning checks has been started) and thereafter we
                // enable NavMeshAgentComponent which makes the whole process smoother.

                // this also solve all the cases where the humanoid AI agent enable navmesh obstacle and after that enable navmesh agent component. another example would be shooting the last enemy and than doing wandering or patrolling.
                if (CurrentChecks >= 1)
                {
                    NavMeshAgentComponent.enabled = true; 
                }
              
                if (agentDestination != PreviousDestination && agentDestination != IsSameDestination || NoMoreChecks == false)
                {
                    CurrentChecks += 1;

                    PathIsUnreachable = false;
                    RecalculatePathCornerOnce = false;
                    SpawnOnce = false;
                    NavMeshPath path = new NavMeshPath();

                    if (NavMesh.CalculatePath(transform.position, agentDestination, NavMesh.AllAreas, path))
                    {
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            NavMeshHit hit;
                            if (NavMesh.SamplePosition(agentDestination, out hit, 5f, NavMesh.AllAreas))// Mathf.Infinity
                            {
                                IsNavMeshThirdAttemptCheck = false;
                                closestPoint = hit.position;
                               // GameObject go = Instantiate(Cube, hit.position, Quaternion.identity);
                                if(DebugDestinationPoints == true)
                                {
                                    GameObject go = Instantiate(FirstStepVisualizer, hit.position, Quaternion.identity);
                                }
                            }
                            else
                            {
                                closestPoint = agentDestination;
                                //GameObject go = Instantiate(Cube, hit.position, Quaternion.identity);
                            }
                            PreviousDestination = agentDestination;
                            RecalculatePathCornerOnce = false;
                        }
                        if (CurrentChecks >= InitialNavigationChecks)
                        {
                            if (path.status == NavMeshPathStatus.PathPartial)
                            {
                                CreateClosestNavMeshPath(agentDestination);
                            }
                        }

                    }

                    if (CurrentChecks >= InitialNavigationChecks)
                    {
                        // when the destination is in the air and navmeshagent can't move there it considered as an invalid path
                        if (path.status == NavMeshPathStatus.PathInvalid)
                        {
                            CreateClosestNavMeshPath(agentDestination);
                        }
                    }

                    //Debug.Log("AgentDestination" + agentDestination + " " + "PreviousDestination" + PreviousDestination + " " + "SameDestination" + IsSameDestination + " " + transform.name);
                    IsSameDestination = agentDestination;
                    NoMoreChecks = false;
                }

                if (agentDestination != PreviousDestination && agentDestination == IsSameDestination)
                {
                    PathIsUnreachable = true;
                }

                if (CurrentChecks >= InitialNavigationChecks && PathIsUnreachable == true)
                {
                    PreviousDestination = agentDestination;
                    IsNavMeshObstacleDisabled = false;
                    NoMoreChecks = true;
                    CurrentChecks = 0;
                }

                if (CurrentChecks >= InitialNavigationChecks || agentDestination == PreviousDestination)
                {
                    IsNavMeshObstacleDisabled = false;
                    NoMoreChecks = true;
                    CurrentChecks = 0;
                }

               
                if (agentDestination == PreviousDestination)
                {
                    RecreateDefaultValue();
                    SpawnOnce = false;
                }
            }

          //  Debug.Log(PreviousDestination + "PrevDest" + agentDestination + "AgentDest");
            if (agentDestination != PreviousDestination && PauseTheNavMeshSearching == false)
            {
                //NoMoreChecks = false;
                //  Debug.Log("YES it is true");
                NavMeshObstacleComponent.enabled = false;
                //NavMeshAgentComponent.enabled = true;
                IsNavMeshObstacleDisabled = true;
                
            }
            else
            {
                NavMeshAgentComponent.enabled = true;
                NavMeshObstacleComponent.enabled = false;
            }

            //if(NoMoreChecks == true)
            //{
            //    if (WasNavMeshObstacleActivated == true)
            //    {
            //        if (NavMeshObstacleComponent.enabled == false)
            //        {
            //            NavMeshObstacleComponent.enabled = true;
            //            WasNavMeshObstacleActivated = false;
            //        }
            //    }
            //}          
        }
        public void ForceEnableNavMeshAgentComponent()
        {
            NavMeshAgentComponent.enabled = true;
            NavMeshObstacleComponent.enabled = false;
        }
        public bool CheckIfPathExist(Vector3 Destination)
        {
            NavMeshPath path = new NavMeshPath();

            if (NavMesh.CalculatePath(transform.position, Destination, NavMesh.AllAreas, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    return true;
                }
                else
                {
                    Vector3 Point = FindClosestPointOnNavMeshOnlyFirstTime(Destination);
                    NavMeshPath pathattemptTwo = new NavMeshPath();
                    if (NavMesh.CalculatePath(transform.position, Point, NavMesh.AllAreas, pathattemptTwo))
                    {
                        if (pathattemptTwo.status == NavMeshPathStatus.PathComplete)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public void CreateClosestNavMeshPath(Vector3 agentDestination)
        {
            if (RecalculatePathCornerOnce == false)
            {
                RecalculatePathCornerOnce = true;

                closestPoint = FindClosestPointOnNavMeshOnlyFirstTime(agentDestination);

                NavMeshPath pathattemptTwo = new NavMeshPath();
                if (NavMesh.CalculatePath(transform.position, closestPoint, NavMesh.AllAreas, pathattemptTwo))
                {
                    if (pathattemptTwo.status == NavMeshPathStatus.PathComplete)
                    {
                        if (DebugDestinationPoints == true)
                        {
                            GameObject go = Instantiate(SecondStepVisualizer, closestPoint, Quaternion.identity);
                        }
                        //GameObject go = Instantiate(Cube, hit.position, Quaternion.identity);
                        RecalculatePathCornerOnce = false;
                        PreviousDestination = agentDestination;
                        CurrentChecks = 1 + InitialNavigationChecks;
                        //if (SpawnOnce == false)
                        //{
                        //    SpawnOnce = true;
                        //    if (SpawnCubes.instance != null)
                        //    {
                        //        SpawnCubes.instance.Spawning(closestPoint, transform.name);
                        //    }
                        //}
                    }
                }
            }

            if (RecalculatePathCornerOnce == true)
            {
                closestPoint = FindClosestPointOnNavMesh(agentDestination);

                MinRadiusFromCoordinate = MinRadiusFromCoordinate + MinRadiusFromCoordinate;
                MaxRadiusFromCoordinate = MaxRadiusFromCoordinate + MaxRadiusFromCoordinate;

                NavMeshPath pathattemptlast = new NavMeshPath();

                if (NavMesh.CalculatePath(transform.position, closestPoint, NavMesh.AllAreas, pathattemptlast))
                {
                    if (pathattemptlast.status == NavMeshPathStatus.PathComplete)
                    {
                        if (DebugDestinationPoints == true)
                        {
                            GameObject go = Instantiate(ThirdStepVisualizer, closestPoint, Quaternion.identity);
                        }
                        PreviousDestination = agentDestination;
                        RecalculatePathCornerOnce = false;
                        CurrentChecks = 1 + InitialNavigationChecks;
                        //if (SpawnOnce == false)
                        //{
                        //    SpawnOnce = true;
                        //    if (SpawnCubes.instance != null)
                        //    {
                        //        SpawnCubes.instance.Spawning(closestPoint, transform.name);
                        //    }
                        //}
                    }
                }
            }
        }
        public void RecreateDefaultValue()
        {
            CurrentRadius = 0f;
            MinRadiusFromCoordinate = DefaultMinDis;
            MaxRadiusFromCoordinate = DefaultMaxDis;
        }
        private Vector3 FindClosestPointOnNavMeshOnlyFirstTime(Vector3 position)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(position, out hit, 10f, NavMesh.AllAreas))
            {
                IsNavMeshThirdAttemptCheck = false;
                return hit.position;

            }

            return position;
        }
        private Vector3 FindClosestPointOnNavMesh(Vector3 position)
        {

            for (int i = 0; i < CompleteNavMeshPathChecksAttempts; i++) // Try a maximum of CompleteNavMeshPathChecksAttempts Attempts
            {
                Vector3 randomPoint = position + Random.insideUnitSphere * Random.Range(MinRadiusFromCoordinate, MaxRadiusFromCoordinate);

                MinRadiusFromCoordinate = RadiusToIncrementingInEachCheck + MinRadiusFromCoordinate;
                MaxRadiusFromCoordinate = RadiusToIncrementingInEachCheck + MaxRadiusFromCoordinate;

                CurrentRadius = CurrentRadius + MeterIncrementing; 

                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, CurrentRadius, NavMesh.AllAreas))
                {
                   // GameObject go = Instantiate(Cube, hit.position, Quaternion.identity);
                    NavMeshPath path = new NavMeshPath();
                    if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path))
                    {
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            IsNavMeshThirdAttemptCheck = true;
                            return hit.position;
                         
                        }
                    }
                }
            }

            return position; // Return a default value if no valid point is found
        }
        // In case the path to the destination is not reach able for example if the player is on the roof and shoot the enemies patrolling in another opposite roof than in this case we need to create a navmesh path
        // in that opposite roof we need create a navmesh path so we need to find any point in that opposite roof and should not find closest point towards the destination. As it will result invalid.for this case we will create
        // a navmesh point around the Ai in the same roof.
        //private Vector3 FindPointOnNavMeshFromCurrentPostion(Vector3 position)
        //{
        //    for (int i = 0; i < 20; i++) // Try a maximum of 20 Attempts
        //    {
        //        Vector3 randomPoint = position + Random.insideUnitSphere * Random.Range(20f, 100f);

        //        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 100f, NavMesh.AllAreas))
        //        {
        //            NavMeshPath path = new NavMeshPath();
        //            if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path))
        //            {
        //                if (path.status == NavMeshPathStatus.PathComplete)
        //                {
        //                    return hit.position;
        //                }
        //            }
        //        }
        //    }

        //    return position; // Return a default value if no valid point is found
        //}
    }
}