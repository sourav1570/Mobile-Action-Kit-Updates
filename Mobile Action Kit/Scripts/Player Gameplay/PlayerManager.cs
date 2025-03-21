using UnityEngine.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// This Script is Responsible for Managing Player Shooting,Run,Walk and few more other Behaviour
namespace MobileActionKit
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager instance;

        [TextArea]
        public string ScriptInfo = "This Script manages various player HUD Ui elements for interactions with game environment and Weapons.";
        [Space(10)]

        [Tooltip("'SwitchingPlayerWeapons'  gameobject with 'SwitchingPlayerWeapons' script component attached to it is to be placed into this field.")]
        public SwitchingPlayerWeapons SwitchingPlayerWeaponsComponent;

        [Tooltip("'FireButton' gameObject with 'Button' component attached to it to be place into this field.")]
        public Button FireButton;
        [Tooltip("'ReloadButton' gameObject with 'Button' component attached to it to be place into this field.")]
        public Button ReloadButton;
        [Tooltip("'AimingButton' gameObject with 'Button' component attached to it to be place into this field.")]
        public Button AimingButton;
      
        //public bool MoveTouchpadOnHoldingFire = false;

        [HideInInspector]
        public bool isRunEnable = false;

        [HideInInspector]
        public bool IsMoving = false;
        [HideInInspector]
        public bool IsScoping = false;



        [HideInInspector]
        public PlayerWeapon CurrentHoldingPlayerWeapon;
        //bool StopEvents = false;

        public enum RunOptions
        {
            RunOrWalkSwitch,
            HoldToRun
        }

        [System.Serializable]
        public class RunProperties
        {
            [Tooltip("Choose one of the two available running activation options from dropdown list. " +
                "First option 'SwitchButtonandRun'(RunOrWalkSwitch) will alter movement joystick " +
                "to always walk or always run depending on the state of the 'Run' button as it will effectively become a movement " +
                "speed switch for main movement joystick. Second option is 'HoldButtonAndRun'(HoldToRun) will not affect movement joystick " +
                "but will make run button to act as a dedicated running button that will need to be kept pressed in order for the player to run.")]
            public RunOptions ChooseRunType;

            [Tooltip("Walking sprite from the project to be placed into this field.")]
            public Sprite WalkingSprite;
            [Tooltip("Running sprite from the project to be placed into this field.")]
            public Sprite SprintingSprite;
        }

        public RunProperties RunningOptions;

        [Tooltip("All ShootingPoint gameobjects of all the weapons from player`s hierarchy are to be placed into this list.")]
        public List<PlayerWeapon> PlayerWeaponScripts = new List<PlayerWeapon>();

        [HideInInspector]
        public bool CanStartProducingRunningSounds = false;
        [HideInInspector]
        public bool CanStartProducingWalkingSounds = false;

        //public Transform ObjectToShake;
        //[Header("Head Bobbing Values on Stand Sprint")]
        //[Range(0,100)]
        //public float HeadBobbingStandSpeed = 0.3f;
        //[Range(0, 100)]
        //public float HeadBobbingStandDistance = 0.1f;


        //[Header("Head Bobbing Values on Crouch Sprint")]
        //[Range(0, 100)]
        //public float HeadBobbingCrouchSpeed = 0.1f;
        //[Range(0, 100)]
        //public float HeadBobbingCrouchDistance = 0.05f;

        //[Range(0, 100)]
        //public float ShakeAmount;

        //float horizontal, vertical, timer, waveSlice;
        //public float ObjectToShakeDefaultXPosition = 0f;
        //public float ObjectToShakeDefaultYPosition = 0.64f;
        //public float ObjectToShakeDefaultZRotation = 0f;
        ////public float MinimumX = 0f;
        ////public float MinimumY = 0.6f;

        //[HideInInspector]
        //public float MinimumValueToIncrease = 0f;

        //public float MaximumValueToIncrease = 0.5f;

        //public Transform FpsCamera;
        //public GameObject WeaponCamera;

        //[System.Serializable]
        //public class HeadBobResetValues
        //{
        //    [System.Serializable]
        //    public class FpsCamResetDuration
        //    {
        //        public float StandSprintResetDuration;

        //        public float CrouchSprintResetDuration;

        //        public float StandWalkResetDuration;

        //        public float CrouchWalkResetDuration;

        //        public float StandJumpResetDuration;

        //        public float CrouchJumpResetDuration;

        //        public float StandAimedResetDuration;

        //        public float CrouchAimedResetDuration;
        //    }

        //    [System.Serializable]
        //    public class WeaponCamResetDuration
        //    {

        //        public float StandSprintResetDuration;


        //        public float CrouchSprintResetDuration;


        //        public float StandWalkResetDuration;


        //        public float CrouchWalkResetDuration;


        //        public float StandJumpResetDuration;


        //        public float CrouchJumpResetDuration;


        //        public float StandAimedResetDuration;

        //        public float CrouchAimedResetDuration;
        //    }
        //}

        //public HeadBobResetValues.FpsCamResetDuration FpsCameraResetDurations;
        //public HeadBobResetValues.WeaponCamResetDuration WeaponCameraResetDurations;

        //[System.Serializable]
        //public class HeadBobValues
        //{      
        //    public Vector3 FpsCamMinimumShift;
        //    public Vector3 FpsCamMaximumShift;

        //    public float FpsCamMinimumZRotation;
        //    public float FpsCamMaximumZRotation;

        //    public float FpsCamXShiftDuration;

        //    public float FpsCamYShiftDuration;

        //    public float FpsCamZShiftDuration;

        //    public float FpsCamZRotationDuration;

        //    public float FpsCamXShiftDelay;
        //    public float FpsCamYShiftDelay;
        //    public float FpsCamZShiftDelay;
        //    public float FpsCamZRotDelay;

        //    public Vector3 WeaponCamMinShift;
        //    public Vector3 WeaponCamMaxShift;

        //    public Vector3 WeaponCamMinRotation;
        //    public Vector3 WeaponCamMaxRotation;


        //    public float WeaponCamXShiftDuration;

        //    public float WeaponCamYShiftDuration;

        //    public float WeaponCamZShiftDuration;

        //    public float WeaponCamRotationDuration;

        //    public float WeaponCamXShiftDelay;
        //    public float WeaponCamYShiftDelay;
        //    public float WeaponCamZShiftDelay;
        //    public float WeaponCamRotationDelay;
        //    //[Range(0f, 100f)]
        //    //public float WeaponCamYRotationDuration;
        //    //[Range(0f, 100f)]
        //    //public float WeaponCamZRotationDuration;

        //}
        //// [System.Serializable]
        ////public class CrouchSprintValues
        ////{
        ////    public Vector3 MinimumShift;
        ////    public Vector3 MaximumShift;

        ////    public float MinimumZRotation;
        ////    public float MaximumZRotation;

        ////    //public float TimeToGetNewXValue;
        ////    //public float TimeToGetNewYValue;
        ////    //public float TimeToGetNewZValue;   

        ////    //public float TimeToGetNewZRot;
        ////    [Range(0f, 100f)]
        ////    public float XShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float YShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float ZShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float ZRotationDuration;

        ////    public float XShiftDelay;
        ////    public float YShiftDelay;
        ////    public float ZShiftDelay;
        ////    public float ZRotDelay;

        ////    public Vector3 WeaponCamMinShift;
        ////    public Vector3 WeaponCamMaxShift;

        ////    public Vector3 WeaponCamMinRotation;
        ////    public Vector3 WeaponCamMaxRotation;

        ////    [Range(0f, 100f)]
        ////    public float WeaponCamXShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamYShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamZShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamXRotationDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamYRotationDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamZRotationDuration;
        ////}
        ////[System.Serializable]
        ////public class StandWalkValues
        ////{
        ////    public Vector3 MinimumShift;
        ////    public Vector3 MaximumShift;

        ////    public float MinimumZRotation;
        ////    public float MaximumZRotation;

        ////    //public float TimeToGetNewXValue;
        ////    //public float TimeToGetNewYValue;
        ////    //public float TimeToGetNewZValue;

        ////    //public float TimeToGetNewZRot;
        ////    [Range(0f, 100f)]
        ////    public float XShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float YShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float ZShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float ZRotationDuration;

        ////    public float XShiftDelay;
        ////    public float YShiftDelay;
        ////    public float ZShiftDelay;
        ////    public float ZRotDelay;

        ////    public Vector3 WeaponCamMinShift;
        ////    public Vector3 WeaponCamMaxShift;

        ////    public Vector3 WeaponCamMinRotation;
        ////    public Vector3 WeaponCamMaxRotation;

        ////    [Range(0f, 100f)]
        ////    public float WeaponCamXShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamYShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamZShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamXRotationDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamYRotationDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamZRotationDuration;
        ////}
        ////[System.Serializable]
        ////public class CrouchWalkValues
        ////{
        ////    public Vector3 MinimumShift;
        ////    public Vector3 MaximumShift;

        ////    public float MinimumZRotation;
        ////    public float MaximumZRotation;

        ////    //public float TimeToGetNewXValue;
        ////    //public float TimeToGetNewYValue;
        ////    //public float TimeToGetNewZValue;

        ////    //public float TimeToGetNewZRot;
        ////    [Range(0f, 100f)]
        ////    public float XShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float YShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float ZShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float ZRotationDuration;

        ////    public float XShiftDelay;
        ////    public float YShiftDelay;
        ////    public float ZShiftDelay;
        ////    public float ZRotDelay;

        ////    public Vector3 WeaponCamMinShift;
        ////    public Vector3 WeaponCamMaxShift;

        ////    public Vector3 WeaponCamMinRotation;
        ////    public Vector3 WeaponCamMaxRotation;

        ////    [Range(0f, 100f)]
        ////    public float WeaponCamXShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamYShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamZShiftDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamXRotationDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamYRotationDuration;
        ////    [Range(0f, 100f)]
        ////    public float WeaponCamZRotationDuration;
        ////}


        //public HeadBobValues HeadBobStandSprintValues;

        //public HeadBobValues HeadBobCrouchSprintValues;

        //public HeadBobValues HeadBobStandWalkValues;

        //public HeadBobValues HeadBobCrouchWalkValues;

        //public HeadBobValues HeadBobSniperStandWalkValues;

        //public HeadBobValues HeadBobSniperCrouchWalkValues;

        //public HeadBobValues HeadBobOpticalStandWalkValues;

        //public HeadBobValues HeadBobOpticalCrouchWalkValues;

        //public HeadBobValues HeadBobStandJumpValues;

        //public HeadBobValues HeadBobCrouchJumpValues;



        //  public Button[] ButtonsToDisableInteractionOnReload;

        [HideInInspector]
        public bool IsShooting = false;

        [Tooltip("All first person weapon skeletal meshes from the player`s hierarchy with 'ProceduralBobbing' " +
            "script attached to them are to be added to this list for the cases of switching player`s weapons. " +
            "So that Player manager would reset bobbing script values in any of the currently selected weapon before deactivating it " +
            "and activating next selected weapon.")]
        public List<GameObject> BobbingScripts = new List<GameObject>();

        [HideInInspector]
        public ProceduralBobbing[] AllBobbingScripts;

        //[HideInInspector]
        //public bool StopLoop = false;
        //[HideInInspector]
        //public bool StartLoop = false;
        //bool AutoReset = false;

        //[HideInInspector]
        //public bool WalkStopLoop = false;
        //[HideInInspector]
        //public bool WalkStartLoop = false;
        //[HideInInspector]
        //public bool WalkAutoReset = false;

        //[HideInInspector]
        //public bool JumpStopLoop = false;
        //[HideInInspector]
        //public bool JumpStartLoop = false;
        //[HideInInspector]
        //public bool JumpAutoReset = false;

        //float PosXTimer = 0f;
        //float PosYTimer = 0f;
        //float PosZTimer = 0f;

        //float RotXTimer = 0f;
        //float RotYTimer = 0f;
        //float RotZTimer = 0f;

        [HideInInspector]
        public bool LoopHeadBob = false;
        int BobbingNum;
        int value = 0;

        Vector3 DefaultWeaponCamPos;
        Vector3 DefaultWeaponCamRot;
        float PlayerCamDefaultFov;
        float WeaponCamDefaultFov;

        [HideInInspector]
        public bool IsSwitchSprinting = false;

        public void ResettingDescription()
        {
            ScriptInfo = "This Script Initialise Player bobbing and Provides Running Variation for Player";
        }
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            for (int x = 0; x < BobbingScripts.Count; x++)
            {
                int BobbingScr = BobbingScripts[x].GetComponents<ProceduralBobbing>().Length;
                BobbingNum = BobbingNum + BobbingScr;

            }
            AllBobbingScripts = new ProceduralBobbing[BobbingNum];

            for (int x = 0; x < BobbingScripts.Count; x++)
            {
                ProceduralBobbing[] Bob = BobbingScripts[x].GetComponents<ProceduralBobbing>();
                for (int i = 0; i < Bob.Length; i++)
                {
                    AllBobbingScripts[value] = Bob[i];
                    value += 1;

                }
            }
        }
        private void Start()
        {

            FindRequiredObjects();
            if (CurrentHoldingPlayerWeapon != null)
            {
                if(CurrentHoldingPlayerWeapon.RequiredComponents.WeaponCamera != null)
                {
                    DefaultWeaponCamPos = CurrentHoldingPlayerWeapon.RequiredComponents.WeaponCamera.transform.localPosition;
                    DefaultWeaponCamRot = CurrentHoldingPlayerWeapon.RequiredComponents.WeaponCamera.transform.localEulerAngles;
                    WeaponCamDefaultFov = CurrentHoldingPlayerWeapon.RequiredComponents.WeaponCamera.fieldOfView;
                }
     
                if (FirstPersonController.instance != null)
                {
                    PlayerCamDefaultFov = FirstPersonController.instance.PlayerCamera.fieldOfView;
                }

            }


            if(ReloadButton != null)
            {
                ReloadButton.onClick.AddListener(Reload);
            }

            if (AimingButton != null)
            {
                AimingButton.onClick.AddListener(Aiming);
            }

            if(FireButton != null)
            {
                EventTrigger eventTrigger = FireButton.GetComponent<EventTrigger>();

                if (eventTrigger == null)
                {
                    eventTrigger = FireButton.gameObject.AddComponent<EventTrigger>();
                }

                // Add PointerDown event
                AddEventTrigger(eventTrigger, EventTriggerType.PointerDown, () => FireContinue());

                // Add PointerUp event
                AddEventTrigger(eventTrigger, EventTriggerType.PointerUp, () => StopFiring());

                // Add FireOneShot to Button onClick
                FireButton.onClick.AddListener(() => FireOneShot());
            }

         

            //BobDefaultPos = FpsCamera.transform.localPosition;
            //BobDefaultRot = FpsCamera.transform.localEulerAngles;

            //WeaponCamDefaultPos = WeaponCamera.transform.localPosition;
            //WeaponCamDefaultRot = WeaponCamera.transform.localEulerAngles;

        }
        private void AddEventTrigger(EventTrigger eventTrigger, EventTriggerType eventType, UnityEngine.Events.UnityAction action)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
            entry.callback.AddListener((_) => action());
            eventTrigger.triggers.Add(entry);
        }
        public void DefaultFov()
        {
            if (CurrentHoldingPlayerWeapon != null)
            {
                CurrentHoldingPlayerWeapon.RequiredComponents.WeaponCamera.transform.localPosition = DefaultWeaponCamPos;
                CurrentHoldingPlayerWeapon.RequiredComponents.WeaponCamera.transform.localEulerAngles = DefaultWeaponCamRot;
                CurrentHoldingPlayerWeapon.RequiredComponents.WeaponCamera.fieldOfView = WeaponCamDefaultFov;
                if (FirstPersonController.instance != null)
                {
                    FirstPersonController.instance.PlayerCamera.fieldOfView = PlayerCamDefaultFov;
                }
            }
        }
        public void FireContinue() // Fire Continue Using Button
        {
            if (CurrentHoldingPlayerWeapon != null)
            {
                if (CurrentHoldingPlayerWeapon.Reload.isreloading == false && CurrentHoldingPlayerWeapon.Reload.CurrentAmmos > 0 && CurrentHoldingPlayerWeapon.NotAShootingWeapon == false)
                {
                    if (CurrentHoldingPlayerWeapon.ShootingFeatures.ForceFullAutoFire == true && CurrentHoldingPlayerWeapon.IsShootingNotAllowed == false && CurrentHoldingPlayerWeapon.StartShooting == true) // Required when adding friendly fire script and choosing the option 'DisableShootingOnFriendlies'
                    {
                        IsShooting = true;
                        //ob.Shoot(); // Commented on 20/3/20
                        CurrentHoldingPlayerWeapon.shootnow = true;
                        CurrentHoldingPlayerWeapon.playershoot = true;
                    }

                }
            }

        }
        public void FireOneShot()// Fire One Shot Using Button
        {
            if (CurrentHoldingPlayerWeapon != null)
            {
                if (CurrentHoldingPlayerWeapon.Reload.isreloading == false && CurrentHoldingPlayerWeapon.Reload.CurrentAmmos > 0 && CurrentHoldingPlayerWeapon.NotAShootingWeapon == false)
                {
                    if (CurrentHoldingPlayerWeapon.ShootingFeatures.ForceFullAutoFire == false && CurrentHoldingPlayerWeapon.IsShootingNotAllowed == false && CurrentHoldingPlayerWeapon.StartShooting == true) // Required when adding friendly fire script and choosing the option 'DisableShootingOnFriendlies'
                    {
                        //   Debug.Break();
                        CurrentHoldingPlayerWeapon.playershoot = true;
                        CurrentHoldingPlayerWeapon.ShotMade = true;
                       
                        CurrentHoldingPlayerWeapon.Shoot();
                    }
                }
            }
        }
        public void PlayerWeaponShootingForConsoles()
        {
            if (CurrentHoldingPlayerWeapon != null)
            {
                if (CurrentHoldingPlayerWeapon.Reload.isreloading == false && CurrentHoldingPlayerWeapon.Reload.CurrentAmmos > 0 && CurrentHoldingPlayerWeapon.NotAShootingWeapon == false) 
                {
                    if (CurrentHoldingPlayerWeapon.ShootingFeatures.ForceFullAutoFire == true && CurrentHoldingPlayerWeapon.IsShootingNotAllowed == false && CurrentHoldingPlayerWeapon.StartShooting == true) // Required when adding friendly fire script and choosing the option 'DisableShootingOnFriendlies'
                    {
                        IsShooting = true;
                        //ob.Shoot(); // Commented on 20/3/20
                        CurrentHoldingPlayerWeapon.shootnow = true;
                        CurrentHoldingPlayerWeapon.playershoot = true;
                    }
                    else if (CurrentHoldingPlayerWeapon.ShootingFeatures.ForceFullAutoFire == false && CurrentHoldingPlayerWeapon.IsShootingNotAllowed == false && CurrentHoldingPlayerWeapon.StartShooting == true) // Required when adding friendly fire script and choosing the option 'DisableShootingOnFriendlies'
                    {
                        //   Debug.Break();
                        CurrentHoldingPlayerWeapon.playershoot = true;
                        CurrentHoldingPlayerWeapon.ShotMade = true;

                        CurrentHoldingPlayerWeapon.Shoot();
                    }

                }
            }
        }
        public void StopFiring()
        {
            if (CurrentHoldingPlayerWeapon != null)
            {
                IsShooting = false;
                CurrentHoldingPlayerWeapon.shootnow = false;
            }

        }
        public void Reload()// Reload Weapon Using Button
        {
            if (CurrentHoldingPlayerWeapon != null)
            {
               
                    if (CurrentHoldingPlayerWeapon.Reload.isreloading == false && CurrentHoldingPlayerWeapon.NotAShootingWeapon == false)
                    {
                        CurrentHoldingPlayerWeapon.CustomReloading();
                    }
                
            }
        }
        public void EnableRun(Button b)// joystick Run Functionality
        {
            if (RunningOptions.ChooseRunType == RunOptions.RunOrWalkSwitch)
            {
                if (isRunEnable == false)
                {
                    isRunEnable = true;
                    b.GetComponent<Image>().sprite = RunningOptions.WalkingSprite;
                }
                else
                {
                    isRunEnable = false;
                    b.GetComponent<Image>().sprite = RunningOptions.SprintingSprite;
                }
            }
        }
        public void EnableRunForPC()// joystick Run Functionality
        {
            if (RunningOptions.ChooseRunType == RunOptions.RunOrWalkSwitch)
            {
                if (isRunEnable == false)
                {
                    isRunEnable = true;
                }
                else
                {
                    isRunEnable = false;
                }
            }
        }
        public void StartSprintBobbing()
        {
            if (LoopHeadBob == false)
            {
                if(AllBobbingScripts.Length > 0)
                {
                    if (AllBobbingScripts[0] != null)
                    {
                        for (int x = 0; x < AllBobbingScripts.Length; x++)
                        {
                            if (AllBobbingScripts[x].StartLoop == false)
                            {
                                AllBobbingScripts[x].StopLoop = false;
                                AllBobbingScripts[x].AutoReset = false;
                                AllBobbingScripts[x].StartLoop = true;
                            }
                        }
                    }
                }
             
                LoopHeadBob = true;
            }
        }
        public void OnpointerDown(bool Moving)//  Holding Run Button Functionality
        {
            StartSprintBobbing();

            if (RunningOptions.ChooseRunType == RunOptions.HoldToRun)
            {
                if (CurrentHoldingPlayerWeapon != null)
                {
                    if (CurrentHoldingPlayerWeapon.IsAimed == true)
                    {
                        Aiming();
                    }
                }
                IsMoving = Moving;
            }
        }
        public void Pc_Controls_StartSprinting()//  Holding Run Button Functionality
        {
            StartSprintBobbing();

            if (RunningOptions.ChooseRunType == RunOptions.HoldToRun)
            {
                if (CurrentHoldingPlayerWeapon != null)
                {
                    if (CurrentHoldingPlayerWeapon.IsAimed == true)
                    {
                        Aiming();
                    }
                }
                IsMoving = true;
            }
        }
        public void StopBobbing()
        {
            if (LoopHeadBob == true)
            {
                if (AllBobbingScripts.Length > 0)
                {
                    if (AllBobbingScripts[0] != null)
                    {
                        for (int x = 0; x < AllBobbingScripts.Length; x++)
                        {
                            AllBobbingScripts[x].StartLoop = false;
                        }

                    }
                }
                LoopHeadBob = false;
            }
        }
        public void OnpointerUp(bool Moving)
        {
            StopBobbing();
            if (RunningOptions.ChooseRunType == RunOptions.HoldToRun)
            {
                if (CurrentHoldingPlayerWeapon != null)
                {
                    if (RunningOptions.ChooseRunType == RunOptions.HoldToRun)
                    {
                        if (CurrentHoldingPlayerWeapon != null)
                        {
                            if (CurrentHoldingPlayerWeapon.IsAimed == true)
                            {
                                CurrentHoldingPlayerWeapon.IsplayingAimedAnim = false;
                                CurrentHoldingPlayerWeapon.AimedBreathAnimController();
                            }
                            else
                            {
                                CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.IdleAnimationParametreName, true);
                                CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.WalkAnimationParametreName, false);
                                CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.RunAnimationParametreName, false);
                            }
                        }
                    }
                   
                }
                IsMoving = Moving;
                StopRunningStepSounds();
            }
        }
        public void Pc_Controls_StopSprinting()
        {
            StopBobbing();
            if (RunningOptions.ChooseRunType == RunOptions.HoldToRun)
            {
                if (CurrentHoldingPlayerWeapon != null)
                {
                    if (RunningOptions.ChooseRunType == RunOptions.HoldToRun)
                    {
                        if (CurrentHoldingPlayerWeapon != null)
                        {
                            if (CurrentHoldingPlayerWeapon.IsAimed == true)
                            {
                                CurrentHoldingPlayerWeapon.IsplayingAimedAnim = false;
                                CurrentHoldingPlayerWeapon.AimedBreathAnimController();
                            }
                            else
                            {
                                CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.IdleAnimationParametreName, true);
                                CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.WalkAnimationParametreName, false);
                                CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.RunAnimationParametreName, false);
                            }
                        }
                    }

                }
                IsMoving = false;
                StopRunningStepSounds();
            }
        }
        public void StopRunningStepSounds()
        {
            if (FirstPersonController.instance != null)
            {
                for (int aud = 0; aud < FirstPersonController.instance.WalkAndRunSounds.RunningSounds.Length; aud++)
                {
                    FirstPersonController.instance.WalkAndRunSounds.RunningSounds[aud].Stop();
                }

            }
        }
        public void Aiming() // Weapon Scope Functionality
        {
            if (CurrentHoldingPlayerWeapon != null)
            {
                if(CurrentHoldingPlayerWeapon.NotAShootingWeapon == false)
                {
                    if (IsScoping == false)
                    {
                        if (CurrentHoldingPlayerWeapon.ShootingFeatures.SniperScopeScript != null && CurrentHoldingPlayerWeapon.ShootingFeatures.UseSniperScopeUI == true)
                        {
                            CurrentHoldingPlayerWeapon.InscopeEffect();
                            // MainCamera.fieldOfView = ob.AimedFov;
                            if (CurrentHoldingPlayerWeapon.ShootingFeatures.UseCrosshair == true)
                            {
                                CurrentHoldingPlayerWeapon.ShootingFeatures.WeaponCrosshair.SetActive(false);
                            }

                            CurrentHoldingPlayerWeapon.ShootingFeatures.SniperScopeScript.Sniperscope();
                            IsScoping = true;
                        }
                        else
                        {
                            CurrentHoldingPlayerWeapon.InscopeEffect();
                            // MainCamera.fieldOfView = ob.AimedFov;
                            if (CurrentHoldingPlayerWeapon.ShootingFeatures.UseCrosshair == true)
                            {
                                CurrentHoldingPlayerWeapon.ShootingFeatures.WeaponCrosshair.SetActive(false);
                            }
                            IsScoping = true;
                        }

                        for (int x = 0; x < AllBobbingScripts.Length; x++)
                        {
                            if (AllBobbingScripts[x].gameObject.activeInHierarchy == true)
                            {
                                AllBobbingScripts[x].DefaultLoopingValues(AllBobbingScripts[x].BobResetDurations.StationaryStandHipFireResetDuration,
                                   AllBobbingScripts[x].BobResetDurations.StationaryCrouchHipFireResetDuration,
                                  AllBobbingScripts[x].BobResetDurations.StationaryStandAimedResetDuration,
                                  AllBobbingScripts[x].BobResetDurations.StationaryCrouchAimedResetDuration, "Aiming");
                            }

                        }
                    }
                    else
                    {


                        if (CurrentHoldingPlayerWeapon.ShootingFeatures.SniperScopeScript != null && CurrentHoldingPlayerWeapon.ShootingFeatures.UseSniperScopeUI == true)
                        {
                            CurrentHoldingPlayerWeapon.OutScope();
                            if (CurrentHoldingPlayerWeapon.ShootingFeatures.UseCrosshair == true)
                            {
                                CurrentHoldingPlayerWeapon.ShootingFeatures.WeaponCrosshair.SetActive(true);
                            }
                            CurrentHoldingPlayerWeapon.ShootingFeatures.SniperScopeScript.Sniperscope();
                            IsScoping = false;
                        }
                        else
                        {
                            CurrentHoldingPlayerWeapon.OutScope();
                            IsScoping = false;
                            // MainCamera.fieldOfView = FOV;
                            if (CurrentHoldingPlayerWeapon.ShootingFeatures.UseCrosshair == true)
                            {
                                CurrentHoldingPlayerWeapon.ShootingFeatures.WeaponCrosshair.SetActive(true);
                            }
                        }

                        CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.IdleAnimationParametreName, true);
                        CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.WalkAnimationParametreName, false);
                        CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.RunAnimationParametreName, false);

                        CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.AimedAnimationParametreName, false);
                        CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.Play(CurrentHoldingPlayerWeapon.IdleAnimationName, -1, 0f);
                        for (int x = 0; x < AllBobbingScripts.Length; x++)
                        {
                            if (AllBobbingScripts[x].gameObject.activeInHierarchy == true)
                            {
                                AllBobbingScripts[x].DefaultLoopingValues(AllBobbingScripts[x].BobResetDurations.StationaryStandHipFireResetDuration,
                                  AllBobbingScripts[x].BobResetDurations.StationaryCrouchHipFireResetDuration,
                                 AllBobbingScripts[x].BobResetDurations.StationaryStandAimedResetDuration,
                                 AllBobbingScripts[x].BobResetDurations.StationaryCrouchAimedResetDuration, "Aiming");
                            }

                        }
                    }
                }
                   
               
            }
            
        }
        private void LateUpdate()
        {
            RunBobbing();

            if (IsMoving == true)
            {
                if (CurrentHoldingPlayerWeapon != null)
                {
                    // Why I added here: Because in case the player is running by holding Ak47 weapon and if the player switch weapon to SVK than we make sure that SVK Animator component make proper transition to
                    // the running animation and it look ok.. As If we put this in OnPointerDown code above than it won't update the transition for SVK so we make sure to put it in late update for the SVK weapon here so
                    // SVK can make transition properly..
                    if (RunningOptions.ChooseRunType == RunOptions.HoldToRun)
                    {
                        CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.IdleAnimationParametreName, false);
                        CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.WalkAnimationParametreName, false);
                        CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.RunAnimationParametreName, true);
                    }
                }
            }

            if (RunningOptions.ChooseRunType == RunOptions.RunOrWalkSwitch)
            {
                if (CanStartProducingRunningSounds == true)
                {
                    if (Crouch.instance != null)
                    {
                        if (Crouch.instance.IsCrouching == false)
                        {
                            FirstPersonController.instance.StandRunning();
                        }
                        else
                        {
                            FirstPersonController.instance.CrouchRunning();
                        }
                    }
                    else
                    {
                        FirstPersonController.instance.StandRunning();
                    }
                }

                if (CanStartProducingWalkingSounds == true)
                {
                    if (JoyStick.Instance != null)
                    {
                        JoyStick.Instance.CheckingForWalkingSound();
                    }
                }
            }
            else
            {
                if (CanStartProducingWalkingSounds == true)
                {
                    if (JoyStick.Instance != null)
                    {
                        JoyStick.Instance.CheckingForWalkingSound();
                    }
                }
            }
        }
        public void RunBobbing()
        {
            if (IsMoving && Crouch.instance.IsCrouching == false)
            {
                if (FirstPersonController.instance != null)
                {
                    FirstPersonController.instance.StandRunning();
                }

                if (CurrentHoldingPlayerWeapon != null)
                {
                    CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetFloat(CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.RunSpeedParameterName, CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.StandRunAnimationSpeed);
                    CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetFloat(CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.RunSpeedParameterName, CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.StandRunAnimationSpeed);
                }

                // Vector3 origpos = MainCamera.transform.localPosition;
                //float x = Random.Range(-1, 1) * ShakeSpeedOnSprinting;
                //float y = Random.Range(-1, 1) * ShakeSpeedOnSprinting;
                //float z = Random.Range(-1, 1) * ShakeSpeedOnSprinting;

                //ObjectToShake.transform.localPosition = new Vector3(x, y,z);

                // Head Bobbing Code

                //waveSlice = 0.0f;
                //horizontal = ObjectToShakeDefaultXPosition;
                //vertical = ObjectToShakeDefaultYPosition;
                //if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
                //{
                //    timer = 0.0f;
                //}
                //else
                //{
                //    waveSlice = Mathf.Sin(timer);
                //    timer = timer + HeadBobbingStandSpeed;
                //    if (timer > Mathf.PI * 2)
                //    {
                //        timer = timer - (Mathf.PI * 2);
                //    }
                //}
                //if (waveSlice != 0)
                //{
                //    float translateChange = waveSlice * HeadBobbingStandDistance;
                //    float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
                //    totalAxes = Mathf.Clamp(totalAxes, MinimumValueToIncrease, MaximumValueToIncrease);
                //    translateChange = totalAxes * translateChange;

                //    Vector3 temp = ObjectToShake.localPosition;
                //    temp.x = ObjectToShakeDefaultXPosition + translateChange;
                //    temp.y = ObjectToShakeDefaultYPosition + translateChange;
                //    ObjectToShake.localPosition = temp;


                //    Vector3 tempo = ObjectToShake.localEulerAngles;
                //    tempo.z = Random.Range(-ShakeAmount * Time.deltaTime, ShakeAmount * Time.deltaTime);
                //    ObjectToShake.localEulerAngles = tempo;
                //}
                //else
                //{
                //    Vector3 temp = ObjectToShake.localPosition;
                //    temp.x = ObjectToShakeDefaultXPosition;
                //    temp.y = ObjectToShakeDefaultYPosition;
                //    ObjectToShake.localPosition = temp;

                //    Vector3 tempo = ObjectToShake.localEulerAngles;
                //    tempo.z = ObjectToShakeDefaultZRotation;
                //    ObjectToShake.localEulerAngles = tempo;
                //}

                //if (StopLoop == false)
                //{
                //    XShift(FpsCamera.gameObject, HeadBobStandSprintValues.FpsCamMinimumShift.x, HeadBobStandSprintValues.FpsCamMaximumShift.x, HeadBobStandSprintValues.FpsCamXShiftDuration, HeadBobStandSprintValues.FpsCamXShiftDelay);
                //    YShift(FpsCamera.gameObject, HeadBobStandSprintValues.FpsCamMinimumShift.y, HeadBobStandSprintValues.FpsCamMaximumShift.y, HeadBobStandSprintValues.FpsCamYShiftDuration, HeadBobStandSprintValues.FpsCamYShiftDelay);
                //    ZShift(FpsCamera.gameObject, HeadBobStandSprintValues.FpsCamMinimumShift.z, HeadBobStandSprintValues.FpsCamMaximumShift.z, HeadBobStandSprintValues.FpsCamZShiftDuration, HeadBobStandSprintValues.FpsCamZShiftDelay);
                //    ZRot(FpsCamera.gameObject, HeadBobStandSprintValues.FpsCamMinimumZRotation, HeadBobStandSprintValues.FpsCamMaximumZRotation, HeadBobStandSprintValues.FpsCamZRotationDuration, HeadBobStandSprintValues.FpsCamZRotDelay);

                //    XShiftWeaponCam(WeaponCamera.gameObject, HeadBobStandSprintValues.WeaponCamMinShift.x, HeadBobStandSprintValues.WeaponCamMaxShift.x, HeadBobStandSprintValues.WeaponCamXShiftDuration, HeadBobStandSprintValues.WeaponCamXShiftDelay);
                //    YShiftWeaponCam(WeaponCamera.gameObject, HeadBobStandSprintValues.WeaponCamMinShift.y, HeadBobStandSprintValues.WeaponCamMaxShift.y, HeadBobStandSprintValues.WeaponCamYShiftDuration, HeadBobStandSprintValues.WeaponCamYShiftDelay);
                //    ZShiftWeaponCam(WeaponCamera.gameObject, HeadBobStandSprintValues.WeaponCamMinShift.z, HeadBobStandSprintValues.WeaponCamMaxShift.z, HeadBobStandSprintValues.WeaponCamZShiftDuration, HeadBobStandSprintValues.WeaponCamZShiftDelay);

                //    RotWeaponCam(WeaponCamera.gameObject, HeadBobStandSprintValues.WeaponCamMinRotation, HeadBobStandSprintValues.WeaponCamMaxRotation, HeadBobStandSprintValues.WeaponCamRotationDuration, HeadBobStandSprintValues.WeaponCamRotationDelay);
                //    //   YRotWeaponCam(WeaponCamera.gameObject, HeadBobStandSprintValues.WeaponCamMinRotation.y, HeadBobStandSprintValues.WeaponCamMaxRotation.y, HeadBobStandSprintValues.WeaponCamYRotationDuration);
                //    //  ZRotWeaponCam(WeaponCamera.gameObject, HeadBobStandSprintValues.WeaponCamMinRotation.z, HeadBobStandSprintValues.WeaponCamMaxRotation.z, HeadBobStandSprintValues.WeaponCamZRotationDuration);


                //    StopLoop = true;
                //}

                //PosXTimer += Time.deltaTime;
                //PosYTimer += Time.deltaTime;
                //PosZTimer += Time.deltaTime;
                //RotXTimer += Time.deltaTime;
                //RotYTimer += Time.deltaTime;
                //RotZTimer += Time.deltaTime;

                //if (PosXTimer > HeadBobStandSprintValues.TimeToGetNewXValue)
                //{
                //    XShift(BobObject.gameObject, HeadBobStandSprintValues.MinimumShift.x, HeadBobStandSprintValues.MaximumShift.x, HeadBobStandSprintValues.XShiftSpeed, BobObject.localPosition.x, HeadBobStandSprintValues.XShiftDelay);
                //    PosXTimer = 0f;
                //}
                //if (PosYTimer > HeadBobStandSprintValues.TimeToGetNewYValue)
                //{
                //    YShift(BobObject.gameObject, HeadBobStandSprintValues.MinimumShift.y, HeadBobStandSprintValues.MaximumShift.y, HeadBobStandSprintValues.YShiftSpeed, BobObject.localPosition.y, HeadBobStandSprintValues.YShiftDelay);
                //    PosYTimer = 0f;
                //}
                //if (PosZTimer > HeadBobStandSprintValues.TimeToGetNewXValue)
                //{
                //    ZShift(BobObject.gameObject, HeadBobStandSprintValues.MinimumShift.z, HeadBobStandSprintValues.MaximumShift.z, HeadBobStandSprintValues.ZShiftSpeed, BobObject.localPosition.z, HeadBobStandSprintValues.ZShiftDelay);
                //    PosZTimer = 0f;
                //}          
                //if (RotZTimer > HeadBobStandSprintValues.TimeToGetNewZRot)
                //{
                //    ZRot(BobObject.gameObject, HeadBobStandSprintValues.MinimumZRotation, HeadBobStandSprintValues.MaximumZRotation, HeadBobStandSprintValues.ZRotationSpeed, BobObject.localEulerAngles.z, HeadBobStandSprintValues.ZRotDelay);
                //    RotZTimer = 0f;
                //}

            }
            else if (IsMoving && Crouch.instance.IsCrouching == true)
            {

                if (FirstPersonController.instance != null)
                {
                    FirstPersonController.instance.CrouchRunning();
                }

                if (CurrentHoldingPlayerWeapon != null)
                {
                    CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetFloat(CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.RunSpeedParameterName, CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.CrouchRunAnimationSpeed);
                }
                //waveSlice = 0.0f;
                //horizontal = ObjectToShakeDefaultXPosition;
                //vertical = ObjectToShakeDefaultYPosition;
                //if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
                //{
                //    timer = 0.0f;
                //}
                //else
                //{
                //    waveSlice = Mathf.Sin(timer);
                //    timer = timer + HeadBobbingCrouchSpeed;
                //    if (timer > Mathf.PI * 2)
                //    {
                //        timer = timer - (Mathf.PI * 2);
                //    }
                //}
                //if (waveSlice != 0)
                //{
                //    float translateChange = waveSlice * HeadBobbingCrouchDistance;
                //    float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
                //    totalAxes = Mathf.Clamp(totalAxes, MinimumValueToIncrease, MaximumValueToIncrease);
                //    translateChange = totalAxes * translateChange;

                //    Vector3 temp = ObjectToShake.localPosition;
                //    temp.x = ObjectToShakeDefaultXPosition + translateChange;
                //    temp.y = ObjectToShakeDefaultYPosition + translateChange;
                //    ObjectToShake.localPosition = temp;

                //    Vector3 tempo = ObjectToShake.localEulerAngles;
                //    tempo.z = Random.Range(-ShakeAmount * Time.deltaTime, ShakeAmount * Time.deltaTime);
                //    ObjectToShake.localEulerAngles = tempo;
                //}
                //else
                //{
                //    Vector3 temp = ObjectToShake.localPosition;
                //    temp.x = ObjectToShakeDefaultXPosition;
                //    temp.y = ObjectToShakeDefaultYPosition;
                //    ObjectToShake.localPosition = temp;


                //    Vector3 tempo = ObjectToShake.localEulerAngles;
                //    tempo.z = ObjectToShakeDefaultZRotation;
                //    ObjectToShake.localEulerAngles = tempo;
                //}

                //if (StopLoop == false)
                //{
                //    XShift(FpsCamera.gameObject, HeadBobCrouchSprintValues.FpsCamMinimumShift.x, HeadBobCrouchSprintValues.FpsCamMaximumShift.x, HeadBobCrouchSprintValues.FpsCamXShiftDuration, HeadBobCrouchSprintValues.FpsCamXShiftDelay);
                //    YShift(FpsCamera.gameObject, HeadBobCrouchSprintValues.FpsCamMinimumShift.y, HeadBobCrouchSprintValues.FpsCamMaximumShift.y, HeadBobCrouchSprintValues.FpsCamYShiftDuration, HeadBobCrouchSprintValues.FpsCamYShiftDelay);
                //    ZShift(FpsCamera.gameObject, HeadBobCrouchSprintValues.FpsCamMinimumShift.z, HeadBobCrouchSprintValues.FpsCamMaximumShift.z, HeadBobCrouchSprintValues.FpsCamZShiftDuration, HeadBobCrouchSprintValues.FpsCamZShiftDelay);
                //    ZRot(FpsCamera.gameObject, HeadBobCrouchSprintValues.FpsCamMinimumZRotation, HeadBobCrouchSprintValues.FpsCamMaximumZRotation, HeadBobCrouchSprintValues.FpsCamZRotationDuration, HeadBobCrouchSprintValues.FpsCamZRotDelay);

                //    XShiftWeaponCam(WeaponCamera.gameObject, HeadBobCrouchSprintValues.WeaponCamMinShift.x, HeadBobCrouchSprintValues.WeaponCamMaxShift.x, HeadBobCrouchSprintValues.WeaponCamXShiftDuration, HeadBobCrouchSprintValues.WeaponCamXShiftDelay);
                //    YShiftWeaponCam(WeaponCamera.gameObject, HeadBobCrouchSprintValues.WeaponCamMinShift.y, HeadBobCrouchSprintValues.WeaponCamMaxShift.y, HeadBobCrouchSprintValues.WeaponCamYShiftDuration, HeadBobCrouchSprintValues.WeaponCamYShiftDelay);
                //    ZShiftWeaponCam(WeaponCamera.gameObject, HeadBobCrouchSprintValues.WeaponCamMinShift.z, HeadBobCrouchSprintValues.WeaponCamMaxShift.z, HeadBobCrouchSprintValues.WeaponCamZShiftDuration, HeadBobCrouchSprintValues.WeaponCamZShiftDelay);

                //    RotWeaponCam(WeaponCamera.gameObject, HeadBobCrouchSprintValues.WeaponCamMinRotation, HeadBobCrouchSprintValues.WeaponCamMaxRotation, HeadBobCrouchSprintValues.WeaponCamRotationDuration, HeadBobCrouchSprintValues.WeaponCamRotationDelay);
                //    // YRotWeaponCam(WeaponCamera.gameObject, HeadBobCrouchSprintValues.WeaponCamMinRotation.y, HeadBobCrouchSprintValues.WeaponCamMaxRotation.y, HeadBobCrouchSprintValues.WeaponCamYRotationDuration);
                //    //ZRotWeaponCam(WeaponCamera.gameObject, HeadBobCrouchSprintValues.WeaponCamMinRotation.z, HeadBobCrouchSprintValues.WeaponCamMaxRotation.z, HeadBobCrouchSprintValues.WeaponCamZRotationDuration);

                //    StopLoop = true;
                //}

                //PosXTimer += Time.deltaTime;
                //PosYTimer += Time.deltaTime;
                //PosZTimer += Time.deltaTime;
                //RotXTimer += Time.deltaTime;
                //RotYTimer += Time.deltaTime;
                //RotZTimer += Time.deltaTime;

                //if (PosXTimer > HeadBobCrouchSprintValues.TimeToGetNewXValue)
                //{
                //    XShift(BobObject.gameObject, HeadBobCrouchSprintValues.MinimumShift.x, HeadBobCrouchSprintValues.MaximumShift.x, HeadBobCrouchSprintValues.XShiftSpeed, BobObject.localPosition.x, HeadBobCrouchSprintValues.XShiftDelay);
                //    PosXTimer = 0f;
                //}
                //if (PosYTimer > HeadBobCrouchSprintValues.TimeToGetNewYValue)
                //{
                //    YShift(BobObject.gameObject, HeadBobCrouchSprintValues.MinimumShift.y, HeadBobCrouchSprintValues.MaximumShift.y, HeadBobCrouchSprintValues.YShiftSpeed, BobObject.localPosition.y, HeadBobCrouchSprintValues.YShiftDelay);
                //    PosYTimer = 0f;
                //}
                //if (PosZTimer > HeadBobCrouchSprintValues.TimeToGetNewXValue)
                //{
                //    ZShift(BobObject.gameObject, HeadBobCrouchSprintValues.MinimumShift.z, HeadBobCrouchSprintValues.MaximumShift.z, HeadBobCrouchSprintValues.ZShiftSpeed, BobObject.localPosition.z, HeadBobCrouchSprintValues.ZShiftDelay);
                //    PosZTimer = 0f;
                //}
                //if (RotZTimer > HeadBobCrouchSprintValues.TimeToGetNewZRot)
                //{
                //    ZRot(BobObject.gameObject, HeadBobCrouchSprintValues.MinimumZRotation, HeadBobCrouchSprintValues.MaximumZRotation, HeadBobCrouchSprintValues.ZRotationSpeed, BobObject.localEulerAngles.z, HeadBobCrouchSprintValues.ZRotDelay);
                //    RotZTimer = 0f;
                //}

            }
            else
            {
                //if (StopLoop == true)
                //{
                //    if (AutoReset == false)
                //    {
                //        LeanTween.cancel(FpsCamera.gameObject);
                //        LeanTween.cancel(WeaponCamera.gameObject);

                //        if(Crouch.instance.IsCrouching == false)
                //        {
                //            LeanTween.moveLocalX(FpsCamera.gameObject, BobDefaultPos.x, FpsCameraResetDurations.StandSprintResetDuration).setFrom(FpsCamera.transform.localPosition.x);
                //            LeanTween.moveLocalY(FpsCamera.gameObject, BobDefaultPos.y, FpsCameraResetDurations.StandSprintResetDuration).setFrom(FpsCamera.transform.localPosition.y);
                //            LeanTween.moveLocalZ(FpsCamera.gameObject, BobDefaultPos.z, FpsCameraResetDurations.StandSprintResetDuration).setFrom(FpsCamera.transform.localPosition.z);

                //            LeanTween.rotateZ(FpsCamera.gameObject, BobDefaultRot.z, FpsCameraResetDurations.StandSprintResetDuration).setFrom(FpsCamera.transform.localEulerAngles.z);

                //            //LeanTween.moveLocalX(WeaponCamera.gameObject, WeaponCamDefaultPos.x, ResetDuration).setFrom(WeaponCamera.transform.localPosition.x);
                //            //LeanTween.moveLocalY(WeaponCamera.gameObject, WeaponCamDefaultPos.y, ResetDuration).setFrom(WeaponCamera.transform.localPosition.y);
                //            //LeanTween.moveLocalZ(WeaponCamera.gameObject, WeaponCamDefaultPos.z, ResetDuration).setFrom(WeaponCamera.transform.localPosition.z);

                //            LeanTween.moveLocal(WeaponCamera.gameObject, WeaponCamDefaultPos, WeaponCameraResetDurations.StandSprintResetDuration).setFrom(WeaponCamera.transform.localPosition);
                //            LeanTween.rotateLocal(WeaponCamera.gameObject, WeaponCamDefaultRot, WeaponCameraResetDurations.StandSprintResetDuration).setFrom(WeaponCamera.transform.localEulerAngles);
                //            //LeanTween.rotateY(WeaponCamera.gameObject, WeaponCamDefaultRot.y, ResetDuration).setFrom(WeaponCamera.transform.localEulerAngles.y);
                //            //LeanTween.rotateZ(WeaponCamera.gameObject, WeaponCamDefaultRot.z, ResetDuration).setFrom(WeaponCamera.transform.localEulerAngles.z);
                //        }
                //        else
                //        {
                //            LeanTween.moveLocalX(FpsCamera.gameObject, BobDefaultPos.x, FpsCameraResetDurations.CrouchSprintResetDuration).setFrom(FpsCamera.transform.localPosition.x);
                //            LeanTween.moveLocalY(FpsCamera.gameObject, BobDefaultPos.y, FpsCameraResetDurations.CrouchSprintResetDuration).setFrom(FpsCamera.transform.localPosition.y);
                //            LeanTween.moveLocalZ(FpsCamera.gameObject, BobDefaultPos.z, FpsCameraResetDurations.CrouchSprintResetDuration).setFrom(FpsCamera.transform.localPosition.z);

                //            LeanTween.rotateZ(FpsCamera.gameObject, BobDefaultRot.z, FpsCameraResetDurations.CrouchSprintResetDuration).setFrom(FpsCamera.transform.localEulerAngles.z);

                //            //LeanTween.moveLocalX(WeaponCamera.gameObject, WeaponCamDefaultPos.x, ResetDuration).setFrom(WeaponCamera.transform.localPosition.x);
                //            //LeanTween.moveLocalY(WeaponCamera.gameObject, WeaponCamDefaultPos.y, ResetDuration).setFrom(WeaponCamera.transform.localPosition.y);
                //            //LeanTween.moveLocalZ(WeaponCamera.gameObject, WeaponCamDefaultPos.z, ResetDuration).setFrom(WeaponCamera.transform.localPosition.z);

                //            LeanTween.moveLocal(WeaponCamera.gameObject, WeaponCamDefaultPos, WeaponCameraResetDurations.CrouchSprintResetDuration).setFrom(WeaponCamera.transform.localPosition);
                //            LeanTween.rotateLocal(WeaponCamera.gameObject, WeaponCamDefaultRot, WeaponCameraResetDurations.CrouchSprintResetDuration).setFrom(WeaponCamera.transform.localEulerAngles);
                //            //LeanTween.rotateY(WeaponCamera.gameObject, WeaponCamDefaultRot.y, ResetDuration).setFrom(WeaponCamera.transform.localEulerAngles.y);
                //            //LeanTween.rotateZ(WeaponCamera.gameObject, WeaponCamDefaultRot.z, ResetDuration).setFrom(WeaponCamera.transform.localEulerAngles.z);
                //        }

                //        AutoReset = true;
                //    }
                //}
                //LeanTween.moveY(BobObject.gameObject,0.6f,2f);
                //if(BobObject.localPosition.y == 0.6f)
                //{
                //    LeanTween.cancel(BobObject.gameObject);
                //}

                //BobObject.transform.localPosition = Vector3.MoveTowards(transform.localPosition, BobDefaultPos, HeadBobStandSprintValues.ResetDuration * Time.deltaTime);
                // BobObject.transform.localEulerAngles = Vector3.MoveTowards(transform.localEulerAngles, BobDefaultPos, HeadBobStandSprintValues.ResetDuration * Time.deltaTime);
            }
        }
        public void FindRequiredObjects()
        {            
            if (FindObjectOfType<PlayerWeapon>() != null)
            {
                CurrentHoldingPlayerWeapon = FindObjectOfType<PlayerWeapon>();
                CurrentHoldingPlayerWeapon.playershoot = false;
            }

             if(FirstPersonController.instance != null)    // if (SwitchingPlayerWeaponsComponent != null)
            {
                //if (SwichWeapons.instance.WasPreviousWeaponAimed == true)
                //{
                //    ob.IsAimed = false;
                //    ob.IsHipFire = true;
                //    Debug.Log("OB " + ob.IsAimed);
                //    SwichWeapons.instance.WasPreviousWeaponAimed = false;
                //}
                if (FirstPersonController.instance.JoystickScript.IsWalking == true) //if (SwitchingPlayerWeaponsComponent.IsPreviousWeaponStillWalking == true)
                {
                    CurrentHoldingPlayerWeapon.IsWalking = true;
                    CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.IdleAnimationParametreName, false);
                    // ob.AnimationsNames.WeaponAnimatorComponent.Play(ob.AnimationsNames.WalkingAnimationName, -1, 0f);
                    CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.WalkAnimationParametreName, true);
                    CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.RunAnimationParametreName, false);

                    if (JoyStick.Instance != null)
                    {
                        for (int o = 0; o < JoyStick.Instance.WalkingSounds.Length; o++)
                        {
                            if (!JoyStick.Instance.WalkingSounds[o].isPlaying)
                            {
                                JoyStick.Instance.WalkingSounds[o].Play();
                            }
                        }
                    }
                    StopCoroutine(ResetWeapon());
                }
                else if (IsMoving == true)
                {
                    CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.IdleAnimationParametreName, false);
                    CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.WalkAnimationParametreName, false);
                    CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetBool(CurrentHoldingPlayerWeapon.RunAnimationParametreName, true);
                    // ob.AnimationsNames.WeaponAnimatorComponent.Play(ob.AnimationsNames.RunAnimationname, -1, 0f);
                }
                else
                {
                    StartCoroutine(ResetWeapon());
                }
                //else
                //{
                //    ob.IsWalking = false;
                //    ob.WeaponAnimatorComponent.SetBool(ob.IdleAnimationName, true);
                //    ob.WeaponAnimatorComponent.Play(ob.IdleAnimationName);

                //    if (JoyStick.Instance != null)
                //    {
                //        if (JoyStick.Instance.WalkingSounds.isPlaying)
                //        {
                //            JoyStick.Instance.WalkingSounds.Stop();
                //        }
                //    }
                //}
            }

          

            if (MobileTest.Instance != null)
            {
                MobileTest.Instance.StartTest();
            }

        }
        IEnumerator ResetWeapon()
        {
            if (CurrentHoldingPlayerWeapon != null)
            {
                if (CurrentHoldingPlayerWeapon.gameObject.activeInHierarchy == true)
                {
                    if (CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent != null)
                    {
                        yield return new WaitForSeconds(CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.GetCurrentAnimatorStateInfo(0).length);
                        if (CurrentHoldingPlayerWeapon.IsWalking == false && CurrentHoldingPlayerWeapon.playershoot == false && CurrentHoldingPlayerWeapon.Reload.isreloading == false && IsMoving == false)
                        {
                            // The lines below are recently commented because If in the 'Switching Player Weapons' script 'Auto Aim If Switched from Aimed Weapon' is enabled we don't want
                            // the weapon to playback the idle animation just after the wield is completed in that case after playing wield we can directly playback the Aimed animation ( which is the initial pose in this case )
                            // as if you uncomment the lines below than it will do it the opposite way meaning wield < Idle and than Aimed ( which is the initial pose in this case ) which is not good.
                            // So keep them to be commented for better results.

                            //ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.IdleAnimationParametreName, true);
                            //ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.WalkAnimationParametreName, false);
                            //ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.RunAnimationParametreName, false);
                            // ob.RequiredComponents.WeaponAnimatorComponent.Play(ob.IdleAnimationName);

                            if (JoyStick.Instance != null)
                            {
                                for (int o = 0; o < JoyStick.Instance.WalkingSounds.Length; o++)
                                {
                                    if (JoyStick.Instance.WalkingSounds[o].isPlaying)
                                    {
                                        JoyStick.Instance.WalkingSounds[o].Stop();
                                    }
                                }
                            }
                        }
                    }

                }
            }


        }
        //public void XShift(GameObject obj, float MinShift, float MaxShift, float Dur, float Delay)
        //{
        //    LeanTween.moveLocalX(obj, MinShift, Mathf.Abs(Dur)).setFrom(obj.transform.localPosition.x).setDelay(Delay);
        //    LeanTween.moveLocalX(obj, MaxShift, Mathf.Abs(Dur)).setFrom(MinShift).setLoopPingPong().setDelay(Mathf.Abs(Dur) + Delay);
        //}
        //public void YShift(GameObject obj, float MinShift, float MaxShift, float Dur, float Delay)
        //{
        //    LeanTween.moveLocalY(obj, MinShift, Mathf.Abs(Dur)).setFrom(obj.transform.localPosition.y).setDelay(Delay);
        //    LeanTween.moveLocalY(obj, MaxShift, Mathf.Abs(Dur)).setFrom(MinShift).setLoopPingPong().setDelay(Mathf.Abs(Dur) + Delay);
        //}
        //public void ZShift(GameObject obj, float MinShift, float MaxShift, float Dur, float Delay)
        //{
        //    LeanTween.moveLocalZ(obj, MinShift, Mathf.Abs(Dur)).setFrom(obj.transform.localPosition.z).setDelay(Delay);
        //    LeanTween.moveLocalZ(obj, MaxShift, Mathf.Abs(Dur)).setFrom(MinShift).setLoopPingPong().setDelay(Mathf.Abs(Dur) + Delay);
        //}
        //public void ZRot(GameObject obj, float MinShift, float MaxShift, float Dur, float Delay)
        //{
        //    Vector3 rot = FpsCamera.transform.localEulerAngles;
        //    rot.z = BobDefaultRot.z;
        //    FpsCamera.transform.localEulerAngles = rot;
        //    LeanTween.rotateZ(obj, MinShift, Mathf.Abs(Dur)).setFrom(obj.transform.localEulerAngles.z).setDelay(Delay);
        //    LeanTween.rotateZ(obj, MaxShift, Mathf.Abs(Dur)).setFrom(MinShift).setLoopPingPong().setDelay(Mathf.Abs(Dur) + Delay);
        //}
        //public void XShiftWeaponCam(GameObject obj, float MinShift, float MaxShift, float Dur,float Delay)
        //{
        //    LeanTween.moveLocalX(obj, MinShift, Mathf.Abs(Dur)).setFrom(obj.transform.localPosition.x).setDelay(Delay);
        //    LeanTween.moveLocalX(obj, MaxShift, Mathf.Abs(Dur)).setFrom(MinShift).setLoopPingPong().setDelay(Mathf.Abs(Dur) + Delay);
        //}
        //public void YShiftWeaponCam(GameObject obj, float MinShift, float MaxShift, float Dur,float Delay)
        //{
        //    LeanTween.moveLocalY(obj, MinShift, Mathf.Abs(Dur)).setFrom(obj.transform.localPosition.y).setDelay(Delay);
        //    LeanTween.moveLocalY(obj, MaxShift, Mathf.Abs(Dur)).setFrom(MinShift).setLoopPingPong().setDelay(Mathf.Abs(Dur) + Delay);
        //}
        //public void ZShiftWeaponCam(GameObject obj, float MinShift, float MaxShift, float Dur,float Delay)
        //{
        //    LeanTween.moveLocalZ(obj, MinShift, Mathf.Abs(Dur)).setFrom(obj.transform.localPosition.z).setDelay(Delay);
        //    LeanTween.moveLocalZ(obj, MaxShift, Mathf.Abs(Dur)).setFrom(MinShift).setLoopPingPong().setDelay(Mathf.Abs(Dur) + Delay);
        //}
        //public void RotWeaponCam(GameObject obj, Vector3 MinShift, Vector3 MaxShift, float Dur,float Delay)
        //{
        //    WeaponCamera.transform.localEulerAngles = WeaponCamDefaultRot;
        //    LeanTween.rotateLocal(obj, MinShift, Mathf.Abs(Dur)).setFrom(obj.transform.localEulerAngles).setDelay(Delay);
        //    LeanTween.rotateLocal(obj, MaxShift, Mathf.Abs(Dur)).setFrom(MinShift).setLoopPingPong().setDelay(Mathf.Abs(Dur) + Delay);
        //}
        //public void YRotWeaponCam(GameObject obj, float MinShift, float MaxShift, float Dur)
        //{
        //    Vector3 rot = WeaponCamera.transform.localEulerAngles;
        //    rot.y = WeaponCamDefaultRot.y;
        //    WeaponCamera.transform.localEulerAngles = rot;
        //    LeanTween.rotateY(obj, MaxShift, Dur).setFrom(MinShift).setLoopPingPong();
        //}
        //public void ZRotWeaponCam(GameObject obj, float MinShift, float MaxShift, float Dur)
        //{
        //    Vector3 rot = WeaponCamera.transform.localEulerAngles;
        //    rot.z = WeaponCamDefaultRot.z;
        //    WeaponCamera.transform.localEulerAngles = rot;
        //    LeanTween.rotateZ(obj, MaxShift, Dur).setFrom(MinShift).setLoopPingPong();
        //}

        //public void WalkStandValues()
        //{
        //    if (WalkStopLoop == false)
        //    {
        //        XShift(FpsCamera.gameObject, HeadBobStandWalkValues.FpsCamMinimumShift.x, HeadBobStandWalkValues.FpsCamMaximumShift.x, HeadBobStandWalkValues.FpsCamXShiftDuration, HeadBobStandWalkValues.FpsCamXShiftDelay);
        //        YShift(FpsCamera.gameObject, HeadBobStandWalkValues.FpsCamMinimumShift.y, HeadBobStandWalkValues.FpsCamMaximumShift.y, HeadBobStandWalkValues.FpsCamYShiftDuration, HeadBobStandWalkValues.FpsCamYShiftDelay);
        //        ZShift(FpsCamera.gameObject, HeadBobStandWalkValues.FpsCamMinimumShift.z, HeadBobStandWalkValues.FpsCamMaximumShift.z, HeadBobStandWalkValues.FpsCamZShiftDuration, HeadBobStandWalkValues.FpsCamZShiftDelay);
        //        ZRot(FpsCamera.gameObject, HeadBobStandWalkValues.FpsCamMinimumZRotation, HeadBobStandWalkValues.FpsCamMaximumZRotation, HeadBobStandWalkValues.FpsCamZRotationDuration, HeadBobStandWalkValues.FpsCamZRotDelay);

        //        XShiftWeaponCam(WeaponCamera.gameObject, HeadBobStandWalkValues.WeaponCamMinShift.x, HeadBobStandWalkValues.WeaponCamMaxShift.x, HeadBobStandWalkValues.WeaponCamXShiftDuration, HeadBobStandWalkValues.WeaponCamXShiftDelay);
        //        YShiftWeaponCam(WeaponCamera.gameObject, HeadBobStandWalkValues.WeaponCamMinShift.y, HeadBobStandWalkValues.WeaponCamMaxShift.y, HeadBobStandWalkValues.WeaponCamYShiftDuration, HeadBobStandWalkValues.WeaponCamYShiftDelay);
        //        ZShiftWeaponCam(WeaponCamera.gameObject, HeadBobStandWalkValues.WeaponCamMinShift.z, HeadBobStandWalkValues.WeaponCamMaxShift.z, HeadBobStandWalkValues.WeaponCamZShiftDuration, HeadBobStandWalkValues.WeaponCamZShiftDelay);

        //        RotWeaponCam(WeaponCamera.gameObject, HeadBobStandWalkValues.WeaponCamMinRotation, HeadBobStandWalkValues.WeaponCamMaxRotation, HeadBobStandWalkValues.WeaponCamRotationDuration, HeadBobStandWalkValues.WeaponCamRotationDelay);
        //        //YRotWeaponCam(WeaponCamera.gameObject, HeadBobStandWalkValues.WeaponCamMinRotation.y, HeadBobStandWalkValues.WeaponCamMaxRotation.y, HeadBobStandWalkValues.WeaponCamYRotationDuration);
        //        // ZRotWeaponCam(WeaponCamera.gameObject, HeadBobStandWalkValues.WeaponCamMinRotation.z, HeadBobStandWalkValues.WeaponCamMaxRotation.z, HeadBobStandWalkValues.WeaponCamZRotationDuration);

        //        WalkStopLoop = true;
        //    }

        //    //PosXTimer += Time.deltaTime;
        //    //PosYTimer += Time.deltaTime;
        //    //PosZTimer += Time.deltaTime;
        //    //RotXTimer += Time.deltaTime;
        //    //RotYTimer += Time.deltaTime;
        //    //RotZTimer += Time.deltaTime;

        //    //if (PosXTimer > HeadBobStandWalkValues.TimeToGetNewXValue)
        //    //{
        //    //    XShift(BobObject.gameObject, HeadBobStandWalkValues.MinimumShift.x, HeadBobStandWalkValues.MaximumShift.x, HeadBobStandWalkValues.XShiftSpeed, BobObject.localPosition.x, HeadBobStandWalkValues.XShiftDelay);
        //    //    PosXTimer = 0f;
        //    //}
        //    //if (PosYTimer > HeadBobStandWalkValues.TimeToGetNewYValue)
        //    //{
        //    //    YShift(BobObject.gameObject, HeadBobStandWalkValues.MinimumShift.y, HeadBobStandWalkValues.MaximumShift.y, HeadBobStandWalkValues.YShiftSpeed, BobObject.localPosition.y, HeadBobStandWalkValues.YShiftDelay);
        //    //    PosYTimer = 0f;
        //    //}
        //    //if (PosZTimer > HeadBobStandWalkValues.TimeToGetNewXValue)
        //    //{
        //    //    ZShift(BobObject.gameObject, HeadBobStandWalkValues.MinimumShift.z, HeadBobStandWalkValues.MaximumShift.z, HeadBobStandWalkValues.ZShiftSpeed, BobObject.localPosition.z, HeadBobStandWalkValues.ZShiftDelay);
        //    //    PosZTimer = 0f;
        //    //}
        //    //if (RotZTimer > HeadBobStandWalkValues.TimeToGetNewZRot)
        //    //{
        //    //    ZRot(BobObject.gameObject, HeadBobStandWalkValues.MinimumZRotation, HeadBobStandWalkValues.MaximumZRotation, HeadBobStandWalkValues.ZRotationSpeed, BobObject.localEulerAngles.z, HeadBobStandWalkValues.ZRotDelay);
        //    //    RotZTimer = 0f;
        //    //}
        //}
        //public void WalkSniperStandValues()
        //{
        //    if (WalkStopLoop == false)
        //    {
        //        XShift(FpsCamera.gameObject, HeadBobSniperStandWalkValues.FpsCamMinimumShift.x, HeadBobSniperStandWalkValues.FpsCamMaximumShift.x, HeadBobSniperStandWalkValues.FpsCamXShiftDuration, HeadBobSniperStandWalkValues.FpsCamXShiftDelay);
        //        YShift(FpsCamera.gameObject, HeadBobSniperStandWalkValues.FpsCamMinimumShift.y, HeadBobSniperStandWalkValues.FpsCamMaximumShift.y, HeadBobSniperStandWalkValues.FpsCamYShiftDuration, HeadBobSniperStandWalkValues.FpsCamYShiftDelay);
        //        ZShift(FpsCamera.gameObject, HeadBobSniperStandWalkValues.FpsCamMinimumShift.z, HeadBobSniperStandWalkValues.FpsCamMaximumShift.z, HeadBobSniperStandWalkValues.FpsCamZShiftDuration, HeadBobSniperStandWalkValues.FpsCamZShiftDelay);
        //        ZRot(FpsCamera.gameObject, HeadBobSniperStandWalkValues.FpsCamMinimumZRotation, HeadBobSniperStandWalkValues.FpsCamMaximumZRotation, HeadBobSniperStandWalkValues.FpsCamZRotationDuration, HeadBobSniperStandWalkValues.FpsCamZRotDelay);

        //        XShiftWeaponCam(WeaponCamera.gameObject, HeadBobSniperStandWalkValues.WeaponCamMinShift.x, HeadBobSniperStandWalkValues.WeaponCamMaxShift.x, HeadBobSniperStandWalkValues.WeaponCamXShiftDuration, HeadBobSniperStandWalkValues.WeaponCamXShiftDelay);
        //        YShiftWeaponCam(WeaponCamera.gameObject, HeadBobSniperStandWalkValues.WeaponCamMinShift.y, HeadBobSniperStandWalkValues.WeaponCamMaxShift.y, HeadBobSniperStandWalkValues.WeaponCamYShiftDuration, HeadBobSniperStandWalkValues.WeaponCamYShiftDelay);
        //        ZShiftWeaponCam(WeaponCamera.gameObject, HeadBobSniperStandWalkValues.WeaponCamMinShift.z, HeadBobSniperStandWalkValues.WeaponCamMaxShift.z, HeadBobSniperStandWalkValues.WeaponCamZShiftDuration, HeadBobSniperStandWalkValues.WeaponCamZShiftDelay);

        //        RotWeaponCam(WeaponCamera.gameObject, HeadBobSniperStandWalkValues.WeaponCamMinRotation, HeadBobSniperStandWalkValues.WeaponCamMaxRotation, HeadBobSniperStandWalkValues.WeaponCamRotationDuration, HeadBobSniperStandWalkValues.WeaponCamRotationDelay);
        //        //YRotWeaponCam(WeaponCamera.gameObject, HeadBobStandWalkValues.WeaponCamMinRotation.y, HeadBobStandWalkValues.WeaponCamMaxRotation.y, HeadBobStandWalkValues.WeaponCamYRotationDuration);
        //        // ZRotWeaponCam(WeaponCamera.gameObject, HeadBobStandWalkValues.WeaponCamMinRotation.z, HeadBobStandWalkValues.WeaponCamMaxRotation.z, HeadBobStandWalkValues.WeaponCamZRotationDuration);

        //        WalkStopLoop = true;
        //    }

        //    //PosXTimer += Time.deltaTime;
        //    //PosYTimer += Time.deltaTime;
        //    //PosZTimer += Time.deltaTime;
        //    //RotXTimer += Time.deltaTime;
        //    //RotYTimer += Time.deltaTime;
        //    //RotZTimer += Time.deltaTime;

        //    //if (PosXTimer > HeadBobStandWalkValues.TimeToGetNewXValue)
        //    //{
        //    //    XShift(BobObject.gameObject, HeadBobStandWalkValues.MinimumShift.x, HeadBobStandWalkValues.MaximumShift.x, HeadBobStandWalkValues.XShiftSpeed, BobObject.localPosition.x, HeadBobStandWalkValues.XShiftDelay);
        //    //    PosXTimer = 0f;
        //    //}
        //    //if (PosYTimer > HeadBobStandWalkValues.TimeToGetNewYValue)
        //    //{
        //    //    YShift(BobObject.gameObject, HeadBobStandWalkValues.MinimumShift.y, HeadBobStandWalkValues.MaximumShift.y, HeadBobStandWalkValues.YShiftSpeed, BobObject.localPosition.y, HeadBobStandWalkValues.YShiftDelay);
        //    //    PosYTimer = 0f;
        //    //}
        //    //if (PosZTimer > HeadBobStandWalkValues.TimeToGetNewXValue)
        //    //{
        //    //    ZShift(BobObject.gameObject, HeadBobStandWalkValues.MinimumShift.z, HeadBobStandWalkValues.MaximumShift.z, HeadBobStandWalkValues.ZShiftSpeed, BobObject.localPosition.z, HeadBobStandWalkValues.ZShiftDelay);
        //    //    PosZTimer = 0f;
        //    //}
        //    //if (RotZTimer > HeadBobStandWalkValues.TimeToGetNewZRot)
        //    //{
        //    //    ZRot(BobObject.gameObject, HeadBobStandWalkValues.MinimumZRotation, HeadBobStandWalkValues.MaximumZRotation, HeadBobStandWalkValues.ZRotationSpeed, BobObject.localEulerAngles.z, HeadBobStandWalkValues.ZRotDelay);
        //    //    RotZTimer = 0f;
        //    //}
        //}
        //public void WalkOpticalStandValues()
        //{
        //    if (WalkStopLoop == false)
        //    {
        //        XShift(FpsCamera.gameObject, HeadBobOpticalStandWalkValues.FpsCamMinimumShift.x, HeadBobOpticalStandWalkValues.FpsCamMaximumShift.x, HeadBobOpticalStandWalkValues.FpsCamXShiftDuration, HeadBobStandWalkValues.FpsCamXShiftDelay);
        //        YShift(FpsCamera.gameObject, HeadBobOpticalStandWalkValues.FpsCamMinimumShift.y, HeadBobOpticalStandWalkValues.FpsCamMaximumShift.y, HeadBobOpticalStandWalkValues.FpsCamYShiftDuration, HeadBobStandWalkValues.FpsCamYShiftDelay);
        //        ZShift(FpsCamera.gameObject, HeadBobOpticalStandWalkValues.FpsCamMinimumShift.z, HeadBobOpticalStandWalkValues.FpsCamMaximumShift.z, HeadBobOpticalStandWalkValues.FpsCamZShiftDuration, HeadBobStandWalkValues.FpsCamZShiftDelay);
        //        ZRot(FpsCamera.gameObject, HeadBobOpticalStandWalkValues.FpsCamMinimumZRotation, HeadBobOpticalStandWalkValues.FpsCamMaximumZRotation, HeadBobOpticalStandWalkValues.FpsCamZRotationDuration, HeadBobStandWalkValues.FpsCamZRotDelay);

        //        XShiftWeaponCam(WeaponCamera.gameObject, HeadBobOpticalStandWalkValues.WeaponCamMinShift.x, HeadBobOpticalStandWalkValues.WeaponCamMaxShift.x, HeadBobOpticalStandWalkValues.WeaponCamXShiftDuration, HeadBobOpticalStandWalkValues.WeaponCamXShiftDelay);
        //        YShiftWeaponCam(WeaponCamera.gameObject, HeadBobOpticalStandWalkValues.WeaponCamMinShift.y, HeadBobOpticalStandWalkValues.WeaponCamMaxShift.y, HeadBobOpticalStandWalkValues.WeaponCamYShiftDuration, HeadBobOpticalStandWalkValues.WeaponCamYShiftDelay);
        //        ZShiftWeaponCam(WeaponCamera.gameObject, HeadBobOpticalStandWalkValues.WeaponCamMinShift.z, HeadBobOpticalStandWalkValues.WeaponCamMaxShift.z, HeadBobOpticalStandWalkValues.WeaponCamZShiftDuration, HeadBobOpticalStandWalkValues.WeaponCamZShiftDelay);

        //        RotWeaponCam(WeaponCamera.gameObject, HeadBobOpticalStandWalkValues.WeaponCamMinRotation, HeadBobOpticalStandWalkValues.WeaponCamMaxRotation, HeadBobOpticalStandWalkValues.WeaponCamRotationDuration, HeadBobOpticalStandWalkValues.WeaponCamRotationDelay);
        //        //YRotWeaponCam(WeaponCamera.gameObject, HeadBobStandWalkValues.WeaponCamMinRotation.y, HeadBobStandWalkValues.WeaponCamMaxRotation.y, HeadBobStandWalkValues.WeaponCamYRotationDuration);
        //        // ZRotWeaponCam(WeaponCamera.gameObject, HeadBobStandWalkValues.WeaponCamMinRotation.z, HeadBobStandWalkValues.WeaponCamMaxRotation.z, HeadBobStandWalkValues.WeaponCamZRotationDuration);

        //        WalkStopLoop = true;
        //    }

        //    //PosXTimer += Time.deltaTime;
        //    //PosYTimer += Time.deltaTime;
        //    //PosZTimer += Time.deltaTime;
        //    //RotXTimer += Time.deltaTime;
        //    //RotYTimer += Time.deltaTime;
        //    //RotZTimer += Time.deltaTime;

        //    //if (PosXTimer > HeadBobStandWalkValues.TimeToGetNewXValue)
        //    //{
        //    //    XShift(BobObject.gameObject, HeadBobStandWalkValues.MinimumShift.x, HeadBobStandWalkValues.MaximumShift.x, HeadBobStandWalkValues.XShiftSpeed, BobObject.localPosition.x, HeadBobStandWalkValues.XShiftDelay);
        //    //    PosXTimer = 0f;
        //    //}
        //    //if (PosYTimer > HeadBobStandWalkValues.TimeToGetNewYValue)
        //    //{
        //    //    YShift(BobObject.gameObject, HeadBobStandWalkValues.MinimumShift.y, HeadBobStandWalkValues.MaximumShift.y, HeadBobStandWalkValues.YShiftSpeed, BobObject.localPosition.y, HeadBobStandWalkValues.YShiftDelay);
        //    //    PosYTimer = 0f;
        //    //}
        //    //if (PosZTimer > HeadBobStandWalkValues.TimeToGetNewXValue)
        //    //{
        //    //    ZShift(BobObject.gameObject, HeadBobStandWalkValues.MinimumShift.z, HeadBobStandWalkValues.MaximumShift.z, HeadBobStandWalkValues.ZShiftSpeed, BobObject.localPosition.z, HeadBobStandWalkValues.ZShiftDelay);
        //    //    PosZTimer = 0f;
        //    //}
        //    //if (RotZTimer > HeadBobStandWalkValues.TimeToGetNewZRot)
        //    //{
        //    //    ZRot(BobObject.gameObject, HeadBobStandWalkValues.MinimumZRotation, HeadBobStandWalkValues.MaximumZRotation, HeadBobStandWalkValues.ZRotationSpeed, BobObject.localEulerAngles.z, HeadBobStandWalkValues.ZRotDelay);
        //    //    RotZTimer = 0f;
        //    //}
        //}
        //public void WalkCrouchValues()
        //{
        //    if (WalkStopLoop == false)
        //    {
        //        XShift(FpsCamera.gameObject, HeadBobCrouchWalkValues.FpsCamMinimumShift.x, HeadBobCrouchWalkValues.FpsCamMaximumShift.x, HeadBobCrouchWalkValues.FpsCamXShiftDuration, HeadBobCrouchWalkValues.FpsCamXShiftDelay);
        //        YShift(FpsCamera.gameObject, HeadBobCrouchWalkValues.FpsCamMinimumShift.y, HeadBobCrouchWalkValues.FpsCamMaximumShift.y, HeadBobCrouchWalkValues.FpsCamYShiftDuration, HeadBobCrouchWalkValues.FpsCamYShiftDelay);
        //        ZShift(FpsCamera.gameObject, HeadBobCrouchWalkValues.FpsCamMinimumShift.z, HeadBobCrouchWalkValues.FpsCamMaximumShift.z, HeadBobCrouchWalkValues.FpsCamZShiftDuration, HeadBobCrouchWalkValues.FpsCamZShiftDelay);
        //        ZRot(FpsCamera.gameObject, HeadBobCrouchWalkValues.FpsCamMinimumZRotation, HeadBobCrouchWalkValues.FpsCamMaximumZRotation, HeadBobCrouchWalkValues.FpsCamZRotationDuration, HeadBobCrouchWalkValues.FpsCamZRotDelay);

        //        XShiftWeaponCam(WeaponCamera.gameObject, HeadBobCrouchWalkValues.WeaponCamMinShift.x, HeadBobCrouchWalkValues.WeaponCamMaxShift.x, HeadBobCrouchWalkValues.WeaponCamXShiftDuration, HeadBobCrouchWalkValues.WeaponCamXShiftDelay);
        //        YShiftWeaponCam(WeaponCamera.gameObject, HeadBobCrouchWalkValues.WeaponCamMinShift.y, HeadBobCrouchWalkValues.WeaponCamMaxShift.y, HeadBobCrouchWalkValues.WeaponCamYShiftDuration, HeadBobCrouchWalkValues.WeaponCamYShiftDelay);
        //        ZShiftWeaponCam(WeaponCamera.gameObject, HeadBobCrouchWalkValues.WeaponCamMinShift.z, HeadBobCrouchWalkValues.WeaponCamMaxShift.z, HeadBobCrouchWalkValues.WeaponCamZShiftDuration, HeadBobCrouchWalkValues.WeaponCamZShiftDelay);

        //        RotWeaponCam(WeaponCamera.gameObject, HeadBobCrouchWalkValues.WeaponCamMinRotation, HeadBobCrouchWalkValues.WeaponCamMaxRotation, HeadBobCrouchWalkValues.WeaponCamRotationDuration, HeadBobCrouchWalkValues.WeaponCamRotationDelay);
        //        // YRotWeaponCam(WeaponCamera.gameObject, HeadBobCrouchWalkValues.WeaponCamMinRotation.y, HeadBobCrouchWalkValues.WeaponCamMaxRotation.y, HeadBobCrouchWalkValues.WeaponCamYRotationDuration);
        //        //ZRotWeaponCam(WeaponCamera.gameObject, HeadBobCrouchWalkValues.WeaponCamMinRotation.z, HeadBobCrouchWalkValues.WeaponCamMaxRotation.z, HeadBobCrouchWalkValues.WeaponCamZRotationDuration);

        //        WalkStopLoop = true;
        //    }

        //    //PosXTimer += Time.deltaTime;
        //    //PosYTimer += Time.deltaTime;
        //    //PosZTimer += Time.deltaTime;
        //    //RotXTimer += Time.deltaTime;
        //    //RotYTimer += Time.deltaTime;
        //    //RotZTimer += Time.deltaTime;

        //    //if (PosXTimer > HeadBobCrouchWalkValues.TimeToGetNewXValue)
        //    //{
        //    //    XShift(BobObject.gameObject, HeadBobCrouchWalkValues.MinimumShift.x, HeadBobCrouchWalkValues.MaximumShift.x, HeadBobCrouchWalkValues.XShiftSpeed, BobObject.localPosition.x, HeadBobCrouchWalkValues.XShiftDelay);
        //    //    PosXTimer = 0f;
        //    //}
        //    //if (PosYTimer > HeadBobCrouchWalkValues.TimeToGetNewYValue)
        //    //{
        //    //    YShift(BobObject.gameObject, HeadBobCrouchWalkValues.MinimumShift.y, HeadBobCrouchWalkValues.MaximumShift.y, HeadBobCrouchWalkValues.YShiftSpeed, BobObject.localPosition.y, HeadBobCrouchWalkValues.YShiftDelay);
        //    //    PosYTimer = 0f;
        //    //}
        //    //if (PosZTimer > HeadBobCrouchWalkValues.TimeToGetNewXValue)
        //    //{
        //    //    ZShift(BobObject.gameObject, HeadBobCrouchWalkValues.MinimumShift.z, HeadBobCrouchWalkValues.MaximumShift.z, HeadBobCrouchWalkValues.ZShiftSpeed, BobObject.localPosition.z, HeadBobCrouchWalkValues.ZShiftDelay);
        //    //    PosZTimer = 0f;
        //    //}
        //    //if (RotZTimer > HeadBobCrouchWalkValues.TimeToGetNewZRot)
        //    //{
        //    //    ZRot(BobObject.gameObject, HeadBobCrouchWalkValues.MinimumZRotation, HeadBobCrouchWalkValues.MaximumZRotation, HeadBobCrouchWalkValues.ZRotationSpeed, BobObject.localEulerAngles.z, HeadBobCrouchWalkValues.ZRotDelay);
        //    //    RotZTimer = 0f;
        //    //}
        //}
        //public void WalkSniperCrouchValues()
        //{
        //    if (WalkStopLoop == false)
        //    {
        //        XShift(FpsCamera.gameObject, HeadBobSniperCrouchWalkValues.FpsCamMinimumShift.x, HeadBobSniperCrouchWalkValues.FpsCamMaximumShift.x, HeadBobSniperCrouchWalkValues.FpsCamXShiftDuration, HeadBobCrouchWalkValues.FpsCamXShiftDelay);
        //        YShift(FpsCamera.gameObject, HeadBobSniperCrouchWalkValues.FpsCamMinimumShift.y, HeadBobSniperCrouchWalkValues.FpsCamMaximumShift.y, HeadBobSniperCrouchWalkValues.FpsCamYShiftDuration, HeadBobCrouchWalkValues.FpsCamYShiftDelay);
        //        ZShift(FpsCamera.gameObject, HeadBobSniperCrouchWalkValues.FpsCamMinimumShift.z, HeadBobSniperCrouchWalkValues.FpsCamMaximumShift.z, HeadBobSniperCrouchWalkValues.FpsCamZShiftDuration, HeadBobCrouchWalkValues.FpsCamZShiftDelay);
        //        ZRot(FpsCamera.gameObject, HeadBobSniperCrouchWalkValues.FpsCamMinimumZRotation, HeadBobSniperCrouchWalkValues.FpsCamMaximumZRotation, HeadBobSniperCrouchWalkValues.FpsCamZRotationDuration, HeadBobCrouchWalkValues.FpsCamZRotDelay);

        //        XShiftWeaponCam(WeaponCamera.gameObject, HeadBobSniperCrouchWalkValues.WeaponCamMinShift.x, HeadBobSniperCrouchWalkValues.WeaponCamMaxShift.x, HeadBobSniperCrouchWalkValues.WeaponCamXShiftDuration, HeadBobSniperCrouchWalkValues.WeaponCamXShiftDelay);
        //        YShiftWeaponCam(WeaponCamera.gameObject, HeadBobSniperCrouchWalkValues.WeaponCamMinShift.y, HeadBobSniperCrouchWalkValues.WeaponCamMaxShift.y, HeadBobSniperCrouchWalkValues.WeaponCamYShiftDuration, HeadBobSniperCrouchWalkValues.WeaponCamYShiftDelay);
        //        ZShiftWeaponCam(WeaponCamera.gameObject, HeadBobSniperCrouchWalkValues.WeaponCamMinShift.z, HeadBobSniperCrouchWalkValues.WeaponCamMaxShift.z, HeadBobSniperCrouchWalkValues.WeaponCamZShiftDuration, HeadBobSniperCrouchWalkValues.WeaponCamZShiftDelay);

        //        RotWeaponCam(WeaponCamera.gameObject, HeadBobSniperCrouchWalkValues.WeaponCamMinRotation, HeadBobSniperCrouchWalkValues.WeaponCamMaxRotation, HeadBobSniperCrouchWalkValues.WeaponCamRotationDuration, HeadBobSniperCrouchWalkValues.WeaponCamRotationDelay);
        //        // YRotWeaponCam(WeaponCamera.gameObject, HeadBobCrouchWalkValues.WeaponCamMinRotation.y, HeadBobCrouchWalkValues.WeaponCamMaxRotation.y, HeadBobCrouchWalkValues.WeaponCamYRotationDuration);
        //        //ZRotWeaponCam(WeaponCamera.gameObject, HeadBobCrouchWalkValues.WeaponCamMinRotation.z, HeadBobCrouchWalkValues.WeaponCamMaxRotation.z, HeadBobCrouchWalkValues.WeaponCamZRotationDuration);

        //        WalkStopLoop = true;
        //    }

        //    //PosXTimer += Time.deltaTime;
        //    //PosYTimer += Time.deltaTime;
        //    //PosZTimer += Time.deltaTime;
        //    //RotXTimer += Time.deltaTime;
        //    //RotYTimer += Time.deltaTime;
        //    //RotZTimer += Time.deltaTime;

        //    //if (PosXTimer > HeadBobCrouchWalkValues.TimeToGetNewXValue)
        //    //{
        //    //    XShift(BobObject.gameObject, HeadBobCrouchWalkValues.MinimumShift.x, HeadBobCrouchWalkValues.MaximumShift.x, HeadBobCrouchWalkValues.XShiftSpeed, BobObject.localPosition.x, HeadBobCrouchWalkValues.XShiftDelay);
        //    //    PosXTimer = 0f;
        //    //}
        //    //if (PosYTimer > HeadBobCrouchWalkValues.TimeToGetNewYValue)
        //    //{
        //    //    YShift(BobObject.gameObject, HeadBobCrouchWalkValues.MinimumShift.y, HeadBobCrouchWalkValues.MaximumShift.y, HeadBobCrouchWalkValues.YShiftSpeed, BobObject.localPosition.y, HeadBobCrouchWalkValues.YShiftDelay);
        //    //    PosYTimer = 0f;
        //    //}
        //    //if (PosZTimer > HeadBobCrouchWalkValues.TimeToGetNewXValue)
        //    //{
        //    //    ZShift(BobObject.gameObject, HeadBobCrouchWalkValues.MinimumShift.z, HeadBobCrouchWalkValues.MaximumShift.z, HeadBobCrouchWalkValues.ZShiftSpeed, BobObject.localPosition.z, HeadBobCrouchWalkValues.ZShiftDelay);
        //    //    PosZTimer = 0f;
        //    //}
        //    //if (RotZTimer > HeadBobCrouchWalkValues.TimeToGetNewZRot)
        //    //{
        //    //    ZRot(BobObject.gameObject, HeadBobCrouchWalkValues.MinimumZRotation, HeadBobCrouchWalkValues.MaximumZRotation, HeadBobCrouchWalkValues.ZRotationSpeed, BobObject.localEulerAngles.z, HeadBobCrouchWalkValues.ZRotDelay);
        //    //    RotZTimer = 0f;
        //    //}
        //}
        //public void WalkOpticalCrouchValues()
        //{
        //    if (WalkStopLoop == false)
        //    {
        //        XShift(FpsCamera.gameObject, HeadBobOpticalCrouchWalkValues.FpsCamMinimumShift.x, HeadBobOpticalCrouchWalkValues.FpsCamMaximumShift.x, HeadBobOpticalCrouchWalkValues.FpsCamXShiftDuration, HeadBobOpticalCrouchWalkValues.FpsCamXShiftDelay);
        //        YShift(FpsCamera.gameObject, HeadBobOpticalCrouchWalkValues.FpsCamMinimumShift.y, HeadBobOpticalCrouchWalkValues.FpsCamMaximumShift.y, HeadBobOpticalCrouchWalkValues.FpsCamYShiftDuration, HeadBobOpticalCrouchWalkValues.FpsCamYShiftDelay);
        //        ZShift(FpsCamera.gameObject, HeadBobOpticalCrouchWalkValues.FpsCamMinimumShift.z, HeadBobOpticalCrouchWalkValues.FpsCamMaximumShift.z, HeadBobOpticalCrouchWalkValues.FpsCamZShiftDuration, HeadBobOpticalCrouchWalkValues.FpsCamZShiftDelay);
        //        ZRot(FpsCamera.gameObject, HeadBobOpticalCrouchWalkValues.FpsCamMinimumZRotation, HeadBobOpticalCrouchWalkValues.FpsCamMaximumZRotation, HeadBobOpticalCrouchWalkValues.FpsCamZRotationDuration, HeadBobOpticalCrouchWalkValues.FpsCamZRotDelay);

        //        XShiftWeaponCam(WeaponCamera.gameObject, HeadBobOpticalCrouchWalkValues.WeaponCamMinShift.x, HeadBobOpticalCrouchWalkValues.WeaponCamMaxShift.x, HeadBobOpticalCrouchWalkValues.WeaponCamXShiftDuration, HeadBobOpticalCrouchWalkValues.WeaponCamXShiftDelay);
        //        YShiftWeaponCam(WeaponCamera.gameObject, HeadBobOpticalCrouchWalkValues.WeaponCamMinShift.y, HeadBobOpticalCrouchWalkValues.WeaponCamMaxShift.y, HeadBobOpticalCrouchWalkValues.WeaponCamYShiftDuration, HeadBobOpticalCrouchWalkValues.WeaponCamYShiftDelay);
        //        ZShiftWeaponCam(WeaponCamera.gameObject, HeadBobOpticalCrouchWalkValues.WeaponCamMinShift.z, HeadBobOpticalCrouchWalkValues.WeaponCamMaxShift.z, HeadBobOpticalCrouchWalkValues.WeaponCamZShiftDuration, HeadBobOpticalCrouchWalkValues.WeaponCamZShiftDelay);

        //        RotWeaponCam(WeaponCamera.gameObject, HeadBobOpticalCrouchWalkValues.WeaponCamMinRotation, HeadBobOpticalCrouchWalkValues.WeaponCamMaxRotation, HeadBobOpticalCrouchWalkValues.WeaponCamRotationDuration, HeadBobOpticalCrouchWalkValues.WeaponCamRotationDelay);
        //        // YRotWeaponCam(WeaponCamera.gameObject, HeadBobCrouchWalkValues.WeaponCamMinRotation.y, HeadBobCrouchWalkValues.WeaponCamMaxRotation.y, HeadBobCrouchWalkValues.WeaponCamYRotationDuration);
        //        //ZRotWeaponCam(WeaponCamera.gameObject, HeadBobCrouchWalkValues.WeaponCamMinRotation.z, HeadBobCrouchWalkValues.WeaponCamMaxRotation.z, HeadBobCrouchWalkValues.WeaponCamZRotationDuration);

        //        WalkStopLoop = true;
        //    }

        //    //PosXTimer += Time.deltaTime;
        //    //PosYTimer += Time.deltaTime;
        //    //PosZTimer += Time.deltaTime;
        //    //RotXTimer += Time.deltaTime;
        //    //RotYTimer += Time.deltaTime;
        //    //RotZTimer += Time.deltaTime;

        //    //if (PosXTimer > HeadBobCrouchWalkValues.TimeToGetNewXValue)
        //    //{
        //    //    XShift(BobObject.gameObject, HeadBobCrouchWalkValues.MinimumShift.x, HeadBobCrouchWalkValues.MaximumShift.x, HeadBobCrouchWalkValues.XShiftSpeed, BobObject.localPosition.x, HeadBobCrouchWalkValues.XShiftDelay);
        //    //    PosXTimer = 0f;
        //    //}
        //    //if (PosYTimer > HeadBobCrouchWalkValues.TimeToGetNewYValue)
        //    //{
        //    //    YShift(BobObject.gameObject, HeadBobCrouchWalkValues.MinimumShift.y, HeadBobCrouchWalkValues.MaximumShift.y, HeadBobCrouchWalkValues.YShiftSpeed, BobObject.localPosition.y, HeadBobCrouchWalkValues.YShiftDelay);
        //    //    PosYTimer = 0f;
        //    //}
        //    //if (PosZTimer > HeadBobCrouchWalkValues.TimeToGetNewXValue)
        //    //{
        //    //    ZShift(BobObject.gameObject, HeadBobCrouchWalkValues.MinimumShift.z, HeadBobCrouchWalkValues.MaximumShift.z, HeadBobCrouchWalkValues.ZShiftSpeed, BobObject.localPosition.z, HeadBobCrouchWalkValues.ZShiftDelay);
        //    //    PosZTimer = 0f;
        //    //}
        //    //if (RotZTimer > HeadBobCrouchWalkValues.TimeToGetNewZRot)
        //    //{
        //    //    ZRot(BobObject.gameObject, HeadBobCrouchWalkValues.MinimumZRotation, HeadBobCrouchWalkValues.MaximumZRotation, HeadBobCrouchWalkValues.ZRotationSpeed, BobObject.localEulerAngles.z, HeadBobCrouchWalkValues.ZRotDelay);
        //    //    RotZTimer = 0f;
        //    //}
        //}
        //public void JumpStandValues()
        //{
        //    if (JumpStopLoop == false)
        //    {
        //        XShift(FpsCamera.gameObject, HeadBobStandJumpValues.FpsCamMinimumShift.x, HeadBobStandJumpValues.FpsCamMaximumShift.x, HeadBobStandJumpValues.FpsCamXShiftDuration, HeadBobStandJumpValues.FpsCamXShiftDelay);
        //        YShift(FpsCamera.gameObject, HeadBobStandJumpValues.FpsCamMinimumShift.y, HeadBobStandJumpValues.FpsCamMaximumShift.y, HeadBobStandJumpValues.FpsCamYShiftDuration, HeadBobStandJumpValues.FpsCamYShiftDelay);
        //        ZShift(FpsCamera.gameObject, HeadBobStandJumpValues.FpsCamMinimumShift.z, HeadBobStandJumpValues.FpsCamMaximumShift.z, HeadBobStandJumpValues.FpsCamZShiftDuration, HeadBobStandJumpValues.FpsCamZShiftDelay);
        //        ZRot(FpsCamera.gameObject, HeadBobStandJumpValues.FpsCamMinimumZRotation, HeadBobStandJumpValues.FpsCamMaximumZRotation, HeadBobStandJumpValues.FpsCamZRotationDuration, HeadBobStandJumpValues.FpsCamZRotDelay);

        //        XShiftWeaponCam(WeaponCamera.gameObject, HeadBobStandJumpValues.WeaponCamMinShift.x, HeadBobStandJumpValues.WeaponCamMaxShift.x, HeadBobStandJumpValues.WeaponCamXShiftDuration, HeadBobStandJumpValues.WeaponCamXShiftDelay);
        //        YShiftWeaponCam(WeaponCamera.gameObject, HeadBobStandJumpValues.WeaponCamMinShift.y, HeadBobStandJumpValues.WeaponCamMaxShift.y, HeadBobStandJumpValues.WeaponCamYShiftDuration, HeadBobStandJumpValues.WeaponCamYShiftDelay);
        //        ZShiftWeaponCam(WeaponCamera.gameObject, HeadBobStandJumpValues.WeaponCamMinShift.z, HeadBobStandJumpValues.WeaponCamMaxShift.z, HeadBobStandJumpValues.WeaponCamZShiftDuration, HeadBobStandJumpValues.WeaponCamZShiftDelay);

        //        RotWeaponCam(WeaponCamera.gameObject, HeadBobStandJumpValues.WeaponCamMinRotation, HeadBobStandJumpValues.WeaponCamMaxRotation, HeadBobStandJumpValues.WeaponCamRotationDuration, HeadBobStandJumpValues.WeaponCamRotationDelay);
        //        JumpStopLoop = true;
        //    }
        //}
        //public void JumpCrouchValues()
        //{
        //    if (JumpStopLoop == false)
        //    {
        //        XShift(FpsCamera.gameObject, HeadBobCrouchJumpValues.FpsCamMinimumShift.x, HeadBobCrouchJumpValues.FpsCamMaximumShift.x, HeadBobCrouchJumpValues.FpsCamXShiftDuration, HeadBobCrouchJumpValues.FpsCamXShiftDelay);
        //        YShift(FpsCamera.gameObject, HeadBobCrouchJumpValues.FpsCamMinimumShift.y, HeadBobCrouchJumpValues.FpsCamMaximumShift.y, HeadBobCrouchJumpValues.FpsCamYShiftDuration, HeadBobCrouchJumpValues.FpsCamYShiftDelay);
        //        ZShift(FpsCamera.gameObject, HeadBobCrouchJumpValues.FpsCamMinimumShift.z, HeadBobCrouchJumpValues.FpsCamMaximumShift.z, HeadBobCrouchJumpValues.FpsCamZShiftDuration, HeadBobCrouchJumpValues.FpsCamZShiftDelay);
        //        ZRot(FpsCamera.gameObject, HeadBobCrouchJumpValues.FpsCamMinimumZRotation, HeadBobCrouchJumpValues.FpsCamMaximumZRotation, HeadBobCrouchJumpValues.FpsCamZRotationDuration, HeadBobCrouchJumpValues.FpsCamZRotDelay);

        //        XShiftWeaponCam(WeaponCamera.gameObject, HeadBobCrouchJumpValues.WeaponCamMinShift.x, HeadBobCrouchJumpValues.WeaponCamMaxShift.x, HeadBobCrouchJumpValues.WeaponCamXShiftDuration, HeadBobCrouchJumpValues.WeaponCamXShiftDelay);
        //        YShiftWeaponCam(WeaponCamera.gameObject, HeadBobCrouchJumpValues.WeaponCamMinShift.y, HeadBobCrouchJumpValues.WeaponCamMaxShift.y, HeadBobCrouchJumpValues.WeaponCamYShiftDuration, HeadBobCrouchJumpValues.WeaponCamYShiftDelay);
        //        ZShiftWeaponCam(WeaponCamera.gameObject, HeadBobCrouchJumpValues.WeaponCamMinShift.z, HeadBobCrouchJumpValues.WeaponCamMaxShift.z, HeadBobCrouchJumpValues.WeaponCamZShiftDuration, HeadBobCrouchJumpValues.WeaponCamZShiftDelay);

        //        RotWeaponCam(WeaponCamera.gameObject, HeadBobCrouchJumpValues.WeaponCamMinRotation, HeadBobCrouchJumpValues.WeaponCamMaxRotation, HeadBobCrouchJumpValues.WeaponCamRotationDuration, HeadBobCrouchJumpValues.WeaponCamRotationDelay);
        //        JumpStopLoop = true;
        //    }
        //}
        //public void WalkAutoResetFunction()
        //{
        //    if (WalkStopLoop == true)
        //    {
        //        if (WalkAutoReset == false)
        //        {
        //            LeanTween.cancel(FpsCamera.gameObject);
        //            LeanTween.cancel(WeaponCamera.gameObject);


        //            if (ob != null)
        //            {
        //                if (ob.IsAimed == false)
        //                {
        //                    if(Crouch.instance.IsCrouching == false)
        //                    {
        //                        LeanTween.moveLocalX(FpsCamera.gameObject, BobDefaultPos.x, FpsCameraResetDurations.StandWalkResetDuration).setFrom(FpsCamera.transform.localPosition.x);
        //                        LeanTween.moveLocalY(FpsCamera.gameObject, BobDefaultPos.y, FpsCameraResetDurations.StandWalkResetDuration).setFrom(FpsCamera.transform.localPosition.y);
        //                        LeanTween.moveLocalZ(FpsCamera.gameObject, BobDefaultPos.z, FpsCameraResetDurations.StandWalkResetDuration).setFrom(FpsCamera.transform.localPosition.z);

        //                        //LeanTween.rotateX(BobObject.gameObject, BobDefaultRot.x, ResetDuration).setFrom(BobObject.transform.localEulerAngles.x);
        //                        //LeanTween.rotateY(BobObject.gameObject, BobDefaultRot.y, ResetDuration).setFrom(BobObject.transform.localEulerAngles.y);
        //                        LeanTween.rotateZ(FpsCamera.gameObject, BobDefaultRot.z, FpsCameraResetDurations.StandWalkResetDuration).setFrom(FpsCamera.transform.localEulerAngles.z);

        //                        LeanTween.moveLocal(WeaponCamera.gameObject, WeaponCamDefaultPos, WeaponCameraResetDurations.StandWalkResetDuration).setFrom(WeaponCamera.transform.localPosition);
        //                        LeanTween.rotateLocal(WeaponCamera.gameObject, WeaponCamDefaultRot, WeaponCameraResetDurations.StandWalkResetDuration).setFrom(WeaponCamera.transform.localEulerAngles);
        //                    }
        //                    else
        //                    {
        //                        LeanTween.moveLocalX(FpsCamera.gameObject, BobDefaultPos.x, FpsCameraResetDurations.CrouchWalkResetDuration).setFrom(FpsCamera.transform.localPosition.x);
        //                        LeanTween.moveLocalY(FpsCamera.gameObject, BobDefaultPos.y, FpsCameraResetDurations.CrouchWalkResetDuration).setFrom(FpsCamera.transform.localPosition.y);
        //                        LeanTween.moveLocalZ(FpsCamera.gameObject, BobDefaultPos.z, FpsCameraResetDurations.CrouchWalkResetDuration).setFrom(FpsCamera.transform.localPosition.z);

        //                        //LeanTween.rotateX(BobObject.gameObject, BobDefaultRot.x, ResetDuration).setFrom(BobObject.transform.localEulerAngles.x);
        //                        //LeanTween.rotateY(BobObject.gameObject, BobDefaultRot.y, ResetDuration).setFrom(BobObject.transform.localEulerAngles.y);
        //                        LeanTween.rotateZ(FpsCamera.gameObject, BobDefaultRot.z, FpsCameraResetDurations.CrouchWalkResetDuration).setFrom(FpsCamera.transform.localEulerAngles.z);

        //                        LeanTween.moveLocal(WeaponCamera.gameObject, WeaponCamDefaultPos, WeaponCameraResetDurations.CrouchWalkResetDuration).setFrom(WeaponCamera.transform.localPosition);
        //                        LeanTween.rotateLocal(WeaponCamera.gameObject, WeaponCamDefaultRot, WeaponCameraResetDurations.CrouchWalkResetDuration).setFrom(WeaponCamera.transform.localEulerAngles);
        //                    }

        //                }
        //                else
        //                {
        //                    if (Crouch.instance.IsCrouching == false)
        //                    {
        //                        LeanTween.moveLocalX(FpsCamera.gameObject, BobDefaultPos.x, FpsCameraResetDurations.StandAimedResetDuration).setFrom(FpsCamera.transform.localPosition.x);
        //                        LeanTween.moveLocalY(FpsCamera.gameObject, BobDefaultPos.y, FpsCameraResetDurations.StandAimedResetDuration).setFrom(FpsCamera.transform.localPosition.y);
        //                        LeanTween.moveLocalZ(FpsCamera.gameObject, BobDefaultPos.z, FpsCameraResetDurations.StandAimedResetDuration).setFrom(FpsCamera.transform.localPosition.z);

        //                        //LeanTween.rotateX(BobObject.gameObject, BobDefaultRot.x, ResetDuration).setFrom(BobObject.transform.localEulerAngles.x);
        //                        //LeanTween.rotateY(BobObject.gameObject, BobDefaultRot.y, ResetDuration).setFrom(BobObject.transform.localEulerAngles.y);
        //                        LeanTween.rotateZ(FpsCamera.gameObject, BobDefaultRot.z, FpsCameraResetDurations.StandAimedResetDuration).setFrom(FpsCamera.transform.localEulerAngles.z);

        //                        LeanTween.moveLocal(WeaponCamera.gameObject, new Vector3(WeaponCamDefaultPos.x, WeaponCamDefaultPos.y, ob.WeaponZAxisOnAim), WeaponCameraResetDurations.StandAimedResetDuration).setFrom(WeaponCamera.transform.localPosition);
        //                        LeanTween.rotateLocal(WeaponCamera.gameObject, WeaponCamDefaultRot, WeaponCameraResetDurations.StandAimedResetDuration).setFrom(WeaponCamera.transform.localEulerAngles);

        //                    }
        //                    else
        //                    {
        //                        LeanTween.moveLocalX(FpsCamera.gameObject, BobDefaultPos.x, FpsCameraResetDurations.CrouchAimedResetDuration).setFrom(FpsCamera.transform.localPosition.x);
        //                        LeanTween.moveLocalY(FpsCamera.gameObject, BobDefaultPos.y, FpsCameraResetDurations.CrouchAimedResetDuration).setFrom(FpsCamera.transform.localPosition.y);
        //                        LeanTween.moveLocalZ(FpsCamera.gameObject, BobDefaultPos.z, FpsCameraResetDurations.CrouchAimedResetDuration).setFrom(FpsCamera.transform.localPosition.z);

        //                        //LeanTween.rotateX(BobObject.gameObject, BobDefaultRot.x, ResetDuration).setFrom(BobObject.transform.localEulerAngles.x);
        //                        //LeanTween.rotateY(BobObject.gameObject, BobDefaultRot.y, ResetDuration).setFrom(BobObject.transform.localEulerAngles.y);
        //                        LeanTween.rotateZ(FpsCamera.gameObject, BobDefaultRot.z, FpsCameraResetDurations.StandAimedResetDuration).setFrom(FpsCamera.transform.localEulerAngles.z);

        //                        LeanTween.moveLocal(WeaponCamera.gameObject, new Vector3(WeaponCamDefaultPos.x, WeaponCamDefaultPos.y, ob.WeaponZAxisOnAim), WeaponCameraResetDurations.CrouchAimedResetDuration).setFrom(WeaponCamera.transform.localPosition);
        //                        LeanTween.rotateLocal(WeaponCamera.gameObject, WeaponCamDefaultRot, WeaponCameraResetDurations.CrouchAimedResetDuration).setFrom(WeaponCamera.transform.localEulerAngles);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (Crouch.instance.IsCrouching == false)
        //                {
        //                    LeanTween.moveLocalX(FpsCamera.gameObject, BobDefaultPos.x, FpsCameraResetDurations.StandWalkResetDuration).setFrom(FpsCamera.transform.localPosition.x);
        //                    LeanTween.moveLocalY(FpsCamera.gameObject, BobDefaultPos.y, FpsCameraResetDurations.StandWalkResetDuration).setFrom(FpsCamera.transform.localPosition.y);
        //                    LeanTween.moveLocalZ(FpsCamera.gameObject, BobDefaultPos.z, FpsCameraResetDurations.StandWalkResetDuration).setFrom(FpsCamera.transform.localPosition.z);

        //                    //LeanTween.rotateX(BobObject.gameObject, BobDefaultRot.x, ResetDuration).setFrom(BobObject.transform.localEulerAngles.x);
        //                    //LeanTween.rotateY(BobObject.gameObject, BobDefaultRot.y, ResetDuration).setFrom(BobObject.transform.localEulerAngles.y);
        //                    LeanTween.rotateZ(FpsCamera.gameObject, BobDefaultRot.z, FpsCameraResetDurations.StandWalkResetDuration).setFrom(FpsCamera.transform.localEulerAngles.z);

        //                    LeanTween.moveLocal(WeaponCamera.gameObject, WeaponCamDefaultPos, WeaponCameraResetDurations.StandWalkResetDuration).setFrom(WeaponCamera.transform.localPosition);
        //                    LeanTween.rotateLocal(WeaponCamera.gameObject, WeaponCamDefaultRot, WeaponCameraResetDurations.StandWalkResetDuration).setFrom(WeaponCamera.transform.localEulerAngles);
        //                }
        //                else
        //                {
        //                    LeanTween.moveLocalX(FpsCamera.gameObject, BobDefaultPos.x, FpsCameraResetDurations.CrouchWalkResetDuration).setFrom(FpsCamera.transform.localPosition.x);
        //                    LeanTween.moveLocalY(FpsCamera.gameObject, BobDefaultPos.y, FpsCameraResetDurations.CrouchWalkResetDuration).setFrom(FpsCamera.transform.localPosition.y);
        //                    LeanTween.moveLocalZ(FpsCamera.gameObject, BobDefaultPos.z, FpsCameraResetDurations.CrouchWalkResetDuration).setFrom(FpsCamera.transform.localPosition.z);

        //                    //LeanTween.rotateX(BobObject.gameObject, BobDefaultRot.x, ResetDuration).setFrom(BobObject.transform.localEulerAngles.x);
        //                    //LeanTween.rotateY(BobObject.gameObject, BobDefaultRot.y, ResetDuration).setFrom(BobObject.transform.localEulerAngles.y);
        //                    LeanTween.rotateZ(FpsCamera.gameObject, BobDefaultRot.z, FpsCameraResetDurations.CrouchWalkResetDuration).setFrom(FpsCamera.transform.localEulerAngles.z);

        //                    LeanTween.moveLocal(WeaponCamera.gameObject, WeaponCamDefaultPos, WeaponCameraResetDurations.CrouchWalkResetDuration).setFrom(WeaponCamera.transform.localPosition);
        //                    LeanTween.rotateLocal(WeaponCamera.gameObject, WeaponCamDefaultRot, WeaponCameraResetDurations.CrouchWalkResetDuration).setFrom(WeaponCamera.transform.localEulerAngles);
        //                }
        //            }


        //            WalkAutoReset = true;
        //        }
        //    }
        //}
        //public void JumpAutoResetFunction()
        //{
        //    if (JumpStopLoop == true)
        //    {
        //        if (JumpAutoReset == false)
        //        {
        //            if(Crouch.instance.IsCrouching == false)
        //            {
        //                LeanTween.cancel(FpsCamera.gameObject);
        //                LeanTween.cancel(WeaponCamera.gameObject);

        //                LeanTween.moveLocalX(FpsCamera.gameObject, BobDefaultPos.x, FpsCameraResetDurations.StandJumpResetDuration).setFrom(FpsCamera.transform.localPosition.x);
        //                LeanTween.moveLocalY(FpsCamera.gameObject, BobDefaultPos.y, FpsCameraResetDurations.StandJumpResetDuration).setFrom(FpsCamera.transform.localPosition.y);
        //                LeanTween.moveLocalZ(FpsCamera.gameObject, BobDefaultPos.z, FpsCameraResetDurations.StandJumpResetDuration).setFrom(FpsCamera.transform.localPosition.z);

        //                //LeanTween.rotateX(BobObject.gameObject, BobDefaultRot.x, ResetDuration).setFrom(BobObject.transform.localEulerAngles.x);
        //                //LeanTween.rotateY(BobObject.gameObject, BobDefaultRot.y, ResetDuration).setFrom(BobObject.transform.localEulerAngles.y);
        //                LeanTween.rotateZ(FpsCamera.gameObject, BobDefaultRot.z, FpsCameraResetDurations.StandJumpResetDuration).setFrom(FpsCamera.transform.localEulerAngles.z);
        //            }
        //            else
        //            {
        //                LeanTween.cancel(FpsCamera.gameObject);
        //                LeanTween.cancel(WeaponCamera.gameObject);

        //                LeanTween.moveLocalX(FpsCamera.gameObject, BobDefaultPos.x, FpsCameraResetDurations.CrouchJumpResetDuration).setFrom(FpsCamera.transform.localPosition.x);
        //                LeanTween.moveLocalY(FpsCamera.gameObject, BobDefaultPos.y, FpsCameraResetDurations.CrouchJumpResetDuration).setFrom(FpsCamera.transform.localPosition.y);
        //                LeanTween.moveLocalZ(FpsCamera.gameObject, BobDefaultPos.z, FpsCameraResetDurations.CrouchJumpResetDuration).setFrom(FpsCamera.transform.localPosition.z);

        //                //LeanTween.rotateX(BobObject.gameObject, BobDefaultRot.x, ResetDuration).setFrom(BobObject.transform.localEulerAngles.x);
        //                //LeanTween.rotateY(BobObject.gameObject, BobDefaultRot.y, ResetDuration).setFrom(BobObject.transform.localEulerAngles.y);
        //                LeanTween.rotateZ(FpsCamera.gameObject, BobDefaultRot.z, FpsCameraResetDurations.CrouchJumpResetDuration).setFrom(FpsCamera.transform.localEulerAngles.z);
        //            }


        //            if (ob != null)
        //            {
        //                if (ob.IsAimed == false)
        //                {
        //                    if (Crouch.instance.IsCrouching == false)
        //                    {
        //                        LeanTween.moveLocal(WeaponCamera.gameObject, WeaponCamDefaultPos, WeaponCameraResetDurations.StandJumpResetDuration).setFrom(WeaponCamera.transform.localPosition);
        //                        LeanTween.rotateLocal(WeaponCamera.gameObject, WeaponCamDefaultRot, WeaponCameraResetDurations.StandJumpResetDuration).setFrom(WeaponCamera.transform.localEulerAngles);
        //                    }
        //                    else
        //                    {
        //                        LeanTween.moveLocal(WeaponCamera.gameObject, WeaponCamDefaultPos, WeaponCameraResetDurations.CrouchJumpResetDuration).setFrom(WeaponCamera.transform.localPosition);
        //                        LeanTween.rotateLocal(WeaponCamera.gameObject, WeaponCamDefaultRot, WeaponCameraResetDurations.CrouchJumpResetDuration).setFrom(WeaponCamera.transform.localEulerAngles);
        //                    }
        //                }
        //                else
        //                {
        //                    if (Crouch.instance.IsCrouching == false)
        //                    {
        //                        LeanTween.moveLocal(WeaponCamera.gameObject, new Vector3(WeaponCamDefaultPos.x, WeaponCamDefaultPos.y, ob.WeaponZAxisOnAim), WeaponCameraResetDurations.StandAimedResetDuration).setFrom(WeaponCamera.transform.localPosition);
        //                        LeanTween.rotateLocal(WeaponCamera.gameObject, WeaponCamDefaultRot, WeaponCameraResetDurations.StandAimedResetDuration).setFrom(WeaponCamera.transform.localEulerAngles);
        //                    }
        //                    else
        //                    {
        //                        LeanTween.moveLocal(WeaponCamera.gameObject, new Vector3(WeaponCamDefaultPos.x, WeaponCamDefaultPos.y, ob.WeaponZAxisOnAim), WeaponCameraResetDurations.CrouchAimedResetDuration).setFrom(WeaponCamera.transform.localPosition);
        //                        LeanTween.rotateLocal(WeaponCamera.gameObject, WeaponCamDefaultRot, WeaponCameraResetDurations.CrouchAimedResetDuration).setFrom(WeaponCamera.transform.localEulerAngles);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (Crouch.instance.IsCrouching == false)
        //                {
        //                    LeanTween.moveLocal(WeaponCamera.gameObject, WeaponCamDefaultPos, WeaponCameraResetDurations.StandJumpResetDuration).setFrom(WeaponCamera.transform.localPosition);
        //                    LeanTween.rotateLocal(WeaponCamera.gameObject, WeaponCamDefaultRot, WeaponCameraResetDurations.StandJumpResetDuration).setFrom(WeaponCamera.transform.localEulerAngles);
        //                }
        //                else
        //                {
        //                    LeanTween.moveLocal(WeaponCamera.gameObject, WeaponCamDefaultPos, WeaponCameraResetDurations.CrouchJumpResetDuration).setFrom(WeaponCamera.transform.localPosition);
        //                    LeanTween.rotateLocal(WeaponCamera.gameObject, WeaponCamDefaultRot, WeaponCameraResetDurations.CrouchJumpResetDuration).setFrom(WeaponCamera.transform.localEulerAngles);
        //                }
        //            }


        //            JumpAutoReset = true;
        //        }
        //    }
        //}
    }
}