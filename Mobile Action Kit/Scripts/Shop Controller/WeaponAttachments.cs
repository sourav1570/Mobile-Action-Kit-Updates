using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MobileActionKit
{
    public class WeaponAttachments : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script manages weapon attachments, allowing players to purchase, equip, and unequip them while ensuring only one attachment per category is equipped at a time.";

        [System.Serializable]
        public class InventoryItem
        {
            [Tooltip("Category ensures that only one item from this category can be equipped at a time.")]
            public string Category = "Scope";

            [Tooltip("The weapon this attachment belongs to.")]
            public GameObject Weapon;

            [Tooltip("The price of the attachment in in-game currency.")]
            public int ItemPrice = 500;

            [Tooltip("Button to purchase or equip the attachment.")]
            public Button BuyButton;

            [Tooltip("Unique key for saving purchase data.")]
            public string UniqueNameForSavingData = "AKScope";

            [Tooltip("Text displayed on the Buy button before purchase.")]
            public string BuyText = "Buy 500";

            [Tooltip("Text displayed on the Buy button when the item is equipped.")]
            public string EquippedText = "Equip";

            [Tooltip("Text displayed on the Buy button when the item is unequipped.")]
            public string UnequippedText = "Unequip";

            [Tooltip("Text component to display purchase state on the Buy button.")]
            public TextMeshProUGUI BuyButtonText;

            [Tooltip("Objects to activate once the item is purchased.")]
            public GameObject[] ObjectsToActivateAfterPurchase;

            [Tooltip("Objects to deactivate once the item is purchased.")]
            public GameObject[] ObjectsToDeactivateAfterPurchase;

            [HideInInspector] public bool isEquipped;
            [HideInInspector] public bool isPurchased;

            public string purchaseStatusKey;
            public string shopMoneyKey = "ShopMoney";
            public WeaponAttachments parentScript;

            public void Initialize(WeaponAttachments script)
            {
                parentScript = script;
                purchaseStatusKey = "WeaponPurchaseStatus_" + UniqueNameForSavingData;
                isPurchased = PlayerPrefs.GetInt(purchaseStatusKey, 0) == 1;
                isEquipped = PlayerPrefs.GetInt(UniqueNameForSavingData, 0) == 1;

                UpdateUI();
                if (BuyButton != null)
                {
                    BuyButton.onClick.AddListener(OnBuyButtonClick);
                }
            }

            public void UpdateUI()
            {
                if (isPurchased)
                {
                    BuyButtonText.text = isEquipped ? UnequippedText : EquippedText;
                    SetActiveObjects(isEquipped);
                }
                else
                {
                    BuyButtonText.text = BuyText;
                    SetActiveObjects(false);
                }
            }

            public void SetActiveObjects(bool activate)
            {
                foreach (var obj in ObjectsToActivateAfterPurchase)
                {
                    obj.SetActive(activate);
                }

                // Ensure ObjectsToDeactivateAfterPurchase remain inactive after purchase
                if (isPurchased)
                {
                    foreach (var obj in ObjectsToDeactivateAfterPurchase)
                    {
                        obj.SetActive(false);
                    }
                }
            }

            private void OnBuyButtonClick()
            {
                if (!isPurchased && parentScript.UseConfirmationWindow)
                {
                    parentScript.WeaponAttachmentsManagerComponent.ConfirmationWindowReset();
                    parentScript.currentItemToPurchase = this;
                    parentScript.ConfirmationText.text = parentScript.ConfirmationMessage;

                    if (parentScript.ShowItemPrice)
                    {
                        parentScript.BuyText.text = ItemPrice.ToString();
                    }
                    parentScript.confirmationWindow.SetActive(true);

                }
                else
                {
                    parentScript.EquipItem(this);
                }
            }
        }

        public List<InventoryItem> inventoryItems = new List<InventoryItem>();

        public WeaponAttachmentsManager WeaponAttachmentsManagerComponent;
        public bool UseConfirmationWindow = true;
        public GameObject confirmationWindow;
        public TextMeshProUGUI ConfirmationText;
        public string ConfirmationMessage = "Do you want to Proceed with the purchase";
        public bool ShowItemPrice = false;
        public TextMeshProUGUI BuyText;
        public Button confirmYesButton;
        public Button confirmNoButton;

        [HideInInspector]
        public InventoryItem currentItemToPurchase;

        private void Start()
        {
            foreach (var item in inventoryItems)
            {
                item.Initialize(this);
            }

            if (UseConfirmationWindow)
            {
                confirmYesButton.onClick.AddListener(ConfirmPurchase);
                confirmNoButton.onClick.AddListener(CancelPurchase);
                confirmationWindow.SetActive(false);
            }
        }

        private void ConfirmPurchase()
        {
            if (currentItemToPurchase != null)
            {
                currentItemToPurchase?.parentScript.EquipItem(currentItemToPurchase);
                confirmationWindow.SetActive(false);
            }

        }

        private void CancelPurchase()
        {
            confirmationWindow.SetActive(false);
        }
        //public void EquipItem(InventoryItem itemToEquip)
        //{
        //    if (!itemToEquip.isPurchased)
        //    {
        //        int currentCash = PlayerPrefs.GetInt("ShopMoney", 0);
        //        if (currentCash >= itemToEquip.ItemPrice)
        //        {
        //            PlayerPrefs.SetInt(itemToEquip.purchaseStatusKey, 1);
        //            int newAmount = currentCash - itemToEquip.ItemPrice;
        //            InGameCurrencyManager.instance.UpdateCash(newAmount);
        //            PlayerPrefs.SetInt("ShopMoney", newAmount);

        //            itemToEquip.isPurchased = true;
        //        }
        //    }

        //    foreach (var item in inventoryItems)
        //    {
        //        if (item.Category == itemToEquip.Category) // Only check items in the same category
        //        {
        //            if (item == itemToEquip)
        //            {
        //                item.isEquipped = true;
        //                item.BuyButtonText.text = item.UnequippedText;
        //                item.SetActiveObjects(true);
        //                PlayerPrefs.SetInt(item.UniqueNameForSavingData, 1);
        //            }
        //            else if (item.isPurchased)
        //            {
        //                item.isEquipped = false;
        //                item.BuyButtonText.text = item.EquippedText;
        //                item.SetActiveObjects(false);
        //                PlayerPrefs.SetInt(item.UniqueNameForSavingData, 0);
        //            }
        //        }
        //    }

        //    PlayerPrefs.Save();
        //}
        public void EquipItem(InventoryItem itemToEquip)
        {
            if (!itemToEquip.isPurchased)
            {
                int currentCash = PlayerPrefs.GetInt("ShopMoney", 0);
                if (currentCash >= itemToEquip.ItemPrice)
                {
                    PlayerPrefs.SetInt(itemToEquip.purchaseStatusKey, 1);
                    int newAmount = currentCash - itemToEquip.ItemPrice;
                    GameCurrencyManager.instance.UpdateCash(newAmount);
                    PlayerPrefs.SetInt("ShopMoney", newAmount);

                    itemToEquip.isPurchased = true;
                }
            }

            if (itemToEquip.isEquipped)
            {
                // Unequip the item if it is already equipped
                itemToEquip.isEquipped = false;
                itemToEquip.BuyButtonText.text = itemToEquip.EquippedText;
                itemToEquip.SetActiveObjects(false);
                PlayerPrefs.SetInt(itemToEquip.UniqueNameForSavingData, 0);
            }
            else
            {
                // Equip the item and unequip other items in the same category
                foreach (var item in inventoryItems)
                {
                    if (item.Category == itemToEquip.Category)
                    {
                        if (item == itemToEquip)
                        {
                            item.isEquipped = true;
                            item.BuyButtonText.text = item.UnequippedText;
                            item.SetActiveObjects(true);
                            PlayerPrefs.SetInt(item.UniqueNameForSavingData, 1);
                        }
                        else if (item.isPurchased)
                        {
                            item.isEquipped = false;
                            item.BuyButtonText.text = item.EquippedText;
                            item.SetActiveObjects(false);
                            PlayerPrefs.SetInt(item.UniqueNameForSavingData, 0);
                        }
                    }
                }
            }

            PlayerPrefs.Save();
        }


    }


}





