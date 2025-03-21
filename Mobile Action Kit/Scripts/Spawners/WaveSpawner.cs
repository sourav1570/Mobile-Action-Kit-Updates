using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AI;

namespace MobileActionKit
{
    public class WaveSpawner : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "Wave spawner is the most sophisticated spawner and it allows for a wide variety of ways of respawning Ai" +
            " agents based on a numerous settings that can be configured using flexible set of options provided in this script." +
            "You can set certain number of waves and configure each wave to have a unique settings.You can then increase total amount of waves that exceeds the number of configured waves. " +
            "In this case Wave spawner will create duplicates of the the last configured wave until total number of Waves is reached." +
            "In this case Wave spawner will go though all initially configured waves and will spawn last configured wave again and again until overall waves number is reached." +
            "For example first you set number of waves to be 3. Then you configure them to your liking.After that you change number of waves to be 100. " +
            "Wave spawner will spawn wave 1, then wave 2, then wave 3. All remaining 97 waves will be duplicates of wave 3.";

        public enum WaveType
        {
            StartOnTriggerEnter,
            EnterEachWaveTrigger,
            BeginOnPlay
        }

        [Tooltip("Choose one of the three types of wave  spawner activation." +
            "1'StartWaveBasedOnSingleTrigger'(StartOnTriggerEnter) Spawner begins functioning once the player enters the collider trigger attached to this gameObject with no consecutive triggering required to spawn following waves." +
            "2'StartWaveBasedOnMultipleTrigger'(EnterEachWaveTrigger) initiates waves based on specific triggers assigned to each wave.For instance, wave 1 starts when the player enters" +
            " the trigger collider for wave 1, wave number 2 when player enters trigger for wave 2 and the same applies to all subsequent waves." +
            "3'ActivateAtGameStart'(BeginOnPlay) starts functioning at the beginning of the gameplay without the need for trigger colliders.")]
        public WaveType ActivationType;

        public enum DrawGizmosInScene
        {
            DrawWireSphere,
            DrawWireCube,
            DoNotDebugGizmos
        }

        [Tooltip("Draw the spawning areas (and their activation triggers) in unity scene view to visualise where the Ai agents will spawn in the game.")]
        public DrawGizmosInScene DrawSpawnerGizmos;

        [Tooltip("Set the the colours of the spawning area gizmo in scene view.")]
        public Color GizmosColor = Color.red;

        [Tooltip("[Draft]Drag and drop the player gameObject from the hierarchy into this field. The player gameObject will than enter this trigger area to start the spawning behaviour.")]
        public GameObject Player;

        [Tooltip("Automatically assigns the Collider component to this field when the selected Wave Type option is'StartWaveBasedOnSingleTrigger'.")]
        public Collider ColliderComponent;

        [Tooltip("[If enabled then next wave starts only when all the agents from previous wave are dead.")]
        public bool NextWaveAfterPreviousWaveDestroyed = true;

        [Tooltip("Minimum time between counting casualties.")]
        public float MinCasualtiesCheckTime = 1f;
        [Tooltip("Maximum time between counting casualties.")]
        public float MaxCasualtiesCheckTime = 2f;

        [HideInInspector]
        public int maxSpawnAttemptsPerObstacle; 

        [Tooltip("If checked wave counter text will be displayed in the form of '2D UI' each time new wave starts.")]
        public bool EnableWavesCounterUI = true;
        [Tooltip("Write the word to display before showing the wave number for example : Round,Wave, etc.")]
        public string WaveName = "Wave";
        [Tooltip("Drag and drop the Text UI from the 2D canvas hierarchy into this field.")]
        public Text WaveText;
        [Tooltip("Specify for how long wave text will stay activated.")]
        public float TimeToDeactiveWaveText = 2f;

        [Tooltip("If checked than the countdown timer will be displayed before starting the next wave.")]
        public bool DisplayCountDownTimer = true;
        [Tooltip("Drag and drop the Text UI from the 2D canvas hierarchy into this field to display the count down.")]
        public Text CountDownTimerText;


        //[Tooltip("[Draft]Radius to check For agent before spawning so they do not spawn over each other")]
        //public float AgentsSeparationRadius = 3f;

