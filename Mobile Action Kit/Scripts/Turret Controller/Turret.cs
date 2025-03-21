using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class Turret : MonoBehaviour
    {
        public enum ShootingTypes
        {
            RaycastShooting,
            ProjectileShooting
        }

        [TextArea]
        [Tooltip("Detailed description of the script's purpose and functionality.")]
        public string ScriptInfo = "This Turret script provides the core functionality for an automated turret system in a game. " +
            "It includes enemy detection, shooting mechanisms, rotation logic, and health management. " +
            "The turret detects enemies within a customizable range, targets the closest enemy, and fires based on the specified fire rate. " +
            "It also handles reloading, health bar visualization, and turret destruction when health reaches zero. Field of view settings allow for precise targeting behavior.";

        

        [System.Serializable]
        public class TurretDetectionClass
        {
            [Tooltip("The maximum distance within which the turret can detect enemies.")]
            public float TurretRange = 100f;
            [Tooltip("Reference to the Targets component used for managing enemy detection and filtering.")]
            public Targets TargetsComponent;
            [Tooltip("Enable or disable the turret's field of view restriction.")]
            public bool EnableFieldOfView = false;
            [Tooltip("The transform used as the origin for field of view calculations.")]
            public Transform FovObject;
            [Tooltip("The angle (in degrees) defining the turret's field of view.")]
            public float FieldOfViewValue = 45f;
            [Tooltip("The minimum time interval (in seconds) between consecutive checks for enemies.")]
            public float EnemyCheckIntervalMin = 0.2f;
            [Tooltip("The maximum time interval (in seconds) between consecutive checks for enemies.")]
            public float EnemyCheckIntervalMax = 0.4f;
            [Tooltip("If true, the turret will only switch targets when the current one is null.")]
            public bool SwitchOnlyIfEnemyIsNull = true;
        }

        public TurretDetectionClass Detection;

        [System.Serializable]
        public class TurretShootingClass
        {
            public ShootingTypes ShootingOptions;
            public ObjectPooler ObjectPoolerComponent;
            public string DefaultImpactEffectName = "PavementImpactEffect";
            public string ProjectileName = "AssaultRifleBullet";
            [Tooltip("The point from which projectiles or raycasts are emitted.")]
            public Transform ShootingPoint;
            [Tooltip("The maximum range within which the turret can shoot.")]
            public float ShootingRange = 50f;
            public int ProjectilesPerShot = 1;
            [Tooltip("The particle system used for muzzle flash effects when the turret fires.")]
            public ParticleSystem muzzleFlash;
            [Tooltip("The number of shots the turret can fire per second.")]
            public float fireRate = 15f;
            [Tooltip("The amount of damage dealt to a target with each shot.")]
            public int DamageToTarget = 10;

            [Tooltip("Bullet Shell Name Inside Object Pooler Script")]
            public string BulletShellName = "BulletShell";
            public GameObject BulletShellSpawnPoint;
            public float ShellsEjectingSpeed = 10f;
       
            public bool UseBulletSpread = true;
            public float MinBulletSpreadRotationX = -0.1f;
            public float MaxBulletSpreadRotationX = 0.1f;
            public float MinBulletSpreadRotationY = -0.1f;
            public float MaxBulletSpreadRotationY = 0.1f;
        }

        public TurretShootingClass Shooting;


        [System.Serializable]
        public class TurretAmmoClass
        {
            [Tooltip("The total ammunition available for the turret.")]
            public int TotalAmmo = 10000;
            [Tooltip("The current number of rounds loaded in the turret.")]
            public int LoadedAmmo = 60;
           
            [Tooltip("The time (in seconds) it takes to reload the turret.")]
            public float ReloadTime = 2f;
        }

        public TurretAmmoClass Ammo;


        [System.Serializable]
        public class TurretRotationClass
        {
            [Tooltip("The GameObject that will rotate to aim at the target.")]
            public GameObject GameobjectToRotate;
            [Tooltip("Enable or disable rotation on the X-axis.")]
            public bool RotateOnX = true;
            [Tooltip("Enable or disable rotation on the Y-axis.")]
            public bool RotateOnY = true;
            [Tooltip("The speed at which the turret rotates toward the target.")]
            public float RotationSpeed = 5f;
        }

        public TurretRotationClass Rotation;

        [System.Serializable]
        public class TurretHealthClass
        {
            [Tooltip("The maximum health of the turret.")]
            public float TurretHealth = 100f;
            [Tooltip("Enable or disable the health bar for the turret.")]
            public bool EnableHealthBar = true;
            [Tooltip("The prefab for the health bar UI.")]
            public GameObject healthBarPrefab;
            [Tooltip("Determines whether the health bar rotates with the turret.")]
            public bool RotateHealthBarWithObject = true;
            [Tooltip("The position offset of the health bar relative to the turret.")]
            public Vector3 HealthBarOffset;
            [Tooltip("The GameObject to activate when the turret is destroyed.")]
            public GameObject damagedTurret;
            [Tooltip("Array of mesh renderers to disable when the turret is destroyed.")]
            public MeshRenderer[] MeshRenderersToDeactive;

            public List<HealthConditions> HealthThresholdAction = new List<HealthConditions>();
        }

        public TurretHealthClass Health;

        [System.Serializable]
        public class HealthConditions
        {
            public int ActivationThreshold = 20;
            public GameObject GameObjectToActivate;

        }

        [System.Serializable]
        public class TurretAudioClass
        {
            public AudioSource AudioSourceComponent;
            public AudioClip Fire;
            public AudioClip Reload;
            public AudioClip TurretDamaged;
        }

        public TurretAudioClass Sounds;

        private int MaxTurretAmmoCapacity = 60;
        private bool isReloading = false;
        private Transform target;
        private List<GameObject> enemies = new List<GameObject>();
        private GameObject healthBarInstance;
        private Transform canvas3D;
        private float nextFireTime;
        private bool isDestroyed = false;
        [HideInInspector]
        [Tooltip("Indicates whether the turret is dead.")]
        public bool IsDied = false;
        private Image HealthBarfill;
        private float MaximumHealth;
  
        RaycastHit hit;
        Vector3 DefaultShootingPointRotation;

        void Start()
        {
            DefaultShootingPointRotation = Shooting.ShootingPoint.localEulerAngles;
               MaxTurretAmmoCapacity = Ammo.LoadedAmmo;
               MaximumHealth = Health.TurretHealth;

            if (Health.EnableHealthBar)
            {
                canvas3D = GameObject.FindGameObjectWithTag("Canvas3D")?.transform;
                if (canvas3D != null && Health.healthBarPrefab != null)
                {
                    healthBarInstance = Instantiate(Health.healthBarPrefab, canvas3D);
                    healthBarInstance.transform.SetParent(canvas3D);
                    healthBarInstance.transform.position = transform.position + Health.HealthBarOffset;

                    foreach (Image childImage in healthBarInstance.GetComponentsInChildren<Image>())
                    {
                        if (childImage.type == Image.Type.Filled)
                        {
                            HealthBarfill = childImage;
                            break;
                        }
                    }
                }
            }

            // Find initial list of enemies and start the enemy detection coroutine
            FindEnemies();
            StartCoroutine(FindClosestEnemyRoutine());
        }

        void Update()
        {
            if(IsDied == false)
            {
                // Check if the target is within the field of view
                if (Detection.EnableFieldOfView && !IsTargetInFieldOfView())
                {
                    target = null;
                }

                // Handle rotation
                if (Rotation.GameobjectToRotate != null && target != null)
                {
                    RotateTowardsTarget();
                }

                // Rotate the health bar if enabled
                if (Health.RotateHealthBarWithObject && healthBarInstance != null)
                {
                    RotateHealthBar();
                }

                // Automatically shoot if the raycast hits the target
                if (Time.time >= nextFireTime && Ammo.LoadedAmmo > 0)
                {
                    ShootIfTargetHit();
                }

                // Reload if turret ammo is empty
                if (isReloading == false)
                {
                    if (Ammo.LoadedAmmo <= 0 && Ammo.TotalAmmo > 0)
                    {
                        StartCoroutine(ReloadTurret());
                        isReloading = true;
                    }
                }
            }
        }

        private bool IsTargetInFieldOfView()
        {
            if (target == null)
                return false;

            Vector3 directionToTarget = (target.position - Detection.FovObject.position).normalized;
            float angleToTarget = Vector3.Angle(Detection.FovObject.forward, directionToTarget);

            return angleToTarget <= Detection.FieldOfViewValue / 2f;
        }
        public void BulletsFunctionality(Vector3 pos, Quaternion rot)
        {
            GameObject bullet = Shooting.ObjectPoolerComponent.SpawnFromPool(Shooting.ProjectileName, pos, rot);
            BulletScript B = bullet.GetComponent<BulletScript>();
            B.Movement(transform, transform.root, true);
        }
        private void ShootIfTargetHit()
        {
            if (target == null)
                return;
                
            if (Physics.Raycast(Shooting.ShootingPoint.position, Shooting.ShootingPoint.forward, out hit, Shooting.ShootingRange))
            {
                if (hit.transform.root == target.transform.root)
                {
                    if (Shooting.ObjectPoolerComponent != null)
                    {
                        GameObject bulletShellrb = Shooting.ObjectPoolerComponent.SpawnFromPool(Shooting.BulletShellName, Shooting.BulletShellSpawnPoint.transform.position, Shooting.BulletShellSpawnPoint.transform.rotation);
                        Rigidbody r = bulletShellrb.GetComponent<Rigidbody>();
                        r.velocity = Shooting.BulletShellSpawnPoint.transform.TransformDirection(Vector3.right * Shooting.ShellsEjectingSpeed);
                    }

                    if (Shooting.UseBulletSpread == true)
                    {
                        Vector3 Spread = hit.point;
                        Spread.x = Random.Range(hit.point.x + Shooting.MinBulletSpreadRotationX, hit.point.x + Shooting.MaxBulletSpreadRotationX);
                        Spread.y = Random.Range(hit.point.y + Shooting.MinBulletSpreadRotationY, hit.point.y + Shooting.MaxBulletSpreadRotationY);
                        hit.point = Spread;
                    }

                    if (Shooting.ShootingOptions == ShootingTypes.RaycastShooting)
                    {
                        Ammo.LoadedAmmo--; // Reduce ammo with each shot

                        if (hit.transform.root.GetComponent<PlayerHealth>() != null)
                        {
                            PlayerHealth.instance.PlayerHealthbar.Curvalue -= Shooting.DamageToTarget;
                            PlayerHealth.instance.CheckPlayerHealth();
                        }
                        else if (hit.transform.root.GetComponent<HumanoidAiHealth>() != null)
                        {
                            if (hit.transform.tag == "WeakPoint")
                            {
                                hit.collider.gameObject.transform.root.GetComponent<HumanoidAiHealth>().FindColliderName(hit.collider.name);
                                hit.collider.gameObject.transform.root.GetComponent<HumanoidAiHealth>().WeakPointdamage(Shooting.DamageToTarget);
                                hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.WhoKilledMe = transform.root;
                                hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.WhoShootingMe = transform.root;
                                hit.collider.gameObject.transform.root.GetComponent<HumanoidAiHealth>().Effects(hit);
                            }
                            else
                            {
                                hit.collider.gameObject.transform.root.GetComponent<HumanoidAiHealth>().FindColliderName(hit.collider.name);
                                hit.collider.gameObject.transform.root.GetComponent<HumanoidAiHealth>().Takedamage(Shooting.DamageToTarget);
                                hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.WhoKilledMe = transform.root;
                                hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.WhoShootingMe = transform.root;
                                hit.collider.gameObject.transform.root.GetComponent<HumanoidAiHealth>().Effects(hit);
                            }
                        }
                        else
                        {
                            GameObject impacteffect = Shooting.ObjectPoolerComponent.SpawnFromPool(Shooting.DefaultImpactEffectName, hit.point, Quaternion.LookRotation(hit.normal));

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
                                        impacteffect.gameObject.GetComponent<ImpactEffect>().EffectActivation(transform);
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        if (Shooting.ProjectilesPerShot > 0)
                        {
                            for (int x = 0; x < Shooting.ProjectilesPerShot; x++)
                            {
                                if (Shooting.UseBulletSpread == true)
                                {
                                    Vector3 Spread = Shooting.ShootingPoint.localEulerAngles;
                                    Spread.x = Random.Range(Shooting.MinBulletSpreadRotationX, Shooting.MaxBulletSpreadRotationX);
                                    Spread.y = Random.Range(Shooting.MinBulletSpreadRotationY, Shooting.MaxBulletSpreadRotationY);
                                    Shooting.ShootingPoint.localEulerAngles = Spread;
                                }
                                Ammo.LoadedAmmo--; // Reduce ammo with each shot
                                BulletsFunctionality(Shooting.ShootingPoint.position, Shooting.ShootingPoint.rotation);
                            }
                            Shooting.ShootingPoint.localEulerAngles = DefaultShootingPointRotation;
                        }
                    }

                    if(Sounds.AudioSourceComponent != null && Sounds.Fire != null)
                    {
                        Sounds.AudioSourceComponent.clip = Sounds.Fire;
                        Sounds.AudioSourceComponent.PlayOneShot(Sounds.Fire);
                    }
                    nextFireTime = Time.time + 1f / Shooting.fireRate;
                    PlayMuzzleFlash();
                   
                }
            }
          
        }

        private void PlayMuzzleFlash()
        {
            if (Shooting.muzzleFlash != null)
            {
                Shooting.muzzleFlash.Play(); // Play the particle system
            }
        }

        private IEnumerator ReloadTurret()
        {
            if (Sounds.AudioSourceComponent != null && Sounds.Reload != null)
            {
                Sounds.AudioSourceComponent.clip = Sounds.Reload;
                Sounds.AudioSourceComponent.PlayOneShot(Sounds.Reload);
            }
            yield return new WaitForSeconds(Ammo.ReloadTime);
            int ammoToReload = Mathf.Min(MaxTurretAmmoCapacity, Ammo.TotalAmmo);
            Ammo.LoadedAmmo = ammoToReload;
            Ammo.TotalAmmo -= ammoToReload;
            isReloading = false;
        }
        private void RotateTowardsTarget()
        {
            Vector3 directionToTarget = target.position - Rotation.GameobjectToRotate.transform.position;

            // Modify the direction vector based on the enabled axes
            if (!Rotation.RotateOnX)
                directionToTarget.x = 0; // Ignore X-axis rotation if disabled
            if (!Rotation.RotateOnY)
                directionToTarget.y = 0; // Ignore Y-axis rotation if disabled

            if (directionToTarget.magnitude <= Detection.TurretRange)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                Rotation.GameobjectToRotate.transform.rotation = Quaternion.Slerp(
                    Rotation.GameobjectToRotate.transform.rotation,
                    targetRotation,
                    Time.deltaTime * Rotation.RotationSpeed
                );
            }
        }

        private void RotateHealthBar()
        {
            if (target == null || healthBarInstance == null)
                return;

            Vector3 directionToTarget = target.position - healthBarInstance.transform.position;
            directionToTarget.y = 0; // Ignore Y-axis for rotation

            if (directionToTarget.magnitude <= Detection.TurretRange)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                healthBarInstance.transform.rotation = Quaternion.Slerp(
                    healthBarInstance.transform.rotation,
                    targetRotation,
                    Time.deltaTime * Rotation.RotationSpeed
                );
            }
        }

        private void FindEnemies()
        {
            enemies.Clear();
            Targets[] allTargets = FindObjectsOfType<Targets>();
            foreach (Targets targetScript in allTargets)
            {
                if (targetScript.MyTeamID != Detection.TargetsComponent.MyTeamID)
                {
                    enemies.Add(targetScript.MyBodyPartToTarget.gameObject);
                }
            }
        }

        private IEnumerator FindClosestEnemyRoutine()
        {
            while (!IsDied)
            {
                FindClosestEnemy();
                float waitTime = Random.Range(Detection.EnemyCheckIntervalMin, Detection.EnemyCheckIntervalMax);
                yield return new WaitForSeconds(waitTime);
            }
        }

        private void FindClosestEnemy()
        {
            float closestDistance = float.MaxValue;
            GameObject closestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                if (enemy == null)
                    continue;

                if(Detection.SwitchOnlyIfEnemyIsNull == false)
                {
                    if (enemy.transform.root.GetComponent<HumanoidAiHealth>() != null)
                    {
                        if (enemy.transform.root.GetComponent<HumanoidAiHealth>().IsDied == true)
                        {
                            continue;
                        }
                    }
                    else if (enemy.transform.root.GetComponent<PlayerHealth>() != null)
                    {
                        if (enemy.transform.root.GetComponent<PlayerHealth>().IsDead == true)
                        {
                            continue;
                        }
                    }
                }

                float distance = Vector3.Distance(transform.position, enemy.transform.root.position);
                if (distance < closestDistance && distance <= Detection.TurretRange)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }

            target = closestEnemy != null ? closestEnemy.transform : null;
        }

        public void TakeDamage(float damageAmount)
        {
            Health.TurretHealth -= damageAmount;

            for(int x = 0;x < Health.HealthThresholdAction.Count; x++)
            {
                if(Health.TurretHealth <= Health.HealthThresholdAction[x].ActivationThreshold)
                {
                    Health.HealthThresholdAction[x].GameObjectToActivate.SetActive(true);
                }
            }

            if (Health.EnableHealthBar)
            {
                UpdateHealthBar(Health.TurretHealth, MaximumHealth);
            }

            if (Health.TurretHealth <= 0 && !isDestroyed)
            {
                if (WinningProperties.instance != null)
                {
                    WinningProperties.instance.ShowTotalBodyKills++;
                    WinningProperties.instance.TotalBodyKillsBonusRecieved = WinningProperties.instance.TotalBodyKillsBonusRecieved + WinningProperties.instance.BonusPerBodyKill;
                }
                DestroyTurret();
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

        private void DestroyTurret()
        {
            isDestroyed = true;

            if (Health.damagedTurret != null)
            {
                Health.damagedTurret.SetActive(true); // Activate the damaged turret GameObject
            }

            for (int x = 0; x < Health.MeshRenderersToDeactive.Length; x++)
            {
                Health.MeshRenderersToDeactive[x].enabled = false;
            }

            if (healthBarInstance != null)
            {
                Destroy(healthBarInstance);
            }
            if (Sounds.AudioSourceComponent != null && Sounds.TurretDamaged != null)
            {
                Sounds.AudioSourceComponent.clip = Sounds.TurretDamaged;
                Sounds.AudioSourceComponent.PlayOneShot(Sounds.TurretDamaged);
            }

            IsDied = true;
        }
    }
}




