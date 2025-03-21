using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MobileActionKit
{
    public class ShootingWeaponsSlotManager : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "Manages purchasable and equippable weapons in a slot-based system. Handles purchasing, equipping, and saving weapon states.";

        [Tooltip("Unique slot name used for saving data (e.g., Slot1, Slot2).")]
        public string SlotSaveKey = "Slot";

        [Tooltip("UI GameObject representing this weapon slot.")]
        public GameObject Slot;

        [Tooltip("Text displayed on the Buy button.")]
        public string BuyText = "Buy";

        [Tooltip("Text displayed on the Equip button.")]
        public string EquipText = "Equip";

        [Tooltip("Text displayed when a weapon is currently equipped.")]
        public string EquippedText = "Equipped";

        [Tooltip("List of weapons available in this slot.")]
        public List<AddWeaponsExistInSlot> AddWeapons = new List<AddWeaponsExistInSlot>();

        [Tooltip("If enabled, a default weapon will be assigned at startup.")]
        public bool UseDefaultWeapon = true;

        [Tooltip("Index of the default weapon in the list.")]
        public int DefaultWeaponIndex;

        [Tooltip("Reference to the PurchasableWeaponsManager component.")]
        public PurchasableWeaponsManager PurchasableWeaponsManagerComponent;

        [Tooltip("If enabled, a confirmation window will appear before purchasing.")]
        public bool UseConfirmationWindow = true;

        [Tooltip("UI GameObject for the confirmation window.")]
        public GameObject confirmationWindow;

        [Tooltip("Text element displaying the confirmation message.")]
        public TextMeshProUGUI ConfirmationText;

        [Tooltip("Message shown in the confirmation window.")]
        public string ConfirmationMessage = "Do you want to proceed with the purchase?";

        [Tooltip("If enabled, weapon price will be displayed in the confirmation window.")]
        public bool ShowPriceTextInConfirmationWindow;

        [Tooltip("Text element displaying the weapon price in the confirmation window.")]
        public TextMeshProUGUI ConfirmationWindowPriceText;

        [Tooltip("Button for confirming the purchase.")]
        public Button confirmYesButton;

        [Tooltip("Button for canceling the purchase.")]
        public Button confirmNoButton;

        private string cashKey = "ShopMoney";

        [HideInInspector]
        public AddWeaponsExistInSlot pendingWeapon;

        [System.Serializable]
        public class AddWeaponsExistInSlot
        {
            [Tooltip("Reference to the weapon's unique ID script.")]
            public WeaponId WeaponIdScript;

            [Tooltip("Weapon GameObject associated with this slot.")]
            public GameObject Weapon;

            [Tooltip("Button used to purchase the weapon.")]
            public Button BuyButton;

            [Tooltip("Button used to equip the weapon.")]
            public Button EquipButton;

            [Tooltip("Price of the weapon.")]
            public int WeaponPrice;

            [Tooltip("Text element displaying the weapon price.")]
            public TextMeshProUGUI PriceText;

            [Tooltip("Text element on the Equip button.")]
            public TextMeshProUGUI EquipButtonText;

            [Tooltip("GameObjects to activate after purchasing the weapon.")]
            public GameObject[] ObjectsToActivateAfterPurchase;
        }

        [Tooltip("Reference to the currently equipped weapon for debugging purposes.")]
        public GameObject DebugCurrentlyEquippedWeapon;

        private void Start()
        {
            if (UseDefaultWeapon == true && PlayerPrefs.GetInt(SlotSaveKey + "ShowDefaultWeapon") != 1)
            {
                AddWeapons[DefaultWeaponIndex].WeaponPrice = 0;
                pendingWeapon = AddWeapons[DefaultWeaponIndex];
                AttemptPurchase();
                PlayerPrefs.SetInt(SlotSaveKey + "ShowDefaultWeapon", 1);
            }

            // Set up button listeners and initial text/prices for each weapon
            foreach (var weapon in AddWeapons)
            {
                weapon.PriceText.text = weapon.WeaponPrice.ToString();
                weapon.BuyButton.onClick.AddListener(() => OnBuyButtonClicked(weapon));
                weapon.EquipButton.onClick.AddListener(() => EquipWeapon(AddWeapons.IndexOf(weapon)));
            }

            // Restore saved states at start
            LoadSavedWeaponStates();

            // Confirmation button listeners
            confirmYesButton.onClick.AddListener(OnConfirmYes);
            confirmNoButton.onClick.AddListener(OnConfirmNo);
        }
        public void ObjectsToActivateAfterPurchase(AddWeaponsExistInSlot Weapon)
        {
            for (int x = 0; x < Weapon.ObjectsToActivateAfterPurchase.Length; x++)
            {
                Weapon.ObjectsToActivateAfterPurchase[x].gameObject.SetActive(true);
            }
        }
        private void LoadSavedWeaponStates()
        {
            for (int i = 0; i < AddWeapons.Count; i++)
            {
                var weapon = AddWeapons[i];
                string status = PlayerPrefs.GetString(weapon.WeaponIdScript.WeaponName, "NotPurchased");

                if (PlayerPrefs.GetInt(AddWeapons[i].WeaponIdScript.WeaponName + "Equipped") == 1)
                {
                    ObjectsToActivateAfterPurchase(weapon);
                    EquipWeapon(i); // Equip the previously equipped weapon
                }
                else if (status == "Purchased")
                {
                    ObjectsToActivateAfterPurchase(weapon);
                    weapon.BuyButton.gameObject.SetActive(false);
                    weapon.EquipButton.gameObject.SetActive(true);
                    weapon.EquipButtonText.text = EquipText;
                    weapon.EquipButton.interactable = true;
                }
                else
                {
                    weapon.BuyButton.gameObject.SetActive(true);
                    weapon.EquipButton.gameObject.SetActive(false);
                }
            }
        }

        private void OnBuyButtonClicked(AddWeaponsExistInSlot weapon)
        {
            PurchasableWeaponsManagerComponent.ConfirmationWindowReset();
            pendingWeapon = weapon;

            if (UseConfirmationWindow)
            {
                confirmationWindow.SetActive(true);
                ConfirmationText.text = ConfirmationMessage;
                ConfirmationWindowPriceText.text = ShowPriceTextInConfirmationWindow ? $"Price: {weapon.WeaponPrice}" : "";
            }
            else
            {
                AttemptPurchase();
            }
        }

        private void OnConfirmYes()
        {
            confirmationWindow.SetActive(false);
            AttemptPurchase();
        }

        private void OnConfirmNo()
        {
            confirmationWindow.SetActive(false);
            pendingWeapon = null;
        }

        private void AttemptPurchase()
        {
            if (pendingWeapon == null) return;

            int currentCash = PlayerPrefs.GetInt(cashKey, 0);

            if (currentCash >= pendingWeapon.WeaponPrice)
            {
                // Deduct cash
                currentCash -= pendingWeapon.WeaponPrice;
                PlayerPrefs.SetInt(cashKey, currentCash);
                PlayerPrefs.SetString(pendingWeapon.WeaponIdScript.WeaponName, "Purchased");
                PlayerPrefs.Save();

                // Activate the purchased weapon
                ActivateWeapon(pendingWeapon);

                // Equip the new weapon
                EquipWeapon(AddWeapons.IndexOf(pendingWeapon));
                pendingWeapon = null;
            }
            else
            {
                Debug.Log("Not enough cash to purchase this weapon.");
            }
        }

        private void ActivateWeapon(AddWeaponsExistInSlot weapon)
        {
            weapon.BuyButton.gameObject.SetActive(false); // Deactivate Buy Button
            weapon.EquipButton.gameObject.SetActive(true); // Activate Equip Button
            weapon.EquipButtonText.text = EquippedText; // Set Equip Button text to Equipped
            weapon.EquipButton.interactable = false; // Make Equip Button non-interactable



            // Update other purchased weapons to activate their Equip buttons
            foreach (var w in AddWeapons)
            {
                if (w != weapon)
                {

                    string status = PlayerPrefs.GetString(w.WeaponIdScript.WeaponName, "NotPurchased");
                    if (status == "Purchased")
                    {
                        w.EquipButton.gameObject.SetActive(true); // Activate Equip Button
                        w.EquipButtonText.text = EquipText; // Set text to Equip
                        w.EquipButton.interactable = true; // Make it interactable
                    }
                }
            }



            PlayerPrefs.Save();
        }

        private void EquipWeapon(int weaponIndex)
        {
            for (int i = 0; i < AddWeapons.Count; i++)
            {
                var weapon = AddWeapons[i];
                PlayerPrefs.SetInt(AddWeapons[i].WeaponIdScript.WeaponName + "Equipped", 0);
                if (i == weaponIndex)
                {
                    weapon.EquipButtonText.text = EquippedText; // Set to Equipped
                    weapon.EquipButton.interactable = false; // Make it non-interactable
                    weapon.EquipButton.gameObject.SetActive(true);
                    weapon.BuyButton.gameObject.SetActive(false); // Deactivate Buy Button
                    DebugCurrentlyEquippedWeapon = weapon.Weapon; // Set the currently equipped weapon
                }
                else
                {
                    string status = PlayerPrefs.GetString(weapon.WeaponIdScript.WeaponName, "NotPurchased");

                    if (status == "Purchased")
                    {
                        weapon.EquipButtonText.text = EquipText; // Change button text to Equip
                        weapon.EquipButton.interactable = true; // Make it interactable
                        weapon.EquipButton.gameObject.SetActive(true); // Activate Equip Button
                        weapon.BuyButton.gameObject.SetActive(false); // Deactivate Buy Button
                    }
                    else
                    {
                        weapon.BuyButton.gameObject.SetActive(true); // Activate Buy Button for not purchased
                        weapon.EquipButton.gameObject.SetActive(false); // Deactivate Equip Button
                    }
                }
            }
            ObjectsToActivateAfterPurchase(AddWeapons[weaponIndex]);
            PlayerPrefs.SetInt(AddWeapons[weaponIndex].WeaponIdScript.WeaponName + "Equipped", 1); // Save equipped weapon index
            ShopMenuWeaponsManager.instance.UpdateWeaponsCarried();
            PlayerPrefs.Save();

        }
    }

}
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class ShootingWeaponsSlotManager : MonoBehaviour
//{
//    public GameObject Slot;
//    public string BuyText = "Buy";
//    public string EquipText = "Equip";
//    public string EquippedText = "Equipped";
//    public List<AddWeaponsExistInSlot> AddWeapons = new List<AddWeaponsExistInSlot>();
//    public bool UseDefaultWeapon = true;
//    public int DefaultWeaponIndex;

