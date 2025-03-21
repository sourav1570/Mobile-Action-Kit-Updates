using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace MobileActionKit
{
    public class JoyStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        public static JoyStick Instance;

        [TextArea] 
        public string ScriptInfo = "This Script Controls The Joystick Movements in the game";
        [Space(10)]

        public Image JoystickBackground;
        public Image JoystickHandler;
        public Vector3 InputDirection;

        [HideInInspector]
        public AudioSource[] WalkingSounds;

        [HideInInspector]
        public float Timer = 1000f;

        bool RunningIsActive = false;

        [HideInInspector]
        public bool IsWalking = false;

        float WalkSpeedOnStand;
        float WalkSpeedOnCrouch;
        float RunSpeedOnStand;
        float RunSpeedOnCrouch;

        [HideInInspector]
        public bool ActiveForShake = false;

        PlayerWeapon ob;

        PlayerManager fc;
        FirstPersonController fps;

        private void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            InputDirection = Vector3.zero;
            if (FirstPersonController.instance != null)
            {
                WalkSpeedOnStand = FirstPersonController.instance.Speeds.WalkingSpeed;
                WalkSpeedOnCrouch = FirstPersonController.instance.Speeds.CrouchingSpeed;

                RunSpeedOnStand = FirstPersonController.instance.Speeds.RunSpeedOnStanding;
                RunSpeedOnCrouch = FirstPersonController.instance.Speeds.RunSpeedOnCrouching;

                WalkingSounds = FirstPersonController.instance.WalkAndRunSounds.WalkingSounds;
            }

            fc = FindObjectOfType<PlayerManager>();
            fps = FindObjectOfType<FirstPersonController>();
        }
        public virtual void OnDrag(PointerEventData ped)
        {
            Vector2 position;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(JoystickBackground.rectTransform, ped.position, ped.pressEventCamera, out position))
            {
                position.x = (position.x / JoystickBackground.rectTransform.sizeDelta.x);
                position.y = (position.y / JoystickBackground.rectTransform.sizeDelta.y);

                InputDirection = new Vector3(position.x * 2, 0, position.y * 2);
                InputDirection = (InputDirection.magnitude > 1.0f) ? InputDirection.normalized : InputDirection;

                JoystickHandler.rectTransform.anchoredPosition = new Vector3(InputDirection.x * (JoystickBackground.rectTransform.sizeDelta.x / 3), InputDirection.z * (JoystickBackground.rectTransform.sizeDelta.y) / 3);
            }
        }
        public virtual void OnPointerDown(PointerEventData ped)
        {
            OnDrag(ped);
            IsWalking = true;
            if (FindObjectOfType<PlayerWeapon>() != null)
            {
                PlayerWeapon ob = FindObjectOfType<PlayerWeapon>();

                ob.IsWalking = true;

                if (ob.Reload.isreloading == false)
                {
                    if (fc.isRunEnable == true)
                    {
                        if (ob.IsAimed == true)
                        {
                            ActiveForShake = false;
                            fc.Aiming();
                        }
                        ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.IdleAnimationParametreName, false);
                        // ob.AnimationsNames.WeaponAnimatorComponent.Play(ob.AnimationsNames.RunAnimationname, -1, 0f);
                        ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.WalkAnimationParametreName, false);
                        ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.RunAnimationParametreName, true);

                        if (FirstPersonController.instance != null)
                        {
                            FirstPersonController.instance.Speeds.WalkingSpeed = RunSpeedOnStand;
                            FirstPersonController.instance.Speeds.CrouchingSpeed = RunSpeedOnCrouch;
                        }

                        if (PlayerManager.instance != null)
                        {
                            PlayerManager.instance.CanStartProducingRunningSounds = true;
                            PlayerManager.instance.CanStartProducingWalkingSounds = false;
                            PlayerManager.instance.StartSprintBobbing();
                            PlayerManager.instance.IsSwitchSprinting = true;
                        }

                        RunningIsActive = true;
                    }
                    else
                    {
                        if (PlayerManager.instance != null)
                        {
                            PlayerManager.instance.CanStartProducingRunningSounds = false;
                            PlayerManager.instance.CanStartProducingWalkingSounds = true;
                        }
                        if (ob.IsAimed == true)
                        {
                            ActiveForShake = true;
                        }
                        if (FirstPersonController.instance != null)
                        {
                            FirstPersonController.instance.Speeds.WalkingSpeed = WalkSpeedOnStand;
                            FirstPersonController.instance.Speeds.CrouchingSpeed = WalkSpeedOnCrouch;
                        }
                        RunningIsActive = false;

                        ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.IdleAnimationParametreName, false);
                        ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.WalkAnimationParametreName, true);
                        ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.RunAnimationParametreName, false);
                        //ob.AnimationsNames.WeaponAnimatorComponent.Play(ob.AnimationsNames.WalkingAnimationName, -1, 0f);

                        //ob.WeaponAnimatorComponent.Play(ob.WalkingAnimationName, -1, 0f);
                    }


                }
            }
            else if(fc.isRunEnable == true)
            {
                if (PlayerManager.instance != null)
                {
                    PlayerManager.instance.CanStartProducingRunningSounds = true;
                    PlayerManager.instance.CanStartProducingWalkingSounds = false;
                }
                if (FirstPersonController.instance != null)
                {
                    FirstPersonController.instance.Speeds.WalkingSpeed = RunSpeedOnStand;
                    FirstPersonController.instance.Speeds.CrouchingSpeed = RunSpeedOnCrouch;
                }
            }
            else
            {
                if (PlayerManager.instance != null)
                {
                    PlayerManager.instance.CanStartProducingRunningSounds = false;
                    PlayerManager.instance.CanStartProducingWalkingSounds = true;
                }
                if (FirstPersonController.instance != null)
                {
                    FirstPersonController.instance.Speeds.WalkingSpeed = WalkSpeedOnStand;
                    FirstPersonController.instance.Speeds.CrouchingSpeed = WalkSpeedOnCrouch;
                }
            }
        }
        public virtual void OnPointerUp(PointerEventData ped)
        {
            Timer = 0;
            InputDirection = Vector3.zero;
            JoystickHandler.rectTransform.anchoredPosition = Vector3.zero;
            IsWalking = false;
            if (FindObjectOfType<PlayerWeapon>() != null)
            {
                ob = FindObjectOfType<PlayerWeapon>();
                if (ob != null)
                {
                    if (ob.IsAimed == true)
                    {
                        ActiveForShake = false;
                    }
                    ob.IsWalking = false;
                }

                if (fc != null)
                {
                    if (fc.isRunEnable == true)
                    {
                        if (ob.IsAimed == true)
                        {
                            if (ob.UseProceduralAimedBreath == true && ob.CombineProceduralBreathWithAnimation == true)
                            {
                                ob.IsplayingAimedAnim = false;
                                ob.AimedBreathAnimController();
                            }
                            else if (ob.UseProceduralAimedBreath == false)
                            {
                                ob.IsplayingAimedAnim = false;
                                ob.AimedBreathAnimController();
                            }
                            else
                            {
                                ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.IdleAnimationParametreName, true);
                                ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.WalkAnimationParametreName, false);
                                ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.RunAnimationParametreName, false);
                                // ob.WeaponAnimatorComponent.Play(ob.IdleAnimationName, -1, 0f);
                            }
                        }
                        else
                        {
                            ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.IdleAnimationParametreName, true);
                            ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.WalkAnimationParametreName, false);
                            ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.RunAnimationParametreName, false);
                            // ob.WeaponAnimatorComponent.Play(ob.IdleAnimationName, -1, 0f);
                        }
                        if (PlayerManager.instance != null)
                        {
                            PlayerManager.instance.StopRunningStepSounds();
                            PlayerManager.instance.CanStartProducingRunningSounds = false;
                            PlayerManager.instance.StopBobbing();
                            PlayerManager.instance.IsSwitchSprinting = false;
                        }
                    }
                    else
                    {
                        if (ob.IsAimed == true)
                        {
                            if (ob.UseProceduralAimedBreath == true && ob.CombineProceduralBreathWithAnimation == true)
                            {
                                ob.IsplayingAimedAnim = false;
                                ob.AimedBreathAnimController();
                            }
                            //else if(ob.UseProceduralAimedBreath == false)
                            //{
                            //    ob.IsplayingAimedAnim = false;
                            //    ob.AimedBreathAnimController();
                            //}
                            else
                            {
                                
                                    ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.IdleAnimationParametreName, true);
                                    ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.WalkAnimationParametreName, false);
                                    ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.RunAnimationParametreName, false);
                                    //  ob.WeaponAnimatorComponent.Play(ob.IdleAnimationName, -1, 0f);
                                

                            }
                        }
                        else
                        {
                            ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.IdleAnimationParametreName, true);
                            ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.WalkAnimationParametreName, false);
                            ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.RunAnimationParametreName, false);
                            //ob.WeaponAnimatorComponent.Play(ob.IdleAnimationName, -1, 0f);
                        }
                        StopWalkingSounds();
                    }




                }

            }
            else
            {
                StopWalkingSounds();
            }
        }
        public void Pc_Controls_StartWalking()
        {           
            IsWalking = true;
            if (FindObjectOfType<PlayerWeapon>() != null)
            {
                PlayerWeapon ob = FindObjectOfType<PlayerWeapon>();

                ob.IsWalking = true;

                if (ob.Reload.isreloading == false)
                {
                    if (fc.isRunEnable == true)
                    {
                        if (ob.IsAimed == true)
                        {
                            ActiveForShake = false;
                            fc.Aiming();
                        }
                        ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.IdleAnimationParametreName, false);
                        // ob.AnimationsNames.WeaponAnimatorComponent.Play(ob.AnimationsNames.RunAnimationname, -1, 0f);
                        ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.WalkAnimationParametreName, false);
                        ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.RunAnimationParametreName, true);

                        if (FirstPersonController.instance != null)
                        {
                            FirstPersonController.instance.Speeds.WalkingSpeed = RunSpeedOnStand;
                            FirstPersonController.instance.Speeds.CrouchingSpeed = RunSpeedOnCrouch;
                        }

                        if (PlayerManager.instance != null)
                        {
                            PlayerManager.instance.CanStartProducingRunningSounds = true;
                            PlayerManager.instance.CanStartProducingWalkingSounds = false;
                            PlayerManager.instance.StartSprintBobbing();
                            PlayerManager.instance.IsSwitchSprinting = true;
                        }

                        RunningIsActive = true;
                    }
                    else
                    {
                        if (PlayerManager.instance != null)
                        {
                            PlayerManager.instance.CanStartProducingRunningSounds = false;
                            PlayerManager.instance.CanStartProducingWalkingSounds = true;
                        }
                        if (ob.IsAimed == true)
                        {
                            ActiveForShake = true;
                        }
                        if (FirstPersonController.instance != null)
                        {
                            FirstPersonController.instance.Speeds.WalkingSpeed = WalkSpeedOnStand;
                            FirstPersonController.instance.Speeds.CrouchingSpeed = WalkSpeedOnCrouch;
                        }
                        RunningIsActive = false;

                        ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.IdleAnimationParametreName, false);
                        ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.WalkAnimationParametreName, true);
                        ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.RunAnimationParametreName, false);
                        //ob.AnimationsNames.WeaponAnimatorComponent.Play(ob.AnimationsNames.WalkingAnimationName, -1, 0f);
                        //ob.WeaponAnimatorComponent.Play(ob.WalkingAnimationName, -1, 0f);
                    }




                }
            }
            else
            {
                if (FirstPersonController.instance != null)
                {
                    FirstPersonController.instance.Speeds.WalkingSpeed = WalkSpeedOnStand;
                    FirstPersonController.instance.Speeds.CrouchingSpeed = WalkSpeedOnCrouch;
                }
            }
        }
        public void Pc_Controls_StopWalking()
        {
            Timer = 0;
            InputDirection = Vector3.zero;
            JoystickHandler.rectTransform.anchoredPosition = Vector3.zero;
            IsWalking = false;
            if (FindObjectOfType<PlayerWeapon>() != null)
            {
                ob = FindObjectOfType<PlayerWeapon>();
                if (ob != null)
                {
                    if (ob.IsAimed == true)
                    {
                        ActiveForShake = false;
                    }
                    ob.IsWalking = false;
                }

                if (fc != null)
                {
                    if (fc.isRunEnable == true)
                    {
                        if (ob.IsAimed == true)
                        {
                            if (ob.UseProceduralAimedBreath == true && ob.CombineProceduralBreathWithAnimation == true)
                            {
                                ob.IsplayingAimedAnim = false;
                                ob.AimedBreathAnimController();
                            }
                            else if (ob.UseProceduralAimedBreath == false)
                            {
                                ob.IsplayingAimedAnim = false;
                                ob.AimedBreathAnimController();
                            }
                            else
                            {
                                ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.IdleAnimationParametreName, true);
                                ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.WalkAnimationParametreName, false);
                                ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.RunAnimationParametreName, false);
                                // ob.WeaponAnimatorComponent.Play(ob.IdleAnimationName, -1, 0f);
                            }
                        }
                        else
                        {
                            ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.IdleAnimationParametreName, true);
                            ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.WalkAnimationParametreName, false);
                            ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.RunAnimationParametreName, false);
                            // ob.WeaponAnimatorComponent.Play(ob.IdleAnimationName, -1, 0f);
                        }
                        if (PlayerManager.instance != null)
                        {
                            PlayerManager.instance.StopRunningStepSounds();
                            PlayerManager.instance.CanStartProducingRunningSounds = false;
                            PlayerManager.instance.StopBobbing();
                            PlayerManager.instance.IsSwitchSprinting = false;
                        }
                    }
                    else
                    {
                        if (ob.IsAimed == true)
                        {
                            if (ob.UseProceduralAimedBreath == true && ob.CombineProceduralBreathWithAnimation == true)
                            {
                                ob.IsplayingAimedAnim = false;
                                ob.AimedBreathAnimController();
                            }
                            //else if(ob.UseProceduralAimedBreath == false)
                            //{
                            //    ob.IsplayingAimedAnim = false;
                            //    ob.AimedBreathAnimController();
                            //}
                            else
                            {
                                
                                    ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.IdleAnimationParametreName, true);
                                    ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.WalkAnimationParametreName, false);
                                    ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.RunAnimationParametreName, false);
                                    //  ob.WeaponAnimatorComponent.Play(ob.IdleAnimationName, -1, 0f);
                                

                            }
                        }
                        else
                        {
                            ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.IdleAnimationParametreName, true);
                            ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.WalkAnimationParametreName, false);
                            ob.RequiredComponents.WeaponAnimatorComponent.SetBool(ob.RunAnimationParametreName, false);
                            //ob.WeaponAnimatorComponent.Play(ob.IdleAnimationName, -1, 0f);
                        }


                        StopWalkingSounds();
                    }
                }
            }
            else
            {
                StopWalkingSounds();


            }
        }
        public float Horizontal()
        {
            //if (IsWalking == true)
            //{
            //    Timer += Time.deltaTime;
            //}

            if (InputDirection.x != 0)
                return InputDirection.x;
            else
                return Input.GetAxisRaw("Horizontal");
        }
        public float Vertical()
        {
            //if (IsWalking == true)
            //{
            //    Timer += Time.deltaTime;
            //}

            if (InputDirection.z != 0)
                return InputDirection.z;
            else
                return Input.GetAxisRaw("Vertical");
        }
        public float RightWalk()
        {
            //if (IsWalking == true)
            //{
            //    Timer += Time.deltaTime;
            //}

            InputDirection.x = 1f;
            return InputDirection.x;

        }
        public float LeftWalk()
        {
            //if (IsWalking == true)
            //{
            //    Timer += Time.deltaTime;
            //}


            InputDirection.x = -1f;
            return InputDirection.x;
        }
        public float BackwardWalk()
        {
            //if (IsWalking == true)
            //{
            //    Timer += Time.deltaTime;
            //}
            InputDirection.z = -1;
            return InputDirection.z;
        }
        public float ForwardWalk()
        {
            //if (IsWalking == true)
            //{
            //    Timer += Time.deltaTime;
            //}

            InputDirection.z = 1;
            return InputDirection.z;

        }
        public float StopWalk()
        {
            InputDirection.z = 0;
            return InputDirection.z;
        }
        public void StopWalkingSounds()
        {
            if (PlayerManager.instance != null)
            {
                PlayerManager.instance.CanStartProducingWalkingSounds = false;
                PlayerManager.instance.CanStartProducingRunningSounds = false;
            }
            for (int aud = 0; aud < FirstPersonController.instance.WalkAndRunSounds.WalkingSounds.Length; aud++)
            {
                if (FirstPersonController.instance.WalkAndRunSounds.WalkingSounds[aud].isPlaying)
                {
                    FirstPersonController.instance.WalkAndRunSounds.WalkingSounds[aud].Stop();
                }
            }
        }

        public void CheckingForWalkingSound()
        {
            if (RunningIsActive == false)
            {
                Timer += Time.deltaTime;
                if (Crouch.instance.IsCrouching == false && PlayerManager.instance != null)
                {
                    if (PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                    {
                        if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == false)
                        {
                            if (FirstPersonController.instance != null)
                            {
                                if (Timer > FirstPersonController.instance.WalkAndRunSounds.TimeBetweenStandWalkSteps)
                                {
                                    for (int aud = 0; aud < fps.WalkAndRunSounds.WalkingSounds.Length; aud++)
                                    {
                                        if (!fps.WalkAndRunSounds.WalkingSounds[aud].isPlaying)
                                        {
                                            fps.WalkAndRunSounds.WalkingSounds[aud].Play();
                                        }
                                    }


                                    Timer = 0;
                                }
                            }

                        }
                        else
                        {
                            if (Timer > FirstPersonController.instance.WalkAndRunSounds.TimeBetweenAimedStandSteps)
                            {
                                if (FirstPersonController.instance != null)
                                {
                                    for (int aud = 0; aud < fps.WalkAndRunSounds.WalkingSounds.Length; aud++)
                                    {
                                        if (!fps.WalkAndRunSounds.WalkingSounds[aud].isPlaying)
                                        {
                                            fps.WalkAndRunSounds.WalkingSounds[aud].Play();
                                        }
                                    }
                                }
                                Timer = 0;
                            }
                        }
                    }
                    else
                    {
                        if (FirstPersonController.instance != null)
                        {
                            if (Timer > FirstPersonController.instance.WalkAndRunSounds.TimeBetweenStandWalkSteps)
                            {
                                for (int aud = 0; aud < fps.WalkAndRunSounds.WalkingSounds.Length; aud++)
                                {
                                    if (!fps.WalkAndRunSounds.WalkingSounds[aud].isPlaying)
                                    {
                                        fps.WalkAndRunSounds.WalkingSounds[aud].Play();
                                    }
                                }


                                Timer = 0;
                            }
                        }
                    }

                }
                else
                {
                    if (PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                    {
                        if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == false)
                        {
                            if (FirstPersonController.instance != null)
                            {
                                if (Timer > FirstPersonController.instance.WalkAndRunSounds.TimeBetweenCrouchWalkSteps)
                                {
                                    for (int aud = 0; aud < fps.WalkAndRunSounds.WalkingSounds.Length; aud++)
                                    {
                                        if (!fps.WalkAndRunSounds.WalkingSounds[aud].isPlaying)
                                        {
                                            fps.WalkAndRunSounds.WalkingSounds[aud].Play();
                                        }
                                    }
                                    Timer = 0;
                                }
                            }
                        }
                        else
                        {
                            if (FirstPersonController.instance != null)
                            {
                                if (Timer > FirstPersonController.instance.WalkAndRunSounds.TimeBetweenAimedCrouchSteps)
                                {
                                    for (int aud = 0; aud < fps.WalkAndRunSounds.WalkingSounds.Length; aud++)
                                    {
                                        if (!fps.WalkAndRunSounds.WalkingSounds[aud].isPlaying)
                                        {
                                            fps.WalkAndRunSounds.WalkingSounds[aud].Play();
                                        }
                                    }
                                    Timer = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (FirstPersonController.instance != null)
                        {
                            if (Timer > FirstPersonController.instance.WalkAndRunSounds.TimeBetweenCrouchWalkSteps)
                            {
                                for (int aud = 0; aud < fps.WalkAndRunSounds.WalkingSounds.Length; aud++)
                                {
                                    if (!fps.WalkAndRunSounds.WalkingSounds[aud].isPlaying)
                                    {
                                        fps.WalkAndRunSounds.WalkingSounds[aud].Play();
                                    }
                                }
                                Timer = 0;
                            }
                        }
                    }
                }
            }
        }
    }
}
