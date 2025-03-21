using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class ShareSoundCoordinates : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script will share alerting sound coordinate that this Ai agent has with other Ai agents that will get affected by the trigger collider attached to 'SoundInvestigationRadius' game object. " +
            "Thus making those agents to join the sound investigation." +
            "Those Agents that didn`t hear the alerting sound but joined the investigation will also enable their own 'SoundInvestigationRadius' colliders to affect other unaware Ai agents of this investigation." +
            "Such design allows for a domino effect where first alerted Ai agent is alerting other nearby Ai agents.";

        [HideInInspector]
        public List<Transform> Friendlies = new List<Transform>();

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.transform.root.tag == "AI")
            {
                if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null && other.gameObject.transform.root != this.transform.root
                    && other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
                {
                    if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().SearchingForSound == false)
                    {
                        if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().NonCombatBehaviours.EnableSoundAlerts == true &&
                            other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().AiHearing.RecieveFriendlySoundCoordinate == true
                            && !Friendlies.Contains(other.gameObject.transform.root) &&
                            other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().CombatStarted == false
                            && other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsInDefaultInvestigation == true
                                || other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().CombatStarted == false &&
                                   other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().IsBodyguard == true)
                        {
                            //if (transform.root.gameObject.GetComponent<CoreAiBehaviour>() != null)
                            //{
                            //    transform.root.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidAiAudioPlayerComponent.PlaySoundClipsForSingleAudio(transform.root.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidAiAudioPlayerComponent.NonRecurringSounds.OnceHearingInvestigationAudioClips);
                            //}

                            other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().GenerateSoundCoorinate = false;
                            other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().ForceMoveTowardsSoundCoordinate = true;
                            other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().GetSoundCoordinate = gameObject.transform.root.GetComponent<CoreAiBehaviour>().GetSoundCoordinate;
                            other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().SearchingForSound = true;
                            Friendlies.Add(other.gameObject.transform.root);
                            other.gameObject.transform.root.GetComponent<CoreAiBehaviour>().AiHearing.ShareSoundCoordinatesComponent.Friendlies.Add(gameObject.transform.root);
                        }
                    }
                }
            }
        }
    }
}
