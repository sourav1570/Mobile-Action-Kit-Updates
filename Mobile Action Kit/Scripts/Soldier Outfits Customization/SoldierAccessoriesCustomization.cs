using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class SoldierAccessoriesCustomization : MonoBehaviour
    {
        [TextArea]
        public string scriptInfo = "SoldierAccessoriesCustomisation manages purchasing, previewing, and equipping soldier accessories.";

        [System.Serializable]
        public class Item
        {
            [Tooltip("Unique identifier for the item.")]
            public string UniqueID;

            [Tooltip("UI text displaying the item's price.")]
            public TextMeshProUGUI PriceText;

            [Tooltip("Button to purchase the item.")]
            public Button PurchaseButton;

            [Tooltip("Button to preview the item before purchase.")]
            public Button PreviewButton;

            [Tooltip("Toggle for equipping the item.")]
            public Toggle EquipToggle;

            [Tooltip("GameObject representing purchase-related UI elements.")]
            public GameObject PurchaseObject;

            [Tooltip("List of GameObjects that get activated when the item is equipped.")]
            public GameObject[] ItemsToActivate;

            [Tooltip("Price of the item.")]
            public int Price;

            [Tooltip("Event triggered when the item is equipped.")]
            public UnityEvent onEquipFunction;
        }

        [Tooltip("List of available items for customization.")]
        public List<Item> itemList;

        [Tooltip("If true, a default item is automatically equipped.")]
        public bool EnableDefaultAccessory = false;

        [Tooltip("Index of the default item in the itemList.")]
        public int defaultItemIndex = 0;

        private string cashKey = "ShopMoney";
        private bool isProgrammaticToggleChange = false;

        [Tooltip("Enable or disable the purchase confirmation window.")]
        public bool UseConfirmationWindow = true;

        [Tooltip("The UI panel for confirming a purchase.")]
        public GameObject confirmationWindow;

        [Tooltip("Text UI element displaying the confirmation message.")]
        public TextMeshProUGUI ConfirmationText;

        [Tooltip("Message displayed in the confirmation window.")]
        public string ConfirmationMessage = "Do you want to proceed with the purchase?";

        [Tooltip("If true, the item's price will be displayed in the confirmation window.")]
        public bool ShowItemPrice = false;

        [Tooltip("UI text element displaying the price in the confirmation window.")]
        public TextMeshProUGUI BuyText;

        [Tooltip("Button to confirm the purchase.")]
        public Button confirmYesButton;

        [Tooltip("Button to cancel the purchase.")]
        public Button confirmNoButton;

        private Item currentItemToPurchase;
        private Item equippedItem;

        private void Start()
        {
            foreach (var item in itemList)
            {
                item.PriceText.text = item.Price.ToString();

                item.PurchaseButton.onClick.AddListener(() => ShowConfirmationWindow(item));
                if (item.PreviewButton != null)
                {
                    item.PreviewButton.onClick.AddListener(() => PreviewItem(item));
                }
                item.EquipToggle.onValueChanged.AddListener((isOn) =>
                {
                    if (!isProgrammaticToggleChange)
                    {
                        EquipItem(item, isOn);
                    }
                });

                bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;
                bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;

                if (isPurchased)
                {
                    item.PurchaseObject.SetActive(false);
                    item.PriceText.gameObject.SetActive(false);
                    //if (item.PreviewButton != null)
                    //{
                    //    item.PreviewButton.gameObject.SetActive(false);
                    //}
                }
                else
                {
                    item.PurchaseObject.SetActive(true);
                    item.PriceText.gameObject.SetActive(true);
                    if (item.PreviewButton != null)
                    {
                        item.PreviewButton.gameObject.SetActive(true);
                    }
                }

                item.EquipToggle.isOn = isEquipped;
                item.EquipToggle.interactable = false;
            }

            EnsureEquippedState();

            confirmYesButton.onClick.AddListener(ConfirmPurchase);
            confirmNoButton.onClick.AddListener(CancelPurchase);
            confirmationWindow.SetActive(false);
        }

        private void ShowConfirmationWindow(Item item)
        {
            bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;

            if (isPurchased)
            {
                bool isCurrentlyEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;
                EquipItem(item, !isCurrentlyEquipped);
            }
            else
            {
                if (UseConfirmationWindow)
                {
                    currentItemToPurchase = item;
                    ConfirmationText.text = ConfirmationMessage;
                    if (ShowItemPrice)
                    {
                        BuyText.text = currentItemToPurchase.Price.ToString();
                    }
                    confirmationWindow.SetActive(true);
                }
                else
                {
                    PurchaseItem(item);
                }
            }
        }

        private void ConfirmPurchase()
        {
            PurchaseItem(currentItemToPurchase);
            confirmationWindow.SetActive(false);
        }

        private void CancelPurchase()
        {
            if (currentItemToPurchase != null && PlayerPrefs.GetInt(currentItemToPurchase.UniqueID + "_Purchased", 0) == 0)
            {
                ActivateItems(currentItemToPurchase.ItemsToActivate, false); // Deactivate previewed items
                currentItemToPurchase.EquipToggle.isOn = false; // Reset the toggle state
            }

            confirmationWindow.SetActive(false);
        }

        private void EnsureEquippedState()
        {
            if (EnableDefaultAccessory)
            {
                Item defaultItem = itemList[defaultItemIndex];
                defaultItem.EquipToggle.isOn = true;
                ActivateItems(defaultItem.ItemsToActivate, true);
                defaultItem.PurchaseObject.SetActive(false);
                defaultItem.PriceText.gameObject.SetActive(false);
            }

            foreach (var item in itemList)
            {
                if (PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1)
                {
                    bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;
                    EquipItem(item, isEquipped);
                }
            }
        }

        private void PurchaseItem(Item item)
        {
            int currentCash = PlayerPrefs.GetInt(cashKey, 0);

            if (currentCash >= item.Price && PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 0)
            {
                int newAmount = currentCash - item.Price;
                GameCurrencyManager.instance.UpdateCash(newAmount);
                PlayerPrefs.SetInt(cashKey, newAmount);

                item.PurchaseObject.SetActive(false);
                item.PriceText.gameObject.SetActive(false);
                PlayerPrefs.SetInt(item.UniqueID + "_Purchased", 1);

                EquipItem(item, true);

                //if (item.PreviewButton != null)
                //{
                //    item.PreviewButton.gameObject.SetActive(false);
                //}
            }
            else if (PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 1)
            {
                bool isCurrentlyEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;
                EquipItem(item, !isCurrentlyEquipped);

                //if (item.PreviewButton != null)
                //{
                //    item.PreviewButton.gameObject.SetActive(false);
                //}
            }
            else
            {
                Debug.Log("Not enough cash.");
            }
        }

        private void EquipItem(Item item, bool isOn)
        {
            if (isOn)
            {
                ActivateItems(item.ItemsToActivate, true);
                isProgrammaticToggleChange = true;
                item.EquipToggle.isOn = true;
                isProgrammaticToggleChange = false;
                PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 1);

                item.onEquipFunction?.Invoke();
            }
            else
            {
                ActivateItems(item.ItemsToActivate, false);
                isProgrammaticToggleChange = true;
                item.EquipToggle.isOn = false;
                isProgrammaticToggleChange = false;
                PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 0);
            }
        }

        //private void PreviewItem(Item item)
        //{
        //    if (PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 0)
        //    {
        //        if (equippedItem != null)
        //        {
        //            ActivateItems(equippedItem.ItemsToActivate, false);
        //        }

        //        ActivateItems(item.ItemsToActivate, true);
        //        item.EquipToggle.isOn = false;
        //        equippedItem = item;
        //    }

        //}
        private void PreviewItem(Item item)
        {
            if (PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 0)
            {
                // Check if the equipped item is the same as the one being previewed
                if (equippedItem != null && equippedItem != item && !equippedItem.EquipToggle.isOn)
                {
                    // Deactivate previously equipped item only if its toggle is off
                    ActivateItems(equippedItem.ItemsToActivate, false);
                }

                // Activate previewed item and set it as the current preview item
                ActivateItems(item.ItemsToActivate, true);
                item.EquipToggle.isOn = false; // Toggle stays off to indicate preview
                equippedItem = item; // Set as the current previewed item
            }
        }


        private void ActivateItems(GameObject[] items, bool isActive)
        {
            foreach (var item in items)
            {
                item.SetActive(isActive);
            }
        }
    }
}


