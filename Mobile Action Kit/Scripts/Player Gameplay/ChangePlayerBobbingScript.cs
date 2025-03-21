using UnityEngine;

namespace MobileActionKit
{
    public class ChangePlayerBobbingScript : MonoBehaviour
    {
        [Header("It will change the player Bobbing Values On Activation")]
        [Space()]
        public ProceduralBobbing BobbingScript;
        public FirstPersonController FirstPersonControllerScript;
        public PlayerManager PlayerManagerScript;

        [Space()]
        public GameObject BobObject;

        GameObject StoreBobObject;

        [System.Serializable]
        public class DefaultValues
        {
            public Vector3 HipFirePosValue;
            public Vector3 HipFireRotValue;
        }

        [System.Serializable]
        public class AllHeadBobResetValues
        {
            public float StandSprintResetDuration;
            public float CrouchSprintResetDuration;
            public float StandWalkResetDuration;
            public float CrouchWalkResetDuration;
            public float StandJumpResetDuration;
            public float CrouchJumpResetDuration;
        }

        [System.Serializable]
        public class HeadBobValues
        {
            public Vector3 MinShift;
            public Vector3 MaxShift;

            public Vector3 MinRotation;
            public Vector3 MaxRotation;


            public float XShiftDuration;

            public float YShiftDuration;

            public float ZShiftDuration;

            public float RotationDuration;

            public float XShiftDelay;
            public float YShiftDelay;
            public float ZShiftDelay;
            public float RotationDelay;
        }

        public DefaultValues DefaultBobValues;
        public AllHeadBobResetValues BobResetDurations;
        public HeadBobValues BobStandSprintValues;
        public HeadBobValues BobCrouchSprintValues;
        public HeadBobValues BobStandWalkValues;
        public HeadBobValues BobCrouchWalkValues;
        public HeadBobValues BobStandJumpValues;
        public HeadBobValues BobCrouchJumpValues;

        [HideInInspector]
        public bool StopLoop = false;
        [HideInInspector]
        public bool StartLoop = false;

        [HideInInspector]
        public bool AutoReset = false;

        [HideInInspector]
        public bool WalkStopLoop = false;
        [HideInInspector]
        public bool WalkStartLoop = false;
        [HideInInspector]
        public bool WalkAutoReset = false;

        [HideInInspector]
        public bool JumpStopLoop = false;
        [HideInInspector]
        public bool JumpStartLoop = false;
        [HideInInspector]
        public bool JumpAutoReset = false;

        [System.Serializable]
        public class HeadBobValuesStored
        {
            public Vector3 MinShift;
            public Vector3 MaxShift;

            public Vector3 MinRotation;
            public Vector3 MaxRotation;


            public float XShiftDuration;

            public float YShiftDuration;

            public float ZShiftDuration;

            public float RotationDuration;

            public float XShiftDelay;
            public float YShiftDelay;
            public float ZShiftDelay;
            public float RotationDelay;
        }
        [System.Serializable]
        public class DefaultValuesStored
        {
            public Vector3 HipFirePosValue;
            public Vector3 HipFireRotValue;
        }
        [System.Serializable]
        public class AllHeadBobResetValuesStored
        {
            public float StandSprintResetDuration;
            public float CrouchSprintResetDuration;
            public float StandWalkResetDuration;
            public float CrouchWalkResetDuration;
            public float StandJumpResetDuration;
            public float CrouchJumpResetDuration;
        }

        [HideInInspector]
        public DefaultValuesStored DefaultBobValuesStored;
        [HideInInspector]
        public AllHeadBobResetValuesStored BobResetDurationsStored;
        [HideInInspector]
        public HeadBobValuesStored BobStandSprintValuesStored;
        [HideInInspector]
        public HeadBobValuesStored BobCrouchSprintValuesStored;
        [HideInInspector]
        public HeadBobValuesStored BobStandWalkValuesStored;
        [HideInInspector]
        public HeadBobValuesStored BobCrouchWalkValuesStored;
        [HideInInspector]
        public HeadBobValuesStored BobStandJumpValuesStored;
        [HideInInspector]
        public HeadBobValuesStored BobCrouchJumpValuesStored;

        bool DoOnce = false;

