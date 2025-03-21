using UnityEngine;

namespace MobileActionKit
{
    public class DeveloperMode : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This Script can be useful for developers testing the game or kit ";
        [Space(10)]

        [Tooltip("Add cash in the game for testing.")]
        public bool AddGoldBars;
        [Tooltip("Enter the amount to add.")]
        public int GoldBars;

        public void ResettingDescription()
        {
            ScriptInfo = "This Script can be useful for developers testing the game or kit ";
        }
        private void Start()
        {
            if (AddGoldBars == true)
            {
                PlayerPrefs.SetInt("ShopMoney", GoldBars);
                if(GameCurrencyManager.instance != null)
                {
                    GameCurrencyManager.instance.TotalGoldText.text = PlayerPrefs.GetInt("ShopMoney").ToString();
                }
            }
        }
        public void deletealldata()
        {
            PlayerPrefs.DeleteAll();
           
        }
    }
}
