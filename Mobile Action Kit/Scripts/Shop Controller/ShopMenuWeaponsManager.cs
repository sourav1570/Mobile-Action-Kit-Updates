using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MobileActionKit
{
    public class ShopMenuWeaponsManager : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script manages the player's carried weapons, ensuring equipped weapons are tracked and displayed correctly across weapon slots. It interacts with PlayerPrefs to save and retrieve weapon ownership and status.";

        public static ShopMenuWeaponsManager instance;

        [Header("Add 'ShootingWeaponsSlotManager' Scripts In Sequence")]
        [Tooltip("Array of ShootingWeaponsSlotManager scripts, defining weapon slots.")]
        public ShootingWeaponsSlotManager[] WeaponSlots;

        [Tooltip("List of weapons currently purchased and displayed in each slot.")]
        public List<GameObject> DisplayPurchasedWeapon = new List<GameObject>();

        int currentindex;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        private void Start()
        {
            PlayerPrefs.SetInt("SlotsDefined", WeaponSlots.Length);
        }
        public void UpdateWeaponsCarried()
        {
            currentindex = 0;
            DisplayPurchasedWeapon.Clear();
            for (int x = 0; x < WeaponSlots.Length; x++)
            {
                for (int i = 0; i < WeaponSlots[x].AddWeapons.Count; i++)
                {
                    if (PlayerPrefs.GetInt(WeaponSlots[x].AddWeapons[i].Weapon.GetComponent<WeaponId>().WeaponName + currentindex) == 1)
                    {
                        PlayerPrefs.SetInt(WeaponSlots[x].AddWeapons[i].Weapon.GetComponent<WeaponId>().WeaponName + currentindex, 0);
                        ++currentindex;
                    }
                }
            }
            currentindex = 0;
            for (int x = 0; x < WeaponSlots.Length; x++)
            {
                for (int i = 0; i < WeaponSlots[x].AddWeapons.Count; i++)
                {
                    if (PlayerPrefs.GetInt(WeaponSlots[x].AddWeapons[i].WeaponIdScript.WeaponName + "Equipped") == 1)
                    {
                        if (!DisplayPurchasedWeapon.Contains(WeaponSlots[x].AddWeapons[i].Weapon))
                        {
                            DisplayPurchasedWeapon.Add(WeaponSlots[x].AddWeapons[i].Weapon);

                            PlayerPrefs.SetInt(WeaponSlots[x].AddWeapons[i].Weapon.GetComponent<WeaponId>().WeaponName + currentindex, 1);

                            ++currentindex;
                        }
                    }
                }
            }
        }

    }
}