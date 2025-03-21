using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class SoldierCustomization : MonoBehaviour
    {
        [TextArea]
        public string scriptInfo = "Handles soldier outfit and accessory customization, including purchasing and equipping items.";

        [System.Serializable]
        public class Item
        {
            [Tooltip("Unique ID for saving purchase and equip state")]
            public string UniqueID;

            [Tooltip("Price display for the item")]
            public TextMeshProUGUI PriceText;

            [Tooltip("Button to purchase the item")]
            public Button PurchaseButton;

            [Tooltip("Button to preview the item before purchase")]
            public Button PreviewButton;

            [Tooltip("Toggle to equip the item")]
            public Toggle EquipToggle;

            [Tooltip("GameObject representing the purchase UI elements")]
            public GameObject PurchaseObject;

            [Tooltip("GameObjects to activate when this item is equipped")]
            public GameObject[] ItemsToActivate;

            [Tooltip("Price of the item in in-game currency")]
            public int Price;

            [Tooltip("Event triggered when the item is equipped")]
            public UnityEvent onEquipFunction;
        }


        [Tooltip("List of all customizable items (outfits and accessories)")]
        public List<Item> itemList;

        [Tooltip("Index of the default item that should be unlocked by default")]
        public int defaultItemIndex = 0;

        private string cashKey = "ShopMoney";
        private bool isProgrammaticToggleChange = false;

        [Header("Confirmation Settings")]
        [Tooltip("Enable or disable purchase confirmation window")]
        public bool UseConfirmationWindow = true;

        [Tooltip("UI window for confirming purchases")]
        public GameObject confirmationWindow;

        [Tooltip("Text element displaying the confirmation message")]
        public TextMeshProUGUI ConfirmationText;

        [Tooltip("Message to display when confirming a purchase")]
        public string ConfirmationMessage = "Do you want to proceed with the purchase?";

        [Tooltip("Whether to show item price in confirmation message")]
        public bool ShowItemPrice = false;

        [Tooltip("Text element for displaying item price in confirmation window")]
        public TextMeshProUGUI BuyText;

        [Tooltip("Button to confirm purchase")]
        public Button confirmYesButton;

        [Tooltip("Button to cancel purchase")]
        public Button confirmNoButton;

        private Item currentItemToPurchase;
        private Item equippedItem;
        private Item previousEquippedItem; // Store the previously equipped item

        private void Start()
        {
            foreach (var item in itemList)
            {
                item.PriceText.text = item.Price.ToString();
                item.PurchaseButton.onClick.AddListener(() => HandleItemButtonClick(item));

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
                bool isEquipped = PlayerPrefs.GetInt(item.UniqueID, 0) == 1;

                if (item == itemList[defaultItemIndex] && !isPurchased)
                {
                    item.PurchaseObject.SetActive(false);
                    item.PriceText.gameObject.SetActive(false);
                    isPurchased = true;
                    PlayerPrefs.SetInt(item.UniqueID + "_Purchased", 1);
                }

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

            if (UseConfirmationWindow)
            {
                confirmYesButton.onClick.AddListener(ConfirmPurchase);
                confirmNoButton.onClick.AddListener(CancelPurchase);
                confirmationWindow.SetActive(false);
            }
        }

        private void HandleItemButtonClick(Item item)
        {
            bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;
            bool isEquipped = PlayerPrefs.GetInt(item.UniqueID, 0) == 1;

            if (!isPurchased)
            {
                if (UseConfirmationWindow)
                {
                    currentItemToPurchase = item;
                    previousEquippedItem = equippedItem; // Store the currently equipped item

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
            else if (!isEquipped)
            {
                EquipItem(item, true);
            }
        }

        private void ConfirmPurchase()
        {
            PurchaseItem(currentItemToPurchase);
            confirmationWindow.SetActive(false);
        }

        private void CancelPurchase()
        {
            confirmationWindow.SetActive(false);

            // Re-equip the previously equipped item if "No" is selected
            if (previousEquippedItem != null)
            {
                EquipItem(previousEquippedItem, true);
            }
        }

        private void EnsureEquippedState()
        {
            foreach (var item in itemList)
            {
                if (PlayerPrefs.GetInt(item.UniqueID, 0) == 1)
                {
                    equippedItem = item;
                    item.EquipToggle.isOn = true;
                    ActivateItems(item.ItemsToActivate, true);
                    item.onEquipFunction.Invoke();
                    return;
                }
            }

            if (defaultItemIndex >= 0 && defaultItemIndex < itemList.Count)
            {
                Item defaultItem = itemList[defaultItemIndex];
                equippedItem = defaultItem;
                defaultItem.EquipToggle.isOn = true;
                ActivateItems(defaultItem.ItemsToActivate, true);
                defaultItem.PurchaseObject.SetActive(false);
                defaultItem.PriceText.gameObject.SetActive(false);
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
            else
            {
                Debug.Log("Not enough cash.");
            }
        }

        private void EquipItem(Item item, bool isOn)
        {
            if (isOn)
            {
                if (equippedItem != null)
                {
                    ActivateItems(equippedItem.ItemsToActivate, false);
                    isProgrammaticToggleChange = true;
                    equippedItem.EquipToggle.isOn = false;
                    PlayerPrefs.SetInt(equippedItem.UniqueID, 0);
                    isProgrammaticToggleChange = false;
                }

                ActivateItems(item.ItemsToActivate, true);
                isProgrammaticToggleChange = true;
                item.EquipToggle.isOn = true;
                PlayerPrefs.SetInt(item.UniqueID, 1);
                equippedItem = item;
                isProgrammaticToggleChange = false;

                item.onEquipFunction?.Invoke();
            }
        }

        private void PreviewItem(Item item)
        {
            if (PlayerPrefs.GetInt(item.UniqueID + "_Purchased") == 0)
            {
                if (equippedItem != null)
                {
                    ActivateItems(equippedItem.ItemsToActivate, false);
                }

                ActivateItems(item.ItemsToActivate, true);
                item.EquipToggle.isOn = false;

                item.onEquipFunction?.Invoke();
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

//public class AvatarOutfitCustomisation : MonoBehaviour
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
//    public int defaultItemIndex = 0; // Set the default item using this public integer field
//    private string cashKey = "ShopMoney"; // Updated cash key

//    private bool isProgrammaticToggleChange = false; // Flag to track programmatic toggle changes

//    // New field for the confirmation window
//    public bool UseConfirmationWindow = true; // Optional confirmation window toggle
//    public GameObject confirmationWindow;
//    public TextMeshProUGUI ConfirmationText;
//    public string ConfirmationMessage = "Do you want to Proceed with the purchase";
//    public bool ShowItemPrice = false;
//    public TextMeshProUGUI BuyText;
//    public Button confirmYesButton;
//    public Button confirmNoButton;

//    private Item currentItemToPurchase; // Stores the item awaiting confirmation
//    private Item equippedItem; // Keep track of the currently equipped item

//    private void Start()
//    {
//        // Initialize UI and set saved states
//        foreach (var item in itemList)
//        {
//            item.PriceText.text = item.Price.ToString(); // Display price text

//            item.PurchaseButton.onClick.AddListener(() => HandleItemButtonClick(item));
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

//            // Check if the item is purchased or equipped
//            bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;
//            bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;

//            // Handle default item logic
//            if (item == itemList[defaultItemIndex] && !isPurchased)
//            {
//                item.PurchaseObject.SetActive(false);
//                item.PriceText.gameObject.SetActive(false);
//                isPurchased = true;
//                PlayerPrefs.SetInt(item.UniqueID + "_Purchased", 1);
//            }

//            // Handle item purchased state
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

//            item.EquipToggle.isOn = isEquipped; // Set toggle state based on PlayerPrefs
//            item.EquipToggle.interactable = false; // Make toggle non-interactable
//        }

//        EnsureEquippedState();

//        if (UseConfirmationWindow)
//        {
//            // Setup confirmation window buttons
//            confirmYesButton.onClick.AddListener(ConfirmPurchase);
//            confirmNoButton.onClick.AddListener(CancelPurchase);
//            confirmationWindow.SetActive(false); // Hide confirmation window at start
//        }
//    }

//    private void HandleItemButtonClick(Item item)
//    {
//        bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;
//        bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;

//        if (!isPurchased)
//        {
//            if (UseConfirmationWindow)
//            {
//                // Show confirmation window if not purchased
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
//                // Directly purchase if not using confirmation window
//                PurchaseItem(item);
//            }
//        }
//        else if (!isEquipped)
//        {
//            // Directly equip if purchased and not already equipped
//            EquipItem(item, true);
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
//        foreach (var item in itemList)
//        {
//            if (PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1)
//            {
//                equippedItem = item; // Track currently equipped item
//                item.EquipToggle.isOn = true;
//                ActivateItems(item.ItemsToActivate, true);
//                item.onEquipFunction.Invoke();
//                return; // Exit after finding the equipped item
//            }
//        }

//        // If no equipped item exists, equip the default item
//        if (defaultItemIndex >= 0 && defaultItemIndex < itemList.Count)
//        {
//            Item defaultItem = itemList[defaultItemIndex];
//            equippedItem = defaultItem; // Track currently equipped item
//            defaultItem.EquipToggle.isOn = true;
//            ActivateItems(defaultItem.ItemsToActivate, true);
//            defaultItem.PurchaseObject.SetActive(false);
//            defaultItem.PriceText.gameObject.SetActive(false);
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
//            // Deactivate currently equipped item, if any
//            if (equippedItem != null)
//            {
//                ActivateItems(equippedItem.ItemsToActivate, false); // Deactivate previous item
//                isProgrammaticToggleChange = true;
//                equippedItem.EquipToggle.isOn = false; // Turn off the toggle for the previously equipped item
//                PlayerPrefs.SetInt(equippedItem.UniqueID + "_Equipped", 0);
//                isProgrammaticToggleChange = false;
//            }

//            // Activate new item
//            ActivateItems(item.ItemsToActivate, true); // Activate items for the newly equipped item
//            isProgrammaticToggleChange = true;
//            item.EquipToggle.isOn = true; // Turn on the toggle for the currently equipped item
//            PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 1);
//            equippedItem = item; // Update currently equipped item
//            isProgrammaticToggleChange = false;

//            item.onEquipFunction?.Invoke(); // Call the equip function
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
//        // Ensure the toggle remains OFF for preview
//        item.EquipToggle.isOn = false; // Toggle stays OFF to indicate preview

//        item.onEquipFunction?.Invoke(); // Call the equip function
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

//public class AvatarOutfitCustomisation : MonoBehaviour
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
//    public int defaultItemIndex = 0; // Set the default item using this public integer field
//    private string cashKey = "ShopMoney"; // Updated cash key

//    private bool isProgrammaticToggleChange = false; // Flag to track programmatic toggle changes

//    // New field for the confirmation window
//    public bool UseConfirmationWindow = true;
//    public GameObject confirmationWindow;
//    public Button confirmYesButton;
//    public Button confirmNoButton;

//    private Item currentItemToPurchase; // Stores the item awaiting confirmation
//    private Item equippedItem; // Keep track of the currently equipped item



//    private void Start()
//    {
//        // Initialize UI and set saved states
//        foreach (var item in itemList)
//        {
//            item.PriceText.text = item.Price.ToString(); // Display price text

//            item.PurchaseButton.onClick.AddListener(() => HandleItemButtonClick(item));
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

//            // Check if the item is purchased or equipped
//            bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;
//            bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;

//            // Handle default item logic
//            if (item == itemList[defaultItemIndex] && !isPurchased)
//            {
//                item.PurchaseObject.SetActive(false);
//                item.PriceText.gameObject.SetActive(false);
//                isPurchased = true;
//                PlayerPrefs.SetInt(item.UniqueID + "_Purchased", 1);
//            }

//            // Handle item purchased state
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

//            item.EquipToggle.isOn = isEquipped; // Set toggle state based on PlayerPrefs
//            item.EquipToggle.interactable = false; // Make toggle non-interactable
//        }

//        EnsureEquippedState();

//        if(UseConfirmationWindow == true)
//        {
//            // Setup confirmation window buttons
//            confirmYesButton.onClick.AddListener(ConfirmPurchase);
//            confirmNoButton.onClick.AddListener(CancelPurchase);
//            confirmationWindow.SetActive(false); // Hide confirmation window at start
//        }

//    }

//    private void HandleItemButtonClick(Item item)
//    {
//        bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;
//        bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;

//        if (!isPurchased)
//        {
//            // Show confirmation window if not purchased
//            currentItemToPurchase = item;
//            confirmationWindow.SetActive(true);
//        }
//        else if (!isEquipped)
//        {
//            // Directly equip if purchased and not already equipped
//            EquipItem(item, true);
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
//        foreach (var item in itemList)
//        {
//            if (PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1)
//            {
//                equippedItem = item; // Track currently equipped item
//                item.EquipToggle.isOn = true;
//                ActivateItems(item.ItemsToActivate, true);
//                item.onEquipFunction.Invoke();
//                return; // Exit after finding the equipped item
//            }
//        }

//        // If no equipped item exists, equip the default item
//        if (defaultItemIndex >= 0 && defaultItemIndex < itemList.Count)
//        {
//            Item defaultItem = itemList[defaultItemIndex];
//            equippedItem = defaultItem; // Track currently equipped item
//            defaultItem.EquipToggle.isOn = true;
//            ActivateItems(defaultItem.ItemsToActivate, true);
//            defaultItem.PurchaseObject.SetActive(false);
//            defaultItem.PriceText.gameObject.SetActive(false);
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
//            // Deactivate currently equipped item, if any
//            if (equippedItem != null)
//            {
//                ActivateItems(equippedItem.ItemsToActivate, false); // Deactivate previous item
//                isProgrammaticToggleChange = true;
//                equippedItem.EquipToggle.isOn = false; // Turn off the toggle for the previously equipped item
//                PlayerPrefs.SetInt(equippedItem.UniqueID + "_Equipped", 0);
//                isProgrammaticToggleChange = false;
//            }

//            // Activate new item
//            ActivateItems(item.ItemsToActivate, true); // Activate items for the newly equipped item
//            isProgrammaticToggleChange = true;
//            item.EquipToggle.isOn = true; // Turn on the toggle for the currently equipped item
//            PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 1);
//            equippedItem = item; // Update currently equipped item
//            isProgrammaticToggleChange = false;

//            item.onEquipFunction?.Invoke(); // Call the equip function
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
//        // Ensure the toggle remains OFF for preview
//        item.EquipToggle.isOn = false; // Toggle stays OFF to indicate preview

//        item.onEquipFunction?.Invoke(); // Call the equip function
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

//public class AvatarOutfitCustomisation : MonoBehaviour
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
//        // Initialize UI and set saved states
//        foreach (var item in itemList)
//        {
//            item.PriceText.text = item.Price.ToString(); // Display price text

//            item.PurchaseButton.onClick.AddListener(() => HandleItemButtonClick(item));
//            item.EquipToggle.onValueChanged.AddListener((isOn) =>
//            {
//                if (!isProgrammaticToggleChange)
//                {
//                    EquipItem(item, isOn);
//                }
//            });

//            bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;
//            bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;

//            if (item == itemList[defaultItemIndex] && !isPurchased)
//            {
//                item.PurchaseObject.SetActive(false);
//                item.PriceText.gameObject.SetActive(false);
//                isPurchased = true;
//                PlayerPrefs.SetInt(item.UniqueID + "_Purchased", 1);
//            }

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

//    private void HandleItemButtonClick(Item item)
//    {
//        bool isPurchased = PlayerPrefs.GetInt(item.UniqueID + "_Purchased", 0) == 1;
//        bool isEquipped = PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1;

//        if (!isPurchased)
//        {
//            // Show confirmation window if not purchased
//            currentItemToPurchase = item;
//            confirmationWindow.SetActive(true);
//        }
//        else if (!isEquipped)
//        {
//            // Directly equip if purchased and not already equipped
//            EquipItem(item, true);
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
//        bool equippedItemExists = false;

//        foreach (var item in itemList)
//        {
//            if (PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1)
//            {
//                equippedItemExists = true;
//                item.EquipToggle.isOn = true;
//                ActivateItems(item.ItemsToActivate, true);
//                item.onEquipFunction.Invoke();
//                break;
//            }
//        }

//        if (!equippedItemExists && defaultItemIndex >= 0 && defaultItemIndex < itemList.Count)
//        {
//            Item defaultItem = itemList[defaultItemIndex];
//            defaultItem.EquipToggle.isOn = true;
//            ActivateItems(defaultItem.ItemsToActivate, true);
//            defaultItem.PurchaseObject.SetActive(false);
//            defaultItem.PriceText.gameObject.SetActive(false);
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
//        else
//        {
//            Debug.Log("Not enough cash.");
//        }
//    }

//    private void EquipItem(Item item, bool isOn)
//    {
//        if (isOn)
//        {
//            foreach (var otherItem in itemList)
//            {
//                if (otherItem != item)
//                {
//                    ActivateItems(otherItem.ItemsToActivate, false);
//                    otherItem.EquipToggle.isOn = false;

//                    isProgrammaticToggleChange = true;
//                    PlayerPrefs.SetInt(otherItem.UniqueID + "_Equipped", 0);
//                    otherItem.EquipToggle.isOn = false;
//                    isProgrammaticToggleChange = false;
//                }
//            }

//            ActivateItems(item.ItemsToActivate, true);
//            isProgrammaticToggleChange = true;
//            item.EquipToggle.isOn = true;
//            isProgrammaticToggleChange = false;
//            PlayerPrefs.SetInt(item.UniqueID + "_Equipped", 1);

//            item.onEquipFunction?.Invoke();
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

//public class AvatarOutfitCustomisation : MonoBehaviour
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

//            if (item == itemList[defaultItemIndex] && !isPurchased)
//            {
//                item.PurchaseObject.SetActive(false);
//                item.PriceText.gameObject.SetActive(false);
//                isPurchased = true; // Mark it as "purchased"
//                PlayerPrefs.SetInt(item.UniqueID + "_Purchased", 1);
//            }

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
//        bool equippedItemExists = false;

//        foreach (var item in itemList)
//        {
//            if (PlayerPrefs.GetInt(item.UniqueID + "_Equipped", 0) == 1)
//            {
//                equippedItemExists = true;
//                item.EquipToggle.isOn = true; // Set the toggle directly
//                ActivateItems(item.ItemsToActivate, true); // Activate the associated items
//                item.onEquipFunction.Invoke(); // Call the equipped function
//                break;
//            }
//        }

//        // If no item was equipped, equip the default item
//        if (!equippedItemExists && defaultItemIndex >= 0 && defaultItemIndex < itemList.Count)
//        {
//            Item defaultItem = itemList[defaultItemIndex];
//            defaultItem.EquipToggle.isOn = true; // Set the toggle directly
//            ActivateItems(defaultItem.ItemsToActivate, true); // Activate the associated items
//            defaultItem.PurchaseObject.SetActive(false);
//            defaultItem.PriceText.gameObject.SetActive(false);
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
//            foreach (var otherItem in itemList)
//            {
//                if (otherItem != item)
//                {
//                    ActivateItems(otherItem.ItemsToActivate, false);
//                    otherItem.EquipToggle.isOn = false;

//                    // Suppress toggle listener to prevent double calls
//                    isProgrammaticToggleChange = true;
//                    PlayerPrefs.SetInt(otherItem.UniqueID + "_Equipped", 0);
//                    otherItem.EquipToggle.isOn = false;
//                    isProgrammaticToggleChange = false; // Reset the flag
//                }
//            }

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
