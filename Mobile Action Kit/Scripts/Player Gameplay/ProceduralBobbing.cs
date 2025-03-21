using System.Collections;
using UnityEngine;

namespace MobileActionKit
{
    public class ProceduralBobbing : MonoBehaviour
    {

        [TextArea]

        public string ScriptInfo = "Procedural Bobbing script is providing looping animation to any of the player`s child gameObjects it is being attached to (cameras, weapons etc.) " +
            "be it Walking,Sprinting, Jumping, Crouching movements, for Hip/Idle and Aimed poses. It provides the ability to add extra layers of control and fine-tuning to the overall dynamics of the player's view.";
        [Space(10)]

        [Tooltip("Player root gameobject containing 'FirstPersonController' script is to be placed into this field.")]
        public FirstPersonController FirstPersonControllerScript;

        [Tooltip("PlayerManager  gameobject containing 'PlayerManager' script is to be placed into this field.")]
        public PlayerManager PlayerManagerScript = new PlayerManager();
        [Space()]

        [Tooltip("Gameobject to be bobbed is to be placed into this field.")]
        public GameObject BobObject;

        [System.Serializable]
        public class DefaultValues
        {
            [HideInInspector]
            [Tooltip("Toggle procedural Hip Fire Idle animation.")]
            public bool EnableProceduralHipFireIdle = false;

            [HideInInspector]
            [Tooltip("Toggle procedural Aimed Idle animation.")]
            public bool EnableProceduralAimedIdle = false;

            [Tooltip("Position offset values for the Hip Fire Idle pose.")]
            public Vector3 HipFirePosValue;

            [Tooltip("Rotation offset values for the Hip Fire Idle pose.")]
            public Vector3 HipFireRotValue;

            [Tooltip("Position offset values for the Aimed Idle pose.")]
            public Vector3 AimedPosValue;

            [Tooltip("Rotation offset values for the Aimed Idle pose.")]
            public Vector3 AimedRotValue;

            [HideInInspector]
            [Tooltip("Bob values for Standing Hip Fire Idle.")]
            public HeadBobValues StandHipFireIdleValues;

            [HideInInspector]
            [Tooltip("Bob values for Crouching Hip Fire Idle.")]
            public HeadBobValues CrouchHipFireIdleValues;

            [HideInInspector]
            [Tooltip("Bob values for Standing Aimed Idle.")]
            public HeadBobValues StandAimedIdleValues;

            [HideInInspector]
            [Tooltip("Bob values for Crouching Aimed Idle.")]
            public HeadBobValues CrouchAimedIdleValues;
        }

        [System.Serializable]
        public class AllHeadBobResetValues
        {
            [Tooltip("Reset duration for Stationary Stand Hip Fire pose.")]
            public float StationaryStandHipFireResetDuration;

            [Tooltip("Reset duration for Stationary Crouch Hip Fire pose.")]
            public float StationaryCrouchHipFireResetDuration;

            [Tooltip("Reset duration for Stationary Stand Aimed pose.")]
            public float StationaryStandAimedResetDuration;

            [Tooltip("Reset duration for Stationary Crouch Aimed pose.")]
            public float StationaryCrouchAimedResetDuration;

            [Tooltip("Reset duration for Stand Sprint.")]
            public float StandSprintResetDuration;

            [Tooltip("Reset duration for Crouch Sprint.")]
            public float CrouchSprintResetDuration;

            [Tooltip("Reset duration for Stand Walk.")]
            public float StandWalkResetDuration;

            [Tooltip("Reset duration for Crouch Walk.")]
            public float CrouchWalkResetDuration;

            [Tooltip("Reset duration for Stand Jump.")]
            public float StandJumpResetDuration;

            [Tooltip("Reset duration for Crouch Jump.")]
            public float CrouchJumpResetDuration;
        }

        public DefaultValues DefaultBobValues;
        public AllHeadBobResetValues BobResetDurations;

        [System.Serializable]
        public class HeadBobValues
        {
            [Tooltip("Minimum position shift values for the bobbing effect.")]
            public Vector3 MinShift;

            [Tooltip("Maximum position shift values for the bobbing effect.")]
            public Vector3 MaxShift;

            [Tooltip("Minimum rotation values for the bobbing effect.")]
            public Vector3 MinRotation;

