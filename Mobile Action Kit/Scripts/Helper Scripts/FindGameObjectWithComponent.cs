using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MobileActionKit
{
    public class FindGameObjectWithComponent : MonoBehaviour
    {
        public GameObject ObjToFind;

        private void Start()
        {
            ObjToFind = FindObjectOfType<AiLeaderHaltPoint>().gameObject;
            Debug.Log(ObjToFind.transform.name);
        }
    }
}
