using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class WeaponAttachmentsManager : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script manages weapon attachments and provides a global instance to access all weapon attachment scripts.";

        [Tooltip("Singleton instance of the WeaponAttachmentsManager.")]
        public static WeaponAttachmentsManager instance;

        [Tooltip("Array of all WeaponAttachments scripts controlling different weapon attachments.")]
        public WeaponAttachments[] AllWeaponAttachmentsScript;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        public void ConfirmationWindowReset()
        {
            for (int x = 0; x < AllWeaponAttachmentsScript.Length; x++)
            {
                AllWeaponAttachmentsScript[x].currentItemToPurchase = null;
            }
        }
    }
}