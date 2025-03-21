using UnityEngine.AI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEditor;

namespace MobileActionKit
{
    // This Script Gets Called When Bots Takes Cover 

    public class CoverNode : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script allows for creation of 4 types of cover nodes." +
            " First type of cover node is called 'Crouch Firing Cover'. " +
            "Its forcing AI agent to crouch while occupying it. And allows AI agent to perform crouch fire at the enemies." +
            "Second crouch type of cover node(point) is called 'Crouch Hiding cover'.Its also forcing occupying AI Agent to crouch but does not allow Ai agent to shoot." +
            "Third and fourth cover nodes (points) types are called 'Stand Firing Cover' and 'Stand Hiding Cover'. " +
            "They are the same as the first two crouching nodes with the only difference being is that they are forcing covering Ai agents to stand.";

        public enum CoverType
        {
            CrouchFiringCover,
            CrouchHidingCover,
            StandHidingCover,
            StandFiringCover
            //   OpenFire
        }

        [Tooltip("Select one of the  available cover types (that are described in the script info) for this cover node.")]
        public CoverType ChooseCoverType;

        [HideInInspector]
        public bool CrouchFiringCoverNode = false;
        [HideInInspector]
        public bool StandingCover = false;
        [HideInInspector]
        public bool CrouchCover = false;
        [HideInInspector]
        public bool StandFiringCover = false;

        //[HideInInspector]
        //public bool TriggerOnce = false;

        //[HideInInspector]
        //public float CheckTime = 2f;

        //[HideInInspector]
        //public float Timer; 

        //[HideInInspector]
        //public float XPos;

        //[Range(0,90)]
        // [Tooltip("Before Taking Cover Check For The Enemy Position If he is not in field of view than its a Valid cover To Take .This will  Make Sure That The Bot is Always Behind some Cover")]
        //  [HideInInspector]
        // [Range(0,360)]
        [HideInInspector]
        public int FOVToCheckForCoverValidation = 240;

        [HideInInspector]
        public float Angle;

        [HideInInspector]
        public bool IsValidCover = false;

        //[HideInInspector]
        //public float TempStandCoverZAxis;

        Vector3 dir;
        // bool StartTimer;


        //  [HideInInspector]
        //  public bool ActivateStrafe = false;
        //  public Transform[] StrafePoints;
        //  int StrafeCounts;

        //  public string DebugCoverState = "Nobody is in This Cover";

        [HideInInspector]
        public bool IsAlreadyRegisteredCover = false;

        [Tooltip("Allows the use of additional'Open Fire Points' for Ai agent behind cover." +
            "Game object named 'Open Fire Point' is the auxiliary element of the cover assembly that can be useful to both Firing and Hiding cover variations." +
            "For Firing covers - as an additional firing point for  variety of firing behaviour of Ai agent in Firing covers. " +
            "And for Hiding covers it is the only possible way for Ai agent behind those covers to be able to shoot at enemies." +
            "There is a set of timers in 'Core Ai Behaviour' script's  paragraph named 'Ai Covers' that regulate covering related behaviours including Open Fire Points related parameters.")]
        public bool UseOpenFirePoints = false;
        //  public bool RandomStayOrFindNewCover = false;
        //public bool EnableStrafeOnFire = false;
        //public bool RandomStrafeOrStationedFire = false;

        [Tooltip("Add one or more 'Open Fire Point(s)' to this cover assembly for Ai agent(s) to use after cover occupying time is expired." +
            " This behaviour may be set the way so that Ai agent would cycle between cover point and open fire point (i.e return to the cover point after open fire point's timer is expired and vice versa)" +
            " for entirety of combat state unless this cover becomes invalid.Open fire points are of stand type by default.If you want open fire point to be of crouch type then" +
            " you have to setup open fire point to be crouch type by selecting its tag to be 'CrouchFirePoint'.")]
        public Transform[] OpenFirePoints;

        [Tooltip("'Secondary Open Fire Point' is a backup option for the cases when predefined 'Open Fire Point' is unavailable(occupied coz same OpenFirePoint can be setup to be shared between few cover nodes, " +
            "or not provided at all for this cover node). It can be  created within specified radius from this cover node. " +
            "In case this would not be desirable option due to the possibility for Ai agent to pick bad spot like in front of the cover if the value of that radius is big enough to allow that, " +
            "then this value can be made small enough to force Ai agent to just stand up from the 'Crouch Cover Point' and shoot his weapon (If this node is setup to be crouch cover type). " +
            "If none of the solutions above are suitable for the cover you are making, " +
            "then you can create an empty game object, position it the way you like and drag and drop it into this field. " +
            "This way the radius for the random OpenFirePoint will be created from that game object instead of cover node itself. ")]
        public Transform SecondaryOpenFirePoint;