//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//namespace MobileActionKit
//{
//    public class Turret : MonoBehaviour
//    {
//        [Header("Shooting Settings")]
//        public Transform ShootingPoint;
//        public float TurretRange = 100f;  // Turret's detection range
//        public ParticleSystem muzzleFlash; // Particle system for shooting effects
//        public float ShootingRange = 50f; // Shooting range
//        public float fireRate = 1f; // Fire rate (shots per second)

//        [Header("Rotation Settings")]
//        public GameObject objectToRotate; // The object that will rotate
//        Transform target; // The current target
//        public float RotationSpeed = 5f;
//        public bool RotateHealthBarWithObject = true; // Checkbox to rotate health bar with object

//        [Header("Field of View Settings")]
//        public bool EnableFieldOfView = false; // Checkbox to enable field of view
//        public Transform FovObject;
//        public float FieldOfViewValue = 45f; // Field of view angle in degrees

//        [Header("Health Settings")]
//        public float TurretHealth = 100f; // Turret health
//        public bool EnableHealthBar = true;
//        public GameObject healthBarPrefab; // Health bar prefab
//        public Vector3 HealthBarOffset;
//        public GameObject damagedTurret; // Damaged turret GameObject
//        public MeshRenderer[] MeshRenderersToDeactive;

//        [Header("Enemy Detection")]
//        public Targets TargetsComponent;
//        public float EnemyCheckIntervalMin = 0.2f; // Minimum interval to check for enemies
//        public float EnemyCheckIntervalMax = 0.4f; // Maximum interval to check for enemies

