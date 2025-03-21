using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

namespace MobileActionKit
{
    // This Script is Responsible For Loading Scenes Choosing Loading Variant

    [System.Serializable]
    public class ButtonFunctionDataForSceneLoading
    {
        [Tooltip("Name of the button for reference.")]
        public string ButtonName;

        [Tooltip("Reference to the UI Button that triggers scene loading.")]
        public Button button;

        [Tooltip("Scene number in the build settings to load.")]
        public int SceneNumber;
    }

    public class LoadingManager : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script handles scene loading asynchronously, provides a loading screen with a progress bar, and deactivates specified objects during loading.";

        public static LoadingManager instance;

        [Tooltip("Assign the loading screen GameObject. It will be activated during scene loading.")]
        public GameObject LoadingScreen;

        [Tooltip("Add the UI Slider that will display the loading progress.")]
        public Slider LoadingProgressSlider;

        [Tooltip("Objects that should be deactivated when the loading starts.")]
        public GameObject[] GameObjectToDeactivate;


        [Tooltip("List of button functions with their assigned scene names.")]
        public List<ButtonFunctionDataForSceneLoading> buttonFunctions = new List<ButtonFunctionDataForSceneLoading>();


        private void Awake()
        {
            MakeSingleton();
        }
        public void MakeSingleton()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        private void Start()
        {
            SaveChanges();
        }
        // Method to add new button function data
        public void AddButtonFunction(Button button)
        {
            ButtonFunctionDataForSceneLoading newData = new ButtonFunctionDataForSceneLoading();
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

        }
        // Method to activate and deactivate objects for a specific button
        private void ActivateAndDeactivate(ButtonFunctionDataForSceneLoading data)
        {
            SceneManager.LoadScene(data.SceneNumber);
        }
      
        public IEnumerator LoadAsynchronously(int SceneIndex)
        {
            AsyncOperation opertion = SceneManager.LoadSceneAsync(SceneIndex);

            for(int x = 0;x < GameObjectToDeactivate.Length; x++)
            {
                GameObjectToDeactivate[x].SetActive(false);
            }
            while (!opertion.isDone)
            {
                LoadingScreen.SetActive(true);
                float progress = Mathf.Clamp01(opertion.progress / .9f);
                LoadingProgressSlider.value = progress;
                yield return null;
            }
        }
    }
}