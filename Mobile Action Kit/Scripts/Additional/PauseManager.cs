using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

namespace MobileActionKit
{
    public class PauseManager : MonoBehaviour
    {
        public static PauseManager instance;

        public GameObject PausePanel;
        public GameObject[] UIToDeactivateOnEnable;

        //public GameObject MiniMapUI;
        //public Toggle MiniMapToggle;

        public Button MainMenuButton;
        public Button ResumeButton;

        public int LoadSceneOnMainMenuButtonClick = 0;

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            //if(MiniMapToggle != null)
            //{
            //    CheckFeature();
            //}
            MainMenuButton.onClick.AddListener(() => MainMenu(LoadSceneOnMainMenuButtonClick));
            ResumeButton.onClick.AddListener(ResumeGame);
        }
        public void PauseGame()
        {
            for (int x = 0; x < UIToDeactivateOnEnable.Length; x++)
            {
                UIToDeactivateOnEnable[x].SetActive(false);
            }
            PausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
        public void ResumeGame()
        {
            for (int x = 0; x < UIToDeactivateOnEnable.Length; x++)
            {
                UIToDeactivateOnEnable[x].SetActive(false);
            }
            PausePanel.SetActive(false);
            Time.timeScale = 1f;
        }
        public void MainMenu(int BuildIndex)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(BuildIndex);
        }
        //public void EnableMinimap(bool Enable)
        //{
        //    MiniMapToggle.isOn = Enable;
        //    if (MiniMapToggle.isOn == true)
        //    {
        //        PlayerPrefs.SetInt("MiniMapFeature", 1);
        //    }
        //    else
        //    {
        //        PlayerPrefs.SetInt("MiniMapFeature", 0);
        //    }
        //    CheckFeature();
        //}
        //public void CheckFeature()
        //{
        //    if (PlayerPrefs.GetInt("MiniMapFeature") == 1)
        //    {
        //        MiniMapToggle.isOn = true;
        //        MiniMapUI.SetActive(true);
        //    }
        //    else
        //    {
        //        MiniMapToggle.isOn = false;
        //        MiniMapUI.SetActive(false);
        //    }
        //}
    }
}