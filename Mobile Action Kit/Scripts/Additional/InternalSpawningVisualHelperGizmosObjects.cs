using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class InternalSpawningVisualHelperGizmosObjects : MonoBehaviour
    {
        public static InternalSpawningVisualHelperGizmosObjects instance;

        public GameObject ObjToInstantiate;

        GameObject Go;

        //bool Once = false;

        private void Awake()
        {
            instance = this;
        }
        //private void Start()
        //{
        //    Spawning(transform.position, "Obj");
        //}
        public void Spawning(Vector3 Position, string RootName)
        {
            //if(Once == false)
            //{
                GameObject Go = Instantiate(ObjToInstantiate, Position, Quaternion.identity);
                Go.name = RootName;
            //    Once = true;
            //}
          
        }
    }
}