using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class AiCover : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This is the cover assembly parent script. " +
            "It is defining accessibility of the various child cover nodes based on the enemy visibility conditions for those cover points. " +
            "There are 5 types of cover nodes that can be assembled in any combination to provide general cover behaviour for combat situations. 4 of the 5 cover points are used during AI agents  combat state. " +
            "When request is made to the cover assembly for availability of it`s child cover nodes then the visibility check is done by all cover points in the assembly. " +
            "Those cover points that have Ai agents current closest enemy within their field of view(240 degrees) are discarded as invalid. " +
            "The remaining valid cover points ar then sorted by AI agent and best cover point is selected. It is the cover point that has the biggest angle to turn towards current enemy of interest  directly." +
            " In other words the on that is closest to being directly opposite to the current closest enemy of Ai agent who made cover request." +
            "Fifth cover node type is called 'Emergency Cover' and is designed for the non combat or pre-combat cover behaviour." +
            "For example  when Ai agents are under the threat but can not detect and locate the attackers." +
            "In this case they will sprint towards emergency cover and will hide there for some specified amount of time before moving out towards supposed enemy position by performing movement between " +
            "available emergency covers and/or sprinting  in a zigzag pattern to increase their survival chances. ";
       
        [Space(10)]
        [Tooltip("This checkbox limits the cover assembly capacity to only one AI agent at a time regardless of how many child cover nodes it has.")]
        public bool SingleAgentCover = false;
        [Tooltip("Enables this cover assembly to become available for all AI agents of both opposing teams. This allows opposing Ai agents to occupy opposite Hide Cover nodes of the same Cover assembly.")]
        public bool UniversalHidingCover = false;

        [Tooltip("Specifies team that is allowed to access this cover assembly." +
            "Useful for saving  performance in cases when  there is no possible way for second team to reach and  occupy this cover assembly that is on the roof of the building for example or any other isolated area." +
            "If checked then additional 'Team Name' field will appear to enter the name of the designated team.")]
        public bool SpecificTeamCover = false;

        [Tooltip("If the above field name 'SpecificTeamCover' is checked than it is required to enter the TeamName for which this whole cover can be available too.")]
        public string TeamName;

        //[HideInInspector]
        public Transform[] coverNodes;
        int count;
        int assignIndex;
        int combatcount;

        [HideInInspector]
        public List<Transform> OpenFirePoints = new List<Transform>();

        private void Start()
        {
            for (int x = 0; x < transform.childCount; x++)
            {
                if (transform.GetChild(x).GetComponent<CoverNode>() != null)
                {
                    ++combatcount;
                }
            }

            coverNodes = new Transform[combatcount];

            for (int x = 0; x < transform.childCount; x++)
            {
                if (transform.GetChild(x).GetComponent<CoverNode>() != null)
                {
                    coverNodes[assignIndex] = transform.GetChild(x).transform;
                    ++assignIndex;
                }
            }
        }
    }
}