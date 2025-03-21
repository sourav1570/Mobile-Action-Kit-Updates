using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace MobileActionKit
{
    public class PcInput_Settings : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "PcInputSettingsManager provides functionality for modifying keybindings in a PC game. "
     + "It manages the settings menu UI, allowing players to customize input keys for movement, combat, and utility functions. "
     + "This script integrates with PcInputManager, MouseControls, and MouseScrollWeaponZoom to apply and save input changes.";


        public static PcInput_Settings instance;

        [Tooltip("Reference to the settings menu GameObject, which is displayed when configuring controls.")]
        public GameObject SettingsMenu;

        [Tooltip("Button to save the updated key bindings and settings.")]
        public Button SaveButton;

        [Tooltip("Button to close the settings menu without saving changes.")]
        public Button CloseButton;

        [Tooltip("Reference to the PcInputManager script, which manages overall player input handling.")]
        public PcInputManager PcInputManagerScript;

        [Tooltip("Reference to the MouseControls script, handling mouse input settings.")]
        public MouseControls MouseControlsScript;

        [Tooltip("Reference to the MouseScrollWeaponZoom script, which manages weapon switching and sniper zoom functionality.")]
        public MouseScrollWeaponZoom MouseScrollWeaponZoomScript;

        [Header("INPUT FIELDS")]

        [Tooltip("Input field to set the key for moving forward.")]
        public TMP_InputField MoveForward;

        [Tooltip("Input field to set the key for moving backward.")]
        public TMP_InputField MoveBackward;

        [Tooltip("Input field to set the key for moving left.")]
        public TMP_InputField MoveLeft;

        [Tooltip("Input field to set the key for moving right.")]
        public TMP_InputField MoveRight;

        [Tooltip("Input field to set the key for sprinting while holding the key.")]
        public TMP_InputField HoldSprint;

        [Tooltip("Input field to set the key for performing a knife attack.")]
        public TMP_InputField KnifeAttack;

        [Tooltip("Input field to set the key for throwing a grenade.")]
        public TMP_InputField GrenadeThrow;

        [Tooltip("Input field to set the key for pausing the game.")]
        public TMP_InputField Pause;

        [Tooltip("Input field to set the key for reloading a weapon.")]
        public TMP_InputField Reload;

        [Tooltip("Input field to set the key for firing a weapon.")]
        public TMP_InputField Fire;

        [Tooltip("Input field to set the key for aiming down sights.")]
        public TMP_InputField Aim;

        [Tooltip("Input field to set the key for jumping.")]
        public TMP_InputField Jump;

        [Tooltip("Input field to set the key for crouching.")]
        public TMP_InputField Crouch;

        [Tooltip("Input field to set the key for throwing an incendiary grenade.")]
        public TMP_InputField InsendiaryGrenadeThrow;

        [Tooltip("Input field to set the key for calling artillery support.")]
        public TMP_InputField CallArtillerySupport;

        [Tooltip("Input field to set the key for calling air support.")]
        public TMP_InputField CallAirSupport;

        [Tooltip("Input field to set the key for using a health pack.")]
        public TMP_InputField UseHealthPack;

        [Tooltip("Input field to set the key for using a ammobox pack.")]
        public TMP_InputField AmmoBoxPickup;

        [Tooltip("Input field to set the key for using a weapon pickup.")]
        public TMP_InputField WeaponPickup;

        [Tooltip("Input field to set the key for using a ammo pickup.")]
        public TMP_InputField WeaponAmmoPickup;


        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            SaveButton.onClick.AddListener(SavePcSettings);
            CloseButton.onClick.AddListener(ClosePcSettings);

            SavePcSettings();
            for (int x = 0; x < PcInputManagerScript.SingleKeycodeInputs.Count; x++)
            {
                if ("MoveForward" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    MoveForward.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("MoveBackward" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    MoveBackward.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("MoveLeft" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    MoveLeft.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("MoveRight" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    MoveRight.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("CallArtillerySupport" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    CallArtillerySupport.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("CallAirSupport" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    CallAirSupport.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("UseHealthPack" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    UseHealthPack.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("KnifeAttack" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    KnifeAttack.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("GrenadeThrow" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    GrenadeThrow.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("InsendiaryGrenadeThrow" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    InsendiaryGrenadeThrow.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("Pause" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    Pause.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("Reload" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    Reload.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("Jump" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    Jump.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("Crouch" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    Crouch.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("AmmoBoxPickup" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    AmmoBoxPickup.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("WeaponPickup" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    WeaponPickup.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
                else if ("WeaponAmmoPickup" == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    WeaponAmmoPickup.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                }
            }

            if (PcInputManagerScript.UseTwoKeyInputsForSprinting == true)
            {
                if (PcInputManagerScript.SprintKeycodeSetup.Length >= 2)
                {
                    for (int x = 0; x < PcInputManagerScript.SingleKeycodeInputs.Count; x++)
                    {
                        if (PcInputManagerScript.SingleKeycodeInputs[x].Control == PcInputManager.Options.MoveForward)
                        {
                            PcInputManagerScript.SprintKeycodeSetup[0] = StringToKeyCode(PlayerPrefs.GetString(PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString(), PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString()));
                        }
                        if (PcInputManagerScript.SingleKeycodeInputs[x].Control == PcInputManager.Options.Sprint)
                        {
                            PcInputManagerScript.SprintKeycodeSetup[1] = StringToKeyCode(PlayerPrefs.GetString(PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString(), PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString()));
                            HoldSprint.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < PcInputManagerScript.SingleKeycodeInputs.Count; x++)
                {
                    if (PcInputManagerScript.SingleKeycodeInputs[x].Control == PcInputManager.Options.Sprint)
                    {
                        PcInputManagerScript.SprintKeycodeSetup[0] = StringToKeyCode(PlayerPrefs.GetString(PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString(), PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString()));
                        HoldSprint.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                    }
                }
            }



            for (int x = 0; x < MouseControlsScript.MouseClicks.Count; x++)
            {
                if ("Fire" == MouseControlsScript.MouseClicks[x].Control.ToString())
                {
                    Fire.text = MouseControlsScript.MouseClicks[x].KeyCode.ToString();
                }
                else if ("Aim" == MouseControlsScript.MouseClicks[x].Control.ToString())
                {
                    Aim.text = MouseControlsScript.MouseClicks[x].KeyCode.ToString();
                }
            }

        }
        //// This method is called when the dropdown value changes
        //void OnDropdownValueChanged(int optionIndex)
        //{
        //    // Get the text of the selected option
        //    string selectedOption = ChooseRunType.options[optionIndex].text;

        //    if (selectedOption == "HOLD SPRINT")
        //    {
        //        if (PlayerManager.instance != null)
        //        {
        //            PlayerManager.instance.RunningOptions.ChooseRunType = PlayerManager.RunOptions.HoldToRun;
        //            PlayerPrefs.SetString("RunOption", PlayerManager.RunOptions.HoldToRun.ToString());
        //        }
        //    }
        //    else if (selectedOption == "SWITCH SPRINT")
        //    {
        //        if (PlayerManager.instance != null)
        //        {
        //            PlayerManager.instance.RunningOptions.ChooseRunType = PlayerManager.RunOptions.RunOrWalkSwitch;
        //            PlayerPrefs.SetString("RunOption", PlayerManager.RunOptions.RunOrWalkSwitch.ToString());
        //        }
        //    }
        //}
        public void OpenPcSettings()
        {
            Time.timeScale = 0f;
            SettingsMenu.SetActive(true);

            PcInputManagerScript.StopFunctioning = true;
            MouseControlsScript.StopFunctioning = true;
            MouseScrollWeaponZoomScript.StopFunctioning = true;

            PcInputManagerScript.ResetSettings();
            MouseControlsScript.ResetSettings();
        }
        public void ClosePcSettings()
        {
            Time.timeScale = 1f;
            SettingsMenu.SetActive(false);
            PcInputManagerScript.StopFunctioning = false;
            MouseControlsScript.StopFunctioning = false;
            MouseScrollWeaponZoomScript.StopFunctioning = false;
            PcInputManagerScript.IsUsingMenu = false;

        }
        public void SavePcSettings()
        {
            Time.timeScale = 1f;
            SettingsMenu.SetActive(false);

            if (PlayerManager.instance != null)
            {
                if (PlayerPrefs.GetString("RunOption", PlayerManager.RunOptions.HoldToRun.ToString()) == PlayerManager.RunOptions.HoldToRun.ToString())
                {
                    PlayerManager.instance.RunningOptions.ChooseRunType = PlayerManager.RunOptions.HoldToRun;
                    //ChooseRunType.value = 0;
                }
                else
                {
                    PlayerManager.instance.RunningOptions.ChooseRunType = PlayerManager.RunOptions.RunOrWalkSwitch;
                    //ChooseRunType.value = 1;
                }
            }

            for (int x = 0; x < PcInputManagerScript.SingleKeycodeInputs.Count; x++)
            {
                PcInputManagerScript.SingleKeycodeInputs[x].KeyCode = StringToKeyCode(PlayerPrefs.GetString(PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString(), PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString()));
            }
            for (int x = 0; x < MouseControlsScript.MouseClicks.Count; x++)
            {
                MouseControlsScript.MouseClicks[x].KeyCode = StringToKeyCode(PlayerPrefs.GetString(MouseControlsScript.MouseClicks[x].Control.ToString(), MouseControlsScript.MouseClicks[x].KeyCode.ToString()));
            }

            if (PcInputManagerScript.UseTwoKeyInputsForSprinting == true)
            {
                if (PcInputManagerScript.SprintKeycodeSetup.Length >= 2)
                {
                    for (int x = 0; x < PcInputManagerScript.SingleKeycodeInputs.Count; x++)
                    {
                        if (PcInputManagerScript.SingleKeycodeInputs[x].Control == PcInputManager.Options.MoveForward)
                        {
                            PcInputManagerScript.SprintKeycodeSetup[0] = StringToKeyCode(PlayerPrefs.GetString(PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString(), PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString()));
                        }
                        if (PcInputManagerScript.SingleKeycodeInputs[x].Control == PcInputManager.Options.Sprint)
                        {
                            PcInputManagerScript.SprintKeycodeSetup[1] = StringToKeyCode(PlayerPrefs.GetString(PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString(), PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString()));
                            HoldSprint.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < PcInputManagerScript.SingleKeycodeInputs.Count; x++)
                {
                    if (PcInputManagerScript.SingleKeycodeInputs[x].Control == PcInputManager.Options.Sprint)
                    {
                        PcInputManagerScript.SprintKeycodeSetup[0] = StringToKeyCode(PlayerPrefs.GetString(PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString(), PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString()));
                        HoldSprint.text = PcInputManagerScript.SingleKeycodeInputs[x].KeyCode.ToString();
                    }
                }
            }

            PcInputManagerScript.StopFunctioning = false;
            MouseControlsScript.StopFunctioning = false;
            MouseScrollWeaponZoomScript.StopFunctioning = false;
            PcInputManagerScript.IsUsingMenu = false;

        }
        public void UpdateTextAfterSwap(string SwappedControlText, string KeycodeName)
        {
            if (SwappedControlText == "MoveForward")
            {
                MoveForward.text = KeycodeName;
            }
            else if (SwappedControlText == "MoveBackward")
            {
                MoveBackward.text = KeycodeName;
            }
            else if (SwappedControlText == "MoveLeft")
            {
                MoveLeft.text = KeycodeName;
            }
            else if (SwappedControlText == "MoveRight")
            {
                MoveRight.text = KeycodeName;
            }
            else if (SwappedControlText == "HoldSprint")
            {
                HoldSprint.text = KeycodeName;
            }
            else if (SwappedControlText == "CallArtillerySupport")
            {
                CallArtillerySupport.text = KeycodeName;
            }
            else if (SwappedControlText == "CallAirSupport")
            {
                CallAirSupport.text = KeycodeName;
            }
            else if (SwappedControlText == "UseHealthPack")
            {
                UseHealthPack.text = KeycodeName;
            }
            else if (SwappedControlText == "InsendiaryGrenadeThrow")
            {
                InsendiaryGrenadeThrow.text = KeycodeName;
            }
            //else if (SwappedControlText == "SwitchSprint")
            //{
            //    SwitchSprint.text = KeycodeName;
            //}
            else if (SwappedControlText == "KnifeAttack")
            {
                KnifeAttack.text = KeycodeName;
            }
            else if (SwappedControlText == "GrenadeThrow")
            {
                GrenadeThrow.text = KeycodeName;
            }
            else if (SwappedControlText == "Pause")
            {
                Pause.text = KeycodeName;
            }
            else if (SwappedControlText == "Reload")
            {
                Reload.text = KeycodeName;
            }
            else if (SwappedControlText == "Fire")
            {
                Fire.text = KeycodeName;
            }
            else if (SwappedControlText == "Aim")
            {
                Aim.text = KeycodeName;
            }
            else if (SwappedControlText == "Jump")
            {
                Jump.text = KeycodeName;
            }
            else if (SwappedControlText == "Crouch")
            {
                Crouch.text = KeycodeName;
            }
            else if (SwappedControlText == "AmmoBoxPickup")
            {
                AmmoBoxPickup.text = KeycodeName;
            }
            else if (SwappedControlText == "WeaponPickup")
            {
                WeaponPickup.text = KeycodeName;
            }
            else if (SwappedControlText == "WeaponAmmoPickup")
            {
                WeaponAmmoPickup.text = KeycodeName;
            }
        }
        public void SwapKeycodes(string InputFieldName, KeyCode keyCode)
        {
            for (int x = 0; x < PcInputManagerScript.SingleKeycodeInputs.Count; x++)
            {
                if (keyCode == PcInputManagerScript.SingleKeycodeInputs[x].KeyCode)
                {
                    for (int i = 0; i < PcInputManagerScript.SingleKeycodeInputs.Count; i++)
                    {
                        if (PcInputManagerScript.SingleKeycodeInputs[i].Control.ToString() == InputFieldName)
                        {
                            UpdateTextAfterSwap(PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString(), PcInputManagerScript.SingleKeycodeInputs[i].KeyCode.ToString());
                            PcInputManagerScript.SingleKeycodeInputs[x].KeyCode = PcInputManagerScript.SingleKeycodeInputs[i].KeyCode;
                            PlayerPrefs.SetString(PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString(), PcInputManagerScript.SingleKeycodeInputs[i].KeyCode.ToString());
                        }
                    }
                    for (int i = 0; i < MouseControlsScript.MouseClicks.Count; i++)
                    {
                        if (MouseControlsScript.MouseClicks[i].Control.ToString() == InputFieldName)
                        {
                            UpdateTextAfterSwap(PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString(), MouseControlsScript.MouseClicks[i].KeyCode.ToString());
                            PcInputManagerScript.SingleKeycodeInputs[x].KeyCode = MouseControlsScript.MouseClicks[i].KeyCode;
                            PlayerPrefs.SetString(PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString(), MouseControlsScript.MouseClicks[i].KeyCode.ToString());
                        }
                    }
                }
            }
            for (int x = 0; x < MouseControlsScript.MouseClicks.Count; x++)
            {
                if (keyCode == MouseControlsScript.MouseClicks[x].KeyCode)
                {
                    for (int i = 0; i < PcInputManagerScript.SingleKeycodeInputs.Count; i++)
                    {
                        if (PcInputManagerScript.SingleKeycodeInputs[i].Control.ToString() == InputFieldName)
                        {
                            UpdateTextAfterSwap(MouseControlsScript.MouseClicks[x].Control.ToString(), PcInputManagerScript.SingleKeycodeInputs[i].KeyCode.ToString());
                            MouseControlsScript.MouseClicks[x].KeyCode = PcInputManagerScript.SingleKeycodeInputs[i].KeyCode;
                            PlayerPrefs.SetString(MouseControlsScript.MouseClicks[x].Control.ToString(), PcInputManagerScript.SingleKeycodeInputs[i].KeyCode.ToString());
                        }
                    }
                    for (int i = 0; i < MouseControlsScript.MouseClicks.Count; i++)
                    {
                        if (MouseControlsScript.MouseClicks[i].Control.ToString() == InputFieldName)
                        {
                            UpdateTextAfterSwap(MouseControlsScript.MouseClicks[x].Control.ToString(), MouseControlsScript.MouseClicks[i].KeyCode.ToString());
                            MouseControlsScript.MouseClicks[x].KeyCode = MouseControlsScript.MouseClicks[i].KeyCode;
                            PlayerPrefs.SetString(MouseControlsScript.MouseClicks[x].Control.ToString(), MouseControlsScript.MouseClicks[i].KeyCode.ToString());
                        }
                    }
                }
            }
        }
        public void UpdateKeyboardControls(string InputFieldName, KeyCode keyCode)
        {
            SwapKeycodes(InputFieldName, keyCode);
            for (int x = 0; x < PcInputManagerScript.SingleKeycodeInputs.Count; x++)
            {
                if (InputFieldName == PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString())
                {
                    PcInputManagerScript.SingleKeycodeInputs[x].KeyCode = keyCode;
                    PlayerPrefs.SetString(PcInputManagerScript.SingleKeycodeInputs[x].Control.ToString(), keyCode.ToString());
                }
            }
        }
        public void UpdateMouseControls(string InputFieldName, KeyCode keyCode)
        {
            SwapKeycodes(InputFieldName, keyCode);
            for (int x = 0; x < MouseControlsScript.MouseClicks.Count; x++)
            {
                if (InputFieldName == MouseControlsScript.MouseClicks[x].Control.ToString())
                {
                    MouseControlsScript.MouseClicks[x].KeyCode = keyCode;
                    PlayerPrefs.SetString(MouseControlsScript.MouseClicks[x].Control.ToString(), keyCode.ToString());
                }
            }
        }
        KeyCode StringToKeyCode(string keyString)
        {
            try
            {
                return (KeyCode)Enum.Parse(typeof(KeyCode), keyString, true);
            }
            catch
            {
                Debug.LogError("Invalid key string: " + keyString);
                return KeyCode.None;
            }
        }
        private void Update()
        {
            if (MoveForward.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    MoveForward.text = keyName;

                    UpdateKeyboardControls("MoveForward", StringToKeyCode(keyName));
                }
            }
            else if (MoveBackward.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    MoveBackward.text = keyName;

                    UpdateKeyboardControls("MoveBackward", StringToKeyCode(keyName));
                }
            }
            else if (MoveLeft.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    MoveLeft.text = keyName;

                    UpdateKeyboardControls("MoveLeft", StringToKeyCode(keyName));
                }
            }
            else if (MoveRight.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    MoveRight.text = keyName;

                    UpdateKeyboardControls("MoveRight", StringToKeyCode(keyName));
                }
            }
            else if (HoldSprint.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    HoldSprint.text = keyName;

                    if (PcInputManagerScript.SprintKeycodeSetup.Length >= 2)
                    {
                        PcInputManagerScript.SprintKeycodeSetup[1] = StringToKeyCode(keyName);
                    }
                }
            }
            //else if (SwitchSprint.isFocused)
            //{
            //    // Check if any key is pressed
            //    if (Input.anyKeyDown)
            //    {
            //        // Get the name of the pressed key
            //        string keyName = GetPressedKeyName();

            //        // Update the input field text
            //        SwitchSprint.text = keyName;

            //        UpdateKeyboardControls("SwitchSprint", StringToKeyCode(keyName));
            //    }
            //}
            else if (KnifeAttack.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    KnifeAttack.text = keyName;

                    UpdateKeyboardControls("KnifeAttack", StringToKeyCode(keyName));
                }
            }
            else if (GrenadeThrow.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    GrenadeThrow.text = keyName;

                    UpdateKeyboardControls("GrenadeThrow", StringToKeyCode(keyName));
                }
            }
            else if (Pause.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    Pause.text = keyName;

                    UpdateKeyboardControls("Pause", StringToKeyCode(keyName));
                }
            }
            else if (Reload.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    Reload.text = keyName;

                    UpdateKeyboardControls("Reload", StringToKeyCode(keyName));
                }
            }
            else if (Fire.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    Fire.text = keyName;

                    UpdateMouseControls("Fire", StringToKeyCode(keyName));
                }
            }
            else if (Aim.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    Aim.text = keyName;

                    UpdateMouseControls("Aim", StringToKeyCode(keyName));
                }
            }
            else if (Jump.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    Jump.text = keyName;

                    UpdateKeyboardControls("Jump", StringToKeyCode(keyName));
                }
            }
            else if (Crouch.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    Crouch.text = keyName;

                    UpdateKeyboardControls("Crouch", StringToKeyCode(keyName));
                }
            }
            else if (InsendiaryGrenadeThrow.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    InsendiaryGrenadeThrow.text = keyName;

                    UpdateKeyboardControls("InsendiaryGrenadeThrow", StringToKeyCode(keyName));
                }
            }
            else if (CallArtillerySupport.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    CallArtillerySupport.text = keyName;

                    UpdateKeyboardControls("CallArtillerySupport", StringToKeyCode(keyName));
                }
            }
            else if (CallAirSupport.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    CallAirSupport.text = keyName;

                    UpdateKeyboardControls("CallAirSupport", StringToKeyCode(keyName));
                }
            }
            else if (UseHealthPack.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    UseHealthPack.text = keyName;

                    UpdateKeyboardControls("UseHealthPack", StringToKeyCode(keyName));
                }
            }
            else if (AmmoBoxPickup.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    AmmoBoxPickup.text = keyName;

                    UpdateKeyboardControls("AmmoBoxPickup", StringToKeyCode(keyName));
                }
            }
            else if (WeaponPickup.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    WeaponPickup.text = keyName;

                    UpdateKeyboardControls("WeaponPickup", StringToKeyCode(keyName));
                }
            }
            else if (WeaponAmmoPickup.isFocused)
            {
                // Check if any key is pressed
                if (Input.anyKeyDown)
                {
                    // Get the name of the pressed key
                    string keyName = GetPressedKeyName();

                    // Update the input field text
                    WeaponAmmoPickup.text = keyName;

                    UpdateKeyboardControls("WeaponAmmoPickup", StringToKeyCode(keyName));
                }
            }
        }

        private string GetPressedKeyName()
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    return keyCode.ToString();
                }
            }

            return string.Empty;
        }
    }
}
