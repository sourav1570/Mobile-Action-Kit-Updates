using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class AlertingSoundActivator : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This Script activates the alerting sound and specifies if this sound will travel with its parent game object.";

        public void ResettingDescription()
        {
            ScriptInfo = "This script is responsible for recieving incoming events from other scripts to activate the sound producer gameobject.";
        }

        [Tooltip("Drag and drop Sound Source(Alerting Sound) child game object of this Sound Activator into this field.")]
        public AlertingSound AlertingSoundScript;

        [Tooltip("If checked 'AlertingSound' gameObject will stay with its parent 'SoundActivator' gameobject thus will be able to move together with it. " +
            "Which will result in a dynamic trigger for something like a drone, rocket or a car creating fly by or drive by kind of sound, that would affect all Ai agents touched by the moving sound trigger." +
            "In case unchecked than the Sound gameObject will get unparented and will not follow the root gameObject.For example weapon shot or explosion sound.")]
        public bool KeepSoundAsChild = true;

        [Tooltip("Minimum Delay before activating the 'AlertingSound' gameObject.")]
        public float MinInitialDelayBeforeActivation;
        [Tooltip("Maximum Delay before activating the 'AlertingSound' gameObject.")]
        public float MaxInitialDelayBeforeActivation;

        bool IsCoroAlreadyStarted = false;

        public void ActivateNoiseHandler(Transform SoundCreator)
        {
            if (AlertingSoundScript.gameObject.activeInHierarchy == false)
            {
                if(IsCoroAlreadyStarted == false)
                {               
                    StartCoroutine(FirstDelay(SoundCreator));
                    IsCoroAlreadyStarted = true;
                }
            }
        }
        IEnumerator FirstDelay(Transform SoundCreator)
        {
            float Randomise = Random.Range(MinInitialDelayBeforeActivation, MaxInitialDelayBeforeActivation);
            yield return new WaitForSeconds(Randomise);
            AlertingSoundScript.SoundCreatorEntity = SoundCreator;
            AlertingSoundScript.gameObject.SetActive(true);
            if (KeepSoundAsChild == false)
            {
                AlertingSoundScript.gameObject.transform.parent = null;
            }
            StartCoroutine(HandleSoundDeactivation());
        }
        IEnumerator HandleSoundDeactivation()
        {
            yield return new WaitForSeconds(AlertingSoundScript.TimeToDeactivate);
            AlertingSoundScript.transform.parent = transform;
            AlertingSoundScript.transform.localPosition = Vector3.zero;
            AlertingSoundScript.gameObject.SetActive(false);
            IsCoroAlreadyStarted = false;
        }
    }
}