            [Tooltip("Maximum rotation values for the bobbing effect.")]
            public Vector3 MaxRotation;

            [Tooltip("Duration of X-axis position shift.")]
            public float XShiftDuration;

            [Tooltip("Duration of Y-axis position shift.")]
            public float YShiftDuration;

            [Tooltip("Duration of Z-axis position shift.")]
            public float ZShiftDuration;

            [Tooltip("Duration of the rotation effect.")]
            public float RotationDuration;

            [Tooltip("Delay before starting X-axis position shift.")]
            public float XShiftDelay;

            [Tooltip("Delay before starting Y-axis position shift.")]
            public float YShiftDelay;

            [Tooltip("Delay before starting Z-axis position shift.")]
            public float ZShiftDelay;

            [Tooltip("Delay before starting the rotation effect.")]
            public float RotationDelay;
        }

        [Tooltip("Bobbing values for Stand Sprint.")]
        public HeadBobValues BobStandSprintValues;

        [Tooltip("Bobbing values for Crouch Sprint.")]
        public HeadBobValues BobCrouchSprintValues;

        [Tooltip("Bobbing values for Stand Walk.")]
        public HeadBobValues BobStandWalkValues;

        [Tooltip("Bobbing values for Crouch Walk.")]
        public HeadBobValues BobCrouchWalkValues;

        [Tooltip("Bobbing values for Aimed Stand Walk.")]
        public HeadBobValues BobAimedStandWalkValues;

        [Tooltip("Bobbing values for Aimed Crouch Walk.")]
        public HeadBobValues BobAimedCrouchWalkValues;

        [Tooltip("Bobbing values for Stand Jump.")]
        public HeadBobValues BobStandJumpValues;

        [Tooltip("Bobbing values for Crouch Jump.")]
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

        bool AimedLoop = false;
        bool HipFireLoop = false;

        bool FixSprintBobbing = false;
        bool FixWalkBobbing = false;