//    public bool UseConfirmationWindow = true;
//    public GameObject confirmationWindow;
//    public TextMeshProUGUI ConfirmationText;
//    public string ConfirmationMessage = "Do you want to proceed with the purchase?";
//    public bool ShowPriceTextInConfirmationWindow;
//    public TextMeshProUGUI PriceTextInConfirmationWindow;
//    public Button confirmYesButton;
//    public Button confirmNoButton;

//    private string cashKey = "ShopMoney";
//    private AddWeaponsExistInSlot pendingWeapon;

//    [System.Serializable]
//    public class AddWeaponsExistInSlot
//    {
//        public string WeaponNameForSavingData;
//        public GameObject Weapon;
//        public Button BuyButton;
//        public Button EquipButton;
//        public int WeaponPrice;
//        public TextMeshProUGUI PriceText;
//        public TextMeshProUGUI EquipButtonText;
//    }

//    public GameObject DebugCurrentlyEquippedWeapon;

//    private void Start()
//    {
//        foreach (var weapon in AddWeapons)
//        {
//            weapon.PriceText.text = weapon.WeaponPrice.ToString();
//            UpdateButtonState(weapon);
//            weapon.BuyButton.onClick.AddListener(() => OnBuyButtonClicked(weapon));
//            weapon.EquipButton.onClick.AddListener(() => EquipWeapon(AddWeapons.IndexOf(weapon)));

