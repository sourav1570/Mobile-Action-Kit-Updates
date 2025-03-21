using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class PickupItem : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script allows the player to pick up items in the game. Items can be picked up automatically or manually, and UI elements can display item information before pickup.";

        [Space(10)]
        [Tooltip("Picks the item automatically on player collision")]
        public bool AutoItemPickup;

        [Tooltip("A UI Panel to show the item information before picking it up")]
        public GameObject UIPickupPanel;

        [Tooltip("The image UI element to display the item sprite")]
        public Image ItemImage;

        [Tooltip("The button that allows manual item pickup when clicked")]
        public Button UIButtonForPickUp;

        [Tooltip("The sprite representing the item in the UI")]
        public Sprite ItemSprite;

        [Tooltip("The scale of the item image in the UI panel")]
        public Vector3 ItemImageScale;

        [Tooltip("Text UI element to display 'Item Received' notification")]
        public TextMeshProUGUI RecievedText;

        [Tooltip("The text displayed when an item is received")]
        public string RecievedTextInfo = "Item Received";

        [Tooltip("Time in seconds before hiding the received text notification")]
        public float DeactivateTextDelay = 2f;

        [Tooltip("The collider that gets disabled when the item is picked up")]
        public Collider ColliderToDisableOnPickup;

        [System.Serializable]
        public class UIText
        {
            [Tooltip("The TextMeshPro UI element for displaying additional information")]
            public TextMeshProUGUI TextMeshProText;

            [Tooltip("The text content to display in the UI element")]
            public string TextInfo;
        }

        [Tooltip("A list of UI texts to display additional information about the item")]
        public List<UIText> UITextInfo = new List<UIText>();

        [Tooltip("The item object that will be picked up and deactivated upon collection")]
        public GameObject PickupObj;

        [Tooltip("The name of the item being picked up")]
        public string ItemName;

        [HideInInspector]
        public bool CanPickNow = false;

        [Tooltip("If enabled, the item will respawn after a delay once picked up")]
        public bool ReactivateItem = true;

        [Tooltip("Time in seconds before the item respawns if ReactivateItem is enabled")]
        public float ReactivationDelay = 3f;

        [Tooltip("The target script that will execute a specific function upon pickup")]
        public MonoBehaviour TargetScript;

        [HideInInspector]
        public string SelectedFunction;

        [HideInInspector]
        public bool CantPickItem = false;

        string ItemNameToPick;

        private void Start()
        {
            if (RecievedText != null)
            {
                RecievedText.text = RecievedTextInfo;
            }
            //if (UIButtonForPickUp != null)
            //{
            //    UIButtonForPickUp.onClick.AddListener(PickItem);
            //}
        }

        public void ResettingDescription()
        {
            ScriptInfo = "Create pickup items for player. Before using it, make sure to create the item using the Items Creator Script.";
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                CanPickNow = true;
                if (AutoItemPickup)
                {
                    PickItem();
                }
                else
                {
                    ItemNameToPick = ItemName;
                    if (UIButtonForPickUp != null)
                    {
                        UIButtonForPickUp.onClick.RemoveAllListeners();
                        UIButtonForPickUp.onClick.AddListener(PickItem);
                    }
                    if (RecievedText != null)
                    {
                        RecievedText.text = RecievedTextInfo;
                    }
                    UIPickupPanel.SetActive(true);
                    if (ItemImage != null && ItemSprite != null)
                    {
                        ItemImage.sprite = ItemSprite;
                        ItemImage.transform.localScale = ItemImageScale;
                    }
                    ShowUIText();
                }
            }
        }
        public void PcControls_ItemPickup()
        {
            if(ItemNameToPick == ItemName)
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
        public void ShowUIText()
        {
            foreach (var uiText in UITextInfo)
            {
                uiText.TextMeshProText.text = uiText.TextInfo;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!AutoItemPickup)
                {
                    ItemNameToPick = "";
                    CanPickNow = false;
                    UIPickupPanel.SetActive(false);
                }
            }
        }

        public void PickItem()
        {
            if(CantPickItem == false)
            {
                // Additional logic for picking up items (original implementation)
                if (RecievedText != null)
                {
                    RecievedText.gameObject.SetActive(true);
                    RecievedText.text = RecievedTextInfo;
                }

                if (ColliderToDisableOnPickup != null)
                {
                    ColliderToDisableOnPickup.enabled = false;
                }
                if (UIPickupPanel != null)
                {
                    UIPickupPanel.SetActive(false);
                }
                PickupObj.SetActive(false);

                if (TargetScript != null && !string.IsNullOrEmpty(SelectedFunction))
                {
                    MethodInfo method = TargetScript.GetType().GetMethod(SelectedFunction);
                    method?.Invoke(TargetScript, null);
                }

                StartCoroutine(HandleReactivation());
            }
          
        }

        private IEnumerator HandleReactivation()
        {
            yield return new WaitForSeconds(DeactivateTextDelay);

            if (RecievedText != null)
            {
                RecievedText.gameObject.SetActive(false);
            }

            if (ReactivateItem)
            {
                yield return new WaitForSeconds(ReactivationDelay);
                PickupObj.SetActive(true);
                if (ColliderToDisableOnPickup != null)
                {
                    ColliderToDisableOnPickup.enabled = true;
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PickupItem))]
    public class PickupItemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PickupItem pickupItem = (PickupItem)target;

            if (pickupItem.TargetScript != null)
            {
                var methods = pickupItem.TargetScript.GetType()
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
                    .Select(m => m.Name)
                    .ToArray();

                int selectedIndex = Array.IndexOf(methods, pickupItem.SelectedFunction);
                selectedIndex = EditorGUILayout.Popup("Select Function", selectedIndex, methods);

                if (selectedIndex >= 0)
                {
                    pickupItem.SelectedFunction = methods[selectedIndex];
                }
            }
        }
    }