        void Start()
        {
            AimedLoop = DefaultBobValues.EnableProceduralAimedIdle;
            HipFireLoop = DefaultBobValues.EnableProceduralHipFireIdle;

            DefaultLoopingValues(BobResetDurations.StationaryStandHipFireResetDuration, BobResetDurations.StationaryCrouchHipFireResetDuration
                , BobResetDurations.StationaryStandAimedResetDuration, BobResetDurations.StationaryCrouchAimedResetDuration, "StartFunction");
        }
        void OnEnable()
        {
            LeanTween.cancel(BobObject.gameObject);

            // so if 1 weapon is disabled and new weapon enabled when switching we make sure for example: Weapon Camera used for sprinting we make sure to reset it's position and rotation for the new weapon enabled.
            BobObject.gameObject.transform.localPosition = DefaultBobValues.HipFirePosValue;
            BobObject.gameObject.transform.localEulerAngles = DefaultBobValues.HipFireRotValue;

            // Reset everything if new weapon get enabled so restart everything by scratch.
            StopLoop = false;
            StartLoop = false;
            AutoReset = false;
            WalkStopLoop = false;
            WalkStartLoop = false;
            WalkAutoReset = false;
            JumpStopLoop = false;
            JumpStartLoop = false;
            JumpAutoReset = false;
            AimedLoop = false;
            HipFireLoop = false;

            FixSprintBobbing = false;
            FixWalkBobbing = false;

            FirstPersonControllerScript.LoopHeadBob = false;
            FirstPersonControllerScript.LoopJumpHeadBob = false;
        }
        public void DefaultLoopingValues(float StandResetDuration, float CrouchResetDuration, float StandAimedResetDuration, float CrouchAimedResetDuration, string WhoIsCalling)
        {
            FixWalkBobbing = false;

            LeanTween.cancel(BobObject.gameObject);
            //            Debug.Log("Going to default value" + WhoIsCalling);

            if (PlayerManagerScript.CurrentHoldingPlayerWeapon != null)
            {
                if (PlayerManagerScript.CurrentHoldingPlayerWeapon.IsAimed == false)
                {
                    if (Crouch.instance.IsCrouching == false)
                    {

                        LeanTween.moveLocal(BobObject.gameObject, DefaultBobValues.HipFirePosValue, StandResetDuration).setFrom(BobObject.transform.localPosition);
                        LeanTween.rotateLocal(BobObject.gameObject, DefaultBobValues.HipFireRotValue, StandResetDuration).setFrom(BobObject.transform.localEulerAngles);

                    }
                    else
                    {

                        LeanTween.moveLocal(BobObject.gameObject, DefaultBobValues.HipFirePosValue, CrouchResetDuration).setFrom(BobObject.transform.localPosition);
                        LeanTween.rotateLocal(BobObject.gameObject, DefaultBobValues.HipFireRotValue, CrouchResetDuration).setFrom(BobObject.transform.localEulerAngles);

                    }
                }
                else
                {
                    if (Crouch.instance.IsCrouching == false)
                    {
                        LeanTween.moveLocal(BobObject.gameObject, new Vector3(DefaultBobValues.AimedPosValue.x, DefaultBobValues.AimedPosValue.y, DefaultBobValues.AimedPosValue.z), StandAimedResetDuration).setFrom(BobObject.transform.localPosition);
                        LeanTween.rotateLocal(BobObject.gameObject, DefaultBobValues.AimedRotValue, BobResetDurations.StationaryStandAimedResetDuration).setFrom(BobObject.transform.localEulerAngles);

                    }
                    else
                    {

                        LeanTween.moveLocal(BobObject.gameObject, new Vector3(DefaultBobValues.AimedPosValue.x, DefaultBobValues.AimedPosValue.y, DefaultBobValues.AimedPosValue.z), CrouchAimedResetDuration).setFrom(BobObject.transform.localPosition);
                        LeanTween.rotateLocal(BobObject.gameObject, DefaultBobValues.AimedRotValue, BobResetDurations.StationaryCrouchAimedResetDuration).setFrom(BobObject.transform.localEulerAngles);
                    }
                }
            }
            else
            {
                if (Crouch.instance.IsCrouching == false)
                {

                    LeanTween.moveLocal(BobObject.gameObject, DefaultBobValues.HipFirePosValue, StandResetDuration).setFrom(BobObject.transform.localPosition);
                    LeanTween.rotateLocal(BobObject.gameObject, DefaultBobValues.HipFireRotValue, StandResetDuration).setFrom(BobObject.transform.localEulerAngles);

                }
                else
                {

                    LeanTween.moveLocal(BobObject.gameObject, DefaultBobValues.HipFirePosValue, CrouchResetDuration).setFrom(BobObject.transform.localPosition);
                    LeanTween.rotateLocal(BobObject.gameObject, DefaultBobValues.HipFireRotValue, CrouchResetDuration).setFrom(BobObject.transform.localEulerAngles);

                }
            }
        }
        void Update()
        {
            if (PlayerManagerScript.IsMoving && Crouch.instance.IsCrouching == false)
            {
                if (FixSprintBobbing == false)
                {
                    WalkStopLoop = false;
                    FixWalkBobbing = false;
                    LeanTween.cancel(BobObject.gameObject);
                    FixSprintBobbing = true;
                }
                if (StopLoop == false)
                {
                    if (!LeanTween.isTweening(BobObject.gameObject))
                    {
                        // Debug.Log(gameObject.name);
                        XShiftWeaponCam(BobObject.gameObject, BobStandSprintValues.MinShift.x, BobStandSprintValues.MaxShift.x, BobStandSprintValues.XShiftDuration, BobStandSprintValues.XShiftDelay);
                        YShiftWeaponCam(BobObject.gameObject, BobStandSprintValues.MinShift.y, BobStandSprintValues.MaxShift.y, BobStandSprintValues.YShiftDuration, BobStandSprintValues.YShiftDelay);
                        ZShiftWeaponCam(BobObject.gameObject, BobStandSprintValues.MinShift.z, BobStandSprintValues.MaxShift.z, BobStandSprintValues.ZShiftDuration, BobStandSprintValues.ZShiftDelay);
                        RotWeaponCam(BobObject.gameObject, BobStandSprintValues.MinRotation, BobStandSprintValues.MaxRotation, BobStandSprintValues.RotationDuration, BobStandSprintValues.RotationDelay);
                        StopLoop = true;
                    }

                }
            }
            else if (PlayerManagerScript.IsMoving && Crouch.instance.IsCrouching == true)
            {
                if (FixSprintBobbing == false)
                {
                    WalkStopLoop = false;
                    FixWalkBobbing = false;
                    LeanTween.cancel(BobObject.gameObject);
                    FixSprintBobbing = true;
                }
                if (StopLoop == false)
                {
                    if (!LeanTween.isTweening(BobObject.gameObject))
                    {
                        XShiftWeaponCam(BobObject.gameObject, BobCrouchSprintValues.MinShift.x, BobCrouchSprintValues.MaxShift.x, BobCrouchSprintValues.XShiftDuration, BobCrouchSprintValues.XShiftDelay);
                        YShiftWeaponCam(BobObject.gameObject, BobCrouchSprintValues.MinShift.y, BobCrouchSprintValues.MaxShift.y, BobCrouchSprintValues.YShiftDuration, BobCrouchSprintValues.YShiftDelay);
                        ZShiftWeaponCam(BobObject.gameObject, BobCrouchSprintValues.MinShift.z, BobCrouchSprintValues.MaxShift.z, BobCrouchSprintValues.ZShiftDuration, BobCrouchSprintValues.ZShiftDelay);
                        RotWeaponCam(BobObject.gameObject, BobCrouchSprintValues.MinRotation, BobCrouchSprintValues.MaxRotation, BobCrouchSprintValues.RotationDuration, BobCrouchSprintValues.RotationDelay);
                        StopLoop = true;

                    }

                }

            }
            else
            {
                if (FixSprintBobbing == true)
                {
                    LeanTween.cancel(BobObject.gameObject);
                    FixSprintBobbing = false;
                }
                if (StopLoop == true)
                {
                    if (AutoReset == false)
                    {
                        DefaultLoopingValues(BobResetDurations.StandSprintResetDuration, BobResetDurations.CrouchSprintResetDuration, BobResetDurations.StationaryStandAimedResetDuration, BobResetDurations.StationaryCrouchAimedResetDuration, "AfterSprint");
                        AutoReset = true;
                    }
                }
            }
        }
        public void XShiftWeaponCam(GameObject obj, float MinShift, float MaxShift, float Dur, float Delay)
        {
            LeanTween.moveLocalX(obj, MinShift, Mathf.Abs(Dur)).setFrom(obj.transform.localPosition.x).setDelay(Delay);
            LeanTween.moveLocalX(obj, MaxShift, Mathf.Abs(Dur)).setFrom(MinShift).setLoopPingPong().setDelay(Mathf.Abs(Dur) + Delay);
        }
        public void YShiftWeaponCam(GameObject obj, float MinShift, float MaxShift, float Dur, float Delay)
        {
            LeanTween.moveLocalY(obj, MinShift, Mathf.Abs(Dur)).setFrom(obj.transform.localPosition.y).setDelay(Delay);
            LeanTween.moveLocalY(obj, MaxShift, Mathf.Abs(Dur)).setFrom(MinShift).setLoopPingPong().setDelay(Mathf.Abs(Dur) + Delay);
        }
        public void ZShiftWeaponCam(GameObject obj, float MinShift, float MaxShift, float Dur, float Delay)
        {
            LeanTween.moveLocalZ(obj, MinShift, Mathf.Abs(Dur)).setFrom(obj.transform.localPosition.z).setDelay(Delay);
            LeanTween.moveLocalZ(obj, MaxShift, Mathf.Abs(Dur)).setFrom(MinShift).setLoopPingPong().setDelay(Mathf.Abs(Dur) + Delay);
        }
        //public void RotWeaponCam(GameObject obj, Vector3 MinShift, Vector3 MaxShift, float Dur, float Delay)
        //{
        //    LeanTween.rotateLocal(obj, MinShift, Mathf.Abs(Dur)).setFrom(obj.transform.localEulerAngles).setDelay(Delay + 0.1f);
        //    LeanTween.rotateLocal(obj, MaxShift, Mathf.Abs(Dur)).setFrom(MinShift).setLoopPingPong().setDelay(Mathf.Abs(Dur) + Delay + 0.1f);
        //}
        public void RotWeaponCam(GameObject obj, Vector3 MinShift, Vector3 MaxShift, float Dur, float Delay)
        {
            // Get the object's actual current rotation (not a predefined variable)
            Vector3 currentRotation = DefaultBobValues.HipFireRotValue;

            // Target rotations based on MinShift and MaxShift
            Vector3 targetRotationA = currentRotation + MinShift;
            Vector3 targetRotationB = currentRotation + MaxShift;

            // Apply smooth rotation from current rotation to MinShift
            LeanTween.rotateLocal(obj, targetRotationA, Dur)
                .setDelay(Delay)
                .setEase(LeanTweenType.easeInOutSine)
                .setOnComplete(() =>
                {
                    // After reaching MinShift, ping-pong between MinShift and MaxShift
                    LeanTween.rotateLocal(obj, targetRotationB, Dur)
                                .setLoopPingPong()
                                .setEase(LeanTweenType.easeInOutSine);
                });
        }