//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.UI;

//public class AvatarAccessoriesCustomisation : MonoBehaviour
//{
//    [System.Serializable]
//    public class Item
//    {
//        public string UniqueID;
//        public TextMeshProUGUI PriceText;
//        public Button PurchaseButton;
//        public Button PreviewButton; // New field for Preview Button
//        public Toggle EquipToggle;
//        public GameObject PurchaseObject;
//        public GameObject[] ItemsToActivate;
//        public int Price;

//        public UnityEvent onEquipFunction; // Public function that will be called
//    }

//    public List<Item> itemList;
//    public bool UseDefaultItem = false; // Public field to use default item
//    public int defaultItemIndex = 0; // Set the default item using this public integer field

//    private string cashKey = "ShopMoney"; // Updated cash key
//    private bool isProgrammaticToggleChange = false; // Flag to track programmatic toggle changes

//    // New field for the confirmation window
//    public bool UseConfirmationWindow = true; // New boolean for confirmation window
//    public GameObject confirmationWindow;
//    public TextMeshProUGUI ConfirmationText;
//    public string ConfirmationMessage = "Do you want to Proceed with the purchase";
//    public bool ShowItemPrice = false;
//    public TextMeshProUGUI BuyText;
//    public Button confirmYesButton;
//    public Button confirmNoButton;