//        private List<GameObject> enemies = new List<GameObject>();
//        private GameObject healthBarInstance;
//        private Transform canvas3D;
//        private float nextFireTime;
//        private bool isDestroyed = false;

//        [HideInInspector]
//        public bool IsDied = false;
//        Image HealthBarfill;
//        float MaximumHealth;

//        void Start()
//        {
//            MaximumHealth = TurretHealth;

//            if (EnableHealthBar)
//            {
//                canvas3D = GameObject.FindGameObjectWithTag("Canvas3D")?.transform;
//                if (canvas3D != null && healthBarPrefab != null)
//                {
//                    healthBarInstance = Instantiate(healthBarPrefab, canvas3D);
//                    healthBarInstance.transform.SetParent(canvas3D);
//                    healthBarInstance.transform.position = transform.position + HealthBarOffset;

//                    foreach (Image childImage in healthBarInstance.GetComponentsInChildren<Image>())
//                    {
//                        if (childImage.type == Image.Type.Filled)
//                        {
//                            HealthBarfill = childImage;
//                            break;
//                        }
//                    }
//                }
//            }

//            // Find initial list of enemies and start the enemy detection coroutine
//            FindEnemies();
//            StartCoroutine(FindClosestEnemyRoutine());
//        }

//        void Update()
//        {
//            if (IsDied)
//                return;

