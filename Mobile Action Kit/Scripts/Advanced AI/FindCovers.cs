using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class FindCovers : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This script is responsible to find covers within the specified radius given in  the script named 'CoreAiBehaviour' which is attached " +
          "with the root of this Ai agent.";

        public void ResettingDescription()
        {
            ScriptInfo = "This script is responsible to find covers within the specified radius given in  the script named 'CoreAiBehaviour' which is attached " +
          "with the root of this Ai agent.";
        }

        [Tooltip("Drag and drop the 'CoreAiBehaviour' component attached with the root of this gameobject from the hierarchy into this field.")]
        public CoreAiBehaviour CoreAiBehaviourScript;

        //[HideInInspector]
        //public float MinTimeToDeactive = 1f;
        //[HideInInspector]
        //public float MaxTimeToDeactive = 1f;

        bool once = false;


        [HideInInspector]
        public Transform RootObj;

        [HideInInspector]
        public List<CoverNode> AllCoverNodes = new List<CoverNode>();

        [HideInInspector]
        public Transform[] CoverPoints;
        //float RandomTimeToDeactive;

        private AiPathFinder PathFinderScript;

        void OnEnable()
        {
            CoreAiBehaviourScript.DisableTemporaryWhenSearchingForCoverOrFiringPoints = true;
            PathFinderScript = transform.root.GetComponent<AiPathFinder>();
            RootObj = transform.root;
            transform.parent = null;
            AllCoverNodes.Clear();
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
            if(CoreAiBehaviourScript.InitialiseCoverBehaviour == true && CoreAiBehaviourScript.Components.NavMeshAgentComponent.enabled == true)
            {
                //// Its important to call these 2 lines as PathFinderScript.CheckIfPathExist need to make sure that there is no obstacle on Ai agent before searching
                //// whether the path is complete from the current position of the Ai agent or not.
                //CoreAiBehaviourScript.NavMeshObstacleComponent.enabled = false;
                //CoreAiBehaviourScript.Components.NavMeshAgentComponent.enabled = true;

                if (other.gameObject.GetComponent<CoverNode>() != null)
                {
                    if (other.gameObject.GetComponent<CoverNode>().AiCoverScript.SpecificTeamCover == true && CoreAiBehaviourScript.T.MyTeamID == other.gameObject.GetComponent<CoverNode>().AiCoverScript.TeamName)
                    {
                        if (PathFinderScript != null)
                        {
                            if (PathFinderScript.CheckIfPathExist(other.gameObject.transform.position) == true)
                            {
                                AllCoverNodes.Add(other.gameObject.GetComponent<CoverNode>());
                            }
                        }
                    }
                    else if (other.gameObject.GetComponent<CoverNode>().AiCoverScript.SpecificTeamCover == false)
                    {
                        if (PathFinderScript != null)
                        {
                            if (PathFinderScript.CheckIfPathExist(other.gameObject.transform.position) == true)
                            {
                                AllCoverNodes.Add(other.gameObject.GetComponent<CoverNode>());
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
            CoverPoints = new Transform[AllCoverNodes.Count];
            for (int x = 0; x < AllCoverNodes.Count; x++)
            {
                CoverPoints[x] = AllCoverNodes[x].transform;
            }
            // Important to call it after line number 94 to 98 as Finding cover point function is reliable on CoverPoints so it should be all added first above 
            CoreAiBehaviourScript.DeregisterCoverNodes = false;
            CoreAiBehaviourScript.FindingCoverPoint();
            transform.parent = RootObj;
            Vector3 Pos = transform.localPosition;
            Pos = Vector3.zero;
            transform.localPosition = Pos;
            gameObject.SetActive(false);
            once = false;
            // reset it so other functionality could keep running in core ai behaviour script
            PathFinderScript.PauseTheNavMeshSearching = false;
            CoreAiBehaviourScript.DisableTemporaryWhenSearchingForCoverOrFiringPoints = false;
        }
        //IEnumerator TimeTodeactive()
        //{
        //    yield return new WaitForSeconds(RandomTimeToDeactive);
        //    CoreAiBehaviourScript.FindingCoverPoint();
        //    CoverPoints = new Transform[AllCoverNodes.Count];
        //    for (int x = 0; x < AllCoverNodes.Count; x++)
        //    {
        //        CoverPoints[x] = AllCoverNodes[x].transform;
        //    }
        //    transform.parent = RootObj;
        //    Vector3 Pos = transform.localPosition;
        //    Pos = Vector3.zero;
        //    transform.localPosition = Pos;
        //    gameObject.SetActive(false);
        //    once = false;
        //    CoreAiBehaviourScript.DisableTemporaryWhenSearchingForCoverOrFiringPoints = false;
        //    //if (AllCoverNodes.Count <= 1)
        //    //{
        //    //    CoreAiBehaviourScript.CurrentCoverPoint = 0;
        //    //}
        //}
    }
}