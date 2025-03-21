using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class EmergencyAlertRadius : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script  activates emergency alert trigger on  the Ai agent at the moment it sees  dying friendly or hears dying " +
            "sound and starts sprinting towards the nearby random safe point or the emergency cover point to get into safety." +
            "And as he does that, he sends message to other Ai agents who will happen to be within his emergency alert trigger, to activate their own emergency alert triggers and inherit his state." +
            "Activation of emergency alert trigger on some AI agents will cause similar activation on other Ai agents" +
            " who will happen to be within activated triggers which will result chain reaction effect for all affected agents.";

        [Tooltip("Display friendlies AI agents who are within emergency state.")]
        public List<Transform> DisplayAiAgents = new List<Transform>();

        private void OnEnable()
        {
            DisplayAiAgents.Clear();
        }
        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        void OnTriggerEnter(Collider Other)
        {
            //if(ShouldActivateRadius == true)
            //{
            if (Other.gameObject.transform.root.tag == "AI" && Other.transform.root != this.transform.root && Other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
            {
                if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>() != null)
                {
                    //if (!Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().DeadBodiesSeen.Contains(transform.root.GetComponent<CoreAiBehaviour>().FriendlyDeadBodyName))
                    ////&& Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().SearchingForSound == false)// Sounds Should be turned Off as the soldier should first secure himself from the threat.
                    //{
                    if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().CombatStarted == false && !DisplayAiAgents.Contains(Other.gameObject.transform.root.transform)
                         && Other.gameObject.transform.root.gameObject.GetComponent<HumanoidAiHealth>().IsDied == false)
                    {
                      
                        DisplayAiAgents.Add(Other.gameObject.transform.root.transform);

                        if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsNearDeadBody == false ||
                            Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsInEmergencyState == false)
                        {
                            //if (transform.root.gameObject.GetComponent<CoreAiBehaviour>() != null)
                            //{
                            //    transform.root.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidAiAudioPlayerComponent.PlaySoundClipsForSingleAudio(transform.root.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceEmergencyAudioClips);
                            //}
                            Debug.Log("Emergency Alert");
                            Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().SearchingForSound = false; // Added on 20th March
                            Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().EmergencyDirectionPoint = transform.root.GetComponent<CoreAiBehaviour>().EmergencyDirectionPoint;
                            Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().InvestigationCoordinates = transform.root.GetComponent<CoreAiBehaviour>().InvestigationCoordinates;
                            Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsNearDeadBody = true;
                            Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsEmergencyRun = true;
                            Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsInEmergencyState = true;
                            // Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().DeadBodiesSeen.Add(transform.root.GetComponent<CoreAiBehaviour>().FriendlyDeadBodyName);
                            // Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().FriendlyDeadBodyName = transform.root.GetComponent<CoreAiBehaviour>().FriendlyDeadBodyName;

                        }

                    }
                    //}

                }
                //}
            }
        }
        //void OnTriggerStay(Collider Other)
        //{
        //    if (Other.gameObject.transform.root.tag == "AI" && Other.gameObject.transform.root != this.transform.root)
        //    {
        //        if (Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>() != null)
        //        {
        //            if (!Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().DeadBodiesSeen.Contains(transform.root.GetComponent<SimpleAiBehaviour>().InvestigationPoint)
        //                && Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().SearchingForSound == false)
        //            {
        //                Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().EmergencyDirectionPoint = transform.root.GetComponent<SimpleAiBehaviour>().EmergencyDirectionPoint;
        //                Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().InvestigationPoint = transform.root.GetComponent<SimpleAiBehaviour>().InvestigationPoint;
        //                Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().IsNearDeadBody = true;
        //                Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().IsEmergencyRun = true;
        //            }
        //        }
        //    }
        //}


    }
}