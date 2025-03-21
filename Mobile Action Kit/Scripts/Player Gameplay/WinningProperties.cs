using UnityEngine;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class WinningProperties : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script handles the mission success mechanics. It calculates and displays bonuses for headshots and body kills, updates the total cash won, and manages UI activation upon mission completion.";

        public static WinningProperties instance;

        [Tooltip("UI text element displaying bonus earned from body kills.")]
        public Text BodyKillsBonusText;

        [Tooltip("UI text element displaying bonus earned from headshots.")]
        public Text HeadshotBonusText;

        [Tooltip("UI text element displaying total cash earned.")]
        public Text TotalCashEarnedText;

        [Tooltip("Bonus amount awarded per headshot.")]
        public int BonusPerHeadShot = 100;

        [Tooltip("Bonus amount awarded per body kill.")]
        public int BonusPerBodyKill = 50;

        [Tooltip("Gameobjects to activate when the mission is successfully completed.")]
        public GameObject[] GameObjectsToActivateOnMissionSuccess;

        [Tooltip("Gameobjects to deactivate when the mission is successfully completed.")]
        public GameObject[] GameObjectsToDeactivateOnMissionSuccess;

        [Header("Debug Values")]
        [Tooltip("Number of body kills in the current mission.")]
        public int ShowTotalBodyKills = 0;

        [Tooltip("Number of headshots in the current mission.")]
        public int ShowTotalHeadShots = 0;

        [Tooltip("Total bonus earned from headshots.")]
        public int TotatHeadShotBonusRecieved;

        [Tooltip("Total bonus earned from body kills.")]
        public int TotalBodyKillsBonusRecieved;

        [Tooltip("Total cash won in the mission.")]
        public int TotalCashEarned = 0;

        bool Playaudio = false;

        [HideInInspector]
        public bool IsLevelCompleted = false;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        void Start()
        {
            HeadshotBonusText.text = BonusPerHeadShot.ToString();
        }
        public void WinStuff()
        {
            TotalCashEarned = TotatHeadShotBonusRecieved + TotalBodyKillsBonusRecieved;
            PlayerPrefs.SetInt("TotalCash", TotalCashEarned);
            TotalCashEarnedText.text = TotalCashEarned.ToString();
            HeadshotBonusText.text = TotatHeadShotBonusRecieved.ToString();
            BodyKillsBonusText.text = TotalBodyKillsBonusRecieved.ToString();

            if (PlayerHealth.instance != null)
            {
                PlayerHealth.instance.gameObject.GetComponent<PlayerHealth>().enabled = false;
            }

            for (int x = 0; x < GameObjectsToActivateOnMissionSuccess.Length; x++)
            {
                GameObjectsToActivateOnMissionSuccess[x].SetActive(true);
            }
            for (int x = 0; x < GameObjectsToDeactivateOnMissionSuccess.Length; x++)
            {
                GameObjectsToDeactivateOnMissionSuccess[x].SetActive(false);
            }
            PlayerPrefs.SetInt("Levelmenuactivation", 1);
            if(LevelToUnlock.instance != null)
            {
                LevelToUnlock.instance.LevelCompleted();
            } 
            IsLevelCompleted = true;
        }
    }
}