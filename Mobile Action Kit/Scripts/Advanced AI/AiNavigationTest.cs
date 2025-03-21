using UnityEngine;
using UnityEngine.AI;


namespace MobileActionKit
{
    public class AiNavigationTest : MonoBehaviour
    {
        public Transform target;
        public GameObject cubePrefab; // Assign your cube prefab in the Inspector

        private NavMeshPath navMeshPath;
        public float distanceToTarget;

        public Color GizmoColor = Color.yellow;

        void Start()
        {
            navMeshPath = new NavMeshPath();
        }

        void Update()
        {
            if (target == null)
            {
                Debug.LogError("Target is not assigned!");
                return;
            }

            CheckNavMeshPath();
            CalculateDistanceToTarget();
        }

        void CheckNavMeshPath()
        {
            if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, navMeshPath))
            {
                if (navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    Debug.Log("NavMesh path is available!");
                    gameObject.GetComponent<NavMeshAgent>().destination = target.position;
                }
                else if (navMeshPath.status == NavMeshPathStatus.PathPartial)
                {
                    Debug.Log("NavMesh path is partial. Status: " + navMeshPath.status);

                    // Uncomment to spawn a cube at the last valid corner of the partial path
                    // if (navMeshPath.corners.Length > 0)
                    // {
                    //     Vector3 lastValidPosition = navMeshPath.corners[navMeshPath.corners.Length - 1];
                    //     Instantiate(cubePrefab, lastValidPosition, Quaternion.identity);
                    // }
                }
                else
                {
                    Debug.Log("NavMesh path is not complete. Status: " + navMeshPath.status);
                }
            }
            else
            {
                Debug.Log("No NavMesh path found.");
            }
        }

        void CalculateDistanceToTarget()
        {
            distanceToTarget = Vector3.Distance(transform.position, target.position);
        }

        void OnDrawGizmos()
        {
            if (navMeshPath != null && navMeshPath.corners.Length > 0)
            {
                Gizmos.color = GizmoColor;
                for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(navMeshPath.corners[i], navMeshPath.corners[i + 1]);
                }
            }
        }
    }
}


//using UnityEngine;
//using UnityEngine.AI;

//public class AiNavigationTest : MonoBehaviour
//{
//    public Transform target;

//    void Update()
//    {
//        if (target == null)
//        {
//            Debug.LogError("Target is not assigned!");
//            return;
//        }


//        CheckNavMeshPath();
//    }
//    void CheckNavMeshPath()
//    {
//        NavMeshPath navMeshPath = new NavMeshPath();

//        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, navMeshPath))
//        {
//            if (navMeshPath.status == NavMeshPathStatus.PathComplete)
//            {
//                Debug.Log("NavMesh path is available!");
//                gameObject.GetComponent<NavMeshAgent>().destination = target.position;
//            }
//            else
//            {
//                Debug.Log("NavMesh path is not complete. Status: " + navMeshPath.status);
//            }
//        }
//        else
//        {
//            Debug.Log("No NavMesh path found.");
//        }
//    }

//}
