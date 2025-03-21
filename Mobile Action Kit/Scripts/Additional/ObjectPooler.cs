using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class ObjectPooler : MonoBehaviour
    {

        [TextArea]
        [Tooltip("This Script will activate and deactivate all game Objects that reappear multiple times during gameplay." +
            "It`s purpose to reduce garbage collection proccess by allocating ram to storage of needed instances of certain game objects.")]
        public string ScriptInfo = "This Script will activate and deactivate all game Objects that reappear multiple times during gameplay." +
            "It`s purpose to reduce garbage collection proccess by allocating ram to storage of needed instances of certain game objects.";

        [System.Serializable]
        public class Pool
        {

            [Tooltip("Written Type of the required prefab in the calling script should exactly match prefab type written inside this field." +
                "For example in 'Gunscript' there is a field called 'Projectile Name'." +
                "Text from that field and text in this field should be exactly the same for projectile object pooling process to take effect.")]
            public string PrefabType;
            [Tooltip("Drag and drop prefab from your project into this field to create instances from.")]
            public GameObject Prefab;
            [Tooltip("Amount of instances of this prefab to store in operative memory.")]
            public int size;

        }

        #region Singleton
        public static ObjectPooler instance;
        private void Awake()
        {
            instance = this;
        }
        #endregion
        [Tooltip("Create number of pools needed for different game entities such as bullets, grenades, AI agents etc..")]
        public List<Pool> CreatePools;

        public Dictionary<string, Queue<GameObject>> pooldictionary;

        private void Start()
        {
            CreateObjects();
        }
        private void CreateObjects()
        {
            pooldictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in CreatePools)
            {
                Queue<GameObject> objectpool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.Prefab);
                    obj.SetActive(false);
                    objectpool.Enqueue(obj);
                }

                pooldictionary.Add(pool.PrefabType, objectpool);
            }

        }
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!pooldictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool With Tag" + tag + "Does not Exist");
                return null;
            }

            GameObject ObjToSpawn = pooldictionary[tag].Dequeue();

            ObjToSpawn.SetActive(true);
            ObjToSpawn.transform.position = position;
            ObjToSpawn.transform.rotation = rotation;

            pooldictionary[tag].Enqueue(ObjToSpawn);
            return ObjToSpawn;

        }

    }
}