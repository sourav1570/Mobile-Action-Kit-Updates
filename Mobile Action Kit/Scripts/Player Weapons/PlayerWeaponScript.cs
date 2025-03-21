using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

// This Script is Responsible For Weapon Shooting 
namespace MobileActionKit
{
    public class PlayerWeapon : MonoBehaviour
    {
        public static PlayerWeapon instance;

        [TextArea]   
        public string ScriptInfo = "Player Weapon Script is the main script responsible for all kinds of weapons available to player, " +
            "such as various kinds of firearms, grenades & melee weapons. It defines majority of aspects of weapons overall functionality & " +
            "communicates with number of auxiliary scripts that are handling additional elements of the player weapons functionality. ";
        [Space(10)]

        [HideInInspector]
        public ObjectPooler pooler;

        public RaycastHit hit;
        [HideInInspector]
        public bool shootnow = false;
        private float Nexttimetofire = 0f;
        [HideInInspector]
        public bool playershoot;
        [System.Serializable]
        public class RequireComponents
        {
            public WeaponId WeaponIdScript;
            public Button FireButton;
            public Camera PlayerCamera;
            public Camera WeaponCamera;
            public AlertingSoundActivator AlertingSoundActivatorComponent;
            public Animator WeaponAnimatorComponent;
            public AttachmentsActivator AttachmentsActivatorComponent;
            public RecoilScript RecoilComponent;
        }

        public RequireComponents RequiredComponents = new RequireComponents();

        [System.Serializable]
        public class AboutWeapon
        {
            [Tooltip("Shooting Range of weapon if it uses raycast.")]
            public float RaycastShootingRange = 100f;

            public bool UseMuzzleFlashMesh = false;
            public ParticleSystem MuzzleFlashParticleFX;
            public GameObject MuzzleFlashMesh;
           

            [Tooltip("Affects the Weapon Fire Rate In Both Raycast and projectile Shooting Functionality")]
            public float FullAutoFireRate = 15f;

            public float MuzzleFlashMeshActiveTime = 0.2f;
            [Tooltip("Damage To target on shoot")]
            public float TargetDamagePerShot = 5f;

            [Tooltip("If The Bullet Hits On Other Colliders For Ex - Road , Houses etc. Than Which Prefab To Spawn From Object Pooler on that hit point - Prefab could be Ex - BulletHole")]
            public string DefaultImpactEffectName = "BulletHole";

            [Tooltip("'MuzzleFlashRotator' game object needs to be Dragged & Dropped into this field.")]
            public GameObject MuzzleFlashRotatingObject;

            [Tooltip("Value of maximal possible rotational angle offset of the MuzzleFlashRotator gameobject in negative Z axis per weapon shot.")]
            public float MuzzleFlashRotatorMaxNegativeAngle = -180f;

            [Tooltip("Value of maximal possible rotational angle offset of the MuzzleFlashRotator gameobject in positive Z axis per weapon shot.")]
            public float MuzzleFlashRotatorMaxPositiveAngle = 180f;

            public bool CanRotatePlayerWithPressedFireButton = false;

            public bool UseCrosshair = true;
            [Tooltip("This field is populated with advanced crosshair UI element but if you want to use the simple crosshair you can drag&drop it into this field instead.")]
            public GameObject WeaponCrosshair;

            [Tooltip("Specify what layers raycast shooting will be affected by.Unchecked layers will be ignored.")]
            public LayerMask RaycastShootingVisibleLayers;

            //[Tooltip("This field is automatically populated with  shooting point gameobject of the weapon.If you want shooting to happen from the middle of the screen then make this field to be empty.")]
            //public GameObject HipFireRaycastShotsSpawnPosition;
            //[Tooltip("This field is automatically populated with  shooting point gameobject of the weapon.If you want shooting to happen from the middle of the screen then make this field to be empty.")]
            //public GameObject AimedFireRaycastShotsSpawnPosition;

            [Tooltip("If checked weapon will shoot in full automatic mode.")]
            public bool ForceFullAutoFire;

            [Tooltip("Value in seconds during which weapon idle animation clip inside weapon 'Idle' animator state will continue to be played before switching to whatever animation clip is inside" +
                " 'InitialPose' animator state. It can be animation clip without any motion in it in case you plan to use only procedural animation for shaking aimed idle weapon. " +
                "Or with motion(to be played back without using procedural animation or in a combination of the two).")]
            public float IdleToAimedAnimationTransitionDelay = 0f;

            public bool UseSniperScopeUI = false;
            public SniperScope SniperScopeScript;

            public bool UseBlurEffectOnAim = true;
            public GameObject BlurEffect;
         
        }

        public AboutWeapon ShootingFeatures;
        private Color defaultStartColor = Color.red; // Default start color
        private Color defaultEndColor = Color.red;   // Default end color

        [System.Serializable]
        public class BulletShellClass
        {
            [Tooltip("Bullet Shell Name Inside Object Pooler Script")]
            public string BulletShellName = "BulletShell";
            public GameObject BulletShellSpawnPoint;
            public float ShellsEjectingSpeed = 10f;
        }

        public BulletShellClass BulletShell;
        bool DoOnce = false;

        public enum ShootingOptions
        {
            RaycastShooting,
            ProjectileShooting
        }

        [System.Serializable]
        public class ShootingTypesClass
        {
            [Tooltip("Name In this field should match the name of the respective field inside object pooler script in order for the shooting to function properly.")]
            public string ProjectileName = "Bullet";

            [Tooltip("Takes effect only if this weapon is utilising 'ProjectileShooting'. Number of projectiles per each shot of the weapon. Useful for weapons that can fire more than one bullet per shot(mostly shotguns). The spread of those projectiles is specified in 'BulletSpread' paragraph below.")]
            public int ProjectilesPerShot = 1;

            [Tooltip("Select one of the three available options for the kind of shooting that this weapon will utilise. RaycastShootingWithTracers is an option where those tracers are not dealing damage to targets and are there only for visuals.")]
            public ShootingOptions ShootingOption;
        }

        public ShootingTypesClass ShootingMechanics;


        [System.Serializable]
        public class RaycastForceClass
        {
            [Tooltip("If checked will apply raycast force to targets with rigidbody.(AI agents as well as any non AI gameobjects with rigidbody attached(cans,boxes etc)." +
                "& will do so with random force between Min & Max values set in the fields of this paragraph.")]
            public bool AddRaycastForce = false;

            //[Tooltip("Minimum overall force to apply")]
            //public float MinRaycastForce = 800;
            //[Tooltip("Maximum overall force to apply")]
            //public float MaxRaycastForce = 1100;

            //[Tooltip("Minimum force to apply in upward direction.")]
            //public float MinUpwardForceToApplyOnTarget = 2f;
            //[Tooltip("Maximum force to apply in upward direction.")]
            //public float MaxUpwardForceToApplyOnTarget = 5f;

            //[Tooltip("Minimum force to apply in right direction.")]
            //public float MinRightForceToApplyOnTarget = 2f;
            //[Tooltip("Maximum force to apply in right direction.")]
            //public float MaxRightForceToApplyOnTarget = 2f;

            //[Tooltip("Minimum force to apply in left direction.")]
            //public float MinLeftForceToApplyOnTarget = 1f;
            //[Tooltip("Maximum force to apply in left direction.")]
            //public float MaxLeftForceToApplyOnTarget = 1f;

            //[Tooltip("Minimum force to apply in backward direction.")]
            //public float MinBackwardForceToApplyOnTarget = 5f;
            //[Tooltip("Maximum force to apply in backward direction.")]
            //public float MaxBackwardForceToApplyOnTarget = 8f;

            //[Tooltip("Minimum force to apply in forward direction.")]
            //public float MinForwardForceToApplyOnTarget = 1f;
            //[Tooltip("Maximum force to apply in forward direction.")]
            //public float MaxForwardForceToApplyOnTarget = 1f;

            [Tooltip("Minimal possible base force value that gets multiplied by the values of all the fields below except the last one that is not related to Ai.This last field will apply force specified in it directly as is. ")]
            public float MinRaycastForceToAi = 8f;
            [Tooltip("Maximal possible base force value that gets multiplied by the values of all the fields below except the last one that is not related to Ai.This last field will apply force specified in it directly as is.")]
            public float MaxRaycastForceToAi = 8f;

            public float NonAIGameObjectImpactForce = 4f;

            public float RadiusToApplyForce = 50f;

            [HideInInspector]
            public List<Transform> TargetsToApplyForce = new List<Transform>();

        }

        [Tooltip("Applicable in case RaycastShooting option is chosen for this weapon. There are two ways of applying raycast force. " +
            "One for Ai agents where you can specify exact directions of the force. All fields of this paragraph except the last one are there for this first Ai related category." +
            "Second way is intended for non AI gameobjects with rigid body attached to them(crates, cans, etc).It is set inside last 'NonAIGameObjectImpactForce' field of this paragraph.")]
        public RaycastForceClass RaycastForce;

        [HideInInspector]
        public bool IsFire = false;
        private Targets EnemyIDScript;
        [HideInInspector]
        public int StoreTeamId;
        [HideInInspector]
        public bool UpdateKills = false;
    
        [HideInInspector]
        public bool IsWalking = false;

        float Timer = 0;
        [HideInInspector]
        public bool ShotMade = false;


        [HideInInspector]
        public bool IsAimed = false;
        [HideInInspector]
        public bool IsHipFire = false;

        float horizontal, vertical, timer, waveSlice;

        [HideInInspector]
        public float ObjectToShakeMidPoint = 2.0f;

        //bool ShotMaded = false;
        float ShotLength;
        float ReloadLength;
        float ReloadEmptyLength;
        [HideInInspector]
        public float RemoveLength;

        bool IsFireAnimNameOk = false;
        bool IsReloadAnimNameOk = false;
        bool IsReloadEmptyAnimNameOk = false;
        bool Wield = false;
        bool Remove = false;

        [HideInInspector]
        public bool UseProceduralAimedBreath = false;
        //[HideInInspector]
        //public Transform ObjectToAnimate;
        [HideInInspector]
        public float ShiftSpeed;
        float SaveShiftSpeed;

        [HideInInspector]
        public float RotationSpeed;

        [HideInInspector]
        public bool CombineProceduralBreathWithAnimation = false;

        [System.Serializable]
        public class ReloadingClass
        {
           
            public int FirstMagazineSize = 30;

            public int FullMagazineSize = 30;
            [HideInInspector]
            public int CurrentAmmos;
            [HideInInspector]
            public bool isreloading = false;

            public int TotalAmmo = 60;
            public TextMeshProUGUI TotalAmmoText;

            public TextMeshProUGUI ShotsInMagCountText;

            [HideInInspector]
            public bool Reloadation = false;

            [HideInInspector]
            public bool ForceReloadPositioning = false;

            public Vector3 WeaponPivotReloadPosition;
            public Vector3 WeaponPivotReloadRotation;

           // public bool UseLerpTimer = true;
            //public float TimeToStartLerpingToIdle = 3f;
            [Tooltip("Linear Interpolation speed between weapon idle & weapon reloading positions.")]
            public float WeaponPositionLerpSpeed = 2f;
            [Tooltip("Linear Interpolation speed between weapon idle & weapon reloading rotations.")]
            public float WeaponRotationLerpSpeed = 2f;

            public float WeaponResetPositioningSpeed = 2f;

            public Button[] ButtonsToDisableInteractionOnReload;
            //public AudioSource MagazineDropSound;
            //public float MagazineDropSoundDelay;
        }

        public ReloadingClass Reload;
        
        [Tooltip("If you want to introduce bullet spread effect for each consecutive shot of the weapon or making the shotgun then this paragraph can be set to change shooting point`s rotation to " +
            "various angles between min/max limitations specified in respective fields.")]
        public BulletSreadOptions BulletSpread;

        [System.Serializable]
        public class Sounds
        {
            public AudioSource FireSoundAudioSource;
            public AudioSource ReloadSoundAudioSource;
            public AudioSource ReloadEmptySoundAudioSource;
            public AudioSource WieldSoundAudioSource;

            [HideInInspector]
            public AudioClip FireAudioClip;
            [HideInInspector]
            public AudioClip ReloadAudioClip;
            [HideInInspector]
            public AudioClip ReloadEmptyAudioClip;
            [HideInInspector]
            public AudioClip WieldAudioClip;
        }

        public Sounds WeaponSounds = new Sounds();

        [HideInInspector]
        public string WeaponAimedAnimationName = "InitialPose";
        [HideInInspector]
        public string IdleAnimationName = "Idle";
        [HideInInspector]
        public string WieldAnimationName = "Wield";
        [HideInInspector]
        public string FireAnimationName = "Fire";
        [HideInInspector]
        public string ReloadAnimationName = "Reload";
        [HideInInspector]
        public string ReloadEmptyAnimationName = "ReloadEmpty";
        [HideInInspector]
        public string WalkingAnimationName = "Walk";
        [HideInInspector]
        public string RunAnimationName = "Run";
        [HideInInspector]
        public string RemoveAnimationName = "Remove";
        [HideInInspector]
        public string IdleAnimationParametreName = "Idle";
        [HideInInspector]
        public string WalkAnimationParametreName = "Walk";
        [HideInInspector]
        public string RunAnimationParametreName = "Run";

        [System.Serializable]
        public class Positions
        {
     
            public GameObject WeaponPivot;

            public Vector3 AimedWeaponPivotPosition;
            public Vector3 AimedWeaponPivotRotation;

            public Vector3 HipFireWeaponPivotPosition;
            public Vector3 HipFireWeaponPivotRotation;

            public BobDurValues WeaponDurations;

        }


        //[System.Serializable]
        //public class GameObjectPositionsClass
        //{
         
        //    public GameObject PlayerManager;
           
        //    public Vector3 AimedPosition;
        //    public Vector3 AimedRotation;
        //    public Vector3 HipFirePosition;
        //    public Vector3 HipFireRotation;

        //}

        public Positions WeaponPositionsRotationsDurations;
       // public GameObjectPositionsClass PlayerManagerPositions;
        public CamerasFovParametersClass CamerasFovParameters;

        [System.Serializable]
        public class CamerasFovParametersClass
        {
            [Header("Player Camera Fov Parameters")]
            public float DefaultPlayerCamFov = 60f;
            public float PlayerCameraMagnifiedFov = 15f;
            public float PlayerCameraFovChangeSpeed = 7f;

            [Header("Weapon Camera Fov Parameters")]
            public float DefaultWeaponCamFov = 60f;
            public float WeaponCameraMagnifiedFov = 10f;
            public float WeaponCameraFovChangeSpeed = 6f;
        }

        [System.Serializable]
        public class BobDurValues
        {
            public float AimedXShiftDuration;
            public float AimedYShiftDuration;
            public float AimedZShiftDuration;
            public float AimedRotDuration;


            public float HipFireXShiftDuration;
            public float HipFireYShiftDuration;
            public float HipFireZShiftDuration;
            public float HipFireRotDuration;
        }

        [HideInInspector]
        public Vector3 MinimalNegativePositionalValues;
        [HideInInspector]
        public Vector3 MaximumNegativePositionalValues;
        [HideInInspector]
        public Vector3 MinimalPositivePositionalValues;
        [HideInInspector]
        public Vector3 MaximumPositivePositionalValues;

        [HideInInspector]
        public Vector3 MinimalNegativeRotationalValues;
        [HideInInspector]
        public Vector3 MaximumNegativeRotationalValues;
        [HideInInspector]
        public Vector3 MinimalPositiveRotationalValues;
        [HideInInspector]
        public Vector3 MaximumPositiveRotationalValues;

        [HideInInspector]
        public float MidXNegativeAxis;
        [HideInInspector]
        public float MidXPositiveAxis;
        [HideInInspector]
        public float MidYNegativeAxis;
        [HideInInspector]
        public float MidYPositiveAxis;
        [HideInInspector]
        public float MidZNegativeAxis;
        [HideInInspector]
        public float MidZPositiveAxis;

        [HideInInspector]
        public float MidXrotNegativeAxis;
        [HideInInspector]
        public float MidXrotPositiveAxis;
        [HideInInspector]
        public float MidYrotNegativeAxis;
        [HideInInspector]
        public float MidYrotPositiveAxis;
        [HideInInspector]
        public float MidZrotNegativeAxis;
        [HideInInspector]
        public float MidZrotPositiveAxis;


        [HideInInspector]
        public float MaxPosXNegativeAxis;
        [HideInInspector]
        public float MaxPosXPositiveAxis;
        [HideInInspector]
        public float MaxPosYNegativeAxis;
        [HideInInspector]
        public float MaxPosYPositveAxis;
        [HideInInspector]
        public float MaxPosZNegativeAxis;
        [HideInInspector]
        public float MaxPosZPositveAxis;
        [HideInInspector]
        public float MaxRotXNegativeAxis;
        [HideInInspector]
        public float MaxRotXPositiveAxis;
        [HideInInspector]
        public float MaxRotYNegativeAxis;
        [HideInInspector]
        public float MaxRotYPositiveAxis;
        [HideInInspector]
        public float MaxRotZNegativeAxis;
        [HideInInspector]
        public float MaxRotZPositiveAxis;

        [HideInInspector]
        public float PreviousX;
        [HideInInspector]
        public float PreviousY;
        [HideInInspector]
        public float PreviousZ;

        [HideInInspector]
        public float PreviousXRot;
        [HideInInspector]
        public float PreviousYRot;
        [HideInInspector]
        public float PreviousZRot;

        [HideInInspector]
        public bool LoopX;
        [HideInInspector]
        public bool LoopY;
        [HideInInspector]
        public bool LoopZ;

        [HideInInspector]
        public bool LoopXRot;
        [HideInInspector]
        public bool LoopYRot;
        [HideInInspector]
        public bool LoopZRot;

        [HideInInspector]
        public float WieldTime;
        [HideInInspector]
        public bool IsplayingAimedAnim = false;

        float StoreWeaponResetingSpeed;
        //bool LerpBack = false;
        bool LerpNow = false;

        bool isReloadCompleted = false;

        private CrossHair Ch;

        [System.Serializable]
        public class BulletSreadOptions
        {
            public bool UseBulletSpread = true;

            [Header("Player Standing")]
            public float MinBulletSpreadRotationXStanding = -3f;
            public float MaxBulletSpreadRotationXStanding = 3f;
            public float MinBulletSpreadRotationYStanding = -88f;
            public float MaxBulletSpreadRotationYStanding = -92f;

            [Header("Player Crouched")]
            public float MinBulletSpreadRotationXCrouched = -3f;
            public float MaxBulletSpreadRotationXCrouched = 3f;
            public float MinBulletSpreadRotationYCrouched = -88f;
            public float MaxBulletSpreadRotationYCrouched = -92f;
        }



        [HideInInspector]
        public bool AimedAnimState = true;

        //public RaycastHit StoreHit;
        //bool ShouldSpawnImpactEffect = true;

        bool CanRaycastPass = false;

        bool CanTweakAimedPositionAndRotations = false;
        bool CanTweakHipFirePositionAndRotations = false;


        [System.Serializable]
        public class AnimationSpeedClass
        {
            [HideInInspector]
            public string RunSpeedParameterName = "RunSpeed";
            [HideInInspector]
            public string WalkSpeedParameterName = "WalkSpeed";
            public float StandRunAnimationSpeed = 1f;
            public float CrouchRunAnimationSpeed = 0.5f;
            public float StandWalkAnimationSpeed = 1f;
            public float CrouchWalkAnimationSpeed = 0.5f;

            [HideInInspector]
            public string AimedAnimationName = "Aimed";
            [HideInInspector]
            public string AimedAnimationParametreName = "PlayAimed";
            [HideInInspector]
            public string SpeedParameterName = "AimedSpeed";
            [HideInInspector]
            public string IdleAnimationSpeed = "IdleAnimationSpeed";

            public float StandAimedAnimationSpeed = 1f;
            public float CrouchAimedAnimationSpeed = 1f;
            public float StandAimedWalkAnimationSpeed = 1f;
            public float CrouchAimedWalkAnimationSpeed = 1f;

            public float IdleAnimationSpeedWhenHipToAimed = 10f;
            public float IdleAnimationSpeedWhenAimedToHip = 1f;
        }

        public AnimationSpeedClass WeaponAnimationClipsSpeeds = new AnimationSpeedClass();

        AudioSource aud;

        string TempWeaponAimedAnimationName = "InitialPose";
        string TempIdleAnimationName = "Idle";
        string TempWieldAnimationName = "Wield";
        string TempFireAnimationName = "Fire";
        string TempReloadAnimationName = "Reload";
        string TempReloadEmptyAnimationName = "ReloadEmpty";
        string TempWalkingAnimationName = "Walk";
        string TempRunAnimationname = "Run";
        string TempRemoveAnimationName = "Remove";
        string TempIdleAnimationParameterName = "Idle";


        bool StartMuzzleMeshTimer = false;

        bool IsProjectileShooting = false;

        [HideInInspector]
        public bool IsShootingNotAllowed = false;

        PlayerManager PlayerManagerObject;

        Vector3 shootingDirection;
        LineRenderer lineRenderer;

        Vector3 start;
        Vector3 end;

         Vector3 DefaultAimedWeaponPivotPosition;
        Vector3 DefaultAimedWeaponPivotRotation;
        bool DefaultBlurEffect;
        GameObject DefaultBlurEffectGameObject;
        bool DefaultSniperScope;
        float DefaultPlayerCameraFovChangeSpeed;
        float DefaultPlayerCameraMagnifiedFov;
        float DefaultWeaponCameraFovChangeSpeed;
        float DefaultWeaponCameraMagnifiedFov;

        bool IsDefaultValuesSaved = false;

        [HideInInspector]
        public bool DoNotResetValues = false;

        [HideInInspector]
        public bool StartShooting = false;