        [Tooltip("Drag and drop Ai Updater game object from the hierarchy with 'SpawnedAgentsTargetScriptUpdater'(AiUpdater) script attached to it into this field.")]
        public AiUpdater AiUpdaterScript;
            
        [System.Serializable]
        public class NormalWave
        {
            [Tooltip("If checked it will spawn agents that are listed inside 'AiToSpawnList' script that can hold various types of Ai agents. Spawner then randomly spawns Ai agents from that list.")]
            public bool UseAgentsListScript = true;

            [Tooltip("Drag and drop 'AiToSpawnList' gameobject with 'AiAgentsListScript' attached to it in this field from the hierarchy of the 'WaveSpawner' gameobject.")]
            public AiToSpawnList AiAgentsListScript;
            [Tooltip("This dropdown list is an alternative way to 'AiToSpawnList' way of Ai agents assignment. It can be set to desired number of fields that would hold Ai agent prefabs to be spawned." +
                "Drag and drop Ai agents from the project into these fields to be spawned within this wave.")]
            public GameObject[] Agents;

            [Tooltip("If checked then will spawn Ai agents at specified spawn points that should be placed in 'SpawnPoint' fields.")]
            public bool UsePreciseSpawnPoints;
            [Tooltip("Set the number of pre-placed Spawn points used for this wave.Drag and drop those spawnPoints to be used for this wave from hierarchy tab into this field.")]
            public Transform[] PreciseSpawnPoints;

            [Tooltip("If checked than Ai agents will spawn within the volume of the trigger collider attached to child game object of this 'WaveSpawner' named 'SpawnVolume'which you`ll have to create, " +
                "add trigger collider to it and set up its dimensions for this purpose to make sure that Agents will spawn only within a certain rooms or any other kind of enclosed spaces.")]
            public bool SpawnWithinVolume = false;
            [Tooltip("Drag and drop 'SpawnVolume' child game object with trigger collider attached to it into this field.")]
            public Collider SpawnVolume;

            [Tooltip("Set the SpawnPoint radius within which Ai agents can be spawned.")]
            public float SpawnPointRadius = 15f;
            [Tooltip("Drag and drop the empty gameObject child of this gameObject and place the child gameObject where you want the agents to spawn.")]
            public Transform SpawnPoint;
            
            [Tooltip("Set the number of Ai agents for this wave to spawn.")]
            public int NumberOfAgentsToSpawn;
            [Tooltip("Set the number of seconds for the delay of this wave before it starts.")]
            public int WaveDelay;

            [HideInInspector]
            public bool IsWaveFinished = false;
        }
        [System.Serializable]
        public class TriggerWave
        {
            public WaveActivator WaveScript;
            [Tooltip("If checked it will spawn agents that are listed inside 'AiToSpawnList' script that can hold various types of Ai agents. Spawner then randomly spawns Ai agents from that list.")]
            public bool UseAgentsListScript = true;

            [Tooltip("Drag and drop 'AiToSpawnList' gameobject with 'AiAgentsListScript' attached to it in this field from the hierarchy of the 'WaveSpawner' gameobject.")]
            public AiToSpawnList AiAgentsListScript;
            [Tooltip("This dropdown list is an alternative way to 'AiToSpawnList' way of Ai agents assignment. It can be set to desired number of fields that would hold Ai agent prefabs to be spawned." +
                 "Drag and drop Ai agents from the project into these fields to be spawned within this wave.")]
            public GameObject[] Agents;   

            [Tooltip("If checked then will spawn Ai agents at specified spawn points that should be placed in 'SpawnPoint' fields.")]
            public bool UsePreciseSpawnPoints;
            [Tooltip("Set the number of pre-placed Spawn points used for this wave.Drag and drop those spawnPoints to be used for this wave from hierarchy tab into this field.")]
            public Transform[] PreciseSpawnPoints;

            [Tooltip("If checked than Ai agents will spawn within the volume of the trigger collider attached to child game object of this 'WaveSpawner' named 'SpawnVolume'which you`ll have to create, " +
                "add trigger collider to it and set up its dimensions for this purpose to make sure that Agents will spawn only within a certain rooms or any other kind of enclosed spaces.")]
            public bool SpawnWithinVolume = false;
            [Tooltip("Drag and drop 'SpawnVolume' child game object with trigger collider attached to it into this field.")]
            public Collider SpawnVolume;

