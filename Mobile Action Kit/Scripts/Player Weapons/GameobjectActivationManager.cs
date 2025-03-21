using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class GameobjectActivationManager : MonoBehaviour
    {

        [TextArea]
        public string ScriptInfo = "This Script Only Activates and Deactivates Objects Added Below On Enable";
        

        [Space(10)]
        [Tooltip("Array of GameObjects that will be deactivated when this script is enabled.")]
        public GameObject[] GameObjectsToDeactivate;

        [Tooltip("Array of GameObjects that will be activated when this script is enabled.")]
        public GameObject[] GameObjectsToActivate;

        void OnEnable()
        {
            StartCoroutine(Coro());
        }
        IEnumerator Coro()
        {
            yield return new WaitForSeconds(0.0001f);
            for (int x = 0; x < GameObjectsToDeactivate.Length; x++)
            {
                GameObjectsToDeactivate[x].SetActive(false);
            }
            for (int x = 0; x < GameObjectsToActivate.Length; x++)
            {
                GameObjectsToActivate[x].SetActive(true);
            }
        }

    }
}
