using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace MobileActionKit
{
    public class HumanoidGrenadeThrower : MonoBehaviour
    {

        [TextArea]
        [Tooltip("This script lets humanAI agent to throw grenades at enemies.")]
        public string ScriptInfo = "This script lets humanAI agent to throw grenades at enemies.";

        [Tooltip("If checked than spawned Ai agent will or will not have grenades to throw for his entire life span")]
        public bool RandomiseGrenadesEquipment = false;
        //[Tooltip("This field is for the game developers to be able to to know which of the spawned Ai agents have grenades and which do not have them equiped." +
        //    "This field will display any of the two hard coded phrases that will indicate whether selected bot can or can not throw grenades.")]
        //public string DebugAiType = "I can throw grenades";

        [Tooltip("If set to 100 AI agent will always throw grenades at enemies if within grenade throwing range")]
        [Range(0, 100)]
        public int ThrowingGrenadesProbability = 5;

        [Tooltip("Drag and drop animator that includes a grenade throwing animation clip.")]
        public Animator AiAnimatorComponent;

        public float GrenadeThrowAnimationSpeed = 1f;

        public enum spawntype
        {
            SpawnUsingObjectPooler,
            SpawnUsingInstantiate
        }

        [Tooltip("Lets you choose which of the two available grenade spawning methods will be used." +
            "First option is 'Spawn using object pooler' for optimizing processor performance by allocating couple of megabites ram for storage of predefined ammount of grenades to be pooled from the memory." +
            "Second option is - 'Spawn Using Instantiation' which does not require extra ram allocation and relies entirelly on the cpu and contributes to garbage collection process." +
            "Choose one of the two options depending on the target device or platform." +
            "First option to save processor performance at the expense of extra ram consumption, or second to save the ram at the expense of processor performance. " +
            "Usually first option is more preferable for mobile devices," +
            "In case of consoles or desktop it will not make any noticeable difference." +
            " if spawn using object pooler option is selected than user have to specify overall amount of grenades inside object pooler script.")]
        public spawntype SpawnType;


        [Tooltip("Specify the time when Grenade collision raycasting will be enabled when the grenade is thrown by the Ai agent.")]
        public float GrenadeCollisionRaycastDelay = 0.2f;

        [Tooltip("Display raycast from origin point in 6 directions - Forward,backward,right,left,Up and down to detect colliders.")]
        public bool ShowGrenadeCollisionRaycast = false;
        [Tooltip("Distance to create raycast from origin point in 6 directions - Forward,backward,right,left,Up and down to detect colliders.")]
        public float GrenadeCollisionCheckRaycastDistance = 0.3f;
        [Tooltip("The Distance when the thrown grenade will activate rigidbody on it.")]
        private float PrelandingRigidbodyActivationDistance = 1f;

        [Tooltip("This field helps AI agent to know if enemy is a viable target for hand grenade. " +
            "To prevent agent from spending his grenades trying to destroy aerial targets that are above specified altitude.")]
        public float EnemyGroundProximityCheck = 1f;
        [Tooltip("Min allowed distance towards the enemy for throwing grenade. As soon as enemies are within specified distance Ai agent will be able to throw the grenades.")]
        public float MinGrenadeThrowingRange = 10f;
        [Tooltip("Max allowed distance towards the enemy for throwing grenade. As soon as enemies are within specified distance Ai agent will be able to throw the grenades.")]
        public float MaxGrenadeThrowingRange = 30f;
        [Tooltip("Amount of grenades carried by AI agent.")]
        public int NumberOfGrenades = 2;
        int CountGrenades = 0;

        [Tooltip("This field becomes relevant if  'Spawn using object pooler' option is selected ." +
            "Name in the field should match the name of grenade prefab in object pooler for Ai agent to be able to spawn grenades for the grenade attacks. " +
            "If 'Spawn Using Instantiation' option is selected than Grenade Prefab field which is located below becomes relevant for grenade throwing ability.")]
        public string GrenadeName;

        [Tooltip("Drag and drop 'Find enemies' component from the top of the inspector into this field.")]
        private FindEnemies FindEnemiesScript;
        [Tooltip("Defines trajectory of thrown grenade.")]
        public float ThrowAngle = 45.0f;
        //   public float GrenadeThrowDelay = 2.5f;

        [HideInInspector]
        public float Gravity = 9.8f;
        [Tooltip("Delays grenade spawn to match animation length and spawn it at the right moment of grenade throw animation.")]
        public float GrenadeSpawnDelay = 0.8f;

        [Tooltip("Affect how close or far away grenade will land from the target." +
            "The bigger the value is the bigger the distance from the target to the place where grenade lands.")]
        [Range(0f, 100f)]
        public float ThrowAccuracyError = 35f;

        [Tooltip("Drag and drop grenade prefab from the project folder into this field." +
            "Different Ai agents types can have their own unique grenade.")]
        public GameObject GrenadePrefab;
        [Tooltip("Drag and drop skeletal bone or it`s child object for grenade origin point at spawn.")]
        public Transform GrenadeSpawnPoint;

        [Tooltip("Drag and drop temporary parent gameObject of AI main weapon named 'Weapon Sling' from the hierarchy into this field. Ai agent's weapon will snap to position and rotation of that temporary weapon parent gameObject " +
            "for the duration of grenade throw.")]
        public Transform WeaponParentDuringGrenadeThrow;
        [Tooltip("Drag and drop weapons which is the child of the pelvis bone from the hierarchy into this field.")]
        public Transform AiWeapon;

        public float DelayWeaponDefaultParenting = 0.09f;

        [Tooltip("To set min time intervals for Ai agent to check if conditions are suitable for grenade throw. ")]
        public float MinCheckTimeForThrow = 7f;
        [Tooltip("To set max time intervals for Ai agent to check if conditions are suitable for grenade throw. ")]
        public float MaxCheckTimeForThrow = 9f;


        private float GrenadeReachPosition;
        Vector3 RandomDir;

        [Tooltip("This field is responsible for applying correct delay before grenade spawn when Ai agent is performing grenade throw. " +
            "It is referencing time length of grenade throw animation " +
            "clip being played inside animator controller. This functionality is achieved by putting the same animation clip that is played inside animator controller into this field .")]
        public AnimationClip GrenadeThrowAnimationClip;
        float ClipLength;

        [Tooltip("Copy the name of animation state from GRENADE layer inside animator controller and paste it into this field." +
            "Grenade thrower script is referencing the name of animation state inside GRENADE layer of animator controller" +
            " to playback whatever animation clip is within that state." +
            "Make sure that the text in this field is exactly the same as name of the animation state this field is referencing.")]
        public string GrenadeAnimationStateName = "Grenade Throw";

        //    private MasterAiBehaviour MAB;
        private SpineRotation SR;
        private CoreAiBehaviour CoreAiBehaviourScript;
        HumanoidAiHealth HumandoidHealth;

        private ObjectPooler pooler;
        GameObject Grenade;

        bool GetDir = false;
        bool CanThrowGrenadeNow = false;

        [HideInInspector]
        public bool ThrowingCapability = true;

        Transform PreviouslyHoldedHand;

        Vector3 PreviousPositionOfAiWeapon;
        Vector3 PreviousEulerAnglesOfAiWeapon;

        [Tooltip("Enable this checkbox only if you want the Ai agent to do raycast checks before throwing grenade towards enemy. If your level is simple where you want the Ai agent to throw grenade in every situation" +
            " you can disable this checkbox i.e fighting on 2 sides of the roof. Whereas if your level is complicated i.e construction site where lots of geometry is involved like stairs.You can enable this checkbox which" +
            " will than make sure that Ai agent to proper raycast checks towards enemy before grenade throw.")]
        public bool DoRaycastChecksBeforeGrenadeThrow = true;

        [Tooltip("Drag and drop 'Roof Check Raycaster' gameObject attached as a child of this Ai agent.")]
        public Transform RoofCheckRaycaster;
        [Tooltip("Drag and drop 'Enemy Raycaster' gameObject attached as a child of this Ai agent.")]
        public Transform DirectEnemyRaycaster;
        [Tooltip("Drag and drop 'Horizontal Raycaster' gameObject attached as a child of this Ai agent.")]
        public Transform HorizontalRaycaster;
        [Tooltip("Specify what would be the highest point the raycast will be drawn to detect obstacle before deciding whether to throw the grenade or not." +
            " while this is not the only condition for Ai agent to check for before deciding whether to throw grenade or not. Suppose if this condition is true meaning there is a roof above Ai agent but" +
            " the enemy is clearly visible to Ai agent according to 'Enemy Raycaster' than Ai agent will throw the grenade.i.e one of the condition should be true before deciding whether to throw the grenade or not.")]
        public float UpwardsRaycastDistance = 10f;

        bool IsUpwardsRaycastingGood = false;
        bool IsDirectRaycastingGood = false;
        bool IsHorizontalRaycastingGood = false;
        bool IsFlightFinished = false;

        bool IsGrenadeRaycastBegins = false;

        [HideInInspector]
        public bool IsWithinNoThrowArea = false;

        void Start()
        {
            //if (GetComponent<MasterAiBehaviour>() != null)
            //{
            //    MAB = GetComponent<MasterAiBehaviour>();
            //    HumandoidHealth = GetComponent<HumanoidAiHealth>();
            //}
            if (GetComponent<CoreAiBehaviour>() != null)
            {
                CoreAiBehaviourScript = GetComponent<CoreAiBehaviour>();
                HumandoidHealth = GetComponent<HumanoidAiHealth>();
            }

            if (GetComponent<SpineRotation>() != null)
            {
                SR = GetComponent<SpineRotation>();
            }

            if (GetComponent<FindEnemies>() != null)
            {
                FindEnemiesScript = GetComponent<FindEnemies>();
            }
            //if(RandomiseGrenadesEquipment == true)
            //{
            //    int Randomise = Random.Range(0, 2);

            //    if(Randomise != 1)
            //    {
            //        DebugAiType = "I can throw grenades";
            //        ThrowingCapability = true;
            //    }
            //    else
            //    {
            //        DebugAiType = "I can not throw grenades";
            //        ThrowingCapability = false;
            //    }
            //}

            if (CoreAiBehaviourScript.CombatStateBehaviours.UseGrenades == true)
            {
                if (ThrowingCapability == true)
                {
                    ClipLength = GrenadeThrowAnimationClip.length;
                    if (ObjectPooler.instance != null)
                    {
                        pooler = ObjectPooler.instance;
                    }
                }
            }

            PreviouslyHoldedHand = AiWeapon.transform.parent;
            PreviousPositionOfAiWeapon = AiWeapon.transform.localPosition;
            PreviousEulerAnglesOfAiWeapon = AiWeapon.transform.localEulerAngles;


        }
        IEnumerator ThrowGrenadeTimer()
        {
            float ThrowingDelay = Random.Range(MinCheckTimeForThrow, MaxCheckTimeForThrow);
            yield return new WaitForSeconds(ThrowingDelay);
            ThrowGrenade();
        }
        public void ThrowGrenade()
        {
            if (CoreAiBehaviourScript.Components.HumanoidFiringBehaviourComponent.isreloading == false)
            {
                int Randomise = Random.Range(0, 100);

                if (Randomise <= ThrowingGrenadesProbability)
                {
                    CanThrowGrenadeNow = true;
                }
                else
                {
                    CanThrowGrenadeNow = false;
                }

                //if (MAB != null)
                //{
                //    if (MAB.CombatStarted == true)
                //    {
                //        if (HumandoidHealth != null)
                //        {
                //            if (HumandoidHealth.Health > 0)
                //            {
                //                if (CanThrowGrenadeNow == true)
                //                {
                //                    Throw();
                //                }
                //            }
                //        }

                //    }
                //}
                if (CoreAiBehaviourScript != null)
                {
                    if (CoreAiBehaviourScript.CombatStarted == true)
                    {
                        if (HumandoidHealth != null)
                        {
                            if (HumandoidHealth.Health > 0)
                            {
                                if (CanThrowGrenadeNow == true)
                                {
                                    Throw();
                                }
                            }
                        }

                    }
                }
            }
            else
            {
                if (CountGrenades < NumberOfGrenades)
                {
                    StartingTheThrowDelay();
                }
            }
        }
        IEnumerator SimulateProjectile()
        {
            if (FindEnemiesScript.FindedEnemies == true)
            {
                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                {
                    if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<Targets>() != null)
                    {
                        if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.GetComponent<Targets>().MyBodyPartToTarget.transform.localPosition.y < EnemyGroundProximityCheck)
                        {
                            IsFlightFinished = false;
                            StartCoroutine(ThrowAnimationControl());
                            //   CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(4, 1f);
                            ++CountGrenades;
                            if (SR != null)
                            {
                                SR.CanMoveSpineInCombatState = false;
                            }
                            if (CoreAiBehaviourScript.Components.HumanoidFiringBehaviourComponent.FireNow == true || CoreAiBehaviourScript.SprintingActivated == false)
                            {
                                AiWeapon.transform.parent = WeaponParentDuringGrenadeThrow.transform;
                            }
                            CoreAiBehaviourScript.ThrowingGrenade = true;
                           
                            AiAnimatorComponent.SetBool("Throw", true);
                            CoreAiBehaviourScript.Components.HumanoidAiAudioPlayerComponent.PlayNonRecurringSoundClips(CoreAiBehaviourScript.Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceThrowingGrenadeAudioClips);
                            CoreAiBehaviourScript.Components.HumanoidFiringBehaviourComponent.ShootIntruppt = true;
                            AiAnimatorComponent.SetFloat("GrenadeThrowAnimationSpeed", GrenadeThrowAnimationSpeed);
                            // CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(5, 1f);
                            yield return new WaitForSeconds(GrenadeSpawnDelay);
                            if (spawntype.SpawnUsingInstantiate == SpawnType)
                            {
                                Grenade = Instantiate(GrenadePrefab, GrenadeSpawnPoint.position, GrenadeSpawnPoint.rotation);
                            }
                            else
                            {
                                if (pooler != null)
                                {
                                    Grenade = pooler.SpawnFromPool(GrenadeName, GrenadeSpawnPoint.position, GrenadeSpawnPoint.rotation);
                                }
                            }

                            Grenade.transform.position = GrenadeSpawnPoint.position + new Vector3(0, 0.0f, 0);

                            Vector3 distance = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - transform.position;
                            distance.y = 0;
                            float Getdistance = Mathf.FloorToInt(distance.magnitude);
                            float approachtoPos = Getdistance / 100f;
                            float FinalApproach = approachtoPos * ThrowAccuracyError;
                            GrenadeReachPosition = FinalApproach;

                            if (GetDir == false)
                            {
                                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null)
                                {
                                    RandomDir = GenerateRandomNavmeshLocation.RandomLocation(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy], GrenadeReachPosition);
                                    GetDir = true;
                                }

                            }
                            float target_Distance = Vector3.Distance(Grenade.transform.position, RandomDir);
                            float projectile_Velocity = target_Distance / (Mathf.Sin(2 * ThrowAngle * Mathf.Deg2Rad) / Gravity);
                            float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(ThrowAngle * Mathf.Deg2Rad);
                            float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(ThrowAngle * Mathf.Deg2Rad);
                            float flightDuration = target_Distance / Vx;
                            Grenade.transform.rotation = Quaternion.LookRotation(RandomDir - Grenade.transform.position);

                            float elapse_time = 0;

                            StartCoroutine(DelayGrenadeCollisionRaycasting());
                            while (elapse_time < flightDuration)
                            {
                                

                                Grenade.transform.Translate(0, (Vy - (Gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
                                elapse_time += Time.deltaTime;

                                //RaycastHit hit;
                                //Debug.DrawLine(Grenade.transform.position, Grenade.transform.forward, Color.red, 0.1f);
                                //if (Physics.Raycast(Grenade.transform.position, Grenade.transform.forward, out hit, 0.5f))
                                //{
                                //    if (Grenade.GetComponent<GrenadeExplosion>() != null)
                                //    {
                                //        if (spawntype.SpawnUsingInstantiate == SpawnType)
                                //        {
                                //            Grenade.GetComponent<GrenadeExplosion>().CanDestroy = true;
                                //        }
                                //        else
                                //        {
                                //            if (pooler != null)
                                //            {
                                //                Grenade.GetComponent<GrenadeExplosion>().CanDestroy = false;
                                //            }
                                //        }
                                //        Grenade.GetComponent<GrenadeExplosion>().CanStartExplosion = true;
                                //        Grenade.GetComponent<GrenadeExplosion>().GrenadeExplosionStarts();
                                //    }
                                //    Grenade.transform.root.GetComponent<GrenadePhysics>().PhysicsAdder();
                                //    IsFlightFinished = true;
                                //    break;
                                //}

                                if(IsGrenadeRaycastBegins == true)
                                {
                                    CheckRaycast(Vector3.forward);
                                    CheckRaycast(Vector3.back);
                                    CheckRaycast(Vector3.left);
                                    CheckRaycast(Vector3.right);
                                    CheckRaycast(Vector3.up);
                                    CheckRaycast(Vector3.down);
                                }
                               

                                if (IsFlightFinished == true)
                                {
                                    IsGrenadeRaycastBegins = false;
                                    break;
                                }

                                yield return null;

                            }

                            if (IsFlightFinished == false)
                            {
                                Vector3 Dis = RandomDir - Grenade.transform.position;
                                if (Dis.magnitude < PrelandingRigidbodyActivationDistance)
                                {                                  
                                    if (Grenade.GetComponent<ExplosiveDevice>() != null)
                                    {
                                        if (spawntype.SpawnUsingInstantiate == SpawnType)
                                        {
                                            Grenade.GetComponent<ExplosiveDevice>().CanDestroy = true;
                                        }
                                        else
                                        {
                                            if (pooler != null)
                                            {
                                                Grenade.GetComponent<ExplosiveDevice>().CanDestroy = false;
                                            }
                                        }
                                        Grenade.GetComponent<ExplosiveDevice>().CanStartExplosion = true;
                                        Grenade.GetComponent<ExplosiveDevice>().GrenadeExplosionStarts();
                                    }
                                    Grenade.transform.root.GetComponent<ExplosiveCollision>().PhysicsAdder();
                                }
                                IsGrenadeRaycastBegins = false;
                            }


                            StartingTheThrowDelay();
                        }
                    }
                }
            }

        }
        void CheckRaycast(Vector3 direction)
        {
            RaycastHit hit;
            Vector3 raycastStart = Grenade.transform.position;
            Vector3 raycastEnd = raycastStart + direction * GrenadeCollisionCheckRaycastDistance;

            // Debug line to visualize the ray
            if (ShowGrenadeCollisionRaycast == true)
            {
                Debug.DrawLine(raycastStart, raycastEnd, Color.yellow, 0.1f);
            }
            if (Physics.Raycast(raycastStart, direction, out hit, GrenadeCollisionCheckRaycastDistance))
            {
                if(hit.transform != Grenade.transform) // to make sure it does not detect its own collider
                {
                    //Debug.Log(hit.transform.name);
                    if (Grenade.GetComponent<ExplosiveDevice>() != null)
                    {
                        if (spawntype.SpawnUsingInstantiate == SpawnType)
                        {
                            Grenade.GetComponent<ExplosiveDevice>().CanDestroy = true;
                        }
                        else
                        {
                            if (pooler != null)
                            {
                                Grenade.GetComponent<ExplosiveDevice>().CanDestroy = false;
                            }
                        }
                        Grenade.GetComponent<ExplosiveDevice>().CanStartExplosion = true;
                        Grenade.GetComponent<ExplosiveDevice>().GrenadeExplosionStarts();
                    }
                    Grenade.transform.root.GetComponent<ExplosiveCollision>().PhysicsAdder();
                    IsFlightFinished = true;
                }
                
            }
        }
        IEnumerator DelayGrenadeCollisionRaycasting()
        {
            yield return new WaitForSeconds(GrenadeCollisionRaycastDelay);
            IsGrenadeRaycastBegins = true;
        }
        IEnumerator ThrowAnimationControl()
        {
            float NewClipLength = ClipLength / GrenadeThrowAnimationSpeed;
            yield return new WaitForSeconds(NewClipLength);
            AiAnimatorComponent.SetBool("Throw", false);
            CoreAiBehaviourScript.ThrowingGrenade = false;
            // CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(5, 0f,true);
            // CoreAiBehaviourScript.AnimatorLayerWeightControllerScript.ChangeLayerWeight(4, 0f, true);
            yield return new WaitForSeconds(DelayWeaponDefaultParenting);
            AiWeapon.transform.parent = PreviouslyHoldedHand.transform;
            AiWeapon.transform.localPosition = PreviousPositionOfAiWeapon;
            AiWeapon.transform.localEulerAngles = PreviousEulerAnglesOfAiWeapon;

            // yield return new WaitForSeconds(0.01f);
            CoreAiBehaviourScript.Components.HumanoidFiringBehaviourComponent.ShootIntruppt = false;
            if (SR != null)
            {
                SR.CanMoveSpineInCombatState = true;
            }
        }
        public void Throw()
        {
            if (CountGrenades < NumberOfGrenades)
            {
                Vector3 CheckDistanceWithTarget = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - transform.position;
                if (CheckDistanceWithTarget.magnitude <= MaxGrenadeThrowingRange && CheckDistanceWithTarget.magnitude >= MinGrenadeThrowingRange && IsWithinNoThrowArea == false) 
                {
                    IsGrenadeRaycastBegins = false;
                    if (DoRaycastChecksBeforeGrenadeThrow == true)
                    {
                        IsUpwardsRaycastingGood = true;
                        IsDirectRaycastingGood = false;
                        IsHorizontalRaycastingGood = false;

                        //Vertical Raycasting
                        Ray ray = new Ray(RoofCheckRaycaster.transform.position, Vector3.up);
                        if (Physics.Raycast(ray, out RaycastHit hit, UpwardsRaycastDistance))
                        {
                            IsUpwardsRaycastingGood = false;
                        }
                        //Direct Enemy Raycasting
                        DirectEnemyRaycaster.LookAt(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position);
                        Ray EnemyRaycasterRay = new Ray(DirectEnemyRaycaster.transform.position, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - DirectEnemyRaycaster.transform.position);
                        RaycastHit hitResult;
                        //Debug.DrawRay(DirectEnemyRaycaster.transform.position, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - DirectEnemyRaycaster.transform.position, Color.red, 2.0f);
                        if (Physics.Raycast(EnemyRaycasterRay, out hitResult, MaxGrenadeThrowingRange))
                        {
                            if(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null && hitResult.transform.parent != null)
                            {
                                if (hitResult.transform.root == FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].root ||
                                   hitResult.transform.parent.GetComponent<AiCover>() != null)
                                {
                                    IsDirectRaycastingGood = true;
                                }
                            }
                       
                        }
                        //Horizontal Raycasting
                        var lookPos = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - HorizontalRaycaster.transform.position;
                        lookPos.y = 0;
                        var rotation = Quaternion.LookRotation(lookPos);
                        HorizontalRaycaster.transform.rotation = Quaternion.Slerp(HorizontalRaycaster.transform.rotation, rotation, Time.deltaTime * 5000f);

                        Ray HorizontalRaycasterRay = new Ray(HorizontalRaycaster.transform.position, HorizontalRaycaster.transform.forward);
                        RaycastHit[] hits = Physics.RaycastAll(HorizontalRaycasterRay, MaxGrenadeThrowingRange);
                        foreach (RaycastHit hitresult in hits)
                        {
                            if (hitresult.transform.root == FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].root)
                            {
                                IsHorizontalRaycastingGood = true;
                                break;
                            }
                        }


                        if (IsUpwardsRaycastingGood == true && IsDirectRaycastingGood == true || IsDirectRaycastingGood == true || IsHorizontalRaycastingGood == true)
                        {
                            GetDir = false;
                            StartCoroutine(SimulateProjectile());
                        }
                        else
                        {
                            StartingTheThrowDelay();
                        }
                    }
                    else
                    {
                        GetDir = false;
                        StartCoroutine(SimulateProjectile());
                    }
                   

                    // Old code :
                    //DirectEnemyRaycaster.LookAt(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
                    //Ray EnemyRaycasterRay = new Ray(DirectEnemyRaycaster.transform.position, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - DirectEnemyRaycaster.transform.position);
                    //RaycastHit[] Directhits = Physics.RaycastAll(EnemyRaycasterRay, MaxGrenadeThrowingRange);

                    //// Debug the ray direction
                    //Debug.DrawRay(DirectEnemyRaycaster.transform.position, FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - DirectEnemyRaycaster.transform.position, Color.red, 2.0f);

                    //foreach (RaycastHit hitresult in Directhits)
                    //{
                    //    Debug.Log(hitresult.transform.root.name);
                    //    if (hitresult.transform.root == FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].root)
                    //    {
                    //        IsDirectRaycastingGood = true;
                    //        break;
                    //    }
                    //    else
                    //    {
                    //        IsDirectRaycastingGood = false;
                    //        break;
                    //    }
                    //}

                }
                else
                {
                    StartingTheThrowDelay();
                }
            }
        }
        public void StartingTheThrowDelay()
        {

            AiWeapon.transform.parent = PreviouslyHoldedHand.transform;
            AiWeapon.transform.localPosition = PreviousPositionOfAiWeapon;
            AiWeapon.transform.localEulerAngles = PreviousEulerAnglesOfAiWeapon;
            StartCoroutine(ThrowGrenadeTimer());
        }
    }
}