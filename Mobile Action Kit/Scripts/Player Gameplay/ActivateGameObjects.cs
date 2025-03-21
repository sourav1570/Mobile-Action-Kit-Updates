using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateGameObjects : MonoBehaviour
{
    public GameObject[] GameObjectsToActivate;

    public GameObject[] GameObjectsToDeactivate;

    public void ActivateGameObjects_Function()
    {
        for(int x = 0;x < GameObjectsToActivate.Length; x++)
        {
            GameObjectsToActivate[x].SetActive(true);
        }
        for (int x = 0; x < GameObjectsToDeactivate.Length; x++)
        {
            GameObjectsToDeactivate[x].SetActive(false);
        }
    }

}
