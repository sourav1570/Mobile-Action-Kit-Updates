using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class ImpactEffect : MonoBehaviour
    {
        [Tooltip("Specify whether the impact effect GameObject should be destroyed.")]
        public bool ShouldDestroy = false;

        [Tooltip("Time in seconds after which the impact effect will be deactivated or destroyed.")]
        public float TimeToDeactivate = 1f;

        [Tooltip("Reference to the AlertingSoundActivator script to handle sound alerts.")]
        public AlertingSoundActivator AlertingSoundScript;


        // using effect activation as a function and not using OnEnable improves the performane as well as help variable DoNotActivateEffect communicate with other scripts.
        public void EffectActivation(Transform SoundCreator)
        {
            if(AlertingSoundScript != null)
            {
                AlertingSoundScript.ActivateNoiseHandler(SoundCreator);
            }
            StartCoroutine(Timer());
        }
        IEnumerator Timer()
        {
            yield return new WaitForSeconds(TimeToDeactivate);
            if (ShouldDestroy == false)
            {
                if (AlertingSoundScript != null)
                {
                    AlertingSoundScript.AlertingSoundScript.DoNotActivateEffect = false;
                }
                gameObject.SetActive(false);
            }
            else
            {
                if (AlertingSoundScript != null)
                {
                    AlertingSoundScript.AlertingSoundScript.DoNotActivateEffect = false;
                }
                Destroy(gameObject);
            }

        }
        //public void TeamWhichWillBeAffectedByTheShot(string TeamName)
        //{
        //    if (AlertingSoundScript != null)
        //    {
        //        AlertingSoundScript.AlertingSoundScript.TeamAffectedBySound = AlertingSound.ChooseHumanoidAiTeamSelection.SelectedTeam;
        //        AlertingSoundScript.AlertingSoundScript.TeamName = TeamName;
        //    }
        //}
    }
}
