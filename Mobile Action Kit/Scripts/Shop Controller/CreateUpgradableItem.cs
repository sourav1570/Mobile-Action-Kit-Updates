using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Reflection;


namespace MobileActionKit
{
    [Serializable]
    public class CreateUpgradableItemClass
    {
        [Tooltip("Enter the price for this upgrade.")]
        public int PriceTag;
        [Tooltip("Drag and drop mono behaviour script from the hierarchy into this field. and choose the function to call when this item is purchased/Upgraded.")]
        public MonoBehaviour AddScript;
        [Tooltip("Select the function to call when this item is purchased.")]
        public string FunctionToInvokeWhenPurchased;
        [Tooltip("Select the function to call when this item is purchased and the game restarts.")]
        public string FunctionToInvokeWhenPurchasedAndGameRestart;

        [Tooltip("Message to display the benefits of this new upgrade.")]
        public string UpgradeMessage = "upgrade to increase ammo storage";
    }
    public class CreateUpgradableItem : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script helps create upgradable item for the shop menu. For example : upgradable item for the player weapons ammo pouch.";
        [Tooltip("Enter the unique name in this field 'Item Name' to be used for saving the data in the game.")]
        public string ItemName;
        [Tooltip("Drag and drop the UI button for this item from the hierarchy into this field.")]
        public Button UIButton;

        [Tooltip("Enter the words to display before displaying price to the text for example : Buy,Purchase.")]
        public string TextBeforePrice = "For example : Buy,Purchase";

        [Tooltip("Drag and drop the TextMeshProUGUI text for this item from the hierarchy into this field.")]
        public TextMeshProUGUI DisplayPriceText;

        public Image GoldBarImageToDeactivate;

        [Tooltip("Drag and drop the TextMeshProUGUI text for this item from the hierarchy into this field. The text will than display the benefits of the upgrade for example : Upgrade to store 200 Ammo.")]
        public TextMeshProUGUI UpgradeMessageText;

        [Tooltip("Enter the words to display on 'DisplayPriceText' after the item upgrade is reach to the Max for example : Upgraded to the MAX.")]
        public string TextAfterFinalUpgradeIsCompleted = "MAX";

        [Tooltip("Enter the message to display after the item upgrade is reach to the Max for example : Fully Upgraded.")]
        public string UpgradeMessageTextAfterFinalUpgrade = "UPGRADED TO THE MAX";

        [HideInInspector]
        public List<CreateUpgradableItemClass> CreateItem = new List<CreateUpgradableItemClass>();
        int CurrentCoins;
        int NewPrice;

        private void Start()
        {
            UpgradeMessageText.text = PlayerPrefs.GetInt(ItemName + "UpgradeMessage", 0).ToString();
            int ItemUpgrade = PlayerPrefs.GetInt(ItemName + "UpgradeLevel", 0);

            if (CreateItem.Count > ItemUpgrade)
            {
                if (CreateItem[ItemUpgrade].AddScript != null)
                {
                    if (ItemUpgrade >= 1)
                    {
                        MethodInfo method = CreateItem[ItemUpgrade - 1].AddScript.GetType().GetMethod(CreateItem[ItemUpgrade - 1].FunctionToInvokeWhenPurchasedAndGameRestart);
                        if (method != null)
                        {
                            // Check if the method is public and has no parameters
                            if (method.IsPublic && method.GetParameters().Length == 0)
                            {
                                method.Invoke(CreateItem[ItemUpgrade - 1].AddScript, null);
                            }
                            else
                            {
                                Debug.LogError("Function '" + CreateItem[ItemUpgrade - 1].FunctionToInvokeWhenPurchasedAndGameRestart + "' is not public or has parameters.");
                            }
                        }
                    }

                }

                NewPrice = PlayerPrefs.GetInt(ItemName + "UpgradeLevel", 0);
                DisplayPriceText.text = TextBeforePrice + CreateItem[NewPrice].PriceTag;
                UpgradeMessageText.text = CreateItem[ItemUpgrade].UpgradeMessage.ToString();
            }
            else
            {
                if(CreateItem.Count > 0)
                {
                    ItemUpgrade = CreateItem.Count - 1;
                    if (CreateItem[ItemUpgrade].AddScript != null)
                    {
                        MethodInfo method = CreateItem[ItemUpgrade].AddScript.GetType().GetMethod(CreateItem[ItemUpgrade].FunctionToInvokeWhenPurchasedAndGameRestart);
                        if (method != null)
                        {
                            // Check if the method is public and has no parameters
                            if (method.IsPublic && method.GetParameters().Length == 0)
                            {
                                method.Invoke(CreateItem[ItemUpgrade].AddScript, null);
                            }
                            else
                            {
                                Debug.LogError("Function '" + CreateItem[ItemUpgrade].FunctionToInvokeWhenPurchasedAndGameRestart + "' is not public or has parameters.");
                            }
                        }
                    }

                    DisplayPriceText.text = TextAfterFinalUpgradeIsCompleted;
                    UpgradeMessageText.text = UpgradeMessageTextAfterFinalUpgrade;
                    if (GoldBarImageToDeactivate != null)
                    {
                        GoldBarImageToDeactivate.gameObject.SetActive(false);
                    }

                }
                
            }

            UIButton.onClick.AddListener(() => BuyItem());
        }

