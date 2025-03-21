using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class DyingSoundRadius : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This script activates the trigger collider on top of the Ai agent at the time when it is dead and helps sends the message to other Ai agents who enters this trigger to move to the nearest emergency covers.";

        string GetName;
        string MyTeamID;

        public void ResettingDescription()
        {
            ScriptInfo = "This script activates the trigger collider on top of the Ai agent at the time when it is dead and helps sends the message to other Ai agents who enters this trigger to move to the nearest emergency covers.";
        }
        void Start()
        {
            //Randomise = Random.Range(0, Directions.Length);
            GetName = gameObject.transform.root.gameObject.GetComponent<Targets>().AutoUniqueIdentity;
            MyTeamID = gameObject.transform.root.gameObject.GetComponent<Targets>().MyTeamID;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        void OnTriggerEnter(Collider Other)
        {
            if (Other.gameObject.transform.root.tag == "AI")
            {
                if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>() != null)
                {
                    if (!Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().DeadBodiesSeen.Contains(GetName))
                        //&& Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().SearchingForSound == false) // Sounds Should be turned Off as the soldier should first secure himself from the threat.
                    {
                        if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().CombatStarted == false)
                        {
                            // uncommented Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().T.MyTeamID == MyTeamID as the AI agent says Buddy KIA which means only friendlies should be affect not enemies.
                            // so during emergency state only friendlies will be affected by the dying sound radius.
                            if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsNearDeadBody == false && Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().T.MyTeamID == MyTeamID
                                && Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().WasInCombatLastTime == false
                                || Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsInEmergencyState == false &&
                                Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().T.MyTeamID == MyTeamID && Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().WasInCombatLastTime == false)
                            {
                                if(Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().NonCombatBehaviours.EnableEmergencyAlerts == true)
                                {
                                    Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().SearchingForSound = false; // Added on 20th March
                                                                                                                                          // Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().ResetEmergencyBehaviourVariables();
                                                                                                                                          //Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().EmergencyDirectionPoint = Directions[Randomise];
                                    Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().InvestigationCoordinates = gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().HealthScript.DeadBodyMesh.transform.position;
                                    Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsNearDeadBody = true;
                                    Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsEmergencyRun = true;
                                    Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsInEmergencyState = true;
                                    Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().DeadBodiesSeen.Add(GetName);
                                    Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().FriendlyDeadBodyName = GetName;
                                    Debug.Log("Dying Sound Alert");
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
        //                Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().EmergencyDirectionPoint = Directions[Randomise];
        //                Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().InvestigationPoint = DeadBodyMesh.transform;
        //                Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().IsNearDeadBody = true;
        //                Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().IsEmergencyRun = true;
        //            }
        //        }
        //    }
        //}
    }
}
