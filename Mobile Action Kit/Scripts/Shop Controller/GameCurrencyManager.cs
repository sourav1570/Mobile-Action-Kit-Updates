using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace MobileActionKit
{
    public class GameCurrencyManager : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script manage the in game currency and display the total coins in the game.";

        public static GameCurrencyManager instance;

        [Tooltip("Drag and drop the TextMeshProUGUI text for this item from the hierarchy into this field.")]
        public TextMeshProUGUI TotalGoldText;

        [Tooltip("this field display the total cash in the game the player have.")]
        public int DebugTotalGoldBars;

        int CurrentCoins;

        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            CurrentCoins = PlayerPrefs.GetInt("ShopMoney");
            TotalGoldText.text = CurrentCoins.ToString();
        }
        public void UpdateCash(int Amount)
        {
            TotalGoldText.text = Amount.ToString();
        }
    }
}