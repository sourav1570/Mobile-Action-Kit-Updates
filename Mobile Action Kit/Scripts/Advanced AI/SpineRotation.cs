using UnityEngine;

// This Script is Responsible for Spine Rotation To The Target :

// Here is the explanation of this script the higher the bone rotation speed value the more precision shot will be made and to make the bone smoothly rotate from the current spine rotation we have to use the
// spine last fram variable so it can't be removed as when we don't use the procedural spine rotation during sprinting or scanning we need to make sure that we store the spine coordinates during those states so when we
// use the procedural spine rotation again we start from the current rotation always if we don't store the last spine rotation values even when we are not using it. this will create a snapping effect on the Ai agent

// now to solve the bot make precision shot always we can introduce distance based smooth bone rotation speed meaning if distance is 10 meters we can make the speed of bone rotation to be 10 as well this way bot will
// always make sure that he makes the most precision shot no matter the distance 
namespace MobileActionKit
{
    public class SpineRotation : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This Script Is Responsible For Rotating Ai Spine Bone .";
        [Space(10)]

        [Tooltip("The bone to be rotated by the Shooting point towards the target in combat state. " +
            "Select the appropriate bone of the rig somewhere in the middle of the spine. In the rig provided in this kit it is 'chest' bone.")]
        public Transform BoneToRotate;

        [Tooltip("Drag and drop the spine bone that is located right below the  'BoneToRotate' in the skeleton of Ai agent. " +
            "This bone will serve as a pivotal point for the rotating bone above it. In other words it will serve as a reference bone relative to which the rotations will be performed by the Shooting Point. " +
            "It will be parametrically   rotating BoneToRotate  around RotationalPivotBone which serves as a reference gameObject to correctly offset the rotational angle values of 'BoneToRotate'.")]
        public Transform RotationalPivotBone;

        [System.Serializable]
        public class DistanceScaling
        {
            [Tooltip("[Draft]The speed at which the BoneToRotate will rotate towards the target when the distance with target is less than the value mentioned in 'MinNearDistanceToTarget' field.")]
            public float InitialBoneRotationSpeed = 10f;

            [Tooltip("[Draft]Min near Distance at which bone rotation speed get changed.")]
            public float MinNearDistanceToTarget = 10f;
            [Tooltip("[Draft]Max near Distance at which bone rotation speed get changed.")]
            public float MaxNearDistanceToTarget = 50f;
            [Tooltip("[Draft]bone rotation speed value in Min/Max near distance.")]
            public float NearDistanceBoneRotationSpeed = 20f;

            [Tooltip("[Draft]Min mid Distance at which bone rotation speed get changed.")]
            public float MinMidDistanceToTarget = 50f;
            [Tooltip("[Draft]Max mid Distance at which bone rotation speed get changed.")]
            public float MaxMidDistanceToTarget = 200f;
            [Tooltip("[Draft]bone rotation speed value in Min/Max mid distance.")]
            public float MidDistanceBoneRotationSpeed = 40f;

            [Tooltip("[Draft]Min far Distance at which bone rotation speed get changed.")]
            public float MinFarDistance = 200f;
            [Tooltip("[Draft]Max far Distance at which bone rotation speed get changed.")]
            public float MaxFarDistance = 600f;
            [Tooltip("[Draft]bone rotation speed value in Min/Max far distance.")]
            public float FarDistanceBoneRotationSpeed = 50f;
        }

        [Tooltip("[Draft] The properties outlined in this section control the rotation of the 'spine' bone for the AI agent, determined by the distance to its enemy." +
            "For instance, if the distance to the enemy is greater, " +
            "increasing the bone rotation speed ensures more precise aiming towards the enemy." +
            "On the other hand, if the distance is shorter, a moderate speed like 6 - 7 is adequate for the AI agent to rotate its spine accurately and target the enemy effectively.")]
        [HideInInspector]
        public DistanceScaling CombatBoneRotationSpeed;

        [Tooltip("[Draft]The speed at which the BoneToRotate will rotate towards the target.")]
        [HideInInspector]
        public float NonCombatBoneRotationSpeed = 20f;

