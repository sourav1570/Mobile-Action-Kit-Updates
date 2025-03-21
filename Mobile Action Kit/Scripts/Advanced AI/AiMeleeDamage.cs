using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class AiMeleeDamage : MonoBehaviour
    {
        public float DamageToTarget = 5;

        private List<Transform> HitEnemies = new List<Transform>();

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        private void OnEnable()
        {
            HitEnemies.Clear();
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.tag == "Player" && !HitEnemies.Contains(other.gameObject.transform.root))
            {
                PlayerHealth.instance.PlayerHealthbar.Curvalue -= DamageToTarget;
                PlayerHealth.instance.CheckPlayerHealth();
                HitEnemies.Add(other.gameObject.transform.root);
            }
            if (other.transform.root.tag == "AI" && other.transform.root != this.transform.root && other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast") && !HitEnemies.Contains(other.gameObject.transform.root))
            {
                if(other.gameObject.transform.root.GetComponent<Targets>().MyTeamID != transform.root.GetComponent<Targets>().MyTeamID) // So if it is the zombie horde we make sure zombie don't give damage to each other.
                {
                    if (other.gameObject.transform.root.GetComponent<HumanoidAiHealth>() != null)
                    {
                        other.gameObject.transform.root.GetComponent<HumanoidAiHealth>().FindColliderName(other.gameObject.transform.name);

                        Vector3 hitPoint = other.ClosestPoint(transform.position);
                        Vector3 normal = (hitPoint - transform.position).normalized;

                        Quaternion rotation = Quaternion.LookRotation(normal);

                        other.gameObject.transform.root.GetComponent<HumanoidAiHealth>().BloodEffectsFromZombieAttack(hitPoint, rotation);
                    }
                    other.gameObject.transform.root.SendMessage("Takedamage", DamageToTarget, SendMessageOptions.DontRequireReceiver);
                    
                    HitEnemies.Add(other.gameObject.transform.root);
                }
            }
            if (other.transform.tag == "Target" && !HitEnemies.Contains(other.gameObject.transform))
            {
                other.gameObject.transform.SendMessage("Damage", SendMessageOptions.DontRequireReceiver);
                HitEnemies.Add(other.gameObject.transform);
            }
        }
    }
}