        [Tooltip("Value of this field sets the limit for 'Procedural Open Fire Point Radius' within which open fire point coordinate will be created.")]
        public float SecondaryOpenFirePointRadius = 5f;

        // [HideInInspector]
        //  public bool IsCoverInUse = false;

        [HideInInspector]
        public string TeamUsingCover;

        [System.Serializable]
        public class StandCoverLogic
        {
            public Transform StandHideCoverDirection;
            public ChooseAnimationToPlay AnimationToPlay;
        }

        public enum ChooseAnimationToPlay
        {
            RifleIdle,
            StandRight,
            StandLeft
        }

        [Tooltip("This list of up to 3 hiding directions or hiding positions  for Ai agent to assume when occupying Hiding cover. " +
            "Neutral , Left and Right. All 3 directions may be useful in case of a 3D object  for this Cover assembly being something like a column or a pillar. " +
            "Ai agent then will orient himself in any  of the available directions depending on closer to which side of the cover the closest enemy currently is situated.")]
        public Transform[] StandHideCoverDirections;

        [Tooltip("In case chosen cover node type is 'Stand Hiding Cover' and Stand Hide Cover Directions are set in the fields above, " +
            "than this list will provide  animation clips to be played back by AI agent occupying this Stand Hiding Cover for each respected direction. " +
            "Which one of those 3 hiding directions will be selected and which one of the 3 corresponding stand hiding animations will be used by hiding AI agent is decided by the direction of the closest enemy that Ai agent will be hiding from. " +
            "Stand Hiding Cover node can have 3,2 or as little as one hiding direction based on near what kind of 3d object this node is placed. If it is a column or a pillar like 3d object then you might set all 3 hiding directions. " +
            "If it is something like edge of the wall or side of the gates or the corner of the building then 2 hiding directions will be enough. You can get away with only one 'Neutral' hiding posture for the Ai agent if you'd like. " +
            "It is not mandatory to have all 3 or 2 directions for any given 'Stand Hiding Cover' node.")]
        public List<StandCoverLogic> StandHideCoverAnimationPlayer = new List<StandCoverLogic>();

        [HideInInspector]
        public AiCover AiCoverScript;
        [HideInInspector]
        public DebugCoverState DebugCoverStateScript;

        [HideInInspector]
        public bool AdvanceBookingForCover = false;

        [HideInInspector]
        public float RotationIHaveToDo;

        //[HideInInspector]
        //bool CanProgressWithTrigger = false;

        //bool DoOnce = false;
        //float MinTimeToCheckcollision = 0.2f;
        //float MaxTimeToCheckcollision = 0.5f;

        [HideInInspector]
        public bool DistanceCleared = false;

        //[HideInInspector]
        //public int CoverNumber;

        [HideInInspector]
        public TextMeshProUGUI spawnedText; // Reference to the spawned text object

        [HideInInspector]
        public float DebugDistanceWithAiAgent;

        [HideInInspector]
        public string AgentNameTakingCover;

        float OriginalFov;

        bool DoOnce = false;
        int Index;

        private void Awake()
        {
            if (transform.parent != null)
            {
                AiCoverScript = transform.parent.GetComponent<AiCover>();
            }
            if (transform.parent != null)
            {
                DebugCoverStateScript = transform.parent.GetComponent<DebugCoverState>();
            }

            if (CoverType.StandHidingCover == ChooseCoverType)
            {
                StandingCover = true;
            }
            else if (CoverType.CrouchFiringCover == ChooseCoverType)
            {
                CrouchFiringCoverNode = true;
            }
            else if (CoverType.CrouchHidingCover == ChooseCoverType)
            {
                CrouchCover = true;
            }
            else if (CoverType.StandFiringCover == ChooseCoverType)
            {
                StandFiringCover = true;
            }
            //else if (CoverType.OpenFire == Placement)
            //{
            //    OpenFireCover = true;
            //}


            //TempStandCoverZAxis = transform.parent.localPosition.z;
            //if (StandingCover == false)
            //{
            //    if (transform.parent != null)
            //    {             
            //        if(transform.root.name == transform.parent.name)
            //        {
            //            transform.parent = null;
            //        }
            //        else
            //        {
            //            transform.parent = transform.root;
            //        }               
            //    }
            //}
        }
        private void Start()
        {
            OriginalFov = FOVToCheckForCoverValidation * 0.5f;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        //private void Update()
        //{
            //if (StartTimer == true)
            //{
            //    Timer += Time.deltaTime;
            //}

            //if (Timer > CheckTime)
            //{
            //    Info("VACANT", Color.green);
            //    IsCoverInUse = false;
            //    DebugCoverState = "Nobody is in this cover";
            //    TriggerOnce = false;
            //    StartTimer = false;
            //    Timer = 0f;
            //}
            //else if(TriggerOnce == true)
            //{
            //    Info("OCCUPIED", Color.red);
            //    DebugCoverState = "Cover is in use";
            //}


        //}
        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.gameObject.transform.root.tag == "AI")
        //    {

