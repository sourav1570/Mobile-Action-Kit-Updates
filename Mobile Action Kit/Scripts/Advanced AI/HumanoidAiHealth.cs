using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
// This Script is Responsible For Humanoid Health 

namespace MobileActionKit
{
    public class HumanoidAiHealth : MonoBehaviour
    {
        public static HumanoidAiHealth instance;

        [TextArea]
        public string ScriptInfo = "This script is used for the Ai agent Health and death . It is responsible for spawning pickup items after Ai dies . ";
        [Space(10)]

        [Tooltip("Health of the Ai agent which get decreased when the Ai agent takes damage.")]
        public float Health = 60f;
        [Tooltip("Multiplies the damage of the bullet or grenade with the value in this field.")]
        public float WeakPointDamageMultiplier = 5f;

        [Tooltip("Enable this checkbox to spawn the 'Healthbar UI' on top of the Ai agent.")]
        public bool EnableHealthBar = false;

        [Tooltip("If checked 'HealthBar' will not be visible when agent is in non combat.")]
        public bool KeepHealthBarDeactivatedInNonCombat = true;

        [Tooltip("Drag and drop 'Healthbar UI' prefab from the project into this field.")]
        public GameObject HealthBarPrefab;
        [Tooltip("Adjust the position of the spawned 'HealthBar UI' during the time Ai agent is standing.")]
        public Vector3 StandHealthBarUIOffset = new Vector3(0f, 1.8f, 0f);
        [Tooltip("Adjust the position of the spawned 'HealthBar UI' during the time Ai agent is crouching.")]
        public Vector3 CrouchHealthBarUIOffset = new Vector3(0f, 1.3f, 0f);
        Image HealthBarfill;

        [Tooltip("If enabled for Ai agent than it will use the ragdoll physics when it dies. If disabled Ai agent will use random animation clips.")]
        public bool EnableRagdoll = true;

        [Tooltip("Drag and drop the animator component attached with this Ai agent from the hierarchy into this field.")]
        public Animator AnimatorComponent;
        [Tooltip("You can put one or several animation state names into the fields below for the Ai to playback when it dies during stand shooting. " +
            "Names you put into the fields should precisely match the names of the animation states inside animator controller.")]
        public string[] StandDyingAnimationStateNames;
        [Tooltip("You can put one or several animation state names into the fields below for the Ai to playback when it dies during crouch shooting. " +
         "Names you put into the fields should precisely match the names of the animation states inside animator controller.")]
        public string[] CrouchDyingAnimationStateNames;

        [Tooltip("Drag and drop the collider attached with the 'HeadBone' which is the child of this Ai agent pelvis bone from the hierarchy into this field.")]
        public Collider HeadBoneCollider;
        [Tooltip("You can put one or several animation state names into the fields below for the Ai to playback when it dies during stand shooting. " +
        "Names you put into the fields should precisely match the names of the animation states inside animator controller.")]
        public string[] StandHeadShotDyingAnimationStateNames;
        [Tooltip("You can put one or several animation state names into the fields below for the Ai to playback when it dies during crouch shooting. " +
         "Names you put into the fields should precisely match the names of the animation states inside animator controller.")]
        public string[] CrouchHeadShotDyingAnimationStateNames;

        [Tooltip("Bullet impact effects that will be played when this Ai is hit.")]
        public GameObject[] HitEffects;
        [HideInInspector]
        public bool IsDied = false;

        //    MasterAiBehaviour SL;
        CoreAiBehaviour SAB;

        [Tooltip("If Enabled other components will be destroy and the mesh will be unparent")]
        public bool KeepDeadBody = true;
        [Tooltip("In case developers will want to keep dead body of killed Ai agents for sometime after agents die they should drag skinned mesh renderer from Ai hierarchy into this field. " +
            "When the Ai agent will die his deadbody will appear in the exact pose of the last frame of the ragdoll or dying animation." +
            "This DeadBodyMesh is also required to store the coordinates for the investigation after the emergency state has been expired.")]
        public SkinnedMeshRenderer DeadBodyMesh;

        [Tooltip("Add all the additional Items like googles,Mask to attach with the Deadbody Mesh during the time it spawn.")]
        public Transform[] DeadBodyMeshItemsToAttach;

