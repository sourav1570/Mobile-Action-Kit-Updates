using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace MobileActionKit
{
    public class SimpleSpawner : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This spawner can spawn any type of game entities (Ai or non Ai prefabs).And it starts functioning with the start of the game. ";

        public enum DrawGizmosInScene
        {
            DrawWireSphere,
            DrawWireCube,
            DoNotDebug
        }

        public enum SpawnObjectType
        {
            AI,
            GameObject
        }

       
        [Tooltip("Draw the spawning area  gizmo in the unity scene view.")]
        public DrawGizmosInScene ShowGizmosInTheScene;

        [Tooltip("Set the color of the spawning area gizmos in the unity scene view.")]
        public Color GizmoColor = Color.red;

        [Tooltip("Drag&drop the gameobject(usually player) from the hierarchy into this field to trigger spawn event.")]
        public GameObject TriggeringGameObject;

        [Tooltip("If checked than the Spawnables will be spawned only if the player will enter the trigger collider  attached to 'Simple Spawner' game object.")]
        public bool SpawnOnlyOnTriggerEnter = false;
        [Tooltip("Choose one of the type of the Spawnable objects.")]
        public SpawnObjectType SpawnableType;

        [Tooltip("Assign game entities to be spawned by this spawner.")]
        public GameObject[] Spawnables;

        [Tooltip("Minimum time between spawns.")]
        public float MinSpawnDelay = 1f;
        [Tooltip("Maximum time between spawns.")]
        public float MaxSpawnDelay = 2f;

        [Tooltip("If checked than Ai agents or Gameobjects will spawn within the volume of the trigger collider attached to child game object of this 'GeneralSpawner' named 'SpawnVolume'which you`ll have to create, " +
            "add trigger collider to it and set up its dimensions for this purpose to make sure that Agents or Gameobjects will spawn only within a certain rooms or any other kind of enclosed spaces.")]
        public bool SpawnWithinVolume = false;
        [Tooltip("Drag and drop 'SpawnVolume' child game object with trigger collider attached to it into this field.")]
        public Collider SpawnVolume;

        [Tooltip("Drag and drop child gameobject named 'SpawnPoint' into this field.")]
        public Transform SpawnPoint;
        [Tooltip("Number of agents this spawner script can spawn.")]
        public int AmountOfSpawnables = 10;

        [Tooltip("Minimal offset of non Ai 'Spawnables'from spawn point along x,y, and z axis to  space them out from each other. ")]
        public Vector3 MinNonAiSpawnablesOffset = Vector3.zero;
        [Tooltip("Maximal offset of non Ai 'Spawnables' from spawn point along x,y, and z axis tto space them out from each other.")]
        public Vector3 MaxNonAiSpawnablesOffset = Vector3.zero;

        [Tooltip("Radius within limits of which Ai agents will be spawned.")]
        public float AiSpawnPointRadius = 200f;

        [Tooltip("Drag and drop Ai Updater game object from the hierarchy with 'AiUpdater' script attached to it into this field in case Spawnables of this spawner will be Ai agents.")]
        public AiUpdater AiUpdaterComponent;

        int Randomise;
        private List<GameObject> go = new List<GameObject>();
        bool StartOnce = false;

        GameObject gos;

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (ShowGizmosInTheScene != DrawGizmosInScene.DoNotDebug)
            {
                if (ShowGizmosInTheScene == DrawGizmosInScene.DrawWireSphere)
                {
                    Gizmos.color = GizmoColor;
                    if(SpawnPoint != null)
                    {
                        Gizmos.DrawWireSphere(SpawnPoint.position, AiSpawnPointRadius);
                    }
                }
                else
                {
                    Gizmos.color = GizmoColor;
                    if (SpawnPoint != null)
                    {
                        Gizmos.DrawWireCube(SpawnPoint.position, new Vector3(AiSpawnPointRadius * 2, AiSpawnPointRadius * 2, AiSpawnPointRadius * 2));
                    }
                }

#if UNITY_EDITOR

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
             
        }
        private void Start()
        {
            if(SpawnOnlyOnTriggerEnter == false)
            {
                StartCoroutine(DelayBeforeSpawn());
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if(SpawnOnlyOnTriggerEnter == true)
            {
                if (other.gameObject == TriggeringGameObject)
                {
                    if (StartOnce == false)
                    {
                        StartCoroutine(DelayBeforeSpawn());
                        StartOnce = true;
                    }
                }
            }
           
        }
        IEnumerator DelayBeforeSpawn()
        {
            float RandomSpawnTime = Random.Range(MinSpawnDelay, MaxSpawnDelay);
            yield return new WaitForSeconds(RandomSpawnTime);
            Randomise = Random.Range(0, Spawnables.Length);
            if (SpawnableType == SpawnObjectType.AI)
            {
                if (SpawnWithinVolume == false)
                {
                    Vector3 position = Vector3.zero;
                    bool validPosition = false;
                    int spawnAttempts = 0;
                    while (!validPosition)
                    {
                        spawnAttempts++;
                        position = GenerateRandomNavmeshLocation.RandomLocation(SpawnPoint, AiSpawnPointRadius);
                        validPosition = true;
                    }
                    if (validPosition)
                    {
                        gos = Instantiate(Spawnables[Randomise], position, Spawnables[Randomise].transform.rotation);
                        go.Add(gos);

                    }
                }
                else
                {
                    Bounds bounds = SpawnVolume.bounds;

                    // Sample a position on the NavMesh within the bounds
                    Vector3 spawnPosition;
                    if (RandomPointInBounds(bounds, out spawnPosition))
                    {

                        gos = Instantiate(Spawnables[Randomise], spawnPosition, Spawnables[Randomise].transform.rotation);
                        go.Add(gos);


                    }
                }
            }
            else
            {
                if (SpawnWithinVolume == false)
                {
                    Vector3 areaCenter = SpawnPoint.position;
                    Vector3 randomPosition = areaCenter + new Vector3(Random.Range(MinNonAiSpawnablesOffset.x, MaxNonAiSpawnablesOffset.x),
                        Random.Range(MinNonAiSpawnablesOffset.y , MaxNonAiSpawnablesOffset.y), Random.Range(MinNonAiSpawnablesOffset.z, MaxNonAiSpawnablesOffset.z)
                    );

                    gos = Instantiate(Spawnables[Randomise], randomPosition, Spawnables[Randomise].transform.rotation);
                    go.Add(gos);
                }
                else
                {
                    Bounds bounds = SpawnVolume.bounds;

                    // Sample a position on the NavMesh within the bounds
                    Vector3 spawnPosition;
                    if (RandomPointInBounds(bounds, out spawnPosition))
                    {

                        gos = Instantiate(Spawnables[Randomise], spawnPosition, Spawnables[Randomise].transform.rotation);
                        go.Add(gos);


                    }
                }
            }
            AiUpdaterComponent.Checking();
            if (go.Count < AmountOfSpawnables)
            {
                StartCoroutine(DelayBeforeSpawn());
            }

        }  
        private bool RandomPointInBounds(Bounds bounds, out Vector3 result)
        {
            for (int i = 0; i < 30; i++) // Attempt to find a valid point within 30 tries
            {
                Vector3 randomPoint = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    Random.Range(bounds.min.z, bounds.max.z)
                );

                if (SpawnableType == SpawnObjectType.AI)
                {
                    // Check if the random point is on the NavMesh
                    if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out UnityEngine.AI.NavMeshHit hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
                    {
                        result = hit.position;
                        return true;
                    }
                }
                else
                {
                    result = randomPoint;
                    return true;
                }
            }

            // If after 30 attempts no valid point is found, return false
            result = Vector3.zero;
            return false;
        }
    }
}