        void OnEnable()
        {
            if (DoOnce == false)
            {
                StoreBobObject = BobbingScript.BobObject;
                BobValuesStored(BobStandWalkValuesStored, BobbingScript.BobStandWalkValues);
                BobValuesStored(BobCrouchWalkValuesStored, BobbingScript.BobCrouchWalkValues);
                BobValuesStored(BobStandJumpValuesStored, BobbingScript.BobStandJumpValues);
                BobValuesStored(BobCrouchJumpValuesStored, BobbingScript.BobCrouchJumpValues);
                BobValuesStored(BobStandSprintValuesStored, BobbingScript.BobStandSprintValues);
                BobValuesStored(BobCrouchSprintValuesStored, BobbingScript.BobCrouchSprintValues);

                BobDefaultValuesStored(DefaultBobValuesStored, BobbingScript.DefaultBobValues);
                BobResetValuesStored(BobResetDurationsStored, BobbingScript.BobResetDurations);

                DoOnce = true;
            }


            BobbingScript.BobObject = BobObject;
            BobValues(BobStandWalkValues, BobbingScript.BobStandWalkValues);
            BobValues(BobCrouchWalkValues, BobbingScript.BobCrouchWalkValues);
            BobValues(BobStandJumpValues, BobbingScript.BobStandJumpValues);
            BobValues(BobCrouchJumpValues, BobbingScript.BobCrouchJumpValues);
            BobValues(BobStandSprintValues, BobbingScript.BobStandSprintValues);
            BobValues(BobCrouchSprintValues, BobbingScript.BobCrouchSprintValues);

            BobDefaultValues(DefaultBobValues, BobbingScript.DefaultBobValues);
            BobResetValues(BobResetDurations, BobbingScript.BobResetDurations);
        }
        void OnDisable()
        {
            BobbingScript.BobObject = StoreBobObject;

            ChangeBackBobValues(BobStandWalkValuesStored, BobbingScript.BobStandWalkValues);
            ChangeBackBobValues(BobCrouchWalkValuesStored, BobbingScript.BobCrouchWalkValues);
            ChangeBackBobValues(BobStandJumpValuesStored, BobbingScript.BobStandJumpValues);
            ChangeBackBobValues(BobCrouchJumpValuesStored, BobbingScript.BobCrouchJumpValues);
            ChangeBackBobValues(BobStandSprintValuesStored, BobbingScript.BobStandSprintValues);
            ChangeBackBobValues(BobCrouchSprintValuesStored, BobbingScript.BobCrouchSprintValues);

            ChangeBackBobDefaultValues(DefaultBobValuesStored, BobbingScript.DefaultBobValues);
            ChangeBackBobResetValues(BobResetDurationsStored, BobbingScript.BobResetDurations);
        }
        public void BobValues(HeadBobValues ChangingValue, ProceduralBobbing.HeadBobValues PlayerBobbingValue)
        {
            PlayerBobbingValue.MaxRotation = ChangingValue.MaxRotation;
            PlayerBobbingValue.MaxShift = ChangingValue.MinShift;
            PlayerBobbingValue.MinRotation = ChangingValue.MinRotation;
            PlayerBobbingValue.MinShift = ChangingValue.MinShift;
            PlayerBobbingValue.RotationDelay = ChangingValue.RotationDelay;
            PlayerBobbingValue.RotationDuration = ChangingValue.RotationDuration;
            PlayerBobbingValue.XShiftDelay = ChangingValue.XShiftDelay;
            PlayerBobbingValue.XShiftDuration = ChangingValue.XShiftDuration;
            PlayerBobbingValue.YShiftDelay = ChangingValue.YShiftDelay;
            PlayerBobbingValue.YShiftDuration = ChangingValue.YShiftDuration;
            PlayerBobbingValue.ZShiftDelay = ChangingValue.ZShiftDelay;
            PlayerBobbingValue.ZShiftDuration = ChangingValue.ZShiftDuration;
        }
        public void BobDefaultValues(DefaultValues ChangingValue, ProceduralBobbing.DefaultValues PlayerBobbingValue)
        {
            PlayerBobbingValue.HipFirePosValue = ChangingValue.HipFirePosValue;
            PlayerBobbingValue.HipFireRotValue = ChangingValue.HipFireRotValue;
        }
        public void BobResetValues(AllHeadBobResetValues ChangingValue, ProceduralBobbing.AllHeadBobResetValues PlayerBobbingValue)
        {
            PlayerBobbingValue.StandWalkResetDuration = ChangingValue.StandWalkResetDuration;
            PlayerBobbingValue.CrouchWalkResetDuration = ChangingValue.CrouchWalkResetDuration;
            PlayerBobbingValue.StandSprintResetDuration = ChangingValue.StandSprintResetDuration;
            PlayerBobbingValue.CrouchSprintResetDuration = ChangingValue.CrouchSprintResetDuration;
            PlayerBobbingValue.StandJumpResetDuration = ChangingValue.StandJumpResetDuration;
            PlayerBobbingValue.CrouchJumpResetDuration = ChangingValue.CrouchJumpResetDuration;

        }