        [Tooltip("Specify vertical and lateral offset of the scan aiming pose for the 3 cases when Ai agent is performing scan aiming during walking, running and standing still," +
            " to fine-tune the spine bending angle for each of those 3 cases. This improves overall appearance of scan aiming as it helps to fit together" +
            " the best way the two separate upper body(for the torso) and lower body(for the legs) animations used together for those scanning aims. " +
            "This random aiming is a part of area scanning functionality of the various non - combat states such as sound and dead body  investigating, following the leader, " +
            "and stationary area scanning while standing.The best overall looking of scan aiming poses is achieved by dragging and dropping those scan aiming points" +
            " gameobjects into their respective fields and adjusting their position until the aiming pose looks the best it can." +
            "RunScanAimLevel and WalkScanAimLevel are used when Ai agent is on the move.StationaryScanAimLevel is for the cases when Ai agent stands still.")]
        public AimingPointsDuringNonCombatBehaviourClass ScanAimAdjustmentPoints;

        //[Tooltip("The minimum time interval to check the last rotation values of the spine.")]
        //public float MinCheckTimeForLastRotValues = 0.3f;

        //[Tooltip("The maximum time interval to check the last rotation values of the spine.")]
        //public float MaxCheckTimeForLastRotValues = 0.6f;

        [Header("Values to set the limits for 'BoneToRotate'  in order to prevent unnatural and exaggerated spine bending angles.")]
        [Tooltip("Enable clamping to limit the rotation angle of 'BoneToRotate'.")]
        public bool EnableClamping = true;

        [Tooltip("The minimum downward rotation angle (local X-axis) for 'BoneToRotate'.")]
        public float DownwardClampX = -60f;

        [Tooltip("The maximum upward rotation angle (local X-axis) for 'BoneToRotate'.")]
        public float UpwardClampX = 60f;

        [Tooltip("The minimum left rotation angle (local Y-axis) for 'BoneToRotate'.")]
        public float LeftClampY = -50f;

        [Tooltip("The maximum right rotation angle (local Y-axis) for 'BoneToRotate'.")]
        public float RightClampY = 80f;

        [Tooltip("The minimum right lean rotation angle (local Z-axis) for 'BoneToRotate'.")]
        public float RightLeanClampZ = -50f;

        [Tooltip("The maximum left lean rotation angle (local Z-axis) for 'BoneToRotate'.")]
        public float LeftLeanClampZ = 50f;


        [System.Serializable]
        public class AimingPointsDuringNonCombatBehaviourClass
        {
            // public CoreAiBehaviour CoreAiBehaviourScript;

            //public Transform SpineBone; // Drag and drop the spine bone into this field in the Inspector.  

            //  public Transform LookAtPointRootObject;
            //public Transform LookAtRotatorPoint;

            [Tooltip("The target GameObject that the bone rotator of this humanoid Ai agent assigned in this script will look at during scanning.")]
            public Transform StationaryScanAimLevel;

            //[Tooltip("The target GameObject that the Upper body of this humanoid Ai agent will face during walking.")]
            //public Transform LookAtPointDuringWalk;

            [Tooltip("The target GameObject that the bone rotator of this humanoid Ai agent assigned in this script will look at during walking.")]
            public Transform WalkScanAimLevel;

            [Tooltip("The target GameObject that the bone rotator of this humanoid Ai agent assigned in this script will look at during running.")]
            public Transform RunScanAimLevel;

            //[Tooltip("The target GameObject that the bone rotator of this humanoid Ai agent assigned in this script will face during Idle.")]
            //public Transform IdleAimPoint;

            //[Tooltip("The target GameObject that the Upper body of this humanoid Ai agent will face during sprinting.")]
            //public Transform LookAtPointDuringSprinting;

            //public float TransitioningSpeed = 5f;

            //public Vector3 DefaultSpineRotation = new Vector3(0f, 0f, 0f);

            //[HideInInspector]
            //[Tooltip("The rotation speed of the SpineBone during non-combat state.")]
            //public float RotationSpeed = 0.1f;

            //[HideInInspector]
            //[Tooltip("Control the influence of script rotation on the SpineBone during non-combat state.")]
            //public float scriptRotationWeight = 0.5f;
        }



        // public CombatBehaviourSpineRotation SpineRotationControlsInCombatState; 

        Vector3 OriginPoint;
        Quaternion spineRotationLastFrame;
        private FindEnemies FindEnemiesScript;