//            if (PlayerPrefs.GetString(weapon.WeaponNameForSavingData) == "Equipped")
//            {
//                weapon.EquipButton.gameObject.SetActive(true);
//                weapon.BuyButton.gameObject.SetActive(false);
//            }
//        }

//        // Equip the default weapon if specified and no weapon is equipped
//        if (UseDefaultWeapon && DefaultWeaponIndex >= 0 && DefaultWeaponIndex < AddWeapons.Count)
//        {
//            var defaultWeapon = AddWeapons[DefaultWeaponIndex];
//            if (PlayerPrefs.GetString(defaultWeapon.WeaponNameForSavingData) != "Purchased" &&
//                PlayerPrefs.GetString(defaultWeapon.WeaponNameForSavingData) != "Equipped")
//            {
//                EquipWeapon(DefaultWeaponIndex);
//                PlayerPrefs.SetString(defaultWeapon.WeaponNameForSavingData, "Equipped");
//                PlayerPrefs.SetInt("EquippedWeaponIndex", DefaultWeaponIndex);
//                PlayerPrefs.Save();
//            }
//        }



//        confirmYesButton.onClick.AddListener(OnConfirmYes);
//        confirmNoButton.onClick.AddListener(OnConfirmNo);
//    }

//    private void OnBuyButtonClicked(AddWeaponsExistInSlot weapon)
//    {
//        pendingWeapon = weapon;

