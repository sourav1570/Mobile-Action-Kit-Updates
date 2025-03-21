using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class AiFollower : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "If you want this Ai agent to become the follower of another friendly Ai agent or the Player than this script is required to be attached with the field name" +
            " 'Follower Agents' located in the script called 'AiGuide' Or the 'PlayerGuide' script.";

        // public bool OverriteSprinting =  
        [Tooltip("Minimum distance between the leader and this Ai agent to maintain.")]
        public float MinStoppingDistanceToLeader = 4f;
        [Tooltip("Maximum distance between the leader and this Ai agent to maintain.")]
        public float MaxStoppingDistanceToLeader = 10f;

        [Tooltip("Minimum distance before start running towards commander.")]
        public float MinRunningDistanceToLeader = 3f;
        [Tooltip("Maximum distance before start running towards commander.")]
        public float MaxRunningDistanceToLeader = 6f;

        [Tooltip("Minimum distance before start walking towards commander.")]
        public float MinWalkingDistanceToLeader = 7f;
        [Tooltip("Maximum distance before start walking towards commander.")]
        public float MaxWalkingDistanceToLeader = 10f;

        [Tooltip("Minimum distance to trigger sprinting towards the commander if the distance becomes greater.")]
        public float MinSprintingDistanceToLeader = 20f;
        [Tooltip("Maximum distance to trigger sprinting towards the commander if the distance becomes greater.")]
        public float MaxSprintingDistanceToLeader = 25f;


        //[Tooltip("Not to get farther away from the player than value in this field ")]
        //public float  = 10f;
        //[Tooltip("If Ai Gude has made a stop to wait for the player to catch up with him, as soon as player will get within this value," +
        //    "Ai Guide will resume guidance behaviour and will start move towards next Destination point.")]
        //public float  = 20f;

        [HideInInspector]
        public float RandomValueForDistanceCheck;
        //[HideInInspector]
        //public float RandomValueForSprintIfCommanderFurtherThan;
        //[HideInInspector]
        //public float RandomValueToRunToClosestDistance;

        void Start()
        {
            //for(int x = 0;x < FollowerAgents.Count; x++)
            //{
            if (gameObject.GetComponent<CoreAiBehaviour>() != null)
            {
                UpdateFollowerValues();
            }
            //}
        }
        public void UpdateFollowerValues()
        {
            // RandomValueToRunToClosestDistance = Random.Range(MinRunningDistanceToLeader, MaxRunningDistanceToLeader);
            RandomValueForDistanceCheck = Random.Range(MinStoppingDistanceToLeader, MaxStoppingDistanceToLeader);
            //   RandomValueForSprintIfCommanderFurtherThan = Random.Range(MinSprintingDistanceToLeader, MaxSprintingDistanceToLeader);
            // gameObject.GetComponent<CoreAiBehaviour>().IsBodyguard = true;
            // gameObject.GetComponent<CoreAiBehaviour>().BossTransform = this.transform;
            gameObject.GetComponent<CoreAiBehaviour>().FollowCommanderValues.StoppingDistanceToCommander = RandomValueForDistanceCheck;
            //   gameObject.GetComponent<CoreAiBehaviour>().FollowCommanderValues.MaxStoppingDistanceToLeader = MaxStoppingDistanceToLeader;


            gameObject.GetComponent<CoreAiBehaviour>().FollowCommanderValues.MinRunningDistanceToLeader = MinRunningDistanceToLeader;
            gameObject.GetComponent<CoreAiBehaviour>().FollowCommanderValues.MaxRunningDistanceToLeader = MaxRunningDistanceToLeader;

            gameObject.GetComponent<CoreAiBehaviour>().FollowCommanderValues.MinWalkingDistanceToLeader = MinWalkingDistanceToLeader;
            gameObject.GetComponent<CoreAiBehaviour>().FollowCommanderValues.MaxWalkingDistanceToLeader = MaxWalkingDistanceToLeader;

            gameObject.GetComponent<CoreAiBehaviour>().FollowCommanderValues.MinSprintingDistanceToLeader = MinSprintingDistanceToLeader;
            gameObject.GetComponent<CoreAiBehaviour>().FollowCommanderValues.MaxSprintingDistanceToLeader = MaxSprintingDistanceToLeader;

        }
    }
}