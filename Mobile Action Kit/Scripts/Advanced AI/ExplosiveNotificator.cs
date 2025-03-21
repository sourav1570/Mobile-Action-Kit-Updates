using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class ExplosiveNotificator : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script is responsible for explosion behaviour of the grenade and also thus notify other humanoid Ai agent's about incoming grenade." +
            "This script is responsible to notify other humanoid Ai agent to sprint away from the incoming grenade based upon the configuration of the humanoid Ai agent.";

        [HideInInspector]
        public List<Transform> AiInMyList = new List<Transform>();

        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.transform.root.tag == "AI" && col.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
            {
                if (col.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
                {
                    if (!AiInMyList.Contains(col.gameObject.transform.root.transform)) // important to this because Ai agent contains multiple collider as it is a ragdoll and if let say there are
                                                                                       // 10 collider and it has to do 10 checks in grenade visibility check which indeed can create a bug so to avoid that
                                                                                       // we make sure that we do 1 check per bot so to avoid any miscalculations with multiple collider of same bot
                    {
                        col.gameObject.transform.root.GetComponent<CoreAiBehaviour>().GrenadeVisibilityChecker(transform.root);
                        if (col.gameObject.transform.root.GetComponent<CoreAiBehaviour>().RunFromGrenades == true &&
                            col.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CombatStateBehaviours.EnableGrenadeAlerts == true
                            && col.gameObject.transform.root.GetComponent<CoreAiBehaviour>().DefinatelyRunFromGrenade == true)
                        {
                            col.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.CompleteFirstHitAnimation = false;
                            col.gameObject.transform.root.GetComponent<CoreAiBehaviour>().BotMovingAwayFromGrenade = true;
                        }
                        AiInMyList.Add(col.gameObject.transform.root.transform);
                    }
                   
                }
            }
        }
        // do not do stay related checks as it increase performance Hit on CPU
        //void OnTriggerStay(Collider col)
        //{
        //    if (col.gameObject.transform.root.tag == "AI" && col.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        //    {
        //        if (col.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
        //        {
        //            if (!AiInMyList.Contains(col.gameObject.transform.root.transform))
        //            {
        //                col.gameObject.transform.root.GetComponent<CoreAiBehaviour>().GrenadeVisibilityChecker(transform.root);
        //                if (col.gameObject.transform.root.GetComponent<CoreAiBehaviour>().RunFromGrenades == true
        //                && col.gameObject.transform.root.GetComponent<CoreAiBehaviour>().CombatStateBehaviours.EnableGrenadeAlerts == true
        //                && col.gameObject.transform.root.GetComponent<CoreAiBehaviour>().DefinatelyRunFromGrenade == true)
        //                {
        //                    col.gameObject.transform.root.GetComponent<CoreAiBehaviour>().HealthScript.CompleteFirstHitAnimation = false;
        //                    col.gameObject.transform.root.GetComponent<CoreAiBehaviour>().BotMovingAwayFromGrenade = true;
        //                }
        //                AiInMyList.Add(col.gameObject.transform.root.transform);
        //            }

        //        }
        //    }
        //}
        //private void OnTriggerExit(Collider col)
        //{
        //    if (col.gameObject.transform.root.tag == "AI" && GrenadeCollisionScript.IsCollided == true)
        //    {
        //        if (col.gameObject.transform.root.GetComponent<SimpleAiBehaviour>() != null)
        //        {
        //            StartCoroutine(col.gameObject.transform.root.GetComponent<SimpleAiBehaviour>().RunTimer());
        //        }
        //    }
        //}



    }
}
