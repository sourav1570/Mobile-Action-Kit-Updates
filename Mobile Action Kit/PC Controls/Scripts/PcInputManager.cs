using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class PcInputManager : MonoBehaviour
    {
        [TextArea]
        [HideInInspector]
        public string ScriptInfo = "PcInputHandler manages all keyboard inputs for controlling the player character in a first-person shooter (FPS) or action game. "
      + "It allows for single-key and dual-key inputs, supports continuous function execution while keys are held, and provides customizable bindings for actions like movement, sprinting, jumping, crouching, "
      + "weapon switching, grenade throwing, air/artillery support calls, and menu interactions. "
      + "This script also handles weapon switching. "
      + "Developers can add custom scripts and functions to be executed when specific keys are pressed or reset. "
      + "Additionally, it includes built-in logic to track player states such as running, crouching, jumping, and using items.";


        [Tooltip("Reference to the MouseScrollingControl script, used for handling mouse scrolling inputs.")]
        public MouseScrollWeaponZoom MouseScrollWeaponZoomScript;

        [Tooltip("If enabled, sprinting requires two key inputs (e.g., Move Forward + Sprint key).")]
        public bool UseTwoKeyInputsForSprinting = true;

        public enum Options
        {
            MoveForward,
            MoveBackward,
            MoveRight,
            MoveLeft,
            Sprint,
            Jump,
            Crouch,
            Reload,
            GrenadeThrow,
            InsendiaryGrenadeThrow,
            UseHealthPack,
            KnifeAttack,
            CallAirSupport,
            CallArtillerySupport,
            Pause,
            WeaponPickup,
            WeaponAmmoPickup,
            AmmoBoxPickup,
            ItemPickup
        }

        [System.Serializable]
        public class PcKeys
        {
            [Tooltip("Select the action assigned to this key (e.g., MoveForward, Sprint, Jump, etc.).")]
            public Options Control;

            [Tooltip("The KeyCode assigned to trigger this action.")]
            public KeyCode KeyCode;

            [Tooltip("If enabled, the function will be called continuously while the key is held down.")]
            public bool CallFunctionContinously = false;

            [Tooltip("List of scripts that will be enabled when this key is pressed.")]
            public List<MonoBehaviour> AddScripts = new List<MonoBehaviour>();

            [HideInInspector]
            [Tooltip("List of function names to invoke when this key is pressed.")]
            public List<string> FunctionsToInvoke = new List<string>();

            [Tooltip("List of scripts that will be enabled when resetting this key action.")]
            public List<MonoBehaviour> AddScriptsForReset = new List<MonoBehaviour>();

            [HideInInspector]
            [Tooltip("List of function names to invoke when resetting this key action.")]
            public List<string> FunctionsToInvokeForReset = new List<string>();
        }

        [Tooltip("List of key bindings for various player actions.")]
        public List<PcKeys> SingleKeycodeInputs = new List<PcKeys>();

        //[System.Serializable]
        //public class SprintingKeycodes
        //{
        //    public KeyCode MoveForward;
        //    public KeyCode Sprint;
        //}

        //public SprintingKeycodes SprintKeycode;
        [System.Serializable]
        public class SwitchWeaponNumericClass
        { 
            [Tooltip("Key assigned to switch to this weapon.")]
            public KeyCode KeyCode;
        }

        [Tooltip("Reference to the PlayerWeaponsManager script, which handles weapon switching.")]
        public PlayerWeaponsManager PlayerWeaponsManagerScript;

        [Tooltip("List of numeric keys assigned for switching between shooting weapons.")]
        public List<SwitchWeaponNumericClass> ShootingWeaponsSwitching = new List<SwitchWeaponNumericClass>();

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

        bool IsWalking = false;

        [HideInInspector]
        public bool IsRunning = false;

        bool IsCrouching = false;

        [HideInInspector]
        public bool IsJumping = false;

        [HideInInspector]
        public bool IsThrowingGrende = false;
        [HideInInspector]
        public bool IsThrowingInsendiaryGrende = false;

        [HideInInspector]
        public bool IsUsingHealthPack = false;
        [HideInInspector]
        public bool IsCallingAirSupport = false;
        [HideInInspector]
        public bool IsCallingArtillerySupport = false;

        [HideInInspector]
        public bool IsKnifeAttack = false;

        [HideInInspector]
        public bool IsUsingMenu = false;
        [HideInInspector]
        public bool SwitchSprinting = false;

        bool ResetDualControl = false;
        bool IsSwitchingToRun = false;

        [HideInInspector]
        public KeyCode[] SprintKeycodeSetup;
        int OneTimeKey;

        [HideInInspector]
        public bool StopFunctioning = false;

        int GetSprintIndex ;


        //bool IsInsendiaryHandsPreviouslyActivated = false;
        //bool IsInsendiaryHandsPreviouslyActivated = false;
        //bool IsMeleeHandsPreviouslyActivated = false;
        private void Awake()
        {
            if (UseTwoKeyInputsForSprinting == true)
            {
                SprintKeycodeSetup = new KeyCode[2];

                for (int x = 0; x < SingleKeycodeInputs.Count; x++)
                {
                    if (SingleKeycodeInputs[x].Control.ToString() == Options.MoveForward.ToString())
                    {
                        SprintKeycodeSetup[0] = SingleKeycodeInputs[x].KeyCode;
                    }
                    if (SingleKeycodeInputs[x].Control.ToString() == Options.Sprint.ToString())
                    {
                        GetSprintIndex = x;
                           SprintKeycodeSetup[1] = SingleKeycodeInputs[x].KeyCode;
                    }
                }
            }
            else
            {
                SprintKeycodeSetup = new KeyCode[1];

                for (int x = 0; x < SingleKeycodeInputs.Count; x++)
                {
                    if (SingleKeycodeInputs[x].Control.ToString() == Options.Sprint.ToString())
                    {
                        GetSprintIndex = x;
                        SprintKeycodeSetup[0] = SingleKeycodeInputs[x].KeyCode;
                    }
                }
            }



        }
        bool GetAnyKey(params KeyCode[] aKeys)
        {
            StorePrevKeys = new KeyCode[aKeys.Length];

            for (int p = 0; p < StorePrevKeys.Length; p++)
            {
                StorePrevKeys[p] = aKeys[p];
            }
            //bool CheckOnce = false;
            Counts = 0;
            //foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            //{
            //    if (Input.GetKey(vKey))
            //    {
            //        ++Counts;
            //    }
            //}

            for (int x = 0; x < aKeys.Length; x++)
            {
                if (Input.GetKey(aKeys[x]))
                {
                    ++Counts;
                }
            }

            //for (int x = 0; x < aKeys.Length; x++)
            //{
            //    if (CheckOnce == false)
            //    {
            //        if (!Input.GetKey(aKeys[x]))
            //        {
            //            return false;
            //        }
            //    }

            //}

            if (Counts >= aKeys.Length)
            {
                return true;
            }
            else
            {
                return false;
            }



        }
        public void ResetSettings()
        {
            SomeKeyIsinuse = false;
            ResetValues = false;
            IsReseting = false;
            ShouldStartCalculations = false;
            IsAnyActionPerformed = false;

            //PlayerManager.instance.OnpointerUp(false);
            //JoyStick.Instance.StopWalk();
            //JoyStick.Instance.PcStopWalk();
            IsRunning = false;
            IsThrowingGrende = false;
            IsThrowingInsendiaryGrende = false;
            IsUsingHealthPack = false;
            IsCallingAirSupport = false;
            IsCallingArtillerySupport = false;
            IsKnifeAttack = false;
            IsJumping = false;

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

                if (Input.anyKey == true)
                {
                    for (int s = 0; s < ShootingWeaponsSwitching.Count; s++)
                    {
                        if (Input.GetKeyDown(ShootingWeaponsSwitching[s].KeyCode))
                        {
                            if(PlayerWeaponsManagerScript.ActiveWeaponsList[s] != null)
                            {
                                for(int x =0;x < PlayerWeaponsManagerScript.WeaponSlots.Count; x++)
                                {
                                    for(int i = 0; i < PlayerWeaponsManagerScript.WeaponSlots[x].Weapons.Count; i++)
                                    {
                                        if(PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon == PlayerWeaponsManagerScript.ActiveWeaponsList[s].gameObject)
                                        {
                                            ShouldStartCalculations = true;
                                            PlayerWeaponsManagerScript.WeaponActivation(PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].WeaponIdComponent.WeaponName);
                                        }
                                    }
                                }
                            }
                         
                            //PlayerWeaponsManagerScript.WeaponActivation(ShootingWeaponsSwitching[s].Slot);
                        }
                    }

                    //for (int y = 0; y < PlayerWeaponsManagerScript.WeaponSlots.Count; y++)
                    //{
                    //    for (int x = 0; x < PlayerWeaponsManagerScript.WeaponSlots.Count; x++)
                    //    {
                    //        for (int p = 0; p < PlayerWeaponsManagerScript.WeaponSlots[x].Weapons.Count; p++)
                    //        {
                    //            for (int s = 0; s < ShootingWeaponsSwitching.Count; s++)
                    //            {
                    //                if (PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[p].WeaponIdComponent.WeaponName == ShootingWeaponsSwitching[s].WeaponName)
                    //                {
                    //                    if (Input.GetKeyDown(ShootingWeaponsSwitching[x].KeyCode))
                    //                    {
                    //                        PlayerWeaponsManagerScript.WeaponActivation(ShootingWeaponsSwitching[x].WeaponName);
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}                      
                }

                if (GetAnyKey(SprintKeycodeSetup))
                {
                    if (ResetDualControl == true)
                    {
                        var keyInput = SingleKeycodeInputs[GetSprintIndex];
                        ResetOtherVariableButCall(ref IsRunning);
                        if (keyInput.CallFunctionContinously == false)
                        {
                            foreach (var script in keyInput.AddScripts.Select((s, j) => new { Script = s, Index = j }))
                            {
                                if (script.Script != null)
                                {
                                    string functionName = keyInput.FunctionsToInvoke[script.Index];

                                    MethodInfo method = script.Script.GetType().GetMethod(functionName);
                                  //  Debug.Log(functionName);
                                    if (method != null && method.IsPublic && method.GetParameters().Length == 0)
                                    {
                                        method.Invoke(script.Script, null);
                                    }
                                    else
                                    {
                                        Debug.LogError($"Function '{functionName}' in {script.Script.GetType().Name} is not public or has parameters.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var script in keyInput.AddScripts.Select((s, j) => new { Script = s, Index = j }))
                            {
                                if (script.Script != null)
                                {
                                    string functionName = keyInput.FunctionsToInvoke[script.Index];

                                    MethodInfo method = script.Script.GetType().GetMethod(functionName);
                                   // Debug.Log(functionName);
                                    if (method != null && method.IsPublic && method.GetParameters().Length == 0)
                                    {
                                        method.Invoke(script.Script, null);
                                    }
                                    else
                                    {
                                        Debug.LogError($"Function '{functionName}' in {script.Script.GetType().Name} is not public or has parameters.");
                                    }
                                }
                            }
                        }
                        // PlayerManager.instance.OnpointerDown(true);
                        IsAnyActionPerformed = true;
                    }
                    if (ResetDualControl == false)
                    {
                        var keyInput = SingleKeycodeInputs[GetSprintIndex];

                        foreach (var script in keyInput.AddScriptsForReset)
                        {
                            if (script != null)
                            {
                                int methodIndex = keyInput.AddScriptsForReset.IndexOf(script);
                                string functionName = keyInput.FunctionsToInvokeForReset[methodIndex];

                                MethodInfo method = script.GetType().GetMethod(functionName);
                                if (method != null && method.IsPublic && method.GetParameters().Length == 0)
                                {
                                    method.Invoke(script, null);
                                }
                                else
                                {
                                    Debug.LogError($"Function '{functionName}' in {script.GetType().Name} is not public or has parameters.");
                                }
                            }
                        }

                        ResetSettings();
                        ResetDualControl = true;
                    }

                }
                else
                {
                    HashSet<Options> validControls = new HashSet<Options>
                  {
                       Options.Jump,
                       Options.Crouch,
                       Options.KnifeAttack,
                       Options.GrenadeThrow,
                       Options.Reload,
                       Options.UseHealthPack,
                       Options.CallAirSupport,
                       Options.CallArtillerySupport,
                       Options.InsendiaryGrenadeThrow,
                       Options.AmmoBoxPickup,
                       Options.ItemPickup,
                       Options.WeaponPickup,
                       Options.WeaponAmmoPickup

                   };

                    for (int x = 0; x < SingleKeycodeInputs.Count; x++)
                    {
                        var keyInput = SingleKeycodeInputs[x];
                        
                        // Change to GetKeyDown to ensure it fires only once per press
                        if (Input.GetKeyDown(keyInput.KeyCode) && validControls.Contains(keyInput.Control))
                        {
                            if (keyInput.CallFunctionContinously == false)
                            {
                                
                                foreach (var script in keyInput.AddScripts.Select((s, j) => new { Script = s, Index = j }))
                                {
                                    if (script.Script != null)
                                    {
                                        string functionName = keyInput.FunctionsToInvoke[script.Index];

                                        MethodInfo method = script.Script.GetType().GetMethod(functionName);
                                        //Debug.Log(functionName);
                                        if (method != null && method.IsPublic && method.GetParameters().Length == 0)
                                        {
                                            method.Invoke(script.Script, null);
                                        }
                                        else
                                        {
                                            Debug.LogError($"Function '{functionName}' in {script.Script.GetType().Name} is not public or has parameters.");
                                        }
                                    }
                                }
                            }
                            OneTimeKey = x;
                            GameControls(keyInput.Control.ToString());
                        }


                        if (Input.GetKey(keyInput.KeyCode) && validControls.Contains(keyInput.Control))
                        {
                            if (keyInput.CallFunctionContinously == true)
                            {
                                foreach (var script in keyInput.AddScripts.Select((s, j) => new { Script = s, Index = j }))
                                {
                                    if (script.Script != null)
                                    {
                                        string functionName = keyInput.FunctionsToInvoke[script.Index];

                                        MethodInfo method = script.Script.GetType().GetMethod(functionName);
                                       // Debug.Log(functionName);
                                        if (method != null && method.IsPublic && method.GetParameters().Length == 0)
                                        {
                                            method.Invoke(script.Script, null);
                                        }
                                        else
                                        {
                                            Debug.LogError($"Function '{functionName}' in {script.Script.GetType().Name} is not public or has parameters.");
                                        }
                                    }
                                }
                            }
                        }

                       
                    }


                    //for (int x = 0; x < SingleKeycodeInputs.Count; x++)
                    //{
                    //    //if (SingleKeycodeInputs[x].ControlName == SingleKeycodeInputs[x].Control.ToString())
                    //    //{                  
                    //    if (GetAnyKey(SingleKeycodeInputs[x].KeyCode) && SingleKeycodeInputs[x].Control.ToString() == Options.Jump.ToString()
                    //    || GetAnyKey(SingleKeycodeInputs[x].KeyCode) && SingleKeycodeInputs[x].Control.ToString() == Options.Crouch.ToString()
                    //    || GetAnyKey(SingleKeycodeInputs[x].KeyCode) && SingleKeycodeInputs[x].Control.ToString() == Options.KnifeAttack.ToString()
                    //    || GetAnyKey(SingleKeycodeInputs[x].KeyCode) && SingleKeycodeInputs[x].Control.ToString() == Options.GrenadeThrow.ToString()
                    //    || GetAnyKey(SingleKeycodeInputs[x].KeyCode) && SingleKeycodeInputs[x].Control.ToString() == Options.Reload.ToString()
                    //     || GetAnyKey(SingleKeycodeInputs[x].KeyCode) && SingleKeycodeInputs[x].Control.ToString() == Options.UseHealthPack.ToString()
                    //      || GetAnyKey(SingleKeycodeInputs[x].KeyCode) && SingleKeycodeInputs[x].Control.ToString() == Options.CallAirSupport.ToString()
                    //       || GetAnyKey(SingleKeycodeInputs[x].KeyCode) && SingleKeycodeInputs[x].Control.ToString() == Options.CallArtillerySupport.ToString()
                    //        || GetAnyKey(SingleKeycodeInputs[x].KeyCode) && SingleKeycodeInputs[x].Control.ToString() == Options.InsendiaryGrenadeThrow.ToString())
                    //    {
                    //        if (SingleKeycodeInputs[x].AddScript != null)
                    //        {
                    //            MethodInfo method = SingleKeycodeInputs[x].AddScript.GetType().GetMethod(SingleKeycodeInputs[x].FunctionToInvokeWhenUsing);
                    //            if (method != null)
                    //            {
                    //                if (method.IsPublic && method.GetParameters().Length == 0)
                    //                {
                    //                    method.Invoke(SingleKeycodeInputs[x].AddScript, null);
                    //                }
                    //                else
                    //                {
                    //                    Debug.LogError("Function '" + SingleKeycodeInputs[x].FunctionToInvokeWhenUsing + "' is not public or has parameters.");
                    //                }
                    //            }
                    //        }
                    //        OneTimeKey = x;
                    //        GameControls(SingleKeycodeInputs[x].Control.ToString());
                    //    }
                    //    // }
                    //}


                    if (ResetDualControl == false)
                    {
                        if (SomeKeyIsinuse == false && Input.anyKey == true && ResetValues == false)
                        {
                            for (int x = 0; x < SingleKeycodeInputs.Count; x++)
                            {
                                var keyInput = SingleKeycodeInputs[x];
                                //if (SingleKeycodeInputs[x].ControlName == SingleKeycodeInputs[x].Control.ToString())
                                //{                  
                                if (GetAnyKey(SingleKeycodeInputs[x].KeyCode) && SomeKeyIsinuse == false && !validControls.Contains(keyInput.Control))
                                {
                                    if(SingleKeycodeInputs[x].Control != Options.Sprint)
                                    {
                                        if (keyInput.CallFunctionContinously == true)
                                        {
                                            foreach (var script in keyInput.AddScripts.Select((s, j) => new { Script = s, Index = j }))
                                            {
                                                if (script.Script != null)
                                                {
                                                    string functionName = keyInput.FunctionsToInvoke[script.Index];

                                                    MethodInfo method = script.Script.GetType().GetMethod(functionName);
                                                   // Debug.Log(functionName);
                                                    if (method != null && method.IsPublic && method.GetParameters().Length == 0)
                                                    {
                                                        method.Invoke(script.Script, null);
                                                    }
                                                    else
                                                    {
                                                        Debug.LogError($"Function '{functionName}' in {script.Script.GetType().Name} is not public or has parameters.");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if(UseTwoKeyInputsForSprinting == false)
                                        {
                                            if (keyInput.CallFunctionContinously == true)
                                            {
                                                foreach (var script in keyInput.AddScripts.Select((s, j) => new { Script = s, Index = j }))
                                                {
                                                    if (script.Script != null)
                                                    {
                                                        string functionName = keyInput.FunctionsToInvoke[script.Index];

                                                        MethodInfo method = script.Script.GetType().GetMethod(functionName);
                                                       // Debug.Log(functionName);
                                                        if (method != null && method.IsPublic && method.GetParameters().Length == 0)
                                                        {
                                                            method.Invoke(script.Script, null);
                                                        }
                                                        else
                                                        {
                                                            Debug.LogError($"Function '{functionName}' in {script.Script.GetType().Name} is not public or has parameters.");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                       
                                    }
                                   
                                    IsAnyActionPerformed = false;
                                    GameControls(SingleKeycodeInputs[x].Control.ToString());

                                    if (IsAnyActionPerformed == true)
                                    {
                                        if (SingleKeycodeInputs[x].Control != Options.Sprint)
                                        {
                                           
                                            if (keyInput.CallFunctionContinously == false)
                                            {
                                                foreach (var script in keyInput.AddScripts.Select((s, j) => new { Script = s, Index = j }))
                                                {
                                                    if (script.Script != null)
                                                    {
                                                        string functionName = keyInput.FunctionsToInvoke[script.Index];

                                                        MethodInfo method = script.Script.GetType().GetMethod(functionName);
                                                       // Debug.Log(functionName);
                                                        if (method != null && method.IsPublic && method.GetParameters().Length == 0)
                                                        {
                                                            method.Invoke(script.Script, null);
                                                        }
                                                        else
                                                        {
                                                            Debug.LogError($"Function '{functionName}' in {script.Script.GetType().Name} is not public or has parameters.");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (UseTwoKeyInputsForSprinting == false)
                                            {
                                                if (keyInput.CallFunctionContinously == false)
                                                {
                                                    foreach (var script in keyInput.AddScripts.Select((s, j) => new { Script = s, Index = j }))
                                                    {
                                                        if (script.Script != null)
                                                        {
                                                            string functionName = keyInput.FunctionsToInvoke[script.Index];

                                                            MethodInfo method = script.Script.GetType().GetMethod(functionName);
                                                          //  Debug.Log(functionName);
                                                            if (method != null && method.IsPublic && method.GetParameters().Length == 0)
                                                            {
                                                                method.Invoke(script.Script, null);
                                                            }
                                                            else
                                                            {
                                                                Debug.LogError($"Function '{functionName}' in {script.Script.GetType().Name} is not public or has parameters.");
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                        }


                                        SomeKeyIsinuse = true;
                                        IsReseting = true;
                                        MyKey = x;
                                        ShouldStartCalculations = true;
                                        PreviousKeyCode = SingleKeycodeInputs[x].KeyCode;
                                        IsSwitchingToRun = true;
                                        NewControlName = SingleKeycodeInputs[x].Control.ToString();
                                    }
                                }
                                // }
                            }
                        }

                        if (IsAnyActionPerformed == true && ShouldStartCalculations == true)
                        {
                            if (GetAnyKey(SingleKeycodeInputs[MyKey].KeyCode) == false)
                            {
                                IsReseting = false;
                                ResetValues = true;
                                ResetOnce = true;
                            }


                            if (Input.anyKey == true && Input.GetKey(PreviousKeyCode))
                            {
                              
                                var keyInput = SingleKeycodeInputs[MyKey];
                                //if (keyInput.CallFunctionContinously == true)
                                //{
                                //    foreach (var script in keyInput.AddScripts.Select((s, j) => new { Script = s, Index = j }))
                                //    {
                                //        if (script.Script != null)
                                //        {
                                //            string functionName = keyInput.FunctionsToInvoke[script.Index];

                                //            MethodInfo method = script.Script.GetType().GetMethod(functionName);
                                //            Debug.Log(functionName);
                                //            if (method != null && method.IsPublic && method.GetParameters().Length == 0)
                                //            {
                                //                method.Invoke(script.Script, null);
                                //            }
                                //            else
                                //            {
                                //                Debug.LogError($"Function '{functionName}' in {script.Script.GetType().Name} is not public or has parameters.");
                                //            }
                                //        }
                                //    }

                                //}

                                if (SingleKeycodeInputs[MyKey].Control != Options.Sprint && !validControls.Contains(keyInput.Control))
                                {
                                    if (keyInput.CallFunctionContinously == false)
                                    {
                                        foreach (var script in keyInput.AddScripts.Select((s, j) => new { Script = s, Index = j }))
                                        {
                                            if (script.Script != null)
                                            {
                                                string functionName = keyInput.FunctionsToInvoke[script.Index];

                                                MethodInfo method = script.Script.GetType().GetMethod(functionName);
                                               // Debug.Log(functionName);
                                                if (method != null && method.IsPublic && method.GetParameters().Length == 0)
                                                {
                                                    method.Invoke(script.Script, null);
                                                }
                                                else
                                                {
                                                    Debug.LogError($"Function '{functionName}' in {script.Script.GetType().Name} is not public or has parameters.");
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (UseTwoKeyInputsForSprinting == false && !validControls.Contains(keyInput.Control))
                                    {
                                        if (keyInput.CallFunctionContinously == false)
                                        {
                                            foreach (var script in keyInput.AddScripts.Select((s, j) => new { Script = s, Index = j }))
                                            {
                                                if (script.Script != null)
                                                {
                                                    string functionName = keyInput.FunctionsToInvoke[script.Index];

                                                    MethodInfo method = script.Script.GetType().GetMethod(functionName);
                                                   // Debug.Log(functionName);
                                                    if (method != null && method.IsPublic && method.GetParameters().Length == 0)
                                                    {
                                                        method.Invoke(script.Script, null);
                                                    }
                                                    else
                                                    {
                                                        Debug.LogError($"Function '{functionName}' in {script.Script.GetType().Name} is not public or has parameters.");
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }



                                IsAnyActionPerformed = false;
                                GameControls(SingleKeycodeInputs[MyKey].Control.ToString());

                                if (IsAnyActionPerformed == true)
                                {
                                    NewControlName = SingleKeycodeInputs[MyKey].Control.ToString();
                                    SomeKeyIsinuse = true;
                                    IsReseting = true;
                                    ShouldStartCalculations = true;
                                }
                            }

                        }

                        if (ResetValues == true && IsReseting == false || Input.anyKey == false && ShouldStartCalculations == true)
                        {
                            if (Input.anyKey == true && Input.GetKey(PreviousKeyCode))
                            {
                              
                                IsAnyActionPerformed = false;

                                GameControls(SingleKeycodeInputs[MyKey].Control.ToString());

                                if (IsAnyActionPerformed == true)
                                {
                                    NewControlName = SingleKeycodeInputs[MyKey].Control.ToString();
                                    SomeKeyIsinuse = true;
                                    IsReseting = true;
                                    ShouldStartCalculations = true;
                                }
                            }
                            else
                            {
                                

                                for (int x = 0; x < SingleKeycodeInputs.Count; x++)
                                {
                                    var keyInput = SingleKeycodeInputs[x];
                                    foreach (var script in keyInput.AddScriptsForReset)
                                    {
                                        if (script != null)
                                        {
                                            int methodIndex = keyInput.AddScriptsForReset.IndexOf(script);
                                            string functionName = keyInput.FunctionsToInvokeForReset[methodIndex];

                                            MethodInfo method = script.GetType().GetMethod(functionName);
                                            if (method != null && method.IsPublic && method.GetParameters().Length == 0)
                                            {
                                                method.Invoke(script, null);
                                            }
                                            else
                                            {
                                                Debug.LogError($"Function '{functionName}' in {script.GetType().Name} is not public or has parameters.");
                                            }
                                        }
                                    }
                                }
                                //if(IsWalking == false || Input.anyKey == false)
                                //{

                                //}

                                //if(IsWalking == false)
                                //{

                                //}
                                //JoyStick.Instance.StopWalk();
                                //JoyStick.Instance.PcStopWalk();
                                //if (IsRunning == false || Input.anyKey == false)
                                //{
                                // PlayerManager.instance.OnpointerUp(false);

                                //}

                                //if(IsRunning == true || Input.anyKey == false)
                                //{
                                //    PlayerManager.instance.StopFiring();
                                //}
                                //PlayerManager.instance.StopFiring();

                                IsRunning = false;
                                IsThrowingGrende = false;
                                IsThrowingInsendiaryGrende = false;
                                IsUsingHealthPack = false;
                                IsCallingAirSupport = false;
                                IsCallingArtillerySupport = false;

                                IsKnifeAttack = false;
                                IsJumping = false;

                                SomeKeyIsinuse = false;
                                ResetValues = false;
                                if (Input.anyKey == false)
                                {
                                    ShouldStartCalculations = false;
                                }
                            }


                        }
                    }

                    if (ResetDualControl == true)
                    {
                        SomeKeyIsinuse = false;
                        ResetValues = false;
                        IsReseting = false;
                        ShouldStartCalculations = false;
                        IsAnyActionPerformed = false;

                        //PlayerManager.instance.OnpointerUp(false);
                        //JoyStick.Instance.StopWalk();
                        //JoyStick.Instance.PcStopWalk();

                        for (int x = 0; x < SingleKeycodeInputs.Count; x++)
                        {
                            var keyInput = SingleKeycodeInputs[x];
                            foreach (var script in keyInput.AddScriptsForReset)
                            {
                                if (script != null)
                                {
                                    int methodIndex = keyInput.AddScriptsForReset.IndexOf(script);
                                    string functionName = keyInput.FunctionsToInvokeForReset[methodIndex];

                                    MethodInfo method = script.GetType().GetMethod(functionName);
                                    if (method != null && method.IsPublic && method.GetParameters().Length == 0)
                                    {
                                        method.Invoke(script, null);
                                    }
                                    else
                                    {
                                        Debug.LogError($"Function '{functionName}' in {script.GetType().Name} is not public or has parameters.");
                                    }
                                }
                            }
                        }

                        IsRunning = false;
                        IsThrowingGrende = false;
                        IsThrowingInsendiaryGrende = false;
                        IsUsingHealthPack = false;
                        IsCallingAirSupport = false;
                        IsCallingArtillerySupport = false;
                        IsKnifeAttack = false;
                        IsJumping = false;
                        ResetDualControl = false;
                    }

                }
            }

        }
        public void GameControls(string Control)
        {           
            if (Control == Options.MoveForward.ToString())
            {
                ResetOtherVariableButCall(ref IsWalking);
                //JoyStick.Instance.PcStartWalk();
                //JoyStick.Instance.ForwardWalk();
                IsAnyActionPerformed = true;
            }
            else if (Control == Options.MoveBackward.ToString())
            {
                ResetOtherVariableButCall(ref IsWalking);
                //JoyStick.Instance.PcStartWalk();
                //JoyStick.Instance.BackwardWalk();
                IsAnyActionPerformed = true;
            }
            else if (Control == Options.MoveLeft.ToString())
            {
                ResetOtherVariableButCall(ref IsWalking);
                //JoyStick.Instance.PcStartWalk();
                //JoyStick.Instance.LeftWalk();
                IsAnyActionPerformed = true;
            }
            else if (Control == Options.MoveRight.ToString())// && PlayerManager.instance.IsMoving == false)
            {
                ResetOtherVariableButCall(ref IsWalking);
                //JoyStick.Instance.PcStartWalk();
                //JoyStick.Instance.RightWalk();
                IsAnyActionPerformed = true;
            }
            else if (Control == Options.Sprint.ToString())// && JoyStick.Instance.IsWalking == false)
            {
                if (UseTwoKeyInputsForSprinting == false)
                {
                    ResetOtherVariableButCall(ref IsRunning);
                    //PlayerManager.instance.OnpointerDown(true);
                    IsAnyActionPerformed = true;
                }
            }
            else if (Control == Options.Jump.ToString())
            {
                if (FirstPersonController.instance != null)
                {
                    //if (Input.GetKeyDown(SingleKeycodeInputs[OneTimeKey].KeyCode))
                    //{
                        //if (FirstPersonController.instance.IsJumpDelayFinished == false)
                        //{
                           // Debug.Log("Jumping");
                            ResetOtherVariableButCall(ref IsJumping);
                          //  FirstPersonController.instance.PlayerJump();
                            IsAnyActionPerformed = true;
                        //}
                   // }
                }

            }
            else if (Control == Options.Crouch.ToString())
            {
                //if (Input.GetKeyDown(SingleKeycodeInputs[OneTimeKey].KeyCode))
                //{
                  //  ResetOtherVariableButCall(ref IsCrouching);
                  //  Crouch.instance.PcCrouch();
                    IsAnyActionPerformed = true;
               // }
                // PreviousKeyCode = KeyCode.None;
            }
            //else if (Control == Options.Shoot.ToString())
            //{
            //    if (PlayerManager.instance.ob.WeaponOptions.ContinuousFire == true)
            //    {
            //        PlayerManager.instance.FireContinue();
            //        IsAnyActionPerformed = true;
            //    }
            //    else
            //    {
            //        PlayerManager.instance.FireOneShot();
            //        IsAnyActionPerformed = true;
            //    }

            //}
            else if (Control == Options.Reload.ToString())
            {
                //if (Input.GetKeyDown(SingleKeycodeInputs[OneTimeKey].KeyCode))
                //{
                //    if (PlayerManager.instance.CurrentHoldingPlayerWeapon.Reload.isreloading == false)
                //    {
                     //   PlayerManager.instance.Reload();
                        IsAnyActionPerformed = true;
                //    }
                //}
            }
            //else if (Control == Options.Aiming.ToString())
            //{
            //    PlayerManager.instance.Aiming();
            //    IsAnyActionPerformed = true;
            //}
            //else if (Control == Options.SwitchWeaponNext.ToString())
            //{
            //    SwitchWeapons.instance.SwitchGunRight();
            //    IsAnyActionPerformed = true;
            //}
            //else if (Control == Options.SwitchWeaponPrevious.ToString())
            //{
            //    SwitchWeapons.instance.SwitchGunLeft();
            //    IsAnyActionPerformed = true;
            //}
            else if (Control == Options.GrenadeThrow.ToString())
            {
                //if (Input.GetKeyDown(SingleKeycodeInputs[OneTimeKey].KeyCode))
                //{
                //    if (PlayerGrenadeThrowerScript.DirectSpawn == false)
                //    {
                        ResetOtherVariableButCall(ref IsThrowingGrende);
                       // PlayerGrenadeThrowerScript.ThrowGrenade();
                        IsAnyActionPerformed = true;
                if (MouseControls.instance != null)
                {
                    MouseControls.instance.StopFunctioning = true;
                }
                //    }
                //}
            }
            else if (Control == Options.InsendiaryGrenadeThrow.ToString())
            {
                //if (Input.GetKeyDown(SingleKeycodeInputs[OneTimeKey].KeyCode))
                //{
                //    if (PlayerInsendiaryGrenadeThrowerScript.DirectSpawn == false)
                //    {
                        ResetOtherVariableButCall(ref IsThrowingInsendiaryGrende);
                     //   PlayerInsendiaryGrenadeThrowerScript.ThrowGrenade();
                        IsAnyActionPerformed = true;
                if (MouseControls.instance != null)
                {
                    MouseControls.instance.StopFunctioning = true;
                }
                //    }
                //}
            }
            else if (Control == Options.KnifeAttack.ToString())
            {
                //if (Input.GetKeyDown(SingleKeycodeInputs[OneTimeKey].KeyCode))
                //{
                //    if (MeleeAttack.instance.Directattack == false)
                //    {
                        ResetOtherVariableButCall(ref IsKnifeAttack);
                        MeleeAttack.instance.MeleeAttackFunction();
                        IsAnyActionPerformed = true;
                if (MouseControls.instance != null)
                {
                    MouseControls.instance.StopFunctioning = true;
                }
                //    }
                //}

            }
            else if (Control == Options.CallAirSupport.ToString())
            {
                if (Input.GetKeyDown(SingleKeycodeInputs[OneTimeKey].KeyCode))
                {
                     
                        ResetOtherVariableButCall(ref IsCallingArtillerySupport);
                    //AirSupportScript.ActivateAirSupport();
                        IsAnyActionPerformed = true;
                    
                }

            }
            else if (Control == Options.CallArtillerySupport.ToString())
            {
                if (Input.GetKeyDown(SingleKeycodeInputs[OneTimeKey].KeyCode))
                {
                     
                        ResetOtherVariableButCall(ref IsCallingArtillerySupport);
                  //  ArtillerySupportScript.ActivateArtillerySupport();
                    IsAnyActionPerformed = true;
                   
                }

            }
            else if (Control == Options.UseHealthPack.ToString())
            {
                if (Input.GetKeyDown(SingleKeycodeInputs[OneTimeKey].KeyCode))
                {
                    ResetOtherVariableButCall(ref IsUsingHealthPack);
                   // IncreasePlayerHealthScript.IncreaseHealth();
                   IsAnyActionPerformed = true;
                }

            }
            else if (Control == Options.Pause.ToString())
            {
                ResetOtherVariableButCall(ref IsUsingMenu);
                // PauseManager.instance.PauseGame();
               // PcInput_Settings.instance.OpenPcSettings();
                IsAnyActionPerformed = true;
            }
            else if (Control == Options.ItemPickup.ToString()  || Control == Options.WeaponAmmoPickup.ToString() || Control == Options.WeaponPickup.ToString()
                 || Control == Options.AmmoBoxPickup.ToString())
            {
         
                //if (Input.GetKeyDown(SingleKeycodeInputs[OneTimeKey].KeyCode))
                //{
                //  ResetOtherVariableButCall(ref IsCrouching);
                //  Crouch.instance.PcCrouch();
                IsAnyActionPerformed = true;
                // }
                // PreviousKeyCode = KeyCode.None;
            }
            //else if (Control == Options.SwitchSprint.ToString())
            //{
            //    if (Input.GetKeyDown(SingleKeycodeInputs[OneTimeKey].KeyCode))
            //    {
            //        if (IsSwitchingToRun == true)
            //        {
            //            ResetOtherVariableButCall(ref SwitchSprinting);
            //          // PlayerManager.instance.EnableRunForPC();
            //            IsAnyActionPerformed = true;
            //            IsSwitchingToRun = false;
            //        }
            //    }
            //}
        }
        public void ResetOtherVariableButCall(ref bool WhatToDo)
        {
            SwitchSprinting = false;
            IsWalking = false;
            IsRunning = false;
            IsCrouching = false;
            IsJumping = false;
            IsThrowingGrende = false;
            IsThrowingInsendiaryGrende = false;

            IsUsingHealthPack = false;
            IsCallingArtillerySupport = false;
            IsCallingAirSupport = false;

            IsKnifeAttack = false;
            IsUsingMenu = false;

            WhatToDo = true;
        }

    }
}
