using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MobileActionKit;
using UnityEngine.SceneManagement;


namespace MobileActionKit
{
    [RequireComponent(typeof(CrossHairColorChanger))]
    public class FriendlyFire : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script regulates friendly fire and its consequences to player.";
   
        public static FriendlyFire instance;

        public enum FriendlyFireOptionsClass
        {
            FriendliesDoNotReact,
            GameOver,
            MakePlayerToBeTraitor,
            DisableShootingOnFriendlies
        }

        [Tooltip("Select one of the available options for Friendly fire. " +
            "1'GameOver' option will end the game as soon as first shot at friendly AI is made by the player. " +
            "2'PlayerBecomesTraitor' option will make friendly Ai agents to shoot back at player if he shot them specified number of times(). " +
            "3'DisableShootingFriendlies' will stop weapon functionality when crosshair is pointing at friendly Ai agents. " +
            "4'FriendliesDoNotReact'(NoPunishment) option allows friendly fire to be made with no consequenses to the player.")]
        public FriendlyFireOptionsClass FriendlyFireOptions;

        [Tooltip("Drag and drop Target script from above in the inspector into this field.")]
        public Targets TargetsScript;

        private string TraitorTag = "Traitor";

        [Tooltip("Drag and drop the default game over panel from the hierarchy into this field.")]
        public GameObject DefaultGameOverPanel;

        [Tooltip("Drag and drop the friendly fire game over panel from the hierarchy into this field.")]
        public GameObject FriendlyFireGameOverPanel;

        [Tooltip("Number of times player can shoot friendlies without being punished for that.")]
        public int WarningsToPlayer = 2;

        [Tooltip("If checked then friendlies will not wait for number of warnings and will make player traitor as soon as nearby friendly died.")]
        public bool MakePlayerTraitorImmediatelyIfFriendlyDie = true;

        [Tooltip("Specify the delay (in seconds) before activating the game over panel.")]
        public float FriendlyFireGameOverUIActivationDelay = 2f;

        [Tooltip("Drag and drop respective button from UI Canvas.")]
        public Button RestartGameUIButton;
       
        [System.Serializable]
        public class FriendlyDistance
        {
            public AudioClip[] AudioClipsToPlay;
            public float MinFriendlyDistance = 5f;
            public float MaxFriendlyDistance = 10f;
            public float MinVolume = 0.5f;
            public float MaxVolume = 1f;
        }

        [Tooltip("Audio source component used to play sounds based on friendly distance.")]
        public AudioSource AudioSourceComponent;

        [Tooltip("List of audio clips to play based on the distance of friendlies, with customizable ranges and volumes.")]
        public List<FriendlyDistance> AudioClipsToPlaybackBasedOnFriendlyDistance = new List<FriendlyDistance>();


        int CurrentWarning;

        bool IsPanelEnabled = false;

