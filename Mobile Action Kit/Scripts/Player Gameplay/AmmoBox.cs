using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class AmmoBox : MonoBehaviour
    {
        public static AmmoBox Instance;

        [TextArea]
        public string ScriptInfo = "This script provides ammo to the player weapon when collected. Supports automatic pickup and UI-based pickup options.";

        [Tooltip("If enabled, the ammo box will be picked up automatically when the player collides with it.")]
        public bool AutoItemPickup;

        public string ItemName = "AmmoBox";

        [Tooltip("A UI panel that appears when the player is near the ammo box, showing pickup options.")]
        public GameObject UIPickupPanel;

        [Tooltip("UI Image displaying the ammo box sprite.")]
        public Image ItemImage;

        [Tooltip("Button to confirm ammo pickup when UI panel is active.")]
        public Button UIButtonForPickUp;

        [Tooltip("The sprite representing the ammo box in the UI.")]
        public Sprite ItemSprite;

        [Tooltip("The scale of the item image in the UI.")]
        public Vector3 ItemImageScale;

        [Tooltip("Collider that will be disabled once the ammo box is collected.")]
        public Collider ColliderToDisableOnPickup;

        [System.Serializable]
        public class UIText
        {
            [Tooltip("Text element to display information about the ammo box.")]
            public TextMeshProUGUI TextMeshProText;

            [Tooltip("The text content to be displayed.")]
            public string TextInfo;
        }

        [Tooltip("List of UI text elements associated with the ammo box.")]
        public List<UIText> UITextInfo = new List<UIText>();

        [Tooltip("Text displayed when the player receives ammo.")]
        public TextMeshProUGUI RecievedText;

        [Tooltip("The message to display when ammo is picked up.")]
        public string RecievedTextInfo = "Item Received";

        [Tooltip("Duration before the received text disappears.")]
        public float TextDeactivationDelay = 2f;

        [Tooltip("Sound effect played when the player collects the ammo box.")]
        public AudioSource AmmoCollectSound;

        [Tooltip("Amount of ammo given when the player picks up the box.")]
        public int AmmoAmount = 30;

        [Tooltip("If enabled, adds ammo only to the currently equipped weapon.")]
        public bool ApplyAmmoToCurrentWeapon = false;

        [Tooltip("If enabled, distributes ammo across all weapons in the player's inventory.")]
        public bool ApplyAmmoToAllWeapons = false;

        [Tooltip("Objects to deactivate when the ammo box is picked up.")]
        public GameObject[] ObjectsToDeactivateOnCollect;

        [Tooltip("If enabled, deactivated objects will reactivate after a delay.")]
        public bool ShouldReactivateObjects = true;

        [Tooltip("Delay before reactivating objects that were deactivated upon pickup.")]
        public float ReactivationDelay = 3f;

        private PlayerManager playerManager;
        string ItemNameToPick;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            playerManager = PlayerManager.instance;
          
        }
        void OnDisable()
        {
            foreach (var obj in ObjectsToDeactivateOnCollect)
            {
                obj.SetActive(true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (AutoItemPickup == true)
                {
                    ItemPicked();
                }
                else
                {
                    ItemNameToPick = ItemName;
                    if (UIButtonForPickUp != null)
                    {
                        UIButtonForPickUp.interactable = true;
                        UIButtonForPickUp.onClick.RemoveAllListeners();
                        UIButtonForPickUp.onClick.AddListener(ItemPicked);
                    }
                    if (RecievedText != null)
                    {
                        RecievedText.text = RecievedTextInfo;
                    }

                    if (UIPickupPanel != null)
                    {
                        UIPickupPanel.gameObject.SetActive(true);
                    }
                    if (ItemImage != null && ItemSprite != null)
                    {
                        ItemImage.sprite = ItemSprite;
                        ItemImage.transform.localScale = ItemImageScale;
                    }
                    showUIText();
                }
            }
        }
        public void PcControls_ItemPickup()
        {
            if (ItemNameToPick == ItemName)
            {
                if (UIButtonForPickUp != null)
                {
                    if (UIButtonForPickUp.interactable == true)
                    {
                        UIButtonForPickUp.onClick.Invoke();
                    }
                }
                ItemNameToPick = "";
            }
           
        }
        public void showUIText()
        {
            for (int x = 0; x < UITextInfo.Count;x++)
            {
                UITextInfo[x].TextMeshProText.text = UITextInfo[x].TextInfo; 
            }
        }
        public void ItemPicked()
        {
            foreach (var obj in ObjectsToDeactivateOnCollect)
            {
                obj.SetActive(false);
            }

            if (RecievedText != null)
            {
                RecievedText.gameObject.SetActive(true);
                RecievedText.text = RecievedTextInfo;
            }

            if (ApplyAmmoToAllWeapons)
            {
                foreach (var weaponScript in playerManager.PlayerWeaponScripts)
                {
                    if (weaponScript != null)
                    {
                        weaponScript.Reload.TotalAmmo += AmmoAmount;
                    }
                }
            }
            else if (ApplyAmmoToCurrentWeapon)
            {
                foreach (var weaponScript in playerManager.PlayerWeaponScripts)
                {
                    if (weaponScript != null && weaponScript.gameObject.activeInHierarchy)
                    {
                        weaponScript.Reload.TotalAmmo += AmmoAmount;
                    }
                }
            }

            if (AmmoCollectSound != null)
            {
                AmmoCollectSound.Play();
            }

            if (ColliderToDisableOnPickup != null)
            {
                ColliderToDisableOnPickup.enabled = false;
            }

            if (UIPickupPanel != null)
            {
                UIPickupPanel.gameObject.SetActive(false);
            }

            StartCoroutine(HandleAmmoFound());
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                if (AutoItemPickup == false)
                {
                    ItemNameToPick = "";
                    UIPickupPanel.gameObject.SetActive(false);
                }
            }
        }
        private IEnumerator HandleAmmoFound()
        {
            yield return new WaitForSeconds(TextDeactivationDelay);
            if (RecievedText != null)
            {
                RecievedText.gameObject.SetActive(false);
            }
            if (ShouldReactivateObjects)
            {
                yield return new WaitForSeconds(ReactivationDelay);
                foreach (var obj in ObjectsToDeactivateOnCollect)
                {
                    obj.SetActive(true);
                }
                if (ColliderToDisableOnPickup != null)
                {
                    ColliderToDisableOnPickup.enabled = true;
                }
            }
        }
    }
}