        //        // if (other.gameObject.transform.root.GetComponent<MasterAiBehaviour>() != null && other.gameObject.tag != "AIWeapon")
        //        //{
        //        //    if (TriggerOnce == false && other.gameObject.transform.root.GetComponent<MasterAiBehaviour>().CrouchPositions[other.gameObject.transform.root.GetComponent<MasterAiBehaviour>().CurrentCoverPoint].transform == this.transform)
        //        //    {
        //        //        ActivateStrafe = true;
        //        //        other.gameObject.transform.root.GetComponent<MasterAiBehaviour>().ChangeCover = false;
        //        //        other.gameObject.transform.root.GetComponent<MasterAiBehaviour>().Reachnewpoints = true;
        //        //        other.gameObject.transform.root.GetComponent<NavMeshAgent>().isStopped = true;
        //        //        TriggerOnce = true;
        //        //        CheckTime = other.gameObject.transform.root.GetComponent<MasterAiBehaviour>().SaveResetedCoverRandomisation;
        //        //    }
        //        //}
        //        if(other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CombatStarted == true && other.gameObject.transform.root.GetComponent<HumanoidAiHealth>().IsDied == false)
        //        {
        //            if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
        //            {
        //                if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CombatStateBehaviours.TakeCovers == true && AdvanceBookingForCover == true && DistanceCleared == true)
        //                {
        //                    CanProgressWithTrigger = false;
        //                    for (int x = 0; x < other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().AiCovers.ColliderExcludedFromCoverDetection.Length; x++)
        //                    {
        //                        if (other.gameObject.transform.GetComponent<Collider>() == other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().AiCovers.ColliderExcludedFromCoverDetection[x].GetComponent<Collider>())
        //                        {
        //                            CanProgressWithTrigger = true;
        //                        }

        //                    }


        //                    if (CanProgressWithTrigger == false && TriggerOnce == false && other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CrouchPositions[other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CurrentCoverPoint].transform == this.transform)
        //                    {
        //                        //ActivateStrafe = true;
        //                        other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().ChangeCover = false;
        //                        other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().Reachnewpoints = true;
        //                        other.gameObject.transform.root.GetComponent<NavMeshAgent>().isStopped = true;
        //                        TriggerOnce = true;
        //                        IsCoverInUse = true;
        //                        //CheckTime = other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().SaveResetedCoverRandomisation;

        //                        Info("OCCUPIED", Color.red);
        //                        DistanceCleared = false;

        //                        //if (TriggerOnce == true)
        //                        //{
        //                        //    StartTimer = true;
        //                        //}
        //                    }
        //                }

        //            }
        //        }