//    private Item currentItemToPurchase; // Stores the item awaiting confirmation
//    private Item equippedItem; // Track currently equipped item

//    private void Start()
//    {
//        foreach (var item in itemList)
//        {
//            item.PriceText.text = item.Price.ToString(); // Display price text

//            item.PurchaseButton.onClick.AddListener(() => ShowConfirmationWindow(item));
//            if (item.PreviewButton != null)
//            {
//                item.PreviewButton.onClick.AddListener(() => PreviewItem(item)); // Add listener for Preview Button
//            }
//            item.EquipToggle.onValueChanged.AddListener((isOn) =>
//            {
//                if (!isProgrammaticToggleChange)
//                {
//                    EquipItem(item, isOn);
//                }
//            });

//            bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;
//            bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;

//            if (isPurchased)
//            {
//                item.PurchaseObject.SetActive(false);
//                item.PriceText.gameObject.SetActive(false);
//                if (item.PreviewButton != null)
//                {
//                    item.PreviewButton.gameObject.SetActive(false); // Disable Preview Button if purchased
//                }
//            }
//            else
//            {
//                item.PurchaseObject.SetActive(true);
//                item.PriceText.gameObject.SetActive(true);
//                if (item.PreviewButton != null)
//                {
//                    item.PreviewButton.gameObject.SetActive(true); // Enable Preview Button if not purchased
//                }
//            }

//            item.EquipToggle.isOn = isEquipped;
//            item.EquipToggle.interactable = false; // Make toggle non-interactable
//        }

//        EnsureEquippedState();

//        // Setup confirmation window buttons
//        confirmYesButton.onClick.AddListener(ConfirmPurchase);
//        confirmNoButton.onClick.AddListener(CancelPurchase);
//        confirmationWindow.SetActive(false); // Hide confirmation window at start
//    }

//    private void ShowConfirmationWindow(Item item)
//    {
//        bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;

//        if (isPurchased)
//        {
//            // If already purchased, directly toggle equipped state
//            bool isCurrentlyEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;
//            EquipItem(item, !isCurrentlyEquipped);
//        }
//        else
//        {
//            // If not purchased, either show the confirmation window or call Purchase directly
//            if (UseConfirmationWindow)
//            {
//                currentItemToPurchase = item;
//                ConfirmationText.text = ConfirmationMessage;
//                if (ShowItemPrice == true)
//                {
//                    BuyText.text = currentItemToPurchase.Price.ToString();
//                }
//                confirmationWindow.SetActive(true);
//            }
//            else
//            {
//                PurchaseItem(item);
//            }
//        }
//    }

//    private void ConfirmPurchase()
//    {
//        PurchaseItem(currentItemToPurchase);
//        confirmationWindow.SetActive(false);
//    }

//    private void CancelPurchase()
//    {
//        confirmationWindow.SetActive(false);
//    }

//    private void EnsureEquippedState()
//    {
//        if (UseDefaultItem)
//        {
//            Item defaultItem = itemList[defaultItemIndex];
//            defaultItem.EquipToggle.isOn = true;
//            ActivateItems(defaultItem.ItemsToActivate, true);
//            defaultItem.PurchaseObject.SetActive(false);
//            defaultItem.PriceText.gameObject.SetActive(false);
//        }

//        foreach (var item in itemList)
//        {
//            if (PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1)
//            {
//                bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;
//                EquipItem(item, isEquipped);
//            }
//        }
//    }

//    private void PurchaseItem(Item item)
//    {
//        int currentCash = PlayerPrefs.GetInt(cashKey, 0);

//        if (currentCash >= item.Price && PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 0)
//        {
//            int newAmount = currentCash - item.Price;
//            InGameCurrencyManager.instance.UpdateCash(newAmount);
//            PlayerPrefs.SetInt(cashKey, newAmount);

//            item.PurchaseObject.SetActive(false);
//            item.PriceText.gameObject.SetActive(false);
//            PlayerPrefs.SetInt(item.UniqueID + "_Purchased", 1);

//            EquipItem(item, true); // Equip immediately after purchase

