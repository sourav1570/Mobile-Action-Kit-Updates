using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace MobileActionKit
{
    public class AiMaintainingSpawner : MonoBehaviour
    {
        [TextArea]        
        public string ScriptInfo = "This script specialises on spawning Ai agents only. " +
            "It gives more control over Ai spawning and respawning functionality by monitoring spawned Ai agents and replenishing their numbers if some of them die. " +
            "And it can start functioning with the start of the game as well as on entering trigger.";

        public enum DrawGizmosInScene
        {
            DrawWireSphere,
            DrawWireCube,
            DoNotDebugGizmos
        }


        [Tooltip("Draw the gizmos of the spawning areas in the unity scene view.")]
        public DrawGizmosInScene ShowGizmosInTheScene;
        [Tooltip("Set the colour of the spawning area gizmos in the unity scene view.")]
        public Color GizmosColor = Color.red;

        [Tooltip("Drag&drop the gameobject(usually player) from the hierarchy into this field to trigger spawn event.")]
        public GameObject TriggeringGameObject;

        public enum ChooseSpawnerType
        {
            SpawnOnStart,
            SpawnOnTriggerEnter
        }

        [Tooltip("Choose one of the types of the 'AiMaintainingSpawner' activation. " +
            "If 'SpawnOnTriggerEnter' is chosen than Ai Agents will be spawned only if the 'TriggeringGameObject'(usually player) will enter the trigger collider  attached to 'AiMaintainingSpawner' " +
            "game object.Additional trigger collider creation options will be created at the bottom of this script for you to choose the one you want as your spawn trigger." +
            "All three of them can be used if needed and positioned in a certain places of the level in which case entering any of them will trigger the spawn event." +
            "If 'SpawnOnStart' is chosen than spawner will start functioning with the start of the game after specified delay. ")]
        public ChooseSpawnerType SpawnerActivationType;

        [Tooltip("If checked than Ai agents will spawn within the volume of the trigger collider attached to child game object of this 'AiMaintainingSpawner' named 'SpawnVolume'which you`ll have to create, " +
            "add trigger collider to it and set up its dimensions for this purpose to make sure that Agents will spawn only within a certain rooms or any other kind of enclosed spaces.")]
        public bool SpawnWithinVolume = false;
        [Tooltip("Drag and drop 'SpawnVolume' child game object with trigger collider attached to it into this field.")]
        public Collider SpawnVolume;

        [Tooltip("If checked it will spawn agents that are listed inside 'AiToSpawnList' script that can hold various types of Ai agents. Spawner then randomly spawns Ai agents from that list.")]
        public bool UseAgentsListScript = true;
        [Tooltip("Drag and drop 'AiToSpawnList' gameobject with 'AiAgentsListScript' attached to it in this field from the hierarchy of the 'AiMaintainingSpawner' gameobject.")]
        public AiToSpawnList AiAgentsListScript;
        [Tooltip("This dropdown list is an alternative to 'AiToSpawnList' of Ai agents assignment. It can be set to desired number of fields that would hold Ai agent prefabs to be spawned." +
            "Drag and drop Ai agents from the project into these fields.")]
        public GameObject[] Agents;

        [Tooltip("Drag and drop child gameobject named 'SpawnPoint' into this field.")]
        public Transform SpawnPoint;
        [Tooltip("Number of agents this spawner script can spawn in total.")]
        public int MaxNumberOfAgentsToSpawn = 7;

        [Range(0.01f,600)][Tooltip("This slider delays the spawning of Ai agents. Minimal value of the delay can be 0.01 seconds and maximal value can be 600 seconds.")]
        public float SpawnOnStartDelay = 0.01f;

        [Tooltip("If checked than this script will use 'AiMaintainingSpawnersGlobalAiList' to store its spawned agents.If unchecked than this script will itself going to store its spawned agents.")]
        public bool UseAiMaintainingSpawnersGlobalAiList = false;
        [Tooltip("Drag and drop 'TriggerSpawnersGlobalAiList' script from the hierarchy into this field.")]
        public AiMaintainingSpawnersGlobalAiList AiMaintainingSpawnersGlobalAiListScript;
        //[Tooltip("[Draft]Minimum time to spawn the agents.for example - If this value is 0 than the number inside the field 'AgentsToMaintainInGame' will be used to spawn all agents at the same time.i.e" +
        //    " if the Agents To Maintain In Game is 5 than all of the 5 agents will be spawned at the same time.")]
        //public float MinSpawnTimeIntervals;

        //[Tooltip("[Draft]Maximum time to spawn the agents.for example - If this value is 0 than the number inside the field 'AgentsToMaintainInGame' will be used to spawn all agents at the same time.i.e" +
        //   " if the Agents To Maintain In Game is 5 than all of the 5 agents will be spawned at the same time.")]
        //public float MaxSpawnTimeIntervals;

        [Tooltip("If checked then will spawn Ai agents at specified spawn points that should be placed in 'SpawnPoint' fields.")]
        public bool UsePreciseSpawnPoints;
        [Tooltip("Set the number of preplaced Spawn points used for this wave.Drag and drop those spawnPoints to be used for this wave from hierarchy tab into this field.")]
        public Transform[] PreciseSpawnPoints;

        [Tooltip("Minimum time to check the number of alive Ai agents.")]
        private float MinCasualtiesCheckTime = 1;
        [Tooltip("Maximum time to check the number of alive Ai agents.")]
        private float MaxCasualtiesCheckTime = 2;

        int maxSpawnAttemptsPerObstacle;

        [Tooltip("Drag and drop Ai Updater game object from the hierarchy with 'AiUpdater' script attached to it into this field.")]
        public AiUpdater AiUpdaterScript;
        [Tooltip("Number of alive agents to keep in game. This number will be be replenished by this spawner as long as there are enough agents in 'MaxNumberOfAgentsToSpawn' for that.")]
        public int AgentsToMaintainInGame = 5;

        [Tooltip("Radius within limits of which Ai Agents will be spawned. ")]
        public float SpawnPointRadius = 200f;
        //[Tooltip("[Draft]Radius to check For agent before spawning so they do not spawn over each other")]
        //public float AgentsSeparationRadius = 3f;

        //[HideInInspector]
        [Tooltip("List of Ai Agents to be dispayed in this subsection for debuging purposes.")]
        public List<GameObject> DisplaySpawnedAgentsList = new List<GameObject>();

        int CountingSpawnPoints;
        bool StartChecking = false;
        bool SpawnEnemies = true;
        int ExactTeamList;
        bool ReachedTheThreshold = false;
        bool Spawned = false;
        [HideInInspector]
        public int Maxenemies;
        [HideInInspector]
        public int LimitofEnemies;
        private List<GameObject> SpawnedAgentsList = new List<GameObject>();
        bool CanstartChecking = false;
        GameObject gos;
        bool AlreadySpawned = false;
        Collider collidercomponent;
        //List<int> StoreDeadEnemies = new List<int>();

        int RandomAgentToSpawn;

        bool IsActivated = false;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (ShowGizmosInTheScene != DrawGizmosInScene.DoNotDebugGizmos)
            {
                Gizmos.color = GizmosColor;

                if (ShowGizmosInTheScene == DrawGizmosInScene.DrawWireSphere)
                {
                    if(SpawnPoint != null)
                    {
                        GizmosHandler.DrawWireSphereBasedOfRange(SpawnPoint.position, SpawnPointRadius, GizmosColor);

                    }

                }
                else
                {
                    if (SpawnPoint != null)
                    {
                        GizmosHandler.DrawWireCubeBasedOfRange(SpawnPoint.position, SpawnPointRadius, GizmosColor);
                    }
                }

//#if UNITY_EDITOR
               
//                if(GetComponent<Collider>() != null)
//                {
//                    collidercomponent = GetComponent<Collider>();
//                    // Draw a wireframe representation based on the Collider type
//                    GizmosHandler.DrawColliderWireframe(collidercomponent);
//                }

//#endif

#if UNITY_EDITOR
                // Draw label in Scene view
                GizmosHandler.DisplayText(transform.position, transform.name);
                if (SpawnPoint != null)
                {
                    GizmosHandler.DisplayText(SpawnPoint.position, SpawnPoint.transform.name);
                }
#endif
            }


        }
#endif
        private void Awake()
        {
            if (SpawnWithinVolume == true)
            {
                SpawnVolume.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }

            maxSpawnAttemptsPerObstacle = AgentsToMaintainInGame + 1;
            Maxenemies = AgentsToMaintainInGame;
          
        }
        private void OnDisable()
        {
            if (UseAiMaintainingSpawnersGlobalAiList == true)
            {
                CanstartChecking = false;
                StartChecking = true;
                IsActivated = false;
            }
        }
        private void OnEnable()
        {
            collidercomponent = GetComponent<Collider>();
            if (UseAiMaintainingSpawnersGlobalAiList == true)
            {
                CanstartChecking = true;
                StartChecking = true;

                if (collidercomponent != null)
                {
                    collidercomponent.enabled = true;
                }

            }
        }
        private void Start()
        {
            if (SpawnerActivationType == ChooseSpawnerType.SpawnOnStart)
            {
                float TimeToRespawnAi = Random.Range(MinCasualtiesCheckTime, MaxCasualtiesCheckTime);

                IsActivated = true;

                InvokeRepeating("CheckForAliveEnemies", 0, TimeToRespawnAi);

                if (UsePreciseSpawnPoints == false)
                {
                    InvokeRepeating("StartingWait", SpawnOnStartDelay, TimeToRespawnAi);
                }
                else if (UsePreciseSpawnPoints == true)
                {
                    InvokeRepeating("CustomEnemies", SpawnOnStartDelay, TimeToRespawnAi);
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if(SpawnerActivationType == ChooseSpawnerType.SpawnOnTriggerEnter)
            {
                if (other.gameObject == TriggeringGameObject)
                {
                    if (collidercomponent != null)
                    {
                        collidercomponent.enabled = false;
                    }

                    float TimeToRespawnAi = Random.Range(MinCasualtiesCheckTime, MaxCasualtiesCheckTime);

                    IsActivated = true;

                    CheckForAliveEnemies();
                    InvokeRepeating("CheckForAliveEnemies", TimeToRespawnAi, TimeToRespawnAi);

                    if (UsePreciseSpawnPoints == false)
                    {
                        InvokeRepeating("StartingWait", 0f, TimeToRespawnAi);
                    }
                    else if (UsePreciseSpawnPoints == true)
                    {
                        InvokeRepeating("CustomEnemies", 0f, TimeToRespawnAi);
                    }

                }
            }
            
        }
        private void CheckForAliveEnemies()// Checking For Enemies Alive if not then Spawn and maintain enemies 
        {
            if (CanstartChecking == true)
            {
                if(UseAiMaintainingSpawnersGlobalAiList == false)
                {
                    DisplaySpawnedAgentsList.Clear();


                    int TemporarySpawnedAgentList = SpawnedAgentsList.Count;
                    for (int i = 0; i < TemporarySpawnedAgentList; ++i)
                    {
                        DisplaySpawnedAgentsList.Add(SpawnedAgentsList[i]);
                    }

                    for (int i = 0; i < TemporarySpawnedAgentList; ++i)
                    {
                        //if (i < SpawnedAgentsList.Count)
                        //{
                            if (SpawnedAgentsList[i] != null)
                            {
                                if (SpawnedAgentsList[i].GetComponent<HumanoidAiHealth>() != null)
                                {
                                    if (SpawnedAgentsList[i].GetComponent<HumanoidAiHealth>().IsDied == true)
                                    {
                                        DisplaySpawnedAgentsList.Remove(SpawnedAgentsList[i]);
                                       // SpawnedAgentsList.Remove(SpawnedAgentsList[i]);
                                    }
                                }
                            }
                            else
                            {
                                DisplaySpawnedAgentsList.Remove(SpawnedAgentsList[i]);
                               // SpawnedAgentsList.Remove(SpawnedAgentsList[i]);
                            }
                        //}
                    }

                    TemporarySpawnedAgentList = DisplaySpawnedAgentsList.Count;
                    SpawnedAgentsList.Clear();

                    for (int i = 0; i < TemporarySpawnedAgentList; ++i)
                    {
                        SpawnedAgentsList.Add(DisplaySpawnedAgentsList[i]);
                    }

                    ExactTeamList = SpawnedAgentsList.Count;
                    if (StartChecking == true)
                    {
                        Maxenemies = AgentsToMaintainInGame - ExactTeamList;
                        SpawnEnemies = true;
                        ReachedTheThreshold = false;

                    }
                    if (ExactTeamList >= AgentsToMaintainInGame)
                    {
                        StartChecking = true;
                        SpawnEnemies = false;
                        ReachedTheThreshold = true;
                        Maxenemies = 0;


                    }
                }
                else
                {
                    if(AiMaintainingSpawnersGlobalAiListScript != null)
                    {
                      //  Debug.Log("WORKING");
                        AiMaintainingSpawnersGlobalAiListScript.DisplaySpawnedAgentsList.Clear();

                        int TemporarySpawnedAgentList = AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList.Count;
                        for (int i = 0; i < TemporarySpawnedAgentList; ++i)
                        {
                            AiMaintainingSpawnersGlobalAiListScript.DisplaySpawnedAgentsList.Add(AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList[i]);
                        }

                        for (int i = 0; i < TemporarySpawnedAgentList; ++i)
                        {
                            //if (i < AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList.Count)
                            //{
                                if(AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList[i] != null)
                                {
                                    if (AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList[i].GetComponent<HumanoidAiHealth>() != null)
                                    {
                                        if (AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList[i].GetComponent<HumanoidAiHealth>().IsDied == true)
                                        {
                                            AiMaintainingSpawnersGlobalAiListScript.DisplaySpawnedAgentsList.Remove(AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList[i]);
                                           // AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList.Remove(AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList[i]);
                                        }
                                    }
                                }
                                else
                                {
                                    AiMaintainingSpawnersGlobalAiListScript.DisplaySpawnedAgentsList.Remove(AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList[i]);
                                   // AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList.Remove(AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList[i]);
                                }
                            //}
                        }

                        TemporarySpawnedAgentList = AiMaintainingSpawnersGlobalAiListScript.DisplaySpawnedAgentsList.Count;
                        AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList.Clear();

                        for (int i = 0; i < TemporarySpawnedAgentList; ++i)
                        {
                            AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList.Add(AiMaintainingSpawnersGlobalAiListScript.DisplaySpawnedAgentsList[i]);
                        }

                        ExactTeamList = AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList.Count;

                        if (StartChecking == true)
                        {
                            Maxenemies = AgentsToMaintainInGame - ExactTeamList;
                            SpawnEnemies = true;
                            ReachedTheThreshold = false;

                        }
                        if (ExactTeamList >= AgentsToMaintainInGame)
                        {
                            StartChecking = true;
                            SpawnEnemies = false;
                            ReachedTheThreshold = true;
                            Maxenemies = 0;


                        }
                    }
                   
                }
               
            }

        }
        public void EnemiesSpawn()
        {
            if (UseAgentsListScript == false)
            {
                RandomAgentToSpawn = Random.Range(0, Agents.Length);
            }
            else
            {
                RandomAgentToSpawn = Random.Range(0, AiAgentsListScript.Agents.Length);
            }
        }
        void StartingWait()
        {
            if(gameObject.activeInHierarchy == true && IsActivated == true)
            {
                if (SpawnEnemies == true && ReachedTheThreshold == false && LimitofEnemies < MaxNumberOfAgentsToSpawn)
                {
                    for (int i = 0; i < Maxenemies; i++)
                    {
                        EnemiesSpawn();
                        int spawnAttempts = 0;
                        if (SpawnWithinVolume == false)
                        {
                            Vector3 position = Vector3.zero;
                            bool validPosition = false;
                            while (!validPosition && spawnAttempts < maxSpawnAttemptsPerObstacle)
                            {
                                spawnAttempts++;
                                position = GenerateRandomNavmeshLocation.RandomLocation(SpawnPoint, SpawnPointRadius);
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
                                if (UseAgentsListScript == false)
                                {
                                    gos = Instantiate(Agents[RandomAgentToSpawn], position, Quaternion.identity);
                                    AddSpawnedAgentToTheList(gos);
                                    // gos.SetActive(false);
                                }
                                else
                                {
                                    gos = Instantiate(AiAgentsListScript.Agents[RandomAgentToSpawn], position, Quaternion.identity);
                                    AddSpawnedAgentToTheList(gos);
                                    //  gos.SetActive(false);
                                }

                                ++LimitofEnemies;
                            }
                        }
                        else
                        {
                            //while (spawnAttempts < maxSpawnAttemptsPerObstacle)
                            //{
                            //spawnAttempts++;
                            // Get the bounds of the collider
                            Bounds bounds = SpawnVolume.bounds;

                            // Sample a position on the NavMesh within the bounds
                            Vector3 spawnPosition;
                            if (RandomPointInBounds(bounds, out spawnPosition))
                            {
                                if (UseAgentsListScript == false)
                                {
                                    gos = Instantiate(Agents[RandomAgentToSpawn], spawnPosition, Quaternion.identity);
                                    AddSpawnedAgentToTheList(gos);
                                    // gos.SetActive(false);
                                }
                                else
                                {
                                    gos = Instantiate(AiAgentsListScript.Agents[RandomAgentToSpawn], spawnPosition, Quaternion.identity);
                                    AddSpawnedAgentToTheList(gos);
                                    //  gos.SetActive(false);
                                }
                                ++LimitofEnemies;

                            }
                            //}

                        }
                    }
                    // Randomise = -1;
                    //StartCoroutine(SpawningIntervals());
                }

                CanstartChecking = true;
                AiUpdaterScript.Checking();
                CheckForAliveEnemies();
            }
           

        }
        public void CustomEnemies()
        {
            if (gameObject.activeInHierarchy == true && IsActivated == true) 
            {
                if (SpawnEnemies == true && LimitofEnemies < MaxNumberOfAgentsToSpawn)// Resume == true
                {
                    for (int i = 0; i < Maxenemies; i++)
                    {
                        EnemiesSpawn();
                        int RandomisePlaces = Random.Range(0, PreciseSpawnPoints.Length);

                        if (CountingSpawnPoints <= (PreciseSpawnPoints.Length - 1))
                        {
                            if (UseAgentsListScript == false)
                            {
                                gos = Instantiate(Agents[RandomAgentToSpawn], PreciseSpawnPoints[CountingSpawnPoints].transform.position, PreciseSpawnPoints[CountingSpawnPoints].transform.rotation);
                                AddSpawnedAgentToTheList(gos);
                                //  gos.SetActive(false);
                            }
                            else
                            {
                                gos = Instantiate(AiAgentsListScript.Agents[RandomAgentToSpawn], PreciseSpawnPoints[CountingSpawnPoints].transform.position, PreciseSpawnPoints[CountingSpawnPoints].transform.rotation);
                                AddSpawnedAgentToTheList(gos);
                                //  gos.SetActive(false);
                            }

                            CountingSpawnPoints++;
                            ++LimitofEnemies;
                        }
                        else
                        {
                            if (UseAgentsListScript == false)
                            {
                                gos = Instantiate(Agents[RandomAgentToSpawn], PreciseSpawnPoints[RandomisePlaces].transform.position, PreciseSpawnPoints[RandomisePlaces].transform.rotation);
                                AddSpawnedAgentToTheList(gos);
                                // gos.SetActive(false);
                            }
                            else
                            {
                                gos = Instantiate(AiAgentsListScript.Agents[RandomAgentToSpawn], PreciseSpawnPoints[RandomisePlaces].transform.position, PreciseSpawnPoints[RandomisePlaces].transform.rotation);
                                AddSpawnedAgentToTheList(gos);
                                // gos.SetActive(false);
                            }

                            CountingSpawnPoints = 0;
                            ++LimitofEnemies;
                        }


                    }
                    // Randomise = -1;
                    //StartCoroutine(SpawningIntervals());
                }

                CanstartChecking = true;
                AiUpdaterScript.Checking();
                CheckForAliveEnemies();
            }
        }
        public void AddSpawnedAgentToTheList(GameObject item)
        {
            if (UseAiMaintainingSpawnersGlobalAiList == false)
            {
                SpawnedAgentsList.Add(item);
            }
            else
            {
                AiMaintainingSpawnersGlobalAiListScript.SpawnedAgentsList.Add(item);
            }
        }
        //IEnumerator SpawningIntervals()
        //{
        //    Spawned = false;
        //    float SpawnRandomisation = Random.Range(MinSpawnTimeIntervals, MaxSpawnTimeIntervals);
        //    yield return new WaitForSeconds(SpawnRandomisation);

        //    for (int x = 0; x < SpawnedAgentsList.Count; x++)
        //    {
        //        if (Spawned == false)
        //        {
        //            if (SpawnedAgentsList != null)
        //            {
        //                if (SpawnedAgentsList[x].activeInHierarchy == false)
        //                {
        //                    SpawnedAgentsList[x].SetActive(true);
        //                    Spawned = true;
        //                }
        //            }
        //        }

        //    }
        //    if (Spawned == true)
        //    {
        //        StartCoroutine(SpawningIntervals());
        //    }
        //}
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