//            // Check if the target is within the field of view
//            if (EnableFieldOfView && !IsTargetInFieldOfView())
//                return;

//            // Handle rotation
//            if (objectToRotate != null && target != null)
//            {
//                RotateTowardsTarget();
//            }

//            // Rotate the health bar if enabled
//            if (RotateHealthBarWithObject && healthBarInstance != null)
//            {
//                RotateHealthBar();
//            }

//            // Automatically shoot if the raycast hits the target
//            if (Time.time >= nextFireTime)
//            {
//                ShootIfTargetHit();
//            }
//        }

//        private bool IsTargetInFieldOfView()
//        {
//            if (target == null)
//                return false;

//            Vector3 directionToTarget = (target.position - FovObject.position).normalized;
//            float angleToTarget = Vector3.Angle(FovObject.forward, directionToTarget);

//            return angleToTarget <= FieldOfViewValue / 2f;
//        }

//        private void ShootIfTargetHit()
//        {
//            if (target == null)
//                return;

//            RaycastHit hit;
//            if (Physics.Raycast(ShootingPoint.position, ShootingPoint.forward, out hit, ShootingRange))
//            {
//                if (hit.transform.root == target)
//                {
//                    nextFireTime = Time.time + 1f / fireRate;
//                    Shoot();
//                }
//            }
//        }

//        private void Shoot()
//        {
//            if (muzzleFlash != null)
//            {
//                muzzleFlash.Play(); // Play the particle system
//            }
//        }

//        private void RotateTowardsTarget()
//        {
//            if (target == null)
//                return;

//            Vector3 directionToTarget = target.position - objectToRotate.transform.position;
//            directionToTarget.y = 0; // Ignore Y-axis for rotation

//            if (directionToTarget.magnitude <= TurretRange)
//            {
//                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
//                objectToRotate.transform.rotation = Quaternion.Slerp(
//                    objectToRotate.transform.rotation,
//                    targetRotation,
//                    Time.deltaTime * RotationSpeed
//                );
//            }
//        }

//        private void RotateHealthBar()
//        {
//            if (target == null || healthBarInstance == null)
//                return;

//            Vector3 directionToTarget = target.position - healthBarInstance.transform.position;
//            directionToTarget.y = 0; // Ignore Y-axis for rotation

//            if (directionToTarget.magnitude <= TurretRange)
//            {
//                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
//                healthBarInstance.transform.rotation = Quaternion.Slerp(
//                    healthBarInstance.transform.rotation,
//                    targetRotation,
//                    Time.deltaTime * RotationSpeed
//                );
//            }
//        }

//        private void FindEnemies()
//        {
//            enemies.Clear();
//            Targets[] allTargets = FindObjectsOfType<Targets>();
//            foreach (Targets targetScript in allTargets)
//            {
//                if (targetScript.MyTeamID != TargetsComponent.MyTeamID)
//                {
//                    enemies.Add(targetScript.gameObject);
//                }
//            }
//        }

//        private IEnumerator FindClosestEnemyRoutine()
//        {
//            while (!IsDied)
//            {
//                FindClosestEnemy();
//                float waitTime = Random.Range(EnemyCheckIntervalMin, EnemyCheckIntervalMax);
//                yield return new WaitForSeconds(waitTime);
//            }
//        }

//        private void FindClosestEnemy()
//        {
//            float closestDistance = float.MaxValue;
//            GameObject closestEnemy = null;

//            foreach (GameObject enemy in enemies)
//            {
//                if (enemy == null)
//                    continue;

//                float distance = Vector3.Distance(transform.position, enemy.transform.position);
//                if (distance < closestDistance && distance <= TurretRange)
//                {
//                    closestDistance = distance;
//                    closestEnemy = enemy;
//                }
//            }

//            target = closestEnemy != null ? closestEnemy.transform : null;
//        }
//        public void TakeDamage(float damageAmount)
//        {
//            TurretHealth -= damageAmount;

//            if (EnableHealthBar)
//            {
//                UpdateHealthBar(TurretHealth, MaximumHealth);
//            }

//            if (TurretHealth <= 0 && !isDestroyed)
//            {
//                DestroyTurret();
//            }
//        }

//        public void UpdateHealthBar(float currentHealth, float maxHealth)
//        {
//            float fillAmount = currentHealth / maxHealth;
//            if (HealthBarfill != null)
//            {
//                HealthBarfill.fillAmount = fillAmount;
//            }
//        }

//        private void DestroyTurret()
//        {
//            isDestroyed = true;