//            if (item.PreviewButton != null)
//            {
//                item.PreviewButton.gameObject.SetActive(false); // Disable Preview Button if purchased
//            }
//        }
//        else if (PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 1)
//        {
//            bool isCurrentlyEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;
//            EquipItem(item, !isCurrentlyEquipped); // Toggle equip state

//            if (item.PreviewButton != null)
//            {
//                item.PreviewButton.gameObject.SetActive(false); // Disable Preview Button if purchased
//            }
//        }
//        else
//        {
//            Debug.Log("Not enough cash.");
//        }
//    }

//    private void EquipItem(Item item, bool isOn)
//    {
//        if (isOn)
//        {
//            ActivateItems(item.ItemsToActivate, true);
//            isProgrammaticToggleChange = true;
//            item.EquipToggle.isOn = true;
//            isProgrammaticToggleChange = false;
//            PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 1);

//            item.onEquipFunction?.Invoke();
//        }
//        else
//        {
//            ActivateItems(item.ItemsToActivate, false);
//            isProgrammaticToggleChange = true;
//            item.EquipToggle.isOn = false;
//            isProgrammaticToggleChange = false;
//            PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 0);
//        }
//    }

//    private void PreviewItem(Item item)
//    {
//        // Temporarily equip the item for preview, but don't save the state
//        if (equippedItem != null)
//        {
//            ActivateItems(equippedItem.ItemsToActivate, false); // Deactivate previously equipped item
//        }

//        ActivateItems(item.ItemsToActivate, true); // Activate preview item
//        item.EquipToggle.isOn = false; // Toggle stays OFF to indicate preview
//        equippedItem = item; // Update the currently previewed item
//    }

//    private void ActivateItems(GameObject[] items, bool isActive)
//    {
//        foreach (var item in items)
//        {
//            item.SetActive(isActive);
//        }
//    }
//}


//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.UI;

//public class AvatarAccessoriesCustomisation : MonoBehaviour
//{
//    [System.Serializable]
//    public class Item
//    {
//        public string UniqueID;
//        public TextMeshProUGUI PriceText;
//        public Button PurchaseButton;
//        public Button PreviewButton; // New field for Preview Button
//        public Toggle EquipToggle;
//        public GameObject PurchaseObject;
//        public GameObject[] ItemsToActivate;
//        public int Price;

//        public UnityEvent onEquipFunction; // Public function that will be called
//    }

//    public List<Item> itemList;
//    public bool UseDefaultItem = false; // Public field to use default item
//    public int defaultItemIndex = 0; // Set the default item using this public integer field

//    private string cashKey = "ShopMoney"; // Updated cash key
//    private bool isProgrammaticToggleChange = false; // Flag to track programmatic toggle changes

//    // New field for the confirmation window
//    public GameObject confirmationWindow;
//    public Button confirmYesButton;
//    public Button confirmNoButton;

//    private Item currentItemToPurchase; // Stores the item awaiting confirmation
//    private Item equippedItem; // Track currently equipped item

//    private void Start()
//    {
//        foreach (var item in itemList)
//        {
//            item.PriceText.text = item.Price.ToString(); // Display price text

//            item.PurchaseButton.onClick.AddListener(() => ShowConfirmationWindow(item));
//            if(item.PreviewButton != null)
//            {
//                item.PreviewButton.onClick.AddListener(() => PreviewItem(item)); // Add listener for Preview Button
//            }
//            item.EquipToggle.onValueChanged.AddListener((isOn) =>
//            {
//                if (!isProgrammaticToggleChange)
//                {
//                    EquipItem(item, isOn);
//                }
//            });

//            bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;
//            bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;

//            if (isPurchased)
//            {
//                item.PurchaseObject.SetActive(false);
//                item.PriceText.gameObject.SetActive(false);
//                if (item.PreviewButton != null)
//                {
//                    item.PreviewButton.gameObject.SetActive(false); // Disable Preview Button if purchased
//                }
//            }
//            else
//            {
//                item.PurchaseObject.SetActive(true);
//                item.PriceText.gameObject.SetActive(true);
//                if (item.PreviewButton != null)
//                {
//                    item.PreviewButton.gameObject.SetActive(true); // Disable Preview Button if purchased
//                }
//            }

//            item.EquipToggle.isOn = isEquipped;
//            item.EquipToggle.interactable = false; // Make toggle non-interactable
//        }

//        EnsureEquippedState();

//        // Setup confirmation window buttons
//        confirmYesButton.onClick.AddListener(ConfirmPurchase);
//        confirmNoButton.onClick.AddListener(CancelPurchase);
//        confirmationWindow.SetActive(false); // Hide confirmation window at start
//    }

