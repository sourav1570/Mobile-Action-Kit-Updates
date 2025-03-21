using System.Collections;
using System.Collections.Generic;
using MobileActionKit;
using UnityEngine;

namespace MobileActionKit
{
    public class ShotSoundAlertOptimizer : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script enables the trigger collider that cancels every other agents weapon firing  sound alerts if those agents happened to be within its radius. " +
            "It is created to optimise performance by minimising the number of weapon fire sound alert colliders and the additional checks involved with those colliders. " +
            "This radius gets enabled when first Ai agent makes his first shot and this radius stays enabled for the entirety of the current combat state of this Ai agent, until he wins the fight or dies." +
            "In case Ai agent dies then every other Ai agent that was within this canceling radius will start their own timers for activating other agents weapon fire alert canceling radiuses. " +
            "That timer will have a random delay between min and max limits that are set inside 'Humanoid Weapon Firing Behaviour' script." +
            "And whichever of the remaining Ai agents of the initial group, will activate this canceling radius the earliest, will have the next canceling radius working the same way. ";

        public bool DrawShotAlertOptimizerRadiusGizmo = false;
        public Color ShotAlertOptimizerRadiusGizmosColor = Color.blue;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (DrawShotAlertOptimizerRadiusGizmo == true)
            {
                Gizmos.color = ShotAlertOptimizerRadiusGizmosColor;

                if (gameObject.GetComponent<SphereCollider>() != null)
                {
                    Gizmos.DrawWireSphere(transform.position, gameObject.GetComponent<SphereCollider>().radius);
                }
            }
        }
#endif
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.transform.root.tag == "AI" && other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast") && other.gameObject.transform.root != gameObject.transform.root)
            {
                if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
                {
                    if (other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent != null)
                    {
                        other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.AgentWhoActivatedAlertingSoundGameObject = transform.root.gameObject;
                        other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.DoNotActivateSoundCoordinate = true;
                    }
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.transform.root.tag == "AI" && other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast") && other.gameObject.transform.root != gameObject.transform.root)
            {
                if (other.gameObject.transform.root.GetComponent<CoreAiBehaviour>() != null)
                {
                    if (other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent != null)
                    {
                        other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.AgentWhoActivatedAlertingSoundGameObject = null;
                        other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.DoNotActivateSoundCoordinate = true;
                        other.gameObject.transform.root.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.StartActivateSoundCoroutine = false;
                    }
                }
            }
        }
    }
}
