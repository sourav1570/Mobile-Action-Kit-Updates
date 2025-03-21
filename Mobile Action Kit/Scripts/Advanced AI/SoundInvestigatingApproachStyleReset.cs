using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MobileActionKit
{
    public class SoundInvestigatingApproachStyleReset : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "Default approach style can be overwritten by the values stored in the 'AlertingSound'script attached to Alerting sound activator. 'SoundInvestigatingApproachReset' " +
            " decides if to restore or not the approach style of the sound investigating Ai agents towards alerting sound coordinate to default values that are set inside 'Ai Hearing' paragraph of " +
            "the 'CoreAiBehaviour'script. 'InvestigatingApproachReset' gets activated for a brief moment upon Ai agent's death and checks if there are alive friendly Ai agents in the combat state as well" +
            " as approaching sound alert investigators within it's collider radius.If there are alive friendly Ai agents that are in combat state within this collider at the moment" +
            " of this check then no approach style reset is applied to investigators inside this collider. " +
            "If there are no alive friendly Ai agents in combat state at the moment of check then approach style reset will be made to investigators inside this collider.";

        [HideInInspector]
        public List<CoreAiBehaviour> HumanoidAi = new List<CoreAiBehaviour>();

        bool IsCombatIsGoing = false;

        bool once = false;

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        void OnTriggerEnter(Collider Other)
        {
            if (Other.gameObject.transform.root.tag == "AI")
            {
                if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>() != null)
                {
                    if(Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().CombatStarted == true)
                    {
                        if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().HealthScript != null)
                        {
                            if (Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().HealthScript.IsDied == false)
                            {
                                IsCombatIsGoing = true;
                            }
                        }
                    }
                    else
                    {
                        if (!HumanoidAi.Contains(Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>()))
                        {
                            HumanoidAi.Add(Other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>());
                        }
                    }
                }
            }
            if (once == false)
            {
                StartCoroutine(TimeTodeactive());
                once = true;
            }
        }
        private IEnumerator TimeTodeactive()
        {
            yield return null;
            if(IsCombatIsGoing == false)
            {
                for (int x = 0; x < HumanoidAi.Count; x++)
                {
                    if (HumanoidAi[x].gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>() != null)
                    {
                        HumanoidAi[x].gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().OverriteWalkingForSounds = true;
                        HumanoidAi[x].gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().OverriteRunningForSounds = true;
                        HumanoidAi[x].gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().OverriteSprintingForSounds = true;
                    }
                }

                gameObject.SetActive(false);

            }
          
        }
    }

}