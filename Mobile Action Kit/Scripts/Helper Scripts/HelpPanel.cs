using UnityEngine.UI;
using UnityEngine;

namespace MobileActionKit
{
    // This Script is Only Made For Testing Purpose And Can be usefull For Developer To Test Ai

    public class HelpPanel : MonoBehaviour
    {

        [Header("Panels and Fields")]
        public GameObject EnemiesChecker;
        public InputField NumberOfEnemiesToSpawnField;
        public InputField TimeToRespawnFieldA;
        public InputField TimeToRespawnFieldB;
        public InputField RangeField;
        public InputField MaxEnemiesField;
        public Dropdown EnemySelection;

        [Header("Viewing The Retrieved Data For Debug Purpose")]
        public int NumberofEnemies;
        public float TimeToRespawnA;
        public float TimeToRespawnB;
        public int Range;
        public int MaxEnemies;

        public void ApplyButton()
        {
            NumberofEnemies = int.Parse(NumberOfEnemiesToSpawnField.text);
            TimeToRespawnA = float.Parse(TimeToRespawnFieldA.text);
            TimeToRespawnB = float.Parse(TimeToRespawnFieldB.text);
            Range = int.Parse(RangeField.text);
            MaxEnemies = int.Parse(MaxEnemiesField.text);
            PlayerPrefs.SetFloat("TIMEDATARANDOMA", TimeToRespawnA);
            PlayerPrefs.SetFloat("TIMEDATARANDOMB", TimeToRespawnB);
            PlayerPrefs.SetInt("ENEMIESDATA", NumberofEnemies);
            PlayerPrefs.SetInt("RADIUS", Range);
            PlayerPrefs.SetInt("MAXENEMIES", MaxEnemies);
            EnemiesChecker.SetActive(false);
        }
        public void OnEnemyTypeSelection(int EnemyType)
        {
            PlayerPrefs.SetInt("EnemyType", EnemyType);
        }
    }
}