using System.Collections;
using UnityEngine;

namespace MobileActionKit
{
    // This Script is used To Detect enemies in field of view 
    public class HumanoidVisibilityChecker : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script is responsible for detecting the enemies in case field of view option is chosen for AI agent as main detection condition rather than just option of the Detection Radius alone." +
            "It takes into account the geometry of the game level and does the hacks for clear line of sight to the targets before triggering the combat state of Ai agent." +
            "This script also take part as a key element of another behaviour that depends on visibility factor within detection range." +
            "Which is enemy pursue behaviour that can be triggered if enemy becomes no longer visible by manoeuvring behind objects that obscure clear line of sight(strafes behind building or gets behind hiding covers). ";

        //[Tooltip("This field is responsible for creating a field of view angle with the value you put in the core Ai behaviour script." +
        //    "This gameobject will going to create a angle in the forward direction with the detection distance specified in the core Ai behaviour script." +
        //    "This object automatically becomes the child of head bone object i.e the field you see in this script." +
        //    "On visual field of view gameobject you can also add a script called 'Fov Visualisation' to be able to visualise the field of view in the scene view and in real time playmode.")]
        // public GameObject VisualFieldOfView;

        [Tooltip("This field is responsible for detecting  obstacles between the nearest enemy and this Ai agent. If Ai agent is in combat and his enemy gets himself hidden " +
            "behind the building.In that case raycast checker will do number of checks specified in the field named 'NumberOfRaycastChecksToDo' each frame to confirm that Ai agent can't see the Player." +
            "After the checks completed Ai agent will go in the Alerted state and will move to the last seen position of the enemy.")]
        public GameObject VisibilityObstacleChecker;

        [Tooltip("Drag and drop the head bone of this Ai agent from the hierarchy into this field. This script will then create a DetectionValidator object above this bone with vertical offset specified in the field(s)" +
            " named 'DetectionValidatorYOffset' (and DetectionValidatorZOffset). " +
            "The purpose of this validator is to prevent premature detection of the enemies that might be possible in cases when Ai agent is about to pass the corner of the building or the edge of some structure " +
            "(but not quite there yet) behind which the enemy is located. Because 'VisionObstacleEnemyRaycaster' is offset farther to the front of the AI agent (to not get the interference from AI agents body parts and child objects) " +
            "it will trigger the detection  the same frame at which it will acquire clear line of sight to the enemy. But because the head of AI agent is not yet reached clear line of sight it will cause AI agent to detect and " +
            "engage its target before he should be able to do so and might cause AI agent to shoot through the geometry(walls, vehicles etc. ). Thats where DetectionValidator cones into the picture as a final decisive factor of detection sequence. " +
            "DetectionValidator will ensure more natural visual detection if it would be much closer to the head of AI agent.")]
        public GameObject HeadBoneForDetectionValidator;

        [Tooltip("Specify the the Y offset of the DetectionValidator from the transform of the head bone so that its raycast would not be intersecting with head itself or any of the child objects of this AI agent." +
            "Together with the forward Z offset you can fine tune the position of DetectionValidator to your liking.")]
        public float DetectionValidatorYOffset;

        [Tooltip("Specify the the forward Z offset of the DetectionValidator from the transform of the head bone so that its raycast would not be intersecting with head itself or any of the child objects Of this AI agent." +
            "Together with the upward Y offset you can fine tune the position of DetectionValidator to your liking.")]
        public float DetectionValidatorZOffset = 0.1f;

        [Tooltip("Minimal delay of the detection validating event that triggers the combat state.")]
        public float MinDetectionValidatorDelay;

        [Tooltip("Maximal delay of the detection validating event that triggers the combat state.")]
        public float MaxDetectionValidatorDelay;

        [Tooltip("If checked will enable raycasts for deactivated and activated states of the DetectionValidator.")]
        public bool DebugDetectionValidatorRaycast = true;

        [Tooltip("This color indicates the activated state of the DetectionValidator. ")]
        public Color DetectionValidatorActivatedColor = Color.green;

        [Tooltip("This color indicates the deactivated state of the DetectionValidator.")]
        public Color DetectionValidatorDeactivatedColor = Color.yellow;


        [Tooltip("Minimum time between visibility raycast checks ")]
        public float MinTimeBetweenVisibilityChecks = 0.2f;
        [Tooltip("Maximum time between visibility raycast checks ")]
        public float MaxTimeBetweenVisibilityChecks = 0.4f;

        bool CanRayCastNow = true;
        bool RayCastCoroutineStarted = false;
        Vector3 rayDirection;
        Vector3 rayDirectionFromHeadBone;
        RaycastHit hitObj;

