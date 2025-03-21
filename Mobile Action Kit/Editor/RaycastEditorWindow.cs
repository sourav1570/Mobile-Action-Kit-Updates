using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


namespace MobileActionKit
{
    public class RaycastEditorWindow : EditorWindow
    {
        private GameObject raycastOrigin;
        private float raycastLength = 10f;
        private GameObject objectToSpawn;

        private GameObject lastSpawnedObject;
        private Vector3 lastHitPoint;

        private List<GameObject> spawnedObjects = new List<GameObject>();

        [MenuItem("Help/Sourav_PersonalTools/Raycast Spawner")]
        public static void ShowWindow()
        {
            GetWindow<RaycastEditorWindow>("Raycast Spawner");
        }

        private void OnGUI()
        {
            GUILayout.Label("Raycast Spawner Tool", EditorStyles.boldLabel);

            // Drag-and-drop field for the raycast origin
            raycastOrigin = (GameObject)EditorGUILayout.ObjectField("Raycast Origin", raycastOrigin, typeof(GameObject), true);

            // Input field for raycast length
            raycastLength = EditorGUILayout.FloatField("Raycast Length", raycastLength);

            // Drag-and-drop field for the object to spawn
            objectToSpawn = (GameObject)EditorGUILayout.ObjectField("Object to Spawn", objectToSpawn, typeof(GameObject), false);

            if (GUILayout.Button("Perform Raycast"))
            {
                PerformRaycast();
            }

            if (GUILayout.Button("Clear Last Spawned Object"))
            {
                ClearLastSpawnedObject();
            }

            if (GUILayout.Button("Spawn Prefab"))
            {
                SpawnPrefab();
            }

            if (GUILayout.Button("Destroy All Spawned Prefabs"))
            {
                DestroyAllSpawnedPrefabs();
            }
        }

        private void PerformRaycast()
        {
            if (raycastOrigin == null || objectToSpawn == null)
            {
                Debug.LogError("Please assign both the Raycast Origin and the Object to Spawn.");
                return;
            }

            // Perform a raycast from the origin
            Ray ray = new Ray(raycastOrigin.transform.position, raycastOrigin.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastLength))
            {
                Debug.Log($"Hit {hit.collider.name} at {hit.point}");

                // Check if the hit point is different from the last one
                if (lastSpawnedObject != null && lastHitPoint == hit.point)
                {
                    return; // No need to update if it's the same point
                }

                // Destroy the last spawned object if it exists
                ClearLastSpawnedObject();

                // Spawn a new object at the hit point
                lastSpawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(objectToSpawn);
                lastSpawnedObject.transform.position = hit.point;
                lastSpawnedObject.transform.rotation = Quaternion.identity;

                // Add to the list of spawned objects
                spawnedObjects.Add(lastSpawnedObject);

                // Update the last hit point
                lastHitPoint = hit.point;
            }
            else
            {
                Debug.Log("Raycast did not hit anything.");
            }
        }

        private void ClearLastSpawnedObject()
        {
            if (lastSpawnedObject != null)
            {
                spawnedObjects.Remove(lastSpawnedObject);
                DestroyImmediate(lastSpawnedObject);
                lastSpawnedObject = null;
                lastHitPoint = Vector3.zero;
            }
        }

        private void SpawnPrefab()
        {
            if (raycastOrigin == null || objectToSpawn == null)
            {
                Debug.LogError("Please assign both the Raycast Origin and the Object to Spawn.");
                return;
            }

            // Perform a raycast to determine where to spawn the object
            Ray ray = new Ray(raycastOrigin.transform.position, raycastOrigin.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastLength))
            {
                GameObject spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(objectToSpawn);
                spawnedObject.transform.position = hit.point;
                spawnedObject.transform.rotation = Quaternion.identity;

                spawnedObjects.Add(spawnedObject);
            }
            else
            {
                Debug.LogError("Raycast did not hit anything, cannot spawn prefab.");
            }
        }

        private void DestroyAllSpawnedPrefabs()
        {
            foreach (var obj in spawnedObjects)
            {
                if (obj != null)
                {
                    DestroyImmediate(obj);
                }
            }
            spawnedObjects.Clear();
            lastSpawnedObject = null;
            lastHitPoint = Vector3.zero;
        }
    }
}