            //[Tooltip("[Draft]Drag and drop the trigger collider for this wave which is the child of this gameObject")]
            //public Collider TriggerCollider;
            [Tooltip("Set the SpawnPoint radius within which Ai agents can be spawned.")]
            public float SpawnPointRadius = 15f;
            [Tooltip("Drag and drop the empty gameObject child of this gameObject and place the child gameObject where you want the agents to spawn.")]
            public Transform SpawnPoint;
            [Tooltip("[Draft]Number of agents to spawn in this wave.")]
            public int NumberOfAgentsToSpawn;
        }
        [Tooltip("Set the total number of waves for this spawner. Each element in this list contains set of fields and checkboxes for wave customisation.")]
        public List<NormalWave> Waves = new List<NormalWave>();

        //public Wave[] AllWaveScripts;

        [Tooltip("Create one or more waves that will get activated when the player will hit their trigger colliders. Those trigger colliders should be attached to 'WaveActivator' game objects." +
            "Waves are activated by their dedicated activators.Each wave requires such activator which still can be shared between different waves." +
            "To spawn next wave player has to enter trigger assigned to that wave which stays deactivated until previous waves are destroyed.")]
        public List<TriggerWave> TriggerWaves = new List<TriggerWave>();

        [HideInInspector]
        public int countingWaves = 0;

        [HideInInspector]
        public bool IsWavesFinished = false;
        int CountingSpawnPoints;

        [HideInInspector]
        public int CurrentWaveRunning = 0;

        [HideInInspector]
        public List<Transform> AliveAgents = new List<Transform>();

        bool ShouldLookForAliveAgentsNow = false;
        float TimeToCheckForDeadAgents;

        // Why this TargetsScript is only exist in the wave spawner script because in the wave spawner script if you spawn for example max all 5 friendlies in the first wave than the second wave will
        // be started and will not check if the Ai is alive or not because they are not the enemy and similary if it is mutliple trigger based than it will activate the gameObject for player to get into
        // to start the wave. Now this functionality do not exist in the trigger spawner or the basic spawner because there task is to maintain a certain number of Ai agents no matter they are enemy or
        // friendlies but in wave spawner this change happens dynamically which indeed has to make sure to get activated only in the case when enemies are not around.
        [HideInInspector]
        public Targets TargetsScript;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (DrawSpawnerGizmos != DrawGizmosInScene.DoNotDebugGizmos)
            {
                Gizmos.color = GizmosColor;

                if (DrawSpawnerGizmos == DrawGizmosInScene.DrawWireSphere)
                {
                    if(ActivationType == WaveType.BeginOnPlay || ActivationType == WaveType.StartOnTriggerEnter)
                    {
                        for (int x = 0; x < Waves.Count; x++)
                        {
                            if (Waves[x].SpawnPoint != null)
                            {
                                GizmosHandler.DrawWireSphereBasedOfRange(Waves[x].SpawnPoint.position, Waves[x].SpawnPointRadius, GizmosColor);
                            }
                        }
                    }
                    else if (ActivationType == WaveType.EnterEachWaveTrigger)
                    {
                        for (int x = 0; x < TriggerWaves.Count; x++)
                        {
                            if (TriggerWaves[x].SpawnPoint != null)
                            {
                                GizmosHandler.DrawWireSphereBasedOfRange(TriggerWaves[x].SpawnPoint.position, TriggerWaves[x].SpawnPointRadius, GizmosColor);
                            }
                        }
                    }
                   

                }
                else
                {
                    if (ActivationType == WaveType.BeginOnPlay || ActivationType == WaveType.StartOnTriggerEnter) 
                    {
                        for (int x = 0; x < Waves.Count; x++)
                        {
                            if (Waves[x].SpawnPoint != null)
                            {
                                GizmosHandler.DrawWireCubeBasedOfRange(Waves[x].SpawnPoint.position, Waves[x].SpawnPointRadius, GizmosColor);
                            }
                        }
                    }
                    else if(ActivationType == WaveType.EnterEachWaveTrigger)
                    {
                        for (int x = 0; x < TriggerWaves.Count; x++)
                        {
                            if (TriggerWaves[x].SpawnPoint != null)
                            {
                                GizmosHandler.DrawWireCubeBasedOfRange(TriggerWaves[x].SpawnPoint.position, TriggerWaves[x].SpawnPointRadius, GizmosColor);
                            }
                        }
                    }
                }

//#if UNITY_EDITOR
//                if (ChooseWaveType == WaveType.StartWaveBasedOnMultipleTrigger)
//                {
//                    for (int x = 0; x < TriggerWaves.Count; x++)
//                    {
//                        GizmosHandler.DrawColliderWireframe(TriggerWaves[x].TriggerCollider);
//                    }
//                }
//#endif

//#if UNITY_EDITOR
                // Draw label in Scene view
                GizmosHandler.DisplayText(transform.position, transform.name);
                if (ActivationType == WaveType.BeginOnPlay)
                {
                    for (int x = 0; x < Waves.Count; x++)
                    {
                        if (Waves[x].SpawnPoint != null)
                        {
                            GizmosHandler.DisplayText(Waves[x].SpawnPoint.transform.position, Waves[x].SpawnPoint.transform.name);
                        }
                    }
                }
                //else
                //{
                //    for (int x = 0; x < TriggerWaves.Count; x++)
                //    {
                //        GizmosHandler.DisplayText(TriggerWaves[x].TriggerCollider.transform.position, TriggerWaves[x].TriggerCollider.transform.name);
                //        GizmosHandler.DisplayText(TriggerWaves[x].AgentsSpawningArea.transform.position, TriggerWaves[x].AgentsSpawningArea.transform.name);
                //    }
                //}
//#endif
            }
        }
