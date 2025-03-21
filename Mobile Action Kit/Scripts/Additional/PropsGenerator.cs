using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MobileActionKit
{
    public class PropsGenerator : MonoBehaviour
    {
        public bool UseCustomAreas = false;

        [System.Serializable]
        public class customiseProp
        {
            public BoxCollider Area;
            public bool MakePrefabChildOfArea = true;
            public GameObject[] Prefabs;
            public bool DestroyAreaColliderAfterGenerate = false;
            public bool EnableRandomPrefabGeneration = true;
            public int NumberofPrefabToGenerate;
            public float PrefabPositionAtY = -0.53f;
            public float MinimumPrefabRotationAtY;
            public float MaximumPrefabRotationAtY;
            // public float DistanceWithOtherPrefab;
        }

        public List<customiseProp> PropsCustomisation;

        public Transform PositionToCheckRadiusFrom;
        public float RadiusToSpawnWithin = 1000f;
        public GameObject[] Prefabs;
        public bool CreateBoxColliderOnPrefab = true;
        public bool EnableRandomPrefabGeneration = true;
        public int NumberofPrefabToGenerate;
        public float PrefabPositionAtY = -0.53f;
        public float MinimumPrefabRotationAtY;
        public float MaximumPrefabRotationAtY;

        public float DistanceOffset = 10;

        int CountingPrefabs;
        Vector3 Centre;
        Vector3 Size;

        GameObject go;

        public GameObject[] ObjectsToCreateColliderOn;
        public GameObject RootObjectOfAllMeshesToCombine;

        private List<Material> Mat = new List<Material>();

        Ray ray;
        RaycastHit hit;

        void Update()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    GameObject obj = Instantiate(Prefabs[Random.Range(0, Prefabs.Length)], new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity) as GameObject;

                }
            }
        }
        public void GenerateProp()
        {
            if (UseCustomAreas == true)
            {
                foreach (customiseProp pool in PropsCustomisation)
                {
                    for (int x = 0; x < pool.Prefabs.Length; x++)
                    {
                        Centre = pool.Area.transform.position;
                        Size = pool.Area.size;
                        while (CountingPrefabs < pool.NumberofPrefabToGenerate)
                        {
                            Vector3 rndPosWithin = Centre + new Vector3(Random.Range(-pool.Area.size.x / 2, pool.Area.size.x / 2), 0, Random.Range(-pool.Area.size.z / 2, pool.Area.size.z / 2));

                            if (pool.EnableRandomPrefabGeneration == true)
                            {
                                go = Instantiate(pool.Prefabs[Random.Range(0, pool.Prefabs.Length)], rndPosWithin, pool.Area.transform.rotation);
                            }
                            else
                            {
                                go = Instantiate(pool.Prefabs[x], rndPosWithin, pool.Area.transform.rotation);
                            }

                            if (pool.MakePrefabChildOfArea == true)
                            {
                                go.transform.parent = pool.Area.transform;
                            }
                            else
                            {
                                go.transform.parent = null;
                            }

                            Vector3 pos = go.transform.localPosition;
                            pos.y = pool.PrefabPositionAtY;
                            go.transform.localPosition = pos;

                            Vector3 temp = go.transform.eulerAngles;
                            temp.y = Random.Range(pool.MinimumPrefabRotationAtY, pool.MaximumPrefabRotationAtY);
                            go.transform.eulerAngles = temp;


                            ++CountingPrefabs;
                        }
                    }

                    if (pool.DestroyAreaColliderAfterGenerate == true)
                    {
                        DestroyImmediate(pool.Area.gameObject.GetComponent<BoxCollider>());
                    }

                }
            }
            else
            {
                for (int x = 0; x < Prefabs.Length; x++)
                {
                    while (CountingPrefabs < NumberofPrefabToGenerate)
                    {
                        Vector3 pos = GenerateRandomNavmeshLocation.RandomLocation(PositionToCheckRadiusFrom, RadiusToSpawnWithin);
                        if (EnableRandomPrefabGeneration == true)
                        {
                            go = Instantiate(Prefabs[Random.Range(0, Prefabs.Length)], pos, Quaternion.identity);
                            DistanceOffset += DistanceOffset;
                        }
                        else
                        {
                            go = Instantiate(Prefabs[x], pos, Quaternion.identity);
                        }

                        Vector3 posi = go.transform.localPosition;
                        posi.y = PrefabPositionAtY;
                        go.transform.localPosition = posi;

                        Vector3 temp = go.transform.eulerAngles;
                        temp.y = Random.Range(MinimumPrefabRotationAtY, MaximumPrefabRotationAtY);
                        go.transform.eulerAngles = temp;
                        ++CountingPrefabs;
                    }
                }
            }
        }
        public void GenerateColliders()
        {
            for (int i = 0; i < ObjectsToCreateColliderOn.Length; i++)
            {
                BoxCollider c = ObjectsToCreateColliderOn[i].AddComponent<BoxCollider>();
                BoxCollider col = ObjectsToCreateColliderOn[i].GetComponent<BoxCollider>();
                Bounds b = new Bounds(Vector3.zero, Vector3.zero);

                if (ObjectsToCreateColliderOn[i].GetComponent<MeshFilter>() != null)
                {
                    MeshFilter filter = ObjectsToCreateColliderOn[i].GetComponent<MeshFilter>();
                    b.Encapsulate(filter.sharedMesh.bounds);
                    b.center = new Vector3(0, 0.8f, 1.5f);
                }
                col.size = b.size;
                col.center = b.center;
            }
        }
        public void CombineMeshes()
        {
            Mat.Clear();

            RootObjectOfAllMeshesToCombine.AddComponent<MeshFilter>();

            RootObjectOfAllMeshesToCombine.AddComponent<MeshRenderer>();


            Vector3 pos = RootObjectOfAllMeshesToCombine.transform.position;
            RootObjectOfAllMeshesToCombine.transform.position = Vector3.zero;

            MeshFilter[] meshFilters = RootObjectOfAllMeshesToCombine.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            for (int x = 0; x < meshFilters.Length; x++)
            {
                if (!Mat.Contains(meshFilters[x].gameObject.GetComponent<MeshRenderer>().sharedMaterial) && meshFilters[x] != RootObjectOfAllMeshesToCombine.gameObject.GetComponent<MeshFilter>())
                {
                    Mat.Add(meshFilters[x].gameObject.GetComponent<MeshRenderer>().sharedMaterial);
                }
            }

            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
                i++;
            }
            RootObjectOfAllMeshesToCombine.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            RootObjectOfAllMeshesToCombine.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine, true, true);
            RootObjectOfAllMeshesToCombine.GetComponent<MeshRenderer>().materials = Mat.ToArray();
            RootObjectOfAllMeshesToCombine.gameObject.SetActive(true);

            RootObjectOfAllMeshesToCombine.transform.position = pos;
        }
    }
}