//            if (damagedTurret != null)
//            {
//                damagedTurret.SetActive(true); // Activate the damaged turret GameObject
//            }

//            for (int x = 0; x < MeshRenderersToDeactive.Length; x++)
//            {
//                MeshRenderersToDeactive[x].enabled = false;
//            }

//            if (healthBarInstance != null)
//            {
//                Destroy(healthBarInstance);
//            }

//            IsDied = true;
//        }
//    }
//}




//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;

//namespace MobileActionKit
//{
//    public class Turret : MonoBehaviour
//    {
//        [Header("Shooting Settings")]
//        public Transform ShootingPoint;
//        public float TurretRange = 100f;  // this will make the turret to keep looking at its enemy
//        public ParticleSystem muzzleFlash; // Particle system for shooting effects
//        public float ShootingRange = 50f; // Shooting range
//        public float fireRate = 1f; // Fire rate (shots per second)

//        [Header("Rotation Settings")]
//        public GameObject objectToRotate; // The object that will rotate
//        public Transform target; // The target to look at
//        public float RotationSpeed = 5f;
//        public bool RotateHealthBarWithObject = true; // Checkbox to rotate health bar with object

//        [Header("Field of View Settings")]
//        public bool EnableFieldOfView = false; // Checkbox to enable field of view
//        public Transform FovObject;
//        public float FieldOfViewValue = 45f; // Field of view angle in degrees

//        [Header("Health Settings")]
//        public float TurretHealth = 100f; // Turret health
//        public bool EnableHealthBar = true;
//        public GameObject healthBarPrefab; // Health bar prefab
//        public Vector3 HealthBarOffset;
//        public GameObject damagedTurret; // Damaged turret GameObject
//        public MeshRenderer[] MeshRenderersToDeactive;

//        private GameObject healthBarInstance;
//        private Transform canvas3D;
//        private float nextFireTime;
//        private bool isDestroyed = false;

//        [HideInInspector]
//        public bool IsDied = false;
//        Image HealthBarfill;
//        float MaximumHealth;

//        void Start()
//        {
//            MaximumHealth = TurretHealth;

//            if (EnableHealthBar)
//            {
//                // Find the Canvas3D object to spawn the health bar
//                canvas3D = GameObject.FindGameObjectWithTag("Canvas3D")?.transform;
//                if (canvas3D != null && healthBarPrefab != null)
//                {
//                    healthBarInstance = Instantiate(healthBarPrefab, canvas3D);
//                    healthBarInstance.transform.SetParent(canvas3D);
//                    healthBarInstance.transform.position = transform.position + HealthBarOffset;

//                    foreach (Image childImage in healthBarInstance.GetComponentsInChildren<Image>())
//                    {
//                        if (childImage.type == Image.Type.Filled)
//                        {
//                            HealthBarfill = childImage;
//                            break;
//                        }
//                    }
//                }
//            }
//        }

//        void Update()
//        {
//            if (IsDied)
//                return;

//            // Check if the target is within the field of view
//            if (EnableFieldOfView && !IsTargetInFieldOfView())
//                return;

//            // Handle rotation
//            if (objectToRotate != null && target != null)
//            {
//                RotateTowardsTarget();
//            }

//            // Rotate the health bar if enabled
//            if (RotateHealthBarWithObject && healthBarInstance != null)
//            {
//                RotateHealthBar();
//            }

//            // Automatically shoot if the raycast hits the target
//            if (Time.time >= nextFireTime)
//            {
//                ShootIfTargetHit();
//            }
//        }

//        private bool IsTargetInFieldOfView()
//        {
//            if (target == null)
//                return false;

//            Vector3 directionToTarget = (target.position - FovObject.position).normalized;
//            float angleToTarget = Vector3.Angle(FovObject.forward, directionToTarget);

//            return angleToTarget <= FieldOfViewValue / 2f;
//        }

//        private void ShootIfTargetHit()
//        {
//            RaycastHit hit;
//            if (Physics.Raycast(ShootingPoint.transform.position, ShootingPoint.transform.forward, out hit, ShootingRange))
//            {
//                if (hit.transform == target)
//                {
//                    nextFireTime = Time.time + 1f / fireRate;
//                    Shoot();
//                }
//            }
//        }

//        private void Shoot()
//        {
//            if (muzzleFlash != null)
//            {
//                muzzleFlash.Play(); // Play the particle system
//            }
//        }

//        private void RotateTowardsTarget()
//        {
//            Vector3 directionToTarget = target.position - objectToRotate.transform.position;
//            directionToTarget.y = 0; // Ignore Y-axis for rotation

//            if(directionToTarget.magnitude <= TurretRange)
//            {
//                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
//                objectToRotate.transform.rotation = Quaternion.Slerp(
//                    objectToRotate.transform.rotation,
//                    targetRotation,
//                    Time.deltaTime * RotationSpeed // Rotation speed multiplier (adjust as needed)
//                );
//            }

//        }

//        private void RotateHealthBar()
//        {
//            Vector3 directionToTarget = target.position - healthBarInstance.transform.position;
//            directionToTarget.y = 0; // Ignore Y-axis for rotation

//            if (directionToTarget.magnitude <= TurretRange)
//            {
//                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
//                healthBarInstance.transform.rotation = Quaternion.Slerp(
//                    healthBarInstance.transform.rotation,
//                    targetRotation,
//                    Time.deltaTime * RotationSpeed // Same rotation speed as objectToRotate
//                );
//            }
//        }

//        public void TakeDamage(float damageAmount)
//        {
//            TurretHealth -= damageAmount;

//            if (EnableHealthBar)
//            {
//                UpdateHealthBar(TurretHealth, MaximumHealth);
//            }

//            if (TurretHealth <= 0 && !isDestroyed)
//            {
//                DestroyTurret();
//            }
//        }

//        public void UpdateHealthBar(float currentHealth, float maxHealth)
//        {
//            float fillAmount = currentHealth / maxHealth;
//            if (HealthBarfill != null)
//            {
//                HealthBarfill.fillAmount = fillAmount;
//            }
//        }

//        private void DestroyTurret()
//        {
//            isDestroyed = true;

//            if (damagedTurret != null)
//            {
//                damagedTurret.SetActive(true); // Activate the damaged turret GameObject
//            }

//            for (int x = 0; x < MeshRenderersToDeactive.Length; x++)
//            {
//                MeshRenderersToDeactive[x].enabled = false;
//            }

//            if (healthBarInstance != null)
//            {
//                Destroy(healthBarInstance);
//            }

//            IsDied = true;
//        }
//    }
//}





