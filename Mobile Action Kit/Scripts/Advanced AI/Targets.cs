using UnityEngine;

namespace MobileActionKit
{
    public class Targets : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This script is used for the Ai agent to be able to identify other game AI agents and the player as friendlies or hostiles.";
        [Space(10)]

        [Tooltip("Team ID of this Ai agent")]
        public string MyTeamID;
        [Tooltip("Debugs the name of the enemy this bot is currently shooting at")]
        public string DebugOtherTargetName;
        [Tooltip("Drag the Ai agent body part(bone or any other gameobject from Ai hierarchy) into this field for enemy Ai agents to shoot at")]
        public Transform MyBodyPartToTarget;

        //These Values are being used in FindEnemies Script. Friendlies is being used as a reference
        //[HideInInspector]
        //public bool RemoveEnemyFromList = true;
        [HideInInspector]
        public bool EnabledFOV = false;

        private FindEnemies FindEnemiesScript;

        [HideInInspector]
        public string AutoUniqueIdentity;
        bool IsAiIdentityCreated = false;

        [HideInInspector]
        public bool PlayEnemyEliminatedClips = false;

        public void ResettingDescription()
        {
            ScriptInfo = "This script is used for the Ai agent to be able to identify other game AI agents and the player as friendlies or hostiles.";
        }
        void Start()
        {
            FindEnemiesScript = GetComponent<FindEnemies>();
            if(gameObject.tag == "Player")
            {
                AutoUniqueIdentity = "Player";
            }
        }
        public void CreateAUniqueId(int num)
        {
            if (IsAiIdentityCreated == false)
            {
                AutoUniqueIdentity = "HumanoidAi" + num.ToString();
                IsAiIdentityCreated = true;
            }
        }
        //public void CheckAiEnemyList(bool IsEnemyRemoved)
        //{
        //    RemoveEnemyFromList = IsEnemyRemoved;

        //    //Leave it like this for now : // Previously it was not here
        //    FindEnemiesScript.IsEnemyRemoved();
        //    FindEnemiesScript.CheckforEnemy(false);
        //}
        public void CheckAiFov(bool IsFovEnabled)
        {
            EnabledFOV = IsFovEnabled;
        }

    }

}