        [Tooltip("Drag the Pelvis Bone of the Ai skinned mesh into this field." +
            "The pelvic bone of the skinned mesh that you imported does not have to be named - pelvis." +
            "In the skinned meshes provided for the kit pelvic bone has a name - hips.")]
        public Transform PelvisBoneTransform;

        [Tooltip("Will destroy all other components (scripts and skeleton) except Humanoid Ai Health script." +
            "This field is responsible for creating dead body mesh that will stay in the level for specified amount of time.")]
        public float TIME_TO_DESTROY_OTHER_COMPONENTS = 1f;

        [Tooltip("The time deadbody will stay visible in the game.")]
        public float DeadBodyExistenceTime = 20f;
        [Tooltip("Unparenting the resulted non skeletal body mesh is necessary to achieve this dead body effect." +
            "This delay value should be tweaked so that there would be enough time to inherit skeletal position of the last frame that skeleton existed. ")]
        public float TIME_TO_UNPARENT_BODYMESH = 4f;
        private Vector3 rootPos;
        private Vector3 rootRot;

        [Tooltip("If enabled than the Ai agent will spawn pickup items after death.")]
        public bool SpawnPickupItemsAfterDying = true;

        public Vector3 PickupItemOffsetFromPosition = new Vector3(0f, 0f, 0f);

        [Tooltip("Drag and drop one or more items from the project window into this field to spawn when the Ai agent deadbody existence time get expired.")]
        public GameObject[] PickupItems;

        [HideInInspector]
        public int StoreTeamId;
        [HideInInspector]
        public bool UpdateKills = false;
        [HideInInspector]
        public bool CompleteFirstHitAnimation = false;

        [HideInInspector]
        public HumanoidBodyHits aiImpactScript;

        [HideInInspector]
        public string collidername;

        [Tooltip("This radius is enabled every time Ai agent dies regardless of the reason of his death. " +
            "It will cause emergency alert state for any Ai agent that will be within dying sound radius.")]
        public GameObject DyingSoundRadius;

        [Tooltip("Drag and drop 'SoundInvestigatingApproachStyleReset' script located in the hierarchy of this Agent.")]
        public GameObject SoundInvestigatingApproachStyleReset;

        bool TaskCompleted = false;

        [HideInInspector]
        public bool SpawnHealthBar = false;
        [HideInInspector]
        public Transform SpawnedHealthBarUI;

        float MaximumHealth;
        bool PlayDeathAnimation = false;

        [HideInInspector]
        public bool AddForceToHumanoidAi = false;

        [HideInInspector]
        public bool AvoidThisFinalBulletFromDeadEnemy = false;

        [HideInInspector]
        public bool DoNotActivateSoundTrigger = false;

        [HideInInspector]
        public Transform WhoShootingMe;

        private float timer;
        private bool isReducingDamage = false;
        private float initialDamage;
        private float reducedDamage;

        public GameObject[] BurningEffectForIncendiaryGrenade;

        Color DefaultColorOfDeadBodyMeshMaterial;

        [HideInInspector]
        public Transform WhoKilledMe;// Basically this field is created so that the AI agent at the moment of his death can send notification to other agent/Player/any Ai entity to play the audio clip present in the OnceTargetKilled field
                                     // Located in Humanoid AI audio player script.So that only and only that Ai agent can play the audio clip who has made the final shot to kill this AI agent.we send notification in the Targets script
                                     // as this is the most common script exist in all AI entities and even the Player so that the Ai or the player responsible for killing the AI agent can playback the audio clips OnceTargetKilled.

        bool IsAiLastShotWasOnHead = false;

        [ContextMenu("Kill Agent")]
        private void UpdateHealthBarInEditor()
        {
            Takedamage(1000000f);
        }


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            //if (GetComponent<MasterAiBehaviour>() != null)
            //{
            //    SL = GetComponent<MasterAiBehaviour>();
            //}
            if (GetComponent<CoreAiBehaviour>() != null)
            {
                SAB = GetComponent<CoreAiBehaviour>();
            }

            if (GetComponent<HumanoidBodyHits>() != null)
            {
                aiImpactScript = GetComponent<HumanoidBodyHits>();
            }

