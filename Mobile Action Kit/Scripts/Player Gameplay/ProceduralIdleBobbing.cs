using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class ProceduralIdleBobbing : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script dynamically adjusts the weapon's idle bobbing movement based on the player's idle behaviour. It smoothly interpolates positional and rotational shifts to create a procedural idle animations" +
            "The script utilizes LeanTween for animations and supports separate bobbing behaviors for hip fire and aimed states, both in standing and crouching positions.";
        [Space(10)]

        [Tooltip("Reference to the PlayerManager script, which handles player state and weapon interactions.")]
        public PlayerManager PlayerManagerScript;

        [Tooltip("The object that will be animated for procedural bobbing (typically the weapon or camera).")]
        public GameObject BobObject;

        [Tooltip("Default position offset for hip fire stance.")]
        public Vector3 HipFirePosValue;

        [Tooltip("Default rotation offset for hip fire stance.")]
        public Vector3 HipFireRotValue;

        [Tooltip("Default position offset when aiming down sights.")]
        public Vector3 AimedPosValue;

        [Tooltip("Default rotation offset when aiming down sights.")]
        public Vector3 AimedRotValue;

        [Tooltip("Time taken to reset the bobbing effect when the player is standing in hip fire mode.")]
        public float StationaryStandHipFireResetDuration;

        [Tooltip("Time taken to reset the bobbing effect when the player is crouching in hip fire mode.")]
        public float StationaryCrouchHipFireResetDuration;

        [Tooltip("Time taken to reset the bobbing effect when the player is standing in aimed mode.")]
        public float StationaryStandAimedResetDuration;

        [Tooltip("Time taken to reset the bobbing effect when the player is crouching in aimed mode.")]
        public float StationaryCrouchAimedResetDuration;

        [Tooltip("Bobbing properties when the player is standing in hip fire mode.")]
        public HeadBobValues StandHipFireIdleValues;

        [Tooltip("Bobbing properties when the player is crouching in hip fire mode.")]
        public HeadBobValues CrouchHipFireIdleValues;

        [Tooltip("Bobbing properties when the player is standing while aiming.")]
        public HeadBobValues StandAimedIdleValues;

        [Tooltip("Bobbing properties when the player is crouching while aiming.")]
        public HeadBobValues CrouchAimedIdleValues;

        [System.Serializable]
        public class HeadBobValues
        {
            [Tooltip("Minimum position shift values for procedural bobbing.")]
            public Vector3 MinShift;

            [Tooltip("Maximum position shift values for procedural bobbing.")]
            public Vector3 MaxShift;

            [Tooltip("Minimum rotation shift values for procedural bobbing.")]
            public Vector3 MinRotation;

            [Tooltip("Maximum rotation shift values for procedural bobbing.")]
            public Vector3 MaxRotation;

            [Tooltip("Time taken to complete one bobbing cycle along the X-axis.")]
            public float XShiftDuration;

            [Tooltip("Time taken to complete one bobbing cycle along the Y-axis.")]
            public float YShiftDuration;

            [Tooltip("Time taken to complete one bobbing cycle along the Z-axis.")]
            public float ZShiftDuration;

            [Tooltip("Time taken to complete one bobbing cycle for rotations.")]
            public float RotationDuration;

            [Tooltip("Delay before starting the bobbing motion on the X-axis.")]
            public float XShiftDelay;

            [Tooltip("Delay before starting the bobbing motion on the Y-axis.")]
            public float YShiftDelay;

            [Tooltip("Delay before starting the bobbing motion on the Z-axis.")]
            public float ZShiftDelay;

            [Tooltip("Delay before starting the bobbing motion for rotations.")]
            public float RotationDelay;
        }

        bool StartBob = false;
        bool StopBob = false;

        bool ResetBob = false;
        bool ResetFromSwitch = false;

        bool CancelTween = false;
        bool CheckAiming = false;

        bool CheckCrouch = false;

        void OnDisable()
        {
            LeanTween.cancel(BobObject.gameObject);
            ResettingDefaultValues();
        }
        void OnEnable()
        {
            LeanTween.cancel(BobObject.gameObject);
        }
        void Update()
        {
            if(PlayerManagerScript.CurrentHoldingPlayerWeapon != null)
            {
                if (PlayerManagerScript.CurrentHoldingPlayerWeapon.Reload.isreloading == true)
                {
                    if (CancelTween == false)
                    {
                        //PlayerManagerScript.ob.IsHipFire = true;
                        //PlayerManagerScript.ob.IsAimed = false;
                        LeanTween.cancel(BobObject.gameObject);
                        StartBob = false;
                        CancelTween = true;
                    }
                }
                else
                {
                    if (CancelTween == true)
                    {
                        CancelTween = false;
                        ResetBob = false;
                        PlayerManagerScript.CurrentHoldingPlayerWeapon.IsHipFire = true;
                    }
                }
            }
            else
            {
                if (CancelTween == true)
                {
                    CancelTween = false;
                    ResetBob = false;
                    PlayerManagerScript.CurrentHoldingPlayerWeapon.IsHipFire = true;
                }

            }


            //if(SwitchWeapons.instance != null)
            //{
            //    if(SwitchWeapons.instance.WeaponSwitched == true)
            //    {
            //        //if(ResetFromSwitch == false)
            //        //{
            //        //    ResettingDefaultValues();
            //        //    ResetFromSwitch = true;
            //        //}
            //        ResetBob = false;
            //        PlayerManagerScript.ob.IsHipFire = true;
            //    }
            //}


            if (Crouch.instance.IsCrouching == true)
            {
                if (CheckCrouch == false)
                {
                    CheckAiming = false;
                    ResetBob = false;
                    CheckCrouch = true;
                }
            }
            else
            {
                if (CheckCrouch == true)
                {
                    CheckAiming = false;
                    ResetBob = false;
                    CheckCrouch = false;
                }
            }



            if(PlayerManagerScript.CurrentHoldingPlayerWeapon != null)
            {
                if (PlayerManagerScript.CurrentHoldingPlayerWeapon.IsHipFire == true)
                {
                    if (ResetBob == false)
                    {
                        CheckAiming = false;
                        ResettingDefaultValues();
                        ResetBob = true;
                    }

                }
                else
                {
                    //if(ResetBob == true)
                    //{
                    //    ResettingDefaultValues();
                    //    ResetBob = false;
                    //}
                    if (CheckAiming == false)
                    {
                        ResetBob = false;
                        ResettingDefaultValues();
                        CheckAiming = true;
                    }
                }
            }
            else
            {
                if (CheckAiming == false)
                {
                    ResetBob = false;
                    ResettingDefaultValues();
                    CheckAiming = true;
                }
            }
          

            if (PlayerManager.instance.IsMoving == false && FirstPersonController.instance.jump == false)
            {
                if(JoyStick.Instance != null)
                {
                    if (JoyStick.Instance.IsWalking == false)
                    {
                        IdleValues();
                    }
                }
                else
                {
                    IdleValues();
                }
               
            }
            else
            {
                ResettingDefaultValues();
            }

        }
        public void IdleValues()
        {
            if (PlayerManagerScript.CurrentHoldingPlayerWeapon != null && StartBob == false && !LeanTween.isTweening(BobObject.gameObject) && PlayerManagerScript.CurrentHoldingPlayerWeapon.Reload.isreloading == false)
            {
                if (PlayerManagerScript.CurrentHoldingPlayerWeapon.IsAimed == true)
                {
                    if (Crouch.instance.IsCrouching == false)
                    {
                        XShiftWeaponCam(BobObject.gameObject, StandAimedIdleValues.MinShift.x, StandAimedIdleValues.MaxShift.x, StandAimedIdleValues.XShiftDuration, StandAimedIdleValues.XShiftDelay);
                        YShiftWeaponCam(BobObject.gameObject, StandAimedIdleValues.MinShift.y, StandAimedIdleValues.MaxShift.y, StandAimedIdleValues.YShiftDuration, StandAimedIdleValues.YShiftDelay);
                        ZShiftWeaponCam(BobObject.gameObject, StandAimedIdleValues.MinShift.z, StandAimedIdleValues.MaxShift.z, StandAimedIdleValues.ZShiftDuration, StandAimedIdleValues.ZShiftDelay);
                        RotWeaponCam(BobObject.gameObject, StandAimedIdleValues.MinRotation, StandAimedIdleValues.MaxRotation, StandAimedIdleValues.RotationDuration, StandAimedIdleValues.RotationDelay);
                    }
                    else
                    {
                        XShiftWeaponCam(BobObject.gameObject, CrouchAimedIdleValues.MinShift.x, CrouchAimedIdleValues.MaxShift.x, CrouchAimedIdleValues.XShiftDuration, CrouchAimedIdleValues.XShiftDelay);
                        YShiftWeaponCam(BobObject.gameObject, CrouchAimedIdleValues.MinShift.y, CrouchAimedIdleValues.MaxShift.y, CrouchAimedIdleValues.YShiftDuration, CrouchAimedIdleValues.YShiftDelay);
                        ZShiftWeaponCam(BobObject.gameObject, CrouchAimedIdleValues.MinShift.z, CrouchAimedIdleValues.MaxShift.z, CrouchAimedIdleValues.ZShiftDuration, CrouchAimedIdleValues.ZShiftDelay);
                        RotWeaponCam(BobObject.gameObject, CrouchAimedIdleValues.MinRotation, CrouchAimedIdleValues.MaxRotation, CrouchAimedIdleValues.RotationDuration, CrouchAimedIdleValues.RotationDelay);
                    }
                }
                else
                {
                    if (Crouch.instance.IsCrouching == false)
                    {
                        XShiftWeaponCam(BobObject.gameObject, StandHipFireIdleValues.MinShift.x, StandHipFireIdleValues.MaxShift.x, StandHipFireIdleValues.XShiftDuration, StandHipFireIdleValues.XShiftDelay);
                        YShiftWeaponCam(BobObject.gameObject, StandHipFireIdleValues.MinShift.y, StandHipFireIdleValues.MaxShift.y, StandHipFireIdleValues.YShiftDuration, StandHipFireIdleValues.YShiftDelay);
                        ZShiftWeaponCam(BobObject.gameObject, StandHipFireIdleValues.MinShift.z, StandHipFireIdleValues.MaxShift.z, StandHipFireIdleValues.ZShiftDuration, StandHipFireIdleValues.ZShiftDelay);
                        RotWeaponCam(BobObject.gameObject, StandHipFireIdleValues.MinRotation, StandHipFireIdleValues.MaxRotation, StandHipFireIdleValues.RotationDuration, StandHipFireIdleValues.RotationDelay);
                    }
                    else
                    {
                        XShiftWeaponCam(BobObject.gameObject, CrouchHipFireIdleValues.MinShift.x, CrouchHipFireIdleValues.MaxShift.x, CrouchHipFireIdleValues.XShiftDuration, CrouchHipFireIdleValues.XShiftDelay);
                        YShiftWeaponCam(BobObject.gameObject, CrouchHipFireIdleValues.MinShift.y, CrouchHipFireIdleValues.MaxShift.y, CrouchHipFireIdleValues.YShiftDuration, CrouchHipFireIdleValues.YShiftDelay);
                        ZShiftWeaponCam(BobObject.gameObject, CrouchHipFireIdleValues.MinShift.z, CrouchHipFireIdleValues.MaxShift.z, CrouchHipFireIdleValues.ZShiftDuration, CrouchHipFireIdleValues.ZShiftDelay);
                        RotWeaponCam(BobObject.gameObject, CrouchHipFireIdleValues.MinRotation, CrouchHipFireIdleValues.MaxRotation, CrouchHipFireIdleValues.RotationDuration, CrouchHipFireIdleValues.RotationDelay);
                    }
                }

                StartBob = true;
                StopBob = false;
                ResetFromSwitch = false;
            }
        }
        public void ResettingDefaultValues()
        {
            StartBob = false;
            if (StopBob == false)
            {
                LeanTween.cancel(BobObject);

                if(PlayerManagerScript.CurrentHoldingPlayerWeapon != null)
                {
                    if (PlayerManagerScript.CurrentHoldingPlayerWeapon.IsAimed == true)
                    {
                        if (Crouch.instance.IsCrouching == false)
                        {
                            LeanTween.moveLocal(BobObject.gameObject, HipFirePosValue, StationaryStandAimedResetDuration).setFrom(BobObject.transform.localPosition);
                            LeanTween.rotateLocal(BobObject.gameObject, HipFireRotValue, StationaryStandAimedResetDuration).setFrom(BobObject.transform.localEulerAngles);
                        }
                        else
                        {
                            LeanTween.moveLocal(BobObject.gameObject, HipFirePosValue, StationaryCrouchAimedResetDuration).setFrom(BobObject.transform.localPosition);
                            LeanTween.rotateLocal(BobObject.gameObject, HipFireRotValue, StationaryCrouchAimedResetDuration).setFrom(BobObject.transform.localEulerAngles);
                        }
                    }
                    else
                    {
                        if (Crouch.instance.IsCrouching == false)
                        {
                            LeanTween.moveLocal(BobObject.gameObject, HipFirePosValue, StationaryStandHipFireResetDuration).setFrom(BobObject.transform.localPosition);
                            LeanTween.rotateLocal(BobObject.gameObject, HipFireRotValue, StationaryStandHipFireResetDuration).setFrom(BobObject.transform.localEulerAngles);
                        }
                        else
                        {
                            LeanTween.moveLocal(BobObject.gameObject, HipFirePosValue, StationaryCrouchHipFireResetDuration).setFrom(BobObject.transform.localPosition);
                            LeanTween.rotateLocal(BobObject.gameObject, HipFireRotValue, StationaryCrouchHipFireResetDuration).setFrom(BobObject.transform.localEulerAngles);
                        }
                    }
                }
                else
                {
                    if (Crouch.instance.IsCrouching == false)
                    {
                        LeanTween.moveLocal(BobObject.gameObject, HipFirePosValue, StationaryStandHipFireResetDuration).setFrom(BobObject.transform.localPosition);
                        LeanTween.rotateLocal(BobObject.gameObject, HipFireRotValue, StationaryStandHipFireResetDuration).setFrom(BobObject.transform.localEulerAngles);
                    }
                    else
                    {
                        LeanTween.moveLocal(BobObject.gameObject, HipFirePosValue, StationaryCrouchHipFireResetDuration).setFrom(BobObject.transform.localPosition);
                        LeanTween.rotateLocal(BobObject.gameObject, HipFireRotValue, StationaryCrouchHipFireResetDuration).setFrom(BobObject.transform.localEulerAngles);
                    }
                }
            

                StopBob = true;
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
        public void RotWeaponCam(GameObject obj, Vector3 MinShift, Vector3 MaxShift, float Dur, float Delay)
        {
            LeanTween.rotateLocal(obj, MinShift, Mathf.Abs(Dur)).setFrom(obj.transform.localEulerAngles).setDelay(Delay);
            LeanTween.rotateLocal(obj, MaxShift, Mathf.Abs(Dur)).setFrom(MinShift).setLoopPingPong().setDelay(Mathf.Abs(Dur) + Delay);
        }
    }
}