        //    }
        //}
        //private void OnTriggerStay(Collider other)
        //{
        //    if (other.gameObject.transform.root.tag == "AI")
        //    {
        //        if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CombatStarted == true && other.gameObject.transform.root.GetComponent<HumanoidAiHealth>().IsDied == false)
        //        {
        //            if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
        //            {
        //                if (CanProgressWithTrigger == false && other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CombatStateBehaviours.TakeCovers == true && AdvanceBookingForCover == true)
        //                {
        //                    if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().ChangeCover == true && other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CrouchPositions[other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CurrentCoverPoint].transform == this.transform)
        //                    {
        //                        StartTimer = false;
        //                        other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().Reachnewpoints = false;
        //                        Info("VACANT", Color.green);
        //                        IsCoverInUse = false;
        //                    }
        //                    else
        //                    {
        //                        if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().ChangeCover == false && other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CrouchPositions[other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CurrentCoverPoint].transform == this.transform)
        //                        {
        //                            other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().Reachnewpoints = true;
        //                            Info("OCCUPIED", Color.red);
        //                            IsCoverInUse = true;
        //                            if (TriggerOnce == true)
        //                            {
        //                                StartTimer = true;
        //                            }
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
        //        // StandCoverStrafes = true;
        //        IsCoverInUse = false;
        //        CanProgressWithTrigger = false;
        //        DoOnce = false;
        //        DistanceCleared = false;
        //        TriggerOnce = false;
        //        Info("VACANT", Color.green);
        //    }
        //}
        public void Info(Transform enemy, Targets Team)
        {
#if UNITY_EDITOR
            if (DebugCoverStateScript != null && DoOnce == false)
            {
                if(DebugCoverStateScript.EnableDebugging == true)
                {
                    for (int x = 0; x < DebugCoverStateScript.DebugTextColors.Count; x++)
                    {
                        if (DebugCoverStateScript.DebugTextColors[x].TeamName == Team.MyTeamID)
                        {
                            Index = x;
                        }
                    }


                    spawnedText.gameObject.SetActive(true);
                    spawnedText.text = "OCCUPIED";

                    spawnedText.color = DebugCoverStateScript.DebugTextColors[Index].OccupyingTextColor;


                    for (int x = 0; x < DebugCoverStateScript.combatcoverNodes.Length; x++)
                    {
                        if (DebugCoverStateScript.combatcoverNodes[x].gameObject != spawnedText.gameObject)
                        {
                            DebugCoverStateScript.combatcoverNodes[x].GetComponent<CoverNode>().CheckifEnemyIsInView(enemy);

                            if (DebugCoverStateScript.combatcoverNodes[x].GetComponent<CoverNode>().spawnedText.text != "OCCUPIED")
                            {
                                if (DebugCoverStateScript.combatcoverNodes[x].GetComponent<CoverNode>().IsValidCover == false)
                                {
                                    DebugCoverStateScript.combatcoverNodes[x].GetComponent<CoverNode>().spawnedText.gameObject.SetActive(true);
                                    DebugCoverStateScript.combatcoverNodes[x].GetComponent<CoverNode>().spawnedText.text = "INVALID";
                                    DebugCoverStateScript.combatcoverNodes[x].GetComponent<CoverNode>().spawnedText.color = DebugCoverStateScript.DebugTextColors[Index].InvalidTextColor;
                                }
                                else
                                {
                                    DebugCoverStateScript.combatcoverNodes[x].GetComponent<CoverNode>().spawnedText.gameObject.SetActive(true);
                                    DebugCoverStateScript.combatcoverNodes[x].GetComponent<CoverNode>().spawnedText.text = "VACANT";
                                    DebugCoverStateScript.combatcoverNodes[x].GetComponent<CoverNode>().spawnedText.color = DebugCoverStateScript.DebugTextColors[Index].VacantTextColor;
                                }
                            }
                        }
                    }

                    StartCoroutine(TextStayTime());

                    //}
                    //else if(Message == "Vacant")
                    //{
                    //    spawnedText.text = "VACANT";
                    //    spawnedText.color = Color.green;
                    //}
                    //else if (Message == "Invalid")
                    //{
                    //    spawnedText.text = "INVALID";
                    //    spawnedText.color = Color.green;
                    //}
                    DoOnce = true;
                }
                
            }
#endif

        }
        public void DeactivateOccupiedText()
        {
            if(spawnedText != null)
            {
                spawnedText.text = "VACANT";
                spawnedText.gameObject.SetActive(false);
            }
          
        }
        IEnumerator TextStayTime()
        {
            float Randomise = Random.Range(DebugCoverStateScript.MinDebugTextVisibilityTime, DebugCoverStateScript.MaxDebugTextVisibilityTime);
            yield return new WaitForSeconds(Randomise);
            for (int x = 0; x < DebugCoverStateScript.combatcoverNodes.Length; x++)
            {
                if (DebugCoverStateScript.combatcoverNodes[x].GetComponent<CoverNode>().spawnedText.text != "OCCUPIED")
                {
                    DebugCoverStateScript.combatcoverNodes[x].GetComponent<CoverNode>().spawnedText.gameObject.SetActive(false);
                }
            }
            DoOnce = false;

        }
        //IEnumerator CollisionChecks()
        //{
        //    float Randomise = Random.Range(MinTimeToCheckcollision, MaxTimeToCheckcollision);
        //    yield return new WaitForSeconds(Randomise);
        //    DoOnce = false;
        //    CanProgressWithTrigger = true;

        //}
        public void CheckifEnemyIsInView(Transform enemy)
        {
            dir = enemy.position - this.transform.position;
            Angle = Vector3.Angle(dir, this.transform.forward);

            //if(OpenFireCover == true)
            //{
            //    ValidCover = true;
            //}
            //else
            //{
            if (Angle <= OriginalFov)
            {
                IsValidCover = false;
            }
            else
            {
                IsValidCover = true;
            }

            // }

        }
        //public void CheckRotations(Transform enemy)
        //{
        //    // Calculate the direction from the current object's position to the enemy's position
        //    Vector3 direction = enemy.position - transform.position;

        //    // Convert the direction to local space
        //    Vector3 localDirection = transform.InverseTransformDirection(direction);

        //    // Calculate the local rotation needed to point towards the target on the Y-axis
        //    float localRotation = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;

        //    // Log the positive local rotation value on the Y-axis
        //    float positiveRotation = Mathf.Abs(localRotation);
        //    RotationIHaveToDo = positiveRotation;

        //}


    }
}