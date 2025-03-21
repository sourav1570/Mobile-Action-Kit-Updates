using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class BulletScript : MonoBehaviour
    {

        [TextArea]
        public string ScriptInfo = "This script is used in to perform projectile shooting.";

        public bool UseRigidbody = false;

        [Tooltip("Layers that will get affected by the bullets")]
        public LayerMask AffectedLayers;

        [Tooltip(" If bullet didn`t hit anything and continues to fly in the world this will deactivate it after set time to reactivate it inside object pooler.")]
        public float TimeToDeactivate = 5f;

        [Tooltip("Choose the speed that would be appropriate for the real world projectile bullet that you trying to implement in your project.")]
        public float BulletSpeed = 20f;

        [Tooltip("Amount of the damage points this bullet will apply to targets.")]
        public float DamageToTarget = 5f;

        [Tooltip("This value should be tweaked in order for trail renderer to adequately display tracer bullet effect. ")]
        public float DelayTrailRendering = 1f;

        public bool EnableGuidedProjectile = false;

        [Tooltip("If The Bullet Hits On Other Colliders For Ex - Road , Houses etc. Than Which Prefab To Spawn From Object Pooler on that hit point - Prefab could be Ex - BulletHole")]
        public string ImpactEffectName = "BulletHole";

        public AlertingSoundActivator SoundManagerScript;
        //  Gunscript playergun;
        RaycastHit hit;

        public float TrajectoryDeclineSpeed = 0.01f;

        Rigidbody rb;
        TrailRenderer TR;

        float NormalSpeed;

        Vector3 PrevPosition;

        [HideInInspector]
        public bool UpdateKills = false;

        //  [HideInInspector]
        // public int StoreTeamId;

        public float ImpactEffectOffset = 0.004f;

        //public bool UseBulletSpread = true;
        //public float BulletSpreadXRotationValue = 0.4f;
        //public float BulletSpreadYRotationValue = 0.7f;
        //float ratio = 1;
        //float TargetX = 0;
        //float TargetY = 0;

        [HideInInspector]
        public bool CanSpread = false;

        Vector3 MyRotation;

        //public RaycastHit StoreCollider;

        //bool ShouldSpawnImpactEffect = true;
        Transform BulletManRootName;

        bool IsProjectileShootingActive = false;

        public ObjectPooler Pooler;

        [Tooltip("If checked than the force will be applied to humanoid Ai agent at the time of death using the fields provided below.")]
        public bool AddBulletForce = false;

        //[Tooltip("Minimum overall force to apply")]
        //public float MinBulletForce = 800;
        //[Tooltip("Maximum overall force to apply")]
        //public float MaxBulletForce = 1100;

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

        //public float NonAIImpactForce = 150f;

        public float AIMinBulletImpactForce = 8f;
       
        public float AIMaxBulletImpactForce = 8f;

        public float NonAIBulletImpactForce = 4f;

        public float RadiusToApplyForce = 50f;

        public PlayerForce PlayerBulletImpactForceOptions = PlayerForce.ShakePlayerCamera;

        [HideInInspector]
        public List<Transform> TargetsToApplyForce = new List<Transform>();

        [System.Serializable]
        public enum PlayerForce
        {
            ShakePlayerCamera,
            ApplyForceToPlayerRigidbody,
            ApplyForceToPlayerRigidbodyAndShakePlayerCamera,
            DoNotApplyForceToPlayer
        }

        public float PlayerBulletImpactForce = 0.5f;

        private float distanceTraveled;

        Transform ShootingPointFromWhereTheBulletSpawn;

        public void Movement(Transform RotToFollow, Transform rootName, bool IsChoosenProjectileShooting)
        {
            TargetsToApplyForce.Clear();
            BulletManRootName = rootName;
            //Debug.Break();
            //if (UseBulletSpread == true)
            //{
            //    transform.eulerAngles = Vector3.zero;
            //} 
            ShootingPointFromWhereTheBulletSpawn = RotToFollow;
            if (GetComponent<Collider>() != null)
            {
                GetComponent<Collider>().enabled = false;
            }

            NormalSpeed = BulletSpeed;
            // Debug.Break();
            if (EnableGuidedProjectile == true)
            {
                gameObject.transform.parent = RotToFollow;
            }
            //else
            //{
            //    StartCoroutine(QuickUnparent());
            //}
            //  StoreTeamId = playergun.StoreTeamId;
            if (UseRigidbody == true)
            {
                rb = transform.GetComponent<Rigidbody>();
                //  rb.useGravity = false; // need to remove it
                // Reset velocity and angular velocity
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                //rb.mass = rb.mass * 10f;
                rb.AddForce(transform.forward * NormalSpeed, ForceMode.Impulse);

                // rb.velocity = transform.forward * NormalSpeed * Time.deltaTime;
            }

            TR = GetComponent<TrailRenderer>();
            //   transform.localPosition = Vector3.zero;
            StartCoroutine(TrailTime());


            StartCoroutine(DestroyBulletAfterTime());
            PrevPosition = transform.position;

            if (FindObjectOfType<ObjectPooler>() != null)
            {
                Pooler = FindObjectOfType<ObjectPooler>();
            }

            //MyRotation = RotToFollow.eulerAngles;
            //transform.eulerAngles = MyRotation;

            IsProjectileShootingActive = IsChoosenProjectileShooting;
            //if (BulletManRootName.root.gameObject.tag != "Player")
            //{

            //}
        }
        //IEnumerator QuickUnparent()
        //{
        //    yield return new WaitForSeconds(0.001f);
        //    gameObject.transform.parent = null;
        //}
        IEnumerator TrailTime()
        {
            yield return new WaitForSeconds(DelayTrailRendering);
            TR.enabled = true;
        }
        private IEnumerator DestroyBulletAfterTime()
        {
            yield return new WaitForSeconds(TimeToDeactivate);
            //Debug.Break();
            NormalSpeed = BulletSpeed;
            TR.enabled = false;
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        }
        private void Update()
        {
            //  Debug.DrawRay(PrevPosition, (transform.position - PrevPosition).normalized * 1000f,Color.green);
            //transform.eulerAngles = playergun.transform.eulerAngles;
            // Debug.Break();
            //Comment made on 11/07/20
            //if (UseBulletSpread == true)
            //{
            //    if (CanSpread == false)
            //    {
            //        TargetX = Random.Range(BulletSpreadXRotationValue, BulletSpreadYRotationValue);
            //        TargetY = Random.Range(BulletSpreadXRotationValue, BulletSpreadYRotationValue);
            //        if (transform != null)
            //        {
            //            transform.rotation = Quaternion.Euler(transform.rotation.x + TargetX,transform.rotation.y + TargetY , transform.rotation.z);
            //        }
            //        CanSpread = true;
            //    }
            //}


            //if (playergun != null)
            //{
            //    //if (ChangeParent == true)
            //    //{
            //        //if(Doit == false)
            //        //{
            //    //        gameObject.transform.parent = null;
            //           //  PrevPosition = transform.position;
            //         //    Doit = true;  
            //       // }            
            //    //}       
            //    if (Gunscript.ShootingOptions.ProjectileShootingWithoutRaycast == playergun.ShootingTypes.ShootingOption)
            //    {
            //        PlayerRealisticShooting();
            //    }
            //    else
            //    {
            //    }        
            //}
            //else
            //{

            //}

            //  if(BulletTypes.RealisticBullet == BulletType)
            //  {
            if (IsProjectileShootingActive == true)
            {
                if(BulletManRootName != null)
                {
                    if (BulletManRootName.root.gameObject.tag == "Player")
                    {
                        PlayerRealisticShooting();
                    }
                    else
                    {
                        AiRealisticShooting();
                    }
                }
                else
                {
                    DeactivateAfterHit();
                }

            }


            //}
            if (UseRigidbody == false)
            {
                //   Debug.Break();
                if (NormalSpeed > 0)
                {
                    NormalSpeed -= Time.deltaTime;
                }
                else
                {
                    NormalSpeed = BulletSpeed;
                    TR.enabled = false;
                    transform.localPosition = Vector3.zero;
                    gameObject.SetActive(false);
                }


                //transform.position += transform.forward * NormalSpeed * Time.deltaTime;
                //transform.Translate(0f, -TrajectoryDeclineSpeed * Time.deltaTime, 0f);


                // Move the bullet forward
                transform.position += transform.forward * NormalSpeed * Time.deltaTime;

                // Update the distance traveled by the bullet
                distanceTraveled += NormalSpeed * Time.deltaTime;

                // Adjust the decline speed based on the distance traveled
                float declineSpeed = CalculateDeclineSpeed(distanceTraveled);

                // Update the position of the bullet with the adjusted decline speed
                transform.Translate(0f, -declineSpeed * Time.deltaTime, 0f);
            }

        }
        private float CalculateDeclineSpeed(float distance)
        {
            // Calculate the decline speed based on the distance traveled
            // You can define any formula here to adjust the decline speed as the bullet travels further
            // For example, you can make it proportional to the distance traveled
            float declineSpeed = TrajectoryDeclineSpeed + (distance / 1000f); // Adjust 1000f as needed

            return declineSpeed;
        }
        //public void AddingForceToHumanoidAI(RaycastHit hit)
        //{
        //    if(AddBulletForce == true)
        //    {
        //        if (hit.transform.root.GetComponent<HumanoidAiHealth>() != null)
        //        {
        //            if (hit.transform.root.GetComponent<HumanoidAiHealth>().Health <= 0)
        //            {
        //                foreach (Transform g in hit.transform.gameObject.transform.root.GetComponentsInChildren<Transform>())
        //                {
        //                    if (g.gameObject.GetComponent<Rigidbody>() != null)
        //                    {
        //                        g.gameObject.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(MinBulletForce, MaxBulletForce), hit.transform.gameObject.transform.root.position + ForceDirection, BulletForceRadius);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //}
        //public void AddingForceToHumanoidAI(RaycastHit hit)
        //{
        //    if (AddBulletForce)
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
        //                        // Calculate the force direction based on the specified offsets
        //                        Vector3 forceDirection = Random.Range(MinUpwardForceToApplyOnTarget,MaxUpwardForceToApplyOnTarget) * Vector3.up +
        //                                                Random.Range(MinRightForceToApplyOnTarget, MaxRightForceToApplyOnTarget) * Vector3.right +
        //                                                Random.Range(MinLeftForceToApplyOnTarget, MaxLeftForceToApplyOnTarget) * Vector3.left +
        //                                                Random.Range(MinBackwardForceToApplyOnTarget, MaxBackwardForceToApplyOnTarget) * Vector3.back +
        //                                                 Random.Range(MinForwardForceToApplyOnTarget, MaxForwardForceToApplyOnTarget) * Vector3.forward;


        //                        // Apply force in the calculated direction
        //                        rb.AddForce(forceDirection.normalized * Random.Range(MinBulletForce, MaxBulletForce));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        public void AddForceToRigidBodies(RaycastHit hit, Transform RaycastToStartFrom) // Logic is updated here because the bullet is movable so we can't do proper check with direction which is one closest to apply force too.
        {                                                                               // as in one frame the bullet get spawn and in other frame the bullet may hit and cross the AI agent body so from there the closet point will be
                                                                                        // AI agent right direction even though the shot was to the left body part so this could result in pushing AI agent to wrong direction
                                                                                        // and to fix this we make sure to do distance comparsion check to the point where the bullet is spawned i.e 'Shooting Point'
                                                                                      
            //Vector3 directionToTarget = hit.transform.position - RaycastToStartFrom.position; // Grenade -> Target direction
           // Debug.DrawLine(RaycastToStartFrom.position, RaycastToStartFrom.position + RaycastToStartFrom.forward * directionToTarget.magnitude, Color.red, 0.1f); 

            if (hit.transform.root.tag == "AI" && !TargetsToApplyForce.Contains(hit.transform.root)) // AI Logic
            {
                if (hit.transform.root.GetComponent<HumanoidAiHealth>() != null)
                {
                    if (hit.transform.root.GetComponent<HumanoidAiHealth>().Health <= 0)
                    {
                        if (hit.transform.root.GetComponent<Animator>() != null)
                        {
                            hit.transform.root.GetComponent<Animator>().enabled = false;
                        }

                        TargetsToApplyForce.Add(hit.transform.root);
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
                                float distanceFactor = Mathf.Clamp01(1 - (distanceToGrenade / RadiusToApplyForce)); // Normalize effect
                                float forceMagnitude = Mathf.Lerp(AIMinBulletImpactForce, AIMaxBulletImpactForce, distanceFactor);

                                // Apply the force
                                rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);

                                // Debugging information
                                //Debug.Log($"{g.gameObject.name} - Closest Direction: {closestDirection}, Force Applied: {forceDirection}, Magnitude: {forceMagnitude}");
                            }
                        }
                    }
                }

            }
            else if (hit.transform.root.tag == "Player" && PlayerBulletImpactForceOptions == PlayerForce.ShakePlayerCamera)
            {

                if (hit.transform.root.GetComponent<CameraShakerEffect>() != null)
                {
                    hit.transform.root.GetComponent<CameraShakerEffect>().Shake();
                }
            }
            else if (hit.transform.root.tag == "Player" && PlayerBulletImpactForceOptions == PlayerForce.ApplyForceToPlayerRigidbody)
            {

                if (hit.transform.GetComponent<Rigidbody>() != null)
                {

                    Rigidbody rb = hit.transform.gameObject.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                    rb.AddForce(-hit.normal * PlayerBulletImpactForce, ForceMode.Impulse);
                }

            }
            else if (hit.transform.root.tag == "Player" && PlayerBulletImpactForceOptions == PlayerForce.ApplyForceToPlayerRigidbodyAndShakePlayerCamera)
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

                    rb.AddForce(-hit.normal * PlayerBulletImpactForce, ForceMode.Impulse);
                }
            }
            else if (hit.transform.root.tag == "Player" && PlayerBulletImpactForceOptions == PlayerForce.DoNotApplyForceToPlayer)
            {
                // Don't do anything here 
            }
            else // Non-AI Logic
            {
                if (!TargetsToApplyForce.Contains(hit.transform))
                {
                    //foreach (Transform g in hit.transform.root.GetComponentsInChildren<Transform>())
                    //{
                        if (hit.transform.GetComponent<Rigidbody>() != null)
                        {
                            TargetsToApplyForce.Add(hit.transform);
                            Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                            rb.isKinematic = false;
                            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                            // Non-AI: Add force in explosion normal direction
                            rb.AddForce(-hit.normal * NonAIBulletImpactForce, ForceMode.Impulse);
                        }
                    //}
                }
            }

        }
        //public void AddingForceToHumanoidAI(RaycastHit hit)
        //{
        //    if (AddBulletForce)
        //    {
        //        if (hit.transform.root.GetComponent<HumanoidAiHealth>() != null)
        //        {
        //            if (hit.transform.root.GetComponent<HumanoidAiHealth>().Health <= 0 && hit.transform.root.GetComponent<HumanoidAiHealth>().AddForceToHumanoidAi == false)
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
        //                        //Vector3 forceDirectionWorld = Random.Range(MinUpwardForceToApplyOnTarget, MaxUpwardForceToApplyOnTarget) * Vector3.up +
        //                        //                            Random.Range(MinRightForceToApplyOnTarget, MaxRightForceToApplyOnTarget) * Vector3.right +
        //                        //                            Random.Range(MinLeftForceToApplyOnTarget, MaxLeftForceToApplyOnTarget) * Vector3.left +
        //                        //                            Random.Range(MinBackwardForceToApplyOnTarget, MaxBackwardForceToApplyOnTarget) * Vector3.back +
        //                        //                            Random.Range(MinForwardForceToApplyOnTarget, MaxForwardForceToApplyOnTarget) * Vector3.forward;

        //                        //// Transform the force direction from world space to local space
        //                        //Vector3 forceDirectionLocal = rb.transform.TransformVector(forceDirectionWorld);

        //                        //// Apply force in the calculated local direction
        //                        //rb.AddForce(forceDirectionLocal.normalized * Random.Range(MinBulletForce, MaxBulletForce));


        //                        // Calculate and apply the upward force in world space
        //                        Vector3 upwardForceWorld = Random.Range(MinUpwardForceToApplyOnTarget, MaxUpwardForceToApplyOnTarget) * Vector3.up;
        //                        rb.AddForce(upwardForceWorld * Random.Range(MinBulletForce, MaxBulletForce));

        //                        // Calculate and apply other forces in local space
        //                        Vector3 forceDirectionLocal = Random.Range(MinRightForceToApplyOnTarget, MaxRightForceToApplyOnTarget) * Vector3.right +
        //                                                    Random.Range(MinLeftForceToApplyOnTarget, MaxLeftForceToApplyOnTarget) * Vector3.left +
        //                                                    Random.Range(MinBackwardForceToApplyOnTarget, MaxBackwardForceToApplyOnTarget) * Vector3.back +
        //                                                    Random.Range(MinForwardForceToApplyOnTarget, MaxForwardForceToApplyOnTarget) * Vector3.forward;

        //                        rb.AddRelativeForce(forceDirectionLocal * Random.Range(MinBulletForce, MaxBulletForce));
        //                    }
        //                }
        //                hit.transform.root.GetComponent<HumanoidAiHealth>().AddForceToHumanoidAi = true;
        //            }
        //        }
        //        else
        //        {
        //            foreach (Transform g in hit.transform.gameObject.transform.root.GetComponentsInChildren<Transform>())
        //            {
        //                Rigidbody rb = g.gameObject.GetComponent<Rigidbody>();
        //                if (rb != null)
        //                {
        //                    rb.AddForce(-hit.normal * NonAIImpactForce);
        //                    //// Calculate and apply the upward force in world space
        //                    //Vector3 upwardForceWorld = Random.Range(MinUpwardForceToApplyOnTarget, MaxUpwardForceToApplyOnTarget) * Vector3.up;
        //                    //rb.AddForce(upwardForceWorld * Random.Range(MinBulletForce, MaxBulletForce));

        //                    //// Calculate and apply other forces in local space
        //                    //Vector3 forceDirectionLocal = Random.Range(MinRightForceToApplyOnTarget, MaxRightForceToApplyOnTarget) * Vector3.right +
        //                    //                            Random.Range(MinLeftForceToApplyOnTarget, MaxLeftForceToApplyOnTarget) * Vector3.left +
        //                    //                            Random.Range(MinBackwardForceToApplyOnTarget, MaxBackwardForceToApplyOnTarget) * Vector3.back +
        //                    //                            Random.Range(MinForwardForceToApplyOnTarget, MaxForwardForceToApplyOnTarget) * Vector3.forward;

        //                    //rb.AddRelativeForce(forceDirectionLocal * Random.Range(MinBulletForce, MaxBulletForce));
        //                }
        //            }
        //        }
        //    }
        //}



        public void PlayerRealisticShooting()
        {
            if (Physics.Raycast(PrevPosition, (transform.position - PrevPosition).normalized, out hit, (transform.position - PrevPosition).magnitude, AffectedLayers))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.transform.root.tag == "AI" && hit.transform.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
                    {
                        hit.collider.gameObject.transform.root.SendMessage("FindColliderName", hit.collider.name, SendMessageOptions.DontRequireReceiver);
                        if (hit.collider.gameObject.transform.tag != "WeakPoint")
                        {
                            hit.collider.gameObject.transform.root.SendMessage("Takedamage", DamageToTarget, SendMessageOptions.DontRequireReceiver);
                        }
                        else
                        {
                            hit.collider.gameObject.transform.root.SendMessage("WeakPointdamage", DamageToTarget, SendMessageOptions.DontRequireReceiver);
                        }
                        hit.collider.gameObject.transform.root.SendMessage("Effects", hit, SendMessageOptions.DontRequireReceiver);
                       
                  
                        if (FriendlyFire.instance != null)
                        {
                            if (hit.collider.gameObject.transform.root.GetComponent<Targets>() != null)
                            {
                                if (hit.collider.gameObject.transform.root.GetComponent<Targets>().MyTeamID == BulletManRootName.GetComponent<Targets>().MyTeamID)
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
                    else if (hit.transform.root.tag == "Turret")
                    {
                        if (hit.transform.root.GetComponent<Turret>() != null)
                        {
                            hit.transform.root.GetComponent<Turret>().TakeDamage(DamageToTarget);
                        }
                    }
                    else if (hit.collider.gameObject.tag == "Target")
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

                                // Keep the below lines to be commented because during emergency state based on the selected properties it should affect the other Humanoid AI agent and not only friendlies
                                // as when you overrite the properties it affects the behaviour when you change promatically that only friendlies will be affected using below code.
                                // This may create a bug when you shoot the enemies and impact effects
                                // are spawned with the settings being force emergency state on AI agent.so Make sure you don't use this code at all even in the case of sounds. for more details why not to use you can change
                                // impact effect properties to be Force Emergency state and choose All teams and shoot near the enemies using player weapon by uncommenting this code.

                               // impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.Enemy = BulletManRootName;
                               // impacteffect.gameObject.GetComponent<ImpactEffect>().TeamWhichWillBeAffectedByTheShot(BulletManRootName.transform.root.GetComponent<Targets>().MyTeamID);
                                impacteffect.gameObject.GetComponent<ImpactEffect>().EffectActivation(BulletManRootName);
                            }

                            // the problem when you make it parent is that if that parent is supposed to be destroy ( for example grenade explosion gameobject )s
                            // in the game it destroy the effect which was being used by the object pooler due to which the condition becomes null and throws error 
                            //if (impacteffect != null)
                            //{
                            //    impacteffect.transform.parent = hit.transform;
                            //}
                        }
                        else
                        {
                            if (Pooler != null)
                            {
                                GameObject impacteffect = Pooler.SpawnFromPool(ImpactEffectName, hit.point, Quaternion.LookRotation(hit.normal));
                                if (impacteffect.GetComponent<AudioSource>() != null)
                                {
                                    if (!impacteffect.GetComponent<AudioSource>().isPlaying)
                                    {
                                        impacteffect.GetComponent<AudioSource>().Play();
                                    }

                                }

                                if (impacteffect.gameObject.GetComponent<ImpactEffect>() != null)
                                {
                                    // Keep the below lines to be commented because during emergency state based on the selected properties it should affect the other Humanoid AI agent and not only friendlies
                                    // as when you overrite the properties it affects the behaviour when you change promatically that only friendlies will be affected using below code.
                                    // This may create a bug when you shoot the enemies and impact effects
                                    // are spawned with the settings being force emergency state on AI agent.so Make sure you don't use this code at all even in the case of sounds. for more details why not to use you can change
                                    // impact effect properties to be Force Emergency state and choose All teams and shoot near the enemies using player weapon by uncommenting this code.

                                    //impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.Enemy = BulletManRootName;
                                    //impacteffect.gameObject.GetComponent<ImpactEffect>().TeamWhichWillBeAffectedByTheShot(BulletManRootName.transform.root.GetComponent<Targets>().MyTeamID);
                                    impacteffect.gameObject.GetComponent<ImpactEffect>().EffectActivation(BulletManRootName);
                                }


                                // the problem when you make it parent is that if that parent is supposed to be destroy (for example grenade explosion gameobject)
                                // in the game it destroy the effect which was being used by the object pooler due to which the condition becomes null and throws error 
                                //if (impacteffect != null)
                                //{
                                //    impacteffect.transform.parent = hit.transform;
                                //}

                            }
                        }
                        //if (hit.collider.gameObject.GetComponent<HitImpactEffect>() != null)
                        //{
                        //    if (hit.collider.gameObject.GetComponent<HitImpactEffect>().DeactivateBulletOnCollision == true)
                        //    {
                        //        DeactivateAfterHit();
                        //    }
                        //    else
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
                        //        StoreCollider = hit;
                        //        if (hit.collider.gameObject.GetComponent<HitImpactEffect>().SpawnImpactEffectOnSurfacesBehindThisOne == false)
                        //        {
                        //            ShouldSpawnImpactEffect = false;
                        //        }
                        //    }
                        //}
                        //else
                        //{
                           // DeactivateAfterHit();
                        //}
                    }
                    DeactivateAfterHit();
                    if (AddBulletForce == true)
                    {
                        AddForceToRigidBodies(hit, ShootingPointFromWhereTheBulletSpawn);
                    }
                }

            }
        }
        public void AiRealisticShooting()
        {
            if (Physics.Raycast(PrevPosition, (transform.position - PrevPosition).normalized, out hit, (transform.position - PrevPosition).magnitude, AffectedLayers))
            {
                if (hit.collider != null)
                {

                    if (hit.transform.root.tag == "Player")
                    {
                        // BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.GivingfriendlyBotPosOnShotHeard();
                        if(BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>() != null)
                        {
                            BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.IsFiring = true;
                        }

                        
                        PlayerHealth.instance.PlayerHealthbar.Curvalue -= DamageToTarget;
                        PlayerHealth.instance.CheckPlayerHealth();

                        if (TeamMatch.instance != null)
                        {
                            if (TeamMatch.instance.EnableScoreSystemBetweenTeamsAsWinCondition == true)
                            {
                                if (PlayerHealth.instance.PlayerHealthbar.Curvalue <= 0)
                                {
                                    if (UpdateKills == false)
                                    {
                                        TeamMatch.instance.Teams[BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.StoreTeamId].Kills += 1;
                                        UpdateKills = true;
                                    }
                                }
                                else
                                {
                                    int StoreID = BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.StoreTeamId;
                                    TeamMatch.instance.Teams[StoreID].TeamScore += TeamMatch.instance.SingleShotPoints;
                                    TeamMatch.instance.Teams[StoreID].ScoreText.text = TeamMatch.instance.Teams[StoreID].TeamName + " - " + TeamMatch.instance.Teams[StoreID].TeamScore;
                                    UpdateKills = false;
                                }
                            }
                        }

                        

                    }
                    else if (hit.transform.root.tag == "Turret")
                    {
                        if (hit.transform.root.GetComponent<Turret>() != null)
                        {
                            hit.transform.root.GetComponent<Turret>().TakeDamage(DamageToTarget);
                        }
                    }
                    else if (hit.collider.gameObject.transform.root.tag == "AI" && hit.transform.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
                    {
                        if(hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
                        {
                            hit.collider.gameObject.transform.root.SendMessage("FindColliderName", hit.collider.name, SendMessageOptions.DontRequireReceiver);
                        }

                        //if(BulletManRootName.transform.tag == "AI")
                        //{
                        if (hit.collider.gameObject.transform.tag != "WeakPoint" && hit.transform.root.GetComponent<Targets>().MyTeamID != BulletManRootName.gameObject.GetComponent<Targets>().MyTeamID)
                        {
                            //BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.GivingfriendlyBotPosOnShotHeard();
                            if(BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>() != null)
                            {
                                BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.IsFiring = true;
                            }

                            if (hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
                            {
                                hit.collider.gameObject.transform.root.SendMessage("FindColliderName", hit.collider.name, SendMessageOptions.DontRequireReceiver);
                                hit.collider.gameObject.transform.root.SendMessage("Takedamage", DamageToTarget, SendMessageOptions.DontRequireReceiver);
                            }

                            if (hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
                            {
                                if (BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>() != null)
                                {
                                    hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.WhoKilledMe = BulletManRootName;
                                    hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.WhoShootingMe = BulletManRootName;

                                    if (BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().HealthScript.IsDied == true)
                                    {
                                        hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.DoNotActivateSoundTrigger = true;
                                    }
                                }
                                else
                                {
                                    if(hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
                                    {
                                        hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.DoNotActivateSoundTrigger = true;
                                    }
                                }
                            }


                            if (hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
                            {
                                hit.collider.gameObject.transform.root.SendMessage("Effects", hit, SendMessageOptions.DontRequireReceiver);
                            }

                            if(BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>() != null)
                            {
                                if (BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().HealthScript.IsDied == true)
                                {
                                    BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.PointsChecker();
                                }
                            }
                           
                            //   hit.collider.gameObject.transform.root.SendMessage("RemoveFromListOfEnemies", transform.root, SendMessageOptions.DontRequireReceiver);
                        }
                        else if (hit.collider.transform.tag == "WeakPoint" && hit.transform.root.GetComponent<Targets>().MyTeamID != BulletManRootName.gameObject.GetComponent<Targets>().MyTeamID)
                        {
                            // BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.GivingfriendlyBotPosOnShotHeard();

                            if (BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>() != null)
                            {
                                if (BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().HealthScript.IsDied == true)
                                {
                                    BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.IsFiring = true;
                                }
                            }

                            if (hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
                            {
                                hit.collider.gameObject.transform.root.SendMessage("FindColliderName", hit.collider.name, SendMessageOptions.DontRequireReceiver);
                                hit.collider.gameObject.transform.root.SendMessage("WeakPointdamage",DamageToTarget, SendMessageOptions.DontRequireReceiver);
                            }
                            if (hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
                            {
                                if (BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>() != null)
                                {
                                    hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.WhoKilledMe = BulletManRootName;
                                    hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.WhoShootingMe = BulletManRootName;

                                    if (BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().HealthScript.IsDied == true)
                                    {
                                        hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.DoNotActivateSoundTrigger = true;
                                    }
                                }
                                else
                                {
                                    if (hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.IsDied == true)
                                    {
                                        hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.DoNotActivateSoundTrigger = true;
                                    }
                                }
                            }

                            if (hit.collider.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
                            {
                                hit.collider.gameObject.transform.root.SendMessage("Effects", hit, SendMessageOptions.DontRequireReceiver);
                            }

                            if (BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>() != null)
                            {
                                if (BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().HealthScript.IsDied == true)
                                {
                                    BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.PointsChecker();
                                }
                            }
                            // hit.collider.gameObject.transform.root.SendMessage("RemoveFromListOfEnemies", transform.root, SendMessageOptions.DontRequireReceiver);
                        }
                        //}
                        //else
                        //{
                        //    if (hit.collider.gameObject.transform.tag != "WeakPoint" && hit.transform.root.GetComponent<Targets>().FriendlyTeamTag != BulletManRootName.gameObject.GetComponent<Targets>().FriendlyTeamTag)
                        //    {
                        //        BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.GivingfriendlyBotPosOnShotHeard();
                        //        BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.IsFiring = true;
                        //        hit.collider.gameObject.transform.root.SendMessage("FindColliderName", hit.collider.name, SendMessageOptions.DontRequireReceiver);
                        //        hit.collider.gameObject.transform.root.SendMessage("Takedamage", BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.ShootingFeatures.DamageToTarget, SendMessageOptions.DontRequireReceiver);
                        //        hit.collider.gameObject.transform.root.SendMessage("Effects", BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.ShootingFeatures.hit, SendMessageOptions.DontRequireReceiver);
                        //        BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.PointsChecker();
                        //        //   hit.collider.gameObject.transform.root.SendMessage("RemoveFromListOfEnemies", transform.root, SendMessageOptions.DontRequireReceiver);
                        //    }
                        //    else if (hit.collider.transform.tag == "WeakPoint" && hit.transform.root.GetComponent<Targets>().FriendlyTeamTag != BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.nametolookfor)
                        //    {
                        //        BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.GivingfriendlyBotPosOnShotHeard();
                        //        BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.IsFiring = true;
                        //        hit.collider.gameObject.transform.root.SendMessage("FindColliderName", hit.collider.name, SendMessageOptions.DontRequireReceiver);
                        //        hit.collider.gameObject.transform.root.SendMessage("WeakPointdamage", BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.ShootingFeatures.DamageToTarget, SendMessageOptions.DontRequireReceiver);
                        //        hit.collider.gameObject.transform.root.SendMessage("Effects", BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.ShootingFeatures.hit, SendMessageOptions.DontRequireReceiver);
                        //        BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.PointsChecker();
                        //        // hit.collider.gameObject.transform.root.SendMessage("RemoveFromListOfEnemies", transform.root, SendMessageOptions.DontRequireReceiver);
                        //    }
                        //}


                         
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

                                if (BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>() != null)
                                {
                                    if (BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().HealthScript.IsDied == true)
                                    {
                                        impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.DoNotActivateEffect = true;
                                    }
                                }
                                else
                                {
                                    impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.DoNotActivateEffect = true;
                                }

                                // Keep the below lines to be commented because during emergency state based on the selected properties it should affect the other Humanoid AI agent and not only friendlies
                                // as when you overrite the properties it affects the behaviour when you change promatically that only friendlies will be affected using below code.
                                // This may create a bug when you shoot the enemies and impact effects
                                // are spawned with the settings being force emergency state on AI agent.so Make sure you don't use this code at all even in the case of sounds. for more details why not to use you can change
                                // impact effect properties to be Force Emergency state and choose All teams and shoot near the enemies using player weapon by uncommenting this code.

                                //impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.Enemy = BulletManRootName;
                                // impacteffect.gameObject.GetComponent<ImpactEffect>().TeamWhichWillBeAffectedByTheShot(BulletManRootName.transform.root.GetComponent<Targets>().MyTeamID);
                                impacteffect.gameObject.GetComponent<ImpactEffect>().EffectActivation(BulletManRootName);
                            }

                            // the problem when you make it parent is that if that parent is supposed to be destroy ( for example grenade explosion gameobject )
                            // in the game it destroy the effect which was being used by the object pooler due to which the condition becomes null and throws error 
                            //if (impacteffect != null)
                            //{
                            //    impacteffect.transform.parent = hit.transform;
                            //}
                        }
                        else
                        {
                            if (Pooler != null)
                            {
                                //Debug.Break();
                                GameObject impacteffect = Pooler.SpawnFromPool(ImpactEffectName, hit.point, Quaternion.LookRotation(hit.normal));
                                if (impacteffect.GetComponent<AudioSource>() != null)
                                {
                                    if (!impacteffect.GetComponent<AudioSource>().isPlaying)
                                    {
                                        impacteffect.GetComponent<AudioSource>().Play();
                                    }

                                }
                                if (impacteffect.gameObject.GetComponent<ImpactEffect>() != null)
                                {
                                    if (BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>() != null)
                                    {
                                        if (BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().HealthScript.IsDied == true)
                                        {
                                            impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.DoNotActivateEffect = true;
                                        }
                                    }
                                    else
                                    {
                                        impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.DoNotActivateEffect = true;
                                    }

                                    // Keep the below lines to be commented because during emergency state based on the selected properties it should affect the other Humanoid AI agent and not only friendlies
                                    // as when you overrite the properties it affects the behaviour when you change promatically that only friendlies will be affected using below code.
                                    // This may create a bug when you shoot the enemies and impact effects
                                    // are spawned with the settings being force emergency state on AI agent.so Make sure you don't use this code at all even in the case of sounds. for more details why not to use you can change
                                    // impact effect properties to be Force Emergency state and choose All teams and shoot near the enemies using player weapon by uncommenting this code.

                                    // impacteffect.gameObject.GetComponent<ImpactEffect>().AlertingSoundScript.AlertingSoundScript.Enemy = BulletManRootName;
                                    // impacteffect.gameObject.GetComponent<ImpactEffect>().TeamWhichWillBeAffectedByTheShot(BulletManRootName.transform.root.GetComponent<Targets>().MyTeamID);
                                    impacteffect.gameObject.GetComponent<ImpactEffect>().EffectActivation(BulletManRootName);
                                }

                                // the problem when you make it parent is that if that parent is supposed to be destroy ( for example grenade explosion gameobject )
                                // in the game it destroy the effect which was being used by the object pooler due to which the condition becomes null and throws error 

                                //if (impacteffect != null)
                                //{
                                //    impacteffect.transform.parent = hit.transform;
                                //}

                            }
                        }
                        //if (hit.collider.gameObject.GetComponent<HitImpactEffect>() != null)
                        //{
                        //    if (hit.collider.gameObject.GetComponent<HitImpactEffect>().DeactivateBulletOnCollision == true)
                        //    {
                        //        DeactivateAfterHit();
                        //    }
                        //    else
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
                        //        StoreCollider = hit;
                        //        if (hit.collider.gameObject.GetComponent<HitImpactEffect>().SpawnImpactEffectOnSurfacesBehindThisOne == false)
                        //        {
                        //            ShouldSpawnImpactEffect = false;
                        //        }
                        //    }
                        //}
                        //else
                        //{
                          //  DeactivateAfterHit();
                        //}
                    }
                    //else
                    //{
                    //    BulletManRootName.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.IsFiring = false;
                     DeactivateAfterHit();
                    //}
                    if(AddBulletForce == true)
                    {
                        AddForceToRigidBodies(hit, ShootingPointFromWhereTheBulletSpawn);
                    }
                }
            }
        }
        public void DeactivateAfterHit()
        {
            // Debug.Break();
            NormalSpeed = BulletSpeed;
            TR.enabled = false;
            //if (StoreCollider.collider != null)
            //{
            //    if (StoreCollider.collider.GetComponent<MeshCollider>() != null)
            //    {
            //        StoreCollider.collider.isTrigger = false;
            //        StoreCollider.collider.GetComponent<MeshCollider>().convex = false;

            //    }
            //    else
            //    {
            //        StoreCollider.collider.isTrigger = false;
            //    }
            //    ShouldSpawnImpactEffect = true;
            //}

            if (rb != null)
            {
                rb.velocity = new Vector3(0f, 0f, 0f);
                rb.angularVelocity = new Vector3(0f, 0f, 0f);
            }

            //transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        }

    }
}