//public void EquipItem(InventoryItem itemToEquip)
//{
//    if (!itemToEquip.isPurchased)
//    {
//        int currentCash = PlayerPrefs.GetInt("ShopMoney", 0);
//        if (currentCash >= itemToEquip.ItemPrice)
//        {
//            PlayerPrefs.SetInt(itemToEquip.purchaseStatusKey, 1);
//            int newAmount = currentCash - itemToEquip.ItemPrice;
//            InGameCurrencyManager.instance.UpdateCash(newAmount);
//            PlayerPrefs.SetInt("ShopMoney", newAmount);

//            itemToEquip.isPurchased = true;
//        }
//    }

//    foreach (var item in inventoryItems)
//    {
//        if (item == itemToEquip)
//        {
//            item.isEquipped = true;
//            item.BuyButtonText.text = item.UnequippedText;
//            item.SetActiveObjects(true);
//            PlayerPrefs.SetInt(item.UniqueNameForSavingData, 1);
//        }
//        else if (item.isPurchased)
//        {
//            item.isEquipped = false;
//            item.BuyButtonText.text = item.EquippedText;
//            item.SetActiveObjects(false);
//            PlayerPrefs.SetInt(item.UniqueNameForSavingData, 0);
//        }
//    }

//    PlayerPrefs.Save();
//}