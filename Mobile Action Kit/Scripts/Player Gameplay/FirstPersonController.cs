using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MobileActionKit
{
    [RequireComponent(typeof(PlayerHealth), typeof(Crouch))]
    public class FirstPersonController : MonoBehaviour
    {
        public static FirstPersonController instance;

        [TextArea]
        public string ScriptInfo = "This Script Controls The Player Walk,Sprint,Jump and TouchPad Behaviour";
        [Space(10)]

        [Header("SHOULD BE UNCHECKED FOR MOBILE")]
        public bool UseCustomLookScripts = false;
        [Space(10)]

        public Camera PlayerCamera;
        public JoyStick JoystickScript;
        public TouchPadLook TouchpadLook = new TouchPadLook();
        public WeaponTouch WeaponAimedTouchPadLookValues;

        //public string[] TagsToDiscard;

        [System.Serializable]
        public class Speed
        {
            public float WalkingSpeed = 3f;
            public float CrouchingSpeed = 1f;
            public float AimedWalkingSpeed = 3f;
            public float AimedCrouchingSpeed = 1f;
            [Tooltip("Run Speed While the Player is Standing")]
            public float RunSpeedOnStanding = 6f;
            [Tooltip("Run Speed While the Player is Crouched")]
            public float RunSpeedOnCrouching = 2f;

            public float PlayerFallSpeed = 10f;
        }

        public Speed Speeds;

        [System.Serializable]
        public class WalkRunSounds
        {
            public AlertingSoundActivator StandRunStepsSoundTrigger;
            public AlertingSoundActivator  CrouchRunStepsSoundTrigger;

            public AlertingSoundActivator AimedStandWalkStepsSoundTrigger;
            public AlertingSoundActivator AimedCrouchWalkStepsSoundTrigger;

            public AlertingSoundActivator StandWalkStepsSoundTrigger;
            public AlertingSoundActivator CrouchWalkStepsSoundTrigger;

            public AudioSource[] WalkingSounds;
            public AudioSource[] RunningSounds;

            public float TimeBetweenStandRunSteps = 0.4f;
            public float TimeBetweenCrouchRunSteps = 0.8f;

            public float TimeBetweenStandWalkSteps = 0.6f;
            public float TimeBetweenCrouchWalkSteps = 0.8f;
            public float TimeBetweenAimedStandSteps = 1f;
            public float TimeBetweenAimedCrouchSteps = 1.2f;
        }

        public WalkRunSounds WalkAndRunSounds;

        [System.Serializable]
        public class JumpingClass
        {
            public Button JumpButton;
            public float JumpButtonActivationDelay = 0.3f;
            [Tooltip("Till What Height The Player is going to Jump From Current Axis Y")]
            public float StandJumpingMeters = 2f;

            [HideInInspector]
            public float StandHeight;

            [Tooltip("How Fast will the Player going to reach the jumping height")]
            public float StandJumpingDuration = 0.4f;
            [Tooltip("How Fast will the Player going to reach the ground")]
            public float StandFallingDuration = 0.4f;

            [Tooltip("Till What Height The Player is going to Jump From Current Axis Y")]
            public float CrouchJumpingMeters = 1.5f;

            [HideInInspector]
            public float CrouchHeight;

            [Tooltip("How Fast will the Player going to reach the jumping height")]
            public float CrouchJumpingDuration = 0.1f;
            [Tooltip("How Fast will the Player going to reach the ground")]
            public float CrouchFallingDuration = 0.1f;

            public AudioSource JumpStartSound;
            public AudioSource JumpLandingSound;
        }

        public JumpingClass Jump;



        [System.Serializable]
        public class WeaponTouch
        {
            public float XSensitivity = 0.01f;
            public float YSensitivity = 0.01f;
            public bool Smooth = true;
            public float SmoothTime = 15f;
        }

        Transform myTransform, cameraTransform;
        float rotation;
        [HideInInspector]
        public bool jump;
        bool prevGrounded;
        float weapReadyTime;
        bool weapReady = true;

        bool CanPlayJumpSounds = false;
        bool CheckjumpLanding = false;

        private Rigidbody Rb;

        //public Transform ObjectToShake;

        //[Header("Head Bobbing Values on Stand Walk")]
        //[Range(0, 100)]
        //public float HeadBobbingStandSpeed = 0.3f;
        //[Range(0, 100)]
        //public float HeadBobbingStandDistance = 0.1f;

        //[Header("Head Bobbing Values on Crouch Walk")]
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
        //public float MinimumX = 0f;
        //public float MinimumY = 0.6f;

        //[HideInInspector]
        //public float MinimumValueToIncrease = 0f;

        //public float MaximumValueToIncrease = 0.5f;

        [HideInInspector]
        public float DefaultXSenstivity;
        [HideInInspector]
        public float DefaultYSenstivity;

        bool Smooth;
        float smoothtime;

        bool InitialiseLook = false;

        [HideInInspector]
        public bool GroundCheck = true;

        float Multiply = 5f;
        [HideInInspector]
        public float Timer = 1000f;

        [HideInInspector]
        public bool LoopHeadBob = false;

        [HideInInspector]
        public bool LoopJumpHeadBob = false;

        bool HeadBobJumpReset = false;
        bool HeadBobWalkReset = false;

        bool Jumping = false;

        float DefaultYPos;

        float MovingSpeed;

        [HideInInspector]
        public bool IsJumpDelayFinished = false;

        bool IsFallingCompleted = false;

        bool IsStandAimed = false;
        bool IsCrouchAimed = false;
        bool IsLeanTweenCancelled = false;  

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            myTransform = transform;
            cameraTransform = Camera.main.transform;
            DefaultLook();
        }
        private void Start()
        {
            TouchpadLook.Init(transform, PlayerCamera.transform);
            Rb = GetComponent<Rigidbody>();
        }
        public void DefaultLook()
        {
            DefaultXSenstivity = TouchpadLook.XSensitivity;
            DefaultYSenstivity = TouchpadLook.YSensitivity;
            Smooth = TouchpadLook.smooth;
            smoothtime = TouchpadLook.smoothTime;
            InitialiseLook = true;
        }
        void OnCollisionStay(Collision collision)
        {
            //for(int x = 0; x < TagsToDiscard.Length;x++ )
            //{
            //    if(collis)
            //}
            GroundCheck = true;

        }
        void OnCollisionExit()
        {
            GroundCheck = false;

        }
        private void RotateView()
        {
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            if (PlayerManager.instance != null)
            {
                if (PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                {
                    if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                    {
                        if (InitialiseLook == true)
                        {
                            TouchpadLook.XSensitivity = WeaponAimedTouchPadLookValues.XSensitivity;
                            TouchpadLook.YSensitivity = WeaponAimedTouchPadLookValues.YSensitivity;
                            TouchpadLook.smooth = WeaponAimedTouchPadLookValues.Smooth;
                            TouchpadLook.smoothTime = WeaponAimedTouchPadLookValues.SmoothTime;
                        }
                    }
                    else
                    {
                        TouchpadLook.XSensitivity = DefaultXSenstivity;
                        TouchpadLook.YSensitivity = DefaultYSenstivity;
                        TouchpadLook.smooth = Smooth;
                        TouchpadLook.smoothTime = smoothtime;
                    }
                    TouchpadLook.LookRotation(transform, PlayerCamera.transform);
                }
                else
                {
                    TouchpadLook.XSensitivity = DefaultXSenstivity;
                    TouchpadLook.YSensitivity = DefaultYSenstivity;
                    TouchpadLook.smooth = Smooth;
                    TouchpadLook.smoothTime = smoothtime;

                    TouchpadLook.LookRotation(transform, PlayerCamera.transform);
                }

            }

        }
        void Update()
        {
            if (jump == false && GroundCheck == false)
            {
                Rb.AddForce(Physics.gravity * Speeds.PlayerFallSpeed * Time.deltaTime * 20f, ForceMode.Acceleration);
            }
            if (TouchpadLook.EnableMouseClickAndTouchInput == true && UseCustomLookScripts == false)
            {
                RotateView();
            }
            if (weapReady == false)
            {
                weapReadyTime += Time.deltaTime;
                if (weapReadyTime > .15f)
                {
                    weapReady = true;
                    weapReadyTime = 0f;
                }
            }
        }
        //private Vector2 GetInput()
        //{
        //    Vector2 input = new Vector2
        //    {
        //        x = CrossPlatformInputManager.GetAxis("Horizontal"),
        //        y = CrossPlatformInputManager.GetAxis("Vertical")
        //    };
        //    return input;
        //}
        void FixedUpdate()
        {
            // Vector2 input = GetInput();
            // Vector3 desiredMove = FpsCamera.transform.forward * input.y + FpsCamera.transform.right * input.x;



            if (JoystickScript != null)
            {
                PlayerMovement(JoystickScript.Horizontal(), JoystickScript.Vertical());
            }

            //if(PlayerManager.instance != null)
            //{
            //    if (JoystickScript.IsWalking == false && PlayerManager.instance.IsMoving == false)
            //    {
            //        //Vector3 temp = ObjectToShake.localPosition;
            //        //temp.x = ObjectToShakeDefaultXPosition;
            //        //temp.y = ObjectToShakeDefaultYPosition;
            //        //ObjectToShake.localPosition = temp;

            //        //Vector3 tempo = ObjectToShake.localEulerAngles;
            //        //tempo.z = ObjectToShakeDefaultZRotation;
            //        //ObjectToShake.localEulerAngles = tempo;
            //    }
            //}


        }
        public void NormalStandBobbing()
        {
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

            if (LoopHeadBob == false)
            {
                if (PlayerManager.instance != null)
                {
                    if (PlayerManager.instance.AllBobbingScripts.Length >= 1)
                    {
                        if (PlayerManager.instance.AllBobbingScripts[0] != null)
                        {
                            for (int x = 0; x < PlayerManager.instance.AllBobbingScripts.Length; x++)
                            {
                                if (PlayerManager.instance.AllBobbingScripts[x].gameObject.activeInHierarchy == true)
                                {
                                    if (PlayerManager.instance.AllBobbingScripts[x].WalkStartLoop == false)
                                    {
                                        PlayerManager.instance.AllBobbingScripts[x].WalkStopLoop = false;
                                        PlayerManager.instance.AllBobbingScripts[x].WalkAutoReset = false;
                                        PlayerManager.instance.AllBobbingScripts[x].WalkStartLoop = true;
                                    }

                                   
                                    PlayerManager.instance.AllBobbingScripts[x].WalkStandValues();
                                }


                            }
                        }
                    }
                }
                HeadBobWalkReset = false;
                //LoopHeadBob = true;
            }
            
        }
        //public void NormalSniperStandBobbing()
        //{
        //    if (LoopHeadBob == false)
        //    {
        //        if (PlayerManager.instance != null)
        //        {
        //            if (PlayerManager.instance.BobbingScripts[0] != null)
        //            {
        //                for (int x = 0; x < PlayerManager.instance.BobbingScripts.Length; x++)
        //                {
        //                    if (PlayerManager.instance.BobbingScripts[x].WalkStartLoop == false)
        //                    {
        //                        PlayerManager.instance.BobbingScripts[x].WalkStopLoop = false;
        //                        PlayerManager.instance.BobbingScripts[x].WalkAutoReset = false;
        //                        PlayerManager.instance.BobbingScripts[x].WalkStartLoop = true;
        //                    }
        //                  //  PlayerManager.instance.BobbingScripts[x].WalkSniperStandValues();
        //                }
        //            }
        //        }
        //        HeadBobWalkReset = false;
        //        LoopHeadBob = true;
        //    }
        //}
        public void NormalOpticalStandBobbing()
        {
            if (LoopHeadBob == false)
            {
                if (PlayerManager.instance != null)
                {
                    if (PlayerManager.instance.AllBobbingScripts.Length >= 1)
                    {
                        if (PlayerManager.instance.AllBobbingScripts[0] != null)
                        {
                            for (int x = 0; x < PlayerManager.instance.AllBobbingScripts.Length; x++)
                            {
                                if (PlayerManager.instance.AllBobbingScripts[x].gameObject.activeInHierarchy == true)
                                {
                                    if (PlayerManager.instance.AllBobbingScripts[x].WalkStartLoop == false)
                                    {
                                        PlayerManager.instance.AllBobbingScripts[x].WalkStopLoop = false;
                                        PlayerManager.instance.AllBobbingScripts[x].WalkAutoReset = false;
                                        PlayerManager.instance.AllBobbingScripts[x].WalkStartLoop = true;
                                    }

                                    PlayerManager.instance.AllBobbingScripts[x].WalkOpticalStandValues();
                                }

                            }
                        }
                    }
                }
                HeadBobWalkReset = false;
                // LoopHeadBob = true;
            }
        }
        public void NormalCrouchWalking()
        {
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

            if (LoopHeadBob == false)
            {
                if (PlayerManager.instance != null)
                {
                    if (PlayerManager.instance.AllBobbingScripts.Length >= 1)
                    {
                        if (PlayerManager.instance.AllBobbingScripts[0] != null)
                        {
                            for (int x = 0; x < PlayerManager.instance.AllBobbingScripts.Length; x++)
                            {
                                if (PlayerManager.instance.AllBobbingScripts[x].gameObject.activeInHierarchy == true)
                                {
                                    if (PlayerManager.instance.AllBobbingScripts[x].WalkStartLoop == false)
                                    {
                                        PlayerManager.instance.AllBobbingScripts[x].WalkStopLoop = false;
                                        PlayerManager.instance.AllBobbingScripts[x].WalkAutoReset = false;
                                        PlayerManager.instance.AllBobbingScripts[x].WalkStartLoop = true;
                                    }
                                    PlayerManager.instance.AllBobbingScripts[x].WalkCrouchValues();
                                }
                            }
                        }
                    }
                }
                HeadBobWalkReset = false;
                //  LoopHeadBob = true;
            }
        }
        public void NormalOpticalCrouchWalking()
        {
            if (LoopHeadBob == false)
            {
                if (PlayerManager.instance != null)
                {
                    if (PlayerManager.instance.AllBobbingScripts.Length >= 1)
                    {
                        if (PlayerManager.instance.AllBobbingScripts[0] != null)
                        {
                            for (int x = 0; x < PlayerManager.instance.AllBobbingScripts.Length; x++)
                            {
                                if (PlayerManager.instance.AllBobbingScripts[x].gameObject.activeInHierarchy == true)
                                {
                                    if (PlayerManager.instance.AllBobbingScripts[x].WalkStartLoop == false)
                                    {
                                        PlayerManager.instance.AllBobbingScripts[x].WalkStopLoop = false;
                                        PlayerManager.instance.AllBobbingScripts[x].WalkAutoReset = false;
                                        PlayerManager.instance.AllBobbingScripts[x].WalkStartLoop = true;
                                    }
                                    PlayerManager.instance.AllBobbingScripts[x].WalkOpticalCrouchValues();
                                }
                            }
                        }
                    }
                }
                HeadBobWalkReset = false;
                //  LoopHeadBob = true;
            }
        }
        //public void NormalSniperCrouchWalking()
        //{
        //    if (LoopHeadBob == false)
        //    {
        //        if (PlayerManager.instance != null)
        //        {
        //            if (PlayerManager.instance.BobbingScripts[0] != null)
        //            {
        //                for (int x = 0; x < PlayerManager.instance.BobbingScripts.Length; x++)
        //                {
        //                    if (PlayerManager.instance.BobbingScripts[x].WalkStartLoop == false)
        //                    {
        //                        PlayerManager.instance.BobbingScripts[x].WalkStopLoop = false;
        //                        PlayerManager.instance.BobbingScripts[x].WalkAutoReset = false;
        //                        PlayerManager.instance.BobbingScripts[x].WalkStartLoop = true;
        //                    }
        //                  //  PlayerManager.instance.BobbingScripts[x].WalkSniperCrouchValues();
        //                }
        //            }
        //        }
        //        HeadBobWalkReset = false;
        //        LoopHeadBob = true;
        //    }       
        //}
        // Jumping
        public void PlayerJump()
        {
            if(IsJumpDelayFinished == false)
            {
                if (PlayerManager.instance != null)
                {
                    if (PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                    {
                        if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                        {
                            PlayerManager.instance.Aiming();
                        }
                    }
                }
                if (LoopJumpHeadBob == false)
                {
                    if (PlayerManager.instance != null)
                    {
                        if (PlayerManager.instance.AllBobbingScripts.Length >= 1)
                        {
                            if (PlayerManager.instance.AllBobbingScripts[0] != null)
                            {
                                for (int x = 0; x < PlayerManager.instance.AllBobbingScripts.Length; x++)
                                {
                                    //Debug.Log(PlayerManager.instance.AllBobbingScripts[x].gameObject.name);
                                    if (PlayerManager.instance.AllBobbingScripts[x].gameObject.activeInHierarchy == true)
                                    {
                                        if (PlayerManager.instance.AllBobbingScripts[x].JumpStartLoop == false)
                                        {
                                            PlayerManager.instance.AllBobbingScripts[x].JumpStopLoop = false;
                                            PlayerManager.instance.AllBobbingScripts[x].JumpAutoReset = false;
                                            PlayerManager.instance.AllBobbingScripts[x].JumpStartLoop = true;
                                        }

                                        if (Crouch.instance != null)
                                        {
                                            if (Crouch.instance.IsCrouching == false)
                                            {
                                                PlayerManager.instance.AllBobbingScripts[x].JumpStandValues();
                                            }
                                            else
                                            {
                                                PlayerManager.instance.AllBobbingScripts[x].JumpCrouchValues();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    HeadBobJumpReset = false;
                    LoopJumpHeadBob = true;
                }


                Jump.StandHeight = transform.position.y + Jump.StandJumpingMeters;
                Jump.CrouchHeight = transform.position.y + Jump.CrouchJumpingMeters;

                DefaultYPos = transform.position.y;
                Rb.isKinematic = true;
                IsJumpDelayFinished = true;
                Jump.JumpButton.interactable = false;
                CanPlayJumpSounds = false;
                if (GroundCheck)
                    jump = true;
            }
           
        }
        public void StandRunning()
        {
            Timer += Time.deltaTime;
            if (Timer > WalkAndRunSounds.TimeBetweenStandRunSteps)
            {
                for (int aud = 0; aud < WalkAndRunSounds.RunningSounds.Length; aud++)
                {
                    WalkAndRunSounds.RunningSounds[aud].Play();
                }

                Timer = 0;
            }
            WalkAndRunSounds.StandRunStepsSoundTrigger.ActivateNoiseHandler(transform);

            // transform.Translate(Vector3.forward * Speeds.RunSpeedOnStanding * Time.deltaTime);
            //   Rb.velocity = Vector3.forward * Speeds.RunSpeedOnStanding * Time.deltaTime * 10f;
            transform.Translate(Vector3.forward * Speeds.RunSpeedOnStanding * Time.deltaTime / 10);
            // Rb.velocity = transform.TransformDirection(Vector3.forward) * Speeds.RunSpeedOnStanding * Time.deltaTime * 10f;
        }
        public void CrouchRunning()
        {
            Timer += Time.deltaTime;
            if (Timer > WalkAndRunSounds.TimeBetweenCrouchRunSteps)
            {
                for (int aud = 0; aud < WalkAndRunSounds.RunningSounds.Length; aud++)
                {
                    WalkAndRunSounds.RunningSounds[aud].Play();
                }
                Timer = 0;
            }
            WalkAndRunSounds.CrouchRunStepsSoundTrigger.ActivateNoiseHandler(transform);
            transform.Translate(Vector3.forward * Speeds.RunSpeedOnCrouching * Time.deltaTime / 10);
        }
        // PlayerMovement
        public void PlayerMovement(float horizontal, float vertical)
        {
            if (JoystickScript != null)
            {
                if (JoystickScript.IsWalking == true)
                {
                    if (PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                    {
                        if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                        {
                            if (Crouch.instance.IsCrouching == false)
                            {
                                PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetFloat(PlayerManager.instance.CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.WalkSpeedParameterName, PlayerManager.instance.CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.StandAimedWalkAnimationSpeed);
                                WalkAndRunSounds.AimedStandWalkStepsSoundTrigger.ActivateNoiseHandler(transform);
                            }
                            else
                            {
                                PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetFloat(PlayerManager.instance.CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.WalkSpeedParameterName, PlayerManager.instance.CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.CrouchAimedWalkAnimationSpeed);
                                WalkAndRunSounds.AimedCrouchWalkStepsSoundTrigger.ActivateNoiseHandler(transform);
                            }
                        }
                        else
                        {
                            if (Crouch.instance.IsCrouching == false)
                            {
                                PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetFloat(PlayerManager.instance.CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.WalkSpeedParameterName, PlayerManager.instance.CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.StandWalkAnimationSpeed);
                                WalkAndRunSounds.StandWalkStepsSoundTrigger.ActivateNoiseHandler(transform);

                            }
                            else
                            {
                                PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.SetFloat(PlayerManager.instance.CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.WalkSpeedParameterName, PlayerManager.instance.CurrentHoldingPlayerWeapon.WeaponAnimationClipsSpeeds.CrouchWalkAnimationSpeed);
                                WalkAndRunSounds.CrouchWalkStepsSoundTrigger.ActivateNoiseHandler(transform);
                            }
                        }
                    }
                    
                }
            }

            bool grounded = GroundCheck;

            //Vector3 movedir = transform.TransformDirection(Vector3.forward) * vertical;
            // movedir += transform.TransformDirection(Vector3.right) * horizontal; 

            // Rb.AddForce();

            // Vector3 moveDirection = 
            // moveDirection += myTransform.InverseTransformDirection(transform.right) * horizontal;

            //moveDirection.y = -10f;

            if (jump)
            {
                if (CanPlayJumpSounds == false)
                {
                    Jump.JumpStartSound.PlayOneShot(Jump.JumpStartSound.clip);
                    CanPlayJumpSounds = true;
                }
                //jump = false;

                if (Crouch.instance != null)
                {
                    if (Crouch.instance.IsCrouching == false)
                    {
                        if (transform.position.y < Jump.StandHeight)
                        {
                            //Vector3 vel = transform.position;
                            //vel.y += Gravity * Time.deltaTime;
                            //transform.position = vel;
                            // movedir.y += JumpSpeed;  

                            if (Jumping == false)
                            {
                                LeanTween.moveY(transform.gameObject, Jump.StandHeight, Jump.StandJumpingDuration).setEaseInOutCubic();
                                Jumping = true;
                            }

                        }
                        else
                        {
                            CheckjumpLanding = true;
                            CanPlayJumpSounds = false;
                            jump = false;
                        }
                    }
                    else
                    {
                        if (transform.position.y < Jump.CrouchHeight)
                        {
                            //Vector3 vel = transform.position;
                            //vel.y += Gravity * Time.deltaTime;
                            //transform.position = vel;
                            // movedir.y += JumpSpeed;  

                            if (Jumping == false)
                            {
                                LeanTween.moveY(transform.gameObject, Jump.CrouchHeight, Jump.CrouchJumpingDuration).setEaseInOutCubic();
                                Jumping = true;
                            }

                        }
                        else
                        {
                            CheckjumpLanding = true;
                            CanPlayJumpSounds = false;
                            jump = false;
                        }
                    }

                }
                else
                {
                    if (transform.position.y < Jump.StandHeight)
                    {
                        //Vector3 vel = transform.position;
                        //vel.y += Gravity * Time.deltaTime;
                        //transform.position = vel;
                        // movedir.y += JumpSpeed;  

                        if (Jumping == false)
                        {
                            LeanTween.moveY(transform.gameObject, Jump.StandHeight, Jump.StandJumpingDuration).setEaseInOutCubic();
                            Jumping = true;
                        }

                    }
                    else
                    {
                        CheckjumpLanding = true;
                        CanPlayJumpSounds = false;
                        jump = false;
                    }
                }


                //if (controller.isGrounded)
                //{

                //}
                //  isPorjectileCube = !isPorjectileCube;
            }

            if (grounded && Crouch.instance.IsCrouching == false)
            {
                if (PlayerManager.instance != null)
                {
                    if(PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                    {
                        if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                        {
                            MovingSpeed = Speeds.AimedWalkingSpeed;
                        }
                        else
                        {
                            MovingSpeed = Speeds.WalkingSpeed;
                            //if(PlayerManager.instance != null)
                            //{
                            //    if (PlayerManager.instance.ob != null)
                            //    {
                            //        if (PlayerManager.instance.ob.gameObject.activeInHierarchy == true)
                            //        {
                            //            PlayerManager.instance.ob.AnimationsNames.WeaponAnimatorComponent.SetFloat(PlayerManager.instance.ob.AnimationSpeeds.WalkSpeedParametreName, PlayerManager.instance.ob.AnimationSpeeds.StandWalkAnimationSpeed);
                            //        }

                            //    }
                            //}

                        }
                    }
                    else
                    {
                        MovingSpeed = Speeds.WalkingSpeed;
                    }
                   
                }


                if (JoystickScript != null)
                {
                    if (JoystickScript.IsWalking == true)
                    {
                        IsLeanTweenCancelled = false;
                        if (PlayerManager.instance != null)
                        {
                            //if (PlayerManager.instance.ob.WeaponPosition.SniperScopeScript != null)
                            //{
                            //    if (PlayerManager.instance.ob.IsAimed == true)
                            //    {
                            //        NormalSniperStandBobbing();
                            //       // PlayerManager.instance.ob.SniperScopeScript.ScopeStandBobbing();
                            //    }
                            //    else
                            //    {
                            //        NormalStandBobbing();
                            //    }
                            //}
                            //else
                            //{

                            if(PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                            {
                                if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                                {
                                    if (IsStandAimed == false)
                                    {
                                        for (int x = 0; x < PlayerManager.instance.AllBobbingScripts.Length; x++)
                                        {
                                            if (PlayerManager.instance.AllBobbingScripts[x].gameObject.activeInHierarchy == true)
                                            {
                                                IsLeanTweenCancelled = true;
                                                LoopHeadBob = false;
                                                PlayerManager.instance.AllBobbingScripts[x].WalkStartLoop = false;
                                                LeanTween.cancel(PlayerManager.instance.AllBobbingScripts[x].BobObject);
                                            }

                                        }
                                        IsStandAimed = true;

                                    }
                                    if (IsLeanTweenCancelled == false)
                                    {
                                        NormalOpticalStandBobbing();
                                    }
                                }
                                else
                                {
                                    if (IsStandAimed == true)
                                    {
                                        for (int x = 0; x < PlayerManager.instance.AllBobbingScripts.Length; x++)
                                        {
                                            if (PlayerManager.instance.AllBobbingScripts[x].gameObject.activeInHierarchy == true)
                                            {
                                                IsLeanTweenCancelled = true;
                                                LoopHeadBob = false;
                                                PlayerManager.instance.AllBobbingScripts[x].WalkStartLoop = false;
                                                LeanTween.cancel(PlayerManager.instance.AllBobbingScripts[x].BobObject);
                                            }

                                        }

                                        IsStandAimed = false;

                                    }
                                    if (IsLeanTweenCancelled == false)
                                    {
                                        NormalStandBobbing();
                                    }
                                }
                            }
                            
                           
                            //}
                        }


                    }
                    else
                    {
                        if (HeadBobWalkReset == false)
                        {
                            if (PlayerManager.instance != null)
                            {
                                if (PlayerManager.instance.AllBobbingScripts.Length >= 1)
                                {
                                    if (PlayerManager.instance.AllBobbingScripts[0] != null)
                                    {
                                        for (int x = 0; x < PlayerManager.instance.AllBobbingScripts.Length; x++)
                                        {
                                            if (PlayerManager.instance.AllBobbingScripts[x].gameObject.activeInHierarchy == true)
                                            {
                                                PlayerManager.instance.AllBobbingScripts[x].WalkStartLoop = false;
                                                PlayerManager.instance.AllBobbingScripts[x].WalkAutoResetFunction();
                                            }

                                        }
                                    }
                                }

                            }
                            LoopHeadBob = false;
                            HeadBobWalkReset = true;
                        }
                    }
                }
            }
            else
            {
                if (JoystickScript != null)
                {
                    if (JoystickScript.IsWalking == true)
                    {
                        //if (PlayerManager.instance.ob.IsAimed == true)
                        //{
                        //    if(Crouch.instance.IsCrouching == false)
                        //    {
                        //        PlayerManager.instance.ob.AnimationsNames.WeaponAnimatorComponent.SetFloat(PlayerManager.instance.ob.AnimationSpeeds.WalkSpeedParametreName, PlayerManager.instance.ob.KeyframeAimingAnimationValues.StandAimedWalkAnimationSpeed);

                        //    }
                        //    else
                        //    {
                        //        PlayerManager.instance.ob.AnimationsNames.WeaponAnimatorComponent.SetFloat(PlayerManager.instance.ob.AnimationSpeeds.WalkSpeedParametreName, PlayerManager.instance.ob.KeyframeAimingAnimationValues.CrouchAimedWalkAnimationSpeed);
                        //    }
                        //}
                        //else
                        //{
                        //    if (Crouch.instance.IsCrouching == false)
                        //    {
                        //        PlayerManager.instance.ob.AnimationsNames.WeaponAnimatorComponent.SetFloat(PlayerManager.instance.ob.AnimationSpeeds.WalkSpeedParametreName, PlayerManager.instance.ob.AnimationSpeeds.StandWalkAnimationSpeed);


                        //    }
                        //    else
                        //    {                           
                        //        PlayerManager.instance.ob.AnimationsNames.WeaponAnimatorComponent.SetFloat(PlayerManager.instance.ob.AnimationSpeeds.WalkSpeedParametreName, PlayerManager.instance.ob.AnimationSpeeds.CrouchWalkAnimationSpeed);

                        //    }
                        //}


                        if(PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                        {
                            if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                            {
                                MovingSpeed = Speeds.AimedCrouchingSpeed;
                            }
                            else
                            {
                                MovingSpeed = Speeds.CrouchingSpeed;
                            }
                        }
                        else
                        {
                            MovingSpeed = Speeds.CrouchingSpeed;
                        }
                      
                        if (PlayerManager.instance != null)
                        {
                            //if (PlayerManager.instance.ob.WeaponPosition.SniperScopeScript != null)
                            //{
                            //    if (PlayerManager.instance.ob.IsAimed == true)
                            //    {
                            //        NormalSniperCrouchWalking();
                            //       // PlayerManager.instance.ob.SniperScopeScript.ScopeCrouchBobbing();
                            //    }
                            //    else
                            //    {
                            //       NormalCrouchWalking();
                            //    }
                            //}
                            //else
                            //{
                            if (PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                            {
                                if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                                {
                                    NormalOpticalCrouchWalking();
                                }
                                else
                                {
                                    NormalCrouchWalking();
                                }
                            }
                            else
                            {
                                NormalCrouchWalking();
                            }
                            // }
                        }
                    }
                    else
                    {
                        if (HeadBobWalkReset == false)
                        {
                            if (PlayerManager.instance != null)
                            {
                                if (PlayerManager.instance.AllBobbingScripts.Length >= 1)
                                {
                                    if (PlayerManager.instance.AllBobbingScripts[0] != null)
                                    {
                                        for (int x = 0; x < PlayerManager.instance.AllBobbingScripts.Length; x++)
                                        {
                                            if (PlayerManager.instance.AllBobbingScripts[x].gameObject.activeInHierarchy == true)
                                            {
                                                PlayerManager.instance.AllBobbingScripts[x].WalkStartLoop = false;
                                                PlayerManager.instance.AllBobbingScripts[x].WalkAutoResetFunction();
                                            }
                                        }
                                    }
                                }
                            }
                            LoopHeadBob = false;
                            HeadBobWalkReset = true;
                        }
                    }
                }
            }




            if (jump == false && CheckjumpLanding == true)
            {
                //Vector3 vel = transform.position;
                //vel.y -= Gravity * Time.deltaTime;
                //transform.position = vel;f

                if (Jumping == true)
                {
                    IsFallingCompleted = false;
                    if (Crouch.instance != null)
                    {
                        if (Crouch.instance.IsCrouching == false)
                        {
                            LeanTween.moveY(transform.gameObject, DefaultYPos, Jump.StandFallingDuration).setEaseInOutCubic();
                            StartCoroutine(FallDurationCompleted(Jump.StandFallingDuration));

                        }
                        else
                        {
                            LeanTween.moveY(transform.gameObject, DefaultYPos, Jump.CrouchFallingDuration).setEaseInOutCubic();
                            StartCoroutine(FallDurationCompleted(Jump.CrouchFallingDuration));
                        }
                    }
                    else
                    {
                        LeanTween.moveY(transform.gameObject, DefaultYPos, Jump.StandFallingDuration).setEaseInOutCubic();
                        StartCoroutine(FallDurationCompleted(Jump.StandFallingDuration));
                    }
                    Jumping = false;
                }

                //   if (GroundCheck)
                if (IsFallingCompleted == true)
                {
                    Rb.isKinematic = false;
                    if (HeadBobJumpReset == false)
                    {
                        if (PlayerManager.instance != null)
                        {
                            if (PlayerManager.instance.AllBobbingScripts.Length >= 1)
                            {
                                if (PlayerManager.instance.AllBobbingScripts[0] != null)
                                {
                                    for (int x = 0; x < PlayerManager.instance.AllBobbingScripts.Length; x++)
                                    {
                                        if (PlayerManager.instance.AllBobbingScripts[x].gameObject.activeInHierarchy == true)
                                        {
                                            PlayerManager.instance.AllBobbingScripts[x].JumpStartLoop = false;
                                            PlayerManager.instance.AllBobbingScripts[x].JumpAutoResetFunction();
                                        }

                                    }
                                }
                            }
                        }
                        LoopJumpHeadBob = false;
                        HeadBobJumpReset = true;
                    }

                    if (CanPlayJumpSounds == false)
                    {
                        StartCoroutine(JumpButtonActivator());
                        Jump.JumpLandingSound.PlayOneShot(Jump.JumpLandingSound.clip);
                        CanPlayJumpSounds = true;
                        CheckjumpLanding = false;
                    }
                }
            }
            //transform.Translate(movedir * Time.deltaTime / 10);
            transform.Translate(horizontal * Time.deltaTime * MovingSpeed / 10, 0f, vertical * Time.deltaTime * MovingSpeed / 10);

            // Rb.velocity = movedir * Time.deltaTime * 10;
            //  Rb.velocity = movedir * RunSpeedOnStanding * Time.fixedDeltaTime * Multiply;
            //if (!prevGrounded && grounded)
            //    moveDirection.y = 0f;
            //prevGrounded = grounded;
        }

        // PlayerRotation
        public void PlayerRotation(float horizontal, float vertical)
        {
            myTransform.Rotate(0f, horizontal * 12f, 0f);
            rotation += vertical * 12f;
            rotation = Mathf.Clamp(rotation, -60f, 60f);
            cameraTransform.localEulerAngles = new Vector3(-rotation, cameraTransform.localEulerAngles.y, 0f);
        }
        IEnumerator JumpButtonActivator()
        {
            yield return new WaitForSeconds(Jump.JumpButtonActivationDelay);
            Jump.JumpButton.interactable = true;
            IsJumpDelayFinished = false;
        }

        // PlayerFiring
        public void PlayerFiring()
        {
            if (!weapReady)
                return;

            weapReady = false;

            // GameObject primitive = GameObject.CreatePrimitive(isPorjectileCube ? PrimitiveType.Cube : PrimitiveType.Sphere);
            //primitive.transform.position = (myTransform.position + myTransform.right);
            //primitive.transform.localScale = Vector3.one * .2f;
            //Rigidbody rBody = primitive.AddComponent<Rigidbody>();
            //Transform camTransform = Camera.main.transform;
            //rBody.AddForce(camTransform.forward * Random.Range(25f, 35f) + camTransform.right * Random.Range(-2f, 2f) + camTransform.up * Random.Range(-2f, 2f), ForceMode.Impulse);
            //Destroy(primitive, 3.5f);
        }
        IEnumerator FallDurationCompleted(float Duration)
        {
            yield return new WaitForSeconds(Duration);
            IsFallingCompleted = true;
        }
    };
}