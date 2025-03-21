using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class HumanoidAiManager : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "The HumanoidAiManager script serves as a central manager for coordinating the behavior of humanoid AI agents within a Unity game environment. " +
            "It optimizes performance by consolidating AI behavior updates and firing actions into a single Update() function, and handles spine rotation updates in the LateUpdate() function. " +
            "This consolidation reduces the overhead of multiple Update() and LateUpdate() calls for individual AI agents, enhancing overall performance.";

        public static HumanoidAiManager instance;

        private CoreAiBehaviour[] masters;

        private GameObject[] go;

        [HideInInspector]
        public Transform Player;

        #region Singleton
        private void Awake()
        {
            instance = this;
        }
        #endregion
        private void Start()
        {
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                Player = GameObject.FindGameObjectWithTag("Player").transform;
            }

            go = GameObject.FindGameObjectsWithTag("AI");

            masters = new CoreAiBehaviour[go.Length];

            for (int i = 0; i < go.Length; ++i)
            {
                if (go[i].GetComponent<CoreAiBehaviour>() != null)
                {
                    masters[i] = go[i].GetComponent<CoreAiBehaviour>();
                    go[i].GetComponent<Targets>().CreateAUniqueId(i);
                }
            }
        }
        private void Update()
        {
            for (int i = 0; i < masters.Length; i++)
            {
                if (masters[i] != null)
                {
                    masters[i].AiFunctioning();

                    if (masters[i].Components.HumanoidFiringBehaviourComponent != null)
                    {
                        masters[i].Components.HumanoidFiringBehaviourComponent.fireSystem();
                    }
                    if (masters[i].Components.HumanoidAiAudioPlayerComponent != null)
                    {
                        masters[i].Components.HumanoidAiAudioPlayerComponent.PlayRecurringAndNonRecurringSounds();
                    }
                }
            }
        }
        public void CheckingForNewEnemies()
        {
            go = GameObject.FindGameObjectsWithTag("AI");

            masters = new CoreAiBehaviour[go.Length];

            for (int i = 0; i < go.Length; ++i)
            {
                masters[i] = go[i].GetComponent<CoreAiBehaviour>();

                if (masters[i] != null)
                {
                    masters[i].FindNewEnemies();
                    if (masters[i].Components.HumanoidFiringBehaviourComponent != null)
                    {
                        masters[i].Components.HumanoidFiringBehaviourComponent.FindTargetAI();
                    }
                }
            }
        }
        private void LateUpdate()
        {
            for (int i = 0; i < masters.Length; i++)
            {
                if (masters[i] != null)
                {
                    masters[i].SpineRotation();
                }
            }
        }
    }
}