//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;

//namespace MobileActionKit
//{
//    public class Turret : MonoBehaviour
//    {
//        [Header("Shooting Settings")]
//        public Transform ShootingPoint;
//        public ParticleSystem muzzleFlash; // Particle system for shooting effects
//        public float range = 50f; // Shooting range
//        public float fireRate = 1f; // Fire rate (shots per second)

//        [Header("Rotation Settings")]
//        public GameObject objectToRotate; // The object that will rotate
//        public Transform target; // The target to look at
//        public float RotationSpeed = 5f;
//        public bool RotateHealthBarWithObject = true; // Checkbox to rotate health bar with object

//        [Header("Health Settings")]
//        public float TurretHealth = 100f; // Turret health
//        public bool EnableHealthBar = true;
//        public GameObject healthBarPrefab; // Health bar prefab
//        public Vector3 HealthBarOffset;
//        public GameObject damagedTurret; // Damaged turret GameObject
//        public MeshRenderer[] MeshRenderersToDeactive;

//        private GameObject healthBarInstance;
//        private Transform canvas3D;
//        private float nextFireTime;
//        private bool isDestroyed = false;

//        [HideInInspector]
//        public bool IsDied = false;
//        Image HealthBarfill;
//        float MaximumHealth;

//        void Start()
//        {
//            MaximumHealth = TurretHealth;

//            if (EnableHealthBar == true)
//            {
//                // Find the Canvas3D object to spawn the health bar
//                canvas3D = GameObject.FindGameObjectWithTag("Canvas3D")?.transform;
//                if (canvas3D != null && healthBarPrefab != null)
//                {
//                    healthBarInstance = Instantiate(healthBarPrefab, canvas3D);
//                    healthBarInstance.transform.SetParent(canvas3D);
//                    healthBarInstance.transform.position = transform.position + HealthBarOffset;

//                    foreach (Image childImage in healthBarInstance.GetComponentsInChildren<Image>())
//                    {
//                        if (childImage.type == Image.Type.Filled)
//                        {
//                            HealthBarfill = childImage;
//                            break;
//                        }
//                    }
//                }
//            }

//        }

//        void Update()
//        {
//            if (IsDied == false)
//            {
//                // Handle rotation
//                if (objectToRotate != null && target != null)
//                {
//                    RotateTowardsTarget();
//                }

//                // Rotate the health bar if enabled
//                if (RotateHealthBarWithObject && healthBarInstance != null)
//                {
//                    RotateHealthBar();
//                }

//                // Automatically shoot if the raycast hits the target
//                if (Time.time >= nextFireTime)
//                {
//                    ShootIfTargetHit();
//                }
//            }

//        }

//        private void ShootIfTargetHit()
//        {
//            RaycastHit hit;
//            if (Physics.Raycast(ShootingPoint.transform.position, ShootingPoint.transform.forward, out hit, range))
//            {
//                if (hit.transform == target)
//                {
//                    nextFireTime = Time.time + 1f / fireRate;
//                    Shoot();
//                }
//            }
//        }

//        private void Shoot()
//        {
//            if (muzzleFlash != null)
//            {
//                muzzleFlash.Play(); // Play the particle system
//            }
//        }

//        private void RotateTowardsTarget()
//        {
//            Vector3 directionToTarget = target.position - objectToRotate.transform.position;
//            directionToTarget.y = 0; // Ignore Y-axis for rotation

//            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
//            objectToRotate.transform.rotation = Quaternion.Slerp(
//                objectToRotate.transform.rotation,
//                targetRotation,
//                Time.deltaTime * RotationSpeed // Rotation speed multiplier (adjust as needed)
//            );
//        }

//        private void RotateHealthBar()
//        {
//            Vector3 directionToTarget = target.position - healthBarInstance.transform.position;
//            directionToTarget.y = 0; // Ignore Y-axis for rotation

//            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
//            healthBarInstance.transform.rotation = Quaternion.Slerp(
//                healthBarInstance.transform.rotation,
//                targetRotation,
//                Time.deltaTime * RotationSpeed // Same rotation speed as objectToRotate
//            );
//        }

//        public void TakeDamage(float damageAmount)
//        {
//            TurretHealth -= damageAmount;

//            if (EnableHealthBar == true)
//            {
//                UpdateHealthBar(TurretHealth, MaximumHealth);
//            }

//            if (TurretHealth <= 0 && !isDestroyed)
//            {
//                DestroyTurret();
//            }
//        }
//        public void UpdateHealthBar(float currentHealth, float maxHealth)
//        {
//            float fillAmount = currentHealth / maxHealth;
//            if (HealthBarfill != null)
//            {
//                HealthBarfill.fillAmount = fillAmount;
//            }
//        }

//        private void DestroyTurret()
//        {
//            isDestroyed = true;

//            if (damagedTurret != null)
//            {
//                damagedTurret.SetActive(true); // Activate the damaged turret GameObject
//            }

//            for (int x = 0; x < MeshRenderersToDeactive.Length; x++)
//            {
//                MeshRenderersToDeactive[x].enabled = false;
//            }

//            if (healthBarInstance != null)
//            {
//                Destroy(healthBarInstance);
//            }

//            IsDied = true;
//        }
//    }
//}




//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;

//namespace MobileActionKit
//{
//    public class Turret : MonoBehaviour
//    {
//        [Header("Shooting Settings")]
//        public Transform ShootingPoint;
//        public ParticleSystem muzzleFlash; // Particle system for shooting effects
//        public float range = 50f; // Shooting range
//        public float fireRate = 1f; // Fire rate (shots per second)

//        [Header("Rotation Settings")]
//        public GameObject objectToRotate; // The object that will rotate
//        public Transform target; // The target to look at
//        public float RotationSpeed = 5f;

//        [Header("Health Settings")]
//        public float TurretHealth = 100f; // Turret health
//        public bool EnableHealthBar = true;
//        public GameObject healthBarPrefab; // Health bar prefab
//        public Vector3 HealthBarOffset;
//        public GameObject damagedTurret; // Damaged turret GameObject
//        public MeshRenderer[] MeshRenderersToDeactive;

//        private GameObject healthBarInstance;
//        private Transform canvas3D;
//        private float nextFireTime;
//        private bool isDestroyed = false;

//        [HideInInspector]
//        public bool IsDied = false;
//        Image HealthBarfill;
//        float MaximumHealth;

//        void Start()
//        {
//            MaximumHealth = TurretHealth;

