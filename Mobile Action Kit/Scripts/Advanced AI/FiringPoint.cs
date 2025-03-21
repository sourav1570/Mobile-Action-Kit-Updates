using UnityEngine.AI;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace MobileActionKit
{
    public class FiringPoint : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script is responsible for firing points creation for the Ai agents.";

        public void ResettingDescription()
        {
            ScriptInfo = "This script is responsible for firing points creation for the Ai agents.";
        }

        [Tooltip("If checked than Ai agent will debug firing point status if it is occupied or vacant")]
        public bool DebugFiringPointStatus = true;
        [Tooltip("This field is responsible for debugging firing point state in a form of text whether it is vacant or occupied.")]
        public GameObject DebugInfoTextUI;

        [Tooltip("Adjust text UI horizontally or vertically by tweaking values in this field")]
        public Vector3 DebugInfoTextUIOffset = new Vector3(0f, 2f, 0f);

        [Tooltip("If checked than Ai agent will crouch and shoot on this firing point position.")]
        public bool EnableCrouchFiring = false;
        [Tooltip("If checked than this firing point can only be available to one Team.")]
        public bool SpecificTeamFiringPoint = false;

        [Tooltip("If the above field name 'SpecificTeamCover' is checked than it is required to enter the TeamName for which this firing point can be available too.")]
        public string TeamName;
        //[HideInInspector]
        //public bool Reached = false;

        [HideInInspector]
        public bool IsFiringPointAlreadyRegistered = false;
        private TextMeshProUGUI spawnedText; // Reference to the spawned text object

        [HideInInspector]
        public bool DistanceCleared = false;

        [HideInInspector]
        public float DebugDistanceWithAiAgent;

        // IT'S VERY IMPORTANT TO HAVE COVER NUMBER FOR EACH FIRING POINT AS THE AI FINDING CLOSEST FIRING POINT LIST IS ALWAYS GETTING SORTED TO FIND THE BEST COVER FOR HIM
        // FOR INSTANCE IF YOU DON'T USE THE COVER NUMBER THAN WHEN AI AGENT IS WITHIN THE FIRING POINT I.E USING IT AND WHEN IT WILL BE REQUIRED TO FIND ANOTHER FIRING POINT FOR HIM
        // THE LIST WILL BE SORTED FOR AI AGENT TO FIND THE CLOSEST FIRING POINT FOR HIM AGAIN WHICH INDEED CAN GIVE WRONG RESULTS IF WE DON'T USE COVER NUMBER. THE BENEFIT OF USING COVER NUMBER
        // IS THAT.EACH FIRING POINT HAS UNIQUE NUMBER SO WHICH IS IN USE AND WHICH IS NOT AI AGENT CAN CLEARLY IDENTIFY THAT NO MATTER HOW THERE LIST IS BEING SORTED BEFORE FINDING CLOSEST FIRING POINT.
        // WHENEVER WE LOOK FOR CLOSEST FIRING POINT WE SORT THE LIST SO IF WE DON'T USE COVER NUMBER IT COULD REGISTER THE WRONG COVER FOR CURRENT AI AGENT AS WELL AS FOR FRIENDLIES.
        [HideInInspector]
        public int CoverNumber;

        private void Start()
        {
            if (DebugFiringPointStatus == true)
            {
                SpawnText();
                Info(false);
            }
        }
        private void SpawnText()
        {
            spawnedText = Instantiate(DebugInfoTextUI, transform.position + DebugInfoTextUIOffset, Quaternion.identity).GetComponent<TextMeshProUGUI>();
            GameObject canvasfound = GameObject.FindGameObjectWithTag("Canvas3D");
            spawnedText.transform.SetParent(canvasfound.transform, false);
            //Rotate the spawned text 90 degrees up on the X-axis
            spawnedText.transform.rotation = Quaternion.Euler(-90f, 90f, 0f);
        }
        private void Update()
        {
#if UNITY_EDITOR
            // Rotate towards the SceneView camera
            if (spawnedText != null)
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                if (sceneView != null)
                {
                    spawnedText.transform.rotation = Quaternion.LookRotation(sceneView.camera.transform.forward, sceneView.camera.transform.up);
                }
                spawnedText.transform.position = transform.position + DebugInfoTextUIOffset;
            }

#endif


            //if (StartTimer == true)
            //{
            //    Timer += Time.deltaTime;
            //}

            //if (Timer > CheckTime)
            //{
            //    TriggerOnce = false;
            //    Reached = false;  
            //    StartTimer = false;
            //    Timer = 0f;
            //}
        }
        public void Info(bool IsOccupied)
        {
            if (DebugFiringPointStatus == true)
            {
                if (IsOccupied == true)
                {
                    spawnedText.text = "OCCUPIED";
                    spawnedText.color = Color.red;
                }
                else
                {
                    spawnedText.text = "VACANT";
                    spawnedText.color = Color.green;
                }
            }
        }
        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.gameObject.transform.root.tag == "AI")
        //    {
        //        if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
        //        {
        //            if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CombatStateBehaviours.UseFiringPoints == true && other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().WaypointsFinded == true)
        //            {
        //                if (TriggerOnce == false && Reached == false && other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().WayPointPositions[other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CurrentWayPoint].transform == this.transform)
        //                {
        //                    other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().ChangeCover = false;
        //                    other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().Reachnewpoints = true;
        //                    other.gameObject.transform.root.GetComponent<NavMeshAgent>().isStopped = true;
        //                    Reached = true;
        //                    TriggerOnce = true;
        //                    CheckTime = other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().SaveResetedCoverRandomisation;
        //                }
        //            }
        //        }
        //    }
        //}
        //private void OnTriggerStay(Collider other)
        //{
        //    if (other.gameObject.transform.root.tag == "AI")
        //    {
        //        if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
        //        {
        //            if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CombatStateBehaviours.UseFiringPoints == true && other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().WaypointsFinded == true)
        //            {
        //                if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().ChangeCover == true && Reached == true && other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().WayPointPositions[other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CurrentWayPoint].transform == this.transform)
        //                {
        //                    StartTimer = false;
        //                    other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().Reachnewpoints = false;
        //                    Reached = false;
        //                }
        //                else
        //                {
        //                    if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().ChangeCover == false && Reached == false && other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().WayPointPositions[other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CurrentWayPoint].transform == this.transform)
        //                    {
        //                        other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().Reachnewpoints = true;
        //                        Reached = true;
        //                        if (TriggerOnce == true)
        //                        {
        //                            StartTimer = true;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //private void OnTriggerExit(Collider other)
        //{
        //    if (other.gameObject.transform.root.tag == "AI")
        //    {
        //        StartTimer = true;
        //    }
        //}
    }
}