using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class FindFiringPoints : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This script is responsible to find firing points within the specified radius given in  the script named 'CoreAiBehaviour' which is attached " +
           "with the root of this Ai agent.";

        public void ResettingDescription()
        {
            ScriptInfo = "This script is responsible to find firing points within the specified radius given in  the script named 'CoreAiBehaviour' which is attached " +
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

        [HideInInspector]
        public Transform RootObj;
        [HideInInspector]
        public List<FiringPoint> AllFiringPoints = new List<FiringPoint>();
        [HideInInspector]
        public Transform[] FiringPoints;

        private AiPathFinder PathFinderScript;

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        void OnEnable()
        {
            PathFinderScript = transform.root.GetComponent<AiPathFinder>();
            RootObj = transform.root;
            transform.parent = null;
            AllFiringPoints.Clear();
            PathFinderScript.PauseTheNavMeshSearching = true;
            PathFinderScript.ForceEnableNavMeshAgentComponent();

           // RandomTimeToDeactive = Random.Range(MinTimeToDeactive, MaxTimeToDeactive);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (CoreAiBehaviourScript.InitialiseFiringPointsBehaviour == true && CoreAiBehaviourScript.Components.NavMeshAgentComponent.enabled == true)
            {
                //// Its important to call these 2 lines as PathFinderScript.CheckIfPathExist need to make sure that there is no obstacle on Ai agent before searching
                //// whether the path is complete from the current position of the Ai agent or not.
                //CoreAiBehaviourScript.NavMeshObstacleComponent.enabled = false;
                //CoreAiBehaviourScript.Components.NavMeshAgentComponent.enabled = true;

                if (other.gameObject.GetComponent<FiringPoint>() != null)
                {
                    if (other.gameObject.GetComponent<FiringPoint>().SpecificTeamFiringPoint == true && CoreAiBehaviourScript.T.MyTeamID == other.gameObject.GetComponent<FiringPoint>().TeamName)
                    {
                        if (PathFinderScript != null)
                        {
                            if (PathFinderScript.CheckIfPathExist(other.gameObject.transform.position) == true)
                            {
                                AllFiringPoints.Add(other.gameObject.GetComponent<FiringPoint>());
                            }
                        }
                    }
                    else if (other.gameObject.GetComponent<FiringPoint>().SpecificTeamFiringPoint == false)
                    {
                        if (PathFinderScript != null)
                        {
                            if (PathFinderScript.CheckIfPathExist(other.gameObject.transform.position) == true)
                            {
                                AllFiringPoints.Add(other.gameObject.GetComponent<FiringPoint>());
                            }
                        }
                    }
                }

                if (once == false)
                {
                    StartCoroutine(TimeTodeactive());
                    once = true;
                }
            }
        }
        private IEnumerator TimeTodeactive()
        {
            yield return null;
            FiringPoints = new Transform[AllFiringPoints.Count];
            for (int x = 0; x < AllFiringPoints.Count; x++)
            {
                FiringPoints[x] = AllFiringPoints[x].transform;
            }
            // Important to call it after line number 91 to 94 as Finding way point function is reliable on FiringPoints so it should be all added first above 
            CoreAiBehaviourScript.Deregisterfirepoint = false;
            CoreAiBehaviourScript.FindingwayPoint();
            transform.parent = RootObj;
            Vector3 Pos = transform.localPosition;
            Pos = Vector3.zero;
            transform.localPosition = Pos;
            gameObject.SetActive(false);
            // reset it so other functionality could keep running in core ai behaviour script
            PathFinderScript.PauseTheNavMeshSearching = false;
            CoreAiBehaviourScript.DisableTemporaryWhenSearchingForCoverOrFiringPoints = false;
            once = false;
        }
        //IEnumerator TimeTodeactive()
        //{
        //    yield return new WaitForSeconds(RandomTimeToDeactive);
        //    CoreAiBehaviourScript.FindingwayPoint();
        //    FiringPoints = new Transform[AllFiringPoints.Count];
        //    for (int x = 0; x < AllFiringPoints.Count; x++)
        //    {
        //        FiringPoints[x] = AllFiringPoints[x].transform;
        //    }
        //    transform.parent = RootObj;
        //    Vector3 Pos = transform.localPosition;
        //    Pos = Vector3.zero;
        //    transform.localPosition = Pos;
        //    gameObject.SetActive(false);
        //    once = false;
        //}
    }
}