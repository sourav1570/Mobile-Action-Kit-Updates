using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MobileActionKit
{
    public class ItemUsageController : MonoBehaviour
    {

        public static ItemUsageController instance;

        [TextArea]
        public string ScriptInfo = "Manages item usage, updates UI elements, handles item interactions, and invokes custom user-defined functions when items are used or game restarts.";
        [Space(10)]

        [HideInInspector][Tooltip("A Unique name To Save The Item Data")]
        public string ItemName = "";
        [Tooltip("Drag and Drop The Objects Which you want to deactivate After Each Use For example : Grenades")]
        public GameObject[] GameobjectsToDeactivateOnUIButtonClick;
        [HideInInspector] [Tooltip("A Timer For Deactivating Objects")]
        public float GameobjectsDeactivationDelay = 2.3f;

        [Tooltip("Item Text To be Updated Each Time Item is Being Used For Example : If you use 1 grenade out of 3 greandes The text will show 2 grenades")]
        public TextMeshProUGUI[] ItemTexts;
        [HideInInspector]
        [Tooltip("Number of items player carry on each time level launches")]
        public int DefaultItems;
        [HideInInspector]
        [Tooltip("Number of items player can maximum carry in Level")]
        public int CanMaximumCarry;

        public Button[] ItemUIButtons;
        [HideInInspector]
        public float TimeToEnableButtons = 2.8f;
        [HideInInspector]
        public int CurrentlyCarried;

        [Space]
        [HideInInspector]
        [Tooltip("field to call custom function.")]
        public MonoBehaviour AddScript;
        [HideInInspector]
        public string FunctionToInvokeWhenUsing;
        [HideInInspector]
        public string FunctionToInvokeWhenGameRestart;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        void Start()
        {
           
            if (DefaultItems >= CanMaximumCarry)
            {
                DefaultItems = CanMaximumCarry;
            }

            PlayerPrefs.SetInt(ItemName + "TemporaryStored", DefaultItems);
            ItemInventoryManager.Instance.ShowPurchaseInfo(ItemTexts, ItemName);

            CurrentlyCarried = PlayerPrefs.GetInt(ItemName + "TemporaryStored") + PlayerPrefs.GetInt(ItemName + "DisplayNumberOfPurchases", 0);

            if (CurrentlyCarried >= CanMaximumCarry)
            {
                CurrentlyCarried = CanMaximumCarry;
            }
            PlayerPrefs.SetInt(ItemName + "CurrentlyCarried", CurrentlyCarried);

            for (int y = 0; y < ItemTexts.Length; y++)
            {
                ItemTexts[y].text = PlayerPrefs.GetInt(ItemName + "CurrentlyCarried").ToString();
            }


            if (PlayerPrefs.GetInt(ItemName + "CurrentlyCarried") <= 0)
            {
                StartCoroutine(DeactivateObjects());
                ButtonFunction(false);
                for (int y = 0; y < ItemTexts.Length; y++)
                {
                    ItemTexts[y].text = "0";
                }
            }

            if (AddScript != null)
            {
                MethodInfo method = AddScript.GetType().GetMethod(FunctionToInvokeWhenGameRestart);
                if (method != null)
                {
                    // Check if the method is public and has no parameters
                    if (method.IsPublic && method.GetParameters().Length == 0)
                    {
                        method.Invoke(AddScript, null);
                    }
                    else
                    {
                        Debug.LogError("Function '" + FunctionToInvokeWhenGameRestart + "' is not public or has parameters.");
                    }
                }
            }

            for (int x = 0; x < ItemUIButtons.Length; x++)
            {
                ItemUIButtons[x].onClick.AddListener(CheckItemUse);
            }
        }
        public void AddThisItem()
        {
            if (DefaultItems >= CanMaximumCarry)
            {
                DefaultItems = CanMaximumCarry;
            }

            CurrentlyCarried = PlayerPrefs.GetInt(ItemName + "TemporaryStored") + 1;
            PlayerPrefs.SetInt(ItemName + "TemporaryStored", CurrentlyCarried);
 
            ItemInventoryManager.Instance.ShowPurchaseInfo(ItemTexts, ItemName);

            CurrentlyCarried = PlayerPrefs.GetInt(ItemName + "TemporaryStored") + PlayerPrefs.GetInt(ItemName + "DisplayNumberOfPurchases", 0);

            if (CurrentlyCarried >= CanMaximumCarry)
            {
                CurrentlyCarried = CanMaximumCarry;
            }
            PlayerPrefs.SetInt(ItemName + "CurrentlyCarried", CurrentlyCarried);

            for (int y = 0; y < ItemTexts.Length; y++)
            {
                ItemTexts[y].text = PlayerPrefs.GetInt(ItemName + "CurrentlyCarried").ToString();
            }

            for (int x = 0; x < GameobjectsToDeactivateOnUIButtonClick.Length; x++)
            {
                GameobjectsToDeactivateOnUIButtonClick[x].gameObject.SetActive(true);
            }

            for (int x = 0; x < ItemUIButtons.Length; x++)
            {
                ItemUIButtons[x].interactable = true;
            }
        }
        public void ItemIsInUse()
        {
            if (PlayerPrefs.GetInt(ItemName + "CurrentlyCarried") <= 0)
            {
                StartCoroutine(DeactivateObjects());
                ButtonFunction(false);

                for (int y = 0; y < ItemTexts.Length; y++)
                {
                    ItemTexts[y].text = "0";
                }
            }
            else
            {
                PlayerPrefs.SetInt(ItemName + "CurrentlyCarried", --CurrentlyCarried);
                ButtonFunction(true);
                
                if (AddScript != null)
                {
                    MethodInfo method = AddScript.GetType().GetMethod(FunctionToInvokeWhenUsing);
                    if (method != null)
                    {
                        if (method.IsPublic && method.GetParameters().Length == 0)
                        {
                            method.Invoke(AddScript, null);
                        }
                        else
                        {
                            Debug.LogError("Function '" + FunctionToInvokeWhenUsing + "' is not public or has parameters.");
                        }
                    }
                }


                ItemInventoryManager.Instance.UpdatePurchaseInfo(ItemTexts, ItemName);
                for (int y = 0; y < ItemTexts.Length; y++)
                {
                    ItemTexts[y].text = PlayerPrefs.GetInt(ItemName + "CurrentlyCarried").ToString();
                }

            }
            if (ItemInventoryManager.Instance.PurchasableItem(ItemName) <= 0)
            {
                StartCoroutine(DeactivateObjects());
                ButtonFunction(false);
                for (int y = 0; y < ItemTexts.Length; y++)
                {
                    ItemTexts[y].text = "0";
                }
            }
            else
            {
                ButtonFunction(false);
                StartCoroutine(Timer());
            }
          
            StartCoroutine(DeactivateObjects());
        }
        public void CheckItemUse()
        {
            if(GetComponent<ItemUsageCheck>() == null)
            {
                ItemIsInUse();
            }
            else
            {
                if (AddScript != null)
                {
                    if (PlayerPrefs.GetInt(ItemName + "CurrentlyCarried") > 0)
                    {
                        MethodInfo method = AddScript.GetType().GetMethod(FunctionToInvokeWhenUsing);
                        if (method != null)
                        {
                            if (method.IsPublic && method.GetParameters().Length == 0)
                            {
                                method.Invoke(AddScript, null);
                            }
                            else
                            {
                                Debug.LogError("Function '" + FunctionToInvokeWhenUsing + "' is not public or has parameters.");
                            }
                        }
                    }
                }

                StartCoroutine(DelayItemUse());
            }
        }
        IEnumerator DelayItemUse()
        {
            yield return new WaitForSeconds(0.0001f);
            if (PlayerPrefs.GetInt(ItemName + "IsInUse") == 1)
            {
                ItemIsInUse();
                PlayerPrefs.SetInt(ItemName + "IsInUse", 0);
            }
            
          
        }
        IEnumerator DeactivateObjects()
        {
            yield return new WaitForSeconds(GameobjectsDeactivationDelay);
            for (int x = 0; x < GameobjectsToDeactivateOnUIButtonClick.Length; x++)
            {
                GameobjectsToDeactivateOnUIButtonClick[x].gameObject.SetActive(false);
            }
        }
        IEnumerator Timer()
        {
            yield return new WaitForSeconds(TimeToEnableButtons);
            ButtonFunction(true);
        }
        public void ButtonFunction(bool Activate)
        {
            for (int x = 0; x < ItemUIButtons.Length; x++)
            {
                ItemUIButtons[x].interactable = Activate;
            }

            if (PlayerPrefs.GetInt(ItemName + "CurrentlyCarried") > 0)
            {
                for (int x = 0; x < GameobjectsToDeactivateOnUIButtonClick.Length; x++)
                {
                    GameobjectsToDeactivateOnUIButtonClick[x].gameObject.SetActive(true);
                }
            }
        }
    }
}

