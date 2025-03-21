using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace MobileActionKit
{
    public class ReticleAdjusterScript : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script helps in changing the sniper scope reticle position in X and Y axis when the weapon is aiming.";

        [Tooltip("Reference to the player's weapon script to retrieve shooting and aiming settings.")]
        public PlayerWeapon PlayerWeaponScript;

        [Tooltip("Settings for configuring the reticle, including X and Y slider adjustments.")]
        public ReticleSettings ReticleConfig = new ReticleSettings();

        [Tooltip("Audio source that plays when a slider adjustment is made.")]
        public AudioSource SliderAdjusterSound;

        [System.Serializable]
        public class ReticleSettings
        {
            [Tooltip("The reticle's transform, which will be adjusted based on the slider settings.")]
            public Transform Reticle;

            [Tooltip("Settings for the horizontal slider that adjusts the X position of the reticle.")]
            public SliderSettings XSliderSettings = new SliderSettings();

            [Tooltip("Settings for the vertical slider that adjusts the Y position of the reticle.")]
            public SliderSettings YSliderSettings = new SliderSettings();
        }

        [System.Serializable]
        public class SliderSettings
        {
            [Tooltip("The slider UI element used to adjust the position of the reticle.")]
            public Slider Slider;

            [Tooltip("Determines whether the slider uses discrete steps for adjustments.")]
            public bool EnableStepping = false;

            [Tooltip("The number of steps on either side of the middle position for stepping adjustments.")]
            public int StepsFromMiddle;

            [Tooltip("The size of each step for stepping adjustments.")]
            public float StepSize = 0.001f;

            [Tooltip("Speed at which the reticle moves when the slider value changes.")]
            public float MovementSpeed;
        }

        private int RealXSteps;
        private float[] XAlgo;

        private float[] NegativeX;
        private float[] PositiveX;

        private int RealYSteps;
        private float[] YAlgo;

        private float[] NegativeY;
        private float[] PositiveY;


        private void Start()
        {
            if(PlayerWeaponScript != null)
            {
                if(PlayerWeaponScript.ShootingFeatures.UseSniperScopeUI == true)
                {
                    if (ReticleConfig.XSliderSettings.Slider != null)
                    {
                        if (ReticleConfig.XSliderSettings.EnableStepping == true)
                        {
                            RealXSteps = ReticleConfig.XSliderSettings.StepsFromMiddle * 2;
                            XAlgo = new float[RealXSteps + 1];
                            NegativeX = new float[ReticleConfig.XSliderSettings.StepsFromMiddle];
                            PositiveX = new float[ReticleConfig.XSliderSettings.StepsFromMiddle];

                            XAlgo[ReticleConfig.XSliderSettings.StepsFromMiddle] = ReticleConfig.Reticle.transform.localPosition.x;

                            for (int x = 0; x < NegativeX.Length; x++)
                            {
                                XAlgo[ReticleConfig.XSliderSettings.StepsFromMiddle - x - 1] = XAlgo[ReticleConfig.XSliderSettings.StepsFromMiddle - x] + ReticleConfig.XSliderSettings.StepSize;
                                XAlgo[ReticleConfig.XSliderSettings.StepsFromMiddle - x - 1] = +XAlgo[ReticleConfig.XSliderSettings.StepsFromMiddle - x - 1];
                            }

                            for (int x = 0; x < PositiveX.Length; x++)
                            {
                                XAlgo[ReticleConfig.XSliderSettings.StepsFromMiddle + x + 1] = XAlgo[ReticleConfig.XSliderSettings.StepsFromMiddle - x] + ReticleConfig.XSliderSettings.StepSize;
                                XAlgo[ReticleConfig.XSliderSettings.StepsFromMiddle + x + 1] = -XAlgo[ReticleConfig.XSliderSettings.StepsFromMiddle + x + 1];
                            }

                            ReticleConfig.XSliderSettings.Slider.wholeNumbers = true;
                            ReticleConfig.XSliderSettings.Slider.minValue = 0;
                            ReticleConfig.XSliderSettings.Slider.maxValue = RealXSteps;
                            ReticleConfig.XSliderSettings.Slider.value = ReticleConfig.XSliderSettings.StepsFromMiddle;

                            ReticleConfig.XSliderSettings.Slider.onValueChanged.AddListener(delegate { XSteppingFunction(); });
                        }
                        //else
                        //{
                        //    ReticleConfig.XSliderSettings.Slider.minValue = ReticleConfig.XSliderSettings.MinValue;
                        //    ReticleConfig.XSliderSettings.Slider.maxValue = ReticleConfig.XSliderSettings.MaxValue;
                        //    ReticleConfig.XSliderSettings.Slider.value = 0;
                        //}
                    }

                    if (ReticleConfig.YSliderSettings.Slider != null)
                    {
                        if (ReticleConfig.YSliderSettings.EnableStepping == true)
                        {
                            RealYSteps = ReticleConfig.YSliderSettings.StepsFromMiddle * 2;
                            YAlgo = new float[RealYSteps + 1];
                            NegativeY = new float[ReticleConfig.YSliderSettings.StepsFromMiddle];
                            PositiveY = new float[ReticleConfig.YSliderSettings.StepsFromMiddle];

                            YAlgo[ReticleConfig.YSliderSettings.StepsFromMiddle] = ReticleConfig.Reticle.transform.localPosition.y;

                            for (int x = 0; x < NegativeY.Length; x++)
                            {
                                YAlgo[ReticleConfig.YSliderSettings.StepsFromMiddle - x - 1] = ReticleConfig.YSliderSettings.StepSize + YAlgo[ReticleConfig.YSliderSettings.StepsFromMiddle - x];
                                YAlgo[ReticleConfig.YSliderSettings.StepsFromMiddle - x - 1] = +YAlgo[ReticleConfig.YSliderSettings.StepsFromMiddle - x - 1];
                            }

                            for (int x = 0; x < PositiveY.Length; x++)
                            {
                                YAlgo[ReticleConfig.YSliderSettings.StepsFromMiddle + x + 1] = ReticleConfig.YSliderSettings.StepSize + YAlgo[ReticleConfig.YSliderSettings.StepsFromMiddle - x];
                                YAlgo[ReticleConfig.YSliderSettings.StepsFromMiddle + x + 1] = -YAlgo[ReticleConfig.YSliderSettings.StepsFromMiddle + x + 1];
                            }


                            ReticleConfig.YSliderSettings.Slider.wholeNumbers = true;
                            ReticleConfig.YSliderSettings.Slider.minValue = 0;
                            ReticleConfig.YSliderSettings.Slider.maxValue = RealYSteps;
                            ReticleConfig.YSliderSettings.Slider.value = ReticleConfig.YSliderSettings.StepsFromMiddle;

                            ReticleConfig.YSliderSettings.Slider.onValueChanged.AddListener(delegate { YSteppingFunction(); });
                        }
                        //else
                        //{
                        //    ReticleConfig.YSliderSettings.Slider.minValue = ReticleConfig.YSliderSettings.MinValue;
                        //    ReticleConfig.YSliderSettings.Slider.maxValue = ReticleConfig.YSliderSettings.MaxValue;
                        //    ReticleConfig.YSliderSettings.Slider.value = 0;

                        //}
                    }
                }
            }
          
        }
        void OnDisable()
        {
            if (ReticleConfig.YSliderSettings.Slider != null)
            {
                ReticleConfig.YSliderSettings.Slider.gameObject.SetActive(false);
            }
            if (ReticleConfig.XSliderSettings.Slider != null)
            {
                ReticleConfig.XSliderSettings.Slider.gameObject.SetActive(false);
            }
        }
        public void XSteppingFunction()
        {
            Vector3 pos = ReticleConfig.Reticle.transform.localPosition;
            pos.x = XAlgo[Mathf.RoundToInt(ReticleConfig.XSliderSettings.Slider.value)];
            ReticleConfig.Reticle.transform.localPosition = pos;
            SliderAdjusterSound.Play();
        }
        public void YSteppingFunction()
        {
            Vector3 pos = ReticleConfig.Reticle.transform.localPosition;
            pos.y = YAlgo[Mathf.RoundToInt(ReticleConfig.YSliderSettings.Slider.value)];
            ReticleConfig.Reticle.transform.localPosition = pos;
            SliderAdjusterSound.Play();
        }
        void Update()
        {
            if (PlayerWeaponScript != null)
            {
                if (PlayerWeaponScript.ShootingFeatures.UseSniperScopeUI == true)
                {
                    if (PlayerWeaponScript.IsAimed == true)
                    {
                        if (ReticleConfig.YSliderSettings.Slider != null)
                        {
                            ReticleConfig.YSliderSettings.Slider.gameObject.SetActive(true);
                        }
                        if (ReticleConfig.XSliderSettings.Slider != null)
                        {
                            ReticleConfig.XSliderSettings.Slider.gameObject.SetActive(true);
                        }
                        if (ReticleConfig.Reticle != null)
                        {
                            if (ReticleConfig.YSliderSettings.Slider != null)
                            {
                                if (ReticleConfig.YSliderSettings.EnableStepping == false)
                                {
                                    Vector3 pos = ReticleConfig.Reticle.transform.localPosition;
                                    pos.y = Mathf.Lerp(pos.y, ReticleConfig.YSliderSettings.Slider.value, ReticleConfig.YSliderSettings.MovementSpeed * Time.deltaTime);
                                    ReticleConfig.Reticle.transform.localPosition = pos;
                                }

                            }
                            if (ReticleConfig.XSliderSettings.Slider != null)
                            {
                                if (ReticleConfig.XSliderSettings.EnableStepping == false)
                                {
                                    Vector3 pos = ReticleConfig.Reticle.transform.localPosition;
                                    pos.x = Mathf.Lerp(pos.x, ReticleConfig.XSliderSettings.Slider.value, ReticleConfig.XSliderSettings.MovementSpeed * Time.deltaTime);
                                    ReticleConfig.Reticle.transform.localPosition = pos;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (ReticleConfig.YSliderSettings.Slider != null)
                        {
                            ReticleConfig.YSliderSettings.Slider.gameObject.SetActive(false);
                        }
                        if (ReticleConfig.XSliderSettings.Slider != null)
                        {
                            ReticleConfig.XSliderSettings.Slider.gameObject.SetActive(false);
                        }
                    }
                }
            }

        }
    }
}