using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This Script will Make The Ai Bot To Go To Predefined Destinations Taking The Player Making Sure He will behave as Commander

namespace MobileActionKit
{
    public class PlayerGuide : MonoBehaviour
    {

        [TextArea]
        public string ScriptInfo = "This script makes assigned Ai agents to follow player." +
            "If Agents following the player get into combat they will stop following him for the duration of the combat and will resume player following behaviour once combat ends.";

        [Tooltip("Drag and drop into this field one or multiple Ai agents from the hierarchy tab (with their role set as the 'Follower' inside the 'core Ai behaviour' script). " +
            "This functionality is designed to work with AI agents that are preplaced in the level by  mission designer(not spawned AI agents).")]
        public List<AiFollower> FollowerAgents = new List<AiFollower>();

        void Start()
        {
            PlayerGuards();
        }
        public void PlayerGuards()
        {
            for (int x = 0; x < FollowerAgents.Count; x++)
            {
                if (FollowerAgents[x].GetComponent<CoreAiBehaviour>() != null)
                {
                    FollowerAgents[x].GetComponent<CoreAiBehaviour>().IsBodyguard = true;
                    FollowerAgents[x].GetComponent<CoreAiBehaviour>().BossTransform = transform;

                }
            }
        }
    }
}