            MaximumHealth = Health;

            if (SAB != null)
            {
                if (SAB.Components.HumanoidFiringBehaviourComponent != null)
                {
                    if(SAB.Components.HumanoidFiringBehaviourComponent.Components.WeaponMesh != null)
                    {
                        SAB.Components.HumanoidFiringBehaviourComponent.Components.WeaponMesh.GetComponent<Collider>().enabled = false;
                    }
                }
            }
            DefaultColorOfDeadBodyMeshMaterial = DeadBodyMesh.material.color;
        }

        public void Effects(RaycastHit hit)
        {
            if (HitEffects.Length > 0)
            {
                int Randomise = Random.Range(0, HitEffects.Length);
                GameObject impacteffect = Instantiate(HitEffects[Randomise].gameObject, hit.point, Quaternion.LookRotation(hit.normal));
                if (impacteffect.gameObject.GetComponent<ImpactEffect>() != null)
                {                   
                    impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.DoNotActivateEffect = DoNotActivateSoundTrigger;

                    // Keep the below lines to be commented because during emergency state based on the selected properties it should affect the other Humanoid AI agent and not only friendlies
                    // as when you overrite the properties it affects the behaviour when you change promatically that only friendlies will be affected using below code.
                    // This may create a bug when you shoot the enemies and impact effects
                    // are spawned with the settings being force emergency state on AI agent.so Make sure you don't use this code at all even in the case of sounds. for more details why not to use you can change
                    // impact effect properties to be Force Emergency state and choose All teams and shoot near the enemies using player weapon by uncommenting this code.

                    //impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.Enemy = WhoShootingMe;
                    //if(SAB != null)
                    //{
                    //    impacteffect.gameObject.GetComponent<ImpactEffect>().TeamWhichWillBeAffectedByTheShot(SAB.T.MyTeamID);
                    //}
                    impacteffect.gameObject.GetComponent<ImpactEffect>().EffectActivation(WhoShootingMe);
                }
            }

            if (TeamMatch.instance != null)
            {
                if (TeamMatch.instance.EnableScoreSystemBetweenTeamsAsWinCondition == true)
                {
                    if (IsDied == true)
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
        public void BloodEffectsFromZombieAttack(Vector3 Position , Quaternion Rot)
        {
            if (HitEffects.Length > 0)
            {
                int Randomise = Random.Range(0, HitEffects.Length);
                GameObject impacteffect = Instantiate(HitEffects[Randomise].gameObject, Position, Rot);
                if (impacteffect.gameObject.GetComponent<ImpactEffect>() != null)
                {
                    impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.DoNotActivateEffect = DoNotActivateSoundTrigger;
                    impacteffect.gameObject.GetComponent<ImpactEffect>().EffectActivation(WhoShootingMe);
                }
            }
        }
        public void UpdateHealthBar(float currentHealth, float maxHealth)
        {
          
            float fillAmount = currentHealth / maxHealth;
            if (HealthBarfill != null)
            {
                HealthBarfill.fillAmount = fillAmount;
            }
        }
        public void Takedamage(float amount) // Ai Damage Function
        {
            if (SpawnHealthBar == false && EnableHealthBar == true)
            {
                SpawnedHealthBarUI = Instantiate(HealthBarPrefab, transform.position + StandHealthBarUIOffset, Quaternion.identity).transform;
                GameObject canvasfound = GameObject.FindGameObjectWithTag("Canvas3D");

                if(canvasfound != null)
                {
                    SpawnedHealthBarUI.transform.SetParent(canvasfound.transform, false);

                    foreach (Image childImage in SpawnedHealthBarUI.GetComponentsInChildren<Image>())
                    {
                        if (childImage.type == Image.Type.Filled)
                        {
                            HealthBarfill = childImage;
                            break;
                        }
                    }
                }
                SpawnHealthBar = true;
            }

            Health -= amount;

           
            if (EnableHealthBar == true && SpawnHealthBar == true)
            {
                UpdateHealthBar(Health, MaximumHealth);
            }

            if (Health <= 0f && IsDied == false)
            {
                if (SAB != null)
                {
                    SAB.Components.HumanoidAiAudioPlayerComponent.PlayDeathSound();  
                }

                //  for(int y = 0;y < DeadBodyZoneRadius.Length; y++)
                //{

                if (DyingSoundRadius != null)
                {
                    DyingSoundRadius.SetActive(true);
                }


                if (SoundInvestigatingApproachStyleReset != null)
                {
                    SoundInvestigatingApproachStyleReset.SetActive(true);
                }

                //}

                //if (!theEnemy.name.Contains("Player"))
                // {
                //if (SL != null)
                //{            
                //    SL.AiCurrentState = "DEAD";
                //    SL.IsDead = true;
                //    SL.NavMeshAgentComponent.isStopped = true;
                //    SL.DefaultStateAnimation();
                //}
                if(GetComponent<Targets>() != null)// this code is important so that newly spawned Ai agents do not add this Ai agent in there enemy or in there friendly list at all.
                {
                    Targets t = GetComponent<Targets>();
                    t.enabled = false;
                }
                if(WhoKilledMe != null)
                {
                    if(WhoKilledMe.gameObject.GetComponent<Targets>() != null)
                    {
                        WhoKilledMe.gameObject.GetComponent<Targets>().PlayEnemyEliminatedClips = true;
                    }
                }
                IsDied = true;
                Die();

                if (EnableHealthBar == true)
                {
                    SpawnedHealthBarUI.gameObject.SetActive(false);
                }

                if (WinningProperties.instance != null)
                {
                    if(IsAiLastShotWasOnHead == false)
                    {
                        WinningProperties.instance.ShowTotalBodyKills++;
                        WinningProperties.instance.TotalBodyKillsBonusRecieved = WinningProperties.instance.TotalBodyKillsBonusRecieved + WinningProperties.instance.BonusPerBodyKill;
                    }
                  
                }
                //}                   
            }
            else
            {
                SAB.Components.HumanoidAiAudioPlayerComponent.PlayNonRecurringSoundClips(SAB.Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceWoundedAudioClips);

                if (aiImpactScript != null)
                {
                    if (aiImpactScript.enabled == true)
                    {
                        aiImpactScript.PlayHitImpactAnimations();
                    }
                }
            }
        }

        public void StartDamageReduction(float duration, float damage)
        {
            for(int x = 0; x < BurningEffectForIncendiaryGrenade.Length; x++)
            {
                BurningEffectForIncendiaryGrenade[x].SetActive(true);
            }
            initialDamage = damage;
            timer = duration;
            isReducingDamage = true;
             StartCoroutine(DamageReductionTimer());
            
        }

        //private IEnumerator DamageReductionTimer()
        //{
        //    float elapsedTime = 0f;
        //    while (elapsedTime < timer)
        //    {
        //        elapsedTime += Time.deltaTime;
        //        reducedDamage = Mathf.Lerp(initialDamage, 0f, elapsedTime / timer);
        //        float MyHealth = Health - reducedDamage;
        //        Takedamage(MyHealth);
        //        yield return null;
        //    }

        //}
        //private IEnumerator DamageReductionTimer()
        //{
        //    initialDamage = Health - initialDamage;
        //    float elapsedTime = 0f;
        //    while (elapsedTime < timer)
        //    {
        //        elapsedTime += Time.deltaTime;
        //        reducedDamage = Mathf.Lerp(Health, initialDamage, elapsedTime / timer);
        //        float MyHealth = Health - reducedDamage;
        //        Takedamage(MyHealth);

        //        yield return null;
        //    }
        //    if (IsDied == false)
        //    {
        //        MaterialColorToChange(DefaultColorOfDeadBodyMeshMaterial, timer);
        //        for (int x = 0; x < BurningEffectForIncendiaryGrenade.Length; x++)
        //        {
        //            BurningEffectForIncendiaryGrenade[x].SetActive(false);
        //        }
        //    }
        //}
        private IEnumerator DamageReductionTimer()
        {
            // Calculate the total amount of damage to reduce
            initialDamage = Health - initialDamage;
            float elapsedTime = 0f;

            while (elapsedTime < timer)
            {
                elapsedTime += Time.deltaTime;

                // Smoothly interpolate reduced damage
                reducedDamage = Mathf.Lerp(Health, initialDamage, elapsedTime / timer);

                // Ensure no floating-point drift by clamping the value
                reducedDamage = Mathf.Clamp(reducedDamage, initialDamage, Health);

                // Calculate current health based on reduced damage
                float MyHealth = Health - reducedDamage;

                // Apply the damage
                Takedamage(MyHealth);

                yield return null; // Wait for the next frame
            }

            // Apply the final damage
            Takedamage(initialDamage);

            // Actions after the health reduction is complete
            if (!IsDied)
            {
                MaterialColorToChange(DefaultColorOfDeadBodyMeshMaterial, timer);
                for (int x = 0; x < BurningEffectForIncendiaryGrenade.Length; x++)
                {
                    BurningEffectForIncendiaryGrenade[x].SetActive(false);
                }
            }
        }


        // Calling Send Message Through The Gunscript and Humanoid Firing Behaviour Script
        public void FindColliderName(string HitObjName)
        {
            collidername = HitObjName;

        }
        public void WeakPointdamage(float amount)
        {
            if (IsDied == false)
            {
                float DamageToTakeHere = amount * WeakPointDamageMultiplier;
                // Debug.Log(transform.name + DamageToTakeHere);
                if (WinningProperties.instance != null)
                {
                    WinningProperties.instance.ShowTotalHeadShots++;
                    WinningProperties.instance.TotatHeadShotBonusRecieved = WinningProperties.instance.TotatHeadShotBonusRecieved + WinningProperties.instance.BonusPerHeadShot;
                    IsAiLastShotWasOnHead = true;
                }
                Takedamage(DamageToTakeHere);
            }
        }

        public void Die() // Checking For Which die Feature to use and apply
        {
            StartCoroutine(DelayDeath());
        }
        IEnumerator DelayDeath() // Delay is important so that we can disable navmesh obstacle and enable navmesh agent on this humanoid AI agent to avoid snapping effect.
        {
            yield return new WaitForSeconds(0.01f); 
            if (EnableRagdoll == true)
            {
                if (AnimatorComponent != null)
                {
                    AnimatorComponent.enabled = false;
                }
                if (SAB != null)
                {
                    if (SAB.Components.HumanoidFiringBehaviourComponent != null)
                    {
                        if (SAB.Components.HumanoidFiringBehaviourComponent.Components.WeaponMesh != null)
                        {
                            SAB.Components.HumanoidFiringBehaviourComponent.Components.WeaponMesh.GetComponent<Collider>().enabled = true;
                            SAB.Components.HumanoidFiringBehaviourComponent.Components.WeaponMesh.gameObject.AddComponent<Rigidbody>();
                        }
                        SAB.Components.HumanoidFiringBehaviourComponent.DissapearGun();
                        SAB.Components.HumanoidFiringBehaviourComponent.Components.WeaponMesh.transform.parent = null;
                    }

                    StartCoroutine(enemydeadbody());
                }

                foreach (Transform g in transform.GetComponentsInChildren<Transform>())
                {
                    if (g.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        g.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                        g.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
                    }
                }
            }
            else
            {
                if (SAB.Components.HumanoidFiringBehaviourComponent != null)
                {
                    if (SAB.Components.HumanoidFiringBehaviourComponent.Components.WeaponMesh != null)
                    {
                        SAB.Components.HumanoidFiringBehaviourComponent.Components.WeaponMesh.GetComponent<Collider>().enabled = true;
                        SAB.Components.HumanoidFiringBehaviourComponent.Components.WeaponMesh.gameObject.AddComponent<Rigidbody>();
                    }
                    SAB.Components.HumanoidFiringBehaviourComponent.DissapearGun();
                    SAB.Components.HumanoidFiringBehaviourComponent.Components.WeaponMesh.transform.parent = null;
                }
              
                RandomDeadAnimations();
                StartCoroutine(enemydeadbody());
            }
        }
        public IEnumerator enemydeadbody()
        {
            if (KeepDeadBody == true)
            {
                if (SpawnPickupItemsAfterDying == true)
                {
                    for (int z = 0; z < PickupItems.Length; z++)
                    {
                        Instantiate(PickupItems[z], transform.position + new Vector3(PickupItemOffsetFromPosition.x, PickupItemOffsetFromPosition.y, PickupItemOffsetFromPosition.z), Quaternion.identity);
                    }
                }
                yield return new WaitForSeconds(TIME_TO_UNPARENT_BODYMESH);
                PelvisBoneTransform.transform.parent = null;
                rootPos = PelvisBoneTransform.position;
                rootRot = PelvisBoneTransform.localEulerAngles;
                DeadBodyMesh.gameObject.transform.parent = null;
                DeadBodyMesh.transform.position = rootPos;
                DeadBodyMesh.transform.eulerAngles = rootRot;

                for (int x = 0; x < DeadBodyMeshItemsToAttach.Length; x++)
                {
                    if(DeadBodyMeshItemsToAttach[x] != null && DeadBodyMesh != null)
                    {
                        DeadBodyMeshItemsToAttach[x].transform.parent = DeadBodyMesh.gameObject.transform;
                    }
                }

                for (int x = 0; x < BurningEffectForIncendiaryGrenade.Length; x++)
                {
                    Vector3 EffectScale = BurningEffectForIncendiaryGrenade[x].transform.localScale;
                    BurningEffectForIncendiaryGrenade[x].transform.parent = DeadBodyMesh.gameObject.transform;
                    BurningEffectForIncendiaryGrenade[x].transform.localScale = EffectScale;
                }

                DeadBodyMesh.gameObject.AddComponent<DisappearMeshScript>();
                DeadBodyMesh.gameObject.GetComponent<DisappearMeshScript>().BodyDisappearTime = DeadBodyExistenceTime;
               // DeadBodyMesh.gameObject.GetComponent<DisappearMeshScript>().ItemRotation = ResetPickupRotation;
             //   DeadBodyMesh.gameObject.GetComponent<DisappearMeshScript>().ItemsToSpawn = new GameObject[PickupItems.Length];
                //for (int z = 0; z < PickupItems.Length; z++)
                //{
                //    DeadBodyMesh.gameObject.GetComponent<DisappearMeshScript>().ItemsToSpawn[z] = PickupItems[z];
                //}
                //if (SpawnPickUpItemsAfterAgentIsDestroyed == false)
                //{
                //    DeadBodyMesh.gameObject.GetComponent<DisappearMeshScript>().ShouldSpawnMagazines = false;
                //}
                //else
                //{
                //    DeadBodyMesh.gameObject.GetComponent<DisappearMeshScript>().ShouldSpawnMagazines = true;
                //}
                DeadBodyMesh.gameObject.GetComponent<DisappearMeshScript>().StartDisappearMethod();
                // Added line 438 to 446 because previously when Enable Ragdoll is set to be false and this line is used Destroy(PelvisBoneTransform.gameObject, TIME_TO_DESTROY_OTHER_COMPONENTS - 1f); than the mesh
                // get disappear when pelvis bone is unparented so and untill the pelvis bone do not get destroyed mesh keep disappeared.
                if (EnableRagdoll == false)
                {
                    Destroy(PelvisBoneTransform.gameObject);
                }
                else
                {
                    Destroy(PelvisBoneTransform.gameObject); 
                    // Destroy(PelvisBoneTransform.gameObject, TIME_TO_DESTROY_OTHER_COMPONENTS - 1f); 
                }
                // PelvisBoneTransform.gameObject.SetActive(false);
                Destroy(transform.root.gameObject, TIME_TO_DESTROY_OTHER_COMPONENTS);
            }
            else
            {
                yield return new WaitForSeconds(DeadBodyExistenceTime);
                if (SpawnPickupItemsAfterDying == true)
                {
                    for (int z = 0; z < PickupItems.Length; z++)
                    {
                        Instantiate(PickupItems[z], transform.position + new Vector3(PickupItemOffsetFromPosition.x, PickupItemOffsetFromPosition.y, PickupItemOffsetFromPosition.z), Quaternion.identity);
                    }
                }
                yield return new WaitForSeconds(0.1f);
                Destroy(transform.root.gameObject);
            }

        }
        public void RandomDeadAnimations() // Randomising Dead Animations
        {
            if (PlayDeathAnimation == false)
            {
                if (SAB != null)
                {
                    if (SAB.IsCrouched == true)
                    {
                        if (HeadBoneCollider != null)
                        {
                            if (collidername == HeadBoneCollider.transform.name)
                            {
                                int randomise = Random.Range(0, CrouchHeadShotDyingAnimationStateNames.Length);
                                if (AnimatorComponent != null)
                                {
                                    AnimatorComponent.Play(CrouchHeadShotDyingAnimationStateNames[randomise], -1, 0);
                                }
                            }
                            else
                            {
                                int randomise = Random.Range(0, CrouchDyingAnimationStateNames.Length);
                                if (AnimatorComponent != null)
                                {
                                    AnimatorComponent.Play(CrouchDyingAnimationStateNames[randomise], -1, 0);
                                }
                            }

                        }
                        else
                        {
                            int randomise = Random.Range(0, CrouchDyingAnimationStateNames.Length);
                            if (AnimatorComponent != null)
                            {
                                AnimatorComponent.Play(CrouchDyingAnimationStateNames[randomise], -1, 0);
                            }
                        }



                        SAB.Components.NavMeshAgentComponent.baseOffset = SAB.NavMeshAgentSettings.NavMeshAgentCrouchBaseOffsetDuringDeath;
                    }
                    else
                    {
                        if (HeadBoneCollider != null)
                        {
                            if (collidername == HeadBoneCollider.transform.name)
                            {
                                int randomise = Random.Range(0, StandHeadShotDyingAnimationStateNames.Length);
                                if (AnimatorComponent != null)
                                {
                                    AnimatorComponent.Play(StandHeadShotDyingAnimationStateNames[randomise], -1, 0);
                                }
                            }
                            else
                            {
                                int randomise = Random.Range(0, StandDyingAnimationStateNames.Length);
                                if (AnimatorComponent != null)
                                {
                                    AnimatorComponent.Play(StandDyingAnimationStateNames[randomise], -1, 0);
                                }
                            }
                        }
                        else
                        {
                            int randomise = Random.Range(0, StandDyingAnimationStateNames.Length);
                            if (AnimatorComponent != null)
                            {
                                AnimatorComponent.Play(StandDyingAnimationStateNames[randomise], -1, 0);
                            }
                        }


                        SAB.Components.NavMeshAgentComponent.baseOffset = SAB.NavMeshAgentSettings.NavMeshAgentStandBaseOffsetDuringDeath;
                    }
                    PlayDeathAnimation = true;
                }

                if (SAB.AgentRole != CoreAiBehaviour.Role.Zombie)
                {
                    SAB.AnimatorLayerWeightControllerScript.ChangeLayerWeight(5, 1f);

                    SAB.AnimatorLayerWeightControllerScript.ChangeLayerWeight(1, 0f, true);
                    SAB.AnimatorLayerWeightControllerScript.ChangeLayerWeight(2, 0f, true);
                    SAB.AnimatorLayerWeightControllerScript.ChangeLayerWeight(3, 0f, true);
                    SAB.AnimatorLayerWeightControllerScript.ChangeLayerWeight(4, 0f, true);
                }
                else
                {
                    SAB.AnimatorLayerWeightControllerScript.ChangeLayerWeight(2, 1f);

                    SAB.AnimatorLayerWeightControllerScript.ChangeLayerWeight(0, 0f, true);
                    SAB.AnimatorLayerWeightControllerScript.ChangeLayerWeight(1, 0f, true);
                

                }
              

            }


        }
        public void MaterialColorToChange(Color ColorToChangeTo,float dur)
        {
            // LeanTween.color will animate the material's color over a specified duration (1 second)
            LeanTween.value(gameObject, DeadBodyMesh.material.color, ColorToChangeTo, dur)
                .setOnUpdate((Color value) => {
                    DeadBodyMesh.material.color = value;
                });
        }
        //public void RemoveFromListOfEnemies(Transform EnemyNameWhoKilled)
        //{
        //     if(Health <= 0f)
        //     {
        //        if (TaskCompleted == false)
        //        {
        //            EnemyNameWhoKilled.gameObject.SendMessage("FindImmediateEnemy", SendMessageOptions.DontRequireReceiver);
        //            TaskCompleted = true;
        //        }
        //     } 
        //}
    }
}