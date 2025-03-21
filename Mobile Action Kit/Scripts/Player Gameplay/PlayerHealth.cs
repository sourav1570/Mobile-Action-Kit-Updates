using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This Script is Responsible For Player Health 
namespace MobileActionKit
{
    public class PlayerHealth : MonoBehaviour
    {
        public static PlayerHealth instance;

        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This Script Regulates The Player Health In game";
        [Space(10)]

        [Space(10)]
        [Tooltip("The Rigidbody component of the player, used to detect falls and calculate velocity.")]
        public Rigidbody RigidbodyComponent;

        [Tooltip("Reference to the FirstPersonController script, used to check player movement and grounding.")]
        public FirstPersonController FirstPersonControllerScript;

        [Tooltip("The player's health bar, used to display and manage the player's health status.")]
        public Stat PlayerHealthbar;

        [Tooltip("The duration for the fade-out effect of damage indicators on the screen.")]
        public float DamageEffectScreensFadeOutDuration = 1f;

        [Tooltip("The minimum time interval between damage indicator effects on the screen.")]
        public float TimeBetweenDamageEffectScreens = 5f;

        [System.Serializable]
        public class DamageEffectHealthProperties
        {
            [Tooltip("The minimum health value at which this effect is triggered.")]
            public float MinHealthToCheck;

            [Tooltip("The maximum health value at which this effect is triggered.")]
            public float MaxHealthToCheck;

            [Tooltip("Enable or disable rotation of blood screen effects.")]
            public bool RotateImpactEffectBloodScreens = true;

            [Tooltip("The minimum angle for rotating blood screen effects.")]
            public float MinImpactEffectBloodScreenAngle = 0f;

            [Tooltip("The maximum angle for rotating blood screen effects.")]
            public float MaxImpactEffectBloodScreenAngle = 360f;

            [Tooltip("Enable or disable scaling of blood screen effects.")]
            public bool ScaleImpactEffectBloodScreens = true;

            [Tooltip("The minimum scale for blood screen effects.")]
            public float MinImpactEffectBloodScreenScale = 0.5f;

            [Tooltip("The maximum scale for blood screen effects.")]
            public float MaxImpactEffectBloodScreenScale = 1f;

            [Tooltip("The array of UI images used for blood screen effects.")]
            public Image[] DamageEffectUIImages;
        }

        [Tooltip("A list of temporary damage effect properties based on the player's health.")]
        public DamageEffectHealthProperties[] TemporaryDamageEffectBloodScreens;

        [System.Serializable]
        public class DieAlgo
        {
            [Tooltip("UI elements to deactivate when the player dies.")]
            public GameObject[] UIToDeactivateOnDie;

            [Tooltip("The root object for all weapons, to deactivate upon death.")]
            public GameObject AllWeaponsRoot;

            [Tooltip("The player's dead arms object, activated upon death.")]
            public GameObject DeadArmsObj;

            [Tooltip("The rotation of the player's arms when dead.")]
            public Vector3 DeadArmsRotation;

            [Tooltip("The position of the player's arms when dead.")]
            public Vector3 DeadArmsPosition;

            [Tooltip("The duration for moving the player's arms to their dead position.")]
            public float DeadArmsMoveDuration;

            [Tooltip("The duration for rotating the player's arms to their dead rotation.")]
            public float DeadArmsRotationDuration;

            [Tooltip("The player bobber object, for simulating a death bob effect.")]
            public GameObject PlayerBobber;

            [Tooltip("The position of the player bobber when the player is dead.")]
            public Vector3 PlayerBobberPosition;

            [Tooltip("The rotation of the player bobber when the player is dead.")]
            public Vector3 PlayerBobberRotation;

            [Tooltip("The duration for moving the player bobber to its dead position.")]
            public float PlayerBobberMoveDuration;

            [Tooltip("The duration for rotating the player bobber to its dead rotation.")]
            public float PlayerBobberRotationDuration;

            [Tooltip("The player camera object, for simulating death animations.")]
            public GameObject PlayerCamera;

