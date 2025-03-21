using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class ScanningScript : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script will help Ai agent scan its surroundings during idle state if default investigation type 'Scanning' is selected in the core Ai behaviour script for the Ai agent. " +
            "You can add multiple searching animations below to be randomly played at the time of scanning. This script allows two way for scanning.";


        //[Tooltip("If enabled Ai agent will use animations for rotating towards the points of interest.")]
        //public bool UseSearchingAnimations = true;
        //[Tooltip("Minimum Ai agent rotation in degrees at the time of scanning.")]
        //public float MinimumSearchingInDegrees = 0f;
        //[Tooltip("Maximum Ai agent rotation in degrees at the time of scanning.")]
        //public float MaximumSearchingInDegrees = 360f;
        //[Tooltip("The speed at which Ai agent will perform rotation.")]
        //public float SearchingSpeedAtY = 2f;

        [System.Serializable]
        public enum ChooseUpperBodyAnim
        {
            AimScan,
            IdleScan
        }

        [Tooltip("If checked then it will stop head  rotations during aimed scanning.")]
        public bool StopHeadTurnsWhenAiming = true;
        [Tooltip("Drag and Drop 'Head IK' script into this field.")]
        public HeadIK HeadIKScript;
        //public bool UnparentLookAtPointInStart = false;
        [Tooltip("If checked then aimed scan point will randomly will be offset during each new aimed scan. The offset values can be set using Change Aimed Scan Point Position sliders.")]
        public bool OffsetScanAimPoint = true;

        [Tooltip("Drag and Drop 'Aimed Scan Point' located in the hierarchy of this AI Agent.")]
        public Transform AimedScanPoint;

        [System.Serializable]
        public class ScanAimToMove
        {

            [Range(-100, 100f)]
            [Tooltip("Minimum X axis value to move the Scan Aim point too.")]
            public float MinX = 0f;

            [Range(-100, 100f)]
            [Tooltip("Maximum X axis value to move the Scan Aim point too.")]
            public float MaxX = 100f;

            [Range(-100, 100f)]
            [Tooltip("Minimum Y axis value to move the Scan Aim point too.")]
            public float MinY = 0f;

            [Range(-100, 100f)]
            [Tooltip("Maximum Y axis value to move the Scan Aim point too.")]
            public float MaxY = 100f;

            [Range(-100, 100f)]
            [Tooltip("Minimum Z axis value to move the Scan Aim point too.")]
            public float MinZ = 0f;

            [Range(-100, 100f)]
            [Tooltip("Maximum Z axis value to move the Scan Aim point too.")]
            public float MaxZ = 100f;
        }

        [Tooltip("Set of values within which the offset of the ScanAimPoint will happen each time the AimedScan will take place.")]
        public ScanAimToMove ScanAimPointOffset;

        [Range(0, 100)][Tooltip("Probability of Aiming during scanning behaviour.")]
        public int AimedScanProbability = 50;
        [Tooltip("If enabled, then AI agent will play turning animations randomly. If disabled, then AI agent will play turning animations in the order that they are placed in the drop down list.")]
        public bool RandomizeScanTurns;  // Before It was named as : ScanRandomlyBetweenPoints
        [Tooltip("Minimum time interval between area scanning turns.")]
        public float MinTimeBetweenTurns = 2f;
        [Tooltip("Maximum time interval between area scanning turns.")]
        public float MaxTimeBetweenTurns = 4f;
        // [Tooltip("Time interval between seaching a new point of interest to look around.")]
        // [HideInInspector]
        // public float TimeBetweenSearching = 0f;
        //[Tooltip("Drag and drop the 'HeadIk Script' component from the Ai agent into this field.")]
        //public HeadIk HeadIkScript;
        // [Tooltip("If enabled Ai agent will automatically find all the predefined scan points which are active at the time when the game starts and will scan between them.")]
        // public bool AutoFindScanPoints = true;

        //public Transform ScanPointsParentObject;
        //[Tooltip("Add one or Multiple Scan points for scanning points either sequently or randomly ( in case 'ScanRandomlyBetweenPoints' checkbox is enabled ) ")]
        //public Transform[] CustomScanPoints;


        [System.Serializable]
        public enum AnimationDirectionName
        {
            Forward,
            Backward,
            Right,
            Left

        }
        [System.Serializable]
        public class ScanTurnDirectionsClass
        {
            //[Tooltip("Give the same direction name that you gave to your customly created point of interest direction.For example if the point of interest name is 'Forward' than you can give the direction name as 'Forward'.")]
            //public string TransformDirectionName;
            //[Tooltip("Drag and drop the child gameobject created with a particular direction facing.For example : a arrow in the forward direction on top of the Ai agent.")]
            //public Transform Direction;
            [Tooltip("Choose any of the provided turn directions.")]
            public AnimationDirectionName ChooseTurnAnimation;
            // public string SearchingAnimationSpeedParameterName;
            [Tooltip("Minimal playback speed of the turning animation.")]
            public float MinTurnSpeed = 1f;
            [Tooltip("Maximal playback speed of the turning animation.")]
            public float MaxTurnSpeed = 1f;
            [Tooltip("Minimal playback speed of the aimed turning animation.")]
            public float MinAimedTurnSpeed = 1f;
            [Tooltip("Maximal playback speed of the aimed turning animation.")]
            public float MaxAimedTurnSpeed = 1f;
            // public GameObject[] LookAtPoints;
            //[Tooltip("Write down the transiton value of that particular animation clip that you just named above from the animation tree.")]
            //public int AnimatorTransitionValue;
        }
        //[HideInInspector]
        //public string DisplayClosestDirection;
        [Tooltip("You can add up to 4 elements to this list with each element having 4 'ScanTurnDirections'." +
            "Names of those directions are Forward, Backward, Left and Right." +
            "Those 4 names are stored as events inside animation controller that will play respective turning animation clip whenever any of those directions are selected by Ai agent." +
            "You can randomise the speed of each turn as well as speed of each aimed turn within scanning behaviour.")]
        public List<ScanTurnDirectionsClass> ScanTurnDirections = new List<ScanTurnDirectionsClass>();

        Transform closestDir;

        List<Transform> ClosestPoint = new List<Transform>();

        [HideInInspector]
        public int TransitionVal;


        float RandomiseProximityTimes;

        Vector3 PrevPointofInterestPos;

        [HideInInspector]
        public int Index;

        //[HideInInspector]
        //public bool IsScanning = false;

        [HideInInspector]
        public int ScanValue = -1;

        [HideInInspector]
        public string ScanPointChosenName;

        [HideInInspector]
        public string ChosenAnimationName = "";

        [HideInInspector]
        public Vector3 DefaultScanAimingPointPosition;

        //private void Awake()
        //{
        //    if (AutoFindScanPoints == true)
        //    {
        //        ScanPointFinder = GameObject.FindGameObjectWithTag("ScanPoints");
        //        if (ScanPointFinder != null)
        //        {
        //            CustomScanPoints = new Transform[ScanPointFinder.transform.childCount];
        //            for (int x = 0; x < ScanPointFinder.transform.childCount; x++)
        //            {
        //                CustomScanPoints[x] = ScanPointFinder.transform.GetChild(x).transform;
        //            }
        //        }

        //    }
        //}

        public void ResettingDescription()
        {
            ScriptInfo = "This script will help Ai agent scan 360 degree in the idle state if default investigation type 'Scanning' is selected from the core Ai behaviour script of the Ai agent." +
            " You can add multiple searching animations below to be randomly played at the time of investigation. This script allows two way for scanning. " +
            "The first one is where you can add multiple custom points of interest for the Ai agent inside the 'HeadIk script' to look around for example : A sniper bot scanning only two particular areas of the building." +
            "The second one is where the Ai agent will randomly scan the area specified in Min and Max degrees below.";
        }
        void Start()
        {
            if(AimedScanPoint != null)
            {
                DefaultScanAimingPointPosition = AimedScanPoint.transform.localPosition;
            }
            //if(UnparentLookAtPointInStart == true)
            //{
            //    for (int x = 0; x < PointOfInterestScan.Count; x++)
            //    {
            //        for(int y = 0; y < PointOfInterestScan[x].LookAtPoints.Length; y++)
            //        {
            //            PointOfInterestScan[x].LookAtPoints[y].transform.parent = null;
            //        }
            //    }
            //}
            //for (int x = 0; x < PointOfInterestScan.Count; x++)
            //{
            //    ClosestPoint.Add(PointOfInterestScan[x].Direction);
            //}
        }
        public void FindScanPoint()
        {
            //if (UseSearchingAnimations == true)
            //{
            //if (ScanRandomlyBetweenPoints == true)
            //{
            //    ScanValue = Random.Range(0, CustomScanPoints.Length);
            //}
            //RandomiseProximityTimes = Random.Range(MinimumTimeBetweenPointOfChecks, MaximumTimeBetweenPointOfChecks);
            //  InvokeRepeating("LookAtCustomPoint", 0.0f, RandomiseProximityTimes);
            LookAtCustomPoint();
            // }
        }

        public void LookAtCustomPoint()
        {
            //if (UseSearchingAnimations == true)
            //{
            //if (CustomScanPoints.Length > 0)
            //{
            //    if (ScanRandomlyBetweenPoints == true)
            //    {
            //        ScanValue = Random.Range(0, CustomScanPoints.Length);
            //    }
            //    else
            //    {
            //        if (ScanValue < CustomScanPoints.Length)
            //        {
            //            if (ScanValue == CustomScanPoints.Length - 1)
            //            {
            //                ScanValue = 0;
            //            }
            //            else
            //            {
            //                ++ScanValue;
            //            }

            //        }
            //    }
            //    ScanPointChosenName = CustomScanPoints[ScanValue].name;
            //    closestDir = GetClosestEnemy(ClosestPoint, CustomScanPoints[ScanValue].position);

            //    DisplayClosestDirection = closestDir.name;
            //    for (int x = 0; x < PointOfInterestScan.Count; x++)
            //    {
            //        if (PointOfInterestScan[x].Direction.name == DisplayClosestDirection)
            //        {
            //            //  IsScanning = true;
            //            Index = x;
            //            InitializeAnimation(Index);

            //        }

            //    }
            //}
            //else
            //{
            //int Randomise = Random.Range(0, PointOfInterestScan.Count);
            //Index = Randomise;
            //InitializeAnimation(Index);
            //}

            if (RandomizeScanTurns == true)
            {
                int Randomise = Random.Range(0, ScanTurnDirections.Count);
                Index = Randomise;
                PickDefaultScanAnimationToPlay(ScanTurnDirections[Index].ChooseTurnAnimation.ToString());
            }
            else
            {
                if (ScanValue < ScanTurnDirections.Count)
                {

                    ++ScanValue;

                    if (ScanValue >= ScanTurnDirections.Count)
                    {
                        ScanValue = 0;
                    }
                }
                Index = ScanValue;
                PickDefaultScanAnimationToPlay(ScanTurnDirections[Index].ChooseTurnAnimation.ToString());

            }

            //}


        }
        
        public void PickDefaultScanAnimationToPlay(string AnimationName)
        {        
            if (AnimationName == AnimationDirectionName.Forward.ToString())
            {
                TransitionVal = 3;               
            }
            if (AnimationName == AnimationDirectionName.Backward.ToString())
            {
                TransitionVal = 2;
            }
            if (AnimationName == AnimationDirectionName.Left.ToString())
            {
                TransitionVal = 1;
            }
            if (AnimationName == AnimationDirectionName.Right.ToString())
            {
                TransitionVal = 0;
            }
            ChosenAnimationName = AnimationName;
        }

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

























































    //void Start()
    //{
    //   // ScanDirectionsRootObject.transform.parent = null;
    //    CheckDistance();
    //    if (HeadIkScript.EnableCustomPointOfInterest == true)
    //    {
    //        FindNearestPointOfInterest();
    //    }
    //    RandomiseProximityTimes = Random.Range(MinimumTimeBetweenPointOfChecks, MaximumTimeBetweenPointOfChecks);
    //    InvokeRepeating("CheckingDistance", 0.0f, RandomiseProximityTimes);
    //}
    //public void CheckDistance()
    //{
    //   // ClosestPoint.Clear();
    //    for (int x = 0; x < PointOfInterestScan.Count; x++)
    //    {
    //        ClosestPoint.Add(PointOfInterestScan[x].Direction);
    //    }
    //}
    //public void FindNearestPointOfInterest()
    //{
    //    closestDir = GetClosestEnemy(ClosestPoint, HeadIkScript.TestPointOfInterset.transform);
    //    DisplayClosestDirection = closestDir.name;
    //    for (int x = 0; x < PointOfInterestScan.Count; x++)
    //    {
    //        if (PointOfInterestScan[x].TransformName == DisplayClosestDirection)
    //        {
    //            IsScanning = true;
    //            Index = x;
    //            TransitionVal = PointOfInterestScan[x].AnimatorTransitionValue;
    //            PrevPointofInterestPos = HeadIkScript.TestPointOfInterset.transform.position;
    //        }

    //    }
    //}
    //Transform GetClosestEnemy(List<Transform> enemies, Transform fromThis)
    //{
    //    Transform bestTarget = null;
    //    float closestDistanceSqr = Mathf.Infinity;
    //    Vector3 currentPosition = fromThis.position;
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
    //public void CheckingDistance() // Comment this Line of code if enable debugging is Off
    //{

    //    if (HeadIkScript.EnableCustomPointOfInterest == true && PrevPointofInterestPos != HeadIkScript.TestPointOfInterset.transform.position)
    //    {
    //        FindNearestPointOfInterest();

    //    }

    //}
}