//            if(EnableHealthBar == true)
//            {
//                // Find the Canvas3D object to spawn the health bar
//                canvas3D = GameObject.FindGameObjectWithTag("Canvas3D")?.transform;
//                if (canvas3D != null && healthBarPrefab != null)
//                {
//                    healthBarInstance = Instantiate(healthBarPrefab, canvas3D);
//                    healthBarInstance.transform.SetParent(canvas3D);
//                    healthBarInstance.transform.position = transform.position + HealthBarOffset;

//                    foreach (Image childImage in healthBarInstance.GetComponentsInChildren<Image>())
//                    {
//                        if (childImage.type == Image.Type.Filled)
//                        {
//                            HealthBarfill = childImage;
//                            break;
//                        }
//                    }
//                }
//            }

//        }

//        void Update()
//        {
//            if(IsDied == false)
//            {
//                // Handle rotation
//                if (objectToRotate != null && target != null)
//                {
//                    RotateTowardsTarget();
//                }

//                // Automatically shoot if the raycast hits the target
//                if (Time.time >= nextFireTime)
//                {
//                    ShootIfTargetHit();
//                }
//            }

//        }

//        private void ShootIfTargetHit()
//        {
//            RaycastHit hit;
//            if (Physics.Raycast(ShootingPoint.transform.position, ShootingPoint.transform.forward, out hit, range))
//            {
//                if (hit.transform == target)
//                {
//                    nextFireTime = Time.time + 1f / fireRate;
//                    Shoot();
//                }
//            }
//        }

//        private void Shoot()
//        {
//            if (muzzleFlash != null)
//            {
//                muzzleFlash.Play(); // Play the particle system
//            }
//        }

//        private void RotateTowardsTarget()
//        {
//            Vector3 directionToTarget = target.position - objectToRotate.transform.position;
//            directionToTarget.y = 0; // Ignore Y-axis for rotation

//            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
//            objectToRotate.transform.rotation = Quaternion.Slerp(
//                objectToRotate.transform.rotation,
//                targetRotation,
//                Time.deltaTime * RotationSpeed // Rotation speed multiplier (adjust as needed)
//            );
//        }

//        public void TakeDamage(float damageAmount)
//        {
//            TurretHealth -= damageAmount;

//            if (EnableHealthBar == true)
//            {
//                UpdateHealthBar(TurretHealth, MaximumHealth);
//            }

//            if (TurretHealth <= 0 && !isDestroyed)
//            {
//                DestroyTurret();
//            }
//        }
//        public void UpdateHealthBar(float currentHealth, float maxHealth)
//        {
//            float fillAmount = currentHealth / maxHealth;
//            if (HealthBarfill != null)
//            {
//                HealthBarfill.fillAmount = fillAmount;
//            }
//        }

//        private void DestroyTurret()
//        {
//            isDestroyed = true;

//            if (damagedTurret != null)
//            {
//                damagedTurret.SetActive(true); // Activate the damaged turret GameObject
//            }

//            for(int x =0;x < MeshRenderersToDeactive.Length; x++)
//            {
//                MeshRenderersToDeactive[x].enabled = false;  
//            }

//            if (healthBarInstance != null)
//            {
//                Destroy(healthBarInstance);
//            }

//            IsDied = true;
//        }
//    }
//}





//using System.Collections;
//using UnityEngine;

//// This Script is Responsible For Turrent Shooting and Searching Behaviour 
//namespace MobileActionKit
//{ 
//    public class Turret : MonoBehaviour
//    {
//        public static Turret instance;

//        [Tooltip("Range To Detect Enemies")]
//        public int DetectionRange = 10;
//        public bool EnableFieldOfView = false;
//        public float FieldOfViewAngle = 60;
//        int SaveDetectingDistance;

//        [HideInInspector]
//        public bool RemoveEnemiesFromList = true;

//        [HideInInspector]
//        public bool NoEnemyInView = true;
//        float Angle;
//        bool IsEnemyLocked = false;
//        Transform EnemyTransform;

//        public float MinimumTimeToSearchForClosestEnemy = 1f;
//        public float MaximumTimeToSearchForClosetEnemy = 2f;

//        private Transform TurretChild;
//        [Tooltip("The Parent Of This GameObject")]
//        public Transform TurretParent;
//        public float TurretRotationSpeed = 10f;
//        [Tooltip("Should The Parent Also Rotate At Y")]
//        public bool RotateParentOnYAxis = false;
//        public bool RotateXAxis = false;
//        public bool ClampXAxisRotationValues = false;
//        public float MinimumXAxisRotation;
//        public float MaximumXAxisRotation;

//        Vector3 dir;

//        public float MinimumSearchingAngle = -180;
//        public float MaximumSearchingAngle = 180f;
//        [Tooltip("How Fast it Should Rotate At Y")]
//        public float SearchingSpeedAtY = 8f;
//        bool StartRotating = false;
//        bool Rotatenow = false;
//        float randomisation;
//        bool StoreRotOnce = false;


//        float RandomiseTimeToSearch;
//        private FindEnemies FindEnemiesScript;


//        bool Calculatpath = false;
//        bool reseted = false;
//        bool OneTimeMessage = false;

//        void Awake()
//        {
//            MakeSingleton();
//            FindEnemiesScript = GetComponent<FindEnemies>();
//        }
//        public void MakeSingleton()
//        {
//            instance = this;
//        }
//        private void Start()
//        {
//            TurretChild = GetComponent<Transform>();
//            FindEnemiesScript.FindingEnemies();
//            RandomiseTimeToSearch = Random.Range(MinimumTimeToSearchForClosestEnemy, MaximumTimeToSearchForClosetEnemy);
//            InvokeRepeating("FindClosestEnemyNow", 0, RandomiseTimeToSearch);
//            SaveDetectingDistance = DetectionRange;

//            gameObject.SendMessage("CheckforEnemy", EnableFieldOfView, SendMessageOptions.DontRequireReceiver);
//        }
//        public void EnemyView(bool View)
//        {
//            NoEnemyInView = View;
//        }
//        private void Update()
//        {
//            if (FindEnemiesScript.FindedEnemies == true)
//            {
//                if (FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy] != null && FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform.root.gameObject.transform != this.transform)
//                {
//                    dir = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - this.transform.position;
//                    RemoveEnemiesFromList = false;
//                    if (OneTimeMessage == false)
//                    {
//                        gameObject.SendMessage("CheckAiEnemyList", RemoveEnemiesFromList, SendMessageOptions.DontRequireReceiver);
//                        OneTimeMessage = true;
//                    }
//                    if (EnableFieldOfView == true)
//                    {
//                        Vector3 dir = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - this.transform.position;
//                        Angle = Vector3.Angle(dir, this.transform.root.transform.forward);

