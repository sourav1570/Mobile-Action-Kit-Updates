using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class PlayerFollower : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script allows the player to become a follower of the friendly AI agent. " +
            "Simply drag and drop the player follower script onto the AI guide component. This will ensure that the AI waits for the player when moving towards destinations.";

        [Tooltip("Specify the minimum distance to the leader to continue moving towards the destination.")]
        public float MinNearDistanceToLeader = 4f;

        [Tooltip("Specify the maximum distance to the leader to continue moving towards the destination.")]
        public float MaxNearDistanceToLeader = 10f;

        [HideInInspector]
        public float RandomValueForDistanceCheck;

        void Start()
        {

            RandomValueForDistanceCheck = Random.Range(MinNearDistanceToLeader, MaxNearDistanceToLeader);

        }




    }
}