            [Tooltip("The position of the player camera when dead.")]
            public Vector3 PlayerCameraPosition;

            [Tooltip("The rotation of the player camera when dead.")]
            public Vector3 PlayerCameraRotation;

            [Tooltip("The duration for moving the player camera to its dead position.")]
            public float PlayerCameraMoveDuration;

            [Tooltip("The duration for rotating the player camera to its dead rotation.")]
            public float PlayerCameraRotationDuration;

            [Tooltip("Game objects to activate upon mission failure.")]
            public GameObject[] GameObjectToActivateOnMissionFail;

            [Tooltip("Game objects to deactivate upon mission failure.")]
            public GameObject[] GameObjectToDeactivateOnMissionFail;
        }

        [Tooltip("Properties for handling player death animations and effects.")]
        public DieAlgo PlayerDieProperties;

        [System.Serializable]
        public class ManageHealth
        {
            [Tooltip("The minimum health value to check.")]
            public float MinHealthToCheck;

            [Tooltip("The maximum health value to check.")]
            public float MaxHealthToCheck;

            [Tooltip("The UI element to display for this health range.")]
            public GameObject UIToShow;
        }

        [Tooltip("A list of permanent damage effect properties based on health ranges.")]
        public ManageHealth[] PermanentDamageEffectBloodScreens;

        [System.Serializable]
        public class FallAlgo
        {
            [Tooltip("The minimum velocity of the Rigidbody for a fall to cause damage.")]
            public float MinimumRigidbodyVelocity = 5f;

            [Tooltip("The maximum velocity of the Rigidbody for a fall to cause damage.")]
            public float MaximumRigidbodyVelocity = 10f;

            [Tooltip("The percentage of health to reduce upon falling within the velocity range.")]
            [Range(0, 100)]
            public float HealthToReduceInPercent = 10f;
        }

        [Tooltip("A list of fall damage properties based on Rigidbody velocity.")]
        public List<FallAlgo> FallingProperties = new List<FallAlgo>();

        [Tooltip("The minimum time to check if the player is grounded after falling.")]
        public float MinTimeToGroundCheck = 3f;

        [Tooltip("The maximum time to check if the player is grounded after falling.")]
        public float MaxTimeToGroundCheck = 4f;

        [Tooltip("Debug value to monitor the Rigidbody's vertical velocity.")]
        public float DebugRigidbodyVelocity;    

        private float fadeTimer; // Timer to manage the fade out
        private float effectTimer; // Timer to manage the delay before a new effect can be shown
        private Image currentlyFadingImage;
        private bool isFading;
        private bool canShowEffect = true;
        bool CanShowNewEffect = false;
        private bool IsfadeCompleted = true;
 
      
        bool playaudio = false;

        bool NowCheck = false;
        //float StoreFallHeight;
        bool CheckFallNow = false;
        FallAlgo TempfallingProperty;

        [HideInInspector]
        public bool IsDead = false;