//    private void ShowConfirmationWindow(Item item)
//    {
//        bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;

//        if (isPurchased)
//        {
//            // If already purchased, directly toggle equipped state
//            bool isCurrentlyEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;
//            EquipItem(item, !isCurrentlyEquipped);
//        }
//        else
//        {
//            // If not purchased, show the confirmation window
//            currentItemToPurchase = item;
//            confirmationWindow.SetActive(true);
//        }
//    }

//    private void ConfirmPurchase()
//    {
//        PurchaseItem(currentItemToPurchase);
//        confirmationWindow.SetActive(false);
//    }

//    private void CancelPurchase()
//    {
//        confirmationWindow.SetActive(false);
//    }

//    private void EnsureEquippedState()
//    {
//        if (UseDefaultItem)
//        {
//            Item defaultItem = itemList[defaultItemIndex];
//            defaultItem.EquipToggle.isOn = true;
//            ActivateItems(defaultItem.ItemsToActivate, true);
//            defaultItem.PurchaseObject.SetActive(false);
//            defaultItem.PriceText.gameObject.SetActive(false);
//        }

//        foreach (var item in itemList)
//        {
//            if (PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1)
//            {
//                bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;
//                EquipItem(item, isEquipped);
//            }
//        }
//    }

//    private void PurchaseItem(Item item)
//    {
//        int currentCash = PlayerPrefs.GetInt(cashKey, 0);

//        if (currentCash >= item.Price && PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 0)
//        {
//            int newAmount = currentCash - item.Price;
//            InGameCurrencyManager.instance.UpdateCash(newAmount);
//            PlayerPrefs.SetInt(cashKey, newAmount);

//            item.PurchaseObject.SetActive(false);
//            item.PriceText.gameObject.SetActive(false);
//            PlayerPrefs.SetInt(item.UniqueID + "_Purchased", 1);

//            EquipItem(item, true); // Equip immediately after purchase

//            if (item.PreviewButton != null)
//            {
//                item.PreviewButton.gameObject.SetActive(false); // Disable Preview Button if purchased
//            }
//        }
//        else if (PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 1)
//        {
//            bool isCurrentlyEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;
//            EquipItem(item, !isCurrentlyEquipped); // Toggle equip state

//            if (item.PreviewButton != null)
//            {
//                item.PreviewButton.gameObject.SetActive(false); // Disable Preview Button if purchased
//            }
//        }
//        else
//        {
//            Debug.Log("Not enough cash.");
//        }
//    }

//    private void EquipItem(Item item, bool isOn)
//    {
//        if (isOn)
//        {
//            ActivateItems(item.ItemsToActivate, true);
//            isProgrammaticToggleChange = true;
//            item.EquipToggle.isOn = true;
//            isProgrammaticToggleChange = false;
//            PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 1);

//            item.onEquipFunction?.Invoke();
//        }
//        else
//        {
//            ActivateItems(item.ItemsToActivate, false);
//            isProgrammaticToggleChange = true;
//            item.EquipToggle.isOn = false;
//            isProgrammaticToggleChange = false;
//            PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 0);
//        }
//    }

//    private void PreviewItem(Item item)
//    {
//        // Temporarily equip the item for preview, but don't save the state
//        if (equippedItem != null)
//        {
//            ActivateItems(equippedItem.ItemsToActivate, false); // Deactivate previously equipped item
//        }

//        ActivateItems(item.ItemsToActivate, true); // Activate preview item
//        item.EquipToggle.isOn = false; // Toggle stays OFF to indicate preview
//        equippedItem = item; // Update the currently previewed item
//    }

//    private void ActivateItems(GameObject[] items, bool isActive)
//    {
//        foreach (var item in items)
//        {
//            item.SetActive(isActive);
//        }
//    }
//}



//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.UI;

//public class AvatarAccessoriesCustomisation : MonoBehaviour
//{
//    [System.Serializable]
//    public class Item
//    {
//        public string UniqueID;
//        public TextMeshProUGUI PriceText;
//        public Button PurchaseButton;
//        public Toggle EquipToggle;
//        public GameObject PurchaseObject;
//        public GameObject[] ItemsToActivate;
//        public int Price;

//        public UnityEvent onEquipFunction; // Public function that will be called
//    }

//    public List<Item> itemList;
//    public bool UseDefaultItem = false; // Public field to use default item
//    public int defaultItemIndex = 0; // Set the default item using this public integer field

//    private string cashKey = "ShopMoney"; // Updated cash key
//    private bool isProgrammaticToggleChange = false; // Flag to track programmatic toggle changes

//    // New field for the confirmation window
//    public GameObject confirmationWindow;
//    public Button confirmYesButton;
//    public Button confirmNoButton;

