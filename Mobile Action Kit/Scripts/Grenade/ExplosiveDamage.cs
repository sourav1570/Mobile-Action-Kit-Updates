using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class ExplosiveDamage : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script is responsible for explosion behaviour of the grenade and also thus notify other humanoid Ai agent's about incoming grenade." +
            "This script is responsible for grenade damage behaviour to other entites in game like - Player or Ai ";

        [Tooltip("Drag and drop 'ExplosiveDevice' script from the hierarchy into this field.")]
        public ExplosiveDevice ExplosiveDeviceComponent;

        [Tooltip("Specify the damage the grenade will give to other entities if touched by this trigger. In case target is behind a wall or cover than this value will not be used.")]
        public float Damage = 100f;

        public bool AddRaycastForce = true;
        [Tooltip("Minimum explosive force value will be applied to other Humanoid Ai agent on grenade explosion. the higher the value is the higher the humanoid Ai agent will fly if get fall into the " +
            "grenade trap. For the player this value is not true but you can add camera shaker effect script to the camera of the player and tweak the values there which will in return going to shake the camera of the player.")]
        public float AIMinExplosiveForce = 5f;
        [Tooltip("Maximum explosive force value will be applied to other Humanoid Ai agent on grenade explosion. the higher the value is the higher the humanoid Ai agent will fly if get fall into the " +
            "grenade trap. For the player this value is not true but you can add camera shaker effect script to the camera of the player and tweak the values there which will in return going to shake the camera of the player.")]
        public float AIMaxExplosiveForce = 7f;

        public float NonAIExplosiveForce = 7f;

        public float RadiusToApplyForce = 10f;

        public PlayerForce PlayerExplosiveForce = PlayerForce.ShakePlayerCamera;

        public LayerMask LayersToApplyRaycastForce;

        [System.Serializable]
        public enum PlayerForce
        {
            ShakePlayerCamera,
            ApplyForceToPlayerRigidbody,
            ApplyForceToPlayerRigidbodyAndShakePlayerCamera,
            DoNotApplyForceToPlayer
        }

        //[Tooltip("explosive radius value will be applied to other Humanoid Ai agent on grenade explosion if they are within the explosion radius.the higher the value is the higher the humanoid Ai agent will fly if get fall into the " +
        //    "grenade trap. For the player this value is not true but you can add camera shaker effect script to the camera of the player and tweak the values there which will in return going to shake the camera of the player.")]
        //public float ExplosionRadius = 10f;

        [Tooltip("Time to deactivate the grenade damage script.")]
        public float TimeToDeactive = 0.5f;

        [HideInInspector]
        public List<Transform> MovableAi = new List<Transform>();
        bool CanShake = false;
        bool DoneWithExplosion = false;

        [Tooltip("If enabled than before explosion this script will draw a raycast from the origin point to the target destination.")]
        public bool DebugRaycastFromOriginToTargetPosition = true;

        [Tooltip("Drag and drop the enemy raycaster gameObject from the hierarchy into this field.")]
        public Transform EnemyRaycaster;
        [Tooltip("Drag and drop the object detector gameObject from the hierarchy into this field.")]
        public Transform ObjectDetector;
        [Tooltip("Specify the layers to be ignored when raycasting using 'EnemyRaycaster' and 'ObjectDetector'.")]
        public LayerMask IgnoredLayers;
        [Tooltip("If enabled than either the 'MinDamageBehindCover' will be applied or the 'MaxDamageBehindCover' on grenade explosion.If disabled than only 'MinDamageBehindCover' will be applied")]
        public bool RandomiseDamageBehindCover = true;
        [Tooltip("Specify the Minimum Damage to give to the target behind cover.")]
        public float MinDamageBehindCover;
        [Tooltip("Specify the Maximum Damage to give to the target behind cover.")]
        public float MaxDamageBehindCover; 

        bool CompleteDamage = false;

        float ActualDamage;

        HealthReduction HealthReductionComponent;

        private void Start()
        {
            if(GetComponent<HealthReduction>() != null)
            {
                HealthReductionComponent = GetComponent<HealthReduction>();
            }
            ActualDamage = Damage;
        }
        private void OnTriggerEnter(Collider col)
        {
            if (col.transform.root.CompareTag("AI") && col.transform.root.GetComponent<HumanoidAiHealth>() != null && col.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
            {
                // important to add a list because if we hit any other part of the Ai agent like hands or spine the calculations will be done again so we doing it only once if we hit any collider exist in Ai agent.
                if (!MovableAi.Contains(col.transform.root) && !ExplosiveDeviceComponent.EnemiesGotHitByDamage.Contains(col.transform.root))
                {
                    if (HealthReductionComponent == null)
                    {
                        ObjectDetection(col.transform.root.GetComponent<Targets>().MyBodyPartToTarget);
                        CalculateDamage();
                        col.gameObject.transform.root.SendMessage("Takedamage", ActualDamage, SendMessageOptions.DontRequireReceiver);
                        if (col.transform.root.GetComponent<HumanoidAiHealth>() != null)
                        {
                            if (col.transform.root.GetComponent<HumanoidAiHealth>().Health <= 0)
                            {
                                //foreach (Transform g in col.gameObject.transform.root.GetComponentsInChildren<Transform>())
                                //{
                                //    if (g.gameObject.GetComponent<Rigidbody>() != null)
                                //    {
                                //        g.gameObject.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(MinExplosiveForce, MaxExplosiveForce), col.gameObject.transform.root.position, ExplosionRadius);
                                //    }
                                //}

                                if (col.transform.root.GetComponent<Animator>() != null)
                                {
                                    col.transform.root.GetComponent<Animator>().enabled = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (col.transform.root.GetComponent<HumanoidAiHealth>() != null)
                        {
                            col.transform.root.GetComponent<HumanoidAiHealth>().StartDamageReduction(HealthReductionComponent.HealthReductionDuration, Damage);
                            col.transform.root.GetComponent<HumanoidAiHealth>().MaterialColorToChange(HealthReductionComponent.MaterialColorToChange, HealthReductionComponent.HealthReductionDuration);
                        }
                    }
                 
                  
                   
                    MovableAi.Add(col.transform.root);
                    ExplosiveDeviceComponent.EnemiesGotHitByDamage.Add(col.transform.root);
                }
            }
            if (col.transform.root.CompareTag("Player"))
            {
                if (CanShake == false)
                {
                    if(col.transform.root.GetComponent<Targets>() != null)
                    {
                        ObjectDetection(col.transform.root.GetComponent<Targets>().MyBodyPartToTarget);
                    }
                    else
                    {
                        ObjectDetection(col.gameObject.transform.root);
                    }
                    CalculateDamage();
                    PlayerHealth.instance.PlayerHealthbar.Curvalue -= ActualDamage;
                    PlayerHealth.instance.CheckPlayerHealth();
                   
                    CanShake = true;
                }

            }
            if (col.transform.root.CompareTag("AI") && col.transform.root.GetComponent<HumanoidAiHealth>() == null)
            {
                if (!MovableAi.Contains(col.transform.root) && !ExplosiveDeviceComponent.EnemiesGotHitByDamage.Contains(col.transform.root))
                {
                    ObjectDetection(col.transform.root.GetComponent<Targets>().MyBodyPartToTarget);
                    CalculateDamage();
                    col.gameObject.transform.root.SendMessage("Takedamage", ActualDamage, SendMessageOptions.DontRequireReceiver);
                    MovableAi.Add(col.transform.root);
                    ExplosiveDeviceComponent.EnemiesGotHitByDamage.Add(col.transform.root);
                }
            }

            if (col.transform.CompareTag("Target") && col.transform.GetComponent<Target>() != null)
            {
                if (!MovableAi.Contains(col.transform) && !ExplosiveDeviceComponent.EnemiesGotHitByDamage.Contains(col.transform))
                {
                    col.gameObject.transform.GetComponent<Target>().StartRotating = true;
                    MovableAi.Add(col.transform);
                    ExplosiveDeviceComponent.EnemiesGotHitByDamage.Add(col.transform);
                }
            }
            if (AddRaycastForce == true)
            {
                AddForceToRigidBodies(col);
            }

            if (DoneWithExplosion == false)
            {
                StartCoroutine(coro());
                DoneWithExplosion = true;
            }

        }
        public void AddForceToRigidBodies(Collider col)
        {
            EnemyRaycaster.LookAt(col.transform.position);
            Vector3 directionToTarget = col.transform.position - EnemyRaycaster.position; // Grenade -> Target direction
            RaycastHit hit;

            if (Physics.Raycast(EnemyRaycaster.position, EnemyRaycaster.forward, out hit, directionToTarget.magnitude, LayersToApplyRaycastForce))
            {
                //Debug.DrawLine(EnemyRaycaster.position, EnemyRaycaster.position + EnemyRaycaster.forward * directionToTarget.magnitude, Color.red, 0.1f);
                //Debug.Break();
                if (hit.transform.root.tag == "AI" && !ExplosiveDeviceComponent.TargetsToApplyForce.Contains(hit.transform.root)) // AI Logic
                {
                    if (hit.transform.root.GetComponent<HumanoidAiHealth>() != null)
                    {
                        if (hit.transform.root.GetComponent<HumanoidAiHealth>().Health <= 0)
                        {
                            if (col.transform.root.GetComponent<Animator>() != null)
                            {
                                col.transform.root.GetComponent<Animator>().enabled = false;
                            }

                            ExplosiveDeviceComponent.TargetsToApplyForce.Add(hit.transform.root);
                            Vector3 grenadePosition = transform.position; // Position of the grenade
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
                                    float forceMagnitude = Mathf.Lerp(AIMinExplosiveForce, AIMaxExplosiveForce, distanceFactor);

                                    // Apply the force
                                    rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);

                                    // Debugging information
                                  //  Debug.Log($"{g.gameObject.name} - Closest Direction: {closestDirection}, Force Applied: {forceDirection}, Magnitude: {forceMagnitude}");
                                }
                            }
                        }
                    }

                }
                else if (hit.transform.root.tag == "Player" && !ExplosiveDeviceComponent.TargetsToApplyForce.Contains(hit.transform.root) && PlayerExplosiveForce == PlayerForce.ShakePlayerCamera)
                {
                    ExplosiveDeviceComponent.TargetsToApplyForce.Add(hit.transform.root);
                    if (hit.transform.root.GetComponent<CameraShakerEffect>() != null)
                    {
                        hit.transform.root.GetComponent<CameraShakerEffect>().Shake();
                    }
                }
                else if (hit.transform.root.tag == "Player" && !ExplosiveDeviceComponent.TargetsToApplyForce.Contains(hit.transform.root) && PlayerExplosiveForce == PlayerForce.ApplyForceToPlayerRigidbody)
                {

                    if (hit.transform.GetComponent<Rigidbody>() != null)
                    {
                        ExplosiveDeviceComponent.TargetsToApplyForce.Add(hit.transform);
                        Rigidbody rb = hit.transform.gameObject.GetComponent<Rigidbody>();
                        rb.isKinematic = false;
                        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                        rb.AddForce(-hit.normal * NonAIExplosiveForce, ForceMode.Impulse);
                    }

                }
                else if (hit.transform.root.tag == "Player" && !ExplosiveDeviceComponent.TargetsToApplyForce.Contains(hit.transform.root) && PlayerExplosiveForce == PlayerForce.ApplyForceToPlayerRigidbodyAndShakePlayerCamera)
                {
                    ExplosiveDeviceComponent.TargetsToApplyForce.Add(hit.transform.root);
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

                        rb.AddForce(-hit.normal * NonAIExplosiveForce, ForceMode.Impulse);
                    }
                }
                else if (hit.transform.root.tag == "Player" && !ExplosiveDeviceComponent.TargetsToApplyForce.Contains(hit.transform.root) && PlayerExplosiveForce == PlayerForce.DoNotApplyForceToPlayer)
                {
                    ExplosiveDeviceComponent.TargetsToApplyForce.Add(hit.transform.root);
                }
                else // Non-AI Logic
                {
                    if (!ExplosiveDeviceComponent.TargetsToApplyForce.Contains(hit.transform))
                    {
                        //foreach (Transform g in hit.transform.root.GetComponentsInChildren<Transform>())
                        //{
                        if (hit.transform.GetComponent<Rigidbody>() != null)
                        {
                            ExplosiveDeviceComponent.TargetsToApplyForce.Add(hit.transform);
                            Rigidbody rb = hit.transform.gameObject.GetComponent<Rigidbody>();
                            rb.isKinematic = false;
                            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                            // Non-AI: Add force in explosion normal direction
                            rb.AddForce(-hit.normal * NonAIExplosiveForce, ForceMode.Impulse);
                        }
                        //}
                    }
                }
            }
        }

        //public void AddForceToRigidBodies(Collider col)
        //{
        //    EnemyRaycaster.LookAt(col.transform.position);
        //    Vector3 directionToTarget = col.transform.position - EnemyRaycaster.position; // Grenade -> Target direction
        //    RaycastHit hit;

        //    if (DebugRaycastFromOriginToTargetPosition)
        //    {
        //        // Debug line to visualize the ray
        //        Debug.DrawLine(EnemyRaycaster.position, EnemyRaycaster.position + EnemyRaycaster.forward * directionToTarget.magnitude, Color.blue, 0.1f);
        //    }

        //    if (Physics.Raycast(EnemyRaycaster.position, EnemyRaycaster.forward, out hit, directionToTarget.magnitude))
        //    {
        //        if (hit.transform.root.tag == "AI") // AI Logic
        //        {
        //            Vector3 grenadePosition = col.transform.position; // Position of the grenade
        //            Transform aiRoot = hit.transform.root;            // AI root transform
        //            Vector3 aiToGrenadeDirection = (grenadePosition - aiRoot.position).normalized;

        //            // Use Dot Product to determine if the grenade is left or right of the AI
        //            float rightDot = Vector3.Dot(aiRoot.right, aiToGrenadeDirection);

        //            // Iterate over all rigidbodies in the AI hierarchy
        //            foreach (Transform g in aiRoot.GetComponentsInChildren<Transform>())
        //            {
        //                if (g.gameObject.GetComponent<Rigidbody>() != null)
        //                {
        //                    Rigidbody rb = g.gameObject.GetComponent<Rigidbody>();
        //                    rb.isKinematic = false;
        //                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        //                    // Apply force based on grenade's relative position (left or right)
        //                    if (rightDot > 0) // Grenade is on the RIGHT side of AI -> Apply force to the LEFT
        //                    {
        //                        rb.AddForce(-aiRoot.right * Random.Range(AIMinExplosiveForce, AIMaxExplosiveForce), ForceMode.Impulse);
        //                        Debug.Log($"{g.gameObject.name} - Force Applied: LEFT");
        //                    }
        //                    else if (rightDot < 0) // Grenade is on the LEFT side of AI -> Apply force to the RIGHT
        //                    {
        //                        rb.AddForce(aiRoot.right * Random.Range(AIMinExplosiveForce, AIMaxExplosiveForce), ForceMode.Impulse);
        //                        Debug.Log($"{g.gameObject.name} - Force Applied: RIGHT");
        //                    }
        //                }
        //            }
        //        }
        //        else // Non-AI Logic
        //        {
        //            foreach (Transform g in hit.transform.root.GetComponentsInChildren<Transform>())
        //            {
        //                if (g.gameObject.GetComponent<Rigidbody>() != null)
        //                {
        //                    Rigidbody rb = g.gameObject.GetComponent<Rigidbody>();
        //                    rb.isKinematic = false;
        //                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        //                    // Non-AI: Add force in explosion normal direction
        //                    rb.AddForce(-hit.normal * 5f, ForceMode.Impulse);
        //                }
        //            }
        //        }
        //    }
        //}
        //public void AddForceToRigidBodies(Collider col)
        //{
        //    EnemyRaycaster.LookAt(col.transform.position);
        //    Vector3 directionToTarget = col.transform.position - EnemyRaycaster.position; // Grenade -> Target direction
        //    RaycastHit hit;

        //    if (DebugRaycastFromOriginToTargetPosition)
        //    {
        //        // Debug line to visualize the ray
        //        Debug.DrawLine(EnemyRaycaster.position, EnemyRaycaster.position + EnemyRaycaster.forward * directionToTarget.magnitude, Color.blue, 0.1f);
        //    }

        //    if (Physics.Raycast(EnemyRaycaster.position, EnemyRaycaster.forward, out hit, directionToTarget.magnitude))
        //    {
        //        if (hit.transform.root.tag == "AI" && !tempo.Contains(hit.transform.root)) // AI Logic
        //        {
        //            tempo.Add(hit.transform.root);
        //            Vector3 grenadePosition = col.transform.position; // Position of the grenade
        //            Transform aiRoot = hit.transform.root;            // AI root transform
        //            Vector3 aiToGrenadeDirection = (grenadePosition - aiRoot.position).normalized;

        //            // Calculate distances using Dot Product
        //            float rightDot = Vector3.Dot(aiRoot.right, aiToGrenadeDirection);       // Positive = Right, Negative = Left
        //            float forwardDot = Vector3.Dot(aiRoot.forward, aiToGrenadeDirection);   // Positive = Front, Negative = Back

        //            // Determine the closest direction
        //            string closestDirection;
        //            Vector3 forceDirection = Vector3.zero;

        //            if (Mathf.Abs(rightDot) > Mathf.Abs(forwardDot)) // Left or Right dominates
        //            {
        //                if (rightDot > 0) // Grenade is closer to the RIGHT → Apply force to the LEFT
        //                {
        //                    forceDirection = -aiRoot.right;
        //                    closestDirection = "Right";
        //                }
        //                else // Grenade is closer to the LEFT → Apply force to the RIGHT
        //                {
        //                    forceDirection = aiRoot.right;
        //                    closestDirection = "Left";
        //                }
        //            }
        //    else // Front or Back dominates
        //    {
        //        if (forwardDot > 0) // Grenade is closer to the FRONT → Apply force BACKWARD
        //        {
        //            forceDirection = -aiRoot.forward;
        //            closestDirection = "Front";
        //        }
        //        else // Grenade is closer to the BACK → Apply force FORWARD
        //        {
        //            forceDirection = aiRoot.forward;
        //            closestDirection = "Back";
        //        }
        //    }

        //    // Apply the force to all rigidbodies in the AI
        //    foreach (Transform g in aiRoot.GetComponentsInChildren<Transform>())
        //    {
        //        if (g.gameObject.GetComponent<Rigidbody>() != null)
        //        {
        //            Rigidbody rb = g.gameObject.GetComponent<Rigidbody>();
        //            rb.isKinematic = false;
        //            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        //            // Calculate force magnitude based on distance
        //            float distanceToGrenade = Vector3.Distance(grenadePosition, aiRoot.position);
        //            float distanceFactor = Mathf.Clamp01(1 - (distanceToGrenade / RadiusToApplyForce)); // Normalize effect
        //            float forceMagnitude = Mathf.Lerp(AIMinExplosiveForce, AIMaxExplosiveForce, distanceFactor);

        //            // Apply the force
        //            rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);

        //            // Debugging information
        //            Debug.Log($"{g.gameObject.name} - Closest Direction: {closestDirection}, Force Applied: {forceDirection}, Magnitude: {forceMagnitude}");
        //        }
        //    }
        //}
        //        else // Non-AI Logic
        //        {
        //            if (!tempo.Contains(hit.transform.root))
        //            {
        //                tempo.Add(hit.transform.root);
        //                foreach (Transform g in hit.transform.root.GetComponentsInChildren<Transform>())
        //                {
        //                    if (g.gameObject.GetComponent<Rigidbody>() != null)
        //                    {
        //                        Rigidbody rb = g.gameObject.GetComponent<Rigidbody>();
        //                        rb.isKinematic = false;
        //                        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        //                        // Non-AI: Add force in explosion normal direction
        //                        rb.AddForce(-hit.normal * NonAIExplosiveForce, ForceMode.Impulse);
        //                    }
        //                }
        //            }

        //        }
        //    }
        //}


        //public void AddForceToRigidBodies(Collider col)
        //{
        //    EnemyRaycaster.LookAt(col.transform.position);
        //    Vector3 directionToTarget = col.transform.position - EnemyRaycaster.position;
        //    RaycastHit hit;

        //    if (DebugRaycastFromOriginToTargetPosition == true)
        //    {
        //        // Debug line to visualize the first ray
        //        Debug.DrawLine(EnemyRaycaster.position, EnemyRaycaster.position + EnemyRaycaster.forward * directionToTarget.magnitude, Color.blue, 0.1f);
        //    }
        //    Debug.Break();
        //    if (Physics.Raycast(EnemyRaycaster.position, EnemyRaycaster.forward, out hit, directionToTarget.magnitude))
        //    {
        //        if (hit.transform.root.tag == "AI")
        //        {
        //            foreach (Transform g in hit.transform.root.GetComponentsInChildren<Transform>())
        //            {
        //                if (g.gameObject.GetComponent<Rigidbody>() != null)
        //                {
        //                    g.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //                    g.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;

        //                    Vector3 explosionPosition = col.gameObject.transform.position + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        //                    g.gameObject.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(MinExplosiveForce, MaxExplosiveForce), explosionPosition, ExplosionRadius);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (Transform g in hit.transform.root.GetComponentsInChildren<Transform>())
        //            {
        //                if (g.gameObject.GetComponent<Rigidbody>() != null)
        //                {
        //                    g.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //                    g.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;

        //                    g.gameObject.GetComponent<Rigidbody>().AddForce(-hit.normal * 100f);
        //                }
        //            }
        //        }
        //    }
        //}
        public void ObjectDetection(Transform EnemyToLookAt)
        {
            CompleteDamage = false;
            bool DoISeeEnemy = false;
            EnemyRaycaster.LookAt(EnemyToLookAt.position);
            Vector3 directionToTarget = EnemyToLookAt.position - EnemyRaycaster.position;


            RaycastHit hit;
            if (Physics.Raycast(EnemyRaycaster.position, EnemyRaycaster.forward, out hit, directionToTarget.magnitude, IgnoredLayers))
            {
                if (DebugRaycastFromOriginToTargetPosition == true)
                {
                    // Debug line to visualize the first ray
                    Debug.DrawLine(EnemyRaycaster.position, EnemyRaycaster.position + EnemyRaycaster.forward * directionToTarget.magnitude, Color.blue, 0.1f);
                }
                if (hit.transform.root == EnemyToLookAt.transform.root)
                {
                    DoISeeEnemy = true;
                }
            }
            if (DoISeeEnemy == true)
            {
                var lookPos = EnemyToLookAt.position - ObjectDetector.transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                ObjectDetector.rotation = Quaternion.Slerp(ObjectDetector.rotation, rotation, Time.deltaTime * 5000f);

                if (DebugRaycastFromOriginToTargetPosition == true)
                {
                    // Debug line to visualize the second ray
                    Debug.DrawLine(ObjectDetector.position, ObjectDetector.position + ObjectDetector.forward * directionToTarget.magnitude, Color.green, 0.1f);
                }

                RaycastHit Newhit;
                if (Physics.Raycast(ObjectDetector.position, ObjectDetector.forward, out Newhit, directionToTarget.magnitude, IgnoredLayers))
                {
                    if (Newhit.transform.root == EnemyToLookAt.transform.root)
                    {
                        CompleteDamage = true;
                    }
                }
                else
                {
                    CompleteDamage = true;
                }
            }
        }
        public void CalculateDamage()
        {
            if (CompleteDamage == true)
            {
                ActualDamage = Damage;
            }
            else
            {
                if(RandomiseDamageBehindCover == true)
                {
                    int Randomise = Random.Range(0, 2);
                    if(Randomise == 0)
                    {
                        ActualDamage = MinDamageBehindCover;
                    }
                    else
                    {
                        ActualDamage = MaxDamageBehindCover;
                    }
                }
                else
                {
                    ActualDamage = MinDamageBehindCover;                    
                }
            }       
        }
        IEnumerator coro()
        {
            yield return new WaitForSeconds(TimeToDeactive);
            DoneWithExplosion = false;
            ExplosiveDeviceComponent.Explode = false;
            MovableAi.Clear();
            CanShake = false;
            gameObject.SetActive(false);
        }
    }
}