using System.Collections;
using UnityEngine.UI;
using UnityEngine;
namespace MobileActionKit
{
    // This Script is Responsible For Resizing and Dragging All The Game UI 
    public class HUDSettings : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script allows users to enable/disable resizing and dragging of UI elements in the game HUD.It also provides options to save, reset, and edit HUD settings.";
        [Space(10)]
        [Tooltip("The main panel containing the HUD settings UI.")]
        public GameObject HudSettingsPanel;
        private DragImage[] d;
        private ResizeImage[] r;
        private PauseManager manager;
        [Tooltip("The pause menu panel that should be disabled when editing HUD settings.")]
        public GameObject PausePanel;
        [Tooltip("Button that opens the HUD settings panel.")]
        public Button HudActivatorButton;
        [Tooltip("Button to enable dragging mode for UI elements.")]
        public Button DragButton;
        [Tooltip("Button to enable resizing mode for UI elements.")]
        public Button ResizeButton;
        [Tooltip("Button to save the current HUD layout and settings.")]
        public Button SaveButton;
        [Tooltip("Button to reset the HUD layout to default settings.")]
        public Button ResetButton;
        //[Header("For Devs to reset hud settings")]
        //public bool ResetHudSettings = false;
        private void Awake()
        {
            d = FindObjectsOfType<DragImage>();
            r = FindObjectsOfType<ResizeImage>();
            DisableDragging();
        }
        private void Start()
        {
            if(HudActivatorButton != null)
            {
                HudActivatorButton.onClick.AddListener(EditHudSettings);
            }
            DragButton.onClick.AddListener(EnableDragging);
            ResizeButton.onClick.AddListener(EnableResizing);
            SaveButton.onClick.AddListener(SaveHudSettings);
            ResetButton.onClick.AddListener(ResetAllHudSettings);
            //if (ResetHudSettings == true)
            //{
            //    ResetAllHudSettings();
            //}
            //else
            //{
                SaveHudSettings();
            //}
        }
        public void SaveHudSettings() // Saving All Hud Settings 
        {
            for (int i = 0; i < d.Length; i++)
            {
                d[i].EnableDragging = false;
                d[i].SaveSettings();
                for (int x = 0; x < d[i].transform.childCount; x++)
                {
                    if (d[i].transform.GetComponent<Image>() != null)
                    {
                        d[i].transform.GetComponent<Image>().enabled = false;
                        // Get the Image component
                        Image image = d[i].transform.GetComponent<Image>();
                        // Set the alpha to 0
                        if (image != null)
                        {
                            Color tempColor = image.color;
                            tempColor.a = 1; // Set alpha to 0
                            image.color = tempColor;
                        }
                    }
                    if (d[i].transform.GetChild(x).gameObject.GetComponent<Image>() != null)
                    {
                        if (d[i].transform.GetChild(x).gameObject.GetComponent<Image>() != null)
                        {
                            d[i].transform.GetChild(x).gameObject.GetComponent<Image>().raycastTarget = true;
                        }
                    }
                }
            }
            for (int i = 0; i < r.Length; i++)
            {
                r[i].EnableResizing = false;
                // In case you only using resizing let's make sure change everything
                for (int x = 0; x < r[i].transform.childCount; x++)
                {
                    if (r[i].transform.GetComponent<Image>() != null)
                    {
                        r[i].transform.GetComponent<Image>().enabled = false;
                        // Get the Image component
                        Image image = r[i].transform.GetComponent<Image>();
                        // Set the alpha to 0
                        if (image != null)
                        {
                            Color tempColor = image.color;
                            tempColor.a = 1; // Set alpha to 0
                            image.color = tempColor;
                        }
                    }
                    if (r[i].transform.GetChild(x).gameObject.GetComponent<Image>() != null)
                    {
                        if (r[i].transform.GetChild(x).gameObject.GetComponent<Image>() != null)
                        {
                            r[i].transform.GetChild(x).gameObject.GetComponent<Image>().raycastTarget = true;
                        }
                    }
                }
            }
            HudSettingsPanel.SetActive(false);
            Time.timeScale = 1f;
        }
        public void EditHudSettings()// Editing Hud Settings 
        {
            PausePanel.SetActive(false);
            HudSettingsPanel.SetActive(true);
            for (int i = 0; i < d.Length; i++)
            {
                for (int x = 0; x < d[i].transform.childCount; x++)
                {
                    if (d[i].transform.GetComponent<Image>() != null)
                    {
                        d[i].transform.GetComponent<Image>().enabled = false;
                    }
                    if (d[i].transform.GetChild(x).gameObject.GetComponent<Image>() != null)
                    {
                        d[i].transform.GetChild(x).gameObject.GetComponent<Image>().raycastTarget = false;
                    }
                }
            }
            for (int i = 0; i < d.Length; i++)
            {
                d[i].EnableDragging = false;
                d[i].enabled = false;
            }
            for (int i = 0; i < r.Length; i++)
            {
                r[i].EnableResizing = false;
                r[i].enabled = false;
            }
            Time.timeScale = 0f;
        }
        void DisableDragging()
        {
            for (int i = 0; i < d.Length; i++)
            {
                d[i].EnableDragging = false;
                d[i].enabled = false;
            }
        }
        public void EnableDragging()
        {
            for (int i = 0; i < d.Length; i++)
            {
                d[i].enabled = true;
                d[i].EnableDragging = true;
                for (int x = 0; x < d[i].transform.childCount; x++)
                {
                    // Enable the Image component
                    if (d[i].transform.GetComponent<Image>() != null)
                    {
                        d[i].transform.GetComponent<Image>().enabled = true;
                        // Get the Image component
                        Image image = d[i].transform.GetComponent<Image>();
                        // Set the alpha to 0
                        if (image != null)
                        {
                            Color tempColor = image.color;
                            tempColor.a = 0; // Set alpha to 0
                            image.color = tempColor;
                        }
                            if (d[i].transform.GetChild(x).gameObject.GetComponent<Image>() != null)
                            {
                                d[i].transform.GetChild(x).gameObject.GetComponent<Image>().raycastTarget = false;
                            }
                    }
                }
            }
            DisableResizing();
        }
        void DisableResizing()
        {
            for (int i = 0; i < r.Length; i++)
            {
                r[i].EnableResizing = false;
                r[i].enabled = false;             
            }
        }
        void EnableResizing()
        {
            for (int i = 0; i < r.Length; i++)
            {
                r[i].enabled = true;
                r[i].EnableResizing = true;
                for (int x = 0; x < r[i].transform.childCount; x++)
                {
                    // Enable the Image component
                    if (r[i].transform.GetComponent<Image>() != null)
                    {
                        r[i].transform.GetComponent<Image>().enabled = true;
                        // Get the Image component
                        Image image = r[i].transform.GetComponent<Image>();
                        // Set the alpha to 0
                        if (image != null)
                        {
                            Color tempColor = image.color;
                            tempColor.a = 1; // Set alpha to 0
                            image.color = tempColor;
                        }
                            if (r[i].transform.GetChild(x).gameObject.GetComponent<Image>() != null)
                            {
                                r[i].transform.GetChild(x).gameObject.GetComponent<Image>().raycastTarget = false;
                            }
                    }
                }
            }
            DisableDragging();
        }
        [ContextMenu("Reset HUD PlayerPrefs Keys")]
        public void ResetAllHudSettings()
        {
            PausePanel.SetActive(false);
            HudSettingsPanel.SetActive(false);
            Time.timeScale = 1f;
            for (int i = 0; i < d.Length; i++)
            {
                d[i].EnableDragging = false;
                PlayerPrefs.DeleteKey(d[i].UniqueNameToSaveDragging + "SavePositionX");
                PlayerPrefs.DeleteKey(d[i].UniqueNameToSaveDragging + "SavePositionY");
                d[i].ResetDrag();
                for (int x = 0; x < d[i].transform.childCount; x++)
                {
                    if (d[i].transform.GetComponent<Image>() != null)
                    {
                        // Enable the Image component
                        d[i].transform.GetComponent<Image>().enabled = false;
                        // Get the Image component
                        Image image = d[i].transform.GetComponent<Image>();
                        // Set the alpha to 0
                        if (image != null)
                        {
                            Color tempColor = image.color;
                            tempColor.a = 1; // Set alpha to 0
                            image.color = tempColor;
                        }
                        if (d[i].transform.GetChild(x).gameObject.GetComponent<Image>() != null)
                        {
                            d[i].transform.GetChild(x).gameObject.GetComponent<Image>().raycastTarget = true;
                        }
                    }
                }
            }
            for (int i = 0; i < r.Length; i++)
            {
                r[i].EnableResizing = false;
                PlayerPrefs.DeleteKey(r[i].UniqueNameToSaveResizing + "SaveSizeX");
                PlayerPrefs.DeleteKey(r[i].UniqueNameToSaveResizing + "SaveSizeY");
                r[i].ResetSize();
                for (int x = 0; x < r[i].transform.childCount; x++)
                {
                    if (r[i].transform.GetComponent<Image>() != null)
                    {
                        // Enable the Image component
                        r[i].transform.GetComponent<Image>().enabled = false;
                        // Get the Image component
                        Image image = r[i].transform.GetComponent<Image>();
                        // Set the alpha to 0
                        if (image != null)
                        {
                            Color tempColor = image.color;
                            tempColor.a = 1; // Set alpha to 0
                            image.color = tempColor;
                        }
                        if (r[i].transform.GetChild(x).gameObject.GetComponent<Image>() != null)
                        {
                            r[i].transform.GetChild(x).gameObject.GetComponent<Image>().raycastTarget = true;
                        }
                    }
                }
            }
        }
    }
}
