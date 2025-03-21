using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class MouseControls : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script manages mouse input for player actions such as shooting and aiming. It tracks mouse button presses and interacts with player movement, weapon handling, and game controls.";

        public static MouseControls instance;

        [Tooltip("Reference to the MouseScrollWeaponZoom script responsible for managing FOV adjustments.")]
        public MouseScrollWeaponZoom MouseScrollWeaponZoomScript;

        [Tooltip("Reference to the PcInputManager script.")]
        public PcInputManager PcInputManagerScript;


        public enum Options
        {
            Fire,
            Aim
        }

        [System.Serializable]
        public class MouseKeys
        {
            [Tooltip("The action triggered by the assigned mouse key.")]
            public Options Control;

            [Tooltip("The key assigned to perform the selected action.")]
            public KeyCode KeyCode;
        }

        [Space(10)]
        [Tooltip("List of mouse key bindings for different actions.")]
        public List<MouseKeys> MouseClicks = new List<MouseKeys>();

        bool SomeKeyIsinuse = false;

        KeyCode NewKey;
        KeyCode Prevkey;

        bool IsAnyActionPerformed = false;
        string PrevControlName;
        string NewControlName;
        bool ResetValues = false;
        bool IsReseting = false;

        bool Once = false;
        bool ResetOnce = false;
        int Counts;

        KeyCode[] StorePrevKeys;
        int MyKey;

        bool ShouldStartCalculations = false;

        KeyCode PreviousKeyCode;
        bool WasRunningPreviously = false;

        bool FixAiming = false;

        [HideInInspector]
        public bool StopFunctioning = false;

        //bool GetAnyKey(params KeyCode[] aKeys)
        //{
        //    StorePrevKeys = new KeyCode[aKeys.Length];

        //    for (int p = 0; p < StorePrevKeys.Length; p++)
        //    {
        //        StorePrevKeys[p] = aKeys[p];
        //    }
        //    bool CheckOnce = false;
        //    Counts = 0;
        //    foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        //    {
        //        if (Input.GetKey(vKey))
        //        {
        //            ++Counts;
        //        }
        //    }

        //    for (int x = 0; x < aKeys.Length; x++)
        //    {
        //        if (CheckOnce == false)
        //        {
        //            if (!Input.GetKey(aKeys[x]))
        //            {
        //                return false;
        //            }
        //        }

        //    }

        //    if (aKeys.Length > 0 && Counts == aKeys.Length)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        private void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if (StopFunctioning == false)
            {
                if (ShouldStartCalculations == true)
                {
                    foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
                    {
                        if (Input.GetKey(vKey))
                        {
                            if (vKey != NewKey)
                            {
                                Once = false;
                                NewKey = vKey;
                                SomeKeyIsinuse = false;

                                if (ResetOnce == false)
                                {
                                    IsReseting = false;
                                    ResetValues = true;
                                    ResetOnce = true;
                                }
                            }
                            else
                            {
                                if (SomeKeyIsinuse == true && Once == false)
                                {
                                    ResetOnce = false;
                                    SomeKeyIsinuse = false;
                                    IsReseting = false;
                                    ResetValues = true;
                                    Once = true;
                                }

                            }

                        }
                    }
                }

                if (SomeKeyIsinuse == false && ResetValues == false && PcInputManagerScript.IsRunning == false
                    && PcInputManagerScript.IsJumping == false && PcInputManagerScript.IsKnifeAttack == false && PcInputManagerScript.IsThrowingGrende == false
                    && PcInputManagerScript.IsThrowingInsendiaryGrende == false
                    && PcInputManagerScript.IsUsingMenu == false)
                {
                    for (int x = 0; x < MouseClicks.Count; x++)
                    {
                        //if (MouseClicks[x].ControlName == MouseClicks[x].Control.ToString())
                        //{
                        //Debug.Log(MouseClicks[x].ControlName + "Using Mouse Controls Key" + " " + SomeKeyIsinuse + GetAnyKey(MouseClicks[x].KeyCode));
                        if (Input.GetKey(MouseClicks[x].KeyCode) && SomeKeyIsinuse == false)//GetAnyKey(MouseClicks[x].KeyCode)
                        {
                            NewControlName = MouseClicks[x].Control.ToString();
                            IsAnyActionPerformed = false;
                            SomeKeyIsinuse = true;
                            IsReseting = true;
                            MyKey = x;
                            ShouldStartCalculations = true;
                            FixAiming = false;
                            GameControls(MouseClicks[x].Control.ToString());
                            PreviousKeyCode = MouseClicks[x].KeyCode;
                       
                        }
                        //}
                    }
                }

                if (PcInputManagerScript.IsRunning == true && WasRunningPreviously == false
                    || PcInputManagerScript.IsJumping == true && WasRunningPreviously == false
                    || PcInputManagerScript.IsKnifeAttack == true && WasRunningPreviously == false
                    || PcInputManagerScript.IsThrowingGrende == true && WasRunningPreviously == false
                    || PcInputManagerScript.IsUsingMenu == true && WasRunningPreviously == false
                    || PcInputManagerScript.IsThrowingInsendiaryGrende == true && WasRunningPreviously == false)
                {
                    PreviousKeyCode = KeyCode.None;
                    WasRunningPreviously = true;
                }
                else if (WasRunningPreviously == true)
                {
                    PreviousKeyCode = KeyCode.None;
                    WasRunningPreviously = false;
                }

                if (IsAnyActionPerformed == true && ShouldStartCalculations == true)
                {
                    //if (GetAnyKey(MouseClicks[MyKey].KeyCode) == false)
                    if (Input.GetKey(MouseClicks[MyKey].KeyCode))
                    {
                        IsReseting = false;
                        ResetValues = true;
                        ResetOnce = true;
                    }


                    if (Input.GetKey(PreviousKeyCode))
                    {
                        NewControlName = MouseClicks[MyKey].Control.ToString();
                        IsAnyActionPerformed = false;
                        SomeKeyIsinuse = true;
                        IsReseting = true;
                        ShouldStartCalculations = true;
                        // GameControls(MouseClicks[MyKey].ControlName);
                    }

                }

                if (ResetValues == true && IsReseting == false || ShouldStartCalculations == true)
                {
                    if (Input.GetKey(PreviousKeyCode))
                    {
                        NewControlName = MouseClicks[MyKey].Control.ToString();
                        IsAnyActionPerformed = false;
                        SomeKeyIsinuse = true;
                        IsReseting = true;
                        ShouldStartCalculations = true;
                        //GameControls(MouseClicks[MyKey].ControlName);
                    }
                    else
                    {
                        ResetSettings();
                    }
                }
            }

        }
        public void ResetSettings()
        {
            SomeKeyIsinuse = false;
            ResetValues = false;
            ShouldStartCalculations = true;
            PlayerManager.instance.StopFiring();
        }
        public void GameControls(string Control)
        {
            if (PlayerManager.instance.CurrentHoldingPlayerWeapon.Reload.isreloading == false)
            {
                if (Control == Options.Fire.ToString())
                {
                    PlayerManager.instance.StopBobbing();
                    if (PlayerManager.instance.CurrentHoldingPlayerWeapon.ShootingFeatures.ForceFullAutoFire == true)
                    {
                        PlayerManager.instance.FireContinue();
                        IsAnyActionPerformed = true;
                    }
                    else
                    {
                        PlayerManager.instance.FireOneShot();
                        IsAnyActionPerformed = true;
                    }
                }
                else if (Control == Options.Aim.ToString())
                {
                    if (Input.GetKeyDown(MouseClicks[MyKey].KeyCode))
                    {
                        if (FixAiming == false)
                        {                          
                            PlayerManager.instance.Aiming();
                            MouseScrollWeaponZoomScript.currentFOV = PlayerManager.instance.CurrentHoldingPlayerWeapon.CamerasFovParameters.PlayerCameraMagnifiedFov;
                            IsAnyActionPerformed = true;
                            FixAiming = true;
                        }

                    }
                }
            }
        }
    }
}