using UnityEngine;

namespace MobileActionKit
{
    public class HumanoidAISurfaces : MonoBehaviour
    {
        [TextArea]      
        public string ScriptInfo = "This Script replaces Humanoid AI foot step Sounds within the trigger that should be placed in a certain Surface Area. " +
            "When Ai agent will enter such trigger then its default footstep sounds will be replaced by the particular sound in the field named 'Foot Step Audio' of this script. " +
            "For Example : Metal Surface , Grass Surface etc... ";

        [Tooltip("Drag and drop the desired audio clip from the project into this field to playback this distinct footstep audio clip for the duration of HumanoidAI agents " +
            "inside this trigger (e.g., for grass, metal, wood surfaces etc).")]
        public AudioClip FootStepAudio;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.tag == "AI")
            {
                if (other.transform.root.gameObject.GetComponent<CoreAiBehaviour>() != null)
                {
                    if (other.transform.root.GetComponent<CoreAiBehaviour>().Components.HumanoidAiAudioPlayerComponent != null)
                    {
                        other.transform.root.GetComponent<CoreAiBehaviour>().Components.HumanoidAiAudioPlayerComponent.DefaultFootStepSounds.FootStepAudioClip = FootStepAudio;
                    }
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.transform.root.tag == "AI")
            {
                if (other.transform.root.GetComponent<CoreAiBehaviour>() != null)
                {
                    if (other.transform.root.GetComponent<CoreAiBehaviour>().Components.HumanoidAiAudioPlayerComponent != null)
                    {
                        other.transform.root.GetComponent<CoreAiBehaviour>().Components.HumanoidAiAudioPlayerComponent.DefaultFootStepSounds.FootStepAudioClip =
                             other.transform.root.GetComponent<CoreAiBehaviour>().Components.HumanoidAiAudioPlayerComponent.StoredFootStepAudioClip;
                    }
                }
            }
        }


    }
}
