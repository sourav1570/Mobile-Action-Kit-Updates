using System.Collections;
using System.Collections.Generic;
using MobileActionKit;
using UnityEngine;

namespace MobileActionKit
{
    public class PlayerWeaponsManager : MonoBehaviour
    {
        //public bool ShowAllSlots = false;

        //[System.Serializable]
        //public class WeaponsInventory
        //{
        //    public string SlotName;
        //    public List<Weapons> Weapons = new List<Weapons>();
        //}
        //[System.Serializable]
        //public class Weapons
        //{
        //    public WeaponId WeaponIdComponent;
        //    public GameObject Weapon;
        //    public AttachmentsActivator WeaponAttachments;
        //}

        //public List<WeaponsInventory> WeaponsSlot = new List<WeaponsInventory>();

        //[System.Serializable]
        //public class WeaponsPickup
        //{
        //    public string WeaponName;
        //    public GameObject WeaponPickupPrefab;
        //}

        //public PlayerGrenadeThrower[] PlayerGrenadeThrowerScripts;

        //public MeleeAttack[] PlayerMeleeAttackScripts;

        //public List<WeaponsPickup> PickupWeapons = new List<WeaponsPickup>();

        //public List<GameObject> DebugWeapons = new List<GameObject>();

        public static PlayerWeaponsManager instance;

        [TextArea]
        public string ScriptInfo = "This script serves as the central manager for handling the player's weapon system, including inventory, pickups, and activation. " +
            "Other scripts can communicate with it to switch weapons, activate the next weapon, and manage slots.";


        [HideInInspector][Tooltip("If enabled, all available weapon slots will be visible. If disabled, only the purchased weapon slots will be shown.")]
        public bool ShowAllAvailableWeaponSlots = true;

        [System.Serializable]
        public class WeaponSlot
        {
            [Tooltip("The name of the weapon slot, e.g., Primary, Secondary.")]
            public string SlotName;

            [Tooltip("List of weapons assigned to this slot.")]
            public List<WeaponData> Weapons = new List<WeaponData>();
        }

        [System.Serializable]
        public class WeaponData
        {
            [Tooltip("Reference to the Weapon ID component.")]
            public WeaponId WeaponIdComponent;

            [Tooltip("The actual weapon GameObject.")]
            public GameObject Weapon;

            [Tooltip("Weapon attachments activator script to be placed in this field.")]
            public AttachmentsActivator WeaponAttachments;
        }

        [Tooltip("List of all weapon slots available to the player.")]
        public List<WeaponSlot> WeaponSlots = new List<WeaponSlot>();

        [System.Serializable]
        public class WeaponPickupData
        {
            [Tooltip("The name of the weapon pickup.")]
            public string WeaponName;

            [Tooltip("Prefab of the weapon pickup object.")]
            public GameObject WeaponPickupPrefab;
        }

        [Tooltip("References to all grenade-throwing scripts attached to the player.")]
        public PlayerGrenadeThrower[] PlayerGrenadeThrowerScripts;

        [Tooltip("References to all melee attack scripts attached to the player.")]
        public MeleeAttack[] MeleeAttackScripts;

        [Tooltip("List of available weapon pickups in the game.")]
        public List<WeaponPickupData> AvailableWeaponPickups = new List<WeaponPickupData>();

        [Tooltip("Debugging list for tracking active weapons.")]
        public List<GameObject> ActiveWeaponsList = new List<GameObject>();

        [HideInInspector]
        public WeaponSlotManager WeaponSlotManagerScript;

