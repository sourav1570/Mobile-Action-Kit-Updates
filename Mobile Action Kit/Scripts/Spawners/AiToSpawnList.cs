using UnityEngine;

namespace MobileActionKit
{
    public class AiToSpawnList : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script serves as an enhancement for spawners, streamlining the process of assigning agents across multiple instances of the same spawner script." +
            " For instance, if you have 5 spawners and wish to assign agents to all of them effortlessly, use this script. Simply add your agents to this centralized script, " +
            "and then reference it in the 'UseSeparateScriptForAssigningAgents' field within each spawner script. " +
            "This allows you to efficiently manage and apply agent assignments by dragging and dropping this script directly into the designated field in each spawner script.";

        [Tooltip("Assign one or more agents in this field.")]
        public GameObject[] Agents;
    }
}