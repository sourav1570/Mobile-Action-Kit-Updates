using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class HeadIK : MonoBehaviour
    {

        // Public Variables
        [TextArea]
        public string ScriptInfo = "The script fields are responsible for rotating Ai agent's head to look at the assigned points either randomly or sequentially during patrolling, wandering ,scanning and idle states.";

        [Tooltip("Drag and drop the 'CoreAiBehaviour' component attached with this Ai agent from the hierarchy into this field.")]
        public CoreAiBehaviour CoreAiBehaviourScript;

    
        [Tooltip("The minimum speed with which the head bone of the Ai agent will turn towards LookAt points.")]
        public float MinHeadTurningSpeed = 12f;

        [Tooltip("The maximum speed with which the head bone of the Ai agent will turn towards LookAt points.")]
        public float MaxHeadTurningSpeed = 12f;
        //[Tooltip("")]
        //public float MinHorizontalSearching = -100;
        //public float MaxHorizontalSearching = 100;
        //public float MinLookUp = 0;
        //[Tooltip("Sets the limit for a random placements for a points of interested 1000 unity world units higher than current position " +
        //    "of a Ai Agent root object along Y Axis in unity world units")]
        //public float MaxLookUp = 1000;
        //public float MinLookDown = -100;
        //public float MaxLookDown = 0;

        //[Tooltip("Head Rotation To Look Up")]
        //public float MinTimeHorizontalLook = 5f;
        //[Tooltip("Head Rotation To Look Up")]
        //public float MaxTimeHorizontalLook = 10f;
        //[Tooltip("Head Rotation To Look Up")]
        //public float MinTimeLookUp = 5f;
        //[Tooltip("Head Rotation To Look Up")]
        //public float MaxTimeLookUp = 10f;
        //[Tooltip("Head Rotation To Look Down")]
        //public float MinTimeLookDown = 5f;
        //[Tooltip("Head Rotation To Look Down")]
        //public float MaxTimeToLookDown = 10f;
        //[Tooltip("Head Rotation Original Values To Reset")]
        //public float TimeToCreatePointInWorldSpace = 2f;

        //[Tooltip("Initial horizontal anchor point created on the navmesh." +
        //    "After its creation a second point of interest gets created along the anchor's point Y Axis." +
        //    "Point of interest inherits the world values along Z and X Axis from anchor point." +
        //    "This functionality results a behaviour similar to human randomly looking up,down,left and right")]
        //public float AnchorPointDistance = 2f;

        //public bool LookDown = false;
        //public bool LookUp = false;

        // public bool EnableCustomPointOfInterest = true;
        [Tooltip("Minimum time to stare at LookAt point before turning towards another LookAt point.")]
        public float MinStareTime;
        [Tooltip("Maximum time to stare at LookAt point before turning towards another LookAt point.")]
        public float MaxStareTime;

        [Tooltip("If checked it will keep the head turning independent of scanning turns directions and unsync with them.")]
        public bool UnlinkLookAtPoints = true;
        [Tooltip("If enabled than Ai agents head will rotate and look at the points randomly.If disabled then Ai agent will look at them in recurring fashion (0,1,2,0,1,2 etc).")]
        public bool RandomRotation = false;

        [Tooltip("Assign child LookAtPoints from the hierarchy of this Ai agent to this list." +
            "They can be child game objects of Ai agent named and positioned according to their direction name(front, left, right, down, up, front up, left up, etc.) " +
            "in which case the Ai agent head will be turning between them.Alternatively those LookAt points can be independently placed at a certain spots on the level and would not be child game objects of Ai agent." +
            "This would force Ai agent to focus on those independent LookAt points thus ensuring that Ai agent will concentrate on those directions." +
            "This would be useful for example for sniper bot observing only few particular areas of the level(certain building or a crossroad etc.).")]
        public Transform[] LookAtPoints;

        [HideInInspector]
        public int PreviousValue = 0;
        [HideInInspector]
        public int ScanValue = -1;

        [HideInInspector]
        public float RandomiseProximityTimes = 0f;


        //Transform closestDir;

        //List<Transform> ClosestPoint = new List<Transform>();
        //[HideInInspector]
        //public string DisplayClosestDirection;
        //    [Tooltip("Add multiple custom directions with there particular animation to be played using this list.")]
        //public List<FollowPointofinterestScan> PointOfInterestScan = new List<FollowPointofinterestScan>();

        bool DoCustomvaluesetup = false;

        [HideInInspector]
        public bool Check = false;

        bool GenerateADetectPointForHead = false;
        Vector3 RandomPoint;
        bool ChangePosOfRandomPointWhileSearch = false;
        Vector3 newpos;
        bool LookWhileSearching = false;
        bool LookDownWhileSearching = false;
        bool CanCreateaRandomPoint = false;

        bool ReInitializeHeadPointToLookAt = false;

        bool IsFirstInitialisationCompleted = false;
        float HeadTurningSpeed;

        [HideInInspector]
        public bool StopTurning = false;


        public void ResettingDescription()
        {
            ScriptInfo = "The script fields are responsible for rotating the head to look at the assigned points either randomly or sequentially during patroling, wandering ,scanning and idle states.";

        }
        public void Start()
        {
            if (UnlinkLookAtPoints == true)
            {
                for (int x = 0; x < LookAtPoints.Length; x++)
                {
                    LookAtPoints[x].transform.parent = null;
                }
            }

            HeadTurningSpeed = Random.Range(MinHeadTurningSpeed, MaxHeadTurningSpeed);
            if (CoreAiBehaviourScript.NonCombatBehaviours.EnableHeadRotations == true)
            {
                if (RandomRotation == true)
                {
                    ScanValue = Random.Range(0, LookAtPoints.Length);
                }
                RandomiseProximityTimes = Random.Range(MinStareTime, MaxStareTime);
                InvokeRepeating("LookAtCustomPoint", 0.0f, RandomiseProximityTimes);
            }
        }
        public void LookAtCustomPoint()
        {
            HeadTurningSpeed = Random.Range(MinHeadTurningSpeed, MaxHeadTurningSpeed);
            if (DoCustomvaluesetup == true)
            {
                PreviousValue = ScanValue;
            }

            if (RandomRotation == true)
            {
                ScanValue = Random.Range(0, LookAtPoints.Length);
            }
            else
            {
                if (ScanValue < LookAtPoints.Length)
                {
                    if (ScanValue == LookAtPoints.Length - 1)
                    {
                        ScanValue = 0;
                    }
                    else
                    {
                        ++ScanValue;
                    }

                }
            }

            if (DoCustomvaluesetup == false)
            {
                PreviousValue = ScanValue;
                DoCustomvaluesetup = true;
            }

            Check = false;
            IsFirstInitialisationCompleted = true;
        }
        private void OnAnimatorIK(int layerIndex)
        {
            if (CoreAiBehaviourScript.NonCombatBehaviours.EnableHeadRotations == true
                && CoreAiBehaviourScript.CombatStarted == false && CoreAiBehaviourScript.SearchingForSound == false && IsFirstInitialisationCompleted == true
                && CoreAiBehaviourScript.IsNearDeadBody == false && CoreAiBehaviourScript.IsBodyguard == false && CoreAiBehaviourScript.IsAgentRoleLeader == false && StopTurning == false)
            {
                ControlHeadIK();
                CoreAiBehaviourScript.anim.SetLookAtWeight(1f, 0f, 1f, 1f, 0.5f);
            }
            else
            {
                ReInitializeHeadPointToLookAt = false;
            }
        }
        public void ControlHeadIK()
        {
            //if (EnableCustomPointOfInterest == true)
            //{

            if (ReInitializeHeadPointToLookAt == false)
            {
                RandomPoint = LookAtPoints[PreviousValue].transform.position;
                ReInitializeHeadPointToLookAt = true;
            }

            //if (Check == false)
            //{           
            //    Check = true;
            //}

            RandomPoint = Vector3.MoveTowards(RandomPoint, LookAtPoints[ScanValue].transform.position, HeadTurningSpeed * Time.deltaTime);
            //anim.SetLookAtPosition(HeadIkScript.CustomLookPoints[HeadIkScript.ScanValue].transform.position);
            // Set look-at position for both head and spine

            CoreAiBehaviourScript.anim.SetLookAtPosition(RandomPoint);

            //}
            //else
            //{
            //if (CoreAiBehaviourScript.CombatStarted == false && CoreAiBehaviourScript.SearchingForSound == false)
            //{
            //    if (GenerateADetectPointForHead == false)
            //    {
            //        if (ChangePosOfRandomPointWhileSearch == false)
            //        {
            //            RandomPoint = GenerateRandomNavmeshLocation.RandomLocation(transform, AnchorPointDistance);
            //            ChangePosOfRandomPointWhileSearch = true;
            //            //if(ScanScript != null)
            //            //{
            //            //    StartCoroutine(SearchScan());
            //            //}

            //            //  HorizontalValues(HeadIkScript.MinHorizontalSearching, HeadIkScript.MaxHorizontalSearching);                          
            //        }

            //        StartCoroutine(RotateHeadForSearching());
            //        GenerateADetectPointForHead = true;
            //    }
            //    RandomPoint = Vector3.Lerp(RandomPoint, newpos, HeadRotationSpeed * Time.deltaTime);
            //    // anim.SetLookAtWeight(1f, 0f, 1f, 0.5f, 0.5f);
            //    CoreAiBehaviourScript.anim.SetLookAtPosition(RandomPoint);



            //}
        }
        //public void HorizontalValues(float MinHorizontalValue, float MaxHorizontalValue)
        //{
        //    newpos = RandomPoint;
        //    newpos.x = Random.Range(MinHorizontalValue, MaxHorizontalValue);
        //    //newpos.y = 0f;
        //    newpos.z = Random.Range(MinHorizontalValue, MaxHorizontalValue);
        //}
        //public void LookUpValues(float Minlookup, float maxlookup)
        //{
        //    newpos = RandomPoint;
        //    //newpos.x = 0f;
        //    newpos.y = Random.Range(Minlookup, maxlookup);
        //    //newpos.z = 0f;
        //}
        //public void LookDownValues(float MinDownValue, float MaxDownValue)
        //{
        //    newpos = RandomPoint;
        //    //newpos.x = 0f;
        //    newpos.y = Random.Range(MinDownValue, MaxDownValue);
        //    //newpos.z = 0f;
        //}
        //IEnumerator RotateHeadForSearching()
        //{
        //    if (LookDown == false && LookUp == false)
        //    {

        //        HorizontalValues(MinHorizontalSearching, MaxHorizontalSearching);
        //        float Randomise = Random.Range(MinTimeHorizontalLook, MaxTimeHorizontalLook);
        //        yield return new WaitForSeconds(Randomise);
        //        GenerateADetectPointForHead = false;
        //        // ChangePosOfRandomPointWhileSearch = false;
        //    }
        //    else if (LookDown == true && LookUp == true)
        //    {
        //        if (CanCreateaRandomPoint == false)
        //        {
        //            int RandomValue = Random.Range(0, 2);
        //            if (RandomValue == 0)
        //            {
        //                if (LookWhileSearching == false)
        //                {
        //                    CanCreateaRandomPoint = true;
        //                    LookUpValues(MinLookUp, MaxLookUp);
        //                    float Randomise = Random.Range(MinTimeLookUp, MaxTimeLookUp);
        //                    yield return new WaitForSeconds(Randomise);
        //                    LookWhileSearching = true;
        //                    CanCreateaRandomPoint = false;
        //                    GenerateADetectPointForHead = false;

        //                }
        //                else
        //                {
        //                    CanCreateaRandomPoint = true;
        //                    HorizontalValues(MinHorizontalSearching, MaxHorizontalSearching);
        //                    float Randomise = Random.Range(MinTimeHorizontalLook, MaxTimeHorizontalLook);
        //                    yield return new WaitForSeconds(Randomise);
        //                    LookWhileSearching = false;
        //                    CanCreateaRandomPoint = false;
        //                    GenerateADetectPointForHead = false;

        //                }
        //            }
        //            else
        //            {
        //                if (LookWhileSearching == false)
        //                {
        //                    CanCreateaRandomPoint = true;
        //                    LookDownValues(MinLookDown, MaxLookDown);
        //                    float Randomise = Random.Range(MinTimeLookDown, MaxTimeToLookDown);
        //                    yield return new WaitForSeconds(Randomise);
        //                    LookWhileSearching = true;
        //                    CanCreateaRandomPoint = false;
        //                    GenerateADetectPointForHead = false;

        //                }
        //                else
        //                {
        //                    CanCreateaRandomPoint = true;
        //                    HorizontalValues(MinHorizontalSearching, MaxHorizontalSearching);
        //                    float Randomise = Random.Range(MinTimeHorizontalLook, MaxTimeHorizontalLook);
        //                    yield return new WaitForSeconds(Randomise);
        //                    LookWhileSearching = false;
        //                    LookDownWhileSearching = false;
        //                    CanCreateaRandomPoint = false;
        //                    GenerateADetectPointForHead = false;

        //                }
        //            }


        //        }



        //    }
        //    else if (LookDown == true && LookUp == false)
        //    {
        //        if (CanCreateaRandomPoint == false)
        //        {
        //            if (LookWhileSearching == false)
        //            {
        //                CanCreateaRandomPoint = true;
        //                LookDownValues(MinLookDown, MaxLookDown);
        //                float Randomise = Random.Range(MinTimeLookDown, MaxTimeToLookDown);
        //                yield return new WaitForSeconds(Randomise);
        //                LookWhileSearching = true;
        //                CanCreateaRandomPoint = false;
        //                GenerateADetectPointForHead = false;

        //            }
        //            else
        //            {
        //                CanCreateaRandomPoint = true;
        //                HorizontalValues(MinHorizontalSearching, MaxHorizontalSearching);
        //                float Randomise = Random.Range(MinTimeHorizontalLook, MaxTimeHorizontalLook);
        //                yield return new WaitForSeconds(Randomise);
        //                LookWhileSearching = false;
        //                LookDownWhileSearching = false;
        //                CanCreateaRandomPoint = false;
        //                GenerateADetectPointForHead = false;

        //            }


        //        }

        //    }
        //    else
        //    {

        //        if (CanCreateaRandomPoint == false)
        //        {
        //            if (LookWhileSearching == false)
        //            {
        //                CanCreateaRandomPoint = true;
        //                LookUpValues(MinLookUp, MaxLookUp);
        //                float Randomise = Random.Range(MinTimeLookUp, MaxTimeLookUp);
        //                yield return new WaitForSeconds(Randomise);
        //                LookWhileSearching = true;
        //                CanCreateaRandomPoint = false;
        //                GenerateADetectPointForHead = false;

        //            }
        //            else
        //            {
        //                CanCreateaRandomPoint = true;
        //                HorizontalValues(MinHorizontalSearching, MaxHorizontalSearching);
        //                float Randomise = Random.Range(MinTimeHorizontalLook, MaxTimeHorizontalLook);
        //                yield return new WaitForSeconds(Randomise);
        //                LookWhileSearching = false;
        //                CanCreateaRandomPoint = false;
        //                GenerateADetectPointForHead = false;

        //            }

        //        }


        //    }
        //}
    }







    //public void LookAtNewSpawnPoint(Vector3 RandomPoint)
    //{
    //    closestDir = GetClosestEnemy(ClosestPoint, RandomPoint);

    //    DisplayClosestDirection = closestDir.name;
    //    for (int x = 0; x < PointOfInterestScan.Count; x++)
    //    {
    //        if (PointOfInterestScan[x].TransformDirectionName == DisplayClosestDirection)
    //        {
    //            IsScanning = true;
    //            Index = x;
    //            TransitionVal = PointOfInterestScan[x].AnimatorTransitionValue;
    //        }

    //    }

    //}
    //Transform GetClosestEnemy(List<Transform> enemies, Vector3 fromThis)
    //{
    //    Transform bestTarget = null;
    //    float closestDistanceSqr = Mathf.Infinity;
    //    Vector3 currentPosition = fromThis;
    //    foreach (Transform potentialTarget in enemies)
    //    {
    //        Vector3 directionToTarget = potentialTarget.position - currentPosition;
    //        float dSqrToTarget = directionToTarget.sqrMagnitude;
    //        if (dSqrToTarget < closestDistanceSqr)
    //        {
    //            closestDistanceSqr = dSqrToTarget;
    //            bestTarget = potentialTarget;

    //        }
    //    }
    //    return bestTarget;
    //}



}
