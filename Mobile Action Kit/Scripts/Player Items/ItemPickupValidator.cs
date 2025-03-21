using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class ItemPickupValidator : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script checks whether the player can pick up an item based on their carrying limit. " +
                                  "If the player has reached the maximum carry limit, the item cannot be picked up, and the UI button will be non interactable.";

        [Tooltip("Reference to the PickupItem script controlling the pickup behavior.")]
        public PickupItem PickupItemScript;

        [Tooltip("Reference to the ItemUsageController script that tracks currently carried items.")]
        public ItemUsageController ItemUsageControllerScript;

        [Tooltip("The UI button that allows the player to claim the item.")]
        public Button ItemClaimButton;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.transform.root.tag == "Player")
            {
                if (ItemUsageControllerScript.CurrentlyCarried >= ItemUsageControllerScript.CanMaximumCarry)
                {
                    PickupItemScript.CantPickItem = true;
                    ItemClaimButton.interactable = false;
                }
                else
                {
                    PickupItemScript.CantPickItem = false;
                    ItemClaimButton.interactable = true;
                }

            }
        }

    }
}