        public void ResettingDescription()
        {
            ScriptInfo = "This Script Regulates The Player Health In game";
        }
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            if (transform != null)
            {
                // StoreFallHeight = transform.transform.position.y;
            }

        }
        public void EmptyFunctionToTest()
        {
            // Here is the code
        }
        private void Start()
        {
            // Initialize the effect timer
            effectTimer = TimeBetweenDamageEffectScreens;

            PlayerHealthbar.Initialize();
            CheckHealthProperties();

            float RepeatRate = Random.Range(MinTimeToGroundCheck, MaxTimeToGroundCheck);
            InvokeRepeating("CheckForFall", RepeatRate, RepeatRate);

            for(int x =0; x < TemporaryDamageEffectBloodScreens.Length; x++)
            {
                for(int y=0;y < TemporaryDamageEffectBloodScreens[x].DamageEffectUIImages.Length; y++)
                {
                    TemporaryDamageEffectBloodScreens[x].DamageEffectUIImages[y].raycastTarget = false;
                }
            }

            for (int x = 0; x < PermanentDamageEffectBloodScreens.Length; x++)
            {
                if(PermanentDamageEffectBloodScreens[x].UIToShow.GetComponent<Image>() != null)
                {
                    PermanentDamageEffectBloodScreens[x].UIToShow.GetComponent<Image>().raycastTarget = false;
                }
            }
        }
        public void CheckForFall()
        {
            float vel = Mathf.Abs(RigidbodyComponent.velocity.y);
            DebugRigidbodyVelocity = vel;
            if (FirstPersonControllerScript.GroundCheck == false)
            {
                foreach (FallAlgo f in FallingProperties)
                {

                    // if(CheckFallNow == false)
                    //{
                    if (vel >= f.MinimumRigidbodyVelocity && vel < f.MaximumRigidbodyVelocity)
                    {
                        CheckFallNow = true;
                    }

                    if (CheckFallNow == true)
                    {
                        if (FirstPersonControllerScript.GroundCheck == false)
                        {
                            NowCheck = true;
                            TempfallingProperty = f;
                            CheckFallNow = false;
                        }
                    }
                    //}

                }
            }
            else
            {
                if (NowCheck == true)
                {
                    float HealthToReduce = PlayerHealthbar.Maxvalue * TempfallingProperty.HealthToReduceInPercent / 100f;
                    PlayerHealthbar.Curvalue -= HealthToReduce;
                    CheckPlayerHealth();
                    NowCheck = false;
                    CheckFallNow = false;
                }
            }

        }
        IEnumerator DieAnims()
        {
            yield return new WaitForSeconds(0f);
            if (PlayerDieProperties.DeadArmsObj != null)
            {

                LeanTween.moveLocal(PlayerDieProperties.DeadArmsObj, PlayerDieProperties.DeadArmsPosition, PlayerDieProperties.DeadArmsMoveDuration);
                LeanTween.rotateLocal(PlayerDieProperties.DeadArmsObj, PlayerDieProperties.DeadArmsRotation, PlayerDieProperties.DeadArmsRotationDuration);

                //Vector3 campos = FpsCamAnimator.transform.localPosition;
                //campos.x = FpsCamPosition.x;
                //campos.y = FpsCamPosition.y;
                //campos.z = FpsCamPosition.z;
                //FpsCamAnimator.transform.localPosition = campos;

                LeanTween.moveLocal(PlayerDieProperties.PlayerCamera, PlayerDieProperties.PlayerCameraPosition, PlayerDieProperties.PlayerCameraMoveDuration);
                LeanTween.rotateLocal(PlayerDieProperties.PlayerCamera, PlayerDieProperties.PlayerCameraRotation, PlayerDieProperties.PlayerCameraRotationDuration);

                LeanTween.moveLocal(PlayerDieProperties.PlayerBobber, PlayerDieProperties.PlayerBobberPosition, PlayerDieProperties.PlayerBobberMoveDuration);
                LeanTween.rotateLocal(PlayerDieProperties.PlayerBobber, PlayerDieProperties.PlayerBobberRotation, PlayerDieProperties.PlayerBobberRotationDuration);

                for (int x = 0; x < PlayerDieProperties.UIToDeactivateOnDie.Length; x++)
                {
                    PlayerDieProperties.UIToDeactivateOnDie[x].gameObject.SetActive(false);
                }

                PlayerDieProperties.DeadArmsObj.SetActive(true);
            }
        }
        public void CheckPlayerHealth()
        {
            if (PlayerHealthbar.Curvalue <= 0)
            {
                if (playaudio == false)
                {
                    PlayerDieProperties.AllWeaponsRoot.SetActive(false);
                    if (PlayerManager.instance != null)
                    {
                        if (PlayerManager.instance.CurrentHoldingPlayerWeapon != null)
                        {
                            if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                            {
                                PlayerManager.instance.DefaultFov();
                            }
                        }
                    }

                    FirstPersonControllerScript.Speeds.WalkingSpeed = 0f;
                    FirstPersonControllerScript.Speeds.CrouchingSpeed = 0f;
                    FirstPersonControllerScript.Speeds.AimedWalkingSpeed = 0f;
                    FirstPersonControllerScript.Speeds.AimedCrouchingSpeed = 0f;
                    FirstPersonControllerScript.Speeds.RunSpeedOnStanding = 0f;
                    FirstPersonControllerScript.Speeds.RunSpeedOnCrouching = 0f;
                    LeanTween.cancelAll();
                    PlayerDieProperties.AllWeaponsRoot.SetActive(false);
                    StartCoroutine(DieAnims());
                    playaudio = true;

                }
                IsDead = true;
                CheckHealthProperties();
                StartCoroutine(Died());
            }
            else
            {
                CanShowNewEffect = true;
                CheckHealthProperties();
            }
          
        }
        void Update()
        {
            // Update the effect timer
            effectTimer -= Time.deltaTime;

            // Handle the fade-out transition independently
            if (isFading && currentlyFadingImage != null)
            {
                fadeTimer += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, fadeTimer / DamageEffectScreensFadeOutDuration);
                SetImageAlpha(currentlyFadingImage, alpha);

                // When fadeTimer exceeds fadeDuration, stop fading
                if (fadeTimer >= DamageEffectScreensFadeOutDuration)
                {  
                    SetImageAlpha(currentlyFadingImage, 0f);
                    currentlyFadingImage.gameObject.SetActive(false);
                    isFading = false;
                    IsfadeCompleted = true; // Mark fade as completed
                    currentlyFadingImage = null;
                }
            }