        //public void RotWeaponCam(GameObject obj, Vector3 MinShift, Vector3 MaxShift, float Dur, float Delay)
        //{
        //    // Get the current rotation of the object
        //    Vector3 currentRotation = DefaultBobValues.HipFireRotValue;
        //    // Rotate from the current rotation to MinShift with a delay
        //    LeanTween.rotateLocal(obj, currentRotation + MinShift, Dur)
        //        .setFrom(currentRotation)
        //        .setDelay(Delay)
        //        .setEase(LeanTweenType.easeInOutSine); // Adjust the ease function if needed

        //    // Calculate the final rotation based on MaxShift relative to MinShift
        //    Vector3 finalRotation = currentRotation + MaxShift;

        //    // Rotate to the final rotation with a delay and ping-pong effect
        //    LeanTween.rotateLocal(obj, finalRotation, Dur)
        //        .setFrom(currentRotation + MinShift)
        //        .setLoopPingPong()
        //        .setDelay(Delay + Dur)
        //        .setEase(LeanTweenType.easeInOutSine); // Adjust the ease function if needed
        //}
        //public void RotWeaponCam(GameObject obj, Vector3 MinShift, Vector3 MaxShift, float Dur, float Delay)
        //{
        //    // Get the current rotation of the object
        //    Vector3 currentRotation = obj.transform.localEulerAngles;
        //    // Rotate from the current rotation to MinShift with a delay
        //    LeanTween.rotateLocal(obj, MinShift, Dur) // previously this was there currentRotation + changed after adding controls for PC 
        //        .setFrom(currentRotation)
        //        .setDelay(Delay)
        //        .setEase(LeanTweenType.easeInOutSine); // Adjust the ease function if needed