        [HideInInspector]
        public bool NotAShootingWeapon = false; // required when having PC or console controllers so in case we select the grenade hands and if the grenads hands have the player weapon script than we make sure to disable the aiming and shooting buttons.

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }                   
        }
        private void OnDisable()
        {
            DoNotResetValues = false;
            if (ShootingFeatures.BlurEffect != null)
            {
                if(ShootingFeatures.UseBlurEffectOnAim == true)
                {
                    ShootingFeatures.BlurEffect.gameObject.SetActive(false);
                }
              
            }
        }
        // Call in weapon optics script
        public void StoreDefaultValues() 
        {
            if(IsDefaultValuesSaved == false)
            {
                DefaultAimedWeaponPivotPosition = WeaponPositionsRotationsDurations.AimedWeaponPivotPosition;
                DefaultAimedWeaponPivotRotation = WeaponPositionsRotationsDurations.AimedWeaponPivotRotation;
                DefaultBlurEffect = ShootingFeatures.BlurEffect;
                if (ShootingFeatures.BlurEffect != null)
                {
                    DefaultBlurEffectGameObject = ShootingFeatures.BlurEffect;
                }
                else
                {
                    DefaultBlurEffectGameObject = null;
                }

                DefaultSniperScope = ShootingFeatures.UseSniperScopeUI;
                DefaultPlayerCameraFovChangeSpeed = CamerasFovParameters.PlayerCameraFovChangeSpeed;
                DefaultPlayerCameraMagnifiedFov = CamerasFovParameters.PlayerCameraMagnifiedFov;
                DefaultWeaponCameraFovChangeSpeed = CamerasFovParameters.WeaponCameraFovChangeSpeed;
                DefaultWeaponCameraMagnifiedFov = CamerasFovParameters.WeaponCameraMagnifiedFov;
                IsDefaultValuesSaved = true;
            }
        
        }
        // Required for weapon pickup case
        public void ResetAimedWeaponPositionsAndRotations()
        {
            if(DoNotResetValues == false)
            {
                ShootingFeatures.UseBlurEffectOnAim = DefaultBlurEffect;

                if (DefaultBlurEffectGameObject != null)
                {
                    ShootingFeatures.BlurEffect = DefaultBlurEffectGameObject;
                }
                else
                {
                    ShootingFeatures.BlurEffect = null;
                }


                ShootingFeatures.UseSniperScopeUI = DefaultSniperScope;
                WeaponPositionsRotationsDurations.AimedWeaponPivotPosition = DefaultAimedWeaponPivotPosition;
                WeaponPositionsRotationsDurations.AimedWeaponPivotRotation = DefaultAimedWeaponPivotRotation;

                CamerasFovParameters.WeaponCameraFovChangeSpeed = DefaultWeaponCameraFovChangeSpeed;
                CamerasFovParameters.WeaponCameraMagnifiedFov = DefaultWeaponCameraMagnifiedFov;
                CamerasFovParameters.PlayerCameraFovChangeSpeed = DefaultPlayerCameraFovChangeSpeed;
                CamerasFovParameters.PlayerCameraMagnifiedFov = DefaultPlayerCameraMagnifiedFov;
                DoNotResetValues = true;
            }

        }
        private void Start()
        {
            if (WeaponSounds.FireSoundAudioSource != null)
            {
                WeaponSounds.FireAudioClip = WeaponSounds.FireSoundAudioSource.clip;
            }
            if (WeaponSounds.ReloadSoundAudioSource != null)
            {
                WeaponSounds.ReloadAudioClip = WeaponSounds.ReloadSoundAudioSource.clip;
            }
            if (WeaponSounds.ReloadEmptySoundAudioSource != null)
            {
                WeaponSounds.ReloadEmptyAudioClip = WeaponSounds.ReloadEmptySoundAudioSource.clip;
            }
            if (WeaponSounds.WieldSoundAudioSource != null)
            {
                WeaponSounds.WieldAudioClip = WeaponSounds.WieldSoundAudioSource.clip;
            }

            start = transform.position;


           

            if (ShootingMechanics.ShootingOption == ShootingOptions.ProjectileShooting)
            {
                IsProjectileShooting = true;
            }


            EnemyIDScript = transform.root.gameObject.GetComponent<Targets>();
            pooler = ObjectPooler.instance;

            if (TeamMatch.instance != null)
            {
                if (TeamMatch.instance.EnableScoreSystemBetweenTeamsAsWinCondition == true)
                {
                    for (int i = 0; i < TeamMatch.instance.Teams.Count; i++)
                    {
                        if (TeamMatch.instance.Teams[i].TeamName == EnemyIDScript.MyTeamID)
                        {
                            StoreTeamId = i;
                        }
                    }
                }
            }


            //if (ObjectToAnimate != null)
            //{
            //    PreviousX = ObjectToAnimate.transform.localPosition.x;
            //    PreviousY = ObjectToAnimate.transform.localPosition.y;
            //    PreviousZ = ObjectToAnimate.transform.localPosition.z;

            //    PreviousXRot = ObjectToAnimate.transform.localEulerAngles.x;
            //    PreviousYRot = ObjectToAnimate.transform.localEulerAngles.y;
            //    PreviousZRot = ObjectToAnimate.transform.localEulerAngles.z;
            //}

            MaxPosXNegativeAxis = Random.Range(MinimalNegativePositionalValues.x, MaximumNegativePositionalValues.x);
            MaxPosXPositiveAxis = Random.Range(MinimalPositivePositionalValues.x, MaximumPositivePositionalValues.x);
            MaxPosYNegativeAxis = Random.Range(MinimalNegativePositionalValues.y, MaximumNegativePositionalValues.y);
            MaxPosYPositveAxis = Random.Range(MinimalPositivePositionalValues.y, MaximumPositivePositionalValues.y);
            MaxPosZNegativeAxis = Random.Range(MinimalNegativePositionalValues.z, MaximumNegativePositionalValues.z);
            MaxPosZPositveAxis = Random.Range(MinimalPositivePositionalValues.z, MaximumPositivePositionalValues.z);


            MaxPosXNegativeAxis = Random.Range(MinimalNegativeRotationalValues.x, MaximumNegativeRotationalValues.x);
            MaxPosXPositiveAxis = Random.Range(MinimalPositiveRotationalValues.x, MaximumPositiveRotationalValues.x);
            MaxPosYNegativeAxis = Random.Range(MinimalNegativeRotationalValues.y, MaximumNegativeRotationalValues.y);
            MaxPosYPositveAxis = Random.Range(MinimalPositiveRotationalValues.y, MaximumPositiveRotationalValues.y);
            MaxPosZNegativeAxis = Random.Range(MinimalNegativeRotationalValues.z, MaximumNegativeRotationalValues.z);
            MaxPosZPositveAxis = Random.Range(MinimalPositiveRotationalValues.z, MaximumPositiveRotationalValues.z);

            MidXNegativeAxis = Random.Range(MinimalNegativePositionalValues.x, MaximumNegativePositionalValues.x);
            MidXPositiveAxis = Random.Range(MinimalPositivePositionalValues.x, MaximumPositivePositionalValues.x);
            MidYNegativeAxis = Random.Range(MinimalNegativePositionalValues.y, MaximumNegativePositionalValues.y);
            MidYPositiveAxis = Random.Range(MinimalPositivePositionalValues.y, MaximumPositivePositionalValues.y);
            MidZNegativeAxis = Random.Range(MinimalNegativePositionalValues.z, MaximumNegativePositionalValues.z);
            MidZPositiveAxis = Random.Range(MinimalPositivePositionalValues.z, MaximumPositivePositionalValues.z);


            MidXrotNegativeAxis = Random.Range(MinimalNegativeRotationalValues.x, MaximumNegativeRotationalValues.x);
            MidXrotPositiveAxis = Random.Range(MinimalPositiveRotationalValues.x, MaximumPositiveRotationalValues.x);
            MidYrotNegativeAxis = Random.Range(MinimalNegativeRotationalValues.y, MaximumNegativeRotationalValues.y);
            MidYrotPositiveAxis = Random.Range(MinimalPositiveRotationalValues.y, MaximumPositiveRotationalValues.y);
            MidZNegativeAxis = Random.Range(MinimalNegativeRotationalValues.z, MaximumNegativeRotationalValues.z);
            MidZPositiveAxis = Random.Range(MinimalPositiveRotationalValues.z, MaximumPositiveRotationalValues.z);

            AllAnimationNames();

        }
        public void AllAnimationNames()
        {
            TempWeaponAimedAnimationName = WeaponAimedAnimationName;
            TempIdleAnimationName = IdleAnimationName;
            TempWieldAnimationName = WieldAnimationName;
            TempFireAnimationName = FireAnimationName;
            TempReloadAnimationName = ReloadAnimationName;
            TempReloadEmptyAnimationName = ReloadEmptyAnimationName;
            TempWalkingAnimationName = WalkingAnimationName;
            TempRunAnimationname = RunAnimationName;
            TempRemoveAnimationName = RemoveAnimationName;
            TempIdleAnimationParameterName = IdleAnimationParametreName;


        }
        public void ResetAnimationNames()
        {
            WeaponAimedAnimationName = TempWeaponAimedAnimationName;
            IdleAnimationName = TempIdleAnimationName;
            WieldAnimationName = TempWieldAnimationName;
            FireAnimationName = TempFireAnimationName;
            ReloadAnimationName = TempReloadAnimationName;
            ReloadEmptyAnimationName = TempReloadEmptyAnimationName;
            WalkingAnimationName = TempWalkingAnimationName;
            RunAnimationName = TempRunAnimationname;
            RemoveAnimationName = TempRemoveAnimationName;
            IdleAnimationParametreName = TempIdleAnimationParameterName;
        }
        public void StartChecks()
        {
            if (DoOnce == false)
            {
                Reload.CurrentAmmos = Reload.FirstMagazineSize;
                StoreWeaponResetingSpeed = Reload.WeaponResetPositioningSpeed;
                SaveShiftSpeed = ShiftSpeed;
                RuntimeAnimatorController ac = RequiredComponents.WeaponAnimatorComponent.runtimeAnimatorController;    //Get Animator controller
                                                                                                                //  ShotLength = WeaponAnimatorComponent.runtimeAnimatorController.animationClips[0].length;
                for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
                {
                    if (ac.animationClips[i].name == FireAnimationName)        //If it has the same name as your clip
                    {
                        IsFireAnimNameOk = true;
                        ShotLength = ac.animationClips[i].length;
                    }

                    if (ac.animationClips[i].name == ReloadAnimationName)
                    {
                        IsReloadAnimNameOk = true;
                        ReloadLength = ac.animationClips[i].length;
                    }

                    if (ac.animationClips[i].name == ReloadEmptyAnimationName)
                    {
                        IsReloadEmptyAnimNameOk = true;
                        ReloadEmptyLength = ac.animationClips[i].length;
                    }

                    if (ac.animationClips[i].name == WieldAnimationName)
                    {
                        Wield = true;
                        WieldTime = ac.animationClips[i].length;
                    }

                    if (ac.animationClips[i].name == RemoveAnimationName)
                    {
                        Remove = true;
                        RemoveLength = ac.animationClips[i].length;
                    }
                }


                DoOnce = true;
            }

            shootnow = false;
            Reload.isreloading = false;

            if (WeaponSounds.WieldSoundAudioSource != null && WeaponSounds.WieldAudioClip != null)
            {
                if (!WeaponSounds.WieldSoundAudioSource.isPlaying)
                {
                    WeaponSounds.WieldSoundAudioSource.Play();
                }
            }
            if (aud != null)
            {
                Destroy(aud.gameObject, aud.clip.length);
            }

            if (Reload.CurrentAmmos > 0)
            {
                if (PlayerManager.instance != null)
                {
                    for (int x = 0; x < Reload.ButtonsToDisableInteractionOnReload.Length; x++)
                    {
                        if (Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>() != null)
                        {
                            Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>().raycastTarget = true;
                        }
                        Reload.ButtonsToDisableInteractionOnReload[x].interactable = true;
                    }
                }
            }
            else
            {
                if (PlayerManager.instance != null)
                {
                    isReloadCompleted = false;
                    for (int x = 0; x < Reload.ButtonsToDisableInteractionOnReload.Length; x++)
                    {
                        if (Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>() != null)
                        {
                            Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>().raycastTarget = false;
                        }
                        Reload.ButtonsToDisableInteractionOnReload[x].interactable = false;
                    }
                }
            }

            StartCoroutine(StartHipFireIdle());

        }
        // Helper Methods
        private void SetHipFireState()
        {
            IsplayingAimedAnim = false;
            IsHipFire = true;
            IsAimed = false;
            CanTweakAimedPositionAndRotations = false;
            CanTweakHipFirePositionAndRotations = false;
        }

        private void SetAimedState()
        {
            AimedAnimState = true;
            IsplayingAimedAnim = false;
            IsHipFire = false;
            IsAimed = true;
            CanTweakAimedPositionAndRotations = false;
            CanTweakHipFirePositionAndRotations = false;
        }
        public void HipFireState()
        {
            IsplayingAimedAnim = false;
            IsHipFire = true;
            IsAimed = false;
            CanTweakAimedPositionAndRotations = false;
            CanTweakHipFirePositionAndRotations = false;
        }
        public void AimedState()
        {
            AimedAnimState = true;
            IsplayingAimedAnim = false;
            IsHipFire = false;
            IsAimed = true;
            CanTweakAimedPositionAndRotations = false;
            CanTweakHipFirePositionAndRotations = false;
        }
        private void OnEnable()
        {           
            if (ShootingFeatures.MuzzleFlashMesh != null)
            {
                ShootingFeatures.MuzzleFlashMesh.SetActive(false); // Important for PC controls 
            }
            StartMuzzleMeshTimer = false; //Resetting Muzzle Mesh Timer  // Important for PC controls 
            StartShooting = false; // until wield animation not completed stop shooting 
            StoreDefaultValues();

            PlayerManagerObject = FindObjectOfType<PlayerManager>();
            if (PlayerManagerObject != null)
            {
                if (PlayerManagerObject.SwitchingPlayerWeaponsComponent != null)
                {
                    if (PlayerManagerObject.SwitchingPlayerWeaponsComponent.AutoAimIfSwitchedFromAimedWeapon == false)
                    {
                        HipFireState();
                    }
                    else
                    {
                        if (PlayerManagerObject.CurrentHoldingPlayerWeapon != null)
                        {
                            if (PlayerManagerObject.CurrentHoldingPlayerWeapon.IsAimed == true)
                            {
                                AimedState();
                            }
                            else
                            {
                                HipFireState();
                            }
                        }
                        else
                        {
                            HipFireState();
                        }

                    }
                }
                else
                {
                    HipFireState();

                }
            }
            else
            {
                // Default State
                IsplayingAimedAnim = false;
                IsHipFire = false;
                IsAimed = false;
                CanTweakAimedPositionAndRotations = false;
                CanTweakHipFirePositionAndRotations = false;
            }


            // Components.WeaponAnimatorComponent.Rebind();
            StartChecks();
        }
        IEnumerator StartHipFireIdle()
        {
           
            yield return new WaitForSeconds(WieldTime);
            StartShooting = true; // after wield animation completed start shooting
            // I put this code here so crosshair only get activated when wield animation is completed playing back.
            if (ShootingFeatures.WeaponCrosshair != null)
            {
                if (IsAimed == false)
                {
                    if (ShootingFeatures.UseCrosshair == true)
                    {
                        ShootingFeatures.WeaponCrosshair.gameObject.SetActive(true);
                        if (ShootingFeatures.WeaponCrosshair.GetComponent<CrossHair>() != null)
                        {
                            Ch = ShootingFeatures.WeaponCrosshair.GetComponent<CrossHair>();
                        }
                    }
                    else
                    {
                        ShootingFeatures.WeaponCrosshair.gameObject.SetActive(false);
                    }
                }
                else
                {
                    ShootingFeatures.WeaponCrosshair.gameObject.SetActive(false);
                }

            }

            if (Reload.isreloading == false)
            {
                if (IsAimed == true)
                {
                    IsplayingAimedAnim = false;
                    AimedBreathAnimController();
    
                }
                else
                {
                    if (JoyStick.Instance != null && PlayerManager.instance != null)
                    {
                        if (JoyStick.Instance.IsWalking == false && PlayerManager.instance.IsMoving == false)
                        {
                            RequiredComponents.WeaponAnimatorComponent.SetBool(IdleAnimationParametreName, true);
                            RequiredComponents.WeaponAnimatorComponent.SetBool(WalkAnimationParametreName, false);
                            RequiredComponents.WeaponAnimatorComponent.SetBool(RunAnimationParametreName, false);
                            RequiredComponents.WeaponAnimatorComponent.Play(IdleAnimationName, -1, 0f);
                        }
                        else
                        {
                            RequiredComponents.WeaponAnimatorComponent.Play(IdleAnimationName, -1, 0f);
                        }
                    }
                  
                }
            }
        }
        //IEnumerator StopShooting()
        //{
        //    yield return new WaitForSeconds(WeaponOptions.ShotAnimationDelay);
        //    yield return new WaitForSeconds(ShotLength);
        //    if (Reload.isreloading == false)
        //    {
        //        if (IsAimed == true)
        //        {
        //            IsplayingAimedAnim = false;
        //            AimedAnimState = true;
        //        }
        //    }
        //}
        //IEnumerator StartLerpingIdle()
        //{
        //    yield return new WaitForSeconds(Reload.TimeToStartLerpingToIdle);
        //    LerpNow = true;
        //}
        IEnumerator MuzzleMeshTimer()
        {
            yield return new WaitForSeconds(ShootingFeatures.MuzzleFlashMeshActiveTime);
            if(ShootingFeatures.MuzzleFlashMesh != null)
            {
                ShootingFeatures.MuzzleFlashMesh.SetActive(false);
            }
            StartMuzzleMeshTimer = false;
        }
        void Update()
        {
          
            if (Reload.TotalAmmoText != null)
            {
                Reload.TotalAmmoText.text = Reload.TotalAmmo.ToString();
            }

            if (Reload.ShotsInMagCountText != null)
            {
                Reload.ShotsInMagCountText.text = Reload.CurrentAmmos + " " + "/".ToString();
            }

            if (Reload.TotalAmmo <= 0)
            {
                if (Reload.TotalAmmoText != null)
                {
                    Reload.TotalAmmoText.text = "0";
                }

            }

            if (Reload.CurrentAmmos < 0)
            {
                Reload.CurrentAmmos = 0;
                if (Reload.ShotsInMagCountText != null)
                {

                    Reload.ShotsInMagCountText.text = "0" + " " + "/";
                }
            }
            if (Reload.isreloading && LerpNow == false)
            {
                //if (Reload.UseLerpTimer == true)
                //{
                //    if (LerpBack == false)
                //    {
                //        StartCoroutine(StartLerpingIdle());
                //        LerpBack = true;
                //    }
                //}
               
                IsAimed = false;
                IsHipFire = false;
             
                Reload.ForceReloadPositioning = true;

                RequiredComponents.PlayerCamera.fieldOfView = Mathf.Lerp(RequiredComponents.PlayerCamera.fieldOfView, CamerasFovParameters.DefaultPlayerCamFov, CamerasFovParameters.PlayerCameraFovChangeSpeed * Time.deltaTime);

                if (ShootingFeatures.UseBlurEffectOnAim == true)
                {
                    if (ShootingFeatures.BlurEffect != null)
                    {
                        ShootingFeatures.BlurEffect.SetActive(false);
                    }
                }
                if (RequiredComponents.WeaponCamera != null)
                {
                    RequiredComponents.WeaponCamera.fieldOfView = Mathf.Lerp(RequiredComponents.WeaponCamera.fieldOfView, CamerasFovParameters.DefaultWeaponCamFov, CamerasFovParameters.WeaponCameraFovChangeSpeed * Time.deltaTime);
                }
                WeaponPositionsRotationsDurations.WeaponPivot.transform.localPosition = Vector3.MoveTowards(WeaponPositionsRotationsDurations.WeaponPivot.transform.localPosition, Reload.WeaponPivotReloadPosition, Reload.WeaponPositionLerpSpeed * Time.deltaTime);
                WeaponPositionsRotationsDurations.WeaponPivot.transform.localEulerAngles = Vector3.MoveTowards(WeaponPositionsRotationsDurations.WeaponPivot.transform.localEulerAngles, Reload.WeaponPivotReloadRotation, Reload.WeaponRotationLerpSpeed * Time.deltaTime);

            }
            else
            {
                if (Reload.ForceReloadPositioning == true)
                {
                    //LerpBack = false;
                    if (IsAimed == true)
                    {
                        Reload.WeaponResetPositioningSpeed = Reload.WeaponResetPositioningSpeed * 100f;
                    }
            
                    Vector3 temp = WeaponPositionsRotationsDurations.WeaponPivot.transform.localEulerAngles;
                    temp.x = Mathf.LerpAngle(temp.x, WeaponPositionsRotationsDurations.HipFireWeaponPivotRotation.x, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
                    temp.y = Mathf.LerpAngle(temp.y, WeaponPositionsRotationsDurations.HipFireWeaponPivotRotation.y, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
                    temp.z = Mathf.LerpAngle(temp.z, WeaponPositionsRotationsDurations.HipFireWeaponPivotRotation.z, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
            
                    WeaponPositionsRotationsDurations.WeaponPivot.transform.localEulerAngles = temp;// Vector3.Lerp(ShootingPointParent.transform.localEulerAngles, temp, WeaponResetPositioningSpeed * Time.deltaTime);

                    //Vector3 camtemp = PlayerManagerPositions.PlayerManager.transform.localEulerAngles;
                    //camtemp.x = Mathf.LerpAngle(camtemp.x, PlayerManagerPositions.HipFireRotation.x, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
                    //camtemp.y = Mathf.LerpAngle(camtemp.y, PlayerManagerPositions.HipFireRotation.y, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
                    //camtemp.z = Mathf.LerpAngle(camtemp.z, PlayerManagerPositions.HipFireRotation.z, Reload.WeaponResetPositioningSpeed * Time.deltaTime);

                    //PlayerManagerPositions.PlayerManager.transform.localEulerAngles = camtemp;

                    //Vector3 tempo = WeaponPosition.ShootingPointParent.transform.localPosition;
                    //tempo.x = WeaponPosition.HipFireRotation.x;
                    //tempo.y = WeaponPosition.HipFireRotation.y;
                    //tempo.z = WeaponPosition.HipFireRotation.z;
                    //WeaponPosition.ShootingPointParent.transform.localPosition = Vector3.Lerp(WeaponPosition.ShootingPointParent.transform.localPosition, tempo, Reload.WeaponResetPositioningSpeed * Time.deltaTime);

                    //Vector3 cameratempo = CameraPositions.PlayerCamera.transform.localPosition;
                    //cameratempo.x = CameraPositions.HipFirePosition.x;
                    //cameratempo.y = CameraPositions.HipFirePosition.y;
                    //cameratempo.z = CameraPositions.HipFirePosition.z;
                    //CameraPositions.PlayerCamera.transform.localPosition = Vector3.Lerp(CameraPositions.PlayerCamera.transform.localPosition, cameratempo, Reload.WeaponResetPositioningSpeed * Time.deltaTime);

                    if (WeaponPositionsRotationsDurations.WeaponPivot.transform.localEulerAngles == WeaponPositionsRotationsDurations.HipFireWeaponPivotRotation && WeaponPositionsRotationsDurations.WeaponPivot.transform.localPosition == WeaponPositionsRotationsDurations.HipFireWeaponPivotPosition)
                        //&& PlayerManagerPositions.PlayerManager.transform.localEulerAngles == PlayerManagerPositions.HipFireRotation && PlayerManagerPositions.PlayerManager.transform.localPosition == PlayerManagerPositions.HipFirePosition)
                    {
                        Reload.WeaponResetPositioningSpeed = StoreWeaponResetingSpeed;
                        LerpNow = false;
                        Reload.ForceReloadPositioning = false;
                       // LerpBack = false;
                    }
                }
            }
            if (Reload.isreloading && LerpNow == true)
            {
                if (Reload.ForceReloadPositioning == true)
                {
                    if (IsAimed == true)
                    {
                        Reload.WeaponResetPositioningSpeed = Reload.WeaponResetPositioningSpeed * 100f;
                    }
                   
                    Vector3 temp = WeaponPositionsRotationsDurations.WeaponPivot.transform.localEulerAngles;
                    temp.x = Mathf.LerpAngle(temp.x, WeaponPositionsRotationsDurations.HipFireWeaponPivotRotation.x, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
                    temp.y = Mathf.LerpAngle(temp.y, WeaponPositionsRotationsDurations.HipFireWeaponPivotRotation.y, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
                    temp.z = Mathf.LerpAngle(temp.z, WeaponPositionsRotationsDurations.HipFireWeaponPivotRotation.z, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
    
                    WeaponPositionsRotationsDurations.WeaponPivot.transform.localEulerAngles = temp;// Vector3.Lerp(ShootingPointParent.transform.localEulerAngles, temp, WeaponResetPositioningSpeed * Time.deltaTime) ;


                    //Vector3 camtemp = PlayerManagerPositions.PlayerManager.transform.localEulerAngles;
                    //camtemp.x = Mathf.LerpAngle(camtemp.x, PlayerManagerPositions.HipFireRotation.x, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
                    //camtemp.y = Mathf.LerpAngle(camtemp.y, PlayerManagerPositions.HipFireRotation.y, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
                    //camtemp.z = Mathf.LerpAngle(camtemp.z, PlayerManagerPositions.HipFireRotation.z, Reload.WeaponResetPositioningSpeed * Time.deltaTime);

                    //PlayerManagerPositions.PlayerManager.transform.localEulerAngles = camtemp;


                    Vector3 tempo = WeaponPositionsRotationsDurations.WeaponPivot.transform.localPosition;
                    tempo.x = WeaponPositionsRotationsDurations.HipFireWeaponPivotPosition.x;
                    tempo.y = WeaponPositionsRotationsDurations.HipFireWeaponPivotPosition.y;
                    tempo.z = WeaponPositionsRotationsDurations.HipFireWeaponPivotPosition.z;
                    WeaponPositionsRotationsDurations.WeaponPivot.transform.localPosition = Vector3.Lerp(WeaponPositionsRotationsDurations.WeaponPivot.transform.localPosition, tempo, Reload.WeaponResetPositioningSpeed * Time.deltaTime);

                    //Vector3 camerapositiontempo = PlayerManagerPositions.PlayerManager.transform.localPosition;
                    //camerapositiontempo.x = PlayerManagerPositions.HipFirePosition.x;
                    //camerapositiontempo.y = PlayerManagerPositions.HipFirePosition.y;
                    //camerapositiontempo.z = PlayerManagerPositions.HipFirePosition.z;
                    //PlayerManagerPositions.PlayerManager.transform.localPosition = Vector3.Lerp(PlayerManagerPositions.PlayerManager.transform.localPosition, camerapositiontempo, Reload.WeaponResetPositioningSpeed * Time.deltaTime);

                    if (WeaponPositionsRotationsDurations.WeaponPivot.transform.localEulerAngles == WeaponPositionsRotationsDurations.HipFireWeaponPivotRotation && WeaponPositionsRotationsDurations.WeaponPivot.transform.localPosition == WeaponPositionsRotationsDurations.HipFireWeaponPivotPosition)
                        //&& PlayerManagerPositions.PlayerManager.transform.localEulerAngles == PlayerManagerPositions.HipFireRotation && PlayerManagerPositions.PlayerManager.transform.localPosition == PlayerManagerPositions.HipFirePosition)
                    {
                        Reload.WeaponResetPositioningSpeed = StoreWeaponResetingSpeed;
                        LerpNow = false;
                        Reload.ForceReloadPositioning = false;
                      //  LerpBack = false;
                    }
                }
            }


            if (Reload.CurrentAmmos <= 0 && Reload.TotalAmmo > 0)
            {
                if (isReloadCompleted == false && PlayerManagerObject.IsShooting == false)
                {
                    StartCoroutine(ReloadCoroutine(ReloadEmptyLength));
                    isReloadCompleted = true;
                }
            }

            if (Reload.Reloadation == true)
            {
                StartCoroutine(ReloadCoroutine(ReloadLength));
                Reload.Reloadation = false;
            }


            if (Reload.isreloading)
            {
                shootnow = false;
            }
            if (Ch != null)
            {
                if (ShootingFeatures.ForceFullAutoFire == true)
                {
                    Ch.UpdateCrossHair(shootnow);
                }
                else
                {
                    Ch.UpdateCrossHair(ShotMade);
                }
            }

            if (RequiredComponents.PlayerCamera != null)
            {
                Ray ray = RequiredComponents.PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of the screen
                                                                                                           // RaycastHit hit;
                Vector3 targetPoint;

                if (Physics.Raycast(ray, out hit, ShootingFeatures.RaycastShootingRange))
                {
                    targetPoint = hit.point; // Where the crosshair is pointing
                }
                else
                {
                    targetPoint = ray.GetPoint(ShootingFeatures.RaycastShootingRange); // Default to max range
                }

                // Adjust the weapon's shooting direction
                shootingDirection = (targetPoint - transform.position).normalized;

              
            }


            //if(ShootingFeatures.EnableLaserSight == true)
            //{
            //    ShootingFeatures.LaserSightGameObject.transform.position = shootingDirection;
            //}




            // Optional: Debug line for visualization
            // Debug.DrawRay(transform.position, shootingDirection * ShootingFeatures.RaycastShootingRange, Color.red);


            // if (ShootingFeatures.VisualiseShootingDirection == true)
            // {
            //     Vector3 start = transform.position;
            //     Vector3 end = start + shootingDirection * ShootingFeatures.LineRendererLength;
            //     lineRenderer.SetPosition(0, start);
            //     lineRenderer.SetPosition(1, end);

            //     // Update line color dynamically if changed
            //     lineRenderer.startColor = ShootingFeatures.LineRendererColor;
            //     lineRenderer.endColor = ShootingFeatures.LineRendererColor;
            // }

            // Create a ray from the center of the screen
            //Ray ray = RequiredComponents.PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            //start = transform.position; // Weapon muzzle or shooting point

            //if (Physics.Raycast(ray, out hit, ShootingFeatures.RaycastShootingRange))
            //{
            //    end = hit.point; // Set the end point to the raycast hit location
            //}
            //else
            //{
            //    end = ray.GetPoint(ShootingFeatures.RaycastShootingRange); // Default to the max range
            //}

            //// Debug.DrawRay (keep the old logic for visualizing the direction)
            //Vector3 shootingDirection = (end - start).normalized; // Compute the direction vector
            //Debug.DrawRay(start, shootingDirection * ShootingFeatures.RaycastShootingRange, Color.red); // Draw ray from start in the direction


            if (CanRaycastPass == true && IsShootingNotAllowed == false )
            {
                Shoot();
                CanRaycastPass = false;
            }


            if (shootnow == true && Time.time >= Nexttimetofire && Reload.CurrentAmmos > 0 && IsShootingNotAllowed == false )
            {
                Nexttimetofire = Time.time + 1f / ShootingFeatures.FullAutoFireRate;
                Shoot();
                if (ShootingFeatures.MuzzleFlashMesh != null)
                {
                    if (ShootingFeatures.UseMuzzleFlashMesh == true)
                    {
                        if (StartMuzzleMeshTimer == false)
                        {
                          
                                ShootingFeatures.MuzzleFlashMesh.SetActive(true);
                                StartCoroutine(MuzzleMeshTimer());
                                StartMuzzleMeshTimer = true;
                           
                        }
                    }
                }
            }
            else if (ShootingFeatures.ForceFullAutoFire == false && ShotMade == true)
            {
                Timer += Time.deltaTime;
                if (Timer > 0.05f)
                {
                    if (ShootingFeatures.MuzzleFlashMesh != null)
                    {
                        if (ShootingFeatures.UseMuzzleFlashMesh == true)
                        {
                            ShootingFeatures.MuzzleFlashMesh.SetActive(false);
                        }
                    }
                    ShotMade = false;
                    Timer = 0;
                }
            }

            //if (ShotMaded == true)
            //{
            //    StartCoroutine(StopShooting());
            //    ShotMaded = false;
            //}

            if (IsAimed == true)
            {

                RequiredComponents.WeaponAnimatorComponent.SetFloat(WeaponAnimationClipsSpeeds.IdleAnimationSpeed, WeaponAnimationClipsSpeeds.IdleAnimationSpeedWhenHipToAimed);

                AnimatorStateInfo stateInfo = RequiredComponents.WeaponAnimatorComponent.GetCurrentAnimatorStateInfo(0);
//                Debug.Log(stateInfo.normalizedTime);
                // Check if the animation has completed or is near completion
                if (stateInfo.IsName(IdleAnimationName) && (stateInfo.normalizedTime % 1) >= 0.95f
                    || stateInfo.IsName(WieldAnimationName) && (stateInfo.normalizedTime % 1) >= 0.95f)// && stateInfo.normalizedTime >= 1f)
                {
                    if (stateInfo.IsName(IdleAnimationName))
                    {
                        RequiredComponents.WeaponAnimatorComponent.SetFloat(WeaponAnimationClipsSpeeds.IdleAnimationSpeed, 0f);
                    }
                    if (IsplayingAimedAnim == true)
                    {
                        RequiredComponents.WeaponAnimatorComponent.SetBool(WeaponAnimationClipsSpeeds.AimedAnimationParametreName, true);
                    }
                }

                Reload.WeaponResetPositioningSpeed = StoreWeaponResetingSpeed;
                LerpNow = false;
                Reload.ForceReloadPositioning = false;
             //   LerpBack = false;

                if (ShootingFeatures.SniperScopeScript != null && ShootingFeatures.UseSniperScopeUI == true)
                {
                    if (ShootingFeatures.SniperScopeScript.IsScoped == false)
                    {
                        ShootingFeatures.SniperScopeScript.Sniperscope();
                    }

                    if (ShootingFeatures.SniperScopeScript.ZoomConfig.EnableZoomControls == true)
                    {
                        //if (MouseScrollingControl.instance == null)
                        //{
                        if(FirstPersonController.instance != null)
                        {
                            if(FirstPersonController.instance.UseCustomLookScripts == false) // For Mobile This code is important otherwise when using sniper scope the default Scope fov value will not work.
                            {
                                if (ShootingFeatures.SniperScopeScript.ZoomConfig.UseZoomSlider == true)
                                {
                                    RequiredComponents.PlayerCamera.fieldOfView = Mathf.Lerp(RequiredComponents.PlayerCamera.fieldOfView, ShootingFeatures.SniperScopeScript.ZoomConfig.ZoomSlider.value, CamerasFovParameters.PlayerCameraFovChangeSpeed * Time.deltaTime);
                                }
                                else
                                {
                                    RequiredComponents.PlayerCamera.fieldOfView = Mathf.Lerp(RequiredComponents.PlayerCamera.fieldOfView, ShootingFeatures.SniperScopeScript.ScopeConfig.ScopeFOV, CamerasFovParameters.PlayerCameraFovChangeSpeed * Time.deltaTime);
                                }
                            }
                        }
                           
                        //}
                    }

                    if (UseProceduralAimedBreath == true)
                    {
                        //if (!IsWalking)
                        //{
                        //    ProceduralBreath();
                        //}


                        if (CombineProceduralBreathWithAnimation == true)
                        {
                            AimedBreathAnimController();
                        }
                    }
                    else
                    {
                        AimedBreathAnimController();
                    }
                   
                }
                else
                {
                    RequiredComponents.PlayerCamera.fieldOfView = Mathf.Lerp(RequiredComponents.PlayerCamera.fieldOfView, CamerasFovParameters.PlayerCameraMagnifiedFov, CamerasFovParameters.PlayerCameraFovChangeSpeed * Time.deltaTime);
                    if (UseProceduralAimedBreath == true)
                    {
                        //if (!IsWalking)
                        //{
                        //    ProceduralBreath();
                        //}

                        if (CombineProceduralBreathWithAnimation == true)
                        {
                            AimedBreathAnimController();
                        }
                    }
                    else
                    {
                        AimedBreathAnimController();
                    }
                }


                if (ShootingFeatures.UseBlurEffectOnAim == true)
                {
                    if (ShootingFeatures.BlurEffect != null)
                    {
                        ShootingFeatures.BlurEffect.SetActive(true);
                    }
                }


                if (CanTweakAimedPositionAndRotations == false)
                {
                    LeanTween.cancel(WeaponPositionsRotationsDurations.WeaponPivot.gameObject);
                    LeanTween.moveLocalX(WeaponPositionsRotationsDurations.WeaponPivot, WeaponPositionsRotationsDurations.AimedWeaponPivotPosition.x, WeaponPositionsRotationsDurations.WeaponDurations.AimedXShiftDuration);
                    LeanTween.moveLocalY(WeaponPositionsRotationsDurations.WeaponPivot, WeaponPositionsRotationsDurations.AimedWeaponPivotPosition.y, WeaponPositionsRotationsDurations.WeaponDurations.AimedYShiftDuration);
                    LeanTween.moveLocalZ(WeaponPositionsRotationsDurations.WeaponPivot, WeaponPositionsRotationsDurations.AimedWeaponPivotPosition.z, WeaponPositionsRotationsDurations.WeaponDurations.AimedZShiftDuration);

                    //LeanTween.moveLocalX(PlayerManagerPositions.PlayerManager, PlayerManagerPositions.AimedPosition.x, WeaponPositionsRotationsDurations.WeaponDurations.AimedXShiftDuration);
                    //LeanTween.moveLocalY(PlayerManagerPositions.PlayerManager, PlayerManagerPositions.AimedPosition.y, WeaponPositionsRotationsDurations.WeaponDurations.AimedYShiftDuration);
                    //LeanTween.moveLocalZ(PlayerManagerPositions.PlayerManager, PlayerManagerPositions.AimedPosition.z, WeaponPositionsRotationsDurations.WeaponDurations.AimedZShiftDuration);

                    LeanTween.rotateLocal(WeaponPositionsRotationsDurations.WeaponPivot, WeaponPositionsRotationsDurations.AimedWeaponPivotRotation, WeaponPositionsRotationsDurations.WeaponDurations.AimedRotDuration);

                    //LeanTween.rotateLocal(PlayerManagerPositions.PlayerManager, PlayerManagerPositions.AimedRotation, WeaponPositionsRotationsDurations.WeaponDurations.AimedRotDuration);

                    CanTweakAimedPositionAndRotations = true;
                    CanTweakHipFirePositionAndRotations = false;

                }
   
                if (RequiredComponents.WeaponCamera != null)
                {

                    RequiredComponents.WeaponCamera.fieldOfView = Mathf.Lerp(RequiredComponents.WeaponCamera.fieldOfView, CamerasFovParameters.WeaponCameraMagnifiedFov, CamerasFovParameters.WeaponCameraFovChangeSpeed * Time.deltaTime);
          
                }


            }

            if (IsHipFire == true)
            {            
                RequiredComponents.WeaponAnimatorComponent.SetFloat(WeaponAnimationClipsSpeeds.IdleAnimationSpeed, WeaponAnimationClipsSpeeds.IdleAnimationSpeedWhenAimedToHip);
                AimedAnimState = true;
                if (RequiredComponents.PlayerCamera != null)
                {

                    RequiredComponents.PlayerCamera.fieldOfView = Mathf.Lerp(RequiredComponents.PlayerCamera.fieldOfView, CamerasFovParameters.DefaultPlayerCamFov, CamerasFovParameters.PlayerCameraFovChangeSpeed * Time.deltaTime);
                }

                if (ShootingFeatures.UseBlurEffectOnAim == true)
                {
                    if (ShootingFeatures.BlurEffect != null)
                    {
                        ShootingFeatures.BlurEffect.SetActive(false);
                    }
                }
                else
                {
                    if (ShootingFeatures.BlurEffect != null)
                    {
                        ShootingFeatures.BlurEffect.SetActive(false);
                    }
                }
                if (WeaponPositionsRotationsDurations.WeaponPivot != null)
                {            
                    if (CanTweakHipFirePositionAndRotations == false)
                    {
                        LeanTween.cancel(WeaponPositionsRotationsDurations.WeaponPivot.gameObject);
                        LeanTween.moveLocalX(WeaponPositionsRotationsDurations.WeaponPivot, WeaponPositionsRotationsDurations.HipFireWeaponPivotPosition.x, WeaponPositionsRotationsDurations.WeaponDurations.HipFireXShiftDuration);
                        LeanTween.moveLocalY(WeaponPositionsRotationsDurations.WeaponPivot, WeaponPositionsRotationsDurations.HipFireWeaponPivotPosition.y, WeaponPositionsRotationsDurations.WeaponDurations.HipFireYShiftDuration);
                        LeanTween.moveLocalZ(WeaponPositionsRotationsDurations.WeaponPivot, WeaponPositionsRotationsDurations.HipFireWeaponPivotPosition.z, WeaponPositionsRotationsDurations.WeaponDurations.HipFireZShiftDuration);

                        LeanTween.rotateLocal(WeaponPositionsRotationsDurations.WeaponPivot, WeaponPositionsRotationsDurations.HipFireWeaponPivotRotation, WeaponPositionsRotationsDurations.WeaponDurations.HipFireRotDuration);

                        //LeanTween.cancel(PlayerManagerPositions.PlayerManager.gameObject);
                        //LeanTween.moveLocalX(PlayerManagerPositions.PlayerManager, PlayerManagerPositions.HipFirePosition.x, WeaponPositionsRotationsDurations.WeaponDurations.HipFireXShiftDuration);
                        //LeanTween.moveLocalY(PlayerManagerPositions.PlayerManager, PlayerManagerPositions.HipFirePosition.y, WeaponPositionsRotationsDurations.WeaponDurations.HipFireYShiftDuration);
                        //LeanTween.moveLocalZ(PlayerManagerPositions.PlayerManager, PlayerManagerPositions.HipFirePosition.z, WeaponPositionsRotationsDurations.WeaponDurations.HipFireZShiftDuration);

                        //LeanTween.rotateLocal(PlayerManagerPositions.PlayerManager, PlayerManagerPositions.HipFireRotation, WeaponPositionsRotationsDurations.WeaponDurations.HipFireRotDuration);

                        CanTweakHipFirePositionAndRotations = true;
                        CanTweakAimedPositionAndRotations = false;

                    }
                }


                if (RequiredComponents.WeaponCamera != null)
                {
                    RequiredComponents.WeaponCamera.fieldOfView = Mathf.Lerp(RequiredComponents.WeaponCamera.fieldOfView, CamerasFovParameters.DefaultWeaponCamFov, CamerasFovParameters.WeaponCameraFovChangeSpeed * Time.deltaTime);  
                }


                //if (ShootingFeatures.SniperScopeScript != null && ShootingFeatures.UseSniperScopeUI == true)
                //{
                     
                //        ObjectToAnimate.transform.localPosition = Vector3.MoveTowards(ObjectToAnimate.transform.localPosition, new Vector3(PreviousX, PreviousY, PreviousZ), ShiftSpeed * Time.deltaTime);
                //        ObjectToAnimate.transform.localEulerAngles = Vector3.MoveTowards(ObjectToAnimate.transform.localEulerAngles, new Vector3(PreviousXRot, PreviousYRot, PreviousZRot), RotationSpeed * Time.deltaTime);
                    
                //}
                //else
                //{
                //    if (UseProceduralAimedBreath == true)
                //    {
                //        ObjectToAnimate.transform.localPosition = Vector3.MoveTowards(ObjectToAnimate.transform.localPosition, new Vector3(PreviousX, PreviousY, PreviousZ), Time.deltaTime * ShiftSpeed);
                //        ObjectToAnimate.transform.localEulerAngles = Vector3.MoveTowards(ObjectToAnimate.transform.localPosition, new Vector3(PreviousX, PreviousY, PreviousZ), Time.deltaTime * RotationSpeed);
                //    }
                //}

                ResetHipFireState();
            }
        }
        public void BulletsFunctionality(Vector3 pos, Quaternion rot)
        {
            GameObject bullet = pooler.SpawnFromPool(ShootingMechanics.ProjectileName, pos, rot);
            BulletScript B = bullet.GetComponent<BulletScript>();
            B.Movement(transform, transform.root, IsProjectileShooting);
          

        }
        public void RotateZOnShot()
        {
            if(ShootingFeatures.MuzzleFlashRotatingObject != null)
            {
                Vector3 ZRot = ShootingFeatures.MuzzleFlashRotatingObject.transform.localEulerAngles;
                ZRot.z = Random.Range(ShootingFeatures.MuzzleFlashRotatorMaxNegativeAngle, ShootingFeatures.MuzzleFlashRotatorMaxPositiveAngle);
                ShootingFeatures.MuzzleFlashRotatingObject.transform.localEulerAngles = ZRot;
            }
        }
        public void Shoot()
        {
            RequiredComponents.WeaponAnimatorComponent.Play("Shoot", -1, 0f);

            if (RequiredComponents.AlertingSoundActivatorComponent != null)
            {
                RequiredComponents.AlertingSoundActivatorComponent.ActivateNoiseHandler(transform);
            }


            if (ShootingFeatures.UseMuzzleFlashMesh == true)
            {
                if (ShootingFeatures.MuzzleFlashMesh != null)
                {
                    ShootingFeatures.MuzzleFlashMesh.SetActive(true);
                }
            }
            else
            {
                if (ShootingFeatures.MuzzleFlashParticleFX != null)
                {
                    if (ShootingFeatures.MuzzleFlashParticleFX.isPlaying == false)
                        ShootingFeatures.MuzzleFlashParticleFX.Play();
                }
            }
            //  AudioSource aud = Instantiate(WeaponSounds.FireSoundAudioSource, transform.position, transform.rotation);
            WeaponSounds.FireSoundAudioSource.PlayOneShot(WeaponSounds.FireAudioClip);
          //  Destroy(aud.gameObject, aud.clip.length);
            playershoot = true;

            // previous enable silence is unchecked now removed that field
                IsFire = true;
           

            Reload.CurrentAmmos--;
            //ShotMaded = true;
            RotateZOnShot();

            if (IsWalking == true)
            {
                RequiredComponents.WeaponAnimatorComponent.SetBool(IdleAnimationParametreName, false);
                RequiredComponents.WeaponAnimatorComponent.SetBool(WalkAnimationParametreName, true);
                RequiredComponents.WeaponAnimatorComponent.SetBool(RunAnimationParametreName, false);

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


            }
            

            if (ShootingFeatures.SniperScopeScript != null && ShootingFeatures.UseSniperScopeUI == true)
            {
                if(ShootingFeatures.SniperScopeScript.ShootingConfig.ShotAnimatorComponent != null)
                {
                    ShootingFeatures.SniperScopeScript.ShootingConfig.ShotAnimatorComponent.Play(ShootingFeatures.SniperScopeScript.ShootingConfig.ShotAnimationClip.name, -1, 0f);
                }

            }
            if (pooler != null)
            {
                GameObject bulletShellrb = pooler.SpawnFromPool(BulletShell.BulletShellName, BulletShell.BulletShellSpawnPoint.transform.position, BulletShell.BulletShellSpawnPoint.transform.rotation);
                Rigidbody r = bulletShellrb.GetComponent<Rigidbody>();
                r.velocity = BulletShell.BulletShellSpawnPoint.transform.TransformDirection(Vector3.right * BulletShell.ShellsEjectingSpeed);

                if (ShootingOptions.ProjectileShooting == ShootingMechanics.ShootingOption)
                {

                    Vector3 shootingDirectionSaveForBulletSpread = shootingDirection;
                    if (ShootingMechanics.ProjectilesPerShot > 0)
                    {
                        for (int x = 0; x < ShootingMechanics.ProjectilesPerShot; x++)
                        {
                           
                            if (BulletSpread.UseBulletSpread == true)
                            {
                                //if (Crouch.instance != null)
                                //{
                                //    if (Crouch.instance.IsCrouching == false)
                                //    {
                                //        Vector3 Spread = shootingDirectionSaveForBulletSpread;
                                //        Spread.y = Random.Range(shootingDirectionSaveForBulletSpread.y + BulletSpread.MinBulletSpreadRotationXStanding, shootingDirectionSaveForBulletSpread.y + BulletSpread.MaxBulletSpreadRotationXStanding);
                                //        Spread.z = Random.Range(shootingDirectionSaveForBulletSpread.z + BulletSpread.MinBulletSpreadRotationYStanding, shootingDirectionSaveForBulletSpread.z + BulletSpread.MaxBulletSpreadRotationYStanding);
                                //        shootingDirectionSaveForBulletSpread = Spread;
                                //    }
                                //    else
                                //    {
                                //        Vector3 Spread = shootingDirectionSaveForBulletSpread;
                                //        Spread.y = Random.Range(shootingDirectionSaveForBulletSpread.y + BulletSpread.MinBulletSpreadRotationXCrouched, shootingDirectionSaveForBulletSpread.y + BulletSpread.MaxBulletSpreadRotationXCrouched);
                                //        Spread.z = Random.Range(shootingDirectionSaveForBulletSpread.z + BulletSpread.MinBulletSpreadRotationYCrouched, shootingDirectionSaveForBulletSpread.z + BulletSpread.MaxBulletSpreadRotationYCrouched);
                                //        shootingDirectionSaveForBulletSpread = Spread;
                                //    }
                                //}
                                //else
                                //{
                                //    Vector3 Spread = shootingDirectionSaveForBulletSpread;
                                //    Spread.y = Random.Range(shootingDirectionSaveForBulletSpread.y + BulletSpread.MinBulletSpreadRotationXStanding, shootingDirectionSaveForBulletSpread.y + BulletSpread.MaxBulletSpreadRotationXStanding);
                                //    Spread.z = Random.Range(shootingDirectionSaveForBulletSpread.z + BulletSpread.MinBulletSpreadRotationYStanding, shootingDirectionSaveForBulletSpread.z + BulletSpread.MaxBulletSpreadRotationYStanding);
                                //    shootingDirectionSaveForBulletSpread = Spread;
                                //}

                                if (Crouch.instance != null)
                                {
                                    if (Crouch.instance.IsCrouching == false)
                                    {
                                        Vector3 Spread = shootingDirectionSaveForBulletSpread;
                                        Spread.x = Random.Range(shootingDirectionSaveForBulletSpread.x + BulletSpread.MinBulletSpreadRotationXStanding, shootingDirectionSaveForBulletSpread.x + BulletSpread.MaxBulletSpreadRotationXStanding);
                                        Spread.y = Random.Range(shootingDirectionSaveForBulletSpread.y + BulletSpread.MinBulletSpreadRotationYStanding, shootingDirectionSaveForBulletSpread.y + BulletSpread.MaxBulletSpreadRotationYStanding);
                                        shootingDirectionSaveForBulletSpread = Spread;
                                    }
                                    else
                                    {
                                        Vector3 Spread = shootingDirectionSaveForBulletSpread;
                                        Spread.x = Random.Range(shootingDirectionSaveForBulletSpread.x + BulletSpread.MinBulletSpreadRotationXCrouched, shootingDirectionSaveForBulletSpread.x + BulletSpread.MaxBulletSpreadRotationXCrouched);
                                        Spread.y = Random.Range(shootingDirectionSaveForBulletSpread.y + BulletSpread.MinBulletSpreadRotationYCrouched, shootingDirectionSaveForBulletSpread.y + BulletSpread.MaxBulletSpreadRotationYCrouched);
                                        shootingDirectionSaveForBulletSpread = Spread;
                                    }
                                }
                                else
                                {
                                    Vector3 Spread = shootingDirectionSaveForBulletSpread;
                                    Spread.x = Random.Range(shootingDirectionSaveForBulletSpread.x + BulletSpread.MinBulletSpreadRotationXStanding, shootingDirectionSaveForBulletSpread.x + BulletSpread.MaxBulletSpreadRotationXStanding);
                                    Spread.y = Random.Range(shootingDirectionSaveForBulletSpread.y + BulletSpread.MinBulletSpreadRotationYStanding, shootingDirectionSaveForBulletSpread.y + BulletSpread.MaxBulletSpreadRotationYStanding);
                                    shootingDirectionSaveForBulletSpread = Spread;
                                }
                            }





                            //if (IsAimed == true)
                            //{
                            //    BulletsFunctionality(ShootingFeatures.AimedFireRaycastShotsSpawnPosition.transform.position, ShootingFeatures.AimedFireRaycastShotsSpawnPosition.transform.rotation);
                            //}
                            //else
                            //{
                            //    BulletsFunctionality(ShootingFeatures.HipFireRaycastShotsSpawnPosition.transform.position, ShootingFeatures.HipFireRaycastShotsSpawnPosition.transform.rotation);
                            //}




                            BulletsFunctionality(transform.position, Quaternion.LookRotation(shootingDirectionSaveForBulletSpread));
                          
                        }
                    }
                   
                }
            }
            if (ShootingOptions.RaycastShooting == ShootingMechanics.ShootingOption)
            {
                if (BulletSpread.UseBulletSpread == true)
                {
                    if (Crouch.instance != null)
                    {
                        if (Crouch.instance.IsCrouching == false)
                        {
                            Vector3 Spread = hit.point;
                            Spread.x = Random.Range(hit.point.x + BulletSpread.MinBulletSpreadRotationXStanding, hit.point.x + BulletSpread.MaxBulletSpreadRotationXStanding);
                            Spread.y = Random.Range(hit.point.y + BulletSpread.MinBulletSpreadRotationYStanding, hit.point.y + BulletSpread.MaxBulletSpreadRotationYStanding);
                            hit.point = Spread;
                        }
                        else
                        {
                            Vector3 Spread = hit.point;
                            Spread.x = Random.Range(hit.point.x + BulletSpread.MinBulletSpreadRotationXCrouched, hit.point.x + BulletSpread.MaxBulletSpreadRotationXCrouched);
                            Spread.y = Random.Range(hit.point.y + BulletSpread.MinBulletSpreadRotationYCrouched, hit.point.y + BulletSpread.MaxBulletSpreadRotationYCrouched);
                            hit.point = Spread;
                        }
                    }
                    else
                    {
                        Vector3 Spread = hit.point;
                        Spread.x = Random.Range(hit.point.x + BulletSpread.MinBulletSpreadRotationXStanding, hit.point.x + BulletSpread.MaxBulletSpreadRotationXStanding);
                        Spread.y = Random.Range(hit.point.y + BulletSpread.MinBulletSpreadRotationYStanding, hit.point.y + BulletSpread.MaxBulletSpreadRotationYStanding);
                        hit.point = Spread;
                    }
                }
                 
                // Fire the weapon (instantiate a bullet or apply damage)
                Fire();

                //if (ShootingFeatures.HipFireRaycastShotsSpawnPosition == null)
                //{
                //    if (IsAimed == false)
                //    {
                //        if (Physics.Raycast(RequiredComponents.PlayerCamera.transform.position, RequiredComponents.PlayerCamera.transform.forward, out hit, ShootingFeatures.RaycastShootingRange, ShootingFeatures.RaycastShootingVisibleLayers))
                //        {
                //            Fire();
                //        }
                //    }
                //}
                //else
                //{
                //    if (IsAimed == false)
                //    {
                //        if (ShootingFeatures.HipFireRaycastShotsSpawnPosition != null)
                //        {
                //            if (Physics.Raycast(ShootingFeatures.HipFireRaycastShotsSpawnPosition.transform.position, ShootingFeatures.HipFireRaycastShotsSpawnPosition.transform.forward, out hit, ShootingFeatures.RaycastShootingRange, ShootingFeatures.RaycastShootingVisibleLayers))
                //            {
                //                Fire();
                //            }
                //        }
                //    }
                //}

                //if (ShootingFeatures.AimedFireRaycastShotsSpawnPosition == null)
                //{
                //    if (IsAimed == true)
                //    {
                //        if (Physics.Raycast(RequiredComponents.PlayerCamera.transform.position, RequiredComponents.PlayerCamera.transform.forward, out hit, ShootingFeatures.RaycastShootingRange, ShootingFeatures.RaycastShootingVisibleLayers))
                //        {
                //            Fire();
                //        }
                //    }
                //}
                //else
                //{
                //    if (IsAimed == true)
                //    {
                //        if (ShootingFeatures.AimedFireRaycastShotsSpawnPosition != null)
                //        {
                //            if (Physics.Raycast(ShootingFeatures.AimedFireRaycastShotsSpawnPosition.transform.position, ShootingFeatures.AimedFireRaycastShotsSpawnPosition.transform.forward, out hit, ShootingFeatures.RaycastShootingRange, ShootingFeatures.RaycastShootingVisibleLayers))
                //            {
                //                Fire();
                //            }
                //        }
                //    }
                //}


            }
            if (CameraRecoil.instance != null)
            {
                CameraRecoil.instance.Recoil(IsAimed);
            }

            if (RequiredComponents.RecoilComponent != null)
            {
                RequiredComponents.RecoilComponent.Recoil(IsAimed);
            }
        }
        //public void ProceduralBreath()
        //{
        //    Vector3 Temp = ObjectToAnimate.transform.localPosition;
        //    Vector3 Rot = ObjectToAnimate.transform.localEulerAngles;
 
        //    if (LoopX == false)
        //    {
        //        Temp.x = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.x, MaxPosXNegativeAxis, ShiftSpeed * Time.deltaTime);


        //        if (Temp.x == MaxPosXNegativeAxis)
        //        {
        //            LoopX = true;
        //            MaxPosXPositiveAxis = Random.Range(MinimalPositivePositionalValues.x, MaximumPositivePositionalValues.x);

        //            Temp.x = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.x, MaxPosXPositiveAxis, ShiftSpeed * Time.deltaTime);
        //        }


        //    }
        //    else
        //    {
        //        if (LoopX == true)
        //        {
        //            Temp.x = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.x, MaxPosXPositiveAxis, ShiftSpeed * Time.deltaTime);

        //            if (Temp.x == MaxPosXPositiveAxis)
        //            {
        //                MaxPosXNegativeAxis = Random.Range(MinimalNegativePositionalValues.x, MaximumNegativePositionalValues.x);

        //                LoopX = false;
        //                Temp.x = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.x, MaxPosXNegativeAxis, ShiftSpeed * Time.deltaTime);
        //            }
        //        }
        //    }

        //    if (LoopY == false)
        //    {
        //        Temp.y = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.y, MaxPosYNegativeAxis, ShiftSpeed * Time.deltaTime);


        //        if (Temp.y == MaxPosYNegativeAxis)
        //        {
        //            LoopY = true;
        //            MaxPosYPositveAxis = Random.Range(MinimalPositivePositionalValues.y, MaximumPositivePositionalValues.y);

        //            Temp.y = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.y, MaxPosYPositveAxis, ShiftSpeed * Time.deltaTime);
        //        }


        //    }
        //    else
        //    {
        //        if (LoopY == true)
        //        {
        //            Temp.y = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.y, MaxPosYPositveAxis, ShiftSpeed * Time.deltaTime);

        //            if (Temp.y == MaxPosYPositveAxis)
        //            {
        //                MaxPosYNegativeAxis = Random.Range(MinimalNegativePositionalValues.y, MaximumNegativePositionalValues.y);

        //                LoopY = false;
        //                Temp.y = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.y, MaxPosYNegativeAxis, ShiftSpeed * Time.deltaTime);
        //            }
        //        }
        //    }

        //    if (LoopZ == false)
        //    {
        //        Temp.z = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.z, MaxPosZNegativeAxis, ShiftSpeed * Time.deltaTime);

        //        if (Temp.z == MaxPosZNegativeAxis)
        //        {
        //            LoopZ = true;
        //            MaxPosZPositveAxis = Random.Range(MinimalPositivePositionalValues.z, MaximumPositivePositionalValues.z);

        //            Temp.z = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.z, MaxPosZPositveAxis, ShiftSpeed * Time.deltaTime);
        //        }


        //    }
        //    else
        //    {
        //        if (LoopZ == true)
        //        {
        //            Temp.z = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.z, MaxPosZPositveAxis, ShiftSpeed * Time.deltaTime);

        //            if (Temp.z == MaxPosZPositveAxis)
        //            {
        //                MaxPosZNegativeAxis = Random.Range(MinimalNegativePositionalValues.z, MaximumNegativePositionalValues.z);

        //                LoopZ = false;
        //                Temp.z = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.z, MaxPosZNegativeAxis, ShiftSpeed * Time.deltaTime);
        //            }
        //        }
        //    }


        //    if (LoopXRot == false)
        //    {
        //        Rot.x = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.x, MaxRotXNegativeAxis, RotationSpeed * Time.deltaTime);

        //        if (Rot.x == MaxRotXNegativeAxis)
        //        {
        //            LoopXRot = true; ;
        //            MaxRotXPositiveAxis = Random.Range(MinimalPositiveRotationalValues.x, MaximumPositiveRotationalValues.x);
        //            Rot.x = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.x, MaxRotXPositiveAxis, RotationSpeed * Time.deltaTime);
        //        }
        //    }
        //    else
        //    {
        //        if (LoopXRot == true)
        //        {
        //            Rot.x = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.x, MaxRotXPositiveAxis, RotationSpeed * Time.deltaTime);

        //            if (Rot.x == MaxRotXPositiveAxis)
        //            {
        //                MaxRotXNegativeAxis = Random.Range(MinimalNegativeRotationalValues.x, MaximumNegativeRotationalValues.x);
        //                LoopXRot = false;
        //                Rot.x = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.x, MaxRotXNegativeAxis, RotationSpeed * Time.deltaTime);
        //            }
        //        }
        //    }

        //    if (LoopYRot == false)
        //    {
        //        Rot.y = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.y, MaxRotYNegativeAxis, RotationSpeed * Time.deltaTime);

        //        if (Rot.y == MaxRotYNegativeAxis)
        //        {
        //            LoopYRot = true;
        //            MaxRotYPositiveAxis = Random.Range(MinimalPositiveRotationalValues.y, MaximumPositiveRotationalValues.y);
        //            Rot.y = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.y, MaxRotYPositiveAxis, RotationSpeed * Time.deltaTime);
        //        }


        //    }
        //    else
        //    {
        //        if (LoopYRot == true)
        //        {
        //            Rot.y = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.y, MaxRotYPositiveAxis, RotationSpeed * Time.deltaTime);

        //            if (Rot.y == MaxRotYPositiveAxis)
        //            {
        //                MaxRotYNegativeAxis = Random.Range(MinimalNegativeRotationalValues.y, MaximumNegativeRotationalValues.y);
        //                LoopYRot = false;
        //                Rot.y = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.y, MaxRotYNegativeAxis, RotationSpeed * Time.deltaTime);
        //            }
        //        }
        //    }

        //    if (LoopZRot == false)
        //    {
        //        Rot.z = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.z, MaxRotZNegativeAxis, RotationSpeed * Time.deltaTime);

        //        if (Rot.z == MaxRotZNegativeAxis)
        //        {
        //            LoopZRot = true;
        //            MaxRotZPositiveAxis = Random.Range(MinimalPositiveRotationalValues.z, MaximumPositiveRotationalValues.z);
        //            Rot.z = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.z, MaxRotZPositiveAxis, RotationSpeed * Time.deltaTime);
        //        }
        //    }
        //    else
        //    {
        //        if (LoopZRot == true)
        //        {
        //            Rot.z = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.z, MaxRotZPositiveAxis, RotationSpeed * Time.deltaTime);
        //            if (Rot.z == MaxRotZPositiveAxis)
        //            {
        //                MaxRotZNegativeAxis = Random.Range(MinimalNegativeRotationalValues.z, MaximumNegativeRotationalValues.z);
        //                LoopZRot = false;
        //                Rot.z = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.z, MaxRotZNegativeAxis, RotationSpeed * Time.deltaTime);
        //            }
        //        }
        //    }

        //    ObjectToAnimate.transform.localPosition = Temp;
        //    ObjectToAnimate.transform.localEulerAngles = Rot;
        //}
        //IEnumerator MagazineRemoveSoundTimer()
        //{
        //    yield return new WaitForSeconds(Reload.MagazineDropSoundDelay);
        //    AudioSource aud = Instantiate(Reload.MagazineDropSound, transform.position, transform.rotation);
        //    Destroy(aud.gameObject, aud.clip.length);
        //}
        public IEnumerator ReloadCoroutine(float AnimTimer)
        {
            //if (Reload.MagazineDropSound != null)
            //{
            //    StartCoroutine(MagazineRemoveSoundTimer());
            //}
            ShotMade = false;
            Reload.WeaponResetPositioningSpeed = StoreWeaponResetingSpeed;
            LerpNow = false;
            Reload.ForceReloadPositioning = false;
          //  LerpBack = false;
        
            if (ShootingFeatures.UseMuzzleFlashMesh == true)
            {
                if (ShootingFeatures.MuzzleFlashMesh != null)
                {
                    ShootingFeatures.MuzzleFlashMesh.SetActive(false);
                }
            }
            if (PlayerManagerObject != null)
            {
                if (PlayerManagerObject.IsScoping == true)
                {
                    PlayerManagerObject.Aiming();
                }
            }

            if (PlayerManager.instance != null)
            {
                for (int x = 0; x < Reload.ButtonsToDisableInteractionOnReload.Length; x++)
                {
                    if (Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>() != null)
                    {
                        Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>().raycastTarget = false;
                    }
                    Reload.ButtonsToDisableInteractionOnReload[x].interactable = false;
                }
            }
            Reload.isreloading = true;

            if (Reload.CurrentAmmos > 0)
            {
                //AudioSource aud = Instantiate(WeaponSounds.ReloadSoundAudioSource, transform.position, transform.rotation);
                //WeaponSounds.ReloadSoundAudioSource.PlayOneShot(WeaponSounds.ReloadAudioClip);
                //Destroy(aud.gameObject, aud.clip.length);
                WeaponSounds.ReloadSoundAudioSource.PlayOneShot(WeaponSounds.ReloadAudioClip);
                RequiredComponents.WeaponAnimatorComponent.Play(ReloadAnimationName, -1, 0f);
            }
            else
            {
                //AudioSource aud = Instantiate(WeaponSounds.ReloadEmptySoundAudioSource, transform.position, transform.rotation);
                //WeaponSounds.ReloadEmptySoundAudioSource.PlayOneShot(WeaponSounds.ReloadEmptyAudioClip);
                //Destroy(aud.gameObject, aud.clip.length);
                WeaponSounds.ReloadEmptySoundAudioSource.PlayOneShot(WeaponSounds.ReloadEmptyAudioClip);
                RequiredComponents.WeaponAnimatorComponent.Play(ReloadEmptyAnimationName, -1, 0f);

                MagazineCartridges SA = FindObjectOfType<MagazineCartridges>();
                if (SA != null)
                {
                    SA.ActivateForReload();
                }
            }
            yield return new WaitForSeconds(AnimTimer);
            if (Reload.TotalAmmo < Reload.FullMagazineSize && Reload.TotalAmmo > 0)
            {
                int maxiammo = Reload.TotalAmmo;
                Reload.TotalAmmo = 0;
                Reload.CurrentAmmos = Reload.CurrentAmmos + maxiammo;
                Reload.ShotsInMagCountText.text = Reload.CurrentAmmos.ToString();
                Reload.TotalAmmoText.text = Reload.TotalAmmo.ToString();

            }
            else if (Reload.TotalAmmo >= Reload.FullMagazineSize)
            {
                int reduceammo = Reload.FullMagazineSize - Reload.CurrentAmmos;
                Reload.TotalAmmo = Reload.TotalAmmo - reduceammo;

                if (Reload.TotalAmmoText != null)
                {
                    Reload.TotalAmmoText.text = Reload.TotalAmmo.ToString();
                }

                if(Reload.ShotsInMagCountText != null)
                {
                    Reload.CurrentAmmos =  Reload.TotalAmmo - Reload.FullMagazineSize;
                    Reload.CurrentAmmos = Reload.TotalAmmo - Reload.CurrentAmmos;
                    Reload.ShotsInMagCountText.text = Reload.CurrentAmmos.ToString();
                }
              
            }

            Reload.isreloading = false;
            isReloadCompleted = false;

            CheckForWalking();
            ResetHipFireState();
            
         
            if (PlayerManager.instance != null)
            {
                for (int x = 0; x < Reload.ButtonsToDisableInteractionOnReload.Length; x++)
                {
                    if (Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>() != null)
                    {
                        Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>().raycastTarget = true;
                    }
                    Reload.ButtonsToDisableInteractionOnReload[x].interactable = true;
                }
            }
            CanTweakHipFirePositionAndRotations = false; // newly added on 22/04/21
            CanTweakAimedPositionAndRotations = false; // newly added on 22/04/21

           
        }
        public void ResetHipFireState()
        {
            if (Reload.isreloading == false)
            {
                if (UseProceduralAimedBreath == false || UseProceduralAimedBreath == true && CombineProceduralBreathWithAnimation == true)
                {
                    if (IsplayingAimedAnim == true)
                    {
                        if (PlayerManager.instance != null)
                        {
                            if (PlayerManager.instance.IsMoving == true)
                            {
                                //  AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.RunAnimationname, -1, 0f);
                                RequiredComponents.WeaponAnimatorComponent.SetBool(IdleAnimationParametreName, false);
                                RequiredComponents.WeaponAnimatorComponent.SetBool(RunAnimationParametreName, true);
                                RequiredComponents.WeaponAnimatorComponent.SetBool(WalkAnimationParametreName, false);
                                RequiredComponents.WeaponAnimatorComponent.SetFloat(WeaponAnimationClipsSpeeds.SpeedParameterName, 1.0f);
                                IsplayingAimedAnim = false;
                            }
                            else
                            {
                                RequiredComponents.WeaponAnimatorComponent.Play(IdleAnimationName, -1, 0f);
                                RequiredComponents.WeaponAnimatorComponent.SetBool(IdleAnimationParametreName, true);
                                RequiredComponents.WeaponAnimatorComponent.SetBool(RunAnimationParametreName, false);
                                RequiredComponents.WeaponAnimatorComponent.SetBool(WalkAnimationParametreName, false);
                                RequiredComponents.WeaponAnimatorComponent.SetFloat(WeaponAnimationClipsSpeeds.SpeedParameterName, 1.0f);
                                IsplayingAimedAnim = false;
                            }
                        }
                        else
                        {
                            RequiredComponents.WeaponAnimatorComponent.Play(IdleAnimationName, -1, 0f);
                            RequiredComponents.WeaponAnimatorComponent.SetBool(IdleAnimationParametreName, true);
                            RequiredComponents.WeaponAnimatorComponent.SetBool(WalkAnimationParametreName, false);
                            RequiredComponents.WeaponAnimatorComponent.SetBool(RunAnimationParametreName, false);
                            RequiredComponents.WeaponAnimatorComponent.SetFloat(WeaponAnimationClipsSpeeds.SpeedParameterName, 1.0f);
                            IsplayingAimedAnim = false;
                        }
                    }
                }
            }

        }
        public void CustomReloading()
        {
            if (Reload.TotalAmmo > 0)
            {
                Reload.Reloadation = true;
            }
        }
        public void InscopeEffect()
        {
            //float GetAimedDuration =
            //if (ShootingFeatures.SniperScopeScript != null && ShootingFeatures.UseSniperScopeUI == true)
            //{
            //    if ()
            //        currentZoomLevel = ZoomLevels;
            //    // Tween the camera's field of view
            //    LeanTween.value(RequiredComponents.PlayerCamera.gameObject, UpdateCameraFOV, RequiredComponents.PlayerCamera.fieldOfView, CamerasFovParameters.PlayerCameraMagnifiedFov, C)
            //        .setOnUpdate((float value) =>
            //        {
            //            PlayerWeaponInUse.RequiredComponents.PlayerCamera.fieldOfView = value;
            //        })
            //        .setOnComplete(() =>
            //        {
            //            Debug.Log("Camera FOV change complete.");

            //        });
            //}
            IsAimed = true;
            IsHipFire = false;

           
        }
        //private void UpdateCameraFOV(float value)
        //{
        //    PlayerWeaponInUse.RequiredComponents.PlayerCamera.fieldOfView = value;
        //}
        public void OutScope()
        {
            IsAimed = false;
            IsHipFire = true;
        }
        public void CheckForWalking()
        {
            if (IsWalking == true)
            {
                RequiredComponents.WeaponAnimatorComponent.SetBool(IdleAnimationParametreName, false);
                RequiredComponents.WeaponAnimatorComponent.SetBool(WalkAnimationParametreName, true);
                RequiredComponents.WeaponAnimatorComponent.SetBool(RunAnimationParametreName, false);

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
            }
            else
            {

                RequiredComponents.WeaponAnimatorComponent.SetBool(IdleAnimationParametreName, true);
                RequiredComponents.WeaponAnimatorComponent.Play(IdleAnimationName);
                RequiredComponents.WeaponAnimatorComponent.SetBool(WalkAnimationParametreName, false);
                RequiredComponents.WeaponAnimatorComponent.SetBool(RunAnimationParametreName, false);

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
        public void AimedBreathAnimController()
        {
            if (JoyStick.Instance != null)
            {
                if (JoyStick.Instance.IsWalking == false)
                {
                    if (IsplayingAimedAnim == false)
                    {
                        if (AimedAnimState == true)
                        {
                            RequiredComponents.WeaponAnimatorComponent.SetBool(IdleAnimationParametreName, true);
                            RequiredComponents.WeaponAnimatorComponent.SetBool(WalkAnimationParametreName, false);
                            RequiredComponents.WeaponAnimatorComponent.SetBool(RunAnimationParametreName, false);
                            StartCoroutine(Aimeddealay());
                            AimedAnimState = false;
                        }
                    }
                }
                else
                {
                    if (AimedAnimState == false)
                    {
                        IsplayingAimedAnim = false;
                        AimedAnimState = true;
                    }
                }
            }
        }
        IEnumerator Aimeddealay()
        {
            yield return new WaitForSeconds(ShootingFeatures.IdleToAimedAnimationTransitionDelay);

            if (IsAimed == true)
            {
                //if (WeaponAnimationClipsSpeeds.AimedAnimationName == IdleAnimationName)
                //{
                //    RequiredComponents.WeaponAnimatorComponent.SetBool(IdleAnimationParametreName, true);
                //    RequiredComponents.WeaponAnimatorComponent.SetBool(WalkAnimationParametreName, false);
                //    RequiredComponents.WeaponAnimatorComponent.SetBool(RunAnimationParametreName, false);

                //    if (Crouch.instance != null)
                //    {
                //        if (Crouch.instance.IsCrouching == false)
                //        {
                //            RequiredComponents.WeaponAnimatorComponent.SetFloat(WeaponAnimationClipsSpeeds.SpeedParameterName, WeaponAnimationClipsSpeeds.StandAimedAnimationSpeed);
                //        }
                //        else
                //        {
                //            RequiredComponents.WeaponAnimatorComponent.SetFloat(WeaponAnimationClipsSpeeds.SpeedParameterName, WeaponAnimationClipsSpeeds.CrouchAimedAnimationSpeed);
                //        }
                //    }
                //    else
                //    {
                //        RequiredComponents.WeaponAnimatorComponent.SetFloat(WeaponAnimationClipsSpeeds.SpeedParameterName, WeaponAnimationClipsSpeeds.StandAimedAnimationSpeed);
                //    }
                   
                //}
                //else
                //{
                   
                  

                    if (Crouch.instance != null)
                    {
                        if (Crouch.instance.IsCrouching == false)
                        {
                            RequiredComponents.WeaponAnimatorComponent.SetFloat(WeaponAnimationClipsSpeeds.SpeedParameterName, WeaponAnimationClipsSpeeds.StandAimedAnimationSpeed);
                        }
                        else
                        {
                            RequiredComponents.WeaponAnimatorComponent.SetFloat(WeaponAnimationClipsSpeeds.SpeedParameterName, WeaponAnimationClipsSpeeds.CrouchAimedAnimationSpeed);
                        }
                    }
                    else
                    {
                        RequiredComponents.WeaponAnimatorComponent.SetFloat(WeaponAnimationClipsSpeeds.SpeedParameterName, WeaponAnimationClipsSpeeds.StandAimedAnimationSpeed);
                    }
                //}
                IsplayingAimedAnim = true;
            }
        }
     
        public void Fire()
        {
            if (hit.collider != null)
            {
                //Debug.Log(hit.collider.name);
                if (hit.collider.gameObject.transform.root.tag == "AI")
                {
                    hit.collider.gameObject.transform.root.SendMessage("FindColliderName", hit.collider.name, SendMessageOptions.DontRequireReceiver);
                    if (hit.collider.gameObject.transform.tag != "WeakPoint")
                    {
                        hit.collider.gameObject.transform.root.SendMessage("Takedamage", ShootingFeatures.TargetDamagePerShot, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        hit.collider.gameObject.transform.root.SendMessage("WeakPointdamage", ShootingFeatures.TargetDamagePerShot, SendMessageOptions.DontRequireReceiver);
                    }
                    hit.collider.gameObject.transform.root.SendMessage("Effects", hit, SendMessageOptions.DontRequireReceiver);
                   

                    if(hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
                    {
                        hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.WhoKilledMe = transform.root;
                    }

                    if (FriendlyFire.instance != null)
                    {
                        if (hit.collider.gameObject.transform.root.GetComponent<Targets>() != null)
                        {
                            if (hit.collider.gameObject.transform.root.GetComponent<Targets>().MyTeamID == EnemyIDScript.MyTeamID)
                            {
                                if (hit.collider.gameObject.transform.root.GetComponent<HumanoidAiHealth>() != null)
                                {
                                    FriendlyFire.instance.Enemy = hit.collider.gameObject.transform.root.gameObject;

                                    if (hit.collider.gameObject.transform.root.GetComponent<HumanoidAiHealth>().IsDied == true)
                                    {
                                        FriendlyFire.instance.TraitorPlayer(true);
                                    }
                                    else
                                    {
                                        FriendlyFire.instance.TraitorPlayer(false);
                                    }
                                }
                                else
                                {
                                    FriendlyFire.instance.TraitorPlayer(false);
                                }

                            }
                        }
                    }
                }
                if (hit.collider.transform.root.tag == "Turret")
                {
                    if (hit.collider.transform.root.GetComponent<Turret>() != null)
                    {
                        hit.collider.transform.root.GetComponent<Turret>().TakeDamage(ShootingFeatures.TargetDamagePerShot);
                    }
                }
                if (hit.collider.gameObject.tag == "Target")
                {
                    if (hit.transform.gameObject.GetComponent<Target>() != null)
                    {
                        hit.transform.gameObject.GetComponent<Target>().StartRotating = true;
                    }
                }
                if (hit.collider.gameObject.transform.root.tag != "Player" && hit.collider.gameObject.transform.root.tag != "AI" && hit.collider.gameObject.tag != "WeakPoint")
                {
                  
                    if (hit.collider.gameObject.GetComponent<ImpactEffectSpawner>() != null)
                    {
                        //Vector3 p = new Vector3(hit.point.x + ImpactEffectOffsetValue, hit.point.y + ImpactEffectOffsetValue, hit.point.z + ImpactEffectOffsetValue);                   
                        GameObject impacteffect = Instantiate(hit.collider.gameObject.GetComponent<ImpactEffectSpawner>().HitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                        hit.collider.gameObject.GetComponent<ImpactEffectSpawner>().PlaySound();

                        if (impacteffect.gameObject.GetComponent<ImpactEffect>() != null)
                        {
                            if (transform.root.GetComponent<Targets>() != null)
                            {
                                // Keep the below lines to be commented  (line number:2024,2025) because during emergency state based on the selected properties it should affect the other Humanoid AI agent and not only friendlies
                                // as when you overrite the properties it affects the behaviour when you change promatically that only friendlies will be affected using below code.
                                // This may create a bug when you shoot the enemies and impact effects
                                // are spawned with the settings being force emergency state on AI agent.so Make sure you don't use this code at all even in the case of sounds. for more details why not to use you can change
                                // impact effect properties to be Force Emergency state and choose All teams and shoot near the enemies using player weapon by uncommenting this code.

                                // impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.Enemy = transform.root;
                                // impacteffect.gameObject.GetComponent<ImpactEffect>().TeamWhichWillBeAffectedByTheShot(transform.root.GetComponent<Targets>().MyTeamID);
                                impacteffect.gameObject.GetComponent<ImpactEffect>().EffectActivation(transform);

                            }
                        }

                    }
                    else
                    {
                        if (pooler != null)
                        {

                            GameObject impacteffect = pooler.SpawnFromPool(ShootingFeatures.DefaultImpactEffectName, hit.point, Quaternion.LookRotation(hit.normal));

                            if (impacteffect != null)
                            {
                                if (impacteffect.GetComponent<AudioSource>() != null)
                                {
                                    if (!impacteffect.GetComponent<AudioSource>().isPlaying)
                                    {
                                        impacteffect.GetComponent<AudioSource>().Play();
                                    }

                                }

                                if (impacteffect.gameObject.GetComponent<ImpactEffect>() != null)
                                {
                                    if (transform.root.GetComponent<Targets>() != null)
                                    {
                                        // Keep the below lines to be commented  (line number:2024,2025) because during emergency state based on the selected properties it should affect the other Humanoid AI agent and not only friendlies
                                        // as when you overrite the properties it affects the behaviour when you change promatically that only friendlies will be affected using below code.
                                        // This may create a bug when you shoot the enemies and impact effects
                                        // are spawned with the settings being force emergency state on AI agent.so Make sure you don't use this code at all even in the case of sounds. for more details why not to use you can change
                                        // impact effect properties to be Force Emergency state and choose All teams and shoot near the enemies using player weapon by uncommenting this code.

                                       // impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.Enemy = transform.root;
                                       // impacteffect.gameObject.GetComponent<ImpactEffect>().TeamWhichWillBeAffectedByTheShot(transform.root.GetComponent<Targets>().MyTeamID);
                                        impacteffect.gameObject.GetComponent<ImpactEffect>().EffectActivation(transform);
                                    }
                                }

                            }

                        }
                    }

                    //if (hit.collider.gameObject.GetComponent<HitImpactEffect>() != null)
                    //{
                    //    if (hit.collider.gameObject.GetComponent<HitImpactEffect>().DeactivateBulletOnCollision == false)
                    //    {
                    //        if (hit.collider.GetComponent<MeshCollider>() != null)
                    //        {
                    //            hit.collider.GetComponent<MeshCollider>().convex = true;
                    //            hit.collider.isTrigger = true;
                    //        }
                    //        else
                    //        {
                    //            hit.collider.isTrigger = true;
                    //        }
                    //        StoreHit = hit;
                    //        if (hit.collider.gameObject.GetComponent<HitImpactEffect>().SpawnImpactEffectOnSurfacesBehindThisOne == false)
                    //        {
                    //            ShouldSpawnImpactEffect = false;
                    //        }
                    //        CanRaycastPass = true;
                    //    }

                    //}
                    //else
                    //{
                    //    if (StoreHit.collider != null)
                    //    {
                    //        if (StoreHit.collider.GetComponent<MeshCollider>() != null)
                    //        {
                    //            StoreHit.collider.isTrigger = false;
                    //            StoreHit.collider.GetComponent<MeshCollider>().convex = false;

                    //        }
                    //        else
                    //        {
                    //            StoreHit.collider.isTrigger = false;
                    //        }
                    //        ShouldSpawnImpactEffect = true;
                    //    }
                    //}

                    

                }
                if(RaycastForce.AddRaycastForce == true)
                {
                    AddForceToRigidBodies(hit, transform);
                }
             
            }
        }
        public void AddForceToRigidBodies(RaycastHit hit, Transform RaycastToStartFrom)
        {

            //Vector3 directionToTarget = hit.transform.position - RaycastToStartFrom.position; // Grenade -> Target direction
            //Debug.DrawLine(RaycastToStartFrom.position, RaycastToStartFrom.position + RaycastToStartFrom.forward * directionToTarget.magnitude, Color.red, 0.1f);

            if (hit.transform.root.tag == "AI" && !RaycastForce.TargetsToApplyForce.Contains(hit.transform.root)) // AI Logic
            {
                if (hit.transform.root.GetComponent<HumanoidAiHealth>() != null)
                {
                    if (hit.transform.root.GetComponent<HumanoidAiHealth>().Health <= 0)
                    {
                        if (hit.transform.root.GetComponent<Animator>() != null)
                        {
                            hit.transform.root.GetComponent<Animator>().enabled = false;
                        }

                        RaycastForce.TargetsToApplyForce.Add(hit.transform.root);
                        Vector3 grenadePosition = RaycastToStartFrom.position; // Position of the grenade
                        Transform aiRoot = hit.transform.root;            // AI root transform
                        Vector3 aiToGrenadeDirection = (grenadePosition - aiRoot.position).normalized;

                        // Calculate directions using Dot Product
                        float rightDot = Vector3.Dot(aiRoot.right, aiToGrenadeDirection);       // Right vs Left
                        float forwardDot = Vector3.Dot(aiRoot.forward, aiToGrenadeDirection);   // Front vs Back

                        // Default force direction
                        Vector3 forceDirection = Vector3.zero;
                        string closestDirection = "";

                        // Determine closest direction and apply force in the opposite direction
                        if (Mathf.Abs(rightDot) > Mathf.Abs(forwardDot)) // Left/Right dominates
                        {
                            if (rightDot > 0) // Grenade is on the RIGHT → Apply force to the LEFT
                            {
                                forceDirection = -aiRoot.right;
                                closestDirection = "Right → Force Left";
                            }
                            else // Grenade is on the LEFT → Apply force to the RIGHT
                            {
                                forceDirection = aiRoot.right;
                                closestDirection = "Left → Force Right";
                            }
                        }
                        else // Front/Back dominates
                        {
                            if (forwardDot > 0) // Grenade is in FRONT → Apply force BACKWARD
                            {
                                forceDirection = -aiRoot.forward;
                                closestDirection = "Front → Force Backward";
                            }
                            else // Grenade is in BACK → Apply force FORWARD
                            {
                                forceDirection = aiRoot.forward;
                                closestDirection = "Back → Force Forward";
                            }
                        }

                        // Apply force to all rigidbodies in AI
                        foreach (Transform g in aiRoot.GetComponentsInChildren<Transform>())
                        {
                            if (g.gameObject.GetComponent<Rigidbody>() != null)
                            {
                                Rigidbody rb = g.gameObject.GetComponent<Rigidbody>();
                                rb.isKinematic = false;
                                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                                // Calculate force magnitude based on distance
                                float distanceToGrenade = Vector3.Distance(grenadePosition, aiRoot.position);
                                float distanceFactor = Mathf.Clamp01(1 - (distanceToGrenade / RaycastForce.RadiusToApplyForce)); // Normalize effect
                                float forceMagnitude = Mathf.Lerp(RaycastForce.MinRaycastForceToAi, RaycastForce.MaxRaycastForceToAi, distanceFactor);

                                // Apply the force
                                rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);

                                // Debugging information
                                //Debug.Log($"{g.gameObject.name} - Closest Direction: {closestDirection}, Force Applied: {forceDirection}, Magnitude: {forceMagnitude}");
                            }
                        }
                    }
                }

            }          
            else // Non-AI Logic
            {
                //foreach (Transform g in hit.transform.root.GetComponentsInChildren<Transform>())
                //{
                    if (hit.transform.GetComponent<Rigidbody>() != null)
                    {
                        Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                        rb.isKinematic = false;
                        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                        // Non-AI: Add force in explosion normal direction
                        rb.AddForce(-hit.normal * RaycastForce.NonAIGameObjectImpactForce, ForceMode.Impulse);
                    }
                //}
            }

        }
        //public void AddingForceToHumanoidAI(RaycastHit hit)
        //{
        //    if (RaycastForce.AddRaycastForce)
        //    {

        //        if (hit.transform.root.GetComponent<HumanoidAiHealth>() != null)
        //        {
        //            if (hit.transform.root.GetComponent<HumanoidAiHealth>().Health <= 0)
        //            {
        //                foreach (Transform g in hit.transform.gameObject.transform.root.GetComponentsInChildren<Transform>())
        //                {
        //                    Rigidbody rb = g.gameObject.GetComponent<Rigidbody>();
        //                    if (rb != null)
        //                    {

        //                        // Calculate and apply the upward force in world space
        //                        Vector3 upwardForceWorld = Random.Range(RaycastForce.MinUpwardForceToApplyOnTarget, RaycastForce.MaxUpwardForceToApplyOnTarget) * Vector3.up;
        //                        rb.AddForce(upwardForceWorld * Random.Range(RaycastForce.MinRaycastForce, RaycastForce.MaxRaycastForce));

        //                        // Calculate and apply other forces in local space
        //                        Vector3 forceDirectionLocal = Random.Range(RaycastForce.MinRightForceToApplyOnTarget, RaycastForce.MaxRightForceToApplyOnTarget) * Vector3.right +
        //                                                    Random.Range(RaycastForce.MinLeftForceToApplyOnTarget, RaycastForce.MaxLeftForceToApplyOnTarget) * Vector3.left +
        //                                                    Random.Range(RaycastForce.MinBackwardForceToApplyOnTarget, RaycastForce.MaxBackwardForceToApplyOnTarget) * Vector3.back +
        //                                                    Random.Range(RaycastForce.MinForwardForceToApplyOnTarget, RaycastForce.MaxForwardForceToApplyOnTarget) * Vector3.forward;

        //                        rb.AddRelativeForce(forceDirectionLocal * Random.Range(RaycastForce.MinRaycastForce, RaycastForce.MaxRaycastForce));
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //foreach (Transform g in hit.transform.gameObject.transform.root.GetComponentsInChildren<Transform>())
        //{
        //    Rigidbody rb = g.gameObject.GetComponent<Rigidbody>();
        //    if (rb != null)
        //    {
        //        // Calculate and apply the upward force in world space
        //        Vector3 upwardForceWorld = Random.Range(RaycastForce.MinUpwardForceToApplyOnTarget, RaycastForce.MaxUpwardForceToApplyOnTarget) * ..transform.up;
        //        rb.AddForce(upwardForceWorld * Random.Range(RaycastForce.MinRaycastForce, RaycastForce.MaxRaycastForce));

        //        // Calculate and apply other forces in local space
        //        Vector3 forceDirectionLocal = Random.Range(RaycastForce.MinRightForceToApplyOnTarget, RaycastForce.MaxRightForceToApplyOnTarget) * rb.gameObject.transform.right +
        //                                    Random.Range(RaycastForce.MinLeftForceToApplyOnTarget, RaycastForce.MaxLeftForceToApplyOnTarget) * (-rb.gameObject.transform.right) +
        //                                    Random.Range(RaycastForce.MinBackwardForceToApplyOnTarget, RaycastForce.MaxBackwardForceToApplyOnTarget) * (-rb.gameObject.transform.forward) +
        //                                    Random.Range(RaycastForce.MinForwardForceToApplyOnTarget, RaycastForce.MaxForwardForceToApplyOnTarget) * rb.gameObject.transform.forward;

        //        rb.AddRelativeForce(forceDirectionLocal * Random.Range(RaycastForce.MinRaycastForce, RaycastForce.MaxRaycastForce));
        //    }
        //}


        //foreach (Transform g in hit.transform.gameObject.transform.root.GetComponentsInChildren<Transform>())
        //{
        //    Rigidbody rb = g.gameObject.GetComponent<Rigidbody>();
        //    if (rb != null)
        //    {

        //        rb.AddForce(-hit.normal * RaycastForce.NonAIImpactForce);
        //// 1. Calculate the shooting direction from player (or camera) to the hit point
        //Vector3 shootingDirection = (hit.point - Camera.main.transform.position).normalized;

        //// DEBUG: Visualize the shooting direction
        //Debug.DrawRay(hit.point, shootingDirection * 5, Color.red, 2.0f);
        //Debug.Log($"Shooting Direction: {shootingDirection}");

        //// 2. Apply upward force in world space
        //float upwardForce = Random.Range(RaycastForce.MinUpwardForceToApplyOnTarget, RaycastForce.MaxUpwardForceToApplyOnTarget);
        //Vector3 upwardForceWorld = upwardForce * Vector3.up; // World-space upward force
        //rb.AddForce(upwardForceWorld, ForceMode.Impulse);

        //// 3. Calculate and apply other forces relative to the shooting direction
        //Vector3 rightForce = Random.Range(RaycastForce.MinRightForceToApplyOnTarget, RaycastForce.MaxRightForceToApplyOnTarget) * Vector3.Cross(shootingDirection, Vector3.up); // Perpendicular to shooting direction
        //Vector3 leftForce = Random.Range(RaycastForce.MinLeftForceToApplyOnTarget, RaycastForce.MaxLeftForceToApplyOnTarget) * -Vector3.Cross(shootingDirection, Vector3.up); // Opposite to right force
        //Vector3 backwardForce = Random.Range(RaycastForce.MinBackwardForceToApplyOnTarget, RaycastForce.MaxBackwardForceToApplyOnTarget) * shootingDirection; // Opposite of shooting direction
        //Vector3 forwardForce = Random.Range(RaycastForce.MinForwardForceToApplyOnTarget, RaycastForce.MaxForwardForceToApplyOnTarget) * -shootingDirection; // Along shooting direction

        //// Combine forces
        //Vector3 totalForce = rightForce + leftForce + backwardForce + forwardForce;

        //// Apply the combined forces
        //rb.AddForce(totalForce, ForceMode.Impulse);

        //// DEBUG: Log applied forces
        //Debug.Log($"Applied Forces -> Right: {rightForce}, Left: {leftForce}, Backward: {backwardForce}, Forward: {forwardForce}, Upward: {upwardForceWorld}");
        //    }
        //}



        //        }
        //    }
        //}
    }
}







//using UnityEngine;
//using System.Collections;
//using UnityEngine.UI;

//// This Script is Responsible For Weapon Shooting 
//namespace MobileActionKit
//{
//    public class Gunscript : MonoBehaviour
//    {
//        public static Gunscript instance;

//        [TextArea]
//        [ContextMenuItem("Reset Description", "ResettingDescription")]
//        public string ScriptInfo = "This Script Controls The Whole Weapon Shooting Behaviour";
//        [Space(10)]

//        [HideInInspector]
//        public ObjectPooler pooler;

//        public RaycastHit hit;
//        [HideInInspector]
//        public bool shootnow = false;
//        private float Nexttimetofire = 0f;
//        [HideInInspector]
//        public bool playershoot;
//        [System.Serializable]
//        public class RequireComponents
//        {
//            public WeaponId WeaponIdScript;
//            public Button FireButton;
//            public Camera PlayerCamera;
//            public SoundManager CreateNoiseScript;
//        }

//        public RequireComponents Components;

//        [System.Serializable]
//        public class AboutWeapon
//        {
//            [Tooltip("Weapon Shooting Range")]
//            public float ShootingRange = 100f;

//            public bool EnableSilencer = false;

//            public bool UseMeshMuzzleFlash = false;
//            public ParticleSystem MuzzleFlash;
//            public GameObject MuzzleMesh;

//            [Tooltip("Affects the Weapon Fire Rate In Both Raycast and projectile Shooting Functionality")]
//            public float WeaponFireRate = 15f;

//            public float TimeToDeactivateMuzzleMesh = 0.2f;
//            [Tooltip("Damage To target on shoot")]
//            public float DamageToTarget = 5f;

//            [Tooltip("If The Bullet Hits On Other Colliders For Ex - Road , Houses etc. Than Which Prefab To Spawn From Object Pooler on that hit point - Prefab could be Ex - BulletHole")]
//            public string ImpactEffectName = "BulletHole";

//            public float MinZRotationOnTrigger = -180f;
//            public float MaxZRotationOnTrigger = 180f;
//        }

//        public AboutWeapon ShootingFeatures;

//        [System.Serializable]
//        public class Options
//        {
//            public bool MoveTouchpadOnHoldingFire = false;

//            [Tooltip("Main Camera Attached To Player")]
//            public GameObject WeaponCrosshair;

//            public LayerMask VisibleLayers;

//            [Tooltip("If Not Assigned Shooting Will Happenend from the Middle Of The Screen")]
//            public GameObject HipFirePointPosition;
//            [Tooltip("If Not Assigned Aiming Will Happenend from the Middle Of The Screen")]
//            public GameObject AimedPointPosition;

//            [Tooltip("Shooting Continously on holding Fire Button")]
//            public bool ContinuousFire;

//            public float WalkToIdleTransitionTime = 0.25f;
//            public float ShotAnimationDelay = 0.1f;
//        }

//        public Options WeaponOptions;

//        [System.Serializable]
//        public class BulletShellClass
//        {
//            [Tooltip("Bullet Shell Name Inside Object Pooler Script")]
//            public string BulletShellName = "BulletShell";
//            public GameObject BulletShellSpawnPosition;
//            public float ShellsEjectingSpeed = 10f;
//        }

//        public BulletShellClass BulletShell;
//        bool DoOnce = false;

//        public enum ShootingOptions
//        {
//            RaycastShootingWithoutProjectiles,
//            RaycastShootingWithVisualProjectiles,
//            ProjectileShootingWithoutRaycast
//        }

//        [System.Serializable]
//        public class ShootingTypesClass
//        {
//            [Tooltip("Projectile Name inside Object Pooler")]
//            public string ProjectileName = "Bullet";
//            public int BulletsToSpawn = 1;

//            public ShootingOptions ShootingOption;
//        }

//        public ShootingTypesClass ShootingTypes;


//        [System.Serializable]
//        public class RaycastForceClass
//        {
//            [Tooltip("If checked than the force will be applied to humanoid Ai agent at the time of death using the fields provided below.")]
//            public bool AddRaycastForce = false;

//            [Tooltip("Minimum overall force to apply")]
//            public float MinRaycastForce = 800;
//            [Tooltip("Maximum overall force to apply")]
//            public float MaxRaycastForce = 1100;

//            [Tooltip("Minimum force to apply in upward direction.")]
//            public float MinUpwardForceToApplyOnTarget = 2f;
//            [Tooltip("Maximum force to apply in upward direction.")]
//            public float MaxUpwardForceToApplyOnTarget = 5f;

//            [Tooltip("Minimum force to apply in right direction.")]
//            public float MinRightForceToApplyOnTarget = 2f;
//            [Tooltip("Maximum force to apply in right direction.")]
//            public float MaxRightForceToApplyOnTarget = 2f;

//            [Tooltip("Minimum force to apply in left direction.")]
//            public float MinLeftForceToApplyOnTarget = 1f;
//            [Tooltip("Maximum force to apply in left direction.")]
//            public float MaxLeftForceToApplyOnTarget = 1f;

//            [Tooltip("Minimum force to apply in backward direction.")]
//            public float MinBackwardForceToApplyOnTarget = 5f;
//            [Tooltip("Maximum force to apply in backward direction.")]
//            public float MaxBackwardForceToApplyOnTarget = 8f;

//            [Tooltip("Minimum force to apply in forward direction.")]
//            public float MinForwardForceToApplyOnTarget = 1f;
//            [Tooltip("Maximum force to apply in forward direction.")]
//            public float MaxForwardForceToApplyOnTarget = 1f;

//        }

//        [Tooltip("In case 'ProjectileShootingWithoutRaycast' is not selected than the force on the Humanoid Ai agent during death will be applied using raycast.")]
//        public RaycastForceClass RaycastForce;

//        [HideInInspector]
//        public bool IsFire = false;
//        private Targets EnemyIDScript;
//        [HideInInspector]
//        public int StoreTeamId;
//        [HideInInspector]
//        public bool UpdateKills = false;
//        MechaniseAiHealth drone;
//        [HideInInspector]
//        public bool IsWalking = false;

//        float Timer = 0;
//        [HideInInspector]
//        public bool ShotMade = false;


//        [HideInInspector]
//        public bool IsAimed = false;
//        [HideInInspector]
//        public bool IsHipFire = false;

//        float horizontal, vertical, timer, waveSlice;

//        [HideInInspector]
//        public float ObjectToShakeMidPoint = 2.0f;

//        bool ShotMaded = false;
//        float ShotLength;
//        float ReloadLength;
//        float ReloadEmptyLength;
//        [HideInInspector]
//        public float RemoveLength;

//        bool IsFireAnimNameOk = false;
//        bool IsReloadAnimNameOk = false;
//        bool IsReloadEmptyAnimNameOk = false;
//        bool Wield = false;
//        bool Remove = false;

//        [HideInInspector]
//        public bool UseProceduralAimedBreath = false;
//        [HideInInspector]
//        public Transform ObjectToAnimate;
//        [HideInInspector]
//        public float ShiftSpeed;
//        float SaveShiftSpeed;

//        [HideInInspector]
//        public float RotationSpeed;

//        [HideInInspector]
//        public bool CombineProceduralBreathWithAnimation = false;

//        [System.Serializable]
//        public class RecoilClass
//        {
//            public GenericRecoil GenericRecoilScript;
//            public AuthenticRecoil AuthenticRecoilScript;
//        }

//        public RecoilClass Recoil;



//        [System.Serializable]
//        public class ReloadingClass
//        {
//            [Tooltip("GUN RELOADING FUNCTIONS")]
//            public int InitialAmmo = 30;
//            public int FullMagazineSize = 30;
//            [HideInInspector]
//            public int CurrentAmmos;
//            [HideInInspector]
//            public bool isreloading = false;

//            public int MaximumAmmo = 60;
//            public Text MaximumAmmoText;
//            public Text BulletsInMagazinesText;

//            [HideInInspector]
//            public bool Reloadation = false;

//            [HideInInspector]
//            public bool ForceReloadPositioning = false;
//            public Vector3 ReloadPosition;
//            public Vector3 ReloadRotation;
//            public bool UseLerpTimer = true;
//            public float TimeToStartLerpingToIdle = 3f;
//            public float WeaponPositionLerpSpeed = 2f;
//            public float WeaponRotationLerpSpeed = 2f;
//            public float WeaponResetPositioningSpeed = 2f;

//            public Button[] ButtonsToDisableInteractionOnReload;

//            public AudioSource MagazineDropSound;
//            public float MagazineDropSoundDelay;
//        }

//        public ReloadingClass Reload;
//        public BulletSreadOptions BulletSpread;

//        [System.Serializable]
//        public class Sounds
//        {
//            public AudioSource FireSoundAudioSource;
//            public AudioSource ReloadSoundAudioSource;
//            public AudioSource ReloadEmptySoundAudioSource;
//            public AudioSource WieldSoundAudioSource;
//            public AudioSource RemoveSoundAudioSource;
//        }

//        public Sounds WeaponSounds;

//        [System.Serializable]
//        public class Animations
//        {
//            //  public Animator HeadshotAnimatorComponent;
//            public Animator WeaponAnimatorComponent;
//            public string WeaponAimedAnimationName = "AKInitialPose";
//            public string IdleAnimationName = "Idle";
//            public string WieldAnimationName = "wield";
//            public string FireAnimationName = "Fire";
//            public string ReloadAnimationName = "Reload";
//            public string ReloadEmptyAnimationName = "ReloadEmpty";
//            public string WalkingAnimationName = "Walk";
//            public string RunAnimationName = "Run";
//            public string RemoveAnimationName = "Remove";
//            public string IdleAnimationParametreName = "idle";
//            public string WalkAnimationParametreName = "Walk";
//            public string RunAnimationParametreName = "Run";
//        }

//        public Animations AnimationsNames;

//        //public bool ProjectileShooting = true;
//        //[Tooltip("If Disable Projectiles will be used To give damage To Enemies")]
//        //public bool RaycastShooting = true;

//        [System.Serializable]
//        public class Positions
//        {
//            [Tooltip("Parent With Animator Component Attached to it")]
//            public GameObject ShootingPointParent;
//            public SniperScope SniperScopeScript;
//            public Camera WeaponCamera;
//            // public float WeaponZAxisOnAim = -0.71f;
//            public float WeaponZoomSpeed = 6f;
//            public bool UseBlurEffectOnAim = true;
//            public GameObject BlurEffect;
//            public Vector3 AimedPosition;
//            public Vector3 AimedRotation;
//            public Vector3 HipFirePosition;
//            public Vector3 HipFireRotation;

//            public BobDurValues WeaponDurations;

//            public float DefaultPlayerCamFov = 60f;
//            //    public float DefaultWeaponCamZAxisPos = 0f;
//            public float DefaultWeaponCamFov = 60f;
//            public float WeaponAimingSpeed = 7f;
//            public float FovChangeSpeed = 7f;
//            public float MagnificationFov = 15f;
//            public float WeaponCameraAimedFov = 10f;
//            //   public Transform ObjectToShakeOnAiming;
//            // public float BobbingSpeed = 1f;
//            //  public float BobbingDistance = 1f;
//        }

//        public Positions WeaponPosition;

//        //public float MinimumX = -0.3f;
//        //public float MaximumX = 0.3f;
//        //public float MinimumY = -0.3f;
//        //public float MaximumY = 0.3f;
//        //public float MinimumZ = -0.3f;
//        //public float MaximumZ = 0.3f;
//        //public float DampingSpeed = 1f;

//        //public float GunBobXSpeed;
//        //public float GunBobYSpeed;
//        //public float MinXPosition = -0.19f;
//        //public float MaxXPosition = 0.02f;
//        //public float MinYPosition = -0.19f;
//        //public float MaxYPosition = 0.02f;
//        //bool OverwriteBobMovement = false;
//        //bool OverwriteBobMovementAtY = false;

//        [System.Serializable]
//        public class BobDurValues
//        {
//            public float StationaryAimedXShiftDuration;
//            public float StationaryAimedYShiftDuration;
//            public float StationaryAimedZShiftDuration;
//            public float StationaryAimedRotDuration;


//            public float StationaryHipFireXShiftDuration;
//            public float StationaryHipFireYShiftDuration;
//            public float StationaryHipFireZShiftDuration;
//            public float StationaryHipFireRotDuration;
//        }

//        [System.Serializable]
//        public class AimingAnimationValues
//        {
//            public bool PlayCustomAnimationClip = false;
//            public string CustomAnimationClipName = "New State";
//            public Animator AlternativeAimedAnimator;
//            public string AimedAnimationName = "Aimed";
//            public string AimedAnimationParametreName = "PlayAimed";
//            public string SpeedParameterName = "AimedSpeed";
//            public float StationaryAimedAnimationSpeed = 1f;
//            public float StationaryAimedAnimationDelay = 1f;
//            public float StandAimedWalkAnimationSpeed = 1f;
//            public float CrouchAimedWalkAnimationSpeed = 1f;
//        }

//        public AimingAnimationValues KeyframeAimingAnimationValues;
//        // [Header("Procedural Aiming Animation Values")]

//        // public float SlowDownFactor;
//        //public float MinimumXPos;
//        //public float MaximumXPos;
//        //public float MinimumYPos;
//        //public float MaximumYPos;
//        //public float MinimumZPos;
//        //public float MaximumZPos;

//        //public float MinimumXRot;
//        //public float MaximumXRot;
//        //public float MinimumYRot;
//        //public float MaximumYRot;
//        //public float MinimumZRot;
//        //public float MaximumZRot;

//        [HideInInspector]
//        public Vector3 MinimalNegativePositionalValues;
//        [HideInInspector]
//        public Vector3 MaximumNegativePositionalValues;
//        [HideInInspector]
//        public Vector3 MinimalPositivePositionalValues;
//        [HideInInspector]
//        public Vector3 MaximumPositivePositionalValues;

//        [HideInInspector]
//        public Vector3 MinimalNegativeRotationalValues;
//        [HideInInspector]
//        public Vector3 MaximumNegativeRotationalValues;
//        [HideInInspector]
//        public Vector3 MinimalPositiveRotationalValues;
//        [HideInInspector]
//        public Vector3 MaximumPositiveRotationalValues;

//        [HideInInspector]
//        public float MidXNegativeAxis;
//        [HideInInspector]
//        public float MidXPositiveAxis;
//        [HideInInspector]
//        public float MidYNegativeAxis;
//        [HideInInspector]
//        public float MidYPositiveAxis;
//        [HideInInspector]
//        public float MidZNegativeAxis;
//        [HideInInspector]
//        public float MidZPositiveAxis;

//        [HideInInspector]
//        public float MidXrotNegativeAxis;
//        [HideInInspector]
//        public float MidXrotPositiveAxis;
//        [HideInInspector]
//        public float MidYrotNegativeAxis;
//        [HideInInspector]
//        public float MidYrotPositiveAxis;
//        [HideInInspector]
//        public float MidZrotNegativeAxis;
//        [HideInInspector]
//        public float MidZrotPositiveAxis;


//        [HideInInspector]
//        public float MaxPosXNegativeAxis;
//        [HideInInspector]
//        public float MaxPosXPositiveAxis;
//        [HideInInspector]
//        public float MaxPosYNegativeAxis;
//        [HideInInspector]
//        public float MaxPosYPositveAxis;
//        [HideInInspector]
//        public float MaxPosZNegativeAxis;
//        [HideInInspector]
//        public float MaxPosZPositveAxis;
//        [HideInInspector]
//        public float MaxRotXNegativeAxis;
//        [HideInInspector]
//        public float MaxRotXPositiveAxis;
//        [HideInInspector]
//        public float MaxRotYNegativeAxis;
//        [HideInInspector]
//        public float MaxRotYPositiveAxis;
//        [HideInInspector]
//        public float MaxRotZNegativeAxis;
//        [HideInInspector]
//        public float MaxRotZPositiveAxis;

//        [HideInInspector]
//        public float PreviousX;
//        [HideInInspector]
//        public float PreviousY;
//        [HideInInspector]
//        public float PreviousZ;

//        [HideInInspector]
//        public float PreviousXRot;
//        [HideInInspector]
//        public float PreviousYRot;
//        [HideInInspector]
//        public float PreviousZRot;

//        [HideInInspector]
//        public bool LoopX;
//        [HideInInspector]
//        public bool LoopY;
//        [HideInInspector]
//        public bool LoopZ;

//        [HideInInspector]
//        public bool LoopXRot;
//        [HideInInspector]
//        public bool LoopYRot;
//        [HideInInspector]
//        public bool LoopZRot;

//        [HideInInspector]
//        public float WieldTime;
//        [HideInInspector]
//        public bool IsplayingAimedAnim = false;

//        float StoreWeaponResetingSpeed;
//        bool LerpBack = false;
//        bool LerpNow = false;

//        bool isReloadCompleted = false;

//        private CrossHair Ch;

//        [System.Serializable]
//        public class BulletSreadOptions
//        {
//            public bool UseBulletSpread = true;
//            public float MinBulletSpreadRotationX = -3f;
//            public float MaxBulletSpreadRotationX = 3f;
//            public float MinBulletSpreadRotationY = -88f;
//            public float MaxBulletSpreadRotationY = -92f;
//        }



//        [HideInInspector]
//        public bool AimedAnimState = true;

//        public RaycastHit StoreHit;
//        bool ShouldSpawnImpactEffect = true;

//        bool CanRaycastPass = false;

//        bool IsWeaponAimed = false;
//        bool IsWeaponHipFire = false;


//        [System.Serializable]
//        public class AnimationSpeedClass
//        {
//            public string RunSpeedParametreName = "RunSpeed";
//            public string WalkSpeedParametreName = "WalkSpeed";
//            public float StandRunAnimationSpeed = 1f;
//            public float CrouchRunAnimationSpeed = 0.5f;
//            public float StandWalkAnimationSpeed = 1f;
//            public float CrouchWalkAnimationSpeed = 0.5f;
//        }

//        public AnimationSpeedClass AnimationSpeeds;

//        AudioSource aud;

//        string TempWeaponAimedAnimationName = "AKInitialPose";
//        string TempIdleAnimationName = "Idle";
//        string TempWieldAnimationName = "wield";
//        string TempFireAnimationName = "Fire";
//        string TempReloadAnimationName = "Reload";
//        string TempReloadEmptyAnimationName = "ReloadEmpty";
//        string TempWalkingAnimationName = "Walk";
//        string TempRunAnimationname = "Run";
//        string TempRemoveAnimationName = "Remove";
//        string TempIdleAnimationParametreName = "idle";


//        bool StartMuzzleMeshTimer = false;
//        //public Vector3 ShootingPointParentDefaultPos;

//        bool IsProjectileShooting = false;

//        [HideInInspector]
//        public bool IsShootingNotAllowed = false;

//        public void ResettingDescription()
//        {
//            ScriptInfo = "This Script Controls The Whole Weapon Shooting Behaviour";
//        }
//        private void Awake()
//        {
//            if (instance == null)
//            {
//                instance = this;
//            }
//        }
//        private void Start()
//        {
//            //if(WeaponPosition.ShootingPointParent != null)
//            //{
//            //    ShootingPointParentDefaultPos = WeaponPosition.ShootingPointParent.transform.localPosition;
//            //}
//            if (WeaponOptions.WeaponCrosshair != null)
//            {
//                if (WeaponOptions.WeaponCrosshair != null)
//                {
//                    if (WeaponOptions.WeaponCrosshair.GetComponent<CrossHair>() != null)
//                    {
//                        Ch = WeaponOptions.WeaponCrosshair.GetComponent<CrossHair>();
//                    }
//                }
//            }

//            if (KeyframeAimingAnimationValues.PlayCustomAnimationClip == false)
//            {
//                if (KeyframeAimingAnimationValues.AlternativeAimedAnimator != null)
//                {
//                    if (KeyframeAimingAnimationValues.AlternativeAimedAnimator.enabled == true)
//                    {
//                        KeyframeAimingAnimationValues.AlternativeAimedAnimator.enabled = false;
//                    }
//                }
//            }

//            if (ShootingTypes.ShootingOption == ShootingOptions.ProjectileShootingWithoutRaycast)
//            {
//                IsProjectileShooting = true;
//            }
//            //if(PlayerManager.instance != null)
//            //{
//            //    PlayerManager.instance.Aiming();
//            //}




//            //if(ItemUsage.Instance != null)
//            //{
//            //    if (ItemUsage.Instance.UpgradableItem("Ammo") == 1)
//            //    {
//            //        Reload.MaximumAmmo = 128;
//            //        Reload.MaximumAmmoText.text = Reload.MaximumAmmo.ToString();
//            //    }
//            //    if (ItemUsage.Instance.UpgradableItem("Ammo") == 2)
//            //    {
//            //        Reload.MaximumAmmo = 220;
//            //        Reload.MaximumAmmoText.text = Reload.MaximumAmmo.ToString();
//            //    }
//            //    if (ItemUsage.Instance.UpgradableItem("Ammo") == 3)
//            //    {
//            //        Reload.MaximumAmmo = 300;
//            //        Reload.MaximumAmmoText.text = Reload.MaximumAmmo.ToString();
//            //    }
//            //}


//            EnemyIDScript = transform.root.gameObject.GetComponent<Targets>();
//            pooler = ObjectPooler.instance;

//            if (TeamMatch.instance != null)
//            {
//                if (TeamMatch.instance.EnableScoreSystemBetweenTeamsAsWinCondition == true)
//                {
//                    for (int i = 0; i < TeamMatch.instance.Teams.Count; i++)
//                    {
//                        if (TeamMatch.instance.Teams[i].TeamName == EnemyIDScript.MyTeamTag)
//                        {
//                            StoreTeamId = i;
//                        }
//                    }
//                }
//            }


//            if (ObjectToAnimate != null)
//            {
//                PreviousX = ObjectToAnimate.transform.localPosition.x;
//                PreviousY = ObjectToAnimate.transform.localPosition.y;
//                PreviousZ = ObjectToAnimate.transform.localPosition.z;

//                PreviousXRot = ObjectToAnimate.transform.localEulerAngles.x;
//                PreviousYRot = ObjectToAnimate.transform.localEulerAngles.y;
//                PreviousZRot = ObjectToAnimate.transform.localEulerAngles.z;
//            }

//            MaxPosXNegativeAxis = Random.Range(MinimalNegativePositionalValues.x, MaximumNegativePositionalValues.x);
//            MaxPosXPositiveAxis = Random.Range(MinimalPositivePositionalValues.x, MaximumPositivePositionalValues.x);
//            MaxPosYNegativeAxis = Random.Range(MinimalNegativePositionalValues.y, MaximumNegativePositionalValues.y);
//            MaxPosYPositveAxis = Random.Range(MinimalPositivePositionalValues.y, MaximumPositivePositionalValues.y);
//            MaxPosZNegativeAxis = Random.Range(MinimalNegativePositionalValues.z, MaximumNegativePositionalValues.z);
//            MaxPosZPositveAxis = Random.Range(MinimalPositivePositionalValues.z, MaximumPositivePositionalValues.z);


//            MaxPosXNegativeAxis = Random.Range(MinimalNegativeRotationalValues.x, MaximumNegativeRotationalValues.x);
//            MaxPosXPositiveAxis = Random.Range(MinimalPositiveRotationalValues.x, MaximumPositiveRotationalValues.x);
//            MaxPosYNegativeAxis = Random.Range(MinimalNegativeRotationalValues.y, MaximumNegativeRotationalValues.y);
//            MaxPosYPositveAxis = Random.Range(MinimalPositiveRotationalValues.y, MaximumPositiveRotationalValues.y);
//            MaxPosZNegativeAxis = Random.Range(MinimalNegativeRotationalValues.z, MaximumNegativeRotationalValues.z);
//            MaxPosZPositveAxis = Random.Range(MinimalPositiveRotationalValues.z, MaximumPositiveRotationalValues.z);

//            MidXNegativeAxis = Random.Range(MinimalNegativePositionalValues.x, MaximumNegativePositionalValues.x);
//            MidXPositiveAxis = Random.Range(MinimalPositivePositionalValues.x, MaximumPositivePositionalValues.x);
//            MidYNegativeAxis = Random.Range(MinimalNegativePositionalValues.y, MaximumNegativePositionalValues.y);
//            MidYPositiveAxis = Random.Range(MinimalPositivePositionalValues.y, MaximumPositivePositionalValues.y);
//            MidZNegativeAxis = Random.Range(MinimalNegativePositionalValues.z, MaximumNegativePositionalValues.z);
//            MidZPositiveAxis = Random.Range(MinimalPositivePositionalValues.z, MaximumPositivePositionalValues.z);


//            MidXrotNegativeAxis = Random.Range(MinimalNegativeRotationalValues.x, MaximumNegativeRotationalValues.x);
//            MidXrotPositiveAxis = Random.Range(MinimalPositiveRotationalValues.x, MaximumPositiveRotationalValues.x);
//            MidYrotNegativeAxis = Random.Range(MinimalNegativeRotationalValues.y, MaximumNegativeRotationalValues.y);
//            MidYrotPositiveAxis = Random.Range(MinimalPositiveRotationalValues.y, MaximumPositiveRotationalValues.y);
//            MidZNegativeAxis = Random.Range(MinimalNegativeRotationalValues.z, MaximumNegativeRotationalValues.z);
//            MidZPositiveAxis = Random.Range(MinimalPositiveRotationalValues.z, MaximumPositiveRotationalValues.z);

//            AllAnimationNames();
//        }
//        public void AllAnimationNames()
//        {
//            TempWeaponAimedAnimationName = AnimationsNames.WeaponAimedAnimationName;
//            TempIdleAnimationName = AnimationsNames.IdleAnimationName;
//            TempWieldAnimationName = AnimationsNames.WieldAnimationName;
//            TempFireAnimationName = AnimationsNames.FireAnimationName;
//            TempReloadAnimationName = AnimationsNames.ReloadAnimationName;
//            TempReloadEmptyAnimationName = AnimationsNames.ReloadEmptyAnimationName;
//            TempWalkingAnimationName = AnimationsNames.WalkingAnimationName;
//            TempRunAnimationname = AnimationsNames.RunAnimationName;
//            TempRemoveAnimationName = AnimationsNames.RemoveAnimationName;
//            TempIdleAnimationParametreName = AnimationsNames.IdleAnimationParametreName;


//        }
//        public void ResetAnimationNames()
//        {
//            AnimationsNames.WeaponAimedAnimationName = TempWeaponAimedAnimationName;
//            AnimationsNames.IdleAnimationName = TempIdleAnimationName;
//            AnimationsNames.WieldAnimationName = TempWieldAnimationName;
//            AnimationsNames.FireAnimationName = TempFireAnimationName;
//            AnimationsNames.ReloadAnimationName = TempReloadAnimationName;
//            AnimationsNames.ReloadEmptyAnimationName = TempReloadEmptyAnimationName;
//            AnimationsNames.WalkingAnimationName = TempWalkingAnimationName;
//            AnimationsNames.RunAnimationName = TempRunAnimationname;
//            AnimationsNames.RemoveAnimationName = TempRemoveAnimationName;
//            AnimationsNames.IdleAnimationParametreName = TempIdleAnimationParametreName;
//        }
//        public void DisableAnimations()
//        {
//            AnimationsNames.WeaponAimedAnimationName = KeyframeAimingAnimationValues.CustomAnimationClipName;
//            AnimationsNames.IdleAnimationName = KeyframeAimingAnimationValues.CustomAnimationClipName;
//            AnimationsNames.WieldAnimationName = KeyframeAimingAnimationValues.CustomAnimationClipName;
//            AnimationsNames.FireAnimationName = KeyframeAimingAnimationValues.CustomAnimationClipName;
//            AnimationsNames.ReloadAnimationName = KeyframeAimingAnimationValues.CustomAnimationClipName;
//            AnimationsNames.ReloadEmptyAnimationName = KeyframeAimingAnimationValues.CustomAnimationClipName;
//            AnimationsNames.WalkingAnimationName = KeyframeAimingAnimationValues.CustomAnimationClipName;
//            AnimationsNames.RunAnimationName = KeyframeAimingAnimationValues.CustomAnimationClipName;
//            AnimationsNames.RemoveAnimationName = KeyframeAimingAnimationValues.CustomAnimationClipName;
//            AnimationsNames.IdleAnimationParametreName = KeyframeAimingAnimationValues.CustomAnimationClipName;

//        }
//        public void StartChecks()
//        {
//            if (AnimationsNames.WeaponAnimatorComponent != null)
//            {
//                if (SwitchWeapons.instance != null)
//                {
//                    if (SwitchWeapons.instance.UsingActivationAlgo == false)
//                    {
//                        AnimationsNames.WeaponAnimatorComponent.Rebind();
//                    }
//                }
//                else
//                {
//                    AnimationsNames.WeaponAnimatorComponent.Rebind();
//                }

//            }


//            if (DoOnce == false)
//            {
//                Reload.CurrentAmmos = Reload.InitialAmmo;
//                StoreWeaponResetingSpeed = Reload.WeaponResetPositioningSpeed;
//                SaveShiftSpeed = ShiftSpeed;
//                RuntimeAnimatorController ac = AnimationsNames.WeaponAnimatorComponent.runtimeAnimatorController;    //Get Animator controller
//                                                                                                                     //  ShotLength = WeaponAnimatorComponent.runtimeAnimatorController.animationClips[0].length;
//                for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
//                {
//                    if (ac.animationClips[i].name == AnimationsNames.FireAnimationName)        //If it has the same name as your clip
//                    {
//                        IsFireAnimNameOk = true;
//                        ShotLength = ac.animationClips[i].length;
//                    }

//                    if (ac.animationClips[i].name == AnimationsNames.ReloadAnimationName)
//                    {
//                        IsReloadAnimNameOk = true;
//                        ReloadLength = ac.animationClips[i].length;
//                    }

//                    if (ac.animationClips[i].name == AnimationsNames.ReloadEmptyAnimationName)
//                    {
//                        IsReloadEmptyAnimNameOk = true;
//                        ReloadEmptyLength = ac.animationClips[i].length;
//                    }

//                    if (ac.animationClips[i].name == AnimationsNames.WieldAnimationName)
//                    {
//                        Wield = true;
//                        WieldTime = ac.animationClips[i].length;
//                    }

//                    if (ac.animationClips[i].name == AnimationsNames.RemoveAnimationName)
//                    {
//                        Remove = true;
//                        RemoveLength = ac.animationClips[i].length;
//                    }
//                }

//                //if (IsFireAnimNameOk == false)
//                //{
//                //    Debug.LogError("Your Fire animation clip name does not match with the animation layer name that you have written in the Inspector . Make sure to match that ");
//                //}

//                //if (IsReloadEmptyAnimNameOk == false)
//                //{
//                //    Debug.LogError("Your Reload empty animation clip name does not match with the animation layer name that you have written in the Inspector. Make sure to match that ");
//                //}

//                //if (IsReloadAnimNameOk == false)
//                //{
//                //    Debug.LogError("Your reload animation clip name does not match with the animation layer name that you have written in the Inspector . Make sure to match that ");
//                //}

//                //if (Wield == false)
//                //{
//                //    Debug.LogError("Your Wield animation clip name does not match with the animation layer name that you have written in the Inspector . Make sure to match that ");
//                //}

//                //if (Remove == false)
//                //{
//                //    Debug.LogError("Your Remove animation clip name does not match with the animation layer name that you have written in the Inspector . Make sure to match that ");
//                //}

//                DoOnce = true;
//            }

//            shootnow = false;
//            Reload.isreloading = false;

//            if (WeaponSounds.WieldSoundAudioSource != null)
//            {
//                aud = Instantiate(WeaponSounds.WieldSoundAudioSource, transform.position, transform.rotation);
//            }
//            if (aud != null)
//            {
//                Destroy(aud.gameObject, aud.clip.length);
//            }

//            IsAimed = false;
//            IsHipFire = true;
//            if (Reload.CurrentAmmos > 0)
//            {
//                if (PlayerManager.instance != null)
//                {
//                    for (int x = 0; x < Reload.ButtonsToDisableInteractionOnReload.Length; x++)
//                    {
//                        if (Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>() != null)
//                        {
//                            Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>().raycastTarget = true;
//                        }
//                        Reload.ButtonsToDisableInteractionOnReload[x].interactable = true;
//                    }
//                }
//            }
//            else
//            {
//                if (PlayerManager.instance != null)
//                {
//                    isReloadCompleted = false;
//                    for (int x = 0; x < Reload.ButtonsToDisableInteractionOnReload.Length; x++)
//                    {
//                        if (Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>() != null)
//                        {
//                            Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>().raycastTarget = false;
//                        }
//                        Reload.ButtonsToDisableInteractionOnReload[x].interactable = false;
//                    }
//                }
//            }




//            StartCoroutine(StartHipFireIdle());
//        }
//        private void OnEnable()
//        {
//            StartChecks();
//        }
//        IEnumerator StartHipFireIdle()
//        {
//            yield return new WaitForSeconds(WieldTime);
//            if (Reload.isreloading == false)
//            {
//                if (IsAimed == true)
//                {
//                    IsplayingAimedAnim = false;
//                    AimedBreathAnimController();
//                }
//                else
//                {
//                    if (JoyStick.Instance != null && PlayerManager.instance != null)
//                    {
//                        if (JoyStick.Instance.IsWalking == false && PlayerManager.instance.IsMoving == false)
//                        {
//                            AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.IdleAnimationParametreName, true);
//                            AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.WalkAnimationParametreName, false);
//                            AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.RunAnimationParametreName, false);
//                            AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.IdleAnimationName, -1, 0f);
//                        }
//                        else
//                        {
//                            AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.IdleAnimationName, -1, 0f);
//                        }
//                    }
//                }



//                //if(SwichWeapons.instance  != null)
//                //{
//                //    SwichWeapons.instance.ActivateButtons();
//                //    SwichWeapons.instance.CanSwipe = true;
//                //}
//            }
//        }
//        IEnumerator StopShooting()
//        {
//            yield return new WaitForSeconds(WeaponOptions.ShotAnimationDelay);
//            if (Reload.isreloading == false)
//            {
//                //AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.FireAnimationName, -1, 0f);
//            }
//            yield return new WaitForSeconds(ShotLength);
//            if (Reload.isreloading == false)
//            {
//                if (IsAimed == true)
//                {
//                    IsplayingAimedAnim = false;
//                    AimedAnimState = true;
//                    // AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.IdleAnimationName, -1, 0f);
//                    //     AimedBreathAnimController();
//                }
//                else
//                {
//                    // commented this if statement to achieve smooth firing while walking 
//                    //if (!AnimationsNames.WeaponAnimatorComponent.GetCurrentAnimatorStateInfo(0).IsName(AnimationsNames.IdleAnimationName))
//                    //{
//                    //    AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.IdleAnimationName, -1, 0f);
//                    //}


//                    //  AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.IdleAnimationName, -1, 0f);
//                }
//            }
//            //if(UseBulletSpread == true)
//            //{
//            //    transform.localEulerAngles = Vector3.zero;
//            //}

//        }
//        IEnumerator StartLerpingIdle()
//        {
//            yield return new WaitForSeconds(Reload.TimeToStartLerpingToIdle);
//            LerpNow = true;
//        }
//        IEnumerator MuzzleMeshTimer()
//        {
//            yield return new WaitForSeconds(ShootingFeatures.TimeToDeactivateMuzzleMesh);
//            ShootingFeatures.MuzzleMesh.SetActive(false);
//            StartMuzzleMeshTimer = false;
//        }
//        // Update is called once per frame
//        void Update()
//        {
//            //if(IsWalking == fa)
//            //{
//            //    if(WeaponPosition.ShootingPointParent.transform.localPosition != ShootingPointParentDefaultPos)
//            //    {
//            //        LeanTween.moveLocal(WeaponPosition.ShootingPointParent.gameObject, ShootingPointParentDefaultPos, 0.4f);
//            //    }

//            //}
//            if (Reload.MaximumAmmoText != null)
//            {
//                Reload.MaximumAmmoText.text = Reload.MaximumAmmo.ToString();
//            }

//            // ammopack2txt.text = ammopack2.ToString();
//            if (Reload.BulletsInMagazinesText != null)
//            {
//                Reload.BulletsInMagazinesText.text = Reload.CurrentAmmos + " " + "/".ToString();
//            }

//            if (Reload.MaximumAmmo <= 0)
//            {
//                //   shootnow = false;
//                if (Reload.MaximumAmmoText != null)
//                {
//                    Reload.MaximumAmmoText.text = "0";
//                }
//                //  maxfiretxt.text = "0" + "/";

//            }

//            if (Reload.CurrentAmmos < 0)
//            {
//                Reload.CurrentAmmos = 0;
//                if (Reload.BulletsInMagazinesText != null)
//                {

//                    Reload.BulletsInMagazinesText.text = "0" + " " + "/";
//                }
//            }
//            if (Reload.isreloading && LerpNow == false)
//            {
//                if (Reload.UseLerpTimer == true)
//                {
//                    if (LerpBack == false)
//                    {
//                        StartCoroutine(StartLerpingIdle());
//                        LerpBack = true;
//                    }
//                }


//                IsAimed = false;
//                IsHipFire = false;
//                //if(ForceReloadPositioning == true)
//                //{
//                Reload.ForceReloadPositioning = true;

//                Components.PlayerCamera.fieldOfView = Mathf.Lerp(Components.PlayerCamera.fieldOfView, WeaponPosition.DefaultPlayerCamFov, WeaponPosition.WeaponAimingSpeed * Time.deltaTime);

//                if (WeaponPosition.UseBlurEffectOnAim == true)
//                {
//                    if (WeaponPosition.BlurEffect != null)
//                    {
//                        WeaponPosition.BlurEffect.SetActive(false);
//                    }
//                }
//                if (WeaponPosition.WeaponCamera != null)
//                {
//                    //Vector3 temp = WeaponPosition.WeaponCamera.transform.localPosition;
//                    //temp.z = Mathf.Lerp(temp.z, WeaponPosition.DefaultWeaponCamZAxisPos, WeaponPosition.WeaponZoomSpeed * Time.deltaTime);
//                    //WeaponPosition.WeaponCamera.transform.localPosition = temp;

//                    WeaponPosition.WeaponCamera.fieldOfView = Mathf.Lerp(WeaponPosition.WeaponCamera.fieldOfView, WeaponPosition.DefaultWeaponCamFov, WeaponPosition.WeaponZoomSpeed * Time.deltaTime);
//                }
//                WeaponPosition.ShootingPointParent.transform.localPosition = Vector3.MoveTowards(WeaponPosition.ShootingPointParent.transform.localPosition, Reload.ReloadPosition, Reload.WeaponPositionLerpSpeed * Time.deltaTime);
//                WeaponPosition.ShootingPointParent.transform.localEulerAngles = Vector3.MoveTowards(WeaponPosition.ShootingPointParent.transform.localEulerAngles, Reload.ReloadRotation, Reload.WeaponRotationLerpSpeed * Time.deltaTime);

//                //}      
//                return;

//            }
//            else
//            {
//                if (Reload.ForceReloadPositioning == true)
//                {
//                    LerpBack = false;
//                    if (IsAimed == true)
//                    {
//                        Reload.WeaponResetPositioningSpeed = Reload.WeaponResetPositioningSpeed * 100f;
//                    }
//                    //  IsAimed = false;  
//                    //  IsHipFire = false;
//                    // ShootingPointParent.transform.localPosition = Vector3.Lerp(ShootingPointParent.transform.localPosition, HipFirePosition, WeaponPositionLerpSpeed * Time.deltaTime);
//                    //  ShootingPointParent.transform.localEulerAngles = Vector3.Slerp(ShootingPointParent.transform.localEulerAngles, HipFireRotation, WeaponRotationLerpSpeed * Time.deltaTime);
//                    Vector3 temp = WeaponPosition.ShootingPointParent.transform.localEulerAngles;
//                    temp.x = Mathf.LerpAngle(temp.x, WeaponPosition.HipFireRotation.x, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
//                    temp.y = Mathf.LerpAngle(temp.y, WeaponPosition.HipFireRotation.y, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
//                    temp.z = Mathf.LerpAngle(temp.z, WeaponPosition.HipFireRotation.z, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
//                    //temp.x = HipFireRotation.x;
//                    //temp.y = HipFireRotation.y;
//                    //temp.z = HipFireRotation.z;
//                    WeaponPosition.ShootingPointParent.transform.localEulerAngles = temp;// Vector3.Lerp(ShootingPointParent.transform.localEulerAngles, temp, WeaponResetPositioningSpeed * Time.deltaTime) ;

//                    Vector3 tempo = WeaponPosition.ShootingPointParent.transform.localPosition;
//                    tempo.x = WeaponPosition.HipFirePosition.x;
//                    tempo.y = WeaponPosition.HipFirePosition.y;
//                    tempo.z = WeaponPosition.HipFirePosition.z;
//                    WeaponPosition.ShootingPointParent.transform.localPosition = Vector3.Lerp(WeaponPosition.ShootingPointParent.transform.localPosition, tempo, Reload.WeaponResetPositioningSpeed * Time.deltaTime);

//                    if (WeaponPosition.ShootingPointParent.transform.localEulerAngles == WeaponPosition.HipFireRotation && WeaponPosition.ShootingPointParent.transform.localPosition == WeaponPosition.HipFirePosition)
//                    {
//                        Reload.WeaponResetPositioningSpeed = StoreWeaponResetingSpeed;
//                        LerpNow = false;
//                        Reload.ForceReloadPositioning = false;
//                        LerpBack = false;
//                    }
//                }
//            }
//            if (Reload.isreloading && LerpNow == true)
//            {
//                if (Reload.ForceReloadPositioning == true)
//                {
//                    if (IsAimed == true)
//                    {
//                        Reload.WeaponResetPositioningSpeed = Reload.WeaponResetPositioningSpeed * 100f;
//                    }
//                    //  IsAimed = false;  
//                    //  IsHipFire = false;
//                    // ShootingPointParent.transform.localPosition = Vector3.Lerp(ShootingPointParent.transform.localPosition, HipFirePosition, WeaponPositionLerpSpeed * Time.deltaTime);
//                    //  ShootingPointParent.transform.localEulerAngles = Vector3.Slerp(ShootingPointParent.transform.localEulerAngles, HipFireRotation, WeaponRotationLerpSpeed * Time.deltaTime);
//                    Vector3 temp = WeaponPosition.ShootingPointParent.transform.localEulerAngles;
//                    temp.x = Mathf.LerpAngle(temp.x, WeaponPosition.HipFireRotation.x, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
//                    temp.y = Mathf.LerpAngle(temp.y, WeaponPosition.HipFireRotation.y, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
//                    temp.z = Mathf.LerpAngle(temp.z, WeaponPosition.HipFireRotation.z, Reload.WeaponResetPositioningSpeed * Time.deltaTime);
//                    //temp.x = HipFireRotation.x;
//                    //temp.y = HipFireRotation.y;
//                    //temp.z = HipFireRotation.z;
//                    WeaponPosition.ShootingPointParent.transform.localEulerAngles = temp;// Vector3.Lerp(ShootingPointParent.transform.localEulerAngles, temp, WeaponResetPositioningSpeed * Time.deltaTime) ;

//                    Vector3 tempo = WeaponPosition.ShootingPointParent.transform.localPosition;
//                    tempo.x = WeaponPosition.HipFirePosition.x;
//                    tempo.y = WeaponPosition.HipFirePosition.y;
//                    tempo.z = WeaponPosition.HipFirePosition.z;
//                    WeaponPosition.ShootingPointParent.transform.localPosition = Vector3.Lerp(WeaponPosition.ShootingPointParent.transform.localPosition, tempo, Reload.WeaponResetPositioningSpeed * Time.deltaTime);

//                    if (WeaponPosition.ShootingPointParent.transform.localEulerAngles == WeaponPosition.HipFireRotation && WeaponPosition.ShootingPointParent.transform.localPosition == WeaponPosition.HipFirePosition)
//                    {
//                        Reload.WeaponResetPositioningSpeed = StoreWeaponResetingSpeed;
//                        LerpNow = false;
//                        Reload.ForceReloadPositioning = false;
//                        LerpBack = false;
//                    }
//                }
//            }


//            if (Reload.CurrentAmmos <= 0 && Reload.MaximumAmmo > 0)
//            {
//                if (isReloadCompleted == false)
//                {
//                    StartCoroutine(ReloadCoroutine(ReloadEmptyLength));
//                    isReloadCompleted = true;
//                }
//            }

//            if (Reload.Reloadation == true)
//            {
//                StartCoroutine(ReloadCoroutine(ReloadLength));
//                Reload.Reloadation = false;
//                return;
//            }


//            if (Reload.isreloading)
//            {
//                shootnow = false;
//            }
//            if (Ch != null)
//            {
//                if (WeaponOptions.ContinuousFire == true)
//                {
//                    Ch.UpdateCrossHair(shootnow);
//                }
//                else
//                {
//                    Ch.UpdateCrossHair(ShotMade);
//                }
//            }

//            if (CanRaycastPass == true && IsShootingNotAllowed == false)
//            {
//                Shoot();
//                CanRaycastPass = false;
//            }


//            if (shootnow == true && Time.time >= Nexttimetofire && Reload.CurrentAmmos > 0 && IsShootingNotAllowed == false)
//            {
//                Nexttimetofire = Time.time + 1f / ShootingFeatures.WeaponFireRate;
//                Shoot();
//                if (ShootingFeatures.UseMeshMuzzleFlash == true)
//                {
//                    if (StartMuzzleMeshTimer == false)
//                    {
//                        ShootingFeatures.MuzzleMesh.SetActive(true);
//                        StartCoroutine(MuzzleMeshTimer());
//                        StartMuzzleMeshTimer = true;
//                    }
//                }
//            }
//            else if (WeaponOptions.ContinuousFire == false && ShotMade == true)
//            {
//                Timer += Time.deltaTime;
//                if (Timer > 0.05f)
//                {
//                    if (ShootingFeatures.UseMeshMuzzleFlash == true)
//                    {
//                        ShootingFeatures.MuzzleMesh.SetActive(false);
//                    }
//                    ShotMade = false;
//                    Timer = 0;
//                }
//            }
//            //else
//            //{
//            //    if (ShootingFeatures.UseMeshMuzzleFlash == true && ShootingFeatures.AutoDeactivateMuzzleMesh == true)
//            //    {
//            //        ShootingFeatures.MuzzleMesh.SetActive(false);
//            //    }
//            //    //if(MuzzleMesh)
//            //}



//            if (ShotMaded == true)
//            {
//                StartCoroutine(StopShooting());
//                ShotMaded = false;
//            }

//            //if (JoyStick.Instance != null)
//            //{
//            //    if (JoyStick.Instance.ActiveForShake == true)
//            //    {
//            //        if (IsAimed == true)
//            //        {
//            //            // Vector3 origpos = WeaponCameraToShake.transform.localEulerAngles;

//            //            //if (Shakenow == false)
//            //            //{
//            //            //    x =  Random.Range(MinimumX, MaximumX);
//            //            //    y = Random.Range(MinimumY, MaximumY);
//            //            //    z = Random.Range(MinimumZ, MaximumZ);
//            //            //    Shakenow = true;
//            //            //}

//            //            //Vector3 temp = WeaponCameraToShake.transform.localEulerAngles;
//            //            //temp.x = Mathf.LerpAngle(temp.x, x, DampingSpeed * Time.deltaTime);
//            //            //temp.y = Mathf.LerpAngle(temp.y, y, DampingSpeed * Time.deltaTime);
//            //            //temp.z = Mathf.LerpAngle(temp.z, z, DampingSpeed * Time.deltaTime);
//            //            //WeaponCameraToShake.transform.localEulerAngles = temp;

//            //            //WeaponCameraToShake.transform.localEulerAngles = Vector3.Lerp(origpos, new Vector3(x, y, z), DampingSpeed * Time.deltaTime);

//            //            //  CurrentGunBobX = Mathf.Sin(1.0f) * GunBobX;
//            //            //CurrentGunBobY = Mathf.Sin(1.0f * 2f) * GunBobY ;

//            //            // WeaponCameraToShake.position = WeaponCameraToShake.transform.position + (Quaternion.Euler(0f, .5f, 0f) * new Vector3(0.5f * 1.0f, .5f * 1.0f, 0f) + Quaternion.Euler(.2f, .5f, 0f) * new Vector3(0f, 0f, 0f));



//            //            //Vector3 Temp = WeaponCameraToShake.transform.localPosition;
//            //            //Temp.x = Mathf.Clamp(Temp.x, Temp.x, GunBobXValue);
//            //            //Temp.y = Mathf.Clamp(Temp.y, Temp.y, GunBobYValue);
//            //            //  WeaponCameraToShake.transform.localPosition = Temp;

//            //            //  WeaponCameraToShake.transform.Translate(GunBobXSpeed * Time.deltaTime, GunBobYSpeed * Time.deltaTime, 0f);

//            //            //if (WeaponCameraToShake.transform.localPosition.x > GunBobXValueRight && WeaponCameraToShake.transform.localPosition.x < GunBobXValueLeft)
//            //            //{
//            //            //    WeaponCameraToShake.transform.Translate(-GunBobXSpeed * Time.deltaTime, WeaponCameraToShake.transform.localPosition.y, WeaponCameraToShake.transform.localPosition.z);
//            //            //}
//            //            //if(WeaponCameraToShake.transform.localPosition.x < GunBobXValueRight && WeaponCameraToShake.transform.localPosition.x > GunBobXValueLeft)
//            //            //{
//            //            //    WeaponCameraToShake.transform.Translate(GunBobXSpeed * Time.deltaTime, WeaponCameraToShake.transform.localPosition.y, WeaponCameraToShake.transform.localPosition.z);
//            //            //}

//            //            //if (WeaponCameraToShake.transform.localPosition.y < GunBobYValueLeft)
//            //            //{
//            //            //    WeaponCameraToShake.transform.Translate(WeaponCameraToShake.transform.localPosition.x, GunBobYSpeed * Time.deltaTime, WeaponCameraToShake.transform.localPosition.z);
//            //            //}
//            //            //else
//            //            //{

//            //            //}

//            //            //Vector3 temp =  WeaponCameraToShake.transform.localPosition;
//            //            //temp.x = Mathf.Lerp(-0.19f, 0.02f, GunBobXSpeed * Time.deltaTime);
//            //            //WeaponCameraToShake.transform.localPosition = temp;

//            //            //if(WeaponCameraToShake.transform.localPosition.x != MinXPosition && OverwriteBobMovement == false)
//            //            //{
//            //            //    WeaponCameraToShake.transform.localPosition = Vector3.MoveTowards(WeaponCameraToShake.transform.localPosition, new Vector3(MinXPosition, WeaponCameraToShake.transform.localPosition.y,
//            //            //   WeaponCameraToShake.transform.localPosition.z), GunBobXSpeed * Time.deltaTime);
//            //            //}
//            //            //else if(WeaponCameraToShake.transform.localPosition.x == MinXPosition || OverwriteBobMovement == true)
//            //            //{
//            //            //    OverwriteBobMovement = true;
//            //            //    WeaponCameraToShake.transform.localPosition = Vector3.MoveTowards(WeaponCameraToShake.transform.localPosition, new Vector3(MaxXPosition, WeaponCameraToShake.transform.localPosition.y,
//            //            // WeaponCameraToShake.transform.localPosition.z), GunBobXSpeed * Time.deltaTime);

//            //            //    if(WeaponCameraToShake.transform.localPosition.x == MaxXPosition)
//            //            //    {
//            //            //        OverwriteBobMovement = false;
//            //            //    }
//            //            //}

//            //            //if (WeaponCameraToShake.transform.localPosition.y != MaxYPosition && OverwriteBobMovementAtY == false)
//            //            //{
//            //            //    WeaponCameraToShake.transform.localPosition = Vector3.Lerp(WeaponCameraToShake.transform.localPosition, new Vector3(WeaponCameraToShake.transform.localPosition.x, MaxYPosition,
//            //            //   WeaponCameraToShake.transform.localPosition.z), GunBobYSpeed * Time.deltaTime);
//            //            //}
//            //            //else if (WeaponCameraToShake.transform.localPosition.y == MaxYPosition || OverwriteBobMovementAtY == true)
//            //            //{
//            //            //    OverwriteBobMovementAtY = true;
//            //            //    WeaponCameraToShake.transform.localPosition = Vector3.Lerp(WeaponCameraToShake.transform.localPosition, new Vector3(WeaponCameraToShake.transform.localPosition.x, MinYPosition,
//            //            //WeaponCameraToShake.transform.localPosition.z), GunBobYSpeed * Time.deltaTime);

//            //            //    if (WeaponCameraToShake.transform.localPosition.y == MinYPosition)
//            //            //    {
//            //            //        OverwriteBobMovementAtY = false;
//            //            //    }
//            //            //}


//            //            horizontal = JoyStick.Instance.Horizontal();
//            //            vertical = JoyStick.Instance.Vertical();

//            //            Vector3 localPosition = WeaponPosition.ObjectToShakeOnAiming.localPosition;

//            //            if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
//            //            {
//            //                timer = 0.0f;
//            //            }
//            //            else
//            //            {
//            //                waveSlice = Mathf.Sin(timer);
//            //                timer = timer + WeaponPosition.BobbingSpeed;
//            //                if (timer > Mathf.PI * 2)
//            //                {
//            //                    timer = timer - (Mathf.PI * 2);
//            //                }
//            //            }
//            //            if (waveSlice != 0)
//            //            {
//            //                float translateChange = waveSlice * WeaponPosition.BobbingDistance;
//            //                float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
//            //                totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
//            //                translateChange = totalAxes * translateChange;
//            //                localPosition.y = WeaponPosition.AimedPosition.y + translateChange;
//            //                localPosition.x = WeaponPosition.AimedPosition.x + translateChange;
//            //            }
//            //            else
//            //            {
//            //                localPosition.y = WeaponPosition.AimedPosition.y;
//            //                localPosition.x = WeaponPosition.AimedPosition.x;
//            //            }

//            //            WeaponPosition.ObjectToShakeOnAiming.transform.localPosition = localPosition;

//            //        }
//            //    }
//            //}

//            //if (WeaponOptions.DebugRaycast == true)
//            //{
//            //    if (WeaponOptions.HipFirePointPosition != null)
//            //    {
//            //        Debug.DrawRay(WeaponOptions.HipFirePointPosition.transform.position, WeaponOptions.HipFirePointPosition.transform.forward * 1000, Color.red, .01f);
//            //    }
//            //    else
//            //    {
//            //        Debug.DrawRay(Components.FpsCamera.transform.position, Components.FpsCamera.transform.forward * 1000, Color.red, .01f);
//            //    }
//            //    if (WeaponOptions.AimedPointPosition != null)
//            //    {
//            //        Debug.DrawRay(WeaponOptions.AimedPointPosition.transform.position, WeaponOptions.AimedPointPosition.transform.forward * 1000, Color.yellow, .01f);
//            //    }
//            //    else
//            //    {
//            //        Debug.DrawRay(Components.FpsCamera.transform.position, Components.FpsCamera.transform.forward * 1000, Color.yellow, .01f);
//            //    }
//            //}

//            //   if(running == true)
//            // {
//            //   RunningPlayer();
//            //    }

//            if (IsAimed == true)
//            {

//                Reload.WeaponResetPositioningSpeed = StoreWeaponResetingSpeed;
//                LerpNow = false;
//                Reload.ForceReloadPositioning = false;
//                LerpBack = false;

//                if (WeaponPosition.SniperScopeScript != null)
//                {
//                    if (WeaponPosition.SniperScopeScript.ZoomProperties.ControlZoomUsingMobileUI == true)
//                    {
//                        if (MouseScrollingControl.instance == null)
//                        {
//                            if (WeaponPosition.SniperScopeScript.ZoomProperties.UseAZoomSlider == true)
//                            {
//                                Components.PlayerCamera.fieldOfView = Mathf.Lerp(Components.PlayerCamera.fieldOfView, WeaponPosition.SniperScopeScript.ZoomProperties.ZoomSlider.value, WeaponPosition.WeaponAimingSpeed * Time.deltaTime);
//                            }
//                            else
//                            {
//                                Components.PlayerCamera.fieldOfView = Mathf.Lerp(Components.PlayerCamera.fieldOfView, WeaponPosition.SniperScopeScript.ScopeActivationProperties.CameraFieldOfView, WeaponPosition.WeaponAimingSpeed * Time.deltaTime);
//                            }
//                        }

//                    }

//                    if (UseProceduralAimedBreath == true)
//                    {
//                        if (!IsWalking)
//                        {
//                            ProceduralBreath();
//                        }


//                        if (CombineProceduralBreathWithAnimation == true)
//                        {
//                            AimedBreathAnimController();
//                        }
//                    }
//                    else
//                    {
//                        AimedBreathAnimController();
//                    }
//                }
//                else
//                {
//                    Components.PlayerCamera.fieldOfView = Mathf.Lerp(Components.PlayerCamera.fieldOfView, WeaponPosition.MagnificationFov, WeaponPosition.WeaponAimingSpeed * Time.deltaTime);
//                    if (UseProceduralAimedBreath == true)
//                    {
//                        if (!IsWalking)
//                        {
//                            ProceduralBreath();
//                        }

//                        if (CombineProceduralBreathWithAnimation == true)
//                        {
//                            AimedBreathAnimController();
//                        }
//                    }
//                    else
//                    {
//                        AimedBreathAnimController();
//                    }

//                    //Vector3 Temp = ObjectToMoveOnAimedWhileBreathing.transform.localPosition;
//                    //Vector3 Rot = ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles;
//                    //if (Temp.x < MaximumX - 0.001f && LoopX == false)
//                    //{
//                    //    Temp.x = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.x, MaximumX, BreathSpeed * Time.deltaTime);
//                    //    Rot.x = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.x, MaximumXRot, BreathRotSpeed * Time.deltaTime);
//                    //    if (Temp.x >= MaximumX - 0.001f)
//                    //    {
//                    //        LoopX = true;
//                    //        Temp.x = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.x, PreviousX, BreathSpeed * Time.deltaTime);
//                    //        Rot.x = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.x, PreviousXRot, BreathRotSpeed * Time.deltaTime);
//                    //    }
//                    //}
//                    //else
//                    //{
//                    //    if (LoopX == true)
//                    //    {
//                    //        Temp.x = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.x, PreviousX, BreathSpeed * Time.deltaTime);
//                    //        Rot.x = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.x, PreviousXRot, BreathRotSpeed * Time.deltaTime);
//                    //        if (Temp.x < PreviousX + 0.001f)
//                    //        {
//                    //            LoopX = false;
//                    //            Temp.x = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.x, MaximumX, BreathSpeed * Time.deltaTime);
//                    //            Rot.x = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.x, MaximumXRot, BreathRotSpeed * Time.deltaTime);
//                    //        }
//                    //    }
//                    //}


//                    //if (Temp.y < MaximumY - 0.001f && LoopY == false)
//                    //{
//                    //    Temp.y = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.y, MaximumY, BreathSpeed * Time.deltaTime);
//                    //    Rot.y = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.y, MaximumYRot, BreathRotSpeed * Time.deltaTime);
//                    //    if (Temp.y >= MaximumY - 0.001f)
//                    //    {
//                    //        LoopY = true;
//                    //        Temp.y = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.y, PreviousY, BreathSpeed * Time.deltaTime);
//                    //        Rot.y = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.y, PreviousYRot, BreathRotSpeed * Time.deltaTime);
//                    //    }
//                    //}
//                    //else
//                    //{
//                    //    if (LoopY == true)
//                    //    {
//                    //        Temp.y = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.y, PreviousY, BreathSpeed * Time.deltaTime);
//                    //        Rot.y = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.y, PreviousYRot, BreathRotSpeed * Time.deltaTime);
//                    //        if (Temp.y < PreviousY + 0.001f)
//                    //        {
//                    //            LoopY = false;
//                    //            Temp.y = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.y, MaximumY, BreathSpeed * Time.deltaTime);
//                    //            Rot.y = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.y, MaximumYRot, BreathRotSpeed * Time.deltaTime);
//                    //        }
//                    //    }
//                    //}

//                    //if (Temp.z < MaximumZ - 0.001f && LoopZ == false)
//                    //{
//                    //    Temp.z = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.z, MaximumZ, BreathSpeed * Time.deltaTime);
//                    //    Rot.z = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.z, MaximumZRot, BreathRotSpeed * Time.deltaTime);
//                    //    if (Temp.z >= MaximumZ - 0.001f)
//                    //    {
//                    //        LoopZ = true;
//                    //        Temp.z = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.z, PreviousZ, BreathSpeed * Time.deltaTime);
//                    //        Rot.z = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.z, PreviousZRot, BreathRotSpeed * Time.deltaTime);
//                    //    }
//                    //}
//                    //else
//                    //{
//                    //    if (LoopZ == true)
//                    //    {
//                    //        Temp.z = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.z, PreviousZ, BreathSpeed * Time.deltaTime);
//                    //        Rot.z = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.z, PreviousZRot, BreathRotSpeed * Time.deltaTime);
//                    //        if (Temp.z < PreviousZ + 0.001f)
//                    //        {
//                    //            LoopZ = false;
//                    //            Temp.z = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.z, MaximumZ, BreathSpeed * Time.deltaTime);
//                    //            Rot.z = Mathf.LerpAngle(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.z, MaximumZRot, BreathRotSpeed * Time.deltaTime);
//                    //        }
//                    //    }
//                    //}

//                    //ObjectToMoveOnAimedWhileBreathing.transform.localPosition = Temp;
//                    //ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles = Rot;

//                }


//                if (WeaponPosition.UseBlurEffectOnAim == true)
//                {
//                    if (WeaponPosition.BlurEffect != null)
//                    {
//                        WeaponPosition.BlurEffect.SetActive(true);
//                    }
//                }


//                //  WeaponPosition.ShootingPointParent.transform.localPosition = Vector3.Slerp(WeaponPosition.ShootingPointParent.transform.localPosition, WeaponPosition.AimedPosition, WeaponPosition.WeaponAimingSpeed * Time.deltaTime);

//                //ShootingPointParent.transform.localEulerAngles = Vector3.Slerp(ShootingPointParent.transform.localEulerAngles, AimedRotation, WeaponAimingSpeed * Time.deltaTime);


//                if (IsWeaponAimed == false)
//                {
//                    LeanTween.cancel(WeaponPosition.ShootingPointParent.gameObject);
//                    LeanTween.moveLocalX(WeaponPosition.ShootingPointParent, WeaponPosition.AimedPosition.x, WeaponPosition.WeaponDurations.StationaryAimedXShiftDuration);
//                    LeanTween.moveLocalY(WeaponPosition.ShootingPointParent, WeaponPosition.AimedPosition.y, WeaponPosition.WeaponDurations.StationaryAimedYShiftDuration);
//                    LeanTween.moveLocalZ(WeaponPosition.ShootingPointParent, WeaponPosition.AimedPosition.z, WeaponPosition.WeaponDurations.StationaryAimedZShiftDuration);

//                    LeanTween.rotateLocal(WeaponPosition.ShootingPointParent, WeaponPosition.AimedRotation, WeaponPosition.WeaponDurations.StationaryAimedRotDuration);

//                    IsWeaponAimed = true;
//                    IsWeaponHipFire = false;

//                }
//                //Vector3 tempo = WeaponPosition.ShootingPointParent.transform.localEulerAngles;
//                //tempo.x = Mathf.LerpAngle(tempo.x, WeaponPosition.AimedRotation.x, WeaponPosition.WeaponAimingSpeed * Time.time);
//                //tempo.y = Mathf.LerpAngle(tempo.y, WeaponPosition.AimedRotation.y, WeaponPosition.WeaponAimingSpeed * Time.time);
//                //tempo.z = Mathf.LerpAngle(tempo.z, WeaponPosition.AimedRotation.z, WeaponPosition.WeaponAimingSpeed * Time.time);
//                //WeaponPosition.ShootingPointParent.transform.localEulerAngles = tempo;


//                if (WeaponPosition.WeaponCamera != null)
//                {

//                    //WeaponCamera.orthographic = true;

//                    //Vector3 temp = WeaponPosition.WeaponCamera.transform.localPosition;
//                    //temp.z = Mathf.Lerp(temp.z, WeaponPosition.WeaponZAxisOnAim, WeaponPosition.WeaponZoomSpeed * Time.deltaTime);
//                    //WeaponPosition.WeaponCamera.transform.localPosition = temp;


//                    WeaponPosition.WeaponCamera.fieldOfView = Mathf.Lerp(WeaponPosition.WeaponCamera.fieldOfView, WeaponPosition.WeaponCameraAimedFov, WeaponPosition.WeaponZoomSpeed * Time.deltaTime);
//                    //WeaponCamera.orthographicSize = ScopeSize;


//                    //  WeaponCamera.projectionMatrix. = WeaponCamera.orthographic;
//                    //.fieldOfView = Mathf.Lerp(ScopeCamera.fieldOfView, AimedFov, WeaponAimingSpeed * Time.deltaTime);
//                }
//                //if (ShootingPointParent.transform.localPosition.x <= AimedPosition.x || ShootingPointParent.transform.localPosition.y <= AimedPosition.y || ShootingPointParent.transform.localPosition.z <= AimedPosition.z)
//                //{
//                //    CanAim = false;
//                //}

//            }

//            if (IsHipFire == true)
//            {
//                AimedAnimState = true;
//                if (Components.PlayerCamera != null)
//                {

//                    Components.PlayerCamera.fieldOfView = Mathf.Lerp(Components.PlayerCamera.fieldOfView, WeaponPosition.DefaultPlayerCamFov, WeaponPosition.WeaponAimingSpeed * Time.deltaTime);
//                }

//                //if (ScopeCamera != null)
//                //{
//                //    ScopeCamera.fieldOfView = Mathf.Lerp(ScopeCamera.fieldOfView, FOV, WeaponAimingSpeed * Time.deltaTime);
//                //}
//                if (WeaponPosition.UseBlurEffectOnAim == true)
//                {
//                    if (WeaponPosition.BlurEffect != null)
//                    {
//                        WeaponPosition.BlurEffect.SetActive(false);
//                    }
//                }
//                else
//                {
//                    if (WeaponPosition.BlurEffect != null)
//                    {
//                        WeaponPosition.BlurEffect.SetActive(false);
//                    }
//                }
//                if (WeaponPosition.ShootingPointParent != null)
//                {
//                    // WeaponPosition.ShootingPointParent.transform.localPosition = Vector3.Slerp(WeaponPosition.ShootingPointParent.transform.localPosition, WeaponPosition.HipFirePosition, WeaponPosition.WeaponAimingSpeed * Time.deltaTime);
//                    //      ShootingPointParent.transform.localEulerAngles = Vector3.Slerp(ShootingPointParent.transform.localEulerAngles, HipFireRotation, WeaponAimingSpeed * Time.deltaTime);
//                    //Vector3 tempo = WeaponPosition.ShootingPointParent.transform.localEulerAngles;
//                    //tempo.x = Mathf.LerpAngle(tempo.x, WeaponPosition.HipFireRotation.x, WeaponPosition.WeaponAimingSpeed * Time.time);
//                    //tempo.y = Mathf.LerpAngle(tempo.y, WeaponPosition.HipFireRotation.y, WeaponPosition.WeaponAimingSpeed * Time.time);
//                    //tempo.z = Mathf.LerpAngle(tempo.z, WeaponPosition.HipFireRotation.z, WeaponPosition.WeaponAimingSpeed * Time.time);
//                    //WeaponPosition.ShootingPointParent.transform.localEulerAngles = tempo;

//                    if (IsWeaponHipFire == false)
//                    {
//                        LeanTween.cancel(WeaponPosition.ShootingPointParent.gameObject);
//                        LeanTween.moveLocalX(WeaponPosition.ShootingPointParent, WeaponPosition.HipFirePosition.x, WeaponPosition.WeaponDurations.StationaryHipFireXShiftDuration);
//                        LeanTween.moveLocalY(WeaponPosition.ShootingPointParent, WeaponPosition.HipFirePosition.y, WeaponPosition.WeaponDurations.StationaryHipFireYShiftDuration);
//                        LeanTween.moveLocalZ(WeaponPosition.ShootingPointParent, WeaponPosition.HipFirePosition.z, WeaponPosition.WeaponDurations.StationaryHipFireZShiftDuration);

//                        LeanTween.rotateLocal(WeaponPosition.ShootingPointParent, WeaponPosition.HipFireRotation, WeaponPosition.WeaponDurations.StationaryHipFireRotDuration);

//                        IsWeaponHipFire = true;
//                        IsWeaponAimed = false;

//                    }
//                }


//                if (WeaponPosition.WeaponCamera != null)
//                {
//                    //Vector3 temp = WeaponPosition.WeaponCamera.transform.localPosition;
//                    //temp.z = Mathf.Lerp(temp.z, WeaponPosition.DefaultWeaponCamZAxisPos, WeaponPosition.WeaponZoomSpeed * Time.deltaTime);
//                    //WeaponPosition.WeaponCamera.transform.localPosition = temp;

//                    WeaponPosition.WeaponCamera.fieldOfView = Mathf.Lerp(WeaponPosition.WeaponCamera.fieldOfView, WeaponPosition.DefaultWeaponCamFov, WeaponPosition.WeaponZoomSpeed * Time.deltaTime);
//                    //WeaponCamera.orthographicSize = DefaultScopeSize;
//                    //WeaponCamera.orthographic = false;
//                    //WeaponCamera.orthographicSize = ScopeSize;
//                    //  WeaponCamera.projectionMatrix. = WeaponCamera.orthographic;
//                    //.fieldOfView = Mathf.Lerp(ScopeCamera.fieldOfView, AimedFov, WeaponAimingSpeed * Time.deltaTime);
//                }


//                if (WeaponPosition.SniperScopeScript != null)
//                {
//                    ObjectToAnimate.transform.localPosition = Vector3.MoveTowards(ObjectToAnimate.transform.localPosition, new Vector3(PreviousX, PreviousY, PreviousZ), ShiftSpeed * Time.deltaTime);
//                    ObjectToAnimate.transform.localEulerAngles = Vector3.MoveTowards(ObjectToAnimate.transform.localEulerAngles, new Vector3(PreviousXRot, PreviousYRot, PreviousZRot), RotationSpeed * Time.deltaTime);
//                }
//                else
//                {
//                    if (UseProceduralAimedBreath == true)
//                    {
//                        ObjectToAnimate.transform.localPosition = Vector3.MoveTowards(ObjectToAnimate.transform.localPosition, new Vector3(PreviousX, PreviousY, PreviousZ), Time.deltaTime * ShiftSpeed);
//                        ObjectToAnimate.transform.localEulerAngles = Vector3.MoveTowards(ObjectToAnimate.transform.localPosition, new Vector3(PreviousX, PreviousY, PreviousZ), Time.deltaTime * RotationSpeed);
//                    }
//                }

//                ResetHipFireState();
//                //if (ShootingPointParent.transform.localPosition == HipFirePosition)
//                //{
//                //    IsHipFire = false;
//                //}
//            }

//            //  if(WeaponAnimatorComponent.nor)
//        }
//        public void BulletsFunctionality(Vector3 pos, Quaternion rot)
//        {
//            GameObject bullet = pooler.SpawnFromPool(ShootingTypes.ProjectileName, pos, rot);
//            // Debug.Break();
//            BulletScript B = bullet.GetComponent<BulletScript>();

//            B.Movement(transform, transform.root, IsProjectileShooting);
//            //if (B.UseBulletSpread == true)
//            //{
//            //    B.CanSpread = false;
//            //}
//        }
//        public void RotateZOnShot()
//        {
//            Vector3 ZRot = transform.localEulerAngles;
//            ZRot.z = Random.Range(ShootingFeatures.MinZRotationOnTrigger, ShootingFeatures.MaxZRotationOnTrigger);
//            transform.localEulerAngles = ZRot;
//        }
//        public void Shoot()
//        {
//            //if (WeaponPosition.SniperScopeScript != null)
//            //{
//            //    if(WeaponPosition.SniperScopeScript.EnableBulletTimeCamera == true)
//            //    {
//            //        WeaponPosition.SniperScopeScript.BulletTimeCamera.gameObject.SetActive(true);
//            //        //WeaponPosition.SniperScopeScript.BulletTimeCamera.transform.parent = 
//            //    }
//            //}
//            if (Components.CreateNoiseScript != null)
//            {
//                Components.CreateNoiseScript.ActivateNoiseHandler(transform);
//            }


//            if (ShootingFeatures.UseMeshMuzzleFlash == true)
//            {
//                ShootingFeatures.MuzzleMesh.SetActive(true);
//            }
//            else
//            {
//                if (ShootingFeatures.MuzzleFlash != null)
//                {
//                    if (ShootingFeatures.MuzzleFlash.isPlaying == false)
//                        ShootingFeatures.MuzzleFlash.Play();
//                }
//            }
//            AudioSource aud = Instantiate(WeaponSounds.FireSoundAudioSource, transform.position, transform.rotation);
//            Destroy(aud.gameObject, aud.clip.length);
//            playershoot = true;
//            if (ShootingFeatures.EnableSilencer == false)
//            {
//                IsFire = true;
//            }

//            Reload.CurrentAmmos--;
//            ShotMaded = true;
//            RotateZOnShot();
//            //Debug.Log("Weapon Shots");

//            //WeaponAnimatorComponent.Play(FireAnimationName, -1, 0f);

//            //if(ChangeAnimCounter == 0)
//            //{
//            //    ChangeAnimCounter = 1f;
//            //}
//            //else
//            //{
//            //    ChangeAnimCounter = 0f;
//            //}


//            if (IsWalking == true)
//            {
//                AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.IdleAnimationParametreName, false);
//                // AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.WalkingAnimationName, -1, 0f);
//                AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.WalkAnimationParametreName, true);
//                AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.RunAnimationParametreName, false);

//                if (JoyStick.Instance != null)
//                {
//                    for (int o = 0; o < JoyStick.Instance.WalkingSounds.Length; o++)
//                    {
//                        if (!JoyStick.Instance.WalkingSounds[o].isPlaying)
//                        {
//                            JoyStick.Instance.WalkingSounds[o].Play();
//                        }
//                    }
//                }


//            }
//            //  else
//            //{
//            //if(IsAimed == true)
//            //{
//            //    IsplayingAimedAnim = false;
//            //    AimedBreathAnimController();
//            //}
//            //else
//            //{
//            //    WeaponAnimatorComponent.SetBool(IdleAnimationParametreName, true);
//            //    WeaponAnimatorComponent.Play(IdleAnimationName);

//            //    if (JoyStick.Instance != null)
//            //    {
//            //        if (JoyStick.Instance.WalkingSounds.isPlaying)
//            //        {
//            //            JoyStick.Instance.WalkingSounds.Stop();
//            //        }
//            //    }
//            //}

//            //}

//            if (BulletSpread.UseBulletSpread == true)
//            {
//                Vector3 Spread = transform.localEulerAngles;
//                Spread.x = Random.Range(BulletSpread.MinBulletSpreadRotationX, BulletSpread.MaxBulletSpreadRotationX);
//                Spread.y = Random.Range(BulletSpread.MinBulletSpreadRotationY, BulletSpread.MaxBulletSpreadRotationY);
//                transform.localEulerAngles = Spread;
//                //  transform.localRotation = Quaternion.Euler(TargetX, TargetY, 0f);
//            }

//            if (WeaponPosition.SniperScopeScript != null)
//            {
//                WeaponPosition.SniperScopeScript.ShootProperties.ShotAnimatorComponent.Play(WeaponPosition.SniperScopeScript.ShootProperties.ShotAnimationClip.name, -1, 0f);
//            }
//            if (pooler != null)
//            {
//                GameObject bulletShellrb = pooler.SpawnFromPool(BulletShell.BulletShellName, BulletShell.BulletShellSpawnPosition.transform.position, BulletShell.BulletShellSpawnPosition.transform.rotation);
//                Rigidbody r = bulletShellrb.GetComponent<Rigidbody>();
//                r.velocity = BulletShell.BulletShellSpawnPosition.transform.TransformDirection(Vector3.right * BulletShell.ShellsEjectingSpeed);

//                if (ShootingOptions.RaycastShootingWithVisualProjectiles == ShootingTypes.ShootingOption || ShootingOptions.ProjectileShootingWithoutRaycast == ShootingTypes.ShootingOption)
//                {
//                    //if(SniperScopeScript != null)
//                    // {
//                    //   if(IsAimed == false)
//                    // {

//                    if (ShootingTypes.BulletsToSpawn > 0)
//                    {
//                        for (int x = 0; x < ShootingTypes.BulletsToSpawn; x++)
//                        {
//                            //if(SniperScopeScript != null)
//                            //{
//                            //    if(IsAimed == true)
//                            //    {
//                            //    }
//                            //}
//                            if (WeaponPosition.SniperScopeScript != null)
//                            {
//                                if (IsAimed == true)
//                                {
//                                    if (WeaponPosition.SniperScopeScript.ShootProperties.ProjectileSpawnPoint != null)
//                                    {
//                                        BulletsFunctionality(WeaponPosition.SniperScopeScript.ShootProperties.ProjectileSpawnPoint.transform.position, WeaponPosition.SniperScopeScript.ShootProperties.ProjectileSpawnPoint.transform.rotation);
//                                    }
//                                    else
//                                    {
//                                        BulletsFunctionality(transform.position, transform.rotation);
//                                    }
//                                }
//                                else
//                                {
//                                    BulletsFunctionality(transform.position, transform.rotation);
//                                }
//                            }
//                            else
//                            {
//                                BulletsFunctionality(transform.position, transform.rotation);
//                            }

//                        }
//                    }
//                    //}
//                    //}
//                    //  else
//                    //{
//                    //  GameObject bullet = pooler.SpawnFromPool(ProjectileName, transform.position, transform.rotation);
//                    //    bullet.GetComponent<BulletScript>().Movement();

//                    // }


//                }
//            }

//            // PlayerWeapon.ins.Fire();


//            if (ShootingOptions.RaycastShootingWithoutProjectiles == ShootingTypes.ShootingOption || ShootingOptions.RaycastShootingWithVisualProjectiles == ShootingTypes.ShootingOption)
//            {

//                if (WeaponOptions.HipFirePointPosition == null)
//                {
//                    //  if (Physics.Raycast(FpsCamera.transform.position, FpsCamera.transform.forward, out hit, ShootingRange)) // Previously it was  transform.position and transform.forward
//                    // {
//                    if (IsAimed == false)
//                    {

//                        if (Physics.Raycast(Components.PlayerCamera.transform.position, Components.PlayerCamera.transform.forward, out hit, ShootingFeatures.ShootingRange, WeaponOptions.VisibleLayers))
//                        {
//                            Fire();
//                        }
//                    }

//                    // }
//                }
//                else
//                {
//                    //if (Physics.Raycast(HipFirePointPosition.transform.position, HipFirePointPosition.transform.forward, out hit, ShootingRange)) // Previously it was  transform.position and transform.forward
//                    //{
//                    //    Fire();
//                    //}

//                    if (IsAimed == false)
//                    {
//                        if (WeaponOptions.HipFirePointPosition != null)
//                        {
//                            if (Physics.Raycast(WeaponOptions.HipFirePointPosition.transform.position, WeaponOptions.HipFirePointPosition.transform.forward, out hit, ShootingFeatures.ShootingRange, WeaponOptions.VisibleLayers))
//                            {
//                                // Debug.Log(hit.collider.gameObject.name + " " + hit.collider.transform.gameObject.layer.ToString());
//                                Fire();
//                            }
//                        }
//                    }
//                }

//                if (WeaponOptions.AimedPointPosition == null)
//                {
//                    if (IsAimed == true)
//                    {
//                        if (Physics.Raycast(Components.PlayerCamera.transform.position, Components.PlayerCamera.transform.forward, out hit, ShootingFeatures.ShootingRange, WeaponOptions.VisibleLayers))
//                        {
//                            Fire();
//                        }
//                    }
//                }
//                else
//                {
//                    if (IsAimed == true)
//                    {
//                        if (WeaponOptions.AimedPointPosition != null)
//                        {
//                            if (Physics.Raycast(WeaponOptions.AimedPointPosition.transform.position, WeaponOptions.AimedPointPosition.transform.forward, out hit, ShootingFeatures.ShootingRange, WeaponOptions.VisibleLayers))
//                            {
//                                Fire();
//                            }
//                        }
//                    }
//                }


//            }
//            if (CameraRecoil.instance != null)
//            {
//                CameraRecoil.instance.Recoil(IsAimed);
//            }

//            if (Recoil.AuthenticRecoilScript != null && Recoil.GenericRecoilScript != null)
//            {
//                Debug.LogError("You can use only 1 Recoil Script");
//            }
//            else if (Recoil.AuthenticRecoilScript != null && Recoil.GenericRecoilScript == null)
//            {
//                Recoil.AuthenticRecoilScript.Recoil(IsAimed);
//            }
//            else if (Recoil.GenericRecoilScript != null && Recoil.AuthenticRecoilScript == null)
//            {
//                Recoil.GenericRecoilScript.Recoil(IsAimed);
//            }
//        }
//        public void ProceduralBreath()
//        {
//            Vector3 Temp = ObjectToAnimate.transform.localPosition;
//            Vector3 Rot = ObjectToAnimate.transform.localEulerAngles;
//            //if (ShiftSpeed > SlowDownFactor - 0.0001)
//            //{
//            //    ShiftSpeed -= SlowDownFactor * Time.deltaTime;
//            //}
//            //else
//            //{
//            //    ShiftSpeed = SaveShiftSpeed;
//            //}
//            if (LoopX == false)
//            {
//                //ShiftSpeed -= SlowDownFactor / Time.deltaTime;
//                Temp.x = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.x, MaxPosXNegativeAxis, ShiftSpeed * Time.deltaTime);

//                // Rot.x = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.x, MaxRotXNegativeAxis, RotationSpeed * Time.deltaTime);

//                if (Temp.x == MaxPosXNegativeAxis)
//                {
//                    // ShiftSpeed = SaveShiftSpeed;
//                    LoopX = true;
//                    MaxPosXPositiveAxis = Random.Range(MinimalPositivePositionalValues.x, MaximumPositivePositionalValues.x);
//                    //   MaxRotXPositiveAxis = Random.Range(MinimumRotPositiveAxis.x, MaximumRotPositiveAxis.x);

//                    Temp.x = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.x, MaxPosXPositiveAxis, ShiftSpeed * Time.deltaTime);
//                    // Rot.x = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.x, MaxRotXPositiveAxis, RotationSpeed * Time.deltaTime);
//                }


//            }
//            else
//            {
//                if (LoopX == true)
//                {
//                    //ShiftSpeed -= SlowDownFactor / Time.deltaTime;
//                    Temp.x = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.x, MaxPosXPositiveAxis, ShiftSpeed * Time.deltaTime);
//                    //Rot.x = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.x, MaxRotXPositiveAxis, RotationSpeed * Time.deltaTime);

//                    if (Temp.x == MaxPosXPositiveAxis)
//                    {
//                        //ShiftSpeed = SaveShiftSpeed;
//                        MaxPosXNegativeAxis = Random.Range(MinimalNegativePositionalValues.x, MaximumNegativePositionalValues.x);
//                        //  MaxRotXNegativeAxis = Random.Range(MinimumRotNegativeAxis.x, MaximumRotNegativeAxis.x);

//                        LoopX = false;
//                        Temp.x = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.x, MaxPosXNegativeAxis, ShiftSpeed * Time.deltaTime);
//                        //Rot.x = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.x, MaxRotXNegativeAxis, RotationSpeed * Time.deltaTime);
//                    }
//                }
//            }

//            if (LoopY == false)
//            {
//                Temp.y = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.y, MaxPosYNegativeAxis, ShiftSpeed * Time.deltaTime);

//                //Rot.y = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.y, MaxRotYNegativeAxis, RotationSpeed * Time.deltaTime);

//                if (Temp.y == MaxPosYNegativeAxis)
//                {
//                    // ShiftSpeed = SaveShiftSpeed;
//                    LoopY = true;
//                    MaxPosYPositveAxis = Random.Range(MinimalPositivePositionalValues.y, MaximumPositivePositionalValues.y);
//                    //  MaxRotYPositiveAxis = Random.Range(MinimumRotPositiveAxis.y, MaximumRotPositiveAxis.y);

//                    Temp.y = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.y, MaxPosYPositveAxis, ShiftSpeed * Time.deltaTime);
//                    //Rot.y = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.y, MaxRotYPositiveAxis, RotationSpeed * Time.deltaTime);
//                }


//            }
//            else
//            {
//                if (LoopY == true)
//                {
//                    // ShiftSpeed -= SlowDownFactor / Time.deltaTime;
//                    Temp.y = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.y, MaxPosYPositveAxis, ShiftSpeed * Time.deltaTime);
//                    //Rot.y = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.y, MaxRotYPositiveAxis, RotationSpeed * Time.deltaTime);

//                    if (Temp.y == MaxPosYPositveAxis)
//                    {
//                        //  ShiftSpeed = SaveShiftSpeed;
//                        MaxPosYNegativeAxis = Random.Range(MinimalNegativePositionalValues.y, MaximumNegativePositionalValues.y);
//                        //  MaxRotYNegativeAxis = Random.Range(MinimumRotNegativeAxis.y, MaximumRotNegativeAxis.y);

//                        LoopY = false;
//                        Temp.y = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.y, MaxPosYNegativeAxis, ShiftSpeed * Time.deltaTime);
//                        //Rot.y = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.y, MaxRotYNegativeAxis, RotationSpeed * Time.deltaTime);
//                    }
//                }
//            }

//            if (LoopZ == false)
//            {
//                Temp.z = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.z, MaxPosZNegativeAxis, ShiftSpeed * Time.deltaTime);
//                // Rot.z = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.z, MaxRotZNegativeAxis, RotationSpeed * Time.deltaTime);

//                if (Temp.z == MaxPosZNegativeAxis)
//                {
//                    LoopZ = true;
//                    MaxPosZPositveAxis = Random.Range(MinimalPositivePositionalValues.z, MaximumPositivePositionalValues.z);
//                    //   MaxRotZPositiveAxis = Random.Range(MinimumRotPositiveAxis.z, MaximumRotPositiveAxis.z);

//                    Temp.z = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.z, MaxPosZPositveAxis, ShiftSpeed * Time.deltaTime);
//                    // Rot.z = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.z, MaxRotZPositiveAxis, RotationSpeed * Time.deltaTime);
//                }


//            }
//            else
//            {
//                if (LoopZ == true)
//                {
//                    Temp.z = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.z, MaxPosZPositveAxis, ShiftSpeed * Time.deltaTime);
//                    //Rot.z = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.z, MaxRotZPositiveAxis, RotationSpeed * Time.deltaTime);

//                    if (Temp.z == MaxPosZPositveAxis)
//                    {
//                        MaxPosZNegativeAxis = Random.Range(MinimalNegativePositionalValues.z, MaximumNegativePositionalValues.z);
//                        //   MaxRotZNegativeAxis = Random.Range(MinimumRotNegativeAxis.z, MaximumRotNegativeAxis.z);

//                        LoopZ = false;
//                        Temp.z = Mathf.MoveTowards(ObjectToAnimate.transform.localPosition.z, MaxPosZNegativeAxis, ShiftSpeed * Time.deltaTime);
//                        // Rot.z = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.z, MaxRotZNegativeAxis, RotationSpeed * Time.deltaTime);
//                    }
//                }
//            }


//            if (LoopXRot == false)
//            {
//                Rot.x = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.x, MaxRotXNegativeAxis, RotationSpeed * Time.deltaTime);

//                if (Rot.x == MaxRotXNegativeAxis)
//                {
//                    LoopXRot = true; ;
//                    MaxRotXPositiveAxis = Random.Range(MinimalPositiveRotationalValues.x, MaximumPositiveRotationalValues.x);
//                    Rot.x = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.x, MaxRotXPositiveAxis, RotationSpeed * Time.deltaTime);
//                }
//            }
//            else
//            {
//                if (LoopXRot == true)
//                {
//                    Rot.x = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.x, MaxRotXPositiveAxis, RotationSpeed * Time.deltaTime);

//                    if (Rot.x == MaxRotXPositiveAxis)
//                    {
//                        MaxRotXNegativeAxis = Random.Range(MinimalNegativeRotationalValues.x, MaximumNegativeRotationalValues.x);
//                        LoopXRot = false;
//                        Rot.x = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.x, MaxRotXNegativeAxis, RotationSpeed * Time.deltaTime);
//                    }
//                }
//            }

//            if (LoopYRot == false)
//            {
//                Rot.y = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.y, MaxRotYNegativeAxis, RotationSpeed * Time.deltaTime);

//                if (Rot.y == MaxRotYNegativeAxis)
//                {
//                    LoopYRot = true;
//                    MaxRotYPositiveAxis = Random.Range(MinimalPositiveRotationalValues.y, MaximumPositiveRotationalValues.y);
//                    Rot.y = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.y, MaxRotYPositiveAxis, RotationSpeed * Time.deltaTime);
//                }


//            }
//            else
//            {
//                if (LoopYRot == true)
//                {
//                    Rot.y = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.y, MaxRotYPositiveAxis, RotationSpeed * Time.deltaTime);

//                    if (Rot.y == MaxRotYPositiveAxis)
//                    {
//                        MaxRotYNegativeAxis = Random.Range(MinimalNegativeRotationalValues.y, MaximumNegativeRotationalValues.y);
//                        LoopYRot = false;
//                        Rot.y = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.y, MaxRotYNegativeAxis, RotationSpeed * Time.deltaTime);
//                    }
//                }
//            }

//            if (LoopZRot == false)
//            {
//                Rot.z = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.z, MaxRotZNegativeAxis, RotationSpeed * Time.deltaTime);

//                if (Rot.z == MaxRotZNegativeAxis)
//                {
//                    LoopZRot = true;
//                    MaxRotZPositiveAxis = Random.Range(MinimalPositiveRotationalValues.z, MaximumPositiveRotationalValues.z);
//                    Rot.z = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.z, MaxRotZPositiveAxis, RotationSpeed * Time.deltaTime);
//                }
//            }
//            else
//            {
//                if (LoopZRot == true)
//                {
//                    Rot.z = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.z, MaxRotZPositiveAxis, RotationSpeed * Time.deltaTime);
//                    if (Rot.z == MaxRotZPositiveAxis)
//                    {
//                        MaxRotZNegativeAxis = Random.Range(MinimalNegativeRotationalValues.z, MaximumNegativeRotationalValues.z);
//                        LoopZRot = false;
//                        Rot.z = Mathf.MoveTowards(ObjectToAnimate.transform.localEulerAngles.z, MaxRotZNegativeAxis, RotationSpeed * Time.deltaTime);
//                    }
//                }
//            }



//            //if (LoopY == false)
//            //{
//            //    Temp.y = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.y, MaxY, BreathSpeed * Time.deltaTime);
//            //    Rot.y = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.y, MaxYRot, BreathRotSpeed * Time.deltaTime);
//            //    if (Temp.y == MaxY)
//            //    {
//            //        PreviousY = Random.Range(MinimumYPos, MaximumYPos);
//            //        PreviousYRot = Random.Range(MinimumYRot, MaximumYRot);
//            //        LoopY = true;
//            //        Temp.y = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.y, PreviousY, BreathSpeed * Time.deltaTime);
//            //        Rot.y = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.y, PreviousYRot, BreathRotSpeed * Time.deltaTime);
//            //    }
//            //}
//            //else
//            //{
//            //    if (LoopY == true)
//            //    {
//            //        Temp.y = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.y, PreviousY, BreathSpeed * Time.deltaTime);
//            //        Rot.y = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.y, PreviousYRot, BreathRotSpeed * Time.deltaTime);
//            //        if (Temp.y == PreviousY)
//            //        {
//            //            MaxY = Random.Range(MinimumYPos, MaximumYPos);
//            //            MaxYRot = Random.Range(MinimumYRot, MaximumYRot);
//            //            LoopY = false;
//            //            Temp.y = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.y, MaxY, BreathSpeed * Time.deltaTime);
//            //            Rot.y = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.y, MaxYRot, BreathRotSpeed * Time.deltaTime);
//            //        }
//            //    }
//            //}

//            //if (LoopZ == false)
//            //{
//            //    Temp.z = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.z, MaxZ, BreathSpeed * Time.deltaTime);
//            //    Rot.z = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.z, MaxZRot, BreathRotSpeed * Time.deltaTime);
//            //    if (Temp.z == MaxZ)
//            //    {
//            //        PreviousZ = Random.Range(MinimumZPos, MaximumZPos);
//            //        PreviousZRot = Random.Range(MinimumZRot, MaximumZRot);
//            //        LoopZ = true;
//            //        Temp.z = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.z, PreviousZ, BreathSpeed * Time.deltaTime);
//            //        Rot.z = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.z, PreviousZRot, BreathRotSpeed * Time.deltaTime);
//            //    }
//            //}
//            //else
//            //{
//            //    if (LoopZ == true)
//            //    {
//            //        Temp.z = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.z, PreviousZ, BreathSpeed * Time.deltaTime);
//            //        Rot.z = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.z, PreviousZRot, BreathRotSpeed * Time.deltaTime);
//            //        if (Temp.z == PreviousZ)
//            //        {
//            //            MaxZ = Random.Range(MinimumZPos, MaximumZPos);
//            //            MaxZRot = Random.Range(MinimumZRot, MaximumZRot);
//            //            LoopZ = false;
//            //            Temp.z = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localPosition.z, MaxZ, BreathSpeed * Time.deltaTime);
//            //            Rot.z = Mathf.MoveTowards(ObjectToMoveOnAimedWhileBreathing.transform.localEulerAngles.z, MaxZRot, BreathRotSpeed * Time.deltaTime);
//            //        }
//            //    }
//            //}

//            ObjectToAnimate.transform.localPosition = Temp;
//            ObjectToAnimate.transform.localEulerAngles = Rot;
//        }
//        //public IEnumerator removeheadshot()
//        //{
//        //    yield return new WaitForSeconds(1f);
//        //    if(HeadshotAnimatorComponent != null)
//        //    {
//        //        HeadshotAnimatorComponent.SetBool("headshot", false);
//        //    }

//        //}
//        IEnumerator MagazineRemoveSoundTimer()
//        {
//            yield return new WaitForSeconds(Reload.MagazineDropSoundDelay);
//            AudioSource aud = Instantiate(Reload.MagazineDropSound, transform.position, transform.rotation);
//            Destroy(aud.gameObject, aud.clip.length);
//        }
//        public IEnumerator ReloadCoroutine(float AnimTimer)
//        {
//            if (Reload.MagazineDropSound != null)
//            {
//                StartCoroutine(MagazineRemoveSoundTimer());
//            }
//            ShotMade = false;
//            Reload.WeaponResetPositioningSpeed = StoreWeaponResetingSpeed;
//            LerpNow = false;
//            Reload.ForceReloadPositioning = false;
//            LerpBack = false;
//            PlayerManager fs = FindObjectOfType<PlayerManager>();
//            if (ShootingFeatures.UseMeshMuzzleFlash == true)
//            {
//                ShootingFeatures.MuzzleMesh.SetActive(false);
//            }
//            if (fs != null)
//            {
//                if (fs.IsScoping == true)
//                {
//                    fs.Aiming();
//                }
//            }

//            if (PlayerManager.instance != null)
//            {
//                for (int x = 0; x < Reload.ButtonsToDisableInteractionOnReload.Length; x++)
//                {
//                    if (Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>() != null)
//                    {
//                        Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>().raycastTarget = false;
//                    }
//                    Reload.ButtonsToDisableInteractionOnReload[x].interactable = false;
//                }
//            }
//            Reload.isreloading = true;

//            if (Reload.CurrentAmmos > 0)
//            {
//                AudioSource aud = Instantiate(WeaponSounds.ReloadSoundAudioSource, transform.position, transform.rotation);
//                Destroy(aud.gameObject, aud.clip.length);
//                AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.ReloadAnimationName, -1, 0f);
//            }
//            else
//            {
//                AudioSource aud = Instantiate(WeaponSounds.ReloadEmptySoundAudioSource, transform.position, transform.rotation);
//                Destroy(aud.gameObject, aud.clip.length);
//                AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.ReloadEmptyAnimationName, -1, 0f);

//                MagazineCartridges SA = FindObjectOfType<MagazineCartridges>();
//                if (SA != null)
//                {
//                    SA.ActivateForReload();
//                }
//            }
//            // yield return new WaitForSeconds(1f);
//            yield return new WaitForSeconds(AnimTimer);
//            if (Reload.MaximumAmmo < Reload.FullMagazineSize && Reload.MaximumAmmo > 0)
//            {
//                //int reduceammo = amm - currentammo;
//                // ammopack = ammopack - reduceammo;
//                int maxiammo = Reload.MaximumAmmo;
//                Reload.MaximumAmmo = 0;
//                Reload.CurrentAmmos = Reload.CurrentAmmos + maxiammo;
//                Reload.BulletsInMagazinesText.text = Reload.CurrentAmmos.ToString();
//                Reload.MaximumAmmoText.text = Reload.MaximumAmmo.ToString();

//            }
//            else if (Reload.MaximumAmmo >= Reload.FullMagazineSize)
//            {
//                int reduceammo = Reload.FullMagazineSize - Reload.CurrentAmmos;
//                Reload.MaximumAmmo = Reload.MaximumAmmo - reduceammo;
//                Reload.CurrentAmmos = Reload.FullMagazineSize;
//                Reload.MaximumAmmoText.text = Reload.MaximumAmmo.ToString();
//            }
//            CheckForWalking();
//            ResetHipFireState();
//            Reload.isreloading = false;
//            isReloadCompleted = false;
//            if (PlayerManager.instance != null)
//            {
//                for (int x = 0; x < Reload.ButtonsToDisableInteractionOnReload.Length; x++)
//                {
//                    if (Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>() != null)
//                    {
//                        Reload.ButtonsToDisableInteractionOnReload[x].gameObject.GetComponent<Image>().raycastTarget = true;
//                    }
//                    Reload.ButtonsToDisableInteractionOnReload[x].interactable = true;
//                }
//            }
//            IsWeaponHipFire = false; // newly added on 22/04/21
//            IsWeaponAimed = false; // newly added on 22/04/21
//        }
//        public void ResetHipFireState()
//        {
//            if (Reload.isreloading == false)
//            {
//                if (UseProceduralAimedBreath == false || UseProceduralAimedBreath == true && CombineProceduralBreathWithAnimation == true)
//                {
//                    if (KeyframeAimingAnimationValues.PlayCustomAnimationClip == false)
//                    {
//                        if (KeyframeAimingAnimationValues.AlternativeAimedAnimator != null)
//                        {
//                            if (IsplayingAimedAnim == true)
//                            {
//                                KeyframeAimingAnimationValues.AlternativeAimedAnimator.Play(KeyframeAimingAnimationValues.AimedAnimationName, -1, 0f);
//                                KeyframeAimingAnimationValues.AlternativeAimedAnimator.SetFloat(KeyframeAimingAnimationValues.SpeedParameterName, 1.0f);
//                                IsplayingAimedAnim = false;
//                            }
//                            KeyframeAimingAnimationValues.AlternativeAimedAnimator.enabled = false;
//                        }
//                        else
//                        {
//                            if (IsplayingAimedAnim == true)
//                            {
//                                if (PlayerManager.instance != null)
//                                {
//                                    if (PlayerManager.instance.IsMoving == true)
//                                    {
//                                        //  AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.RunAnimationname, -1, 0f);
//                                        AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.IdleAnimationParametreName, false);
//                                        AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.RunAnimationParametreName, true);
//                                        AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.WalkAnimationParametreName, false);
//                                        AnimationsNames.WeaponAnimatorComponent.SetFloat(KeyframeAimingAnimationValues.SpeedParameterName, 1.0f);
//                                        IsplayingAimedAnim = false;
//                                    }
//                                    else
//                                    {
//                                        AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.IdleAnimationName, -1, 0f);
//                                        AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.IdleAnimationParametreName, true);
//                                        AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.RunAnimationParametreName, false);
//                                        AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.WalkAnimationParametreName, false);
//                                        AnimationsNames.WeaponAnimatorComponent.SetFloat(KeyframeAimingAnimationValues.SpeedParameterName, 1.0f);
//                                        IsplayingAimedAnim = false;
//                                    }
//                                }
//                                else
//                                {
//                                    AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.IdleAnimationName, -1, 0f);
//                                    AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.IdleAnimationParametreName, true);
//                                    AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.WalkAnimationParametreName, false);
//                                    AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.RunAnimationParametreName, false);
//                                    AnimationsNames.WeaponAnimatorComponent.SetFloat(KeyframeAimingAnimationValues.SpeedParameterName, 1.0f);
//                                    IsplayingAimedAnim = false;
//                                }
//                            }
//                        }
//                    }
//                }
//            }

//        }
//        public void reloadbtn()
//        {
//            if (Reload.MaximumAmmo > 0)
//            {
//                Reload.Reloadation = true;
//            }
//        }
//        public void InscopeEffect()
//        {
//            IsAimed = true;
//            IsHipFire = false;
//            //ShootingPointParent.transform.localPosition = Vector3.Slerp(HipFirePosition, AimedPosition, AimingSpeed * Time.deltaTime);
//        }
//        public void OutScope()
//        {
//            IsAimed = false;
//            IsHipFire = true;
//            // ShootingPointParent.transform.localPosition = Vector3.Slerp(AimedPosition, HipFirePosition, AimingSpeed * Time.deltaTime);
//        }
//        public void CheckForWalking()
//        {
//            if (IsWalking == true)
//            {
//                AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.IdleAnimationParametreName, false);
//                //  AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.WalkingAnimationName, -1, 0f);
//                AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.WalkAnimationParametreName, true);
//                AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.RunAnimationParametreName, false);

//                if (JoyStick.Instance != null)
//                {
//                    for (int o = 0; o < JoyStick.Instance.WalkingSounds.Length; o++)
//                    {
//                        if (!JoyStick.Instance.WalkingSounds[o].isPlaying)
//                        {
//                            JoyStick.Instance.WalkingSounds[o].Play();
//                        }
//                    }
//                }
//            }
//            else
//            {

//                AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.IdleAnimationParametreName, true);
//                AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.IdleAnimationName);
//                AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.WalkAnimationParametreName, false);
//                AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.RunAnimationParametreName, false);

//                if (JoyStick.Instance != null)
//                {
//                    for (int o = 0; o < JoyStick.Instance.WalkingSounds.Length; o++)
//                    {
//                        if (JoyStick.Instance.WalkingSounds[o].isPlaying)
//                        {
//                            JoyStick.Instance.WalkingSounds[o].Stop();
//                        }
//                    }
//                }
//            }
//        }
//        public void AimedBreathAnimController()
//        {
//            //if (SniperScopeScript.IsScoped == false)
//            //{
//            //    if (AimedAnimatorComponent != null)
//            //    {
//            //        AimedAnimatorComponent.enabled = false;
//            //    }
//            //    IsplayingAimedAnim = false;

//            //}
//            //else
//            //{


//            if (KeyframeAimingAnimationValues.PlayCustomAnimationClip == false)
//            {
//                if (KeyframeAimingAnimationValues.AlternativeAimedAnimator != null)
//                {
//                    KeyframeAimingAnimationValues.AlternativeAimedAnimator.enabled = true;
//                    if (IsplayingAimedAnim == false)
//                    {
//                        KeyframeAimingAnimationValues.AlternativeAimedAnimator.Play(KeyframeAimingAnimationValues.AimedAnimationName, -1, 0f);
//                        KeyframeAimingAnimationValues.AlternativeAimedAnimator.SetFloat(KeyframeAimingAnimationValues.SpeedParameterName, KeyframeAimingAnimationValues.StationaryAimedAnimationSpeed);
//                        IsplayingAimedAnim = true;
//                        AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.WeaponAimedAnimationName, -1, 0f);
//                    }
//                }
//                else
//                {
//                    if (JoyStick.Instance != null)
//                    {
//                        if (JoyStick.Instance.IsWalking == false)
//                        {
//                            if (IsplayingAimedAnim == false)
//                            {
//                                if (AimedAnimState == true)
//                                {
//                                    AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.IdleAnimationParametreName, true);
//                                    AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.WalkAnimationParametreName, false);
//                                    AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.RunAnimationParametreName, false);
//                                    StartCoroutine(Aimeddealay());
//                                    AimedAnimState = false;
//                                }
//                            }
//                        }
//                        else
//                        {
//                            if (AimedAnimState == false)
//                            {
//                                IsplayingAimedAnim = false;
//                                //  AnimationsNames.WeaponAnimatorComponent.Play(AnimationsNames.WalkingAnimationName, -1, 0f);
//                                AimedAnimState = true;
//                            }
//                        }
//                    }
//                }
//            }

//        }
//        IEnumerator Aimeddealay()
//        {
//            yield return new WaitForSeconds(KeyframeAimingAnimationValues.StationaryAimedAnimationDelay);
//            yield return new WaitForSeconds(WeaponOptions.WalkToIdleTransitionTime);
//            //WeaponAnimatorComponent.Play(AimedAnimationName, -1, 0f);
//            //if (AimedAnimationName == IdleAnimationName)
//            //{
//            //    WeaponAnimatorComponent.SetBool(IdleAnimationParametreName, true);
//            //}
//            //WeaponAnimatorComponent.SetFloat(SpeedParameterName, AimedAnimationSpeed);
//            //IsplayingAimedAnim = true;
//            if (IsAimed == true)
//            {
//                if (KeyframeAimingAnimationValues.AimedAnimationName == AnimationsNames.IdleAnimationName)
//                {
//                    AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.IdleAnimationParametreName, true);
//                    AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.WalkAnimationParametreName, false);
//                    AnimationsNames.WeaponAnimatorComponent.SetBool(AnimationsNames.RunAnimationParametreName, false);
//                    AnimationsNames.WeaponAnimatorComponent.SetFloat(KeyframeAimingAnimationValues.SpeedParameterName, KeyframeAimingAnimationValues.StationaryAimedAnimationSpeed);
//                }
//                else
//                {
//                    AnimationsNames.WeaponAnimatorComponent.SetBool(KeyframeAimingAnimationValues.AimedAnimationParametreName, true);
//                    AnimationsNames.WeaponAnimatorComponent.SetFloat(KeyframeAimingAnimationValues.SpeedParameterName, KeyframeAimingAnimationValues.StationaryAimedAnimationSpeed);
//                }
//                IsplayingAimedAnim = true;
//            }
//        }
//        public void Fire()
//        {
//            if (hit.collider != null)
//            {
//                Debug.Log(hit.collider.name);
//                if (hit.collider.gameObject.transform.root.tag == "AI")
//                {
//                    hit.collider.gameObject.transform.root.SendMessage("FindColliderName", hit.collider.name, SendMessageOptions.DontRequireReceiver);
//                    if (hit.collider.gameObject.transform.tag != "WeakPoint")
//                    {
//                        hit.collider.gameObject.transform.root.SendMessage("Takedamage", ShootingFeatures.DamageToTarget, SendMessageOptions.DontRequireReceiver);
//                    }
//                    else
//                    {
//                        hit.collider.gameObject.transform.root.SendMessage("WeakPointdamage", ShootingFeatures.DamageToTarget, SendMessageOptions.DontRequireReceiver);
//                    }
//                    hit.collider.gameObject.transform.root.SendMessage("Effects", hit, SendMessageOptions.DontRequireReceiver);
//                    AddingForceToHumanoidAI(hit);

//                    if (FriendlyFire.instance != null)
//                    {
//                        if (hit.collider.gameObject.transform.root.GetComponent<Targets>() != null)
//                        {
//                            if (hit.collider.gameObject.transform.root.GetComponent<Targets>().MyTeamTag == EnemyIDScript.MyTeamTag)
//                            {
//                                if (hit.collider.gameObject.transform.root.GetComponent<HumanoidAiHealth>() != null)
//                                {
//                                    if (hit.collider.gameObject.transform.root.GetComponent<HumanoidAiHealth>().IsDied == true)
//                                    {
//                                        FriendlyFire.instance.TraitorPlayer(true);
//                                    }
//                                    else
//                                    {
//                                        FriendlyFire.instance.TraitorPlayer(false);
//                                    }
//                                }
//                                else
//                                {
//                                    FriendlyFire.instance.TraitorPlayer(false);
//                                }

//                            }
//                        }
//                    }

//                }
//                if (hit.collider.gameObject.tag == "Target")
//                {
//                    if (hit.transform.gameObject.GetComponent<Target>() != null)
//                    {
//                        hit.transform.gameObject.GetComponent<Target>().StartRotating = true;
//                    }
//                }
//                if (hit.collider.gameObject.transform.root.tag != "Player" && hit.collider.gameObject.transform.root.tag != "AI" && hit.collider.gameObject.tag != "WeakPoint")
//                {
//                    if (hit.collider.gameObject.GetComponent<HitImpactEffect>() != null && ShouldSpawnImpactEffect == true)
//                    {
//                        //Vector3 p = new Vector3(hit.point.x + ImpactEffectOffsetValue, hit.point.y + ImpactEffectOffsetValue, hit.point.z + ImpactEffectOffsetValue);                   
//                        GameObject impacteffect = Instantiate(hit.collider.gameObject.GetComponent<HitImpactEffect>().HitEffect, hit.point, Quaternion.LookRotation(hit.normal));
//                        hit.collider.gameObject.GetComponent<HitImpactEffect>().PlaySound();

//                        if (impacteffect.gameObject.GetComponent<ImpactEffect>() != null)
//                        {
//                            if(transform.root.GetComponent<Targets>() != null)
//                            {
//                                impacteffect.gameObject.GetComponent<ImpactEffect>().SoundManagerScript.SoundProducerScript.Enemy = transform.root;
//                                impacteffect.gameObject.GetComponent<ImpactEffect>().TeamWhichWillBeAffectedByTheShot(transform.root.GetComponent<Targets>().MyTeamTag);
//                                impacteffect.gameObject.GetComponent<ImpactEffect>().EffectActivation(transform);

//                            }
//                        }
//                        // the problem when you make it parent is that if that parent is supposed to be destroy ( for example grenade explosion gameobject )
//                        // in the game it destroy the effect which was being used by the object pooler due to which the condition becomes null and throws error 
//                        //if (impacteffect != null)
//                        //{
//                        //    impacteffect.transform.parent = hit.transform;
//                        //}
//                    }
//                    else
//                    {
//                        if (pooler != null && ShouldSpawnImpactEffect == true)
//                        {

//                            GameObject impacteffect = pooler.SpawnFromPool(ShootingFeatures.ImpactEffectName, hit.point, Quaternion.LookRotation(hit.normal));

//                            if (impacteffect != null)
//                            {
//                                if (impacteffect.GetComponent<AudioSource>() != null)
//                                {
//                                    if (!impacteffect.GetComponent<AudioSource>().isPlaying)
//                                    {
//                                        impacteffect.GetComponent<AudioSource>().Play();
//                                    }

//                                }

//                                if (impacteffect.gameObject.GetComponent<ImpactEffect>() != null)
//                                {
//                                    if (transform.root.GetComponent<Targets>() != null)
//                                    {
//                                        impacteffect.gameObject.GetComponent<ImpactEffect>().SoundManagerScript.SoundProducerScript.Enemy = transform.root;
//                                        impacteffect.gameObject.GetComponent<ImpactEffect>().TeamWhichWillBeAffectedByTheShot(transform.root.GetComponent<Targets>().MyTeamTag);
//                                        impacteffect.gameObject.GetComponent<ImpactEffect>().EffectActivation(transform);
//                                    }
//                                }


//                                // the problem when you make it parent is that if that parent is supposed to be destroy ( for example grenade explosion gameobject )
//                                // in the game it destroy the effect which was being used by the object pooler due to which the condition becomes null and throws error 
//                                //impacteffect.transform.parent = hit.transform;
//                            }

//                        }
//                    }

//                    if (hit.collider.gameObject.GetComponent<HitImpactEffect>() != null)
//                    {
//                        if (hit.collider.gameObject.GetComponent<HitImpactEffect>().DeactivateBulletOnCollision == false)
//                        {
//                            if (hit.collider.GetComponent<MeshCollider>() != null)
//                            {
//                                hit.collider.GetComponent<MeshCollider>().convex = true;
//                                hit.collider.isTrigger = true;
//                            }
//                            else
//                            {
//                                hit.collider.isTrigger = true;
//                            }
//                            StoreHit = hit;
//                            if (hit.collider.gameObject.GetComponent<HitImpactEffect>().SpawnImpactEffectOnSurfacesBehindThisOne == false)
//                            {
//                                ShouldSpawnImpactEffect = false;
//                            }
//                            CanRaycastPass = true;
//                        }

//                    }
//                    else
//                    {
//                        if (StoreHit.collider != null)
//                        {
//                            if (StoreHit.collider.GetComponent<MeshCollider>() != null)
//                            {
//                                StoreHit.collider.isTrigger = false;
//                                StoreHit.collider.GetComponent<MeshCollider>().convex = false;

//                            }
//                            else
//                            {
//                                StoreHit.collider.isTrigger = false;
//                            }
//                            ShouldSpawnImpactEffect = true;
//                        }
//                    }

//                }

//                //if (hit.collider.gameObject.transform.root.tag == "AI")
//                //{
//                //    if (hit.transform.GetComponent<MechaniseAiHealth>() != null)
//                //    {
//                //        drone = hit.transform.GetComponent<MechaniseAiHealth>();
//                //    }

//                //    if (hit.transform.root.gameObject.GetComponent<HumanoidAiHealth>() != null)
//                //    {
//                //        hit.collider.transform.root.GetComponent<HumanoidAiHealth>().Takedamage(DamageToTarget,
//                //        hit.collider.transform.root.GetComponent<HumanoidAiHealth>().gameObject);

//                //        int Randomise = Random.Range(0, hit.collider.transform.root.GetComponent<HumanoidAiHealth>().BloodParticles.Length);
//                //        Blood = Instantiate(hit.collider.transform.root.GetComponent<HumanoidAiHealth>().BloodParticles[Randomise].gameObject, hit.point, Quaternion.LookRotation(hit.normal));
//                //        Destroy(Blood, 1f);
//                //        if (hit.collider.transform.root.GetComponent<HumanoidAiHealth>().Health > 0 &&
//                //            hit.collider.transform.root.GetComponent<HumanoidAiHealth>().Health <= 5)
//                //        {
//                //            GamePlayManager.instance.ShowComboKillsTaken++;
//                //            GamePlayManager.instance.TotalComboKillBonusGot = GamePlayManager.instance.TotalComboKillBonusGot + GamePlayManager.instance.BonusPerComboKillShot;
//                //        }

//                //        if(TeamMatch.instance != null)
//                //        {
//                //            if (TeamMatch.instance.EnableScoreSystemBetweenTeamsAsWinCondition == true)
//                //            {
//                //                if (hit.collider.transform.root.GetComponent<HumanoidAiHealth>().IsDied == true)
//                //                {
//                //                    if (UpdateKills == false)
//                //                    {
//                //                        TeamMatch.instance.Teams[StoreTeamId].Kills += 1;
//                //                        UpdateKills = true;
//                //                    }
//                //                }
//                //                else
//                //                {
//                //                    TeamMatch.instance.Teams[StoreTeamId].TeamScore += TeamMatch.instance.SingleShotPoints;
//                //                    TeamMatch.instance.Teams[StoreTeamId].ScoreText.text = TeamMatch.instance.Teams[StoreTeamId].TeamName + " - " + TeamMatch.instance.Teams[StoreTeamId].TeamScore;
//                //                    UpdateKills = false;
//                //                }
//                //            }
//                //        }

//                //    }
//                //    else if (hit.transform.root.gameObject.GetComponent<ZombieHealth>() != null)
//                //    {
//                //        hit.collider.transform.root.GetComponent<ZombieHealth>().Takedamage(DamageToTarget,
//                //         hit.collider.transform.root.GetComponent<ZombieHealth>().gameObject);
//                //        int Randomise = Random.Range(0, hit.collider.gameObject.transform.root.GetComponent<ZombieHealth>().BloodParticles.Length);
//                //        Blood = Instantiate(hit.collider.gameObject.transform.root.GetComponent<ZombieHealth>().BloodParticles[Randomise].gameObject, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
//                //        //      Blood = Instantiate(bloodEffects.instance.Bloodeffect, hit.point, Quaternion.LookRotation(hit.normal));
//                //        Destroy(Blood, 1f);
//                //        if (hit.collider.transform.root.GetComponent<ZombieHealth>().Health > 0 &&
//                //             hit.collider.transform.root.GetComponent<ZombieHealth>().Health <= 10)
//                //        {
//                //            GamePlayManager.instance.ShowComboKillsTaken++;
//                //            GamePlayManager.instance.TotalComboKillBonusGot = GamePlayManager.instance.TotalComboKillBonusGot + GamePlayManager.instance.BonusPerComboKillShot;
//                //        }
//                //    }
//                //}
//            }


//            // if (drone != null)
//            // {
//            //if(drone.transform.root.GetComponent<TankAiBehaviour>() != null)
//            //{
//            //    //drone.Takedamage(TankDamage, drone.gameObject);
//            //    if (drone.Health > 0)
//            //    {
//            //        int Randomise = Random.Range(0, hit.collider.GetComponent<MechaniseAiHealth>().HitEffects.Length);
//            //        HitEffect = Instantiate(hit.collider.GetComponent<MechaniseAiHealth>().HitEffects[Randomise].gameObject, hit.point, Quaternion.LookRotation(hit.normal));
//            //        Destroy(HitEffect, 1f);
//            //    }
//            //}
//            //else if (drone.transform.root.GetComponent<RotaryWingAiBehaviour>() != null || 
//            //    drone.transform.root.gameObject.transform.GetChild(0).GetComponent<RotaryWingAiBehaviour>() != null)
//            //{
//            //    //drone.Takedamage(HelicopterDamage, drone.gameObject);
//            //    if(drone.Health > 0)
//            //    {
//            //        int Randomise = Random.Range(0, hit.collider.GetComponent<MechaniseAiHealth>().HitEffects.Length);
//            //        HitEffect = Instantiate(hit.collider.GetComponent<MechaniseAiHealth>().HitEffects[Randomise].gameObject, hit.point, Quaternion.LookRotation(hit.normal));
//            //        Destroy(HitEffect, 1f);
//            //    }

//            //}
//            //if (drone.transform.root.GetComponent<Turret>() != null)
//            //{
//            //    drone.Takedamage(DamageToTarget, drone.gameObject);
//            //}
//            //else
//            //{
//            //    drone.Takedamage(DamageToTarget, drone.gameObject);
//            //    if (drone.Health > 0)
//            //    {
//            //        int Randomise = Random.Range(0, hit.collider.GetComponent<MechaniseAiHealth>().HitEffects.Length);
//            //        HitEffect = Instantiate(hit.collider.GetComponent<MechaniseAiHealth>().HitEffects[Randomise].gameObject, hit.point, Quaternion.LookRotation(hit.normal));
//            //        Destroy(HitEffect, 1f);
//            //    }
//            //}

//            //if (drone.Health > 0 && drone.Health <= 5)
//            //{
//            //    GamePlayManager.instance.ShowComboKillsTaken++;
//            //    GamePlayManager.instance.TotalComboKillBonusGot = GamePlayManager.instance.TotalComboKillBonusGot + GamePlayManager.instance.BonusPerComboKillShot;
//            //}
//            // }

//            //if (hit.collider != null)
//            //{
//            //    if (hit.collider.gameObject.name != "Player" && hit.collider.gameObject.transform.root.tag != "AI" && hit.collider.gameObject.tag != "WeakPoint")
//            //    {
//            //        if (pooler != null)
//            //        {
//            //            GameObject impacteffect = pooler.SpawnFromPool(ImpactEffectName, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
//            //            impacteffect.transform.parent = hit.transform;

//            //        }
//            //    }
//            //}
//            //if (hit.collider != null)
//            //{
//            //    if (hit.collider.gameObject.tag == "WeakPoint")
//            //    {
//            //        if (hit.collider.transform.root.GetComponent<HumanoidAiHealth>() != null)
//            //        {
//            //            //if(HeadshotAnimatorComponent != null)
//            //            //{
//            //            //    HeadshotAnimatorComponent.SetBool("headshot", true);
//            //            //    HeadshotAnimatorComponent.gameObject.GetComponent<AudioSource>().Play();
//            //            //}

//            //            int Randomise = Random.Range(0, hit.collider.transform.root.GetComponent<HumanoidAiHealth>().BloodParticles.Length);
//            //            Blood = Instantiate(hit.collider.transform.root.GetComponent<HumanoidAiHealth>().BloodParticles[Randomise].gameObject, hit.point, Quaternion.LookRotation(hit.normal));
//            //            Destroy(Blood, 1f);
//            //            //StartCoroutine(removeheadshot());
//            //            hit.collider.transform.root.GetComponent<HumanoidAiHealth>().Takedamage(DamageToTarget * hit.collider.transform.root.GetComponent<HumanoidAiHealth>().DamageToWeakPoint,
//            //            hit.collider.transform.root.GetComponent<HumanoidAiHealth>().gameObject);

//            //            if(GamePlayManager.instance != null)
//            //            {
//            //                GamePlayManager.instance.ShowHeadShotsTaken++;
//            //                GamePlayManager.instance.TotatHeadShotBonusGot = GamePlayManager.instance.TotatHeadShotBonusGot + GamePlayManager.instance.BonusPerHeadShot;
//            //            }

//            //            if(TeamMatch.instance != null)
//            //            {
//            //                if (TeamMatch.instance.EnableScoreSystemBetweenTeamsAsWinCondition == true)
//            //                {
//            //                    if (hit.collider.transform.root.GetComponent<HumanoidAiHealth>().IsDied == true)
//            //                    {
//            //                        if (UpdateKills == false)
//            //                        {
//            //                            TeamMatch.instance.Teams[StoreTeamId].Kills += 1;
//            //                            UpdateKills = true;
//            //                        }
//            //                    }
//            //                    else
//            //                    {
//            //                        TeamMatch.instance.Teams[StoreTeamId].TeamScore += TeamMatch.instance.HeadShotPoints;
//            //                        TeamMatch.instance.Teams[StoreTeamId].HeadshotsTaken += 1;
//            //                        TeamMatch.instance.Teams[StoreTeamId].ScoreText.text = TeamMatch.instance.Teams[StoreTeamId].TeamName + " - " + TeamMatch.instance.Teams[StoreTeamId].TeamScore;
//            //                        UpdateKills = false;
//            //                    }
//            //                }
//            //            }

//            //        }
//            //        else if (hit.transform.root.gameObject.GetComponent<ZombieHealth>() != null)
//            //        {
//            //            //if (HeadshotAnimatorComponent != null)
//            //            //{
//            //            //    HeadshotAnimatorComponent.SetBool("headshot", true);
//            //            //    HeadshotAnimatorComponent.gameObject.GetComponent<AudioSource>().Play();
//            //            //}
//            //            int Randomise = Random.Range(0, hit.collider.gameObject.transform.root.GetComponent<ZombieHealth>().BloodParticles.Length);
//            //            Blood = Instantiate(hit.collider.gameObject.transform.root.GetComponent<ZombieHealth>().BloodParticles[Randomise].gameObject, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
//            //            Destroy(Blood, 1f);
//            //           // StartCoroutine(removeheadshot());
//            //            hit.collider.transform.root.GetComponent<ZombieHealth>().Takedamage(DamageToTarget * hit.collider.transform.root.GetComponent<ZombieHealth>().DamageToWeakPoint,
//            //                 hit.collider.transform.root.GetComponent<ZombieHealth>().gameObject);
//            //            GamePlayManager.instance.ShowHeadShotsTaken++;
//            //            GamePlayManager.instance.TotatHeadShotBonusGot = GamePlayManager.instance.TotatHeadShotBonusGot + GamePlayManager.instance.BonusPerHeadShot;
//            //        }
//            //        else if (hit.collider.GetComponent<MechaniseAiHealth>() != null)
//            //        {
//            //            drone.Takedamage(DamageToTarget * hit.collider.GetComponent<MechaniseAiHealth>().DamageToWeakPoint, drone.gameObject);
//            //            if (drone.Health > 0)
//            //            {
//            //                int Randomise = Random.Range(0, hit.collider.GetComponent<MechaniseAiHealth>().HitEffects.Length);
//            //                HitEffect = Instantiate(hit.collider.GetComponent<MechaniseAiHealth>().HitEffects[Randomise].gameObject, hit.point, Quaternion.LookRotation(hit.normal));
//            //                Destroy(HitEffect, 1f);
//            //            }
//            //        }
//            //    }
//            //}
//        }
//        public void AddingForceToHumanoidAI(RaycastHit hit)
//        {
//            if (RaycastForce.AddRaycastForce)
//            {
//                if (hit.transform.root.GetComponent<HumanoidAiHealth>() != null)
//                {
//                    if (hit.transform.root.GetComponent<HumanoidAiHealth>().Health <= 0)
//                    {
//                        foreach (Transform g in hit.transform.gameObject.transform.root.GetComponentsInChildren<Transform>())
//                        {
//                            Rigidbody rb = g.gameObject.GetComponent<Rigidbody>();
//                            if (rb != null)
//                            {
//                                // // Calculate the force direction based on the specified offsets in world space
//                                // Vector3 forceDirectionWorld = Random.Range(RaycastForce.MinUpwardForceToApplyOnTarget, RaycastForce.MaxUpwardForceToApplyOnTarget) * Vector3.up +
//                                //                             Random.Range(RaycastForce.MinRightForceToApplyOnTarget, RaycastForce.MaxRightForceToApplyOnTarget) * Vector3.right +
//                                //                             Random.Range(RaycastForce.MinLeftForceToApplyOnTarget, RaycastForce.MaxLeftForceToApplyOnTarget) * Vector3.left +
//                                //                             Random.Range(RaycastForce.MinBackwardForceToApplyOnTarget, RaycastForce.MaxBackwardForceToApplyOnTarget) * Vector3.back +
//                                //                             Random.Range(RaycastForce.MinForwardForceToApplyOnTarget, RaycastForce.MaxForwardForceToApplyOnTarget) * Vector3.forward;

//                                // // Transform the force direction from world space to local space
//                                // //Vector3 forceDirectionLocal = rb.transform.TransformVector(forceDirectionWorld);

//                                // rb.AddForceAtPosition(forceDirectionWorld.normalized * Random.Range(RaycastForce.MinRaycastForce, RaycastForce.MaxRaycastForce), rb.worldCenterOfMass);

//                                // // Apply force in the calculated local direction
//                                //// rb.AddForce(forceDirectionLocal.normalized * Random.Range(RaycastForce.MinRaycastForce, RaycastForce.MaxRaycastForce));
//                                ///

//                                // Calculate and apply the upward force in world space
//                                Vector3 upwardForceWorld = Random.Range(RaycastForce.MinUpwardForceToApplyOnTarget, RaycastForce.MaxUpwardForceToApplyOnTarget) * Vector3.up;
//                                rb.AddForce(upwardForceWorld * Random.Range(RaycastForce.MinRaycastForce, RaycastForce.MaxRaycastForce));

//                                // Calculate and apply other forces in local space
//                                Vector3 forceDirectionLocal = Random.Range(RaycastForce.MinRightForceToApplyOnTarget, RaycastForce.MaxRightForceToApplyOnTarget) * Vector3.right +
//                                                            Random.Range(RaycastForce.MinLeftForceToApplyOnTarget, RaycastForce.MaxLeftForceToApplyOnTarget) * Vector3.left +
//                                                            Random.Range(RaycastForce.MinBackwardForceToApplyOnTarget, RaycastForce.MaxBackwardForceToApplyOnTarget) * Vector3.back +
//                                                            Random.Range(RaycastForce.MinForwardForceToApplyOnTarget, RaycastForce.MaxForwardForceToApplyOnTarget) * Vector3.forward;

//                                rb.AddRelativeForce(forceDirectionLocal * Random.Range(RaycastForce.MinRaycastForce, RaycastForce.MaxRaycastForce));
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }
//}