//                        if (EnemyTransform != FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform)
//                        {
//                            IsEnemyLocked = false;

//                        }
//                        else
//                        {
//                            IsEnemyLocked = true;
//                        }
//                    }
//                    else
//                    {
//                        IsEnemyLocked = true;
//                    }

//                    if (EnableFieldOfView == true && Angle < FieldOfViewAngle || IsEnemyLocked == true)
//                    {
//                        EnemyTransform = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].transform;
//                        IsEnemyLocked = true;
//                    }

//                    GetComponent<Targets>().DebugOtherTargetName = FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].gameObject.name;
//                    if (Vector3.Distance(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position, this.transform.position) < DetectionRange && NoEnemyInView == false)// && FOV.CanSeeTarget(EnableFieldOfView, FieldOfViewAngle, DetectionRange, reseted) && IsEnemyLocked == true)
//                    {


//                        StopCoroutine(StartDetecting());
//                        if (RotateXAxis == true)
//                        {
//                            TurretChild.transform.parent = this.gameObject.transform;
//                            Vector3 resetheMgparent = TurretParent.localEulerAngles;
//                            resetheMgparent.x = 0f;
//                            TurretParent.localEulerAngles = resetheMgparent;
//                            Quaternion MGTarget = Quaternion.LookRotation(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - TurretChild.position, TurretChild.up);
//                            TurretChild.rotation = Quaternion.RotateTowards(TurretChild.rotation, MGTarget, TurretRotationSpeed * Time.deltaTime);
//                            if (ClampXAxisRotationValues == true)
//                            {
//                                var MgangleX = ClampAngle.ClampAngles(TurretChild.localEulerAngles.x, MinimumXAxisRotation, MaximumXAxisRotation);
//                                TurretChild.localEulerAngles = new Vector3(MgangleX, TurretChild.localEulerAngles.y, 0);
//                            }
//                            else
//                            {
//                                TurretChild.localEulerAngles = new Vector3(TurretChild.localEulerAngles.x, TurretChild.localEulerAngles.y, 0);
//                            }

//                            if (RotateParentOnYAxis == true)
//                            {
//                                Quaternion MGTargets = Quaternion.LookRotation(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - TurretParent.position, TurretParent.up);
//                                TurretParent.rotation = Quaternion.RotateTowards(TurretParent.rotation, MGTargets, TurretRotationSpeed * Time.deltaTime);
//                                TurretParent.localEulerAngles = new Vector3(0, TurretParent.localEulerAngles.y, 0);
//                            }
//                        }
//                        else
//                        {
//                            if (RotateParentOnYAxis == true)
//                            {
//                                Quaternion MGTarget = Quaternion.LookRotation(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - TurretParent.position, TurretParent.up);
//                                TurretParent.rotation = Quaternion.RotateTowards(TurretParent.rotation, MGTarget, TurretRotationSpeed * Time.deltaTime);
//                                TurretParent.localEulerAngles = new Vector3(0, TurretParent.localEulerAngles.y, 0);
//                            }
//                            else
//                            {
//                                TurretChild.transform.parent = this.gameObject.transform;
//                                Quaternion MGTarget = Quaternion.LookRotation(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy].position - TurretChild.position, TurretChild.up);
//                                TurretChild.rotation = Quaternion.RotateTowards(TurretChild.rotation, MGTarget, TurretRotationSpeed * Time.deltaTime);
//                                if (ClampXAxisRotationValues == true)
//                                {
//                                    var MgangleX = ClampAngle.ClampAngles(TurretChild.localEulerAngles.x, MinimumXAxisRotation, MaximumXAxisRotation);
//                                    TurretChild.localEulerAngles = new Vector3(MgangleX, TurretChild.localEulerAngles.y, 0);
//                                }
//                                else
//                                {
//                                    TurretChild.localEulerAngles = new Vector3(TurretChild.localEulerAngles.x, TurretChild.localEulerAngles.y, 0);
//                                }
//                            }
//                        }
//                        ActivateFiringScript(true);
//                    }
//                    else
//                    {
//                        Detection();
//                    }
//                }
//                else
//                {
//                    FindImmediateEnemy();
//                    Detection();
//                }
//            }
//            else
//            {
//                Detection();

//            }

//            if (StartRotating == true)
//            {
//                if (StoreRotOnce == false)
//                {
//                    randomisation = Random.Range(MinimumSearchingAngle, MaximumSearchingAngle);
//                    StoreRotOnce = true;
//                }
//                Vector3 currentAngle = new Vector3(0f, Mathf.LerpAngle(transform.localEulerAngles.y, randomisation, SearchingSpeedAtY * Time.deltaTime), 0f);
//                transform.localEulerAngles = currentAngle;
//            }
//        }
//        public void Detection()
//        {
//            if (Rotatenow == false)
//            {
//                ActivateFiringScript(false);
//                StartCoroutine(StartDetecting());
//            }
//        }
//        public void FindImmediateEnemy()
//        {
//            if (RemoveEnemiesFromList == false)
//            {
//                FindEnemiesScript.enemy.Remove(FindEnemiesScript.enemy[FindEnemiesScript.CurrentEnemy]);
//                FindEnemiesScript.LockedTarget = 0;
//                gameObject.SendMessage("CheckAiEnemyList", RemoveEnemiesFromList, SendMessageOptions.DontRequireReceiver);
//                OneTimeMessage = false;
//                FindClosestEnemyNow();
//                RemoveEnemiesFromList = true;
//            }
//        }
//        public void ActivateFiringScript(bool Activate)
//        {
//            //for (int i = 0; i < FiringScripts.Length; i++)
//            //{
//            //    FiringScripts[i].FireNow = Activate;
//            //}
//        }
//        void FindClosestEnemy()
//        {
//            FindEnemiesScript.FindClosestEnemy();
//        }
//        public void FindNewEnemies()
//        {
//            FindEnemiesScript.FindingEnemies();
//        }
//        IEnumerator StartDetecting()
//        {
//            Rotatenow = true;
//            FindNewEnemies();
//            StartRotating = true;
//            StoreRotOnce = false;
//            yield return new WaitForSeconds(3f);
//            Rotatenow = false;
//            StartRotating = false;
//        }
//        public void FindClosestEnemyNow()
//        {
//            FindEnemiesScript.FindClosestEnemyNow(transform);
//            if (FindEnemiesScript.ContainThisTransform == true)
//            {
//                DetectionRange = 0;
//            }
//            else
//            {
//                DetectionRange = SaveDetectingDistance;
//            }
//        }
//    }
//}