        //    // Calculate the final rotation based on MaxShift relative to MinShift
        //    Vector3 finalRotation = MaxShift; // previously this was there currentRotation + changed after adding controls for PC 

        //    // Rotate to the final rotation with a delay and ping-pong effect
        //    LeanTween.rotateLocal(obj, finalRotation, Dur)
        //        .setFrom(MinShift) // currentRotation + MinShift // previously this was there changed after adding controls for PC 
        //        .setLoopPingPong()
        //        .setDelay(Delay + Dur)
        //        .setEase(LeanTweenType.easeInOutSine); // Adjust the ease function if needed
        //}
        public void WalkStandValues()
        {
            if (FixWalkBobbing == false)
            {

                LeanTween.cancel(BobObject.gameObject, true);
                FixWalkBobbing = true;
            }
            if (WalkStopLoop == false)
            {

                //if (!LeanTween.isTweening(BobObject.gameObject)) // I have commented this because when we have Ak47 weapon which weapon camera ( which is usually the bob object is bobbing )
                // and while bobbing if the player picks up the M11 or any weapon when this code was uncommented it basically do not allow bobbing
                // because the weapon camera results in already performing tweening which do not allow M11 to perform Walk bobbing but when we comment
                // this code than after it allow only.
                //{
                // Delay new tweening slightly to ensure cancel() has fully processed
                LeanTween.delayedCall(0.01f, () =>
                {
                    XShiftWeaponCam(BobObject.gameObject, BobStandWalkValues.MinShift.x, BobStandWalkValues.MaxShift.x, BobStandWalkValues.XShiftDuration, BobStandWalkValues.XShiftDelay);
                    YShiftWeaponCam(BobObject.gameObject, BobStandWalkValues.MinShift.y, BobStandWalkValues.MaxShift.y, BobStandWalkValues.YShiftDuration, BobStandWalkValues.YShiftDelay);
                    ZShiftWeaponCam(BobObject.gameObject, BobStandWalkValues.MinShift.z, BobStandWalkValues.MaxShift.z, BobStandWalkValues.ZShiftDuration, BobStandWalkValues.ZShiftDelay);
                    RotWeaponCam(BobObject.gameObject, BobStandWalkValues.MinRotation, BobStandWalkValues.MaxRotation, BobStandWalkValues.RotationDuration, BobStandWalkValues.RotationDelay);
                    WalkStopLoop = true;
                    FirstPersonControllerScript.LoopHeadBob = true;
                    //}
                });
            }
        }