//using System.Collections;
//using UnityEngine;

//namespace MobileActionKit
//{
//    public class AmmoBox : MonoBehaviour
//    {
//        public static AmmoBox ins;

//        public GameObject AmmoFoundText;
//        public float TimeToDeactivateText = 2f;

//        public AudioSource AmmoCollectingSoundPrefab;

//        public int Ammos = 30;
//        public bool AmmoToCurrentHoldingWeapon = false;
//        public bool AmmoToAllHoldingWeapons = false;

//        public GameObject[] ObjToDeactiveOnCollect;

//        PlayerManager playerManager;

//        public bool ShouldReactivate = true;

//        public float TimeToReactivate = 3f;

//        private void Awake()
//        {
//            if (ins == null)
//            {
//                ins = this;
//            }
//        }
//        void Start()
//        {
//            playerManager = PlayerManager.instance;
//        }
//        void OnDisable()
//        {
//            for (int x = 0; x < ObjToDeactiveOnCollect.Length; x++)
//            {
//                ObjToDeactiveOnCollect[x].SetActive(true);
//            }
//        }
//        private void OnTriggerEnter(Collider other)
//        {
//            if (other.tag == "Player")
//            {
//                for (int x = 0; x < ObjToDeactiveOnCollect.Length; x++)
//                {
//                    ObjToDeactiveOnCollect[x].SetActive(false);
//                }

//                if (AmmoFoundText != null)
//                {
//                    AmmoFoundText.SetActive(true);
//                }


//                if (AmmoToAllHoldingWeapons == true)
//                {
//                    for (int i = 0; i < playerManager.AllWeaponsPlayerWeaponScript.Count; i++)
//                    {
//                        if (playerManager.AllWeaponsPlayerWeaponScript[i] != null)
//                        {
//                            playerManager.AllWeaponsPlayerWeaponScript[i].Reload.TotalAmmo += Ammos;
//                        }
//                    }
//                }
//                else if (AmmoToCurrentHoldingWeapon == true)
//                {
//                    for (int i = 0; i < playerManager.AllWeaponsPlayerWeaponScript.Count; i++)
//                    {
//                        if (playerManager.AllWeaponsPlayerWeaponScript[i] != null)
//                        {
//                            if (playerManager.AllWeaponsPlayerWeaponScript[i].gameObject.activeInHierarchy == true)
//                            {
//                                playerManager.AllWeaponsPlayerWeaponScript[i].Reload.TotalAmmo += Ammos;
//                            }
//                        }
//                    }
//                }

//                if (AmmoCollectingSoundPrefab != null)
//                {
//                    AmmoCollectingSoundPrefab.Play();
//                }


//                StartCoroutine(AmmoFound());

//            }
//        }
//        IEnumerator AmmoFound()
//        {
//            yield return new WaitForSeconds(TimeToDeactivateText);
//            if (AmmoFoundText != null)
//            {
//                AmmoFoundText.SetActive(false);
//            }
//            if (ShouldReactivate == true)
//            {
//                yield return new WaitForSeconds(TimeToReactivate);
//                for (int x = 0; x < ObjToDeactiveOnCollect.Length; x++)
//                {
//                    ObjToDeactiveOnCollect[x].SetActive(true);
//                }
//            }
//        }
//    }
//}