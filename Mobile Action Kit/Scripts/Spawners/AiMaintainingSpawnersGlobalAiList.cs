using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class AiMaintainingSpawnersGlobalAiList : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script holds shared list of Ai agents for multiple 'AiMaintainingSpawners' to refer to. " +
            "It allows keeping global count of alive Ai agents spawned by all 'AiMaintainingSpawners' throughout the mission. " +
            "So that it would be possible to keep constant overall Ai agents count for a team by replenishing it without exceeding shared value of 'Ai agents to Maintain' fields of 'AiMaintainingSpawners' " +
            "in case 'AiMaintainingSpawners' are used together with 'WaveSpawner' that would spawn enemy waves. 'AiMaintainingSpawners' could then respawn required amount of specified friendly Ai agents to maintain the size of player`s team. " +
            "Respawning them synchronously with each new enemy wave. It is used in case game developers want player to be assisted by team of friendly Ai agents to engage enemy waves. ";


        [HideInInspector]
        public List<GameObject> SpawnedAgentsList = new List<GameObject>();

        [Tooltip("This field visualises the list of all alive agents that were spawned by 'AiMaintainingSpawners'.")]
        public List<GameObject> DisplaySpawnedAgentsList = new List<GameObject>();
    }
}