//    private Item currentItemToPurchase; // Stores the item awaiting confirmation

//    private void Start()
//    {
//        foreach (var item in itemList)
//        {
//            item.PriceText.text = item.Price.ToString(); // Display price text

//            item.PurchaseButton.onClick.AddListener(() => ShowConfirmationWindow(item));
//            item.EquipToggle.onValueChanged.AddListener((isOn) =>
//            {
//                if (!isProgrammaticToggleChange)
//                {
//                    EquipItem(item, isOn);
//                }
//            });

//            bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;
//            bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;

//            if (isPurchased)
//            {
//                item.PurchaseObject.SetActive(false);
//                item.PriceText.gameObject.SetActive(false);
//            }
//            else
//            {
//                item.PurchaseObject.SetActive(true);
//                item.PriceText.gameObject.SetActive(true);
//            }

//            item.EquipToggle.isOn = isEquipped;
//            item.EquipToggle.interactable = false;
//        }

//        EnsureEquippedState();

//        // Setup confirmation window buttons
//        confirmYesButton.onClick.AddListener(ConfirmPurchase);
//        confirmNoButton.onClick.AddListener(CancelPurchase);
//        confirmationWindow.SetActive(false); // Hide confirmation window at start
//    }

//    private void ShowConfirmationWindow(Item item)
//    {
//        bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;

//        if (isPurchased)
//        {
//            // If already purchased, directly toggle equipped state
//            bool isCurrentlyEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;
//            EquipItem(item, !isCurrentlyEquipped);
//        }
//        else
//        {
//            // If not purchased, show the confirmation window
//            currentItemToPurchase = item;
//            confirmationWindow.SetActive(true);
//        }
//    }

//    private void ConfirmPurchase()
//    {
//        PurchaseItem(currentItemToPurchase);
//        confirmationWindow.SetActive(false);
//    }

//    private void CancelPurchase()
//    {
//        confirmationWindow.SetActive(false);
//    }

//    private void EnsureEquippedState()
//    {
//        if (UseDefaultItem)
//        {
//            Item defaultItem = itemList[defaultItemIndex];
//            defaultItem.EquipToggle.isOn = true;
//            ActivateItems(defaultItem.ItemsToActivate, true);
//            defaultItem.PurchaseObject.SetActive(false);
//            defaultItem.PriceText.gameObject.SetActive(false);
//        }

//        foreach (var item in itemList)
//        {
//            if (PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1)
//            {
//                bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;
//                EquipItem(item, isEquipped);
//            }
//        }
//    }

//    private void PurchaseItem(Item item)
//    {
//        int currentCash = PlayerPrefs.GetInt(cashKey, 0);

//        if (currentCash >= item.Price && PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 0)
//        {
//            int newAmount = currentCash - item.Price;
//            InGameCurrencyManager.instance.UpdateCash(newAmount);
//            PlayerPrefs.SetInt(cashKey, newAmount);

//            item.PurchaseObject.SetActive(false);
//            item.PriceText.gameObject.SetActive(false);
//            PlayerPrefs.SetInt(item.UniqueID + "_Purchased", 1);

//            EquipItem(item, true); // Equip immediately after purchase
//        }
//        else if (PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 1)
//        {
//            bool isCurrentlyEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;
//            EquipItem(item, !isCurrentlyEquipped); // Toggle equip state
//        }
//        else
//        {
//            Debug.Log("Not enough cash.");
//        }
//    }

//    private void EquipItem(Item item, bool isOn)
//    {
//        if (isOn)
//        {
//            ActivateItems(item.ItemsToActivate, true);
//            isProgrammaticToggleChange = true;
//            item.EquipToggle.isOn = true;
//            isProgrammaticToggleChange = false;
//            PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 1);

//            item.onEquipFunction?.Invoke();
//        }
//        else
//        {
//            ActivateItems(item.ItemsToActivate, false);
//            isProgrammaticToggleChange = true;
//            item.EquipToggle.isOn = false;
//            isProgrammaticToggleChange = false;
//            PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 0);
//        }
//    }

//    private void ActivateItems(GameObject[] items, bool isActive)
//    {
//        foreach (var item in items)
//        {
//            item.SetActive(isActive);
//        }
//    }
//}




//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.UI;

//public class AvatarAccessoriesCustomisation : MonoBehaviour
//{
//    [System.Serializable]
//    public class Item
//    {
//        public string UniqueID;
//        public TextMeshProUGUI PriceText;
//        public Button PurchaseButton;
//        public Toggle EquipToggle;
//        public GameObject PurchaseObject;
//        public GameObject[] ItemsToActivate;
//        public int Price;