        [HideInInspector]
        public GameObject Enemy;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        private void Start()
        {
            if (RestartGameUIButton != null)
            {
                RestartGameUIButton.onClick.AddListener(RestartGame);
            }
        }
        public void RestartGame()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
            Time.timeScale = 1f;
        }
        public void TraitorPlayer(bool IsFriendlyDead)
        {
            if (FriendlyFireOptions == FriendlyFireOptionsClass.MakePlayerToBeTraitor)
            {
                //if()
                if (MakePlayerTraitorImmediatelyIfFriendlyDie == true)
                {
                    if (IsFriendlyDead == true)
                    {
                        TargetsScript.MyTeamID = TraitorTag;
                        if (HumanoidAiManager.instance != null)
                        {
                            HumanoidAiManager.instance.CheckingForNewEnemies();
                        }
                        if (Enemy != null)
                        {
                            if (Enemy.GetComponent<FindEnemies>() != null)
                            {
                                Enemy.GetComponent<FindEnemies>().EnableFieldOfView = false;
                            }
                            if (Enemy.GetComponent<CoreAiBehaviour>() != null)
                            {
                                Enemy.GetComponent<CoreAiBehaviour>().IsFieldOfViewForDetectingEnemyEnabled = false;
                                Enemy.GetComponent<CoreAiBehaviour>().IsEnemyLocked = true;
                            }
                        }
                    }
                    else
                    {
                        if (CurrentWarning >= WarningsToPlayer)
                        {
                            TargetsScript.MyTeamID = TraitorTag;
                            if (HumanoidAiManager.instance != null)
                            {
                                HumanoidAiManager.instance.CheckingForNewEnemies();
                            }
                            if (Enemy != null)
                            {
                                if (Enemy.GetComponent<FindEnemies>() != null)
                                {
                                    Enemy.GetComponent<FindEnemies>().EnableFieldOfView = false;
                                }
                                if (Enemy.GetComponent<CoreAiBehaviour>() != null)
                                {
                                    Enemy.GetComponent<CoreAiBehaviour>().IsFieldOfViewForDetectingEnemyEnabled = false;
                                    Enemy.GetComponent<CoreAiBehaviour>().IsEnemyLocked = true;
                                }
                            }

                        }
                    }
                }
                else
                {
                    if (CurrentWarning >= WarningsToPlayer)
                    {
                        TargetsScript.MyTeamID = TraitorTag;
                        if (HumanoidAiManager.instance != null)
                        {
                            HumanoidAiManager.instance.CheckingForNewEnemies();
                        }
                        if (Enemy != null)
                        {
                            if (Enemy.GetComponent<FindEnemies>() != null)
                            {
                                Enemy.GetComponent<FindEnemies>().EnableFieldOfView = false;
                            }
                            if (Enemy.GetComponent<CoreAiBehaviour>() != null)
                            {
                                Enemy.GetComponent<CoreAiBehaviour>().IsFieldOfViewForDetectingEnemyEnabled = false;
                                Enemy.GetComponent<CoreAiBehaviour>().IsEnemyLocked = true;
                            }
                        }
                    }
                }

                if(AudioSourceComponent != null)
                {
                    if(AudioClipsToPlaybackBasedOnFriendlyDistance.Count >= 1)
                    {
                        if(Enemy != null)
                        {
                            Vector3 Distance = Enemy.transform.position - transform.position;
                            for (int x = 0; x < AudioClipsToPlaybackBasedOnFriendlyDistance.Count; x++)
                            {
                                if (Distance.magnitude <= AudioClipsToPlaybackBasedOnFriendlyDistance[x].MaxFriendlyDistance && Distance.magnitude > AudioClipsToPlaybackBasedOnFriendlyDistance[x].MinFriendlyDistance)
                                {
                                    float Randomise = Random.Range(AudioClipsToPlaybackBasedOnFriendlyDistance[x].MinVolume, AudioClipsToPlaybackBasedOnFriendlyDistance[x].MaxVolume);
                                    AudioSourceComponent.volume = Randomise;
                                    int RandomAudioClip = Random.Range(0,AudioClipsToPlaybackBasedOnFriendlyDistance[x].AudioClipsToPlay.Length);
                                    AudioSourceComponent.clip = AudioClipsToPlaybackBasedOnFriendlyDistance[x].AudioClipsToPlay[RandomAudioClip];
                                    AudioSourceComponent.Play();
                                }
                            }

                        }
                     
                    }
                }
               

                ++CurrentWarning;
            }
            else if (FriendlyFireOptions == FriendlyFireOptionsClass.GameOver)
            {
                GameOverPanels();
            }

        }
        public void GameOverPanels()
        {
            if (IsPanelEnabled == false)
            {
                StartCoroutine(Coro());
                IsPanelEnabled = true;
            }

        }
        IEnumerator Coro()
        {
            yield return new WaitForSeconds(FriendlyFireGameOverUIActivationDelay);
            Time.timeScale = 0f;
            DefaultGameOverPanel.SetActive(false);
            FriendlyFireGameOverPanel.SetActive(true);
        }
    }
}