using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace MobileActionKit
{
    public class ItemInventoryManager : MonoBehaviour
    {
        public static ItemInventoryManager Instance;

        [TextArea]
        public string ScriptInfo = "Handles item inventory, including saving, retrieving, and updating item counts.It manages item availability, purchases, and upgrades.";
        [Space(10)]

        [HideInInspector]
        int ItemData;
        [HideInInspector]
        int UpgradeData;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        public void ShowPurchaseInfo(TextMeshProUGUI[] Info, string ItemName)
        {
            ItemData = PlayerPrefs.GetInt(ItemName, 0) + PlayerPrefs.GetInt(ItemName + "TemporaryStored");
            for (int y = 0; y < Info.Length; y++)
            {
                Info[y].text = ItemData.ToString();
            }
        }
        public int PurchasableItem(string ItemName)
        {
            ItemData = PlayerPrefs.GetInt(ItemName, 0) + PlayerPrefs.GetInt(ItemName + "TemporaryStored");
            return ItemData;
        }
        public void UpdatePurchaseInfo(TextMeshProUGUI[] Info, string ItemName)
        {
            ItemData = PlayerPrefs.GetInt(ItemName, 0);
            if (ItemData <= 0)
            {
                PlayerPrefs.SetInt(ItemName, 0);
                int TempData = PlayerPrefs.GetInt(ItemName + "TemporaryStored", 0);
                PlayerPrefs.SetInt(ItemName + "TemporaryStored", --TempData);
                int Amount = PlayerPrefs.GetInt(ItemName) + PlayerPrefs.GetInt(ItemName + "TemporaryStored");
                for (int y = 0; y < Info.Length; y++)
                {
                    Info[y].text = Amount.ToString();
                }
            }
            else
            {
                PlayerPrefs.SetInt(ItemName, --ItemData);  
                int TempData = PlayerPrefs.GetInt(ItemName + "TemporaryStored", 0);
                int Amount = PlayerPrefs.GetInt(ItemName) + PlayerPrefs.GetInt(ItemName + "TemporaryStored");
                for (int y = 0; y < Info.Length; y++)
                {
                    Info[y].text = Amount.ToString();
                }
            }

        }
        public int UpgradableItem(string ItemName)
        {
            UpgradeData = PlayerPrefs.GetInt(ItemName + "LastUpgrade", 0);
            return UpgradeData;
        }


    }
}