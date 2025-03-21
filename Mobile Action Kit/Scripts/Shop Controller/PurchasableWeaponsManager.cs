using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class PurchasableWeaponsManager : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "PurchasableWeaponsManager handles the management of weapons available for purchase. It ensures that only one weapon purchase confirmation is active at a time and interacts with ShootingWeaponsSlotManager to manage pending purchases.";

        /// <summary>
        /// Singleton instance of the PurchasableWeaponsManager.
        /// </summary>
        public static PurchasableWeaponsManager instance;

        /// <summary>
        /// Array containing all ShootingWeaponsSlotManager scripts that handle individual weapon slots.
        /// </summary>
        [Tooltip("List of all ShootingWeaponsSlotManager scripts responsible for handling weapon slots.")]
        public ShootingWeaponsSlotManager[] WeaponSlots;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        public void ConfirmationWindowReset()
        {
            for (int x = 0; x < WeaponSlots.Length; x++)
            {
                WeaponSlots[x].pendingWeapon = null;
            }
        }
    }
}