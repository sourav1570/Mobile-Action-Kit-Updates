using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class FindEmergencyCovers : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This script is responsible to find emergency covers within the specified radius given in  the script named 'CoreAiBehaviour' which is attached " +
           "with the root of this Ai agent.";

        public void ResettingDescription()
        {
            ScriptInfo = "This script is responsible to find emergency covers within the specified radius given in  the script named 'CoreAiBehaviour' which is attached " +
           "with the root of this Ai agent.";
        }

        [Tooltip("Drag and drop the 'CoreAiBehaviour' component attached with the root of this gameobject from the hierarchy into this field.")]
        public CoreAiBehaviour CoreAiBehaviourScript;

        //[HideInInspector]
        //public float MinTimeToDeactive = 0.1f;
        //[HideInInspector]
        //public float MaxTimeToDeactive = 0.1f;
        //float RandomTimeToDeactive;

        bool once = false;

        [Header("Values automatically assigned")]
        [HideInInspector]
        public Transform RootObj;

        [HideInInspector]
        public List<EmergencyCoverNode> AllEmergencyCoverNodes = new List<EmergencyCoverNode>();

        [HideInInspector]
        public Transform[] EmergencyPoint;

        private AiPathFinder PathFinderScript;

        void OnEnable()
        {
            PathFinderScript = transform.root.GetComponent<AiPathFinder>();
            RootObj = transform.root;
            transform.parent = null;
            AllEmergencyCoverNodes.Clear();
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
            if (other.gameObject.GetComponent<EmergencyCoverNode>() != null && CoreAiBehaviourScript.Components.NavMeshAgentComponent.enabled == true)
            {
                //// Its important to call these 2 lines as PathFinderScript.CheckIfPathExist need to make sure that there is no obstacle on Ai agent before searching
                //// whether the path is complete from the current position of the Ai agent or not.
                //CoreAiBehaviourScript.NavMeshObstacleComponent.enabled = false;
                //CoreAiBehaviourScript.Components.NavMeshAgentComponent.enabled = true;

                if (PathFinderScript != null)
                {
                    if (PathFinderScript.CheckIfPathExist(other.gameObject.transform.position) == true)
                    {
                        AllEmergencyCoverNodes.Add(other.gameObject.GetComponent<EmergencyCoverNode>());
                    }

                    ///////////////////
                    //if (once == false)
                    //{
                    //    StartCoroutine(TimeTodeactive());
                    //    once = true;
                    //}
                    ///////////////////
                }

            }

            //If we uncommented the same above code the problem is it will not going to make this value PathFinderScript.PauseTheNavMeshSearching = false; if no emergency cover is found
            // due to which Ai wll not be able to calculate the path with the destination so we need to make sure to disable it if no emergeny cover is found.
            if (once == false)
            {
                StartCoroutine(TimeTodeactive());
                once = true;
            }


        }
        private IEnumerator TimeTodeactive()
        {
            yield return null;
            EmergencyPoint = new Transform[AllEmergencyCoverNodes.Count];
            for (int x = 0; x < AllEmergencyCoverNodes.Count; x++)
            {
                EmergencyPoint[x] = AllEmergencyCoverNodes[x].transform;
            }
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
        //    EmergencyPoint = new Transform[AllEmergencyCoverNodes.Count];
        //    for (int x = 0; x < AllEmergencyCoverNodes.Count; x++)
        //    {
        //        EmergencyPoint[x] = AllEmergencyCoverNodes[x].transform;
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
