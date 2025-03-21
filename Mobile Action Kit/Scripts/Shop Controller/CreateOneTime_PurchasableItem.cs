using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
using System;


namespace MobileActionKit
{
    [Serializable]
    public class CreateOneTimePurchaseItemClass
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

        [Tooltip("Drag and drop the UI button for this item from the hierarchy into this field.")]
        public Button UIButton;
    }
    public class CreateOneTime_PurchasableItem : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script helps create one time purchasable item for the shop menu. For example : Unlock New Map,Unlock player skill";
        [HideInInspector]
        public List<CreateOneTimePurchaseItemClass> CreateItem = new List<CreateOneTimePurchaseItemClass>();
        int CurrentCoins;

        private void Start()
        {
            for (int x = 0; x < CreateItem.Count; x++)
            {
                if (PlayerPrefs.GetInt(CreateItem[x].ItemName + "OneTimePurchase") == 1)
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
                }

                CreateItem[x].DisplayPriceText.text = CreateItem[x].ItemPrice.ToString();
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

                    CurrentCoins = CurrentCoins - CreateItem[x].ItemPrice;

                    PlayerPrefs.SetInt(CreateItem[x].ItemName + "OneTimePurchase", 1);

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
