using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace MobileActionKit
{
    // This Script is Responsible For The Behaviour of Simple Bots 
    public class CoreAiBehaviour : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This script contains set of Ai agent`s behavioural options. It can be tweaked to create various AI agent types." +
            " The more options you will activate the more computationally expensive resulted Ai agent prefab will become." +
            " However this script allows to produce a multitude of 'cheaper' AI agent variants designed for their specific tasks. ";


        [Tooltip("Use unique names for each of the tweaked AI agents. After renaming and tweaking your current AI agent type create a prefab out of it for future use (with spawners for example).")]
        public string AgentName;
        public enum Role
        {
            Fighter,
            Leader,
            Follower,
            Zombie
        }
        [Tooltip("This list contains 3 default roles: Fighter,Follower,Leader. Fighter AI type has functionality set to fight it`s enemies." +
            " Follower AI type will have Fighter`s functionality and will always follow it`s Leader. Leader is an AI agent that it`s Followers will follow everywhere it moves." +
            " Player can also be set as Leader for AI agents to follow.")]

        public Role AgentRole;

        [Tooltip("Enable this to add a 'Debug info' option in the core Ai behaviour script.")]
        public bool DebugAgentState;

        [System.Serializable]
        public enum GrenadeAlertClass
        {
            [Tooltip("If Chosen AI will move away from nearby landed grenade towards randomly choosen retreat point from the list of available retreat points.")]
            MoveAwayFromLandedGrenades,
            [Tooltip("If Chosen there will be 50/50 chance for Ai to perform 'Run From Grenades' behaviour.")]
            RandomizeMoveFromLandedGrenades,
        }

        [HideInInspector][Tooltip("If enabled AI will move away from nearby landed grenade towards randomly choosen retreat point from the list of available retreat points.")]
        public bool RunFromGrenades = true;
        [HideInInspector][Tooltip("If enabled there will be 50/50 chance for Ai to perform 'Run From Grenades' behaviour.")]
        public bool RandomiseRunFromGrenadesBehaviour = true;

        [HideInInspector]
        public bool DefinatelyRunFromGrenade = false;

        [System.Serializable]
        public class CombatStateBehavioursClass
        {
            //public GameObject cube;
            [Tooltip("Minimal period of time in seconds for Ai Agent to stay in any given behaviour.")]
            public float MinStateSwitchTime;
            [Tooltip("Maximal period of time in seconds for Ai Agent to stay in any given behaviour.")]
            public float MaxStateSwitchTime;
            [Tooltip("Checking this checkbox gives AI agent possibility to behave like Stormtrooper i.e. move towards closest enemy and fire his weapon on the go." +
                " AI Charging paragraph will appear with the settings for this behaviour if this checkbox is checked. ")]
            public bool EnableAiCharging;
            [Tooltip("Makes AI agent to take covers during combat state and activates AI Covers paragraph for fine-tuning of cover parameters.")]
            public bool TakeCovers;
            [Tooltip("Enables AI agent to move between Firing Points during a combat state. Firing Points usage can be fine-tuned inside AI Firing Points paragraph that will appear once this checkbox is checked.")]
            public bool UseFiringPoints;
            [Tooltip("Makes AI agent able to perform strafing during combat state. Strafing behaviour can be tweaked inside 'AI Strafing' paragraph that will appear if this checkbox is checked.")]
            public bool EnableStrafing;

            [Tooltip("Makes AI agent perform quick scanning after combat state is completed.")]
            public bool EnablePostCombatScan;

            [Tooltip("Makes AI agent able to perform melee attacks during combat state.")]
            public bool EnableMeleeAttack;

            [Tooltip("If enabled than Ai agent will use grenades against his enemies")]
            public bool UseGrenades = false;

            [Tooltip("This checkbox will enable AI agent to be alerted from grenades landed")]
            public bool EnableGrenadeAlerts;

            [Tooltip("Enables the AI agent to have two shooting postures: standing and crouching when stationary.")]
            public bool EnableStationedCrouchedShooting;

            [Tooltip("This checkbox will enable AI agent to perform bullet impact animations. It will add 'Humanoid Body Hits' script that contains all necessary settings and fields to this Ai agent. ")]
            public bool UseImpactAnimations;

            [Tooltip("If enabled Ai agent will get alerted in case target is lost during combat")]
            public EnemyPursueTypes ChooseEnemyPursueType;
        }

        [Tooltip("All combat state related behaviours grouped in this section. " +
            "Min and Max State Swich Time fields will set time frame within which random value will be selected for states to be active.")]

        public CombatStateBehavioursClass CombatStateBehaviours;

        [Range(0, 100)]
        [Tooltip("Increasing this slider value will increase the chances of the Ai agent to perform charging behaviour and decreasing the slider value will reduce the chances" +
            " of the Ai agent to perform charging behaviour.")]
        public int ChargingProbability = 100;
        [Range(0, 100)]
        [Tooltip("Increasing this slider value will increase the chances of the Ai agent to use covers and decreasing the slider value will reduce the chances" +
            " of the Ai agent to use covers.")]
        public int CoversUsageProbability = 100;
        [Range(0, 100)]
        [Tooltip("Increasing this slider value will increase the chances of the Ai agent to use firing points and decreasing the slider value will reduce the chances" +
            " of the Ai agent to use firing points.")]
        public int FiringPointsUsageProbability = 100;
        //[ReadOnly]
        [HideInInspector]
        public int DebugTotalProbabilityValue;

        [System.Serializable]
        public enum EnemyPursueTypes
        {
            EnableApproachingEnemyPursue,
            EnableStationedEnemyPursue,
            DoNotPursueEnemy
        }

        [System.Serializable]
        public class NonCombatBehavioursClass
        {
            [Tooltip("AI agent will behave according to selected option." +
                " 'Scanning' option makes AI agent to remain stationary and look around thus scanning the near by area." +
                " If 'Patrolling' is selected then AI agent will move between patrolling points placed in the level." +
                " 'Wandering' option makes AI agent to unpredictably walk around within specified range and randomly choose each next direction.")]
            public InvestigationTypes DefaultBehaviour;
            [Tooltip("Enable this to add a Ai hearing to the Ai agent and display the 'Ai Hearing' paragraph in the core Ai behaviour script.")]
            public bool EnableSoundAlerts = true;

            [Tooltip("Enables Ai agent`s reaction to emergency state.")]
            public bool EnableEmergencyAlerts;
            [Tooltip("Enables Ai agent`s reaction to dead bodies of friendly AI agents.")]
            public bool EnableDeadBodyAlerts;

            [Tooltip("Enables Head Rotations during Wander,patrol,scan and Idle behaviours.")]
            public bool EnableHeadRotations = true;
            // public bool FollowCommander;
        }

        [Tooltip("Set of AI agent`s properties for non combat states that are grouped in this paragraph are performed both within this script itself and outside of this script. " +
            "Activation of options such as Patrolling or Wandering or Scanning will automatically add required separate scripts responsible for executing selected behaviours.")]
        public NonCombatBehavioursClass NonCombatBehaviours;

        public enum InvestigationTypes
        {
            Stationary,
            Scan,
            Patrol,
            Wander
        }

        [System.Serializable]
        public class MeleeCombatBehavioursClass
        {
            [Tooltip("Activate this checkbox to enable time intervals between Melee attacks")]
            public bool EnableTimeBetweenMeleeAttacks = true;

            [Tooltip("Minimum Time interval between Melee Attacks")]
            public float MinTimeIntervalBetweenMeleeAttack;
            [Tooltip("Maximum time interval between Melee Attacks")]
            public float MaxTimeIntervalBetweenMeleeAttack;

            [Tooltip("Minimum required distance to attack player")]
            public float MinDistanceToAttackPlayer;
            [Tooltip("Maximum required distance to attack player")]
            public float MaxDistanceToAttackPlayer;

            [Tooltip("Minimum required distance to attack enemies Ai agents")]
            public float MinDistanceToAttackEnemyAiAgent;

            [Tooltip("Maximum required distance to attack enemies Ai agents")]
            public float MaxDistanceToAttackEnemyAiAgent;

            [Range(1, 3)]
            [Tooltip("Use slider to decide how many animations to play for Melee attack")]
            public int AnimationsToPlay = 1;

            [Tooltip("Set of fields to create melee animations for the Ai agent.")]
            public List<MeleeAnimationBehavioursClass> MeleeAnimation = new List<MeleeAnimationBehavioursClass>();

            [HideInInspector]
            public int previousListCount;


        }

        //[System.Serializable]
        //public class QuickScanBehavioursClass
        //{
        //    [Tooltip("[Draft]Minimum time to play new turning animation.")]
        //    public float MinTimeBetweenTurns = 2f;
        //    [Tooltip("[Draft]Maximum time to play new turning animation.")]
        //    public float MaxTimeBetweenTurns = 4f;
        //    [Tooltip("[Draft]Minimum time to wait before activating non combat state.")]
        //    public float MinScanCompletionTime = 10f;
        //    [Tooltip("[Draft]Maximum time to wait before activating non combat state.")]
        //    public float MaxScanCompletionTime = 20f;

        //    [Tooltip("[Draft]Enter the forward turning animation speed when Quick scan is activated after combat state expired.")]
        //    public float ForwardTurnAnimationSpeed = 2f;
        //    [Tooltip("[Draft]Enter the backward turning animation speed when Quick scan is activated after combat state expired.")]
        //    public float BackwardTurnAnimationSpeed = 2f;
        //    [Tooltip("[Draft]Enter the right turning animation speed when Quick scan is activated after combat state expired.")]
        //    public float RightTurnAnimationSpeed = 2f;
        //    [Tooltip("[Draft]Enter the left turning animation speed when Quick scan is activated after combat state expired.")]
        //    public float LeftTurnAnimationSpeed = 2f;
        //}
        [System.Serializable]
        public class AimedScanBehaviourClass
        {
            [Tooltip("Minimum time to keep aimed look in the direction that resulted after playing previous aimed animation turn clip, before starting playback of the new turn animation clip.")]
            public float MinTimeBetweenAimedTurns = 2f;
            [Tooltip("Maximum time to keep aimed look in the direction that resulted after playing previous aimed animation turn, before starting playback of the new one.")]
            public float MaxTimeBetweenAimedTurns = 4f;
            [Tooltip("Minimum time to keep aimed scanning before switching to non combat state.")]
            public float MinScanCompletionTime = 10f;
            [Tooltip("Maximum time to keep aimed scanning before switching to non combat state.")]
            public float MaxScanCompletionTime = 20f;

            [Tooltip("If checked scan animations will playback sequentially.In case unchecked the scan animations will playback randomly.")]
            public bool PlayAnimationsSequentially = true;
            [Tooltip("Create up to 4 turning directions for Ai agent to perform during aimed scanning.")]
            public List<ScanTurnAnimations> AimedTurningAnimations;
        }
        [System.Serializable]
        public enum AnimationDirectionName
        {
            Forward,
            Backward,
            Right,
            Left

        }
        [System.Serializable]
        public class ScanTurnAnimations
        {
            [Tooltip("Choose one of 4 turning directions from this list. AI agent will play corresponding animation clip from the animation tree that is assigned to this direction.")]
            public AnimationDirectionName TurnDirection;
            [Tooltip("Minimal playback speed of the the turn animation clip.")]
            public float MinAimedTurnAnimationSpeed = 1f;
            [Tooltip("Maximal playback speed of the the turn animation clip.")]
            public float MaxAimedTurnAnimationSpeed = 1f;

        }

        [Tooltip("Set of AI agent`s properties for post combat scan behaviour after the combat state.")]
        public AimedScanBehaviourClass PostCombatAimedScanBehaviour;

        [System.Serializable]
        public class MeleeAnimationBehavioursClass
        {
            [Tooltip("Choose melee attack animation to be played during the melee attack.If melee attack 1 is selected than you can check which animation will be used by going to scene and than click on" +
                " the MobileActionKit<HumanoidAi<EditHumanoidAiAnimations after clicking 'EditHumanoidAiAnimations' you can drag and drop this Humanoid Ai agent into the field name 'Humanoid Ai animator controller'" +
                " and click import animations.this way you will be able to see which animation is been used in melee attack 1.")]
            public ChoosenMeleeAnimations MeleeAttack;
            [Tooltip("Provide same melee attack animation clip that you have used for this Humanoid ai agent.you can check which animation you have used by going to " +
                " the MobileActionKit<HumanoidAi<EditHumanoidAiAnimations after clicking 'EditHumanoidAiAnimations' you can drag and drop this Humanoid Ai agent into the field name 'Humanoid Ai animator controller'" +
                " and click import animations.this way you will be able to see which animation is been used for melee attack.")]
            public AnimationClip MeleeAnimationClip;

            public float MeleeDamageScriptsActivationDelay = 1f;
            [Tooltip("Drag and drop the Ai melee damage scripts attached with the child gameObject of this Ai agent.")]
            public AiMeleeDamage[] AiMeleeDamageScripts;
        }

        [Tooltip("Set of AI agent`s properties for doing melee attack against its enemies")]
        public MeleeCombatBehavioursClass AiMeleeAttack;

        [System.Serializable]
        public enum ChoosenMeleeAnimations
        {
            MeleeAttack1,
            MeleeAttack2,
            MeleeAttack3
        }

        //[Tooltip("Enable this to add a 'Humanoid Grenade Thrower' script on the Ai agent and activates the Grenade throwing ability on the Ai agent.")]
        //public bool EnableGrenadesThrow;
        HumanoidGrenadeThrower grenadeThrowerScript;

        [System.Serializable]
        public class GrenadeAlertBehavioursClass
        {
            [Tooltip("Drag and drop grenade detector gameObject from the hierarchy of this Ai agent into this field.")]
            public Transform GrenadeDetector;
            [Tooltip("Specify which layers will be visible when detecting incoming grenade.")]
            public LayerMask VisibleLayers;

            [Tooltip("Choose one of the option from the list provided below.If 'MoveAwayFromLandedGrenades' is selected than the Ai agent will 100% move away from the grenades if he is within the trigger area." +
                " In case second option is chosen 'RandomizeMoveFromLandedGrenades' than Ai agent will have 50/50 chance to decide whether to move away from landed grenades or not.")]
            public GrenadeAlertClass ChooseAlertType;

            [Tooltip("drag and drop retreat point child game objects of AI agent in this list.")]
            public Transform[] RetreatPoints;
            [Tooltip("After getting world coordinates of a retreat point another second world coordinate is created within retreat point radius. " +
                "AI agent  runs towards this second world coordinate instead of initial world coordinate of a retreat point." +
                "The bigger this value is the more unpredictable resulted retreat direction and distance will get")]
            public float RetreatPointRadius = 10f;
            [Tooltip("Value in this field is defining allowed time for the Ai agent to perform retreat behaviour." +
                "When this value is expired Ai agent will get back to previous behaviour" +
                "In case the fight was over while Ai agent was performing retreat behaviour than the Ai agent will get back to whatever state he was in" +
                "before this fight started.")]
            public float RetreatTimer = 4f;
        }

        [Tooltip("Set of AI agent`s properties for grenade alert behaviour.")]
        public GrenadeAlertBehavioursClass GrenadeAlerts;



        [System.Serializable]
        public class AiPursueClass
        {
            [Tooltip("This slider sets the offset radius from last seen enemy coordinate. Pursuing Ai will set random coordinate to go to within that radius.")]
            [Range(0.0f, 100.0f)]
            public float EnemyPuruseErrorRadius = 10f;

            [Tooltip("This subsection contains the values of aimed turns(scanning) for this agent after arriving at the LastKnownEnemyPosition coordinate. " +
                "It will playback idle turning clips assigned to respective turning directions for lower body layer inside Animator while playing upper body aiming animation clip for all of those turns. ")]
            public AimedScanBehaviourClass AimedScaningAtEnemyLastKnownPosition;

            [Tooltip("Minimum distance to stop near the Pursue point and to consider it as reached.")]
            public float MinNearStoppingDistance = 2f;

            [Tooltip("Maximum  distance to stop near the Pursue point and to consider it as reached.")]
            public float MaxNearStoppingDistance = 3f;


            [Tooltip("Minimal remaining distance to pursue point for the switch from walking or running to sprinting towards it.")]
            public float MinSprintDistance = 20f;

            [Tooltip("Maximum distance for sprinting towards coordinate.")]
            public float MaxSprintDistance = 100f;

            [Tooltip(" Minimal remaining distance to pursue point for the switch from walking or sprinting to running towards it.")]
            public float MinRunDistance = 50f;

            [Tooltip("Maximal remaining distance to pursue point for the switch from walking or sprinting to running towards it.")]
            public float MaxRunDistance = 100f;

            [Tooltip("Minimal remaining distance to pursue point for the switch from running or sprinting to walking towards it.")]
            public float MinWalkDistance = 20f;

            [Tooltip("Maximal remaining distance to pursue point for the switch from running or sprinting to walking towards it.")]
            public float MaxWalkDistance = 35f;

            //[Tooltip("Minimum staring time of Stationed AI agent at the last known position of the enemy before switching to idle state.")]
            //public float MinStationedStareTimeAtEnemyLastKnownPosition = 8f;

            //[Tooltip("Maximum staring time of Stationed AI agent at the last known position of the enemy before switching to idle state.")]
            //public float MaxStationedStareTimeAtEnemyLastKnownPosition = 8f;

            [Tooltip("Minimum staring time of any non Stationed AI agent at the last known position of the enemy before beginning pursue.")]
            public float MinStareTimeAtEnemyLastKnownPosition = 8f;

            [Tooltip("Maximum staring time of any non Stationed AI agent at the last known position of the enemy before beginning pursue.")]
            public float MaxStareTimeAtEnemyLastKnownPosition = 8f;

            [Tooltip("This sub section contains values which show the distance to the last known position of the enemy.")]
            public DebugPursueDistancesClass DebugPursueDistances;
        }

        [System.Serializable]
        public class DebugPursueDistancesClass
        {

            [ReadOnly]
            [Tooltip("Debug the distance to coodinate(Enemy Last Known Position) to reach")]
            public float DebugDistanceToCoordinate = 0f;

            [ReadOnly]
            [Tooltip("Debug the sprint distance to coodinate(Enemy Last Known Position) to reach")]
            public float DebugSprintDistance = 0f;

            [ReadOnly]
            [Tooltip("Debug the run distance to coodinate(Enemy Last Known Position) to reach")]
            public float DebugRunDistance = 0f;

            [ReadOnly]
            [Tooltip("Debug the walk distance to coodinate(Enemy Last Known Position) to reach")]
            public float DebugWalkDistance = 0f;
        }

        [Tooltip("This paragraph defines various parameters of  pursue behaviour of fleeing enemy.")]
        public AiPursueClass AiPursue;

        //public Transform CurrentWayPointTransform;
        //public Transform CurrentCoverPointTransform;
        //public bool ChooseRandomWayPoints = true;

        // Private Variables
       // float StareTimeAtLastKnownEnemyCoordinateIfStationed;
        float StareTimeAtLastKnownEnemyCoordinate;
        float SaveDetectingDistance;
        [HideInInspector]
        public FindEnemies FindEnemiesScript;
        [HideInInspector]
        public HumanoidAiHealth HealthScript;
        [HideInInspector]
        public SpineRotation RotateSpine;
        [HideInInspector]
        public Animator anim;
        float RandomiseProximityTimes;
        float RandomWalkChargeDistance;

        [HideInInspector]
        public bool StopSpineRotation = false;

        [HideInInspector]
        public bool ReachnewCoverpoint = false;
        [HideInInspector]
        public bool ReachnewFirepoint = false;


        [HideInInspector]
        public bool ChangeCover = false;
        [HideInInspector]
        public List<Transform> CrouchPositions = new List<Transform>();
        [HideInInspector]
        public CoverNode[] CoverPoints;
        //[HideInInspector]
        //public float SaveResetedCoverRandomisation = 0f;
        //public float SaveResetedWaypointRandomisation = 0f;
        bool FindingNewCrouchPoint = false;
        bool CheckTime = false;
        bool FindValidCover = true;
        //int LastCoverPoint;
        [HideInInspector]
        public int CurrentCoverPoint = 996;
        //  bool ResetCoverRandomisation = false;
        //  bool ResetWaypointRandomisation = false;
        FiringPoint[] Firepoint;
        int LastWayPointSelected = 9999999;
        [HideInInspector]
        public int CurrentWayPoint = 0;
        bool findWayPoint = false;
        //int NewValue = 0;

        [HideInInspector]
        public bool RemoveEnemiesFromList = true;
       
        float Angle;

        [HideInInspector]
        public bool IsEnemyLocked = false;
        
        Vector3 disWithWayPoint;
        
        //[HideInInspector]
        //public bool NoEnemyInView = true;
        bool TimeToDetect = false;
        bool Detect = false;
        
        bool ResetToNormalState = false;
         
        Vector3 RandomDirAfterTargetLost;
        float RandomApproachToSound = 100f;
        [HideInInspector]
        public bool SearchingForSound = false;
        [HideInInspector]
        public bool CombatStarted = false;
        bool CanPlayDetectanim = false;
        
        bool ResetAnimator = false;       
      
        float TimeToCheckForCover;

        [HideInInspector]
        public HumanoidVisibilityChecker VisibilityCheck;
        private AiPathFinder pathfinder;
        //bool FriendlyShot = false;
        [HideInInspector]
        public bool IsCrouched = false;
        //bool IsShotMade = false;
        //bool SoundAlreadyHeard = false;
        //[HideInInspector]
        //public Vector3 FriendlybotShotPos;

        [HideInInspector]
        public bool IsAgentRoleLeader = false;
        float StoppingDistanceForChargeBot;
        bool OverWriteWalking = false;
        bool OneTimeMessage = false;

       

        bool SelectStrafePoint = false;

        float SelectARandomPoint;
        Vector3 strafePoint;
        bool CanAIStrafeByDefault = false;
      
        bool FirstCover = false;
        bool StartFindRealCovers = false;

        Vector3 nearestCrouchPoint;
        [HideInInspector]
        public bool StartStrafingForAvoidingBots = false;
        public bool AreaCheck = false;

        Patrolling PatrolingScript;
        Wandering WanderingScript;
        [HideInInspector]
        public string PatrollingAnimName = "WalkIdle";

        [HideInInspector]
        public bool GeneratedSprintPoint = false;
        [HideInInspector]
        public bool BotMovingAwayFromGrenade = false;
        Vector3 LocationForSprinting;
        int directionpoint;

        Dictionary<float, GameObject> distDic = new Dictionary<float, GameObject>();

        public bool CanTakeCover = false;

        [HideInInspector]
        public bool IsNearDeadBody = false;
        [HideInInspector]
        public Vector3 InvestigationCoordinates;

        [HideInInspector]
        public List<string> DeadBodiesSeen = new List<string>();

        [HideInInspector]
        public Transform EmergencyDirectionPoint;
        [HideInInspector]
        public bool IsEmergencyRun = false;
        [HideInInspector]
        public bool IsInEmergencyState = false;
        bool StartInvestigation = false;
        bool StayTimeEnded = false;

        [HideInInspector]
        public bool ChoosenAiTypeIsIdle = false;

        // float NavmeshPathTimer;

        private NavMeshPath path;
        //private float elapsed = 0.0f;
        

        bool IsReachedEmergencyPoint = false;
        bool FindEmergencyPoint = false;
        Transform EmergencyNodeHolding;

        //    private HeadIk HeadIkScript;

        // bool CanCreateaRandomPoint = false;

        [HideInInspector]
        public ScanningScript ScanScript;

        [HideInInspector]
        public bool IsBodyguard = false;
        [HideInInspector]
        public Transform BossTransform;

        [HideInInspector]
        public bool SprintingActivated = false;
        bool OverwriteSprinting = false;

        [HideInInspector]
        public bool IsScanning = false;
        // bool OverwriteBodyguardScanning = false;

        //bool SmoothAnimatorWeightChange = false;

        [System.Serializable]
        public class Mycomponents
        {
            [Tooltip("Drag and drop 'NavMeshAgent' component attached to this gameobject from the hierarchy into this field or just dragging in this component within inspector itself.")]
            public NavMeshAgent NavMeshAgentComponent;
            [Tooltip("Drag and drop Shooting point game object with required component attached to it into this field.")]
            public HumanoidAiWeaponFiringBehaviour HumanoidFiringBehaviourComponent;
            [Tooltip("Drag and drop 'HumanoidAiAudioPlayer' component attached to this gameobject from the hierarchy into this field or just drag this component within inspector itself.")]
            public HumanoidAiAudioPlayer HumanoidAiAudioPlayerComponent;

            [Tooltip("Drag and drop 'HumanoidAiConversationSoundsComponent' with required component attached to it into this field.")]
            public AiNonCombatChatter AiNonCombatChatterComponent;

            //[Tooltip("Fields in this section are responsible for playing basic set of footsteps sounds for prevalent surface type in the level." +
            //    "In case there will be more than one surface types in the level than functionality of playing specific step sounds appropriate to the given surface will be provided by 'Surfaces' component.")]
            //public AudioClip[] FootStepsSounds;
            //[Tooltip("Drag and drop gameobject called 'DeadBodyScanner' that has script with the same name attached to it, from this Ai agent's hierarchy into this field.")]
            //public GameObject DeadBodyScanObject;
            //[Tooltip("Drag and drop gameobject called 'Ai Direction Pointer', from this Ai agent's hierarchy into this field.")]
            //public GameObject AiDirectionPointer;
        }
        [System.Serializable]
        public class StationedShootingClass
        {
            [Tooltip("Enable shooting only in crouched position while stationary and in combat.")]
            public bool CrouchedShootingOnly = false;

            [Tooltip("Randomly alternate between crouched and standing while shooting, but only when stationary and in combat.")]
            public bool RandomizeCrouchStandShooting = false;

            [Tooltip("Minimum time to switch posture during shooting, applicable when stationary and in combat.")]
            public float MinTimeToSwitchPosture = 3f;

            [Tooltip("Maximum time to switch posture during shooting, applicable when stationary and in combat.")]
            public float MaxTimeToSwitchPosture = 5f;
        }

        [Tooltip("Configure the AI's shooting posture when stationed and actively engaged in combat.")]
        public StationedShootingClass StationedShooting = new StationedShootingClass();

        [Tooltip("Set of nesessary components for AI agent to function.")]
        public Mycomponents Components = new Mycomponents();

        [System.Serializable]
        public class AnimsSpeeds
        {
            public float FireAnimationSpeed = 1f;
            public float AimingAnimationSpeed = 1f;
            public float ReloadAnimationSpeed = 1f;
            public float IdleAnimationSpeed = 1f;
            public float CrouchFireAnimationSpeed = 1f;
            public float CrouchReloadAnimationSpeed = 1f;
            public float CrouchAimingAnimationSpeed = 1f;
            public float SprintingAnimationSpeed = 1f;
            public float StandCoverRightAnimationSpeed = 1f;
            public float StandCoverLeftAnimationSpeed = 1f;
            public float StandCoverNeutralAnimationSpeed = 1f;
            public float RunForwardAnimationSpeed = 1f;
            public float RunRightAnimationSpeed = 1f;
            public float RunLeftAnimationSpeed = 1f;
            public float RunBackwardAnimationSpeed = 1f;
            public float RunForwardRightAnimationSpeed = 1f;
            public float RunForwardLeftAnimationSpeed = 1f;
            public float RunBackwardRightAnimationSpeed = 1f;
            public float RunBackwardLeftAnimationSpeed = 1f;
            public float WalkIdleAnimationSpeed = 1f;
            public float WalkForwardAnimationSpeed = 1f;
            public float WalkRightSpeed = 1f;
            public float WalkLeftSpeed = 1f;
            public float WalkBackwardSpeed = 1f;
            public float WalkForwardRightSpeed = 1f;
            public float WalkBackwardRightSpeed = 1f;
            public float WalkBackwardLeftAnimationSpeed = 1f;
            public float WalkForwardLeftAnimationSpeed = 1f;
            public float MeleeAttack1AnimationSpeed = 1f;
            public float MeleeAttack2AnimationSpeed = 1f;
            public float MeleeAttack3AnimationSpeed = 1f;
        }

        [System.Serializable]
        public class ZombieAnimsSpeeds
        {
            public float IdleAnimationSpeed = 1f;
            public float SprintingAnimationSpeed = 1f;
            public float RunForwardAnimationSpeed = 1f;
            public float WalkForwardAnimationSpeed = 1f;            
            public float MeleeAttack1AnimationSpeed = 1f;
            public float MeleeAttack2AnimationSpeed = 1f;
            public float MeleeAttack3AnimationSpeed = 1f;
        }
        [System.Serializable]
        public class ZombieMovementSpeedClass
        {
            public float WalkForwardSpeed = 0.8f;            
            public float RunForwardSpeed = 2f;
            public float SprintSpeed = 5f;
            [Tooltip("Rotation Speed At Y")]
            public float BodyRotationSpeed = 6;
        }
        public class ZombieAnims
        {
            public string DefaultStateParameterName = "DefaultState";
            public string IdleParameterName = "Idle";

            public string WalkForwardParameterName = "WalkForward";
            public string RunForwardParameterName = "RunForward";
            public string SprintingParameterName = "Sprinting";

            public string UpperBodyHitAnimation1ParameterName = "UpperBodyHitAnimation1";
            public string UpperBodyHitAnimation2ParameterName = "UpperBodyHitAnimation2";
            public string UpperBodyHitAnimation3ParameterName = "UpperBodyHitAnimation3";

            public string LowerBodyHitAnimation1ParameterName = "LowerBodyHitAnimation1";
            public string LowerBodyHitAnimation2ParameterName = "LowerBodyHitAnimation2";
            public string LowerBodyHitAnimation3ParameterName = "LowerBodyHitAnimation3";

            public string MeleeAttack1ParameterName = "MeleeAttack1";
            public string MeleeAttack2ParameterName = "MeleeAttack2";
            public string MeleeAttack3ParameterName = "MeleeAttack3";

        }

        // [System.Serializable]
        public class Anims
        {
            public string FireParameterName = "Fire";
            public string StandAimingParameterName = "Aiming";

            public string IdleAimingParameterName = "IdleAiming";

            public string ReloadParameterName = "Reload";


            public string IdleParameterName = "Idle";

            public string CrouchFireParameterName = "CrouchFire";

            public string CrouchReloadParameterName = "CrouchReload";

            public string CrouchAimingParameterName = "CrouchAim";

            public string SprintingParameterName = "Sprinting";

            public string StandCoverLeftParameterName = "StandCoverLeft";

            public string StandCoverRightParameterName = "StandCoverRight";

            public string StandCoverNeutralParameterName = "StandCoverNeutral";

            public string RunForwardParameterName = "RunForward";

            public string RunRightParameterName = "RunRight";

            public string RunLeftParameterName = "RunLeft";

            public string RunBackwardParameterName = "RunBackward";

            public string RunForwardRightParameterName = "RunForwardRight";

            public string RunForwardLeftParameterName = "RunForwardLeft";

            public string RunBackwardRightParameterName = "RunBackwardRight";

            public string RunBackwardLeftParameterName = "RunBackwardLeft";

            public string WalkIdleParameterName = "WalkIdle";

            public string WalkForwardParameterName = "WalkForward";

            public string WalkRightParameterName = "WalkRight";

            public string WalkLeftParameterName = "WalkLeft";

            public string WalkBackwardParameterName = "WalkBackward";

            public string WalkForwardRightParameterName = "WalkForwardRight";

            public string WalkBackwardRightParameterName = "WalkBackwardRight";

            public string WalkBackwardLeftParameterName = "WalkBackwardLeft";

            public string WalkForwardLeftParameterName = "WalkForwardLeft";

            public string UpperBodyHitAnimation1ParameterName = "UpperBodyHitAnimation1";
            public string UpperBodyHitAnimation2ParameterName = "UpperBodyHitAnimation2";
            public string UpperBodyHitAnimation3ParameterName = "UpperBodyHitAnimation3";

            public string LowerBodyHitAnimation1ParameterName = "LowerBodyHitAnimation1";
            public string LowerBodyHitAnimation2ParameterName = "LowerBodyHitAnimation2";
            public string LowerBodyHitAnimation3ParameterName = "LowerBodyHitAnimation3";

            public string MeleeAttack1ParameterName = "MeleeAttack1";
            public string MeleeAttack2ParameterName = "MeleeAttack2";
            public string MeleeAttack3ParameterName = "MeleeAttack3";

            public string LeftCoverIdle = "LeftStandCoverIdle";
            public string RightCoverIdle = "RightStandCoverIdle";

            public string DefaultStateParameterName = "DefaultState";
        }
        [Tooltip("This Paragraph contains information about all the parameters and transition names inside animator controller along with the speeds for each animation Ai agent is using." +
            "For example - If 'Fire' parameter becomes true, then it  calls 'Fire' transition to 'Fire' animation state, which in turn plays back whatever animation clip it has stored in it." +
            "It is strongly recommended that Animations states are named the same as the parameters for convinience and clarity of use." +
            "All or some of those fields might be used depending on the way game developer will setup AI Agent." +
            "Ability to change animation clip playback speed at run time is provided for fields below to allow for finer tweaks to animations." +
            "You have to copy and paste the exact parameter name from the animator component and paste it inside the appropriate field provided below.")]
        public Anims AiAgentAnimatorParameters = new Anims();

        [Tooltip("This Paragraph contains information about all the parameters and transition names inside animator controller along with the speeds for each animation Ai agent is using." +
           "For example - If 'Fire' parameter becomes true, then it  calls 'Fire' transition to 'Fire' animation state, which in turn plays back whatever animation clip it has stored in it." +
           "It is strongly recommended that Animations states are named the same as the parameters for convinience and clarity of use." +
           "All or some of those fields might be used depending on the way game developer will setup AI Agent." +
           "Ability to change animation clip playback speed at run time is provided for fields below to allow for finer tweaks to animations." +
           "You have to copy and paste the exact parameter name from the animator component and paste it inside the appropriate field provided below.")]
        public ZombieAnims ZombieAiAnimatorParameters = new ZombieAnims();

        //[System.Serializable]
        //public class DirectionMarkersClass
        //{
        //    //[Tooltip("Set of field groups for intended strafing directions with 3 fields for each direction. 'Direction Name' field should match the name of its respective Direction Marker." +
        //    //    " 'Animation Parameter Name' should match the transition name inside Animator Controller. 'Moving speed' is to be adjusted to achieve visual coherency between animation and movement in given direction. ")]
        //    public StrafeDirections WalkAnimations;

        //    //[Tooltip("Set of field groups for intended strafing directions with 3 fields for each direction. 'Direction Name' field should match the name of its respective Direction Marker." +
        //    //    " 'Animation Parameter Name' should match the transition name inside Animator Controller. 'Moving speed' is to be adjusted to achieve visual coherency between animation and movement in given direction. ")]
        //    public StrafeDirections RunAnimations;

        //}

        //[Tooltip("Fields inside this paragraph are responsible for setting up the strafe animations with there proper direction marker for the Ai agent to make Ai agent" +
        //    " perform strafing correctly.")]
        //public DirectionMarkersClass AiDirectionMarkers;

        [System.Serializable]
        public class NavMeshAgentSettingsClass
        {

            //[Tooltip("Specify the Y axis position of the 'Visual field of view' when the Ai agent is in the crouch posture.")]
            //public float VisualFieldOfViewPositionOnCrouch = 0.84f;
            //[Tooltip("Drag and drop head bone gameobject attached with the child of this gameobject into this field.")]
            //public Transform HeadBone;

            [Tooltip("Base Offset Value of nav mesh agent component during stand shoot")]
            public float NavMeshAgentStandBaseOffset = -0.09f;
            [Tooltip("Base Offset Value of nav mesh agent component during crouch shoot")]
            public float NavMeshAgentCrouchBaseOffset = -0.2f;

            [Tooltip("Base Offset Value of nav mesh agent component during death when standing")]
            public float NavMeshAgentStandBaseOffsetDuringDeath = -0.1f;

            [Tooltip("Base Offset Value of nav mesh agent component during death when crouching")]
            public float NavMeshAgentCrouchBaseOffsetDuringDeath = -0.1f;

            [Tooltip("When Ai agent takes the stand cover the navmesh agent radius value will be changed to this value.")]
            public float AgentRadiusDuringStandCover = 0.2f;
            [Tooltip("When Ai agent takes the crouch cover the navmesh agent radius value will be changed to this value.")]
            public float AgentRadiusDuringCrouchCover = 0.4f;

            [Tooltip("When Ai agent takes the stand Firing cover the navmesh agent radius value will be changed to this value.")]
            public float AgentRadiusDuringStandFiringCover = 0.2f;
            [Tooltip("When Ai agent takes the crouch firing cover the navmesh agent radius value will be changed to this value.")]
            public float AgentRadiusDuringCrouchFiringCover = 0.4f;


        }

        [Tooltip("Set of parameters that define AI agent`s detection characteristics. Can be set to create arcady AI agent`s detection that will force him to always detect and attack it`s enemies." +
            " Or more realistic detection that will be limited by a number of real world factors.")]
        public NavMeshAgentSettingsClass NavMeshAgentSettings = new NavMeshAgentSettingsClass();

        [System.Serializable]
        public class AllSpeeds
        {
            [Tooltip("Movement speeds for the Ai agent")]
            public MovementSpeedClass MovementSpeeds;
            [Tooltip("Animation speeds for the Ai agent")]
            public AnimsSpeeds AnimationSpeeds;
        }

        [System.Serializable]
        public class ZombieAllSpeeds
        {
            [Tooltip("Movement speeds for the Ai agent")]
            public ZombieMovementSpeedClass MovementSpeeds;
            [Tooltip("Animation speeds for the Ai agent")]
            public ZombieAnimsSpeeds AnimationSpeeds;
        }

        [System.Serializable]
        public class MovementSpeedClass
        {
            public float SprintSpeed = 5f;

            public float WalkForwardAimingSpeed = 0.8f;
            public float WalkForwardSpeed = 0.8f;
            public float WalkBackwardSpeed = 0.8f;
            public float WalkLeftSpeed = 0.8f;
            public float WalkRightSpeed = 0.8f;
            public float WalkForwardLeftSpeed = 0.8f;
            public float WalkForwardRightSpeed = 0.8f;
            public float WalkBackwardRightSpeed = 0.8f;
            public float WalkBackwardLeftSpeed = 0.8f;

            public float RunForwardSpeed = 2f;
            public float RunBackwardSpeed = 2f;
            public float RunLeftSpeed = 2f;
            public float RunRightSpeed = 2f;
            public float RunForwardLeftSpeed = 2f;
            public float RunForwardRightSpeed = 2f;
            public float RunBackwardLeftSpeed = 2f;
            public float RunBackwardRightSpeed = 2f;

            [Tooltip("Rotation Speed At Y")]
            public float BodyRotationSpeed = 6;
        }


        public AllSpeeds Speeds = new AllSpeeds();
        public ZombieAllSpeeds ZombieSpeeds = new ZombieAllSpeeds();

        [System.Serializable]
        public class DebugInfoClass
        {
            //[Tooltip("Displays information about current task Ai is performing ")]
            //public string CurrentState = "WILL DEBUG THE CURRENT STATE OF THIS AI";
            //[Tooltip("Will show debug line from this Ai agent's root object towards its current target." +
            //         "This debug line is shown during combat state of the Ai")]
            //public bool DebugRaycastToTarget = true;

            [Tooltip("Provide the information about Ai agent current state.")]
            public GameObject DebugInfoTextUI;
            [Tooltip("Adjust the DebugInfo Text UI position at x,y and z axis")]
            public Vector3 DebugInfoTextUIOffset;
            //public Color DebugRaycastColor;
            //[Tooltip("Agent velocity when moving towards the destination.")]
            //public float AgentVelocity;
            //[HideInInspector]
            //public int DebugCoverNumberToTake;
        }

        [Tooltip("This paragraph is usefull during various developement stages as it lets you see current states of AI agent at any point in time." +
            "Current State Outputs message into the console describing which one of several AI states is active at any point in play mode." +
            "Those states are Shooting, Strafing, Patrolling,Taking cover,Moving towards Waypoints,Charging,Investigating and Alert")]
        public DebugInfoClass DebugInfo = new DebugInfoClass();

        [System.Serializable]
        public class AiCoverClass
        {
            [Tooltip("Drag and drop the child gameObject of this Ai agent which have a script named called 'FindCovers' from the hierarchy into this field.")]
            public FindCovers CoverFinder;
            // public bool MoveImmediateOnFirstCover = true;
            // [Tooltip("If checked this will enable AI agent to constantly use covers and move between cover points that are within 'Range To Find Cover Point' field value.")]
            // public bool ContinouslyTakeCovers = false;

            [Tooltip("If enabled than the Ai agent will find the closest covers from his current position. " +
                "In case if this checkbox is disabled than Ai agent will take random cover within the Range specified in the field name" +
                " 'RangeToFindCoverPoint' ")]
            public bool ChooseClosestCovers = true;

            [Tooltip("The slider value below indicate what would be the probability of the Ai agent to switch new covers." +
                " In case the value in the slider is 100 than the Ai agent will switch between different covers around him. In case this value is 0 than the Ai agent" +
                " will not switch between any new cover after taking his first cover and will make sure to stay in his first cover only for the duration of combat." +
                " Depending on the probability Ai agent will decide whether to pick a new cover or stay at the currently picked one." +
                " but in case taken cover becomes invalid than the Ai agent" +
                " will make sure to switch to any other valid cover")]
            [Range(0,100)]
            public int SwitchingCoversProbability = 100;

            [HideInInspector]
            public bool SwitchCovers;

            [Tooltip("Range within which Ai agent will find cover points to hide behind.")]
            public float RangeToFindExistingCoverPoints = 50f;

            [Tooltip("If unchecked AI agent will perform assault by walking and shooting its target.")]
            public bool EnableSprintingBetweenCovers = true;
            [Tooltip("Minimal value for remaining distance to target to set the shortest limit for random value.")]
            public float MinSprintingDistance = 20f;
            [Tooltip("Maximal value for remaining distance to target to set the longest limit for random value.")]
            public float MaxSprintingDistance = 30f;

            [Tooltip("Minimum time AI agent will check for cover if it is available or not meaning for example when the game starts and if one of the Ai agent unable to find the best cover for him." +
                " He will do the checks to see if there is any free cover available or not. In case Ai agent is already Behind the cover than he will not consider doing the checks as even if his cover " +
                "time has been expired he would be able to take the same cover again (which he is already in). so Ai agent will consider checks only if the cover is unavailable to him and he is not within a cover at all.")]
            public float MinTimeToCheckForCovers = 1f;
            [Tooltip("Maximum time AI agent will check for cover if it is available or not meaning for example when the game starts and if one of the Ai agent unable to find the best cover for him." +
                  " He will do the checks to see if there is any free cover available or not. In case Ai agent is already Behind the cover than he will not consider doing the checks as even if his cover " +
                  "time has been expired he would be able to take the same cover again (which he is already in). so Ai agent will consider checks only if the cover is unavailable to him and he is not within a cover at all.")]
            public float MaxTimeToCheckForCovers = 2f;

            [Tooltip("Minimum time AI agent will spend behind current cover before taking another available cover point.")]
            public float MinimumTimeBetweenCovers = 3f;
            [Tooltip("Maximum time AI agent will spend behind current cover before taking another available cover point.")]
            public float MaximumTimeBetweenCovers = 5f;

            [Tooltip("Minimum time that Ai agent will stay at Open Fire point before returning to cover point, or leaving this cover if other combat behaviour comes into effect. ")]
            public float MinTimeAtOpenFirePoint = 2f;
            [Tooltip("Maximum time that Ai agent will stay at Open Fire point before returning to cover point, or leaving this cover if other combat behaviour comes into effect. ")]
            public float MaxTimeAtOpenFirePoint = 4f;

            [Tooltip("Speed at which AI agent will be procedurally rotated to the stand hiding pose.")]
            public float TransitionSpeedToStandCover = 15f;

            [Tooltip("Remaining distance to cover point at which Ai agent registers himself occupying it.")]
            public float TakenCoverProximityDistance = 0.15f;

            [Tooltip("If enabled than the Ai agent will be able to reload his weapon when behind the hide cover e.g Stand Hide Cover,Crouch Hide Cover")]
            public bool EnableReloadingBehindHideCovers = true;

            [Tooltip("Delay value to allow for Ai agent rotation and playback of stand hide cover animation before reloading his weapon.")]
            public float ReloadDelayBehindStandHideCover = 0.3f;

            [Tooltip("Delay value to allow for Ai agent to playback of crouch hide cover animation before  reloading his weapon. To prevent reloading animation from previous state(like running or walking/strafing).")]
            public float ReloadDelayBehindCrouchHideCover = 0.5f;

            [Tooltip("Allows Ai agent to reload his weapon while he is at stand or crouch hide cover points even if magazine is not yet empty.")]
            public int ReloadIfAmmoLeftInMagazineIsLessThan = 20;

            [Tooltip("Minimum distance To cancel Stop and shoot behaviour if the distance with the cover is less than or equal to the value specified in this field.")]
            public float MinStopAndShootCancelDistanceToCover = 2f;
            [Tooltip("Maximum distance To cancel Stop and shoot behaviour if the distance with the cover is less than or equal to the value specified in this field.")]
            public float MaxStopAndShootCancelDistanceToCover = 4f;

            [Tooltip("Fields in this subsection are setting Ai agent's weapon firing behaviour while he is moving towards or between covers. " +
                 "This subsection is reused in 3 different combat related paragraphs.Those are - Ai Charging, Ai Covers and Ai firing points. " +
                 "And in each of those cases this subsection is defining Open Fire Behaviour in relation to parent paragraphs subject. " +
                 "In 'Ai charging' paragraph fields will define shooting behaviour in relation to enemies. In case of 'AI Covers' paragraph those fields are used in relation to covers. " +
                 "And in 'Ai Firing Points' paragraph in relation to Firing points." +
                " Value in this field is related to cover instead of enemy or the Fire Point.")]
            public OpenFireBehaviourClassForWaypoint OpenFireBehaviour;
        }

        float RandomStopAndShootCancelDistanceToCover;
        float RandomStopAndShootCancelDistanceToFiringPoint;

        [Tooltip("Fields in this section will take effect if AI agent will be structured the way that allows him take covers during combat." +
            "i.e If Ai agent is not configured to be stationary and 'Use Covers' checkbox is checked." +
            "Ai Agent detects cover assemblies by the colliders of their child cover points. " +
            "And then creates the list of all covers within specified range. And then does requests to cover assemblies of interest during the combat state.")]
        public AiCoverClass AiCovers = new AiCoverClass();

        [System.Serializable]
        public class FiringPointClass
        {
            [Tooltip("Fields in this subsection will allow tweaking for AI agent to move towards the firing point.")]
            public FiringPointBehaviourClass FiringPointDetectionBehaviour;
            [Tooltip("Fields in this subsection will allow Ai agent to sprint towards the firing point in case structured that way.")]
            public SprintingClassForFiringBot SprintingBehaviour;
            [Tooltip("Fields in this subsection are setting Ai agent's weapon firing behaviour while he is moving towards or between covers. " +
                "This subsection is reused in 3 different combat related paragraphs.Those are - Ai Charging, Ai Covers and Ai firing points. " +
                "And in each of those cases this subsection is defining Open Fire Behaviour in relation to parent paragraphs subject. " +
                "In 'Ai charging' paragraph fields will define shooting behaviour in relation to enemies. In case of 'AI Covers' paragraph those fields are used in relation to covers. " +
                "And in 'Ai Firing Points' paragraph in relation to Firing points.")]
            public OpenFireBehaviourClassForWaypoint OpenFireBehaviour;
        }

        float RandomFiringPointSprint;
        float RandomTimeToKeepShootWhileMovingBetweenWaypoint;
        float RandomTimeToActivateShootWhileMovingBetweenWaypoint;
        float RandomDistanceForShootWhileMovingBetweenWaypoint;

        bool StartShootingNowForWaypoint = false;
        bool StopMovingBetweenWaypoint = false;

        bool StartShootingNowForOpenfireCover = false;
        bool StopMovingBetweenCovers = false;

        

        [System.Serializable]
        public class FiringPointBehaviourClass
        {
            [Tooltip("Drag and drop the child gameObject of this Ai agent which have a script named attached called 'FindFiringPoints' from the hierarchy into this field.")]
            public FindFiringPoints FiringPointsFinder;

            [Tooltip("If enabled than the Ai agent will find the closest firing point from his current position. " +
                "In case if this checkbox is disabled than Ai agent will take random firing point within the Range specified in the field name" +
                " 'RangeToFindAFiringPoint' ")]
            public bool FindClosestFiringPoint = true;

            [Tooltip("The slider value below indicate what would be the probability of the Ai agent to switch new firing point." +
                     " In case the value in the slider is 100 than the Ai agent will switch between different firing point around him. In case this value is 0 than the Ai agent" +
                     " will not switch between any new firing point after taking his first firing point and will make sure to stay in his first firing point only for the duration of combat." +
                     " Depending on the probability Ai agent will decide whether to pick a new firing point or stay at the currently picked one.")]
            [Range(0, 100)]
            public int SwitchingFiringPointsProbability = 100;

            [HideInInspector]
            public bool SwitchBetweenFiringPoints;

            [Tooltip("A Radius within which Ai agent can find random firing point from its current position to be able to move around.")]
            public float RangeToFindAFiringPoint = 30f;
            [Tooltip("Minimum time needed to find a random firing point from the current position within the range defined in 'Range to Find Firing a Point' field.")]
            public float MinTimeBetweenFiringPoints = 3f;
            [Tooltip("Maximum time needed to find a random firing point from the current position within the range defined in 'Range to Find Firing a Point' field.")]
            public float MaxTimeBetweenFiringPoints = 5f;

            [Tooltip("Distance to stop before occupying a valid firing point.")]
            public float DistanceToStopBeforeFiringPoint = 0.4f;

            [Tooltip("Minimum distance To cancel Stop and shoot behaviour if the distance with the cover is less than or equal to the value specified in this field.")]
            public float MinStopAndShootCancelDistanceToFiringPoint = 2f;
            [Tooltip("Maximum distance To cancel Stop and shoot behaviour if the distance with the cover is less than or equal to the value specified in this field.")]
            public float MaxStopAndShootCancelDistanceToFiringPoint = 4f;
        }
        [System.Serializable]
        public class OpenFireBehaviourClassForWaypoint
        {
            [Range(0, 100)]
            [Tooltip("This slider sets the probability of 'Stop And Shoot' behaviour while Ai agent is moving towards or between covers.")]
            public int StopAndShootProbability = 100;
            [Range(0, 100)]
            [Tooltip("This slider sets the probability of 'Strafing' behaviour while Ai agent is moving towards or between covers.")]
            public int StrafingProbability = 100;

            [Tooltip("Minimal distance towards the cover to activate Stop&Shoot behaviour.")]
            public float MinStopAndShootDistanceToEnemyOrToCover = 300f;
            [Tooltip("Maximum distance towards the cover to activate Stop&Shoot behaviour.")]
            public float MaxStopAndShootDistanceToEnemyOrToCover = 300f;

            [Tooltip("Minimum time since Ai agent decided to go towards or between covers till he starts Stop&Shoot behaviour.")]
            public float MinTimeTillStopAndShootBehaviour = 3f;
            [Tooltip("Maximum time since Ai agent decided to go towards or between covers till he starts Stop&Shoot behaviour.")]
            public float MaxTimeTillStopAndShootBehaviour = 3f;

            [Tooltip("Minimum duration in seconds of the Stop&Shoot behaviour.")]
            public float MinStopAndShootDuration = 3f;
            [Tooltip("Maximum duration in seconds of the Stop&Shoot behaviour.")]
            public float MaxStopAndShootDuration = 5f;
        }

        [System.Serializable]
        public class SprintingClassForWaypointBot
        {
            [Tooltip("If enabled Ai agent will be allowed to sprint towards the destination")]
            public bool EnableSprinting = true;

            public float MinRemainingDistanceToTargetToStopSprinting = 20f;
            public float MaxRemainingDistanceToTargetToStopSprinting = 30f;
        }
        [System.Serializable]
        public class SprintingClassForFiringBot
        {
            [Tooltip("If enabled Ai agent will be allowed to sprint towards the destination")]
            public bool EnableSprinting = true;
            [Tooltip("Minimum distance to stop sprinting towards the firing point.")]
            public float MinRemainingDistanceToFiringPointToStopSprinting = 20f;
            [Tooltip("Maximum distance to stop sprinting towards the firing point.")]
            public float MaxRemainingDistanceToFiringPointToStopSprinting = 30f;
        }

        [Tooltip("Fields in this section will take effect if AI agent will be structured the way that allows him to move between Firing points.")]
        public FiringPointClass AiFiringPoints = new FiringPointClass();

        [System.Serializable]
        public class Commander
        {
            [HideInInspector]
            [Tooltip("This field represents how closer the Ai agent will stay with the commander.")]
            public float ClosestDistanceToRun = 5f;
            [HideInInspector]
            [Tooltip("This field represents how closer the Ai agent will stay with the commander.")]
            public float StoppingDistanceToCommander = 5f;
            [HideInInspector]
            [Tooltip("In case if the distance with the commander gets bigger than the distance specified in this field than the Ai agent will start sprinting towards " +
                "the commander and will try to reach closer to the commander as close as the distance specified in the field name 'Maintain Distance With Commander'.")]
            public float DistanceToSprintToCommanderIfHeIsFurtherThan = 10f;


            [HideInInspector]
            [Tooltip("Minimum distance between the leader and this Ai agent to maintain.")]
            public float MinStoppingDistanceToLeader = 4f;

            [HideInInspector]
            [Tooltip("Maximum distance between the leader and this Ai agent to maintain.")]
            public float MaxStoppingDistanceToLeader = 10f;

            [HideInInspector]
            [Tooltip("Minimum distance before start running towards commander.")]
            public float MinRunningDistanceToLeader = 3f;

            [Tooltip("Maximum distance before start running towards commander.")]
            [HideInInspector]
            public float MaxRunningDistanceToLeader = 6f;


            [HideInInspector]
            [Tooltip("Minimum distance before start walking towards commander.")]
            public float MinWalkingDistanceToLeader = 7f;
            [Tooltip("Maximum distance before start walking towards commander.")]

            [HideInInspector]
            public float MaxWalkingDistanceToLeader = 10f;

            [HideInInspector]
            [Tooltip("Minimum distance to trigger sprinting towards the commander if the distance becomes greater.")]
            public float MinSprintingDistanceToLeader = 20f;

            [HideInInspector]
            [Tooltip("Maximum distance to trigger sprinting towards the commander if the distance becomes greater.")]
            public float MaxSprintingDistanceToLeader = 25f;
        }

        [HideInInspector]
        public Commander FollowCommanderValues = new Commander();


        [System.Serializable]
        public class strafeclass
        {
            [Tooltip("Select the animation to play when Ai agent is moving forward,backward,right,left etc..in combat state.")]
            public StrafeDirections StrafeAnimations;

            [Tooltip("If enabled Ai agent will move between Predefined Custom strafe directions specified in the field 'Custom Strafe Directions'." +
                "This becomes very useful when you want the Ai agent to strafe only in one or two directions for example : strafing right or left")]
            public bool ForceCustomStrafing;

            [Tooltip("If enabled than Ai agent will have 50/50 chance of deciding whether to strafe and shoot and stop and shoot")]
            public bool RandomizeBetweenStopOrStrafeCycles = true;

            public float ClosetStoppingDistanceToStrafePoint = 2f;
            //public bool RandomiseBetweenProceduralAndCustomStrafing = false;
            //public float MinTimeToRandomiseBetweenProceduralAndCustomStrafe = 4f;
            //public float MaxTimeToRandomiseBetweenProceduralAndCustomStrafe = 8f;
            [Tooltip("Minimum time to perform either Stop and shoot Or Strafe and shoot cycles in case checkbox named 'RandomizeBetweenStopOrStrafeCycles' is enabled.")]
            public float MinTimeBetweenStopOrStrafeCycles = 4f;
            [Tooltip("Maximum time to perform either Stop and shoot Or Strafe and shoot cycles in case checkbox named 'RandomizeBetweenStopOrStrafeCycles' is enabled.")]
            public float MaxTimeBetweenStopOrStrafeCycles = 8f;

            //[Tooltip("Minimum time to check for direction to play the correct strafe animation")]
            //public float MinTimeBetweenStrafeDirectionChecks = 0.1f;
            //[Tooltip("Maximum time to check for direction to play the correct strafe animation")]
            //public float MaxTimeBetweenStrafeDirectionChecks = 0.3f;
            [Tooltip("Minimum time to create a new strafe coordinate within the range specified above.")]
            public float MinTimeTillNextStrafeDirection = 3f;
            [Tooltip("Maximum time to create a new strafe coordinate within the range specified above.")]
            public float MaxTimeTillNextStrafeDirection = 5f;
            [Tooltip("Combines shooting and strafing functionalities.")]
            public bool EnableShootingBetweenStrafing = true;
            [Tooltip("Enables strafing AI agent to have periods of aiming without actual shooting between shooting cycles. Otherwise AI agent will continuously shoot at it`s target until this target is destroyed or no more ammo left in the magazine, " +
                "in which case AI agent will reload his weapon and continue firing at it`s target.")]
            public bool EnableShootingCycles;

            [Range(0, 100)]
            [Tooltip("Adjust the slider value and decide what should be the shooting probability of this Ai agent i.e" +
                " The bigger the number it will have the higher the chances of the Ai agent to consider shooting and the smaller number will result in less chance of shooting.")]
            public float ShootingProbability = 50f;

            [Tooltip("Minimal possible period of time for strafing and aiming weapon between shooting cycle's (i.e without actually firing it).")]
            public float MinTimeBetweenShootingCycles = 4f;
            [Tooltip("Maximal possible period of time for strafing and aiming weapon between shooting cycle's (i.e without actually firing it) .")]
            public float MaxTimeBetweenShootingCycles = 8f;
        
            [Tooltip("Minimal raycast checks to do if target is not in view during strafing.")]
            public float MinRaycastWhenEnemyLost = 20f;

            [Tooltip("Maximum raycast checks to do if target is not in view during strafing.")]
            public float MaxRaycastWhenEnemyLost = 30f;

            [Tooltip("Minimum distance to stop charging at enemy.")]
            public float MinDistanceToStopChargingAtEnemy = 4;

            [Tooltip("Maximum distance to stop charging at enemy.")]
            public float MaxDistanceToStopChargingAtEnemy = 6;

            [Tooltip("Custom Strafing is based on a set of custom strafing points that allows to limit overall number and choice of available strafing directions for AI agent. ")]
            public CustomStrafeClass CustomStrafing;
            [Tooltip("This solution provides 360 strafing directions to AI agent. Actual distances of those directions will be also limited by 'Min/Max time Till Next Strafe Direction' timer same as custom strafe directions. " +
                "Procedural strafing is more preferable for cases when AI agent is situated on small isolated Navmesh pieces as it will still allow AI agent" +
                " to strafe within limited ranges like balcony, watch tower or a small roof or any enclosure of a small space.")]
            public ProceduralStrafeClass ProceduralStrafing;
           
            // public FriendliesStrafingDistancingValues FriendliesDistancingValues;
            //[Tooltip("On Each of the 'Custom Strafe Direction' specified above a coordinate will be created for the Ai agent to move using the " +
            //    "radius specified in this field.If the value is 0 than Ai agent will move exactly in the position where the empty gameobject is located.")]
            //public float CustomStrafeDirectionRadius;
        }
        //[System.Serializable]
        //public enum RunAnimationNames
        //{
        //    RunForward,
        //    RunLeft,
        //    RunRight,
        //    RunBackward,
        //    RunForwardRight,
        //    RunForwardLeft,
        //    RunBackwardRight,
        //    RunBackwardLeft,
        //}

        [System.Serializable]
        public enum MovementAnimations
        {
            WalkForward,
            WalkLeft,
            WalkRight,
            WalkBackward,
            WalkForwardRight,
            WalkForwardLeft,
            WalkBackwardRight,
            WalkBackwardLeft,
            RunForward,
            RunLeft,
            RunRight,
            RunBackward,
            RunForwardRight,
            RunForwardLeft,
            RunBackwardRight,
            RunBackwardLeft
        }
        [System.Serializable]
        public class StrafeDirections
        {
            [Tooltip("Choose the animation to play during Forward strafing")]
            public MovementAnimations Forward;

            [Tooltip("Choose the animation to play during Backward strafing")]
            public MovementAnimations Backward;

            [Tooltip("Choose the animation to play during Right strafing")]
            public MovementAnimations Right;

            [Tooltip("Choose the animation to play during Left strafing")]
            public MovementAnimations Left;

            [Tooltip("Choose the animation to play during Forward Right strafing")]
            public MovementAnimations ForwardRight;

            [Tooltip("Choose the animation to play during Forward Left strafing")]
            public MovementAnimations ForwardLeft;

            [Tooltip("Choose the animation to play during Backward Right strafing")]
            public MovementAnimations BackwardRight;

            [Tooltip("Choose the animation to play during Backward Left strafing")]
            public MovementAnimations BackwardLeft;
        }



        [System.Serializable]
        public class ProceduralStrafeClass
        {


            [Tooltip("Minimum range to create a strafe coordinate for the Ai agent to move from its current position.")]
            public float MinStrafeRange = 2.5f;
            [Tooltip("Maximum range to create a strafe coordinate for the Ai agent to move from its current position.")]
            public float MaxStrafeRange = 4;

        }
        [System.Serializable]
        public class CustomStrafeClass
        {
            [Tooltip("This section stores all custom strafe points that represent their respective directions and define maximal possible strafing ranges (that can be limited by 'Min/Max time Till Next Strafe Direction' timer) by their actual distances from AI agent`s root game object. " +
                "Number of custom strafe points added to AI agent can be as low as one or two or all the way up to eight. ")]
            public Transform[] StrafePoints;

        }
        //[System.Serializable]
        //public class FriendliesStrafingDistancingValues
        //{
        //    //[Tooltip("Minimum range to create a strafe coordinate for the Ai agent to move from its current position.")]
        //    //public float MinStrafeAwayDistance = 2.5f;
        //    //[Tooltip("Maximum range to create a strafe coordinate for the Ai agent to move from its current position.")]
        //    //public float MaxStrafeAwayDistance = 4;
        //    //[Tooltip("Minimum time to create a new strafe coordinate within the range specified above.")]
        //    //public float MinTimeBetweenStrafeAway = 3f;
        //    //[Tooltip("Maximum time to create a new strafe coordinate within the range specified above.")]
        //    //public float MaximumTimeBetweenStrafes = 5f;
        //}


        [Tooltip("This paragraph is responsible for AI agent`s strafing behaviour of two kinds.Procedural strafing (in all directions) and Custom strafing (to limit strafing within a few specified directions)." +
            " Once tweaked - Strafing will then take place in combat situations for enemy fire evasion as well as friendlies distancing behaviour to prevent bunching up and getting in friendly line of fire. ")]
        public strafeclass AiStrafing = new strafeclass();

        [System.Serializable]
        public class Emergency
        {
            public EmergencyInvestigationclass EmergencyAlert = new EmergencyInvestigationclass();

        }
        
        [Tooltip("This paragraph is responsible for Ai agent behaviour in life threatening situations where he cannot detect the source of the threat and" +
            " takes several measures to save his life and locate the threat. Most common emergency state triggering situations are when Ai agent will hear bullet impacts " +
            "near him or will hear dying sound of friendly Ai agent or when he will see another friendly Ai agent in emergency state. " +
            "All those conditions will result in Emergency behaviour that Ai agent will execute in a various forms depending on two major conditions. " +
            "When Ai agent gets into Emergency state he will be provided with the coordinate of the supposed enemy position. After that Navmesh path is calculated towards this coordinate." +
            "If navmesh path is complete then Ai agent will hide behind available emergency covers for specified time and when this hiding time is expired then Ai agent will advance between " +
            "emergency covers towards supposed enemy position.If there are no more available emergency covers left for Ai agent on his way to supposed enemy position then Ai agent will start " +
            "run in a zigzag pattern towards those coordinates to increase survival chances.If navmesh path is incomplete towards supposed enemy position(i.e if supposed enemy coordinate " +
            "is on the roof of a different building or on the other side of the river etc.) then Ai agent will perform Emergency hiding and shooting in a general direction of the supposed enemy position." +
            "This behaviour can be set up that way so that Ai agent could switch emergency covers or stay at the first occupied emergency cover without ever leaving it.")]
        public Emergency AiEmergencyState = new Emergency();

        [Tooltip("This subsection is for the case when AI agent finds dead body of a friendly AI agent, that was already there for some time(if it only has active 'Dead Body Radius' collider).")]
        public AiDeadBodyInvestigationclass AiDeadBodyAlerts = new AiDeadBodyInvestigationclass();

        [System.Serializable]
        public class AiDeadBodyInvestigationclass
        {
            [Tooltip("Drag and drop an empty gameobject named 'Dead Body Radius' with a sphere collider attached to it into this field. " +
                "Enable 'Is Trigger' on that sphere collider and tweak its 'Radius' field value to specify how large this trigger should be." +
                "if any friendly Ai agent collides with this trigger than it will immediately enter into Dead Body Investigation state," +
                " Which in turn will activate its own Dead Body Investigation state to notify nearby friendlies of current Investigation.")]
            public GameObject InvestigationAlertRadius;
            [Tooltip("A value within which Ai agent will investigate from the" +
               " point where the dead body is found (for example : if the value is 10 than the Ai agent will investigate within 10 meteres " +
               "radius from the point where the dead body was located).")]
            public float MinInvestigationRadiusFromTheDeadBody = 40f;
            [Tooltip("A value within which Ai agent will investigate from the" +
            " point where the dead body is found (for example : if the value is 10 than the Ai agent will investigate within 10 meteres " +
            "radius from the point where the dead body was located).")]
            public float MaxInvestigationRadiusFromTheDeadBody = 40f;

            public float MinStoppingDistanceFromTheDeadBody = 2f;
            public float MaxStoppingDistanceFromTheDeadBody = 6f;
            [Tooltip("This field determines how long Ai agent will stay near the dead body.")]
            public float MinTimeToStayNearDeadBody = 2f;
            [Tooltip("This field determines how long Ai agent will stay near the dead body.")]
            public float MaxTimeToStayNearDeadBody = 2f;

            [Tooltip("This field determines how long it will take Ai agent to activate the investigation radius on top of him.")]
            public float MinDeadBodyInvestigationRadiusActivationDelay;
            [Tooltip("This field determines how long it will take Ai agent to activate the investigation radius on top of him.")]
            public float MaxDeadBodyInvestigationRadiusActivationDelay;
        }

        float StoppingDistanceFromTheDeadBody;

        [System.Serializable]
        public enum ChooseEmergencySprintState
        {
            ZigZagAdvancing,
            AdvancingBetweenCoversAndZigZagAdvancing,
            EmergencyShooting,
            ChooseRandomly
        }

        [System.Serializable]
        public class NoEmergencyCoverAvailablityOptionClass
        {
            [Tooltip("Set of points with their respective names matching their general flee directions (left, right, backward) and tweakable parametric Min/Max radius is provided for Ai agent in this subsection." +
                "Note that if Min / Max radius values are high enough then Ai agent will be able to flee to what appears as an opposite direction of chosen flee point." +
                "For example with radius value of the left flee point being bigger than the separation distance of left flee point from Ai agent will result in possibility " +
                "for Ai agent to flee to the right even though left point is situated to the actual left side of the Ai agent.This is very flexible fleeing behaviour construct that allow users " +
                "to achieve great deal of customisation.You can get away even with oneFlee point that is located right at the origin of Ai agent and has very small value for minimal radius and very big " +
                "value for maximum radius.Thus allowing Ai agent to flee in any direction or distance within 360 degrees angle.")]
            public Transform[] FleeDestinations;
            [Tooltip("Minimum  duration in seconds for Ai Agent to keep sprinting away in case no emergency cover is available to Ai agent at the beginning of Emergency State.")]
            public float MinFleeDuration;
            [Tooltip("Maximum  duration in seconds for Ai Agent to keep sprinting away in case no emergency cover is available to Ai agent at the beginning of Emergency State.")]
            public float MaxFleeDuration;

            [Tooltip("Minimal Emergency sprint coordinate value for Ai agent to sprint towards if no Emergency cover is found at the beginning of Emergency state.")]
            public float MinEmergencySprintAwayRadius;
            [Tooltip("Maximum Emergency sprint coordinate value for Ai agent to sprint towards if no Emergency cover is found at the beginning of Emergency state.")]
            public float MaxEmergencySprintAwayRadius;

            [Tooltip("Minimum time between crouch shooting.")]
            public float MinTimeBetweenCrouchShooting;

            [Tooltip("Maximum time between crouch shooting.")]
            public float MaxTimeBetweenCrouchShooting;

            [Range(0f, 100f)]
            [Tooltip("Sets the probability of Shooting and Flee away during emergency state")]
            public float FleeAwayProbability = 50f;
        }

        [System.Serializable]
        public class EmergencyInvestigationclass
        {
            [Tooltip("Drag and drop Emergency covers finder game object with 'Find Emergency Covers' script attached to it from this Ai agents hierarchy. ")]
            public FindEmergencyCovers EmergencyCoverFinderComponent;

            [Tooltip("Drag and drop Emergency covers finder game object with 'Emergency Alert Radius' script attached to it from this Ai agents hierarchy. ")]
            public EmergencyAlertRadius EmergencyAlertRadiusComponent;

            int Randomise;
            [Tooltip("In case this checkbox is active than Ai agent will find closest emergency covers during emergency state.")]
            public bool ChooseClosestEmergencyCovers = true; 

            [Tooltip("Specifies the minimum emergency covers detection radius.")]
            public float MinRangeToFindEmergencyCover = 50f;
            [Tooltip("Specifies the maximum emergency covers detection radius.")]
            public float MaxRangeToFindEmergencyCover = 50f;

            [Tooltip("Minimum time to hide behind emergency cover before beginning but not during advancing towards supposed enemy position.")]
            public float MinTimeBehindEmergencyCover = 20f;
            [Tooltip("Maximum time to hide behind emergency cover before beginning but not during advancing towards supposed enemy position.")]
            public float MaxTimeBehindEmergencyCover = 30f;

            [Tooltip("Minimal Emergency Cover occupation distance reaching which Ai agent will stop moving towards it and start hiding behaviour.")]
            public float MinEmergencyCoverOccupationDistance = 0.3f;
            [Tooltip("Maximum Emergency Cover occupation distance reaching which Ai agent will stop moving towards it and start hiding behaviour.")]
            public float MaxEmergencyCoverOccupationDistance = 1f;

            [Tooltip("Minimum amount in seconds of Ai agent's Emergency Alert Radius Trigger Activation  delay.")]
            public float MinEmergencyAlertRadiusActivationDelay;
            [Tooltip("Maximum amount in seconds of Ai agent's Emergency Alert Radius Trigger Activation  delay.")]
            public float MaxEmergencyAlertRadiusActivationDelay;

            [Tooltip("The duration, in seconds, it takes for the AI agent to rotate and focus on emergency cover or shooting points during emergency state." +
                      "A lower value indicates a faster rotation, while a higher value results in a slower rotation.")]
            public float PoseTransitionDuration = 0.3f;

            [Tooltip("Select any of the options from this drop down list that contains various behaviours for Ai agent to perform after expiration of the initial Emergency cover occupation timer.")]
            public ChooseEmergencySprintState PostFirstEmergencyCoverBehaviourActivity;

            [Tooltip("This Subsection regulates the parameters of fleeing behaviour when Ai agent sprints away from danger zone when find himself in the emergency situation.")]
            public NoEmergencyCoverAvailablityOptionClass AiFleeingBehaviour;

            [Tooltip("Fields of this subsection are regulating various aspects of Ai agent's advancing between covers towards enemy supposed coordinates.")]
            public AdvancingBetweenCoversAndZigZagAdvancingClass AdvancingBetweenCovers;

            [Tooltip("This subsection sets the coordinate for the final destination of zigzag sprinting and regulates the amplitude of the zigzag sprinting pattern.")]
            public ZigZagAdvancingClass ZigZagAdvancing;

            [Tooltip("This subsection regulates various aspects of Emergency Shooting behaviour which if activated will make Ai agent to fire his " +
                "weapon in general direction of the enemy situated outside of enemy detection parameters of this Ai agent.")]
            public EmergencyShootingClass EmergencyShooting;

            //[Tooltip("Minimum time to find a new emergency cover from the current position.")]
            //public float MinimumTimeBetweenEmergencyCover = 4f;
            //[Tooltip("Maximum time to find a new emergency cover from the current position.")]
            //public float MaximumTimeBetweenEmergencyCover = 6f;
        }
        [System.Serializable]
        public class EmergencyShootingClass
        {
            //[Tooltip("In case no emergency covers are available and navmesh path to enemy supposed position point is incomplete then Ai agent " +
            //    "will default to Crouch firing at the reappearing offset points of the enemy position. Checking this checkbox will limit the rotation to Ai agent's Upper Body. " +
            //    "If left unchecked than whole body of Ai agent will be procedurally turned towards offset points.")]
            //public bool UpperBodyOnlyCrouchRotations = true;

            [Tooltip("Minimal horizontal and Vertical targeting offset values.")]
            public Vector2 MinEmergencyShootingOffset = new Vector3(-100f, 0f);
            [Tooltip("Maximum horizontal and Vertical targeting offset values.")]
            public Vector2 MaxEmergencyShootingOffset = new Vector3(100f, 0f);

            [Tooltip("Minimum amount of time to create new target offset point for Ai agent to shoot at.")]
            public float MinTimeBetweenTargetOffsetPoints = 2f;
            [Tooltip("Maximum amount of time to create new target offset point for Ai agent to shoot at.")]
            public float MaxTimeBetweenTargetOffsetPoints = 2f;

            [Tooltip("Minimum Time to either switch back to the same/previous emergency cover Or take a new one.")]
            public float MinEmergencyCoverLoopOrSwitchTime = 8f;
            [Tooltip("Maximum Time to either switch back to the same/previous emergency cover Or take a new one.")]
            public float MaxEmergencyCoverLoopOrSwitchTime = 8f;

            [Tooltip("Minimum Time to keep shooting after occupying the emergency open fire point.")]
            public float MinTimeToEmergencyShooting = 5f;
            [Tooltip("Minimum Time to keep shooting after occupying the emergency open fire point.")]
            public float MaxTimeToEmergencyShooting = 6f;

            [Range(0f,100f)]
            [Tooltip("Sets the probability of switching emergency covers in case navmesh path to enemy supposed position is incomplete and there are more then one emergency covers available.")]
            public float EmergencyCoverSwitchingProbability = 50f;

            [Tooltip("Sets the interval between checks for vacant Emergency covers for Ai agent that found all detected Emergency Covers to be occupied by his friendlies. " +
                "The smaller the value in this field will be the faster uncovered Ai agent will be able to occupy Emergency Covers that became vacant.(to be expanded)")]
            public float MinVacantEmergencyCoverCheckInterval = 3f;
            [Tooltip("Sets the interval between checks for vacant Emergency covers for Ai agent that found all detected Emergency Covers to be occupied by his friendlies. " +
                "The smaller the value in this field will be the faster uncovered Ai agent will be able to occupy Emergency Covers that became vacant.(to be expanded)")]
            public float MaxVacantEmergencyCoverCheckInterval = 5f;

        }


        [System.Serializable]
        public class AdvancingBetweenCoversAndZigZagAdvancingClass
        {

            [Tooltip("Minimum amount of covers for Ai Agent to use on his way towards enemy supposed position coordinate.")]
            public int MinEmergencyAdvancingCovers = 2;
            [Tooltip("Maximum amount of covers for Ai Agent to use on his way towards enemy supposed position coordinate.")]
            public int MaxEmergencyAdvancingCovers = 3;

            [Tooltip("Minimal viable distance to next emergency cover for Ai agent to be able to occupy on his way towards enemy supposed position.")]
            public float MinDistanceTowardsNextCover = 10f;
            [Tooltip("Maximum viable distance to next emergency cover for Ai agent to be able to occupy on his way towards enemy supposed position.")]
            public float MaxDistanceTowardsNextCover = 20f;

            [Tooltip("Minimum time spend behind each next Emergency Cover on the way to enemy supposed position.")]
            public float MinTimeToRestBehindCovers = 3f;
            [Tooltip("Maximum time spend behind each next Emergency Cover on the way to enemy supposed position.")]
            public float MaxTimeToRestBehindCovers = 3f;
        }

        [System.Serializable]
        public class ZigZagAdvancingClass
        {
            [Tooltip("Drag and Drop an empty gameobject which will rotate and look at the enemy to generate a coodinate towards the path where the enemy is located. this will make sure that the coordinate" +
                   " always created towards the enemy path to make it look natural. this field will also make sure that the coordinate generated is in the forward direction and towards the enemy path.")]
            public GameObject CoordinateCreator;

            [Tooltip("Minimum radius to create a coordinate from the enemy position")]
            public float MinEnemySupposedPositionOffset = 1f;
            [Tooltip("Maximum radius to create a coordinate from the enemy position")]
            public float MaxEnemySupposedPositionOffset = 1f;

            [Tooltip("Specify how many coordinates to generate when moving towards the enemy. the coordinates generated will be in a V-Direction and the Ai agent has to reach these generated coordinates one by one" +
                " which will result in a zig zag movement. Once all the coordinates are reached than the Ai agent will move directly towards the coordinate created around the enemy.")]
            public List<ZigZagMovement> ZigzagTurnsAmount  = new List<ZigZagMovement>();
        }


        [System.Serializable]
        public class ZigZagMovement
        {
            [Tooltip("Sets the minimal 'inner' radius  for next zigzag point creation distance that can not be at a lesser range than this radius value.")]
            public float MinZigZagPointCreationRadius;
            [Tooltip("Sets the maximum 'inner' radius  for next zigzag point creation distance that can not be at a lesser range than this radius value.")]
            public float MaxZigZagPointCreationRadius;
        }

        float RandomDistanceWithEmergencyCoverToCheck;

        [HideInInspector]
        public float FovForZigZagMovements = 120f;

        [System.Serializable]
        public class Charging
        {
            public float MinNavMeshPathCheckTimeWhenDestinationIsComplete;
            public float MaxNavMeshPathCheckTimeWhenDestinationIsComplete;

            public float MinNavMeshPathCheckTimeWhenDestinationIsInComplete;
            public float MaxNavMeshPathCheckTimeWhenDestinationIsInComplete;

            [Tooltip("If unchecked AI agent will perform assault by Walking/Running/Sprinting and shooting its target.")]
            public bool EnableSprinting = true;

            [Tooltip("Minimal remaining distance to pursue point for the switch from walking or running to sprinting towards it.")]
            public float MinSprintDistance = 20f;

            [Tooltip("Maximum distance for sprinting towards coordinate.")]
            public float MaxSprintDistance = 100f;

            [Tooltip(" Minimal remaining distance to pursue point for the switch from walking or sprinting to running towards it.")]
            public float MinRunDistance = 50f;

            [Tooltip("Maximal remaining distance to pursue point for the switch from walking or sprinting to running towards it.")]
            public float MaxRunDistance = 100f;

            [Tooltip("Minimal closest distance to stop.")]
            public float MinClosestDistanceToStop = 5f;
            [Tooltip("Maximal closest distance to stop.")]
            public float MaxClosestDistanceToStop = 7f;

            [Tooltip("Fields in this subsection are setting Ai agent's weapon firing behaviour while he is moving towards or between covers. " +
                "This subsection is reused in 3 different combat related paragraphs.Those are - Ai Charging, Ai Covers and Ai firing points. " +
                "And in each of those cases this subsection is defining Open Fire Behaviour in relation to parent paragraphs subject. " +
                "In 'Ai charging' paragraph fields will define shooting behaviour in relation to enemies. In case of 'AI Covers' paragraph those fields are used in relation to covers. " +
                "And in 'Ai Firing Points' paragraph in relation to Firing points.")]
            public OpenFireBehaviourClassForWaypoint OpenFireBehaviour;

            [Tooltip("Values in this section will display the distance to target as well as random value between Min/Max SprintDistance and RunDistance")]
            public DebugChargeDistancesClass DebugDistances;
        }
        [System.Serializable]
        public class DebugChargeDistancesClass
        {
            [ReadOnly][Tooltip("Display the current distance to target")]
            public float DebugDistanceToTarget = 0f;

            [ReadOnly]
            [Tooltip("Display the random value of Min/Max SprintDistance")]
            public float DebugSprintDistance = 0f;

            [ReadOnly]
            [Tooltip("Display the random value of Min/Max RunDistance")]
            public float DebugRunDistance = 0f;
           
        }
        //[System.Serializable]
        //public class OpenFireBehaviourClass
        //{
        //    [Tooltip("If enabled it will let the AI agent to stop and shoot at target for specified time during charge.")]
        //    public bool EnableStopAndShoot = true;
        //    [Tooltip("Minimal value for firing distance to target during assault to set the shortest limit for random value. " +
        //        "The greater resulted random value will be, the earlier AI agent will shoot during attack.")]
        //    public float MinOpenFireDistance = 300f;
        //    [Tooltip("Maximal value for firing distance to target during assault to set he longest limit for random value. " +
        //        "The greater resulted random value will be, the earlier AI agent will shoot during attack.")]
        //    public float MaxOpenFireDistance = 300f;
        //    [Tooltip("Minimal possible time interval between stopping and shooting")]
        //    public float MinTimeIntervalToActivateStopAndShootBehaviour = 3f;
        //    [Tooltip("Maximum possible time interval between stopping and shooting")]
        //    public float MaxTimeIntervalToActivateStopAndShootBehaviour = 3f;
        //    [Tooltip("Gives 50/50 chance for AI agent to stop and shoot along the way to it`s target.")]
        //    public bool RandomlyActivateStopAndShootBehaviour = false;
        //    [Tooltip("Minimal possible time for shooting at each stop. Each new stop will have random value between min/max possible values.")]
        //    public float MinTimeToKeepShooting = 3f;
        //    [Tooltip("Maximum possible time for shooting at each stop. Each new stop will have random value between min/max possible values.")]
        //    public float MaxTimeToKeepShooting = 5f;
        //}

        [System.Serializable]
        public class ZombieChargeClass
        {
            // By Default if the distance between Target and Zombie Ai is greater than the running distance than the zombie automatically sprints whereas in the case of Ai agent agent its a crucial field as he hold
            // the shooting weapon in his hand so in that case he can shoot from far distance but for zombie sprinting comes as a default factor.

            //[Tooltip("Minimal value for Checking Distance before starting sprinting towards the target.")]
            //public float MinDistanceToSprinting = 20f;
            //[Tooltip("Maximal value for Checking Distance before starting sprinting towards the target.")]
            //public float MaxDistanceToSprinting = 30f;

            [Header("***** Sprinting animation will playback if distance exceeds max run distance *****")]

            public float MinNavMeshPathCheckTimeWhenDestinationIsComplete;
            public float MaxNavMeshPathCheckTimeWhenDestinationIsComplete;

            public float MinNavMeshPathCheckTimeWhenDestinationIsInComplete;
            public float MaxNavMeshPathCheckTimeWhenDestinationIsInComplete;

            [Tooltip("Minimal value for Checking Distance before starting running towards the target.")]
            public float MinDistanceToRun = 20f;
            [Tooltip("Maximal value for Checking Distance before starting running towards the target.")]
            public float MaxDistanceToRun = 30f;

            [Tooltip("Minimal value for Checking Distance before starting walking towards the target.")]
            public float MinDistanceToWalk = 20f;
            [Tooltip("Maximal value for Checking Distance before starting walking towards the target.")]
            public float MaxDistanceToWalk = 30f;

        }

        public ZombieChargeClass ZombieCharging;


        float RandomChargeRunningCheck;
        float RandomChargeSprintCheck;
      //  float RandomChargeSprint;
        float RandomTimeToKeepShootBetweenSprint;
        float RandomTimeToActivateShootBetweenSprint;
        float RandomDistanceForShootBetweenSprint;

        bool StartShootingNow = false;
        bool StopMoving = false;


        [Tooltip("This paragraph contains set of parameters that define various aspects of AI agent`s offensive behaviour as he advances towards the enemy.")]
        public Charging AiCharging = new Charging();

        [System.Serializable]
        public enum ListeningTypes
        {
            ListenEverySounds, // Listen to incoming sounds.
            MayOrMayNotListenEverySound, // Choose whether to listen or not listen to sounds.
            DoNotListenSounds // Do not listen to any sounds.
        }

        [System.Serializable]
        public class HearingForBot
        {
            [Range(0,100f)][Tooltip("This slider sets the probability of this AI agent to react to sound alerts.")]
            public float SoundAlertProbability = 100f;

            [Tooltip("If checked then Ai agent will move towards the alerting coordinate. Otherwise he will look in the direction of that sound for specified amount of time.")]
            public bool MoveTowardsSoundCoordinate = true;

            [Tooltip("If checked then AI agent will join his friendly in alerting sound investigation if that friendly affects this agent with his sound alert radius. " +
                "That secondary sound alert radius communicates the sound alert to all agents that got within this radius(even in cases when those agents are having zero " +
                "probability to move out towards initial alerting sound that they heard themselves).")]
            public bool RecieveFriendlySoundCoordinate = true;

            [Tooltip("Reference to the component responsible for sharing sound coordinates.")]
            public ShareSoundCoordinates ShareSoundCoordinatesComponent;

            [Tooltip("Enable/disable sprinting towards sound coordinates.")]
            public bool EnableSprintingTowardsSoundCoordinate = true;

            [Tooltip("Minimal remaining distance to initial sound alert coordinate for the switch from walking or running to sprinting towards it.")]
            public float MinSprintDistance = 20f;

            [Tooltip("Maximal remaining distance to initial sound alert coordinate for the switch from walking or running to sprinting towards it.")]
            public float MaxSprintDistance = 100f;

            [Tooltip("Minimal remaining distance to initial sound alert coordinate for the switch from walking or sprinting to running towards it.")]
            public float MinRunDistance = 50f;

            [Tooltip("Maximal remaining distance to initial sound alert coordinate for the switch from walking or sprinting to running towards it.")]
            public float MaxRunDistance = 100f;

            [Tooltip("Minimal remaining distance to initial sound alert coordinate for the switch from running or sprinting to walking towards it.")]
            public float MinWalkDistance = 20f;

            [Tooltip("Maximal remaining distance to initial sound alert coordinate for the switch from running or sprinting to walking towards it.")]
            public float MaxWalkDistance = 35f;

            [Tooltip("Minimum remaining distance to stop near the initial sound alert coordinate and to consider it as reached.")]
            public float MinNearStoppingDistance = 10f;

            [Tooltip("Maximum remaining distance to stop near the initial sound alert coordinate and to consider it as reached.")]
            public float MaxNearStoppingDistance = 20f;

            [Tooltip("Set the minimum time the AI agent waits at the initial sound alert coordinate before returning to idle state.")]
            public float MinTimeAtSoundAlertPoint = 2f;

            [Tooltip("Set the maximum time the AI agent waits at the initial sound alert coordinate before returning to idle state.")]
            public float MaxTimeAtSoundAlertPoint = 4f;

            [Tooltip("Set the minimum time the Stationary AI agent or agent with zero probability of moving towards initial sound alert coordinate, will look in the direction of the initial sound alert coordinate before returning to idle state.")]
            public float MinTimeToLookAtSoundAlertPoint = 2f;

            [Tooltip("Set the maximum time the Stationary AI agent or agent with zero probability of moving towards initial sound alert coordinate, will look in the direction of the initial sound alert coordinate before returning to idle state.")]
            public float MaxTimeToLookAtSoundAlertPoint = 4f;

            [HideInInspector]
            public float ErrorSoundPercentage;
        }
        [Tooltip("This paragraph contains fields related to AI agent reactions to alerting sounds.")]
        public HearingForBot AiHearing = new HearingForBot();
        //float RandomWalkDistanceTowardsTarget;
        //[Tooltip("Checking If NavMeshPath Is Complete Or Not")]
        //public float MinTimeBetweenCalculatingNavMeshPath = 2f;
        //[Tooltip("Checking If NavMeshPath Is Complete Or Not")]
        //public float MaxTimeBetweenCalculatingNavMeshPath = 3f;


        int[] GetProbability;

        int MyChargingProbability;
        int MyCoverProbability;
        int MyWaypointsProbability;
        int TotalProbability;
        int RandomiseStateProbability;

        bool GetNewState = false;
        [HideInInspector]
        public int[] RangeForElement0 = new int[2];
        [HideInInspector]
        public int[] RangeForElement1 = new int[2];
        [HideInInspector]
        public int[] RangeForElement2 = new int[2];

        bool IsChargingProbabilityDone = false;
        bool IsCoverProbabilityDone = false;
        bool IsWaypointsProbabilityDone = false;

        public bool StartVariations = false;
        //bool GiveOneCheck = false;
        bool CheckForStrafeOrStillShoot = false;
        bool SaveStrafeData = false;

        bool IsStrafingEnabled = false;
        int strafedir;

        //bool StartStrafeAnimationCheck = false;
        bool IntermittentShootRandomisation = false;
        //bool SwitchBetweenProceduralAndCustomStrafe = false;

       
        bool MoveTowardsOpenFirePoint = false;
        bool FindOpenFirePoint = false;
        int OpenFirePointSelected;
        bool OverriteOpenFire = false;
        bool StartStayTimeForOpenFirePoint = false;

        [HideInInspector]
        public Targets T;

        [HideInInspector]
        public bool RefreshStandCoverOnce = false;

        int FindIndexForStandCoverRotationDecider = 0;

        CoverNode StandCoverPositionDeciderNode;

        bool RecheckCovers = false;

        float MinTimeToInitilizeFirstCoverPoint = 0.01f;
        float MaxTimeToInitilizeFirstCoverPoint = 0.02f;
        //bool CanInitializeCover = false;

        Vector3 SavedCurrentPositionForCoverFinding;
        Vector3 SavedCurrentPositionForFiringPointFinding;
        Vector3 SavedCurrentPositionForEmergencyCoverFinding;
        float GetAccurateCoverRange;
        float GetAccurateEmergencyCoverRange;
        float GetAccurateWaypointRange;

        bool ResetPostureTime = false;
        public bool SitShootSelected = false;

        bool AnimatorWeightChange = true;
        public int LayerIndexToCheck;
        bool ShouldTweenNewValues = true;
        [HideInInspector]
        public bool WaypointsFinded = false;
        [HideInInspector]
        public bool WaypointSearched = false;

        public bool StopWeaponShakeTemporary = false;

        private GameObject spawnedTextPrefab; // Reference to the spawned text object
        private TextMeshProUGUI spawnedText; // Reference to the spawned text object

        bool IsStationedShoot = false;

        [HideInInspector]
        public AnimatorLayerWeightController AnimatorLayerWeightControllerScript;

        //bool IsQuickTransitionCompleted = false;
        [HideInInspector]
        public bool OvveruleStandShootPostureAnimation = false;
        bool UsualTask = false;

        [HideInInspector]
        public bool StopShootingPosture = false;

        bool ReInitializeEverything = false;

        float highestRotation = 0f;

        bool ResetHighestRotation = false;

        public bool DoNotLookForCover = false;

        Vector3 DistanceWithCoverAfterSearch;
        //int BestCover;

        [HideInInspector]
        public bool ChangePathToRandom = false;
        bool FindNewPath = false;


        Vector3 NewpointOnNavMesh;
        bool StartCreateRandomPath = false;

        bool CanMoveTowardsRandomPoint = false;

        public float DefaultAgentRadius;

        bool CheckingForCoverAvailability = false;

        [HideInInspector]
        public int PreviousCoverNum = 99999999;

        float LeastVel;

        int RandomNumberReceiver;

        public float RandomOpenFireDistanceDuringCoverBehaviour;
        float RandomTimeToKeepShootWhileMovingBetweenCovers;
        float RandomTimeToActivateShootWhileMovingBetweenCovers;

        [HideInInspector]
        public bool DeregisterCoverNodes = false;
        bool DeregisterEmergencyNodes = false;

        [HideInInspector]
        public bool IsProceduralPointGeneratedForOpenFire = false;

        float RandomSprintingDistanceBetweenCovers;

        //bool StayInOneCover = false;

        Vector3 DefaultPosition;
        Vector3 DefaultRotation;
        bool ResetFOVTransform = false;

        bool DoHavePreviousFiringPoint = false;

        [HideInInspector]
        public bool IsTaskOver = false;

        float TimerForNewStateToStart;
        float StateSwitchTimer;

        [HideInInspector]
        public bool Deregisterfirepoint = false;

        bool IsChargingBehaviourSelected = false;
        bool IsFiringBehaviourSelected = false;
        bool IsCoverBehaviourSelected = false;

        bool IsPreviouslyChargingBehaviour = false;
        bool IsPreviouslyFiringBehaviour = false;
        bool IsPreviouslyCoverBehaviour = false;

        float SaveRangeForCover;
        float SaveRangeForFiringPoint;
        bool StartIterating = false;

        bool ShouldStartFunctionality = false;

        int StrafeOrStationedShoot;
        bool CanAiAgentStrafeShoot = false;

        [HideInInspector]
        public bool InitialiseCoverBehaviour = false;
        [HideInInspector]
        public bool InitialiseFiringPointsBehaviour = false;
        bool InitialiseChargingBehaviour = false;

        Vector3 PreviousDestinationWhenRunning = Vector3.positiveInfinity;
        string PreviousAnimName;
        bool StartDirectionCheck = false;

        bool OverriteRunning = false;

        [HideInInspector]
        public bool ActivateWalkIk = false;
        [HideInInspector]
        public bool ActivateRunningIk = false;
        [HideInInspector]
        public bool ActivateScanIk = false;
        [HideInInspector]
        public bool ActivateWalkAimIk = false;
        [HideInInspector]
        public bool ActivateSprintingIk = false;
        [HideInInspector]
        public bool ActivateNoIk = false;

        bool OverriteRunningWhenFollowingCommander = false;

        bool UseCrouchOrStandPosture = true;

        bool CheckReloadingBeforeOpenFirePointUse = false;
        bool ShouldReloadBeforeOpenFirePointUse = false;

        string GetStrafeAnimationName;
        float GetMovingSpeed;

        [HideInInspector]
        public string FriendlyDeadBodyName;

        bool ImmediateStartScanForTheFirstTime = false;
        //float HeadWeight = 0f;
        //float EyesWeight = 0f;

        /// <summary>
        /// Sound coorinates
        /// </summary>
        ///

        [HideInInspector]
        public Vector3 GetSoundCoordinate;
        [HideInInspector]
        public bool GenerateSoundCoorinate = false;
        bool StayNearSoundCoordinate = false;

        float RandomiseStoppingDistanceFromSound;

        int AutoRandomiseSoundBehaviour = 0;

        [HideInInspector]
        public bool ForceMoveTowardsSoundCoordinate = false;

        [HideInInspector]
        public bool ThrowingGrenade = false;
        [HideInInspector]
        public bool IsPlayingBodyHitAnimation = false;

        [HideInInspector]
        public bool StopBotForShoot = false;
        Vector3 horizontalDistanceCheckerForCoverBehaviour = Vector3.zero;

        float RandomDistanceWithPlayerToAttack;
        float RandomDistanceToEnemyAiAgentToAttack;
        int GetRandomMeleeAttack;

        public bool ShouldPlayMeleeAnimation = false;

        bool IsWaitingForNextMeleeAttack = false;

        [HideInInspector]
        public bool ShouldActivateEmergencyRadius;

        [HideInInspector]
        public bool ShouldActivateInvestigationRadius = false;

        float RangeToFindEmergencyCover;
        float EmergencySprintTimerIfNoCoverFound;
        float TimeToStayNearDeadBody;
        float InvestigationRadiusFromTheDeadBody;
        float EmergencyRadiusIfNoCoverFound;
        float ClosestDistanceWithEmergencyCover;
        bool CreateCorrectEmergencySprintCoordinate = false;
        int CountSwitchesForEmergencyStates;

        int RandomValueSwitchBetweenEmergencyCover;
        bool FindBestEmergencyHidePoint = false;
        int CountSwitchesForEmergencyHidePoint;

        [HideInInspector]
        bool WaitForEmergencyStateToFinish = false;

        bool FinalDestinationForAiAgent = false;

        GameObject PreviousEmergencyCoverNode;
        GameObject AdditionalEmergencyCoverNode;
        float AddtionalStayTimerBehindEmergencyCover;
        bool ReachedAdditionalCoverNode = false;

        bool ForceAdditionEmergencyCoverNodes = false;

        [HideInInspector]
        public bool IsEmergencyStateExpiredCompletely = false;

        float GetFovAngleForZigZagMovement;
        int BestWayPoint;
        bool NoNeedToFindNewCover = false;

        float TimeToCheckForCoverIfNoCoverAvailable;

        CoverNode NewCoverNode;
        CoverNode PreviousCoverNode;

        FiringPoint NewFiringPointNode;
        FiringPoint PreviousFiringPointNode;

        bool ShouldStopLookingForCover = false;
        bool ShouldStopLookingForEmergencyCover = false;
        bool IsBestCoverChosen = false;
        bool IsBestEmergencyCoverChosen = false;
        bool FirstCoverInitialized = false;

        EmergencyCoverNode CurrentEmergencyCoverNode;

        bool FirstFindingsCompleted = false;

        

        float ClosestDistanceToStopFromCoordinate;
        float EnemyPursueDistanceToSprint;
        float EnemyPursueDistanceToRun;
        float EnemyPursueDistanceToWalk;
        float MaxTimeToScanAfterPursue;
        bool StayingNearPursuingCoordinate = false;
        bool StareTimeBeginForMovableAi = false;
        bool StareTimeCompletedForMovableAi = false;

        Vector3 DistanceBetweenMeAndEnemyLastLocation;

        bool ActivateConversationSoundsComponent = false;

        [HideInInspector]
        public bool PlayDefaultBehaviourSoundsNow = false;

        //bool DisableNavMeshAgentComponent = false;

        Vector3 PrevDestination = Vector3.positiveInfinity;
        //bool CanStartMoving = false;

        Vector3 DistanceWithPoint;

        bool StaringAtLastEnemyLostPosition = false;
        bool CanMoveTowardsPursueCoordinate = false;

        Transform PreviousEnemyDuringCharging;
        Vector3 DistanceCheckForCharging;
        Vector3 DistanceCheckForZombieMeleeCharging;
        bool CanStopCharge = false;

        bool IsEmergencyPathBroken = false;

        bool IsEmergencyStateWrongShootingStarted = false;

        int ShootingPointForEmergencyCoverNode;
        bool EmergencyCoverCheck = false;
        bool FindBestEmergencyCoverNodeForPathBroken = false;
        bool DidReachedEmergencyPointInPathBroken = false;
        bool ResetEmergencyDecisionMaking = false;
        bool ReachedEmergencyCoverCompletely = false;
        bool IsNewEmergencyCoverFound = false;
        bool SearchForEmergencyCover = false;
        float SearchTimerForEmergencyCover;
        bool IsShootingDuringEmergencyState = false;

        [HideInInspector]
        public NavMeshObstacle NavMeshObstacleComponent; 
        Vector3 TargetOffsetedPosition;

        bool IsInitialEmergencyStateCompleted = false;
        bool CheckPathWithEnemyInEmergencyState = false;

        bool ReRandomise = false;

        bool IsGoingToPreviousPoint = false;
       // bool StrafeOriginPoint = false;
       // Vector3 PreviousStrafeCoordinate;
        bool IsNoncombatstatebegins = false;
        bool Iscombatstatebegins = false;
        float RaycastChecksToDoWhenTargetLostDuringStrafing;
        float DistanceToStopChargingAtEnemyForStrafe;

        bool FindingNewNavMeshPath = false;
        bool CheckIfPathBrokenWithEnemy = false;
        bool ChargePathIsIncompleteWithTarget = false;

        [HideInInspector]
        public bool DisableTemporaryWhenSearchingForCoverOrFiringPoints = false;
        Vector3 horizontalDisplacement = Vector3.zero;
                bool StartReloadCheck = false;
        bool ShouldPauseStandCoverPostureAnims = false;

        //bool ActivatePostCombatState = false;
        //bool WasInCombatStatePreviously = false;

        //Transform LastEnemy;
        [HideInInspector]
        public bool WasInCombatStateBefore = false;
        bool startquickscan = false;
        bool IsCoroutineReseted = false;

        [HideInInspector]
        public bool IsInDefaultInvestigation = false;

        bool StartDelaySoundsAndEmergencyAlertTriggersTimer = false;
        string PreviousScanAnimationName = "None";

        int ScanValueAfterCombat;
        int ScanValueAfterPursue;

        string PreviousMeleeAnimationPlaying = "None";
        bool ContinueWithFiringPoint = false;

        bool IsEmergencyStateCurrentlyActive = false;

        [HideInInspector]
        public bool OverriteSprintingForSounds = true;
        [HideInInspector]
        public bool OverriteRunningForSounds = true;
        [HideInInspector]
        public bool OverriteWalkingForSounds = true;

        bool IsSprintingTowardsSoundCoordinateCompleted = false;

        [HideInInspector]
        public int CountsForEditorScript;

        [HideInInspector]
        public bool IsAnyTaskCurrentlyRunning = false;

        [HideInInspector]
        public bool IsFieldOfViewForDetectingEnemyEnabled = false;

        [HideInInspector]
        public bool IsLeaderMoving = false;


        [HideInInspector]
        public bool UpdateChargeProbability = false;
        [HideInInspector]
        public bool UpdateWaypointProbability = false;
        [HideInInspector]
        public bool UpdateCoverProbability = false;

        bool StopAndShootIsCanceledByFiringPoint = false;
        bool DistanceRecievedFromFiringPoint = false;

        Vector3 DistanceToFiringPoint;

        [HideInInspector]
        public float SavedFov;

        Vector3 StoredEnemyPositionDuringEmergency;

        bool CheckDistanceToEnemy = false;

        [HideInInspector]
        public bool WasInCombatLastTime = false;

        bool MoveToNewStrafePoint = false;
        bool StrafingForChargingAgent = false;
        // bool IsStrafePointReached = false;

        bool StopAndPlayBodyHitAnimationFirst = false;

        string PreviousPickedFullBodyAnimForSoldier;
        string PreviousPickedUpperBodyAnimForSoldier;
        string PreviousPickedFullBodyAnimForZombie;

        private void OnValidate()
        {
            if (AiMeleeAttack.AnimationsToPlay != AiMeleeAttack.previousListCount)
            {
                if (AiMeleeAttack.AnimationsToPlay > AiMeleeAttack.previousListCount)
                {
                    // Increase count: Preserve existing items and add new ones
                    int itemsToAdd = AiMeleeAttack.AnimationsToPlay - AiMeleeAttack.previousListCount;
                    for (int i = 0; i < itemsToAdd; i++)
                    {
                        AiMeleeAttack.MeleeAnimation.Add(null);
                    }
                }
                else if (AiMeleeAttack.AnimationsToPlay < AiMeleeAttack.previousListCount && AiMeleeAttack.AnimationsToPlay > 0)
                {
                    // Decrease count while preserving values and keeping it above 0
                    AdjustListSize();
                }

                AiMeleeAttack.previousListCount = AiMeleeAttack.AnimationsToPlay;
            }
        }

        private void AdjustListSize()
        {
            AiMeleeAttack.MeleeAnimation.RemoveRange(AiMeleeAttack.AnimationsToPlay, AiMeleeAttack.MeleeAnimation.Count - AiMeleeAttack.AnimationsToPlay);
        }

        public void ResettingDescription()
        {
            ScriptInfo = "This script is used for the Ai agents to exhibit various kinds of behaviours depending on which Ai feature is selected from the checkboxes below ." +
            "The more options you will select from the checkboxes below the heavier the Ai will become in terms of performance as the Ai agent will have more abilities to perform.";

        }
        public void ChangeGameobjectName()
        {
            AgentName = transform.name;
        }
        void Awake()
        {
            transform.parent = null;
        }
        private void Start()
        {
            PreviousEnemyDuringCharging = transform;
            FindEnemiesScript = GetComponent<FindEnemies>();

            NavMeshObstacleComponent = GetComponent<NavMeshObstacle>();

            SavedCurrentPositionForEmergencyCoverFinding = transform.position;
            if (FindEnemiesScript.EnableFieldOfView == true)
            {
                IsFieldOfViewForDetectingEnemyEnabled = true;
                FindEnemiesScript.OriginalFov = FindEnemiesScript.FieldOfViewValue * 0.5f;
                SavedFov = FindEnemiesScript.OriginalFov;
                IsEnemyLocked = false;
            }
            else
            {
                IsEnemyLocked = true;
            }

            if(AgentRole == Role.Zombie)
            {
                NonCombatBehaviours.EnableEmergencyAlerts = false;
                NonCombatBehaviours.EnableDeadBodyAlerts = false;
                CombatStateBehaviours.EnableMeleeAttack = true;

                // Important so it could update all speeds when agent role is zombie.
                Speeds.MovementSpeeds.SprintSpeed = ZombieSpeeds.MovementSpeeds.SprintSpeed;
                Speeds.MovementSpeeds.WalkForwardSpeed = ZombieSpeeds.MovementSpeeds.WalkForwardSpeed;
                Speeds.MovementSpeeds.RunForwardSpeed = ZombieSpeeds.MovementSpeeds.RunForwardSpeed;

                Speeds.AnimationSpeeds.IdleAnimationSpeed = ZombieSpeeds.AnimationSpeeds.IdleAnimationSpeed;
                Speeds.AnimationSpeeds.SprintingAnimationSpeed = ZombieSpeeds.AnimationSpeeds.SprintingAnimationSpeed;
                Speeds.AnimationSpeeds.RunForwardAnimationSpeed = ZombieSpeeds.AnimationSpeeds.RunForwardAnimationSpeed;
                Speeds.AnimationSpeeds.WalkForwardAnimationSpeed = ZombieSpeeds.AnimationSpeeds.WalkForwardAnimationSpeed;
                Speeds.AnimationSpeeds.MeleeAttack1AnimationSpeed = ZombieSpeeds.AnimationSpeeds.MeleeAttack1AnimationSpeed;
                Speeds.AnimationSpeeds.MeleeAttack2AnimationSpeed = ZombieSpeeds.AnimationSpeeds.MeleeAttack2AnimationSpeed;
                Speeds.AnimationSpeeds.MeleeAttack3AnimationSpeed = ZombieSpeeds.AnimationSpeeds.MeleeAttack3AnimationSpeed;

            }

            path = new NavMeshPath();

            // elapsed = 0.0f;
            if (GetComponent<Targets>() != null)
            {
                T = GetComponent<Targets>();
            }

            if (GetComponent<AnimatorLayerWeightController>() != null)
            {
                AnimatorLayerWeightControllerScript = GetComponent<AnimatorLayerWeightController>();
            }

            HealthScript = GetComponent<HumanoidAiHealth>();
            RotateSpine = GetComponent<SpineRotation>();
            VisibilityCheck = GetComponent<HumanoidVisibilityChecker>();
            anim = GetComponent<Animator>();
            pathfinder = GetComponent<AiPathFinder>();
            if (FindEnemiesScript.EnableFieldOfView == true)
            {
                FindEnemiesScript.FOV = FindEnemiesScript.OriginalFov;
            }
            else
            {
                FindEnemiesScript.OriginalFov = 360;
            }

            FindEnemiesScript.FindingEnemies();
            RandomiseProximityTimes = Random.Range(FindEnemiesScript.MinDetectionInterval, FindEnemiesScript.MaxDetectionInterval);
            InvokeRepeating("FindClosestEnemyNow", 0.0f, RandomiseProximityTimes);
            SaveDetectingDistance = FindEnemiesScript.DetectionRadius;

            //   NavmeshPathTimer = Random.Range(MinTimeBetweenCalculatingNavMeshPath, MaxTimeBetweenCalculatingNavMeshPath);
           


            //if (ChooseRandomWayPoints == true)
            //{
            //    CurrentWayPoint = Random.Range(0, WayPointPositions.Count);
            //}
            gameObject.SendMessage("CheckAiFov", FindEnemiesScript.EnableFieldOfView, SendMessageOptions.DontRequireReceiver);

            if (GetComponent<ScanningScript>() != null)
            {
                ScanScript = GetComponent<ScanningScript>();
            }
            if (GetComponent<Patrolling>() != null)
            {
                PatrolingScript = GetComponent<Patrolling>();
            }
            if (GetComponent<Wandering>() != null)
            {
                WanderingScript = GetComponent<Wandering>();
            }
            if (GetComponent<HumanoidGrenadeThrower>() != null)
            {
                grenadeThrowerScript = GetComponent<HumanoidGrenadeThrower>();
            }
            //if (GetComponent<HeadIk>() != null)
            //{
            //    HeadIkScript = GetComponent<HeadIk>();
            //}

            if (CountsForEditorScript >= 2)
            {
                StartVariations = true;
            }
            else
            {
                StartVariations = false;
            }

            if (StartVariations == true)
            {
                if(CombatStateBehaviours.EnableAiCharging == false)
                {
                    ChargingProbability = 0;
                }
                if (CombatStateBehaviours.UseFiringPoints == false)
                {
                    FiringPointsUsageProbability = 0;
                }
                if (CombatStateBehaviours.TakeCovers == false)
                {
                    CoversUsageProbability = 0;
                }

                if (ChargingProbability == 0 && CombatStateBehaviours.EnableAiCharging == true)
                {
                    CombatStateBehaviours.EnableAiCharging = false;
                }
                if (FiringPointsUsageProbability == 0 && CombatStateBehaviours.UseFiringPoints == true)
                {
                    CombatStateBehaviours.UseFiringPoints = false;
                }
                if (CoversUsageProbability == 0 && CombatStateBehaviours.TakeCovers == true)
                {
                    CombatStateBehaviours.TakeCovers = false;
                }

                GetProbability = new int[3];

                if (CombatStateBehaviours.EnableAiCharging == true)
                {
                    GetProbability[0] = ChargingProbability;
                }
                if (CombatStateBehaviours.UseFiringPoints == true)
                {
                    GetProbability[1] = FiringPointsUsageProbability;
                }
                if (CombatStateBehaviours.TakeCovers == true)
                {
                    GetProbability[2] = CoversUsageProbability;
                }

                System.Array.Sort(GetProbability);


                TotalProbability = ChargingProbability + CoversUsageProbability + FiringPointsUsageProbability;

                GetFirstProbability();
                GetSecondProbability();
                GetThirdProbability();

                TotalProbability = RangeForElement2[1];
            }

            if (DebugAgentState == true)
            {
                SpawnText();
            }




            DefaultAgentRadius = Components.NavMeshAgentComponent.radius;

            //if (FindEnemiesScript.EnableFieldOfView == true)
            //{
            //    FOVDefaultTransform();
            //}

            StateSwitchTimer = Random.Range(CombatStateBehaviours.MinStateSwitchTime, CombatStateBehaviours.MaxStateSwitchTime);

            ShouldStartFunctionality = true;

            if (GrenadeAlerts.ChooseAlertType == GrenadeAlertClass.MoveAwayFromLandedGrenades)
            {
                RandomiseRunFromGrenadesBehaviour = false;
                RunFromGrenades = true;
            }
            else if (GrenadeAlerts.ChooseAlertType == GrenadeAlertClass.RandomizeMoveFromLandedGrenades)
            {
                RunFromGrenades = true;
                RandomiseRunFromGrenadesBehaviour = true;
            }

            RandomDistanceWithPlayerToAttack = Random.Range(AiMeleeAttack.MinDistanceToAttackPlayer, AiMeleeAttack.MaxDistanceToAttackPlayer);
            RandomDistanceToEnemyAiAgentToAttack = Random.Range(AiMeleeAttack.MinDistanceToAttackEnemyAiAgent, AiMeleeAttack.MaxDistanceToAttackEnemyAiAgent);

            PreviousEmergencyCoverNode = gameObject;

            if (NonCombatBehaviours.EnableEmergencyAlerts == true)
            {
                if(AiEmergencyState.EmergencyAlert.PostFirstEmergencyCoverBehaviourActivity == ChooseEmergencySprintState.ChooseRandomly)
                {
                    RandomPickEmergencyState();
                }

                RangeToFindEmergencyCover = Random.Range(AiEmergencyState.EmergencyAlert.MinRangeToFindEmergencyCover, AiEmergencyState.EmergencyAlert.MaxRangeToFindEmergencyCover);
                EmergencySprintTimerIfNoCoverFound = Random.Range(AiEmergencyState.EmergencyAlert.AiFleeingBehaviour.MinFleeDuration, AiEmergencyState.EmergencyAlert.AiFleeingBehaviour.MaxFleeDuration);
                EmergencyRadiusIfNoCoverFound = Random.Range(AiEmergencyState.EmergencyAlert.AiFleeingBehaviour.MinEmergencySprintAwayRadius, AiEmergencyState.EmergencyAlert.AiFleeingBehaviour.MaxEmergencySprintAwayRadius);
               
                RandomDistanceWithEmergencyCoverToCheck = Random.Range(AiEmergencyState.EmergencyAlert.AdvancingBetweenCovers.MinDistanceTowardsNextCover, AiEmergencyState.EmergencyAlert.AdvancingBetweenCovers.MaxDistanceTowardsNextCover);

                RandomValueSwitchBetweenEmergencyCover = Random.Range(AiEmergencyState.EmergencyAlert.AdvancingBetweenCovers.MinEmergencyAdvancingCovers, AiEmergencyState.EmergencyAlert.AdvancingBetweenCovers.MaxEmergencyAdvancingCovers);
                AddtionalStayTimerBehindEmergencyCover = Random.Range(AiEmergencyState.EmergencyAlert.AdvancingBetweenCovers.MinTimeToRestBehindCovers, AiEmergencyState.EmergencyAlert.AdvancingBetweenCovers.MaxTimeToRestBehindCovers);
                GetRandomFleeDirection();
            }

            if(NonCombatBehaviours.EnableDeadBodyAlerts == true)
            {
                RandomFiringPointSprint = TimeToStayNearDeadBody = Random.Range(AiDeadBodyAlerts.MinTimeToStayNearDeadBody, AiDeadBodyAlerts.MaxTimeToStayNearDeadBody);
                InvestigationRadiusFromTheDeadBody = Random.Range(AiDeadBodyAlerts.MinInvestigationRadiusFromTheDeadBody, AiDeadBodyAlerts.MaxInvestigationRadiusFromTheDeadBody); ;
            }

            if (CombatStateBehaviours.ChooseEnemyPursueType != EnemyPursueTypes.DoNotPursueEnemy)
            {
                ClosestDistanceToStopFromCoordinate = Random.Range(AiPursue.MinNearStoppingDistance, AiPursue.MaxNearStoppingDistance);
                EnemyPursueDistanceToSprint = Random.Range(AiPursue.MinSprintDistance, AiPursue.MaxSprintDistance);
                EnemyPursueDistanceToRun = Random.Range(AiPursue.MinRunDistance, AiPursue.MaxRunDistance);
                EnemyPursueDistanceToWalk = Random.Range(AiPursue.MinWalkDistance, AiPursue.MaxWalkDistance);
                MaxTimeToScanAfterPursue = Random.Range(AiPursue.AimedScaningAtEnemyLastKnownPosition.MinScanCompletionTime, AiPursue.AimedScaningAtEnemyLastKnownPosition.MaxScanCompletionTime);

              //  StareTimeAtLastKnownEnemyCoordinateIfStationed = Random.Range(AiPursue.MinStationedStareTimeAtEnemyLastKnownPosition, AiPursue.MaxStationedStareTimeAtEnemyLastKnownPosition);
                StareTimeAtLastKnownEnemyCoordinate = Random.Range(AiPursue.MinStareTimeAtEnemyLastKnownPosition, AiPursue.MaxStareTimeAtEnemyLastKnownPosition);

                AiPursue.DebugPursueDistances.DebugSprintDistance = EnemyPursueDistanceToSprint;
                AiPursue.DebugPursueDistances.DebugRunDistance = EnemyPursueDistanceToRun;
                AiPursue.DebugPursueDistances.DebugWalkDistance = EnemyPursueDistanceToWalk;
            }

            if (CombatStateBehaviours.EnableStrafing == true)
            {
                IsStrafingEnabled = true;
                SaveStrafeData = CombatStateBehaviours.EnableStrafing;
                RaycastChecksToDoWhenTargetLostDuringStrafing = Random.Range(AiStrafing.MinRaycastWhenEnemyLost, AiStrafing.MaxRaycastWhenEnemyLost);
                DistanceToStopChargingAtEnemyForStrafe = Random.Range(AiStrafing.MinDistanceToStopChargingAtEnemy, AiStrafing.MaxDistanceToStopChargingAtEnemy);
            }

            GetFovAngleForZigZagMovement = FovForZigZagMovements * 0.5f;
        }
        public void GetRandomFleeDirection()
        {
            int RandomDirectionForEmergencyPoint = Random.Range(0, AiEmergencyState.EmergencyAlert.AiFleeingBehaviour.FleeDestinations.Length);
            EmergencyDirectionPoint = AiEmergencyState.EmergencyAlert.AiFleeingBehaviour.FleeDestinations[RandomDirectionForEmergencyPoint];
        }
        public void RandomPickEmergencyState()
        {
            int RandomlyPicking = Random.Range(0, 3);
            if(RandomlyPicking == 0)
            {
                AiEmergencyState.EmergencyAlert.PostFirstEmergencyCoverBehaviourActivity = ChooseEmergencySprintState.ZigZagAdvancing; 
            }
            else if (RandomlyPicking == 1)
            {
                AiEmergencyState.EmergencyAlert.PostFirstEmergencyCoverBehaviourActivity = ChooseEmergencySprintState.AdvancingBetweenCoversAndZigZagAdvancing;
            }
            else
            {
                AiEmergencyState.EmergencyAlert.PostFirstEmergencyCoverBehaviourActivity = ChooseEmergencySprintState.EmergencyShooting;
            }
        }
        public void enableIkupperbodyRotations(ref bool WhatToActivate)
        {
            ActivateWalkIk = false;
            ActivateRunningIk = false;
            ActivateScanIk = false;
            ActivateWalkAimIk = false;
            ActivateSprintingIk = false;
            ActivateNoIk = false;

            WhatToActivate = true;
        }
        //public void FOVDefaultTransform()
        //{
        //    DefaultPosition = Detections.FieldOfView.transform.localPosition;
        //    DefaultRotation = Detections.FieldOfView.transform.localEulerAngles;
        //}
        //public void AdjustingFOV(bool combatstarted, bool sprintActivate, bool iscrouched, bool searchingForSound)
        //{
        //    Detections.FieldOfView.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //    if (combatstarted == true)
        //    {
        //        Detections.FieldOfView.transform.parent = transform.root.transform;
        //        if (ResetFOVTransform == false)
        //        {
        //            Detections.FieldOfView.transform.localPosition = DefaultPosition;
        //            Detections.FieldOfView.transform.localEulerAngles = DefaultRotation;
        //            ResetFOVTransform = true;
        //        }
        //        if (sprintActivate == false)
        //        {
        //            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
        //            {
        //                Detections.FieldOfView.transform.LookAt(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform);
        //            }
        //            if (iscrouched == true)
        //            {
        //                Vector3 pos = Detections.FieldOfView.transform.localPosition;
        //                pos.y = Detections.VisualFieldOfViewPositionOnCrouch;
        //                Detections.FieldOfView.transform.localPosition = pos;

        //            }
        //            else
        //            {
        //                ResetFOVTransform = false;
        //            }
        //        }
        //        else
        //        {
        //            Detections.FieldOfView.transform.parent = transform.root.transform;
        //            Detections.FieldOfView.transform.localPosition = DefaultPosition;
        //            Detections.FieldOfView.transform.localEulerAngles = DefaultRotation;
        //        }
        //    }
        //    else
        //    {
        //        if (searchingForSound == true || VisibilityCheck.ConnectionLost == true)
        //        {
        //            Detections.FieldOfView.transform.parent = transform.root.transform;
        //            Detections.FieldOfView.transform.localPosition = DefaultPosition;
        //            Detections.FieldOfView.transform.localEulerAngles = DefaultRotation;
        //        }
        //        else
        //        {
        //            ResetFOVTransform = false;
        //            if (Detections.HeadBone != null)
        //            {
        //                Detections.FieldOfView.transform.parent = Detections.HeadBone.transform;
        //            }
        //            Detections.FieldOfView.transform.localEulerAngles = Vector3.zero;
        //            Detections.FieldOfView.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //        }
        //    }
        //}
        private void SpawnText()
        {
#if UNITY_EDITOR
            spawnedTextPrefab = Instantiate(DebugInfo.DebugInfoTextUI, transform.position + DebugInfo.DebugInfoTextUIOffset, Quaternion.identity);
            spawnedText = spawnedTextPrefab.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            GameObject canvasfound = GameObject.FindGameObjectWithTag("Canvas3D");
            spawnedTextPrefab.transform.SetParent(canvasfound.transform, false);
            //Rotate the spawned text 90 degrees up on the X-axis
            spawnedTextPrefab.transform.rotation = Quaternion.Euler(-90f, 90f, 0f);
#endif
        }
        public void OverrallProbabilityPercentage()
        {
            DebugTotalProbabilityValue = ChargingProbability + CoversUsageProbability + FiringPointsUsageProbability;
        }
        //IEnumerator StartFindingsForIndepentFiringPoint()
        //{
        //    float randomise = Random.Range(MinTimeToInitilizeFirstCoverPoint, MaxTimeToInitilizeFirstCoverPoint);
        //    yield return new WaitForSeconds(randomise);
        //    FindingwayPoint();
        //}
        //IEnumerator StartfindingfirstCover()
        //{
        //    float randomise = Random.Range(MinTimeToInitilizeFirstCoverPoint, MaxTimeToInitilizeFirstCoverPoint);
        //    yield return new WaitForSeconds(randomise);
        //    StartCoroutine(FindClosestCoverSystematically());
        //}


        // Added crouch reload animation clip in full body because if not added than when AI agent move between 2 crouch fire point
        // than when he stand moves and reload the weapon while moving towards the crouch fire point and than crouch it becomes and legs stays like that does not
        // changed due to which this make it look wierd.so to solve this added crouch reload so both the animation clips in full body and upper body can play simulataneously.


        public void SetAnimationForFullBody(string animName = null) // Setting up All The Animations 
        {

            //Keep this code commented because if uncommented then the strafing can create a problem meaning the strafing animation will not going to work properly due to this code being uncommented the strafing animations were not transitioning properly.
            //if(animName != PreviousPickedFullBodyAnimForSoldier) 
            //{
            PreviousPickedFullBodyAnimForSoldier = animName;

                anim.SetBool(PatrollingAnimName, false);
                anim.SetBool(AiAgentAnimatorParameters.FireParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.IdleAimingParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.StandAimingParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.ReloadParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.IdleParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.WalkIdleParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.WalkForwardParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.CrouchFireParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.CrouchReloadParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.CrouchAimingParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.SprintingParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.StandCoverLeftParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.StandCoverRightParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, false);

                anim.SetBool(AiAgentAnimatorParameters.LeftCoverIdle, false);
                anim.SetBool(AiAgentAnimatorParameters.RightCoverIdle, false);

                anim.SetBool("UpperBodyIdle", false); // Newly Added On 6th April because when this condition is set to be true in case of scanning and Idle: anim.SetBool("UpperBodyIdle", false);  the Ai agent UpperBodyIdle stay to be true so we make 
                                                      // which looks wierd when both upper body and lower body play different animation we need to make sure we set this to be false when Ai agent is sprinting (in case if Ai agent hear sounds like weapon sounds by player )

                anim.SetBool(AiAgentAnimatorParameters.WalkBackwardParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.WalkLeftParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.WalkRightParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.WalkForwardLeftParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.WalkForwardRightParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.WalkBackwardRightParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.WalkBackwardLeftParameterName, false);


                anim.SetBool(AiAgentAnimatorParameters.RunForwardParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.RunBackwardParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.RunRightParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.RunLeftParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.RunForwardLeftParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.RunForwardRightParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.RunBackwardLeftParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.RunBackwardRightParameterName, false);

                anim.SetBool(AiAgentAnimatorParameters.StandCoverNeutralParameterName, false);

                anim.SetBool(AiAgentAnimatorParameters.MeleeAttack1ParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.MeleeAttack2ParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.MeleeAttack3ParameterName, false);


                //This is important so that if scanning behaviour is running and if any different animation like sprinting or running want to get started ( due to hearing , emergency state ) etc.. we immediate make the following
                //animations to be false so other animations can smoothly play without any issues.
                if (animName == "UpperBodyIdle" || animName == "UBScan")
                {
                    // Do not do anything here because we want to play any of this animation during scan behaviour 
                }
                else
                {
                    if (ScanScript.enabled == true)
                    {
                        if (ScanScript.AimedScanPoint != null)
                        {
                            ScanScript.AimedScanPoint.transform.localPosition = ScanScript.DefaultScanAimingPointPosition;
                        }
                    }
                    anim.SetInteger("Scan", 100);
                    anim.SetBool("UBScan", false);
                    anim.SetBool("UpperBodyIdle", false);
                }

                if (IsPlayingBodyHitAnimation == false && ThrowingGrenade == false)
                {
                    if (animName != null)
                        anim.SetBool(animName, true);
                }
                else
                {
                    if (animName != AiAgentAnimatorParameters.ReloadParameterName ||
                        animName != AiAgentAnimatorParameters.CrouchReloadParameterName)
                    {
                        if (animName != null)
                            anim.SetBool(animName, true);
                    }
                }

                PreviousAnimName = animName;

                //if (animName == AiAgentAnimatorParameters.WalkBackwardParameterName ||
                //    animName == AiAgentAnimatorParameters.WalkLeftParameterName ||
                //    animName == AiAgentAnimatorParameters.WalkRightParameterName ||
                //     animName == AiAgentAnimatorParameters.WalkForwardLeftParameterName ||
                //      animName == AiAgentAnimatorParameters.WalkForwardRightParameterName ||
                //         animName == AiAgentAnimatorParameters.WalkBackwardRightParameterName ||
                //     animName == AiAgentAnimatorParameters.WalkBackwardLeftParameterName ||
                //     animName == AiAgentAnimatorParameters.WalkForwardParameterName ||
                //     animName == AiAgentAnimatorParameters.WalkIdleParameterName)
                //{
                //   Components.HumanoidAiSoundManagerComponent.PlayWalkStepsSounds();
                //}
                //if (animName == AiAgentAnimatorParameters.RunForwardParameterName ||
                //    animName == AiAgentAnimatorParameters.RunBackwardParameterName ||
                //    animName == AiAgentAnimatorParameters.RunRightParameterName ||
                //     animName == AiAgentAnimatorParameters.RunLeftParameterName ||
                //      animName == AiAgentAnimatorParameters.RunForwardLeftParameterName ||
                //         animName == AiAgentAnimatorParameters.RunForwardRightParameterName ||
                //     animName == AiAgentAnimatorParameters.RunBackwardLeftParameterName ||
                //     animName == AiAgentAnimatorParameters.RunBackwardRightParameterName)
                //{
                //    Components.HumanoidAiSoundManagerComponent.PlayRunStepsSounds();
                //}
                //if (animName == AiAgentAnimatorParameters.SprintingParameterName)
                //{
                //    Components.HumanoidAiSoundManagerComponent.PlaySprintStepsSounds();
                //}
                //if (animName == AiAgentAnimatorParameters.CrouchReloadParameterName
                //    || animName == AiAgentAnimatorParameters.CrouchAimingParameterName
                //    || animName == AiAgentAnimatorParameters.SprintingParameterName
                //    || animName == AiAgentAnimatorParameters.StandCoverLeftParameterName
                //    || animName == AiAgentAnimatorParameters.StandCoverRightParameterName
                //    || animName == AiAgentAnimatorParameters.CrouchFireParameterName)
                //{
                //   // anim.SetBool(AiAgentAnimatorParameters.CrouchReloadParameterName, false);
                //   // anim.SetBool(AiAgentAnimatorParameters.ReloadParameterName, false);
                //    AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f);
                //    //AnimatorLayerWeightControllerScript.ChangeLayerWeight(1, 0f,true);
                //    //AnimatorLayerWeightControllerScript.ChangeLayerWeight(2, 0f,true);
                //}
                //else
                //{
                //    AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 0f,true);
                //    //AnimatorLayerWeightControllerScript.ChangeLayerWeight(1, 1f);
                //    //AnimatorLayerWeightControllerScript.ChangeLayerWeight(2, 1f);
                //}
                //if (animName == AiAgentAnimatorParameters.FireParameterName)
                // {

                //if (Components.HumanoidFiringBehaviourComponent.FireNow == true && IsQuickTransitionCompleted == false)
                //{
                //    anim.SetBool("StandShootPosture", true);
                //    StartCoroutine(QuickTransitionTowardsFire());
                //}

                if (CombatStarted == true && animName == AiAgentAnimatorParameters.SprintingParameterName)
                {
                    enableIkupperbodyRotations(ref ActivateNoIk);
                }

                if (StopWeaponShakeTemporary == false)
                {
                    anim.SetFloat("FireAnimationSpeed", Speeds.AnimationSpeeds.FireAnimationSpeed);
                    // UpdateNavigationRotation(false);
                }
                else
                {
                    anim.SetFloat("FireAnimationSpeed", 0f);
                }


                if (Components.HumanoidFiringBehaviourComponent != null)
                {

                    if (Components.NavMeshAgentComponent.speed <= 0 && Components.HumanoidFiringBehaviourComponent.IsWeaponReloading == false
                && StopShootingPosture == false && CombatStarted == true && HealthScript.IsDied == false && IsCrouched == false && UseCrouchOrStandPosture == true
                && ThrowingGrenade == false && IsPlayingBodyHitAnimation == false && StopMeleeAttack == false)// && animName != AiAgentAnimatorParameters.SprintingParameterName)
                    {
                        //AnimatorLayerWeightControllerScript.ChangeLayerWeight(2, 1f);
                        anim.SetBool("StandShootPosture", true);
                        anim.SetBool("CrouchShootPosture", false);
                    }
                    else if (Components.NavMeshAgentComponent.speed <= 0 && Components.HumanoidFiringBehaviourComponent.IsWeaponReloading == false
                       && StopShootingPosture == false && CombatStarted == true && HealthScript.IsDied == false && IsCrouched == true && UseCrouchOrStandPosture == true
                       && ThrowingGrenade == false && IsPlayingBodyHitAnimation == false && StopMeleeAttack == false)// && animName != AiAgentAnimatorParameters.SprintingParameterName)
                    {
                        anim.SetBool("StandShootPosture", false);
                        anim.SetBool("CrouchShootPosture", true);
                    }
                    else
                    {
                        anim.SetBool("StandShootPosture", false);
                        anim.SetBool("CrouchShootPosture", false);
                    }

                }


                //}
                if (animName == AiAgentAnimatorParameters.ReloadParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("ReloadAnimationSpeed", Speeds.AnimationSpeeds.ReloadAnimationSpeed);
                    UpdateNavigationRotation(false);
                }
                if (animName == AiAgentAnimatorParameters.StandAimingParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("AimingAnimationSpeed", Speeds.AnimationSpeeds.AimingAnimationSpeed);
                    UpdateNavigationRotation(false);
                }
                if (animName == AiAgentAnimatorParameters.IdleAimingParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("AimingAnimationSpeed", Speeds.AnimationSpeeds.AimingAnimationSpeed);
                    UpdateNavigationRotation(false);
                }
                if (animName == AiAgentAnimatorParameters.IdleParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("IdleAnimationSpeed", Speeds.AnimationSpeeds.IdleAnimationSpeed);
                    UpdateNavigationRotation(false);
                }
                if (animName == AiAgentAnimatorParameters.WalkForwardParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("WalkForwardAnimationSpeed", Speeds.AnimationSpeeds.WalkForwardAnimationSpeed);
                    if (CombatStarted == false)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                        {
                            UpdateNavigationRotation(true);
                        }
                        else
                        {
                            UpdateNavigationRotation(false);
                        }
                    }
                }
                if (animName == AiAgentAnimatorParameters.WalkIdleParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("WalkIdleAnimationSpeed", Speeds.AnimationSpeeds.WalkIdleAnimationSpeed);
                    UpdateNavigationRotation(true);
                }
                if (animName == AiAgentAnimatorParameters.CrouchFireParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentCrouchBaseOffset;
                    anim.SetFloat("CrouchFireAnimationSpeed", Speeds.AnimationSpeeds.CrouchFireAnimationSpeed);
                    UpdateNavigationRotation(false);
                }
                if (animName == AiAgentAnimatorParameters.CrouchReloadParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentCrouchBaseOffset;
                    anim.SetFloat("CrouchReloadAnimationSpeed", Speeds.AnimationSpeeds.CrouchReloadAnimationSpeed);
                    UpdateNavigationRotation(false);
                }
                if (animName == AiAgentAnimatorParameters.CrouchAimingParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentCrouchBaseOffset;
                    anim.SetFloat("CrouchAimingAnimationSpeed", Speeds.AnimationSpeeds.CrouchAimingAnimationSpeed);
                    UpdateNavigationRotation(false);
                }
                if (animName == AiAgentAnimatorParameters.SprintingParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("SprintingAnimationSpeed", Speeds.AnimationSpeeds.SprintingAnimationSpeed);
                    UpdateNavigationRotation(true);
                }
                if (animName == AiAgentAnimatorParameters.StandCoverLeftParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("StandCoverLeftAnimationSpeed", Speeds.AnimationSpeeds.StandCoverLeftAnimationSpeed);
                    UpdateNavigationRotation(false);
                }
                if (animName == AiAgentAnimatorParameters.StandCoverNeutralParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("StandCoverNeutralAnimationSpeed", Speeds.AnimationSpeeds.StandCoverNeutralAnimationSpeed);
                    UpdateNavigationRotation(false);
                }
                if (animName == AiAgentAnimatorParameters.StandCoverRightParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("StandCoverRightAnimationSpeed", Speeds.AnimationSpeeds.StandCoverRightAnimationSpeed);
                    UpdateNavigationRotation(false);
                }
                if (animName == AiAgentAnimatorParameters.WalkRightParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("WalkRightAnimationSpeed", Speeds.AnimationSpeeds.WalkRightSpeed);
                    if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        UpdateNavigationRotation(false);
                    }
                }
                if (animName == AiAgentAnimatorParameters.WalkLeftParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("WalkLeftAnimationSpeed", Speeds.AnimationSpeeds.WalkLeftSpeed);
                    if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        UpdateNavigationRotation(false);
                    }
                }
                if (animName == AiAgentAnimatorParameters.WalkBackwardParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("WalkBackAnimationSpeed", Speeds.AnimationSpeeds.WalkBackwardSpeed);
                    if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        UpdateNavigationRotation(false);
                    }
                }
                if (animName == AiAgentAnimatorParameters.WalkForwardRightParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("WalkForwardRightAnimationSpeed", Speeds.AnimationSpeeds.WalkForwardRightSpeed);
                    if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        UpdateNavigationRotation(false);
                    }
                }
                if (animName == AiAgentAnimatorParameters.WalkBackwardRightParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("WalkBackwardRightAnimationSpeed", Speeds.AnimationSpeeds.WalkBackwardRightSpeed);
                    if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        UpdateNavigationRotation(false);
                    }
                }
                if (animName == AiAgentAnimatorParameters.WalkBackwardLeftParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("WalkBackwardLeftAnimationSpeed", Speeds.AnimationSpeeds.WalkBackwardLeftAnimationSpeed);
                    if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        UpdateNavigationRotation(false);
                    }
                }
                if (animName == AiAgentAnimatorParameters.WalkForwardLeftParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("WalkForwardLeftAnimationSpeed", Speeds.AnimationSpeeds.WalkForwardLeftAnimationSpeed);
                    if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        UpdateNavigationRotation(false);
                    }
                }


                if (animName == AiAgentAnimatorParameters.RunForwardParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("RunFowardSpeed", Speeds.AnimationSpeeds.RunForwardAnimationSpeed);
                    //if (CombatStarted == true && VisibilityCheck.ConnectionLost == true && IsBodyguard == true) // important otherwise AI agent will not be rotated when pursuing towards enemy last know position
                    //{                                                                                           // if uncommented enemy will not be rotated while pursuing enemey in scenario when there is stairs like in construction site of the city.
                    UpdateNavigationRotation(true);
                    //}
                    //else
                    //{
                    //    UpdateNavigationRotation(false);
                    //}
                }
                if (animName == AiAgentAnimatorParameters.RunLeftParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("RunLeftSpeed", Speeds.AnimationSpeeds.RunLeftAnimationSpeed);
                    if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        UpdateNavigationRotation(false);
                    }
                }

                if (animName == AiAgentAnimatorParameters.RunRightParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("RunRightSpeed", Speeds.AnimationSpeeds.RunRightAnimationSpeed);
                    if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        UpdateNavigationRotation(false);
                    }
                }
                if (animName == AiAgentAnimatorParameters.RunBackwardParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("RunBackwardSpeed", Speeds.AnimationSpeeds.RunBackwardAnimationSpeed);
                    if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        UpdateNavigationRotation(false);
                    }
                }

                if (animName == AiAgentAnimatorParameters.RunForwardRightParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("RunForwardRightSpeed", Speeds.AnimationSpeeds.RunForwardRightAnimationSpeed);
                    if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        UpdateNavigationRotation(false);
                    }
                }
                if (animName == AiAgentAnimatorParameters.RunForwardLeftParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("RunForwardLeftSpeed", Speeds.AnimationSpeeds.RunForwardLeftAnimationSpeed);
                    if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        UpdateNavigationRotation(false);
                    }
                }
                if (animName == AiAgentAnimatorParameters.RunBackwardLeftParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("RunBackwardLeftSpeed", Speeds.AnimationSpeeds.RunBackwardLeftAnimationSpeed);
                    if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        UpdateNavigationRotation(false);
                    }
                }
                if (animName == AiAgentAnimatorParameters.RunBackwardRightParameterName)
                {
                    Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentStandBaseOffset;
                    anim.SetFloat("RunBackwardRightSpeed", Speeds.AnimationSpeeds.RunBackwardRightAnimationSpeed);
                    if (CombatStarted == true && VisibilityCheck.ConnectionLost == true)
                    {
                        UpdateNavigationRotation(true);
                    }
                    else
                    {
                        UpdateNavigationRotation(false);
                    }
                }
            //}
            //else
            //{
            //    anim.SetBool(animName, false);
            //}
          
        }
        public void SetAnimationForFullZombieBody(string animName = null) // Setting up All The Animations 
        {
            if (animName != PreviousPickedFullBodyAnimForZombie)
            {
                PreviousPickedFullBodyAnimForZombie = animName;

                PreviousPickedFullBodyAnimForSoldier = animName;
                anim.SetBool(AiAgentAnimatorParameters.IdleParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.WalkForwardParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.SprintingParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.RunForwardParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.MeleeAttack1ParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.MeleeAttack2ParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.MeleeAttack3ParameterName, false);

                UpdateNavigationRotation(true); // required when zombie moving to the closest point on the navmesh during running , walking and sprinting when closest point is created on road and enemy/player is on roof

                if (IsPlayingBodyHitAnimation == false)
                {
                    if (animName != null)
                        anim.SetBool(animName, true);
                }

                PreviousAnimName = animName;

                if (CombatStarted == true)
                {
                    enableIkupperbodyRotations(ref ActivateNoIk);
                }
            }
            else
            {
                anim.SetBool(animName, true);
            }
        }
        public void SetAnimationForUpperBody(string animName = null) // Setting up All The Animations 
        {
            //Keep this code commented because if uncommented then the grenade throw can create a problem meaning the grenade animation will be intruppted by other set of animation for upper body layer and due to this code the grenade upper body animation will not going to work properly.
            //if (animName != PreviousPickedUpperBodyAnimForSoldier)
            //{
            PreviousPickedUpperBodyAnimForSoldier = animName;

                anim.SetBool(AiAgentAnimatorParameters.SprintingParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.StandCoverLeftParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.StandCoverRightParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.StandCoverNeutralParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.WalkIdleParameterName, false);

                anim.SetBool(AiAgentAnimatorParameters.LeftCoverIdle, false);
                anim.SetBool(AiAgentAnimatorParameters.RightCoverIdle, false);

                anim.SetBool("UpperBodyIdle", false);

                anim.SetBool(AiAgentAnimatorParameters.ReloadParameterName, false);
                anim.SetBool("UBScan", false);
                anim.SetBool("StandShootPosture", false);

                anim.SetBool(AiAgentAnimatorParameters.MeleeAttack1ParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.MeleeAttack2ParameterName, false);
                anim.SetBool(AiAgentAnimatorParameters.MeleeAttack3ParameterName, false);

                if (ThrowingGrenade == false && IsPlayingBodyHitAnimation == false)
                {
                    if (animName != null)
                        anim.SetBool(animName, true);
                }
            //}
            //else
            //{
            //    anim.SetBool(animName, true);
            //}
        }
        //IEnumerator QuickTransitionTowardsFire()
        //{
        //    yield return new WaitForSeconds(1f);
        //    IsQuickTransitionCompleted = true;
        //}
        //public void EnemyView(bool View)
        //{
        //    NoEnemyInView = View;
        //}
        //IEnumerator CoroCheckForWeight()
        //{
        //    yield return new WaitForSeconds(AiAgentAnimatorParameters.AnimatorWeightInterpolationDuration);
        //    ShouldTweenNewValues = true;
        //}
        //public void ChangeLayerWeight(int targetWeight)
        //{
        //    if(ShouldTweenNewValues == true)
        //    {
        //        if (anim.GetLayerWeight(LayerIndexToCheck) != targetWeight)
        //        {
        //            AnimatorWeightChange = false;
        //            ShouldTweenNewValues = false;
        //        }
        //    }
        //    if (AnimatorWeightChange == false)
        //    {
        //        // Get the current weight of the layer
        //        float currentWeight = anim.GetLayerWeight(LayerIndexToCheck);
        //        // Use LeanTween to interpolate the weight from its current value to the target value over the specified duration
        //        LeanTween.value(gameObject, SetLayerWeight, currentWeight, targetWeight, AiAgentAnimatorParameters.AnimatorWeightInterpolationDuration);
        //        StartCoroutine(CoroCheckForWeight());
        //        AnimatorWeightChange = true;
        //    }     
        //}
        //private void SetLayerWeight(float weight)
        //{
        //    // Set the weight of the specified layer to the interpolated value
        //    anim.SetLayerWeight(LayerIndexToCheck, weight);
        //}

        Vector3 SoundDestinationToGoTo;

        bool StopMeleeAttack = false;

        public void InitialiseHearing()
        {
            //if (AiHearing.SoundAlertProbability == ListeningTypes.ListenEverySounds)
            //{
            //    AutoRandomiseSoundBehaviour = 0;
            //}
            //else if (AiHearing.SoundAlertProbability == ListeningTypes.MayOrMayNotListenEverySound)
            //{
            //    AutoRandomiseSoundBehaviour = Random.Range(0, 2);
            //}
            //else if (AiHearing.SoundAlertProbability == ListeningTypes.DoNotListenSounds)
            //{
            //    AutoRandomiseSoundBehaviour = 1;
            //}

            if(NonCombatBehaviours.EnableSoundAlerts == true)
            {
                float SoundAlertProbabilityRandomisation = Random.Range(0f, 100f);
                if (SoundAlertProbabilityRandomisation <= AiHearing.SoundAlertProbability)
                {
                    AutoRandomiseSoundBehaviour = 0;
                }
                else
                {
                    AutoRandomiseSoundBehaviour = 1;
                }

                if (AutoRandomiseSoundBehaviour == 0)
                {
                    SearchingForSound = true;
                }
            }
         
        }
        public void GoToHearingSound() // Checking and Moving Towards Sounds 
        {
            if (GenerateSoundCoorinate == false)
            {
                Components.HumanoidAiAudioPlayerComponent.PlayNonRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceHearingInvestigationAudioClips);

                IsSprintingTowardsSoundCoordinateCompleted = false;
             
                if (AutoRandomiseSoundBehaviour == 0 || ForceMoveTowardsSoundCoordinate == true)
                {
                    //RandomiseSprintDistanceFromSound = Random.Range(AiHearing.MinDistanceToSprint, AiHearing.MaxDistanceToSprint);
                    //RandomiseRunDistanceFromSound = Random.Range(AiHearing.MinDistanceToRun, AiHearing.MaxDistanceToRun);
                    //RandomiseWalkDistanceFromSound = Random.Range(AiHearing.MinDistanceToWalk, AiHearing.MaxDistanceToWalk);
                    RandomiseStoppingDistanceFromSound = Random.Range(AiHearing.MinNearStoppingDistance, AiHearing.MaxNearStoppingDistance);

                    //SoundDestinationToGoTo = GenerateRandomNavmeshLocation.RandomLocationInVector3(GetSoundCoordinate, AiHearing.ErrorSoundPercentage);

                    //if(AiHearing.MoveTowardsSoundCoordinate == true)
                    //{
                    //    NavMeshPath path = new NavMeshPath();
                    //    if (NavMesh.CalculatePath(transform.position, SoundDestinationToGoTo, NavMesh.AllAreas, path))
                    //    {
                    //        if (path.status == NavMeshPathStatus.PathComplete)
                    //        {
                    //            AutoRandomiseSoundBehaviour = 0;
                    //        }
                    //        else
                    //        {
                    //            AutoRandomiseSoundBehaviour = 1;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        AutoRandomiseSoundBehaviour = 1;
                    //    }
                    //}

                    if (AiHearing.MoveTowardsSoundCoordinate == true)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            float radius = AiHearing.ErrorSoundPercentage + i;
                            SoundDestinationToGoTo = GenerateRandomNavmeshLocation.RandomLocationInVector3(GetSoundCoordinate, radius);

                            NavMeshPath path = new NavMeshPath();

                            if (NavMesh.CalculatePath(transform.position, SoundDestinationToGoTo, NavMesh.AllAreas, path))
                            {
                                if (path.status == NavMeshPathStatus.PathComplete)
                                {
                                    AutoRandomiseSoundBehaviour = 0;
                                    break;
                                }
                                else
                                {
                                    AutoRandomiseSoundBehaviour = 1;
                                }
                            }
                            else
                            {
                                AutoRandomiseSoundBehaviour = 1;
                            }
                        }
                    }
                    else
                    {
                        SoundDestinationToGoTo = GenerateRandomNavmeshLocation.RandomLocationInVector3(GetSoundCoordinate, AiHearing.ErrorSoundPercentage);
                    }


                    if (AiHearing.ShareSoundCoordinatesComponent != null)
                    {
                        AiHearing.ShareSoundCoordinatesComponent.gameObject.SetActive(true);
                    }
                }

                anim.SetInteger("Scan", 100); // important so when agent is reached sound coordinate and performing scanning behaviour and immediately while performing scanning behaviour hears another sound
                                              // than it should move to the new sound coordinate and reset the scanning behaviour animations so the transtion do not intruppt with run forward to scan.
                StayNearSoundCoordinate = false;
                GenerateSoundCoorinate = true;
            }

            //Vector3 DistanceCalculatedssd = SoundDestinationToGoTo - transform.position;
            //DistanceCalculatedssd.y = 0;
            //Debug.Log(DistanceCalculatedssd.magnitude);

            if(HealthScript.aiImpactScript != null)
            {
                if (HealthScript.CompleteFirstHitAnimation == true && HealthScript.aiImpactScript.PlayCombinedBodyHitAnimations == true)
                {
                    StopAndPlayBodyHitAnimationFirst = true;
                    if (Components.NavMeshAgentComponent.enabled == true)
                    {
                        Components.NavMeshAgentComponent.isStopped = true;
                    }
                }
                else
                {
                    StopAndPlayBodyHitAnimationFirst = false;
                }
            }
            else
            {
                StopAndPlayBodyHitAnimationFirst = false;
            }
           

            if (AutoRandomiseSoundBehaviour == 0 && AiHearing.MoveTowardsSoundCoordinate == true || ForceMoveTowardsSoundCoordinate == true && AiHearing.MoveTowardsSoundCoordinate == true)
            {
                if(StopAndPlayBodyHitAnimationFirst == false)
                {
                    pathfinder.FindClosestPointTowardsDestination(SoundDestinationToGoTo);

                    pathfinder.closestPoint = SoundDestinationToGoTo;

                    Vector3 DistanceCalculated = pathfinder.closestPoint - transform.position;
                    DistanceCalculated.y = 0;
                    
                    // Debug.Log(DistanceCalculated.magnitude);

                    if (DistanceCalculated.magnitude <= AiHearing.MaxWalkDistance && pathfinder.NoMoreChecks == true && OverriteWalkingForSounds == true)
                    {
                        // LookingAtspecificLocation(pathfinder.closestPoint);
                        if (DistanceCalculated.magnitude <= RandomiseStoppingDistanceFromSound)
                        {
                            IsSprintingTowardsSoundCoordinateCompleted = true;
                            Info("Staying Near the Sound coordinates");
                            enableIkupperbodyRotations(ref ActivateNoIk);
                            if (StayNearSoundCoordinate == false)
                            {
                                StartCoroutine(StayTimerNearSoundCoordinate(AiHearing.MinTimeAtSoundAlertPoint, AiHearing.MaxTimeAtSoundAlertPoint));
                                StayNearSoundCoordinate = true;
                            }
                            if (Components.NavMeshAgentComponent.enabled == true)
                            {
                                Components.NavMeshAgentComponent.isStopped = true;
                            }
                            if (Components.HumanoidFiringBehaviourComponent != null)
                            {
                                //if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
                                //{
                                SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                                ConnectWithUpperBodyAimingAnimation();
                                //}
                            }
                            else
                            {
                                // Idle Animation For Zombies
                                SetAnimationForFullBody(AiAgentAnimatorParameters.IdleParameterName);
                                SetAnimationForFullZombieBody(ZombieAiAnimatorParameters.IdleParameterName);
                                SetAnimationForUpperBody("UpperBodyIdle");
                            }

                        }
                        else if(StayNearSoundCoordinate == false)
                        {
                            Info("Walking Near the Sound coordinates" + " " + "Distance Left" + DistanceCalculated.magnitude);
                            enableIkupperbodyRotations(ref ActivateWalkAimIk);
                            WalkForward(pathfinder.closestPoint, false);
                        }
                    }
                    else if (DistanceCalculated.magnitude <= AiHearing.MaxRunDistance && DistanceCalculated.magnitude > AiHearing.MinRunDistance
                        && pathfinder.NoMoreChecks == true && pathfinder.NavMeshAgentComponent.enabled == true && OverriteRunningForSounds == true)
                    {
                        IsSprintingTowardsSoundCoordinateCompleted = true;
                        Info("Running Near the Sound coordinates" + " " + "Distance Left" + DistanceCalculated.magnitude);
                        //  LookingAtspecificLocation(pathfinder.closestPoint);
                        if (Components.NavMeshAgentComponent.enabled == true)
                        {
                            Components.NavMeshAgentComponent.isStopped = true;
                        }
                        enableIkupperbodyRotations(ref ActivateRunningIk);
                        Run(pathfinder.closestPoint, false);
                    }
                    else if (AiHearing.EnableSprintingTowardsSoundCoordinate == true
                      && DistanceCalculated.magnitude <= AiHearing.MaxSprintDistance && DistanceCalculated.magnitude > AiHearing.MinSprintDistance
                      && pathfinder.NoMoreChecks == true && pathfinder.NavMeshAgentComponent.enabled == true && OverriteSprintingForSounds == true && IsSprintingTowardsSoundCoordinateCompleted == false)
                    {
                        Info("Sprinting towards the Sound coordinates" + " " + "Distance Left" + DistanceCalculated.magnitude);
                        enableIkupperbodyRotations(ref ActivateNoIk);
                        StopSpineRotation = true;
                        IsCrouched = false;
                        if (Components.NavMeshAgentComponent.enabled == true)
                        {
                            Components.NavMeshAgentComponent.isStopped = false;
                        }
                        AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, true);
                        SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                        SetAnimationForUpperBody(AiAgentAnimatorParameters.SprintingParameterName);
                        if (Components.HumanoidFiringBehaviourComponent != null)
                        {
                            Components.HumanoidFiringBehaviourComponent.FireNow = false;
                        }
                    }
                    else if (DistanceCalculated.magnitude <= AiHearing.MaxNearStoppingDistance && DistanceCalculated.magnitude > AiHearing.MinNearStoppingDistance
                        && pathfinder.NoMoreChecks == true)
                    {
                        IsSprintingTowardsSoundCoordinateCompleted = true;
                        Info("Staying Near the Sound coordinates");
                        // LookingAtspecificLocation(pathfinder.closestPoint);
                        enableIkupperbodyRotations(ref ActivateNoIk);
                        if (StayNearSoundCoordinate == false)
                        {
                            StartCoroutine(StayTimerNearSoundCoordinate(AiHearing.MinTimeAtSoundAlertPoint, AiHearing.MaxTimeAtSoundAlertPoint));
                            StayNearSoundCoordinate = true;
                        }
                        if (Components.NavMeshAgentComponent.enabled == true)
                        {
                            Components.NavMeshAgentComponent.isStopped = true;
                        }
                        if (Components.HumanoidFiringBehaviourComponent != null)
                        {
                            //if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
                            //{
                            SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                            ConnectWithUpperBodyAimingAnimation();
                            //}
                        }
                        else
                        {
                            SetAnimationForFullBody(AiAgentAnimatorParameters.IdleParameterName);
                            SetAnimationForFullZombieBody(ZombieAiAnimatorParameters.IdleParameterName);
                            SetAnimationForUpperBody("UpperBodyIdle");
                        }
                    }
                    else if (DistanceCalculated.magnitude <= RandomiseStoppingDistanceFromSound)
                    {
                        IsSprintingTowardsSoundCoordinateCompleted = true;
                        if (StayNearSoundCoordinate == false)
                        {
                            StartCoroutine(StayTimerNearSoundCoordinate(AiHearing.MinTimeToLookAtSoundAlertPoint, AiHearing.MaxTimeToLookAtSoundAlertPoint));
                            StayNearSoundCoordinate = true;
                        }
                        //  LookingAtspecificLocation(SoundDestinationToGoTo);
                        if (Components.NavMeshAgentComponent.enabled == true)
                        {
                            Components.NavMeshAgentComponent.isStopped = true;
                        }
                        if (Components.HumanoidFiringBehaviourComponent != null)
                        {
                            SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                            ConnectWithUpperBodyAimingAnimation();
                        }
                        else
                        {
                            SetAnimationForFullBody(AiAgentAnimatorParameters.IdleParameterName);
                            SetAnimationForFullZombieBody(ZombieAiAnimatorParameters.IdleParameterName);
                            SetAnimationForUpperBody("UpperBodyIdle");
                        }
                    }
                    else
                    {
                        if(OverriteWalkingForSounds == true && OverriteRunningForSounds == true && OverriteSprintingForSounds == true && AiHearing.EnableSprintingTowardsSoundCoordinate == true)
                        {
                            // Basically we will be performing sprinting if the distance exceed max sprint distance.

                            Info("Sprinting towards the Sound coordinates" + " " + "Distance Left" + DistanceCalculated.magnitude);
                            enableIkupperbodyRotations(ref ActivateNoIk);
                            StopSpineRotation = true;
                            IsCrouched = false;
                            if (Components.NavMeshAgentComponent.enabled == true)
                            {
                                Components.NavMeshAgentComponent.isStopped = false;
                            }
                            AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, true);
                            SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                            SetAnimationForUpperBody(AiAgentAnimatorParameters.SprintingParameterName);
                            if (Components.HumanoidFiringBehaviourComponent != null)
                            {
                                Components.HumanoidFiringBehaviourComponent.FireNow = false;
                            }
                        }
                        else if(OverriteWalkingForSounds == true && OverriteRunningForSounds == true && OverriteSprintingForSounds == true && AiHearing.EnableSprintingTowardsSoundCoordinate == false)
                        {
                            // Basically we will be performing running if the distance exceed max sprint distance because EnableSprintingTowardsSoundCoordinate is false.

                            Info("Running Near the Sound coordinates" + " " + "Distance Left" + DistanceCalculated.magnitude);
                            //   LookingAtspecificLocation(pathfinder.closestPoint);
                            enableIkupperbodyRotations(ref ActivateRunningIk);
                            if (Components.NavMeshAgentComponent.enabled == true)
                            {
                                Components.NavMeshAgentComponent.isStopped = false;
                            }
                            Run(pathfinder.closestPoint, false);
                        }
                        else if (OverriteWalkingForSounds == true)
                        {
                            Info("Walking Near the Sound coordinates" + " " + "Distance Left" + DistanceCalculated.magnitude);
                            enableIkupperbodyRotations(ref ActivateWalkAimIk);
                            if (Components.NavMeshAgentComponent.enabled == true)
                            {
                                Components.NavMeshAgentComponent.isStopped = false;
                            }
                            WalkForward(pathfinder.closestPoint, false);
                        }
                        else if (OverriteRunningForSounds == true)
                        {
                            Info("Running Near the Sound coordinates" + " " + "Distance Left" + DistanceCalculated.magnitude);
                            //   LookingAtspecificLocation(pathfinder.closestPoint);
                            enableIkupperbodyRotations(ref ActivateRunningIk);
                            if (Components.NavMeshAgentComponent.enabled == true)
                            {
                                Components.NavMeshAgentComponent.isStopped = false;
                            }
                            Run(pathfinder.closestPoint, false);
                        }
                        else if (OverriteSprintingForSounds == true)
                        {
                            Info("Sprinting towards the Sound coordinates" + " " + "Distance Left" + DistanceCalculated.magnitude);
                            enableIkupperbodyRotations(ref ActivateNoIk);
                            StopSpineRotation = true;
                            IsCrouched = false;
                            if (Components.NavMeshAgentComponent.enabled == true)
                            {
                                Components.NavMeshAgentComponent.isStopped = false;
                            }
                            AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, true);
                            SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                            SetAnimationForUpperBody(AiAgentAnimatorParameters.SprintingParameterName);
                            if (Components.HumanoidFiringBehaviourComponent != null)
                            {
                                Components.HumanoidFiringBehaviourComponent.FireNow = false;
                            }
                        }




                    }
                }
              
               
            }
            else
            {
                if (StayNearSoundCoordinate == false)
                {
                    Info("Looking at the Sound coordinates");
                    StartCoroutine(StayTimerNearSoundCoordinate(AiHearing.MinTimeToLookAtSoundAlertPoint,AiHearing.MaxTimeToLookAtSoundAlertPoint));
                    StayNearSoundCoordinate = true;
                }
                LookingAtspecificLocation(SoundDestinationToGoTo);
                if (Components.NavMeshAgentComponent.enabled == true)
                {
                    Components.NavMeshAgentComponent.isStopped = true;
                }
                if (Components.HumanoidFiringBehaviourComponent != null)
                {
                    SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                    ConnectWithUpperBodyAimingAnimation();
                }
                else
                {
                    if(AgentRole == Role.Zombie)
                    {
                        SetAnimationForFullZombieBody(ZombieAiAnimatorParameters.IdleParameterName);
                    }
                }
            }
        }
        IEnumerator StayTimerNearSoundCoordinate(float MinTimeToStayNearSoundCoordinate,float MaxTimeToStayNearSoundCoordinate)
        {
            float RandomValueToStayNearSoundCoordinate = Random.Range(MinTimeToStayNearSoundCoordinate, MaxTimeToStayNearSoundCoordinate);
            yield return new WaitForSeconds(RandomValueToStayNearSoundCoordinate);
            SearchingForSound = false;
            GenerateSoundCoorinate = false;
            if (AiHearing.ShareSoundCoordinatesComponent != null)
            {
                AiHearing.ShareSoundCoordinatesComponent.Friendlies.Clear();
                AiHearing.ShareSoundCoordinatesComponent.gameObject.SetActive(false);
            }
            ForceMoveTowardsSoundCoordinate = false;
        }
        //IEnumerator AlertTimerForStationaryBot(float AlertTime)
        //{
        //    yield return new WaitForSeconds(AlertTime);
        //    NormaliseStates();
        //}
        public void FindingCoverPoint() // Finding Covers For Cover Bot
        {
            if (CombatStateBehaviours.TakeCovers == true)
            {
                // if (GameObject.FindGameObjectsWithTag("CoverPoint") != null)
                // {
                // GameObject[] Coverpoint = GameObject.FindGameObjectsWithTag("CoverPoint");
                CoverPoints = new CoverNode[AiCovers.CoverFinder.AllCoverNodes.Count];
                CrouchPositions.Clear();

                for (int i = 0; i < AiCovers.CoverFinder.AllCoverNodes.Count; ++i)
                {
                    CoverPoints[i] = AiCovers.CoverFinder.AllCoverNodes[i].GetComponent<CoverNode>();
                   // AiCovers.CoverFinder.AllCoverNodes[i].GetComponent<CoverNode>().CoverNumber = i;
                }

                for (int i = 0; i < CoverPoints.Length; i++)
                {
                    CrouchPositions.Add(CoverPoints[i].transform);

                }

                //Debug.Log(CrouchPositions.Count);
                // FindClosestCoverNode();
                //if (AiCovers.ContinouslyTakeCovers == false)
                //{
                //    StartCoroutine(FindClosestCover());
                //}
                //else
                //{
                if (DeregisterCoverNodes == false)
                {
                    StartCoroutine(FindClosestCoverSystematically());
                }
                //}
                //}
            }
        }
        public void FindingwayPoint() // Finding Waypoint For Waypoint bot
        {
            if (CombatStateBehaviours.UseFiringPoints == true)
            {
                Firepoint = new FiringPoint[AiFiringPoints.FiringPointDetectionBehaviour.FiringPointsFinder.AllFiringPoints.Count];
                //FirePointPositions.Clear();

                for (int i = 0; i < AiFiringPoints.FiringPointDetectionBehaviour.FiringPointsFinder.AllFiringPoints.Count; ++i)
                {
                    if (AiFiringPoints.FiringPointDetectionBehaviour.FiringPointsFinder.AllFiringPoints[i].GetComponent<FiringPoint>() != null)
                    {
                        Firepoint[i] = AiFiringPoints.FiringPointDetectionBehaviour.FiringPointsFinder.AllFiringPoints[i].GetComponent<FiringPoint>();
                        AiFiringPoints.FiringPointDetectionBehaviour.FiringPointsFinder.AllFiringPoints[i].GetComponent<FiringPoint>().CoverNumber = i;
                    }
                }
                WaypointSearched = true;
                FindClosestWayPoint();
            }
        }
        Transform FindClosestFirePointCover()
        {
            Transform closest = null;
            float closestDistance = Mathf.Infinity;

            foreach (FiringPoint FirePoint in Firepoint)
            {
                float distance = Vector3.Distance(transform.position, FirePoint.transform.position);

               // Debug.Log(distance + " " + transform.name + " " + "FirePointName" + FirePoint.name);

                DoNotSprintForNewFiringPointIfWithinAlready();

                if (distance < closestDistance && FirePoint.IsFiringPointAlreadyRegistered == false)//FirePoint.CoverNumber != LastWayPointSelected && 
                {

                    Vector3 DistanceCheckWithEnemyAndCover = FirePoint.transform.position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                    Vector3 DistanceCheckWithMeAndCover = FirePoint.transform.position - transform.position;
                    Vector3 DistanceCheckWithMeAndEnemy = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - transform.position;


                    if (DistanceCheckWithMeAndCover.sqrMagnitude > DistanceCheckWithEnemyAndCover.sqrMagnitude)
                    {
                        findWayPoint = false;
                    }
                    else if (DistanceCheckWithMeAndCover.sqrMagnitude > DistanceCheckWithMeAndEnemy.sqrMagnitude)
                    {
                        findWayPoint = false;
                    }
                    else
                    {
                        if (FirePoint.GetComponent<FiringPoint>().SpecificTeamFiringPoint == true)
                        {
                            //if (FirePoint.GetComponent<FiringPoint>().TeamName == T.MyTeamTag)
                            //{
                            //    for(int x = 0; x < Firepoint.Length; x++)
                            //    {
                            //        if(Firepoint[x] == FirePoint)
                            //        {
                                          ContinueWithFiringPoint = true;
                            //            closestDistance = distance;
                            //            closest = FirePoint.transform;
                            //            //BestWayPoint = x;
                            //            BestWayPoint = FirePoint.CoverNumber;
                            //        }
                            //    }
                            //}

                            if (FirePoint.GetComponent<FiringPoint>().TeamName == T.MyTeamID)
                            {
                                closestDistance = distance;
                                closest = FirePoint.transform;
                                BestWayPoint = FirePoint.CoverNumber;
                                NewFiringPointNode = FirePoint;
                            }
                        }
                        else
                        {
                            //for (int x = 0; x < Firepoint.Length; x++)
                            //{
                            //    if (Firepoint[x] == FirePoint)
                            //    {
                                      ContinueWithFiringPoint = true;
                            //        closestDistance = distance;
                            //        closest = FirePoint.transform;
                            //        //BestWayPoint = x;
                            //        BestWayPoint = FirePoint.CoverNumber;
                            //    }
                            //}

                            closestDistance = distance;
                            closest = FirePoint.transform;
                            //BestWayPoint = x;
                            BestWayPoint = FirePoint.CoverNumber;
                            NewFiringPointNode = FirePoint;

                        }
                    }

                }
            }
            if (ContinueWithFiringPoint == true)
            {
                //Debug.Log(BestWayPoint + "CoverName" + " " + FirePointPositions[BestWayPoint].name);
                ChooseBestFiringPoint();
                PickFiringPoint();
            }
            return closest;
        }
        public void FindClosestWayPoint()
        {
            if (CombatStateBehaviours.UseFiringPoints == true && HealthScript.IsDied == false && WaypointSearched == true)
            {
                int ShouldChangeCover = Random.Range(0, 100);

                if (AiFiringPoints.FiringPointDetectionBehaviour.SwitchingFiringPointsProbability <= ShouldChangeCover)
                {
                    AiFiringPoints.FiringPointDetectionBehaviour.SwitchBetweenFiringPoints = false;
                }
                else
                {
                    AiFiringPoints.FiringPointDetectionBehaviour.SwitchBetweenFiringPoints = true;
                }

                if (AiFiringPoints.FiringPointDetectionBehaviour.FindClosestFiringPoint == true || AiFiringPoints.FiringPointDetectionBehaviour.SwitchBetweenFiringPoints == false)
                {
                    if (findWayPoint == false)
                    {
                        System.Array.Sort(Firepoint, (enemy1, enemy2) =>
                     Vector3.Distance(transform.position, enemy1.transform.position)
                     .CompareTo(Vector3.Distance(transform.position, enemy2.transform.position))
                     );

                        FindClosestFirePointCover();
                    }
                }
                else
                {
                    if (findWayPoint == false)
                    {
                        int i = Random.Range(0, Firepoint.Length);

                        Vector3 DistanceCheckWithEnemyAndCover = Firepoint[i].transform.position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                        Vector3 DistanceCheckWithMeAndCover = Firepoint[i].transform.position - transform.position;
                        Vector3 DistanceCheckWithMeAndEnemy = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - transform.position;
                        if (DistanceCheckWithMeAndCover.sqrMagnitude > DistanceCheckWithEnemyAndCover.sqrMagnitude)
                        {
                            findWayPoint = false;
                        }
                        else if (DistanceCheckWithMeAndCover.sqrMagnitude > DistanceCheckWithMeAndEnemy.sqrMagnitude)
                        {
                            findWayPoint = false;
                        }
                        else
                        {
                            if (Firepoint[i].GetComponent<FiringPoint>().SpecificTeamFiringPoint == true)
                            {
                                if (Firepoint[i].IsFiringPointAlreadyRegistered == false)
                                {
                                    BestWayPoint = Firepoint[i].GetComponent<FiringPoint>().CoverNumber;
                                    NewFiringPointNode = Firepoint[i];
                                    ChooseBestFiringPoint();
                                }
                            }
                            else
                            {
                                if (Firepoint[i].IsFiringPointAlreadyRegistered == false)
                                {
                                    BestWayPoint = Firepoint[i].GetComponent<FiringPoint>().CoverNumber;
                                    NewFiringPointNode = Firepoint[i];
                                    ChooseBestFiringPoint();
                                }
                            }
                        }
                    }
                    // If Random Firing point is not available than go through the loop and find the best available ( but at least find )
                    for (int i = 0; i < Firepoint.Length; i++)
                    {
                        if (findWayPoint == false)
                        {
                            //NewValue++;
                            //if (NewValue >= Firepoint.Length - 1)
                            //{
                            //    NewValue = 0;
                            //}

                            if (FindEnemiesScript.FindedEnemies == true)
                            {
                                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                                {

                                    Vector3 DistanceCheckWithEnemyAndCover = Firepoint[i].transform.position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                                    Vector3 DistanceCheckWithMeAndCover = Firepoint[i].transform.position - transform.position;
                                    Vector3 DistanceCheckWithMeAndEnemy = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - transform.position;
                                    if (DistanceCheckWithMeAndCover.sqrMagnitude > DistanceCheckWithEnemyAndCover.sqrMagnitude)
                                    {
                                        findWayPoint = false;
                                    }
                                    else if (DistanceCheckWithMeAndCover.sqrMagnitude > DistanceCheckWithMeAndEnemy.sqrMagnitude)
                                    {
                                        findWayPoint = false;
                                    }
                                    else
                                    {
                                        if (Firepoint[i].GetComponent<FiringPoint>().SpecificTeamFiringPoint == true)
                                        {
                                            if (Firepoint[i].IsFiringPointAlreadyRegistered == false)
                                            {
                                                BestWayPoint = Firepoint[i].GetComponent<FiringPoint>().CoverNumber;
                                                NewFiringPointNode = Firepoint[i];
                                                ChooseBestFiringPoint();
                                            }
                                        }
                                        else
                                        {
                                            if (Firepoint[i].IsFiringPointAlreadyRegistered == false)
                                            {
                                                BestWayPoint = Firepoint[i].GetComponent<FiringPoint>().CoverNumber;
                                                NewFiringPointNode = Firepoint[i];
                                                ChooseBestFiringPoint();
                                            }
                                        }
                                    }





                                    //if (ChooseRandomWayPoints == false)
                                    //{
                                    //BestWayPoint = Firepoint[i].GetComponent<FiringPoint>().CoverNumber;
                                    //NewFiringPointNode = Firepoint[i];
                                    //ChooseBestFiringPoint();

                                    //disWithWayPoint = FirePointPositions[BestWayPoint].transform.position - transform.position;

                                    //if (FirePointPositions[BestWayPoint].GetComponent<FiringPoint>().SpecificTeamCover == true)
                                    //{
                                    //    if (FirePointPositions[BestWayPoint].GetComponent<FiringPoint>().TeamName == T.FriendlyTeamTag
                                    //        && BestWayPoint != LastWayPointSelected && disWithWayPoint.sqrMagnitude < AiFiringPoints.FiringPointDetectionBehaviour.RangeToFindAFiringPoint * AiFiringPoints.FiringPointDetectionBehaviour.RangeToFindAFiringPoint
                                    //    && Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, this.transform.position) < FindEnemiesScript.DetectionRadius
                                    //    && FirePointPositions[BestWayPoint].GetComponent<FiringPoint>().IsFiringPointAlreadyRegistered == false)
                                    //    {
                                    //        findWayPoint = true;
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    if (BestWayPoint != LastWayPointSelected && disWithWayPoint.sqrMagnitude < AiFiringPoints.FiringPointDetectionBehaviour.RangeToFindAFiringPoint * AiFiringPoints.FiringPointDetectionBehaviour.RangeToFindAFiringPoint
                                    //   && Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, this.transform.position) < FindEnemiesScript.DetectionRadius
                                    //   && FirePointPositions[BestWayPoint].GetComponent<FiringPoint>().IsFiringPointAlreadyRegistered == false)
                                    //    {
                                    //        findWayPoint = true;
                                    //    }
                                    //}



                                    //Vector3 DistanceCheckWithEnemyAndCover = FirePointPositions[BestWayPoint].transform.position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                                    //Vector3 DistanceCheckWithMeAndCover = FirePointPositions[BestWayPoint].transform.position - transform.position;

                                    //if (DistanceCheckWithMeAndCover.sqrMagnitude > DistanceCheckWithEnemyAndCover.sqrMagnitude)
                                    //{
                                    //    findWayPoint = false;
                                    //}

                                    //if (findWayPoint == true)
                                    //{
                                    //    Debug.Log("Bot" + transform.name);
                                    //    FirePointPositions[BestWayPoint].GetComponent<FiringPoint>().IsFiringPointAlreadyRegistered = true;
                                    //    CurrentWayPoint = BestWayPoint;
                                    //    WaypointsFinded = true;
                                    //}


                                }
                            }
                        }
                    }

                    PickFiringPoint();
                }



            }
        }
        public void DoNotSprintForNewFiringPointIfWithinAlready()
        {
            //if (CurrentWayPoint <= Firepoint.Length)
            //{
            if (AiFiringPoints.FiringPointDetectionBehaviour.SwitchBetweenFiringPoints == false)
            {
                ChangeCover = true;
                ReachnewFirepoint = false;
                if (PreviousFiringPointNode != null)
                {
                    LastWayPointSelected = 999999;
                    PreviousFiringPointNode.DistanceCleared = false;
                    PreviousFiringPointNode.IsFiringPointAlreadyRegistered = false;
                }

                if (DoHavePreviousFiringPoint == false)
                {
                    DoHavePreviousFiringPoint = true;
                }

                // LastWayPointSelected = 99999;
            }
            //}
        }
        public void DeregisterWaypoint()
        {
            if (Deregisterfirepoint == false)
            {
                //if(Firepoint != null)
                //{
                //    if (Firepoint.Length >= 1)
                //    {
                        if (NewFiringPointNode != null)
                        {
                            if (NewFiringPointNode.IsFiringPointAlreadyRegistered == true)
                            {
                                NewFiringPointNode.Info(false);
                                NewFiringPointNode.DistanceCleared = false;
                                NewFiringPointNode.IsFiringPointAlreadyRegistered = false;
                            }
                        }

                //    }
                //}
               
                Deregisterfirepoint = true;
            }

        }
        public void ChooseBestFiringPoint()
        {
            if (FindEnemiesScript.FindedEnemies == true)
            {
                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                {
                    //if (ChooseRandomWayPoints == false)
                    //{
                    disWithWayPoint = NewFiringPointNode.transform.position - transform.position;

                    if (NewFiringPointNode.SpecificTeamFiringPoint == true)
                    {
                        if (NewFiringPointNode.TeamName == T.MyTeamID
                            && BestWayPoint != LastWayPointSelected && disWithWayPoint.sqrMagnitude < AiFiringPoints.FiringPointDetectionBehaviour.RangeToFindAFiringPoint * AiFiringPoints.FiringPointDetectionBehaviour.RangeToFindAFiringPoint
                        && Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, this.transform.position) < FindEnemiesScript.DetectionRadius
                        && NewFiringPointNode.IsFiringPointAlreadyRegistered == false)
                        {
                            findWayPoint = true;
                        }
                    }
                    else
                    {
                        if (BestWayPoint != LastWayPointSelected && disWithWayPoint.sqrMagnitude < AiFiringPoints.FiringPointDetectionBehaviour.RangeToFindAFiringPoint * AiFiringPoints.FiringPointDetectionBehaviour.RangeToFindAFiringPoint
                       && Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, this.transform.position) < FindEnemiesScript.DetectionRadius
                       && NewFiringPointNode.IsFiringPointAlreadyRegistered == false)
                        {
                            findWayPoint = true;
                        }
                    }



                    Vector3 DistanceCheckWithEnemyAndCover = NewFiringPointNode.transform.position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                    Vector3 DistanceCheckWithMeAndCover = NewFiringPointNode.transform.position - transform.position;
                    Vector3 DistanceCheckWithMeAndEnemy = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - transform.position;

                    if (DistanceCheckWithMeAndCover.sqrMagnitude > DistanceCheckWithEnemyAndCover.sqrMagnitude)
                    {
                        findWayPoint = false;
                    }
                    else if (DistanceCheckWithMeAndCover.sqrMagnitude > DistanceCheckWithMeAndEnemy.sqrMagnitude)
                    {
                        findWayPoint = false;
                    }

                    if (findWayPoint == true)
                    {
                        //  Debug.Log("Bot" + transform.name);
                        NewFiringPointNode.IsFiringPointAlreadyRegistered = true;
                        CurrentWayPoint = BestWayPoint;
                        WaypointsFinded = true;
                    }

                    //}
                    //else
                    //{
                    //    int Randomise = Random.Range(0, WayPointPositions.Count);
                    //    disWithWayPoint = WayPointPositions[Randomise].transform.position - transform.position;
                    //    if (Randomise != LastWayPoint && disWithWayPoint.sqrMagnitude < AiWaypoints.WaypointDetectionBehaviour.RangeToFindWayPoint * AiWaypoints.WaypointDetectionBehaviour.RangeToFindWayPoint
                    //        && Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, this.transform.position) < Detections.DetectionRadius
                    //        && WayPointPositions[CurrentWayPoint].GetComponent<WayPoint>().IsFiringPointAlreadyRegistered == false)
                    //    {
                    //        WayPointPositions[CurrentWayPoint].GetComponent<WayPoint>().IsFiringPointAlreadyRegistered = true;
                    //        CurrentWayPoint = Randomise;
                    //        findWayPoint = true;
                    //    }
                    //}
                }
            }
        }
        public void PickFiringPoint()
        {
            if (findWayPoint == false)
            {
                ChangeCover = false;
                //if (DoHavePreviousFiringPoint == true)
                //{
                //    ReachnewFirepoint = true;
                //}
                // Line from 2720 to 2744 is added on 08/09/2023
                if (LastWayPointSelected != CurrentWayPoint)
                {
                    if (LastWayPointSelected <= Firepoint.Length)
                    {
                        if(PreviousFiringPointNode != null)
                        {
                            PreviousFiringPointNode.IsFiringPointAlreadyRegistered = false;
                            PreviousFiringPointNode.DistanceCleared = false;
                        }
                       
                    }
                    ChangeCover = true;
                    ReachnewFirepoint = false;
                }
                else
                {
                    
                    // Check Distance with CurrentWayPoint if its not completed than move towards it : Added on 29 jan 2024
                    //if (horizontalDisplacement.magnitude > AiFiringPoints.FiringPointDetectionBehaviour.DistanceToStopBeforeFiringPoint)
                    //{
                    //    Firepoint[CurrentWayPoint].GetComponent<FiringPoint>().DistanceCleared = false;
                    //    Firepoint[CurrentWayPoint].GetComponent<FiringPoint>().IsFiringPointAlreadyRegistered = false;
                    //}

                    //if (Firepoint[CurrentWayPoint].GetComponent<FiringPoint>().DistanceCleared == false)
                    //{
                    //    PreviousDestinationWhenRunning = Vector3.positiveInfinity;
                    //    ReachnewFirepoint = false;
                    //}
                    //else
                    //{
                    //    ReachnewFirepoint = true;
                    //}

                    // Added on 5th Feb 2024
                    //if (Firepoint[CurrentWayPoint].GetComponent<FiringPoint>().DistanceCleared == false)
                    //{
                    //    ReachnewFirepoint = false;
                    //}
                    //else
                    //{
                    //    ReachnewFirepoint = true;
                    //}
                    //findWayPoint = true;
                    //ChangeCover = false;
                }
            }
            else
            {
               
                ContinueWithFiringPoint = true;
                // Debug.Log(CurrentWayPoint + transform.name);
                ChangeCover = true;
                ReachnewFirepoint = false;
                if (LastWayPointSelected <= Firepoint.Length)
                {
                    if (PreviousFiringPointNode != null)
                    {
                        PreviousFiringPointNode.GetComponent<FiringPoint>().DistanceCleared = false;
                        PreviousFiringPointNode.GetComponent<FiringPoint>().IsFiringPointAlreadyRegistered = false;
                    }
                }

                if(PreviousFiringPointNode != null && AiFiringPoints.FiringPointDetectionBehaviour.SwitchBetweenFiringPoints == false)
                {
                    if(PreviousFiringPointNode == NewFiringPointNode)
                    {
                        NewFiringPointNode.DistanceCleared = true;
                        ReachnewFirepoint = true;
                    }
                }

                if (DoHavePreviousFiringPoint == false)
                {
                    DoHavePreviousFiringPoint = true;
                }

            }
        }
        //public void FindClosestCoverNode()
        //{
        //    if(ChoosenAiTypeIsCover == true)
        //    {
        //        for (int i = NewValueForCover; i < CrouchPositions.Count; i++)
        //        {
        //            if (FindValidCover == true)
        //            {
        //                NewValueForCover++;
        //                if(NewValueForCover >= CrouchPositions.Count - 1)
        //                {
        //                    NewValueForCover = 0;
        //                }
        //                LastCoverPoint = CurrentCoverPoint;
        //                if (FindEnemiesScript.FindedEnemies == true) 
        //                 {

        //                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != this.transform)
        //                    {
        //                        CrouchPositions[i].GetComponent<CoverNode>().CheckifEnemyIsInView(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
        //                    }
        //                    disWithCoverPoint = CrouchPositions[i].transform.position - transform.position;
        //                    if (CrouchPositions[i].transform.GetComponent<CoverNode>().ValidCover == true && i != LastCoverPoint && CrouchPositions[i].gameObject.GetComponent<CoverNode>().TriggerOnce == false && disWithCoverPoint.sqrMagnitude < RangeToFindCoverPoint * RangeToFindCoverPoint)
        //                    {
        //                        FindValidCover = false;
        //                        CurrentCoverPoint = i;
        //                    }
        //                 }

        //            }
        //        }
        //    }
        //}
        void CheckForValidCover()   // Checking if the Cover is Valid Or not To go
        {
            if (CombatStateBehaviours.TakeCovers == true && CrouchPositions.Count > 0)
            {
                if (CombatStarted == true)
                {
                    if (HealthScript.IsDied == false)
                    {
                        FindingNewCrouchPoint = false;
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != this.transform)
                        {
                            if (NewCoverNode != null)
                            {
                                NewCoverNode.CheckifEnemyIsInView(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
                                if (NewCoverNode.IsValidCover == false)
                                {
                                    if (FindingNewCrouchPoint == false)
                                    {
                                        ChangeCover = true;
                                        ReachnewCoverpoint = false;
                                        FindValidCover = true;
                                        //if (AiCovers.ContinouslyTakeCovers == false)
                                        //{
                                        //    StartCoroutine(FindClosestCover());
                                        //}
                                        //else
                                        //{
                                        if (DeregisterCoverNodes == false)
                                        {
                                            StartCoroutine(FindClosestCoverSystematically());
                                        }
                                        //}
                                        FindingNewCrouchPoint = true;
                                    }

                                    if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                                    {
                                        Fire();
                                    }
                                }

                            }
                            else
                            {
                                if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                                {
                                    Fire();
                                }
                            }
                        }

                    }
                }
            }
        }
        //public void NormaliseStates()
        //{
        //    Detections.DetectionRadius = SaveDetectingDistance;
        //    SearchingForSound = false;
        //    Gunscript.instance.IsFire = false;
        //    ResetToNormalState = false;
        //    FriendlyShot = false;
        //    SoundAlreadyHeard = false;
        //    IsShotMade = false;
        //}
        //public void Hearing() // Hearing functionality
        //{
        //    if (CheckForCombat.instance.IsShootingStarted == true && CheckForCombat.instance.PositionPoint != transform.root && SoundAlreadyHeard == false)
        //    {
        //        if (AiHearing.RandomiseHearing == true)
        //        {
        //            int randomise = Random.Range(0, 1);
        //            if (randomise == 1)
        //            {
        //                FriendlyShot = true;
        //                FriendlybotShotPos = CheckForCombat.instance.PositionPoint.position;
        //                IsShotMade = true;
        //                CheckForFireShot(FriendlybotShotPos);
        //            }
        //        }
        //        else
        //        {
        //            FriendlyShot = true;
        //            FriendlybotShotPos = CheckForCombat.instance.PositionPoint.position;
        //            IsShotMade = true;
        //            CheckForFireShot(FriendlybotShotPos);
        //        }

        //    }
        //    else if (Gunscript.instance.IsFire == true)
        //    {
        //        if (AiHearing.RandomiseHearing == true)
        //        {
        //            int randomise = Random.Range(0, 1);
        //            if (randomise == 1)
        //            {
        //                FriendlyShot = false;
        //                IsShotMade = true;
        //                SoundAlreadyHeard = true;
        //                CheckForFireShot(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position);
        //            }
        //        }
        //        else
        //        {
        //            FriendlyShot = false;
        //            IsShotMade = true;
        //            SoundAlreadyHeard = true;
        //            CheckForFireShot(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position);
        //        }
        //    }
        //    //else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<MasterAiBehaviour>() != null)
        //    //{
        //    //    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<MasterAiBehaviour>().HumanoidFiringBehaviourComponent.IFired == true)
        //    //    {
        //    //        if (AiHearing.RandomiseHearing == true)
        //    //        {
        //    //            int randomise = Random.Range(0, 1);
        //    //            if (randomise == 1)
        //    //            {
        //    //                FriendlyShot = false;
        //    //                IsShotMade = true;
        //    //                SoundAlreadyHeard = true;
        //    //                CheckForFireShot(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position);
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            FriendlyShot = false;
        //    //            IsShotMade = true;
        //    //            SoundAlreadyHeard = true;
        //    //            CheckForFireShot(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position);
        //    //        }
        //    //    }
        //    //}
        //    else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<CoreAiBehaviour>() != null)
        //    {
        //        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.IFired == true)
        //        {
        //            if (AiHearing.RandomiseHearing == true)
        //            {
        //                int randomise = Random.Range(0, 1);
        //                if (randomise == 1)
        //                {
        //                    FriendlyShot = false;
        //                    IsShotMade = true;
        //                    SoundAlreadyHeard = true;
        //                    CheckForFireShot(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position);
        //                }
        //            }
        //            else
        //            {
        //                FriendlyShot = false;
        //                IsShotMade = true;
        //                SoundAlreadyHeard = true;
        //                CheckForFireShot(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position);
        //            }
        //        }
        //    }
        //}
        public void LookingAtCommanderToFollow()
        {
            RotatingTransforms.ChangeRotation(transform, BossTransform.transform.position, transform.position, Speeds.MovementSpeeds.BodyRotationSpeed);
        }

        public void PerformGuardDuty()
        {
            Vector3 dirwithtarget = transform.position - BossTransform.transform.position;


            //if(FollowCommanderValues.ForceCombatStateOnFollowersInCaseCommanderGetInto == true)
            //{
            //    if (BossTransform != null)
            //    {
            //        if (BossTransform.GetComponent<CoreAiBehaviour>() != null)
            //        {
            //            if (BossTransform.GetComponent<CoreAiBehaviour>().CombatStarted == true)
            //            {
            //                CombatStarted = true;
            //                T.EnabledFOV = false;
            //                FindEnemiesScript.EnableFieldOfView = false;
            //            }
            //        }
            //    }
            //}


            if (dirwithtarget.magnitude <= FollowCommanderValues.MaxWalkingDistanceToLeader)
            {

                if (dirwithtarget.magnitude <= FollowCommanderValues.StoppingDistanceToCommander)
                {
                    // The below codes are commented because if enabled than as the Ai agent has to follow the target in this case there case they check for leader transform position rather than
                    // creating the closest point to the leader.due to which it will cause snapping between NavMeshAgent and NavMeshObstacle components so better to keep it commented.
                    // Components.NavMeshAgentComponent.enabled = false;
                    //NavMeshObstacleComponent.enabled = true;
                    SetAnimationForFullBody(AiAgentAnimatorParameters.IdleParameterName);
                    if (Components.NavMeshAgentComponent.enabled == true)
                    {
                        Components.NavMeshAgentComponent.isStopped = true;
                    }
                    enableIkupperbodyRotations(ref ActivateNoIk);
                    OverwriteSprinting = false;

                }
                else
                {
                    Components.NavMeshAgentComponent.enabled = true;
                    Info("Walking Near the Leader" + " " + "DistanceLeft" + dirwithtarget.magnitude);
                    enableIkupperbodyRotations(ref ActivateWalkAimIk);
                    WalkForward(BossTransform.transform.position, false);
                }
            }
            else if (dirwithtarget.magnitude <= FollowCommanderValues.MaxRunningDistanceToLeader && dirwithtarget.magnitude > FollowCommanderValues.MinRunningDistanceToLeader || OverriteRunningWhenFollowingCommander == true)
            {
                ApplyRootMotion(false);

                Components.NavMeshAgentComponent.enabled = true;
                //NavMeshObstacleComponent.enabled = false;


                Components.NavMeshAgentComponent.isStopped = false;

                Components.HumanoidAiAudioPlayerComponent.PlayRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.RecurringSounds.FollowerAudioClips);
                StopSpineRotation = true;
                SprintingActivated = true;
                enableIkupperbodyRotations(ref ActivateRunningIk);
                //  WalkForward();
                Run(BossTransform.transform.position, false);
                //LookingAtCommanderToFollow(); // commented so while sprinting we look at the path ( update navigation rotation ) not to look at commander as if we uncomment it than Ai will look wierd while
                // sprinting to commander. 
                Components.HumanoidFiringBehaviourComponent.FireNow = false;
                OverriteRunningWhenFollowingCommander = true;
                Info("Running Near the Leader" + " " + "DistanceLeft" + dirwithtarget.magnitude);

                if (dirwithtarget.magnitude > FollowCommanderValues.MaxSprintingDistanceToLeader)
                {
                    OverriteRunningWhenFollowingCommander = false;
                }
                //if (dirwithtarget.magnitude < FollowCommanderValues.ClosestDistanceToRun)
                //{
                //    OverriteRunningWhenFollowingCommander = false;
                //}
                if (Components.NavMeshAgentComponent.enabled == true)
                {
                    Components.NavMeshAgentComponent.destination = BossTransform.transform.position;
                }

                IsScanning = false;


            }
            else if (dirwithtarget.magnitude <= FollowCommanderValues.MaxSprintingDistanceToLeader && dirwithtarget.magnitude > FollowCommanderValues.MinSprintingDistanceToLeader && OverriteRunningWhenFollowingCommander == false
                || dirwithtarget.magnitude > FollowCommanderValues.MaxSprintingDistanceToLeader)
            {
                Components.NavMeshAgentComponent.enabled = true;
                //NavMeshObstacleComponent.enabled = false;

                //if (dirwithtarget.magnitude <= FollowCommanderValues.MaxSprintingDistanceToLeader + 0.1f && dirwithtarget.magnitude > FollowCommanderValues.MinSprintingDistanceToLeader + 0.1f)
                //{
                Components.NavMeshAgentComponent.isStopped = false;

                Components.HumanoidAiAudioPlayerComponent.PlayRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.RecurringSounds.FollowerAudioClips);
                enableIkupperbodyRotations(ref ActivateSprintingIk);
                StopSpineRotation = true;
                IsScanning = false;
                ApplyRootMotion(false);
                OverwriteSprinting = true;
                SprintingActivated = true;
                //LookingAtCommanderToFollow(); // commented so while sprinting we look at the path ( update navigation rotation ) not to look at commander as if we uncomment it than Ai will look wierd while
                // sprinting to commander. 
                SprintingTowardsCommander();
                Info("Sprinting Near the Leader" + " " + "DistanceLeft" + dirwithtarget.magnitude);
                //if (BossTransform.transform.position != PrevDestination)
                //{
                //    StartCoroutine(ReEnableNavigation());
                //    CanStartMoving = false;
                //    PrevDestination = BossTransform.transform.position;
                //}

                //if (CanStartMoving == true)
                //{
                if (Components.NavMeshAgentComponent.enabled == true)
                {
                    Components.NavMeshAgentComponent.destination = BossTransform.transform.position;
                }
                //}

                if(Components.HumanoidFiringBehaviourComponent != null)
                {
                    Components.HumanoidFiringBehaviourComponent.FireNow = false;

                }

                if (dirwithtarget.magnitude < FollowCommanderValues.StoppingDistanceToCommander)
                {
                    OverwriteSprinting = false;
                }
                //}
            }







            //if (dirwithtarget.magnitude > FollowCommanderValues.MinStoppingDistanceToLeader && dirwithtarget.magnitude < FollowCommanderValues.MaxStoppingDistanceToLeader && OverriteRunningWhenFollowingCommander == false)
            //{
            //if (FindEnemiesScript.ContainThisTransform == false)
            //{
            //    ApplyRootMotion(false);
            //    SprintingActivated = false;
            //    if (!Components.HumanoidFiringBehaviourComponent.isreloading)
            //    {
            //        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            //        {
            //            RotatingTransforms.ChangeRotation(transform, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, transform.position, MovementSpeeds.BodyRotationSpeed);
            //        }
            //        StopSpineRotation = false;
            //        ShootingController();
            //    }
            //    else
            //    {
            //        StopSpineRotation = false;
            //        Reload();
            //        Components.HumanoidFiringBehaviourComponent.FireNow = false;
            //    }
            //    IsScanning = false;
            //}
            //else
            //{

            //if(NonCombatBehaviours.DefaultBehaviour == InvestigationTypes.Scanning)
            //{
            //    StopSpineRotation = true;
            //    IsScanning = true;
            //    SetAnimationForFullBody(AiAgentAnimatorParameters.IdleParameterName);
            //    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
            //    SearchingState();
            //    SprintingActivated = true;
            //    Components.NavMeshAgentComponent.isStopped = true;
            //    Components.HumanoidFiringBehaviourComponent.FireNow = false;
            //}

            //}

            //}
            //else if (dirwithtarget.magnitude > FollowCommanderValues.MinRunningDistanceToLeader
            //    && dirwithtarget.magnitude < FollowCommanderValues.MaxRunningDistanceToLeader && OverwriteSprinting == false || OverriteRunningWhenFollowingCommander == true)
            //{
            //}
            //else if (dirwithtarget.magnitude > FollowCommanderValues.MinSprintingDistanceToLeader || OverwriteSprinting == true)
            //{

            //}
        }
        public void SprintingTowardsCommander()
        {
            //if (BossTransform.transform.position != PrevDestination)
            //{
            //    StartCoroutine(ReEnableNavigation());
            //    CanStartMoving = false;
            //    PrevDestination = BossTransform.transform.position;
            //}

            //if (CanStartMoving == true)
            //{
                if (Components.NavMeshAgentComponent.enabled == true)
                {
                    Components.NavMeshAgentComponent.destination = BossTransform.transform.position;
                }
            //}

            if (anim != null)
            {
                // AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);
                SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                AnimController(false, Speeds.MovementSpeeds.SprintSpeed, AiAgentAnimatorParameters.DefaultStateParameterName, false, false);
            }
        }
        public void UpdateNavigationRotation(bool ShouldUpdate)
        {
            if (CombatStarted == true)
            {
                Components.NavMeshAgentComponent.updateRotation = ShouldUpdate;
            }
            else
            {
                Components.NavMeshAgentComponent.updateRotation = true;
            }

            // Calculate the rotation angle based on the velocity direction

            //Vector3 velocity = Components.NavMeshAgentComponent.velocity;

            //if (velocity.magnitude > 0)
            //{
            //    Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);

            //    // Smoothly rotate the agent towards the desired rotation
            //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            //}
        }
        public void InitCoverBehaviour()
        {
            if (IsPreviouslyFiringBehaviour == true)
            {
                DeregisterWaypoint();
            }
            if (IsCoverBehaviourSelected == false && StartIterating == true && IsPreviouslyCoverBehaviour == false)// && InitialiseCoverBehaviour == true)
            {
                
                // Debug.Break();
                //FindValidCover = true; 
                //PreviousCoverNum = CurrentCoverPoint; 
                //CanTakeCover = false;
                MoveTowardsOpenFirePoint = false;
                StopBotForShoot = false;
                ReachnewCoverpoint = false;
                DeregisterCoverNodes = false;
                // DeregisterCover();
                //StartCoroutine(FindClosestCoverSystematically());
                // SaveResetedCoverRandomisation = 0f;
                BotMovingAwayFromGrenade = false;
                IsCoverBehaviourSelected = true;
                IsChargingBehaviourSelected = false;
                IsFiringBehaviourSelected = false;
                MoveToNewStrafePoint = false;
                StrafingForChargingAgent = false;
            }
            IsPreviouslyCoverBehaviour = true;
            IsPreviouslyChargingBehaviour = false;
            IsPreviouslyFiringBehaviour = false;

            //Debug.Log("State Name Currently Running is Cover behaviour");
        }
        public void InitChargeBehaviour()
        {
            if(IsPreviouslyCoverBehaviour == true)
            {
                DeregisterCover();
            }
            else if(IsPreviouslyFiringBehaviour == true)
            {
                DeregisterWaypoint();
            }
            if (IsChargingBehaviourSelected == false && StartIterating == true && IsPreviouslyChargingBehaviour == false)// && InitialiseChargingBehaviour == true)
            {
             
                //  Debug.Break();
                StopBotForShoot = false;
                MoveTowardsOpenFirePoint = false;
                StopBotForShoot = false;
                ReachnewCoverpoint = false;
                ReachnewFirepoint = false;
                BotMovingAwayFromGrenade = false;
                IsChargingBehaviourSelected = true;
                IsCoverBehaviourSelected = false;
                IsFiringBehaviourSelected = false;
                MoveToNewStrafePoint = false;
                StrafingForChargingAgent = false;
            }
            //if(IsPreviouslyCoverBehaviour == true)
            //{
            //    DeregisterNodes = false;
            //    DeregisterCover();
            //}
            //else if (IsPreviouslyFiringBehaviour == true)
            //{
            //    Deregisterfirepoint = false;
            //    DeregisterWaypoint();
            //    FindClosestWayPoint();
            //    DoHavePreviousFiringPoint = false;
            //}
            IsPreviouslyCoverBehaviour = false;
            IsPreviouslyChargingBehaviour = true;
            IsPreviouslyFiringBehaviour = false;

           // Debug.Log("State Name Currently Running is Charge behaviour");
        }
        public void InitFiringBehaviour()
        {
            if (IsPreviouslyCoverBehaviour == true)
            {
                DeregisterCover();
            }
            if (IsFiringBehaviourSelected == false && StartIterating == true && IsPreviouslyFiringBehaviour == false)// && InitialiseFiringPointsBehaviour == true)
            {
                // Debug.Break();              
                MoveTowardsOpenFirePoint = false;
                StopBotForShoot = false;
                ReachnewFirepoint = false;
                Deregisterfirepoint = false;
                //DeregisterWaypoint();
                BotMovingAwayFromGrenade = false;
                FindClosestWayPoint();
                DoHavePreviousFiringPoint = false;
                // SaveResetedWaypointRandomisation = 0f;
                IsFiringBehaviourSelected = true;
                IsCoverBehaviourSelected = false;
                IsChargingBehaviourSelected = false;
                MoveToNewStrafePoint = false;
                StrafingForChargingAgent = false;
            }

            IsPreviouslyCoverBehaviour = false;
            IsPreviouslyChargingBehaviour = false;
            IsPreviouslyFiringBehaviour = true;

          //  Debug.Log("State Name Currently Running is Firing point behaviour");
        }
        //private void FootStepSound()
        //{
        //    AudioClip clip = GetRandomClip();
        //    //Components.FootStepsAudioSource.PlayOneShot(clip);
        //}
        //private AudioClip GetRandomClip()
        //{
        //    return Components.FootStepsSounds[Random.Range(0, Components.FootStepsSounds.Length)];
        //}
        //IEnumerator QuickScanNewAnimationTurnTimer()
        //{
        //    float Randomise = Random.Range(QuickScanBehaviour.MinTimeBetweenTurns, QuickScanBehaviour.MaxTimeBetweenTurns);
        //    yield return new WaitForSeconds(Randomise);
        //    if(WasInCombatStateBefore == true)
        //    {
        //        StartCoroutine(QuickScanNewAnimationTurnTimer());
        //    }
        //}
        IEnumerator QuickScanCompletionTimer()
        {
            yield return new WaitForSeconds(0.1f);
            IsInDefaultInvestigation = true;
            float Randomise = Random.Range(PostCombatAimedScanBehaviour.MinScanCompletionTime, PostCombatAimedScanBehaviour.MaxScanCompletionTime);
            yield return new WaitForSeconds(Randomise);
            ResetVariableForQuickScan();
        }
        public void ResetVariableForQuickScan()
        {
            WasInCombatStateBefore = false;
            startquickscan = false;
            TimeToDetect = false;
            ImmediateStartScanForTheFirstTime = false;
            if (ScanScript.HeadIKScript != null)
            {
                ScanScript.HeadIKScript.StopTurning = false;
            }
            if (IsCoroutineReseted == false)
            {
                StopCoroutine(QuickScanCompletionTimer());
                IsCoroutineReseted = true;
            }
        }
        public void ActivateQuickScanning()
        {
            if (WasInCombatStateBefore == true && !Components.HumanoidFiringBehaviourComponent.isreloading)
            {
                if (startquickscan == false)
                {
                    ScanValueAfterCombat = 1000;
                    SetAnimationForFullBody("null");
                    SetAnimationForUpperBody("null");
                    IsCoroutineReseted = false;
                    StartCoroutine(QuickScanCompletionTimer());
                    startquickscan = true;
                }
                //Stop NavMesh Agent
                if (Components.NavMeshAgentComponent.enabled == true)
                {
                    if (Components.NavMeshAgentComponent != null)
                    {
                        Components.NavMeshAgentComponent.speed = 0f;
                        Components.NavMeshAgentComponent.isStopped = true;
                    }
                }
                SearchingState();
            }
        }
        IEnumerator DelayBeforeRespondingToSoundsAndEmergencyAlertTriggers()
        {
            yield return new WaitForSeconds(0.1f);
            IsInDefaultInvestigation = true;

        }
        public void DefaultInvestigationBehaviour()
        {
            CombatStarted = false; // NEWLY ADDED ON 10TH FEB,2024
            if (CombatStateBehaviours.EnablePostCombatScan == true && WasInCombatStateBefore == true && AgentRole != Role.Zombie)
            {
                ActivateQuickScanning();
            }
            else
            {
                WasInCombatStateBefore = false;
                if (StartDelaySoundsAndEmergencyAlertTriggersTimer == false)
                {
                    StartCoroutine(DelayBeforeRespondingToSoundsAndEmergencyAlertTriggers());
                    StartDelaySoundsAndEmergencyAlertTriggersTimer = true;
                }
            }

            if (SearchingForSound == false && WasInCombatStateBefore == false)
            {
                if (NonCombatBehaviours.DefaultBehaviour == InvestigationTypes.Patrol)
                {
                    if (Components.AiNonCombatChatterComponent != null)
                    {
                        if (PlayDefaultBehaviourSoundsNow == true && IsAgentRoleLeader == false && AgentRole != Role.Follower && Components.AiNonCombatChatterComponent.gameObject.activeInHierarchy == false)
                        {
                            Components.HumanoidAiAudioPlayerComponent.PlayRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.RecurringSounds.DefaultBehaviourAudioClips);
                        }
                    }
                    else
                    {
                        if (PlayDefaultBehaviourSoundsNow == true && IsAgentRoleLeader == false && AgentRole != Role.Follower)
                        {
                            Components.HumanoidAiAudioPlayerComponent.PlayRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.RecurringSounds.DefaultBehaviourAudioClips);
                        }
                    }
                   

                    if (Components.AiNonCombatChatterComponent != null)
                    {
                        if (Components.AiNonCombatChatterComponent.gameObject.activeInHierarchy == false)
                        {
                            Info("Patrolling");
                            PatrolingScript.AiPatrol();
                        }
                        else
                        {
                            Info("Ai Non Combat Chatter is Activated");
                        }
                    }
                    else
                    {
                        Info("Patrolling");
                        PatrolingScript.AiPatrol();
                    }
                 
                   
                }
                else if (NonCombatBehaviours.DefaultBehaviour == InvestigationTypes.Wander)
                {
                    if (Components.AiNonCombatChatterComponent != null)
                    {
                        if (PlayDefaultBehaviourSoundsNow == true && IsAgentRoleLeader == false && AgentRole != Role.Follower && Components.AiNonCombatChatterComponent.gameObject.activeInHierarchy == false)
                        {
                            Components.HumanoidAiAudioPlayerComponent.PlayRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.RecurringSounds.DefaultBehaviourAudioClips);
                        }
                    }
                    else
                    {
                        if (PlayDefaultBehaviourSoundsNow == true && IsAgentRoleLeader == false && AgentRole != Role.Follower)
                        {
                            Components.HumanoidAiAudioPlayerComponent.PlayRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.RecurringSounds.DefaultBehaviourAudioClips);
                        }
                    }

                    if(Components.AiNonCombatChatterComponent != null)
                    {
                        if (Components.AiNonCombatChatterComponent.gameObject.activeInHierarchy == false)
                        {
                            Info("Wandering");
                            WanderingScript.Wander();
                        }
                        else
                        {
                            Info("Ai Non Combat Chatter is Activated");
                        }
                    }
                    else
                    {
                        Info("Wandering");
                        WanderingScript.Wander();
                    }
                   
                }
                else if (NonCombatBehaviours.DefaultBehaviour == InvestigationTypes.Scan)
                {
                    if (Components.AiNonCombatChatterComponent != null)
                    {
                        if (PlayDefaultBehaviourSoundsNow == true && IsAgentRoleLeader == false && AgentRole != Role.Follower && Components.AiNonCombatChatterComponent.gameObject.activeInHierarchy == false)
                        {
                            Components.HumanoidAiAudioPlayerComponent.PlayRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.RecurringSounds.DefaultBehaviourAudioClips);
                        }
                    }
                    else
                    {
                        if (PlayDefaultBehaviourSoundsNow == true && IsAgentRoleLeader == false && AgentRole != Role.Follower)
                        {
                            Components.HumanoidAiAudioPlayerComponent.PlayRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.RecurringSounds.DefaultBehaviourAudioClips);
                        }
                    }

                    if (Components.AiNonCombatChatterComponent != null)
                    {
                        if (Components.AiNonCombatChatterComponent.gameObject.activeInHierarchy == false)
                        {
                            Info("Scanning");
                            SearchingState();
                        }
                        else
                        {
                            Info("Ai Non Combat Chatter is Activated");
                        }
                    }
                    else
                    {
                        Info("Scanning");
                        SearchingState();
                    }
                }
                else
                {
                    if (Components.AiNonCombatChatterComponent != null)
                    {
                        if (PlayDefaultBehaviourSoundsNow == true && IsAgentRoleLeader == false && AgentRole != Role.Follower && Components.AiNonCombatChatterComponent.gameObject.activeInHierarchy == false)
                        {
                            Components.HumanoidAiAudioPlayerComponent.PlayRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.RecurringSounds.DefaultBehaviourAudioClips);
                        }
                    }
                    else
                    {
                        if (PlayDefaultBehaviourSoundsNow == true && IsAgentRoleLeader == false && AgentRole != Role.Follower)
                        {
                            Components.HumanoidAiAudioPlayerComponent.PlayRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.RecurringSounds.DefaultBehaviourAudioClips);
                        }
                    }

                    Idle();

                    if (Components.AiNonCombatChatterComponent != null)
                    {
                        if (Components.AiNonCombatChatterComponent.gameObject.activeInHierarchy == false)
                        {
                            Info("Idle");
                        }
                        else
                        {
                            Info("Ai Non Combat Chatter is Activated");
                        }
                    }
                    else
                    {
                        Info("Idle");
                    }
                }

                if (HealthScript != null)
                {
                    if(HealthScript.KeepHealthBarDeactivatedInNonCombat == true)
                    {
                        if (HealthScript.SpawnedHealthBarUI != null)
                        {
                            HealthScript.SpawnedHealthBarUI.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
        IEnumerator CoroForMeleeAttack()
        {
            //DisableNavmeshAgentcomponent();  
            Components.HumanoidAiAudioPlayerComponent.PlayNonRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceMeleeAudioClips);
            float TimeToWait = AiMeleeAttack.MeleeAnimation[GetRandomMeleeAttack].MeleeAnimationClip.length;
            float SpeedOfMeleeAttackAnimation = 1f;
            if (GetRandomMeleeAttack == 0)
            {
                SpeedOfMeleeAttackAnimation = Speeds.AnimationSpeeds.MeleeAttack1AnimationSpeed;
            }
            else if (GetRandomMeleeAttack == 1)
            {
                SpeedOfMeleeAttackAnimation = Speeds.AnimationSpeeds.MeleeAttack2AnimationSpeed;
            }
            else if (GetRandomMeleeAttack == 2)
            {
                SpeedOfMeleeAttackAnimation = Speeds.AnimationSpeeds.MeleeAttack3AnimationSpeed;
            }
            float ActualAttackTime = TimeToWait / SpeedOfMeleeAttackAnimation;
            yield return new WaitForSeconds(ActualAttackTime);
            if(AgentRole != Role.Zombie)
            {
                SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                SetAnimationForUpperBody("StandShootPosture");
            } 
            for (int x = 0; x < AiMeleeAttack.MeleeAnimation[GetRandomMeleeAttack].AiMeleeDamageScripts.Length; x++)
            {
                if (AiMeleeAttack.MeleeAnimation[GetRandomMeleeAttack] != null)
                {
                    AiMeleeAttack.MeleeAnimation[GetRandomMeleeAttack].AiMeleeDamageScripts[x].gameObject.SetActive(false);
                }
            }
            yield return new WaitForSeconds(0.1f);
            if (AiMeleeAttack.EnableTimeBetweenMeleeAttacks == false)
            {
                IsWaitingForNextMeleeAttack = false;
                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                {
                    Vector3 DisWithEnemy = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - this.transform.position;
                    float RandomDistanceForMelee;

                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.tag == "Player")
                    {
                        RandomDistanceForMelee = RandomDistanceWithPlayerToAttack;
                    }
                    else
                    {
                        RandomDistanceForMelee = RandomDistanceToEnemyAiAgentToAttack;
                    }

                    if (DisWithEnemy.magnitude <= RandomDistanceForMelee)
                    {
                        MeleeAttack();
                    }
                    else
                    {
                        if (Components.HumanoidFiringBehaviourComponent != null)
                        {
                            Components.HumanoidFiringBehaviourComponent.StopShootDueToMelee = false;
                        }
                        StopMeleeAttack = false;
                        ShouldPlayMeleeAnimation = false;
                    }
                }
                else
                {
                    if (Components.HumanoidFiringBehaviourComponent != null)
                    {
                        Components.HumanoidFiringBehaviourComponent.StopShootDueToMelee = false;
                    }
                    StopMeleeAttack = false;
                    ShouldPlayMeleeAnimation = false;
                }
            }
            else
            {
                if (Components.HumanoidFiringBehaviourComponent != null)
                {
                    Components.HumanoidFiringBehaviourComponent.StopShootDueToMelee = false;
                }
                StopMeleeAttack = false;
                ShouldPlayMeleeAnimation = false;
                float RandomiseMeleeAttack = Random.Range(AiMeleeAttack.MinTimeIntervalBetweenMeleeAttack, AiMeleeAttack.MaxTimeIntervalBetweenMeleeAttack);
                yield return new WaitForSeconds(RandomiseMeleeAttack);
                IsWaitingForNextMeleeAttack = false;
            }


        }
        public void Melee()
        {
            if (CombatStateBehaviours.EnableMeleeAttack == true)
            {
                if (FindEnemiesScript.enemy.Count >= 1)
                {
                    if(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != transform)
                    {
                        if (StopMeleeAttack == false && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                        {
                          //  Debug.Log(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position);
                            Vector3 DisWithEnemy = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - this.transform.position;
                            float RandomDistanceForMelee;

                            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.tag == "Player")
                            {
                                RandomDistanceForMelee = RandomDistanceWithPlayerToAttack;
                            }
                            else
                            {
                                RandomDistanceForMelee = RandomDistanceToEnemyAiAgentToAttack;
                            }

                            if (DisWithEnemy.magnitude <= RandomDistanceForMelee)
                            {
                                MeleeAttack();
                            }
                        }


                        if (StopMeleeAttack == true)
                        {
                            StopSpineRotation = true;
                            enableIkupperbodyRotations(ref ActivateNoIk);
                            LookingAtEnemy();
                        }
                    }
                  
                }
            }

        }
        public void MeleeAttack()
        {
            if (IsWaitingForNextMeleeAttack == false)
            {
                StopMeleeAttack = true;
                if (Components.HumanoidFiringBehaviourComponent != null)
                {
                    Components.HumanoidFiringBehaviourComponent.StopShootDueToMelee = true;
                }
                ShouldPlayMeleeAnimation = false;
                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                {
                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<CoreAiBehaviour>() != null)
                    {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<CoreAiBehaviour>().HealthScript.IsDied == false)
                        {
                            ShouldPlayMeleeAnimation = true;
                        }
                    }

                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<PlayerHealth>() != null)
                    {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<PlayerHealth>().PlayerHealthbar.Curvalue > 0f)
                        {
                            ShouldPlayMeleeAnimation = true;
                        }
                    }
                } 

                if (ShouldPlayMeleeAnimation == true)
                {
                    
                    GetRandomMeleeAttack = Random.Range(0, AiMeleeAttack.MeleeAnimation.Count);
                    if (AiMeleeAttack.MeleeAnimation[GetRandomMeleeAttack].MeleeAttack.ToString() == PreviousMeleeAnimationPlaying)
                    {
                        SetAnimationForFullBody("null");
                        SetAnimationForUpperBody("null");
                        SetAnimationForFullZombieBody("null");
                        //SetAnimationForUpperBody("null");
                        anim.Play(PreviousMeleeAnimationPlaying, 1, 0f);
                        anim.Play(PreviousMeleeAnimationPlaying, 2, 0f);
                        StartCoroutine(DelayDamageScriptsActivation());
                    }
                    else
                    {
                        SetAnimationForFullBody(AiMeleeAttack.MeleeAnimation[GetRandomMeleeAttack].MeleeAttack.ToString());
                        SetAnimationForUpperBody(AiMeleeAttack.MeleeAnimation[GetRandomMeleeAttack].MeleeAttack.ToString());
                        SetAnimationForFullZombieBody(AiMeleeAttack.MeleeAnimation[GetRandomMeleeAttack].MeleeAttack.ToString());

                        StartCoroutine(DelayDamageScriptsActivation());
                        PreviousMeleeAnimationPlaying = AiMeleeAttack.MeleeAnimation[GetRandomMeleeAttack].MeleeAttack.ToString();
                    }
                    
                }
                IsWaitingForNextMeleeAttack = true;
                StartCoroutine(CoroForMeleeAttack());
            }
        }
        IEnumerator DelayDamageScriptsActivation()
        {
            yield return new WaitForSeconds(AiMeleeAttack.MeleeAnimation[GetRandomMeleeAttack].MeleeDamageScriptsActivationDelay);
            for (int x = 0; x < AiMeleeAttack.MeleeAnimation[GetRandomMeleeAttack].AiMeleeDamageScripts.Length; x++)
            {
                if (AiMeleeAttack.MeleeAnimation[GetRandomMeleeAttack] != null)
                {
                    if (AiMeleeAttack.MeleeAnimation[GetRandomMeleeAttack].AiMeleeDamageScripts[x] != null)
                    {
                        AiMeleeAttack.MeleeAnimation[GetRandomMeleeAttack].AiMeleeDamageScripts[x].gameObject.SetActive(true);
                    }
                }
            }
        }
        IEnumerator DelayNonCombatForAlertSoundTriggers()
        {
            yield return new WaitForSeconds(0.2f);
            WasInCombatLastTime = false;
        }
        public void AiFunctioning()
        {
            if (CombatStarted == false)
            {
                if (IsNoncombatstatebegins == false)
                {
                    ResetToNormalState = false;
                    IsLeaderMoving = false;
                    if(AiHearing.ShareSoundCoordinatesComponent != null)
                    {
                        AiHearing.ShareSoundCoordinatesComponent.gameObject.SetActive(false);
                    }
                    StartDelaySoundsAndEmergencyAlertTriggersTimer = false;
                    IsGoingToPreviousPoint = false;
                    //  StrafeOriginPoint = false;
                    Iscombatstatebegins = false;
                    IsNoncombatstatebegins = true;
                    StartCoroutine(DelayNonCombatForAlertSoundTriggers());
                 //   Components.NavMeshAgentComponent.enabled = true;
                    if(NavMeshObstacleComponent != null)
                    {
                        NavMeshObstacleComponent.enabled = false; 
                    }

                    if (VisibilityCheck != null)
                    {
                        VisibilityCheck.ResetVisibilityWithEnemyWhenInNonCombat();
                    }

                    //Added so after finishing all the agents the leader agent can get to the non combat behaviour and deregister cover points , firepoints and emergency points otherwise the agent will not be able to take new cover in new location.
                    if (CombatStateBehaviours.UseFiringPoints == true)
                    {
                        DeregisterWaypoint();
                    }
                    if (CombatStateBehaviours.TakeCovers == true)
                    {
                        CheckTime = false;
                        MoveTowardsOpenFirePoint = false;
                        ReachnewCoverpoint = false;
                        DeregisterCover();
                    }
                    if (NonCombatBehaviours.EnableEmergencyAlerts == true)
                    {
                        DeregisterEmergencyCover();
                    }

                    if (Components.HumanoidFiringBehaviourComponent != null)
                    {
                        Components.HumanoidFiringBehaviourComponent.DoNotActivateSoundCoordinate = false;
                        if (Components.HumanoidFiringBehaviourComponent.Components.DeactivateOtherAgentsAlertingSoundsGameOject != null)
                        {
                            Components.HumanoidFiringBehaviourComponent.Components.DeactivateOtherAgentsAlertingSoundsGameOject.gameObject.SetActive(false);
                        }
                        Components.HumanoidFiringBehaviourComponent.StartActivateSoundCoroutine = false;
                    }
                }
            }
            else
            {

                if (Iscombatstatebegins == false)
                {
                    if(FindEnemiesScript != null)
                    {
                        FindEnemiesScript.AllVisibleEnemiesDuringCombat();
                    }
              
                    WasInCombatLastTime = true;
                    ResetToNormalState = false;
                    // Components.NavMeshAgentComponent.enabled = true;
                    if (NavMeshObstacleComponent != null)
                    {
                        NavMeshObstacleComponent.enabled = false;
                    }
                    //Added so after finishing all the agents the leader agent can get to the non combat behaviour and deregister cover points , firepoints and emergency points otherwise the agent will not be able to take new cover in new location.
                    DeregisterEmergencyNodes = false;
                    DeregisterCoverNodes = false;
                    Deregisterfirepoint = false;

                    Components.HumanoidAiAudioPlayerComponent.PlayRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.RecurringSounds.EngagingTheEnemyAudioClips);
                    IsLeaderMoving = false;
                    OverriteRunningWhenFollowingCommander = false;
                    OverwriteSprinting = false;                     
                    ResetEmergencyBehaviourVariables();
                    IsGoingToPreviousPoint = false;
                  //  StrafeOriginPoint = false;
                    IsNoncombatstatebegins = false;
                    Iscombatstatebegins = true;
                }
            }

            if (VisibilityCheck != null)
            {
                if (NewCoverNode != null)
                {
                    if (NewCoverNode.DistanceCleared == true)
                    {
                        if (NewCoverNode.StandingCover == true && MoveTowardsOpenFirePoint == false)
                        {
                            VisibilityCheck.PauseRaycastCounts = true;
                        }
                        else if (NewCoverNode.CrouchCover == true && MoveTowardsOpenFirePoint == false)
                        {
                            VisibilityCheck.PauseRaycastCounts = true;
                        }
                        else
                        {
                            VisibilityCheck.PauseRaycastCounts = false;
                        }
                    }
                    else
                    {
                        VisibilityCheck.PauseRaycastCounts = false;
                    }
                }
                else
                {
                    VisibilityCheck.PauseRaycastCounts = false;
                }
            }


            //if (SmoothAnimatorWeightChange == true)
            //{
            //    SmoothAnimatorWeightChanger(AnimLayer, AnimWeight);
            //}
            // Rotate and follow the game object
            //if (spawnedTextPrefab != null)
            //{
            //   spawnedTextPrefab.transform.position = transform.position + DebugInfo.DebugInfoTextUIOffset;
            //}

#if UNITY_EDITOR
            // Rotate towards the SceneView camera

            if (DebugAgentState == true)
            {
                if (spawnedTextPrefab != null)
                {
                    SceneView sceneView = SceneView.lastActiveSceneView;
                    if (sceneView != null)
                    {
                        spawnedTextPrefab.transform.rotation = Quaternion.LookRotation(sceneView.camera.transform.forward, sceneView.camera.transform.up);
                    }
                    spawnedTextPrefab.transform.position = transform.position + DebugInfo.DebugInfoTextUIOffset;

                    // This line of code should not be used as when the Ai does not have any navmesh path or even have path but not true this could create a bug in unity
                    // for example let say Ai is searching for a path its true but not reachable or its outside of boundaries which could result in a bug. you spawn the Ai using spawners and it will result in bug if you uncomment it
                    //if (Components.NavMeshAgentComponent.hasPath == true && CombatStarted == true)
                    //{
                    //    NavMeshPath navMeshPath = new NavMeshPath();
                    //    if (NavMesh.CalculatePath(transform.position, pathfinder.closestPoint, NavMesh.AllAreas, navMeshPath))
                    //    {
                    //        if (navMeshPath.status == NavMeshPathStatus.PathComplete)
                    //        {
                    //            spawnedTextPrefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "NavMesh path is complete";
                    //        }
                    //        else
                    //        {
                    //            spawnedTextPrefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "NavMesh path is partial";
                    //        }
                    //    }
                    //    else
                    //    {
                    //        spawnedTextPrefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "NavMesh path is invalid";
                    //    }
                    //}
                }
            }
#endif


            if (HealthScript != null)
            {
                if (HealthScript.EnableHealthBar == true && HealthScript.SpawnHealthBar == true)
                {
                    Quaternion targetRotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
                    HealthScript.SpawnedHealthBarUI.rotation = targetRotation;
                    if (IsCrouched == true)
                    {
                        HealthScript.SpawnedHealthBarUI.position = transform.position + HealthScript.CrouchHealthBarUIOffset;
                    }
                    else
                    {
                        HealthScript.SpawnedHealthBarUI.position = transform.position + HealthScript.StandHealthBarUIOffset;
                    }
                }


                if (FindEnemiesScript.FindedEnemies == true)
                {
                    if (FindEnemiesScript.enemy != null)
                    {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.transform != this.transform)
                        {
                            CheckFieldofView();

                            if (Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, this.transform.position) < FindEnemiesScript.DetectionRadius && HealthScript.IsDied == false && VisibilityCheck.CanSeeTarget(FindEnemiesScript.OriginalFov, FindEnemiesScript.DetectionRadius, Components.HumanoidFiringBehaviourComponent) && HealthScript.CompleteFirstHitAnimation == false)
                            {
                                //var lookPos = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - Components.AiDirectionPointer.transform.position;
                                //lookPos.y = 0;
                                //var rotation = Quaternion.LookRotation(lookPos);
                                //Components.AiDirectionPointer.transform.rotation = Quaternion.Slerp(Components.AiDirectionPointer.transform.rotation, rotation, Time.deltaTime * 1000f);

                                //if (Components.DeadBodyScanObject != null)
                                //{
                                //    Components.DeadBodyScanObject.gameObject.SetActive(false);
                                //}
                                SearchingForSound = false;
                                IsNearDeadBody = false; // Added on 21st Dec 2024 because if you don't Add it than if AI want to shoot player because of friendly fire you need to make sure to turn off the emergency state on it.
                                                        // AI agent blood splash impact effect is turning on and making the AI agent to go to emergency state even though the player did friendly fire instead AI should turn it off and shoot player.

                                CombatStarted = true;
                                //GiveOneCheck = false;
                                //                            Debug.Log("Searching For Sound 1" + transform.name);
                                SearchingForSound = false;
                                GenerateSoundCoorinate = false;
                                if (AiHearing.ShareSoundCoordinatesComponent != null)
                                {
                                    AiHearing.ShareSoundCoordinatesComponent.gameObject.SetActive(false);
                                }
                            }
                        }
                    }
                   
                }

                if (IsBodyguard == true && CombatStarted == false && VisibilityCheck.ConnectionLost == false && HealthScript.IsDied == false && WasInCombatStateBefore == false)
                {
                    PerformGuardDuty();

                    if (NonCombatBehaviours.EnableSoundAlerts == true)
                    {
                        //if (FindEnemiesScript.FindedEnemies == true)
                        //{
                        //    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.transform != this.transform)
                        //    {
                        if (CombatStarted == false && VisibilityCheck.ConnectionLost == false && SearchingForSound == true
                            && IsNearDeadBody == false)
                        {
                            GoToHearingSound();
                        }
                        //}
                        //}
                    }
                }
                else if (HealthScript.IsDied == false)
                {
                    //if (FollowCommanderValues.ForceCombatStateOnFollowersInCaseCommanderGetInto == true && IsBodyguard == true)
                    //{
                    //    if (CombatStarted == true)
                    //    {
                    //        FindEnemiesScript.EnableFieldOfView = IsFieldOfViewForDetectingEnemyEnabled;
                    //    }
                    //}

                    if (NonCombatBehaviours.EnableSoundAlerts == true)
                    {
                        //if (FindEnemiesScript.FindedEnemies == true)
                        //{
                        //    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.transform != this.transform)
                        //    {
                        if (CombatStarted == false && VisibilityCheck.ConnectionLost == false && SearchingForSound == true
                            && IsNearDeadBody == false)
                        {
                            GoToHearingSound();
                        }
                        //}
                        //}
                    }


                    if (StartVariations == false && ShouldStartFunctionality == true)
                    {
                        if (CombatStateBehaviours.TakeCovers == true)
                        {
                            if (CombatStateBehaviours.EnableMeleeAttack == true)
                            {
                                Melee();
                                if (StopMeleeAttack == false)
                                {

                                    CoverShooter();
                                }
                            }
                            else
                            {
                                CoverShooter();
                            }
                        }
                        else if (CombatStateBehaviours.UseFiringPoints == true)
                        {
                            if (CombatStateBehaviours.EnableMeleeAttack == true)
                            {
                                Melee();
                                if (StopMeleeAttack == false)
                                {
                                    IndependentFiringPointShooter();
                                }
                            }
                            else
                            {
                                IndependentFiringPointShooter();
                            }
                        }
                        else if (CombatStateBehaviours.EnableAiCharging == true)
                        {
                            if (CombatStateBehaviours.EnableMeleeAttack == true)
                            {
                                Melee();
                                if (StopMeleeAttack == false)
                                {
                                    if(AgentRole != Role.Zombie)
                                    {
                                        ChargeShooter();
                                    }
                                    else
                                    {
                                        ZombieChargingBehaviour();
                                    }
                                }
                                else
                                {
                                    if (AgentRole == Role.Zombie)
                                    {
                                        if (Components.NavMeshAgentComponent.enabled == true)
                                        {
                                            Components.NavMeshAgentComponent.speed = 0f;
                                            Components.NavMeshAgentComponent.isStopped = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (AgentRole != Role.Zombie)
                                {
                                    ChargeShooter();
                                }
                                else
                                {
                                    ZombieChargingBehaviour();
                                }
                            }

                        }
                        else if (CombatStateBehaviours.EnableAiCharging == false && CombatStateBehaviours.TakeCovers == false && CombatStateBehaviours.UseFiringPoints == false)
                        {
                            ChoosenAiTypeIsIdle = true;

                            if (CombatStateBehaviours.EnableMeleeAttack == true)
                            {
                                Melee();
                                if (StopMeleeAttack == false)
                                {
                                    IdleFire();
                                }
                            }
                            else
                            {
                                IdleFire();
                            }
                        }
                    }
                    else if (ShouldStartFunctionality == true && StartVariations == true)
                    {
                        TimerForNewStateToStart += Time.deltaTime;

                        if (TimerForNewStateToStart >= StateSwitchTimer)
                        {
                            if (IsTaskOver == true)
                            {
                                StartIterating = true;
                                TimerForNewStateToStart = 0f;
                                GetNewState = false;
                                //OvveruleStandShootPostureAnimation = false;
                                // DeregisterNodes = false;
                                // Deregisterfirepoint = false;
                                //Reachnewpoints = false;                        
                                // DeregisterCover();
                                // DeregisterWaypoint();
                                CheckTime = false;
                                //DoHavePreviousFiringPoint = false;
                                //SaveResetedWaypointRandomisation = 0f;
                                IsAnyTaskCurrentlyRunning = false;
                                IsTaskOver = false;
                            }

                        }
                        else
                        {
                            IsTaskOver = false;
                        }

                        if (GetNewState == false)
                        {
                            StartIterating = true; // added on 29 jan 2024
                            //StartCoroutine(GetRandomStateBehaviour());
                            RandomiseStateProbability = Random.Range(1, TotalProbability);
                            GetNewState = true;
                        }


                        if (RandomiseStateProbability <= RangeForElement0[1] && RandomiseStateProbability >= RangeForElement0[0])
                        {
                            if (MyChargingProbability <= RangeForElement0[1] && MyChargingProbability >= RangeForElement0[0] && CombatStateBehaviours.EnableAiCharging == true)
                            {
                                if (CombatStateBehaviours.EnableMeleeAttack == true)
                                {
                                    Melee();
                                    if (StopMeleeAttack == false)
                                    {
                                        if (AgentRole != Role.Zombie)
                                        {
                                            InitChargeBehaviour();
                                            ChargeShooter();
                                        }
                                        else
                                        {
                                            ZombieChargingBehaviour();
                                        }
                                    }
                                }
                                else
                                {
                                    if (AgentRole != Role.Zombie)
                                    {
                                        InitChargeBehaviour();
                                        ChargeShooter();
                                    }
                                    else
                                    {
                                        ZombieChargingBehaviour();
                                    }
                                }

                            }
                            else if (MyCoverProbability <= RangeForElement0[1] && MyCoverProbability >= RangeForElement0[0] && CombatStateBehaviours.TakeCovers == true)
                            {
                                if (CombatStateBehaviours.EnableMeleeAttack == true)
                                {
                                    Melee();
                                    if (StopMeleeAttack == false)
                                    {
                                        InitCoverBehaviour();
                                        CoverShooter();
                                    }
                                }
                                else
                                {
                                    InitCoverBehaviour();
                                    CoverShooter();

                                }

                            }
                            else if (MyWaypointsProbability <= RangeForElement0[1] && MyWaypointsProbability >= RangeForElement0[0] && CombatStateBehaviours.UseFiringPoints == true)
                            {
                                if (CombatStateBehaviours.EnableMeleeAttack == true)
                                {
                                    Melee();
                                    if (StopMeleeAttack == false)
                                    {
                                        InitFiringBehaviour();
                                        IndependentFiringPointShooter();
                                    }
                                }
                                else
                                {
                                    InitFiringBehaviour();
                                    IndependentFiringPointShooter();
                                }

                            }
                        }
                        else if (RandomiseStateProbability <= RangeForElement1[1] && RandomiseStateProbability >= RangeForElement1[0])
                        {
                            if (MyChargingProbability <= RangeForElement1[1] && MyChargingProbability >= RangeForElement1[0] && CombatStateBehaviours.EnableAiCharging == true)
                            {
                                if (CombatStateBehaviours.EnableMeleeAttack == true)
                                {
                                    Melee();
                                    if (StopMeleeAttack == false)
                                    {
                                        if (AgentRole != Role.Zombie)
                                        {
                                            InitChargeBehaviour();
                                            ChargeShooter();
                                        }
                                        else
                                        {
                                            ZombieChargingBehaviour();
                                        }
                                    }
                                }
                                else
                                {
                                    if (AgentRole != Role.Zombie)
                                    {
                                        InitChargeBehaviour();
                                        ChargeShooter();
                                    }
                                    else
                                    {
                                        ZombieChargingBehaviour();
                                    }
                                }
                            }
                            else if (MyCoverProbability <= RangeForElement1[1] && MyCoverProbability >= RangeForElement1[0] && CombatStateBehaviours.TakeCovers == true)
                            {
                                if (CombatStateBehaviours.EnableMeleeAttack == true)
                                {
                                    Melee();
                                    if (StopMeleeAttack == false)
                                    {
                                        InitCoverBehaviour();
                                        CoverShooter();
                                    }
                                }
                                else
                                {
                                    InitCoverBehaviour();
                                    CoverShooter();

                                }
                            }
                            else if (MyWaypointsProbability <= RangeForElement1[1] && MyWaypointsProbability >= RangeForElement1[0] && CombatStateBehaviours.UseFiringPoints == true)
                            {
                                if (CombatStateBehaviours.EnableMeleeAttack == true)
                                {
                                    Melee();
                                    if (StopMeleeAttack == false)
                                    {
                                        InitFiringBehaviour();
                                        IndependentFiringPointShooter();
                                    }
                                }
                                else
                                {
                                    InitFiringBehaviour();
                                    IndependentFiringPointShooter();
                                }
                            }
                        }
                        else if (RandomiseStateProbability <= RangeForElement2[1] && RandomiseStateProbability >= RangeForElement2[0])
                        {
                            if (MyChargingProbability <= RangeForElement2[1] && MyChargingProbability >= RangeForElement2[0] && CombatStateBehaviours.EnableAiCharging == true)
                            {
                                if (CombatStateBehaviours.EnableMeleeAttack == true)
                                {
                                    Melee();
                                    if (StopMeleeAttack == false)
                                    {
                                        if (AgentRole != Role.Zombie)
                                        {
                                            InitChargeBehaviour();
                                            ChargeShooter();
                                        }
                                        else
                                        {
                                            ZombieChargingBehaviour();
                                        }
                                    }
                                }
                                else
                                {
                                    if (AgentRole != Role.Zombie)
                                    {
                                        InitChargeBehaviour();
                                        ChargeShooter();
                                    }
                                    else
                                    {
                                        ZombieChargingBehaviour();
                                    }
                                }
                            }
                            else if (MyCoverProbability <= RangeForElement2[1] && MyCoverProbability >= RangeForElement2[0] && CombatStateBehaviours.TakeCovers == true)
                            {
                                if (CombatStateBehaviours.EnableMeleeAttack == true)
                                {
                                    Melee();
                                    if (StopMeleeAttack == false)
                                    {
                                        InitCoverBehaviour();
                                        CoverShooter();
                                    }
                                }
                                else
                                {
                                    InitCoverBehaviour();
                                    CoverShooter();

                                }
                            }
                            else if (MyWaypointsProbability <= RangeForElement2[1] && MyWaypointsProbability >= RangeForElement2[0] && CombatStateBehaviours.UseFiringPoints == true)
                            {
                                if (CombatStateBehaviours.EnableMeleeAttack == true)
                                {
                                    Melee();
                                    if (StopMeleeAttack == false)
                                    {
                                        InitFiringBehaviour();
                                        IndependentFiringPointShooter();
                                    }
                                }
                                else
                                {
                                    InitFiringBehaviour();
                                    IndependentFiringPointShooter();
                                }
                            }
                        }
                        //else
                        //{
                        //    IdleFire();
                        //}
                    }

                    //if(CombatStarted == false)
                    //{
                    //    Components.NavMeshAgentComponent.updateRotation = true; // Needed for Patrolling and wandering rotation to look at patrol points 
                    //}
                    //else
                    //{
                    //    Components.NavMeshAgentComponent.updateRotation = false;
                    //}

                    //if (RandomiseStateProbability <= GetProbability[0])
                    //{
                    //    if(MyChargingProbability <= RandomiseStateProbability && CombatStateBehaviours.EnableAiCharging == true)
                    //    {
                    //        Debug.Log("CHARGE");
                    //    }
                    //    else if (MyCoverProbability <= RandomiseStateProbability && CombatStateBehaviours.EnableTakeCovers == true)
                    //    {
                    //        Debug.Log("COVER");
                    //    }
                    //    else if (MyWaypointsProbability <= RandomiseStateProbability && CombatStateBehaviours.UseWaypoints == true)
                    //    {
                    //        Debug.Log("WAYPOINT");
                    //    }
                    //}
                    //else if(RandomiseStateProbability >= GetProbability[0] && RandomiseStateProbability <= GetProbability[1])
                    //{
                    //    if (MyChargingProbability <= RandomiseStateProbability && MyChargingProbability >= GetProbability[0] && CombatStateBehaviours.EnableAiCharging == true)
                    //    {
                    //        Debug.Log("CHARGE");
                    //    }
                    //    else if (MyCoverProbability <= RandomiseStateProbability && MyCoverProbability >= GetProbability[0] && CombatStateBehaviours.EnableTakeCovers == true)
                    //    {
                    //        Debug.Log("COVER");
                    //    }
                    //    else if (MyWaypointsProbability <= RandomiseStateProbability && MyWaypointsProbability >= GetProbability[0] && CombatStateBehaviours.UseWaypoints == true)
                    //    {
                    //        Debug.Log("WAYPOINT");
                    //    }
                    //}
                    //else if (RandomiseStateProbability >= GetProbability[1] && RandomiseStateProbability <= GetProbability[2])
                    //{
                    //    if (MyChargingProbability >= RandomiseStateProbability && CombatStateBehaviours.EnableAiCharging == true)
                    //    {
                    //        Debug.Log("CHARGE");
                    //    }
                    //    else if (MyCoverProbability >= RandomiseStateProbability && CombatStateBehaviours.EnableTakeCovers == true)
                    //    {
                    //        Debug.Log("COVER");
                    //    }
                    //    else if (MyWaypointsProbability >= RandomiseStateProbability && CombatStateBehaviours.UseWaypoints == true)
                    //    {
                    //        Debug.Log("WAYPOINT");
                    //    }
                    //}




                    //if (CombatStateBehaviours.EnableTakeCovers == true)
                    //{
                    //    CoverShooter();
                    //}
                    //else if (CombatStateBehaviours.UseWaypoints == true)
                    //{
                    //    WaypointShooter();
                    //}              
                    //else if(CombatStateBehaviours.EnableAiCharging == true)
                    //{
                    //    ChargeShooter();
                    //}
                    //else if (CombatStateBehaviours.EnableAiCharging == false && CombatStateBehaviours.EnableTakeCovers == false && CombatStateBehaviours.UseWaypoints == false)
                    //{
                    //    ChoosenAiTypeIsIdle = true;
                    //    IdleFire();
                    //}
                }
                else if (HealthScript.IsDied == true)
                {
#if UNITY_EDITOR
                    if (DebugAgentState == true)
                    {
                        if (spawnedTextPrefab != null)
                        {
                            Destroy(spawnedTextPrefab.gameObject);
                        }
                    }
#endif

                    if (CombatStateBehaviours.UseFiringPoints == true)
                    {
                        DeregisterWaypoint();
                    }
                    if (CombatStateBehaviours.TakeCovers == true)
                    {
                        DeregisterCover();
                    }
                    if (NonCombatBehaviours.EnableEmergencyAlerts == true)
                    {
                        DeregisterEmergencyCover();
                    }


                    // Disable NavMeshObstacle and NavMeshAgent
                    // we need to disable both because if agent die while being behind the cover and if another Agent want to take the same cover it will intruppt other Ai agents taking the same cover that previously Ai agent was taking(but now he is dead).
                    // because one of the component is activated and to avoid it both need to be disabled.
                    // Also another reason to keep both disabled is because when agent die it will not snap to the area of the NavMesh in case NavMesh Obstacle is activated at that moment so we make sure to deactivate both.for example
                    // if stationed shooting is enabled so to avoid it both need to be disabled.
                     NavMeshObstacleComponent.enabled = false;
                     Components.NavMeshAgentComponent.enabled = false;

                    if (Components.NavMeshAgentComponent.enabled == true)
                    {
                        Components.NavMeshAgentComponent.isStopped = true;
                        Components.NavMeshAgentComponent.speed = 0;
                    }
                }

            }

            if (T != null && FindEnemiesScript != null)
            {
                if (FindEnemiesScript.FindedEnemies == true)
                {
                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                    {
                        T.DebugOtherTargetName = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.name;
                    }
                }
            }

            if(VisibilityCheck != null)
            {
                if (VisibilityCheck.ConnectionLost == false)
                {
                    // It could be true at any time so even though the stare timer is running behind we can always make sure to reset its variables.
                    StareTimeBeginForMovableAi = false;
                    CanMoveTowardsPursueCoordinate = false;
                    StareTimeCompletedForMovableAi = false;
                   
                    StayingNearPursuingCoordinate = false;
                }
            }
         

           

            if (CombatStarted == false)
            {
                ReInitializeEverything = false;

                if (Components.AiNonCombatChatterComponent != null)
                {
                    if(Components.AiNonCombatChatterComponent.gameObject.activeInHierarchy == true)
                    {
                        if (IsNearDeadBody == true || SearchingForSound == true)
                        {
                            Components.AiNonCombatChatterComponent.ImmediatelyStopConversationSounds();
                            Components.AiNonCombatChatterComponent.gameObject.SetActive(false);
                            ActivateConversationSoundsComponent = false;
                        }
                        else
                        {
                            if (ActivateConversationSoundsComponent == false && Components.AiNonCombatChatterComponent.gameObject.activeInHierarchy == true)
                            {
                                PlayDefaultBehaviourSoundsNow = false;
                                Components.AiNonCombatChatterComponent.gameObject.SetActive(true);
                                ActivateConversationSoundsComponent = true;
                            }

                        }
                    }
                    else
                    {
                        PlayDefaultBehaviourSoundsNow = true;
                    }
                   
                }
                else
                {
                    PlayDefaultBehaviourSoundsNow = true;
                }
                //if (GiveOneCheck == false)
                //{
                //    if (Components.DeadBodyScanObject != null)
                //    {
                //        Components.DeadBodyScanObject.gameObject.SetActive(true);
                //    }
                //    GiveOneCheck = true;
                //}

               
            }
            else
            {
                if (Components.AiNonCombatChatterComponent != null)
                {
                    if (Components.AiNonCombatChatterComponent.gameObject.activeInHierarchy == true)
                    {
                        if (ActivateConversationSoundsComponent == true && Components.AiNonCombatChatterComponent.gameObject.activeInHierarchy == true)
                        {
                            Components.AiNonCombatChatterComponent.ImmediatelyStopConversationSounds();
                            Components.AiNonCombatChatterComponent.gameObject.SetActive(false);
                            ActivateConversationSoundsComponent = false;
                        }
                    }
                }
        
                if (ReInitializeEverything == false)
                {
                    if (NonCombatBehaviours.EnableEmergencyAlerts == true)
                    {
                        AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.gameObject.SetActive(true);
                    }
                    IsNearDeadBody = false;
                    IsInEmergencyState = false;
                    ImmediateStartScanForTheFirstTime = false;

                    if (PreviousEmergencyCoverNode != null)
                    {
                        if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                        {
                            PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().DeactivateOccupiedText();
                        }
                    }
                    ResetEmergencyBehaviourVariables();
                    if (grenadeThrowerScript != null)
                    {
                        if (CombatStateBehaviours.UseGrenades == true)
                        {
                            grenadeThrowerScript.StartingTheThrowDelay();
                        }
                    }
                    ReInitializeEverything = true;
                }


            }


            //if(Detections.EnableFieldOfView == true && HealthScript.IsDied == false)
            //{
            //    AdjustingFOV(CombatStarted, RotateSpine, IsCrouched, SearchingForSound);
            //}

            if (NonCombatBehaviours.EnableEmergencyAlerts == true)
            {
                if (AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.gameObject.activeInHierarchy == true)
                {
                    if (AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.GetComponent<SphereCollider>() != null)
                    {
                        //  SphereCollider b = AiDeadBodyAlerts.EmergencyAlert.EmergencyCoverFinderGameObject.GetComponent<SphereCollider>();
                        //  b.radius = RangeToFindEmergencyCover / 2;
                        AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.GetComponent<SphereCollider>().radius = RangeToFindEmergencyCover;
                    }

                    GetAccurateEmergencyCoverRange = RangeToFindEmergencyCover; // / 2;
                }

                Vector3 CheckforNewCover = SavedCurrentPositionForEmergencyCoverFinding - transform.position;
                if (CheckforNewCover.sqrMagnitude >= GetAccurateEmergencyCoverRange * GetAccurateEmergencyCoverRange)
                {
                    SavedCurrentPositionForEmergencyCoverFinding = transform.position;
                    AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.gameObject.SetActive(true);
                }
            }


        }
        int GetFirstProbability()
        {
            if (ChargingProbability == GetProbability[0] && IsChargingProbabilityDone == false && ChargingProbability >= 1)
            {
                MyChargingProbability = TotalProbability * ChargingProbability / TotalProbability;
                MyChargingProbability = Mathf.RoundToInt(MyChargingProbability);
                RangeForElement0[0] = 0;
                RangeForElement0[1] = MyChargingProbability;
                GetProbability[0] = MyChargingProbability;
                IsChargingProbabilityDone = true;
                return MyChargingProbability;
            }
            else if (CoversUsageProbability == GetProbability[0] && IsCoverProbabilityDone == false && CoversUsageProbability >= 1)
            {
                MyCoverProbability = TotalProbability * CoversUsageProbability / TotalProbability;
                MyCoverProbability = Mathf.RoundToInt(MyCoverProbability);
                RangeForElement0[0] = 0;
                RangeForElement0[1] = MyCoverProbability;
                GetProbability[0] = MyCoverProbability;
                IsCoverProbabilityDone = true;
                return MyCoverProbability;
            }
            else if (FiringPointsUsageProbability == GetProbability[0] && IsWaypointsProbabilityDone == false && FiringPointsUsageProbability >= 1)
            {
                MyWaypointsProbability = TotalProbability * FiringPointsUsageProbability / TotalProbability;
                MyWaypointsProbability = Mathf.RoundToInt(MyWaypointsProbability);
                RangeForElement0[0] = 0;
                RangeForElement0[1] = MyWaypointsProbability;
                GetProbability[0] = MyWaypointsProbability;
                IsWaypointsProbabilityDone = true;
                return MyWaypointsProbability;
            }
            else
            {
                return 0;
            }
        }
        int GetSecondProbability()
        {
            if (ChargingProbability == GetProbability[1] && IsChargingProbabilityDone == false && ChargingProbability >= 1)
            {
                MyChargingProbability = TotalProbability * ChargingProbability / TotalProbability;
                MyChargingProbability = GetProbability[0] + MyChargingProbability;
                MyChargingProbability = Mathf.RoundToInt(MyChargingProbability);
                RangeForElement1[0] = GetProbability[0] + 1;
                RangeForElement1[1] = MyChargingProbability;
                GetProbability[1] = MyChargingProbability;
                IsChargingProbabilityDone = true;
                return MyChargingProbability;
            }
            else if (CoversUsageProbability == GetProbability[1] && IsCoverProbabilityDone == false && CoversUsageProbability >= 1)
            {
                MyCoverProbability = TotalProbability * CoversUsageProbability / TotalProbability;
                MyCoverProbability = GetProbability[0] + MyCoverProbability;
                MyCoverProbability = Mathf.RoundToInt(MyCoverProbability);
                RangeForElement1[0] = GetProbability[0] + 1;
                RangeForElement1[1] = MyCoverProbability;
                GetProbability[1] = MyCoverProbability;
                IsCoverProbabilityDone = true;
                return MyCoverProbability;
            }
            else if (FiringPointsUsageProbability == GetProbability[1] && IsWaypointsProbabilityDone == false && FiringPointsUsageProbability >= 1)
            {
                MyWaypointsProbability = TotalProbability * FiringPointsUsageProbability / TotalProbability;
                MyWaypointsProbability = GetProbability[0] + MyWaypointsProbability;
                MyWaypointsProbability = Mathf.RoundToInt(MyWaypointsProbability);
                RangeForElement1[0] = GetProbability[0] + 1;
                RangeForElement1[1] = MyWaypointsProbability;
                GetProbability[1] = MyWaypointsProbability;
                IsWaypointsProbabilityDone = true;
                return MyWaypointsProbability;
            }
            else
            {
                return 0;
            }
        }
        int GetThirdProbability()
        {
            if (ChargingProbability == GetProbability[2] && IsChargingProbabilityDone == false && ChargingProbability >= 1)
            {
                MyChargingProbability = TotalProbability * ChargingProbability / TotalProbability;
                MyChargingProbability = MyChargingProbability + GetProbability[1];
                MyChargingProbability = Mathf.RoundToInt(MyChargingProbability);
                RangeForElement2[0] = GetProbability[1] + 1;
                RangeForElement2[1] = TotalProbability;
                GetProbability[2] = MyChargingProbability;
                IsChargingProbabilityDone = true;
                return MyChargingProbability;
            }
            else if (CoversUsageProbability == GetProbability[2] && IsCoverProbabilityDone == false && CoversUsageProbability >= 1)
            {
                MyCoverProbability = TotalProbability * CoversUsageProbability / TotalProbability;
                MyCoverProbability = MyCoverProbability + GetProbability[1];
                MyCoverProbability = Mathf.RoundToInt(MyCoverProbability);
                RangeForElement2[0] = GetProbability[1] + 1;
                RangeForElement2[1] = TotalProbability;
                GetProbability[2] = MyCoverProbability;
                IsCoverProbabilityDone = true;
                return MyCoverProbability;
            }
            else if (FiringPointsUsageProbability == GetProbability[2] && IsWaypointsProbabilityDone == false && FiringPointsUsageProbability >= 1)
            {
                MyWaypointsProbability = TotalProbability * FiringPointsUsageProbability / TotalProbability;
                MyWaypointsProbability = MyWaypointsProbability + GetProbability[1];
                MyWaypointsProbability = Mathf.RoundToInt(MyWaypointsProbability);
                RangeForElement2[0] = GetProbability[1] + 1;
                RangeForElement2[1] = TotalProbability;
                GetProbability[2] = MyWaypointsProbability;
                IsWaypointsProbabilityDone = true;
                return MyWaypointsProbability;
            }
            else
            {
                return 0;
            }


        }
        //public void CheckForFireShot(Vector3 pos)
        //{
        //    Vector3 distance = pos - transform.position;
        //    distance.y = 0;
        //    float Getdistance = Mathf.FloorToInt(distance.magnitude);
        //    float approachtosound = Getdistance / 100f;
        //    float FinalApproach = approachtosound * AiHearing.ErrorSoundPercentage;
        //    RandomApproachToSound = FinalApproach;
        //    if (RandomApproachToSound == 0)
        //    {
        //        RandomApproachToSound = 1;
        //    }
        //    GoToHearingSound();
        //}
        public void NonCombatRotations()
        {
            if (HealthScript != null)
            {
                if (HealthScript.IsDied == false)
                {
                    if (RotateSpine != null)
                    {
                        if (ActivateRunningIk == true)
                        {
                            RotateSpine.NonCombatSpineBoneRotations(RotateSpine.ScanAimAdjustmentPoints.RunScanAimLevel);
                        }
                        else if (ActivateWalkAimIk == true)
                        {
                            RotateSpine.NonCombatSpineBoneRotations(RotateSpine.ScanAimAdjustmentPoints.WalkScanAimLevel);
                        }
                        else if (ActivateScanIk == true)
                        {
                            RotateSpine.NonCombatSpineBoneRotations(RotateSpine.ScanAimAdjustmentPoints.StationaryScanAimLevel);
                        }
                        else if (ActivateSprintingIk == true)
                        {
                            // RotateSpine.NonCombatSpineBoneRotations(RotateSpine.AimingPointsDuringNonCombatBehaviour.ScanAimPoint);
                          //  Debug.Log("SPRINT");
                        }
                        else
                        {
                            // If Spine has been stopped rotating that we can not make the spine to look into Idle Aim point as when the Ai agent takes the Stand hide cover the spine actually stop rotating due to which
                            // it makes the Ai agent spine rotating towards the Idle Aim point which makes the behaviour wierd.
                            // RotateSpine.NonCombatSpineBoneRotations(RotateSpine.AimingPointsDuringNonCombatBehaviour.IdleAimPoint);

                            RotateSpine.StoreSpineLastFramRotation();
                        }
                    }

                }
            }
        }
        public void SpineRotation() // Rotation of Spine
        {
            if (RotateSpine != null)
            {
                if (CombatStarted == true)
                {
                    if (StopSpineRotation == false)
                    {
                        if (RotateSpine != null)
                        {
                            enableIkupperbodyRotations(ref ActivateNoIk);
                            if(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                            {
                                RotateSpine.PartToRotate(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position);
                            }
                        }
                    }
                    else
                    {
                        NonCombatRotations();
                    }
                }
                else
                {
                    if (StopSpineRotation == false)
                    {
                        if (IsShootingDuringEmergencyState == true)
                        {
                            if (RotateSpine != null)
                            {
                                enableIkupperbodyRotations(ref ActivateNoIk);
                                RotateSpine.SpineRotationDuringEmergencyShoot(TargetOffsetedPosition);
                            }
                        }
                        else
                        {
                            NonCombatRotations();
                        }
                    }
                    else
                    {
                        NonCombatRotations();
                    }
                }
            }
        }
        public void IdleFire()
        {
#if UNITY_EDITOR
            if (spawnedTextPrefab != null)
            {
                spawnedTextPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "STATIONED AGENT";
                spawnedTextPrefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = transform.name;
            }
#endif
            if (FindEnemiesScript.FindedEnemies == true)
            {
                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null
                    && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.transform != this.transform && IsNearDeadBody == false)//FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform PREVIOUS CODE 
                {
                    //if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.gameObject.GetComponent<HumanoidAiHealth>() != null)
                    //{
                    //    if(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.gameObject.GetComponent<HumanoidAiHealth>().IsDied == true)
                    //    {
                    //        FindImmediateEnemy();
                    //    }
                    //}

                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<HumanoidAiHealth>() != null)
                    {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<HumanoidAiHealth>().IsDied == true)
                        {
                            FindImmediateEnemy();
                        }

                    }
                    else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.tag == "Player")
                    {
                        if (PlayerHealth.instance != null)
                        {
                            if (PlayerHealth.instance.IsDead == true)
                            {
                                FindImmediateEnemy();
                            }
                        }
                    }
                    else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<Turret>() != null)
                    {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<Turret>().IsDied == true)
                        {
                            FindImmediateEnemy();
                        }

                    }


                    CheckFieldofView();

                    if (Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, this.transform.position) < FindEnemiesScript.DetectionRadius
                        && IsEnemyLocked == true && VisibilityCheck.CanSeeTarget(FindEnemiesScript.OriginalFov, FindEnemiesScript.DetectionRadius, Components.HumanoidFiringBehaviourComponent)
                        && HealthScript.CompleteFirstHitAnimation == false && BotMovingAwayFromGrenade == false)
                    {
                        if(CombatStateBehaviours.EnablePostCombatScan == true)
                        {
                            ResetVariableForQuickScan();
                            WasInCombatStateBefore = true;
                        }
                       
                        CheckForLockedTargetInFOV();
                        //if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                        //{
                        Fire();

                        if (Components.HumanoidFiringBehaviourComponent != null)
                        {
                            if (Components.HumanoidFiringBehaviourComponent.isreloading)
                            {
                                OnlyUpperBodyReload();
                            }
                        }

                        //}
                        //else
                        //{
                        //    Reload();
                        //    DebugInfo.CurrentState = "RELOADING";
                        //}
                    }
                    else
                    {
                        if (BotMovingAwayFromGrenade == false)
                        {
                            if (HealthScript.CompleteFirstHitAnimation == false)
                            {
                                if (VisibilityCheck.ConnectionLost == true)
                                {
                                    if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableApproachingEnemyPursue
                                        || HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableStationedEnemyPursue)
                                    {
                                        ConnectionLostForMovingBots();
                                    }
                                    //else if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableStationedEnemyPursue)
                                    //{
                                    //    ConnectionLostForStationarybot();
                                    //}
                                    else if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.DoNotPursueEnemy)
                                    {
                                        FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
                                        VisibilityCheck.ConnectionLost = false;
                                        StaringAtLastEnemyLostPosition = false;
                                    }
                                }
                                else
                                {
                                    DefaultInvestigationBehaviour();
                                }
                            }
                        }
                        else
                        {
                            SprintAwayFromGrenade();
                        }


                    }
                }
                else
                {
                    //if (SearchingForSound == false)
                    //{

                    //    FindImmediateEnemy();
                    //    if (NonCombatBehaviours.DefaultBehaviour == InvestigationTypes.Patrolling)
                    //    {
                    //        PatrolingScript.WanderAndPatrol();
                    //    }
                    //    else if (NonCombatBehaviours.DefaultBehaviour == InvestigationTypes.Wandering)
                    //    {
                    //        WanderingScript.Wander();
                    //    }
                    //    else
                    //    {
                    //        SearchingState();
                    //    }
                    //}


                    //if (IsNearDeadBody == true && NonCombatBehaviours.EnableEmergencyAlerts == true && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != transform.root.transform)
                    //{
                    //    CompleteEmergencyStateBehaviour();
                    //}
                    //else
                    //{
                    //    if (SearchingForSound == false)
                    //    {
                    //        FindImmediateEnemy();
                    //        DefaultInvestigationBehaviour();
                    //    }
                    //}

                    if (IsNearDeadBody == true && NonCombatBehaviours.EnableEmergencyAlerts == true && IsInEmergencyState == true && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != transform.root.transform
                      && HealthScript.CompleteFirstHitAnimation == false)
                    {
                        CompleteEmergencyStateBehaviour();
                    }
                    else if (IsNearDeadBody == true && NonCombatBehaviours.EnableDeadBodyAlerts == true && IsInEmergencyState == false && IsEmergencyStateCurrentlyActive == false
                        && HealthScript.CompleteFirstHitAnimation == false)//FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != transform.root.transform &&
                    {
                        DeadBodyInvestigation();
                    }
                    else
                    {
                        if (SearchingForSound == false && HealthScript.CompleteFirstHitAnimation == false)
                        {
                            // This code is commented on 14th Jan 2025 because when in beginning there was a player and turret and AI agent . The AI agent removed its enemies which was Player and Turret and ignored them basically
                            // and did not updated its enemy and did not shoot even.
                          //  FindImmediateEnemy(); 
                            DefaultInvestigationBehaviour();
                        }
                    }
                }
            }
            else
            {

                DefaultInvestigationBehaviour();
            }

        }
        //public void ConnectionLostForStationarybot()
        //{
        //    CheckTime = false;
        //    // CombatStarted = false;
        //    Components.HumanoidAiAudioPlayerComponent.PlaySoundClipsForSingleAudio(Components.HumanoidAiAudioPlayerComponent.AiSinglePlaybackVoices.ClipsToPlayWhenEnemyLost);
        //    ResetVariablesOnce();
        //    DebugInfo.CurrentState = "ALERTED";
        //    Info("Looking at last known enemy coordinate");
        //    Components.HumanoidFiringBehaviourComponent.FireNow = false;
        //    ApplyRootMotion(false);
        //    //if(Gunscript.instance != null)
        //    //{
        //    //    Gunscript.instance.IsFire = false;
        //    //}
        //    //if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<MasterAiBehaviour>() != null)
        //    //{
        //    //    FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<MasterAiBehaviour>().HumanoidFiringBehaviourComponent.IFired = false;
        //    //}
        //    if (StaringAtLastEnemyLostPosition == false)
        //    {
        //        RandomDirAfterTargetLost = GenerateRandomNavmeshLocation.RandomLocation(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform, 3);
        //        StartCoroutine(NormaliseStateAfterAlert()); 
        //        StaringAtLastEnemyLostPosition = true;
        //    }
        //    RotatingTransforms.ChangeRotation(transform, RandomDirAfterTargetLost, transform.position, Speeds.MovementSpeeds.BodyRotationSpeed);
        //    if (Components.NavMeshAgentComponent.enabled == true)
        //    {
        //        Components.NavMeshAgentComponent.isStopped = true;
        //    }
        //    SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
        //    ConnectWithUpperBodyAimingAnimation();
        //    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
        //    StopSpineRotation = true;
        //    //Debug.DrawLine(transform.position, RandomDirAfterTargetLost, Color.blue);
        //}
        public void ResetVariablesOnce()
        {
            if (CombatStateBehaviours.TakeCovers == true)
            {
                if (NewCoverNode != null)
                {
                    NewCoverNode.DistanceCleared = false;
                }
            }
            StopMovingBetweenCovers = false;
            StopBotForShoot = false;
            CanTakeCover = false;
            FindValidCover = true;
            ChangeCover = false;
            ReachnewCoverpoint = false;
            ReachnewFirepoint = false;
            CheckTime = false;
            SelectStrafePoint = false;
            IsGoingToPreviousPoint = false;
        }
        //IEnumerator NormaliseStateAfterAlert()
        //{
        //    yield return new WaitForSeconds(StareTimeAtLastKnownEnemyCoordinateIfStationed);
        //    FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
        //    VisibilityCheck.ConnectionLost = false;
        //    StaringAtLastEnemyLostPosition = false;
        //}
        IEnumerator NormaliseStateAfterAlertIfMovable()
        {
            yield return new WaitForSeconds(StareTimeAtLastKnownEnemyCoordinate);          
            CanMoveTowardsPursueCoordinate = false;
            StareTimeCompletedForMovableAi = true;
            ResetToNormalState = false;
            StayingNearPursuingCoordinate = false;
            if(CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableStationedEnemyPursue)
            {
                FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
                VisibilityCheck.ConnectionLost = false;
            }
        }
        IEnumerator CoroForBetweenWaypointShots()
        {
            IsAnyTaskCurrentlyRunning = true;
            RandomTimeToActivateShootWhileMovingBetweenWaypoint = Random.Range(AiFiringPoints.OpenFireBehaviour.MinTimeTillStopAndShootBehaviour, AiFiringPoints.OpenFireBehaviour.MaxTimeTillStopAndShootBehaviour);
            yield return new WaitForSeconds(RandomTimeToActivateShootWhileMovingBetweenWaypoint);

            if(StopAndShootIsCanceledByFiringPoint == false)
            {
                StopMovingBetweenWaypoint = false;

                int decider = Random.Range(0, 100);

                if (decider <= AiFiringPoints.OpenFireBehaviour.StopAndShootProbability)
                {
                    if (horizontalDisplacement.sqrMagnitude > RandomStopAndShootCancelDistanceToFiringPoint)
                    {
                        StopMovingBetweenWaypoint = true;
                        int ShouldShoot = Random.Range(0, 100);
                        if (ShouldShoot <= AiFiringPoints.OpenFireBehaviour.StrafingProbability)
                        {
                            CanAIStrafeByDefault = true;
                        }
                        else
                        {
                            CanAIStrafeByDefault = false;
                        }
                    }
                    else
                    {
                        StopMovingBetweenWaypoint = false;
                        StopAndShootIsCanceledByFiringPoint = true;
                    }

                }
                else
                {
                    StopMovingBetweenWaypoint = false;
                }


                if (StopMovingBetweenWaypoint == true)
                {
                    StopBotForShoot = true;
                    RandomTimeToKeepShootWhileMovingBetweenWaypoint = Random.Range(AiFiringPoints.OpenFireBehaviour.MinStopAndShootDuration, AiFiringPoints.OpenFireBehaviour.MaxStopAndShootDuration);
                    yield return new WaitForSeconds(RandomTimeToKeepShootWhileMovingBetweenWaypoint);
                    StartShootingNowForWaypoint = false;
                    StopBotForShoot = false;
                    StopMovingBetweenWaypoint = false;
                }
                else
                {
                    StopBotForShoot = false;
                    StartShootingNowForWaypoint = false;
                }
            }
           

            IsTaskOver = true;
        }
        IEnumerator CoroForBetweenCoverShots()
        {
            IsAnyTaskCurrentlyRunning = true;
            RandomTimeToActivateShootWhileMovingBetweenCovers = Random.Range(AiCovers.OpenFireBehaviour.MinTimeTillStopAndShootBehaviour, AiCovers.OpenFireBehaviour.MaxTimeTillStopAndShootBehaviour);
            yield return new WaitForSeconds(RandomTimeToActivateShootWhileMovingBetweenCovers);
            StopMovingBetweenCovers = false;
            int decider = Random.Range(0, 100);
            if (decider <= AiCovers.OpenFireBehaviour.StopAndShootProbability)
            {
                StopMovingBetweenCovers = true;
                int ShouldShoot = Random.Range(0, 100);
                if (ShouldShoot <= AiCovers.OpenFireBehaviour.StrafingProbability)
                {
                    SelectStrafePoint = false;
                    CanAIStrafeByDefault = true;
                }
                else
                {
                    CanAIStrafeByDefault = false;
                }
            }
            else
            {
                StopMovingBetweenCovers = false;
            }


            if (StopMovingBetweenCovers == true)
            {
                RandomTimeToKeepShootWhileMovingBetweenCovers = Random.Range(AiCovers.OpenFireBehaviour.MinStopAndShootDuration, AiCovers.OpenFireBehaviour.MaxStopAndShootDuration);
                yield return new WaitForSeconds(RandomTimeToKeepShootWhileMovingBetweenCovers);
                StartShootingNowForOpenfireCover = false;
                StopMovingBetweenCovers = false;
                StopBotForShoot = false;
            }
            else
            {
                StopBotForShoot = false;
                StartShootingNowForOpenfireCover = false;

            }
            IsTaskOver = true;
        }
        public void IndependentFiringPointShooter()
        {
#if UNITY_EDITOR
            if (spawnedTextPrefab != null)
            {
                spawnedTextPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "FIRING POINT AGENT";
                spawnedTextPrefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = transform.name;

            }
#endif
            //if (ResetWaypointRandomisation == false)
            //{
            //    //if (AiWaypoints.WaypointDetectionBehaviour.MoveImmediateOnFirstWaypoint == true)
            //    //{
            //    //    if (InitialiseWaypoint == false)
            //    //    {
            //    //        SaveResetedWaypointRandomisation = 0;
            //    //        InitialiseWaypoint = true;
            //    //    }
            //    //}
            //    ResetWaypointRandomisation = true;
            //}
            if (InitialiseFiringPointsBehaviour == false && Iscombatstatebegins == true)
            {
                InitialiseFiringPointsBehaviour = true;
                if (CombatStateBehaviours.UseFiringPoints == true)
                {
                    SavedCurrentPositionForFiringPointFinding = transform.position;
                    if (AiFiringPoints.FiringPointDetectionBehaviour.FiringPointsFinder.GetComponent<SphereCollider>() != null)
                    {
                        // SphereCollider b = AiFiringPoints.FiringPointDetectionBehaviour.FiringPointsFinder.GetComponent<SphereCollider>();
                        // b.radius = AiFiringPoints.FiringPointDetectionBehaviour.RangeToFindAFiringPoint / 2;
                        AiFiringPoints.FiringPointDetectionBehaviour.FiringPointsFinder.GetComponent<SphereCollider>().radius = AiFiringPoints.FiringPointDetectionBehaviour.RangeToFindAFiringPoint;
                    }
                    GetAccurateWaypointRange = AiFiringPoints.FiringPointDetectionBehaviour.RangeToFindAFiringPoint;// / 2;
                }
                RandomStopAndShootCancelDistanceToFiringPoint = Random.Range(AiFiringPoints.FiringPointDetectionBehaviour.MinStopAndShootCancelDistanceToFiringPoint, AiFiringPoints.FiringPointDetectionBehaviour.MaxStopAndShootCancelDistanceToFiringPoint);
                AiFiringPoints.FiringPointDetectionBehaviour.FiringPointsFinder.gameObject.SetActive(true);
                RandomFiringPointSprint = Random.Range(AiFiringPoints.SprintingBehaviour.MinRemainingDistanceToFiringPointToStopSprinting, AiFiringPoints.SprintingBehaviour.MaxRemainingDistanceToFiringPointToStopSprinting);
                RandomDistanceForShootWhileMovingBetweenWaypoint = Random.Range(AiFiringPoints.OpenFireBehaviour.MinStopAndShootDistanceToEnemyOrToCover, AiFiringPoints.OpenFireBehaviour.MaxStopAndShootDistanceToEnemyOrToCover);
               // StartCoroutine(StartFindingsForIndepentFiringPoint());
                SaveRangeForFiringPoint = AiFiringPoints.FiringPointDetectionBehaviour.RangeToFindAFiringPoint;
            }

            if (FindEnemiesScript.FindedEnemies == true)
            {
                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null
                    && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.transform != this.transform && IsNearDeadBody == false)
                {
                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<HumanoidAiHealth>() != null)
                    {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<HumanoidAiHealth>().IsDied == true)
                        {
                            FindImmediateEnemy();
                        }
                    }
                    else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.tag == "Player")
                    {
                        if (PlayerHealth.instance != null)
                        {
                            if (PlayerHealth.instance.IsDead == true)
                            {
                                FindImmediateEnemy();
                            }
                        }
                    }
                    else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<Turret>() != null)
                    {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<Turret>().IsDied == true)
                        {
                            FindImmediateEnemy();
                        }

                    }

                    CheckFieldofView();

                    if (Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, this.transform.position) < FindEnemiesScript.DetectionRadius && IsEnemyLocked == true && VisibilityCheck.CanSeeTarget(FindEnemiesScript.OriginalFov, FindEnemiesScript.DetectionRadius, Components.HumanoidFiringBehaviourComponent) && HealthScript.CompleteFirstHitAnimation == false)
                    {
                        if (CombatStateBehaviours.EnablePostCombatScan == true)
                        {
                            ResetVariableForQuickScan();
                            WasInCombatStateBefore = true;
                        }

                        CheckForLockedTargetInFOV();

                        Vector3 CheckforNewWaypoint = SavedCurrentPositionForFiringPointFinding - transform.position;

                        if (CheckforNewWaypoint.sqrMagnitude >= GetAccurateWaypointRange * GetAccurateWaypointRange)
                        {
                            SavedCurrentPositionForFiringPointFinding = transform.position;
                            AiFiringPoints.FiringPointDetectionBehaviour.FiringPointsFinder.gameObject.SetActive(true);
                        }


                        if (StartStrafingForAvoidingBots == false && BotMovingAwayFromGrenade == false && findWayPoint == true)
                        {
                            if (ReachnewFirepoint == false)
                            {
                                if (StopBotForShoot == false)
                                {
                                    pathfinder.FindClosestPointTowardsDestination(NewFiringPointNode.transform.position);

                                    DistanceToFiringPoint = pathfinder.closestPoint;

                                    if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                                    {
                                        DistanceRecievedFromFiringPoint = true;
                                    }
                                   
                                }

                                if (DistanceRecievedFromFiringPoint == true)
                                {
                                    horizontalDisplacement = new Vector3(DistanceToFiringPoint.x - transform.position.x, 0f, DistanceToFiringPoint.z - transform.position.z);
                                    NewFiringPointNode.DebugDistanceWithAiAgent = horizontalDisplacement.magnitude;

                                    if (horizontalDisplacement.magnitude <= AiFiringPoints.FiringPointDetectionBehaviour.DistanceToStopBeforeFiringPoint)
                                    {
                                        NewFiringPointNode.Info(true);
                                        NewFiringPointNode.DistanceCleared = true;
                                    }

                                    if (horizontalDisplacement.magnitude <= RandomStopAndShootCancelDistanceToFiringPoint)
                                    {
                                        StopMovingBetweenWaypoint = false;
                                        StopAndShootIsCanceledByFiringPoint = true;
                                    }
                                }
                                if (NewFiringPointNode.DistanceCleared == false
                                    && pathfinder.NoMoreChecks == true)//&& pathfinder.IsNavMeshObstacleDisabled == false
                                {
                                    if(PreviousFiringPointNode != null)
                                    {
                                        PreviousFiringPointNode.Info(false);
                                    }
                                    NewFiringPointNode.Info(false);

                                    // pathfinder.CalculatePathForCombat(0f, 0f, WayPointPositions[CurrentWayPoint].position);
                                    if (AiFiringPoints.SprintingBehaviour.EnableSprinting == false)
                                    {
                                        RandomFiringPointSprint = horizontalDisplacement.sqrMagnitude + 1000f;
                                    }
                                    //if (horizontalDisplacement.sqrMagnitude < AiWaypoints.WaypointDetectionBehaviour.RangeToFindWayPoint * AiWaypoints.WaypointDetectionBehaviour.RangeToFindWayPoint)
                                    //{                                                                                                                          
                                    if (horizontalDisplacement.sqrMagnitude >= RandomFiringPointSprint * RandomFiringPointSprint && StopMovingBetweenWaypoint == false
                                        && AiFiringPoints.SprintingBehaviour.EnableSprinting == true && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false
                                        && pathfinder.NavMeshAgentComponent.enabled == true)
                                    {
                                        StopBotForShoot = false;
                                        StopSpineRotation = true;
                                        // AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);
                                        IsCrouched = false;
                                        RotatingTransforms.ChangeRotation(transform, NewFiringPointNode.transform.position, transform.position, Speeds.MovementSpeeds.BodyRotationSpeed);
                                        if (Components.NavMeshAgentComponent.enabled == true)
                                        {
                                            Components.NavMeshAgentComponent.isStopped = false;
                                        }
                                        AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, true);
                                        SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                                        anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, false);
                                        Components.HumanoidFiringBehaviourComponent.FireNow = false;
                                        Info("Sprinting Towards Firing Point");

                                        // PreviousDestinationWhenRunning = Vector3.positiveInfinity;

                                        if (StartShootingNowForWaypoint == false && horizontalDisplacement.sqrMagnitude <= RandomDistanceForShootWhileMovingBetweenWaypoint * RandomDistanceForShootWhileMovingBetweenWaypoint && AiFiringPoints.OpenFireBehaviour.StopAndShootProbability >= 1)
                                        {
                                            StartCoroutine(CoroForBetweenWaypointShots());
                                            StartShootingNowForWaypoint = true;
                                        }
                                    }
                                    else if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false && StopMovingBetweenWaypoint == false && StopBotForShoot == false
                                        && pathfinder.NavMeshAgentComponent.enabled == true)
                                    {
                                        StopBotForShoot = false;
                                        if (Components.NavMeshAgentComponent.enabled == true)
                                        {
                                            Components.NavMeshAgentComponent.isStopped = false;
                                        }
                                        Components.HumanoidFiringBehaviourComponent.FireNow = false;
                                        LookingAtEnemy();
                                        RunTowardsDestination();

                                        AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.RunForwardSpeed, true);

                                        if (Components.HumanoidFiringBehaviourComponent.isreloading)
                                        {
                                            OnlyUpperBodyReload();
                                        }

                                        
                                        Info("Running Towards Firing Point");
                                        if (StartShootingNowForWaypoint == false && horizontalDisplacement.sqrMagnitude <= RandomDistanceForShootWhileMovingBetweenWaypoint * RandomDistanceForShootWhileMovingBetweenWaypoint && AiFiringPoints.OpenFireBehaviour.StopAndShootProbability >= 1)
                                        {
                                            StartCoroutine(CoroForBetweenWaypointShots());
                                            StartShootingNowForWaypoint = true;
                                        }
                                    }
                                    else
                                    {
                                       
                                        pathfinder.PreviousDestination = transform.position;
                                        pathfinder.IsSameDestination = transform.position;

                                       
                                        LookingAtEnemy();
                                        //if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                                        //{
                                        if (CanAIStrafeByDefault == true)
                                        {
                                            Components.HumanoidFiringBehaviourComponent.FireNow = true;
                                            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                            Fire();

                                            if (Components.HumanoidFiringBehaviourComponent.isreloading)
                                            {
                                                OnlyUpperBodyReload();
                                            }
                                        }
                                        else
                                        {
                                            Info("Stand Shooting");
                                            StopSpineRotation = false;
                                            //if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
                                            //{
                                            SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                                            //}
                                            AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);

                                            if (Components.HumanoidFiringBehaviourComponent.isreloading)
                                            {
                                                FullUpperAndLowerBodyReload();
                                            }
                                        }
                                    }

                                    //}
                                    //else
                                    //{
                                    //    findWayPoint = false;
                                    //    FindClosestWayPoint();

                                    //    DebugInfo.CurrentState = "SEARCHING FOR NEW WAYPOINT";
                                    //    LookingAtEnemy();
                                    //    if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                                    //    {
                                    //        Components.HumanoidFiringBehaviourComponent.FireNow = true;
                                    //        anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                    //        Fire();
                                    //    }
                                    //    else
                                    //    {
                                    //        Components.HumanoidFiringBehaviourComponent.FireNow = false;
                                    //        anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                    //        Reload();
                                    //    }
                                    //}
                                }
                                else if(pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                                {
                                    // Added this line so NewFiringPointNode stay occupied and do not get vacant if the AI agent is using it.before when this line was not there.
                                    // which was making the AI agent firing point to be vacant even when he was occupying it. below you will find same code when we check   if (ReachnewFirepoint == true) just for double check..
                                    if (NewFiringPointNode != null)
                                    {
                                        NewFiringPointNode.Info(true);
                                    }
                                 
                                    DistanceRecievedFromFiringPoint = false;
                                    StopAndShootIsCanceledByFiringPoint = false;
                                 //   Debug.Break();
                                    ReachnewFirepoint = true;
                                    //if (FindNewCover == false)
                                    //{
                                    //    ChangeCover = true;
                                    //    Reachnewpoints = false;
                                    //    findWayPoint = false;
                                    //    FindClosestWayPoint();
                                    //    FindNewCover = true;
                                    //}
                                    //DebugInfo.CurrentState = "SEARCHING FOR NEW WAYPOINT";
                                    //LookingAtEnemy();
                                    //if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                                    //{
                                    //    Components.HumanoidFiringBehaviourComponent.FireNow = true;
                                    //    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                    //    Fire();
                                    //}
                                    //else
                                    //{
                                    //    Components.HumanoidFiringBehaviourComponent.FireNow = false;
                                    //    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                    //    Reload();
                                    //}



                                }
                                //CurrentWayPointTransform = WayPointPositions[CurrentWayPoint].transform;
                            }
                            else
                            {
                                StopBotForShoot = false;
                                //Debug.Break();
                                if (ReachnewFirepoint == true)
                                {
                                    if (NewFiringPointNode != null)
                                    {
                                        NewFiringPointNode.Info(true);
                                    }

                                    DistanceRecievedFromFiringPoint = false;
                                    StopAndShootIsCanceledByFiringPoint = false;
                                    ReachedWayPoint();
                                }
                                else
                                {
                                    //if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                                    //{
                                    Components.HumanoidFiringBehaviourComponent.FireNow = true;
                                    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                    Fire();

                                    if (Components.HumanoidFiringBehaviourComponent.isreloading)
                                    {
                                        OnlyUpperBodyReload();
                                    }
                                    //}
                                    //else
                                    //{
                                    //    Components.HumanoidFiringBehaviourComponent.FireNow = false;
                                    //    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                    //    Reload();
                                    //}
                                }
                                if (CheckTime == false)
                                {
                                    StartCoroutine(ResettingWayPoint());
                                    CheckTime = true;
                                }
                            }
                        }
                        else
                        {
                            //  Debug.Log("Strafe" + transform.name);
                            //  Debug.Break();
                            if (StartStrafingForAvoidingBots == true)
                            {
                                RandomStrafing();
                                if (Components.HumanoidFiringBehaviourComponent.isreloading)
                                {
                                    OnlyUpperBodyReload();
                                }
                            }
                            else
                            {
                                if (BotMovingAwayFromGrenade == false)
                                {

                                    StopSpineRotation = false;
                                    if (CheckTime == false && WaypointSearched == true)
                                    {
                                        StartCoroutine(ResettingWayPoint());
                                        CheckTime = true;
                                    }
                                    if (ReachnewFirepoint == true)
                                    {
                                        // Previously line 6033 to 6037 was here.  
                                        ReachedWayPoint();
                                    }
                                    else
                                    {
                                        //if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                                        //{
                                        Components.HumanoidFiringBehaviourComponent.FireNow = true;
                                        anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                        Fire();
                                        if (Components.HumanoidFiringBehaviourComponent.isreloading)
                                        {
                                            OnlyUpperBodyReload();
                                        }
                                        //}
                                        //else
                                        //{
                                        //    Components.HumanoidFiringBehaviourComponent.FireNow = false;
                                        //    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                        //    Reload();
                                        //}
                                    }
                                }
                                else
                                {
                                    SprintAwayFromGrenade();
                                }

                            }
                        }
                    }
                    else
                    {
                        if (BotMovingAwayFromGrenade == false)
                        {
                            if (HealthScript.CompleteFirstHitAnimation == false)
                            {
                                if (VisibilityCheck.ConnectionLost == true)
                                {
                                    if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableApproachingEnemyPursue
                                       || HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableStationedEnemyPursue)
                                    {
                                        ConnectionLostForMovingBots();
                                    }
                                    //else if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableStationedEnemyPursue)
                                    //{
                                    //    ConnectionLostForStationarybot();
                                    //}
                                    else if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.DoNotPursueEnemy)
                                    {
                                        FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
                                        VisibilityCheck.ConnectionLost = false;
                                        StaringAtLastEnemyLostPosition = false;
                                    }
                                }
                                else
                                {
                                    DefaultInvestigationBehaviour();
                                }
                            }
                        }
                        else
                        {
                            SprintAwayFromGrenade();
                        }

                    }
                }
                else
                {
                    if (IsNearDeadBody == true && NonCombatBehaviours.EnableEmergencyAlerts == true && IsInEmergencyState == true && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != transform.root.transform
                        && HealthScript.CompleteFirstHitAnimation == false)
                    {
                        CompleteEmergencyStateBehaviour();
                    }
                    else if (IsNearDeadBody == true && NonCombatBehaviours.EnableDeadBodyAlerts == true && IsInEmergencyState == false && IsEmergencyStateCurrentlyActive == false
                        &&  HealthScript.CompleteFirstHitAnimation == false)//FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != transform.root.transform &&
                    {
                        DeadBodyInvestigation();
                    }
                    else
                    {
                        if (SearchingForSound == false && HealthScript.CompleteFirstHitAnimation == false)
                        {
                            // This code is commented on 14th Jan 2025 because when in beginning there was a player and turret and AI agent . The AI agent removed its enemies which was Player and Turret and ignored them basically
                           // and did not updated its enemy and did not shoot even.
                           // FindImmediateEnemy();
                            DefaultInvestigationBehaviour();
                        }
                    }

                }
            }
            else
            {
                DefaultInvestigationBehaviour();
            }
        }
        public void GrenadeVisibilityChecker(Transform objectToLookAt)
        {
            if (AgentRole != Role.Zombie)
            {
                if(CombatStateBehaviours.EnableGrenadeAlerts == true)
                {
                    GrenadeAlerts.GrenadeDetector.LookAt(objectToLookAt);
                    Vector3 directionToTarget = objectToLookAt.position - GrenadeAlerts.GrenadeDetector.position;
                    RaycastHit hit;
                    if (Physics.Raycast(GrenadeAlerts.GrenadeDetector.position, GrenadeAlerts.GrenadeDetector.forward, out hit, directionToTarget.magnitude, GrenadeAlerts.VisibleLayers))
                    {
                        DefinatelyRunFromGrenade = (hit.transform == objectToLookAt);
                        if (DefinatelyRunFromGrenade == true)
                        {
                            if (RandomiseRunFromGrenadesBehaviour == true)
                            {
                                int Randomise = Random.Range(0, 100);
                                if (Randomise <= 50)
                                {
                                    RunFromGrenades = false;
                                }
                                else
                                {
                                    RunFromGrenades = true;
                                }
                            }
                            else
                            {
                                RunFromGrenades = true;
                            }
                        }

                    }
                    else
                    {
                        DefinatelyRunFromGrenade = false;
                    }
                }
               
            }
        }
        public void SprintAwayFromGrenade()
        {
            if (AgentRole != Role.Zombie)
            {
                if (CombatStateBehaviours.EnableGrenadeAlerts == true)
                {
                    if (RunFromGrenades == true && DefinatelyRunFromGrenade == true)
                    {
                        if (CombatStateBehaviours.EnablePostCombatScan == true)
                        {
                            ResetVariableForQuickScan();
                            WasInCombatStateBefore = true;
                        }
                        Info("Sprinting Away from grenade");
                        if (GeneratedSprintPoint == false)
                        {
                            PreviousEnemyDuringCharging = transform; // important so when path is incomplete AI agent could reset it and find the path again with enemy.
                            Components.HumanoidAiAudioPlayerComponent.PlayNonRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceGrenadeAlertAudioClips);
                            directionpoint = Random.Range(0, GrenadeAlerts.RetreatPoints.Length);
                            LocationForSprinting = GenerateRandomNavmeshLocation.RandomLocation(GrenadeAlerts.RetreatPoints[directionpoint], GrenadeAlerts.RetreatPointRadius);
                            StartCoroutine(RunTimer());
                            GeneratedSprintPoint = true;
                        }
                        pathfinder.FindClosestPointTowardsDestination(LocationForSprinting);
                        Vector3 SaferDistance = pathfinder.closestPoint - transform.position;
                        if (SaferDistance.magnitude <= 3f && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                        {
                            GeneratedSprintPoint = false;
                            BotMovingAwayFromGrenade = false;
                        }
                        else if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false && pathfinder.NavMeshAgentComponent.enabled == true)
                        {
                            StopSpineRotation = true;
                            //  AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);
                            IsCrouched = false;
                            //  RotatingTransforms.ChangeRotation(transform, LocationForGrenadeSprint, transform.position, MovementSpeeds.BodyRotationSpeed);
                            if (Components.NavMeshAgentComponent.enabled == true)
                            {
                                Components.NavMeshAgentComponent.isStopped = false;
                            }
                            AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, true);
                            SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, false);
                            Components.HumanoidFiringBehaviourComponent.FireNow = false;

                        }
                    }
                    else
                    {
                        //Debug.Log(transform.name + " " + "Not properly sprinting away from grenade" + "WasBotMovingAwayFromGrenade" + BotMovingAwayFromGrenade);
                    }
                }
            }
        }
        public IEnumerator RunTimer()
        {
            yield return new WaitForSeconds(GrenadeAlerts.RetreatTimer);
            GeneratedSprintPoint = false;
            BotMovingAwayFromGrenade = false;

        }
        //Transform GetClosestStrafeDestination(Vector3 Destination, Transform[] strafepoint)
        //{
        //    Transform tMin = null;
        //    float minDist = Mathf.Infinity;
        //    Vector3 currentPos = Destination;
        //    foreach (Transform t in strafepoint)
        //    {
        //        float dist = Vector3.Distance(t.position, currentPos);
        //        if (dist < minDist)
        //        {
        //            tMin = t;
        //            minDist = dist;
        //        }
        //    }
        //    return tMin;
        //}
        Transform GetClosestStandCoverDirection(Transform[] standcoverdirection)
        {
            Transform tMin = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
            foreach (Transform t in standcoverdirection)
            {
                float dist = Vector3.Distance(t.position, currentPos);
                if (dist < minDist)
                {
                    tMin = t;
                    minDist = dist;
                }
            }
            return tMin;
        }
        //Transform GetClosestCoverNode(List<Transform> AllCoverNodes)
        //{
        //    Transform tMin = null;
        //    float minDist = Mathf.Infinity;
        //    Vector3 currentPos = strafePoint;
        //    foreach (Transform t in AllCoverNodes)
        //    {
        //        float dist = Vector3.Distance(t.position, currentPos);
        //        if (dist < minDist)
        //        {
        //            tMin = t;
        //            minDist = dist;
        //        }
        //    }
        //    return tMin;
        //}
        //IEnumerator Coro()
        //{
        //    float Randomise = Random.Range(AiStrafing.MinTimeTocheckCorrectStrafeAnimation, AiStrafing.MaxTimeTocheckCorrectStrafeAnimation);
        //    yield return new WaitForSeconds(Randomise);
        //    Transform t = GetClosestStrafe(AiStrafing.AddAllDirections);
        //    for(int x = 0;x < AiStrafing.CustomStrafing.StrafePoint.Length; x++)
        //    {
        //        if(t.name == AiStrafing.CustomStrafing.StrafePoint[x].name)
        //        {
        //            strafedir = x;
        //        }
        //    }
        //    AnimController(false, AiStrafing.StrafeAnimations[strafedir].MovingSpeed, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);
        //    SetAnims(AiStrafing.StrafeAnimations[strafedir].AnimationParameterName);
        //    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);

        //    StartStrafeAnimationCheck = false;
        //}
        //IEnumerator CoroToFindClosestStrafe(Vector3 Destination)
        //{
        //    float Randomise = Random.Range(AiStrafing.MinTimeBetweenStrafeDirectionChecks, AiStrafing.MaxTimeBetweenStrafeDirectionChecks);
        //    yield return new WaitForSeconds(Randomise);
        //    pathfinder.FindClosestPointTowardsDestination(Destination);
        //    Transform t = GetClosestStrafeDestination(pathfinder.closestPoint, AiDirectionMarkers.DirectionMarkers);
        //   // FindNearestDirectionPointer(t);
        //    StartStrafeAnimationCheck = false;
        //}
        IEnumerator CoroForIntermittentShootStrafe()
        {
            float Randomise = Random.Range(AiStrafing.MinTimeBetweenShootingCycles, AiStrafing.MaxTimeBetweenShootingCycles);
            yield return new WaitForSeconds(Randomise);
            int RandomValue = Random.Range(0, 100);
            if (RandomValue > AiStrafing.ShootingProbability)
            {
                AnimController(false, GetMovingSpeed, AiAgentAnimatorParameters.DefaultStateParameterName, true, false);
                SetAnimationForFullBody(GetStrafeAnimationName);
                anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
            }
            else
            {
                AnimController(false, GetMovingSpeed, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);
                SetAnimationForFullBody(GetStrafeAnimationName);
                anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
            }
            IntermittentShootRandomisation = false;
        }
        //IEnumerator CoroForRandomiseProceduralAndCustomStrafe()
        //{
        //    float Randomise = Random.Range(AiStrafing.MinTimeToRandomiseBetweenProceduralAndCustomStrafe, AiStrafing.MaxTimeToRandomiseBetweenProceduralAndCustomStrafe);
        //    yield return new WaitForSeconds(Randomise);
        //    int RandomValue = Random.Range(0, 100);
        //    Debug.Log(RandomValue);
        //    if (RandomValue <= 50)
        //    {
        //        AiStrafing.ForceCustomStrafing = true;
        //    }
        //    else
        //    {
        //        AiStrafing.ForceCustomStrafing = false;
        //    }
        //    SelectStrafePoint = false;
        //    StartStrafeAnimationCheck = false;
        //    IntermittentProceduralShootRandomisation = false;
        //    IntermittentCustomShootRandomisation = false;
        //    SwitchBetweenProceduralAndCustomStrafe = false;
        //}
        //void ChangeAnimationClip(string currentanimationclip, AnimationClip newClip)
        //{
        //    anim.runtimeAnimatorController = new AnimatorOverrideController(anim.runtimeAnimatorController);
        //    var animatorOverrideController = anim.runtimeAnimatorController as AnimatorOverrideController;
        //    animatorOverrideController[currentanimationclip] = newClip;
        //}
        //public void FindNearestDirectionPointer(Transform Direction)
        //{
        //    if (AiStrafing.AnimationsToPlay == StrafeDirection.WalkAnimations)
        //    {
        //        for (int x = 0; x < AiDirectionMarkers.WalkAnimations.Count; x++)
        //        {
        //            if (Direction != null)
        //            {
        //                if (Direction.name == AiDirectionMarkers.WalkAnimations[x].DirectionName)
        //                {
        //                    strafedir = x;
        //                    GetStrafeAnimationName = AiDirectionMarkers.WalkAnimations[x].AnimationParameterName;
        //                    GetMovingSpeed = AiDirectionMarkers.WalkAnimations[x].MovingSpeed;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (int x = 0; x < AiDirectionMarkers.RunAnimations.Count; x++)
        //        {
        //            if(Direction != null)
        //            {
        //                if (Direction.name == AiDirectionMarkers.RunAnimations[x].DirectionName)
        //                {
        //                    strafedir = x;
        //                    GetStrafeAnimationName = AiDirectionMarkers.RunAnimations[x].AnimationParameterName;
        //                    GetMovingSpeed = AiDirectionMarkers.RunAnimations[x].MovingSpeed;
                           
        //                }
        //            }
                   
        //        }
        //    }
        //}
        float GetSpeed(string AnimName)
        {
            if(AnimName == "WalkForward")
            {
                return Speeds.MovementSpeeds.WalkForwardSpeed;
            }
            else if (AnimName == "WalkBackward")
            {
                return Speeds.MovementSpeeds.WalkBackwardSpeed;
            }
            else if (AnimName == "WalkLeft")
            {
                return Speeds.MovementSpeeds.WalkLeftSpeed;
            }
            else if (AnimName == "WalkRight")
            {
                return Speeds.MovementSpeeds.WalkRightSpeed;
            }
            else if (AnimName == "WalkForwardRight")
            {
                return Speeds.MovementSpeeds.WalkForwardRightSpeed;
            }
            else if (AnimName == "WalkForwardLeft")
            {
                return Speeds.MovementSpeeds.WalkForwardLeftSpeed;
            }
            else if (AnimName == "WalkBackwardRight")
            {
                return Speeds.MovementSpeeds.WalkBackwardRightSpeed;
            }
            else if (AnimName == "WalkBackwardLeft")
            {
                return Speeds.MovementSpeeds.WalkBackwardLeftSpeed;
            }
            else if (AnimName == "RunForward")
            {
                return Speeds.MovementSpeeds.RunForwardSpeed;
            }
            else if (AnimName == "RunBackward")
            {
                return Speeds.MovementSpeeds.RunBackwardSpeed;
            }
            else if (AnimName == "RunRight")
            {
                return Speeds.MovementSpeeds.RunRightSpeed;
            }
            else if (AnimName == "RunLeft")
            {
                return Speeds.MovementSpeeds.RunLeftSpeed;
            }
            else if (AnimName == "RunForwardLeft")
            {
                return Speeds.MovementSpeeds.RunForwardLeftSpeed;
            }
            else if (AnimName == "RunForwardRight")
            {
                return Speeds.MovementSpeeds.RunForwardRightSpeed;
            }
            else if (AnimName == "RunBackwardRight")
            {
                return Speeds.MovementSpeeds.RunBackwardRightSpeed;
            }
            else if (AnimName == "RunBackwardLeft")
            {
                return Speeds.MovementSpeeds.RunBackwardLeftSpeed;
            }
            else
            {
                return Speeds.MovementSpeeds.WalkForwardSpeed;
            }
            
        }
        public void GetAllStrafeAnimations(string Direction)
        {
            if (Direction == "Forward")
            {
                GetStrafeAnimationName = AiStrafing.StrafeAnimations.Forward.ToString();
                GetMovingSpeed = GetSpeed(AiStrafing.StrafeAnimations.Forward.ToString());
            }
            if (Direction == "Backward")
            {
                GetStrafeAnimationName = AiStrafing.StrafeAnimations.Backward.ToString();
                GetMovingSpeed = GetSpeed(AiStrafing.StrafeAnimations.Backward.ToString());
            }
            if (Direction == "Right")
            {
                GetStrafeAnimationName = AiStrafing.StrafeAnimations.Right.ToString();
                GetMovingSpeed = GetSpeed(AiStrafing.StrafeAnimations.Right.ToString());
            }
            if (Direction == "Left")
            {
                GetStrafeAnimationName = AiStrafing.StrafeAnimations.Left.ToString();
                GetMovingSpeed = GetSpeed(AiStrafing.StrafeAnimations.Left.ToString());
            }
            if (Direction == "ForwardLeft")
            {
                GetStrafeAnimationName = AiStrafing.StrafeAnimations.ForwardLeft.ToString();
                GetMovingSpeed = GetSpeed(AiStrafing.StrafeAnimations.ForwardLeft.ToString());
            }
            if (Direction == "ForwardRight")
            {
                GetStrafeAnimationName = AiStrafing.StrafeAnimations.ForwardRight.ToString();
                GetMovingSpeed = GetSpeed(AiStrafing.StrafeAnimations.ForwardRight.ToString());
            }
            if (Direction == "BackwardLeft")
            {
                GetStrafeAnimationName = AiStrafing.StrafeAnimations.BackwardLeft.ToString();
                GetMovingSpeed = GetSpeed(AiStrafing.StrafeAnimations.BackwardLeft.ToString());
            }
            if (Direction == "BackwardRight")
            {
                GetStrafeAnimationName = AiStrafing.StrafeAnimations.BackwardRight.ToString();
                GetMovingSpeed = GetSpeed(AiStrafing.StrafeAnimations.BackwardRight.ToString());
            }
        }
        public void GetAllRunAnimations(string Direction)
        {
            if (Direction == "Forward")
            {
                GetStrafeAnimationName =  MovementAnimations.RunForward.ToString();
                GetMovingSpeed = Speeds.MovementSpeeds.RunForwardSpeed;
            }
            if (Direction == "Backward")
            {
                GetStrafeAnimationName = MovementAnimations.RunBackward.ToString();
                GetMovingSpeed = Speeds.MovementSpeeds.RunBackwardSpeed;
            }
            if (Direction == "Right")
            {
                GetStrafeAnimationName = MovementAnimations.RunRight.ToString();
                GetMovingSpeed = Speeds.MovementSpeeds.RunRightSpeed;
            }
            if (Direction == "Left")
            {
                GetStrafeAnimationName = MovementAnimations.RunLeft.ToString();
                GetMovingSpeed = Speeds.MovementSpeeds.RunLeftSpeed;
            }
            if (Direction == "ForwardLeft")
            {
                GetStrafeAnimationName = MovementAnimations.RunForwardLeft.ToString();
                GetMovingSpeed = Speeds.MovementSpeeds.RunForwardLeftSpeed;
            }
            if (Direction == "ForwardRight")
            {
                GetStrafeAnimationName = MovementAnimations.RunForwardRight.ToString();
                GetMovingSpeed = Speeds.MovementSpeeds.RunForwardRightSpeed;
            }
            if (Direction == "BackwardLeft")
            {
                GetStrafeAnimationName = MovementAnimations.RunBackwardLeft.ToString();
                GetMovingSpeed = Speeds.MovementSpeeds.RunBackwardLeftSpeed;
            }
            if (Direction == "BackwardRight")
            {
                GetStrafeAnimationName = MovementAnimations.RunBackwardRight.ToString();
                GetMovingSpeed = Speeds.MovementSpeeds.RunBackwardRightSpeed;
            }
        }
        public void NewStrafeAnimationPicker(bool IsForStrafing)
        {
            Vector3 velocity = Components.NavMeshAgentComponent.velocity.normalized;

            // Get the forward and right vectors of the agent's transform
            Vector3 forwardVector = transform.forward;
            Vector3 rightVector = transform.right;

            // Calculate the dot products to determine movement direction
            float forwardDot = Vector3.Dot(velocity, forwardVector);
            float rightDot = Vector3.Dot(velocity, rightVector);

            // Check movement direction
            if (Mathf.Abs(forwardDot) > Mathf.Abs(rightDot))
            {
                if (forwardDot > 0)
                {
                    if(IsForStrafing == true)
                    {
                        Debug.Log("Forward");
                        GetAllStrafeAnimations("Forward");
                    }
                    else
                    {
                        Debug.Log("Forward");
                        GetAllRunAnimations("Forward");
                    }               
                }
                else
                {
                    if (IsForStrafing == true)
                    {
                        Debug.Log("Backward");
                        GetAllStrafeAnimations("Backward");
                    }
                    else
                    {
                        Debug.Log("Backward");
                        GetAllRunAnimations("Backward");

                    }
                }
            }
            else
            {
                if (rightDot > 0)
                {
                    if (forwardDot > 0)
                    {
                        if (IsForStrafing == true)
                        {
                            Debug.Log("ForwardRight");
                            GetAllStrafeAnimations("ForwardRight");
                        }
                        else
                        {
                            Debug.Log("ForwardRight");
                            GetAllRunAnimations("ForwardRight");
                        }
                    }
                    else
                    {
                        if (IsForStrafing == true)
                        {
                            Debug.Log("BackwardRight");
                            GetAllStrafeAnimations("BackwardRight");
                        }
                        else
                        {
                            Debug.Log("BackwardRight");
                            GetAllRunAnimations("BackwardRight");
                        }
                        
                    }
                }
                else
                {
                    if (forwardDot > 0)
                    {
                        if (IsForStrafing == true)
                        {
                            Debug.Log("ForwardLeft");
                            GetAllStrafeAnimations("ForwardLeft");
                        }
                        else
                        {
                            Debug.Log("ForwardLeft");
                            GetAllRunAnimations("ForwardLeft");
                        }
                    }
                    else
                    {
                        if (IsForStrafing == true)
                        {
                            Debug.Log("BackwardLeft");
                            GetAllStrafeAnimations("BackwardLeft");
                        }
                        else
                        {
                            Debug.Log("BackwardLeft");
                            GetAllRunAnimations("BackwardLeft");
                        }
                    }
                }
            }

            // Additional checks for moving purely right or left
            if (rightDot > 0 && Mathf.Abs(forwardDot) < 0.1f)
            {
                if (IsForStrafing == true)
                {
                    Debug.Log("Right");
                    GetAllStrafeAnimations("Right");
                }
                else
                {
                    Debug.Log("Right");
                    GetAllRunAnimations("Right");
                }
            }
            else if (rightDot < 0 && Mathf.Abs(forwardDot) < 0.1f)
            {
                if (IsForStrafing == true)
                {
                    Debug.Log("Left");
                    GetAllStrafeAnimations("Left");
                }
                else
                {
                    Debug.Log("Left");
                    GetAllRunAnimations("Left");
                }
            }
        }
        public void StrafeControl(float MinimumStrafeRange, float MaximumStrafeRange, float MinimumTimeBetweenStrafes, float MaximumTimeBetweenStrafes)
        {
            ReachnewCoverpoint = false;
            ReachnewFirepoint = false;
            StopSpineRotation = false;
            LookingAtEnemy();

            IsCrouched = false; // newly added 

            if (VisibilityCheck.DebugNumberOfRaycasts >= RaycastChecksToDoWhenTargetLostDuringStrafing && IsGoingToPreviousPoint == false)
            {
                //pathfinder.FindClosestPointTowardsDestination(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position);
                //if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleAgentDisabled == false)
                //{
                //    if (pathfinder.PathIsUnreachable == false)
                //    {
                        IsGoingToPreviousPoint = true;
                //    }
                //    else if(pathfinder.PathIsUnreachable == true)
                //    {
                //        IsGoingToPreviousPoint = true;
                //    }
                //} 
            }

            if (SelectStrafePoint == false && IsGoingToPreviousPoint == false)
            {
                if (AiStrafing.ForceCustomStrafing == true)
                {
                    strafedir = Random.Range(0, AiStrafing.CustomStrafing.StrafePoints.Length);
                    strafePoint = GenerateRandomNavmeshLocation.RandomLocation(AiStrafing.CustomStrafing.StrafePoints[strafedir], 1);
                    StartCoroutine(ResetStrafeRanges(MinimumTimeBetweenStrafes, MaximumTimeBetweenStrafes));
                    SelectStrafePoint = true;

                    if(StrafingForChargingAgent == true)
                    {                    
                        // Create a new NavMeshPath object to store the calculated path
                        NavMeshPath path = new NavMeshPath();

                        NavMeshObstacleComponent.enabled = false;
                        Components.NavMeshAgentComponent.enabled = true;

                        // Calculate the path to the strafePoint
                        Components.NavMeshAgentComponent.CalculatePath(strafePoint, path);

                        // Check if the path is complete
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            MoveToNewStrafePoint = true;
                        }
                        else
                        {
                            MoveToNewStrafePoint = false;
                        }
                    }
                  
                }
                else
                {

                    SelectARandomPoint = Random.Range(MinimumStrafeRange, MaximumStrafeRange);
                    strafePoint = GenerateRandomNavmeshLocation.RandomLocation(transform, SelectARandomPoint);
                    StartCoroutine(ResetStrafeRanges(MinimumTimeBetweenStrafes, MaximumTimeBetweenStrafes));
                    SelectStrafePoint = true;

                    if (StrafingForChargingAgent == true)
                    {
                        // Create a new NavMeshPath object to store the calculated path
                        NavMeshPath path = new NavMeshPath();

                        NavMeshObstacleComponent.enabled = false;
                        Components.NavMeshAgentComponent.enabled = true;

                        // Calculate the path to the strafePoint
                        Components.NavMeshAgentComponent.CalculatePath(strafePoint, path);

                        // Check if the path is complete
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            MoveToNewStrafePoint = true;
                        }
                        else
                        {
                            MoveToNewStrafePoint = false;
                        }
                    }
                }

                //StartCoroutine(EnableNavMeshAgentComponent());
                Components.HumanoidFiringBehaviourComponent.FireNow = true;
            }
            else if (IsGoingToPreviousPoint == true)
            {
                SelectStrafePoint = true;
            }
           
            if (IsGoingToPreviousPoint == false)
            {
                if (StrafingForChargingAgent == true)
                {
                    if (MoveToNewStrafePoint == true)
                    {                     
                        pathfinder.closestPoint = strafePoint;
                    }
                    else
                    {
                        MoveToNewStrafePoint = false;
                        StrafingForChargingAgent = false;
                        if (pathfinder != null)
                        {
                            pathfinder.FindClosestPointTowardsDestination(strafePoint);
                        }
                    }
                }
                else
                {
                    if (pathfinder != null)
                    {
                        pathfinder.FindClosestPointTowardsDestination(strafePoint);
                    }
                }
              
            }
            else
            {
                pathfinder.FindClosestPointTowardsDestination(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position);
                if(pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                {
                    if(pathfinder.PathIsUnreachable == false)
                    {
                        Vector3 dirwithEnemy = pathfinder.closestPoint - transform.position;
                        if (dirwithEnemy.magnitude <= DistanceToStopChargingAtEnemyForStrafe)
                        {
                            //StartCoroutine(ResetStrafeRanges(MinimumTimeBetweenStrafes, MaximumTimeBetweenStrafes));
                            //IsGoingToPreviousPoint = false;
                            SelectStrafePoint = false;
                            IsGoingToPreviousPoint = false;
                            ChargePathIsIncompleteWithTarget = false;
                        }
                        else
                        {
                            VisibilityCheck.DebugNumberOfRaycasts = 0;
                            SelectStrafePoint = false;

                        }
                    }
                    else
                    {
                        if (ChargePathIsIncompleteWithTarget == false)
                        {
                            StartCoroutine(ResetStrafeRanges(MinimumTimeBetweenStrafes, MaximumTimeBetweenStrafes));
                            ChargePathIsIncompleteWithTarget = true;

                        }

                        if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                        {
                           // Debug.Log("Shooting");
                            StandShoot();
                        }
                        else
                        {
                          //  Debug.Log("Reloading");
                            FullUpperAndLowerBodyReload();
                        }
                    }       
                }
            }

            Vector3 dirwithStrafepoint = pathfinder.closestPoint - transform.position;

            if (dirwithStrafepoint.magnitude < AiStrafing.ClosetStoppingDistanceToStrafePoint && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false && pathfinder.PathIsUnreachable == false
                || dirwithStrafepoint.magnitude < AiStrafing.ClosetStoppingDistanceToStrafePoint && MoveToNewStrafePoint == true || StrafingForChargingAgent == true && MoveToNewStrafePoint == false)
            {
                Debug.Log("Shooting");
                if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                {
                   // Debug.Log("Shooting");
                    StandShoot();
                }
                else
                {
                  //  Debug.Log("Reloading");
                    FullUpperAndLowerBodyReload();
                }
            }
            else if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false && pathfinder.PathIsUnreachable == false && pathfinder.NavMeshAgentComponent.enabled == true
                || pathfinder.NavMeshAgentComponent.enabled == true && MoveToNewStrafePoint == true)
            {
                Debug.Log("Strafing And Moving To Destination");

                NewStrafeAnimationPicker(true);
                if (AiStrafing.EnableShootingBetweenStrafing == true && AiStrafing.EnableShootingCycles == false)
                {
                    AnimController(false, GetMovingSpeed, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);
                    SetAnimationForFullBody(GetStrafeAnimationName);
                    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                }
                else if (AiStrafing.EnableShootingBetweenStrafing == true && AiStrafing.EnableShootingCycles == true)
                {
                    AnimController(false, GetMovingSpeed, AiAgentAnimatorParameters.DefaultStateParameterName, true, Components.HumanoidFiringBehaviourComponent.FireNow);
                    SetAnimationForFullBody(GetStrafeAnimationName);
                    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);

                    if (IntermittentShootRandomisation == false)
                    {
                        StartCoroutine(CoroForIntermittentShootStrafe());
                        IntermittentShootRandomisation = true;
                    }
                }
                else
                {
                    AnimController(false, GetMovingSpeed, AiAgentAnimatorParameters.DefaultStateParameterName, true, false);
                    SetAnimationForFullBody(GetStrafeAnimationName);
                    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                }

                SetAnimationForFullBody(GetStrafeAnimationName);

                
                if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                {
                    ConnectWithUpperBodyAimingAnimation();
                }
                else
                {
                    OnlyUpperBodyReload();
                }

                //if (pathfinder.closestPoint != PrevDestination)
                //{
                //    StartCoroutine(ReEnableNavigation());
                //    CanStartMoving = false;
                //    PrevDestination = pathfinder.closestPoint;
                //}

                //if (CanStartMoving == true)
                //{
                if (Components.NavMeshAgentComponent.enabled == true)
                {
                    //InternalSpawningVisualHelperGizmosObjects.instance.Spawning(pathfinder.closestPoint, transform.name);
                    Components.NavMeshAgentComponent.destination = pathfinder.closestPoint;
                }
                //}
            }
           
            PreviousDestinationWhenRunning = Vector3.positiveInfinity;
        }
        public void ShootingController()
        {
            // if (EnableStrafing == true && EnableCrouching == false)
            // {
            //IsCrouched = false;
            if (CombatStateBehaviours.EnableStrafing == true)
            {
                //if (StartStrafingForAvoidingBots == true)
                //{
                //    StrafeControl(AiStrafing.FriendliesDistancingValues.MinStrafeAwayDistance, AiStrafing.FriendliesDistancingValues.MaxStrafeAwayDistance, AiStrafing.
                //        FriendliesDistancingValues.MinTimeBetweenStrafeAway, AiStrafing.FriendliesDistancingValues.MaximumTimeBetweenStrafes);
                //}
                //else
                //{
                //if (AiStrafing.ForceCustomStrafing == true || AiStrafing.EnableShootingWhilePr)
                //{
                //                    Debug.Log("Here");

                // }
                //}

                //if (OvveruleStandShootPostureAnimation == false)
                //{
                //    StartCoroutine(InitialShootBeforeMoving());
                //    OvveruleStandShootPostureAnimation = true;
                //}

                //if (OvveruleStandShootPostureAnimation == true && UsualTask == false)
                //{
                //    StandShoot();
                //}
                //else
                //{
                StrafeControl(AiStrafing.ProceduralStrafing.MinStrafeRange, AiStrafing.
                        ProceduralStrafing.MaxStrafeRange, AiStrafing.MinTimeTillNextStrafeDirection, AiStrafing.MaxTimeTillNextStrafeDirection);
                // }
            }
            else
            {
                Fire();
            }




            //  }
            //else if (EnableStrafing == false && EnableCrouching == true)
            //{
            //    CoverFireController();
            //}
            //else if (EnableStrafing == true && EnableCrouching == true)
            //{
            //    if (ChangeBehaviour == false)
            //    {
            //        RandomBehaviour = Random.Range(0, 3);
            //        ChangeBehaviour = true;
            //        StartCoroutine(ResetShootingPosture());
            //    }

            //    if (VeryNearToEnemy == true)
            //    {
            //        RandomBehaviour = 0;
            //    }

            //    if (RandomBehaviour == 2)
            //    {
            //        CoverFireController();
            //    }
            //    else
            //    {
            //        StrafeControl();
            //    }
            //}
            //else if (EnableStrafing == false && EnableCrouching == false)
            //{
            //    SetAnims(Fire);
            //    AnimController(true, 0f, DefaultState, true, true);
            //}
        }
        IEnumerator ResetStrafeRanges(float MinimumTimeBetweenStrafes, float MaximumTimeBetweenStrafes)
        {
            float StrafeRandomTime = Random.Range(MinimumTimeBetweenStrafes, MaximumTimeBetweenStrafes);
            yield return new WaitForSeconds(StrafeRandomTime);
            SelectStrafePoint = false;
            IsGoingToPreviousPoint = false;
            ChargePathIsIncompleteWithTarget = false;
        }
        //IEnumerator StrafeWaitingPoint()
        //{
        //    float Randomise = Random.Range(AiStrafing.FriendliesDistancingValues.MinTimeBetweenStrafeAway, AiStrafing.FriendliesDistancingValues.MaximumTimeBetweenStrafes);
        //    yield return new WaitForSeconds(Randomise);
        //    CanStrafe = true;
        //    SelectStrafePoint = false;
        //}
        //public void StrafeFiringController()
        //{

        //    //if (StayWithCommander == false && MovingTowardsCover == false)
        //    //  {
        //    StopSpineRotation = false;
        //    //anim.SetBool(DefaultStateAnimationName, true);
        //    LookingAtEnemy();
        //    DebugInfo.CurrentState = "STRAFING";
        //    if (!Components.HumanoidFiringBehaviourComponent.isreloading && CrouchPositions.Count > 0)
        //    {
        //        if (CrouchPositions[CurrentCoverPoint].GetComponent<CoverNode>().StrafePoints.Length > 0 && CrouchPositions[CurrentCoverPoint].GetComponent<CoverNode>().ActivateStrafe == true)
        //        {
        //            if (SelectStrafePoint == false)
        //            {
        //                int RandomPoint = Random.Range(0, CrouchPositions[CurrentCoverPoint].GetComponent<CoverNode>().StrafePoints.Length);
        //                strafePoint = CrouchPositions[CurrentCoverPoint].GetComponent<CoverNode>().StrafePoints[RandomPoint].transform.position;

        //                if (RandomPoint != LastPredefinedStrafePoint)
        //                {
        //                    LastPredefinedStrafePoint = RandomPoint;
        //                    SelectStrafePoint = true;
        //                }

        //            }

        //            if (SelectStrafePoint == true && CanStrafe == true)
        //            {
        //                Vector3 dirwithStrafepoint = strafePoint - transform.position;
        //                //Debug.Log("Strafing Right Now");

        //                if (dirwithStrafepoint.sqrMagnitude < 0.5f && dirwithStrafepoint.sqrMagnitude < 1f)
        //                {
        //                    CanStrafe = false;
        //                   // StartCoroutine(StrafeWaitingPoint());
        //                    //  CrouchPositions[CurrentCoverPoint].GetComponent<CoverNode>().ActivateStrafe = false;

        //                    //  Debug.Log("YESSSS");
        //                    //  Debug.Log("I");                           
        //                    AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);
        //                    SetAnims(AiAgentAnimatorParameters.FireParameterName);
        //                }
        //                else
        //                {
        //                    float Angleforward = Vector3.Angle(this.FOV.RaycastChecker.transform.forward, dirwithStrafepoint);
        //                    float AngleBackward = Vector3.Angle(-this.FOV.RaycastChecker.transform.forward, dirwithStrafepoint);
        //                    float AngleLeft = Vector3.Angle(-this.FOV.RaycastChecker.transform.right, dirwithStrafepoint);
        //                    float AngleRight = Vector3.Angle(this.FOV.RaycastChecker.transform.right, dirwithStrafepoint);

        //                    if (!Components.HumanoidFiringBehaviourComponent.isreloading)
        //                    {
        //                        if (Angleforward < 60)
        //                        {
        //                            AnimController(false, MovementSpeeds.RunSpeed, AiAgentAnimatorParameters.DefaultStateParameterName, true, false);
        //                            SetAnims(AiAgentAnimatorParameters.RunParameterName);
        //                            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
        //                        }
        //                        else if (AngleBackward < 60)
        //                        {
        //                            AnimController(false, MovementSpeeds.WalkingBackwardSpeed, AiAgentAnimatorParameters.DefaultStateParameterName, true, false);
        //                            SetAnims(AiAgentAnimatorParameters.WalkBackParameterName);
        //                            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
        //                        }
        //                        else if (AngleLeft < 60)
        //                        {
        //                            AnimController(false, MovementSpeeds.WalkingLeftSpeed, AiAgentAnimatorParameters.DefaultStateParameterName, true, false);
        //                            SetAnims(AiAgentAnimatorParameters.WalkLeftParameterName);
        //                            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
        //                        }
        //                        else if (AngleRight < 60)
        //                        {
        //                            AnimController(false, MovementSpeeds.WalkingRightSpeed, AiAgentAnimatorParameters.DefaultStateParameterName, true, false);
        //                            SetAnims(AiAgentAnimatorParameters.WalkRightParameterName);
        //                            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
        //                        }
        //                    }
        //                    else
        //                    {

        //                        // Debug.Log("ME");
        //                        // AnimController(true, 0f, DefaultState, true, false);
        //                        Reload();
        //                    }
        //                }

        //                Components.NavMeshAgentComponent.SetDestination(strafePoint);
        //                SmoothAnimatorWeightChange = true;
        //                AnimLayer = 1;
        //                AnimWeight = 0f;
        //                // Debug.DrawLine(transform.position, strafePoint, Color.red);
        //            }
        //            else
        //            {
        //                //  Debug.Log("other Cover");
        //                //if (VeryNearToEnemy == true)
        //                //{
        //                //    RandomBehaviour = 0;
        //                //}

        //                //if (RandomBehaviour == 2)
        //                //{
        //                //    Debug.Log("Crouch Cover");
        //                //    CoverFireController();
        //                //}
        //                //else
        //                //{
        //                SmoothAnimatorWeightChange = true;
        //                AnimLayer = 1;
        //                AnimWeight = 0f;
        //                SetAnims(AiAgentAnimatorParameters.FireParameterName);
        //                AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);
        //                //}
        //            }

        //        }
        //        else
        //        {
        //            ShootingController();
        //        }

        //    }
        //    else
        //    {
        //        Debug.Log("Its Me");
        //        SelectStrafePoint = false;
        //        //if (RandomBehaviour == 2 || EnableCrouching == true)
        //        //{
        //        //    CoverReloadController();
        //        //}
        //        //else
        //        //{
        //        Reload();
        //        anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
        //        Components.HumanoidFiringBehaviourComponent.FireNow = false;
        //        //}
        //    }
        //    //}
        //}
        public void RandomStrafing()
        {
            StopSpineRotation = false;
            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
            LookingAtEnemy();

            //if (!Components.HumanoidFiringBehaviourComponent.isreloading)
            //{
            ShootingController();
            //}
            //else
            //{
            //    SelectStrafePoint = false;
            //    //if (RandomBehaviour == 2 || EnableCrouching == true)
            //    //{
            //    //    CoverReloadController();
            //    //}
            //    //else
            //    //{
            //    Reload();
            //    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
            //    Components.HumanoidFiringBehaviourComponent.FireNow = false;
            //    //}
            //}
        }
        //IEnumerator FindClosestCover() // Finding The Covers which are in Range
        //{
        //    if (CrouchPositions.Count > 0)
        //    {
        //        // while (FindValidCover == true)
        //        //  {
        //        int i = Random.Range(0, CrouchPositions.Count); // Comment this code For systematic finiding 
        //        LastCoverPoint = CurrentCoverPoint;
        //        if (FindEnemiesScript.FindedEnemies == true)
        //        {
        //            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
        //            {
        //                CrouchPositions[i].GetComponent<CoverNode>().CheckifEnemyIsInView(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
        //            }
        //            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
        //            {
        //                //nearestCrouchPoint = CrouchPositions[i].position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
        //                nearestCrouchPoint = CrouchPositions[i].position - transform.position;
        //            }

        //            if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.SpecificTeamCover == true && CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.TeamName
        //                                == T.FriendlyTeamTag)
        //            {
        //                if (CrouchPositions[i].transform.GetComponent<CoverNode>().IsValidCover == true && nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindCoverPoint * AiCovers.RangeToFindCoverPoint
        //                && CrouchPositions[i].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover == false)
        //                {
        //                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
        //                    {
        //                        //Vector3 DistanceBetweenCover = CrouchPositions[i].position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
        //                        //if (DistanceBetweenCover.magnitude >= 2f)
        //                        //{

        //                        //    FindValidCover = false;
        //                        //    CurrentCoverPoint = i;
        //                        //    FirstCover = true;
        //                        //}
        //                        RecheckCovers = false;
        //                        if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript != null)
        //                        {
        //                            for (int c = 0; c < CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes.Length; c++)
        //                            {
        //                                if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
        //                                    && CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().TeamUsingCover != T.FriendlyTeamTag)
        //                                {

        //                                    RecheckCovers = true;
        //                                }

        //                                if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
        //                                    && CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AiCoverScript.OnlyForOneBot == true)
        //                                {
        //                                    RecheckCovers = true;
        //                                }

        //                            }

        //                            if (CrouchPositions[i].transform.GetComponent<CoverNode>().CrouchCover == true && CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.EnableHideCoverUseForAllAi)
        //                            {
        //                                RecheckCovers = false;
        //                            }

        //                            Vector3 DistanceCheckWithEnemyAndCover = CrouchPositions[i].transform.position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
        //                            Vector3 DistanceCheckWithMeAndCover = CrouchPositions[i].transform.position - transform.position;

        //                            if (DistanceCheckWithMeAndCover.sqrMagnitude > DistanceCheckWithEnemyAndCover.sqrMagnitude)
        //                            {
        //                                Debug.Log("ENEMY IS CLOSER WITH THE COVER YOU ARE GOING TO TAKE");
        //                                Info("ENEMY IS CLOSER WITH THE COVER");
        //                                RecheckCovers = true;
        //                            }

        //                            if (RecheckCovers == false)
        //                            {
        //                                CrouchPositions[i].transform.GetComponent<CoverNode>().AdvanceBookingForCover = true;
        //                                CrouchPositions[i].transform.GetComponent<CoverNode>().TeamUsingCover = T.FriendlyTeamTag;
        //                                FindValidCover = false;
        //                                CurrentCoverPoint = i;
        //                                FirstCover = true;
        //                            }

        //                        }
        //                    }
        //                }
        //            }
        //            else if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.SpecificTeamCover == false)
        //            {
        //                if (CrouchPositions[i].transform.GetComponent<CoverNode>().IsValidCover == true && nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindCoverPoint * AiCovers.RangeToFindCoverPoint
        //               && CrouchPositions[i].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover == false)
        //                {
        //                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
        //                    {
        //                        //Vector3 DistanceBetweenCover = CrouchPositions[i].position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
        //                        //if (DistanceBetweenCover.magnitude >= 2f)
        //                        //{
        //                        //    FindValidCover = false;
        //                        //    CurrentCoverPoint = i;
        //                        //    FirstCover = true;
        //                        //}
        //                        RecheckCovers = false;
        //                        if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript != null)
        //                        {
        //                            for (int c = 0; c < CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes.Length; c++)
        //                            {
        //                                if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
        //                                    && CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().TeamUsingCover != T.FriendlyTeamTag)
        //                                {

        //                                    RecheckCovers = true;
        //                                }

        //                                if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
        //                                    && CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AiCoverScript.OnlyForOneBot == true)
        //                                {
        //                                    RecheckCovers = true;
        //                                }

        //                            }

        //                            if (CrouchPositions[i].transform.GetComponent<CoverNode>().CrouchCover == true && CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.EnableHideCoverUseForAllAi)
        //                            {
        //                                RecheckCovers = false;
        //                            }

        //                            Vector3 DistanceCheckWithEnemyAndCover = CrouchPositions[i].transform.position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
        //                            Vector3 DistanceCheckWithMeAndCover = CrouchPositions[i].transform.position - transform.position;

        //                            if (DistanceCheckWithMeAndCover.sqrMagnitude > DistanceCheckWithEnemyAndCover.sqrMagnitude)
        //                            {
        //                                Debug.Log("ENEMY IS CLOSER WITH THE COVER YOU ARE GOING TO TAKE");
        //                                Info("ENEMY IS CLOSER WITH THE COVER");
        //                                RecheckCovers = true;
        //                            }

        //                            if (RecheckCovers == false)
        //                            {
        //                                CrouchPositions[i].transform.GetComponent<CoverNode>().AdvanceBookingForCover = true;
        //                                CrouchPositions[i].transform.GetComponent<CoverNode>().TeamUsingCover = T.FriendlyTeamTag;
        //                                FindValidCover = false;
        //                                CurrentCoverPoint = i;
        //                                FirstCover = true;
        //                            }

        //                        }
        //                    }
        //                }
        //            }

        //            //if (CrouchPositions[i].transform.GetComponent<CoverNode>().ValidCover == true && i != LastCoverPoint && nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindCoverPoint * AiCovers.RangeToFindCoverPoint && CrouchPositions[i].gameObject.GetComponent<CoverNode>().TriggerOnce == false
        //            //     && CrouchPositions[i].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover == false)
        //            //{
        //            //    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
        //            //    {
        //            //        Vector3 DistanceBetweenCover = CrouchPositions[i].position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
        //            //        if (DistanceBetweenCover.magnitude >= 5f)
        //            //        {
        //            //            FindValidCover = false;
        //            //            CurrentCoverPoint = i;
        //            //            FirstCover = true;
        //            //        }
        //            //    }
        //            //}
        //        }
        //        // }
        //        // yield return null;
        //        //}

        //        if (FindValidCover == true)
        //        {
        //            StartCoroutine(TimerForStrafing());
        //            yield return new WaitForSeconds(SaveResetedCoverRandomisation);
        //            LastCoverPoint = int.MaxValue;
        //            ChangeCover = true;
        //            Reachnewpoints = false;
        //            ResetCoverRandomisation = false;
        //            StartCoroutine(FindClosestCover());
        //            //StartCoroutine(ResetCoverTime());
        //        }
        //        else
        //        {
        //            Debug.Log("Current : " + CurrentCoverPoint + "  " + "LastCoverPoint : " + LastCoverPoint);
        //            ChangeCover = true;
        //            Reachnewpoints = false;
        //            ResetCoverRandomisation = false;
        //        }
        //    }
        //}


        Transform FindClosestCombatCover()
        {
            PreviousCoverNum = CurrentCoverPoint;
            PreviousCoverNode = NewCoverNode;

            Transform closest = null;
            float closestDistance = Mathf.Infinity;
            ShouldStopLookingForCover = false;
            IsBestCoverChosen = false;

            foreach (CoverNode combatcover in CoverPoints)
            {
                float distance = Vector3.Distance(transform.position, combatcover.transform.position);
                AiCover AiCoverParent = combatcover.AiCoverScript.GetComponent<AiCover>();

                if (ShouldStopLookingForCover == false && IsBestCoverChosen == true && AiCovers.ChooseClosestCovers == false && AiCovers.SwitchCovers == true)
                {
                    ShouldStopLookingForCover = true;
                }

                // Debug.Log(combatcover.transform.parent.name + " " + distance);
                if (distance < closestDistance && ShouldStopLookingForCover == false)// && CurrentCoverPoint != PreviousCoverNum)
                {
                    DoNotSprintForNewCoverIfWithinCover();
                    //    for (int c = 0; c < AiCoverParent.coverNodes.Length; c++)
                    //    {
                    //        AiCoverParent.coverNodes[c].GetComponent<CoverNode>().CheckRotations(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);

                    //        if (AiCoverParent.coverNodes[c].GetComponent<CoverNode>().RotationIHaveToDo > highestRotation)
                    //        {
                    //            if (AiCoverParent.coverNodes[c].GetComponent<CoverNode>().IsAlreadyRegisteredCover == false || AiCoverParent.coverNodes[c].GetComponent<CoverNode>().IsAlreadyRegisteredCover == true &&
                    //                AiCoverParent.coverNodes[c].GetComponent<CoverNode>().AgentNameTakingCover == transform.name)
                    //            {
                    //                highestRotation = AiCoverParent.coverNodes[c].GetComponent<CoverNode>().RotationIHaveToDo;
                    //                BestCover = AiCoverParent.coverNodes[c].GetComponent<CoverNode>().CoverNumber;
                    //            }

                    //        }
                    //    }

                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                    {
                        combatcover.CheckifEnemyIsInView(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
                    }

                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                    {
                        nearestCrouchPoint = combatcover.transform.position - transform.position;
                    }

                    if (combatcover.AiCoverScript.SpecificTeamCover == true && combatcover.transform.GetComponent<CoverNode>().AiCoverScript.TeamName
                                        == T.MyTeamID)
                    {
                        if (combatcover.IsValidCover == true &&
                           combatcover.IsAlreadyRegisteredCover == false && nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindExistingCoverPoints * AiCovers.RangeToFindExistingCoverPoints)
                        {

                            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                            {
                                RecheckCovers = false;
                                if (combatcover.AiCoverScript != null)
                                {
                                    for (int c = 0; c < combatcover.AiCoverScript.GetComponent<AiCover>().coverNodes.Length; c++)
                                    {
                                        if (combatcover.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
                                            && combatcover.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().TeamUsingCover != T.MyTeamID)
                                        {

                                            RecheckCovers = true;
                                        }

                                        if (combatcover.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
                                            && combatcover.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AiCoverScript.SingleAgentCover == true)
                                        {
                                            RecheckCovers = true;
                                        }

                                    }

                                    if (combatcover.CrouchCover == true && combatcover.AiCoverScript.UniversalHidingCover
                                        || combatcover.StandingCover == true && combatcover.AiCoverScript.UniversalHidingCover)
                                    {
                                        RecheckCovers = false;
                                    }

                                    Vector3 DistanceCheckWithEnemyAndCover = combatcover.transform.position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                                    Vector3 DistanceCheckWithMeAndCover = combatcover.transform.position - transform.position;


                                    if (DistanceCheckWithMeAndCover.magnitude > DistanceCheckWithEnemyAndCover.magnitude)
                                    {
                                        Info("ENEMY IS CLOSER TO THE COVER");
                                        RecheckCovers = true;
                                    }

                                    Vector3 DistanceCheckWithMeAndEnemy = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - transform.position;

                                    if (DistanceCheckWithMeAndCover.magnitude > DistanceCheckWithMeAndEnemy.magnitude)
                                    {
                                        Info("ENEMY IS CLOSER TO ME");
                                        RecheckCovers = true;
                                    }

                                    if (RecheckCovers == false)
                                    {
                                        closestDistance = distance;
                                        closest = combatcover.transform;
                                       // BestCover = combatcover.CoverNumber;
                                        NewCoverNode = combatcover;
                                        IsBestCoverChosen = true;
                                        FirstCoverInitialized = true;
                                    }
                                }
                            }
                        }

                    }
                    else if (combatcover.AiCoverScript.SpecificTeamCover == false)
                    {
                        if (combatcover.IsValidCover == true
                       && combatcover.IsAlreadyRegisteredCover == false && nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindExistingCoverPoints * AiCovers.RangeToFindExistingCoverPoints)
                        {
                            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                            {
                                RecheckCovers = false;
                                if (combatcover.AiCoverScript != null)
                                {
                                    for (int c = 0; c < combatcover.AiCoverScript.GetComponent<AiCover>().coverNodes.Length; c++)
                                    {
                                        if (combatcover.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
                                            && combatcover.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().TeamUsingCover != T.MyTeamID)
                                        {

                                            RecheckCovers = true;
                                        }

                                        if (combatcover.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
                                            && combatcover.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AiCoverScript.SingleAgentCover == true)
                                        {
                                            RecheckCovers = true;
                                        }
                                    }

                                    if (combatcover.CrouchCover == true && combatcover.AiCoverScript.UniversalHidingCover ||
                                        combatcover.StandingCover == true && combatcover.AiCoverScript.UniversalHidingCover)
                                    {
                                        RecheckCovers = false;
                                    }

                                    Vector3 DistanceCheckWithEnemyAndCover = combatcover.transform.position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                                    Vector3 DistanceCheckWithMeAndCover = combatcover.transform.position - transform.position;

                                    if (DistanceCheckWithMeAndCover.magnitude > DistanceCheckWithEnemyAndCover.magnitude)
                                    {
                                        Info("ENEMY IS CLOSER TO THE COVER");
                                        RecheckCovers = true;
                                    }

                                    Vector3 DistanceCheckWithMeAndEnemy = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - transform.position;

                                    if (DistanceCheckWithMeAndCover.magnitude > DistanceCheckWithMeAndEnemy.magnitude)
                                    {
                                        Info("ENEMY IS CLOSER TO ME");
                                        RecheckCovers = true;
                                    }

                                    if (RecheckCovers == false)
                                    {
                                        closestDistance = distance;
                                        closest = combatcover.transform;
                                        //BestCover = combatcover.CoverNumber;
                                        NewCoverNode = combatcover;
                                        IsBestCoverChosen = true;
                                        FirstCoverInitialized = true;
                                       // Debug.Log(NewCoverNode.transform.parent.name);
                                    }

                                }

                            }
                        }
                    }
                }
            }

            if (FirstCoverInitialized == true)
            {
                ChooseBestCombatCover();
                CombatCoverPicking();
            }

            return closest;
        }
        IEnumerator FindClosestCoverSystematically() // Finding The Covers which are in Range  
        {

            // The line from 6179 to 6180 on 7 oct 2023 and the reason was if you uncomment this line this creates a little gap in finding the cover due to which Checktime variable call another coroutine in line
            // 7487 which basically makes the Ai agent continously shift covers and never stay in any cover ( in short duplicates the Reset cover timer twice everytime )
            // Even if the min and max time between cover is same Ai agent will still find the cover at unique time basically depending upon when they reach the selected cover after which the timer to find the next cover starts
            //float Randomise = Random.Range(0f, 0.1f);
            //yield return new WaitForSeconds(Randomise);


            //if (AiCovers.ChooseClosestCovers == true || AiCovers.SwitchCovers == false)
            //{

            int ShouldChangeCover = Random.Range(0, 100);

            if(AiCovers.SwitchingCoversProbability <= ShouldChangeCover)
            {
                AiCovers.SwitchCovers = false;
            }
            else
            {
                AiCovers.SwitchCovers = true;
            }



            if (AiCovers.ChooseClosestCovers == true || AiCovers.SwitchCovers == false)
            {
                System.Array.Sort(CoverPoints, (enemy1, enemy2) =>
                 Vector3.Distance(transform.position, enemy1.transform.position)
                 .CompareTo(Vector3.Distance(transform.position, enemy2.transform.position))
                 );
            }
            else
            {
                //System.Array.Sort(CoverPoints, (enemy1, enemy2) =>
                //Random.Range(0, 2) * 2 - 1 // Generate random -1 or 1 for sorting
                //);

                // Assuming CoverPoints is an array of GameObjects
                int n = CoverPoints.Length;
                while (n > 1)
                {
                    n--;
                    int k = Random.Range(0, n + 1);
                    GameObject temp = CoverPoints[k].gameObject;
                    CoverPoints[k] = CoverPoints[n];
                    CoverPoints[n] = temp.GetComponent<CoverNode>();
                }

            }


            FindClosestCombatCover();

            //else 
            //{
            //    Info("Finding Cover");
            //    //        Debug.Log("Yes Its true" + transform.name);
            //    if (CrouchPositions.Count > 0 && DoNotLookForCover == false && HealthScript.IsDied == false)
            //    {
            //        int i  = Random.Range(0, CrouchPositions.Count);
            //        //if (CurrentCoverPoint <= CrouchPositions.Count)
            //        //{
            //        //    if (AiCovers.SwitchBetweenCovers == false)
            //        //    {
            //        //        i = CurrentCoverPoint;
            //        //        NoNeedToFindNewCover = true;
            //        //    }
            //        //}

            //        if (FindEnemiesScript.FindedEnemies == true)
            //        {
            //            //ResetHighestRotation = false;
            //            //if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().OnlyForOneBot == true)
            //            //{
            //            highestRotation = 0f;
            //            for (int c = 0; c < CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes.Length; c++)
            //            {
            //                CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                   GetComponent<CoverNode>().CheckRotations(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);

            //                if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                   GetComponent<CoverNode>().RotationIHaveToDo > highestRotation)
            //                {
            //                    if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                   GetComponent<CoverNode>().IsAlreadyRegisteredCover == false ||
            //                      CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                   GetComponent<CoverNode>().IsAlreadyRegisteredCover == true &&
            //                   CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                   GetComponent<CoverNode>().AgentNameTakingCover == transform.name)
            //                    {
            //                        highestRotation = CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                       GetComponent<CoverNode>().RotationIHaveToDo;
            //                        BestCover = CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                       GetComponent<CoverNode>().CoverNumber;
            //                        NewCoverNode = CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                       GetComponent<CoverNode>();
            //                    }

            //                }
            //            }


            //            //if (CrouchPositions[i].GetComponent<CoverNode>().IsCoverInUse == false)
            //            //{
            //            //    CrouchPositions[i].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover = false;
            //            //}
            //            //}
            //            //else
            //            //{
            //            //    for (int c = 0; c < CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes.Length; c++)
            //            //    {
            //            //        CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //            //           GetComponent<CoverNode>().CheckRotations(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);

            //            //        if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //            //           GetComponent<CoverNode>().RotationIHaveToDo > highestRotation)
            //            //        {
            //            //            if (CrouchPositions[i].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover == false)
            //            //            {
            //            //                highestRotation = CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //            //                GetComponent<CoverNode>().RotationIHaveToDo;
            //            //            }
            //            //            else
            //            //            {
            //            //                ResetHighestRotation = true;
            //            //            }
            //            //        }
            //            //    }

            //            //    if (ResetHighestRotation == true)
            //            //    {
            //            //        highestRotation = 0f;
            //            //    }
            //            //}

            //            ChooseBestCombatCover();

            //            //if (CrouchPositions[i].transform.GetComponent<CoverNode>().ValidCover == true && i != LastCoverPoint && nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindCoverPoint * AiCovers.RangeToFindCoverPoint && CrouchPositions[i].gameObject.GetComponent<CoverNode>().TriggerOnce == false
            //            //    && CrouchPositions[i].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover == false)
            //            //{
            //            //    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            //            //    {
            //            //        Vector3 DistanceBetweenCover = CrouchPositions[i].position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
            //            //        if(DistanceBetweenCover.magnitude >= 5f)
            //            //        {
            //            //            FindValidCover = false;
            //            //            CurrentCoverPoint = i;
            //            //            FirstCover = true;
            //            //        }
            //            //    }                   
            //            //}

            //        }
            //        //}
            //        // yield return null;
            //        //}
            //    }

            //    // This Code will allow the Ai agent to move to the same cover in case did not found a random nearest cover point 
            //    if (CrouchPositions.Count > 0 && DoNotLookForCover == false && HealthScript.IsDied == false && NoNeedToFindNewCover == false)
            //    {
            //        if (FindValidCover == true)
            //        {
            //            for (int i = 0; i < CrouchPositions.Count; i++)  // Uncomment this code For Systematic Cover Finding  
            //            {
            //                if (CrouchPositions.Count > 0)
            //                {
            //                    if (FindValidCover == true)
            //                    {

            //                        ResetHighestRotation = false;
            //                        //if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().OnlyForOneBot == true)
            //                        //{
            //                        highestRotation = 0f;
            //                        for (int c = 0; c < CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes.Length; c++)
            //                        {
            //                            CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                               GetComponent<CoverNode>().CheckRotations(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);

            //                            if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                               GetComponent<CoverNode>().RotationIHaveToDo > highestRotation)
            //                            {
            //                                if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                               GetComponent<CoverNode>().IsAlreadyRegisteredCover == false ||
            //                                  CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                               GetComponent<CoverNode>().IsAlreadyRegisteredCover == true &&
            //                               CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                               GetComponent<CoverNode>().AgentNameTakingCover == transform.name)
            //                                {
            //                                    highestRotation = CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                                   GetComponent<CoverNode>().RotationIHaveToDo;
            //                                    BestCover = CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                                   GetComponent<CoverNode>().CoverNumber;
            //                                    NewCoverNode = CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                                   GetComponent<CoverNode>();
            //                                }

            //                            }
            //                        }

            //                        //    if (CrouchPositions[i].GetComponent<CoverNode>().IsCoverInUse == false)
            //                        //    {
            //                        //        CrouchPositions[i].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover = false;
            //                        //    }
            //                        //}
            //                        //else
            //                        //{
            //                        //    for (int c = 0; c < CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes.Length; c++)
            //                        //    {
            //                        //        CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                        //           GetComponent<CoverNode>().CheckRotations(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);

            //                        //        if (CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                        //           GetComponent<CoverNode>().RotationIHaveToDo > highestRotation)
            //                        //        {
            //                        //            if (CrouchPositions[i].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover == false)
            //                        //            {
            //                        //                highestRotation = CrouchPositions[i].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].
            //                        //                GetComponent<CoverNode>().RotationIHaveToDo;
            //                        //            }
            //                        //            else
            //                        //            {
            //                        //                ResetHighestRotation = true;
            //                        //            }
            //                        //        }
            //                        //    }

            //                        //    if (ResetHighestRotation == true)
            //                        //    {
            //                        //        highestRotation = 0f;
            //                        //    }
            //                        //}

            //                        ChooseBestCombatCover();

            //                        //if (FindEnemiesScript.FindedEnemies == true)
            //                        //{
            //                        //    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            //                        //    {
            //                        //        CrouchPositions[BestCover].GetComponent<CoverNode>().CheckifEnemyIsInView(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
            //                        //    }
            //                        //    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            //                        //    {
            //                        //        //nearestCrouchPoint = CrouchPositions[i].position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
            //                        //        nearestCrouchPoint = CrouchPositions[BestCover].position - transform.position;
            //                        //    }

            //                        //    if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.SpecificTeamCover == true && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.TeamName
            //                        //         == T.FriendlyTeamTag)
            //                        //    {
            //                        //        if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().IsValidCover == true && nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindCoverPoint * AiCovers.RangeToFindCoverPoint
            //                        //        && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover == false && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().RotationIHaveToDo >= highestRotation)
            //                        //        {
            //                        //            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            //                        //            {
            //                        //                //Vector3 DistanceBetweenCover = CrouchPositions[i].position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
            //                        //                //if (DistanceBetweenCover.magnitude >= 2f)
            //                        //                //{
            //                        //                //    FindValidCover = false;
            //                        //                //    CurrentCoverPoint = i;
            //                        //                //    FirstCover = true;
            //                        //                //}
            //                        //                RecheckCovers = false;
            //                        //                if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript != null)
            //                        //                {
            //                        //                    for (int c = 0; c < CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes.Length; c++)
            //                        //                    {
            //                        //                        if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
            //                        //                            && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().TeamUsingCover != T.FriendlyTeamTag)
            //                        //                        {

            //                        //                            RecheckCovers = true;
            //                        //                        }

            //                        //                        if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
            //                        //            && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AiCoverScript.SingleAgentCover == true)
            //                        //                        {
            //                        //                            RecheckCovers = true;
            //                        //                        }
            //                        //                    }

            //                        //                    if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().CrouchCover == true && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.UniversalCrouchCover)
            //                        //                    {
            //                        //                        RecheckCovers = false;
            //                        //                    }

            //                        //                    Vector3 DistanceCheckWithEnemyAndCover = CrouchPositions[BestCover].transform.position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
            //                        //                    Vector3 DistanceCheckWithMeAndCover = CrouchPositions[BestCover].transform.position - transform.position;

            //                        //                    if (DistanceCheckWithMeAndCover.sqrMagnitude > DistanceCheckWithEnemyAndCover.sqrMagnitude)
            //                        //                    {
            //                        //                        Debug.Log("ENEMY IS CLOSER WITH THE COVER YOU ARE GOING TO TAKE");
            //                        //                        Info("ENEMY IS CLOSER WITH THE COVER");
            //                        //                        RecheckCovers = true;
            //                        //                    }

            //                        //                    if (RecheckCovers == false)
            //                        //                    {
            //                        //                        CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AdvanceBookingForCover = true;
            //                        //                        CrouchPositions[BestCover].transform.GetComponent<CoverNode>().TeamUsingCover = T.FriendlyTeamTag;
            //                        //                        FindValidCover = false;
            //                        //                        CurrentCoverPoint = BestCover;
            //                        //                        FirstCover = true;
            //                        //                    }

            //                        //                }
            //                        //            }
            //                        //        }
            //                        //        //else if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().IsValidCover == true
            //                        //        //         && nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindCoverPoint * AiCovers.RangeToFindCoverPoint
            //                        //        //         && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover == true && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().RotationIHaveToDo >= highestRotation)
            //                        //        //{
            //                        //        //    CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AdvanceBookingForCover = true;
            //                        //        //    CrouchPositions[BestCover].transform.GetComponent<CoverNode>().TeamUsingCover = T.FriendlyTeamTag;
            //                        //        //    FindValidCover = false;
            //                        //        //    CurrentCoverPoint = BestCover;
            //                        //        //    FirstCover = true;
            //                        //        //}
            //                        //    }
            //                        //    else if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.SpecificTeamCover == false)
            //                        //    {
            //                        //        if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().IsValidCover == true && nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindCoverPoint * AiCovers.RangeToFindCoverPoint
            //                        //       && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover == false && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().RotationIHaveToDo >= highestRotation)
            //                        //        {
            //                        //            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            //                        //            {
            //                        //                //Vector3 DistanceBetweenCover = CrouchPositions[i].position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
            //                        //                //if (DistanceBetweenCover.magnitude >= 2f)
            //                        //                //{
            //                        //                //    FindValidCover = false;
            //                        //                //    CurrentCoverPoint = i;
            //                        //                //    FirstCover = true;
            //                        //                //}
            //                        //                RecheckCovers = false;
            //                        //                if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript != null)
            //                        //                {
            //                        //                    for (int c = 0; c < CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes.Length; c++)
            //                        //                    {
            //                        //                        if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
            //                        //                            && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().TeamUsingCover != T.FriendlyTeamTag)
            //                        //                        {
            //                        //                            RecheckCovers = true;
            //                        //                        }

            //                        //                        if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
            //                        //            && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AiCoverScript.SingleAgentCover == true)
            //                        //                        {
            //                        //                            RecheckCovers = true;
            //                        //                        }

            //                        //                    }

            //                        //                    if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().CrouchCover == true && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AiCoverScript.UniversalCrouchCover)
            //                        //                    {
            //                        //                        RecheckCovers = false;
            //                        //                    }

            //                        //                    Vector3 DistanceCheckWithEnemyAndCover = CrouchPositions[BestCover].transform.position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
            //                        //                    Vector3 DistanceCheckWithMeAndCover = CrouchPositions[BestCover].transform.position - transform.position;

            //                        //                    if (DistanceCheckWithMeAndCover.sqrMagnitude > DistanceCheckWithEnemyAndCover.sqrMagnitude)
            //                        //                    {
            //                        //                        Debug.Log("ENEMY IS CLOSER WITH THE COVER YOU ARE GOING TO TAKE");
            //                        //                        Info("ENEMY IS CLOSER WITH THE COVER");
            //                        //                        RecheckCovers = true;
            //                        //                    }

            //                        //                    if (RecheckCovers == false)
            //                        //                    {
            //                        //                        CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AdvanceBookingForCover = true;
            //                        //                        CrouchPositions[BestCover].transform.GetComponent<CoverNode>().TeamUsingCover = T.FriendlyTeamTag;
            //                        //                        FindValidCover = false;
            //                        //                        CurrentCoverPoint = BestCover;
            //                        //                        FirstCover = true;
            //                        //                    }

            //                        //                }

            //                        //            }
            //                        //        }
            //                        //        //else if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().IsValidCover == true
            //                        //        //         && nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindCoverPoint * AiCovers.RangeToFindCoverPoint
            //                        //        //         && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover == true && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().RotationIHaveToDo >= highestRotation)
            //                        //        //{
            //                        //        //    CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AdvanceBookingForCover = true;
            //                        //        //    CrouchPositions[BestCover].transform.GetComponent<CoverNode>().TeamUsingCover = T.FriendlyTeamTag;
            //                        //        //    FindValidCover = false;
            //                        //        //    CurrentCoverPoint = BestCover;
            //                        //        //    FirstCover = true;
            //                        //        //}
            //                        //    }
            //                        //}
            //                    }
            //                }
            //            }
            //        }


            //        // Use This If you want the Ai to take covers like 0,1,2,3 in a sequence 
            //        //if (CrouchPositions.Count > 0)
            //        //{

            //        //    if (FindValidCover == true)
            //        //    {
            //        //        for (int i = 0; i < CrouchPositions.Count; i++)  // Uncomment this code For Systematic Cover Finding 
            //        //        {
            //        //            if (CrouchPositions.Count > 0)
            //        //            {
            //        //                if (FindValidCover == true)
            //        //                {
            //        //                    // if (CrouchPositionFinded == true)
            //        //                    //{
            //        //                    LastCoverPoint = CurrentCoverPoint;
            //        //                    if (FindEnemiesScript.FindedEnemies == true)
            //        //                    {
            //        //                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            //        //                        {
            //        //                            CrouchPositions[i].GetComponent<CoverNode>().CheckifEnemyIsInView(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
            //        //                        }
            //        //                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            //        //                        {
            //        //                            nearestCrouchPoint = CrouchPositions[i].position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
            //        //                        }

            //        //                        if (CrouchPositions[i].transform.GetComponent<CoverNode>().ValidCover == true && i != LastCoverPoint && nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindCoverPoint * AiCovers.RangeToFindCoverPoint && CrouchPositions[i].gameObject.GetComponent<CoverNode>().TriggerOnce == false
            //        //                             && CrouchPositions[i].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover == false)
            //        //                        {
            //        //                            // if()
            //        //                            FindValidCover = false;
            //        //                            CurrentCoverPoint = i;
            //        //                            FirstCover = true;
            //        //                        }

            //        //                    }
            //        //                    // }
            //        //                }
            //        //            }
            //        //        }
            //        //    }

            //    }

            //    CombatCoverPicking();
            //}

            FirstFindingsCompleted = true;
            yield return null;

        }
        public void DoNotSprintForNewCoverIfWithinCover()
        {
            if (NewCoverNode != null)
            {
                if (AiCovers.SwitchCovers == false)
                {
                    if (PreviousCoverNode != null)
                    { 
                        PreviousCoverNode.IsAlreadyRegisteredCover = false;
                        PreviousCoverNode.AdvanceBookingForCover = false;
                        PreviousCoverNode.DistanceCleared = false;
                    }
                    ChangeCover = true;
                    ReachnewCoverpoint = false;
                    CheckTime = false;
                }
            }
        }
        public void ChooseBestCombatCover()
        {
            if (CrouchPositions.Count >= 1)
            {
                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                {
                    NewCoverNode.CheckifEnemyIsInView(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
                }
                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                {
                    //nearestCrouchPoint = CrouchPositions[i].position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                    nearestCrouchPoint = NewCoverNode.transform.position - transform.position;
                }

                if (NewCoverNode.AiCoverScript.SpecificTeamCover == true && NewCoverNode.transform.GetComponent<CoverNode>().AiCoverScript.TeamName
                                    == T.MyTeamID)
                {
                    if (NewCoverNode.GetComponent<CoverNode>().IsValidCover == true &&
                        nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindExistingCoverPoints * AiCovers.RangeToFindExistingCoverPoints
                    && NewCoverNode.IsAlreadyRegisteredCover == false && NewCoverNode.RotationIHaveToDo >= highestRotation)
                    {

                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                        {
                            //Vector3 DistanceBetweenCover = CrouchPositions[i].position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                            //if (DistanceBetweenCover.magnitude >= 2f)
                            //{
                            //    FindValidCover = false;
                            //    CurrentCoverPoint = i;
                            //    FirstCover = true;
                            //}
                            RecheckCovers = false;
                            if (NewCoverNode.AiCoverScript != null)
                            {
                                for (int c = 0; c < NewCoverNode.AiCoverScript.GetComponent<AiCover>().coverNodes.Length; c++)
                                {
                                    if (NewCoverNode.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
                                        && NewCoverNode.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().TeamUsingCover != T.MyTeamID)
                                    {

                                        RecheckCovers = true;
                                    }

                                    if (NewCoverNode.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
                                        && NewCoverNode.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AiCoverScript.SingleAgentCover == true)
                                    {
                                        RecheckCovers = true;
                                    }

                                }

                                if (NewCoverNode.CrouchCover == true && NewCoverNode.AiCoverScript.UniversalHidingCover
                                    || NewCoverNode.StandingCover == true && NewCoverNode.AiCoverScript.UniversalHidingCover)
                                {
                                    RecheckCovers = false;
                                }

                                Vector3 DistanceCheckWithEnemyAndCover = NewCoverNode.transform.position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                                Vector3 DistanceCheckWithMeAndCover = NewCoverNode.transform.position - transform.position;

                                if (DistanceCheckWithMeAndCover.magnitude > DistanceCheckWithEnemyAndCover.magnitude)
                                {
                                    Info("ENEMY IS CLOSER TO THE COVER");
                                    RecheckCovers = true;
                                }

                                Vector3 DistanceCheckWithMeAndEnemy = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - transform.position;

                                if (DistanceCheckWithMeAndCover.magnitude > DistanceCheckWithMeAndEnemy.magnitude)
                                {
                                    Info("ENEMY IS CLOSER TO ME");
                                    RecheckCovers = true;
                                }

                                if (RecheckCovers == false)
                                {
                                    NewCoverNode.AdvanceBookingForCover = true;
                                    NewCoverNode.TeamUsingCover = T.MyTeamID;
                                    FindValidCover = false;
                                    for (int c = 0; c < NewCoverNode.AiCoverScript.GetComponent<AiCover>().coverNodes.Length; c++)
                                    {
                                        if (NewCoverNode.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform == NewCoverNode.transform)
                                        {
                                            CurrentCoverPoint = c;
                                        }
                                    }
                                    //CurrentCoverPoint = BestCover;
                                    FirstCover = true;
                                    // LastCoverRoot = CurrentCoverPoint; // && LastCoverRoot != CurrentCoverPoint
                                }




                            }

                        }
                    }
                    //else if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().IsValidCover == true
                    //    && nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindCoverPoint * AiCovers.RangeToFindCoverPoint
                    //    && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover == true && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().RotationIHaveToDo >= highestRotation)
                    //{
                    //    CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AdvanceBookingForCover = true;
                    //    CrouchPositions[BestCover].transform.GetComponent<CoverNode>().TeamUsingCover = T.FriendlyTeamTag;
                    //    FindValidCover = false;
                    //    CurrentCoverPoint = BestCover;
                    //    FirstCover = true;
                    //}
                }
                else if (NewCoverNode.AiCoverScript.SpecificTeamCover == false)
                {
                    if (NewCoverNode.IsValidCover == true && nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindExistingCoverPoints * AiCovers.RangeToFindExistingCoverPoints
                   && NewCoverNode.IsAlreadyRegisteredCover == false && NewCoverNode.RotationIHaveToDo >= highestRotation)
                    {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                        {
                            //Vector3 DistanceBetweenCover = CrouchPositions[i].position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                            //if (DistanceBetweenCover.magnitude >= 2f)
                            //{
                            //    FindValidCover = false;
                            //    CurrentCoverPoint = i;
                            //    FirstCover = true;
                            //}
                            RecheckCovers = false;
                            if (NewCoverNode.AiCoverScript != null)
                            {
                                for (int c = 0; c < NewCoverNode.AiCoverScript.GetComponent<AiCover>().coverNodes.Length; c++)
                                {
                                    if (NewCoverNode.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
                                        && NewCoverNode.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().TeamUsingCover != T.MyTeamID)
                                    {

                                        RecheckCovers = true;
                                    }

                                    if (NewCoverNode.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AdvanceBookingForCover == true
                                        && NewCoverNode.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform.GetComponent<CoverNode>().AiCoverScript.SingleAgentCover == true)
                                    {
                                        RecheckCovers = true;
                                    }
                                }

                                if (NewCoverNode.CrouchCover == true && NewCoverNode.AiCoverScript.UniversalHidingCover
                                    || NewCoverNode.StandingCover == true && NewCoverNode.AiCoverScript.UniversalHidingCover)
                                {
                                    RecheckCovers = false;
                                }

                                Vector3 DistanceCheckWithEnemyAndCover = NewCoverNode.transform.position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                                Vector3 DistanceCheckWithMeAndCover = NewCoverNode.transform.position - transform.position;

                                if (DistanceCheckWithMeAndCover.magnitude > DistanceCheckWithEnemyAndCover.magnitude)
                                {
                                    Info("ENEMY IS CLOSER TO THE COVER");
                                    RecheckCovers = true;
                                }

                                Vector3 DistanceCheckWithMeAndEnemy = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - transform.position;

                                if (DistanceCheckWithMeAndCover.magnitude > DistanceCheckWithMeAndEnemy.magnitude)
                                {
                                    Info("ENEMY IS CLOSER TO ME");
                                    RecheckCovers = true;
                                }

                                if (RecheckCovers == false)
                                {
                                    NewCoverNode.AdvanceBookingForCover = true;
                                    NewCoverNode.TeamUsingCover = T.MyTeamID;
                                    FindValidCover = false;
                                    for (int c = 0; c < NewCoverNode.AiCoverScript.GetComponent<AiCover>().coverNodes.Length; c++)
                                    {
                                        if (NewCoverNode.AiCoverScript.GetComponent<AiCover>().coverNodes[c].transform == NewCoverNode.transform)
                                        {
                                            CurrentCoverPoint = c;
                                        }
                                    }
                                    //CurrentCoverPoint = BestCover;
                                    FirstCover = true;
                                }

                            }

                        }
                    }
                    //else if (CrouchPositions[BestCover].transform.GetComponent<CoverNode>().IsValidCover == true
                    //   && nearestCrouchPoint.sqrMagnitude < AiCovers.RangeToFindCoverPoint * AiCovers.RangeToFindCoverPoint
                    //   && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().IsAlreadyRegisteredCover == true && CrouchPositions[BestCover].transform.GetComponent<CoverNode>().RotationIHaveToDo >= highestRotation)
                    //{
                    //    CrouchPositions[BestCover].transform.GetComponent<CoverNode>().AdvanceBookingForCover = true;
                    //    CrouchPositions[BestCover].transform.GetComponent<CoverNode>().TeamUsingCover = T.FriendlyTeamTag;
                    //    FindValidCover = false;
                    //    CurrentCoverPoint = BestCover;
                    //    FirstCover = true;
                    //}
                }
            }

        }
        public void CombatCoverPicking()
        {
            if (CrouchPositions.Count >= 1)
            {
                if (FindValidCover == false)
                {
                    //DebugInfo.DebugCoverNumberToTake = CurrentCoverPoint;
                    RegisterCover();

                    ChangeCover = true;
                    ReachnewCoverpoint = false;
                    //ResetCoverRandomisation = false;
                    NewCoverNode.DistanceCleared = false;
                    Vector3 targetPosition = NewCoverNode.transform.position;
                    DistanceWithCoverAfterSearch = new Vector3(targetPosition.x - transform.position.x, 0f, targetPosition.z - transform.position.z);

                    NewCoverNode.AgentNameTakingCover = transform.name;


                }
                //else
                //{
                //    // StartCoroutine(TimerForStrafing());
                //    //yield return new WaitForSeconds(SaveResetedCoverRandomisation);
                //    //LastCoverPoint = 998;
                //    //ChangeCover = true;
                //    //Reachnewpoints = false;
                //    //ResetCoverRandomisation = false;
                //    //StartCoroutine(FindClosestCoverSystematically());
                //}

                if (PreviousCoverNode != null)
                {
                    PreviousCoverNode.DeactivateOccupiedText();
                }

                if (PreviousCoverNode != null && PreviousCoverNum != CurrentCoverPoint)
                {
                    if (PreviousCoverNum <= CrouchPositions.Count)
                    {
                        PreviousCoverNode.IsAlreadyRegisteredCover = false;
                        PreviousCoverNode.AdvanceBookingForCover = false;
                        PreviousCoverNode.DistanceCleared = false;
                    }
                    ChangeCover = true;
                   // Debug.Log("first if statement");
                    ReachnewCoverpoint = false;
                    // ResetCoverRandomisation = false;
                    CheckTime = false;

                }
                else
                {
                    if (NewCoverNode.DistanceCleared == false)
                    {
                        Debug.Log("Picking Cover");
                        PreviousDestinationWhenRunning = Vector3.positiveInfinity;
                        CanTakeCover = true;
                        FirstCover = true;
                        ReachnewCoverpoint = false;
                      //  Debug.Log("else statement first");
                    }
                    else
                    {
                      //  Debug.Log("else statement last");
                        ReachnewCoverpoint = true;
                    }
                    ChangeCover = false;
                    //   ResetCoverRandomisation = false;
                    CheckTime = false;
                }
            }
        }


        IEnumerator ResetCoverTime()
        {
            IsAnyTaskCurrentlyRunning = true;
            float SaveResetedCoverRandomisation = Random.Range(AiCovers.MinimumTimeBetweenCovers, AiCovers.MaximumTimeBetweenCovers);

            if (NewCoverNode.StandingCover == true)
            {
                Components.NavMeshAgentComponent.radius = NavMeshAgentSettings.AgentRadiusDuringStandCover;
            }
            else if (NewCoverNode.CrouchCover == true)
            {
                Components.NavMeshAgentComponent.radius = NavMeshAgentSettings.AgentRadiusDuringCrouchCover;
            }
            else if (NewCoverNode.CrouchFiringCoverNode == true)
            {
                Components.NavMeshAgentComponent.radius = NavMeshAgentSettings.AgentRadiusDuringCrouchFiringCover;
            }
            else if (NewCoverNode.StandFiringCover == true)
            {
                Components.NavMeshAgentComponent.radius = NavMeshAgentSettings.AgentRadiusDuringStandFiringCover;
            }

            ChangeCover = false;
            Info("Searching for cover");
            //Debug.Log("MySaveResetTimer" + SaveResetedCoverRandomisation); 
            yield return new WaitForSeconds(SaveResetedCoverRandomisation);
            Info("cover searched");
            //if(AiCovers.SwitchBetweenCovers == true)
            //{
            //PreviousCoverNum = CurrentCoverPoint;
            ////}
            //PreviousCoverNode = NewCoverNode;

            //if (CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().RandomStayOrFindNewCover == true)
            //{
            //    StayOrMove = Random.Range(0, 2);
            //}
            if (NewCoverNode.UseOpenFirePoints == true && ReachnewCoverpoint == true) //|| 
                                                                                      //CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().RandomStayOrFindNewCover == true &&
                                                                                      //CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().UseOpenFirePoints == true && StayOrMove == 1 && ReachnewCoverpoint == true)
            {
                MoveTowardsOpenFirePoint = true;
            }
            else
            {
                // CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().IsAlreadyRegisteredCover = false;
                // CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().AdvanceBookingForCover = false;

                CanTakeCover = false;
                // ReachnewCoverpoint = false;

                FindValidCover = true;
                if (DeregisterCoverNodes == false)
                {
                    StartCoroutine(FindClosestCoverSystematically());
                }
                //if (CurrentCoverPoint == LastCoverPoint)
                //{
                //   // CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().IsAlreadyRegisteredCover = false; // newly added on 29/06/23
                //    StartCoroutine(FindClosestCoverSystematically());
                //}
                //ChangeCover = true;
                //Reachnewpoints = false;
                //ResetCoverRandomisation = false;
                //CheckTime = false;
                IsTaskOver = true;
                // StopMovingBetweenCovers = false;
                // StopBotForShoot = false;
            }
            RandomSprintingDistanceBetweenCovers = Random.Range(AiCovers.MinSprintingDistance, AiCovers.MaxSprintingDistance);
            Components.NavMeshAgentComponent.radius = DefaultAgentRadius;
        }
        IEnumerator CoverChecksToDoIfNoCoverAvailable()
        {
            yield return new WaitForSeconds(TimeToCheckForCoverIfNoCoverAvailable);
            //if (CurrentCoverPoint > CrouchPositions.Count)
            //{
            CanTakeCover = false;
           // Debug.Log("I AM RESPONSIBLE");
            FindValidCover = true;
            if (DeregisterCoverNodes == false)
            {
                StartCoroutine(FindClosestCoverSystematically());
            }
            CheckingForCoverAvailability = false;
            //}
        }
        //IEnumerator TimerForStrafing()
        //{
        //    float Timer = 0;
        //    SelectStrafePoint = false;
        //    CanStrafe = true;
        //    if (CombatStateBehaviours.EnableStrafing == true)
        //    {
        //        while (SaveResetedCoverRandomisation > Timer)
        //        {
        //            RandomStrafing();
        //            Timer += Time.deltaTime;
        //            yield return null;
        //        }
        //    }

        //}
        public void RegisterCover()
        {
            if (NewCoverNode.IsValidCover == true
                   || NewCoverNode.IsValidCover == true && FirstCover == true)
            {
                if (NewCoverNode.IsAlreadyRegisteredCover == false) // NEW CHANGE   && ChangeCover == true && Reachnewpoints == false
                {

                    NewCoverNode.IsAlreadyRegisteredCover = true;
                    if (T != null)
                    {
                        NewCoverNode.TeamUsingCover = T.MyTeamID;
                    }
                    CanTakeCover = true;
                }
            }
        }
        public void DeregisterCover()
        {
            if (DeregisterCoverNodes == false)
            {
                if (CrouchPositions.Count >= 1)
                {
                    if (NewCoverNode != null)
                    {
                        if (NewCoverNode.IsAlreadyRegisteredCover == true
                          && NewCoverNode.AgentNameTakingCover
                           == transform.name)
                        {
                            NewCoverNode.DeactivateOccupiedText();
                            NewCoverNode.DistanceCleared = false;
                            NewCoverNode.IsAlreadyRegisteredCover = false;
                            NewCoverNode.AdvanceBookingForCover = false;
                        }
                    }
                }
                DeregisterCoverNodes = true;
            }

        }
        public void DeregisterEmergencyCover()
        {
            if (DeregisterEmergencyNodes == false)
            {
                if (AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint.Length >= 1)
                {
                    if (PreviousEmergencyCoverNode != null)
                    {
                        if(PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                        {
                            if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().IsAlreadyRegistered == true)
                            {
                                PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().IsAlreadyRegistered = false;
                            }

                            if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                            {
                                PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().DeactivateOccupiedText();
                            }
                        }
                       
                    }
                }

                DeregisterEmergencyNodes = true;
            }

        }
        public void AiCoverMethod()
        {
            //if (nearestCrouchPoint.sqrMagnitude < combatRange * combatRange && nearestCrouchPoint.sqrMagnitude > 3 * 3)
            //{
            if (StartStrafingForAvoidingBots == false && BotMovingAwayFromGrenade == false)
            {
                if (ReachnewCoverpoint == false)
                {
                    // MoveTowardsOpenFirePoint = false;

                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                    {
                        NewCoverNode.CheckifEnemyIsInView(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
                    }

                    //if (CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().TriggerOnce == true)
                    //{
                    //    FindingNewCrouchPoint = false;  
                    //    FindImmediateCoverPoint();
                    //    Fire();
                    //}

                    //RegisterCover();

                    if (NewCoverNode.IsValidCover == true && CanTakeCover == true && NewCoverNode.AdvanceBookingForCover == true
                    || NewCoverNode.IsValidCover == true && FirstCover == true && CanTakeCover == true && NewCoverNode.AdvanceBookingForCover == true) // This Advance booking added on 25/05/2023
                    {
                        if (StopBotForShoot == false)
                        {
                            Vector3 GetDistanceToCover = NewCoverNode.transform.position - transform.position;
                            if (GetDistanceToCover.magnitude <= AiCovers.TakenCoverProximityDistance)
                            {
                                // Added this line newly on 14th jun 2024 because if not added than when
                                // the switching probability is set to be 0 the humanoid AI agent is deregistering the same cover again and again
                                // due to which the navmesh obstacle gets activate and deactivate again and again.
                                StopMovingBetweenCovers = false;
                                NewCoverNode.DistanceCleared = true;
                            }
                            else
                            {
                                pathfinder.FindClosestPointTowardsDestination(NewCoverNode.transform.position);

                                if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                                {
                                    horizontalDistanceCheckerForCoverBehaviour = new Vector3(pathfinder.closestPoint.x - transform.position.x, 0f, pathfinder.closestPoint.z - transform.position.z);
                                    NewCoverNode.DebugDistanceWithAiAgent = horizontalDistanceCheckerForCoverBehaviour.magnitude;
                                }
                                else
                                {
                                    horizontalDistanceCheckerForCoverBehaviour = new Vector3(NewCoverNode.transform.position.x - transform.position.x, 0f, NewCoverNode.transform.position.z - transform.position.z);
                                    NewCoverNode.DebugDistanceWithAiAgent = horizontalDistanceCheckerForCoverBehaviour.magnitude;
                                }

                                if (horizontalDistanceCheckerForCoverBehaviour.magnitude <= AiCovers.TakenCoverProximityDistance
                                    && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                                {
                                    StopMovingBetweenCovers = false;
                                    NewCoverNode.DistanceCleared = true;
                                }
                            }

                           
                        }

                        // StopBotForShoot = false;

                        if (NewCoverNode.DistanceCleared == false) // NEW CHANGE && ChangeCover == true && Reachnewpoints == false
                        {
                            if (NewCoverNode != null)
                            {
                                NewCoverNode.Info(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy], T);
                            }
                            if (StartShootingNowForOpenfireCover == false && horizontalDistanceCheckerForCoverBehaviour.magnitude <= RandomOpenFireDistanceDuringCoverBehaviour && AiCovers.OpenFireBehaviour.StopAndShootProbability >= 1)
                            {
                                StartCoroutine(CoroForBetweenCoverShots());
                                StartShootingNowForOpenfireCover = true;
                            }
                           // Debug.Log("StopMovingBetweenCovers" + StopMovingBetweenCovers);
                            if (StopMovingBetweenCovers == true && horizontalDistanceCheckerForCoverBehaviour.magnitude >= RandomStopAndShootCancelDistanceToCover)
                            {
                                pathfinder.PreviousDestination = transform.position;
                                pathfinder.IsSameDestination = transform.position;
                                StopBotForShoot = true;
                                LookingAtEnemy();
 
                                if (CanAIStrafeByDefault == true)
                                {
                                    Components.HumanoidFiringBehaviourComponent.FireNow = true;
                                    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                    Fire();
  
                                }
                                else
                                {
                                    Info("Stop and Shoot");
                                    StopSpineRotation = false;
                                    //if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
                                    //{
                                        SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                                    //}
                                    AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);
                                    if (Components.HumanoidFiringBehaviourComponent.isreloading)
                                    {
                                        FullUpperAndLowerBodyReload();
                                    }
                                }
                            }
                            else if(pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false && pathfinder.NavMeshAgentComponent.enabled == true)
                            {
                                Info("Moving Towards Cover");
                                ChangeCover = true;
                                ReachnewCoverpoint = false;
                                if (NewCoverNode.StandingCover == true)
                                {
                                    RefreshStandCoverOnce = false;
                                    // pathfinder.CalculatePathForCombat(0f, 0f, CrouchPositions[CurrentCoverPoint].position);
                                    //if (pathfinder.IsPathComplete == true)
                                    //{

                                    if (AiCovers.EnableSprintingBetweenCovers == true)
                                    {
                                        if (horizontalDistanceCheckerForCoverBehaviour.magnitude >= RandomSprintingDistanceBetweenCovers)
                                        {
                                            if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                                            {
                               //                 Debug.Log("ChangeCover" + ChangeCover + "CoverName" + NewCoverNode.transform.name + "Bot Name" + transform.name
                               //+ "ReachNewPoint" + ReachnewCoverpoint + "AdvanceBooking" + NewCoverNode.AdvanceBookingForCover + "CombatStarted"
                               // + CombatStarted);
                                                StopSpineRotation = true;
                                                SprintingControllerForCover();
                                                Info("Sprinting Towards Cover");
                                                // AiCurrentState = "MOVING TOWARDS COVER";
                                                // Components.NavMeshAgentComponent.SetDestination(CrouchPositions[CurrentCoverPoint].position);
                                                FindingNewCrouchPoint = false; // new change
                                            }
                                            else
                                            {
                                                StopBot();
                                                FullUpperAndLowerBodyReload();
                                            }
                                        }
                                        else
                                        {
                                            LookingAtEnemy();
                                            RunTowardsDestination();
                                            AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.RunForwardSpeed, true);

                                        }


                                    }
                                    else
                                    {
                                        LookingAtEnemy();
                                        RunTowardsDestination();
                                        AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.RunForwardSpeed, true);

                                    }

                                    //}
                                    //else
                                    //{
                                    //    FindImmediateCoverPoint();
                                    //    //  VeryNearToEnemy = false;

                                    //    if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                                    //    {
                                    //        Fire();
                                    //    }
                                    //    else
                                    //    {
                                    //        Reload();                   
                                    //    }
                                    //}
                                }
                                else
                                {
                                    //pathfinder.CalculatePathForCombat(0f, 0f, CrouchPositions[CurrentCoverPoint].position);
                                    //if (pathfinder.IsPathComplete == true)
                                    //{
                                    if (AiCovers.EnableSprintingBetweenCovers == true)
                                    {
                                        if (horizontalDistanceCheckerForCoverBehaviour.magnitude >= RandomSprintingDistanceBetweenCovers)
                                        {
                                            if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                                            {
                                                Info("Sprinting Towards Cover");
                                                StopSpineRotation = true;
                                                Components.HumanoidFiringBehaviourComponent.FireNow = false;
                                                SprintingControllerForCover();
                                                //  AiCurrentState = "MOVING TOWARDS COVER";
                                                //Components.NavMeshAgentComponent.SetDestination(CrouchPositions[CurrentCoverPoint].position);
                                                FindingNewCrouchPoint = false;
                                            }
                                            else
                                            {
                                                StopBot();
                                                FullUpperAndLowerBodyReload();
                                            }

                                        }
                                        else
                                        {
                                            LookingAtEnemy();
                                            RunTowardsDestination();
                                            AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.RunForwardSpeed, true);


                                        }
                                    }
                                    else
                                    {
                                        LookingAtEnemy();
                                        RunTowardsDestination();
                                        AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.RunForwardSpeed, true);

                                    }
                                }
                   
                            }
                        }
                        else if(pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                        {
                            ReachnewCoverpoint = true;
                        }
                        //else
                        //{
                        //    if (CrouchPositions[CurrentCoverPoint].GetComponent<CoverNode>().ValidCover == true && CurrentCoverPoint != LastCoverPoint
                        //        || CrouchPositions[CurrentCoverPoint].GetComponent<CoverNode>().ValidCover == true && FirstCover == true)
                        //    {
                        //    }
                        //}
                    }
                    else
                    {
                        if (NewCoverNode.transform != null)
                        {
                            //Debug.Log(transform.name + " " + "RootName" + NewCoverNode.transform.root.name + " " + NewCoverNode + "CoverName" + " " + "IsValidCover" + NewCoverNode.IsValidCover + " " + "CanTakeCover" + CanTakeCover + " " +
                            //     "AdvanceBookingForCover" + NewCoverNode.AdvanceBookingForCover + " " + "FirstCover" + FirstCover);
                            //if (LastCoverPoint == CurrentCoverPoint)
                            //{
                            //}
                            //else
                            //{
                            if (CheckTime == false)
                            {
                              //  Debug.Log("RESETING COVER");
                                Info("Finding New Cover");
                                StartCoroutine(ResetCoverTime());
                                CheckTime = true;
                            }

                          //  Debug.Log(ReachnewCoverpoint + "ReachedNewCoverPoint");


                            //if (NewCoverNode.IsValidCover == false) Commented again on 29th Jan 2024 while fixing bugs when all the probability sliders were activated on the Ai agent - cover,charging and firing behaviour
                            //{

                                NewCoverNode.DistanceCleared = false;
                                ReachnewCoverpoint = false;
                                StopBotForShoot = false;
                                LookingAtEnemy();
                                Components.HumanoidFiringBehaviourComponent.FireNow = true;
                                anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                                Fire();
                            //}
                             

                            // }

                            // ReachedCoverPoint(); 

                            //if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                            //{
                            //    Fire();
                            //}
                            //else
                            //{
                            //    Reload();
                            //}
                            // ReachedCoverPoint();

                            //}
                            //else
                            //{
                            //    FindingNewCrouchPoint = false;
                            //    FindImmediateCoverPoint();

                            //if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                            //{
                            //    Fire();
                            //}
                            //else
                            //{
                            //    Reload();
                            //}
                            // }

                        }
                    }
                    //NextNearestCoverPoint = CrouchPositions[CurrentCoverPoint].name;
                }
                else
                {
                    if (ShouldReloadBeforeOpenFirePointUse == false && CheckReloadingBeforeOpenFirePointUse == false)
                    {
                        if (MoveTowardsOpenFirePoint == true && Components.HumanoidFiringBehaviourComponent.isreloading)
                        {
                            ShouldReloadBeforeOpenFirePointUse = true;
                            CheckReloadingBeforeOpenFirePointUse = true;
                        }
                    }

                    if (MoveTowardsOpenFirePoint == true && !Components.HumanoidFiringBehaviourComponent.isreloading && ShouldReloadBeforeOpenFirePointUse == true)
                    {
                        ShouldReloadBeforeOpenFirePointUse = false;
                    }

                    if (MoveTowardsOpenFirePoint == true && ShouldReloadBeforeOpenFirePointUse == false)// && !Components.HumanoidFiringBehaviourComponent.isreloading)
                    {
                        // StopBotForShoot = false;
                        CheckReloadingBeforeOpenFirePointUse = true;
                        if (FindOpenFirePoint == false)
                        {
                            IsProceduralPointGeneratedForOpenFire = false;
                            // AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);
                            IsCrouched = false;
                            //OpenFirePointSelected = Random.Range(0, CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().OpenFirePoints.Length);

                            float shortestDistance = float.MaxValue;
                            for (int x = 0; x < NewCoverNode.OpenFirePoints.Length; x++)
                            {
                                Vector3 DistanceWithEnemy = NewCoverNode.OpenFirePoints[x].transform.position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                                if (DistanceWithEnemy.magnitude < shortestDistance)
                                {
                                    shortestDistance = DistanceWithEnemy.magnitude;
                                    OpenFirePointSelected = x;

                                }
                            }

                            if (NewCoverNode.OpenFirePoints.Length >= 1)
                            {
                                if (!NewCoverNode.AiCoverScript.OpenFirePoints.Contains
                                (NewCoverNode.OpenFirePoints[OpenFirePointSelected]))
                                {
                                    NewCoverNode.AiCoverScript
                                        .OpenFirePoints.Add(NewCoverNode.OpenFirePoints[OpenFirePointSelected]);

                                    NewpointOnNavMesh = NewCoverNode.OpenFirePoints[OpenFirePointSelected].position;
                                }
                                else
                                {
                                    if (NewCoverNode.SecondaryOpenFirePoint != null)
                                    {
                                        NewpointOnNavMesh = GenerateRandomNavmeshLocation.RandomLocationInVector3(NewCoverNode.SecondaryOpenFirePoint.transform.position, NewCoverNode.SecondaryOpenFirePointRadius);
                                        IsProceduralPointGeneratedForOpenFire = true;
                                    }
                                    else
                                    {
                                        NewpointOnNavMesh = GenerateRandomNavmeshLocation.RandomLocationInVector3(NewCoverNode.transform.position, NewCoverNode.SecondaryOpenFirePointRadius);
                                        IsProceduralPointGeneratedForOpenFire = true;
                                    }

                                }
                            }
                            else
                            {
                                if (NewCoverNode.SecondaryOpenFirePoint != null)
                                {
                                    NewpointOnNavMesh = GenerateRandomNavmeshLocation.RandomLocationInVector3(NewCoverNode.SecondaryOpenFirePoint.transform.position, NewCoverNode.SecondaryOpenFirePointRadius);
                                    IsProceduralPointGeneratedForOpenFire = true;
                                }
                                else
                                {
                                    NewpointOnNavMesh = GenerateRandomNavmeshLocation.RandomLocationInVector3(NewCoverNode.transform.position, NewCoverNode.SecondaryOpenFirePointRadius);
                                    IsProceduralPointGeneratedForOpenFire = true;
                                }
                            }

                            //if (CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().EnableStrafeOnFire == true && CombatStateBehaviours.EnableStrafing == true
                            //        && CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().RandomStrafeOrStationedFire == true)
                            //{
                            //    RandomNumberReceiver = Random.Range(0, 2);
                            //}
                            OverriteOpenFire = false;
                            FindOpenFirePoint = true;
                        }

                        pathfinder.FindClosestPointTowardsDestination(NewpointOnNavMesh);
                        //Vector3 Dis = CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().OpenFirePoints[OpenFirePointSelected].position - transform.position;

                        Vector3 targetPosition = pathfinder.closestPoint;
                        Vector3 horizontalDisplacement = new Vector3(targetPosition.x - transform.position.x, 0f, targetPosition.z - transform.position.z);

                        if (horizontalDisplacement.magnitude <= 0.5f && pathfinder.NoMoreChecks == true && 
                             pathfinder.IsNavMeshObstacleDisabled == false || OverriteOpenFire == true && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                        {
                           
                            Info("Shooting");
                            OverriteOpenFire = true;
                            RotatingTransforms.ChangeRotation(transform, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, transform.position, Speeds.MovementSpeeds.BodyRotationSpeed);
                            if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                            {
                                //if(CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().EnableStrafeOnFire == true && CombatStateBehaviours.EnableStrafing == true
                                //    && CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().RandomStrafeOrStationedFire == false)
                                //{
                                //    Fire();
                                //}
                                //else if (CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().EnableStrafeOnFire == true && CombatStateBehaviours.EnableStrafing == true
                                //   && CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().RandomStrafeOrStationedFire == true)
                                //{
                                //    if (RandomNumberReceiver == 1)
                                //    {
                                //        Fire();
                                //    }
                                //    else
                                //    {
                                //        StopSpineRotation = false;
                                //        if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
                                //        {
                                //            SetAnims(AiAgentAnimatorParameters.AimingParameterName);
                                //        }
                                //        AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);
                                //    }
                                //}
                                //else
                                //{

                                if (OpenFirePointSelected <= NewCoverNode.OpenFirePoints.Length && NewCoverNode.OpenFirePoints.Length >= 1)
                                {
                                    if (NewCoverNode.OpenFirePoints[OpenFirePointSelected].gameObject.tag == "CrouchFirePoint"
                                   && IsProceduralPointGeneratedForOpenFire == false)
                                    {
                                        Info("Open Fire Crouch Shooting");
                                        CoverFireController();
                                    }
                                    else
                                    {
                                        Info("Open Fire Stand Shooting");
                                        //StopSpineRotation = false;
                                        //SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                                        //ConnectWithUpperBodyAimingAnimation();
                                        //AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);
                                        StandShoot();
                                    }
                                }
                                else
                                {
                                    Info("Open Fire Stand Shooting");
                                    //StopSpineRotation = false;
                                    //SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                                    //ConnectWithUpperBodyAimingAnimation();
                                    //AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);
                                    StandShoot();

                                }


                                //}
                            }
                            else
                            {
                                if (OpenFirePointSelected <= NewCoverNode.OpenFirePoints.Length && NewCoverNode.OpenFirePoints.Length >= 1)
                                {
                                    if (NewCoverNode.OpenFirePoints[OpenFirePointSelected].gameObject.tag == "CrouchFirePoint"
                                    && IsProceduralPointGeneratedForOpenFire == false)
                                    {
                                        Info("Crouch Reloading");
                                        CoverReloadController();

                                    }
                                    else
                                    {
                                        StopSpineRotation = false;
                                        FullUpperAndLowerBodyReload();
                                    }
                                }
                                else
                                {
                                    StopSpineRotation = false;
                                    FullUpperAndLowerBodyReload();
                                }


                            }

                            if (StartStayTimeForOpenFirePoint == false)
                            {
                                StartCoroutine(StayTimerForOpenFire());
                                StartStayTimeForOpenFirePoint = true;
                            }


                        }
                        else if(pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false && pathfinder.NavMeshAgentComponent.enabled == true)
                        {
                            Info("Moving Towards Firing point");
                            RotatingTransforms.ChangeRotation(transform, NewpointOnNavMesh, transform.position, Speeds.MovementSpeeds.BodyRotationSpeed);
                            AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, true);
                            if (Components.NavMeshAgentComponent.enabled == true)
                            {
                                Components.NavMeshAgentComponent.isStopped = false;
                            }
                            SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, false);
                            Components.HumanoidFiringBehaviourComponent.FireNow = false;
                            StopSpineRotation = true;
                        }
                    }
                    else
                    {
                        // MovingTowardsCover = false;
                        if (StartFindRealCovers == false)
                        {
                            FirstCover = false;
                            StartFindRealCovers = true;
                        }
                        ReachedCoverPoint();
                        if (CheckTime == false)
                        {
                          //  Debug.Log("RESETING COVER");
                            //if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                            //{                         
                            //    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.gameObject.GetComponent<CoreAiBehaviour>().CombatStateBehaviours.TakeCovers == true)
                            //    {
                            //        FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.gameObject.GetComponent<CoreAiBehaviour>().CheckCoverDistanceWithEnemy();
                            //    }
                            //}
                            SelectStrafePoint = false;
                            //LastCoverPoint = CurrentCoverPoint;
                            StartCoroutine(ResetCoverTime());
                            //TimerforCoverLogic = 0;
                            CheckTime = true;
                        }
                    }

                }
            }
            else
            {
                if (StartStrafingForAvoidingBots == true)
                {
                    RandomStrafing();
                }
                else
                {
                    if (BotMovingAwayFromGrenade == false)
                    {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                        {
                            NewCoverNode.CheckifEnemyIsInView(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
                        }

                        if (ReachnewCoverpoint == true)
                        {
                            ReachedCoverPoint();
                        }
                        else
                        {
                            if (NewCoverNode.CrouchCover == true)
                            {
                                if (NewCoverNode.DistanceCleared == false)
                                {

                                    if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                                    {
                                        Fire();
                                    }
                                    else
                                    {
                                        FullUpperAndLowerBodyReload();
                                    }
                                }
                                else
                                {
                                    Components.HumanoidFiringBehaviourComponent.FireNow = false;
                                }
                            }
                            else
                            {


                                if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                                {
                                    Fire();
                                }
                                else
                                {
                                    FullUpperAndLowerBodyReload();
                                }
                            }
                        }
                    }
                    else
                    {
                        SprintAwayFromGrenade();
                    }

                }

            }
            //}
            //else
            //{
            //    FindingNewCrouchPoint = false;
            //    FindImmediateCoverPoint();
            //    RandomStrafing();
            //}
        }
        //public void CheckCoverDistanceWithEnemy()
        //{
        //    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
        //    {
        //        Vector3 DistanceBetweenCover = CrouchPositions[CurrentCoverPoint].position - FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
        //        if (DistanceBetweenCover.magnitude < 2f)
        //        {
        //            FindImmediateCoverPoint();
        //        }
        //    }
        //}
        IEnumerator StayTimerForOpenFire()
        {
            DoNotLookForCover = true;
            float Randomise = Random.Range(AiCovers.MinTimeAtOpenFirePoint, AiCovers.MaxTimeAtOpenFirePoint);
            yield return new WaitForSeconds(Randomise);

            // CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().IsAlreadyRegisteredCover = false;
            // CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().AdvanceBookingForCover = false;
            FindValidCover = true;
            MoveTowardsOpenFirePoint = false;
            if (NewCoverNode.OpenFirePoints.Length >= 1)
            {
                if (OpenFirePointSelected <= NewCoverNode.OpenFirePoints.Length)
                {
                    if(NewCoverNode.AiCoverScript.OpenFirePoints.Count > 0)
                    {
                        if (NewCoverNode.AiCoverScript.OpenFirePoints.Contains
                                            (NewCoverNode.OpenFirePoints[OpenFirePointSelected]))
                        {
                            NewCoverNode.AiCoverScript.OpenFirePoints.Remove(NewCoverNode.OpenFirePoints[OpenFirePointSelected]);
                        }
                    }                 
                }

            }
            // ReachnewCoverpoint = false;
            CanTakeCover = false;
            //ChangeCover = true;
            //ResetCoverRandomisation = true;
            FindOpenFirePoint = false;
            OverriteOpenFire = false;
            StartStayTimeForOpenFirePoint = false;
            DoNotLookForCover = false;
            NewCoverNode.DistanceCleared = false;
            if (DeregisterCoverNodes == false)
            {
                StartCoroutine(FindClosestCoverSystematically());
            }
            IsTaskOver = true;
            CheckReloadingBeforeOpenFirePointUse = false;
            // StopBotForShoot = false;
            //  StopMovingBetweenCovers = false;
            //CheckTime = false;
        }
        public void Info(string Message)
        {
#if UNITY_EDITOR
            if (DebugAgentState == true)
            {
                if(spawnedText != null)
                {
                    spawnedText.text = Message;
                }
            }
#endif
        }
        public void CoverShooter()
        {
#if UNITY_EDITOR
            if (spawnedTextPrefab != null)
            {
                spawnedTextPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "COVER POINT AGENT";
                spawnedTextPrefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = transform.name;
            }
#endif
            if (InitialiseCoverBehaviour == false && Iscombatstatebegins == true)
            {
                InitialiseCoverBehaviour = true;
                if (CombatStateBehaviours.TakeCovers == true)
                {
                    SavedCurrentPositionForCoverFinding = transform.position;
                    if (AiCovers.CoverFinder.GetComponent<SphereCollider>() != null)
                    {
                        // SphereCollider b = AiCovers.CoverFinder.GetComponent<SphereCollider>();
                        // b.radius = AiCovers.RangeToFindCoverPoint / 2;
                        AiCovers.CoverFinder.GetComponent<SphereCollider>().radius = AiCovers.RangeToFindExistingCoverPoints;
                    }
                    GetAccurateCoverRange = AiCovers.RangeToFindExistingCoverPoints;// / 2;

                }
                AiCovers.CoverFinder.gameObject.SetActive(true);
                RandomStopAndShootCancelDistanceToCover = Random.Range(AiCovers.MinStopAndShootCancelDistanceToCover, AiCovers.MaxStopAndShootCancelDistanceToCover);
                RandomOpenFireDistanceDuringCoverBehaviour = Random.Range(AiCovers.OpenFireBehaviour.MinStopAndShootDistanceToEnemyOrToCover, AiCovers.OpenFireBehaviour.MaxStopAndShootDistanceToEnemyOrToCover);
                RandomSprintingDistanceBetweenCovers = Random.Range(AiCovers.MinSprintingDistance, AiCovers.MaxSprintingDistance);

                //StartCoroutine(StartfindingfirstCover());
                TimeToCheckForCover = Random.Range(AiCovers.MinTimeToCheckForCovers, AiCovers.MaxTimeToCheckForCovers);
                InvokeRepeating("CheckForValidCover", TimeToCheckForCover, TimeToCheckForCover);
                SaveRangeForCover = AiCovers.RangeToFindExistingCoverPoints;
                // SaveResetedCoverRandomisation = Random.Range(AiCovers.MinimumTimeBetweenCovers, AiCovers.MaximumTimeBetweenCovers);
                TimeToCheckForCoverIfNoCoverAvailable = Random.Range(AiCovers.MinTimeToCheckForCovers, AiCovers.MaxTimeToCheckForCovers);



            }
            // TimerforCoverLogic += Time.deltaTime;

            //if (ResetCoverRandomisation == false)
            //{
            //    SaveResetedCoverRandomisation = Random.Range(AiCovers.MinimumTimeBetweenCovers, AiCovers.MaximumTimeBetweenCovers);
            //    if (AiCovers.MoveImmediateOnFirstCover == true && CanInitializeCover == true)
            //    {
            //        if (InitialiseCover == false)
            //        {
            //            SaveResetedCoverRandomisation = 0;
            //            InitialiseCover = true;
            //        }
            //    }
            //    ResetCoverRandomisation = true;
            //}

            if (FindEnemiesScript.FindedEnemies == true)
            {
                //if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] == null)
                //{

                //}

                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null
                    && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.transform != this.transform && IsNearDeadBody == false)
                {
                    //if(ActivatePostCombatState == false)
                    //{
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<HumanoidAiHealth>() != null)
                        {
                            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<HumanoidAiHealth>().IsDied == true)
                            {
                                //LastEnemy = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform;
                                FindImmediateEnemy();
                            }

                        }
                        else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.tag == "Player")
                        {
                            if (PlayerHealth.instance != null)
                            {
                                if (PlayerHealth.instance.IsDead == true)
                                {
                                    FindImmediateEnemy();
                                }
                            }
                        }
                      else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<Turret>() != null)
                      {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<Turret>().IsDied == true)
                        {
                            FindImmediateEnemy();
                        }

                      }
                    //}
                    //else
                    //{
                    //    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != LastEnemy)
                    //    {
                    //        ActivatePostCombatState = false;
                    //    }
                    //}


                    CheckFieldofView();

                    if (Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, this.transform.position) < FindEnemiesScript.DetectionRadius && IsEnemyLocked == true && VisibilityCheck.CanSeeTarget(FindEnemiesScript.OriginalFov, FindEnemiesScript.DetectionRadius, Components.HumanoidFiringBehaviourComponent) && HealthScript.CompleteFirstHitAnimation == false)
                    {
                        if (CombatStateBehaviours.EnablePostCombatScan == true)
                        {
                            ResetVariableForQuickScan();
                            WasInCombatStateBefore = true;
                        }

                        // WasInCombatStatePreviously = true;
                        Vector3 CheckforNewCover = SavedCurrentPositionForCoverFinding - transform.position;

                        //Vector3 position = transform.position;

                        //// Calculate the direction to draw the line
                        //Vector3 direction = -transform.right;

                        //// Calculate the endpoint of the line
                        //Vector3 endpoint = position + direction * 25f;

                        //// Draw the debug line
                        //Debug.DrawLine(position, endpoint, Color.green, 5f);


                        if (CheckforNewCover.sqrMagnitude >= GetAccurateCoverRange * GetAccurateCoverRange)
                        {
                            SavedCurrentPositionForCoverFinding = transform.position;
                            AiCovers.CoverFinder.gameObject.SetActive(true);
                        }

                        CheckForLockedTargetInFOV();
                        if (CrouchPositions.Count <= 0)
                        {
                            if (BotMovingAwayFromGrenade == false)
                            {
                                RandomStrafing();
                            }
                            else
                            {
                                SprintAwayFromGrenade();
                            }
                        }
                        else
                        {

                            if (NewCoverNode != null)
                            {
                                AiCoverMethod();
                            }
                            else
                            {
                                if (NewCoverNode == null && FirstFindingsCompleted == true)
                                {
                                    if (CheckingForCoverAvailability == false)
                                    {
                                        StartCoroutine(CoverChecksToDoIfNoCoverAvailable());
                                        CheckingForCoverAvailability = true;
                                    }
                                }

                                if (BotMovingAwayFromGrenade == false)
                                {
                                    RandomStrafing();
                                }
                                else
                                {
                                    SprintAwayFromGrenade();
                                }

                            }
                        }

                        // if (Timer > SaveResetedCoverRandomisation)
                        ////{
                        //  if (Reachnewpoints == false)
                        // {
                        //if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                        //{
                        //    CrouchPositions[CurrentCoverPoint].GetComponent<CoverNode>().CheckifEnemyIsInView(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
                        //}

                        //if (CrouchPositions[CurrentCoverPoint].GetComponent<CoverNode>().ValidCover == true && FindValidCover == false)
                        //{
                        //    if (CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().TriggerOnce == false)
                        //    {
                        //        CurrentCoverPointTransform = CrouchPositions[CurrentCoverPoint].transform;
                        //        if (CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().StandingCover == true)
                        //        {
                        //            pathfinder.CalculatePathForCombat(MinTimeBetweenCalculatingNavMeshPath, MaxTimeBetweenCalculatingNavMeshPath, CrouchPositions[CurrentCoverPoint].position);
                        //            if (pathfinder.IsPathComplete == true)
                        //            {
                        //                CurrentState = "MOVING FOR STANDING COVER";
                        //                NavMeshAgentComponent.SetDestination(CrouchPositions[CurrentCoverPoint].position);
                        //                StopSpineRotation = true;
                        //                SprintingControllerForCover();
                        //                FindingNewCrouchPoint = false;  
                        //            }
                        //            else
                        //            {
                        //                FindImmediateCoverPoint();
                        //                StopSpineRotation = false;
                        //                anim.SetBool(DefaultStateAnimationName, true);
                        //                Fire();
                        //                CurrentState = "IN COMBAT";
                        //            }
                        //        }
                        //        else
                        //        {
                        //            pathfinder.CalculatePathForCombat(MinTimeBetweenCalculatingNavMeshPath, MaxTimeBetweenCalculatingNavMeshPath, CrouchPositions[CurrentCoverPoint].position);
                        //            if (pathfinder.IsPathComplete == true)
                        //            {
                        //                StopSpineRotation = true;
                        //                HumanoidFiringBehaviourComponent.FireNow = false;
                        //                SprintingControllerForCover();
                        //                NavMeshAgentComponent.SetDestination(CrouchPositions[CurrentCoverPoint].position);
                        //                FindingNewCrouchPoint = false;
                        //                CurrentState = "MOVING FOR COVER";
                        //            }
                        //            else
                        //            {
                        //                FindImmediateCoverPoint();
                        //                StopSpineRotation = false;
                        //                anim.SetBool(DefaultStateAnimationName, true);
                        //                Fire();
                        //                CurrentState = "IN COMBAT";
                        //            }
                        //        }
                        //        CheckTime = false;
                        //    }
                        //    else
                        //    {
                        //        StopSpineRotation = false;
                        //        CurrentState = "IN COMBAT";
                        //        if (CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().StandingCover == true)
                        //        {                                        
                        //            if (!HumanoidFiringBehaviourComponent.isreloading)
                        //            {
                        //                anim.SetBool(DefaultStateAnimationName, true);
                        //                Fire();
                        //            }
                        //            else
                        //            {
                        //                FindingNewCrouchPoint = false;
                        //                FindImmediateCoverPoint();
                        //            }
                        //        }
                        //        else if (CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().HidingCover == true)
                        //        {
                        //            FindingNewCrouchPoint = false;
                        //            FindImmediateCoverPoint();
                        //            HumanoidFiringBehaviourComponent.FireNow = false;
                        //        }
                        //        else
                        //        {
                        //            FindingNewCrouchPoint = false;
                        //            FindImmediateCoverPoint();
                        //            anim.SetBool(DefaultStateAnimationName, true);
                        //            Fire();
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    if (CrouchPositions[CurrentCoverPoint] != null)
                        //    { 
                        //        StopSpineRotation = false;
                        //        FindingNewCrouchPoint = false;
                        //        FindImmediateCoverPoint();
                        //        anim.SetBool(DefaultStateAnimationName, true);
                        //        Fire();
                        //    }
                        //}
                        //  }
                        //else
                        //  {
                        //CurrentState = "IN COMBAT";
                        //ReachedCoverPoint();
                        //if (CheckTime == false)
                        //{
                        //    StartCoroutine(ResettingCover());
                        //    Timer = 0;
                        //    CheckTime = true;
                        //}
                        //}
                        // }
                        //else
                        //{
                        //    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                        //    {
                        //        CrouchPositions[CurrentCoverPoint].GetComponent<CoverNode>().CheckifEnemyIsInView(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
                        //    }

                        //    if (Reachnewpoints == true)
                        //    {
                        //        CurrentState = "IN COMBAT";
                        //        ReachedCoverPoint();
                        //    }
                        //else
                        //{
                        //    if (CrouchPositions[CurrentCoverPoint].GetComponent<CoverNode>().HidingCover == true && FindValidCover == false) // NEW CHANGE ON 17/10 FINDVALIDCOVR = FALSE (added)
                        //    {
                        //        HumanoidFiringBehaviourComponent.FireNow = false;
                        //    }
                        //    else
                        //    {
                        //        CurrentState = "IN COMBAT";
                        //        if (!HumanoidFiringBehaviourComponent.isreloading)
                        //        {
                        //            HumanoidFiringBehaviourComponent.FireNow = true;
                        //            anim.SetBool(DefaultStateAnimationName, true);
                        //            Fire();
                        //        }
                        //        else
                        //        {
                        //            HumanoidFiringBehaviourComponent.FireNow = false;
                        //            anim.SetBool(DefaultStateAnimationName, true);
                        //            Reload();
                        //        }
                        //    }
                        //}
                        // }
                    }
                    else
                    {
                        if (BotMovingAwayFromGrenade == false)
                        {
                            if (HealthScript.CompleteFirstHitAnimation == false)
                            {
                                if (VisibilityCheck.ConnectionLost == true)
                                {
                                    if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableApproachingEnemyPursue
                                         || HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableStationedEnemyPursue)
                                    {
                                        ConnectionLostForMovingBots();
                                    }
                                    //else if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableStationedEnemyPursue)
                                    //{
                                    //    ConnectionLostForStationarybot();
                                    //}
                                    else if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.DoNotPursueEnemy)
                                    {
                                        FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
                                        VisibilityCheck.ConnectionLost = false;
                                        StaringAtLastEnemyLostPosition = false;
                                    }
                                }
                                else
                                {
                                    DefaultInvestigationBehaviour();
                                }
                            }
                        }
                        else
                        {
                            SprintAwayFromGrenade();
                        }
                    }
                }
                else
                {
                    //if(WasInCombatStatePreviously == true)
                    //{
                    //    if (!FindEnemiesScript.enemy.Contains(LastEnemy))
                    //    {
                    //        FindEnemiesScript.deadenemies.Clear();
                    //        FindEnemiesScript.deadenemies.Add(LastEnemy);
                    //        FindEnemiesScript.enemy.Add(LastEnemy);
                    //        IsNearDeadBody = false;
                    //        ActivatePostCombatState = true;
                    //    }
                    //    //FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] = LastEnemy;
                    //    //if(ActivatePostCombatState == false)
                    //    //{
                    //    //    StartCoroutine(PostCombatState());
                    //    //    ActivatePostCombatState = true;
                    //    //}

                    //}
                    //else
                    //{
                    //if (IsNearDeadBody == true && NonCombatBehaviours.EnableEmergencyAlerts == true && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != transform.root.transform)
                    //{
                    //    CompleteEmergencyStateBehaviour();
                    //}
                    //else
                    //{
                    //    if (SearchingForSound == false)
                    //    {
                    //        FindImmediateEnemy(); 
                    //        DefaultInvestigationBehaviour();
                    //    }
                    //}

                    // }

                    if (IsNearDeadBody == true && NonCombatBehaviours.EnableEmergencyAlerts == true && IsInEmergencyState == true && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != transform.root.transform
                      && HealthScript.CompleteFirstHitAnimation == false)
                    {
                        CompleteEmergencyStateBehaviour();
                    }
                    else if (IsNearDeadBody == true && NonCombatBehaviours.EnableDeadBodyAlerts == true && IsInEmergencyState == false && IsEmergencyStateCurrentlyActive == false
                        && HealthScript.CompleteFirstHitAnimation == false)//FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != transform.root.transform && 
                    {
                        DeadBodyInvestigation();
                    }
                    else
                    {
                        if (SearchingForSound == false && HealthScript.CompleteFirstHitAnimation == false)
                        {
                            // This code is commented on 14th Jan 2025 because when in beginning there was a player and turret and AI agent . The AI agent removed its enemies which was Player and Turret and ignored them basically
                           // and did not updated its enemy and did not shoot even.
                           //  FindImmediateEnemy();
                            DefaultInvestigationBehaviour();
                        }
                    }


                }
            }
            else
            {
                DefaultInvestigationBehaviour();
            }

        }
        //IEnumerator PostCombatState()
        //{
        //    yield return new WaitForSeconds(3f);
        //    WasInCombatStatePreviously = false;
        //    ActivatePostCombatState = false;
        //}
        IEnumerator CoroForBetweenShots()
        {
            RandomTimeToActivateShootBetweenSprint = Random.Range(AiCharging.OpenFireBehaviour.MinTimeTillStopAndShootBehaviour, AiCharging.OpenFireBehaviour.MaxTimeTillStopAndShootBehaviour);
            yield return new WaitForSeconds(RandomTimeToActivateShootBetweenSprint);
            StopMoving = false;
            int decider = Random.Range(0, 100);

            if (decider <= AiCharging.OpenFireBehaviour.StopAndShootProbability)
            {
                StopMoving = true;
                int ShouldShoot = Random.Range(0, 100);
                if (ShouldShoot <= AiCharging.OpenFireBehaviour.StrafingProbability)
                {
                    CanAIStrafeByDefault = true;
                }
                else
                {
                    CanAIStrafeByDefault = false;
                }
            }
            else
            {
                StopMoving = false;
            }

            if (StopMoving == true)
            {
                RandomTimeToKeepShootBetweenSprint = Random.Range(AiCharging.OpenFireBehaviour.MinStopAndShootDuration, AiCharging.OpenFireBehaviour.MaxStopAndShootDuration);
                yield return new WaitForSeconds(RandomTimeToKeepShootBetweenSprint);
                StartShootingNow = false;
                StopMoving = false;

            }
            else
            {
                StartShootingNow = false;
            }
        }
        //IEnumerator InitialShootBeforeMoving()
        //{
        //    UsualTask = false;
        //    yield return new WaitForSeconds(1f);
        //    UsualTask = true;
        //}
        public void StopBot()
        {
            if (Components.NavMeshAgentComponent.enabled == true)
            {
                Components.NavMeshAgentComponent.isStopped = true;
                Components.NavMeshAgentComponent.speed = 0f;
            }
        }
        IEnumerator CheckChargingDistanceToEnemy()
        {
            float Randomise;
            if (AgentRole != Role.Zombie)
            {
                if(pathfinder.IsNavMeshThirdAttemptCheck == true)
                {
                    Randomise = Random.Range(AiCharging.MinNavMeshPathCheckTimeWhenDestinationIsInComplete, AiCharging.MaxNavMeshPathCheckTimeWhenDestinationIsInComplete);
                }
                else
                {
                    Randomise = Random.Range(AiCharging.MinNavMeshPathCheckTimeWhenDestinationIsComplete, AiCharging.MaxNavMeshPathCheckTimeWhenDestinationIsComplete);
                }
            }
            else
            {
                if (pathfinder.IsNavMeshThirdAttemptCheck == true)
                {
                    Randomise = Random.Range(ZombieCharging.MinNavMeshPathCheckTimeWhenDestinationIsInComplete, ZombieCharging.MaxNavMeshPathCheckTimeWhenDestinationIsInComplete);
                }
                else
                {
                    Randomise = Random.Range(ZombieCharging.MinNavMeshPathCheckTimeWhenDestinationIsComplete, ZombieCharging.MaxNavMeshPathCheckTimeWhenDestinationIsComplete);
                }
            }
            yield return new WaitForSeconds(Randomise);
            CheckDistanceToEnemy = false;
        }
        public void QuickDistanceCheckWithEnemy()
        {
            //if (AgentRole == Role.Zombie)
            //{
            //    DistanceCheckForCharging = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
            //    pathfinder.closestPoint = DistanceCheckForCharging;
            //    pathfinder.NoMoreChecks = true;
            //    pathfinder.PathIsUnreachable = false;
            //    CheckDistanceToEnemy = true;
            //}
            //else
            //{
                if (CheckDistanceToEnemy == false)
                {
                    pathfinder.FindClosestPointTowardsDestination(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position);

                    if (pathfinder.NoMoreChecks == true)
                    {
                        // Now this coroutine check is important otherwise the game could freeze due to for loop. we are minimizing game lagging by using min and max timer for all agents to do check to its enemy
                        // this can decrease the load on CPU otherwise if you not use coroutine that with 10 agents the game can lag if all of them are charging agent.
                        StartCoroutine(CheckChargingDistanceToEnemy());
                        CheckDistanceToEnemy = true;
                    }
                    if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                    {
                        DistanceCheckForCharging = new Vector3(pathfinder.closestPoint.x - transform.position.x, 0f, pathfinder.closestPoint.z - transform.position.z);
                        DistanceCheckForCharging.y = 0;
                    }
                    else
                    {
                        DistanceCheckForCharging = new Vector3(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x - transform.position.x, 0f, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.z - transform.position.z);
                    }

                }
                else
                {
                    if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                    {
                        DistanceCheckForCharging = new Vector3(pathfinder.closestPoint.x - transform.position.x, 0f, pathfinder.closestPoint.z - transform.position.z);
                        DistanceCheckForCharging.y = 0;
                    }
                    else
                    {
                        DistanceCheckForCharging = new Vector3(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x - transform.position.x, 0f, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.z - transform.position.z);
                    }
                }
         //   }
        }
        public void ChargeShooter()
        {
#if UNITY_EDITOR
            if (spawnedTextPrefab != null)
            {
                spawnedTextPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "CHARGING AGENT";
                spawnedTextPrefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = transform.name;
            }
#endif
            if (InitialiseChargingBehaviour == false && Iscombatstatebegins == true)
            {
                RandomChargeRunningCheck = Random.Range(AiCharging.MinRunDistance, AiCharging.MaxRunDistance);
                RandomChargeSprintCheck = Random.Range(AiCharging.MinSprintDistance, AiCharging.MaxSprintDistance);
                //RandomWalkChargeDistance = Random.Range(AiCharging.CloseProximityBehaviour.MinDistanceToForceWalking, AiCharging.CloseProximityBehaviour.MaxDistanceToForceWalking);
               // RandomChargeSprint = Random.Range(AiCharging.SprintingBehaviour.MinRemainingDistanceToTargetToStopSprinting, AiCharging.SprintingBehaviour.MaxRemainingDistanceToTargetToStopSprinting);
                RandomDistanceForShootBetweenSprint = Random.Range(AiCharging.OpenFireBehaviour.MinStopAndShootDistanceToEnemyOrToCover, AiCharging.OpenFireBehaviour.MaxStopAndShootDistanceToEnemyOrToCover);
                StoppingDistanceForChargeBot = Random.Range(AiCharging.MinClosestDistanceToStop, AiCharging.MaxClosestDistanceToStop);
              //  RandomWalkDistanceTowardsTarget = Random.Range(AiCharging.SprintingBehaviour.MinRemainingDistanceToTargetToStopRunning, AiCharging.SprintingBehaviour.MaxRemainingDistanceToTargetToStopRunning);
                InitialiseChargingBehaviour = true;

                AiCharging.DebugDistances.DebugSprintDistance = RandomChargeSprintCheck;
                AiCharging.DebugDistances.DebugRunDistance = RandomChargeRunningCheck;
                //AiCharging.DebugDistances.DebugWalkDistance = RandomWalkChargeDistance;
            }

            if (FindEnemiesScript.FindedEnemies == true)
            {
                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null
                    && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.transform != this.transform && IsNearDeadBody == false)
                {
                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<HumanoidAiHealth>() != null)
                    {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<HumanoidAiHealth>().IsDied == true)
                        {
                            FindImmediateEnemy();
                        }

                    }
                    else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.tag == "Player")
                    {
                        if (PlayerHealth.instance != null)
                        {
                            if (PlayerHealth.instance.IsDead == true)
                            {
                                FindImmediateEnemy();
                            }
                        }
                    }
                    else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<Turret>() != null)
                    {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<Turret>().IsDied == true)
                        {
                            FindImmediateEnemy();
                        }

                    }

                    CheckFieldofView();

                    if (Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, this.transform.position) < FindEnemiesScript.DetectionRadius && IsEnemyLocked == true && VisibilityCheck.CanSeeTarget(FindEnemiesScript.OriginalFov, FindEnemiesScript.DetectionRadius, Components.HumanoidFiringBehaviourComponent) && HealthScript.CompleteFirstHitAnimation == false)
                    {
                        if (CombatStateBehaviours.EnablePostCombatScan == true)
                        {
                            ResetVariableForQuickScan();
                            WasInCombatStateBefore = true;
                        }

                        CheckForLockedTargetInFOV();

                        if (StartStrafingForAvoidingBots == true && CombatStateBehaviours.EnableStrafing == true)
                        {
                            RandomStrafing();
                            if (Components.HumanoidFiringBehaviourComponent.isreloading)
                            {
                                OnlyUpperBodyReload();
                            }
                        }
                        else
                        {
                           // bool IsPathPartial = false;

                            if (BotMovingAwayFromGrenade == false)
                            {
                                if(PreviousEnemyDuringCharging != FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy])
                                {
                                    CanStopCharge = false;
                                    if (StopBotForShoot == false)
                                    {
                                        QuickDistanceCheckWithEnemy();
                                    }
                                    else
                                    {
                                        DistanceCheckForCharging = new Vector3(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x - transform.position.x, 0f, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.z - transform.position.z);

                                        if (DistanceCheckForCharging.sqrMagnitude >= RandomChargeSprintCheck * RandomChargeSprintCheck && StopMoving == false
                                            && AiCharging.EnableSprinting == true && OverriteRunning == false)
                                        {
                                            QuickDistanceCheckWithEnemy();
                                        }
                                        else if (DistanceCheckForCharging.sqrMagnitude < RandomChargeSprintCheck * RandomChargeSprintCheck && DistanceCheckForCharging.sqrMagnitude >= RandomChargeRunningCheck * RandomChargeRunningCheck
                                                && StopMoving == false && OverWriteWalking == false || OverriteRunning == true && OverWriteWalking == false)
                                        {
                                            QuickDistanceCheckWithEnemy();

                                        }
                                        else if (DistanceCheckForCharging.sqrMagnitude < RandomChargeRunningCheck * RandomChargeRunningCheck && DistanceCheckForCharging.sqrMagnitude >= StoppingDistanceForChargeBot * StoppingDistanceForChargeBot && StopMoving == false
                                                 || OverWriteWalking == true && StopMoving == false)
                                        {
                                            QuickDistanceCheckWithEnemy();
                                        }
                                    }
                                }
                            
                                //NavMeshPath path = new NavMeshPath();

                                //if (NavMesh.CalculatePath(transform.position, dir, NavMesh.AllAreas, path))
                                //{
                                //    if (path.status == NavMeshPathStatus.PathComplete)
                                //    {
                                //        Debug.Log("ALL GOOD");
                                //        IsPathPartial = false;
                                //    }
                                //    else
                                //    {
                                //        Debug.Log("ALL GOOD BUT PATH IS PARTIAL");
                                //        IsPathPartial = true;
                                //    }
                                //}
                                //else
                                //{
                                //    Debug.Log("PATH IS INVALID");
                                //    StopBotForShoot = false;
                                //    IsPathPartial = true;
                                //}


                                if (pathfinder.PathIsUnreachable == false && CanStopCharge == false)
                                {
                                    StrafingForChargingAgent = false;
                                    IsAnyTaskCurrentlyRunning = true;
                                    if (AiCharging.EnableSprinting == false)
                                    {
                                        RandomChargeSprintCheck = DistanceCheckForCharging.sqrMagnitude + 1000f;
                                    }

                                    AiCharging.DebugDistances.DebugDistanceToTarget = DistanceCheckForCharging.magnitude;

                                    if (DistanceCheckForCharging.sqrMagnitude >= RandomChargeSprintCheck * RandomChargeSprintCheck && StopMoving == false && AiCharging.EnableSprinting == true && OverriteRunning == false)
                                    {
                                        StopBotForShoot = false;
                                        //if (pathfinder.IsPathComplete == true)
                                        //{
                                        if (Components.HumanoidFiringBehaviourComponent != null)
                                        {
                                            if (!Components.HumanoidFiringBehaviourComponent.isreloading && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false
                                                && pathfinder.NavMeshAgentComponent.enabled == true)
                                            {
                                                enableIkupperbodyRotations(ref ActivateSprintingIk);
                                                LookingAtEnemy();
                                                StopSpineRotation = true;
                                                // AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);
                                                IsCrouched = false;
                                                // RotatingTransforms.ChangeRotation(transform, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, transform.position, MovementSpeeds.BodyRotationSpeed);
                                                if (Components.NavMeshAgentComponent.enabled == true)
                                                {
                                                    Components.NavMeshAgentComponent.isStopped = false;
                                                }
                                                AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, true);
                                                SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                                                Components.HumanoidFiringBehaviourComponent.FireNow = false;
                                                Info("Sprinting Towards Enemy");
                                                if (StartShootingNow == false && DistanceCheckForCharging.sqrMagnitude <= RandomDistanceForShootBetweenSprint * RandomDistanceForShootBetweenSprint && AiCharging.OpenFireBehaviour.StopAndShootProbability >= 1)
                                                {
                                                    StartCoroutine(CoroForBetweenShots());
                                                    StartShootingNow = true;
                                                }
                                                // enableIkupperbodyRotations(ref ActivateSprintingIk);
                                            }
                                            else if(Components.HumanoidFiringBehaviourComponent.isreloading && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                                            {
                                                StopBot();
                                                FullUpperAndLowerBodyReload();
                                            }
                                        }
                                        else
                                        {
                                            enableIkupperbodyRotations(ref ActivateSprintingIk);
                                            LookingAtEnemy();
                                            StopSpineRotation = true;
                                            // AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);
                                            IsCrouched = false;
                                            // RotatingTransforms.ChangeRotation(transform, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, transform.position, MovementSpeeds.BodyRotationSpeed);
                                            if (Components.NavMeshAgentComponent.enabled == true)
                                            {
                                                Components.NavMeshAgentComponent.isStopped = false;
                                            }
                                            AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, true);
                                            SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                                            Info("Sprinting Towards Enemy");
                                            if (StartShootingNow == false && DistanceCheckForCharging.sqrMagnitude <= RandomDistanceForShootBetweenSprint * RandomDistanceForShootBetweenSprint && AiCharging.OpenFireBehaviour.StopAndShootProbability >= 1)
                                            {
                                                StartCoroutine(CoroForBetweenShots());
                                                StartShootingNow = true;
                                            }
                                        }

                                        //}
                                        //else
                                        //{

                                        //    Fire();
                                        //    DebugInfo.CurrentState = "SHOOTING";
                                        //    if (Components.HumanoidFiringBehaviourComponent.isreloading)
                                        //    {
                                        //        Reload();
                                        //    }
                                        //}
                                    }
                                    else if (DistanceCheckForCharging.sqrMagnitude < RandomChargeSprintCheck * RandomChargeSprintCheck && DistanceCheckForCharging.sqrMagnitude >= RandomChargeRunningCheck * RandomChargeRunningCheck
                                         && StopMoving == false && OverWriteWalking == false || OverriteRunning == true && OverWriteWalking == false)
                                    {
                                        StopBotForShoot = false;
                                        if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false && pathfinder.NavMeshAgentComponent.enabled == true)
                                        {
                                            OverriteRunning = true;
                                            LookingAtEnemy();
                                            Info("Running Towards Enemy");
                                            Run(pathfinder.closestPoint, true);

                                            
                                            if (Components.HumanoidFiringBehaviourComponent != null)
                                            {
                                                if (Components.HumanoidFiringBehaviourComponent.isreloading)
                                                {
                                                    OnlyUpperBodyReload();
                                                }
                                            }
                                            if (DistanceCheckForCharging.sqrMagnitude >= RandomChargeSprintCheck * RandomChargeSprintCheck)
                                            {
                                                OverriteRunning = false;
                                            }

                                            if (DistanceCheckForCharging.sqrMagnitude < RandomChargeRunningCheck * RandomChargeRunningCheck)
                                            {
                                                OverriteRunning = false;
                                            }
                                            if (StartShootingNow == false && DistanceCheckForCharging.sqrMagnitude <= RandomDistanceForShootBetweenSprint * RandomDistanceForShootBetweenSprint && AiCharging.OpenFireBehaviour.StopAndShootProbability >= 1)
                                            {
                                                StartCoroutine(CoroForBetweenShots());
                                                StartShootingNow = true;
                                            }
                                        }
                                      

                                    }
                                    else if (DistanceCheckForCharging.sqrMagnitude < RandomChargeRunningCheck * RandomChargeRunningCheck && DistanceCheckForCharging.sqrMagnitude >= StoppingDistanceForChargeBot * StoppingDistanceForChargeBot && StopMoving == false
                                        || OverWriteWalking == true && StopMoving == false)
                                    {
                                        StopBotForShoot = false;
                                        if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false && pathfinder.NavMeshAgentComponent.enabled == true)
                                        {
                                            OverWriteWalking = true;
                                            OverriteRunning = false;
                                            LookingAtEnemy();
                                            Info("Charging Towards Enemy");
                                            WalkForward(pathfinder.closestPoint, true);

                                            if (Components.HumanoidFiringBehaviourComponent != null)
                                            {
                                                if (Components.HumanoidFiringBehaviourComponent.isreloading)
                                                {
                                                    OnlyUpperBodyReload();
                                                }
                                            }
                                            if (DistanceCheckForCharging.sqrMagnitude >= RandomChargeRunningCheck * RandomChargeRunningCheck)
                                            {
                                                OverWriteWalking = false;
                                            }

                                            if (DistanceCheckForCharging.sqrMagnitude <= StoppingDistanceForChargeBot * StoppingDistanceForChargeBot)
                                            {
                                                OverWriteWalking = false;
                                            }
                                            if (StartShootingNow == false && DistanceCheckForCharging.sqrMagnitude <= RandomDistanceForShootBetweenSprint * RandomDistanceForShootBetweenSprint && AiCharging.OpenFireBehaviour.StopAndShootProbability >= 1)
                                            {
                                                StartCoroutine(CoroForBetweenShots());
                                                StartShootingNow = true;
                                            }
                                        }
                                     
                                    }
                                    else if (DistanceCheckForCharging.sqrMagnitude < RandomChargeRunningCheck * RandomChargeRunningCheck || StopMoving == true)
                                    {
                                        pathfinder.PreviousDestination = transform.position;
                                        pathfinder.IsSameDestination = transform.position;
                                        StopBotForShoot = true;

                                        if (StopMoving == true)
                                        {
                                            if (CanAIStrafeByDefault == true)
                                            {
                                                OverWriteWalking = false;
                                                Fire();
                                                if (Components.HumanoidFiringBehaviourComponent != null)
                                                {
                                                    if (Components.HumanoidFiringBehaviourComponent.isreloading)
                                                    {
                                                        OnlyUpperBodyReload();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                OverWriteWalking = false;
                                                Info("Stand Shooting");
                                                if (Components.HumanoidFiringBehaviourComponent != null)
                                                {
                                                    //if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
                                                    //{
                                                        SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                                                    //}

                                                    if (Components.HumanoidFiringBehaviourComponent.isreloading)
                                                    {
                                                        FullUpperAndLowerBodyReload();
                                                    }
                                                }
                                                AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);


                                            }
                                        }
                                        else
                                        {
                                            OverWriteWalking = false;
                                            Fire();
                                            if (Components.HumanoidFiringBehaviourComponent != null)
                                            {
                                                if (Components.HumanoidFiringBehaviourComponent.isreloading)
                                                {
                                                    OnlyUpperBodyReload();
                                                }
                                            }
                                        }
                                        IsTaskOver = true;
                                    }
                                    //}
                                    //else
                                    //{
                                    //    Reload();
                                    //    DebugInfo.CurrentState = "RELOADING";
                                    //}
                                }

                                else if(pathfinder.PathIsUnreachable == true && CanStopCharge == false && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                                {
                                    //CanStopCharge = true; // commented so that strafing can be continued when path is unreachable.
                                    PreviousEnemyDuringCharging = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy];

                                    // These two lines were added when testing multiple combat states on 2 sides of the roof with Team 1 and Team 2 soldiers and when they become charging Agent if they can strafe
                                    // using these two lines we will be able to activate only strafing behaviour on the AI agent. If commented charging agent will always be performing stationed shooting.
                                    IsGoingToPreviousPoint = false;
                                    VisibilityCheck.DebugNumberOfRaycasts = 0;

                                    StrafingForChargingAgent = true;

                                    Info("Stand Shooting");
                                    RandomStrafing();
                                }
                      


                            }
                            else
                            {
                                SprintAwayFromGrenade();
                            }

                        }


                    }
                    else
                    {
                        if (BotMovingAwayFromGrenade == false)
                        {
                            if (HealthScript.CompleteFirstHitAnimation == false)
                            {
                                if (VisibilityCheck.ConnectionLost == true)
                                {
                                    if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableApproachingEnemyPursue
                                         || HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableStationedEnemyPursue)
                                    {
                                        ConnectionLostForMovingBots();
                                    }
                                    //else if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableStationedEnemyPursue)
                                    //{
                                    //    ConnectionLostForStationarybot();
                                    //}
                                    else if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.DoNotPursueEnemy)
                                    {
                                        FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
                                        VisibilityCheck.ConnectionLost = false;
                                        StaringAtLastEnemyLostPosition = false;
                                    }
                                }
                                else
                                {
                                    DefaultInvestigationBehaviour();
                                }
                            }
                        }
                        else
                        {
                            SprintAwayFromGrenade();
                        }

                    }
                }
                else
                {
                    //if (IsNearDeadBody == true && NonCombatBehaviours.EnableEmergencyAlerts == true && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != transform.root.transform)
                    //{
                    //    CompleteEmergencyStateBehaviour();
                    //}
                    //else
                    //{
                    //    if (SearchingForSound == false)
                    //    {
                    //        FindImmediateEnemy();
                    //        DefaultInvestigationBehaviour();
                    //    }
                    //}

                    if (IsNearDeadBody == true && NonCombatBehaviours.EnableEmergencyAlerts == true && IsInEmergencyState == true && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != transform.root.transform
                      && HealthScript.CompleteFirstHitAnimation == false)
                    {
                        CompleteEmergencyStateBehaviour();
                    }
                    else if (IsNearDeadBody == true && NonCombatBehaviours.EnableDeadBodyAlerts == true && IsInEmergencyState == false && IsEmergencyStateCurrentlyActive == false
                        && HealthScript.CompleteFirstHitAnimation == false)//FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != transform.root.transform && 
                    {
                        DeadBodyInvestigation();
                    }
                    else
                    {
                        if (SearchingForSound == false && HealthScript.CompleteFirstHitAnimation == false)
                        {
                            // This code is commented on 14th Jan 2025 because when in beginning there was a player and turret and AI agent . The AI agent removed its enemies which was Player and Turret and ignored them basically
                           // and did not updated its enemy and did not shoot even.
                           // FindImmediateEnemy();
                            DefaultInvestigationBehaviour();
                        }
                    }
                }
            }
            else
            {
                DefaultInvestigationBehaviour();
            }
        }
        public void ZombieChargingBehaviour()
        {
            if (InitialiseChargingBehaviour == false && Iscombatstatebegins == true)
            {
                RandomChargeRunningCheck = Random.Range(ZombieCharging.MinDistanceToRun, ZombieCharging.MaxDistanceToRun);
                RandomWalkChargeDistance = Random.Range(ZombieCharging.MinDistanceToWalk, ZombieCharging.MaxDistanceToWalk);
                InitialiseChargingBehaviour = true;
            }

            if (FindEnemiesScript.FindedEnemies == true)
            {
                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null
                    && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.transform != this.transform)
                {
                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<HumanoidAiHealth>() != null)
                    {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<HumanoidAiHealth>().IsDied == true)
                        {
                            FindImmediateEnemy();
                        }

                    }
                    else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.tag == "Player")
                    {
                        if (PlayerHealth.instance != null)
                        {
                            if (PlayerHealth.instance.IsDead == true)
                            {
                                FindImmediateEnemy();
                            }
                        }
                    }
                    else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<Turret>() != null)
                    {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<Turret>().IsDied == true)
                        {
                            FindImmediateEnemy();
                        }

                    }

                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.tag == "Player")
                    {
                        StoppingDistanceForChargeBot = RandomDistanceWithPlayerToAttack;
                    }
                    else
                    {
                        StoppingDistanceForChargeBot = RandomDistanceToEnemyAiAgentToAttack;
                    }

                    CheckFieldofView();
                    // Vector3 DisWithEnemy = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - this.transform.position;
                    // DisWithEnemy.magnitude < FindEnemiesScript.DetectionRadius 
                    if (Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, this.transform.position) < FindEnemiesScript.DetectionRadius && IsEnemyLocked == true && VisibilityCheck.CanSeeTarget(FindEnemiesScript.OriginalFov, FindEnemiesScript.DetectionRadius, Components.HumanoidFiringBehaviourComponent) && HealthScript.CompleteFirstHitAnimation == false)
                    {
                        CheckForLockedTargetInFOV();
                        if (PreviousEnemyDuringCharging != FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy])
                        {
                            QuickDistanceCheckWithEnemy();

                            DistanceCheckForCharging = new Vector3(pathfinder.closestPoint.x - transform.position.x, 0f, pathfinder.closestPoint.z - transform.position.z);
                            DistanceCheckForCharging.y = 0;
                          
                            // Added while recording tutorial as this way the distance check will be equal to how we are doing the distance check when zombie perform melee attack. for example if melee attack start when distance
                            // is less than 1 meter than we should make sure DistanceCheckForCharging.magnitude <= StoppingDistanceForChargeBot is true in line number 11030
                            // DistanceCheckForCharging = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - this.transform.position;  // new Vector3(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x - transform.position.x, 0f, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.z - transform.position.z);
                            DistanceCheckForZombieMeleeCharging = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - this.transform.position;
                        }
                        //if (pathfinder.PathIsUnreachable == false)// && StopMeleeAttack == false) // before here was pathfinder.PathIsUnreachable had to replace because zombie was constantly running.
                        //{
                            if (DistanceCheckForCharging.magnitude >= RandomWalkChargeDistance && DistanceCheckForCharging.magnitude >= RandomChargeRunningCheck)
                            {
                                if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false
                                    && pathfinder.NavMeshAgentComponent.enabled == true)
                                {
                                   // LookingAtEnemy(); // Zombie need to look at the moving path not the enemy.
                                    if (Components.NavMeshAgentComponent.enabled == true)
                                    {
                                        Components.NavMeshAgentComponent.isStopped = false;
                                    }
                                    AgentMovement(Components.NavMeshAgentComponent, ZombieSpeeds.MovementSpeeds.SprintSpeed, true);
                                    SetAnimationForFullZombieBody(ZombieAiAnimatorParameters.SprintingParameterName);
                                    Info("Sprinting Towards Enemy");
                                }
                            }
                            else if (DistanceCheckForCharging.magnitude >= RandomWalkChargeDistance && DistanceCheckForCharging.magnitude < RandomChargeRunningCheck)
                            {
                                if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false
                                    && pathfinder.NavMeshAgentComponent.enabled == true)
                                {
                                  //  LookingAtEnemy();
                                    if (Components.NavMeshAgentComponent.enabled == true)
                                    {
                                        Components.NavMeshAgentComponent.isStopped = false;
                                    }
                                    AgentMovement(Components.NavMeshAgentComponent, ZombieSpeeds.MovementSpeeds.RunForwardSpeed, true);
                                    SetAnimationForFullZombieBody(ZombieAiAnimatorParameters.RunForwardParameterName);
                                    Info("Running Towards Enemy");
                                }
                            }
                            else if (DistanceCheckForCharging.magnitude <= RandomWalkChargeDistance && DistanceCheckForCharging.magnitude > StoppingDistanceForChargeBot)
                            {
                                if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false
                                    && pathfinder.NavMeshAgentComponent.enabled == true)
                                {
                                  //  LookingAtEnemy();
                                    if (Components.NavMeshAgentComponent.enabled == true)
                                    {
                                        Components.NavMeshAgentComponent.isStopped = false;
                                    }
                                    AgentMovement(Components.NavMeshAgentComponent, ZombieSpeeds.MovementSpeeds.WalkForwardSpeed, true);
                                    SetAnimationForFullZombieBody(ZombieAiAnimatorParameters.WalkForwardParameterName);
                                    Info("Walking Towards Enemy");
                                }
                            }
                            else if (DistanceCheckForZombieMeleeCharging.magnitude <= StoppingDistanceForChargeBot)
                            {
                                Info("Melee Attack");
                                Melee();
                                if (Components.NavMeshAgentComponent.enabled == true)
                                {
                                    Components.NavMeshAgentComponent.speed = 0f;
                                    Components.NavMeshAgentComponent.isStopped = true;
                                }
                            }
                            else if (DistanceCheckForCharging.magnitude <= 0.2f)
                            {
                               
                                DefaultInvestigationBehaviour();
                            }
                        //}
                        //else
                        //{
                           
                        //    DefaultInvestigationBehaviour();
                        //}
                    }
                    else
                    {
                        if (HealthScript.CompleteFirstHitAnimation == false)
                        {
                            if (VisibilityCheck.ConnectionLost == true)
                            {
                                if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableApproachingEnemyPursue
                                         || HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableStationedEnemyPursue)
                                {
                                    ConnectionLostForMovingBots();
                                }
                                //else if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.EnableStationedEnemyPursue)
                                //{
                                //    ConnectionLostForStationarybot();
                                //}
                                else if (HealthScript.CompleteFirstHitAnimation == false && CombatStateBehaviours.ChooseEnemyPursueType == EnemyPursueTypes.DoNotPursueEnemy)
                                {
                                    FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
                                    VisibilityCheck.ConnectionLost = false;
                                    StaringAtLastEnemyLostPosition = false;
                                }
                            }
                            else
                            {
                                DefaultInvestigationBehaviour();
                            }
                        }
                    }
                }
                else
                {
                    if (SearchingForSound == false)
                    {
                        FindImmediateEnemy();
                        DefaultInvestigationBehaviour();// important to uncomment and use this code as when there is no enemy we add zombie in the list so list of find enemies script so that the list don't stay empty and the zombie will be able to wander or play defaut behaviour.
                    }
                }
            }
            else
            {
                DefaultInvestigationBehaviour();
            }
        }
        //public void MasterShooter()
        //{
        //    if (HealthScript.IsDied == false)
        //    {
        //        Timer += Time.deltaTime;

        //        if (ResetCoverRandomisation == false)
        //        {
        //            SaveResetedCoverRandomisation = Random.Range(AiCovers.MinimumTimeBetweenCovers, AiCovers.MaximumTimeBetweenCovers);
        //            ResetCoverRandomisation = true;
        //        }

        //        if (FindEnemiesScript.FindedEnemies == true)
        //        {
        //            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.transform != this.transform)
        //            {
        //                CheckFieldofView();

        //                if (Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, this.transform.position) < Detections.DetectionRadius && HealthScript.IsDied == false && NoEnemyInView == false && FOV.CanSeeTarget(Detections.EnableFieldOfView, Detections.FieldOfViewValue, Detections.DetectionRadius, ResetToNormalState, pathfinder.CalculatePathOnce, Components.HumanoidFiringBehaviourComponent) && HealthScript.CompleteFirstHitAnimation == false)
        //                {
        //                    CheckForLockedTargetInFOV();

        //                    if (StartStrafingForAvoidingBots == true)
        //                    {
        //                        RandomStrafing();
        //                    }
        //                    else
        //                    {
        //                        if (BotMovingAway == false)
        //                        {
        //                            Vector3 dir = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - transform.position;
        //                            dir.y = 0;
        //                            if (!Components.HumanoidFiringBehaviourComponent.isreloading)
        //                            {
        //                                if (dir.sqrMagnitude >= RandomChargeDistance * RandomChargeDistance || OverWriteCharging == true)
        //                                {
        //                                    OverWriteCharging = true;
        //                                    pathfinder.CalculatePathForCombat(0f, 0f, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position);
        //                                    if (pathfinder.IsPathComplete == true)
        //                                    {
        //                                        Run();
        //                                        DebugInfo.CurrentState = "RUNNING";
        //                                    }
        //                                    else
        //                                    {
        //                                        Fire();
        //                                        DebugInfo.CurrentState = "SHOOTING";
        //                                    }

        //                                    if (dir.sqrMagnitude <= StoppingDistanceForChargeBot * StoppingDistanceForChargeBot)
        //                                    {
        //                                        OverWriteCharging = false;
        //                                    }
        //                                }
        //                                else if (dir.sqrMagnitude < RandomChargeDistance * RandomChargeDistance)
        //                                {
        //                                    OverWriteCharging = false;
        //                                    // Fire();
        //                                    if (CrouchPositions.Count <= 0)
        //                                    {
        //                                        Debug.Log("I AM CALLING" + transform.name);
        //                                        RandomStrafing();
        //                                    }
        //                                    else
        //                                    {
        //                                        Debug.Log("TAKING COVER" + transform.name);
        //                                        AiCoverMethod();
        //                                    }
        //                                    DebugInfo.CurrentState = "SHOOTING";
        //                                }
        //                            }
        //                            else
        //                            {
        //                                Reload();
        //                                DebugInfo.CurrentState = "RELOADING";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            SprintAwayFromGrenade();
        //                        }

        //                    }


        //                }
        //                else
        //                {
        //                    if (HealthScript.CompleteFirstHitAnimation == false)
        //                    {
        //                        ConnectionLostForMovingBots();
        //                    }

        //                }
        //            }
        //            else
        //            {
        //                if (IsNearDeadBody == true)
        //                {
        //                    GoNearDeadBody();
        //                }
        //                else
        //                {
        //                    if (SearchingForSound == false)
        //                    {
        //                        FindImmediateEnemy();
        //                        if (PatrolingScript != null)
        //                        {
        //                            PatrolingScript.WanderAndPatrol();
        //                        }
        //                        else
        //                        {
        //                            SearchingState();
        //                        }
        //                    }
        //                }


        //            }
        //        }
        //        else
        //        {
        //            if (PatrolingScript != null)
        //            {
        //                PatrolingScript.WanderAndPatrol();
        //            }
        //            else
        //            {
        //                SearchingState();
        //            }


        //        }
        //    }
        //    else
        //    {
        //        Components.NavMeshAgentComponent.isStopped = true;
        //        Components.NavMeshAgentComponent.speed = 0;
        //    }
        //}
        //public void AlertedStateAfterSoundHeardController()
        //{
        //    Vector3 dis;
        //    dis = RandomDirAfterHearing - transform.position;
        //    DebugInfo.CurrentState = "MOVING TO SOUND HEARD POSITION";
        //    if ((dis.sqrMagnitude > StoppingHearingSound * StoppingHearingSound))
        //    {

        //        ReachnewCoverpoint = false;
        //        ReachnewFirepoint = false;
        //        Components.NavMeshAgentComponent.speed = Speeds.SprintSpeed;
        //        Components.NavMeshAgentComponent.isStopped = false;
        //        if (anim != null)
        //        {
        //            SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
        //        }
        //    }
        //    else if (dis.sqrMagnitude < StoppingHearingSound * StoppingHearingSound && dis.sqrMagnitude >= 3 * 3)
        //    {
        //        ApplyRootMotion(false);
        //        ReachnewCoverpoint = false;
        //        ReachnewFirepoint = false;
        //        Components.NavMeshAgentComponent.speed = Speeds.RunForwardSpeed;
        //        Components.NavMeshAgentComponent.isStopped = false;
        //        if (anim != null)
        //        {
        //            SetAnimationForFullBody(AiAgentAnimatorParameters.WalkForwardParameterName);
        //            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
        //        }
        //    }
        //    else if (dis.sqrMagnitude < 3 * 3)
        //    {
        //        if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
        //        {
        //            SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
        //        }
        //        Components.NavMeshAgentComponent.isStopped = true;
        //        CheckForCombat.instance.IsShootingStarted = false;
        //        Gunscript.instance.IsFire = false;
        //    }
        //}
        public void AlertedStateAfterTargetLostController()
        {
            ReachnewCoverpoint = false;
            ReachnewFirepoint = false;
            if (Components.NavMeshAgentComponent.enabled == true)
            {
                Components.NavMeshAgentComponent.isStopped = false;
            }
            AiPursue.DebugPursueDistances.DebugDistanceToCoordinate = DistanceBetweenMeAndEnemyLastLocation.magnitude;
            

            if (DistanceBetweenMeAndEnemyLastLocation.magnitude <= EnemyPursueDistanceToWalk && DistanceBetweenMeAndEnemyLastLocation.magnitude > ClosestDistanceToStopFromCoordinate)
            {
                if(AgentRole == Role.Zombie)
                {
                    Info("Walking Near the enemy pursue coordinate" + " " + "DistanceLeft" + DistanceBetweenMeAndEnemyLastLocation.magnitude);
                    AgentMovement(Components.NavMeshAgentComponent, ZombieSpeeds.MovementSpeeds.WalkForwardSpeed, false);
                    enableIkupperbodyRotations(ref ActivateWalkAimIk);
                    if (anim != null)
                    {
                        SetAnimationForFullZombieBody(ZombieAiAnimatorParameters.WalkForwardParameterName);
                    }
                }
                else
                {
                    Info("Walking Near the enemy pursue coordinate" + " " + "DistanceLeft" + DistanceBetweenMeAndEnemyLastLocation.magnitude);
                    AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.WalkForwardAimingSpeed, false);
                    enableIkupperbodyRotations(ref ActivateWalkAimIk);
                    if (anim != null)
                    {
                        SetAnimationForFullBody(AiAgentAnimatorParameters.WalkForwardParameterName);
                        SetAnimationForUpperBody("StandShootPosture");
                        anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                    }
                }
               
            }
            else if (DistanceBetweenMeAndEnemyLastLocation.magnitude >= EnemyPursueDistanceToWalk && DistanceBetweenMeAndEnemyLastLocation.magnitude <= EnemyPursueDistanceToRun)
            {

                if (AgentRole == Role.Zombie)
                {
                    Components.NavMeshAgentComponent.speed = ZombieSpeeds.MovementSpeeds.RunForwardSpeed;
                    Info("Running Near the enemy pursue coordinate" + " " + "DistanceLeft" + DistanceBetweenMeAndEnemyLastLocation.magnitude);
                    enableIkupperbodyRotations(ref ActivateRunningIk);
                    Run(pathfinder.closestPoint, false);
                }
                else
                {
                    Components.NavMeshAgentComponent.speed = Speeds.MovementSpeeds.RunForwardSpeed;
                    Info("Running Near the enemy pursue coordinate" + " " + "DistanceLeft" + DistanceBetweenMeAndEnemyLastLocation.magnitude);
                    enableIkupperbodyRotations(ref ActivateRunningIk);
                    Run(pathfinder.closestPoint, false);
                }
              
            }
            else if (DistanceBetweenMeAndEnemyLastLocation.magnitude >= EnemyPursueDistanceToRun)
            {
                if (AgentRole == Role.Zombie)
                {
                    AgentMovement(Components.NavMeshAgentComponent, ZombieSpeeds.MovementSpeeds.SprintSpeed, false);
                    Info("Sprinting towards the enemy pursue coordinate" + " " + "DistanceLeft" + DistanceBetweenMeAndEnemyLastLocation.magnitude);
                    SetAnimationForFullZombieBody(ZombieAiAnimatorParameters.SprintingParameterName);
                   
                }
                else
                {
                    AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, false);
                    Info("Sprinting towards the enemy pursue coordinate" + " " + "DistanceLeft" + DistanceBetweenMeAndEnemyLastLocation.magnitude);
                    enableIkupperbodyRotations(ref ActivateNoIk);
                    StopSpineRotation = true;
                    IsCrouched = false;
                    SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                    if (Components.HumanoidFiringBehaviourComponent != null)
                    {
                        Components.HumanoidFiringBehaviourComponent.FireNow = false;
                    }
                }
               
            }
        }
        public void AlertedStateControllerForStationaryBot()
        {
            ReachnewCoverpoint = false;
            ReachnewFirepoint = false;
            if (anim != null)
            {
                //if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
                //{
                    SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                //}
                anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
            }
        }
        bool IsAnimationPlaying(Animator anim, string animationName)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);  // 0 is the layer index, use the appropriate layer index if needed
            return stateInfo.IsName(animationName);
        }
        void StartSearching()
        {
            if (ScanScript != null)
            {
                ActivateWalkIk = false;
                ActivateRunningIk = false;
                ActivateSprintingIk = false;
                IsScanning = true;
                if (TimeToDetect == false)
                {
                    //ReinitializeScanPoint();
                    TimeToDetect = true;
                    CombatStarted = false;
//                    Debug.Log("Searching For Sound 2" + transform.name);
                    SearchingForSound = false;
                    //VisibilityCheck.ConnectionLost = false; // commented on 07/01/2024
                    if (WasInCombatStateBefore == true)
                    {
                        StartCoroutine(CheckForSearchingAnimations(PostCombatAimedScanBehaviour.MinTimeBetweenAimedTurns, PostCombatAimedScanBehaviour.MaxTimeBetweenAimedTurns));
                    }
                    else  if (VisibilityCheck.ConnectionLost == true)
                    {
                        StartCoroutine(CheckForSearchingAnimations(AiPursue.AimedScaningAtEnemyLastKnownPosition.MinTimeBetweenAimedTurns, AiPursue.AimedScaningAtEnemyLastKnownPosition.MaxTimeBetweenAimedTurns));
                    }
                    else  
                    {
                        StartCoroutine(CheckForSearchingAnimations(ScanScript.MinTimeBetweenTurns,ScanScript.MaxTimeBetweenTurns));
                    }
                }

                if (Detect == true)
                {
                    if(Components.NavMeshAgentComponent.enabled == true)
                    {
                        Components.NavMeshAgentComponent.isStopped = true;
                    }

                    //if (ScanScript.UseSearchingAnimations == true)
                    //{
                    if (CanPlayDetectanim == false && HealthScript.CompleteFirstHitAnimation == false)// && ScanScript.IsScanning == true)
                    {
                        if (WasInCombatStateBefore == true)
                        {
                            if (PostCombatAimedScanBehaviour.PlayAnimationsSequentially == true)
                            {
                                if (ScanValueAfterCombat < PostCombatAimedScanBehaviour.AimedTurningAnimations.Count)
                                {
                                    ++ScanValueAfterCombat;

                                    if (ScanValueAfterCombat >= PostCombatAimedScanBehaviour.AimedTurningAnimations.Count)
                                    {
                                        ScanValueAfterCombat = 0;
                                    }
                                }
                                else
                                {
                                    if (ScanValueAfterCombat >= PostCombatAimedScanBehaviour.AimedTurningAnimations.Count)
                                    {
                                        ScanValueAfterCombat = 0;
                                    }
                                }
                                ScanScript.PickDefaultScanAnimationToPlay(PostCombatAimedScanBehaviour.AimedTurningAnimations[ScanValueAfterCombat].TurnDirection.ToString());
                            }
                            else
                            {
                                int Randomise = Random.Range(0, PostCombatAimedScanBehaviour.AimedTurningAnimations.Count);
                                ScanScript.PickDefaultScanAnimationToPlay(PostCombatAimedScanBehaviour.AimedTurningAnimations[Randomise].TurnDirection.ToString());
                            }

                           
                        }
                        else if (VisibilityCheck.ConnectionLost == true)
                        {
                            if (AiPursue.AimedScaningAtEnemyLastKnownPosition.PlayAnimationsSequentially == true)
                            {
                                if (ScanValueAfterPursue < AiPursue.AimedScaningAtEnemyLastKnownPosition.AimedTurningAnimations.Count)
                                {
                                    ++ScanValueAfterPursue;

                                    if (ScanValueAfterPursue >= AiPursue.AimedScaningAtEnemyLastKnownPosition.AimedTurningAnimations.Count)
                                    {
                                        ScanValueAfterPursue = 0;
                                    }
                                }
                                else
                                {
                                    if (ScanValueAfterPursue >= AiPursue.AimedScaningAtEnemyLastKnownPosition.AimedTurningAnimations.Count)
                                    {
                                        ScanValueAfterPursue = 0;
                                    }
                                }
                                ScanScript.PickDefaultScanAnimationToPlay(AiPursue.AimedScaningAtEnemyLastKnownPosition.AimedTurningAnimations[ScanValueAfterPursue].TurnDirection.ToString());
                            }
                            else
                            {
                                int Randomise = Random.Range(0, AiPursue.AimedScaningAtEnemyLastKnownPosition.AimedTurningAnimations.Count);
                                ScanScript.PickDefaultScanAnimationToPlay(AiPursue.AimedScaningAtEnemyLastKnownPosition.AimedTurningAnimations[Randomise].TurnDirection.ToString());
                            }
                        }
                        else
                        {
                            ScanScript.FindScanPoint();
                        }

                        Info("Stand Scan" + "-" + ScanScript.ScanPointChosenName);
                        ApplyRootMotion(true);
                        ResetAnimator = true;
                        
                        string AnimationParameterName = "";

                        if (ScanScript.ChosenAnimationName == ScanningScript.AnimationDirectionName.Forward.ToString())
                        {
                            AnimationParameterName = "TurnForward";
                        }
                        if (ScanScript.ChosenAnimationName == ScanningScript.AnimationDirectionName.Backward.ToString())
                        {
                            AnimationParameterName = "TurnBackward";                        
                        }
                        if (ScanScript.ChosenAnimationName == ScanningScript.AnimationDirectionName.Left.ToString())
                        { 
                            AnimationParameterName = "TurnLeft";                           
                        }
                        if (ScanScript.ChosenAnimationName == ScanningScript.AnimationDirectionName.Right.ToString())
                        { 
                            AnimationParameterName = "TurnRight";
                        }

                       

                        if (AnimationParameterName == PreviousScanAnimationName && IsAnimationPlaying(anim, AnimationParameterName))
                        {
                            anim.SetInteger("Scan", 100);
                            anim.Play(AnimationParameterName, -1, 0f);
                        }
                        else
                        {
                            anim.SetInteger("Scan", ScanScript.TransitionVal);
                        }

                        PreviousScanAnimationName = AnimationParameterName;

                        if (WasInCombatStateBefore == true)
                        {
                            enableIkupperbodyRotations(ref ActivateScanIk);
                            if (AgentRole != Role.Zombie)
                            {
                                SetAnimationForFullBody("UBScan");
                                SetAnimationForUpperBody("UBScan");
                            }
                            else
                            {
                                SetAnimationForFullZombieBody("NULL");
                            }
                            float RandomTurnAnimationSpeed = Random.Range(PostCombatAimedScanBehaviour.AimedTurningAnimations[ScanValueAfterCombat].MinAimedTurnAnimationSpeed, PostCombatAimedScanBehaviour.AimedTurningAnimations[ScanValueAfterCombat].MaxAimedTurnAnimationSpeed);
                            anim.SetFloat(AnimationParameterName, RandomTurnAnimationSpeed);
                        }
                        else if (VisibilityCheck.ConnectionLost == true)
                        {
                            enableIkupperbodyRotations(ref ActivateScanIk);
                            if (AgentRole != Role.Zombie)
                            {
                                SetAnimationForFullBody("UBScan");
                                SetAnimationForUpperBody("UBScan");
                            }
                            else
                            {
                                SetAnimationForFullZombieBody("NULL");
                            }
                            float RandomTurnAnimationSpeed = Random.Range(AiPursue.AimedScaningAtEnemyLastKnownPosition.AimedTurningAnimations[ScanValueAfterPursue].MinAimedTurnAnimationSpeed, AiPursue.AimedScaningAtEnemyLastKnownPosition.AimedTurningAnimations[ScanValueAfterPursue].MaxAimedTurnAnimationSpeed);
                            anim.SetFloat(AnimationParameterName, RandomTurnAnimationSpeed);
                        }
                        else
                        {
                            int Randomise = Random.Range(0, 100);

                            if (ScanScript.AimedScanProbability >= Randomise)
                            {
                                if (ScanScript.StopHeadTurnsWhenAiming == true)
                                {
                                    if (ScanScript.HeadIKScript != null)
                                    {
                                        ScanScript.HeadIKScript.StopTurning = true;
                                    }
                                }

                                if (ScanScript.OffsetScanAimPoint == true)
                                {
                                    if (ScanScript.HeadIKScript != null)
                                    {
                                        float GetXScanPoint = Random.Range(ScanScript.ScanAimPointOffset.MinX, ScanScript.ScanAimPointOffset.MaxX);
                                        float GetYScanPoint = Random.Range(ScanScript.ScanAimPointOffset.MinY, ScanScript.ScanAimPointOffset.MaxY);
                                        float GetZScanPoint = Random.Range(ScanScript.ScanAimPointOffset.MinZ, ScanScript.ScanAimPointOffset.MaxZ);
                                        if (ScanScript.AimedScanPoint != null)
                                        {
                                            ScanScript.AimedScanPoint.transform.localPosition = new Vector3(GetXScanPoint, GetYScanPoint, GetZScanPoint);
                                        }
                                    }
                                }

                                float RandomTurnAnimationSpeed = Random.Range(ScanScript.ScanTurnDirections[ScanScript.Index].MinAimedTurnSpeed, ScanScript.ScanTurnDirections[ScanScript.Index].MaxAimedTurnSpeed);
                                anim.SetFloat(AnimationParameterName, RandomTurnAnimationSpeed);

                                enableIkupperbodyRotations(ref ActivateScanIk);
                                if(AgentRole != Role.Zombie)
                                {
                                    SetAnimationForFullBody("UBScan");
                                    SetAnimationForUpperBody("UBScan");
                                }
                                else
                                {
                                    SetAnimationForFullZombieBody("NULL");
                                }
                            }
                            else
                            {
                                float RandomTurnAnimationSpeed = Random.Range(ScanScript.ScanTurnDirections[ScanScript.Index].MinTurnSpeed, ScanScript.ScanTurnDirections[ScanScript.Index].MaxTurnSpeed);
                                anim.SetFloat(AnimationParameterName, RandomTurnAnimationSpeed);
                                StartCoroutine(SmoothlyChangeToIdle());
                                //if (AgentRole == Role.Zombie)
                                //{
                                //    SetAnimationForFullZombieBody("NULL");
                                //}
                            }
                        }
                        
                        CanPlayDetectanim = true;
                    }                
                }
            }

        }
        IEnumerator SmoothlyChangeToIdle()
        {
            if (ScanScript.AimedScanPoint != null)
            {
                ScanScript.AimedScanPoint.transform.localPosition = ScanScript.DefaultScanAimingPointPosition;
            }
            yield return new WaitForSeconds(0.3f);
            if (ScanScript.StopHeadTurnsWhenAiming == true)
            {
                if (ScanScript.HeadIKScript != null)
                {
                    ScanScript.HeadIKScript.StopTurning = false;
                }
            }
            enableIkupperbodyRotations(ref ActivateNoIk);
            if (AgentRole != Role.Zombie)
            {
                SetAnimationForFullBody("UBScan");
                SetAnimationForUpperBody("UpperBodyIdle");
            }
            else
            {
                SetAnimationForFullZombieBody("NULL");
            }

        }
        IEnumerator CheckForSearchingAnimations(float MinimumTimeBetweenNewScanningTurn,float MaximumTimeBetweenNewScanningTurn)
        {
            float RandomSearchVal = 0f;
            if (ImmediateStartScanForTheFirstTime == true)
            {
                RandomSearchVal = Random.Range(MinimumTimeBetweenNewScanningTurn, MaximumTimeBetweenNewScanningTurn);
            }
            if (ImmediateStartScanForTheFirstTime == false)
            {
                ImmediateStartScanForTheFirstTime = true;
            }
            yield return new WaitForSeconds(RandomSearchVal);
            TimeToDetect = false;
            Detect = true;
            //if (ScanScript.UseSearchingAnimations == true)
            //{
            CanPlayDetectanim = false;
            //}
            //else
            //{
            //    DetectValue = Random.Range(ScanScript.MinimumSearchingInDegrees, ScanScript.MaximumSearchingInDegrees);
            //}
        }
        IEnumerator ResetShootChoice()
        {
            float NewTime = Random.Range(AiStrafing.MinTimeBetweenStopOrStrafeCycles, AiStrafing.MaxTimeBetweenStopOrStrafeCycles);
            yield return new WaitForSeconds(NewTime);
            IsStrafingEnabled = SaveStrafeData;
            CheckForStrafeOrStillShoot = false;
        }
        IEnumerator RandomiseSitORStand()
        {
            float PostureSelectTimer = Random.Range(StationedShooting.MinTimeToSwitchPosture, StationedShooting.MaxTimeToSwitchPosture);
            yield return new WaitForSeconds(PostureSelectTimer);
            ResetPostureTime = false;
        }
        public void StandShootRandomlyInSky()
        {
            IsShootingDuringEmergencyState = true;
            DisableNavmeshAgentcomponent();
            if (PreviousEmergencyCoverNode != null)
            {
                if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                {
                    if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().EmergencyCoverShootingPoints[ShootingPointForEmergencyCoverNode].tag == "CrouchFirePoint")
                    {
                        IsCrouched = true;
                        if (Components.HumanoidFiringBehaviourComponent != null)
                        {
                            if(Components.HumanoidFiringBehaviourComponent.IsWeaponReloading == false)
                            {
                                UseCrouchOrStandPosture = true;
                                anim.SetBool("Sprinting", false);
                                anim.SetBool("StandShootPosture", false);
                                anim.SetBool("CrouchShootPosture", true);
                                // SetAnimationForFullBody(AiAgentAnimatorParameters.CrouchFireParameterName);
                                // SetAnimationForUpperBody(AiAgentAnimatorParameters.CrouchFireParameterName);
                            }

                        }
                    }
                    else
                    {
                        IsCrouched = false;
                        if (Components.HumanoidFiringBehaviourComponent != null)
                        {
                            SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                            ConnectWithUpperBodyAimingAnimation();
                        }
                    }
                }
                else
                {
                    IsCrouched = true;
                    if (Components.HumanoidFiringBehaviourComponent != null)
                    {
                        if (Components.HumanoidFiringBehaviourComponent.IsWeaponReloading == false)
                        {
                            UseCrouchOrStandPosture = true;
                            anim.SetBool("Sprinting", false);
                            anim.SetBool("StandShootPosture", false);
                            anim.SetBool("CrouchShootPosture", true);

                        }
                    }
                }
            }
            else
            {
                IsCrouched = true;
                if (Components.HumanoidFiringBehaviourComponent != null)
                {
                    if (Components.HumanoidFiringBehaviourComponent.IsWeaponReloading == false)
                    {
                        UseCrouchOrStandPosture = true;
                        anim.SetBool("Sprinting", false);
                        anim.SetBool("StandShootPosture", false);
                        anim.SetBool("CrouchShootPosture", true);

                    }
                }
            }

            Components.HumanoidFiringBehaviourComponent.FireNow = true;
            StopSpineRotation = false;
            IsStationedShoot = true;
            
           
            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            {
                if(IsEmergencyStateWrongShootingStarted == false)
                {
                    TargetOffsetedPosition = StoredEnemyPositionDuringEmergency;
                    if (StoredEnemyPositionDuringEmergency == Vector3.zero)
                    {
                        TargetOffsetedPosition = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                    }

                    float WrongShootingOffsetx = Random.Range(AiEmergencyState.EmergencyAlert.EmergencyShooting.MinEmergencyShootingOffset.x,
                        AiEmergencyState.EmergencyAlert.EmergencyShooting.MaxEmergencyShootingOffset.x);
                    float WrongShootingOffsety = Random.Range(AiEmergencyState.EmergencyAlert.EmergencyShooting.MinEmergencyShootingOffset.y,
                        AiEmergencyState.EmergencyAlert.EmergencyShooting.MaxEmergencyShootingOffset.y);

                    // z is basically for horizontal axis and Y for vertical 
                    TargetOffsetedPosition = TargetOffsetedPosition + new Vector3(0f, WrongShootingOffsety, WrongShootingOffsetx);

                    StartCoroutine(CoroForWrongShooting());
                    IsEmergencyStateWrongShootingStarted = true;
                }

                if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                {
                    if (ResetEmergencyDecisionMaking == false)
                    {
                        StartCoroutine(CoroForNextDecsionAfterShooting());
                        ResetEmergencyDecisionMaking = true;
                    }
                }

                //if(AiEmergencyState.EmergencyAlert.EmergencyShooting.UpperBodyOnlyCrouchRotations   == true)
                //{
                //     LookingAtEnemy();
                //}
                //else
                //{
                    RotatingTransforms.ChangeRotation(transform, TargetOffsetedPosition, transform.position, Speeds.MovementSpeeds.BodyRotationSpeed);
                //}
            }

            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, false);

            if (Components.NavMeshAgentComponent.enabled == true)
            {
                if (Components.NavMeshAgentComponent != null)
                {
                    Components.NavMeshAgentComponent.speed = 0f;
                    Components.NavMeshAgentComponent.isStopped = true;
                }

            }
        }
        IEnumerator CoroForWrongShooting()
        {
            float RandomTime = Random.Range(AiEmergencyState.EmergencyAlert.EmergencyShooting.MinTimeBetweenTargetOffsetPoints, AiEmergencyState.EmergencyAlert.
            EmergencyShooting.MaxTimeBetweenTargetOffsetPoints);
            yield return new WaitForSeconds(RandomTime);
            IsEmergencyStateWrongShootingStarted = false;
        }
        IEnumerator CoroForNextDecsionAfterShooting()
        {
            float RandomTime = Random.Range(AiEmergencyState.EmergencyAlert.EmergencyShooting.MinTimeToEmergencyShooting, AiEmergencyState.EmergencyAlert.
            EmergencyShooting.MaxTimeToEmergencyShooting);
            yield return new WaitForSeconds(RandomTime);
           // pathfinder.PreviousDestination = pathfinder.closestPoint;
            EmergencyCoverCheck = true;
            ResetEmergencyDecisionMaking = false;
            IsShootingDuringEmergencyState = false;
        }
        public void StandShoot()
        {
            if (CombatStateBehaviours.EnableStrafing == true)
            {
                if (VisibilityCheck.DebugNumberOfRaycasts >= RaycastChecksToDoWhenTargetLostDuringStrafing)
                {
                    IsStrafingEnabled = true;
                    IsGoingToPreviousPoint = true;
                }
            }
           // Debug.Log("YES ME TOO");
            if(StrafingForChargingAgent == false)
            {
                DisableNavmeshAgentcomponent();
            }
           
            IsCrouched = false;
            StopSpineRotation = false;
            IsStationedShoot = true;
            //  AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 0f);
            //if (Components.HumanoidFiringBehaviourComponent != null)
            //{
            if (Components.HumanoidFiringBehaviourComponent != null) // Required because when Agent role is zombie HumanoidFiringBehaviourComponent is not required.
            {
                Components.HumanoidFiringBehaviourComponent.FireNow = true;

            }
            //if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
            //{


            SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
            ConnectWithUpperBodyAimingAnimation();
                //}
            //}
            LookingAtEnemy();
            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, false);

            if (Components.NavMeshAgentComponent.enabled == true)
            {
                if (Components.NavMeshAgentComponent != null)
                {
                    Components.NavMeshAgentComponent.speed = 0f;
                    Components.NavMeshAgentComponent.isStopped = true;
                }

            }


            //StopSpineRotation = false;
            //SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
            //ConnectWithUpperBodyAimingAnimation();
            //AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);

        }
        public void CrouchShoot()
        {
            if (CombatStateBehaviours.EnableStrafing == true)
            {
                if (VisibilityCheck.DebugNumberOfRaycasts >= RaycastChecksToDoWhenTargetLostDuringStrafing)
                {
                    IsStrafingEnabled = true;
                    IsGoingToPreviousPoint = true;
                }
            }
            DisableNavmeshAgentcomponent();
            LookingAtEnemy();
            // AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);
            CoverFireController();
            //if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
            //{
            //  SetAnimationForFullBody(AiAgentAnimatorParameters.CrouchAimingParameterName); // Commented only this line because if you don't comment than it will create conflict with crouch shooting and aiming
            // in humanoid Firing behaviour component as when the shot happens the Ai agent need to aim and this thing is handle by the Humanoid firing behaviour component so avoid called crouch Aim here so it won't
            // cal 2 times here and inside humanoid firing behaviour component.
            //}
        }
        public void Fire()
        {
            Components.HumanoidAiAudioPlayerComponent.PlayRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.RecurringSounds.EngagingTheEnemyAudioClips);
            if (CombatStateBehaviours.EnableStrafing == true && IsStrafingEnabled == true)
            {
                if (AiStrafing.RandomizeBetweenStopOrStrafeCycles == true)
                {
                    if (CheckForStrafeOrStillShoot == false)
                    {
                        int DecideVal = Random.Range(0, 2);
                      //  Debug.Log(DecideVal + "Value");
                        if (DecideVal == 1)
                        {
                            IsStrafingEnabled = false;
                        }
                        else if (DecideVal == 2)
                        {
                            IsStrafingEnabled = true;
                        }
                        StartCoroutine(ResetShootChoice());
                        CheckForStrafeOrStillShoot = true;
                    }
                }

                if (CombatStateBehaviours.UseFiringPoints == true)
                {
                    if (ReachnewFirepoint == false)
                    {
                        Info("Strafe Shooting");
                        RandomStrafing();
                    }
                    else
                    {
                        Info("Stand Shooting");
                        StandShoot();
                    }

                }
                else
                {
                    Info("Strafe Shooting");
                    RandomStrafing();
                }

            }
            else
            {
                if (CombatStateBehaviours.EnableStationedCrouchedShooting == true && CombatStateBehaviours.UseFiringPoints == false && CombatStateBehaviours.TakeCovers == false)
                {
                    if (StationedShooting.CrouchedShootingOnly == true && StationedShooting.RandomizeCrouchStandShooting == false)
                    {
                        Info("Crouch Shooting");
                        CrouchShoot();
                    }
                    else if (StationedShooting.CrouchedShootingOnly == true && StationedShooting.RandomizeCrouchStandShooting == true || StationedShooting.RandomizeCrouchStandShooting == true)
                    {
                        if (ResetPostureTime == false)
                        {
                            int RandomPostureValue = Random.Range(0, 2);
                          //  Debug.Log(RandomPostureValue);
                            if (RandomPostureValue == 1)
                            {
                                SitShootSelected = false;
                            }
                            else
                            {
                                SitShootSelected = true;
                            }
                            StartCoroutine(RandomiseSitORStand());
                            ResetPostureTime = true;
                        }

                        if (SitShootSelected == true)
                        {
                            Info("Crouch Shooting");
                            CrouchShoot();
                        }
                        else
                        {
                            Info("Stand Shooting");
                            StandShoot();
                        }
                    }
                    else
                    {
                        if (IsCrouched == true)
                        {
                            Info("Crouch Shooting");
                            CrouchShoot();
                        }
                        else
                        {
                            Info("Stand Shooting");
                            StandShoot();
                        }
                    }
                }
                else
                {
                    if (IsCrouched == true)
                    {
                        Info("Crouch Shooting");
                        CrouchShoot();
                    }
                    else
                    {
                        Info("Stand Shooting");
                        StandShoot();
                    }
                }



            }
            //if (ChoosenAiTypeIsMaster == true && EnableStrafeBetweenWaypoints == true)
            //{
            //    IsCrouched = false;
            //    StopSpineRotation = false;
            //    HumanoidFiringBehaviourComponent.FireNow = true;
            //    LookingAtEnemy();
            //    SetAnims(AimingAnimationName);
            //    anim.SetBool(DefaultStateAnimationName, true);
            //    if (NavMeshAgentComponent != null)
            //    {
            //        NavMeshAgentComponent.speed = 0f;
            //        NavMeshAgentComponent.isStopped = true;
            //    }
            //}

            // if (CombatStateBehaviours.EnableStrafing == true)
            // {
            //if (AiStrafing.EnableShootingWhileProceduralStrafing == false && AiStrafing.EnableShootingWhileCustomStrafing == false)
            //{
            //    IsCrouched = false;
            //    StopSpineRotation = false;
            //    Components.HumanoidFiringBehaviourComponent.FireNow = true;
            //    LookingAtEnemy();
            //    SetAnims(AiAgentAnimatorParameters.FireParameterName);
            //    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
            //    if (Components.NavMeshAgentComponent != null)
            //    {
            //        Components.NavMeshAgentComponent.speed = 0f;
            //        Components.NavMeshAgentComponent.isStopped = true;
            //    }
            //}
            //else if (AiStrafing.EnableShootingWhileProceduralStrafing == true || AiStrafing.EnableShootingWhileCustomStrafing == true)
            //{
            //    if (CombatStateBehaviours.UseWaypoints == true)
            //    {
            //        if (Reachnewpoints == false)
            //        {
            //            RandomStrafing();
            //        }
            //        else
            //        {
            //            IsCrouched = false;
            //            StopSpineRotation = false;
            //            Components.HumanoidFiringBehaviourComponent.FireNow = true;
            //            LookingAtEnemy();
            //            SetAnims(AiAgentAnimatorParameters.FireParameterName);
            //            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
            //            if (Components.NavMeshAgentComponent != null)
            //            {
            //                Components.NavMeshAgentComponent.speed = 0f;
            //                Components.NavMeshAgentComponent.isStopped = true;
            //            }
            //        }

            //    }
            //    else
            //    {
            //        RandomStrafing();
            //    }
            //}

        }
        public void Idle()
        {
            if (IsAgentRoleLeader == false)
            {
                if (NonCombatBehaviours.DefaultBehaviour == InvestigationTypes.Stationary && HealthScript.CompleteFirstHitAnimation == false)
                {
                    enableIkupperbodyRotations(ref ActivateNoIk);
                    IsCrouched = false;
                    StopSpineRotation = true;
                    CombatStarted = false;
                    if (Components.HumanoidFiringBehaviourComponent != null)
                    {
                        Components.HumanoidFiringBehaviourComponent.FireNow = false;
                    }

                    SetAnimationForFullBody(AiAgentAnimatorParameters.IdleParameterName);
                    SetAnimationForUpperBody("UpperBodyIdle"); // Keep this commented as when you uncomment this code and when AI agent sprint towards the sound coordinate the upper body movement become wierd.
                    // had to uncomment as after post combat scan the AI agent has to make sure the upper body is back to idle.

                    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                    if (Components.NavMeshAgentComponent != null)
                    {
                        Components.NavMeshAgentComponent.speed = 0f;
                        if (Components.NavMeshAgentComponent.enabled == true)
                        {
                            Components.NavMeshAgentComponent.isStopped = true;
                        }
                    }
                }
            }
          
        }
        public void ConnectWithUpperBodyAimingAnimation()
        {
            if (Components.HumanoidFiringBehaviourComponent != null)
            {
                if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                {
                    SetAnimationForUpperBody("StandShootPosture");
                }
                else
                {
                    SetAnimationForUpperBody("Reload");
                }
            }
        }
        public void Run(Vector3 Destination, bool ShouldFireNow)
        {
            //if (Destination != PrevDestination)
            //{
            //    StartCoroutine(ReEnableNavigation());
            //    CanStartMoving = false;
            //    PrevDestination = Destination;
            //}

            //if (CanStartMoving == true)
            //{
            if(AgentRole == Role.Zombie)
            {
                if (Components.NavMeshAgentComponent.enabled == true)
                {
                    Components.NavMeshAgentComponent.isStopped = false;
                    Components.NavMeshAgentComponent.speed = ZombieSpeeds.MovementSpeeds.RunForwardSpeed;
                    Components.NavMeshAgentComponent.destination = Destination;
                }
               
                SetAnimationForFullZombieBody(ZombieAiAnimatorParameters.RunForwardParameterName);
               
            }
            else
            {
                StopSpineRotation = false;
                IsCrouched = false;
                if (Components.NavMeshAgentComponent.enabled == true)
                {
                    Components.NavMeshAgentComponent.isStopped = false;
                    Components.NavMeshAgentComponent.speed = Speeds.MovementSpeeds.RunForwardSpeed;
                    Components.NavMeshAgentComponent.destination = Destination;
                }
                if (Components.HumanoidFiringBehaviourComponent != null)
                {
                    Components.HumanoidFiringBehaviourComponent.FireNow = ShouldFireNow;  // so can be used by hearing as well as charging and any other behaviour so whether to fire or not can be decided by ai agents depend on there behaviour
                }
                
                SetAnimationForFullBody(AiAgentAnimatorParameters.RunForwardParameterName);
                ConnectWithUpperBodyAimingAnimation();
                anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
            }
               
            //}
        }
        public void WalkForward(Vector3 Destination, bool ShouldFireNow)
        {
            //if (Destination != PrevDestination)
            //{
            //    StartCoroutine(ReEnableNavigation());
            //    CanStartMoving = false;
            //    PrevDestination = Destination;
            //}

            //if (CanStartMoving == true)
            //{
                StopSpineRotation = false;
                IsCrouched = false;
                if (Components.NavMeshAgentComponent.enabled == true)
                {
                    Components.NavMeshAgentComponent.isStopped = false;
                    Components.NavMeshAgentComponent.speed = Speeds.MovementSpeeds.WalkForwardAimingSpeed;
                    Components.NavMeshAgentComponent.destination = Destination;
                }
                if (Components.HumanoidFiringBehaviourComponent != null)
                {
                    Components.HumanoidFiringBehaviourComponent.FireNow = ShouldFireNow;  // so can be used by hearing as well as charging and any other behaviour so whether to fire or not can be decided by ai agents depend on there behaviour
                }
                SetAnimationForFullBody(AiAgentAnimatorParameters.WalkForwardParameterName);
                ConnectWithUpperBodyAimingAnimation();
                anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);

            //}

        }
        //IEnumerator DirectionCheckForRunstrafe()
        //{
        //    yield return new WaitForSeconds(DirectionCheckForRun);
        //    DirectionCheckForRun = Random.Range(AiDirectionMarkers.MinDirectionChecks, AiDirectionMarkers.MaxDirectionChecks);
        //    StartDirectionCheck = false;

        //}
        public void RunTowardsDestination()
        {
            //float Angleforward = Vector3.Angle(this.VisibilityCheck.RaycastChecker.transform.forward, Newpos);
            //float AngleBackward = Vector3.Angle(-this.VisibilityCheck.RaycastChecker.transform.forward, Newpos);
            //float AngleLeft = Vector3.Angle(-this.VisibilityCheck.RaycastChecker.transform.right, Newpos);
            //float AngleRight = Vector3.Angle(this.VisibilityCheck.RaycastChecker.transform.right, Newpos);

            //if (!Components.HumanoidFiringBehaviourComponent.isreloading)
            //{
            //    if (Angleforward < 60)
            //    {
            //        Components.NavMeshAgentComponent.speed = MovementSpeeds.RunForwardSpeed;
            //        SetAnims(AiAgentAnimatorParameters.RunForwardParameterName);
            //        anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
            //    }
            //    else if (AngleBackward < 60)
            //    {
            //        Components.NavMeshAgentComponent.speed = MovementSpeeds.RunBackwardSpeed;
            //        SetAnims(AiAgentAnimatorParameters.RunBackwardParameterName);
            //        anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
            //    }
            //    else if (AngleLeft < 60)
            //    {
            //        Components.NavMeshAgentComponent.speed = MovementSpeeds.RunLeftSpeed;
            //        SetAnims(AiAgentAnimatorParameters.RunLeftParameterName);
            //        anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
            //    }
            //    else if (AngleRight < 60)
            //    {
            //        Components.NavMeshAgentComponent.speed = MovementSpeeds.RunRightSpeed;
            //        SetAnims(AiAgentAnimatorParameters.RunRightParameterName);
            //        anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
            //    }

            //if (PreviousDestinationWhenRunning != Destination)
            //{

                //if (StartDirectionCheck == false)
                //{
                //    Transform t = GetClosestStrafeDestination(Destination, AiDirectionMarkers.DirectionMarkers);

                //    for (int x = 0; x < AiDirectionMarkers.RunAnimations.Count; x++)
                //    {
                //        if (t.name == AiDirectionMarkers.RunAnimations[x].DirectionName)
                //        {
                //            strafedir = x;
                //        }
                //    }

                //    PreviousDestinationWhenRunning = Destination;
                //    StartCoroutine(DirectionCheckForRunstrafe());
                //    StartDirectionCheck = true;
                //}
            //}
            NewStrafeAnimationPicker(false);

            //if (!Components.HumanoidFiringBehaviourComponent.isreloading)
            //{
            AnimController(false, GetMovingSpeed, AiAgentAnimatorParameters.DefaultStateParameterName, true, Components.HumanoidFiringBehaviourComponent.FireNow);
            SetAnimationForFullBody(GetStrafeAnimationName);
            ConnectWithUpperBodyAimingAnimation();

           // Debug.Log(GetStrafeAnimationName);

            StopSpineRotation = false;
            IsCrouched = false;
            Components.HumanoidFiringBehaviourComponent.FireNow = true;

            if (Components.NavMeshAgentComponent.enabled == true)
            {
                Components.NavMeshAgentComponent.isStopped = false;
            }

            //}
            //else
            //{
            //    Components.NavMeshAgentComponent.isStopped = true;
            //    Reload();
            //    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
            //}
        }
        //public void FullUpperAndLowerBodyReload()
        //{
        //    if (Components.HumanoidFiringBehaviourComponent.ReloadDelayCompleted == true && Components.HumanoidFiringBehaviourComponent.NextShotDelay == false)
        //    {
        //        if (IsCrouched == true)
        //        {
        //           // AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f,false);
        //            Info("Crouch Reloading");
        //            SetAnimationForFullBody(AiAgentAnimatorParameters.CrouchReloadParameterName);
        //        }
        //        else
        //        {
        //           // AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 0f);
        //            Info("Stand Reloading");
        //            //  SetAnims(AiAgentAnimatorParameters.ReloadParameterName);
        //            //if(PreviousAnimName == AiAgentAnimatorParameters.SprintingParameterName)
        //            //{
        //            SetAnimationForUpperBody(AiAgentAnimatorParameters.ReloadParameterName);
        //            //}
        //            //else
        //            //{
        //            //    anim.SetBool(AiAgentAnimatorParameters.ReloadParameterName, true);
        //            //}
        //            // AnimatorLayerWeightControllerScript.ChangeLayerWeight(2, 1f);
        //        }
        //    }

        //    //IsCrouched = false;
        //    StopSpineRotation = false;
        //   // Components.HumanoidFiringBehaviourComponent.FireNow = false;
        //    LookingAtEnemy();

        //    //if (Components.NavMeshAgentComponent != null)
        //    //{
        //    //    Components.NavMeshAgentComponent.speed = 0f;
        //    //    Components.NavMeshAgentComponent.isStopped = true;
        //    //}
        //}
        public void FullUpperAndLowerBodyReload()
        {
            if (Components.HumanoidFiringBehaviourComponent.ReloadDelayCompleted == true && Components.HumanoidFiringBehaviourComponent.NextShotDelay == false)
            {
                if(StrafingForChargingAgent == false)
                {
                    DisableNavmeshAgentcomponent();
                }

                if (IsCrouched == true)
                {
                    Info("Crouch Reloading");
                    SetAnimationForFullBody(AiAgentAnimatorParameters.CrouchAimingParameterName);                   
                    SetAnimationForUpperBody(AiAgentAnimatorParameters.CrouchReloadParameterName);
                }
                else
                {
                    Info("Stand Reloading");
                    SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                    SetAnimationForUpperBody(AiAgentAnimatorParameters.ReloadParameterName);
                }
            }

            if (Components.NavMeshAgentComponent.enabled == true)
            {
                if (Components.NavMeshAgentComponent != null)
                {
                    Components.NavMeshAgentComponent.speed = 0f;
                    Components.NavMeshAgentComponent.isStopped = true;
                }
            }

            StopSpineRotation = false;
            LookingAtEnemy();
        }
        public void OnlyUpperBodyReload()
        {
            if (Components.HumanoidFiringBehaviourComponent.ReloadDelayCompleted == true && Components.HumanoidFiringBehaviourComponent.NextShotDelay == false)
            {
                if (IsCrouched == true)
                {
                    anim.SetBool("CrouchShootPosture", false);
                    SetAnimationForUpperBody(AiAgentAnimatorParameters.CrouchReloadParameterName);
                }
                else
                {
                    anim.SetBool("StandShootPosture", false);
                    SetAnimationForUpperBody(AiAgentAnimatorParameters.ReloadParameterName);
                }
            }
        }
        public void LookingAtEnemy()
        {
            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            {
                RotatingTransforms.ChangeRotation(transform, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, transform.position, Speeds.MovementSpeeds.BodyRotationSpeed);
            }
        }
        public void LookingAtspecificLocation(Vector3 Location)
        {
            RotatingTransforms.ChangeRotation(transform, Location, transform.position, Speeds.MovementSpeeds.BodyRotationSpeed);
        }
        public void FindClosestEnemyNow()
        {
            if(HealthScript.IsDied == false)
            {
                //if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                //{
                //}

                if(FindEnemiesScript != null)
                {
                    FindEnemiesScript.FindClosestEnemyNow(transform);

                    if (FindEnemiesScript.ContainThisTransform == true)
                    {
                        FindEnemiesScript.DetectionRadius = 0;
                    }
                    else
                    {
                        FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
                    }
                }
              
            }           
        }
        public void FindNewEnemies()
        {
            if (FindEnemiesScript != null)
            {
                FindEnemiesScript.FindingEnemies();
            }
        }
        public void ReachedWayPoint()
        {
            // Debug.Log("REACHING WAYPOINT" + transform.name);
            LookingAtEnemy();

            if (NewFiringPointNode.EnableCrouchFiring == false)
            {
                //if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                //{
                StandShoot();
                if (Components.HumanoidFiringBehaviourComponent.isreloading)
                {
                    FullUpperAndLowerBodyReload();
                }
                //Info("Stand Shooting");
                //StopSpineRotation = false;
                //if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
                //{
                //    SetAnimationForLowerBody(AiAgentAnimatorParameters.AimingParameterName);
                //}
                //AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);
                //}
                //else
                //{
                //    StopSpineRotation = false;
                //    Reload();
                //}
            }
            else
            {
                if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                {
                    CoverFireController();
                }
                else
                {
                    CoverReloadController();
                }
            }
        }
        public void DisableNavmeshAgentcomponent()
        {
            if(DisableTemporaryWhenSearchingForCoverOrFiringPoints == false && pathfinder.PauseTheNavMeshSearching == false)
            {
                Components.NavMeshAgentComponent.enabled = false;
                NavMeshObstacleComponent.enabled = true;
                pathfinder.IsNavMeshObstacleDisabled = false;
            }
            else
            {
                Components.NavMeshAgentComponent.enabled = true;
                NavMeshObstacleComponent.enabled = false;
                pathfinder.IsNavMeshObstacleDisabled = true;
            }
            
        }
        //IEnumerator EnableNavMeshAgentComponent()
        //{
        //    NavMeshObstacleComponent.enabled = false;
        //    yield return new WaitForSeconds(0.001f);
        //    Components.NavMeshAgentComponent.enabled = true;
        //    pathfinder.IsNavMeshObstacleAgentDisabled = true;
        //}
        public void ReachedCoverPoint()
        {
            if (NewCoverNode.DistanceCleared == true)
            {
                DisableNavmeshAgentcomponent();
              
                if (NewCoverNode.StandingCover == true)// && MoveTowardsOpenFirePoint == false)
                {
                    UseCrouchOrStandPosture = false;
                    Info("Taking Stand Hiding Cover");
                    //Debug.Log(CrouchPositions[CurrentCoverPoint].name);
                    StandPositionDecider();
                    StopSpineRotation = true;
                }
                else
                {
                    LookingAtEnemy();
                    //if (CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().OpenFireCover == true)
                    //{
                    //    if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                    //    {
                    //        StopSpineRotation = false;
                    //        SetAnims(AiAgentAnimatorParameters.AimingParameterName);
                    //        AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);
                    //    }
                    //    else
                    //    {
                    //        StopSpineRotation = false;
                    //        Reload();
                    //    }
                    //}
                    if (NewCoverNode.CrouchFiringCoverNode == true)
                    {
                      
                        Info("Taking Crouch Firing Point");
                     
                       
                            if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                            {
                                CoverFireController();
                            }
                            else
                            {

                                CoverReloadController();
                            }
                        
                    }
                    else if (NewCoverNode.StandFiringCover == true)
                    {
                        if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                        {
                            Info("Taking Stand Firing Point");
                            StandShoot();
                            //StopSpineRotation = false;
                            //SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                            //ConnectWithUpperBodyAimingAnimation();
                            //AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, true, true);
                        }
                        else
                        {
                            FullUpperAndLowerBodyReload();
                        }
                    }
                    else if (NewCoverNode.CrouchCover == true)
                    {
                        
                        ReloadChecksToDo();

                        if (ShouldPauseStandCoverPostureAnims == false)
                        {
                            Info("Taking Crouch Hiding Cover");
                            CrouchHideCover();

                        }

                    }

                }
            }


        }
        public void CrouchHideCover()
        {
            Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentCrouchBaseOffset;
            UseCrouchOrStandPosture = true;
            IsCrouched = true; // added on 26/06/23
            StopSpineRotation = true;
            //if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
            //{
            SetAnimationForFullBody(AiAgentAnimatorParameters.CrouchAimingParameterName);
            if (IsPlayingBodyHitAnimation == false && ThrowingGrenade == false) // Before this if statement code was not there and added while testing in tutorial because when Ai agent throw grenade at the same time
                                                                               //  we play this animation which basically result in transition between both of these animation and AI agent try to play both animation simulaneously when taking crouch hide cover.
            {
                anim.SetBool("CrouchShootPosture", true);
                anim.SetBool("StandShootPosture", false); 
            }
          
            //SetAnimationForFullBody("CrouchShootPosture");
            //}
            AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, false, false);

        }
        //public void ReachedCoverPoint()
        //{
        //    LookingAtEnemy();
        //    if (CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().StandingCover == true)
        //    {
        //        StandPositionDecider();
        //        StopSpineRotation = true;
        //    }
        //    else
        //    {
        //        if (CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().OpenFireCover == true)
        //        {
        //            if (!HumanoidFiringBehaviourComponent.isreloading)
        //            {
        //                StopSpineRotation = false;
        //                SetAnims(FireAnimationName);
        //                AnimController(true, 0f, DefaultStateAnimationName, true, true);
        //            }
        //            else
        //            {
        //                StopSpineRotation = false;
        //                Reload();
        //            }
        //        }
        //        else if (CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().HidingCover == true)
        //        {
        //            StopSpineRotation = true;
        //            SetAnims(CoverFireAnimationName);
        //            AnimController(true, 0f, DefaultStateAnimationName, false, false);
        //        }
        //        else
        //        {
        //            if (!HumanoidFiringBehaviourComponent.isreloading)
        //            {
        //                CoverFireController();
        //            }
        //            else
        //            {
        //                CoverReloadController();
        //            }
        //        }
        //    }
        //}

        //public void SmoothAnimatorWeightChanger(int LayerIndex, float Value)
        //{
        //    if (anim.GetLayerWeight(LayerIndex) != Value)
        //    {
        //        currentWeight = Mathf.Lerp(currentWeight, Value, Time.deltaTime * 5f);
        //        anim.SetLayerWeight(LayerIndex, currentWeight);
        //        if (anim.GetLayerWeight(LayerIndex) == Value)
        //        {
        //            SmoothAnimatorWeightChange = false;
        //        }
        //    }

        //}
        public void SmoothRotationForStandCovers(float Y)
        {
            transform.eulerAngles = Vector3.LerpUnclamped(transform.eulerAngles, new Vector3(0f, Y, 0f), 3f * Time.deltaTime);
        }
        //public void StandingCoverMovement(string animationName)
        //{
        //    SetAnims(animationName);
        //    AnimController(true, 0f, DefaultStateAnimationName, false, false);     
        //}

        IEnumerator ShortReloadCheck()
        {
            float Delay = 1f;
            if (IsCrouched == true)
            {
                Delay = AiCovers.ReloadDelayBehindCrouchHideCover;
            }
            else
            {
                Delay = AiCovers.ReloadDelayBehindStandHideCover;
            }
            yield return new WaitForSeconds(Delay);
            if (Components.HumanoidFiringBehaviourComponent.isreloading && AiCovers.EnableReloadingBehindHideCovers == false || AiCovers.EnableReloadingBehindHideCovers == true
                && Components.HumanoidFiringBehaviourComponent.Reload.MagazineSize <= AiCovers.ReloadIfAmmoLeftInMagazineIsLessThan)
            {
                Components.HumanoidFiringBehaviourComponent.Reload.MagazineSize = 0;
                ShouldPauseStandCoverPostureAnims = true;
                OnlyUpperBodyReload();
            }
            else
            {
                ShouldPauseStandCoverPostureAnims = false;
            }
            StartReloadCheck = false;
        }
        public void ReloadChecksToDo()
        {
            if (StartReloadCheck == false)
            {
                StartCoroutine(ShortReloadCheck());
                StartReloadCheck = true;
            }

            if (ShouldPauseStandCoverPostureAnims == true)
            {
                if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                {
                    ShouldPauseStandCoverPostureAnims = false;
                }
            }
        }
        public void StandPositionDecider()
        {
            ReloadChecksToDo();

            //  AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);
            if (RefreshStandCoverOnce == false)
            {
                StandCoverPositionDeciderNode = NewCoverNode;
                Transform Name = GetClosestStandCoverDirection(StandCoverPositionDeciderNode.StandHideCoverDirections);

                for (int x = 0; x < StandCoverPositionDeciderNode.StandHideCoverAnimationPlayer.Count; x++)
                {
                    if (StandCoverPositionDeciderNode.StandHideCoverAnimationPlayer[x].StandHideCoverDirection.name == Name.name)
                    {
                        FindIndexForStandCoverRotationDecider = x;
                    }
                }

                RefreshStandCoverOnce = true;
            }

            if (ShouldPauseStandCoverPostureAnims == false)
            {
                // Debug.Log(GetClosestStandCoverDirection(CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().AllStandCoverNodes).name);
                if (StandCoverPositionDeciderNode.StandHideCoverAnimationPlayer[FindIndexForStandCoverRotationDecider].AnimationToPlay == CoverNode.ChooseAnimationToPlay.RifleIdle)
                {
                    if (StandCoverPositionDeciderNode.transform.childCount > 0)
                    {
                        var lookPos = StandCoverPositionDeciderNode.transform.GetChild(0).transform.position - transform.position;
                        lookPos.y = 0;
                        var rotation = Quaternion.LookRotation(lookPos);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * AiCovers.TransitionSpeedToStandCover);
                        //RotatingWhileSprinting(StandCoverPositionDeciderNode.StandCover[FindIndexForStandCoverRotationDecider].YAxisToRotateTo);
                    }
                    Info("Stand Hiding Cover Direction - Neutral");
                    StandingCoverMovement(AiAgentAnimatorParameters.StandCoverNeutralParameterName);
                }
                else if (StandCoverPositionDeciderNode.StandHideCoverAnimationPlayer[FindIndexForStandCoverRotationDecider].AnimationToPlay == CoverNode.ChooseAnimationToPlay.StandLeft)
                {
                    if (StandCoverPositionDeciderNode.transform.childCount > 0)
                    {
                        var lookPos = StandCoverPositionDeciderNode.transform.GetChild(0).transform.position - transform.position;
                        lookPos.y = 0;
                        var rotation = Quaternion.LookRotation(lookPos);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * AiCovers.TransitionSpeedToStandCover);
                        // RotatingWhileSprinting(StandCoverPositionDeciderNode.StandCover[FindIndexForStandCoverRotationDecider].YAxisToRotateTo);
                    }
                    Info("Stand Hiding Cover Direction - Left");
                    StandingCoverMovement(AiAgentAnimatorParameters.StandCoverLeftParameterName);
                }
                else if (StandCoverPositionDeciderNode.StandHideCoverAnimationPlayer[FindIndexForStandCoverRotationDecider].AnimationToPlay == CoverNode.ChooseAnimationToPlay.StandRight)
                {
                    if (StandCoverPositionDeciderNode.transform.childCount > 0)
                    {
                        var lookPos = StandCoverPositionDeciderNode.transform.GetChild(0).transform.position - transform.position;
                        lookPos.y = 0;
                        var rotation = Quaternion.LookRotation(lookPos);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * AiCovers.TransitionSpeedToStandCover);
                        //  RotatingWhileSprinting(StandCoverPositionDeciderNode.StandCover[FindIndexForStandCoverRotationDecider].YAxisToRotateTo);
                    }
                    Info("Stand Hiding Cover Direction - Right");
                    StandingCoverMovement(AiAgentAnimatorParameters.StandCoverRightParameterName);
                }
            }

            //if (CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().transform.parent.transform.eulerAngles.y == 0)
            //{
            //if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.z > CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().TempStandCoverZAxis)
            //{
            //Debug.Break();
            //if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x > CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().XPos)
            //{
            //    RotatingWhileSprinting(180f);
            //    anim.SetBool(TakingStandCoverRight, false);
            //   StandingCoverMovement(TakingStandCoverLeft);
            //}
            //else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x < CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().XPos)
            //{
            //    RotatingWhileSprinting(180f);
            //    anim.SetBool(TakingStandCoverLeft, false);
            //    StandingCoverMovement(TakingStandCoverRight);
            //}
            // AiStandCoverRotationZero();
            // }
            // else
            // {
            // AiStandCoverRotationZero();
            //}
            //}
            //else
            //{
            //    AiStandCoverRotationOneEighty();
            //}
        }
        //public void AiStandCoverRotationZero()
        //{
        //    if (CrouchPositions[CurrentCoverPoint].transform.localEulerAngles.y == 180f)
        //    {
        //        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x > CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().XPos)
        //        {
        //            SmoothRotationForStandCovers(180f);
        //            // RotatingWhileSprinting(180f);
        //            anim.SetBool(AiAgentAnimatorParameters.StandCoverRightParameterName, false);
        //            StandingCoverMovement(AiAgentAnimatorParameters.StandCoverLeftParameterName);

        //        }
        //        else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x < CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().XPos)
        //        {
        //            SmoothRotationForStandCovers(180f);

        //            //if(ShouldRotateForCover == false)
        //            //{
        //            //    StartCoroutine(Rotate(Vector3.up,180f, 0.5f));
        //            //    ShouldRotateForCover = true;
        //            //}

        //            //  RotatingWhileSprinting(180f);
        //            anim.SetBool(AiAgentAnimatorParameters.StandCoverLeftParameterName, false);
        //            StandingCoverMovement(AiAgentAnimatorParameters.StandCoverRightParameterName);
        //        }
        //    }
        //    else
        //    {
        //        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x > CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().XPos)
        //        {
        //            SmoothRotationForStandCovers(0f);
        //            // RotatingWhileSprinting(0f);
        //            anim.SetBool(AiAgentAnimatorParameters.StandCoverLeftParameterName, false);
        //            StandingCoverMovement(AiAgentAnimatorParameters.StandCoverRightParameterName);

        //        }
        //        else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x < CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().XPos)
        //        {
        //            SmoothRotationForStandCovers(0f);
        //            // RotatingWhileSprinting(0f);
        //            anim.SetBool(AiAgentAnimatorParameters.StandCoverRightParameterName, false);
        //            StandingCoverMovement(AiAgentAnimatorParameters.StandCoverLeftParameterName);
        //        }
        //    }
        //}
        //public void AiStandCoverRotationOneEighty()
        //{
        //    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.z > CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().TempStandCoverZAxis)
        //    {
        //        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x > CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().XPos)
        //        {
        //            SmoothRotationForStandCovers(275f);
        //            // RotatingWhileSprinting(275f);
        //            anim.SetBool(AiAgentAnimatorParameters.StandCoverLeftParameterName, false);
        //            StandingCoverMovement(AiAgentAnimatorParameters.StandCoverRightParameterName);

        //        }
        //        else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x < CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().XPos)
        //        {
        //            SmoothRotationForStandCovers(65f);
        //            // RotatingWhileSprinting(65f);
        //            anim.SetBool(AiAgentAnimatorParameters.StandCoverRightParameterName, false);
        //            StandingCoverMovement(AiAgentAnimatorParameters.StandCoverLeftParameterName);
        //        }
        //    }
        //    else
        //    {
        //        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x > CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().XPos)
        //        {
        //            SmoothRotationForStandCovers(250f);
        //            // RotatingWhileSprinting(250f);
        //            anim.SetBool(AiAgentAnimatorParameters.StandCoverRightParameterName, false);
        //            StandingCoverMovement(AiAgentAnimatorParameters.StandCoverLeftParameterName);
        //        }
        //        else if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x < CrouchPositions[CurrentCoverPoint].transform.GetComponent<CoverNode>().XPos)
        //        {
        //            SmoothRotationForStandCovers(115f);
        //            // RotatingWhileSprinting(115f);
        //            anim.SetBool(AiAgentAnimatorParameters.StandCoverLeftParameterName, false);
        //            StandingCoverMovement(AiAgentAnimatorParameters.StandCoverRightParameterName);
        //        }
        //    }
        //}
        public void StandingCoverMovement(string animationName)
        {
            SetAnimationForFullBody(animationName);
            SetAnimationForUpperBody(animationName);
            AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, false, false);
            StopSpineRotation = true;
        }
        public void SprintingControllerForCover()
        {
            //if (!Components.HumanoidFiringBehaviourComponent.isreloading)
            //{
            // AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);
            IsCrouched = false;
          //  RotatingTransforms.ChangeRotation(transform, NewCoverNode.transform.position, transform.position, Speeds.MovementSpeeds.BodyRotationSpeed);
            if (Components.NavMeshAgentComponent.enabled == true)
            {
                Components.NavMeshAgentComponent.isStopped = false;
            }
            AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, true);
            SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
            // SetAnimationForUpperBody(AiAgentAnimatorParameters.SprintingParameterName);
            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, false);
            Components.HumanoidFiringBehaviourComponent.FireNow = false;




            //Vector3 targetPosition = CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().transform.position;
            //Vector3 horizontalDisplacement = new Vector3(targetPosition.x - transform.position.x, 0f, targetPosition.z - transform.position.z);

            //if (horizontalDisplacement.magnitude <= 0.25f)
            //{
            //    CrouchPositions[CurrentCoverPoint].gameObject.GetComponent<CoverNode>().DistanceCleared = true;
            //}
            //}
            //else
            //{
            //    Reload();
            //}

        }


        // Explanation of New Changes in CoverFireController and CoverReloadController and why they are like this - Basically now only the upper body will reload and the lower body will still be playing its own animation independently.
        // In the upper body layer we have clicked on the 'crouch reload' animation clip and there changed the transition from 'crouch reload' to crouch aiming with a parameter called 'Crouch Aim' before the parameter was 'CrouchShootPosture'.
        // Now why this was changed is because when we want only the upper body to play reload animation clip ( as this is important in case the animation is stand reload and you want to use upper body for crouch reload i.e case with pistol animations)
        // while the lower body to keep playing the previous animation clip as if we use the parameter 'CrouchShootPosture' for both the upper and lower body animation clip name 'Crouch aiming' than even though upper body is performing
        // crouch reloading it will always going to intruppt with other animation clip 'Crouch aiming' why ? becasue crouch shoot posture parameter needed to stay true When AI agent is in crouch state. so we can't make it false but only for the upper
        // body using a separate parameter will allow us to control the transition for the upper body layer at least during the crouch state.

        public void CoverReloadController()
        {
            //DisableNavmeshAgentcomponent();
            Info("Crouch Reloading");
            //   AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f);
            Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentCrouchBaseOffset;
            IsCrouched = true;
            StopSpineRotation = false;

            UseCrouchOrStandPosture = true;     
          
            SetAnimationForFullBody(AiAgentAnimatorParameters.CrouchReloadParameterName);
            SetAnimationForUpperBody(AiAgentAnimatorParameters.CrouchReloadParameterName);

            anim.SetBool("CrouchShootPosture", true);
            AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, false, false);
        }
        public void CoverFireController()
        {
            //DisableNavmeshAgentcomponent();
            Info("Crouch Firing");
            Components.NavMeshAgentComponent.baseOffset = NavMeshAgentSettings.NavMeshAgentCrouchBaseOffset;
            IsCrouched = true;
            StopSpineRotation = false;

            UseCrouchOrStandPosture = true;

            SetAnimationForFullBody("CrouchShootPosture"); // added newly on 6th sep 2024 otherwise AI agent keeps sprinting between crouch cover firing points until called by AI Weapon Firing Behaviour script.
            anim.SetBool("CrouchShootPosture", true); // added newly on 6th sep 2024 otherwise AI agent keeps sprinting between crouch cover firing points until called by AI Weapon Firing Behaviour script.

            //SetAnimationForFullBody(AiAgentAnimatorParameters.CrouchAimingParameterName);
            //anim.SetBool("CrouchShootPosture", true);
            //anim.SetBool("StandShootPosture", false);
            Components.NavMeshAgentComponent.speed = 0f;

            // Keep these two lines to be commented as when we uncomment it and when the agent is taking crouch firing cover. The crouch fire animation clip is playing again and again even when the shooting is chosen to
            // be semi automatic fire . when we uncomment this crouch fire animation will playback twice . So keep this commented please. Let the fire behaviour script do its own job.
            //SetAnimationForFullBody(AiAgentAnimatorParameters.CrouchFireParameterName);  
            //SetAnimationForUpperBody(AiAgentAnimatorParameters.CrouchFireParameterName); 
            AnimController(true, 0f, AiAgentAnimatorParameters.DefaultStateParameterName, false, true);

        }
        public void RotatingWhileSprinting(float y)
        {
            var newRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - transform.position), Speeds.MovementSpeeds.BodyRotationSpeed * Time.deltaTime).eulerAngles;
            newRotation.x = 0;
            newRotation.y = y;
            newRotation.z = 0;
            transform.rotation = Quaternion.Euler(newRotation);
        }
        public void AnimController(bool AgentStopped, float speed, string animname, bool animCondition, bool firenow)
        {
            if (Components.NavMeshAgentComponent.enabled == true)
            {
                Components.NavMeshAgentComponent.isStopped = AgentStopped;
                Components.NavMeshAgentComponent.speed = speed;
            }

            anim.SetBool(animname, animCondition);
            if (Components.HumanoidFiringBehaviourComponent != null)
            {
                Components.HumanoidFiringBehaviourComponent.FireNow = firenow;
            }
        }
        //public void FindImmediateCoverPoint()
        //{
        //    if (FindingNewCrouchPoint == false)
        //    {
        //        ChangeCover = true;
        //        ReachnewCoverpoint = false;
        //        FindValidCover = true;
        //        //  FindClosestCoverNode();
        //        //if (AiCovers.ContinouslyTakeCovers == false)
        //        //{
        //        //    FindClosestCover();
        //        //}
        //        //else
        //        //{
        //        FindClosestCoverSystematically();
        //        //}
        //        FindingNewCrouchPoint = true;
        //    }
        //}
        public void AgentMovement(NavMeshAgent agent, float speed, bool IsForCovers)
        {
            //if (agent.destination != PrevDestination)
            //{
            //    StartCoroutine(ReEnableNavigation());
            //    CanStartMoving = false;
            //    PrevDestination = agent.destination;
            //}

            //if (CanStartMoving == true)
            //{
                float CurVelocity = agent.speed * 80f / 100f;
                LeastVel = agent.speed - CurVelocity;
                //// Enable avoidance and set avoidance priority
                //agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

                agent.speed = speed;
                // DebugInfo.AgentVelocity = agent.velocity.magnitude;        

                //if (IsForCovers == true)
                //{
                //    if (DebugInfo.AgentVelocity <= LeastVel || CanMoveTowardsRandomPoint == true)
                //    {
                //        if (StartCreateRandomPath == false)
                //        {
                //            StartCoroutine(CheckForFewSeconds());
                //            StartCreateRandomPath = true;
                //        }

                //        if (CanMoveTowardsRandomPoint == true)
                //        {
                //            GenerateRandomPointOnNavmesh(agent);
                //        }
                //        else
                //        {
                //            agent.destination = pathfinder.closestPoint;
                //        }
                //    }
                //    else
                //    {
                //        agent.destination = pathfinder.closestPoint;
                //    }
                //}
                //else
                //{
                agent.destination = pathfinder.closestPoint;
                //}
            //}
        }
        //IEnumerator CheckForFewSeconds()
        //{
        //    yield return new WaitForSeconds(0.5f);
        //    if (DebugInfo.AgentVelocity <= LeastVel)
        //    {
        //        CanMoveTowardsRandomPoint = true;
        //        StartCreateRandomPath = false;
        //        yield return new WaitForSeconds(5f);
        //        CanMoveTowardsRandomPoint = false;
        //    }
        //    else
        //    {
        //        StartCreateRandomPath = false;
        //        CanMoveTowardsRandomPoint = false;
        //    }
        //}
        //private void GenerateRandomPointOnNavmesh(NavMeshAgent agent)
        //{
        //    Vector3 randomDirection = Random.insideUnitCircle.normalized;
        //    Vector3 randomPoint = agent.destination + randomDirection * 7f;

        //    NavMeshHit hit;
        //    if (NavMesh.SamplePosition(randomPoint, out hit, 7f, NavMesh.AllAreas))
        //    {
        //        agent.destination = hit.position;

        //    }
        //}
        //IEnumerator ResettingCover()
        //{
        //    yield return new WaitForSeconds(SaveResetedCoverRandomisation);
        //    ChangeCover = true;
        //    Reachnewpoints = false;
        //    FindValidCover = true;
        //    //  FindClosestCoverNode();
        //    FindClosestCoverSystematically();
        //    ResetCoverRandomisation = false;
        //}
        IEnumerator ResettingWayPoint()
        {
            IsAnyTaskCurrentlyRunning = true;
            float SaveResetedWaypointRandomisation = Random.Range(AiFiringPoints.FiringPointDetectionBehaviour.MinTimeBetweenFiringPoints, AiFiringPoints.FiringPointDetectionBehaviour.MaxTimeBetweenFiringPoints);
            yield return new WaitForSeconds(SaveResetedWaypointRandomisation);
            if (WaypointSearched == true)
            {               
                findWayPoint = false;
                if (ContinueWithFiringPoint == true)
                {
                    LastWayPointSelected = CurrentWayPoint;
                    PreviousFiringPointNode = NewFiringPointNode;
                }

                FindClosestWayPoint();
                if (CurrentWayPoint == LastWayPointSelected)
                {
                    FindClosestWayPoint();
                }
                //  ResetWaypointRandomisation = false;
                CheckTime = false;
                IsTaskOver = true;
            }
           
        }
        public void FindImmediateEnemy()
        {
            if (FindEnemiesScript.ContainThisTransform == false)
            {
                if (T.PlayEnemyEliminatedClips == true)
                {
                    Components.HumanoidAiAudioPlayerComponent.PlayNonRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceTargetKilledAudioClips);
                    T.PlayEnemyEliminatedClips = false;
                }
            }
            FindEnemiesScript.enemy.Remove(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
            FindEnemiesScript.LockedTarget = 0;
            gameObject.SendMessage("CheckAiEnemyList", false, SendMessageOptions.DontRequireReceiver);
            FindEnemiesScript.IsEnemyRemoved = true;
            OneTimeMessage = false;
            RemoveEnemiesFromList = true;
            FindEnemiesScript.NewEnemyLocked = false;
            FindClosestEnemyNow();
        }
        public void ApplyRootMotion(bool Apply) // Applies Root Motion in Animator
        {
            anim.applyRootMotion = Apply;

            if (anim.applyRootMotion == false)
            {
                if (ResetAnimator == true)
                {
                    anim.Rebind();
                    ResetAnimator = false;
                }
            }
        }
        IEnumerator StayTimerNearDeadBody()
        {
            yield return new WaitForSeconds(TimeToStayNearDeadBody);
            StartInvestigation = true;
            StayTimeEnded = true;
            RandomDirAfterTargetLost = GenerateRandomNavmeshLocation.RandomLocationInVector3(InvestigationCoordinates, InvestigationRadiusFromTheDeadBody);
            NavMesh.CalculatePath(transform.position, RandomDirAfterTargetLost, NavMesh.AllAreas, path);
            //  pathfinder.CalculatePathInCombat(RandomDirAfterTargetLost);
            // CreateNavMeshCorners(RandomDirAfterTargetLost);
            // FindClosestPathOnNavemesh(RandomDirAfterTargetLost);
        }
        public IEnumerator EmergencyRunTimer()
        {
            yield return new WaitForSeconds(EmergencySprintTimerIfNoCoverFound);
            //GeneratedSprintPoint = false;
            //BotMovingAway = false;
            //IsEmergencyRun = false;
            IsEmergencyPathBroken = true;
        }
        //Transform GetInitialClosestEmergencyPoint(Transform[] enemies)
        //{
        //    Transform tMin = null;
        //    float minDist = Mathf.Infinity;
        //    Vector3 currentPos = transform.position;
        //    foreach (Transform t in enemies)
        //    {
        //        float dist = Vector3.Distance(t.position, currentPos);
        //        if (dist < minDist && dist <= AiDeadBodyAlerts.EmergencyAlert.RangeToFindEmergencyCover)
        //        {
        //            tMin = t;
        //            minDist = dist;
        //        }
        //    }
        //    ClosestEmergencyPoint = tMin;
        //    return tMin;
        //}
        //Transform GetNewClosestEmergencyPoint(Transform[] enemies)
        //{
        //    Transform tMin = null;
        //    float minDist = Mathf.Infinity;
        //    Vector3 currentPos = transform.position;
        //    foreach (Transform t in enemies)
        //    {
        //        float dist = Vector3.Distance(t.position, currentPos);
        //        if (dist < minDist && t != ClosestEmergencyPoint && dist <= AiDeadBodyAlerts.EmergencyAlert.RangeToFindEmergencyCover)
        //        {
        //            tMin = t;
        //            minDist = dist;
        //        }
        //    }
        //    ClosestEmergencyPoint = tMin;
        //    return tMin;
        //}
        Transform GetClosestStrafePoint(Transform NewStrafePoint, Transform[] StrafeDirections)
        {
            Transform tMin = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = NewStrafePoint.transform.position;
            foreach (Transform t in StrafeDirections)
            {
                float dist = Vector3.Distance(t.position, currentPos);
                if (dist < minDist)
                {
                    tMin = t;
                    minDist = dist;
                }
            }
         
            return tMin;
        }
        //IEnumerator ChangeEmergencyPoint()
        //{
        //    Info("Taking Emergency Cover");
        //    float EmergencyCoverChangeTimer = Random.Range(AiDeadBodyAlerts.EmergencyAlert.MinimumTimeBetweenEmergencyCover, AiDeadBodyAlerts.EmergencyAlert.MaximumTimeBetweenEmergencyCover);
        //    yield return new WaitForSeconds(EmergencyCoverChangeTimer);
        //    EmergencyNodeHolding.GetComponent<CheckForPlayer>().IsAlreadyRegistered = false;
        //    FindEmergencyPoint = false;
        //   // GetNewClosestEmergencyPoint(AiDeadBodyAlerts.EmergencyAlert.EmergencyCoverFinderGameObject.EmergencyPoint);
        //    FindValidEmergencyNode();
        //    if (FindEmergencyPoint == false)
        //    {
        //       //GetNewClosestEmergencyPoint(AiDeadBodyAlerts.EmergencyAlert.EmergencyCoverFinderGameObject.EmergencyPoint);
        //        FindValidEmergencyNode();
        //    }
        //    IsReachedEmergencyPoint = false;
        //}
        Transform FindClosestInitialEmergencyCover()
        {
            Transform closest = null;
            float closestDistance = Mathf.Infinity;
            IsBestEmergencyCoverChosen = false;
            ShouldStopLookingForEmergencyCover = false;

            foreach (Transform emergencycover in AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint)
            {
                float distance = Vector3.Distance(transform.position, emergencycover.position);

                EmergencyCoverNode EmergencyCoverNode = emergencycover.GetComponent<EmergencyCoverNode>();

                EmergencyCoverNode.CheckForTargetPosition(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);

                if (ShouldStopLookingForEmergencyCover == false && IsBestEmergencyCoverChosen == true && AiEmergencyState.EmergencyAlert.ChooseClosestEmergencyCovers == false)
                {
                    ShouldStopLookingForEmergencyCover = true;
                }

                if (distance < closestDistance && EmergencyCoverNode.IsAlreadyRegistered == false
                            && EmergencyCoverNode.DebugCoverValidation == true && ShouldStopLookingForEmergencyCover == false)
                {
                    closestDistance = distance;

                    LocationForSprinting = EmergencyCoverNode.gameObject.transform.position;
                    EmergencyCoverNode.IsAlreadyRegistered = true;
                    PreviousEmergencyCoverNode = EmergencyCoverNode.gameObject;
                    CurrentEmergencyCoverNode = EmergencyCoverNode;
                    EmergencyNodeHolding = EmergencyCoverNode.transform;
                    FindEmergencyPoint = true;
                    closest = EmergencyCoverNode.gameObject.transform;
                    IsNewEmergencyCoverFound = true;
                    IsBestEmergencyCoverChosen = true;
                }
            }

            if (PreviousEmergencyCoverNode != null)
            {
                if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                {
                    ShootingPointForEmergencyCoverNode = Random.Range(0, PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().EmergencyCoverShootingPoints.Length);
                }
            }
            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            {
                if (PreviousEmergencyCoverNode != null)
                {
                    if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                    {
                        PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().Info(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy], T);
                    }
                }
            }

            return closest;
        }
        Transform FindClosestEmergencyCovers(bool TryFindNewCoverIfExist)
        {
            IsNewEmergencyCoverFound = false;
            Transform closest = null;
            float closestDistance = Mathf.Infinity;

            foreach (Transform emergencycover in AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint)
            {
                float distance = Vector3.Distance(transform.position, emergencycover.position);

                EmergencyCoverNode EmergencyCoverN = emergencycover.GetComponent<EmergencyCoverNode>();

                EmergencyCoverN.CheckForTargetPosition(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);

                if(TryFindNewCoverIfExist == true)
                {
                    if (distance < closestDistance && EmergencyCoverN.IsAlreadyRegistered == false
                           && EmergencyCoverN.DebugCoverValidation == true && PreviousEmergencyCoverNode != EmergencyCoverN.gameObject)
                    {
                        closestDistance = distance;
                        LocationForSprinting = EmergencyCoverN.gameObject.transform.position;
                        EmergencyCoverN.IsAlreadyRegistered = true;

                        if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                        {
                            PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().IsAlreadyRegistered = false;
                        }

                        PreviousEmergencyCoverNode = EmergencyCoverN.gameObject;
                        CurrentEmergencyCoverNode = EmergencyCoverN;
                        EmergencyNodeHolding = EmergencyCoverN.transform;
                        closest = EmergencyCoverN.gameObject.transform;
                        IsNewEmergencyCoverFound = true;
                        FindEmergencyPoint = false;
                    }
                }
                else
                {
                    if (IsNewEmergencyCoverFound == false)
                    {
                        if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null && EmergencyCoverN.gameObject.name == PreviousEmergencyCoverNode.gameObject.name)
                        {
                            PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().IsAlreadyRegistered = false;
                            EmergencyCoverN.IsAlreadyRegistered = false;
                        }
                    }

                    if (distance < closestDistance && EmergencyCoverN.IsAlreadyRegistered == false
                           && EmergencyCoverN.DebugCoverValidation == true)
                    {
                        closestDistance = distance;
                        LocationForSprinting = EmergencyCoverN.gameObject.transform.position;
                        EmergencyCoverN.IsAlreadyRegistered = true;
                        PreviousEmergencyCoverNode = EmergencyCoverN.gameObject;
                        CurrentEmergencyCoverNode = EmergencyCoverN;
                        EmergencyNodeHolding = EmergencyCoverN.transform;
                        closest = EmergencyCoverN.gameObject.transform;
                        IsNewEmergencyCoverFound = true;
                        FindEmergencyPoint = false;
                    }
                }
            }

            if(IsNewEmergencyCoverFound == false)
            {
                foreach (Transform emergencycover in AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint)
                {
                    float distance = Vector3.Distance(transform.position, emergencycover.position);

                    EmergencyCoverNode EmergencyCoverN = emergencycover.GetComponent<EmergencyCoverNode>();

                    EmergencyCoverN.CheckForTargetPosition(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);

                    if(IsNewEmergencyCoverFound == false)
                    {
                        if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null && EmergencyCoverN.gameObject.name == PreviousEmergencyCoverNode.gameObject.name)
                        {
                            PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().IsAlreadyRegistered = false;
                            EmergencyCoverN.IsAlreadyRegistered = false;
                        }
                    }
                    

                    if (distance < closestDistance && EmergencyCoverN.IsAlreadyRegistered == false
                           && EmergencyCoverN.DebugCoverValidation == true)
                    {
                        closestDistance = distance;
                        LocationForSprinting = EmergencyCoverN.gameObject.transform.position;
                        EmergencyCoverN.IsAlreadyRegistered = true;
                        PreviousEmergencyCoverNode = EmergencyCoverN.gameObject;
                        CurrentEmergencyCoverNode = EmergencyCoverN;
                        EmergencyNodeHolding = EmergencyCoverN.transform;
                        closest = EmergencyCoverN.gameObject.transform;
                        IsNewEmergencyCoverFound = true;
                        FindEmergencyPoint = false;
                    }
                }
            }
            

            if (PreviousEmergencyCoverNode != null)
            {
                if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                {
                    ShootingPointForEmergencyCoverNode = Random.Range(0, PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().EmergencyCoverShootingPoints.Length);
                }
            }
            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            {
                if (PreviousEmergencyCoverNode != null)
                {
                    if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                    {
                        PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().Info(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy], T);
                    }
                }
            }


            return closest;
        }
        public void SwitchingBetweenNewEmergencyCover(bool TryFindNewCoverIfExist)
        {
            // Sort the enemies array based on distance from the current enemy
            System.Array.Sort(AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint, (enemy1, enemy2) =>
                Vector3.Distance(transform.position, enemy1.transform.position)
                .CompareTo(Vector3.Distance(transform.position, enemy2.transform.position))
            );

            FindClosestEmergencyCovers(TryFindNewCoverIfExist);
        }
        public void FindValidEmergencyNode()
        {
            if (AiEmergencyState.EmergencyAlert.ChooseClosestEmergencyCovers == true)
            {
                if (FindEmergencyPoint == false)
                {
                    // Sort the enemies array based on distance from the current enemy
                    System.Array.Sort(AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint, (enemy1, enemy2) =>
                        Vector3.Distance(transform.position, enemy1.transform.position)
                        .CompareTo(Vector3.Distance(transform.position, enemy2.transform.position))
                    );
                }
            }
            else
            {
                if (FindEmergencyPoint == false)
                {
                    int n = AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint.Length;
                    while (n > 1)
                    {
                        n--;
                        int k = Random.Range(0, n + 1);
                        GameObject temp = AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint[k].gameObject;
                        AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint[k] = AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint[n];
                        AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint[n] = temp.transform;
                    }
                }
            }


            FindClosestInitialEmergencyCover();
            if (FindEmergencyPoint == false)
            {
                //IsEmergencyPathBroken = true;
                // CheckForBrokenPath();
                LocationForSprinting = GenerateRandomNavmeshLocation.RandomLocation(EmergencyDirectionPoint, EmergencyRadiusIfNoCoverFound);
                if (NavMesh.CalculatePath(transform.position, LocationForSprinting, NavMesh.AllAreas, path))
                {
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        StartCoroutine(EmergencyRunTimer());
                        pathfinder.closestPoint = LocationForSprinting;
                        IsEmergencyPathBroken = false;
                    }
                    else
                    {
                        IsEmergencyPathBroken = true;
                    }
                }
            }

            //else
            //{
            //    if (FindEmergencyPoint == false)
            //    {
            //        float HighestRot = 0f;
            //        for (int x = 0; x < AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint.Length; x++)
            //        {
            //            if (FindEmergencyPoint == false)
            //            {
            //                EmergencyCoverNode EmergencyCoverNode = AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint[x].GetComponent<EmergencyCoverNode>();

            //               // EmergencyCoverNode.CheckRotations(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
            //                EmergencyCoverNode.CheckForTargetPosition(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);

            //                if (EmergencyCoverNode.IsAlreadyRegistered == false && EmergencyCoverNode.RotationIHaveToDo > HighestRot
            //                    && EmergencyCoverNode.DebugCoverValidation == true)
            //                {
            //                    LocationForSprinting = EmergencyCoverNode.transform.position;
            //                    PreviousEmergencyCoverNode = EmergencyCoverNode.gameObject;
            //                    EmergencyCoverNode.IsAlreadyRegistered = true;
            //                    CurrentEmergencyCoverNode = EmergencyCoverNode;
            //                    FindEmergencyPoint = true;
            //                    EmergencyNodeHolding = EmergencyCoverNode.transform;
            //                    HighestRot = EmergencyCoverNode.RotationIHaveToDo;
            //                    IsNewEmergencyCoverFound = true;
            //                }
            //            }
            //        }

            //        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            //        {
            //            if (PreviousEmergencyCoverNode != null)
            //            {
            //                if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
            //                {
            //                    PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().Info(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy], T);
            //                }
            //            }
            //        }

            //        if (FindEmergencyPoint == false)
            //        {
            //            //IsEmergencyPathBroken = true;
            //            // CheckForBrokenPath();
            //            StartCoroutine(EmergencyRunTimer());
            //            LocationForSprinting = GenerateRandomNavmeshLocation.RandomLocation(EmergencyDirectionPoint, EmergencyRadiusIfNoCoverFound);
            //            NavMesh.CalculatePath(transform.position, LocationForSprinting, NavMesh.AllAreas, path);
            //            for (int i = 0; i < path.corners.Length - 1; i++)
            //            {
            //                LocationForSprinting = path.corners[i + 1];
            //            }
            //            if (NavMesh.CalculatePath(transform.position, LocationForSprinting, NavMesh.AllAreas, path))
            //            {
            //                if (path.status == NavMeshPathStatus.PathComplete)
            //                {
            //                    IsEmergencyPathBroken = false;
            //                }
            //                else
            //                {
            //                    IsEmergencyPathBroken = true;
            //                }
            //            }
            //            StartCoroutine(RandomiseSprintAwayAndShooting());
            //        }

            //    }
            //}
        }
        IEnumerator RandomiseSprintAwayAndShooting()
        {
            float RandomiseTime = Random.Range(AiEmergencyState.EmergencyAlert.AiFleeingBehaviour.MinTimeBetweenCrouchShooting,
            AiEmergencyState.EmergencyAlert.AiFleeingBehaviour.MaxTimeBetweenCrouchShooting);
            yield return new WaitForSeconds(RandomiseTime);
            if (NavMeshObstacleComponent != null)
            {
                NavMeshObstacleComponent.enabled = false;
                pathfinder.PauseTheNavMeshSearching = true;
            }
            if (IsNewEmergencyCoverFound == false)
            {
                yield return new WaitForSeconds(0.01f);
                Components.NavMeshAgentComponent.enabled = true;
                if (FindEmergencyPoint == false)
                {
                    float Randomise = Random.Range(0, 100);
                    if (Randomise <= AiEmergencyState.EmergencyAlert.AiFleeingBehaviour.FleeAwayProbability)
                    {
                        GetRandomFleeDirection();
                        LocationForSprinting = GenerateRandomNavmeshLocation.RandomLocation(EmergencyDirectionPoint, EmergencyRadiusIfNoCoverFound);
                        if (NavMesh.CalculatePath(transform.position, LocationForSprinting, NavMesh.AllAreas, path))
                        {
                            if (path.status == NavMeshPathStatus.PathComplete)
                            {
                                StopCoroutine(EmergencyRunTimer());
                                StartCoroutine(EmergencyRunTimer());
                                pathfinder.closestPoint = LocationForSprinting;
                                IsEmergencyPathBroken = false;
                                if (pathfinder != null)
                                {
                                    pathfinder.PauseTheNavMeshSearching = false;
                                }

                            }
                            else
                            {
                                IsEmergencyPathBroken = true;
                            }
                        }


                    }
                    else
                    {
                        IsEmergencyPathBroken = true;
                    }
                }
                ReRandomise = false;
            }
            else
            {
                pathfinder.PauseTheNavMeshSearching = false;
                IsEmergencyPathBroken = true;
            }

        }
        public void EmergencyInitialCover()
        {
            if (GeneratedSprintPoint == false)
            {
                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                {
                    StoredEnemyPositionDuringEmergency = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position;
                }
                DeregisterEmergencyNodes = false;
                // DeadBodiesSeen.Add(InvestigationCoordinates);
                // LocationForGrenadeSprint = GenerateRandomNavmeshLocation.RandomLocation(EmergencyDirectionPoint, EmergencyRadius);
                //pathfinder.CalculatePathForCombat(0f, 0f, LocationForGrenadeSprint);           
                GeneratedSprintPoint = true;
                // NavMesh.CalculatePath(transform.position, LocationForGrenadeSprint, NavMesh.AllAreas, path);
                FindValidEmergencyNode();
                

                ClosestDistanceWithEmergencyCover = Random.Range(AiEmergencyState.EmergencyAlert.MinEmergencyCoverOccupationDistance, AiEmergencyState.EmergencyAlert.MaxEmergencyCoverOccupationDistance);
               // Debug.Log(transform.name + "In Emergency");

            }
            //  elapsed += Time.deltaTime;
            //if (elapsed > NavmeshPathTimer)
            //{
            //for (int i = 0; i < path.corners.Length - 1; i++)
            //{
            //    // Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            //    LocationForGrenadeSprint = path.corners[i + 1];
            //}
            //}
         
            if (FindEmergencyPoint == true)
            {
                pathfinder.FindClosestPointTowardsDestination(LocationForSprinting);
                DistanceWithPoint = pathfinder.closestPoint - transform.position;
                DistanceWithPoint.y = 0; // Ignore the Y component

                if (DistanceWithPoint.magnitude <= ClosestDistanceWithEmergencyCover || IsReachedEmergencyPoint == true)
                {
                    if (IsReachedEmergencyPoint == false)
                    {
                       // DisableNavmeshAgentcomponent();

                        if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().EmergencyCoverType == EmergencyCoverNode.Pose.CrouchEmergencyCover)
                        {
                            Components.HumanoidAiAudioPlayerComponent.PlayNonRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceEmergencyAudioClips);

                            // LeanTween.rotateY(gameObject, PreviousEmergencyCoverNode.transform.localEulerAngles.y, AiEmergencyState.EmergencyAlert.EmergencyShooting.PoseTransitionDuration).setEase(LeanTweenType.easeInOutQuad);
                            enableIkupperbodyRotations(ref ActivateWalkAimIk);
                            Info("Reached Emergency Cover");
                            IsReachedEmergencyPoint = true;
                            FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
                            if (Components.NavMeshAgentComponent.enabled == true)
                            {
                                Components.NavMeshAgentComponent.isStopped = true;
                            }
                            IsCrouched = true;
                            anim.SetBool("Sprinting", false);
                            anim.SetBool("CrouchShootPosture", true);
                            StopSpineRotation = true;
                            SearchingForSound = false;
                            StartCoroutine(StayTimerForEmergencyPoint());
                        }
                        else
                        {
                            Components.HumanoidAiAudioPlayerComponent.PlayNonRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceEmergencyAudioClips);

                            LeanTween.rotateY(gameObject, PreviousEmergencyCoverNode.transform.localEulerAngles.y, AiEmergencyState.EmergencyAlert.PoseTransitionDuration).setEase(LeanTweenType.easeInOutQuad); 
                            enableIkupperbodyRotations(ref ActivateNoIk);

                           // DisableNavmeshAgentcomponent();

                            Info("Reached Emergency Cover");
                            IsReachedEmergencyPoint = true;
                            FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
                            if (Components.NavMeshAgentComponent.enabled == true)
                            {
                                Components.NavMeshAgentComponent.isStopped = true;
                            }
                            IsCrouched = false;
                            SetAnimationForFullBody(AiAgentAnimatorParameters.IdleParameterName);
                            StopSpineRotation = true;
                            SearchingForSound = false;
                            StartCoroutine(StayTimerForEmergencyPoint());
                        }
                    }
                }
                else
                {
                    if (IsReachedEmergencyPoint == false && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false && pathfinder.NavMeshAgentComponent.enabled == true)
                    {
                        enableIkupperbodyRotations(ref ActivateSprintingIk);
                        Info("Sprinting Towards Emergency cover");
                        StopSpineRotation = true;
                        //  AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);
                        IsCrouched = false;
                        // RotatingTransforms.ChangeRotation(transform, LocationForGrenadeSprint, transform.position, Speeds.MovementSpeeds.BodyRotationSpeed);
                        if (Components.NavMeshAgentComponent.enabled == true)
                        {
                            Components.NavMeshAgentComponent.isStopped = false;
                        }
                        AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, true);
                        SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                        SetAnimationForUpperBody(AiAgentAnimatorParameters.SprintingParameterName);
                        anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, false);
                        Components.HumanoidFiringBehaviourComponent.FireNow = false;
                    }
                }
            }
            else
            {
                pathfinder.FindClosestPointTowardsDestination(LocationForSprinting);
                DistanceWithPoint = LocationForSprinting - transform.position;
                DistanceWithPoint.y = 0; // Ignore the Y component

                if (IsEmergencyPathBroken == true)
                {
                    //Start Time to check for valid covers ( we need to do this so that in case any other cover becomes available ( when any other Ai dies )
                    if (SearchForEmergencyCover == false)
                    {
                        StartCoroutine(SearchForEmergencyCoverAvailablity());
                        SearchForEmergencyCover = true;
                    }

                    if(ReRandomise == false)
                    {
                        StartCoroutine(RandomiseSprintAwayAndShooting());
                        ReRandomise = true;
                    }

                    if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                    {
                        StandShootRandomlyInSky();
                    }
                    else
                    {
                        if (IsCrouched == false)
                        {
                            FullUpperAndLowerBodyReload();
                        }
                        else
                        {
                            Info("Crouch Reloading");
                            CoverReloadController();
                        }

                        IsEmergencyStateWrongShootingStarted = false;
                    }
                }
                else
                {
                    if (DistanceWithPoint.magnitude <= 1f)
                    {
                        LookingAtEnemy();
                        //GeneratedSprintPoint = false;
                        //IsEmergencyRun = false
                        IsEmergencyPathBroken = true;
                    }
                    else if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false && pathfinder.NavMeshAgentComponent.enabled == true && !Components.HumanoidFiringBehaviourComponent.isreloading)
                    {
                        enableIkupperbodyRotations(ref ActivateSprintingIk);
                        Info("Flee behaviour");
                        StopSpineRotation = true;
                        // AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);
                        IsCrouched = false;
                        //  RotatingTransforms.ChangeRotation(transform, LocationForGrenadeSprint, transform.position, Speeds.MovementSpeeds.BodyRotationSpeed);
                        if (Components.NavMeshAgentComponent.enabled == true)
                        {
                            Components.NavMeshAgentComponent.isStopped = false;
                        }
                        AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, true);
                        SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                        SetAnimationForUpperBody(AiAgentAnimatorParameters.SprintingParameterName);
                        anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, false);
                        Components.HumanoidFiringBehaviourComponent.FireNow = false;

                    }
                    else if (Components.HumanoidFiringBehaviourComponent.isreloading)
                    {
                        if (IsCrouched == false)
                        {
                            FullUpperAndLowerBodyReload();
                        }
                        else
                        {
                            Info("Crouch Reloading");
                            CoverReloadController();
                        }
                    }

                }
              
            }
        }
        IEnumerator StayTimerForEmergencyPointInBrokenPath()
        {
            float EmergencyAlertTimer = Random.Range(AiEmergencyState.EmergencyAlert.MinTimeBehindEmergencyCover, AiEmergencyState.EmergencyAlert.MaxTimeBehindEmergencyCover);
            yield return new WaitForSeconds(EmergencyAlertTimer);
            EmergencyCoverCheck = false;
            FindBestEmergencyCoverNodeForPathBroken = false;
            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            {
                if (PreviousEmergencyCoverNode != null)
                {
                    if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                    {
                        PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().DeactivateOccupiedText();
                    }
                }
            }
        }
        IEnumerator StayTimerForEmergencyPoint()
        {
            float EmergencyAlertTimer = Random.Range(AiEmergencyState.EmergencyAlert.MinTimeBehindEmergencyCover, AiEmergencyState.EmergencyAlert.MaxTimeBehindEmergencyCover);
            yield return new WaitForSeconds(EmergencyAlertTimer);
            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            {
                if (PreviousEmergencyCoverNode != null)
                {
                    if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                    {
                        PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().DeactivateOccupiedText();
                    }
                }
            }
            IsInitialEmergencyStateCompleted = true;
            
            // The thing is if I do not unregister emergency cover node than other friendly Ai won't be able to take the cover once this Ai timer is expired. In case there is only 1 Ai
            // and timer get expired than the same Ai takes the same cover as a sneak point basically.
            //EmergencyNodeHolding.GetComponent<EmergencyCoverNode>().IsAlreadyRegistered = false;
            //   StartInvestigation = true;
        }
        IEnumerator StayTimerForAdditionalEmergencyPoint()
        {
            float EmergencyAlertTimer = Random.Range(AiEmergencyState.EmergencyAlert.AdvancingBetweenCovers.MinTimeToRestBehindCovers, AiEmergencyState.EmergencyAlert.AdvancingBetweenCovers.MaxTimeToRestBehindCovers);
            yield return new WaitForSeconds(EmergencyAlertTimer);
            if (CountSwitchesForEmergencyHidePoint < RandomValueSwitchBetweenEmergencyCover)
            {
                Info("Move to next emergency cover");
                FindBestEmergencyHidePoint = false;
            }
            else
            {
                ForceAdditionEmergencyCoverNodes = false;
            }

            if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
            {
                if (PreviousEmergencyCoverNode != null)
                {
                    if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                    {
                        PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().DeactivateOccupiedText();
                        DeregisterEmergencyNodes = false;
                        DeregisterEmergencyCover();
                    }
                }
            }

            //StartInvestigation = true;
        }
        //public void FindClosestPathOnNavemesh(Vector3 MyDestinationPoint)
        //{
        //    if (pathfinder.IsPathComplete == false)
        //    {
        //        if (NavMeshAgentComponent.path.status == NavMeshPathStatus.PathComplete ||
        //        NavMeshAgentComponent.hasPath && NavMeshAgentComponent.path.status == NavMeshPathStatus.PathPartial)
        //        {

        //            NavMeshAgentComponent.SetDestination(MyDestinationPoint);
        //            pathfinder.IsPathComplete = true;
        //        }
        //    }
        //}
        IEnumerator CoroForEmergencyRadiusActivation()
        {
            float Randomise = Random.Range(AiEmergencyState.EmergencyAlert.MinEmergencyAlertRadiusActivationDelay, AiEmergencyState.EmergencyAlert.MaxEmergencyAlertRadiusActivationDelay);
            yield return new WaitForSeconds(Randomise);
            if (IsEmergencyRun == true && IsNearDeadBody == true)
            {
                AiEmergencyState.EmergencyAlert.EmergencyAlertRadiusComponent.gameObject.SetActive(true);
            }
            ShouldActivateEmergencyRadius = false;
        }
        IEnumerator CoroForInvestigationRadiusActivation()
        {
            float Randomise = Random.Range(AiDeadBodyAlerts.MinDeadBodyInvestigationRadiusActivationDelay, AiDeadBodyAlerts.MaxDeadBodyInvestigationRadiusActivationDelay);
            yield return new WaitForSeconds(Randomise);
            if (IsEmergencyRun == false && IsNearDeadBody == true)
            {
                AiDeadBodyAlerts.InvestigationAlertRadius.SetActive(true);
            }
            ShouldActivateInvestigationRadius = false;
        }
        public void CheckForBrokenPath()
        {
            pathfinder.FindClosestPointTowardsDestination(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position);
            if (AiEmergencyState.EmergencyAlert.PostFirstEmergencyCoverBehaviourActivity == ChooseEmergencySprintState.EmergencyShooting)
            {
                IsEmergencyPathBroken = true;
            }

        }
        private void FindClosestEmergencyCover()
        {
            GameObject closestpoint = null;
            float closestDistance = Mathf.Infinity;

            for (int i = 0; i < AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint.Length; i++)
            {
                EmergencyCoverNode EmergencyCoverNode = AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint[i].GetComponent<EmergencyCoverNode>();
                float distanceFromMeAndCover = Vector3.Distance(transform.position, AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint[i].transform.position);
                float distanceWithEnemyAndCover = Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint[i].transform.position);

               // EmergencyCoverNode.CheckRotations();
                EmergencyCoverNode.CheckForTargetPosition(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);

                if (distanceFromMeAndCover <= RandomDistanceWithEmergencyCoverToCheck && distanceWithEnemyAndCover < closestDistance && EmergencyCoverNode.IsAlreadyRegistered == false
                    && EmergencyCoverNode.DebugCoverValidation == true)
                {
                    if (PreviousEmergencyCoverNode != null)
                    {
                        float distancewithEnemyAndpreviouscover = Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, PreviousEmergencyCoverNode.transform.position);
                        if (distanceWithEnemyAndCover < distancewithEnemyAndpreviouscover)
                        {
                            if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                            {
                                PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().IsAlreadyRegistered = false;
                            }
                            closestDistance = distanceWithEnemyAndCover;
                            closestpoint = AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint[i].gameObject;
                            EmergencyCoverNode.IsAlreadyRegistered = true;
                            CurrentEmergencyCoverNode = EmergencyCoverNode;
                            PreviousEmergencyCoverNode = EmergencyCoverNode.gameObject;
                        }
                    }
                    else
                    {
                        closestDistance = distanceWithEnemyAndCover;
                        closestpoint = AiEmergencyState.EmergencyAlert.EmergencyCoverFinderComponent.EmergencyPoint[i].gameObject;
                        if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                        {
                            PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().IsAlreadyRegistered = false;
                        }
                        EmergencyCoverNode.IsAlreadyRegistered = true;
                        CurrentEmergencyCoverNode = EmergencyCoverNode;
                        PreviousEmergencyCoverNode = EmergencyCoverNode.gameObject;

                    }
                }
            }

            //if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
            //{
            //    PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().IsAlreadyRegistered = false;
            //}

            if (closestpoint != null)
            {
                AdditionalEmergencyCoverNode = closestpoint;
                PreviousEmergencyCoverNode = AdditionalEmergencyCoverNode;
            }
            else
            {
                ForceAdditionEmergencyCoverNodes = false;
            }
            // created on 5th Dec 2023
            if(EmergencyNodeHolding != null)
            {
                EmergencyNodeHolding.GetComponent<EmergencyCoverNode>().IsAlreadyRegistered = false;
            }
        }
        public void CompleteEmergencyStateBehaviour() 
        {          
            ApplyRootMotion(false);
            if (CombatStateBehaviours.EnablePostCombatScan == true)
            {
                ResetVariableForQuickScan();
                WasInCombatStateBefore = true;
            }
            if (BotMovingAwayFromGrenade == false && HealthScript.CompleteFirstHitAnimation == false)
            {
                if(CheckPathWithEnemyInEmergencyState == false && IsInitialEmergencyStateCompleted == true)
                {
                    CheckForBrokenPath();
                    if (pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                    {
                        if (pathfinder.PathIsUnreachable == true && IsEmergencyPathBroken == false)
                        {
                            IsEmergencyPathBroken = true;
                        }
                        Info("Inital emergency state is completed");
                        GeneratedSprintPoint = false;
                        IsEmergencyRun = false;
                        CheckPathWithEnemyInEmergencyState = true;
                    }

                }

                if (IsEmergencyRun == true && IsNearDeadBody == true && IsInitialEmergencyStateCompleted == false)
                {
                    IsEmergencyStateCurrentlyActive = true;
                    
                    // Debug.Log("1st State" + " " + transform.name);

                    ShouldActivateInvestigationRadius = false;
                    if (ShouldActivateEmergencyRadius == false)
                    {
                        StartCoroutine(CoroForEmergencyRadiusActivation());
                        ShouldActivateEmergencyRadius = true;
                    }
                    if (NonCombatBehaviours.EnableDeadBodyAlerts == true)
                    {
                        AiDeadBodyAlerts.InvestigationAlertRadius.SetActive(false);
                    }
                    EmergencyInitialCover();      
                    WaitForEmergencyStateToFinish = true;

                    if (AiEmergencyState.EmergencyAlert.PostFirstEmergencyCoverBehaviourActivity == ChooseEmergencySprintState.AdvancingBetweenCoversAndZigZagAdvancing)
                    {
                        ForceAdditionEmergencyCoverNodes = true;
                    }
                    else
                    {
                        ForceAdditionEmergencyCoverNodes = false;
                    }
                }
                else if (WaitForEmergencyStateToFinish == true && IsNearDeadBody == true
                   && IsEmergencyRun == false && ForceAdditionEmergencyCoverNodes == true && IsEmergencyPathBroken == false)
                {
                    IsEmergencyStateCurrentlyActive = true;
                    //Debug.Log("2nd State" + " " + transform.name);
                    if (FindBestEmergencyHidePoint == false)
                    {
                        if (CountSwitchesForEmergencyHidePoint < RandomValueSwitchBetweenEmergencyCover)
                        {                           
                            Info("Finding closest emergency cover point");
                            ReachedAdditionalCoverNode = false;
                            FindClosestEmergencyCover();
                            ++CountSwitchesForEmergencyHidePoint;
                            FindBestEmergencyHidePoint = true;

                        }
                        else
                        {
                            ForceAdditionEmergencyCoverNodes = false;
                        }
                    }
                    pathfinder.FindClosestPointTowardsDestination(PreviousEmergencyCoverNode.transform.position);

                    if (FindBestEmergencyHidePoint == true && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                    {
                      
                        Vector3 SaferDistance = pathfinder.closestPoint - transform.position;

                        //if (transform.name == "Humanoid AI Prefab_18")
                        //{
                        //    Debug.Log("BREAK");
                        //    if (SaferDistance.magnitude <= ClosestDistanceWithEmergencyCover)
                        //    {
                        //        Debug.Log("Condition is true");
                        //    }
                        //    else if(SaferDistance.magnitude > ClosestDistanceWithEmergencyCover)
                        //    {
                        //        Debug.Log("Condition is false");
                        //        Debug.Log("SaferDistance.magnitude: " + SaferDistance.magnitude.ToString("F10"));
                        //        Debug.Log("ClosestDistanceWithEmergencyCover: " + ClosestDistanceWithEmergencyCover.ToString("F10"));
                        //    }
                        //}
                        if (SaferDistance.magnitude <= ClosestDistanceWithEmergencyCover || ReachedAdditionalCoverNode == true)
                        {
                            Info("Reached emergency cover point");
                          //  Debug.Log("notreach" + transform.name);
                            if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().EmergencyCoverType == EmergencyCoverNode.Pose.CrouchEmergencyCover)
                            {
                                enableIkupperbodyRotations(ref ActivateWalkAimIk);
                                LookingAtEnemy();
                                IsCrouched = true;
                                anim.SetBool("Sprinting", false);
                                SetAnimationForFullBody(AiAgentAnimatorParameters.CrouchAimingParameterName);
                                anim.SetBool("CrouchShootPosture", true);
                                anim.SetBool("StandShootPosture", false);

                            }
                            else
                            {
                                enableIkupperbodyRotations(ref ActivateNoIk);
                                anim.SetBool("Sprinting", false);
                                IsCrouched = false;
                                anim.SetBool("CrouchShootPosture", false);
                                anim.SetBool("StandShootPosture", false);
                                SetAnimationForFullBody(AiAgentAnimatorParameters.IdleParameterName);

                            }

                            if (ReachedAdditionalCoverNode == false)
                            {
                               // DisableNavmeshAgentcomponent();
                                if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().EmergencyCoverType == EmergencyCoverNode.Pose.StandEmergencyCover)
                                {
                                   LeanTween.rotateY(gameObject, PreviousEmergencyCoverNode.transform.localEulerAngles.y, AiEmergencyState.EmergencyAlert.PoseTransitionDuration).setEase(LeanTweenType.easeInOutQuad);
                                }
                                // enableIkupperbodyRotations(ref ActivateWalkAimIk);
                                IsReachedEmergencyPoint = true;
                                FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
                                if (Components.NavMeshAgentComponent.enabled == true)
                                {
                                    Components.NavMeshAgentComponent.isStopped = true;
                                }
                                //IsCrouched = true;
                                //anim.SetBool("Sprinting", false);
                                //anim.SetBool("CrouchShootPosture", true);
                                StopSpineRotation = true;
                                SearchingForSound = false;
                               // Debug.Log("Searching For Sound 5" + transform.name);
                                StartCoroutine(StayTimerForAdditionalEmergencyPoint());
                                ReachedAdditionalCoverNode = true;
                            }
                 
                        }
                        else if(pathfinder.IsNavMeshObstacleDisabled == false && pathfinder.NoMoreChecks == true && pathfinder.NavMeshAgentComponent.enabled == true && pathfinder.NavMeshAgentComponent.enabled == true)
                        {
                            //Debug.Log("Continously" + transform.name + "FindBestEmergencyHidePoint" + FindBestEmergencyHidePoint + "NOMORECHECKS" + pathfinder.NoMoreChecks);
                            Info("Moving towards emergency cover point" + " " + SaferDistance.magnitude.ToString() + " " + "distance left" + "closestdistance" + ClosestDistanceWithEmergencyCover);
                            enableIkupperbodyRotations(ref ActivateSprintingIk);
                            StopSpineRotation = true;
                            // AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);
                            IsCrouched = false;
                            // RotatingTransforms.ChangeRotation(transform, SaferDistance, transform.position, Speeds.MovementSpeeds.BodyRotationSpeed);
                            if (Components.NavMeshAgentComponent.enabled == true)
                            {
                                Components.NavMeshAgentComponent.isStopped = false;
                            }
                            AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, false);
                            SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                            SetAnimationForUpperBody(AiAgentAnimatorParameters.SprintingParameterName);
                            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, false);
                            Components.HumanoidFiringBehaviourComponent.FireNow = false;
                        }
                    }
                    else
                    {
                        Info("something wrong");
                    }
                }
                else if (WaitForEmergencyStateToFinish == true && IsNearDeadBody == true && IsEmergencyRun == false && ForceAdditionEmergencyCoverNodes == false && IsEmergencyPathBroken == false)
                {
                    IsEmergencyStateCurrentlyActive = true;
                    // Debug.Log("3rd State" + " " + transform.name);
                    if (CreateCorrectEmergencySprintCoordinate == false)
                    {
                        if (CountSwitchesForEmergencyStates < AiEmergencyState.EmergencyAlert.ZigZagAdvancing.ZigzagTurnsAmount.Count)
                        {
                            
                            Info("Searching for coordinate");
                            Vector3 prevpoint = pathfinder.closestPoint;
                            FinalDestinationForAiAgent = false;
                            Vector3 CoordinateForEmergencySprint = GenerateRandomNavmeshLocation.RandomLocationInVector3
                            (transform.position, Random.Range(AiEmergencyState.EmergencyAlert.ZigZagAdvancing.ZigzagTurnsAmount[CountSwitchesForEmergencyStates].MinZigZagPointCreationRadius,
                            AiEmergencyState.EmergencyAlert.ZigZagAdvancing.ZigzagTurnsAmount[CountSwitchesForEmergencyStates].MaxZigZagPointCreationRadius));

                            pathfinder.FindClosestPointTowardsDestination(CoordinateForEmergencySprint);

                            Vector3 targetPosition = StoredEnemyPositionDuringEmergency;
                            if (StoredEnemyPositionDuringEmergency == Vector3.zero)
                            {
                                // Calculate the target position without changing the Y-axis
                                 targetPosition = new Vector3(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.x,
                                    AiEmergencyState.EmergencyAlert.ZigZagAdvancing.CoordinateCreator.transform.position.y, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position.z);
                            }
                            else
                            {
                                // Calculate the target position without changing the Y-axis
                                 targetPosition = new Vector3(StoredEnemyPositionDuringEmergency.x,
                                    AiEmergencyState.EmergencyAlert.ZigZagAdvancing.CoordinateCreator.transform.position.y, StoredEnemyPositionDuringEmergency.z);
                            }
                        

                            // Rotate the GameObject to look at the target position
                            AiEmergencyState.EmergencyAlert.ZigZagAdvancing.CoordinateCreator.transform.LookAt(targetPosition);

                            Vector3 dir = pathfinder.closestPoint - AiEmergencyState.EmergencyAlert.ZigZagAdvancing.CoordinateCreator.transform.position;

                            Angle = Vector3.Angle(dir, AiEmergencyState.EmergencyAlert.ZigZagAdvancing.CoordinateCreator.transform.forward);

                            if (Angle < GetFovAngleForZigZagMovement && pathfinder.closestPoint != prevpoint)
                            {
                                //NavMeshPath path = new NavMeshPath();

                                //if (NavMesh.CalculatePath(transform.position, pathfinder.closestPoint, NavMesh.AllAreas, path))
                                //{
                                //    if (path.status == NavMeshPathStatus.PathComplete)
                                //    {
                              //  Debug.Log("Sprinting Randomly Towards Forward Direction");
                                Info("Sprinting in forward direction");
                                CreateCorrectEmergencySprintCoordinate = true;
                                ++CountSwitchesForEmergencyStates;

                                //    }
                                //}
                            }
                        }
                        else
                        {
                            Info("Searching for enemy coordinate");
                            Vector3 prevpoint = pathfinder.closestPoint;

                            Vector3 CoordinateForEmergencySprint = GenerateRandomNavmeshLocation.RandomLocationInVector3
                            (StoredEnemyPositionDuringEmergency, Random.Range(AiEmergencyState.EmergencyAlert.ZigZagAdvancing.MinEnemySupposedPositionOffset,
                            AiEmergencyState.EmergencyAlert.ZigZagAdvancing.MaxEnemySupposedPositionOffset));

                            //SpawnCubes.instance.Spawning(CoordinateForEmergencySprint, transform.name);
                            //Debug.Break();

                            pathfinder.FindClosestPointTowardsDestination(CoordinateForEmergencySprint);

                            //if(pathfinder.NoMoreChecks == true)
                            //{
                            //NavMeshPath path = new NavMeshPath();

                            //if (NavMesh.CalculatePath(transform.position, pathfinder.closestPoint, NavMesh.AllAreas, path))
                            //{
                            //    if (path.status == NavMeshPathStatus.PathComplete)
                            //    {
                            if (pathfinder.closestPoint != prevpoint)
                            {
                                Info("Sprinting near the enemy coordinates");
                                CreateCorrectEmergencySprintCoordinate = true;
                                FinalDestinationForAiAgent = true;
                            }
                            //    }
                            //}
                            //}

                        }
                    }

                    if (CreateCorrectEmergencySprintCoordinate == true)
                    {
                        Vector3 SaferDistance = pathfinder.closestPoint - transform.position;

                        if (SaferDistance.magnitude <= 1f)
                        {
                            CreateCorrectEmergencySprintCoordinate = false;
                            if (FinalDestinationForAiAgent == true)
                            {
                                ResetEmergencyBehaviourVariables();
                                Info("Sprinting Completed");
                                WaitForEmergencyStateToFinish = false;
                                IsEmergencyStateExpiredCompletely = true;
                            }
                        }
                        else  
                        {

                           // Debug.Log("Continously 2nd part" + transform.name);
                            if (FinalDestinationForAiAgent == true)
                            {
                                Info("Sprinting near the enemy coordinates" + " " + SaferDistance.magnitude.ToString() + " " + "Distance Left");
                            }
                            else
                            {
                                Info("Sprinting near the Randomly created coordinates" + " " + SaferDistance.magnitude.ToString() + " " + "Distance Left");
                            }
                            enableIkupperbodyRotations(ref ActivateSprintingIk);

                            StopSpineRotation = true;
                            // AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 1f, false);
                            IsCrouched = false;
                            // RotatingTransforms.ChangeRotation(transform, SaferDistance, transform.position, Speeds.MovementSpeeds.BodyRotationSpeed);
                            if (Components.NavMeshAgentComponent.enabled == true)
                            {
                                Components.NavMeshAgentComponent.isStopped = false;
                            }
                            AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, false);
                            SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                            SetAnimationForUpperBody(AiAgentAnimatorParameters.SprintingParameterName);
                            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, false);
                            Components.HumanoidFiringBehaviourComponent.FireNow = false;
                        }

                    }
                    else
                    {
                        Info("something wrong");
                    }
                }
                else if (IsEmergencyPathBroken == true)
                {
                    IsEmergencyStateCurrentlyActive = true;
                    if (EmergencyCoverCheck == false)
                    {
                        if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                        {
                            if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().EmergencyCoverShootingPoints.Length >= 1)
                            {
                                pathfinder.FindClosestPointTowardsDestination(PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().EmergencyCoverShootingPoints[ShootingPointForEmergencyCoverNode].position);

                                Vector3 SaferDistance = pathfinder.closestPoint - transform.position;

                                //Debug.Log(SaferDistance.magnitude + " " + transform.name + "ShootingPoint");

                                if (SaferDistance.magnitude <= 0.7f && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                                {
                                    DisableNavmeshAgentcomponent();
                                    if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                                    {
                                        StandShootRandomlyInSky(); 
                                    }
                                    else
                                    {
                                        if (IsCrouched == false)
                                        {
                                            FullUpperAndLowerBodyReload();
                                        }
                                        else
                                        {
                                            Info("Crouch Reloading");
                                            CoverReloadController();
                                        }
                                    }
                                }
                                else if(pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false && pathfinder.NavMeshAgentComponent.enabled == true)
                                {
                                    LookingAtspecificLocation(pathfinder.closestPoint);
                                    enableIkupperbodyRotations(ref ActivateSprintingIk);
                                    StopSpineRotation = true;
                                    IsCrouched = false;
                                    if (Components.NavMeshAgentComponent.enabled == true)
                                    {
                                        Components.NavMeshAgentComponent.isStopped = false;
                                    }
                                    AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, false);
                                    SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                                    SetAnimationForUpperBody(AiAgentAnimatorParameters.SprintingParameterName);
                                    anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, false);
                                    Components.HumanoidFiringBehaviourComponent.FireNow = false;
                                }
                            }
                            else
                            {
                                EmergencyCoverCheck = true;
                                ResetEmergencyDecisionMaking = false;
                            }
                        }
                        else
                        {
                            //Start Time to check for valid covers ( we need to do this so that in case any other cover becomes available ( when any other Ai dies )
                            if(SearchForEmergencyCover == false)
                            {
                                StartCoroutine(SearchForEmergencyCoverAvailablity());
                                SearchForEmergencyCover = true;
                            }
                        }

                    }
                    else
                    {
                        if (FindBestEmergencyCoverNodeForPathBroken == false)
                        {
                            ReachedEmergencyCoverCompletely = false;
                            DidReachedEmergencyPointInPathBroken = false;

                            ClosestDistanceWithEmergencyCover = Random.Range(AiEmergencyState.EmergencyAlert.MinEmergencyCoverOccupationDistance, AiEmergencyState.EmergencyAlert.MaxEmergencyCoverOccupationDistance);

                            float Randomise = Random.Range(0f, 100f);
                            if (Randomise <= AiEmergencyState.EmergencyAlert.EmergencyShooting.EmergencyCoverSwitchingProbability
                                && AiEmergencyState.EmergencyAlert.EmergencyShooting.EmergencyCoverSwitchingProbability >= 1)
                            {
                                //Previous cover node should be not taken into account and we need to find new closest emergency cover node
                                SwitchingBetweenNewEmergencyCover(true);
                            }
                            else
                            {
                                //Previous cover node should be used as a emergency cover node
                                SwitchingBetweenNewEmergencyCover(false);
                            }
                            
                            FindBestEmergencyCoverNodeForPathBroken = true;
                        }

                        if(IsNewEmergencyCoverFound == true)
                        {
                            pathfinder.FindClosestPointTowardsDestination(LocationForSprinting);
                           // Vector3 SaferDistance = pathfinder.closestPoint - transform.position;

                            Vector3 SaferDistance = pathfinder.closestPoint - transform.position;
                            SaferDistance.y = 0; // Ignore the Y component
                            //Debug.Log(SaferDistance.magnitude + " " + transform.name + "EmergencyCoverPoint");

                            if (SaferDistance.magnitude <= ClosestDistanceWithEmergencyCover && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false
                                || ReachedEmergencyCoverCompletely == true && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                            {
                                DisableNavmeshAgentcomponent();

                                if (!Components.HumanoidFiringBehaviourComponent.isreloading)
                                {
                                    if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().EmergencyCoverType == EmergencyCoverNode.Pose.CrouchEmergencyCover)
                                    {
                                        enableIkupperbodyRotations(ref ActivateWalkAimIk);

                                        IsCrouched = true;
                                        anim.SetBool("Sprinting", false);
                                        SetAnimationForFullBody(AiAgentAnimatorParameters.CrouchAimingParameterName);
                                        anim.SetBool("CrouchShootPosture", true);
                                        anim.SetBool("StandShootPosture", false);

                                    }
                                    else
                                    {
                                        enableIkupperbodyRotations(ref ActivateNoIk);

                                        IsCrouched = false;
                                        anim.SetBool("CrouchShootPosture", false);
                                        anim.SetBool("StandShootPosture", false);
                                        SetAnimationForFullBody(AiAgentAnimatorParameters.IdleParameterName);

                                    }
                                }
                                else
                                {
                                    if (PreviousEmergencyCoverNode.GetComponent<EmergencyCoverNode>().EmergencyCoverType == EmergencyCoverNode.Pose.CrouchEmergencyCover)
                                    {
                                        Info("Crouch Reloading");
                                        CoverReloadController();

                                    }
                                    else
                                    {
                                        DidReachedEmergencyPointInPathBroken = false;
                                        FullUpperAndLowerBodyReload();
                                    }
                                }
                                   
                                Info("Reached Emergency Cover Point");
                                Components.HumanoidFiringBehaviourComponent.FireNow = false;
                                IsStationedShoot = false;

                                if (DidReachedEmergencyPointInPathBroken == false)
                                {
                                    IsReachedEmergencyPoint = true;
                                    LeanTween.rotateY(gameObject, PreviousEmergencyCoverNode.transform.localEulerAngles.y, AiEmergencyState.EmergencyAlert.PoseTransitionDuration).setEase(LeanTweenType.easeInOutQuad);
                                    StartCoroutine(StayTimerForEmergencyPointInBrokenPath());
                                    FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
                                    if (Components.NavMeshAgentComponent.enabled == true)
                                    {
                                        Components.NavMeshAgentComponent.isStopped = true;
                                    }
                                    StopSpineRotation = true;
                                    SearchingForSound = false;
                                    DidReachedEmergencyPointInPathBroken = true;
                                }

                                if (SaferDistance.magnitude <= ClosestDistanceWithEmergencyCover)
                                {
                                    ReachedEmergencyCoverCompletely = true;
                                }
                            }
                            else if(pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false && pathfinder.NavMeshAgentComponent.enabled == true)
                            {

                                LookingAtspecificLocation(pathfinder.closestPoint);
                                enableIkupperbodyRotations(ref ActivateSprintingIk);
                                Info("Sprinting Towards Emergency cover");
                                StopSpineRotation = true;
                                IsCrouched = false;
                                if (Components.NavMeshAgentComponent.enabled == true)
                                {
                                    Components.NavMeshAgentComponent.isStopped = false;
                                }
                                AgentMovement(Components.NavMeshAgentComponent, Speeds.MovementSpeeds.SprintSpeed, true);
                                SetAnimationForFullBody(AiAgentAnimatorParameters.SprintingParameterName);
                                SetAnimationForUpperBody(AiAgentAnimatorParameters.SprintingParameterName);
                                anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, false);
                                Components.HumanoidFiringBehaviourComponent.FireNow = false;
                            }
                        }
                        else
                        {
                            EmergencyCoverCheck = false;
                            FindBestEmergencyCoverNodeForPathBroken = false;
                        }
                       
                    }
                }
                else if(CheckPathWithEnemyInEmergencyState == true)
                {
                    InvestigationCompleted();
                }
            }
            else
            {
                SprintAwayFromGrenade();
            }

           
        }
        
        public void InvestigationCompleted()
        {
            ResetEmergencyBehaviourVariables();
            FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
            VisibilityCheck.ConnectionLost = false;
            if (Components.NavMeshAgentComponent.enabled == true)
            {
                Components.NavMeshAgentComponent.isStopped = true;
            }
            //if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
            //{
                SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
            //}
            DefaultInvestigationBehaviour();
            StopSpineRotation = true;
            SearchingForSound = false;
            ResetToNormalState = false;
        }
        public void ResetEmergencyBehaviourVariables()
        {           
            if (CurrentEmergencyCoverNode != null)
            {
                if (CurrentEmergencyCoverNode.GetComponent<EmergencyCoverNode>() != null)
                {
                    CurrentEmergencyCoverNode.GetComponent<EmergencyCoverNode>().IsAlreadyRegistered = false;
                    PreviousEmergencyCoverNode = gameObject;
                }
            }
            GeneratedSprintPoint = false;
            IsEmergencyStateCurrentlyActive = false;
            IsEmergencyPathBroken = false; 
            FindEmergencyPoint = false;
            FindBestEmergencyHidePoint = false;
            ReachedAdditionalCoverNode = false;
            CreateCorrectEmergencySprintCoordinate = false;
            FinalDestinationForAiAgent = false;
            ForceAdditionEmergencyCoverNodes = false;
            WaitForEmergencyStateToFinish = false;
            IsReachedEmergencyPoint = false;

            //// So if emergency state get called again after getting expired we make sure to reset the navigation path. 
            ///  keep this code commeneted as when zombie first start moving to the closest path finding point this code was reseting the closest poin to infinity again which make the path and distance to the path calculation invalid
            ///  This is the situation tested in parking area when player is on the roof and 1 zombie spawned on road and stationed and moving towards the player closest coordinate.
            //Debug.Log("Reseting Emergency");
            //pathfinder.closestPoint = Vector3.positiveInfinity;

            CountSwitchesForEmergencyHidePoint = 0;
            CountSwitchesForEmergencyStates = 0;

            if(AiEmergencyState.EmergencyAlert.EmergencyAlertRadiusComponent != null)
            {
                AiEmergencyState.EmergencyAlert.EmergencyAlertRadiusComponent.gameObject.SetActive(false);
            }

            if (NonCombatBehaviours.EnableDeadBodyAlerts == true)
            {
                if (AiDeadBodyAlerts.InvestigationAlertRadius != null)
                {
                    AiDeadBodyAlerts.InvestigationAlertRadius.SetActive(false);
                }
            }

            IsNearDeadBody = false;
            StartInvestigation = false;
            StayTimeEnded = false;
            IsInEmergencyState = false;

            IsEmergencyPathBroken = false;
            EmergencyCoverCheck = false;
            ResetEmergencyDecisionMaking = false;
            FindBestEmergencyCoverNodeForPathBroken = false;
            DidReachedEmergencyPointInPathBroken = false;
            IsEmergencyStateWrongShootingStarted = false;
            ReachedEmergencyCoverCompletely = false;
            IsShootingDuringEmergencyState = false;

            CheckPathWithEnemyInEmergencyState = false;
            IsInitialEmergencyStateCompleted = false;
        }

        IEnumerator SearchForEmergencyCoverAvailablity()
        {
            SearchTimerForEmergencyCover = Random.Range(AiEmergencyState.EmergencyAlert.EmergencyShooting.MinVacantEmergencyCoverCheckInterval, AiEmergencyState.EmergencyAlert.EmergencyShooting.MaxVacantEmergencyCoverCheckInterval); 
            yield return new WaitForSeconds(SearchTimerForEmergencyCover);
            SwitchingBetweenNewEmergencyCover(true);
            SearchForEmergencyCover = false;
            if(IsNewEmergencyCoverFound == true)
            {
                FindEmergencyPoint = false;
                GeneratedSprintPoint = false;
                IsEmergencyRun = false;
                EmergencyCoverCheck = true;
                IsEmergencyPathBroken = true;
                FindBestEmergencyCoverNodeForPathBroken = true;
            }
        }
        IEnumerator StayingNearEnemyPursueCoordinate()
        {
            yield return new WaitForSeconds(MaxTimeToScanAfterPursue);
            FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
            VisibilityCheck.ConnectionLost = false;
            //StartSearching();
            StayingNearPursuingCoordinate = false;
            StopSpineRotation = true;
//            Debug.Log("Searching For Sound 8" + transform.name);
            SearchingForSound = false;
            SetAnimationForFullBody("null");
            SetAnimationForUpperBody("null");
            StareTimeBeginForMovableAi = false;
            StareTimeCompletedForMovableAi = false;


        }
        public void DeadBodyInvestigation()
        {           
            GeneratedSprintPoint = false;

            ShouldActivateEmergencyRadius = false;
            IsReachedEmergencyPoint = false;

            if (ShouldActivateInvestigationRadius == false)
            {
                
                StartCoroutine(CoroForInvestigationRadiusActivation());
                ShouldActivateInvestigationRadius = true;
            }
            AiEmergencyState.EmergencyAlert.EmergencyAlertRadiusComponent.gameObject.SetActive(false);

            if (StartInvestigation == false)
            {
                if (ResetToNormalState == false)
                {
                    Components.HumanoidAiAudioPlayerComponent.PlayNonRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceDeadBodyInvestigationAudioClips);
                    StoppingDistanceFromTheDeadBody = Random.Range(AiDeadBodyAlerts.MinStoppingDistanceFromTheDeadBody, AiDeadBodyAlerts.MaxStoppingDistanceFromTheDeadBody);              
                    RandomDirAfterTargetLost = GenerateRandomNavmeshLocation.RandomLocationInVector3(InvestigationCoordinates, 3f);
                    NavMesh.CalculatePath(transform.position, RandomDirAfterTargetLost, NavMesh.AllAreas, path);
                    ResetToNormalState = true;
                }

                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    RandomDirAfterTargetLost = path.corners[i + 1];
                }

                ApplyRootMotion(false);

                Vector3 Dis = RandomDirAfterTargetLost - transform.position;

                if (Dis.magnitude < StoppingDistanceFromTheDeadBody)
                {
                    Info("Staying Near The DeadBody");
                    FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
                    StartInvestigation = true;
                    if (Components.NavMeshAgentComponent.enabled == true)
                    {
                        Components.NavMeshAgentComponent.isStopped = true;
                    }
                    SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                    ConnectWithUpperBodyAimingAnimation();
                    StartCoroutine(StayTimerNearDeadBody());
                    StopSpineRotation = true;
//                    Debug.Log("Searching For Sound 9" + transform.name);
                    SearchingForSound = false;
                }
                else
                {
                    Components.NavMeshAgentComponent.enabled = true;
                    NavMeshObstacleComponent.enabled = false;
                    Components.NavMeshAgentComponent.isStopped = false;
                    Info("Moving Near The DeadBody");
                    StopSpineRotation = true;
                    enableIkupperbodyRotations(ref ActivateWalkAimIk);
                    Components.NavMeshAgentComponent.speed = Speeds.MovementSpeeds.WalkForwardAimingSpeed;
                    if (anim != null)
                    {
                        SetAnimationForFullBody(AiAgentAnimatorParameters.WalkForwardParameterName);
                        SetAnimationForUpperBody("StandShootPosture");
                        anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                    }
                    if (Components.NavMeshAgentComponent.enabled == true)
                    {
                        Components.NavMeshAgentComponent.SetDestination(RandomDirAfterTargetLost);
                    }
                }
            }
            else
            {
                if (StayTimeEnded == true)
                {
                    for (int i = 0; i < path.corners.Length - 1; i++)
                    {
                        RandomDirAfterTargetLost = path.corners[i + 1];
                    }

                    ApplyRootMotion(false);
                    if (Vector3.Distance(RandomDirAfterTargetLost, this.transform.position) < 1)
                    {
                        Info("Investigation Completed");
                        InvestigationCompleted();
                    }
                    else
                    {
                        Components.NavMeshAgentComponent.enabled = true;
                        NavMeshObstacleComponent.enabled = false;
                        Info("Investigating The Area");
                        Components.NavMeshAgentComponent.isStopped = false;
                        StopSpineRotation = true;

                        enableIkupperbodyRotations(ref ActivateWalkAimIk);
                        Components.NavMeshAgentComponent.speed = Speeds.MovementSpeeds.WalkForwardAimingSpeed;
                        if (anim != null)
                        {
                            SetAnimationForFullBody(AiAgentAnimatorParameters.WalkForwardParameterName);
                            SetAnimationForUpperBody("StandShootPosture");
                            anim.SetBool(AiAgentAnimatorParameters.DefaultStateParameterName, true);
                        }
                        if (Components.NavMeshAgentComponent.enabled == true)
                        {
                            Components.NavMeshAgentComponent.SetDestination(RandomDirAfterTargetLost);
                        }
                        Debug.DrawLine(transform.position, RandomDirAfterTargetLost, Color.blue);
                    }
                }
            }
        }
        public void ConnectionLostForMovingBots() // Alert If Target Lost 
        {
            CheckTime = false;
            if (Components.HumanoidFiringBehaviourComponent != null)
            {
                Components.HumanoidFiringBehaviourComponent.FireNow = false;
            }

            if (VisibilityCheck.ConnectionLost == true && IsNearDeadBody == false)
            {
                if (StareTimeBeginForMovableAi == false)
                {
                    StartCoroutine(NormaliseStateAfterAlertIfMovable());
                    StareTimeBeginForMovableAi = true;
                }


                if (AgentRole == Role.Zombie)
                {
                    if (StareTimeCompletedForMovableAi == false && StareTimeAtLastKnownEnemyCoordinate > 0)
                    {
                        Components.NavMeshAgentComponent.isStopped = true;
                        SetAnimationForFullZombieBody(ZombieAiAnimatorParameters.IdleParameterName);
                    }
                }
               
                if(StayingNearPursuingCoordinate == false && StareTimeCompletedForMovableAi == true && CombatStateBehaviours.ChooseEnemyPursueType != EnemyPursueTypes.EnableStationedEnemyPursue) 
                {
                    //  CombatStarted = false;// added on 11th oct
                   
                    ResetVariablesOnce();
                    Info("Enemy Lost Alert");
                    if (ResetToNormalState == false)
                    {
                        Components.HumanoidAiAudioPlayerComponent.PlayNonRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceEnemyLostAudioClips);
                        RandomDirAfterTargetLost = GenerateRandomNavmeshLocation.RandomLocation(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform, AiPursue.EnemyPuruseErrorRadius);
                        ResetToNormalState = true;
                    }

                    pathfinder.FindClosestPointTowardsDestination(RandomDirAfterTargetLost);
                    DistanceBetweenMeAndEnemyLastLocation = pathfinder.closestPoint - transform.position;
                }


                if (DistanceBetweenMeAndEnemyLastLocation.magnitude <= ClosestDistanceToStopFromCoordinate && pathfinder.NoMoreChecks == true
                    && pathfinder.PathIsUnreachable == false && StareTimeCompletedForMovableAi == true && pathfinder.IsNavMeshObstacleDisabled == false)
                {
                    if (StayingNearPursuingCoordinate == false)
                    {
                        ScanValueAfterPursue = 1000;
                        SetAnimationForFullBody("null");
                        SetAnimationForUpperBody("null");
                        SetAnimationForFullZombieBody("null");
                        StartCoroutine(StayingNearEnemyPursueCoordinate());
                        if (Components.NavMeshAgentComponent.enabled == true)
                        {
                            Components.NavMeshAgentComponent.isStopped = true;
                        }
                        //if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
                        //{
                        //    SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                        //}
                        StayingNearPursuingCoordinate = true;
                    }
                    if (PlayDefaultBehaviourSoundsNow == true)
                    {
                        Components.HumanoidAiAudioPlayerComponent.PlayRecurringSoundClips(Components.HumanoidAiAudioPlayerComponent.RecurringSounds.DefaultBehaviourAudioClips);
                    }
                    Info("Scanning");
                    SearchingState();
                }
                else if(StayingNearPursuingCoordinate == false && StareTimeCompletedForMovableAi == true)
                {
                    
                    ApplyRootMotion(false);
                    if (pathfinder.PathIsUnreachable == false && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false && pathfinder.NavMeshAgentComponent.enabled == true
                        && CombatStateBehaviours.ChooseEnemyPursueType != EnemyPursueTypes.EnableStationedEnemyPursue)
                    {
                       // SpawnCubes.instance.Spawning(pathfinder.closestPoint, transform.name);
                        AlertedStateAfterTargetLostController();
                        StopSpineRotation = true;
                        // enableIkupperbodyRotations(ref ActivateWalkAimIk);
                       // Debug.DrawLine(transform.position, pathfinder.closestPoint, Color.blue);
                    }
                    else if(pathfinder.PathIsUnreachable == true && pathfinder.NoMoreChecks == true && pathfinder.IsNavMeshObstacleDisabled == false)
                    {
                        if (StareTimeBeginForMovableAi == false)
                        {
                            StartCoroutine(NormaliseStateAfterAlertIfMovable());
                            StareTimeBeginForMovableAi = true;
                        }
                        // ConnectionLostForStationarybot();
                    }
                
                }
                //}
                //else
                //{
                //    FindEnemiesScript.DetectionRadius = SaveDetectingDistance;
                //    VisibilityCheck.ConnectionLost = false;
                //    Components.NavMeshAgentComponent.isStopped = true;
                //    if (Components.HumanoidFiringBehaviourComponent.PlayingFiringAnimation == false)
                //    {
                //        SetAnimationForFullBody(AiAgentAnimatorParameters.IdleAimingParameterName);
                //    }
                //    StartSearching();
                //    StopSpineRotation = true;
                //    SearchingForSound = false;
                //}
            }
            else if (IsNearDeadBody == true && NonCombatBehaviours.EnableEmergencyAlerts == true && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != transform.root.transform)
            {
                CompleteEmergencyStateBehaviour();
                IsInDefaultInvestigation = false;
            }
            else
            {
                DefaultInvestigationBehaviour();
            }
        }
        //public void ControlHeadIK()
        //{
        //    if (HeadIkScript.EnableCustomPointOfInterest == true)
        //    {
        //        if (CombatStarted == false && SearchingForSound == false && VisibilityCheck.ConnectionLost == false && IsShotMade == false && HeadIkScript != null)
        //        {
        //            DebugInfo.CurrentState = "SCANNING";

        //            if(HeadIkScript.Check == false)
        //            {
        //                RandomPoint = HeadIkScript.CustomLookPoints[HeadIkScript.PreviousValue].transform.position;
        //                HeadIkScript.Check = true;
        //            }

        //            RandomPoint = Vector3.Lerp(RandomPoint, HeadIkScript.CustomLookPoints[HeadIkScript.ScanValue].transform.position, HeadIkScript.HeadRotationSpeed * Time.deltaTime);
        //            //anim.SetLookAtPosition(HeadIkScript.CustomLookPoints[HeadIkScript.ScanValue].transform.position);
        //            // Set look-at position for both head and spine

        //            anim.SetLookAtPosition(RandomPoint);
        //        }
        //    }
        //    else
        //    {
        //        if (CombatStarted == false && SearchingForSound == false && VisibilityCheck.ConnectionLost == false && IsShotMade == false && HeadIkScript != null)
        //        {
        //            DebugInfo.CurrentState = "SCANNING";
        //            if (GenerateADetectPointForHead == false)
        //            {
        //                if (ChangePosOfRandomPointWhileSearch == false)
        //                {
        //                    RandomPoint = GenerateRandomNavmeshLocation.RandomLocation(transform, HeadIkScript.AnchorPointDistance);
        //                    ChangePosOfRandomPointWhileSearch = true;
        //                    //if(ScanScript != null)
        //                    //{
        //                    //    StartCoroutine(SearchScan());
        //                    //}

        //                    //  HorizontalValues(HeadIkScript.MinHorizontalSearching, HeadIkScript.MaxHorizontalSearching);                          
        //                }

        //                StartCoroutine(RotateHeadForSearching());
        //                GenerateADetectPointForHead = true;
        //            }
        //            RandomPoint = Vector3.Lerp(RandomPoint, newpos, HeadIkScript.HeadRotationSpeed * Time.deltaTime);
        //           // anim.SetLookAtWeight(1f, 0f, 1f, 0.5f, 0.5f);
        //            anim.SetLookAtPosition(RandomPoint);


        //        }
        //    }
        //}
        //private void OnAnimatorIK(int layerIndex)
        //{

        //    if (NonCombatBehaviours.EnableHeadMovements == true)
        //    {
        //        if (IsBodyguard == true && IsScanning == true)
        //        {
        //            ControlHeadIK();
        //            HeadWeight = 1f;
        //            EyesWeight = 0.5f;
        //        }
        //        else if (IsBodyguard == false)
        //        {
        //            ControlHeadIK();
        //            HeadWeight = 1f;
        //            EyesWeight = 0.5f;
        //        }
        //    }

        //    if (RotateSpine.SpineRotationControlsInNonCombatState.ControlSpineBone == true && RotateSpine.SpineRotationControlsInNonCombatState.CoreAiBehaviourScript.CombatStarted == false)
        //    {
        //        if (IsRunningAndFollowingCommander == true)
        //        {
        //            RotateSpine.ChangeLookAtPoint(RotateSpine.SpineRotationControlsInNonCombatState.SpineBoneFocusPointDuringRunning);
        //        }
        //        else if (IsPatrollingOrWandering == true)
        //        {
        //            RotateSpine.ChangeLookAtPoint(RotateSpine.SpineRotationControlsInNonCombatState.SpineBoneFocusPointDuringWalking);
        //        }
        //        else if (IsScanning == true)
        //        {
        //            RotateSpine.ChangeLookAtPoint(RotateSpine.SpineRotationControlsInNonCombatState.SpineBoneFocusPointDuringScanning);
        //        }
        //        else if (IsSprinting == true)
        //        {
        //            RotateSpine.ChangeLookAtPoint(RotateSpine.SpineRotationControlsInNonCombatState.SpineBoneFocusPointDuringSprinting);
        //        }

        //    }

        //    anim.SetLookAtWeight(1f, RotateSpine.SpineWeightInAnimatorIk, HeadWeight, EyesWeight, 0.5f);
        //    Debug.Log("SpineWeight" + RotateSpine.SpineWeightInAnimatorIk + "HeadWeight" + HeadWeight + "EyesWeight" + EyesWeight);

        //}
        //IEnumerator SearchScan()
        //{
        //    //ScanScript.LookAtNewSpawnPoint(RandomPoint);
        //    float RandomiseTimer = Random.Range(HeadIkScript.Min, ScanScript.MaximumTimeBetweenPointOfChecks);
        //    yield return new WaitForSeconds(RandomiseTimer);
        //    if (CombatStarted == false && SearchingForSound == false && FOV.ConnectionLost == false && IsShotMade == false && HeadIkScript != null)
        //    {
        //        StartCoroutine(SearchScan());
        //    }
        //}


        //IEnumerator RotateHeadForSearching()
        //{
        //    if (HeadIkScript.LookDown == false && HeadIkScript.LookUp == false)
        //    {

        //        HorizontalValues(HeadIkScript.MinHorizontalSearching, HeadIkScript.MaxHorizontalSearching);
        //        float Randomise = Random.Range(HeadIkScript.MinTimeHorizontalLook, HeadIkScript.MaxTimeHorizontalLook);
        //        yield return new WaitForSeconds(Randomise);
        //        GenerateADetectPointForHead = false;
        //        // ChangePosOfRandomPointWhileSearch = false;
        //    }
        //    else if (HeadIkScript.LookDown == true && HeadIkScript.LookUp == true)
        //    {
        //        if (CanCreateaRandomPoint == false)
        //        {
        //            int RandomValue = Random.Range(0, 2);
        //            if (RandomValue == 0)
        //            {
        //                if (LookWhileSearching == false)
        //                {
        //                    CanCreateaRandomPoint = true;
        //                    LookUpValues(HeadIkScript.MinLookUp, HeadIkScript.MaxLookUp);
        //                    float Randomise = Random.Range(HeadIkScript.MinTimeLookUp, HeadIkScript.MaxTimeLookUp);
        //                    yield return new WaitForSeconds(Randomise);
        //                    LookWhileSearching = true;
        //                    CanCreateaRandomPoint = false;
        //                    GenerateADetectPointForHead = false;

        //                }
        //                else
        //                {
        //                    CanCreateaRandomPoint = true;
        //                    HorizontalValues(HeadIkScript.MinHorizontalSearching, HeadIkScript.MaxHorizontalSearching);
        //                    float Randomise = Random.Range(HeadIkScript.MinTimeHorizontalLook, HeadIkScript.MaxTimeHorizontalLook);
        //                    yield return new WaitForSeconds(Randomise);
        //                    LookWhileSearching = false;
        //                    CanCreateaRandomPoint = false;
        //                    GenerateADetectPointForHead = false;

        //                }
        //            }
        //            else
        //            {
        //                if (LookWhileSearching == false)
        //                {
        //                    CanCreateaRandomPoint = true;
        //                    LookDownValues(HeadIkScript.MinLookDown, HeadIkScript.MaxLookDown);
        //                    float Randomise = Random.Range(HeadIkScript.MinTimeLookDown, HeadIkScript.MaxTimeToLookDown);
        //                    yield return new WaitForSeconds(Randomise);
        //                    LookWhileSearching = true;
        //                    CanCreateaRandomPoint = false;
        //                    GenerateADetectPointForHead = false;

        //                }
        //                else
        //                {
        //                    CanCreateaRandomPoint = true;
        //                    HorizontalValues(HeadIkScript.MinHorizontalSearching, HeadIkScript.MaxHorizontalSearching);
        //                    float Randomise = Random.Range(HeadIkScript.MinTimeHorizontalLook, HeadIkScript.MaxTimeHorizontalLook);
        //                    yield return new WaitForSeconds(Randomise);
        //                    LookWhileSearching = false;
        //                    LookDownWhileSearching = false;
        //                    CanCreateaRandomPoint = false;
        //                    GenerateADetectPointForHead = false;

        //                }
        //            }


        //        }



        //    }
        //    else if (HeadIkScript.LookDown == true && HeadIkScript.LookUp == false)
        //    {
        //        if (CanCreateaRandomPoint == false)
        //        {
        //            if (LookWhileSearching == false)
        //            {
        //                CanCreateaRandomPoint = true;
        //                LookDownValues(HeadIkScript.MinLookDown, HeadIkScript.MaxLookDown);
        //                float Randomise = Random.Range(HeadIkScript.MinTimeLookDown, HeadIkScript.MaxTimeToLookDown);
        //                yield return new WaitForSeconds(Randomise);
        //                LookWhileSearching = true;
        //                CanCreateaRandomPoint = false;
        //                GenerateADetectPointForHead = false;

        //            }
        //            else
        //            {
        //                CanCreateaRandomPoint = true;
        //                HorizontalValues(HeadIkScript.MinHorizontalSearching, HeadIkScript.MaxHorizontalSearching);
        //                float Randomise = Random.Range(HeadIkScript.MinTimeHorizontalLook, HeadIkScript.MaxTimeHorizontalLook);
        //                yield return new WaitForSeconds(Randomise);
        //                LookWhileSearching = false;
        //                LookDownWhileSearching = false;
        //                CanCreateaRandomPoint = false;
        //                GenerateADetectPointForHead = false;

        //            }


        //        }

        //    }
        //    else
        //    {

        //        if (CanCreateaRandomPoint == false)
        //        {
        //            if (LookWhileSearching == false)
        //            {
        //                CanCreateaRandomPoint = true;
        //                LookUpValues(HeadIkScript.MinLookUp, HeadIkScript.MaxLookUp);
        //                float Randomise = Random.Range(HeadIkScript.MinTimeLookUp, HeadIkScript.MaxTimeLookUp);
        //                yield return new WaitForSeconds(Randomise);
        //                LookWhileSearching = true;
        //                CanCreateaRandomPoint = false;
        //                GenerateADetectPointForHead = false;

        //            }
        //            else
        //            {
        //                CanCreateaRandomPoint = true;
        //                HorizontalValues(HeadIkScript.MinHorizontalSearching, HeadIkScript.MaxHorizontalSearching);
        //                float Randomise = Random.Range(HeadIkScript.MinTimeHorizontalLook, HeadIkScript.MaxTimeHorizontalLook);
        //                yield return new WaitForSeconds(Randomise);
        //                LookWhileSearching = false;
        //                CanCreateaRandomPoint = false;
        //                GenerateADetectPointForHead = false;

        //            }

        //        }


        //    }




        //    //else
        //    //{
        //    //    if (LookDownWhileSearching == false)
        //    //    {
        //    //        if (LookWhileSearching == false)
        //    //        {
        //    //            float Randomise = Random.Range(HeadIkScript.MinimumTimeToLookUpWhileSearching, HeadIkScript.MaximumTimeToLookUpWhileSearching);
        //    //            yield return new WaitForSeconds(Randomise);
        //    //            LookUpValues(HeadIkScript.MinLookUp, HeadIkScript.MaxLookUp);
        //    //            LookWhileSearching = true;
        //    //        }
        //    //        else
        //    //        {
        //    //            yield return new WaitForSeconds(HeadIkScript.TimeToCreatePointInWorldSpace);
        //    //            HorizontalValues(HeadIkScript.MinHorizontalSearching, HeadIkScript.MaxHorizontalSearching);
        //    //            LookWhileSearching = false;
        //    //            LookDownWhileSearching = true;
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        if (LookWhileSearching == false)
        //    //        {
        //    //            float Randomise = Random.Range(HeadIkScript.MinimumTimeToLookDownWhileSearching, HeadIkScript.MaximumTimeToLookDownWhileSearching);
        //    //            yield return new WaitForSeconds(Randomise);
        //    //            LookDownValues(HeadIkScript.MinLookDown, HeadIkScript.MaxLookDown);
        //    //            LookWhileSearching = true;
        //    //        }
        //    //        else
        //    //        {
        //    //            yield return new WaitForSeconds(HeadIkScript.TimeToCreatePointInWorldSpace);
        //    //            HorizontalValues(HeadIkScript.MinHorizontalSearching, HeadIkScript.MaxHorizontalSearching);
        //    //            LookWhileSearching = false;
        //    //            LookDownWhileSearching = false;
        //    //        }
        //    //    }
        //    //    GenerateADetectPointForHead = false;
        //    //}

        //}
        public void CheckFieldofView() // If Field of View Enabled check for it
        {
            if (HealthScript.IsDied == false)
            {
                RemoveEnemiesFromList = false;
                if (OneTimeMessage == false)
                {
                    gameObject.SendMessage("CheckAiEnemyList", RemoveEnemiesFromList, SendMessageOptions.DontRequireReceiver);
                    OneTimeMessage = true;
                }
                //if (Detections.EnableFieldOfView == true)
                //{
                //    Vector3 dir = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - this.transform.position;
                //    Angle = Vector3.Angle(dir, this.transform.forward);

                //    if (DebugInfo.DebugRaycastToTarget == true)
                //    {
                //        Debug.DrawRay(FOV.VisualFieldOfView.transform.position, dir, DebugInfo.DebugRaycastColor);
                //    }

                //    if (EnemyTransform != FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform)
                //    {
                //        //IsEnemyLocked = false;
                //    }
                //    else
                //    {
                //     //   IsEnemyLocked = true;
                //    }
                //}

                if (FindEnemiesScript.EnableFieldOfView == true)
                {
                    if (FindEnemiesScript.NewEnemyLocked == true)
                    {
                        IsEnemyLocked = true;
                    }
                }
            }

        }
        public void CheckForLockedTargetInFOV() // Locking Targets
        {
            //if (Gunscript.instance != null)
            //{
            //    Gunscript.instance.IsFire = false;
            //}

            ResetToNormalState = false;
            ApplyRootMotion(false);
            VisibilityCheck.MyTarget = true;
            CombatStarted = true;
//            Debug.Log("Searching For Sound 10" + transform.name);
            SearchingForSound = false;
            //if (Detections.EnableFieldOfView == true && Angle <= OriginalFov )//|| IsEnemyLocked == true)
            //{
            //    EnemyTransform = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform;
            //   // IsEnemyLocked = true;
            //}
        }
        public void SearchingState()
        {
            //CombatStarted = false;
            if (SearchingForSound == false && IsLeaderMoving == false && IsAgentRoleLeader == false) // If you want the AI agent to scan than area when he is the leader 
            {                                                                                        // and waiting for followers than change the default behaviour to scanning and comment this line && IsAgentRoleLeader == false
                if (Components.HumanoidFiringBehaviourComponent != null)
                {
                    Components.HumanoidFiringBehaviourComponent.FireNow = false;
                }
                //Idle(); commented on 07/01/2024
                StartSearching();
            }
        }

        //public void RevertScanPointsParent()
        //{
        //    if (Shouldrevertscanpoint == true)
        //    {
        //        ScanScript.ScanPointsParentObject.transform.parent = transform;
        //        ScanScript.ScanPointsParentObject.transform.localPosition = Vector3.zero;
        //        ScanScript.ScanPointsParentObject.transform.localEulerAngles = Vector3.zero;
        //        Shouldrevertscanpoint = false;
        //    }

        //}
        //public void ReinitializeScanPoint()
        //{
        //    if (Shouldrevertscanpoint == false)
        //    {
        //        ScanScript.ScanPointsParentObject.transform.parent = null;
        //        Shouldrevertscanpoint = true;
        //    }
        //}
    }
}