//        public UnityEvent onEquipFunction; // Public function that will be called
//    }

//    public List<Item> itemList;
//    public bool UseDefaultItem = false; // Public field to use default item
//    public int defaultItemIndex = 0; // Set the default item using this public integer field

//    private string cashKey = "ShopMoney"; // Updated cash key
//    private bool isProgrammaticToggleChange = false; // Flag to track programmatic toggle changes

//    // New field for the confirmation window
//    public GameObject confirmationWindow;
//    public Button confirmYesButton;
//    public Button confirmNoButton;

//    private Item currentItemToPurchase; // Stores the item awaiting confirmation

//    private void Start()
//    {
//        foreach (var item in itemList)
//        {
//            item.PriceText.text = item.Price.ToString(); // Display price text

//            item.PurchaseButton.onClick.AddListener(() => ShowConfirmationWindow(item));
//            item.EquipToggle.onValueChanged.AddListener((isOn) =>
//            {
//                if (!isProgrammaticToggleChange)
//                {
//                    EquipItem(item, isOn);
//                }
//            });

//            bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;
//            bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;

//            if (isPurchased)
//            {
//                item.PurchaseObject.SetActive(false);
//                item.PriceText.gameObject.SetActive(false);
//            }
//            else
//            {
//                item.PurchaseObject.SetActive(true);
//                item.PriceText.gameObject.SetActive(true);
//            }

//            item.EquipToggle.isOn = isEquipped;
//            item.EquipToggle.interactable = false;
//        }

//        EnsureEquippedState();

//        // Setup confirmation window buttons
//        confirmYesButton.onClick.AddListener(ConfirmPurchase);
//        confirmNoButton.onClick.AddListener(CancelPurchase);
//        confirmationWindow.SetActive(false); // Hide confirmation window at start
//    }

//    private void ShowConfirmationWindow(Item item)
//    {
//        currentItemToPurchase = item;
//        confirmationWindow.SetActive(true);
//    }

//    private void ConfirmPurchase()
//    {
//        PurchaseItem(currentItemToPurchase);
//        confirmationWindow.SetActive(false);
//    }

//    private void CancelPurchase()
//    {
//        confirmationWindow.SetActive(false);
//    }

//    private void EnsureEquippedState()
//    {
//        if (UseDefaultItem)
//        {
//            Item defaultItem = itemList[defaultItemIndex];
//            defaultItem.EquipToggle.isOn = true;
//            ActivateItems(defaultItem.ItemsToActivate, true);
//            defaultItem.PurchaseObject.SetActive(false);
//            defaultItem.PriceText.gameObject.SetActive(false);
//        }

//        foreach (var item in itemList)
//        {
//            if (PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1)
//            {
//                item.EquipToggle.isOn = true;
//                ActivateItems(item.ItemsToActivate, true);
//                item.PurchaseObject.SetActive(false);
//                item.PriceText.gameObject.SetActive(false);
//                item.onEquipFunction.Invoke();
//            }
//        }
//    }

//    private void PurchaseItem(Item item)
//    {
//        int currentCash = PlayerPrefs.GetInt(cashKey, 0);

//        if (currentCash >= item.Price && PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 0)
//        {
//            int newAmount = currentCash - item.Price;
//            InGameCurrencyManager.instance.UpdateCash(newAmount);
//            PlayerPrefs.SetInt(cashKey, newAmount);

//            item.PurchaseObject.SetActive(false);
//            item.PriceText.gameObject.SetActive(false);
//            PlayerPrefs.SetInt(item.UniqueID + "_Purchased", 1);

//            EquipItem(item, true);
//        }
//        else if (PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 1)
//        {
//            EquipItem(item, true);
//        }
//        else
//        {
//            Debug.Log("Not enough cash.");
//        }
//    }

//    private void EquipItem(Item item, bool isOn)
//    {
//        if (isOn)
//        {
//            ActivateItems(item.ItemsToActivate, true);
//            isProgrammaticToggleChange = true;
//            item.EquipToggle.isOn = true;
//            isProgrammaticToggleChange = false;
//            PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 1);

//            item.onEquipFunction?.Invoke();
//        }
//        else
//        {
//            ActivateItems(item.ItemsToActivate, false);
//            PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 0);
//        }
//    }

//    private void ActivateItems(GameObject[] items, bool isActive)
//    {
//        foreach (var item in items)
//        {
//            item.SetActive(isActive);
//        }
//    }
//}




//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.UI;