        public void BobValuesStored(HeadBobValuesStored ChangingValue, ProceduralBobbing.HeadBobValues PlayerBobbingValue)
        {
            ChangingValue.MaxRotation = PlayerBobbingValue.MaxRotation;
            ChangingValue.MinShift = PlayerBobbingValue.MaxShift;
            ChangingValue.MinRotation = PlayerBobbingValue.MinRotation;
            ChangingValue.MinShift = PlayerBobbingValue.MinShift;
            ChangingValue.RotationDelay = PlayerBobbingValue.RotationDelay;
            ChangingValue.RotationDuration = PlayerBobbingValue.RotationDuration;
            ChangingValue.XShiftDelay = PlayerBobbingValue.XShiftDelay;
            ChangingValue.XShiftDuration = PlayerBobbingValue.XShiftDuration;
            ChangingValue.YShiftDelay = PlayerBobbingValue.YShiftDelay;
            ChangingValue.YShiftDuration = PlayerBobbingValue.YShiftDuration;
            ChangingValue.ZShiftDelay = PlayerBobbingValue.ZShiftDelay;
            ChangingValue.ZShiftDuration = PlayerBobbingValue.ZShiftDuration;
        }
        public void BobDefaultValuesStored(DefaultValuesStored ChangingValue, ProceduralBobbing.DefaultValues PlayerBobbingValue)
        {
            ChangingValue.HipFirePosValue = PlayerBobbingValue.HipFirePosValue;
            ChangingValue.HipFireRotValue = PlayerBobbingValue.HipFireRotValue;
        }
        public void BobResetValuesStored(AllHeadBobResetValuesStored ChangingValue, ProceduralBobbing.AllHeadBobResetValues PlayerBobbingValue)
        {
            ChangingValue.StandWalkResetDuration = PlayerBobbingValue.StandWalkResetDuration;
            ChangingValue.CrouchWalkResetDuration = PlayerBobbingValue.CrouchWalkResetDuration;
            ChangingValue.StandSprintResetDuration = PlayerBobbingValue.StandSprintResetDuration;
            ChangingValue.CrouchSprintResetDuration = PlayerBobbingValue.CrouchSprintResetDuration;
            ChangingValue.StandJumpResetDuration = PlayerBobbingValue.StandJumpResetDuration;
            ChangingValue.CrouchJumpResetDuration = PlayerBobbingValue.CrouchJumpResetDuration;
        }

        public void ChangeBackBobValues(HeadBobValuesStored ChangingValue, ProceduralBobbing.HeadBobValues PlayerBobbingValue)
        {
            PlayerBobbingValue.MaxRotation = ChangingValue.MaxRotation;
            PlayerBobbingValue.MaxShift = ChangingValue.MinShift;
            PlayerBobbingValue.MinRotation = ChangingValue.MinRotation;
            PlayerBobbingValue.MinShift = ChangingValue.MinShift;
            PlayerBobbingValue.RotationDelay = ChangingValue.RotationDelay;
            PlayerBobbingValue.RotationDuration = ChangingValue.RotationDuration;
            PlayerBobbingValue.XShiftDelay = ChangingValue.XShiftDelay;
            PlayerBobbingValue.XShiftDuration = ChangingValue.XShiftDuration;
            PlayerBobbingValue.YShiftDelay = ChangingValue.YShiftDelay;
            PlayerBobbingValue.YShiftDuration = ChangingValue.YShiftDuration;
            PlayerBobbingValue.ZShiftDelay = ChangingValue.ZShiftDelay;
            PlayerBobbingValue.ZShiftDuration = ChangingValue.ZShiftDuration;
        }
        public void ChangeBackBobDefaultValues(DefaultValuesStored ChangingValue, ProceduralBobbing.DefaultValues PlayerBobbingValue)
        {
            PlayerBobbingValue.HipFirePosValue = ChangingValue.HipFirePosValue;
            PlayerBobbingValue.HipFireRotValue = ChangingValue.HipFireRotValue;
        }
        public void ChangeBackBobResetValues(AllHeadBobResetValuesStored ChangingValue, ProceduralBobbing.AllHeadBobResetValues PlayerBobbingValue)
        {
            PlayerBobbingValue.StandWalkResetDuration = ChangingValue.StandWalkResetDuration;
            PlayerBobbingValue.CrouchWalkResetDuration = ChangingValue.CrouchWalkResetDuration;
            PlayerBobbingValue.StandSprintResetDuration = ChangingValue.StandSprintResetDuration;
            PlayerBobbingValue.CrouchSprintResetDuration = ChangingValue.CrouchSprintResetDuration;
            PlayerBobbingValue.StandJumpResetDuration = ChangingValue.StandJumpResetDuration;
            PlayerBobbingValue.CrouchJumpResetDuration = ChangingValue.CrouchJumpResetDuration;

        }
    }
}