        private void Awake()
        {
            instance = this;
        }
        public void AddWeaponInListWithoutSaving(GameObject WeaponToAdd, GameObject WeaponToRemove)
        {
            // Find the index of the weapon to remove
            int slotIndex = ActiveWeaponsList.IndexOf(WeaponToRemove);

            if (slotIndex != -1) // Ensure the weapon to remove exists in the list
            {
                // Remove the weapon from the slot
                ActiveWeaponsList.RemoveAt(slotIndex);

                // Insert the new weapon at the same slot index
                ActiveWeaponsList.Insert(slotIndex, WeaponToAdd);
            }
            else
            {
                // If WeaponToRemove is not found, just add the new weapon to the end
                ActiveWeaponsList.Add(WeaponToAdd);
            }

            // If WeaponToRemove is not found, just add the new weapon to the end
            //DebugWeapons.Add(WeaponToAdd);

        }
        public void WeaponActivation(string WeaponID)
        {
            StartCoroutine(WeaponActivator(WeaponID));
        }
        IEnumerator WeaponActivator(string WeaponID)
        {
            if (PlayerManager.instance != null)
            {
                PlayerManager.instance.CurrentHoldingPlayerWeapon.StartShooting = false;
                PlayerManager.instance.CurrentHoldingPlayerWeapon.RequiredComponents.WeaponAnimatorComponent.Play(PlayerManager.instance.CurrentHoldingPlayerWeapon.RemoveAnimationName, -1, 0f);
            }

            yield return new WaitForSeconds(PlayerManager.instance.CurrentHoldingPlayerWeapon.RemoveLength);


            if(SwitchingPlayerWeapons.ins != null)
            {
                if (SwitchingPlayerWeapons.ins.AutoAimIfSwitchedFromAimedWeapon == false)
                {
                    if (PlayerManager.instance != null)
                    {
                        if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                        {
                            PlayerManager.instance.Aiming();
                            PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed = false;
                            PlayerManager.instance.CurrentHoldingPlayerWeapon.IsHipFire = true;
                        }
                    }
                }
            }
            else
            {
                if (PlayerManager.instance != null)
                {
                    if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                    {
                        PlayerManager.instance.Aiming();
                        PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed = false;
                        PlayerManager.instance.CurrentHoldingPlayerWeapon.IsHipFire = true;
                    }
                }

            }
            

            for (int i = 0; i < PlayerGrenadeThrowerScripts.Length; i++)
            {
                PlayerGrenadeThrowerScripts[i].DeactiveHand();
            }

            for (int i = 0; i < MeleeAttackScripts.Length; i++)
            {
                MeleeAttackScripts[i].DeactiveHand();
            }

            for (int x = 0; x < WeaponSlots.Count; x++)
            {
                for (int y = 0; y < WeaponSlots[x].Weapons.Count; y++)
                {
                    WeaponSlots[x].Weapons[y].Weapon.SetActive(false);
                    if (WeaponSlots[x].Weapons[y].WeaponIdComponent.WeaponName == WeaponID)
                    {
                        WeaponSlots[x].Weapons[y].Weapon.SetActive(true);
                    }
                }
            }


            if (SwitchingPlayerWeapons.ins != null)
            {
                if (PlayerManager.instance != null)
                {
                    PlayerManager.instance.FindRequiredObjects();
                }
                if (SwitchingPlayerWeapons.ins.AutoAimIfSwitchedFromAimedWeapon == false)
                {
                    if (PlayerManager.instance != null)
                    {
                       
                        if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                        {
                            PlayerManager.instance.Aiming();
                            PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed = false;
                            PlayerManager.instance.CurrentHoldingPlayerWeapon.IsHipFire = true;
                        }
                    }
                }
            }
            else
            {
                if (PlayerManager.instance != null)
                {
                    PlayerManager.instance.FindRequiredObjects();
                    if (PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed == true)
                    {
                        PlayerManager.instance.Aiming();
                        PlayerManager.instance.CurrentHoldingPlayerWeapon.IsAimed = false;
                        PlayerManager.instance.CurrentHoldingPlayerWeapon.IsHipFire = true;
                    }
                }
            }
           
        }

        //public void ForceDeactivateAllWeapons()
        //{
        //    for (int i = 0; i < PlayerGrenadeThrowerScripts.Length; i++)
        //    {
        //        PlayerGrenadeThrowerScripts[i].RequiredComponents.GrenadeAnimatorComponent.gameObject.SetActive(false);
        //    }
        //    for (int x = 0; x < WeaponsSlot.Count; x++)
        //    {
        //        for (int y = 0; y < WeaponsSlot[x].Weapons.Count; y++)
        //        {
        //            WeaponsSlot[x].Weapons[y].Weapon.SetActive(false);
        //        }
        //    }
        //}
    }
}