        public void BuyItem()
        {
            CurrentCoins = PlayerPrefs.GetInt("ShopMoney");
            int CurrentIndex = PlayerPrefs.GetInt(ItemName + "UpgradeLevel", 0);
            if (CreateItem.Count > CurrentIndex)
            {
                NewPrice = CreateItem[CurrentIndex].PriceTag;

                if (CurrentCoins >= CreateItem[CurrentIndex].PriceTag)
                {
                    if (CreateItem[CurrentIndex].AddScript != null)
                    {
                        MethodInfo method = CreateItem[CurrentIndex].AddScript.GetType().GetMethod(CreateItem[CurrentIndex].FunctionToInvokeWhenPurchased);
                        if (method != null)
                        {
                            // Check if the method is public and has no parameters
                            if (method.IsPublic && method.GetParameters().Length == 0)
                            {
                                method.Invoke(CreateItem[CurrentIndex].AddScript, null);
                            }
                            else
                            {
                                Debug.LogError("Function '" + CreateItem[CurrentIndex].FunctionToInvokeWhenPurchased + "' is not public or has parameters.");
                            }
                        }
                    }

                    int ItemUpgrade = PlayerPrefs.GetInt(ItemName + "UpgradeLevel", 0);

                    PlayerPrefs.SetInt(ItemName + "UpgradeLevel", ++ItemUpgrade);

                    int UpgradeLevel = PlayerPrefs.GetInt(ItemName + "UpgradeLevel", 0);
                    if (CreateItem.Count > UpgradeLevel)
                    {
                        NewPrice = PlayerPrefs.GetInt(ItemName + "UpgradeLevel", 0);
                        DisplayPriceText.text = TextBeforePrice + CreateItem[NewPrice].PriceTag;
                        UpgradeMessageText.text = CreateItem[ItemUpgrade].UpgradeMessage.ToString();
                        CurrentCoins = CurrentCoins - CreateItem[CurrentIndex].PriceTag;

                        if (GameCurrencyManager.instance != null)
                        {
                            GameCurrencyManager.instance.TotalGoldText.text = CurrentCoins.ToString();
                            PlayerPrefs.SetInt("ShopMoney", CurrentCoins);
                            CurrentCoins = PlayerPrefs.GetInt("ShopMoney");
                        }
                    }
                    else
                    {
                        DisplayPriceText.text = TextAfterFinalUpgradeIsCompleted;
                        UpgradeMessageText.text = UpgradeMessageTextAfterFinalUpgrade;
                        if(GoldBarImageToDeactivate != null)
                        {
                            GoldBarImageToDeactivate.gameObject.SetActive(false);
                        }
                    }

                }
            }
        }
    }
}