using UnityEngine.UI;
using UnityEngine;

// This Script is Responsible for All The Sounds in Main Menu and Other Menus
namespace MobileActionKit
{
    public class SoundsController : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script manages all sound effects and background music for the main menu and UI menus. It allows volume adjustments and saves user preferences.";

        public static SoundsController instance;

        [Tooltip("Audio source for background music loop in the main menu.")]
        public AudioSource BackgroundLoopSound;

        [Tooltip("Audio source for equip sound effect.")]
        public AudioSource EquipAudio;

        [Tooltip("Audio source for buy/purchase sound effect.")]
        public AudioSource BuyAudio;

        [Tooltip("Audio source for error sound effect.")]
        public AudioSource ErrorAudio;

        [Tooltip("Audio source for button click sound effects.")]
        public AudioSource ClickSounds;

        private float musicvolume = 1;

        [Tooltip("Slider in the settings menu for adjusting music volume.")]
        public Slider SettingsMusicSlider;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        void Start()
        {
            if(SettingsMusicSlider != null)
            {
                SettingsMusicSlider.value = PlayerPrefs.GetFloat("musicvolume", 1); // Getting The Saved Music Volume
                BackgroundLoopSound.volume = SettingsMusicSlider.value; //  Music Volume shows in Slider
            }
        
        }

        public void setmusicvolume(float volume) // Setting up the volume using Slider
        {
            playorturnoffmusic(volume);
        }
        public void playorturnoffmusic(float volume) // Checking if the Sound is Still Playing Or not
        {
            musicvolume = volume;
            BackgroundLoopSound.volume = musicvolume;

            PlayerPrefs.SetFloat("musicvolume", volume);

            if (BackgroundLoopSound.volume > 0)
            {
                if (!BackgroundLoopSound.isPlaying)
                {
                    BackgroundLoopSound.Play();
                }
            }
            else if (BackgroundLoopSound.volume == 0)
            {
                if (BackgroundLoopSound.isPlaying)
                {
                    BackgroundLoopSound.Pause();
                }
            }
        }
        public float Getmusicvolume()
        {
            return this.musicvolume;
        }
    }
}
