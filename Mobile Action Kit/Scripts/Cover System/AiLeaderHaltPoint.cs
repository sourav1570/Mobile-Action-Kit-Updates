using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MobileActionKit
{
    public class AiLeaderHaltPoint : MonoBehaviour
    {

        [TextArea]
        public string ScriptInfo = "Halt points are designed as an additional stop points between destination points and  placed at a certain locations(street corners, gates, entrances etc. ) " +
            "This script sets the various parameters for the Leader at the halt point.";

        public enum Cover
        {
            HaltLeft,
            HaltRight,
            Neutral
        }

        [Tooltip("Animation Clip to be played back by the Leader at the halt point.")]
        public Cover HaltPoseAnimation;

        [Tooltip("Minimum halt time in case followers are within separation limit. In case they are not it will be prolonged until they catch up with the Leader.")]
        public float MinHaltTime = 3f;

        [Tooltip("Maximum halt time in case followers are within separation limit. In case they are not it will be prolonged until they catch up with the Leader.")]
        public float MaxHaltTime = 5f;

        [Tooltip("Value of the leader's rotational offset at the Halt point in degrees to the left from the direction he was facing when approaching this halt point. ")]
        public float HaltPoseLeftRotation = 180;
        [Tooltip("Value of the leader's rotational offset at the Halt point in degrees to the right from the direction he was facing when approaching this halt point. ")]
        public float HaltPoseRightRotation = -180f;
        [Tooltip("Value of the leader's rotational offset at the Halt point in degrees to the neutral from the direction he was facing when approaching this halt point. ")]
        public float HaltPoseNeutralRotation = 0f;

        [Tooltip("Time it takes for the Leader to turn in the specified direction.")]
        public float RotationalOffsetDuration = 0.2f;

    }
}