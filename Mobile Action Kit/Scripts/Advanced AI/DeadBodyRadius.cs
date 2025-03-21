using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class DeadBodyRadius : MonoBehaviour
    {
        [TextArea]
        [Tooltip("This script activates the trigger collider on top of dead Ai agent. Ai agents that enter this trigger will start investigating  surrounding area.")]
        public string ScriptInfo = "This script activates the trigger collider on top of dead Ai agent. Ai agents that enter this trigger will start investigating  surrounding area.";

        [HideInInspector]
        public string GetName;

        string MyTeamID;

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            GetName = gameObject.transform.root.gameObject.GetComponent<Targets>().AutoUniqueIdentity;
            MyTeamID = gameObject.transform.root.gameObject.GetComponent<Targets>().MyTeamID;
        }
        void OnTriggerEnter(Collider Other)
        {
            if (Other.gameObject.transform.root.tag == "AI")
            {
                if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>() != null)
                {
                    if (!Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().DeadBodiesSeen.Contains(GetName)
                        && Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().SearchingForSound == false)
                    {
                        // uncommented Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().T.MyTeamID == MyTeamID as the AI agent says Buddy KIA which means only friendlies should be affect not enemies.
                        // so during investigation state only friendlies will be affected by the dying sound radius.
                        if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().CombatStarted == false
                             && Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().T.MyTeamID == MyTeamID && Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().WasInCombatLastTime == false)
                        {
                            if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().NonCombatBehaviours.EnableDeadBodyAlerts == true)
                            {
                                if (gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>() != null)
                                {
                                    Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().InvestigationCoordinates = gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().HealthScript.DeadBodyMesh.transform.position;
                                }
                                else
                                {
                                    Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().InvestigationCoordinates = transform.position;
                                }

                                if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsNearDeadBody == false)
                                {
                                    Debug.Log("DeadBody Radius");
                                    Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsNearDeadBody = true;
                                    Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().DeadBodiesSeen.Add(GetName);
                                    Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().FriendlyDeadBodyName = GetName;
                                    Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsEmergencyStateExpiredCompletely = true;
                                }
                            }
                        }
                    }

                }
            }
        }
        //void OnTriggerStay(Collider Other)
        //{
        //    if (Other.gameObject.transform.root.tag == "AI")
        //    {
        //        if (Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>() != null)
        //        {
        //            if (!Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().DeadBodiesSeen.Contains(DeadBodyMesh.transform)
        //                && Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().SearchingForSound == false)
        //            {
        //                Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().InvestigationPoint = DeadBodyMesh.transform;
        //                Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().IsNearDeadBody = true;
        //            }
        //        }
        //    }
        //}
    }
}
