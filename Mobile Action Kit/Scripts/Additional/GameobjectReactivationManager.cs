using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class GameobjectReactivationManager : MonoBehaviour
    {
        [TextArea]
         public string ScriptInfo = "This Script Only Activates GameObjects Added Below On Disable ";
        [Space(10)]

        private bool quitting = false;

        [Header("Activate On Disable")]
        public GameObject[] GameObjectsToReactiveOnDisable;
   
        void OnApplicationQuit()
        {
            quitting = true;
        }
        void OnDisable()
        {
            if (!quitting)
            {
                for (int x = 0; x < GameObjectsToReactiveOnDisable.Length; x++)
                {
                    if(GameObjectsToReactiveOnDisable[x] != null)
                    {
                        GameObjectsToReactiveOnDisable[x].SetActive(true);
                    }
                }
            }
        }
    }
}