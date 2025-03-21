using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class MeleeDamage : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script manages melee damage functionality." +
            " It enables a collider to detect and apply damage to targets during melee attack.";
        [Space(10)]

        public float DamageToTarget = 5;

        //public float TimeToDeactivateCollider = 1f;

        [HideInInspector]
        public bool EnableNow = false;

        //void OnEnable()
        //{
        //    //gameObject.layer = LayerMask.NameToLayer(LayerName);
        // GetComponent<Collider>().enabled = false;
        //  EnableNow = false;

        //}
        public void ActivateNow()
        {
            EnableNow = true;
            GetComponent<Collider>().enabled = true;
        }
        void OnTriggerEnter(Collider other)
        {
            if (EnableNow == true)
            {
//                Debug.Log(other.gameObject.name);
                if (other.transform.root.tag == "AI" && other.transform.root != this.transform.root && other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
                {
                    other.gameObject.transform.root.SendMessage("Takedamage", DamageToTarget, SendMessageOptions.DontRequireReceiver);
                }
                if (other.transform.tag == "Target")
                {
                    other.gameObject.transform.SendMessage("Damage", SendMessageOptions.DontRequireReceiver);
                }
                gameObject.SetActive(true); // newly added
                EnableNow = false;
            }
        }
    }
}
