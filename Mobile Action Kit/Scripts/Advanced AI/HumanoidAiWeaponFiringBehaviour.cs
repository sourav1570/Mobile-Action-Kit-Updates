using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This Script is Responsible For Humanoid Weapon Shooting Behaviour 

namespace MobileActionKit
{
    public class HumanoidAiWeaponFiringBehaviour : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script defines Ai agents weapon characteristics.";


        public static HumanoidAiWeaponFiringBehaviour instance;

        [System.Serializable]
        public class AboutWeapon
        {
            [Tooltip("This field is applicable to raycast and projectile shooting determining how far this humanoid AI agent weapon can shoot.")]
            public float RaycastBulletRange = 100f;
            [Tooltip("Drag and drop Muzzleflash  particles from the hierarchy (child of the Shooting Point gameobject attached to AI agent`s weapon).")]
            public ParticleSystem MuzzleFlashParticle;

            [Tooltip("Use mesh instead of particles for muzzle flash effect.")]
            public bool UseMuzzleFlashMesh = false;
            [Tooltip("Drag and drop MuzzleFlashMesh  gameobject from the hierarchy (child of the Shooting Point gameobject attached to AI agent`s weapon).")]
            public GameObject MuzzleFlashMesh;

            [Tooltip("Deactivates 'MuzzleFlashMesh' right after weapon shot animation clip playback. Otherwise it will get deactivated after set number of seconds specified in the 'TimeToDeactivateMuzzleMesh' field.")]
            public bool AutoDeactivateMuzzleFlashMesh = true;

            [Tooltip("Muzzle Flash mesh deactivation delay.")]
            public float MuzzleFlashDuration = 0.2f;

            [Tooltip("Set the minimal rotational offset angle value for MuzzlFlashMesh to add variations of its appearance with each new shot(not).")]
            public float MinMuzzleFlashMeshZRotation = -180f;

            [Tooltip("Set the maximal rotational offset angle value for MuzzlFlashMesh to add variations of its appearance with each new shot(not).")]
            public float MaxMuzzleFlashMeshZRotation = 180f;

            [Tooltip("Shots per second parameter of this weapon.")]
            public float FireRate = 15f;

            [Tooltip("If checked will decrease performance hit  by doing raycast shot only once for the first shot at the current closest enemy and after that performing set number of " +
                "consecutive shots inside field below named 'OptimizedRaycastShots' without actual raycasting until target is killed or another enemy becomes closest target in which case raycast " +
                "optimisation will reset itself for that new closest enemy.")]
            public bool OptimizeRaycastShooting = false;

            [Tooltip("Specifies how many consecutive shots after first raycast shot(not including it) Ai agent will do at its current target before resuming OptimizeRaycastShooting cycle.")]
            public int OptimizedRaycastShots = 3;
            [Tooltip("Amount of damage points of each raycast shot of this weapon(Does not affect projectiles).")]
            public float DamageToTarget = 3f;
            [Tooltip("Enter the exact name of the default impact effect from the object pooler that will be applied to surfaces which do not have their own impact effects specified in 'HitImpactEffect' script attached to them.")]
            public string ImpactEffectName = "BulletHole";

            public RaycastHit hit;

            [Tooltip("If checked then this weapon will only do single shots and would not be able to do burst(automatic) firing.")]
            public bool SemiAutomaticFire = false;
            [Tooltip("Minimal time interval between semi-automatic shots")]
            public float MinTimeBetweenSemiAutoShots = 1f;
            [Tooltip("Maximal time interval between semi-automatic shots")]
            public float MaxTimeBetweenSemiAutoShots = 1f;

            //[Tooltip("If enabled than the shooting speed will be decided based upon the fire rate and the fire animation speed.")]
            //public bool AutomiseRecoilSpeed = true;

            [Tooltip("If enabled then bullet shells will be ejected when firing this weapon.")]
            public bool ShouldEjectBulletShell = true;
            [Tooltip("Name of the bullet shell effect inside object pooler.")]
            public string BulletShellName = "AiBulletShell";

            [Tooltip("Drag and drop child gameobject of this weapon named 'BulletShellSpawnPoint' from the hierarchy into this field. Bullet shells particle prefabs will then be spawned at the origin of this gameobject and will along the local Z axis of this game object.")]
            public Transform BulletShellSpawnPoint;

            [Tooltip("Copy and paste name of the projectile to be used with this weapon from the 'ObjectPooler' script into this field.")]
            public string ProjectileName = "Bullet";

            [Tooltip("Enter the number of the projectiles of the single shot of this weapon if you want this weapon to be shotgun. The spread of those projectiles is specified in BulletSpread' paragraph.")]
            public int ProjectilesPerShot = 1;

            [Tooltip("Select one of the three available options for the kind of shooting that this weapon will utilise. RaycastShootingWithTracers is an option where those tracers are not dealing damage to targets and are there only for visuals.")]
            public ShootingOptions ShootingOption;

            [Tooltip("Minimum time delay before commence of the weapon firing behaviour.")]
            public float MinOpenFireDelay = 0.4f;
            [Tooltip("Maximum time delay before commence of the weapon firing behaviour.")]
            public float MaxOpenFireDelay = 1;

            [Range(0,100)][Tooltip("Sets the probability for AI agent to continue firing his weapon(for some time specified in the fields below) at the targets that beacame hidden behind covers.")]
            public float SuppresionFireProbability;

            [Tooltip("Specify the minimum duration of the suppressing fire.")]
            public float MinSuppressionFireDuration = 0.5f;

            [Tooltip("Specify the maximum duration of the suppressing fire.")]
            public float MaxSuppressionFireDuration = 1f;
        }
        public AboutWeapon ShootingFeatures;

        public enum ShootingOptions
        {
            RaycastShooting,
            ProjectileShooting
        }

        [System.Serializable]
        public class ReloadingClass
        {
            [Tooltip("Minimal delay time before reloading the weapon after shooting last bullet from magazine or in cases that require manual reload after each single shot.")]
            public float MinReloadDelayTime = 0f;

            [Tooltip("Maximal delay time before reloading the weapon after shooting last bullet from magazine or in cases that require manual reload after each single shot. ")]
            public float MaxReloadDelayTime = 0f;
            [Tooltip("Maximum amount of rounds for this weapon that Ai agent will have.")]
            public int MaxAmmo = 2000;
            [Tooltip("Amount of rounds in the magazines for this weapon.")]
            public int MagazineSize = 50;
            [HideInInspector]
            public int MaxAmmoAfterReload = 40;
            [Tooltip("If checked then duration of the reload will match the duration of the used reload animation clip. " +
                "If unchecked then it will be possible to set the durations of reload for Crouched and Standing positions regardless of the reload animation clip playback time.")]
            public bool MatchReloadTimeWithAnimation = true;
            [Tooltip("The time it will take to reload this weapon in stance position.")]
            public float StandReloadDuration = 2.3f;
            [Tooltip("The time it will take to reload this weapon in crouched position.")]
            public float CrouchedReloadDuration = 3.3f;
        }
        [Tooltip("Reload parameters of this weapon.")]
        public ReloadingClass Reload;

        [HideInInspector]
        public bool isreloading = false;

        [System.Serializable]
        public class ComponentsClass
        {
            [Tooltip("Drag&Drop this weapon mesh from this AI agent`s hierarchy into this field.")]
            public Transform WeaponMesh;
            [Tooltip("Drag and drop 'CoreAiBehaviourScript' component attached with the root gameobject of this Ai agent from the hierarchy into this field.")]
            public CoreAiBehaviour CoreAiBehaviourScript;

            [Tooltip("Drag and drop 'Animator' component attached with the root gameobject of this Ai agent from the hierarchy into this field.")]
            public Animator AnimatorComponent;

            [Tooltip("Drag&drop 'WeaponSounds' gameobject with 'AlertingSoundActivator' script attached to it from this AI agent`s hierarchy into this field.")]
            public AlertingSoundActivator WeaponSounds;

            [Tooltip("Drag&drop 'DeactivateOtherAgentsAlertingSounds' script attached to this AI agent`s hierarchy into this field.")]
            public ShotSoundAlertOptimizer DeactivateOtherAgentsAlertingSoundsGameOject;

            [Tooltip("Drag and drop the stand reload animation clip from the project window into this field.")]
            public AnimationClip StandReloadClip;
            [Tooltip("Drag and drop the crouch reload animation clip from the project window into this field.")]
            public AnimationClip CrouchReloadClip;
            [Tooltip("Drag and drop the stand fire animation clip from the project window into this field.")]
            public AnimationClip StandFireClip;
            [Tooltip("Drag and drop the crouch fire animation clip from the project window into this field.")]
            public AnimationClip CrouchFireClip;

        }

        [Tooltip("List of required components for this weapon to function.")]
        public ComponentsClass Components;

        private float nexttimetofire = 0f;
        [HideInInspector]
        public bool FireNow;

        [System.Serializable]
        public class BulletSpreadClass
        {
            [Tooltip("Toggle to enable or disable bullet spread for this weapon. When enabled, each shot will have a varying rotational offset as defined by the parameters below.")]
            public bool EnableBulletSpread = true;
            [Tooltip("Negative value of the offset from the initial rotation of X axis in degrees.")]
            public float MinBulletSpreadRotationAngleX = -3f;
            [Tooltip("Positive value of the offset from the initial rotation of X axis in degrees.")]
            public float MaxBulletSpreadRotationAngleX = 3f;

            [Tooltip("Negative value of the offset from the initial rotation of Y axis in degrees.")]
            public float MinBulletSpreadRotationAngleY = -88f;
            [Tooltip("Positive value of the offset from the initial rotation of Y axis in degrees.")]
            public float MaxBulletSpreadRotationAngleY = -92f;
        }

        [Tooltip("Values of the 'ShootingPoint' rotational offset with each new shot along X & Y axis to create bullet spread for this weapon.")]
        public BulletSpreadClass BulletSpread;      

        [System.Serializable]
        public enum PlayerForce
        {
            ShakePlayerCamera,
            ApplyForceToPlayerRigidbody,
            ApplyForceToPlayerRigidbodyAndShakePlayerCamera,
            DoNotApplyForceToPlayer
        }

        [System.Serializable]
        public class RaycastForceClass
        {
            [Tooltip("If checked will apply raycast force to target.")]
            public bool AddRaycastForce = false;

            //[Tooltip("Minimal possible RayCastForce to be applied to target.")]
            //public float MinRaycastForce = 800;
            //[Tooltip("Maximal possible RayCastForce to be applied to target.")]
            //public float MaxRaycastForce = 1100;

            //[Tooltip("Minimal possible RaycastForce to be applied to target in upward direction.")]
            //public float MinUpwardForce = 2f;
            //[Tooltip("Maximal possible RaycastForce to be applied to target in upward direction.")]
            //public float MaxUpwardForce = 5f;

            //[Tooltip("Minimal possible RaycastForce to be applied to target in right direction.")]
            //public float MinRightForce = 2f;
            //[Tooltip("Maximal possible RaycastForce to be applied to target in right direction.")]
            //public float MaxRightForce = 2f;

