using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class TimeBasedSwap : MonoBehaviour
    {
        [TextArea]
        [Tooltip("This script can be used in many different situations to swap any number of game objects with any number of other game objects. ")]
        public string ScriptInfo = "This script can be used in many different situations to swap any number of game objects with any number of other game objects. ";

        [Tooltip("The time it will take before activating and deactivating the objects assigned.-- The time after which the swap will take place.")]
        public float Timer = 3f;
        public GameObject[] ObjectsToActivate;
        public GameObject[] ObjectsToDeactivate;


        void OnEnable()
        {
            StartCoroutine(Activation());
        }
        IEnumerator Activation()
        {
            yield return new WaitForSeconds(Timer);
            for (int x = 0; x < ObjectsToActivate.Length; x++)
            {
                ObjectsToActivate[x].SetActive(true);
            }
            for (int y = 0; y < ObjectsToDeactivate.Length; y++)
            {
                ObjectsToDeactivate[y].SetActive(false);
            }
        }
    }
}