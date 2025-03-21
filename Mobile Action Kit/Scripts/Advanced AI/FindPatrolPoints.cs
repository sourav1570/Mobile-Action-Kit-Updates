using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class FindPatrolPoints : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This script is responsible to find patrol points within the specified radius given in  the script named 'Patrolling' which is attached " +
            "with the root of this Ai agent.";

        public void ResettingDescription()
        {
            ScriptInfo = "This script is responsible to find patrol points within the specified radius given in  the script named 'Patrolling' which is attached " +
            "with the root of this Ai agent.";
        }

        [Tooltip("Drag and drop the 'CoreAiBehaviour' component attached with the root of this gameobject from the hierarchy into this field.")]
        public CoreAiBehaviour CoreAiBehaviourScript;
        //[HideInInspector]
        //public float MinTimeToDeactive = 1f;
        //[HideInInspector]
        //public float MaxTimeToDeactive = 1f;

        //float RandomTimeToDeactive;

        bool once = false;

        [Header("Values automatically assigned")]
        [HideInInspector]
        public Transform RootObj;

        [HideInInspector]
        public List<Transform> AllPatrolPoints = new List<Transform>();

        [HideInInspector]
        public Transform[] PatrolPoint;

        [HideInInspector]
        public bool IsFindingCompleted = false;

        private AiPathFinder PathFinderScript;

        void OnEnable()
        {
            PathFinderScript = transform.root.GetComponent<AiPathFinder>();
            RootObj = transform.root;
            transform.parent = null;
            AllPatrolPoints.Clear();
            PathFinderScript.PauseTheNavMeshSearching = true;
            PathFinderScript.ForceEnableNavMeshAgentComponent();
            //RandomTimeToDeactive = Random.Range(MinTimeToDeactive, MaxTimeToDeactive);
        }
        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "PatrolPoint" && CoreAiBehaviourScript.Components.NavMeshAgentComponent.enabled == true)
            {
                //// Its important to call these 2 lines as PathFinderScript.CheckIfPathExist need to make sure that there is no obstacle on Ai agent before searching
                //// whether the path is complete from the current position of the Ai agent or not.
                //CoreAiBehaviourScript.NavMeshObstacleComponent.enabled = false;
                //CoreAiBehaviourScript.Components.NavMeshAgentComponent.enabled = true;

                if (PathFinderScript != null)
                {
                    if (PathFinderScript.CheckIfPathExist(other.gameObject.transform.position) == true)
                    {
                        AllPatrolPoints.Add(other.transform);
                    }
                 
                }
            }

            if (once == false)
            {
                StartCoroutine(TimeTodeactive());
                once = true;
            }


        }
        private IEnumerator TimeTodeactive()
        {
            yield return null;
            PatrolPoint = new Transform[AllPatrolPoints.Count];
            for (int x = 0; x < AllPatrolPoints.Count; x++)
            {
                PatrolPoint[x] = AllPatrolPoints[x].transform;
            }
            IsFindingCompleted = true;
            transform.parent = RootObj;
            Vector3 Pos = transform.localPosition;
            Pos = Vector3.zero;
            transform.localPosition = Pos;
            // reset it so other functionality could keep running in core ai behaviour script
            PathFinderScript.PauseTheNavMeshSearching = false;
            CoreAiBehaviourScript.DisableTemporaryWhenSearchingForCoverOrFiringPoints = false;
            gameObject.SetActive(false);
            once = false;
        }
        //IEnumerator TimeTodeactive()
        //{
        //    yield return new WaitForSeconds(RandomTimeToDeactive);
        //    PatrolPoint = new Transform[AllPatrolPoints.Count];
        //    for (int x = 0; x < AllPatrolPoints.Count; x++)
        //    {
        //        PatrolPoint[x] = AllPatrolPoints[x].transform;
        //    }
        //    IsFindingCompleted = true;
        //    transform.parent = RootObj;
        //    Vector3 Pos = transform.localPosition;
        //    Pos = Vector3.zero;
        //    transform.localPosition = Pos;
        //    gameObject.SetActive(false);
        //    once = false;
        //}
    }
}