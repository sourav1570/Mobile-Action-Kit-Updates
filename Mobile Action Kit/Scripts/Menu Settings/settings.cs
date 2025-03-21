using UnityEngine.UI;
using UnityEngine;

// This Script is Responsible For Different Settings in Game
namespace MobileActionKit
{

    public class settings : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script manages game settings including audio volume, screen resolution, quality settings, and toggling menus like Weapons, Items, and Missions. It also ensures user preferences are saved and applied.";

        [Tooltip("Main settings panel GameObject.")]
        public GameObject SettingsPanel;

        [Tooltip("Slider for adjusting the game music volume.")]
        public Slider MusicSlider;

        [Tooltip("AudioSource responsible for game background music.")]
        public AudioSource GameLoopSound;

        [Tooltip("Weapons menu UI panel.")]
        public GameObject WeaponsMenu;

        [Tooltip("Items menu UI panel.")]
        public GameObject ItemsMenu;

        private bool Checkweaponsmenuactivation = false;
        private bool CheckItemsmenuactivation = false;

        [Tooltip("Enable dropdown for selecting game quality settings.")]
        public bool UsingDropDownBoxForShowingQuality = false;

        [Tooltip("Dropdown UI element for quality selection.")]
        public Dropdown DropDownQualityList;

        [Tooltip("Toggle for enabling or disabling fullscreen mode.")]
        public Toggle FullScreenToggle;

        [Tooltip("Animator for handling UI fading effects.")]
        public Animator FadingAnimator;

        [Tooltip("AudioSource for UI sound effects.")]
        public AudioSource UISoundeffects;

        private bool CheckMissionMenuActivation = false;

        [Tooltip("Mission menu UI panel.")]
        public GameObject MissionMenu;

        private void Start()
        {
            if (MusicSlider.value == 0)
            {
                GameLoopSound.volume = 0;
            }

            if (UsingDropDownBoxForShowingQuality == true) // Check if using Drop Down Box For Showing Quality Level
            {
                if (PlayerPrefs.GetInt("Quality") == 0)
                {
                    DropDownQualityList.value = 0;
                }
                else if (PlayerPrefs.GetInt("Quality") == 1)
                {
                    DropDownQualityList.value = 1;
                }
                else if (PlayerPrefs.GetInt("Quality") == 2)
                {
                    DropDownQualityList.value = 2;
                }
 
              //  renderscale.gameObject.SetActive(false);
            }
            else
            {
                if (DropDownQualityList.transform.parent != null)
                {
                    DropDownQualityList.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    DropDownQualityList.gameObject.SetActive(false);
                }
            }
            CheckRenderScale();
        }
        private void Update()
        {
            if (FullScreenToggle.isOn == true) // Checks For Full Screen
            {
                PlayerPrefs.SetInt("SelectFullScreen", 0);
            }
            else
            {
                PlayerPrefs.SetInt("SelectFullScreen", 1);
            }
        }
        private void FixedUpdate()
        {
            if (PlayerPrefs.GetInt("SelectFullScreen") == 0)
            {
                FullScreenToggle.isOn = true;
            }
            else
            {
                FullScreenToggle.isOn = false;
            }
        }
        public void setFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
        }
        public void Getvolume(float volume)
        {
            MobileActionKit.SoundsController.instance.setmusicvolume(volume);
            PlayerPrefs.SetFloat("musicvolume", volume);
        }
        public void GetRenderScale(float scale)
        {
            PlayerPrefs.SetFloat("RenderScale", scale);
            CheckRenderScale();
        }
        void CheckRenderScale()
        {
            //if (urpAsset != null)
            //{
            //    urpAsset.renderScale = Mathf.Clamp(scale, 0.5f, 2f); // Prevent extreme values
            //}
        }
        public void settingspanelbtn()
        {
            MusicSlider.value = MobileActionKit.SoundsController.instance.Getmusicvolume();
            SettingsPanel.SetActive(true);
            if (MissionMenu.activeInHierarchy == true)
            {
                MissionMenu.SetActive(false);
                CheckMissionMenuActivation = true;

            }
            if (WeaponsMenu.activeInHierarchy == true)
            {
                WeaponsMenu.SetActive(false);
                Checkweaponsmenuactivation = true;
            }
            if (ItemsMenu.activeInHierarchy == true)
            {
                ItemsMenu.SetActive(false);
                CheckItemsmenuactivation = true;
            }
        }
        public void turnoffsettings()
        {
            SettingsPanel.SetActive(false);
            if (CheckMissionMenuActivation == true)
            {
                MissionMenu.SetActive(true);
                CheckMissionMenuActivation = false;
            }
            if (Checkweaponsmenuactivation == true)
            {
                WeaponsMenu.SetActive(true);
                Checkweaponsmenuactivation = false;
            }
            if (CheckItemsmenuactivation == true)
            {
                ItemsMenu.SetActive(true);
                CheckItemsmenuactivation = false;
            }
        }
        void SelectQuality()
        {
            if (PlayerPrefs.GetInt("Quality") == 0)
            {
                DropDownQualityList.value = 0;
            }
            else if (PlayerPrefs.GetInt("Quality") == 1)
            {
                DropDownQualityList.value = 1;
            }
            else if (PlayerPrefs.GetInt("Quality") == 2)
            {
                DropDownQualityList.value = 2;
            }

        }
    }
}
