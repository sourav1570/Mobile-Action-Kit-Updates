using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MobileActionKit
{
    public class AvatarWeaponSwitcher : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "Handles weapon switching for an avatar by checking active weapon states and enabling or disabling the correct weapon and its attachments.";

        [System.Serializable]
        public class weaponholdedbysoldier
        {
            [Tooltip("The GameObject that determines whether this weapon should be activated.")]
            public GameObject WeaponActivationToCheck;

            [Tooltip("An array of GameObjects that determine whether corresponding attachments should be activated.")]
            public GameObject[] WeaponAttachmentsActivationToCheck;

            [Tooltip("The weapon GameObject to activate when 'WeaponActivationToCheck' is active.")]
            public GameObject WeaponToActivate;

            [Tooltip("The corresponding attachments that should be activated when the matching elements in 'WeaponAttachmentsActivationToCheck' are active.")]
            public GameObject[] Attachments;
        }

        [Tooltip("List of primary weapons held by the soldier. Each entry defines a weapon, its activation check object, and its attachments.")]
        public List<weaponholdedbysoldier> PrimaryWeaponsHoldedBySoldier = new List<weaponholdedbysoldier>();

        private void Start()
        {
            StartCoroutine(DelayBeforeCheck());
        }
        private void OnEnable()
        {
            ActivatePrimaryWeapon();
        }
        IEnumerator DelayBeforeCheck()
        {
            yield return new WaitForSeconds(0.000001f); // So there would be enough time for objects to get activated and after that we can activate WeaponToActivate and its attachement 
            ActivatePrimaryWeapon();
        }
        public void ActivatePrimaryWeapon()
        {
            // Loop through each weapon held by the soldier
            foreach (var weapon in PrimaryWeaponsHoldedBySoldier)
            {
                // Check if the WeaponActivationToCheck is active
                if (weapon.WeaponActivationToCheck.activeSelf)
                {
                    // Activate the corresponding WeaponToActivate
                    weapon.WeaponToActivate.SetActive(true);

                    // Loop through the weapon attachments and activate the corresponding attachment based on the active state
                    for (int i = 0; i < weapon.WeaponAttachmentsActivationToCheck.Length; i++)
                    {
                        if (weapon.WeaponAttachmentsActivationToCheck[i].activeSelf)
                        {
                            if (i < weapon.Attachments.Length)
                            {
                                weapon.Attachments[i].SetActive(true); // Activate the corresponding attachment
                            }
                        }
                        else
                        {
                            if (i < weapon.Attachments.Length)
                            {
                                weapon.Attachments[i].SetActive(false); // Deactivate the corresponding attachment
                            }
                        }
                    }
                }
                else
                {
                    // If the WeaponActivationToCheck is not active, deactivate the corresponding WeaponToActivate
                    weapon.WeaponToActivate.SetActive(false);

                    // Deactivate all attachments when the weapon is not active
                    foreach (var attachment in weapon.Attachments)
                    {
                        attachment.SetActive(false);
                    }
                }
            }
        }
    }   
}