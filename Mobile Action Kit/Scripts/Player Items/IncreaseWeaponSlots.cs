using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class IncreaseWeaponSlots : MonoBehaviour
    {
        public ShopMenuWeaponsManager shopMenuWeapons;

        public void OneExtraSlot()
        {
            // Create a new array with one additional slot
            ShootingWeaponsSlotManager[] newSlots = new ShootingWeaponsSlotManager[4];

            // Copy existing slots into the new array
            for (int i = 0; i < shopMenuWeapons.WeaponSlots.Length; i++)
            {
                newSlots[i] = shopMenuWeapons. WeaponSlots[i];
            }

            // Assign the new array back to WeaponSlots
            shopMenuWeapons.WeaponSlots = newSlots;
        }
        public void TwoExtraSlots()
        {
            // Create a new array with one additional slot
            ShootingWeaponsSlotManager[] newSlots = new ShootingWeaponsSlotManager[5];

            // Copy existing slots into the new array
            for (int i = 0; i < shopMenuWeapons.WeaponSlots.Length; i++)
            {
                newSlots[i] = shopMenuWeapons.WeaponSlots[i];
            }

            // Assign the new array back to WeaponSlots
            shopMenuWeapons.WeaponSlots = newSlots;
        }
        public void ThreeExtraSlots()
        {
            // Create a new array with one additional slot
            ShootingWeaponsSlotManager[] newSlots = new ShootingWeaponsSlotManager[6];

            // Copy existing slots into the new array
            for (int i = 0; i < shopMenuWeapons.WeaponSlots.Length; i++)
            {
                newSlots[i] = shopMenuWeapons.WeaponSlots[i];
            }

            // Assign the new array back to WeaponSlots
            shopMenuWeapons.WeaponSlots = newSlots;
        }
    }
}