        private FindEnemies FindEnemiesScript;
        [HideInInspector]
        public bool MyTarget = false;
        [HideInInspector]
        public bool ConnectionLost = false;

        private SpineRotation rotateSpine;

        //[Header("Comment this line only for coding case")]
        // [HideInInspector]

        [Tooltip("Number of checks to do towards the nearest enemy if this Ai agent is unable to see its nearest enemy in a combat state.")]
        public int NumberOfRaycastChecksToDo = 1000;

        [Tooltip("Show the number of raycasts done to hidden enemy.")]
        public int DebugNumberOfRaycasts = 0;

        // bool CanMakeChangesToFOV = false;
        //  private FovVisualisation FOVEyes;

        //[Tooltip("Drag and drop head bone gameobject attached with the child of this gameobject into this field.")]
        //public Transform HeadBone;

        //  private MasterAiBehaviour MAB;
        private CoreAiBehaviour SAB;



        [Tooltip("Specify which layers to ignore when doing raycast checks for example : Patrol points with colliders attached that exist in a scene without a mesh needs to be avoided" +
          " from the raycast checks(i.e should be in layer for example - Ignore Raycast).")]
        public LayerMask IgnoredLayers;

        //public int AiLayerIndex = 8;

        bool ShouldStartEmptyShoot = false;

        Transform PreviousEnemy;
        [HideInInspector]
        public bool PauseRaycastCounts = false;

        int SuppressionProb;

        Vector3 GetHeadBonePosition;

        bool StartValidatorRaycasting = false;
        float DetectionValidatorDelay;

        [HideInInspector]
        public float Timer = 0;

        //[HideInInspector]
        //public bool ForcedCombatState = false;