#endif
        private void Start()
        {       
            if (ActivationType == WaveType.EnterEachWaveTrigger)
            {
                NextWaveAfterPreviousWaveDestroyed = true; // Need to be true because when the player enter the trigger it start wave 1 and only after the wave 1 enemies are dead than
                                                            // only it should activate the trigger collider for the player to get into. because in case suppose this condition is false than
                                                            // it will look strange if the player accidentally touch the collider in game it will keep activate new and new wave which will
                                                            // make the player experience bad and that's why we need to activate only when the player enemies are killed.
            
                if (TriggerWaves.Count >= 1)
                {
                    TriggerWaves[0].WaveScript.gameObject.SetActive(true);
                }
                for (int x = 0; x < TriggerWaves.Count;x++)// TriggerWave wave in )
                {
                    
                   // wave.TriggerCollider.enabled = false;
                    if (TriggerWaves[x].SpawnWithinVolume == true)
                    {
                        TriggerWaves[x].SpawnVolume.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                    }
                }

                TimeToCheckForDeadAgents = Random.Range(MinCasualtiesCheckTime, MaxCasualtiesCheckTime);
            }
            else if (ActivationType == WaveType.StartOnTriggerEnter)
            {
               
                foreach (TriggerWave wave in TriggerWaves)
                {
                    if (wave.SpawnWithinVolume == true)
                    {
                        wave.SpawnVolume.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                    }
                }
            }
            else if(ActivationType == WaveType.BeginOnPlay)
            {
                if (GameObject.FindGameObjectWithTag("Player") != null)
                {
                    GameObject Player = GameObject.FindGameObjectWithTag("Player");
                   
                    if (Player.gameObject.GetComponent<Targets>() != null)
                    {
                        TargetsScript = Player.gameObject.GetComponent<Targets>();
                    }
                }
                for (int x = 0; x < Waves.Count; x++)
                {
                    if (Waves[x].SpawnWithinVolume == true)
                    {
                        Waves[x].SpawnVolume.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                    }
                }
                TimeToCheckForDeadAgents = Random.Range(MinCasualtiesCheckTime, MaxCasualtiesCheckTime);
                ActivateWaves();
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == Player)
            {
                if (ColliderComponent != null)
                {
                    ColliderComponent.enabled = false;
                }

                if(other.gameObject.GetComponent<Targets>() != null)
                {
                    TargetsScript = other.gameObject.GetComponent<Targets>();
                }

                TimeToCheckForDeadAgents = Random.Range(MinCasualtiesCheckTime, MaxCasualtiesCheckTime);
                ActivateWaves();
            }
        }
        public void CheckForDeadEnemies()
        {
            if(ShouldLookForAliveAgentsNow == true)
            {
                bool IsAnyOneAlive = false;
                for (int x = 0; x < AliveAgents.Count; x++)
                {
                    if(IsAnyOneAlive == false)
                    {
                        if (AliveAgents[x] != null)
                        {
                            if (AliveAgents[x].GetComponent<CoreAiBehaviour>() != null)
                            {
                                //if(TargetsScript != null)
                                //{
                                //    if (AliveAgents[x].GetComponent<CoreAiBehaviour>().HealthScript.IsDied == true || AliveAgents[x].GetComponent<CoreAiBehaviour>().T.MyTeamTag == TargetsScript.MyTeamTag)
                                //    {
                                //        IsAnyOneAlive = false;
                                //    }
                                //    else
                                //    {
                                //        IsAnyOneAlive = true;
                                //    }
                                //}
                                //else
                                //{
                                if (AliveAgents[x].GetComponent<CoreAiBehaviour>().HealthScript != null)
                                {
                                    if (AliveAgents[x].GetComponent<CoreAiBehaviour>().HealthScript.IsDied == true)
                                    {
                                        IsAnyOneAlive = false;
                                    }
                                    else
                                    {
                                        IsAnyOneAlive = true;
                                    }
                                }
                                else
                                {
                                    IsAnyOneAlive = false;
                                }
                                // }
                            }

                        }
                        else
                        {
                            IsAnyOneAlive = false;
                        }
                       
                    }
                    
                }

                if (ActivationType == WaveType.BeginOnPlay || ActivationType == WaveType.StartOnTriggerEnter)
                {
                    if (IsAnyOneAlive == false)
                    {
                        AliveAgents.Clear();
                        ++CurrentWaveRunning;
                        if (CurrentWaveRunning < Waves.Count)
                        {

                            SpawnAgentsAfterNoAgentAlive();
                            ShouldLookForAliveAgentsNow = false;
                        }
                        else
                        {
                            ShouldLookForAliveAgentsNow = false;
                        }
                    }
                }
                else
                {
                    if (IsAnyOneAlive == false)
                    {
                        AliveAgents.Clear();
                        ++CurrentWaveRunning;

                        if (CurrentWaveRunning < TriggerWaves.Count)
                        {
                            TriggerWaves[CurrentWaveRunning].WaveScript.gameObject.SetActive(true);
                            ShouldLookForAliveAgentsNow = false;
                        }
                        else
                        {
                            ShouldLookForAliveAgentsNow = false;
                        }
                    }
                }
            }
           
        }
        public void ActivateWaves()
        {
            //if (ChooseWaveType == WaveType.StartWaveBasedOnMultipleTrigger)
            //{
            //    TriggerWaves[CurrentWaveRunning].TriggerCollider.enabled = true;
            //}
            //else
            //{
                if (NextWaveAfterPreviousWaveDestroyed == false)
                {
                    StartCoroutine(CreateWaves());                 
                }
                else
                {
                    
                    StartCoroutine(CreateRandomWavesAfterCurrentAgentsAreDead());
                }
            //}
        }
        public void SpawnAgentsAfterNoAgentAlive()
        {
            StartCoroutine(CreateRandomWavesAfterCurrentAgentsAreDead());
        }
        IEnumerator CreateRandomWavesAfterCurrentAgentsAreDead()
        {
            if (DisplayCountDownTimer == true)
            {
                CountDownTimerText.gameObject.SetActive(true);
            }
            for (int countdown = Mathf.FloorToInt(Waves[CurrentWaveRunning].WaveDelay); countdown > 0; countdown--)
            {
                if (DisplayCountDownTimer == true)
                {
                    CountDownTimerText.text = countdown.ToString();
                }
                yield return new WaitForSeconds(1f);
            }
            CountDownTimerText.text = "0";
            CountDownTimerText.gameObject.SetActive(false);
            for (int i = 0; i < Waves[CurrentWaveRunning].NumberOfAgentsToSpawn; i++)
            {
                if (Waves[CurrentWaveRunning].UsePreciseSpawnPoints == true)
                {
                    SpawnCustomEnemies(Waves[CurrentWaveRunning].UseAgentsListScript, Waves[CurrentWaveRunning].AiAgentsListScript, Waves[CurrentWaveRunning].Agents,
                        Waves[CurrentWaveRunning].PreciseSpawnPoints);
                }
                else
                {
                    maxSpawnAttemptsPerObstacle = Waves[CurrentWaveRunning].NumberOfAgentsToSpawn + 1;
                    SpawnEnemies(Waves[CurrentWaveRunning].SpawnPoint, Waves[CurrentWaveRunning].SpawnPointRadius
                        ,Waves[CurrentWaveRunning].UseAgentsListScript, Waves[CurrentWaveRunning].AiAgentsListScript,
                        Waves[CurrentWaveRunning].Agents,Waves[CurrentWaveRunning].SpawnWithinVolume, Waves[CurrentWaveRunning].SpawnVolume);
                }
                  
            }
            ShouldLookForAliveAgentsNow = true;
            InvokeRepeating("CheckForDeadEnemies", TimeToCheckForDeadAgents, TimeToCheckForDeadAgents);
            if (EnableWavesCounterUI == true)
            {
                WaveText.gameObject.SetActive(true);
                WaveText.text = WaveName + " " + (CurrentWaveRunning + 1);
                StartCoroutine(RemoveText());
            }
        }
        public void SpawnAgentsOnTrigger()
        {
            for (int i = 0; i < TriggerWaves[CurrentWaveRunning].NumberOfAgentsToSpawn; i++)
            {
                if (TriggerWaves[CurrentWaveRunning].UsePreciseSpawnPoints == true)
                {
                    SpawnCustomEnemies(TriggerWaves[CurrentWaveRunning].UseAgentsListScript, TriggerWaves[CurrentWaveRunning].AiAgentsListScript, TriggerWaves[CurrentWaveRunning].Agents
                        , TriggerWaves[CurrentWaveRunning].PreciseSpawnPoints);
                }
                else
                {
                    maxSpawnAttemptsPerObstacle = TriggerWaves[CurrentWaveRunning].NumberOfAgentsToSpawn + 1;
                    SpawnEnemies(TriggerWaves[CurrentWaveRunning].SpawnPoint, TriggerWaves[CurrentWaveRunning].SpawnPointRadius
                         , TriggerWaves[CurrentWaveRunning].UseAgentsListScript, TriggerWaves[CurrentWaveRunning].AiAgentsListScript,
                         TriggerWaves[CurrentWaveRunning].Agents, TriggerWaves[CurrentWaveRunning].SpawnWithinVolume, TriggerWaves[CurrentWaveRunning].SpawnVolume);
                }             
            }
            ShouldLookForAliveAgentsNow = true;
            InvokeRepeating("CheckForDeadEnemies", TimeToCheckForDeadAgents, TimeToCheckForDeadAgents);
        }
        IEnumerator CreateWaves()
        {
            if (IsWavesFinished == false)
            {
                foreach (NormalWave wave in Waves)
                {
                    if (countingWaves < Waves.Count)
                    {
                        ++countingWaves;
                        if (DisplayCountDownTimer == true)
                        {
                            CountDownTimerText.gameObject.SetActive(true);
                        }
                        for (int countdown = Mathf.FloorToInt(wave.WaveDelay); countdown > 0; countdown--)
                        {
                            if (DisplayCountDownTimer == true)
                            {
                                CountDownTimerText.text = countdown.ToString();
                            }
                            yield return new WaitForSeconds(1f);
                        }
                        CountDownTimerText.text = "0";
                        CountDownTimerText.gameObject.SetActive(false);

                        if (wave.UsePreciseSpawnPoints == false)
                        {
                            for (int i = 0; i < wave.NumberOfAgentsToSpawn; i++)
                            {
                                maxSpawnAttemptsPerObstacle = wave.NumberOfAgentsToSpawn + 1;
                                SpawnEnemies(wave.SpawnPoint, wave.SpawnPointRadius
                                    , wave.UseAgentsListScript, wave.AiAgentsListScript, wave.Agents,wave.SpawnWithinVolume,wave.SpawnVolume);
                            }
                            if (EnableWavesCounterUI == true)
                            {
                                WaveText.gameObject.SetActive(true);
                                WaveText.text = WaveName + " " + (CurrentWaveRunning + 1);
                                StartCoroutine(RemoveText());
                            }
                            wave.NumberOfAgentsToSpawn = 0;
                            wave.WaveDelay = 0;
                        }
                        else
                        {
                            for (int i = 0; i < wave.NumberOfAgentsToSpawn; i++)
                            {
                                SpawnCustomEnemies(wave.UseAgentsListScript, wave.AiAgentsListScript, wave.Agents, wave.PreciseSpawnPoints);
                            }
                            if (EnableWavesCounterUI == true)
                            {
                                WaveText.gameObject.SetActive(true);
                                WaveText.text = WaveName + " " + (CurrentWaveRunning + 1);
                                StartCoroutine(RemoveText());
                            }
                            wave.NumberOfAgentsToSpawn = 0;
                            wave.WaveDelay = 0;
                        }
                    

                    }
                    else
                    {
                        IsWavesFinished = true;
                    }

                    AiUpdaterScript.Checking();

                }
            }
        }
        //IEnumerator CreateCustomWaves()
        //{
        //    if (IsWavesFinished == false)
        //    {
        //        foreach (Wave wave in waves)
        //        {
        //            if (countingWaves < waves.Count)
        //            {
        //                ++countingWaves;
        //                if (DisplayCountDownTimer == true)
        //                {
        //                    CountDownTimerText.gameObject.SetActive(true);
        //                }
        //                for (int countdown = Mathf.FloorToInt(wave.TimeToStartThisWave); countdown > 0; countdown--)
        //                {
        //                    if (DisplayCountDownTimer == true)
        //                    {
        //                        CountDownTimerText.text = countdown.ToString();
        //                    }
        //                    yield return new WaitForSeconds(1f);
        //                }
        //                CountDownTimerText.text = "0";
        //                CountDownTimerText.gameObject.SetActive(false);
        //                for (int i = 0; i < wave.NumberOfAgentsToSpawn; i++)
        //                {
        //                    SpawnCustomEnemies(wave.UseSeparateScriptForAssigningAgents, wave.AgentsToSpawnListScript, wave.Agents,wave.PredefinedPoints);
        //                }
        //                if (DisplayWaveCounterText == true)
        //                {
        //                    WaveText.gameObject.SetActive(true);
        //                    WaveText.text = TextBeforeWaveNumber + " " + wave.WaveNumber;
        //                    StartCoroutine(RemoveText());
        //                }
        //                wave.NumberOfAgentsToSpawn = 0;
        //                wave.TimeToStartThisWave = 0;
        //            }
        //            else
        //            {
        //                IsWavesFinished = true;
        //            }
        //            SpawnedAgentsUpdaterScript.Checking();

        //        }
        //    }
        //}
        IEnumerator RemoveText()
        {
            yield return new WaitForSeconds(TimeToDeactiveWaveText);
            WaveText.text = "";
        }
        public void SpawnEnemies(Transform SpawningArea,float Range,bool IsUsingASeparateScript, AiToSpawnList aiToSpawnListScript,GameObject[] ManualAssignOfAgents,bool SpawnUsingColliderVolume,
            Collider ColliderVolumeToSpawnWithin)
        {
            Vector3 position = Vector3.zero;
            bool validPosition = false;
            int spawnAttempts = 0;
            if (SpawnUsingColliderVolume == false)
            {
                while (!validPosition && spawnAttempts < maxSpawnAttemptsPerObstacle)
                {
                    spawnAttempts++;
                    position = GenerateRandomNavmeshLocation.RandomLocation(SpawningArea, Range);
                    validPosition = true;
                    //Collider[] colliders = Physics.OverlapSphere(position, AgentsSeparationRadius);
                    //foreach (Collider col in colliders)
                    //{
                    //    if (col.transform.root.tag == "AI" && col.gameObject.layer == LayerMask.NameToLayer("IgnoreRaycast"))
                    //    {
                    //        validPosition = false;
                    //    }
                    //}
                }
                if (validPosition)
                {
                    if (IsUsingASeparateScript == true)
                    {
                        int Randomise = Random.Range(0, aiToSpawnListScript.Agents.Length);
                        GameObject Go = Instantiate(aiToSpawnListScript.Agents[Randomise], position, Quaternion.identity);
                        AliveAgents.Add(Go.transform);
                    }
                    else
                    {
                        int Randomise = Random.Range(0, ManualAssignOfAgents.Length);
                        GameObject Go = Instantiate(ManualAssignOfAgents[Randomise], position, Quaternion.identity);
                        AliveAgents.Add(Go.transform);
                    }

                }
            }
            else
            {
                Bounds bounds = ColliderVolumeToSpawnWithin.bounds;

                // Sample a position on the NavMesh within the bounds
                Vector3 spawnPosition;
                if (RandomPointInBounds(bounds, out spawnPosition))
                {
                    if (IsUsingASeparateScript == true)
                    {
                        int Randomise = Random.Range(0, aiToSpawnListScript.Agents.Length);
                        GameObject Go = Instantiate(aiToSpawnListScript.Agents[Randomise], spawnPosition, Quaternion.identity);
                        AliveAgents.Add(Go.transform);
                    }
                    else
                    {
                        int Randomise = Random.Range(0, ManualAssignOfAgents.Length);
                        GameObject Go = Instantiate(ManualAssignOfAgents[Randomise], spawnPosition, Quaternion.identity);
                        AliveAgents.Add(Go.transform);
                    }
                    

                }
            }
            AiUpdaterScript.Checking();
        }
        public void SpawnCustomEnemies(bool IsUsingASeparateScript, AiToSpawnList aiToSpawnListScript, GameObject[] ManualAssignOfAgents,Transform[] PredefinedPoints)
        {
            int RandomisePlaces = Random.Range(0, PredefinedPoints.Length);
            int Randomise;
            GameObject Go;
            if (CountingSpawnPoints < PredefinedPoints.Length)
            {
                if (IsUsingASeparateScript == true)
                {
                    Randomise = Random.Range(0, aiToSpawnListScript.Agents.Length);
                    Go = Instantiate(aiToSpawnListScript.Agents[Randomise], PredefinedPoints[CountingSpawnPoints].transform.position, PredefinedPoints[CountingSpawnPoints].transform.rotation);
                }
                else
                {
                    Randomise = Random.Range(0, ManualAssignOfAgents.Length);
                    Go = Instantiate(ManualAssignOfAgents[Randomise], PredefinedPoints[CountingSpawnPoints].transform.position, PredefinedPoints[CountingSpawnPoints].transform.rotation);
                }
                AliveAgents.Add(Go.transform);
                CountingSpawnPoints++;
            }
            else
            {
                if (IsUsingASeparateScript == true)
                {
                    Randomise = Random.Range(0, aiToSpawnListScript.Agents.Length);
                    Go = Instantiate(aiToSpawnListScript.Agents[Randomise], PredefinedPoints[RandomisePlaces].transform.position, PredefinedPoints[RandomisePlaces].transform.rotation);
                }
                else
                {
                    Randomise = Random.Range(0, ManualAssignOfAgents.Length);
                    Go = Instantiate(ManualAssignOfAgents[Randomise], PredefinedPoints[RandomisePlaces].transform.position, PredefinedPoints[RandomisePlaces].transform.rotation);
                }
                AliveAgents.Add(Go.transform);
            }
            AiUpdaterScript.Checking();
        }
        // Function to find a random point on the NavMesh within the given bounds
        private bool RandomPointInBounds(Bounds bounds, out Vector3 result)
        {
            for (int i = 0; i < 30; i++) // Attempt to find a valid point within 30 tries
            {
                Vector3 randomPoint = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    Random.Range(bounds.min.z, bounds.max.z)
                );

                // Check if the random point is on the NavMesh
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }

            // If after 30 attempts no valid point is found, return false
            result = Vector3.zero;
            return false;
        }
    }
}