        [HideInInspector]
        public bool CanMoveSpineInCombatState = true;

        private HumanoidAiHealth HumanoidAiHealthScript;
        private Quaternion previousRotation;

        //[HideInInspector]
        //public float SpineWeightInAnimatorIk = 0f;

        //private Vector3 initialOffset;   // Initial offset between spine and target

        int maxIterations = 2;
        bool StoreSpineLastFrame = false;
        Quaternion StorespineRotationLastFrame;

        [HideInInspector] [Tooltip("Debug the current distance of this Ai agent to the enemy.")]
        public float DebugDistanceToEnemy;
        [HideInInspector] [Tooltip("Debug the current bone rotation speed of this Ai agent based on current distance the Ai agent is within.")]
        public float DebugBoneRotationSpeed;

        private void Awake()
        {
            FindEnemiesScript = GetComponent<FindEnemies>();
            if (GetComponent<HumanoidAiHealth>() != null)
            {
                HumanoidAiHealthScript = GetComponent<HumanoidAiHealth>();
            }
            if(RotationalPivotBone != null)
            {
                RotationalPivotBone.transform.localScale = Vector3.zero;
            }
        }
        public void ResettingDescription()
        {
            ScriptInfo = "This script is responsible for rotating Ai Spine Bone assigned to the Field and limiting its rotation values to prevent exaggerated rotations of the spine";
        }
        private void Start()
        {
            // initialOffset = SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position - SpineRotationControlsInCombatState.BoneRotator.position;

            //spineRotationLastFrame = BoneRotator.rotation;

            StoreSpineLastFramRotation();

            //if (BoneToRotate)
            //    spineRotationLastFrame = BoneToRotate.rotation;

            // float RandomiseLastFrame = Random.Range(SpineRotationControlsInCombatState.MinCheckTimeForLastRotValues, SpineRotationControlsInCombatState.MaxCheckTimeForLastRotValues);
            // InvokeRepeating("CheckForSpineLastFrame", RandomiseLastFrame, RandomiseLastFrame);
        }
        public void StoreSpineLastFramRotation()
        {
            spineRotationLastFrame = BoneToRotate.rotation; // correct code 

        }
        //public void CheckForSpineLastFrame() // Stores The Last Rotation Values of The Spine To Achieve the Smooth Rotation
        //{
        //    if (BoneToRotate)
        //        spineRotationLastFrame = BoneToRotate.rotation; 
        //}
        public void PartToRotate(Vector3 EnemyPosition)
        {
            if (FindEnemiesScript.FindedEnemies == true)
            {
                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                {
                    if (HumanoidAiHealthScript != null)
                    {
                        if (HumanoidAiHealthScript.IsDied == false)
                        {
                            if (CanMoveSpineInCombatState == true)
                            {
                                // Previous Code //
                                float Distance = float.PositiveInfinity;
                                float MinimumDistance = 0.01f;
                                int Iteration = 30;

                                while (Distance > MinimumDistance && Iteration > 0)
                                {
                                    Vector3 AimingPoint = RotationalPivotBone.TransformPoint(RotationalPivotBone.position);

                                    if (FindEnemiesScript.FindedEnemies == true)
                                    {
                                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null && BoneToRotate != null) // Checking if The Enemy is Alive 
                                        {
                                            if (!LineIntersection(out OriginPoint, AimingPoint, -RotationalPivotBone.forward, BoneToRotate.forward, BoneToRotate.position))
                                            {
                                                OriginPoint = AimingPoint;
                                            }
                                        }
                                    }

                                    Vector3 WeaponForward = RotationalPivotBone.forward;
                                    Vector3 AimingForwardPoint = EnemyPosition - OriginPoint;

                                    if (FindEnemiesScript.FindedEnemies == true)
                                    {
                                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null) // Checking if The Enemy is Alive 
                                        {
                                            Quaternion rot = Quaternion.FromToRotation(WeaponForward, AimingForwardPoint);
                                            Distance = rot.eulerAngles.sqrMagnitude;
                                            Iteration--;

                                            if (BoneToRotate != null)
                                            {
                                                BoneToRotate.rotation = rot * BoneToRotate.rotation;
                                            }
                                        }
                                    }
                                }

                                float distanceToEnemy = Vector3.Distance(RotationalPivotBone.position, EnemyPosition);
                                DebugDistanceToEnemy = distanceToEnemy;

                                if (distanceToEnemy > CombatBoneRotationSpeed.MinNearDistanceToTarget && distanceToEnemy < CombatBoneRotationSpeed.MaxNearDistanceToTarget)
                                {
                                    BoneToRotate.rotation = Quaternion.Slerp(spineRotationLastFrame, BoneToRotate.rotation, Time.deltaTime * CombatBoneRotationSpeed.NearDistanceBoneRotationSpeed);
                                    spineRotationLastFrame = BoneToRotate.rotation;
                                    DebugBoneRotationSpeed = CombatBoneRotationSpeed.NearDistanceBoneRotationSpeed;
                                }
                                else if (distanceToEnemy > CombatBoneRotationSpeed.MinMidDistanceToTarget && distanceToEnemy < CombatBoneRotationSpeed.MaxMidDistanceToTarget)
                                {
                                    BoneToRotate.rotation = Quaternion.Slerp(spineRotationLastFrame, BoneToRotate.rotation, Time.deltaTime * CombatBoneRotationSpeed.MidDistanceBoneRotationSpeed);
                                    spineRotationLastFrame = BoneToRotate.rotation;
                                    DebugBoneRotationSpeed = CombatBoneRotationSpeed.MidDistanceBoneRotationSpeed;
                                }
                                else if (distanceToEnemy > CombatBoneRotationSpeed.MinFarDistance)// && distanceToEnemy < CombatBoneRotationSpeeds.MaxFarDistance)
                                {
                                    BoneToRotate.rotation = Quaternion.Slerp(spineRotationLastFrame, BoneToRotate.rotation, Time.deltaTime * CombatBoneRotationSpeed.FarDistanceBoneRotationSpeed);
                                    spineRotationLastFrame = BoneToRotate.rotation;
                                    DebugBoneRotationSpeed = CombatBoneRotationSpeed.FarDistanceBoneRotationSpeed;
                                }
                                else
                                {
                                    BoneToRotate.rotation = Quaternion.Slerp(spineRotationLastFrame, BoneToRotate.rotation, Time.deltaTime * CombatBoneRotationSpeed.InitialBoneRotationSpeed);
                                    spineRotationLastFrame = BoneToRotate.rotation;
                                    DebugBoneRotationSpeed = CombatBoneRotationSpeed.InitialBoneRotationSpeed;
                                }
                                //else if (distanceToEnemy > CombatBoneRotationSpeeds.MaxFarDistance)
                                //{
                                //    float adjustedRotationSpeed = Mathf.Lerp(1f, 1000f, distanceToEnemy / 1000f);
                                //    BoneToRotate.rotation = Quaternion.Slerp(spineRotationLastFrame, BoneToRotate.rotation, Time.deltaTime * adjustedRotationSpeed);
                                //    spineRotationLastFrame = BoneToRotate.rotation;
                                //    DebugBoneRotationSpeed = adjustedRotationSpeed;
                                //}





                                //float distDelta = 129601;
                                //int itsNow = 10;

                                //while (itsNow > 0 && distDelta > 1)
                                //{
                                //    float rayDistance;
                                //    Vector3 origin = BoneRotator.TransformPoint(BoneRotator.position);
                                //    Plane plane = new Plane(BoneToRotate.forward, BoneToRotate.position);

                                //    if (plane.Raycast(new Ray(BoneRotator.position, -BoneRotator.forward), out rayDistance))
                                //    {
                                //        origin = BoneRotator.position + BoneRotator.forward * rayDistance;
                                //    }

                                //    Quaternion rotationIteration = Quaternion.FromToRotation(BoneRotator.forward, EnemyPosition - origin);
                                //    distDelta = rotationIteration.eulerAngles.sqrMagnitude;
                                //    itsNow--;

                                //    BoneToRotate.rotation = rotationIteration * BoneToRotate.rotation;
                                //}

                                //BoneToRotate.rotation = Quaternion.Slerp(spineRotationLastFrame, BoneToRotate.rotation, Time.deltaTime * 5f);
                                //spineRotationLastFrame = BoneToRotate.rotation;





















                                //float distanceToEnemy = Vector3.Distance(BoneRotator.position, EnemyPosition);
                                //float adjustedRotationSpeed = Mathf.Lerp(1f,1000f, distanceToEnemy / 1000f);

                                //BoneToRotate.rotation = Quaternion.Slerp(spineRotationLastFrame, BoneToRotate.rotation, Time.deltaTime * adjustedRotationSpeed);

                                //BoneRotationSpeedInCombat = adjustedRotationSpeed;


                                //float distanceToEnemy = Vector3.Distance(BoneRotator.position, EnemyPosition);
                                //float adjustedRotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, distanceToEnemy / maxDistance);

                                //// Calculate the rotation to align the bone's forward direction with the aiming direction
                                //Quaternion targetRotation = Quaternion.LookRotation(AimingForwardPoint, Vector3.up);

                                //// Smoothly interpolate towards the target rotation
                                //BoneToRotate.rotation = Quaternion.Slerp(BoneToRotate.rotation, targetRotation, Time.deltaTime * adjustedRotationSpeed);




                                //float Distance = float.PositiveInfinity;
                                //float MinimumDistance = 0.01f;
                                //int Iteration = 30;
                                //float rotationTime = 10f;
                                //float elapsedTime = 0f;

                                //while (Distance > MinimumDistance && Iteration > 0)
                                //{
                                //    Vector3 AimingPoint = BoneRotator.TransformPoint(BoneRotator.position);

                                //    if (FindEnemiesScript.FindedEnemies == true)
                                //    {
                                //        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null && BoneToRotate != null)
                                //        {
                                //            if (!LineIntersection(out OriginPoint, AimingPoint, -BoneRotator.forward, BoneToRotate.forward, BoneToRotate.position))
                                //            {
                                //                OriginPoint = AimingPoint;
                                //            }
                                //        }
                                //    }

                                //    Vector3 WeaponForward = BoneRotator.forward;
                                //    Vector3 AimingForwardPoint = EnemyPosition - OriginPoint;

                                //    if (FindEnemiesScript.FindedEnemies == true)
                                //    {
                                //        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                                //        {
                                //            Quaternion rot = Quaternion.FromToRotation(WeaponForward, AimingForwardPoint);
                                //            Distance = rot.eulerAngles.sqrMagnitude;
                                //            Iteration--;

                                //            if (BoneToRotate != null)
                                //            {
                                //                Quaternion targetRotation = rot * BoneToRotate.rotation;

                                //                if (targetRotation != BoneToRotate.rotation)
                                //                {
                                //                    elapsedTime += Time.deltaTime;
                                //                    float t = Mathf.Clamp01(elapsedTime / rotationTime);
                                //                    BoneToRotate.rotation = Quaternion.Slerp(spineRotationLastFrame, targetRotation, t);
                                //                }
                                //            }
                                //        }
                                //    }
                                //}

                                //if (Distance > MinimumDistance)
                                //{
                                //    elapsedTime = 0f;
                                //}

                                //spineRotationLastFrame = BoneToRotate.rotation;





















                                //// New Code //
                                //float distDelta = 129601;
                                //int itsNow = maxIterations;

                                //while (itsNow > 0 && distDelta > 5f)
                                //{
                                //    float rayDistance;
                                //    Vector3 origin = SpineRotationControlsInCombatState.BoneRotator.TransformPoint(SpineRotationControlsInCombatState.BoneRotator.position);
                                //    Plane plane = new Plane(SpineRotationControlsInCombatState.BoneToRotate.forward, SpineRotationControlsInCombatState.BoneRotator.position);

                                //    if (plane.Raycast(new Ray(SpineRotationControlsInCombatState.BoneRotator.position, -SpineRotationControlsInCombatState.BoneRotator.forward), out rayDistance))
                                //    {
                                //        origin = SpineRotationControlsInCombatState.BoneRotator.position + SpineRotationControlsInCombatState.BoneRotator.forward * rayDistance;
                                //    }

                                //    Quaternion rotationIteration = Quaternion.FromToRotation(SpineRotationControlsInCombatState.BoneRotator.forward, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - origin);
                                //    distDelta = rotationIteration.eulerAngles.sqrMagnitude;
                                //    itsNow--;

                                //    SpineRotationControlsInCombatState.BoneToRotate.rotation = rotationIteration * SpineRotationControlsInCombatState.BoneToRotate.rotation;
                                //}

                                //SpineRotationControlsInCombatState.BoneToRotate.rotation = Quaternion.Slerp(spineRotationLastFrame, SpineRotationControlsInCombatState.BoneToRotate.rotation, Time.deltaTime * SpineRotationControlsInCombatState.BoneRotationSpeed);
                                //spineRotationLastFrame = SpineRotationControlsInCombatState.BoneToRotate.rotation;


                                //Debug.DrawRay(SpineRotationControlsInCombatState.BoneRotator.position, SpineRotationControlsInCombatState.BoneRotator.forward * 1000, Color.red);


                                ClamSpine();
                                //// Get the rotation of the spine
                                //Quaternion spineRotation = SpineRotationControlsInCombatState.BoneRotator.rotation;

                                //// Calculate the new target position based on the spine's rotation
                                //Vector3 newTargetPosition = SpineRotationControlsInCombatState.BoneRotator.position + spineRotation * initialOffset;

                                //// Update the target object's position
                                //SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position = newTargetPosition;


                                //Vector3 targetPosition = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].TransformPoint(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].localPosition);

                                //SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position =
                                //Vector3.MoveTowards(SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position,
                                //targetPosition, SpineRotationControlsInNonCombatState.TransitioningSpeed * Time.deltaTime);

                                //if (BoneToRotate != null)
                                //{
                                //    BoneToRotate.rotation = Quaternion.Slerp(spineRotationLastFrame, BoneToRotate.rotation, Time.deltaTime * BoneRotationSpeed);
                                //    spineRotationLastFrame = BoneToRotate.rotation;
                                //}
                            }
                        }
                    }

                }
            }
        }
        public void SpineRotationDuringEmergencyShoot(Vector3 EnemyPosition)
        {
            if (FindEnemiesScript.FindedEnemies == true)
            {
                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                {
                    if (HumanoidAiHealthScript != null)
                    {
                        if (HumanoidAiHealthScript.IsDied == false)
                        {
                            if (CanMoveSpineInCombatState == true)
                            {
                                float Distance = float.PositiveInfinity;
                                float MinimumDistance = 0.01f;
                                int Iteration = 30;

                                while (Distance > MinimumDistance && Iteration > 0)
                                {
                                    Vector3 AimingPoint = RotationalPivotBone.TransformPoint(RotationalPivotBone.position);

                                    if (FindEnemiesScript.FindedEnemies == true)
                                    {
                                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null && BoneToRotate != null) // Checking if The Enemy is Alive 
                                        {
                                            if (!LineIntersection(out OriginPoint, AimingPoint, -RotationalPivotBone.forward, BoneToRotate.forward, BoneToRotate.position))
                                            {
                                                OriginPoint = AimingPoint;
                                            }
                                        }
                                    }

                                    Vector3 WeaponForward = RotationalPivotBone.forward;
                                    Vector3 AimingForwardPoint = EnemyPosition - OriginPoint;

                                    if (FindEnemiesScript.FindedEnemies == true)
                                    {
                                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null) // Checking if The Enemy is Alive 
                                        {
                                            Quaternion rot = Quaternion.FromToRotation(WeaponForward, AimingForwardPoint);
                                            Distance = rot.eulerAngles.sqrMagnitude;
                                            Iteration--;

                                            if (BoneToRotate != null)
                                            {
                                                BoneToRotate.rotation = rot * BoneToRotate.rotation;
                                            }
                                        }
                                    }
                                }

                                float distanceToEnemy = Vector3.Distance(RotationalPivotBone.position, EnemyPosition);
                                DebugDistanceToEnemy = distanceToEnemy;

                                BoneToRotate.rotation = Quaternion.Slerp(spineRotationLastFrame, BoneToRotate.rotation, Time.deltaTime * 7f);
                                spineRotationLastFrame = BoneToRotate.rotation;
                                DebugBoneRotationSpeed = CombatBoneRotationSpeed.NearDistanceBoneRotationSpeed;

                            }
                        }
                    }

                }
            }
        }
        public void ClamSpine()
        {
            if (BoneToRotate != null)
            {
                if (EnableClamping == true) // Clamping The Spine Rotation 
                {
                    var MgangleX = ClampAngle.ClampAngles(BoneToRotate.localEulerAngles.x, DownwardClampX, UpwardClampX);
                    var MgangleY = ClampAngle.ClampAngles(BoneToRotate.localEulerAngles.y, LeftClampY, RightClampY);
                    var MgangleZ = ClampAngle.ClampAngles(BoneToRotate.localEulerAngles.z, RightLeanClampZ, LeftLeanClampZ);
                    BoneToRotate.localEulerAngles = new Vector3(MgangleX, MgangleY, MgangleZ);
                }
            }
        }
        public void NonCombatSpineBoneRotations(Transform LookAtPoint)
        {
            if (LookAtPoint != null)
            {
                // Previous Code //
                float Distance = float.PositiveInfinity;
                float MinimumDistance = 0.01f;
                int Iteration = 30;

                while (Distance > MinimumDistance && Iteration > 0)
                {
                    Vector3 AimingPoint = RotationalPivotBone.TransformPoint(RotationalPivotBone.position);

                    if (LookAtPoint != null && BoneToRotate != null) // Checking if The Enemy is Alive 
                    {
                        if (!LineIntersection(out OriginPoint, AimingPoint, -RotationalPivotBone.forward, BoneToRotate.forward, BoneToRotate.position))
                        {
                            OriginPoint = AimingPoint;
                        }
                    }

                    if (LookAtPoint != null) // Checking if The Enemy is Alive 
                    {
                        Vector3 WeaponForward = RotationalPivotBone.forward;
                        Vector3 AimingForwardPoint = LookAtPoint.position - OriginPoint;

                        Quaternion rot = Quaternion.FromToRotation(WeaponForward, AimingForwardPoint);
                        Distance = rot.eulerAngles.sqrMagnitude;
                        Iteration--;

                        if (BoneToRotate != null)
                        {
                            BoneToRotate.rotation = rot * BoneToRotate.rotation;
                        }
                    }

                }

                BoneToRotate.rotation = Quaternion.Slerp(spineRotationLastFrame, BoneToRotate.rotation, Time.deltaTime * NonCombatBoneRotationSpeed);
                spineRotationLastFrame = BoneToRotate.rotation;

                ClamSpine();
            }

        }
        //public void ChangeLookAtPoint(Vector3 LookAtPoint)
        //{
        //    if (LookAtPoint != null && SpineRotationControlsInNonCombatState.CoreAiBehaviourScript.anim != null)
        //    {
        //        SpineRotationControlsInNonCombatState.CoreAiBehaviourScript.anim.SetLookAtPosition(LookAtPoint);
        //        SpineRotationControlsInNonCombatState.CoreAiBehaviourScript.anim.SetLookAtWeight(1f, SpineWeightInAnimatorIk, 0f, 0f, 0.5f);
        //        SpineWeightInAnimatorIk = 0.5f;
        //    }
        //}
        //private void OnAnimatorIK(int layerIndex)
        //{
        //    //Quaternion targetRot;
        //    //targetRot = SpineRotationControlsInCombatState.BoneToRotate.rotation;
        //    //SpineRotationControlsInCombatState.BoneToRotate.rotation = Quaternion.Slerp(spineRotationLastFrame, targetRot, Time.deltaTime * SpineRotationControlsInCombatState.BoneRotationSpeed * 2);

        //    if (SpineRotationControlsInNonCombatState.ModifyUpperBodyRotations == true && SpineRotationControlsInNonCombatState.CoreAiBehaviourScript.StopSpineRotation == true)
        //    {
        //        spineRotationLastFrame = SpineRotationControlsInCombatState.BoneToRotate.rotation;


        //        if (SpineRotationControlsInNonCombatState.CoreAiBehaviourScript.ActivateRunningIk == true)
        //        {
        //            SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position =
        //                Vector3.MoveTowards(SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position,
        //                SpineRotationControlsInNonCombatState.LookAtPointDuringRunning.position
        //                , SpineRotationControlsInNonCombatState.TransitioningSpeed * Time.deltaTime);
        //            //ChangeLookAtPoint(SpineRotationControlsInNonCombatState.CoreAiBehaviourScript.RotateSpine.SpineRotationControlsInNonCombatState.LookAtPointDuringRunning.position);
        //            ChangeLookAtPoint(SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position);
        //        }
        //        else if (SpineRotationControlsInNonCombatState.CoreAiBehaviourScript.ActivateWalkIk == true)
        //        {
        //            SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position =
        //             Vector3.MoveTowards(SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position,
        //              SpineRotationControlsInNonCombatState.LookAtPointDuringWalk.position
        //             , SpineRotationControlsInNonCombatState.TransitioningSpeed * Time.deltaTime);

        //            ChangeLookAtPoint(SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position);

        //        }
        //        else if (SpineRotationControlsInNonCombatState.CoreAiBehaviourScript.ActivateScanIk == true)
        //        {

        //            SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position =
        //            Vector3.MoveTowards(SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position,
        //             SpineRotationControlsInNonCombatState.LookAtPointDuringScanning.position
        //            , SpineRotationControlsInNonCombatState.TransitioningSpeed * Time.deltaTime);

        //            ChangeLookAtPoint(SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position);


        //        }
        //        else if (SpineRotationControlsInNonCombatState.CoreAiBehaviourScript.ActivateSprintingIk == true)
        //        {
        //            SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position =
        //           Vector3.MoveTowards(SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position,
        //           SpineRotationControlsInNonCombatState.LookAtPointDuringSprinting.position
        //           , SpineRotationControlsInNonCombatState.TransitioningSpeed * Time.deltaTime);

        //            ChangeLookAtPoint(SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position);
        //        }
        //        else if (SpineRotationControlsInNonCombatState.CoreAiBehaviourScript.ActivateWalkAimIk == true)
        //        {
        //            SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position =
        //         Vector3.MoveTowards(SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position,
        //          SpineRotationControlsInNonCombatState.LookAtPointDuringWalkAim.position
        //         , SpineRotationControlsInNonCombatState.TransitioningSpeed * Time.deltaTime);

        //            ChangeLookAtPoint(SpineRotationControlsInNonCombatState.LookAtRotatorPoint.position);

        //        }
        //    }
        //}

        //public void SpineRotationInNonCombatSituation(Transform focuspoint)
        //{
        //    if (focuspoint == null || SpineRotationControlsInNonCombatState.SpineBone == null)
        //        return;

        //    // Calculate the direction to the target
        //    Vector3 targetDirection = focuspoint.position - SpineRotationControlsInNonCombatState.SpineBone.position;
        //    //  targetDirection.y = 0f; // Make sure the rotation is only on the Y-axis (horizontal plane)

        //    if (targetDirection != Vector3.zero)
        //    {
        //        // Calculate the rotation angle to the target
        //        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        //        // Smoothly rotate the spine bone towards the target rotation using script
        //        Quaternion blendedRotation = Quaternion.Slerp(SpineRotationControlsInNonCombatState.SpineBone.rotation, targetRotation, SpineRotationControlsInNonCombatState.RotationSpeed * Time.deltaTime);

        //        // Blend the script rotation with the animation rotation based on scriptRotationWeight
        //        SpineRotationControlsInNonCombatState.SpineBone.rotation = Quaternion.Lerp(SpineRotationControlsInNonCombatState.SpineBone.rotation, blendedRotation, SpineRotationControlsInNonCombatState.scriptRotationWeight);
        //    }

        //   // previousRotation = SpineRotationControlsInNonCombatState.SpineBone.rotation;
        //}
        //public void SpinePreviousRotationInNonCombatSituation()
        //{
        //    if (previousRotation != Quaternion.identity)
        //    {
        //        SpineRotationControlsInNonCombatState.SpineBone.rotation = Quaternion.Lerp(SpineRotationControlsInNonCombatState.SpineBone.rotation, previousRotation, SpineRotationControlsInNonCombatState.RotationSpeed * Time.deltaTime);
        //    }
        //}
        private static bool LineIntersection(out Vector3 Intersect, Vector3 LinePoint, Vector3 LineVector, Vector3 PlaneNormal, Vector3 PlaneVector)
        {
            float DotNumerator = Vector3.Dot(PlaneVector - LinePoint, PlaneNormal);
            float DotDenominator = Vector3.Dot(LineVector, PlaneNormal);

            if (DotDenominator != 0.0f)
            {
                Intersect = LinePoint + LineVector.normalized * (DotNumerator / DotDenominator);
                return true;
            }
            else
            {
                Intersect = Vector3.zero;
                return false;
            }
        }
    }
}