#endif
}


//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//namespace MobileActionKit
//{
//    public class PickupItem : MonoBehaviour
//    {
//        [TextArea]
//        [ContextMenuItem("Reset Description", "ResettingDescription")]
//        public string ScriptInfo = "Create pickup items for player Before Using it Make sure to create the Item Using Items Creator Script";
//        [Space(10)]

//        [Tooltip("Picks the Item Automatically on Player Collision")]
//        public bool AutoItemPickup;
//        [Tooltip("A Info Panel to show the Item Information before picking it up")]
//        public GameObject UIPickupPanel;

//        public Button UIButtonForPickUp;

//        public TextMeshProUGUI RecievedText;
//        public string RecievedTextInfo = "Item Recieved";

//        public float DeactivateTextDelay = 2f;

//        public Collider ColliderToDisableOnPickup;

//        [System.Serializable]
//        public class UIText
//        {
//            public TextMeshProUGUI TextMeshProText;
//            public string TextInfo;
//        }

//        public List<UIText> UITextInfo = new List<UIText>();

//        public GameObject PickupObj;
//        public string ItemName;

//        [HideInInspector]
//        public bool CanPickNow = false;

//        public bool ShouldReactivateObjects = true;
//        public float ReactivateDelay = 3f;

//        private void Start()
//        {
//            if (UIButtonForPickUp != null)
//            {
//                UIButtonForPickUp.onClick.AddListener(PickItem);
//            }

//        }
//        public void ResettingDescription()
//        {
//            ScriptInfo = "Create pickup items for player Before Using it Make sure to create the Item Using Items Creator Script";
//        }
//        private void OnTriggerEnter(Collider other)
//        {
//            if (other.gameObject.tag == "Player")
//            {
//                CanPickNow = true;
//                if (AutoItemPickup == true)
//                {
//                    PickItem();
//                }
//                else
//                {
//                    UIPickupPanel.gameObject.SetActive(true);
//                    showUIText();
//                }
//            }
//        }
//        public void showUIText()
//        {
//            for (int x = 0; x < UITextInfo.Count; x++)
//            {
//                UITextInfo[x].TextMeshProText.text = UITextInfo[x].TextInfo;
//            }
//        }
//        private void OnTriggerExit(Collider other)
//        {
//            if (other.gameObject.tag == "Player")
//            {
//                if (AutoItemPickup == false)
//                {
//                    CanPickNow = false;
//                    UIPickupPanel.gameObject.SetActive(false);
//                }
//            }
//        }
//        public void PickItem()
//        {
//            if (ItemsPickupManager.instance != null)
//            {
//                for (int x = 0; x < ItemsPickupManager.instance.ItemUsageScripts.Length; x++)
//                {
//                    if (ItemsPickupManager.instance.ItemUsageScripts[x].ItemName == ItemName)
//                    {
//                        if (PlayerPrefs.GetInt(ItemName + "CurrentlyCarried") < ItemsPickupManager.instance.ItemUsageScripts[x].MaxCarry)
//                        {
//                            PlayerPrefs.SetInt(ItemName + "TemporaryStored", PlayerPrefs.GetInt(ItemName + "TemporaryStored") + 1);
//                            PlayerPrefs.SetInt(ItemName + "CurrentlyCarried", ++ItemsPickupManager.instance.ItemUsageScripts[x].CurrentlyCarried);
//                            ItemsManager.Instance.ShowPurchaseInfo(ItemsPickupManager.instance.ItemUsageScripts[x].ItemTexts, ItemName);
//                            ItemsPickupManager.instance.ItemUsageScripts[x].ButtonFunction(true);
//                            PickupObj.SetActive(false);
//                        }
//                    }
//                }
//                if (RecievedText != null)
//                {
//                    RecievedText.text = RecievedTextInfo;
//                }
//                if (UIPickupPanel != null)
//                {
//                    UIPickupPanel.gameObject.SetActive(false);
//                }
//                if (ColliderToDisableOnPickup != null)
//                {
//                    ColliderToDisableOnPickup.enabled = false;
//                }

//                StartCoroutine(HandleReactivation());
//            }
//        }
//        private IEnumerator HandleReactivation()
//        {
//            yield return new WaitForSeconds(DeactivateTextDelay);
//            if (RecievedText != null)
//            {
//                RecievedText.gameObject.SetActive(false);
//            }
//            if (ShouldReactivateObjects)
//            {
//                yield return new WaitForSeconds(ReactivateDelay);
//                PickupObj.SetActive(true);
//                if (ColliderToDisableOnPickup != null)
//                {
//                    ColliderToDisableOnPickup.enabled = true;
//                }
//            }
//        }
//    }
//}