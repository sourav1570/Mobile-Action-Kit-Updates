using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    [RequireComponent(typeof(WinningProperties))]
    [RequireComponent(typeof(LevelToUnlock))]
    public class WinningConditions : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script defines different win conditions for a level, such as reaching a location, killing all enemies, or a combination of both. It checks enemy status and triggers level completion when conditions are met.";

        [Tooltip("The team ID of the enemies that must be eliminated to win the level.")]
        public string EnemiesTeamID;

        [System.Serializable]
        public enum WinConditionEnum
        {
            ReachLocationToCompleteLevel,
            ReachLocationAndKillAllEnemies,
            KillAllEnemies
        }

        [Tooltip("Defines the win condition for this level.")]
        public WinConditionEnum WinCondition;

        [Tooltip("Time interval (in seconds) for checking if all enemies are eliminated.")]
        public float TimeToCheckForEnemies = 2f;

        [Tooltip("List of enemy targets currently active in the scene.")]
        public List<Targets> DisplayEnemies = new List<Targets>();

        GameObject Player;

        WinningProperties WinningPropertiesScript;


        private void Start()
        {         
            WinningPropertiesScript = GetComponent<WinningProperties>();

            if (WinCondition == WinConditionEnum.KillAllEnemies || WinCondition == WinConditionEnum.ReachLocationAndKillAllEnemies)
            {
                InvokeRepeating("ShowGameResults", TimeToCheckForEnemies, TimeToCheckForEnemies);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag == "Player")
            {
                if (WinCondition == WinConditionEnum.ReachLocationToCompleteLevel &&  WinCondition == WinConditionEnum.ReachLocationAndKillAllEnemies)
                {
                    Targets[] Enemies = FindObjectsOfType<Targets>();
                    DisplayEnemies.Clear(); 
                    for (int i = 0; i < Enemies.Length; i++)
                    {
                        if (Enemies[i].GetComponent<Targets>().MyTeamID == EnemiesTeamID)
                        {
                            DisplayEnemies.Add(Enemies[i]);
                        }
                    }
                    if (DisplayEnemies.Count <= 0)
                    {
                        WinningPropertiesScript.WinStuff();
                        Time.timeScale = 0f;
                    }
                }
                else if (WinCondition == WinConditionEnum.ReachLocationToCompleteLevel)
                {
                    //Friendlies[] Enemies = FindObjectsOfType<Friendlies>();
                    //Team1List.Clear(); Team2List.Clear();
                    //for (int i = 0; i < Enemies.Length; i++)
                    //{
                    //    if (Enemies[i].GetComponent<Friendlies>().FriendlyTeamTag == TeamName.FriendlyTeamTag)
                    //    {
                    //        Team1List.Add(Enemies[i]);
                    //    }
                    //    else
                    //    {
                    //        Team2List.Add(Enemies[i]);
                    //        Enemies[i].gameObject.SetActive(false);
                    //    }
                    //}
                    WinningPropertiesScript.WinStuff();
                    Time.timeScale = 0f;
                }
            }
        }
        void ShowGameResults() // Check For Enemies
        {
            Targets[] Enemies = FindObjectsOfType<Targets>();
            DisplayEnemies.Clear();  
            for (int i = 0; i < Enemies.Length; i++)
            {
                if (Enemies[i].GetComponent<Targets>().MyTeamID == EnemiesTeamID)
                {
                    DisplayEnemies.Add(Enemies[i]);
                }
 
            }
            if (DisplayEnemies.Count <= 0)
            {
                WinningPropertiesScript.WinStuff();
            }
        }
    }
}