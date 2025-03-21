using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class InvestigationRadius : MonoBehaviour
    {

        [TextArea]
        [Tooltip("This script activates investigation radius on  the Ai agent at the first time when it" +
            " detects a dead body so that other Ai agents who are within this trigger would get into investigation state as well. " +
            "Investigating Agents will move within proximity of dead body to get within distance specified in  the 'Core AI Behaviour script' that has paragraph " +
            "called 'AI Investigation Alerts' where field called 'Investigation Radius From The Dead Body' is located." +
            "Activation of investigation trigger on some AI agents will cause similar activation for other Ai agents who will happen" +
            " to be within resulted investigation triggers which will create chain reaction or domino effect for all affected agents." +
            "If same dead body is encoutered for the second time then by same agents thaen they will not react to it and will not get into investigation state. ")]
        public string ScriptInfo = "This script activates investigation radius on  the Ai agent at the first time when it" +
            " detects a dead body so that other Ai agents who are within this trigger would get into investigation state as well. " +
            "Investigating Agents will move within proximity of dead body to get within distance specified in  the 'Core AI Behaviour script' that has paragraph " +
            "called 'AI Investigation Alerts' where field called 'Investigation Radius From The Dead Body' is located." +
            "Activation of investigation trigger on some AI agents will cause similar activation for other Ai agents who will happen" +
            " to be within resulted investigation triggers which will create chain reaction or domino effect for all affected agents." +
            "If same dead body is encoutered for the second time then by same agents thaen they will not react to it and will not get into investigation state. ";

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
            if (Other.gameObject.transform.root.tag == "AI" && Other.transform.root != this.transform.root && Other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
            {
                if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>() != null)
                {
                    if (!Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().DeadBodiesSeen.Contains(transform.root.GetComponent<CoreAiBehaviour>().FriendlyDeadBodyName)
                        && Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().SearchingForSound == false)
                    {
                        if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().CombatStarted == false && !DisplayAiAgents.Contains(Other.gameObject.transform.root.transform)
                            && Other.gameObject.transform.root.gameObject.GetComponent<HumanoidAiHealth>().IsDied == false)
                        {

                            DisplayAiAgents.Add(Other.gameObject.transform.root.transform);


                            if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsNearDeadBody == false)
                            {
                                //if (transform.root.gameObject.GetComponent<CoreAiBehaviour>() != null)
                                //{
                                //    transform.root.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidAiAudioPlayerComponent.PlaySoundClipsForSingleAudio(transform.root.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceDeadBodyInvestigationAudioClips);
                                //}
                                Debug.Log("Near Deadbody Alert");
                                Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().InvestigationCoordinates = transform.root.GetComponent<CoreAiBehaviour>().InvestigationCoordinates;
                                Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsNearDeadBody = true;
                                Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsEmergencyRun = false;
                                Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsEmergencyStateExpiredCompletely = true;
                                Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().DeadBodiesSeen.Add(transform.root.GetComponent<CoreAiBehaviour>().FriendlyDeadBodyName);
                                Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().FriendlyDeadBodyName = transform.root.GetComponent<CoreAiBehaviour>().FriendlyDeadBodyName;
                            }
                        }
                    }
                    else if(Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().DeadBodiesSeen.Contains(transform.root.GetComponent<CoreAiBehaviour>().FriendlyDeadBodyName))
                    {
                        if (!DisplayAiAgents.Contains(Other.gameObject.transform.root.transform))
                        {
                            DisplayAiAgents.Add(Other.gameObject.transform.root.transform);
                        }
                    }

                }
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
        //                Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().InvestigationPoint = transform.root.GetComponent<SimpleAiBehaviour>().InvestigationPoint;
        //                Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().IsNearDeadBody = true;
        //                Other.gameObject.transform.root.gameObject.GetComponent<SimpleAiBehaviour>().IsEmergencyRun = false;
        //            }
        //        }
        //    }
        //}
    }
}
