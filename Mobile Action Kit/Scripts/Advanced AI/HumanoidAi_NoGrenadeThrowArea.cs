using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MobileActionKit;


namespace MobileActionKit
{
    public class HumanoidAi_NoGrenadeThrowArea : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script enables Humanoid AI agents to refrain from throwing grenades upon entering the trigger area of this collider.";

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.transform.root.tag == "AI")
            {
                if (other.gameObject.transform.root.GetComponent<HumanoidGrenadeThrower>() != null)
                {
                    if (other.gameObject.transform.root.GetComponent<HumanoidGrenadeThrower>().enabled == true)
                    {
                        other.gameObject.transform.root.GetComponent<HumanoidGrenadeThrower>().IsWithinNoThrowArea = true;
                    }
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.transform.root.tag == "AI")
            {
                if (other.gameObject.transform.root.GetComponent<HumanoidGrenadeThrower>() != null)
                {
                    if (other.gameObject.transform.root.GetComponent<HumanoidGrenadeThrower>().enabled == true)
                    {
                        other.gameObject.transform.root.GetComponent<HumanoidGrenadeThrower>().IsWithinNoThrowArea = false;
                    }
                }
            }
        }
    }
}
