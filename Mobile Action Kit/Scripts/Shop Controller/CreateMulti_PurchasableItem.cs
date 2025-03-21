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
    public class CreateMultiItemClass
    {
        [Tooltip("Enter the unique name in this field 'Item Name' to be used for saving the data in the game.")]
        public string ItemName;

        [Tooltip("Enter the price for this item.")]
        public int ItemPrice;

        [Tooltip("Drag and drop the TextMeshProUGUI text for this item from the hierarchy into this field.")]
        public TextMeshProUGUI DisplayPriceText;

        [Tooltip("Drag and drop mono behaviour script from the hierarchy into this field. and choose the function to call when this item is purchased.")]
        public MonoBehaviour AddScript;

        [Tooltip("Select the function to call when this item is purchased.")]
        public string FunctionToInvokeWhenPurchased;
        [Tooltip("Select the function to call when this item is purchased and the game restarts.")]
        public string FunctionToInvokeWhenPurchasedAndGameRestart;

        [Tooltip("Drag and drop the TextMeshProUGUI text for this item from the hierarchy into this field.")]
        public TextMeshProUGUI NumberOfPurchasesText;

        [Tooltip("Display here the number of purchases the player has made.")]
        public int DebugNumberOfPurchases;

        [Tooltip("Drag and drop the UI button for this item from the hierarchy into this field.")]
        public Button UIButton;
    }
    public class CreateMulti_PurchasableItem : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script helps create Multi purchasable item for the shop menu.Basically an item which can be purchased multiple times. For example : Health packs";
        [HideInInspector]
        public List<CreateMultiItemClass> CreateItem = new List<CreateMultiItemClass>();
        int CurrentCoins;

        private void Start()
        {
            for (int x = 0; x < CreateItem.Count; x++)
            {
                if (CreateItem[x].AddScript != null)
                {
                    MethodInfo method = CreateItem[x].AddScript.GetType().GetMethod(CreateItem[x].FunctionToInvokeWhenPurchasedAndGameRestart);
                    if (method != null)
                    {
                        // Check if the method is public and has no parameters
                        if (method.IsPublic && method.GetParameters().Length == 0)
                        {
                            method.Invoke(CreateItem[x].AddScript, null);
                        }
                        else
                        {
                            Debug.LogError("Function '" + CreateItem[x].FunctionToInvokeWhenPurchasedAndGameRestart + "' is not public or has parameters.");
                        }
                    }
                }

                CreateItem[x].DisplayPriceText.text = CreateItem[x].ItemPrice.ToString();
                CreateItem[x].NumberOfPurchasesText.text = PlayerPrefs.GetInt(CreateItem[x].ItemName + "DisplayNumberOfPurchases", 0).ToString();
                CreateItem[x].DebugNumberOfPurchases = PlayerPrefs.GetInt(CreateItem[x].ItemName + "DisplayNumberOfPurchases", 0);

                int index = x; // Store the current index in a local variable to avoid closure issues
                CreateItem[index].UIButton.onClick.AddListener(() => BuyItem(CreateItem[index].ItemName));
            }
        }

        public void BuyItem(string ItemName)
        {
            CurrentCoins = PlayerPrefs.GetInt("ShopMoney");

            for (int x = 0; x < CreateItem.Count; x++)
            {
                if (CurrentCoins >= CreateItem[x].ItemPrice && CreateItem[x].ItemName == ItemName)
                {
                    if (CreateItem[x].AddScript != null)
                    {
                        MethodInfo method = CreateItem[x].AddScript.GetType().GetMethod(CreateItem[x].FunctionToInvokeWhenPurchased);
                        if (method != null)
                        {
                            // Check if the method is public and has no parameters
                            if (method.IsPublic && method.GetParameters().Length == 0)
                            {
                                method.Invoke(CreateItem[x].AddScript, null);
                            }
                            else
                            {
                                Debug.LogError("Function '" + CreateItem[x].FunctionToInvokeWhenPurchased + "' is not public or has parameters.");
                            }
                        }
                    }

                    int NumberOfPurchase = PlayerPrefs.GetInt(ItemName + "DisplayNumberOfPurchases", 0);
                    PlayerPrefs.SetInt(ItemName + "DisplayNumberOfPurchases", ++NumberOfPurchase);
                    CreateItem[x].NumberOfPurchasesText.text = PlayerPrefs.GetInt(ItemName + "DisplayNumberOfPurchases", 0).ToString();

                    CreateItem[x].DebugNumberOfPurchases = PlayerPrefs.GetInt(CreateItem[x].ItemName + "DisplayNumberOfPurchases", 0);

                    CurrentCoins = CurrentCoins - CreateItem[x].ItemPrice;

                    if (GameCurrencyManager.instance != null)
                    {
                        GameCurrencyManager.instance.TotalGoldText.text = CurrentCoins.ToString();
                        PlayerPrefs.SetInt("ShopMoney", CurrentCoins);
                        CurrentCoins = PlayerPrefs.GetInt("ShopMoney");
                    }
                }
            }
        }
    }
}