            // When the effect timer reaches zero, trigger the effect
            if (effectTimer <= 0f && IsfadeCompleted && CanShowNewEffect == true)
            {
                CanShowNewEffect = false;
                ShowEffects();
                effectTimer = TimeBetweenDamageEffectScreens; // Reset the effect timer
            }
            else if (CanShowNewEffect == true && effectTimer > 0f)
            {
                CanShowNewEffect = false;
            }
        }

        public void ShowEffects()
        {
            for(int x = 0;x < TemporaryDamageEffectBloodScreens.Length; x++)
            {
                if(PlayerHealthbar.Curvalue <= TemporaryDamageEffectBloodScreens[x].MaxHealthToCheck && PlayerHealthbar.Curvalue >= TemporaryDamageEffectBloodScreens[x].MinHealthToCheck)
                {
                    if (TemporaryDamageEffectBloodScreens[x].DamageEffectUIImages.Length == 0) return;

                    // Pick a random image from the array
                    Image randomImage = TemporaryDamageEffectBloodScreens[x].DamageEffectUIImages[Random.Range(0, TemporaryDamageEffectBloodScreens[x].DamageEffectUIImages.Length)];

                    // If the same image is picked, reset the fade timer and restart the fade
                    if (randomImage == currentlyFadingImage)
                    {
                        fadeTimer = 0f; // Reset the fade timer
                        SetImageAlpha(randomImage, 1f); // Immediately set alpha to 1 (fully visible)
                        currentlyFadingImage.gameObject.SetActive(true); // Ensure the image is active
                        isFading = true; // Start fading process
                        IsfadeCompleted = false; // Fade is in progress

                        if (currentlyFadingImage != null)
                        {
                            if (TemporaryDamageEffectBloodScreens[x].RotateImpactEffectBloodScreens == true)
                            {
                                float Randomise = Random.Range(TemporaryDamageEffectBloodScreens[x].MinImpactEffectBloodScreenAngle, TemporaryDamageEffectBloodScreens[x].MaxImpactEffectBloodScreenAngle);
                                Vector3 NewRot = currentlyFadingImage.transform.localEulerAngles;
                                NewRot.z = Randomise;
                                currentlyFadingImage.transform.localEulerAngles = NewRot;
                            }
                            if (TemporaryDamageEffectBloodScreens[x].ScaleImpactEffectBloodScreens == true)
                            {
                                float scale = Random.Range(TemporaryDamageEffectBloodScreens[x].MinImpactEffectBloodScreenScale, TemporaryDamageEffectBloodScreens[x].MaxImpactEffectBloodScreenScale);

                                Vector3 Randomise = new Vector3(scale, scale);

                                Vector3 NewRot = currentlyFadingImage.transform.localScale;
                                NewRot = Randomise;
                                currentlyFadingImage.transform.localScale = NewRot;
                            }
                        }
                        return;
                    }
                    else
                    {
                        // If a different image is picked, start fading the new image
                        if (currentlyFadingImage != null)
                        {
                            SetImageAlpha(currentlyFadingImage, Mathf.Lerp(1f, 0f, fadeTimer / DamageEffectScreensFadeOutDuration));
                        }
                        currentlyFadingImage = randomImage;
                        SetImageAlpha(currentlyFadingImage, 1f); // Set alpha to 1 (fully visible)
                        currentlyFadingImage.gameObject.SetActive(true);
                        fadeTimer = 0f; // Reset the fade timer
                        isFading = true;
                        IsfadeCompleted = false; // Fade is in progress


                        if (currentlyFadingImage != null)
                        {
                            if (TemporaryDamageEffectBloodScreens[x].RotateImpactEffectBloodScreens == true)
                            {
                                float Randomise = Random.Range(TemporaryDamageEffectBloodScreens[x].MinImpactEffectBloodScreenAngle, TemporaryDamageEffectBloodScreens[x].MaxImpactEffectBloodScreenAngle);
                                Vector3 NewRot = currentlyFadingImage.transform.localEulerAngles;
                                NewRot.z = Randomise;
                                currentlyFadingImage.transform.localEulerAngles = NewRot;
                            }

                            if (TemporaryDamageEffectBloodScreens[x].ScaleImpactEffectBloodScreens == true)
                            {
                                float scale = Random.Range(TemporaryDamageEffectBloodScreens[x].MinImpactEffectBloodScreenScale, TemporaryDamageEffectBloodScreens[x].MaxImpactEffectBloodScreenScale);

                                Vector3 Randomise = new Vector3(scale, scale);

                                Vector3 NewRot = currentlyFadingImage.transform.localScale;
                                NewRot = Randomise;
                                currentlyFadingImage.transform.localScale = NewRot;
                            }
                        }

                        return;
                    }

                }
            }
           
            //if (DamageEffectUIImages.Length == 0) return; // If no images are assigned, do nothing

            //// Pick a random image from the array
            //Image randomImage = DamageEffectUIImages[Random.Range(0, DamageEffectUIImages.Length)];

            //// If the same image is picked, reset the fade timer and restart the fade
            //if (randomImage == currentlyFadingImage)
            //{
            //    fadeTimer = 0f; // Reset the fade timer
            //    SetImageAlpha(randomImage, 1f); // Immediately set alpha to 1 (fully visible)
            //    currentlyFadingImage.gameObject.SetActive(true); // Ensure the image is active
            //    isFading = true; // Start fading process
            //    IsfadeCompleted = false; // Fade is in progress
            //}
            //else
            //{
            //    // If a different image is picked, start fading the new image
            //    if (currentlyFadingImage != null)
            //    {
            //        SetImageAlpha(currentlyFadingImage, Mathf.Lerp(1f, 0f, fadeTimer / TemporaryDamageEffectImageFadeDuration));
            //    }
            //    currentlyFadingImage = randomImage;
            //    SetImageAlpha(currentlyFadingImage, 1f); // Set alpha to 1 (fully visible)
            //    currentlyFadingImage.gameObject.SetActive(true);
            //    fadeTimer = 0f; // Reset the fade timer
            //    isFading = true;
            //    IsfadeCompleted = false; // Fade is in progress
            //}
        }

        private void SetImageAlpha(Image image, float alpha)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
        //void Update()
        //{
        //    // Update the effect timer
        //    effectTimer -= Time.deltaTime;

        //    // When the effect timer reaches zero, trigger the effect
        //    if (effectTimer <= 0f && CanShowNewEffect == true)
        //    {
        //        ShowEffects();
        //        effectTimer = TimeIntervalBetweenEffect; // Reset the effect timer
        //        CanShowNewEffect = false;
        //    }
        //    else if(CanShowNewEffect == true && effectTimer > 0f)
        //    {
        //        CanShowNewEffect = false;
        //    }

        //    // Handle the fade-out transition independently
        //    if (isFading && currentlyFadingImage != null)
        //    {
        //        fadeTimer += Time.deltaTime;
        //        float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
        //        SetImageAlpha(currentlyFadingImage, alpha);

        //        // When fadeTimer exceeds fadeDuration, stop fading
        //        if (fadeTimer >= fadeDuration)
        //        {
        //            SetImageAlpha(currentlyFadingImage, 0f);
        //            currentlyFadingImage.gameObject.SetActive(false);
        //            isFading = false;
        //            currentlyFadingImage = null;
        //        }
        //    }
        //}

        //public void ShowEffects()
        //{
        //    if (uiImages.Length == 0) return; // If no images are assigned, do nothing

        //    // Pick a random image from the array
        //    Image randomImage = uiImages[Random.Range(0, uiImages.Length)];

        //    // If the same image is picked, reset the fade timer and restart the fade
        //    if (randomImage == currentlyFadingImage)
        //    {
        //        fadeTimer = 0f; // Reset the fade timer
        //        SetImageAlpha(randomImage, 1f); // Immediately set alpha to 1 (fully visible)
        //        currentlyFadingImage.gameObject.SetActive(true); // Ensure the image is active
        //        isFading = true; // Start fading process
        //    }
        //    else
        //    {
        //        // If a different image is picked, start fading the new image
        //        if (currentlyFadingImage != null)
        //        {
        //            SetImageAlpha(currentlyFadingImage, Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration));
        //        }
        //        currentlyFadingImage = randomImage;
        //        SetImageAlpha(currentlyFadingImage, 1f); // Set alpha to 1 (fully visible)
        //        currentlyFadingImage.gameObject.SetActive(true);
        //        fadeTimer = 0f; // Reset the fade timer
        //        isFading = true;
        //    }
        //}

        //private void SetImageAlpha(Image image, float alpha)
        //{
        //    Color color = image.color;
        //    color.a = alpha;
        //    image.color = color;
        //}
        IEnumerator Died()
        {
            yield return new WaitForSeconds(0.1f);
            if (WinningProperties.instance != null)
            {
                if (WinningProperties.instance.IsLevelCompleted == false)
                {
                    for (int x = 0; x < PlayerDieProperties.GameObjectToActivateOnMissionFail.Length; x++)
                    {
                        PlayerDieProperties.GameObjectToActivateOnMissionFail[x].SetActive(true);
                    }
                    for (int x = 0; x < PlayerDieProperties.GameObjectToDeactivateOnMissionFail.Length; x++)
                    {
                        PlayerDieProperties.GameObjectToDeactivateOnMissionFail[x].SetActive(false);
                    }
                }
            }
            else
            {
                for (int x = 0; x < PlayerDieProperties.GameObjectToActivateOnMissionFail.Length; x++)
                {
                    PlayerDieProperties.GameObjectToActivateOnMissionFail[x].SetActive(true);
                }
                for (int x = 0; x < PlayerDieProperties.GameObjectToDeactivateOnMissionFail.Length; x++)
                {
                    PlayerDieProperties.GameObjectToDeactivateOnMissionFail[x].SetActive(false);
                }
            }
        }
        public void CheckHealthProperties()
        {
            for (int i = 0; i < PermanentDamageEffectBloodScreens.Length; i++)
            {
                if (PlayerHealthbar.Curvalue <= PermanentDamageEffectBloodScreens[i].MaxHealthToCheck && PlayerHealthbar.Curvalue >= PermanentDamageEffectBloodScreens[i].MinHealthToCheck
                    || PlayerHealthbar.Curvalue == PermanentDamageEffectBloodScreens[i].MaxHealthToCheck && PlayerHealthbar.Curvalue == PermanentDamageEffectBloodScreens[i].MinHealthToCheck)
                {
                    PermanentDamageEffectBloodScreens[i].UIToShow.SetActive(true);
                }
                else
                {
                    PermanentDamageEffectBloodScreens[i].UIToShow.SetActive(false);
                }
            }
        }
    }
}