        public void WalkOpticalStandValues()
        {
            if (FixWalkBobbing == false)
            {

                LeanTween.cancel(BobObject.gameObject);
                FixWalkBobbing = true;
            }
            if (WalkStopLoop == false)
            {

                XShiftWeaponCam(BobObject.gameObject, BobAimedStandWalkValues.MinShift.x, BobAimedStandWalkValues.MaxShift.x, BobAimedStandWalkValues.XShiftDuration, BobAimedStandWalkValues.XShiftDelay);
                YShiftWeaponCam(BobObject.gameObject, BobAimedStandWalkValues.MinShift.y, BobAimedStandWalkValues.MaxShift.y, BobAimedStandWalkValues.YShiftDuration, BobAimedStandWalkValues.YShiftDelay);
                ZShiftWeaponCam(BobObject.gameObject, BobAimedStandWalkValues.MinShift.z, BobAimedStandWalkValues.MaxShift.z, BobAimedStandWalkValues.ZShiftDuration, BobAimedStandWalkValues.ZShiftDelay);
                RotWeaponCam(BobObject.gameObject, BobAimedStandWalkValues.MinRotation, BobAimedStandWalkValues.MaxRotation, BobAimedStandWalkValues.RotationDuration, BobAimedStandWalkValues.RotationDelay);
                WalkStopLoop = true;
                FirstPersonControllerScript.LoopHeadBob = true;

            }
        }
        public void WalkCrouchValues()
        {
            if (FixWalkBobbing == false)
            {

                LeanTween.cancel(BobObject.gameObject);
                FixWalkBobbing = true;
            }
            if (WalkStopLoop == false)
            {
                //if (!LeanTween.isTweening(BobObject.gameObject))
                //{
                XShiftWeaponCam(BobObject.gameObject, BobCrouchWalkValues.MinShift.x, BobCrouchWalkValues.MaxShift.x, BobCrouchWalkValues.XShiftDuration, BobCrouchWalkValues.XShiftDelay);
                YShiftWeaponCam(BobObject.gameObject, BobCrouchWalkValues.MinShift.y, BobCrouchWalkValues.MaxShift.y, BobCrouchWalkValues.YShiftDuration, BobCrouchWalkValues.YShiftDelay);
                ZShiftWeaponCam(BobObject.gameObject, BobCrouchWalkValues.MinShift.z, BobCrouchWalkValues.MaxShift.z, BobCrouchWalkValues.ZShiftDuration, BobCrouchWalkValues.ZShiftDelay);
                RotWeaponCam(BobObject.gameObject, BobCrouchWalkValues.MinRotation, BobCrouchWalkValues.MaxRotation, BobCrouchWalkValues.RotationDuration, BobCrouchWalkValues.RotationDelay);
                WalkStopLoop = true;
                FirstPersonControllerScript.LoopHeadBob = true;
                //}
            }
        }
        public void WalkOpticalCrouchValues()
        {
            if (FixWalkBobbing == false)
            {

                LeanTween.cancel(BobObject.gameObject);
                FixWalkBobbing = true;
            }
            if (WalkStopLoop == false)
            {

                XShiftWeaponCam(BobObject.gameObject, BobAimedCrouchWalkValues.MinShift.x, BobAimedCrouchWalkValues.MaxShift.x, BobAimedCrouchWalkValues.XShiftDuration, BobAimedCrouchWalkValues.XShiftDelay);
                YShiftWeaponCam(BobObject.gameObject, BobAimedCrouchWalkValues.MinShift.y, BobAimedCrouchWalkValues.MaxShift.y, BobAimedCrouchWalkValues.YShiftDuration, BobAimedCrouchWalkValues.YShiftDelay);
                ZShiftWeaponCam(BobObject.gameObject, BobAimedCrouchWalkValues.MinShift.z, BobAimedCrouchWalkValues.MaxShift.z, BobAimedCrouchWalkValues.ZShiftDuration, BobAimedCrouchWalkValues.ZShiftDelay);

                RotWeaponCam(BobObject.gameObject, BobAimedCrouchWalkValues.MinRotation, BobAimedCrouchWalkValues.MaxRotation, BobAimedCrouchWalkValues.RotationDuration, BobAimedCrouchWalkValues.RotationDelay);
                WalkStopLoop = true;
                FirstPersonControllerScript.LoopHeadBob = true;

            }
        }
        public void JumpStandValues()
        {
            if (JumpStopLoop == false)
            {
                //if (!LeanTween.isTweening(BobObject.gameObject))
                //{
                XShiftWeaponCam(BobObject.gameObject, BobStandJumpValues.MinShift.x, BobStandJumpValues.MaxShift.x, BobStandJumpValues.XShiftDuration, BobStandJumpValues.XShiftDelay);
                YShiftWeaponCam(BobObject.gameObject, BobStandJumpValues.MinShift.y, BobStandJumpValues.MaxShift.y, BobStandJumpValues.YShiftDuration, BobStandJumpValues.YShiftDelay);
                ZShiftWeaponCam(BobObject.gameObject, BobStandJumpValues.MinShift.z, BobStandJumpValues.MaxShift.z, BobStandJumpValues.ZShiftDuration, BobStandJumpValues.ZShiftDelay);

                RotWeaponCam(BobObject.gameObject, BobStandJumpValues.MinRotation, BobStandJumpValues.MaxRotation, BobStandJumpValues.RotationDuration, BobStandJumpValues.RotationDelay);
                JumpStopLoop = true;
                //}
            }
        }
        public void JumpCrouchValues()
        {
            if (JumpStopLoop == false)
            {
                //if (!LeanTween.isTweening(BobObject.gameObject))
                //{
                XShiftWeaponCam(BobObject.gameObject, BobCrouchJumpValues.MinShift.x, BobCrouchJumpValues.MaxShift.x, BobCrouchJumpValues.XShiftDuration, BobCrouchJumpValues.XShiftDelay);
                YShiftWeaponCam(BobObject.gameObject, BobCrouchJumpValues.MinShift.y, BobCrouchJumpValues.MaxShift.y, BobCrouchJumpValues.YShiftDuration, BobCrouchJumpValues.YShiftDelay);
                ZShiftWeaponCam(BobObject.gameObject, BobCrouchJumpValues.MinShift.z, BobCrouchJumpValues.MaxShift.z, BobCrouchJumpValues.ZShiftDuration, BobCrouchJumpValues.ZShiftDelay);

                RotWeaponCam(BobObject.gameObject, BobCrouchJumpValues.MinRotation, BobCrouchJumpValues.MaxRotation, BobCrouchJumpValues.RotationDuration, BobCrouchJumpValues.RotationDelay);
                JumpStopLoop = true;
                //}
            }
        }
        public void WalkAutoResetFunction()
        {
            if (WalkStopLoop == true)
            {
                if (WalkAutoReset == false)
                {
                    DefaultLoopingValues(BobResetDurations.StandWalkResetDuration, BobResetDurations.CrouchWalkResetDuration, BobResetDurations.StationaryStandAimedResetDuration, BobResetDurations.StationaryCrouchAimedResetDuration, "WalkAutoReset");
                    AutoReset = true;
                    WalkAutoReset = true;
                }
            }
        }
        public void JumpAutoResetFunction()
        {
            if (JumpStopLoop == true)
            {
                if (JumpAutoReset == false)
                {
                    DefaultLoopingValues(BobResetDurations.StandJumpResetDuration, BobResetDurations.CrouchJumpResetDuration, BobResetDurations.StationaryStandAimedResetDuration, BobResetDurations.StationaryCrouchAimedResetDuration, "JumpAutoReset");
                    AutoReset = true;
                    JumpAutoReset = true;
                }
            }
        }
    }
}