            //[Tooltip("Minimal possible RaycastForce to be applied to target in left direction.")]
            //public float MinLeftForce = 1f;
            //[Tooltip("Maximal possible RaycastForce to be applied to target in left direction.")]
            //public float MaxLeftForce = 1f;

            //[Tooltip("Minimal possible RaycastForce to be applied to target in backward direction.")]
            //public float MinBackwardForce = 5f;
            //[Tooltip("Maximal possible RaycastForce to be applied to target in backward direction.")]
            //public float MaxBackwardForce = 8f;

            //[Tooltip("Minimal possible RaycastForce to be applied to target in forward direction.")]
            //public float MinForwardForce = 1f;
            //[Tooltip("Maximal possible RaycastForce to be applied to target in forward direction.")]
            //public float MaxForwardForce = 1f;

            [Tooltip("Minimum explosive force value will be applied to other Humanoid Ai agent on grenade explosion. the higher the value is the higher the humanoid Ai agent will fly if get fall into the " +
          "grenade trap. For the player this value is not true but you can add camera shaker effect script to the camera of the player and tweak the values there which will in return going to shake the camera of the player.")]
            public float AIMinExplosiveForce = 8f;
            [Tooltip("Maximum explosive force value will be applied to other Humanoid Ai agent on grenade explosion. the higher the value is the higher the humanoid Ai agent will fly if get fall into the " +
                "grenade trap. For the player this value is not true but you can add camera shaker effect script to the camera of the player and tweak the values there which will in return going to shake the camera of the player.")]
            public float AIMaxExplosiveForce = 8f;

            public float NonAIExplosiveForce = 4f;

            public float RadiusToApplyForce = 50f;

            [HideInInspector]
            public List<Transform> TargetsToApplyForce = new List<Transform>();

            public PlayerForce PlayerExplosiveForce = PlayerForce.ShakePlayerCamera; 
        }

        [Tooltip("This paragraph sets various parameters of raycast bullet force that can be applied  to Humanoid Ai agents with the killing shot(the one that reduces Ai agents health to zero).")]
        public RaycastForceClass RaycastBulletForce;

        [Tooltip("Specify for how long this weapon will stay active after Ai agent that carried it was killed.")]
        public float DroppedWeaponLifeTime = 6f;

        public bool OptimizeShotAlertSound = true;

        [Tooltip("Sets the minimum delay in seconds for activation of ShotAlertOptimizer radius. " +
            "This field is relevant when Ai agent that is still in combat state is no longer affected by initial ShotAlertOptimizer radius because Ai agent that had it on him died or moved away farther pass the limits of that initial collider.")]
        public float MinShotAlertOptimizerDelay = 3;


        [Tooltip("Sets the maximum delay in seconds for activation of ShotAlertOptimizer radius. " +
            "This field is relevant when Ai agent that is still in combat state is no longer affected by initial ShotAlertOptimizer radius because Ai agent that had it on him died or moved away farther pass the limits of that initial collider.")]
        public float MaxShotAlertOptimizerDelay = 5;

        ObjectPooler pooler;
        float ratio = 1;
        float TargetX = 0;
        float TargetY = 0;
        GameObject Blood;
        [HideInInspector]
        public bool IFired = false;
        private Targets EnemyIDScript;
        [HideInInspector]
        public int StoreTeamId;
        bool UpdateKills = false;
        [HideInInspector]
        public bool IsWeaponReloading = false;
        float TimeToShoot;
        bool TimerCompleted = false;
        [HideInInspector]
        public string nametolookfor;
        [HideInInspector]
        public bool IsFiring = false;
        float StandAutoReloadTime;
        float CrouchAutoReloadTime;
        HumanoidAiHealth HAH;
        bool StartMuzzleMeshTimer = false;

        bool CheckForFirstShot = false;

        [HideInInspector]
        public bool StopShoot = false;
        //bool ShouldSpawnImpactEffect = true;
        bool CanRaycastPass = false;

        [HideInInspector]
        public bool HoldShoot = false;

        float OriginalReloadSpeed;

        [HideInInspector]
        public bool ReloadDelayCompleted = false;
        bool StoreReloadDelay = false;

        [HideInInspector]
        public bool PlayingFiringAnimation = false;

        bool CanShootNow = false;

        float TimerForSemiAuto;
        float RandomiseStandFireAiming;
        float RandomiseCrouchFireAiming;
        float RandomiseTriggerPulls;
        bool StopTimer = false;

        [HideInInspector]
        public bool NextShotDelay = false;

        [System.Serializable]
        public class DistanceBasedShootingclass
        {
            [Tooltip("Sets the maximal distance limit for the range within which this burst type will be performed")]
            public float MaxDistance;
            [Tooltip("Sets the minimal distance limit for the range within which this burst type will be performed")]
            public float MinDistance;
            [Tooltip("Minimum number of shots in a burst when firing at a target that is between min and max limits of this range.")]
            public int MinBurstSize;
            [Tooltip("Maximum number of shots in a burst when firing at a target that is between min and max limits of this range.")]
            public int MaxBurstSize;

            [Tooltip("Minimal time interval between bursts")]
            public float MinTimeBetweenBursts = 1f;
            [Tooltip("Maximal time interval between bursts")]
            public float MaxTimeBetweenBursts = 2f;
        }

        [Tooltip("If enabled then Ai agent will fire this weapon based on values of 'DistanceBasedShooting' paragraph which is designed for Ai agent to perform more realistic shooting style to " +
            "increase efficiency of killing the enemy at various ranges.")]
        public bool EnableDistanceBasedShooting = true;

        [Tooltip("Minimal time interval between distance to target checks for performing shooting style based on that distance.")]
        public float MinTargetDistanceCheckInterval = 0.2f;
        [Tooltip("Maximal time interval between distance to target checks for performing shooting style based on that distance.")]
        public float MaxTargetDistanceCheckInterval = 0.5f;

        [Tooltip("This list defines DistanceBasedShooting and sets various shooting parameters for each distance that is set in the each element of this list. It is set by default to increase volume of fire as the distance to target gets shorter.")]
        public List<DistanceBasedShootingclass> DistanceBasedShooting = new List<DistanceBasedShootingclass>();

        bool ShakeSpine = false;
        bool StopSpineShake = false;

        float StoreStandFiring;
        float StoreCrouchFiring;
        int Counts;

        int BurstFireBulletsNumber;
        bool TimeToResetBurst = false;
        bool StartTimerForResetBurst = false;

        bool CheckDistanceWithTarget = false;
        int BurstToDo;
        bool BurstFireUnderProcess = false;

        bool IsDefaultShooting = false;

        Vector3 CheckAllDistanceWithTarget;

        [HideInInspector]
        public bool ShootIntruppt = false;

        bool IsProjectileShootingActive = false;

        [HideInInspector]
        public RaycastHit StoreHit;

        [System.Serializable]
        public class EnhancedFriendlyFireAvoidanceClass
        {
            [Tooltip(" If enabled then Ai agent will check if any of his friendlies are within LineOfFire threshold before firing his weapon and if they are then Ai agent will not shoot until they move out of his line of fire.")]
            public bool EnableLineOfFireChecks = true;

            //[Tooltip("Field of view value to check whether the friendly is within the Ai agent shooting area")]
            //public float FriendlyFireAvoidanceFov = 30f;

            [Tooltip("Distance along local X axis from direct line of fire within which to do the checks for friendlies.")]
            public float LineOfFireProximityX = 0.7f;
            [Tooltip("Distance along local Y axis from direct line of fire within which to do the checks for friendlies.")]
            public float LineOfFireProximityY = 0.7f;

            [Tooltip("If checked then will display in the fields below the proximity values of the closest friendly Ai agent to LineOfFire.")]
            public bool DebugProximityToLineOfFire = true;

            [Tooltip("Displays the friendly Ai agent separation along X axis from direct line of fire.")]
            public float DisplayXAxisProximity;

            [Tooltip("Displays the friendly Ai agent separation along Y axis from direct line of fire.")]
            public float DisplayYAxisProximity;

            [Tooltip("Minimal time interval between checks for friendlies within line of fire dangerous proximity.")]
            public float MinTimeBetweenLineOfFireChecks = 0.3f;

            [Tooltip("Maximal time interval between checks for friendlies within line of fire dangerous proximity.")]
            public float MaxTimeBetweenLineOfFireChecks = 0.6f;
        }

        [Tooltip("This paragraph defines the LineOfFireChecks parameters when Aiming AI agent is careful not to fire his weapon when his friendlies are within a specified distance from the line of fire. " +
            "Thus minimising the chances of accidental friendly fire. Aiming Ai agent will check if any friendly AI agents are within line of fire threshold which is checked along X and Y axis from his root objects local Z " +
            "axis which would be facing the enemy at the time of this check.And if separation between Ai agent`s root object local Z axis and friendly AI agent closest to the line of fire in both X and Y axis Line Of fire threshold values " +
            "is less then allowed for both of those values then AI agent will cease fire until his friendly gets further away from his local Z axis along either Y or X axis so that friendly Ai agent would get outside of line of fire threshold." +
            "In case aiming Ai agent will be standing and closest friendly to the line of fire will be crouching in front of him, then X and Y proximity checks will be bypassed and  aiming Ai agent would be able to open fire." +
            "And in case aiming Ai agent is crouching then X and Y proximity checks will be done regardless of whether closest friendly to the line of fire is crouching or standing. ")]
        public EnhancedFriendlyFireAvoidanceClass LineOfFireChecks;

        List<Transform> targetAIList = new List<Transform>();
        private float timeSinceLastCheck = 0f;
        private float RandomValueForFireAvoidanceCheck;

        [HideInInspector]
        public bool StopShootDueToFriendlyInFOV = false;
        ObjectPooler Pooler;

        [HideInInspector]
        public bool StopShootDueToMelee = false;

        //RaycastHit StoredObject;
        int CurrentBulletShotsCounts = 0;
        bool IsObjectStored = false;

        string PreviousTargetName;
        bool SavePreviousTarget;

        bool NewTargetToShoot = false;

        [HideInInspector]
        public GameObject AgentWhoActivatedAlertingSoundGameObject;
        [HideInInspector]
        public bool DoNotActivateSoundCoordinate = false;

        [HideInInspector]
        public bool StartActivateSoundCoroutine = false;

