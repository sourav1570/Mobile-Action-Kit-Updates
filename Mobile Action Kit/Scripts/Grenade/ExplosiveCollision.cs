using System.Collections;
using UnityEngine;

namespace MobileActionKit
{
    public class ExplosiveCollision : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script adds rigidbody to explosive just before it is about to collide any static or dynamic object along it`s trajectory. " +
            "After adding rigidbody it applies the momentum force in the forward direction of the explosive specified in the field called 'Force Momentum'. " +
            "For convenience reasons this script has fields that allow to set the properties of the rigidbody prior to its creation.";

        [Tooltip("Drag and drop explosive explosion script attached with this gameObject from the hierarchy or within the inspector into this field.")]
        public ExplosiveDevice ExplosiveDeviceComponent;

        [Space]
        [Header("Fields below are provided to tweak the added rigidbody.")]
        [Tooltip("Specify the value for Explosive rigidbody mass.")]
        public float ExplosiveRigidbodyMass = 1f;
        [Tooltip("Specify the value for Explosive rigidbody drag.")]
        public float ExplosiveRigidbodyDrag = 1f;

        [Tooltip("Specify the value for Explosive angular drag.")]
        public float ExplosiveRigidbodyAngularDrag = 0.05f;

        [Tooltip("If checked rigidbody will use gravity.")]
        public bool ExplosiveRigidbodyUseGravity = true;
        [Tooltip("If checked rigidbody will be set as kinematic.")]
        public bool ExplosiveRigidbodyIsKinematic = false;

        [Tooltip("Specify the rigidbody interpolation.")]
        public RigidbodyInterpolation ExplosiveRigidbodyInterpolate;

        [Tooltip("Choose one of the collision detection mode for the rigidbody.")]
        public CollisionDetectionMode ExplosiveRigidbodyCollisionDetectionMode = CollisionDetectionMode.Continuous;

        [Space]
        [Tooltip("Determines the force momentum applied to the grenade in the forward direction when it collides with an object such as a wall or roof. " +
            "Example: If set to 10, the grenade will experience a forward impulse force of 10 units upon collision.")]
        public float ForceMomentumOnCollision = 10f;

        private void OnEnable()
        {
            ExplosiveDeviceComponent.gameObject.SetActive(true);
        }
        public void PhysicsAdder()
        {
            if(GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                rb.mass = ExplosiveRigidbodyMass;
                rb.collisionDetectionMode = ExplosiveRigidbodyCollisionDetectionMode;
                rb.drag = ExplosiveRigidbodyDrag;
                rb.angularDrag = ExplosiveRigidbodyAngularDrag;
                rb.interpolation = ExplosiveRigidbodyInterpolate;
                rb.isKinematic = ExplosiveRigidbodyIsKinematic;

                rb.AddForce(transform.forward * ForceMomentumOnCollision, ForceMode.Impulse);
            }           
        }  
    }
}