//        if (UseConfirmationWindow)
//        {
//            confirmationWindow.SetActive(true);
//            ConfirmationText.text = ConfirmationMessage;

//            if (ShowPriceTextInConfirmationWindow)
//                PriceTextInConfirmationWindow.text = $"Price: {weapon.WeaponPrice}";
//            else
//                PriceTextInConfirmationWindow.text = "";
//        }
//        else
//        {
//            AttemptPurchase();
//        }
//    }

//    private void OnConfirmYes()
//    {
//        confirmationWindow.SetActive(false);
//        AttemptPurchase();
//    }

//    private void OnConfirmNo()
//    {
//        confirmationWindow.SetActive(false);
//        pendingWeapon = null;
//    }

//    private void AttemptPurchase()
//    {
//        if (pendingWeapon == null) return;

//        int currentCash = PlayerPrefs.GetInt(cashKey, 0);

//        if (currentCash >= pendingWeapon.WeaponPrice)
//        {
//            currentCash -= pendingWeapon.WeaponPrice;
//            PlayerPrefs.SetInt(cashKey, currentCash);
//            PlayerPrefs.SetString(pendingWeapon.WeaponNameForSavingData, "Purchased");
//            PlayerPrefs.Save();

//            pendingWeapon.EquipButtonText.text = EquippedText;
//            pendingWeapon.EquipButton.gameObject.SetActive(true);
//            pendingWeapon.BuyButton.gameObject.SetActive(false);

//            EquipWeapon(AddWeapons.IndexOf(pendingWeapon));
//            pendingWeapon = null;
//        }
//        else
//        {
//            Debug.Log("Not enough cash to purchase this weapon.");
//        }
//    }

//    private void EquipWeapon(int weaponIndex)
//    {
//        for (int i = 0; i < AddWeapons.Count; i++)
//        {
//            if (i == weaponIndex)
//            {
//                AddWeapons[i].EquipButtonText.text = EquippedText;
//                AddWeapons[i].EquipButton.interactable = false;
//                AddWeapons[i].EquipButton.gameObject.SetActive(true);
//                AddWeapons[i].BuyButton.gameObject.SetActive(false);
//                PlayerPrefs.SetString(AddWeapons[i].WeaponNameForSavingData, "Equipped");
//                PlayerPrefs.SetInt("EquippedWeaponIndex", weaponIndex);
//                DebugCurrentlyEquippedWeapon = AddWeapons[i].Weapon;
//            }
//            else if (PlayerPrefs.GetString(AddWeapons[i].WeaponNameForSavingData) == "Purchased")
//            {
//                AddWeapons[i].EquipButtonText.text = EquipText;
//                AddWeapons[i].EquipButton.interactable = true;
//                AddWeapons[i].EquipButton.gameObject.SetActive(true);
//                AddWeapons[i].BuyButton.gameObject.SetActive(false);
//            }
//            else
//            {
//                AddWeapons[i].EquipButtonText.text = BuyText;
//                AddWeapons[i].EquipButton.gameObject.SetActive(false);
//                AddWeapons[i].BuyButton.gameObject.SetActive(true);
//            }
//        }
//        PlayerPrefs.Save();
//    }

//    private void UpdateButtonState(AddWeaponsExistInSlot weapon)
//    {
//        string purchaseStatus = PlayerPrefs.GetString(weapon.WeaponNameForSavingData);
//        int equippedWeaponIndex = PlayerPrefs.GetInt("EquippedWeaponIndex", -1);

//        if (purchaseStatus == "Equipped" || AddWeapons.IndexOf(weapon) == equippedWeaponIndex)
//        {
//            weapon.EquipButtonText.text = EquippedText;
//            weapon.EquipButton.interactable = false;
//            weapon.BuyButton.gameObject.SetActive(false);
//            weapon.EquipButton.gameObject.SetActive(true);
//            DebugCurrentlyEquippedWeapon = weapon.Weapon;
//        }
//        else if (purchaseStatus == "Purchased")
//        {
//            weapon.EquipButtonText.text = EquipText;
//            weapon.EquipButton.interactable = true;
//            weapon.BuyButton.gameObject.SetActive(false);
//            weapon.EquipButton.gameObject.SetActive(true);
//        }
//        else
//        {
//            weapon.EquipButtonText.text = BuyText;
//            weapon.BuyButton.gameObject.SetActive(true);
//            weapon.EquipButton.gameObject.SetActive(false);
//        }
//    }
//}
