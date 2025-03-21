using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// This Script is Responsible For All The Levels Unlocking and Locking

namespace MobileActionKit
{
    public class LevelManager : MonoBehaviour
    {
        [TextArea]        
        public string ScriptInfo = "This script manages the unlocking and locking of levels. It allows customization of locked/unlocked level appearance and highlights the current level.";

        public bool UseLoadingBarToLoadLevel = true;
        public LoadingManager LoadingManagerScript;

        public bool UseStartButtonToLoadLevel = true;
        public Button StartButton;

        [System.Serializable]
        public class LevelClass
        {
            [Tooltip("Assign all the level buttons here.")]
            public Button LevelButton;
            public int LevelToLoad;
            public Text ObjectiveText;
            public string WriteObjectiveMessage;
        }


        public List<LevelClass> Levels = new List<LevelClass>();     

        [Header("Locked Levels Functionality")]
        [Tooltip("Enable or disable changing color for locked levels.")]
        public bool EnableLockedLevelColor = false;

        [Tooltip("Color to apply for locked levels.")]
        public Color ColorForLockedLevels;

        [Tooltip("Enable or disable using a different sprite for locked levels.")]
        public bool UseDifferentSpriteForLockedLevel = false;

        [Tooltip("Sprite to apply for locked levels if enabled.")]
        public Sprite SpriteForLockedLevel;

        [Header("Unlocked Levels Functionality")]
        [Tooltip("Color to apply for unlocked levels. If you don’t want to change color, increase Alpha and make it whiter.")]
        public Color ColorForUnlockedLevel;

        [Tooltip("Enable or disable using a different sprite for unlocked levels.")]
        public bool UseDifferentSpriteForUnlockedLevel;

        [Tooltip("Sprite to apply for unlocked levels if enabled.")]
        public Sprite SpriteForUnlockedLevel;

        [Tooltip("Enable highlighting for the current level reached.")]
        public bool EnableLevelHighlighter = true;

        [Tooltip("Image used to highlight the currently reached level.")]
        public Image HighlightingLevelSprite;

        [HideInInspector]
        [Tooltip("Stores the highest level reached, retrieved from PlayerPrefs.")]
        public int lvlreached;

        [HideInInspector]
        public int LevelToLoadInGame;

        private void Awake()
        {
            lvlreached = PlayerPrefs.GetInt("LevelReached", 0);
            LevelsFunctionality();
            for (int i = 0; i < Levels.Count; i++)
            {
                int index = i; // Capture the value of i in a local variable
                Levels[i].LevelButton.onClick.AddListener(() => ShowObjectiveAndLoadLevel(index));
            }

            if(UseStartButtonToLoadLevel == true)
            {
                StartButton.onClick.AddListener(LoadLevelUsingStartButton);
            }
        }
        private void Start()
        {
            if (EnableLevelHighlighter == true)
            {
                Vector3 newposition = HighlightingLevelSprite.gameObject.transform.localPosition;
                newposition.x = Levels[lvlreached].LevelButton.gameObject.transform.localPosition.x;
                newposition.y = Levels[lvlreached].LevelButton.gameObject.transform.localPosition.y;
                HighlightingLevelSprite.gameObject.transform.localPosition = newposition;
                HighlightingLevelSprite.transform.parent = Levels[lvlreached].LevelButton.transform.parent;
                Levels[lvlreached].LevelButton.gameObject.transform.parent = HighlightingLevelSprite.transform;
            }
        }
        public void ShowObjectiveAndLoadLevel(int Index)
        {
            LevelToLoadInGame = Levels[Index].LevelToLoad;
            Levels[Index].ObjectiveText.text = Levels[Index].WriteObjectiveMessage;
            if (UseStartButtonToLoadLevel == false)
            {
                if (UseLoadingBarToLoadLevel == false)
                {
                    SceneManager.LoadScene(Levels[LevelToLoadInGame].LevelToLoad);
                }
                else
                {
                    StartCoroutine(LoadingManagerScript.LoadAsynchronously(LevelToLoadInGame));

                }
            }
        }
        public void LevelsFunctionality()
        {
            for (int i = 0; i < Levels.Count; i++)
            {
                if (i > lvlreached)
                {
                    Levels[i].LevelButton.interactable = false;
                    if (EnableLockedLevelColor == true && UseDifferentSpriteForLockedLevel == true)
                    {
                        Levels[i].LevelButton.gameObject.GetComponent<Image>().color = ColorForLockedLevels;
                        Levels[i].LevelButton.gameObject.GetComponent<Image>().sprite = SpriteForLockedLevel;
                    }
                    else if (EnableLockedLevelColor == true)
                    {
                        Levels[i].LevelButton.gameObject.GetComponent<Image>().color = ColorForLockedLevels;
                    }
                    else if (UseDifferentSpriteForLockedLevel == true)
                    {
                        Levels[i].LevelButton.gameObject.GetComponent<Image>().sprite = SpriteForLockedLevel;
                    }
                }
                else
                {
                    if (UseDifferentSpriteForUnlockedLevel == true)
                    {
                        Levels[i].LevelButton.gameObject.GetComponent<Image>().sprite = SpriteForUnlockedLevel;
                    }
                    Levels[i].LevelButton.gameObject.GetComponent<Image>().color = ColorForUnlockedLevel;

                    if(Levels[i].ObjectiveText != null)
                    {
                        Levels[i].ObjectiveText.text = Levels[i].WriteObjectiveMessage;
                    }

                    LevelToLoadInGame = Levels[i].LevelToLoad;
                }
            }
        }
        public void LoadLevelUsingStartButton()
        {
            if(UseStartButtonToLoadLevel == true)
            {
                if (UseLoadingBarToLoadLevel == false)
                {
                    SceneManager.LoadScene(Levels[LevelToLoadInGame].LevelToLoad);
                }
                else
                {
                    StartCoroutine(LoadingManagerScript.LoadAsynchronously(LevelToLoadInGame));

                }
            }
           
        }
        public void LoadCustomLevel(int LevelToLoad)
        {
            if (UseStartButtonToLoadLevel == true)
            {
                if (UseLoadingBarToLoadLevel == false)
                {
                    SceneManager.LoadScene(Levels[LevelToLoad].LevelToLoad);
                }
                else
                {
                    StartCoroutine(LoadingManagerScript.LoadAsynchronously(LevelToLoad));

                }
            }
            else
            {
                if (UseLoadingBarToLoadLevel == false)
                {
                    SceneManager.LoadScene(Levels[LevelToLoad].LevelToLoad);
                }
                else
                {
                    StartCoroutine(LoadingManagerScript.LoadAsynchronously(LevelToLoad));

                }
            }
        }
    }
}