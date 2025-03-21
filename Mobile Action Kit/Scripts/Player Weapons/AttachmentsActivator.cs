using UnityEngine;
using System.Collections.Generic;
using System.Reflection;


namespace MobileActionKit
{
    public class AttachmentsActivator : MonoBehaviour
    {
        [System.Serializable]
        public class InventoryItem
        {
            //[HideInInspector]
            [Tooltip("Indicates if this item is currently activated. This is managed dynamically by the script.")]
            public bool IsThisItemActivatedCurrently = false;

            [Header("PlayerPrefs Key and Activation Settings")]
            [Tooltip("Unique key name used to track the activation state of this inventory item in PlayerPrefs.")]
            public string keyName;

            [Tooltip("GameObjects that should be activated when this inventory item is equipped.")]
            public GameObject[] ObjectsToActivate;

            [Header("Function Call Settings")]
            [Tooltip("Script that contains the function to be called when this item is activated.")]
            public MonoBehaviour AttachmentFunctionScript; // The script containing the functions

            [Tooltip("Name of the function to be called from the specified script when this item is activated.")]
            public string AttachmentFunction; // The name of the selected function (set via custom editor)
        }

        [Header("Inventory Items")]
        [Tooltip("List of inventory items with their respective activation settings and behaviors.")]
        public List<InventoryItem> inventoryItems = new List<InventoryItem>();

        private void Start()
        {
            ActivateItems();
        }
        public void ActivateItems()
        {
            if(SwitchingPlayerWeapons.ins != null)
            {
                if (SwitchingPlayerWeapons.ins.AvailableWeapons == SwitchingPlayerWeapons.WeaponActivationType.ShopDependent) // Important to add so that if we pick up the weapon with a different attachment we do not have to save it
                                                                                                                              // we only have save and load when the attachment is purchased from shop menu.
                               
                {
                    foreach (var item in inventoryItems)
                    {
                        if (PlayerPrefs.GetInt(item.keyName, 0) == 1) // Check if item is equipped
                        {
                            ActivateObjects(item);
                            CallSelectedFunction(item);
                        }
                    }
                }
            }        

            for (int x = 0; x < inventoryItems.Count; x++)
            {
                for (int y = 0; y < inventoryItems[x].ObjectsToActivate.Length; y++)
                {
                    if (inventoryItems[x].ObjectsToActivate[y].activeSelf)
                    {
                        inventoryItems[x].IsThisItemActivatedCurrently = true;
                        PlayerPrefs.SetInt(inventoryItems[x].keyName, 1);
                    }
                }
            }
        }
        public void ActivateItemsWithoutSaving(string keyName)
        {
            foreach (var item in inventoryItems)
            {
                if (item.keyName == keyName) // Check if item is equipped
                {
                    ActivateObjects(item);
                    CallSelectedFunction(item);
                }
            }
        }
        private void ActivateObjects(InventoryItem item)
        {
            foreach (var obj in item.ObjectsToActivate)
            {
                item.IsThisItemActivatedCurrently = true;
                obj.SetActive(true);
            }
        }

        private void CallSelectedFunction(InventoryItem item)
        {
            if (!string.IsNullOrEmpty(item.AttachmentFunction) && item.AttachmentFunctionScript != null)
            {
                MethodInfo method = item.AttachmentFunctionScript.GetType().GetMethod(item.AttachmentFunction, BindingFlags.Instance | BindingFlags.Public);
                method?.Invoke(item.AttachmentFunctionScript, null);
            }
        }
    }
}