        private void Awake()
        {
            //if (VisualFieldOfView.GetComponent<FovVisualisation>() != null)
            //{
            // FOVEyes = VisualFieldOfView.GetComponent<FovVisualisation>();
            //FOVDefaultTransform();
            //if (GetComponent<MasterAiBehaviour>() != null)
            //{
            //    MAB = GetComponent<MasterAiBehaviour>();
            //}
            if (GetComponent<CoreAiBehaviour>() != null)
            {
                SAB = GetComponent<CoreAiBehaviour>();
            }
            // CanMakeChangesToFOV = true;
            // }
        }
        private void Start()
        {
            rotateSpine = GetComponent<SpineRotation>();
            FindEnemiesScript = GetComponent<FindEnemies>();
            PreviousEnemy = transform;

            DetectionValidatorDelay = Random.Range(MinDetectionValidatorDelay, MaxDetectionValidatorDelay);
             SuppressionProb = Random.Range(0, 100);
        }
        private void Update()
        {
            if (SAB != null)
            {
                if (SAB.HealthScript != null)
                {
                    if (SAB.HealthScript.IsDied == false)
                    {
                        if (VisibilityObstacleChecker != null)
                        {
                            if (FindEnemiesScript.FindedEnemies == true)
                            {
                                if (FindEnemiesScript.enemy != null)
                                {
                                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                                    {
                                        VisibilityObstacleChecker.transform.LookAt(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform); // Make Sure Raycast Checker Always Look On Enemy So if enemy moves We Can Check if he is in raycast or not
                                    }
                                }
                            }
                        }
                    }
                }

            }

            //if (CanMakeChangesToFOV == true)
            //{
            //    if (FOVEyes != null)
            //    {
            //if (MAB != null)
            //{
            //    AdjustingFOV(MAB.CombatStarted, MAB.SprintingActivated, MAB.IsCrouched, MAB.SearchingForSound);
            //}
            //    }
            //}
        }
        IEnumerator EmptyShooting()
        {

            float Randomise = Random.Range(SAB.Components.HumanoidFiringBehaviourComponent.ShootingFeatures.MinSuppressionFireDuration, SAB.Components.HumanoidFiringBehaviourComponent.ShootingFeatures.MaxSuppressionFireDuration);
            yield return new WaitForSeconds(Randomise);

            if (SuppressionProb <= SAB.Components.HumanoidFiringBehaviourComponent.ShootingFeatures.SuppresionFireProbability)
            {
                SAB.StopWeaponShakeTemporary = true;
                SAB.Components.HumanoidFiringBehaviourComponent.HoldShoot = true;
            }
            else
            {
                SAB.StopWeaponShakeTemporary = false;
                SAB.Components.HumanoidFiringBehaviourComponent.HoldShoot = false;
            }
        }
        // This is added recently when testing leader and enemies fight. Its important that after leader get into the non combat state we make sure to reset the Visibility checks with enemy so next time the DetectionValidatorDelay
        // could work properly and enemies could be detected properly. this function is being called at CoreAiBehaviour Script when the agent get into the non combat state.
        public void ResetVisibilityWithEnemyWhenInNonCombat()
        {
            RayCastCoroutineStarted = false;
            CanRayCastNow = false;
            MyTarget = false;
            if(FindEnemiesScript.enemy.Count >= 1)
            {
                PreviousEnemy = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root;
            }
        }
        public bool CanSeeTarget(float FOVAngle, float RadiusToDetectEnemy, HumanoidAiWeaponFiringBehaviour HumanoidFiringBehaviourComponent)
        {
            //if (ForcedCombatState == true)
            //{
            //    return true;
            //}

            if (PreviousEnemy != FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root && FindEnemiesScript.IsEnemyAvailable == false && SAB.FindEnemiesScript.IsSet == true)
            {
                RayCastCoroutineStarted = false;
                CanRayCastNow = false;
                MyTarget = false;              
                PreviousEnemy = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root;
            }

            //if (EnabledFov == true)
            //{
            if (CanRayCastNow == true)
            {
                rayDirection = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.position - VisibilityObstacleChecker.transform.position;
            }

            //Debug.DrawRay(rotateSpine.BoneRotator.transform.position, rayDirection, Color.red);

            if (MyTarget == true && CanRayCastNow == true && SAB.FindEnemiesScript.IsSet == true)
            {
                //if (HumanoidFiringBehaviourComponent != null)
                //{
                //    if (HumanoidFiringBehaviourComponent.FireNow == true)
                //    {
                Raycasting(RadiusToDetectEnemy);
                //    }
                //}

                if (DebugDetectionValidatorRaycast == true)
                {
                    Debug.DrawRay(GetHeadBonePosition, rayDirectionFromHeadBone, DetectionValidatorActivatedColor);
                }

            }
            else
            {
                if(StartValidatorRaycasting == true)
                {
                    Timer = 0f;
                    StartValidatorRaycasting = false;
                }             
                if (DebugDetectionValidatorRaycast == true)
                {
                    Debug.DrawRay(GetHeadBonePosition, rayDirectionFromHeadBone, DetectionValidatorDeactivatedColor);
                }
            }

            if (HeadBoneForDetectionValidator != null)
            {
                GetHeadBonePosition = new Vector3(HeadBoneForDetectionValidator.transform.position.x, HeadBoneForDetectionValidator.transform.position.y + DetectionValidatorYOffset,
                    HeadBoneForDetectionValidator.transform.position.z + DetectionValidatorZOffset);
                if (CanRayCastNow == true)
                {
                    rayDirectionFromHeadBone = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.position - GetHeadBonePosition;
                }
            }
            //  Debug.DrawRay(GetHeadBonePosition, rayDirectionFromHeadBone * 1000f, Color.green,1f);
 
          

            if (SAB.FindEnemiesScript.EnableFieldOfView == true)
            {
                if (Vector3.Angle(rayDirection, SAB.FindEnemiesScript.FieldOfViewGameObject.transform.forward) <= FOVAngle && CanRayCastNow == true && SAB.FindEnemiesScript.IsSet == true)
                {
                    // Detect if target is within the field of view
                    if (MyTarget == false)
                    {
                        if (Physics.Raycast(VisibilityObstacleChecker.transform.position, rayDirection, out hitObj, RadiusToDetectEnemy, IgnoredLayers))
                        {
                            //Debug.DrawRay(RaycastChecker.transform.position, rayDirection, Color.red);
                            if (hitObj.transform.root == FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root)
                            {
                                if (HeadBoneForDetectionValidator != null)
                                {
                                    Timer += Time.deltaTime;

                                    if (Timer >= DetectionValidatorDelay)
                                    {
                                        StartValidatorRaycasting = true;                               
                                    }

                                    if (StartValidatorRaycasting == true)
                                    {

                                        if (Physics.Raycast(GetHeadBonePosition, rayDirectionFromHeadBone, out hitObj, RadiusToDetectEnemy, IgnoredLayers))
                                        {
                                            if (hitObj.transform.root == FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root)
                                            {
                                                ConnectionLost = false;
                                                MyTarget = true;
                                                DebugNumberOfRaycasts = 0;
                                                return (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform);
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    ConnectionLost = false;
                                    MyTarget = true;
                                    DebugNumberOfRaycasts = 0;
                                    return (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform);
                                }
                            }
                            else
                            {
                                if (PauseRaycastCounts == false)
                                {
                                    Timer = 0f;
                                    StartValidatorRaycasting = false;
                                    ++DebugNumberOfRaycasts;
                                    if (DebugNumberOfRaycasts > NumberOfRaycastChecksToDo)
                                    {
                                        DebugNumberOfRaycasts = NumberOfRaycastChecksToDo + 1;
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (CanRayCastNow == true && SAB.FindEnemiesScript.IsSet == false)
                {
                    return false;
                }
            }
            else
            {
                MyTarget = true;
            }


            if (RayCastCoroutineStarted == false)
            {
                StartCoroutine(CanRayCast());
                RayCastCoroutineStarted = true;
            }
            if (MyTarget == true)
            {
                return true;
            }
            else
            {
                return false;
            }
            //}
            //else
            //{
            //    return true;
            //}
        }
        public void Raycasting(float RadiusToDetectEnemy)
        {

            //if ((Vector3.Angle(rayDirection, rotateSpine.BoneRotator.transform.forward)) <= 360)
            //{
            if (Physics.Raycast(VisibilityObstacleChecker.transform.position, rayDirection, out hitObj, RadiusToDetectEnemy, IgnoredLayers)) // Before The Origin was : rotateSpine.BoneRotator.transform.position
            {
                //Debug.Log(hitObj.transform);
                //Debug.Log("ShouldStartEmptyShoot" + " " + ShouldStartEmptyShoot);
                //Debug.Log("HoldShoot" + " " + SAB.Components.HumanoidFiringBehaviourComponent.HoldShoot);
                //if (HumanoidFiringBehaviourComponent.FireNow == false) // before it was HumanoidFiringBehaviourComponent.IsFiring == false
                //{
                //    ++raycastCount;
                //    if (raycastCount > NumberOfRaycastChecksToDo)
                //    {
                //        MyTarget = false;
                //        ConnectionLost = true;
                //        CalculatePathOnce = false;
                //        ResetToNormalState = false;
                //    }
                //}
                //
                //Debug.Log(transform.name + "Object Hitting" + " " + hitObj.transform.name + "Enemy Name" + " " + FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root);
                if (hitObj.transform.root == FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root)
                {
                    //if (ShouldStartEmptyShoot == true)
                    //{
                        if (SAB != null)
                        {
                            if (SAB.Components.HumanoidFiringBehaviourComponent != null)
                            {
                                SAB.StopWeaponShakeTemporary = false;
                                SAB.Components.HumanoidFiringBehaviourComponent.HoldShoot = false;
                                ShouldStartEmptyShoot = false;
                            }

                        }
                    //}
                    DebugNumberOfRaycasts = 0;
                }
                else
                {
                    if (PauseRaycastCounts == false)
                    {
                        ++DebugNumberOfRaycasts;
                        if (DebugNumberOfRaycasts > NumberOfRaycastChecksToDo)
                        {
                            DebugNumberOfRaycasts = NumberOfRaycastChecksToDo + 1;
                        }
                    }
                    if (ShouldStartEmptyShoot == false)
                    {
                        if (SAB != null)
                        {
                            if (SAB.Components.HumanoidFiringBehaviourComponent != null)
                            {
                                if (SuppressionProb <= SAB.Components.HumanoidFiringBehaviourComponent.ShootingFeatures.SuppresionFireProbability)
                                {
                                    StartCoroutine(EmptyShooting());
                                    ShouldStartEmptyShoot = true;
                                }
                                else
                                {
                                    SAB.StopWeaponShakeTemporary = true;
                                    SAB.Components.HumanoidFiringBehaviourComponent.HoldShoot = true;
                                    ShouldStartEmptyShoot = true;
                                }
                            }
                        }
                    }
                    if (DebugNumberOfRaycasts > NumberOfRaycastChecksToDo)
                    {
                        MyTarget = false;
                        Timer = 0f;
                        StartValidatorRaycasting = false;
                        ConnectionLost = true;
                        FindEnemiesScript.NewEnemyLocked = false;
                        DebugNumberOfRaycasts = NumberOfRaycastChecksToDo + 1;
                    }
                }
            }
            // }
        }
        IEnumerator CanRayCast()
        {
            CanRayCastNow = false;
            float Randomise = Random.Range(MinTimeBetweenVisibilityChecks, MaxTimeBetweenVisibilityChecks);
            yield return new WaitForSeconds(Randomise);
            CanRayCastNow = true;
            RayCastCoroutineStarted = false;
        }

    }
}