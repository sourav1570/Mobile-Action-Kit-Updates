using UnityEngine;

namespace MobileActionKit
{
    public class LevelToUnlock : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script manages level unlocking. When a player completes a level, it updates the highest level reached in PlayerPrefs, allowing progression to the next level.";

        public static LevelToUnlock instance;

        [Tooltip("The next level that should be unlocked upon completion of the current level.")]
        public int NextLevelToUnlock = 1;

        int LevelReached;

        #region Singleton
        void Awake()
        {
            instance = this;
        }
        #endregion

        void Start()
        {
            LevelReached = PlayerPrefs.GetInt("LevelReached");
        }
        public void LevelCompleted()
        {
            if (LevelReached < NextLevelToUnlock)
            {
                PlayerPrefs.SetInt("LevelReached", NextLevelToUnlock);
            }
        }
    }
}
