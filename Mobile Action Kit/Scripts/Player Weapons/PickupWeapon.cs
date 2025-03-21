using System.Collections;
using UnityEngine;

// This Script is Responsible For Weapons which are Spreaded in The Scene
namespace MobileActionKit
{
    public class PickupWeapon : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script allows weapons placed in the scene to be picked up by the player. " +
     "When the player enters the pickup area, the script checks if the weapon already exists in their inventory. " +
     "If a matching weapon is found, it can either replace the existing weapon or can just pickup its ammo. " +
     "The script also provides initial ammo, and optionally activates " +
     "a random attachment when the game begin. Additionally, it supports displaying a UI confirmation for weapon pickups. The weapon becomes " +
     "inactive after being picked up to prevent duplicate pickups.";

        [Space(10)]
        [Tooltip("If enabled, the weapon will replace an existing weapon in the active weapons list of the 'PlayerWeaponsManager' script if a weapon with the same slot name exists. " +
         "If no matching slot weapon exists, it will replace the currently equipped weapon. " +
         "If an empty slot is available, the weapon will be added to the player's inventory. " +
         "If disabled, the weapon will either be added to an available empty slot in the active weapons list of the 'PlayerWeaponsManager' script or replace the currently equipped weapon.")]
        public bool ReplaceWeaponBySlotIfExist = true;

        [Tooltip("Enter the same slot name that you have entered in the 'PlayerWeaponsManager' Script.")]
        public string SlotName = "Primary_Slot";

        [Tooltip("If enabled, a UI confirmation will be shown to the player when they attempt to pick up the weapon.")]
        public bool ShowWeaponPickupConfirmationUI = false;

        [Tooltip("The unique identifier for this weapon. It is used to check if the player already owns this weapon in their inventory.")]
        public string KeyName;

        [Tooltip("The amount of ammo the pickup weapon have by default.")]
        public int InitialAmmo;

        [Tooltip("The sprite image representing this weapon, used for UI and inventory display.")]
        public Sprite WeaponSprite;

        [Tooltip("If enabled, a random attachment from the provided list will be activated when the game starts.")]
        public bool ActivateRandomAttachment = false;

        [Tooltip("A list of weapon attachments.")]
        public AttachmentKey[] AttachmentsToActivate;

        bool IsWeaponPicked = false;

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (ActivateRandomAttachment == true)
            {
                int Randomise = Random.Range(0, AttachmentsToActivate.Length);
                AttachmentsToActivate[Randomise].gameObject.SetActive(true);
            }
        }
        private void OnEnable()
        {
            IsWeaponPicked = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" && IsWeaponPicked == false)
            {
                if (other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>() != null)
                {
                    other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().PickupWeaponScript = this;

                    for (int x = 0; x < other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().PlayerWeaponsManagerScript.WeaponSlots.Count; x++)
                    {
                        for (int i = 0; i < other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().PlayerWeaponsManagerScript.WeaponSlots[x].Weapons.Count; i++)
                        {
                            if (other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].WeaponIdComponent.WeaponName == KeyName)
                            {
                                if (ShowWeaponPickupConfirmationUI == false)
                                {
                                    other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().SlotName = SlotName;
                                    other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().Ammo = InitialAmmo;
                                    other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().Icon = WeaponSprite;
                                    other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().WeaponToCollect = other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon;
                                    other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().StartCoroutine(other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().WeaponActivator(other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon));
                                    IsWeaponPicked = true;
                                }
                                else
                                {
                                    other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().NameOfTheWeaponToPick = KeyName;
                                    other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().SlotName = SlotName;
                                    other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().Ammo = InitialAmmo;
                                    other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().Icon = WeaponSprite;
                                    other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().WeaponToCollect = other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().PlayerWeaponsManagerScript.WeaponSlots[x].Weapons[i].Weapon;
                                    other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().ActivateWeaponInfo();
                                    IsWeaponPicked = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                if (other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>() != null)
                {
                    IsWeaponPicked = false;
                    if (ShowWeaponPickupConfirmationUI == true)
                    {
                        other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().NameOfTheWeaponToPick = "";
                        other.gameObject.transform.root.transform.GetComponent<WeaponPickupManager>().DeactivateWeaponInfo();
                      
                    }
                }
            }
        }
    }
}