//public class AvatarAccessoriesCustomisation : MonoBehaviour
//{
//    [System.Serializable]
//    public class Item
//    {
//        public string UniqueID;
//        public TextMeshProUGUI PriceText;
//        public Button PurchaseButton;
//        public Toggle EquipToggle;
//        public GameObject PurchaseObject;
//        public GameObject[] ItemsToActivate;
//        public int Price;

//        public UnityEvent onEquipFunction; // Public function that will be called
//    }

//    public List<Item> itemList;

//    public bool UseDefaultItem = false; // Public field to use default item
//    public int defaultItemIndex = 0; // Set the default item using this public integer field

//    private string cashKey = "ShopMoney"; // Updated cash key


//    private bool isProgrammaticToggleChange = false; // Flag to track programmatic toggle changes

//    private void Start()
//    {
//        // Initialize UI and set saved states
//        foreach (var item in itemList)
//        {
//            // Set the price text to display the price from the inspector
//            item.PriceText.text = item.Price.ToString(); // Adjust the format as needed (e.g., currency)

//            // Add listeners to buttons and toggles
//            item.PurchaseButton.onClick.AddListener(() => PurchaseItem(item));
//            item.EquipToggle.onValueChanged.AddListener((isOn) =>
//            {
//                if (!isProgrammaticToggleChange)
//                {
//                    EquipItem(item, isOn);
//                }
//            });

//            // Load saved data for each item
//            bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;
//            bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;

//            // Update UI based on purchase status
//            if (isPurchased)
//            {
//                item.PurchaseObject.SetActive(false);
//                item.PriceText.gameObject.SetActive(false); // Deactivate price text if purchased
//            }
//            else
//            {
//                item.PurchaseObject.SetActive(true);
//                item.PriceText.gameObject.SetActive(true); // Show price text for unpurchased items
//            }

//            // Set toggle state for equipped items
//            item.EquipToggle.isOn = isEquipped;

//            // Make the toggle non-interactable
//            item.EquipToggle.interactable = false;
//        }

//        EnsureEquippedState();
//    }

//    private void EnsureEquippedState()
//    {
//        // Only equip the default item if the public field is true
//        if (UseDefaultItem)
//        {
//            Item defaultItem = itemList[defaultItemIndex];
//            defaultItem.EquipToggle.isOn = true; // Default item should always be equipped
//            ActivateItems(defaultItem.ItemsToActivate, true); // Activate the associated items
//            defaultItem.PurchaseObject.SetActive(false);
//            defaultItem.PriceText.gameObject.SetActive(false);
//        }

//        // Check for any purchased items and ensure they are also equipped
//        foreach (var item in itemList)
//        {
//            if (PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1)
//            {
//                item.EquipToggle.isOn = true; // Ensure toggle remains On for purchased items
//                ActivateItems(item.ItemsToActivate, true); // Activate the associated items
//                item.PurchaseObject.SetActive(false);
//                item.PriceText.gameObject.SetActive(false);
//                item.onEquipFunction.Invoke(); // Call the equipped function
//            }
//        }
//    }

//    private void PurchaseItem(Item item)
//    {
//        int currentCash = PlayerPrefs.GetInt(cashKey, 0);

//        if (currentCash >= item.Price && PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 0)
//        {
//            int newAmount = currentCash - item.Price;
//            InGameCurrencyManager.instance.UpdateCash(newAmount);
//            PlayerPrefs.SetInt(cashKey, newAmount);

//            // Deactivate UI elements for the purchased item
//            item.PurchaseObject.SetActive(false);
//            item.PriceText.gameObject.SetActive(false);
//            PlayerPrefs.SetInt(item.UniqueID + "_Purchased", 1);

//            // Equip the purchased item and activate its associated GameObjects
//            EquipItem(item, true);
//        }
//        else if(PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 1)
//        {
//            EquipItem(item, true);
//        }
//        else
//        {
//            Debug.Log("Not enough cash.");
//        }
//    }
//    private void EquipItem(Item item, bool isOn)
//    {
//        if (isOn)
//        {
//            // Equip the item and activate its associated GameObjects
//            ActivateItems(item.ItemsToActivate, true);
//            isProgrammaticToggleChange = true; // Suppress toggle listener
//            item.EquipToggle.isOn = true;
//            isProgrammaticToggleChange = false; // Reset the flag
//            PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 1);

//            // Call the function from the assigned script when the item is equipped
//            if (item.onEquipFunction != null)
//            {
//                item.onEquipFunction.Invoke(); // Call the selected public function
//            }
//        }
//        else
//        {
//            ActivateItems(item.ItemsToActivate, false);
//            PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 0);
//        }
//    }

//    private void ActivateItems(GameObject[] items, bool isActive)
//    {
//        foreach (var item in items)
//        {
//            item.SetActive(isActive);
//        }
//    }
//}