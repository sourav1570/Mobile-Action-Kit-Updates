using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MobileActionKit;


namespace MobileActionKit
{
    [System.Serializable]
    public class ButtonFunctionData
    {
        [Tooltip("Enter the button name.")]
        public string ButtonName;
        [Tooltip("Drag and drop the UI button from the hierarchy into this field.")]
        public Button button;

        [Tooltip("Drag and drop the gameobjects to be activated when the above 'UI button' is clicked.")]
        public List<GameObject> objectsToActivate = new List<GameObject>();
        [Tooltip("Drag and drop the gameobjects to be deactivated when the above 'UI button' is clicked.")]
        public List<GameObject> objectsToDeactivate = new List<GameObject>();
    }

    public class ButtonFunctionManager : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script activates and deactivates assigned gameObjects when the desired UI button is clicked." +
            " Each assigned UI button will call the function and will make sure to activate and deactivate assigned gameObjects in there properties.";

        [Tooltip("If enabled than the UI button will play click sounds when calling the function.")]
        public bool PlayClickSounds = true;

        [Tooltip("Drag and drop 'FadeAnimatorComponent' to enable fading when activating and deactivating new gameObjects.")]
        public Animator FadeAnimatorComponent;

        [Tooltip("Enter the 'AnimationName' to playback when calling the function using UI button.")]
        public string AnimationName = "Fade_In";

        [Tooltip("Add one or more properties and drag and drop the UI buttons and add the 'objectsToActivate' and 'objectsToDeactivate' to be called when that UI button is clicked.")]
        public List<ButtonFunctionData> buttonFunctions = new List<ButtonFunctionData>();

        private void Start()
        {
            SaveChanges();
        }
        // Method to add new button function data
        public void AddButtonFunction(Button button)
        {
            ButtonFunctionData newData = new ButtonFunctionData();
            newData.button = button;
            buttonFunctions.Add(newData);
        }

        // Method to save changes
        public void SaveChanges()
        {
            foreach (var data in buttonFunctions)
            {
                data.button.onClick.RemoveAllListeners(); // Remove previous listeners

                data.button.onClick.AddListener(() =>
                {
                    ActivateAndDeactivate(data);
                });
            }

            //        Debug.Log("Changes saved.");
        }

        // Method to activate and deactivate objects for a specific button
        private void ActivateAndDeactivate(ButtonFunctionData data)
        {
            foreach (var obj in data.objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }

            foreach (var obj in data.objectsToDeactivate)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }

            if (PlayClickSounds == true)
            {
                SoundsController.instance.ClickSounds.Play();
            }

            if (FadeAnimatorComponent != null)
            {
                FadeAnimatorComponent.Play(AnimationName, -1, 0f);
            }
        }
    }
}