        private void Start()
        {
           
            if (instance == null)
            {
                instance = this;
            }
            if (ShootingFeatures.OptimizeRaycastShooting == true)
            {
                CurrentBulletShotsCounts = ShootingFeatures.OptimizedRaycastShots;
            }
            Reload.MaxAmmoAfterReload = Reload.MagazineSize;
            pooler = ObjectPooler.instance;
            if (transform.root.gameObject.GetComponent<Targets>() != null)
            {
                EnemyIDScript = transform.root.gameObject.GetComponent<Targets>();
            }
            if (transform.root.gameObject.GetComponent<HumanoidAiHealth>() != null)
            {
                HAH = transform.root.gameObject.GetComponent<HumanoidAiHealth>();
            }

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

            TimeToShoot = Random.Range(ShootingFeatures.MinOpenFireDelay, ShootingFeatures.MaxOpenFireDelay);
            StartCoroutine(IsTimerCompleted());

            nametolookfor = transform.root.GetComponent<Targets>().MyTeamID;

            if (Reload.MatchReloadTimeWithAnimation == true)
            {
                StandAutoReloadTime = Components.StandReloadClip.length;
                CrouchAutoReloadTime = Components.CrouchReloadClip.length;
            }



            //if(Reload.MagazineSize == 1 || ShootingFeatures.EnableSemiAutomaticFire == true)
            //{
            //    StopWeaponRecoil = true;
            //}

            RandomiseTriggerPulls = Random.Range(ShootingFeatures.MinTimeBetweenSemiAutoShots, ShootingFeatures.MaxTimeBetweenSemiAutoShots);

            if (ShootingFeatures.AutoDeactivateMuzzleFlashMesh == true)
            {
                if (ShootingFeatures.SemiAutomaticFire == false || EnableDistanceBasedShooting == true)
                {
                    ShootingFeatures.MuzzleFlashDuration = 1f / ShootingFeatures.FireRate;
                    float MinusTimer = ShootingFeatures.MuzzleFlashDuration * 70f / 100f; // we make sure that before the next shot the muzzle flash can be activated early on //
                                                                                                 // ShootingFeatures.TimeToDeactivateMuzzleMesh = ShootingFeatures.TimeToDeactivateMuzzleMesh - MinusTimer;
                    ShootingFeatures.MuzzleFlashDuration = MinusTimer;
                }
                else
                {
                    ShootingFeatures.MuzzleFlashDuration = RandomiseTriggerPulls * 70f / 100f;
                }
            }



            //if (ShootingFeatures.AutomiseRecoilSpeed == true)
            //{
            if (ShootingFeatures.SemiAutomaticFire == false || EnableDistanceBasedShooting == true)
            {
                float GetValue = 1f / ShootingFeatures.FireRate;
                float NewValue = Components.StandFireClip.length / GetValue;
                Components.CoreAiBehaviourScript.Speeds.AnimationSpeeds.FireAnimationSpeed = NewValue;
            }
            else
            {
                float NewValue = Components.StandFireClip.length / RandomiseTriggerPulls;
                Components.CoreAiBehaviourScript.Speeds.AnimationSpeeds.FireAnimationSpeed = NewValue;
            }

            if (ShootingFeatures.SemiAutomaticFire == false || EnableDistanceBasedShooting == true)
            {
                float GetValue = 1f / ShootingFeatures.FireRate;
                float NewValue = Components.CrouchFireClip.length / GetValue;
                Components.CoreAiBehaviourScript.Speeds.AnimationSpeeds.CrouchFireAnimationSpeed = NewValue;
            }
            else
            {
                float NewValue = Components.CrouchFireClip.length / RandomiseTriggerPulls;
                Components.CoreAiBehaviourScript.Speeds.AnimationSpeeds.CrouchFireAnimationSpeed = NewValue;
            }
            //}

            if (ObjectPooler.instance != null)
            {
                Pooler = ObjectPooler.instance;
            }

            OriginalReloadSpeed = Components.CoreAiBehaviourScript.Speeds.AnimationSpeeds.ReloadAnimationSpeed;
            // OriginalFireAnimationSpeed = Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.FireAnimationSpeed;

            RandomiseStandFireAiming = Components.StandFireClip.length / Components.CoreAiBehaviourScript.Speeds.AnimationSpeeds.FireAnimationSpeed;
            RandomiseCrouchFireAiming = Components.CrouchFireClip.length / Components.CoreAiBehaviourScript.Speeds.AnimationSpeeds.CrouchFireAnimationSpeed;

            StoreStandFiring = RandomiseStandFireAiming;
            StoreCrouchFiring = RandomiseCrouchFireAiming;

            if (ShootingFeatures.ShootingOption == ShootingOptions.ProjectileShooting)
            {
                IsProjectileShootingActive = true;
            }

            if (LineOfFireChecks.EnableLineOfFireChecks == true)
            {
                RandomValueForFireAvoidanceCheck = Random.Range(LineOfFireChecks.MinTimeBetweenLineOfFireChecks, LineOfFireChecks.MaxTimeBetweenLineOfFireChecks);
                FindTargetAI();
            }

        }
        void OnEnable()
        {
            isreloading = false;
        }
        IEnumerator ReloadingDelay()
        {
            float Randomise = Random.Range(Reload.MinReloadDelayTime, Reload.MaxReloadDelayTime);
            yield return new WaitForSeconds(Randomise);
            ReloadDelayCompleted = true;
        }
        IEnumerator NextAutoSemiShotDelay()
        {
            if (ShootingFeatures.SemiAutomaticFire == true)
            {
                yield return new WaitForSeconds(RandomiseTriggerPulls);
                NextShotDelay = false;
                CanShootNow = false;
            }
            else
            {
                NextShotDelay = false;
                CanShootNow = false;
            }

        }
        public void MuzzleMeshFunction()
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
        public void AddBulletSpread()
        {
            if (BulletSpread.EnableBulletSpread == true)
            {
                Vector3 Spread = transform.localEulerAngles;
                Spread.x = Random.Range(BulletSpread.MinBulletSpreadRotationAngleX, BulletSpread.MaxBulletSpreadRotationAngleX);
                Spread.y = Random.Range(BulletSpread.MinBulletSpreadRotationAngleY, BulletSpread.MaxBulletSpreadRotationAngleY);
                transform.localEulerAngles = Spread;
            }
        }
        public void fireSystem() // Firing Functionality
        {
            if (isreloading)
                return;

            if(Reload.MaxAmmo > 0)
            {
                if (Reload.MagazineSize <= 0)
                {
                    if (IsWeaponReloading == false && ReloadDelayCompleted == true)
                    {
                        StartCoroutine(ReloadTimer());
                        IsWeaponReloading = true;
                    }

                    if (StoreReloadDelay == false && NextShotDelay == false)
                    {
                        StopShoot = true;
                        //isreloading = true;
                        StartCoroutine(ReloadingDelay());
                        StoreReloadDelay = true;
                    }
                }
            }
           

            if (CanRaycastPass == true)
            {
                Shoot();
                CanRaycastPass = false;
            }

            CheckShootingDistance();

            if (CheckAllDistanceWithTarget.magnitude <= ShootingFeatures.RaycastBulletRange)
            {
                if (EnableDistanceBasedShooting == false || IsDefaultShooting == true)
                {
                    //if(EnableBurstFire == true)
                    //{
                    //    CheckShootingDistance();
                    //}
                    //  WorkingStatus = "FireNow" + FireNow + "StopShoot" + StopShoot + "HoldShoot" + HoldShoot + "NextshotDelay" + NextShotDelay + "ShootIntruppt" + ShootIntruppt;
                    if (Time.time >= nexttimetofire && FireNow == true && isreloading == false && HAH.IsDied == false && StopShootDueToMelee == false && StopShoot == false && HoldShoot == false && ShootingFeatures.SemiAutomaticFire == false
                   && Reload.MagazineSize >= 1 && NextShotDelay == false && ShootIntruppt == false && StopShootDueToFriendlyInFOV == false)
                    {
                        // WorkingStatus = "Working";
                        if (CheckForFirstShot == false)
                        {
                            TimeToShoot = Random.Range(ShootingFeatures.MinOpenFireDelay, ShootingFeatures.MaxOpenFireDelay);
                            StartCoroutine(IsTimerCompleted());
                            CheckForFirstShot = true;
                        }

                        if (TimerCompleted == true)
                        {
                            //TargetX += (Random.value - 0.5f) * Mathf.Lerp(BulletSpread.BulletSpreadXRotationValue, BulletSpread.BulletSpreadYRotationValue, ratio);
                            //TargetY += (Random.value - 0.5f) * Mathf.Lerp(BulletSpread.BulletSpreadXRotationValue, BulletSpread.BulletSpreadYRotationValue, ratio);

                            AddBulletSpread();
                        

                            //if (transform != null)
                            //{
                            //    transform.localRotation = Quaternion.Euler(TargetX, TargetY, 0f);
                            //}

                            if (Components.CoreAiBehaviourScript.IsCrouched == false)
                            {

                                //  Components.AnimatorComponent.SetLayerWeight(3, 0f);
                                // Components.CoreAiBehaviourScript.LayerIndexToCheck = 2;
                                // Components.CoreAiBehaviourScript.ChangeLayerWeight(1);


                                Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(4, 0f);
                                Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);

                                if (CanShootNow == false)
                                {
                                    RandomiseStandFireAiming = StoreStandFiring;
                                    Components.CoreAiBehaviourScript.SetAnimationForFullBody(Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.FireParameterName);
                                    CanShootNow = true;
                                }
                            }
                            else
                            {

                                //Components.AnimatorComponent.SetLayerWeight(2, 0f);

                                //Components.CoreAiBehaviourScript.LayerIndexToCheck = 3;
                                //Components.CoreAiBehaviourScript.ChangeLayerWeight(1);

                                Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 0f);
                                Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(4, 1f, false);

                                if (CanShootNow == false)
                                {
                                    RandomiseCrouchFireAiming = StoreCrouchFiring;
                                    Components.CoreAiBehaviourScript.SetAnimationForFullBody(Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.CrouchFireParameterName);
                                    CanShootNow = true;
                                }
                            }


                            // ShootingFeatures.shooting = "Shoot";
                            PlayingFiringAnimation = true;
                            nexttimetofire = Time.time + 1f / ShootingFeatures.FireRate;
                            Shoot();

                            MuzzleMeshFunction();
                            NextShotDelay = true;
                            StopTimer = false;


                            //Debug.Log("Shoot");
                            //Timer += Time.deltaTime;
                            //if (Timer >= 1f)
                            //{
                            //    Timer = 0f;
                            //    Debug.Break();
                            //}
                        }

                    }
                    else if (ShootingFeatures.SemiAutomaticFire == true && NextShotDelay == false
                        && FireNow == true && isreloading == false && HAH.IsDied == false && StopShootDueToMelee == false && StopShoot == false && HoldShoot == false && Reload.MagazineSize >= 1 && ShootIntruppt == false && StopShootDueToFriendlyInFOV == false)
                    {
                        if (CheckForFirstShot == false)
                        {
                            TimeToShoot = Random.Range(ShootingFeatures.MinOpenFireDelay, ShootingFeatures.MaxOpenFireDelay);
                            StartCoroutine(IsTimerCompleted());
                            CheckForFirstShot = true;
                        }

                        if (TimerCompleted == true)
                        {
                            //  ShootingFeatures.shooting = "Shoot";
                            //TargetX += (Random.value - 0.5f) * Mathf.Lerp(BulletSpread.BulletSpreadXRotationValue, BulletSpread.BulletSpreadYRotationValue, ratio);
                            //TargetY += (Random.value - 0.5f) * Mathf.Lerp(BulletSpread.BulletSpreadXRotationValue, BulletSpread.BulletSpreadYRotationValue, ratio);

                            AddBulletSpread();

                            //if (transform != null)
                            //{
                            //    transform.localRotation = Quaternion.Euler(TargetX, TargetY, 0f);
                            //}

                            if (Components.CoreAiBehaviourScript.IsCrouched == false)
                            {

                                //Components.AnimatorComponent.SetLayerWeight(3, 0f);
                                //Components.CoreAiBehaviourScript.LayerIndexToCheck = 2;
                                //Components.CoreAiBehaviourScript.ChangeLayerWeight(1);

                                Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(4, 0f);
                                Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);

                                if (CanShootNow == false)
                                {
                                    RandomiseStandFireAiming = StoreStandFiring;
                                    Components.CoreAiBehaviourScript.SetAnimationForFullBody(Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.FireParameterName);
                                    CanShootNow = true;
                                }
                            }
                            else
                            {

                                //Components.AnimatorComponent.SetLayerWeight(2, 0f);
                                //Components.CoreAiBehaviourScript.LayerIndexToCheck = 3;
                                //Components.CoreAiBehaviourScript.ChangeLayerWeight(1);

                                Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 0f);
                                Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(4, 1f, false);

                                if (CanShootNow == false)
                                {
                                    RandomiseCrouchFireAiming = StoreCrouchFiring;
                                    Components.CoreAiBehaviourScript.SetAnimationForFullBody(Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.CrouchFireParameterName);
                                    CanShootNow = true;
                                }
                            }

                            //if (ShootingFeatures.EnableWeaponRecoil == true && StopWeaponRecoil == false)
                            //{
                            //    if (Components.CoreAiBehaviourScript.IsCrouched == false)
                            //    {
                            //        //  Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.ReloadAnimationSpeed = 0f;
                            //        Components.AnimatorComponent.SetLayerWeight(ShootingFeatures.WeaponRecoilAnimatorLayerIndex, 1f);
                            //        Components.AnimatorComponent.SetBool(ShootingFeatures.WeaponRecoilAnimatorParameterName, true);
                            //    }
                            //}
                            PlayingFiringAnimation = true;

                            Shoot();
                            MuzzleMeshFunction();
                            NextShotDelay = true;
                            StopTimer = false;
                        }
                    }
                }
                else
                {
                    if (TimeToResetBurst == false)
                    {

                        CheckShootingDistance();
                        //Vector3 CheckDistanceWithTarget = Components.CoreAiBehaviourScript.FindEnemiesScript.enemy[Components.CoreAiBehaviourScript.FindEnemiesScript.CurrentEnemy].position - Components.CoreAiBehaviourScript.transform.position;
                        //if (CheckDistanceWithTarget.magnitude >= DistanceBasedShooting[Counts].MinDistance && CheckDistanceWithTarget.magnitude <= DistanceBasedShooting[Counts].MaxDistance)
                        //{
                        if (BurstFireUnderProcess == true && IsDefaultShooting == false)
                        {
                            if (Time.time >= nexttimetofire && FireNow == true && isreloading == false && HAH.IsDied == false && StopShootDueToMelee == false && StopShoot == false && HoldShoot == false && Reload.MagazineSize >= 1
                                && NextShotDelay == false && ShootIntruppt == false && StopShootDueToFriendlyInFOV == false)
                            {
                                BurstFire(BurstToDo);
                            }
                        }
                        //else
                        //{
                        //    BurstFire(store);
                        //}
                        //}
                    }
                    else
                    {
                        if (StartTimerForResetBurst == false)
                        {
                            StartCoroutine(CoroForResetBustFire());
                            StartTimerForResetBurst = true;
                        }
                    }

                }

                if (NextShotDelay == true)
                {
                    if (StopTimer == false)
                    {
                        TimerForSemiAuto += Time.deltaTime;
                    }
                    if (TimerForSemiAuto >= RandomiseStandFireAiming && CanShootNow == true)
                    {
                        if (FireNow == true && isreloading == false && HAH.IsDied == false && StopShootDueToFriendlyInFOV == false && StopShootDueToMelee == false && StopShoot == false && HoldShoot == false && ShootIntruppt == false && StopShootDueToFriendlyInFOV == false)
                        {
                            //ShootingFeatures.shooting = "Aiming";
                            if (Components.CoreAiBehaviourScript.IsCrouched == false)
                            {

                                //Components.AnimatorComponent.SetLayerWeight(3, 0f);                              
                                //Components.CoreAiBehaviourScript.LayerIndexToCheck = 2;
                                //Components.CoreAiBehaviourScript.ChangeLayerWeight(1);

                                Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(4, 0f);
                                Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);


                                Components.CoreAiBehaviourScript.SetAnimationForFullBody(Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.StandAimingParameterName);

                            }
                            else
                            {

                                //Components.AnimatorComponent.SetLayerWeight(2, 0f);
                                //Components.CoreAiBehaviourScript.LayerIndexToCheck = 3;
                                //Components.CoreAiBehaviourScript.ChangeLayerWeight(1);
                                Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 0f);
                                Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(4, 1f, false);

                                Components.CoreAiBehaviourScript.SetAnimationForFullBody(Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.CrouchAimingParameterName);

                            }
                            //Components.CoreAiBehaviourScript.anim.SetBool(Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.FireParameterName, false);
                            TimerForSemiAuto = 0f;
                            StopTimer = true;
                            StartCoroutine(NextAutoSemiShotDelay());
                        }

                    }
                }

                if (FireNow == false || StopShoot == true || HoldShoot == true || ShootIntruppt == true || StopShootDueToFriendlyInFOV == true || StopShootDueToMelee == true)
                {
                    if (StopSpineShake == false)
                    {
                        ShakeSpine = false;
                        //Components.AnimatorComponent.SetLayerWeight(2, 0f);
                        // Components.AnimatorComponent.SetLayerWeight(3, 0f);
                        CheckForFirstShot = false;
                        TimerCompleted = false;
                        if(Components.CoreAiBehaviourScript != null)
                        {
                            if(Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript != null)
                            {
                                Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 0f);
                                Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(4, 0f);
                            }
                         
                            Components.CoreAiBehaviourScript.OvveruleStandShootPostureAnimation = false;

                        }
                        NextShotDelay = false;
                        CanShootNow = false;
                        StopSpineShake = true;
                    }
                }
                else
                {
                    StopSpineShake = false;
                }

                if (FireNow == false && NextShotDelay == false || StopShoot == true && NextShotDelay == false || HoldShoot == true && NextShotDelay == false || ShootIntruppt == true && NextShotDelay == false
                    || StopShootDueToFriendlyInFOV == true && NextShotDelay == false || StopShootDueToMelee == true && NextShotDelay == false)
                {
                    Components.CoreAiBehaviourScript.OvveruleStandShootPostureAnimation = false;
                    PlayingFiringAnimation = false;
                    CheckForFirstShot = false;
                    TimerCompleted = false;
                }
            }

            //if(EnhancedFriendlyFireAvoidance.EnableAdvancedFriendlyFireAvoidance == true)
            //{
            //    timeSinceLastCheck += Time.deltaTime;

            //    if (timeSinceLastCheck >= RandomValueForFireAvoidanceCheck)
            //    {
            //        timeSinceLastCheck = 0f;

            //        for (int i = 0; i < targetAIList.Count; i++)
            //        {
            //            Transform targetAI = targetAIList[i];

            //            if (targetAI != null)
            //            {
            //                if (IsWithinFieldOfView(targetAI) && IsWithinDistance(targetAI))
            //                {
            //                    StopShootDueToFriendlyInFOV = true;
            //                }
            //                else
            //                {
            //                    StopShootDueToFriendlyInFOV = false;
            //                }
            //            }
            //        }
            //    }
            //}

            if (LineOfFireChecks.EnableLineOfFireChecks == true)
            {
                timeSinceLastCheck += Time.deltaTime;

                if (timeSinceLastCheck >= RandomValueForFireAvoidanceCheck)
                {
                    timeSinceLastCheck = 0f;
                    Transform closestAI = FindClosestFriendlyAI();

                    if (closestAI != null)
                    {
                        if (CheckXAndYDistances(closestAI))// && IsWithinDistance(closestAI))
                        {
                            StopShootDueToFriendlyInFOV = false;
                        }
                        else
                        {
                            StopShootDueToFriendlyInFOV = true;
                        }
                    }
                    else
                    {
                        StopShootDueToFriendlyInFOV = false;
                    }
                }
            }


        }
        public void CheckShootingDistance()
        {
            if(Components.CoreAiBehaviourScript.FindEnemiesScript != null)
            {
                if (IsDefaultShooting == true)
                {
                    BurstFireUnderProcess = false;
                }
                if(Components.CoreAiBehaviourScript.FindEnemiesScript.enemy.Count >= 1)
                {
                    if (CheckDistanceWithTarget == false && BurstFireUnderProcess == false && Components.CoreAiBehaviourScript.FindEnemiesScript.enemy[Components.CoreAiBehaviourScript.FindEnemiesScript.CurrentEnemy] != null)
                    {
                        CheckAllDistanceWithTarget = Components.CoreAiBehaviourScript.FindEnemiesScript.enemy[Components.CoreAiBehaviourScript.FindEnemiesScript.CurrentEnemy].position - Components.CoreAiBehaviourScript.transform.position;
                        //  DebugCurrentDistanceToTarget = CheckAllDistanceWithTarget.magnitude;

                        if (EnableDistanceBasedShooting == true)
                        {
                            IsDefaultShooting = true;

                            if (DistanceBasedShooting.Count >= 1)
                            {
                                for (int x = 0; x < DistanceBasedShooting.Count; x++)
                                {
                                    if (CheckAllDistanceWithTarget.magnitude >= DistanceBasedShooting[x].MinDistance && CheckAllDistanceWithTarget.magnitude <= DistanceBasedShooting[x].MaxDistance)
                                    {
                                        IsDefaultShooting = false;
                                        Counts = x;
                                    }
                                    else if (CheckAllDistanceWithTarget.magnitude >= DistanceBasedShooting[0].MaxDistance)
                                    {
                                        IsDefaultShooting = false;
                                        Counts = 0;
                                    }
                                }
                                BurstToDo = Random.Range(DistanceBasedShooting[Counts].MinBurstSize, DistanceBasedShooting[Counts].MaxBurstSize);
                                BurstFireUnderProcess = true;
                            }

                        }

                        StartCoroutine(CoroToFindDistanceFire());
                        CheckDistanceWithTarget = true;

                    }
                }
                
            }
          
        }
        IEnumerator CoroForResetBustFire()
        {
            float ResetTimer = Random.Range(DistanceBasedShooting[Counts].MinTimeBetweenBursts, DistanceBasedShooting[Counts].MaxTimeBetweenBursts);
            yield return new WaitForSeconds(ResetTimer);
            TimeToResetBurst = false;
            BurstFireBulletsNumber = 0;
            StartTimerForResetBurst = false;
            BurstFireUnderProcess = false;
        }
        IEnumerator CoroToFindDistanceFire()
        {
            float ResetTimer = Random.Range(MinTargetDistanceCheckInterval, MaxTargetDistanceCheckInterval);
            yield return new WaitForSeconds(ResetTimer);
            CheckDistanceWithTarget = false;
        }
        public void BurstFire(int Bullets)
        {
            if (BurstFireBulletsNumber < Bullets)
            {
                if (CheckForFirstShot == false)
                {
                    TimeToShoot = Random.Range(ShootingFeatures.MinOpenFireDelay, ShootingFeatures.MaxOpenFireDelay);
                    StartCoroutine(IsTimerCompleted());
                    CheckForFirstShot = true;
                }

                if (TimerCompleted == true)
                {
                    AddBulletSpread();
                    if (transform != null)
                    {
                        transform.localRotation = Quaternion.Euler(TargetX, TargetY, 0f);
                    }

                    if (Components.CoreAiBehaviourScript.IsCrouched == false)
                    {

                        // Keep this line to be commented so that distance based shooting works properly as before when it was uncommented reload and the stand weapon recoil animator layer weight were intruppting
                        // each other which makes the reloading more weird.
                        //if(Reload.MagazineSize >= 1)
                        //{
                        //    Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 0f);
                        //    Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(4, 1f, false);
                        //}


                        if (CanShootNow == false)
                        {
                            RandomiseStandFireAiming = StoreStandFiring;
                            Components.CoreAiBehaviourScript.SetAnimationForFullBody(Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.FireParameterName);
                            CanShootNow = true;
                        }
                    }
                    else
                    {

                        // Keep this line to be commented so that distance based shooting works properly as before when it was uncommented reload and the stand weapon recoil animator layer weight were intruppting
                        // each other which makes the reloading more weird.
                        //if (Reload.MagazineSize >= 1)
                        //{
                        //    Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 0f);
                        //    Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(4, 1f, false);
                        //}

                        if (CanShootNow == false)
                        {
                            RandomiseStandFireAiming = RandomiseCrouchFireAiming;
                            Components.CoreAiBehaviourScript.SetAnimationForFullBody(Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.CrouchFireParameterName);
                            CanShootNow = true;
                        }
                    }


                    // ShootingFeatures.shooting = "Shoot";
                    PlayingFiringAnimation = true;
                    nexttimetofire = Time.time + 1f / ShootingFeatures.FireRate;
                    Shoot();

                    MuzzleMeshFunction();
                    NextShotDelay = true;
                    StopTimer = false;
                    ++BurstFireBulletsNumber;
                    //if (BurstFireBulletsNumber < Bullets)
                    //{
                    //    TimeToResetBurst = true;
                    //    NextShotDelay = true;
                    //    StopTimer = false;
                    //}
                }
            }
            else
            {
                TimeToResetBurst = true;
                NextShotDelay = true;
                CanShootNow = true;
                StopTimer = false;
            }

        }
        IEnumerator IsTimerCompleted()
        {
            yield return new WaitForSeconds(TimeToShoot);
            TimerCompleted = true;
        }
        public void RotateZOnShot()
        {
            Vector3 ZRot = ShootingFeatures.MuzzleFlashMesh.transform.localEulerAngles;
            ZRot.z = Random.Range(ShootingFeatures.MinMuzzleFlashMeshZRotation, ShootingFeatures.MaxMuzzleFlashMeshZRotation);
            ShootingFeatures.MuzzleFlashMesh.transform.localEulerAngles = ZRot;
        }
        public void SpawningBulletShell()
        {
            if(pooler != null)
            {
                pooler.SpawnFromPool(ShootingFeatures.BulletShellName, ShootingFeatures.BulletShellSpawnPoint.position, ShootingFeatures.BulletShellSpawnPoint.rotation);
            }
        }
        IEnumerator StartAlertSoundActivationTime()
        {
            float Randomise = Random.Range(MinShotAlertOptimizerDelay, MaxShotAlertOptimizerDelay);
            yield return new WaitForSeconds(Randomise);
            if(DoNotActivateSoundCoordinate == true && AgentWhoActivatedAlertingSoundGameObject == null)
            {
                DoNotActivateSoundCoordinate = false;
                if (Components.DeactivateOtherAgentsAlertingSoundsGameOject != null)
                {
                    Components.DeactivateOtherAgentsAlertingSoundsGameOject.gameObject.SetActive(true);
                }
                StartActivateSoundCoroutine = false;
            }         
        }
        public void DoNotActivateSoundCoordinateGameObject()
        {
            if(OptimizeShotAlertSound == true)
            {
                if (DoNotActivateSoundCoordinate == false)
                {
                    Components.WeaponSounds.ActivateNoiseHandler(transform.root);
                    if (Components.DeactivateOtherAgentsAlertingSoundsGameOject != null)
                    {
                        Components.DeactivateOtherAgentsAlertingSoundsGameOject.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (DoNotActivateSoundCoordinate == true)
                    {
                        if (AgentWhoActivatedAlertingSoundGameObject == null)
                        {
                            if (StartActivateSoundCoroutine == false)
                            {
                                StartCoroutine(StartAlertSoundActivationTime());
                                StartActivateSoundCoroutine = true;
                            }
                        }
                    }
                }
            }         
        }
        public void Shoot()
        {
            if(ShootingFeatures.ShouldEjectBulletShell == true)
            {
                SpawningBulletShell();
            }

            DoNotActivateSoundCoordinateGameObject();

            SimilarFunctionality();
            if (ShootingFeatures.ShootingOption == ShootingOptions.RaycastShooting)
            {
                if (ShootingFeatures.OptimizeRaycastShooting == true)
                {
                    if (IsObjectStored == true)
                    {
                        if (PreviousTargetName != Components.CoreAiBehaviourScript.FindEnemiesScript.enemy[Components.CoreAiBehaviourScript.FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<Targets>().AutoUniqueIdentity)
                        {
                            SavePreviousTarget = false;
                            CurrentBulletShotsCounts = ShootingFeatures.OptimizedRaycastShots;
                            NewTargetToShoot = false;
                        }
                        if (SavePreviousTarget == false)
                        {
                            PreviousTargetName = Components.CoreAiBehaviourScript.FindEnemiesScript.enemy[Components.CoreAiBehaviourScript.FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<Targets>().AutoUniqueIdentity;
                            SavePreviousTarget = true;                           
                        } 
                    }
                }
                   
                if (ShootingFeatures.OptimizeRaycastShooting == true)
                {
                    if(CurrentBulletShotsCounts >= ShootingFeatures.OptimizedRaycastShots)
                    {
                        IsObjectStored = false;
                        NewTargetToShoot = false;
                        if (Physics.Raycast(transform.position, transform.forward, out ShootingFeatures.hit, ShootingFeatures.RaycastBulletRange))//,ShootingFeatures.LayersToIgnore))
                        {
                           // StoredObject = ShootingFeatures.hit;
                            HitObject();
                            //Objshooting = "RaycastWorking";
                            //Objshooting = ShootingFeatures.hit.transform.name;
                            
                        }
                    }
                    else
                    {
                        if(IsObjectStored == true)
                        {
                            ++CurrentBulletShotsCounts;
                            HitObject();
                        }
                       
                    }
                }
                else
                {
                    if (Physics.Raycast(transform.position, transform.forward, out ShootingFeatures.hit, ShootingFeatures.RaycastBulletRange))//,ShootingFeatures.LayersToIgnore))
                    {
                        HitObject();
                    }
                }
               
                //else
                //{
                //    Objshooting = "RaycastNOTWorking";
                //}
            }
            else
            {
                if (Physics.Raycast(transform.position, transform.forward, out ShootingFeatures.hit, ShootingFeatures.RaycastBulletRange))
                {
                    if (ShootingFeatures.hit.transform.root.tag != "Player" ||
                        ShootingFeatures.hit.transform.root.gameObject.tag == "AI"
                        && ShootingFeatures.hit.transform.tag != "WeakPoint" && ShootingFeatures.hit.transform.root.GetComponent<Targets>().MyTeamID != nametolookfor
                        || ShootingFeatures.hit.transform.root.GetComponent<Targets>() != null && ShootingFeatures.hit.transform.root.GetComponent<Targets>().MyTeamID != nametolookfor)
                    {
                        IsFiring = false;
                    }
                }
            }
        }
        public void ResetOptimizedShooting()
        {
            if (ShootingFeatures.OptimizeRaycastShooting == true)
            {
                if(NewTargetToShoot == false)
                {
                    CurrentBulletShotsCounts = 0;
                    IsObjectStored = true;
                    SavePreviousTarget = false;
                    NewTargetToShoot = true;
                }  
            }
        }
        public void HitObject()
        {
            if(ShootingFeatures.hit.collider != null)
            {
                if (ShootingFeatures.hit.transform.root.tag == "Player")
                {
                    // GivingfriendlyBotPosOnShotHeard();
                    IsFiring = true;
                    PlayerHealth.instance.PlayerHealthbar.Curvalue -= ShootingFeatures.DamageToTarget;
                    PlayerHealth.instance.CheckPlayerHealth();
                    ResetOptimizedShooting();

                    if (TeamMatch.instance != null)
                    {
                        if (TeamMatch.instance.EnableScoreSystemBetweenTeamsAsWinCondition == true)
                        {
                            if (PlayerHealth.instance.PlayerHealthbar.Curvalue <= 0)
                            {
                                if (UpdateKills == false)
                                {
                                    TeamMatch.instance.Teams[StoreTeamId].Kills += 1;
                                    UpdateKills = true;
                                }
                            }
                            else
                            {
                                TeamMatch.instance.Teams[StoreTeamId].TeamScore += TeamMatch.instance.SingleShotPoints;
                                TeamMatch.instance.Teams[StoreTeamId].ScoreText.text = TeamMatch.instance.Teams[StoreTeamId].TeamName + " - " + TeamMatch.instance.Teams[StoreTeamId].TeamScore;
                                UpdateKills = false;
                            }
                        }
                    }


                }
                else if (ShootingFeatures.hit.transform.root.tag == "Turret")
                {
                    if (ShootingFeatures.hit.transform.root.GetComponent<Turret>() != null)
                    {
                        ShootingFeatures.hit.transform.root.GetComponent<Turret>().TakeDamage(ShootingFeatures.DamageToTarget);
                    }
                }
                else if (ShootingFeatures.hit.transform.root.gameObject.tag == "AI" && ShootingFeatures.hit.transform.root.GetComponent<Targets>().MyTeamID != nametolookfor)
                {
                    if (ShootingFeatures.hit.transform.tag == "WeakPoint")
                    {
                        // Debug.Log(transform.name + "Target" + " " + ShootingFeatures.hit.transform.root.name);
                        //GivingfriendlyBotPosOnShotHeard();
                        IsFiring = true;
                        ShootingFeatures.hit.collider.gameObject.transform.root.SendMessage("FindColliderName", ShootingFeatures.hit.collider.name, SendMessageOptions.DontRequireReceiver);
                        ShootingFeatures.hit.collider.gameObject.transform.root.SendMessage("WeakPointdamage", ShootingFeatures.DamageToTarget, SendMessageOptions.DontRequireReceiver);
                        // Debug.Break();
                        if (Components.CoreAiBehaviourScript != null)
                        {
                            ShootingFeatures.hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.WhoKilledMe = transform.root;
                            ShootingFeatures.hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.WhoShootingMe = transform.root;
                            if (Components.CoreAiBehaviourScript.HealthScript.IsDied == true)
                            {
                                ShootingFeatures.hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.DoNotActivateSoundTrigger = true;
                            }
                        }
                        else
                        {
                            ShootingFeatures.hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.DoNotActivateSoundTrigger = true;
                        }
                        ShootingFeatures.hit.collider.gameObject.transform.root.SendMessage("Effects", ShootingFeatures.hit, SendMessageOptions.DontRequireReceiver);
                        PointsChecker();
                    }
                    else
                    {
                        //GivingfriendlyBotPosOnShotHeard();
                        IsFiring = true;
                        ShootingFeatures.hit.collider.gameObject.transform.root.SendMessage("FindColliderName", ShootingFeatures.hit.collider.name, SendMessageOptions.DontRequireReceiver);
                        ShootingFeatures.hit.collider.gameObject.transform.root.SendMessage("Takedamage", ShootingFeatures.DamageToTarget, SendMessageOptions.DontRequireReceiver);
                        // Debug.Break();
                        if (Components.CoreAiBehaviourScript != null)
                        {
                            if(ShootingFeatures.hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
                            {
                                ShootingFeatures.hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.WhoKilledMe = transform.root;
                                ShootingFeatures.hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.WhoShootingMe = transform.root;
                                if (Components.CoreAiBehaviourScript.HealthScript.IsDied == true)
                                {
                                    ShootingFeatures.hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.DoNotActivateSoundTrigger = true;
                                }
                            }                         
                        }
                        else
                        {
                            ShootingFeatures.hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.DoNotActivateSoundTrigger = true;
                        }

                        ShootingFeatures.hit.collider.gameObject.transform.root.SendMessage("Effects", ShootingFeatures.hit, SendMessageOptions.DontRequireReceiver);
                        //ShootingFeatures.hit.collider.gameObject.transform.root.SendMessage("RemoveFromListOfEnemies", transform.root, SendMessageOptions.DontRequireReceiver);
                        PointsChecker();
                    }

                    //Debug.Log("Enemy Name" + ShootingFeatures.hit.transform.root.name); 
                    //// Add debug logs to check hit point and AI position update
                    //Debug.Log("Hit Point: " + ShootingFeatures.hit.point);
                    //Debug.Log("AI Position: " + transform.root.position);

                    //// Draw debug lines to visualize hit point and AI position
                    //Debug.DrawLine(transform.root.position, ShootingFeatures.hit.point, Color.blue); // AI position to hit point
                    //Debug.DrawLine(ShootingFeatures.hit.point, ShootingFeatures.hit.point + ShootingFeatures.hit.normal, Color.green); // Hit point normal


                    ResetOptimizedShooting();
                   
                }
                else if (ShootingFeatures.hit.collider.gameObject.transform.root.tag != "Player" && ShootingFeatures.hit.collider.gameObject.transform.root.tag != "AI" && ShootingFeatures.hit.collider.gameObject.tag != "WeakPoint")
                {
                    IsFiring = false;
                    if (ShootingFeatures.hit.collider.gameObject.GetComponent<ImpactEffectSpawner>() != null)
                    {
                        //Vector3 p = new Vector3(hit.point.x + ImpactEffectOffsetValue, hit.point.y + ImpactEffectOffsetValue, hit.point.z + ImpactEffectOffsetValue);                   
                        GameObject impacteffect = Instantiate(ShootingFeatures.hit.collider.gameObject.GetComponent<ImpactEffectSpawner>().HitEffectPrefab, ShootingFeatures.hit.point, Quaternion.LookRotation(ShootingFeatures.hit.normal));
                        ShootingFeatures.hit.collider.gameObject.GetComponent<ImpactEffectSpawner>().PlaySound();

                        if (impacteffect.gameObject.GetComponent<ImpactEffect>() != null)
                        {
                            if (Components.CoreAiBehaviourScript != null)
                            {
                                if (Components.CoreAiBehaviourScript.HealthScript.IsDied == true)
                                {
                                    impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.DoNotActivateEffect = true;
                                }
                            }
                            else
                            {
                                impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.DoNotActivateEffect = true;
                            }

                            // Keep the below lines to be commented  (line number:1334,1338) because during emergency state based on the selected properties it should affect the other Humanoid AI agent and not only friendlies
                            // as when you overrite the properties it affects the behaviour when you change promatically that only friendlies will be affected using below code.
                            // This may create a bug when you shoot the enemies and impact effects
                            // are spawned with the settings being force emergency state on AI agent.so Make sure you don't use this code at all even in the case of sounds. for more details why not to use you can change
                            // impact effect properties to be Force Emergency state and choose All teams and shoot near the enemies using player weapon by uncommenting this code.

                            //if(impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript != null)
                            //{
                            //    impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.Enemy = transform.root;
                            //}
                            //impacteffect.gameObject.GetComponent<ImpactEffect>().TeamWhichWillBeAffectedByTheShot(Components.CoreAiBehaviourScript.T.MyTeamID);
                            impacteffect.gameObject.GetComponent<ImpactEffect>().EffectActivation(transform.root);
                        }

                        //if (Components.CoreAiBehaviourScript != null)
                        //{
                        //    if (Components.CoreAiBehaviourScript.HealthScript.IsDied == false)
                        //    {
                        //        impacteffect.gameObject.GetComponent<ImpactEffect>().TeamMadeTheShot(Components.CoreAiBehaviourScript.T.MyTeamTag);
                        //    }
                        //    else
                        //    {
                        //        impacteffect.gameObject.GetComponent<ImpactEffect>().TeamMadeTheShot("AvoidTheLastShotOfBullet");
                        //        // If the enemy is dead than give random team tag to avoid making other Ai agent who might get into
                        //        // As enemy is dead the bullet could still fly and before that there could be possibility that other Ai agent get into non combat behaviour and
                        //        // activates the emergency state.
                        //    }
                        //}

                        // the problem when you make it parent is that if that parent is supposed to be destroy ( for example grenade explosion gameobject )
                        // in the game it destroy the effect which was being used by the object pooler due to which the condition becomes null and throws error 
                        //if (impacteffect != null)
                        //{
                        //    impacteffect.transform.parent = ShootingFeatures.hit.transform;
                        //}
                    }
                    else
                    {
                        if (pooler != null)
                        {
                            GameObject impacteffect = pooler.SpawnFromPool(ShootingFeatures.ImpactEffectName, ShootingFeatures.hit.point, Quaternion.LookRotation(ShootingFeatures.hit.normal));

//                            Debug.Log(ShootingFeatures.hit.transform.name);

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
                                    if (Components.CoreAiBehaviourScript != null)
                                    {
                                        if (Components.CoreAiBehaviourScript.HealthScript.IsDied == true)
                                        {
                                            impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.DoNotActivateEffect = true;
                                        }
                                    }
                                    else
                                    {
                                        impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.DoNotActivateEffect = true;
                                    }

                                    // Keep the below lines to be commented  (line number:1334,1338) because during emergency state based on the selected properties it should affect the other Humanoid AI agent and not only friendlies
                                    // as when you overrite the properties it affects the behaviour when you change promatically that only friendlies will be affected using below code.
                                    // This may create a bug when you shoot the enemies and impact effects
                                    // are spawned with the settings being force emergency state on AI agent.so Make sure you don't use this code at all even in the case of sounds. for more details why not to use you can change
                                    // impact effect properties to be Force Emergency state and choose All teams and shoot near the enemies using player weapon by uncommenting this code.

                                    //if(impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript != null)
                                    //{
                                    //    impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.Enemy = transform.root;
                                    //}
                                    //impacteffect.gameObject.GetComponent<ImpactEffect>().TeamWhichWillBeAffectedByTheShot(Components.CoreAiBehaviourScript.T.MyTeamID);

                                   
                                    impacteffect.gameObject.GetComponent<ImpactEffect>().EffectActivation(transform.root);
                                }

                                // the problem when you make it parent is that if that parent is supposed to be destroy ( for example grenade explosion gameobject )
                                // in the game it destroy the effect which was being used by the object pooler due to which the condition becomes null and throws error 
                                //impacteffect.transform.parent = ShootingFeatures.hit.transform;
                            }

                        }
                    }

                    //if (ShootingFeatures.hit.collider.gameObject.GetComponent<HitImpactEffect>() != null)
                    //{
                    //    if (ShootingFeatures.hit.collider.gameObject.GetComponent<HitImpactEffect>().DeactivateBulletOnCollision == false)
                    //    {
                    //        if (ShootingFeatures.hit.collider.GetComponent<MeshCollider>() != null)
                    //        {
                    //            ShootingFeatures.hit.collider.GetComponent<MeshCollider>().convex = true;
                    //            ShootingFeatures.hit.collider.isTrigger = true;
                    //        }
                    //        else
                    //        {
                    //            ShootingFeatures.hit.collider.isTrigger = true;
                    //        }
                    //        StoreHit = ShootingFeatures.hit;
                    //        if (ShootingFeatures.hit.collider.gameObject.GetComponent<HitImpactEffect>().SpawnImpactEffectOnSurfacesBehindThisOne == false)
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
                if (RaycastBulletForce.AddRaycastForce == true)
                {
                    AddForceToRigidBodies(ShootingFeatures.hit, transform);
                }
            }
           
        }
        public IEnumerator ReloadTimer()
        {
            isreloading = true;
            //if (ShakeSpine == true)
            //{
            //    Components.AnimatorComponent.SetLayerWeight(2, 0f);
            //    Components.AnimatorComponent.SetLayerWeight(3, 0f);
            //    ShakeSpine = false;
            //}

            // added this code so that upper body 'CrouchAimingParameter' do not play any animation except crouch reload so do not remove it. this line was added after checking stationed crouched shooting and take covers (crouch firing covers) both.
            Components.CoreAiBehaviourScript.anim.SetBool(Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.CrouchAimingParameterName,false);

            Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(4, 0f);
            Components.CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 0f);

            Components.CoreAiBehaviourScript.Speeds.AnimationSpeeds.ReloadAnimationSpeed = OriginalReloadSpeed;
            //Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.FireAnimationSpeed = 0f;

            //if (ShootingFeatures.EnableWeaponRecoil == true && StopWeaponRecoil == false)
            //{
            //    Components.AnimatorComponent.SetLayerWeight(ShootingFeatures.WeaponRecoilAnimatorLayerIndex, 0f);
            //    Components.AnimatorComponent.SetBool(ShootingFeatures.WeaponRecoilAnimatorParameterName, false);
            //}

            //if (StoreOriginalTransform == true)
            //{
            //    // reset position and rotation to original position and rotation
            //    SpineBone.localPosition = originalPos;
            //    SpineBone.localRotation = originalRot;
            //}
            Components.CoreAiBehaviourScript.Components.HumanoidAiAudioPlayerComponent.PlayReloadSound();
            Components.CoreAiBehaviourScript.Components.HumanoidAiAudioPlayerComponent.PlayNonRecurringSoundClips(Components.CoreAiBehaviourScript.Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceReloadingAudioClips);

            //if (ShootingFeatures.UseMeshMuzzleFlash == true)
            //{
            //    ShootingFeatures.MuzzleMesh.SetActive(false);
            //}
            //if (gameObject.transform.root.GetComponent<MasterAiBehaviour>() != null)
            //{
            //    if (gameObject.transform.root.GetComponent<MasterAiBehaviour>().Reachnewpoints == true)
            //    {
            //        if (gameObject.transform.root.GetComponent<MasterAiBehaviour>().CrouchPositions[gameObject.transform.root.GetComponent<MasterAiBehaviour>().CurrentCoverPoint].transform.GetComponent<CoverNode>().StandingCover == true)
            //        {
            //            if (Reload.ChooseAutoReloadTime == true)
            //            {
            //                float ActualReloadTime = StandAutoReloadTime / Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.ReloadAnimationSpeed;
            //                yield return new WaitForSeconds(ActualReloadTime);
            //                Reload.MagazineSize = Reload.MaxAmmoAfterReload;
            //                Reload.MaxAmmo = Reload.MaxAmmo - Reload.MaxAmmoAfterReload;

            //            }
            //            else
            //            {
            //                float ActualReloadTime = Reload.StandReloadTime / Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.ReloadAnimationSpeed;
            //                yield return new WaitForSeconds(ActualReloadTime);
            //                Reload.MagazineSize = Reload.MaxAmmoAfterReload;
            //                Reload.MaxAmmo = Reload.MaxAmmo - Reload.MaxAmmoAfterReload;

            //            }
            //        }
            //        else
            //        {
            //            if (Reload.ChooseAutoReloadTime == true)
            //            {
            //                float ActualReloadTime = CrouchAutoReloadTime / Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.CrouchReloadAnimationSpeed;
            //                yield return new WaitForSeconds(ActualReloadTime);
            //                Reload.MagazineSize = Reload.MaxAmmoAfterReload;
            //                Reload.MaxAmmo = Reload.MaxAmmo - Reload.MaxAmmoAfterReload;

            //            }
            //            else
            //            {
            //                float ActualReloadTime = Reload.CrouchReloadTime / Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.CrouchReloadAnimationSpeed;
            //                yield return new WaitForSeconds(ActualReloadTime);
            //                Reload.MagazineSize = Reload.MaxAmmoAfterReload;
            //                Reload.MaxAmmo = Reload.MaxAmmo - Reload.MaxAmmoAfterReload;

            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (Reload.ChooseAutoReloadTime == true)
            //        {
            //            float ActualReloadTime = StandAutoReloadTime / Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.ReloadAnimationSpeed;
            //            yield return new WaitForSeconds(ActualReloadTime);
            //            Reload.MagazineSize = Reload.MaxAmmoAfterReload;
            //            Reload.MaxAmmo = Reload.MaxAmmo - Reload.MaxAmmoAfterReload;

            //        }
            //        else
            //        {
            //            float ActualReloadTime = Reload.StandReloadTime / Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.ReloadAnimationSpeed;
            //            yield return new WaitForSeconds(ActualReloadTime);
            //            Reload.MagazineSize = Reload.MaxAmmoAfterReload;
            //            Reload.MaxAmmo = Reload.MaxAmmo - Reload.MaxAmmoAfterReload;

            //        }
            //    }
            //}

            if (Components.CoreAiBehaviourScript.IsCrouched == false)
            {
                if (Reload.MatchReloadTimeWithAnimation == true)
                {
                    float ActualReloadTime = StandAutoReloadTime / Components.CoreAiBehaviourScript.Speeds.AnimationSpeeds.ReloadAnimationSpeed;
                    yield return new WaitForSeconds(ActualReloadTime);
                    Reload.MagazineSize = Reload.MaxAmmoAfterReload;
                    Reload.MaxAmmo = Reload.MaxAmmo - Reload.MaxAmmoAfterReload;

                }
                else
                {
                    float ActualReloadTime = Reload.StandReloadDuration / Components.CoreAiBehaviourScript.Speeds.AnimationSpeeds.ReloadAnimationSpeed;
                    yield return new WaitForSeconds(ActualReloadTime);
                    Reload.MagazineSize = Reload.MaxAmmoAfterReload;
                    Reload.MaxAmmo = Reload.MaxAmmo - Reload.MaxAmmoAfterReload;
                }
            }
            else
            {
                if (Reload.MatchReloadTimeWithAnimation == true)
                {
                    float ActualReloadTime = CrouchAutoReloadTime / Components.CoreAiBehaviourScript.Speeds.AnimationSpeeds.CrouchReloadAnimationSpeed;
                    yield return new WaitForSeconds(ActualReloadTime);
                    Reload.MagazineSize = Reload.MaxAmmoAfterReload;
                    Reload.MaxAmmo = Reload.MaxAmmo - Reload.MaxAmmoAfterReload;

                }
                else
                {
                    float ActualReloadTime = Reload.CrouchedReloadDuration / Components.CoreAiBehaviourScript.Speeds.AnimationSpeeds.CrouchReloadAnimationSpeed;
                    yield return new WaitForSeconds(ActualReloadTime);
                    Reload.MagazineSize = Reload.MaxAmmoAfterReload;
                    Reload.MaxAmmo = Reload.MaxAmmo - Reload.MaxAmmoAfterReload;
                }
            }


            Components.CoreAiBehaviourScript.Speeds.AnimationSpeeds.ReloadAnimationSpeed = 0f;
            // Components.CoreAiBehaviourScript.AiAgentAnimatorParameters.FireAnimationSpeed = OriginalFireAnimationSpeed;
            yield return new WaitForSeconds(0.01f);
            IsWeaponReloading = false;
            StopShoot = false;
            StoreReloadDelay = false;
            ReloadDelayCompleted = false;
            isreloading = false;
            CheckForFirstShot = false;

            //Reseting Optimised shooting
            if(ShootingFeatures.OptimizeRaycastShooting == true)
            {
                SavePreviousTarget = false;
                CurrentBulletShotsCounts = ShootingFeatures.OptimizedRaycastShots;
                NewTargetToShoot = false;
            }
            // Components.CoreAiBehaviourScript.anim.SetBool("StandShootPosture", true); // So making it true was causing sprinting behaviour go weird when Ai agent is far away from its enemy ( sprinting ) in stop and shoot behaviour 
        }
        public void CreateProjectiles()
        {
            if (ShootingFeatures.ShootingOption == ShootingOptions.ProjectileShooting)
            {
                if (ShootingFeatures.ProjectilesPerShot >= 2)
                {
                    for (int x = 0; x < ShootingFeatures.ProjectilesPerShot; x++)
                    {
                        if (pooler != null)
                        {
                            //  GameObject bullet = new GameObject();
                            //if (bullet != null)
                            //{
                            AddBulletSpread();
                            GameObject bullet = pooler.SpawnFromPool(ShootingFeatures.ProjectileName, transform.position, transform.rotation);
                            BulletScript B = bullet.GetComponent<BulletScript>();
                            B.Movement(transform, transform.root, IsProjectileShootingActive);
                            //}
                        }

                    }
                }
                else
                {
                    if (pooler != null)
                    {
                        //  GameObject bullet = new GameObject();
                        //if(bullet != null)
                        //{
                        GameObject bullet = pooler.SpawnFromPool(ShootingFeatures.ProjectileName, transform.position, transform.rotation);
                        BulletScript B = bullet.GetComponent<BulletScript>();
                        B.Movement(transform, transform.root, IsProjectileShootingActive);
                        //}
                    }
                }
            }

        }
        public void SimilarFunctionality()
        {
            if (ShootingFeatures.UseMuzzleFlashMesh == false)
            {
                if (ShootingFeatures.MuzzleFlashParticle != null)
                {
                    if (ShootingFeatures.MuzzleFlashParticle.isPlaying == false)
                        ShootingFeatures.MuzzleFlashParticle.Play();

                }
            }
            // AnimatorComponent.SetBool("Fire", true);
            //  AnimatorComponent.Play("Firing Rifle", -1, 0f);
            RotateZOnShot();
            CreateProjectiles();
            Components.CoreAiBehaviourScript.Components.HumanoidAiAudioPlayerComponent.PlayFiringSound();
            IFired = true;
            Reload.MagazineSize--;
        }
        public void PointsChecker()
        {
            if (TeamMatch.instance != null)
            {
                if (TeamMatch.instance.EnableScoreSystemBetweenTeamsAsWinCondition == true)
                {
                    if (ShootingFeatures.hit.transform.root.GetComponent<HumanoidAiHealth>().IsDied == true)
                    {
                        if (UpdateKills == false)
                        {
                            TeamMatch.instance.Teams[StoreTeamId].Kills += 1;
                            UpdateKills = true;
                        }
                    }
                    else
                    {
                        TeamMatch.instance.Teams[StoreTeamId].HeadshotsTaken += 1;
                        TeamMatch.instance.Teams[StoreTeamId].TeamScore += TeamMatch.instance.HeadShotPoints;
                        TeamMatch.instance.Teams[StoreTeamId].ScoreText.text = TeamMatch.instance.Teams[StoreTeamId].TeamName + " - " + TeamMatch.instance.Teams[StoreTeamId].TeamScore;
                        UpdateKills = false;
                    }
                }
            }

        }
        public void DissapearGun()
        {
            StartCoroutine(TimeToDestroyWeapon());
        }
        IEnumerator TimeToDestroyWeapon()
        {
            yield return new WaitForSeconds(DroppedWeaponLifeTime);
            Destroy(Components.WeaponMesh.gameObject);
        }
        IEnumerator MuzzleMeshTimer()
        {
            yield return new WaitForSeconds(ShootingFeatures.MuzzleFlashDuration);
            ShootingFeatures.MuzzleFlashMesh.SetActive(false);
            StartMuzzleMeshTimer = false;
        }

        public void FindTargetAI()
        {
            if(LineOfFireChecks.EnableLineOfFireChecks == true)
            {
                Targets[] AllTargets = FindObjectsOfType<Targets>();

                foreach (Targets aiObject in AllTargets)
                {
                    Targets aiBehaviour = aiObject.GetComponent<Targets>();
                    if (aiBehaviour != null)
                    {
                        if (aiBehaviour.MyTeamID == nametolookfor && aiBehaviour.transform != this.transform.root)
                        {
                            if (!targetAIList.Contains(aiObject.transform))
                            {
                                targetAIList.Add(aiObject.transform);
                            }
                        }
                    }
                }
            } 
        }
        //private bool IsWithinFieldOfView(Transform targetAI)
        //{
        //    Vector3 toAI = targetAI.position - Components.CoreAiBehaviourScript.transform.position;
        //    float angle = Vector3.Angle(Components.CoreAiBehaviourScript.transform.forward, toAI);

        //    //Debug.Log("Calculated angle: " + angle);
        //    //Debug.Log("Field of View angle threshold: " + (ShootingFieldOfViewSight * 0.5f));

        //    return angle <= EnhancedFriendlyFireAvoidance.FriendlyFireAvoidanceFov * 0.5f;
        //}
        //private bool CheckXDistance(Transform targetAI)
        //{
        //    // Convert targetAI's position to the local space of CoreAiBehaviourScript
        //    Vector3 localPosition = Components.CoreAiBehaviourScript.transform.InverseTransformPoint(targetAI.position);

        //    // Calculate the horizontal distance on the x-axis in local space
        //    float horizontalDistance = Mathf.Abs(localPosition.x);

        //    if(EnhancedFriendlyFireAvoidance.DebugXDistance == true)
        //    {
        //       EnhancedFriendlyFireAvoidance.DisplayDistanceCheckAtXWithFriendly = horizontalDistance;
        //    }
        //    // Check if the horizontal distance is within 3 meters
        //    if (horizontalDistance <= EnhancedFriendlyFireAvoidance.DistanceCheckAtX)
        //    {             
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        private bool CheckXAndYDistances(Transform targetAI)
        {
            // Convert targetAI's position to the local space of CoreAiBehaviourScript
            Vector3 localPosition = Components.CoreAiBehaviourScript.transform.InverseTransformPoint(targetAI.position);
           
            // Calculate the horizontal distance on the x-axis in local space
            float horizontalDistance = Mathf.Abs(localPosition.x);
            // Calculate the vertical distance on the y-axis in local space
            float verticalDistance = Mathf.Abs(localPosition.y);

            if (LineOfFireChecks.DebugProximityToLineOfFire == true)
            {
                LineOfFireChecks.DisplayXAxisProximity = horizontalDistance;
                LineOfFireChecks.DisplayYAxisProximity = verticalDistance;
            }

            // Check if the target is in front (localPosition.z > 0) 
            // and the horizontal distance is within the specified limit or the vertical distance is greater than 0.7 

           

            if (Components.CoreAiBehaviourScript != null)
            {
                if (Components.CoreAiBehaviourScript.IsCrouched == true)
                {
                    if (localPosition.z <= 0)
                    {
                        return true;
                    }
                    else if (localPosition.z > 0 && horizontalDistance >= LineOfFireChecks.LineOfFireProximityX && localPosition.z > 0 && verticalDistance >= LineOfFireChecks.LineOfFireProximityY
                        || localPosition.z > 0 && horizontalDistance >= LineOfFireChecks.LineOfFireProximityX)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (localPosition.z <= 0)
                    {
                        return true;
                    }
                    else if (localPosition.z > 0 && horizontalDistance >= LineOfFireChecks.LineOfFireProximityX || localPosition.z > 0 && verticalDistance >= LineOfFireChecks.LineOfFireProximityY)
                    {
                        return true;
                    }
                    else if (targetAI.GetComponent<CoreAiBehaviour>() != null)
                    {
                        if (targetAI.GetComponent<CoreAiBehaviour>().IsCrouched == true)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            else  
            {
                return false;
            }
           
        }





        //private bool IsWithinDistance(Transform targetAI)
        //{
        //    return Vector3.Distance(Components.CoreAiBehaviourScript.transform.position, targetAI.position) <= EnhancedFriendlyFireAvoidance.FriendlyFireAvoidanceRange;
        //}
        private Transform FindClosestFriendlyAI()
        {
            Transform closestAI = null;
            float closestDistance = Mathf.Infinity;

            foreach (Transform targetAI in targetAIList)
            {
                if (targetAI != null)
                {
                    float distance = Vector3.Distance(transform.position, targetAI.position);

                    if (targetAI.GetComponent<CoreAiBehaviour>() != null)
                    {
                        if(targetAI.GetComponent<CoreAiBehaviour>().HealthScript != null)
                        {
                            if (targetAI.GetComponent<CoreAiBehaviour>().HealthScript.IsDied == false)
                            {
                                if (distance < closestDistance)
                                {
                                    closestDistance = distance;
                                    closestAI = targetAI;
                                }
                            }
                        }
                     
                    }
                   
                }
            }

            return closestAI;
        }
        public void AddForceToRigidBodies(RaycastHit hit, Transform RaycastToStartFrom)
        {

           // Vector3 directionToTarget = hit.transform.position - RaycastToStartFrom.position; // Grenade -> Target direction
           // Debug.DrawLine(RaycastToStartFrom.position, RaycastToStartFrom.position + RaycastToStartFrom.forward * directionToTarget.magnitude, Color.red, 0.1f);

            if (hit.transform.root.tag == "AI" && !RaycastBulletForce.TargetsToApplyForce.Contains(hit.transform.root)) // AI Logic
            {
                if (hit.transform.root.GetComponent<HumanoidAiHealth>() != null)
                {
                    if (hit.transform.root.GetComponent<HumanoidAiHealth>().Health <= 0)
                    {
                        if (hit.transform.root.GetComponent<Animator>() != null)
                        {
                            hit.transform.root.GetComponent<Animator>().enabled = false;
                        }

                        RaycastBulletForce.TargetsToApplyForce.Add(hit.transform.root);
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
                                float distanceFactor = Mathf.Clamp01(1 - (distanceToGrenade / RaycastBulletForce.RadiusToApplyForce)); // Normalize effect
                                float forceMagnitude = Mathf.Lerp(RaycastBulletForce.AIMinExplosiveForce, RaycastBulletForce.AIMaxExplosiveForce, distanceFactor);

                                // Apply the force
                                rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);

                                // Debugging information
                                Debug.Log($"{g.gameObject.name} - Closest Direction: {closestDirection}, Force Applied: {forceDirection}, Magnitude: {forceMagnitude}");
                            }
                        }
                    }
                }

            }
            else if (hit.transform.root.tag == "Player" && RaycastBulletForce.PlayerExplosiveForce == PlayerForce.ShakePlayerCamera)
            {
              
                if (hit.transform.root.GetComponent<CameraShakerEffect>() != null)
                {
                    hit.transform.root.GetComponent<CameraShakerEffect>().Shake();
                }
            }
            else if (hit.transform.root.tag == "Player"  && RaycastBulletForce.PlayerExplosiveForce == PlayerForce.ApplyForceToPlayerRigidbody)
            {

                if (hit.transform.GetComponent<Rigidbody>() != null)
                {
                   
                    Rigidbody rb = hit.transform.gameObject.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                    rb.AddForce(-hit.normal * RaycastBulletForce.NonAIExplosiveForce, ForceMode.Impulse);
                }

            }
            else if (hit.transform.root.tag == "Player" && RaycastBulletForce.PlayerExplosiveForce == PlayerForce.ApplyForceToPlayerRigidbodyAndShakePlayerCamera)
            {
               
                if (hit.transform.root.GetComponent<CameraShakerEffect>() != null)
                {
                    hit.transform.root.GetComponent<CameraShakerEffect>().Shake();
                    // StartCoroutine(hit.transform.root.GetComponent<CameraShakerEffect>().Shake());
                }
                if (hit.transform.GetComponent<Rigidbody>() != null)
                {

                    Rigidbody rb = hit.transform.gameObject.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                    rb.AddForce(-hit.normal * RaycastBulletForce.NonAIExplosiveForce, ForceMode.Impulse);
                }
            }
            else if (hit.transform.root.tag == "Player" && RaycastBulletForce.PlayerExplosiveForce == PlayerForce.DoNotApplyForceToPlayer)
            {
                  // Don't do anything here 
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
                            rb.AddForce(-hit.normal * RaycastBulletForce.NonAIExplosiveForce, ForceMode.Impulse);
                        }
                    //}
                
            }

        }
        //public void AddingForceToHumanoidAI(RaycastHit hit)
        //{
        //    if (RaycastBulletForce.AddRaycastForce == true)
        //    {
        //        if (hit.transform.root.GetComponent<HumanoidAiHealth>() != null)
        //        {
        //            if (hit.transform.root.GetComponent<HumanoidAiHealth>().Health <= 0)
        //            {
        //                if (hit.transform.root.GetComponent<Animator>() != null)
        //                {
        //                    hit.transform.root.GetComponent<Animator>().enabled = false;
        //                }
        //                foreach (Transform g in hit.transform.gameObject.transform.root.GetComponentsInChildren<Transform>())
        //                {
        //                    Rigidbody rb = g.gameObject.GetComponent<Rigidbody>();
        //                    if (rb != null)
        //                    {
        //                        g.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //                        g.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        //                        //// Calculate the force direction based on the specified offsets in world space
        //                        //Vector3 forceDirectionWorld = Random.Range(RaycastForce.MinUpwardForceToApplyOnTarget, RaycastForce.MaxUpwardForceToApplyOnTarget) * Vector3.up +
        //                        //                            Random.Range(RaycastForce.MinRightForceToApplyOnTarget, RaycastForce.MaxRightForceToApplyOnTarget) * Vector3.right +
        //                        //                            Random.Range(RaycastForce.MinLeftForceToApplyOnTarget, RaycastForce.MaxLeftForceToApplyOnTarget) * Vector3.left +
        //                        //                            Random.Range(RaycastForce.MinBackwardForceToApplyOnTarget, RaycastForce.MaxBackwardForceToApplyOnTarget) * Vector3.back +
        //                        //                            Random.Range(RaycastForce.MinForwardForceToApplyOnTarget, RaycastForce.MaxForwardForceToApplyOnTarget) * Vector3.forward;

        //                        //// Transform the force direction from world space to local space
        //                        //Vector3 forceDirectionLocal = rb.transform.TransformVector(forceDirectionWorld);

        //                        //// Apply force in the calculated local direction
        //                        //rb.AddForce(forceDirectionLocal.normalized * Random.Range(RaycastForce.MinRaycastForce, RaycastForce.MaxRaycastForce));

        //                        // Calculate and apply the upward force in world space
        //                        Vector3 upwardForceWorld = Random.Range(RaycastBulletForce.MinUpwardForce, RaycastBulletForce.MaxUpwardForce) * Vector3.up;
        //                        rb.AddForce(upwardForceWorld * Random.Range(RaycastBulletForce.MinRaycastForce, RaycastBulletForce.MaxRaycastForce));

        //                        // Calculate and apply other forces in local space
        //                        Vector3 forceDirectionLocal = Random.Range(RaycastBulletForce.MinRightForce, RaycastBulletForce.MaxRightForce) * Vector3.right +
        //                                                    Random.Range(RaycastBulletForce.MinLeftForce, RaycastBulletForce.MaxLeftForce) * Vector3.left +
        //                                                    Random.Range(RaycastBulletForce.MinBackwardForce, RaycastBulletForce.MaxBackwardForce) * Vector3.back +
        //                                                    Random.Range(RaycastBulletForce.MinForwardForce, RaycastBulletForce.MaxForwardForce) * Vector3.forward;

        //                        rb.AddRelativeForce(forceDirectionLocal * Random.Range(RaycastBulletForce.MinRaycastForce, RaycastBulletForce.MaxRaycastForce));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

    }
}